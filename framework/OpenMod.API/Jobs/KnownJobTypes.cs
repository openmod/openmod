namespace OpenMod.API.Jobs
{
    public static class KnownJobTypes
    {
        //Do not append Default to jobTypes
        public const string Default = Repeat;

        public const string Event = "@Event";
        public const string Reboot = "@Reboot";
        public const string Repeat = "@Repeat";
        public const string SingleExec = "@Single_Exec";
        public const string Startup = "@Startup";
        public static readonly string[] JobTypes = { Event, Reboot, Repeat, SingleExec, Startup };
    }
}