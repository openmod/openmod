using System;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Cooldowns
{
    [Serializable]
    public sealed class CooldownSpan
    {
        public string? Command { get; set; }

        public string? Cooldown { get; set; }

        public TimeSpan ToTimeSpan() => TimeSpanHelper.Parse(Cooldown!);
    }
}