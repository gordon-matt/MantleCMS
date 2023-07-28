namespace Mantle.Data.Entity;

public interface IDbSeeder
{
    void Seed(DbContext context);

    int Order { get; }
}