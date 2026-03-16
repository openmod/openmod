using System.Collections.Generic;
using OpenMod.API.Jobs;
using VYaml.Annotations;

namespace OpenMod.Core.Jobs
{
    [YamlObject]
    public partial class ScheduledJobsFile
    {
        public List<ScheduledJob>? Jobs { get; set; } = [];
    }
}