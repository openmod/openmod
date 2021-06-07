using System;
using OpenMod.API.Permissions;

namespace OpenMod.API.Users
{
    [Serializable]
    public class BanData
    {
        public DateTime? ExpireDate { get; set; }

        public string? InstigatorType { get; set; }

        public string? InstigatorId { get; set; }

        public string? Reason { get; set; }

        public BanData()
        {

        }

        public BanData(string reason, IPermissionActor? instigator = null, DateTime? expireDate = null)
        {
            ExpireDate = expireDate ?? DateTime.MaxValue;

            InstigatorType = instigator?.Type;
            InstigatorId = instigator?.Id;

            Reason = reason;
        }
    }
}
