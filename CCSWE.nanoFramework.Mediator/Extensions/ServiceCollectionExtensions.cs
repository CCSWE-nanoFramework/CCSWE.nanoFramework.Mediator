using nanoFramework.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace CCSWE.nanoFramework.Mediator
{
    public static class ServiceCollectionExtensions
    {
        public delegate void ConfigureAsyncMediatorOptions(AsyncMediatorOptions options);

        public static IServiceCollection AddMediator(this IServiceCollection services) => services.AddMediator(_ => { });

        public static IServiceCollection AddMediator(this IServiceCollection services, ConfigureAsyncMediatorOptions configureOptions)
        {
            var options = new AsyncMediatorOptions();
            configureOptions(options);

            services.AddSingleton(typeof(IMediator), typeof(AsyncMediator));
            services.AddSingleton(typeof(AsyncMediatorOptions), options);

            return services;
        }
    }
}
