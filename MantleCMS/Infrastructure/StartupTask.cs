using Mantle.Data.Services;

namespace MantleCMS.Infrastructure;

public class StartupTask : IStartupTask
{
    public int Order => 0;

    public void Execute()
    {
        var contextFactory = DependoResolver.Instance.Resolve<IDbContextFactory>();
        using var context = contextFactory.GetContext() as ApplicationDbContext;
        var efHelper = DependoResolver.Instance.Resolve<IMantleEntityFrameworkHelper>();
        efHelper.EnsureTables(context);
    }
}