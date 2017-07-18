using Microsoft.EntityFrameworkCore;

namespace Mantle.Data.Entity.EntityFramework
{
    public interface IEntityTypeConfiguration
    {
        void Configure(ModelBuilder modelBuilder);

        bool IsEnabled { get; }
    }
}