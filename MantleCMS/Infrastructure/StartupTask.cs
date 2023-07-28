using Mantle.Data.Services;

namespace MantleCMS.Infrastructure;

public class StartupTask : IStartupTask
{
    public int Order => 0;

    public void Execute()
    {
        var contextFactory = EngineContext.Current.Resolve<IDbContextFactory>();
        using var context = contextFactory.GetContext() as ApplicationDbContext;
        var efHelper = EngineContext.Current.Resolve<IMantleEntityFrameworkHelper>();
        efHelper.EnsureTables(context);
    }
}