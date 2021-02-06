using System;
using System.Collections.Generic;
using OpenMod.API.Users;

namespace OpenMod.Core.Users
{
    [Serializable]
    public sealed class UsersData
    {
        public List<UserData>? Users { get; set; }
    }
}