using System;

using OpenMod.API.Permissions;

namespace OpenMod.Core.Permissions.Events
{
    public class PermissionRoleDataUpdatedEvent : PermissionRoleEvent
    {
        public string Key { get; }

        public object? Data { get; }

        public Type DataType { get; }

        public PermissionRoleDataUpdatedEvent(IPermissionRole permissionRole, string key, object? data, Type dataType) : base(permissionRole) {
            Key = key;
            Data = data;
            DataType = dataType;
        }
    }
}