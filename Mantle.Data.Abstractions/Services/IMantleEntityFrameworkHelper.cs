namespace Mantle.Data.Services;

public interface IMantleEntityFrameworkHelper
{
    void EnsureTables<TContext>(TContext context) where TContext : DbContext;
}