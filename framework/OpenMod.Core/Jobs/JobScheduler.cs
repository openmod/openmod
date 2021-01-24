using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Cronos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly List<IJobExecutor> m_JobExecutors;
        private readonly List<ScheduledJob> m_ScheduledJobs;
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

            m_JobExecutors = new List<IJobExecutor>();
            foreach (var provider in options.Value.JobExecutorTypes)
            {
                m_JobExecutors.Add((IJobExecutor)ActivatorUtilitiesEx.CreateInstance(runtime.LifetimeScope, provider));
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
                await ScheduleJobAsync(job, execStartup: !isFromFileChange);
            }

            m_Started = true;
        }

        public async Task AddJobAsync(ScheduledJob job)
        {
            m_File.Jobs.Add(job);
            await WriteJobsFileAsync();
            await ScheduleJobAsync(job, execStartup: false);
        }

        public async Task RemoveJobAsync(ScheduledJob job)
        {
            bool MatchedJobs(ScheduledJob d) => d.Name.Equals(job.Name, StringComparison.Ordinal);

            m_ScheduledJobs.RemoveAll(MatchedJobs);
            m_File.Jobs.RemoveAll(MatchedJobs);
            job.Enabled = false;
            await WriteJobsFileAsync();
        }

        public Task<ICollection<ScheduledJob>> GetScheduledJobsAsync()
        {
            return Task.FromResult<ICollection<ScheduledJob>>(m_File.Jobs);
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
            }
        }

        private async Task WriteJobsFileAsync()
        {
            await m_DataStore.SaveAsync(c_DataStoreKey, m_File);
        }

        private async Task ScheduleJobAsync(ScheduledJob job, bool execStartup)
        {
            if (!job.Enabled)
            {
                return;
            }

            // todo: fix this mess; move to their own classes
            if (job.Schedule.Equals("single_exec", StringComparison.OrdinalIgnoreCase))
            {
                await ExecuteJobAsync(job);
                await RemoveJobAsync(job);

                return;
            }

            if (job.Schedule.Equals("startup", StringComparison.OrdinalIgnoreCase))
            {
                if (execStartup)
                {
                    await ExecuteJobAsync(job);
                }

                return;
            }

            if (job.Schedule.Equals("period", StringComparison.OrdinalIgnoreCase))
            {
                if (!job.Args.ContainsKey("period"))
                {
                    m_Logger.LogError($"Job \"{job.Name}\" uses crontab for scheduling but args.period is not set!");
                    return;
                }

                ScheduleCronJob(job);
            }
        }

        private void ScheduleCronJob(ScheduledJob job)
        {
            var period = (string)job.Args["period"];

            var timezone = TimeZoneInfo.Local;
            var expression = CronExpression.Parse(period);

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
                await jobExecutor.ExecuteAsync(job);
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