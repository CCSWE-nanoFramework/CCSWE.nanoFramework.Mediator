using System;
using CCSWE.nanoFramework.Mediator.Test.Shared;
using nanoFramework.DependencyInjection;
using nanoFramework.TestFramework;

namespace CCSWE.nanoFramework.Mediator.UnitTests.Extensions
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        [Setup]
        public void Setup()
        {
            Assert.SkipTest("These tests currently timeout. Come back to this");
        }

        [TestMethod]
        public void AddMediator_should_configure_AsyncMediatorOptions()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddMediator(options =>
            {
                options.DelayedStart = true;
                options.AddSubscriber(typeof(MediatorEventMock), typeof(IMediatorSubscriberMock));
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var result = serviceProvider.GetService(typeof(AsyncMediatorOptions));

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(AsyncMediatorOptions));

            var options = (AsyncMediatorOptions) result;

            Assert.IsTrue(options.DelayedStart);
            Assert.AreEqual(1, options.Subscribers.Count);
        }

        [TestMethod]
        public void AddMediator_should_register_AsyncMediator()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddMediator();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var result1 = serviceProvider.GetService(typeof(IMediator));
            var result2 = serviceProvider.GetService(typeof(IMediator));

            // Assert
            Assert.IsNotNull(result1);
            Assert.IsInstanceOfType(result1, typeof(AsyncMediator));
            Assert.AreEqual(result1, result2);
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
