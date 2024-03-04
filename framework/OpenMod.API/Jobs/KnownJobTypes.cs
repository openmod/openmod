namespace OpenMod.API.Jobs
{
    public static class KnownJobTypes
    {
        //Do not append Default to jobTypes
        public const string Default = Repeat;

        public const string Event = "@event";
        public const string Reboot = "@reboot";
        public const string Repeat = "@repeat";
        public const string SingleExec = "@single_Exec";
        public const string Startup = "@startup";
        public static readonly string[] JobTypes = { Event, Reboot, Repeat, SingleExec, Startup };
    }
}