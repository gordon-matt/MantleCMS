﻿namespace Mantle.Security.Membership.Permissions;

/// <summary>
/// Implemented by modules to enumerate the types of permissions the which may be granted
/// </summary>
public interface IPermissionProvider
{
    IEnumerable<Permission> GetPermissions();
}