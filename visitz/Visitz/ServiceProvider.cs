#nullable enable

namespace Visitz
{
    /// <summary>
    /// Alternate service for Dependency Injection. Use this when there is a need to bypass constructor injection.
    /// </summary>
    public class ServiceProvider
    {
        public static TService GetService<TService>() =>
            Current.GetService<TService>()
            ?? throw new InvalidOperationException($"Type '{typeof(TService)}' not registered or unavailable");

        public static object GetService(Type serviceType) =>
            Current.GetService(serviceType)
            ?? throw new InvalidOperationException(
                $"Type '{serviceType.GetType().Name}' not registered or unavailable"
            );

        public static IServiceProvider Current =>
            IPlatformApplication.Current?.Services ?? throw new InvalidOperationException("Services unavailable");
    }
}
