using Extenso.Data.Entity;
using Mantle.Data.Services;
using Mantle.Infrastructure;
using MantleCMS.Data;

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