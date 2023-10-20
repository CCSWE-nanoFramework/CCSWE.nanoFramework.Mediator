using nanoFramework.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace CCSWE.nanoFramework.Mediator
{
    /// <summary>
    /// Extension methods for <see cref="AsyncMediator"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// An action for configuring the <see cref="AsyncMediator"/>.
        /// </summary>
        /// <param name="options"></param>
        public delegate void ConfigureAsyncMediatorOptions(AsyncMediatorOptions options);

        /// <summary>
        /// Adds an <see cref="AsyncMediator"/> with default <see cref="AsyncMediatorOptions"/>.
        /// </summary>
        public static IServiceCollection AddMediator(this IServiceCollection services) => services.AddMediator(_ => { });

        /// <summary>
        /// Adds an <see cref="AsyncMediator"/> with the specified <see cref="AsyncMediatorOptions"/>.
        /// </summary>
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
