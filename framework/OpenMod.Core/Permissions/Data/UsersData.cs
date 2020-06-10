using System;
using System.Collections.Generic;

namespace OpenMod.Core.Permissions.Data
{
    [Serializable]
    public class UsersData
    {
        public List<UserData> Users { get; set; }
    }
}