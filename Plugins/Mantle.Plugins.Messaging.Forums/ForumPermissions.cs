namespace Mantle.Plugins.Messaging.Forums;

public class ForumPermissions : IPermissionProvider
{
    public static readonly Permission ReadForums = new() { Name = "Plugin_Forums_ReadForums", Category = "Plugin - Forums", Description = "Plugin: Forums - Read Forums" };
    public static readonly Permission WriteForums = new() { Name = "Plugin_Forums_WriteForums", Category = "Plugin - Forums", Description = "Plugin: Forums - Write Forums" };

    #region IPermissionProvider Members

    public IEnumerable<Permission> GetPermissions() =>
    [
        ReadForums,
        WriteForums
    ];

    #endregion IPermissionProvider Members
}