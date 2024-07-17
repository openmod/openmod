using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Cronos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MoreLinq;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.API.Jobs;
using OpenMod.API.Persistence;
using OpenMod.Core.Helpers;
using OpenMod.Core.Ioc;

namespace OpenMod.Core.Jobs
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    public class JobScheduler : IJobScheduler, IDisposable
    {
        private const string c_DataStoreKey = "autoexec";
        private const string c_JobDelimiter = ":";

        private static bool s_IsFirstStart = true;

        private readonly IDataStore m_DataStore;
        private readonly IEventBus m_EventBus;
        private readonly ILogger<JobScheduler> m_Logger;
        private readonly IOpenModComponent m_OpenModComponent;

        private readonly List<ITaskExecutor> m_JobExecutors = [];
        private readonly ConcurrentDictionary<string, ScheduledJob> m_ScheduledJobs = [];

        private IDisposable? m_FileChangeDisposable;
        private readonly ScheduledJobsFile m_FileData = new();
        private bool m_Started;


        public JobScheduler(IRuntime runtime)
        {
            m_EventBus = runtime.LifetimeScope.Resolve<IEventBus>();
            m_Logger = runtime.LifetimeScope.Resolve<ILogger<JobScheduler>>();
            m_OpenModComponent = runtime;

            var dataStoreFactory = runtime.LifetimeScope.Resolve<IDataStoreFactory>();
            var dataStoreParameters = new DataStoreCreationParameters
            {
                Component = runtime,
                LogOnChange = true,
                WorkingDirectory = runtime.WorkingDirectory
            };
            m_DataStore = dataStoreFactory.CreateDataStore(dataStoreParameters);

            AsyncHelper.RunSync(ReadJobsFileAsync);
            WatchFileChanges();

            var options = runtime.LifetimeScope.Resolve<IOptions<JobExecutorOptions>>();
            foreach (var provider in options.Value.JobExecutorTypes)
            {
                var jobExecutor = ActivatorUtilitiesEx.CreateInstance(runtime.LifetimeScope, provider);
                m_JobExecutors.Add((ITaskExecutor)jobExecutor);
            }
        }

        public void Dispose()
        {
            m_FileChangeDisposable?.Dispose();

            foreach (var jobName in m_ScheduledJobs.Keys)
            {
                DisposeCancellableJob(jobName);
            }

            m_JobExecutors.Clear();
        }


        private async Task ReadJobsFileAsync()
        {
            if (!await m_DataStore.ExistsAsync(c_DataStoreKey))
            {
                return;
            }

            var fileData = await m_DataStore.LoadAsync<ScheduledJobsFile>(c_DataStoreKey) ?? throw new InvalidOperationException("Failed to load jobs from autoexec.yaml!");
            m_FileData.Jobs = fileData.Jobs ??= [];

            var oldFileLen = m_FileData.Jobs.Count;
            m_FileData.Jobs = m_FileData.Jobs.DistinctBy(d => d.Name, StringComparer.OrdinalIgnoreCase).ToList();

            if (oldFileLen ==  m_FileData.Jobs.Count)
            {
                return;
            }

            await WriteJobsFileAsync();
        }

        private async Task WriteJobsFileAsync()
        {
            m_FileData.Jobs ??= [];

            //Do not reload file when internal save
            m_FileChangeDisposable?.Dispose();
            await m_DataStore.SaveAsync(c_DataStoreKey, m_FileData);
            WatchFileChanges();
        }

        private void WatchFileChanges()
        {
            m_FileChangeDisposable?.Dispose();
            m_FileChangeDisposable = m_DataStore.AddChangeWatcher(c_DataStoreKey, m_OpenModComponent, OnJobsFileChange);
        }

        private void OnJobsFileChange()
        {
            foreach (var jobName in m_ScheduledJobs.Keys)
            {
                DisposeCancellableJob(jobName);
            }

            AsyncHelper.RunSync(() => StartAsync(isFromFileChange: true));
        }


        public Task StartAsync()
        {
            return StartAsync(isFromFileChange: false);
        }

        private async Task StartAsync(bool isFromFileChange)
        {
            if (m_Started && !isFromFileChange)
            {
                return;
            }

            foreach (var job in m_FileData.Jobs!.ToList())
            {
                await ScheduleJobInternalAsync(job, !isFromFileChange);
            }

            s_IsFirstStart = false;
            m_Started = true;
        }

        private async Task ScheduleJobInternalAsync(ScheduledJob? job, bool isFromStart)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            var enable = job.Enabled ?? true;
            if (!enable)
            {
                return;
            }

            if (string.IsNullOrEmpty(job.Name))
            {
                m_Logger.LogError("Job of type {TaskType} has no name set", job.Task);
                return;
            }

            if (string.IsNullOrEmpty(job.Task))
            {
                m_Logger.LogError("Job \"{JobName}\" has no task set", job.Name);
                return;
            }

            if (string.IsNullOrEmpty(job.Schedule))
            {
                m_Logger.LogError("Job \"{JobName}\" has no schedule set", job.Name);
                return;
            }

            if (job.Schedule!.StartsWith(KnownJobTypes.SingleExec, StringComparison.OrdinalIgnoreCase))
            {
                await DelayedOrExecuteJob(job, removeAfterExec: true);
                return;
            }

            if (job.Schedule.StartsWith(KnownJobTypes.Reboot, StringComparison.OrdinalIgnoreCase))
            {
                if (s_IsFirstStart)
                {
                    await DelayedOrExecuteJob(job);
                }

                return;
            }

            if (job.Schedule.StartsWith(KnownJobTypes.Startup, StringComparison.OrdinalIgnoreCase))
            {
                if (isFromStart)
                {
                    await DelayedOrExecuteJob(job);
                }

                return;
            }

            if (job.Schedule.StartsWith(KnownJobTypes.Event, StringComparison.OrdinalIgnoreCase))
            {
                SubscribeEventJob(job);
                return;
            }

            /*if (job.Schedule.StartsWith(KnownJobTypes.Repeat, StringComparison.OrdinalIgnoreCase))
            {
                await DelayedOrExecuteJob(job, shouldRepeat: true);
                return;
            }*/

            await DelayedOrExecuteJob(job, shouldRepeat: true);
        }

        private async Task DelayedOrExecuteJob(ScheduledJob job, bool removeAfterExec = false, bool shouldRepeat = false)
        {
            //If shouldRepeat schedule => time or type:time
            //else schedule => type or type:time
            var schedule = RetrieveSchedulerValue(job.Schedule!, !shouldRepeat); //!shouldRepeat
            if (string.IsNullOrEmpty(schedule))
            {
                if (!shouldRepeat)
                {
                    if (await ExecuteJobAsync(job) && removeAfterExec)
                    {
                        await RemoveJobAsync(job);
                    }

                    return;
                }

                m_Logger.LogError("Job \"{JobName}\" has no valid schedule", job.Name);
                return;
            }

            var delay = GetJobDelay(job.Name!, schedule!, !shouldRepeat);
            if (delay == null || delay.Value.TotalMilliseconds is < 0 or > int.MaxValue)
            {
                return;
            }

            AddCancellableJob(job);

            if (shouldRepeat)
            {
                m_Logger.LogInformation("Scheduling job \"{JobName}\" with schedule \"{JobSchedule}\"", job.Name,
                    job.Schedule);
            }
            else
            {
                m_Logger.LogInformation("Delaying job \"{JobName}\" with delay of \"{JobDelay}\"", job.Name,
                    $"{delay:c}");
            }

            AsyncHelper.Schedule($"Execution of job \"{job.Name}\"", async () =>
            {
                try
                {
                    await StartDelayedJob(job, delay.Value, schedule, shouldRepeat);
                }
                catch (TaskCanceledException)
                {
                    // ignore it
                }

                if (removeAfterExec)
                {
                    await RemoveJobAsync(job);
                }
                else
                {
                    DisposeCancellableJob(job.Name!);
                }
            });
        }

        /// <summary>
        ///     Separates schedule value from its original form type:time
        ///     If shouldBePrefixed means it returns null when fail to separate
        ///     else return schedule
        /// </summary>
        private string? RetrieveSchedulerValue(string schedule, bool shouldBePrefixed)
        {
            var delimiterIndex = schedule.IndexOf(c_JobDelimiter, StringComparison.Ordinal);
            if (delimiterIndex != -1)
            {
                return schedule[(delimiterIndex + 1)..];
            }

            return shouldBePrefixed ? null : schedule;
        }

        private async Task<bool> ExecuteJobAsync(ScheduledJob job, params object[] parameters)
        {
            var jobExecutor = m_JobExecutors.FirstOrDefault(d => d.SupportsType(job.Task!));
            if (jobExecutor == null)
            {
                m_Logger.LogError("[{JobName}] Unknown job type: {TaskType}", job.Name, job.Task);
                return false;
            }

            m_Logger.LogInformation("Executing job \"{JobName}\"...", job.Name);
            try
            {
                var jobTask = new JobTask(job.Name!, job.Task!, job.Args ?? [], parameters);
                await jobExecutor.ExecuteAsync(jobTask);
                return true;
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, "Job \"{JobName}\" generated an exception", job.Name);
                return false;
            }
        }

        public async Task<bool> RemoveJobAsync(ScheduledJob job)
        {
            DisposeCancellableJob(job.Name!);
            
            var foundInFile = m_FileData.Jobs!.Remove(job);
            if (foundInFile)
            {
                await WriteJobsFileAsync();
            }

            return foundInFile;
        }

        private TimeSpan? GetJobDelay(string jobName, string jobSchedule, bool onlyTimespan)
        {
            Exception? cronEx = null;

            if (!onlyTimespan)
            {
                try
                {
                    var expression = CronExpression.Parse(jobSchedule);
                    var nextOccurence = expression.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo.Local);
                    if (nextOccurence == null)
                    {
                        m_Logger.LogError("Fail to parse Job \"{JobName}\" schedule \"{JobSchedule}\". The value is not a valid crontab", jobName, jobSchedule);
                        return null;
                    }

                    return nextOccurence.Value - DateTimeOffset.Now;
                }
                catch (Exception ex)
                {
                    cronEx = ex;
                }
            }

            try
            {
                return TimeSpanHelper.Parse(jobSchedule);
            }
            catch (Exception ex)
            {
                if (cronEx != null)
                {
                    m_Logger.LogError(
                        "Fail to parse Job \"{JobName}\" schedule \"{JobSchedule}\". The value is not a valid crontab or time span",
                        jobName, jobSchedule);
                    m_Logger.LogError(cronEx, "Crontab error");
                }
                else
                {
                    m_Logger.LogError(
                        "Fail to parse Job \"{JobName}\" schedule \"{JobSchedule}\". The value is not a valid time span",
                        jobName, jobSchedule);
                }

                m_Logger.LogError(ex, "Time span error");
                return null;
            }
        }

        private void AddCancellableJob(ScheduledJob job)
        {
            lock (job)
            {
                job.CancellationTokenSource?.Cancel();
                job.CancellationTokenSource?.Dispose();
                job.CancellationTokenSource = null;

                job.CancellationTokenSource = new CancellationTokenSource();
            }

            m_ScheduledJobs.TryAdd(job.Name!, job);
        }

        private async Task StartDelayedJob(ScheduledJob job, TimeSpan? delay, string schedule, bool shouldRepeat)
        {
            var token = job.CancellationTokenSource?.Token ?? CancellationToken.None;

            bool Enabled()
            {
                return (job.Enabled ?? true) && m_OpenModComponent.IsComponentAlive && !token.IsCancellationRequested;
            }

            do
            {
                await Task.Delay(delay!.Value, token);
                if (!Enabled())
                {
                    break;
                }

                await ExecuteJobAsync(job);
                if (!shouldRepeat)
                {
                    break;
                }

                //If repeat it means time can be time span or cron
                delay = GetJobDelay(job.Name!, schedule, !shouldRepeat);
                if (delay!.Value.TotalMilliseconds is < 0 or > int.MaxValue)
                {
                    break;
                }
            } while (Enabled());
        }

        private void DisposeCancellableJob(string jobName)
        {
            if (!m_ScheduledJobs.TryRemove(jobName!, out var job))
            {
                return;
            }

            lock (job)
            {
                job.CancellationTokenSource?.Cancel();
                job.CancellationTokenSource?.Dispose();
                job.CancellationTokenSource = null;
            }
        }

        private void SubscribeEventJob(ScheduledJob job)
        {
            var schedule = RetrieveSchedulerValue(job.Schedule!, true);
            if (string.IsNullOrEmpty(schedule))
            {
                m_Logger.LogError("Invalid event job format \"{JobSchedule}\" for \"{JobName}\" job", job.Schedule,
                    job.Name);
                return;
            }

            m_Logger.LogInformation("Subscribed job \"{JobName}\" to \"{JobSchedule}\" event", job.Name, schedule);
            var dispose = m_EventBus.Subscribe(m_OpenModComponent, schedule!, async (_, _, @event) =>
            {
                var token = job.CancellationTokenSource?.Token ?? CancellationToken.None;
                var enable = (job.Enabled ?? true) && m_OpenModComponent.IsComponentAlive && !token.IsCancellationRequested;
                if (!enable)
                {
                    return;
                }

                await ExecuteJobAsync(job, new { Event = @event });
            });

            AddCancellableJob(job);
            job.CancellationTokenSource!.Token.Register(dispose.Dispose);
        }

        public Task<ScheduledJob?> FindJobAsync(string name)
        {
            return Task.FromResult(m_FileData.Jobs?.FirstOrDefault(d =>
                d.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        public Task<ICollection<ScheduledJob>> GetScheduledJobsAsync(bool includeDisabled = false)
        {
            if (m_FileData.Jobs == null)
            {
                return Task.FromResult<ICollection<ScheduledJob>>([]);
            }

            return Task.FromResult<ICollection<ScheduledJob>>(m_FileData.Jobs
                .Where(d => includeDisabled || (d.Enabled ?? true))
                .ToList());
        }

        public async Task<bool> RemoveJobAsync(string name)
        {
            var job = await FindJobAsync(name);
            if (job == null)
            {
                return false;
            }

            return await RemoveJobAsync(job);
        }

        public async Task<ScheduledJob> ScheduleJobAsync(JobCreationParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            m_FileData.Jobs ??= [];
            if (m_FileData.Jobs.Any(d => d.Name?.Equals(parameters.Name, StringComparison.OrdinalIgnoreCase) ?? false))
            {
                throw new Exception($"A job with this name already exists: {parameters.Name}");
            }

            var job = new ScheduledJob
            {
                Name = parameters.Name ?? throw new ArgumentException(nameof(parameters)),
                Task = parameters.Task ?? throw new ArgumentException(nameof(parameters)),
                Args = parameters.Args ?? throw new ArgumentException(nameof(parameters)),
                Schedule = parameters.Schedule ?? throw new ArgumentException(nameof(parameters)),
                Enabled = true
            };
            m_FileData.Jobs.Add(job);

            await WriteJobsFileAsync();
            await ScheduleJobInternalAsync(job, isFromStart: false);
            return job;
        }
    }
}