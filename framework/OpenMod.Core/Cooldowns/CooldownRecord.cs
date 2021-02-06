using System;

namespace OpenMod.Core.Cooldowns
{
    [Serializable]
    public sealed class CooldownRecord
    {
        public string? Command { get; set; }

        public DateTime? Executed { get; set; }

        public CooldownRecord()
        {
            Command = "";
            Executed = DateTime.MinValue;
        }

        public CooldownRecord(string command, DateTime executed)
        {
            Command = command;
            Executed = executed;
        }
    }
}