using NLog;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace MantleCMS;

public class NLogStartup
{
    public static void ConfigureNLog(IServiceProvider serviceProvider)
    {
        try
        {
            using var target = LogManager.Configuration.FindTargetByName("database");
            if (target == null) return;

            var databaseTarget = target switch
            {
                DatabaseTarget directTarget => directTarget,
                WrapperTargetBase { WrappedTarget: DatabaseTarget wrappedTarget } => wrappedTarget,
                _ => null
            };

            if (databaseTarget != null)
            {
                //databaseTarget.DBProvider = "Npgsql"; // For non-SQLServer providers
                var dataSettings = serviceProvider.GetRequiredService<DataSettings>();
                databaseTarget.ConnectionString = dataSettings.ConnectionString;
                LogManager.ReconfigExistingLoggers();
            }
        }
        catch (Exception ex)
        {
            LogManager.GetCurrentClassLogger().Error(ex, "Failed to configure NLog database target");
        }
    }
}