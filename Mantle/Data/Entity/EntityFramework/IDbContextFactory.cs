using Microsoft.EntityFrameworkCore;

namespace Mantle.Data.Entity.EntityFramework
{
    public interface IDbContextFactory
    {
        DbContext GetContext();
    }
}