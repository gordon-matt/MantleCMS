namespace MantleCMS.Infrastructure;

public class LazyServiceWrapper<T>(IServiceProvider serviceProvider)
    : Lazy<T>(() => serviceProvider.GetRequiredService<T>());