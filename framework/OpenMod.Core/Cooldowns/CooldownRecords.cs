using System;
using System.Collections.Generic;

namespace OpenMod.Core.Cooldowns
{
    [Serializable]
    public class CooldownRecords
    {
        public Dictionary<string, List<CooldownRecord>> Records { get; set; }

        public CooldownRecords()
        {
            Records = new Dictionary<string, List<CooldownRecord>>();
        }

        public CooldownRecords(Dictionary<string, List<CooldownRecord>> records)
        {
            Records = records;
        }
    }
}