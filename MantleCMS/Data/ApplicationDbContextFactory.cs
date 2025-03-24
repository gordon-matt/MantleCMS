using Microsoft.EntityFrameworkCore;

namespace MantleCMS.Data;

public class ApplicationDbContextFactory : IDbContextFactory
{
    private readonly IConfiguration configuration;

    public ApplicationDbContextFactory(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    private DbContextOptions<ApplicationDbContext> options;

    private DbContextOptions<ApplicationDbContext> Options
    {
        get
        {
            if (options == null)
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

                if (DataSettingsHelper.IsDatabaseInstalled)
                {
                    var dataSettings = EngineContext.Current.Resolve<DataSettings>();
                    optionsBuilder.UseSqlServer(dataSettings.ConnectionString);
                }
                else
                {
                    optionsBuilder.UseInMemoryDatabase("MantleCMS");
                }

                options = optionsBuilder.Options;
            }
            return options;
        }
    }

    public DbContext GetContext() => new ApplicationDbContext(Options);

    public DbContext GetContext(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(connectionString);
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}