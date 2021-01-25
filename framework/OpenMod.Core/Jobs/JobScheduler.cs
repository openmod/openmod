using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MoreLinq.Extensions;
using OpenMod.API;
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
        private readonly IRuntime m_Runtime;
        private readonly ILogger<JobScheduler> m_Logger;
        private readonly IDataStore m_DataStore;
        private readonly List<ITaskExecutor> m_JobExecutors;
        private readonly List<ScheduledJob> m_ScheduledJobs;
        private static bool s_RunRebootJobs = true;
        private ScheduledJobsFile m_File;
        private bool m_Started;

        public JobScheduler(
            IRuntime runtime,
            ILogger<JobScheduler> logger,
            IDataStoreFactory dataStoreFactory,
            IOptions<JobExecutorOptions> options)
        {
            m_Runtime = runtime;
            m_Logger = logger;

            m_DataStore = dataStoreFactory.CreateDataStore(new DataStoreCreationParameters
            {
                Component = runtime,
                LogOnChange = true,
                WorkingDirectory = runtime.WorkingDirectory
            });

            AsyncHelper.RunSync(ReadJobsFileAsync);

            m_ScheduledJobs = new List<ScheduledJob>();
            m_DataStore.AddChangeWatcher(c_DataStoreKey, runtime, () =>
            {
                m_ScheduledJobs.Clear();
                AsyncHelper.RunSync(() => StartAsync(isFromFileChange: true));
            });

            m_JobExecutors = new List<ITaskExecutor>();
            foreach (var provider in options.Value.JobExecutorTypes)
            {
                m_JobExecutors.Add((ITaskExecutor)ActivatorUtilitiesEx.CreateInstance(runtime.LifetimeScope, provider));
            }
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

            await ReadJobsFileAsync();

            foreach (var job in m_File.Jobs.ToList())
            {
                await ScheduleJobInternalAsync(job, execStartup: !isFromFileChange, execReboot: s_RunRebootJobs);
            }

            s_RunRebootJobs = false;
            m_Started = true;
        }

        public async Task<ScheduledJob> ScheduleJobAsync(JobCreationParameters @params)
        {
            if (m_File.Jobs.Any(d => d.Name.Equals(@params.Name)))
            {
                throw new Exception($"A job with this name already exists: {@params.Name}");
            }

            var job = new ScheduledJob
            {
                Name = @params.Name,
                Task = @params.Task,
                Args = @params.Args,
                Schedule = @params.Schedule,
                Enabled = true,
            };

            m_File.Jobs.Add(job);
            await WriteJobsFileAsync();
            await ScheduleJobInternalAsync(job, execStartup: false, execReboot: false);
            return job;
        }

        public Task<ScheduledJob> FindJobAsync(string name)
        {
            return Task.FromResult(m_File.Jobs.FirstOrDefault(d => d.Name.Equals(name)));
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

        public async Task<bool> RemoveJobAsync(ScheduledJob job)
        {
            bool MatchedJobs(ScheduledJob d) => d.Name.Equals(job.Name, StringComparison.Ordinal);

            m_ScheduledJobs.RemoveAll(MatchedJobs);
            var result = m_File.Jobs.RemoveAll(MatchedJobs) > 0;
            job.Enabled = false;

            if (result)
            {
                await WriteJobsFileAsync();
            }

            return result;
        }

        public Task<ICollection<ScheduledJob>> GetScheduledJobsAsync(bool includeDisabled)
        {
            return Task.FromResult<ICollection<ScheduledJob>>(m_File.Jobs
                .Where(d => includeDisabled || d.Enabled)
                .ToList());
        }

        private async Task ReadJobsFileAsync()
        {
            if (!await m_DataStore.ExistsAsync(c_DataStoreKey))
            {
                m_File = new ScheduledJobsFile();
            }
            else
            {
                m_File = await m_DataStore.LoadAsync<ScheduledJobsFile>(c_DataStoreKey);

                var previousCount = m_File.Jobs.Count;
                m_File.Jobs = m_File.Jobs.DistinctBy(d => d.Name).ToList();

                // Duplicate jobs removed; save
                if (m_File.Jobs.Count != previousCount)
                {
                    await WriteJobsFileAsync();
                }
            }
        }

        private async Task WriteJobsFileAsync()
        {
            await m_DataStore.SaveAsync(c_DataStoreKey, m_File);
        }

        private async Task ScheduleJobInternalAsync(ScheduledJob job, bool execStartup, bool execReboot)
        {
            if (!job.Enabled)
            {
                return;
            }

            if (job.Schedule.Equals("@single_exec", StringComparison.OrdinalIgnoreCase))
            {
                await ExecuteJobAsync(job);
                await RemoveJobAsync(job);

                return;
            }

            if (job.Schedule.Equals("@reboot", StringComparison.OrdinalIgnoreCase))
            {
                if (execReboot)
                {
                    await ExecuteJobAsync(job);
                }

                return;
            }

            if (job.Schedule.Equals("@startup", StringComparison.OrdinalIgnoreCase))
            {
                if (execStartup)
                {
                    await ExecuteJobAsync(job);
                }

                return;
            }

            ScheduleCronJob(job);
        }

        private void ScheduleCronJob(ScheduledJob job)
        {
            var timezone = TimeZoneInfo.Local;

            CronExpression expression;
            try
            {
                expression = CronExpression.Parse(job.Schedule);
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, $"Invalid crontab syntax \"{job.Schedule}\" for job: {job.Name}");
                return;
            }

            var nextOccurence = expression.GetNextOccurrence(DateTimeOffset.Now, timezone);
            if (nextOccurence == null)
            {
                return;
            }

            var delay = nextOccurence.Value - DateTimeOffset.Now;
            if (delay.TotalMilliseconds <= 0)
            {
                return;
            }

            m_ScheduledJobs.Add(job);

            AsyncHelper.Schedule($"Execution of job \"{job.Name}\"", async () =>
            {
                await Task.Delay(delay);

                if (!job.Enabled || !m_ScheduledJobs.Contains(job) || !m_Runtime.IsComponentAlive)
                {
                    return;
                }

                await ExecuteJobAsync(job);
                ScheduleCronJob(job);
            });
        }

        private async Task ExecuteJobAsync(ScheduledJob job)
        {
            var jobExecutor = m_JobExecutors.FirstOrDefault(d => d.SupportsType(job.Task));
            if (jobExecutor == null)
            {
                m_Logger.LogError($"[{job.Name}] Unknown job type: {job.Task}");
                return;
            }

            try
            {
                await jobExecutor.ExecuteAsync(new JobTask
                {
                    JobName = job.Name,
                    Task = job.Task,
                    Args = job.Args
                });
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, $"Job \"{job.Name}\" generated an exception.");
            }
        }

        public void Dispose()
        {
            m_ScheduledJobs?.Clear();
            m_JobExecutors.Clear();
        }
    }
}