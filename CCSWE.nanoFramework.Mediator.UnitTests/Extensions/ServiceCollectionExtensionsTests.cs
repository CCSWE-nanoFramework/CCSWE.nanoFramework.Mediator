using CCSWE.nanoFramework.Mediator.UnitTests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using nanoFramework.TestFramework;

namespace CCSWE.nanoFramework.Mediator.UnitTests.Extensions
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void AddMediator_should_configure_AsyncMediatorOptions()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddMediator(options =>
            {
                options.AddSubscriber(typeof(MediatorEventMock), typeof(IMediatorEventHandlerMock));
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var result = serviceProvider.GetService(typeof(AsyncMediatorOptions));

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(AsyncMediatorOptions));

            var options = (AsyncMediatorOptions) result;

            Assert.AreEqual(1, options.Subscribers.Count);
        }

        [TestMethod]
        public void AddMediator_should_register_AsyncMediator()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(typeof(ILogger), typeof(LoggerMock));

            // Act
            serviceCollection.AddMediator();

            var serviceProvider = serviceCollection.BuildServiceProvider();
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
        public void AddMediator_should_register_AsyncMediatorOptions()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddMediator();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var result1 = serviceProvider.GetService(typeof(AsyncMediatorOptions));
            var result2 = serviceProvider.GetService(typeof(AsyncMediatorOptions));

            // Assert
            Assert.IsNotNull(result1);
            Assert.IsInstanceOfType(result1, typeof(AsyncMediatorOptions));
            Assert.AreEqual(result1, result2);
        }
    }
}
