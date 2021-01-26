using System.Collections.Generic;
using OpenMod.API.Jobs;

namespace OpenMod.Core.Jobs
{
    public class ScheduledJobsFile
    {
        public List<ScheduledJob>? Jobs { get; set; } = new();
    }
}