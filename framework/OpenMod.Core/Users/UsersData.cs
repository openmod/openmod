using System;
using System.Collections.Generic;
using OpenMod.API.Users;

namespace OpenMod.Core.Users
{
    [Serializable]
    public class UsersData
    {
        public List<UserData>? Users { get; set; }
    }
}