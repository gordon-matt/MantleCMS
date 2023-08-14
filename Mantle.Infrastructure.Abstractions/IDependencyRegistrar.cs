namespace Mantle.Infrastructure;

public interface IDependencyRegistrar
{
    void Register(ContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration);

    int Order { get; }
}