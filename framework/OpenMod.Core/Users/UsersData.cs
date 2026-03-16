using System;
using System.Collections.Generic;
using OpenMod.API.Users;
using VYaml.Annotations;

namespace OpenMod.Core.Users
{
    [Serializable]
    [YamlObject]
    public sealed partial class UsersData
    {
        public List<UserData>? Users { get; set; }
    }
}