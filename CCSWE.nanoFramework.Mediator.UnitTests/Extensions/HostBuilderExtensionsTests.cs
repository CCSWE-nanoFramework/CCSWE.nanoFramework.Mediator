using CCSWE.nanoFramework.Mediator.UnitTests.Mocks;
using Microsoft.Extensions.Logging;
using nanoFramework.Hosting;
using nanoFramework.TestFramework;

namespace CCSWE.nanoFramework.Mediator.UnitTests.Extensions
{
    [TestClass]
    public class HostBuilderExtensionsTests
    {
        [TestMethod]
        public void UseMediator_should_configure_AsyncMediatorOptions()
        {
            // Arrange
            var hostBuilder = new HostBuilder();

            // Act
            hostBuilder.UseMediator(options =>
            {
                options.AddSubscriber(typeof(MediatorEventMock), typeof(IMediatorEventHandlerMock));
            });

            var serviceProvider = hostBuilder.Build().Services;
            var result = serviceProvider.GetService(typeof(AsyncMediatorOptions));

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(AsyncMediatorOptions));

            var options = (AsyncMediatorOptions)result;

            Assert.AreEqual(1, options.Subscribers.Count);
        }

        [TestMethod]
        public void UseMediator_should_register_AsyncMediator()
        {
            // Arrange
            var hostBuilder = new HostBuilder();
            hostBuilder.ConfigureServices(services => services.AddSingleton(typeof(ILogger), typeof(LoggerMock)));

            // Act
            hostBuilder.UseMediator();

            var serviceProvider = hostBuilder.Build().Services;
            var result1 = serviceProvider.GetService(typeof(IMediator));
            var result2 = serviceProvider.GetService(typeof(IMediator));

            // Assert
            Assert.IsNotNull(result1);
            Assert.IsInstanceOfType(result1, typeof(AsyncMediator));
            Assert.AreEqual(result1, result2);

            var asyncMediator = (AsyncMediator)result1;
            asyncMediator.Dispose();
        }

        [TestMethod]
        public void UseMediator_should_register_AsyncMediatorOptions()
        {
            // Arrange
            var hostBuilder = new HostBuilder();

            // Act
            hostBuilder.UseMediator();

            var serviceProvider = hostBuilder.Build().Services;
            var result1 = serviceProvider.GetService(typeof(AsyncMediatorOptions));
            var result2 = serviceProvider.GetService(typeof(AsyncMediatorOptions));

            // Assert
            Assert.IsNotNull(result1);
            Assert.IsInstanceOfType(result1, typeof(AsyncMediatorOptions));
            Assert.AreEqual(result1, result2);
        }
    }
}
