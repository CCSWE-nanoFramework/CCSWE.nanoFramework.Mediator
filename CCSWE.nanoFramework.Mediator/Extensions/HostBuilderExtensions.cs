using nanoFramework.Hosting;

// ReSharper disable once CheckNamespace
namespace CCSWE.nanoFramework.Mediator
{
    /// <summary>
    /// Extension methods for <see cref="AsyncMediator"/>.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Adds an <see cref="AsyncMediator"/> with the specified <see cref="AsyncMediatorOptions"/>.
        /// </summary>
        public static IHostBuilder UseMediator(this IHostBuilder builder, ConfigureAsyncMediatorOptions? configureOptions = null)
        {
            builder.ConfigureServices(services => services.AddMediator(configureOptions));

            return builder;
        }
    }
}
