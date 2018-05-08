using Microsoft.EntityFrameworkCore;

namespace Mantle.Data.Entity
{
    public interface IDbContextFactory
    {
        DbContext GetContext();
    }
}