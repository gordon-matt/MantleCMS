using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MantleCMS.Data;

// This is just used for EF Migrations (Not for Production)
public class DesignTimeApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer("Server=.;Initial Catalog=MantleCMS;User=sa;Password=Admin@123;TrustServerCertificate=True;");
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}