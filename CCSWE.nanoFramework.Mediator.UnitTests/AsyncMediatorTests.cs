using nanoFramework.TestFramework;
using System;
using System.Threading;
using CCSWE.nanoFramework.Mediator.UnitTests.Mocks;
using nanoFramework.DependencyInjection;

namespace CCSWE.nanoFramework.Mediator.UnitTests
{
    // TODO: Add tests to validate parameter checking

    [TestClass]
    public class AsyncMediatorTests
    {
        public static TimeSpan PublishDelay = TimeSpan.FromMilliseconds(500);

        [TestMethod]
        public void Subscribe_should_add_singleton_subscriber()
        {
            // Arrange
            var mediatorEvent = new MediatorEventMock();
            var mediatorSubscriber = new MediatorEventHandlerMock();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(typeof(IMediatorEventHandlerMock), mediatorSubscriber);

            using var sut = new AsyncMediator(new AsyncMediatorOptions(), new LoggerMock(), serviceCollection.BuildServiceProvider());

            // Act
            sut.Subscribe(typeof(MediatorEventMock), typeof(IMediatorEventHandlerMock));
            sut.Publish(mediatorEvent);

            WaitForPublisherThread();

            // Assert
            Assert.AreEqual(1, mediatorSubscriber.EventsReceived);
            Assert.AreEqual(mediatorEvent, mediatorSubscriber.LastEvent);
        }

        [TestMethod]
        public void Subscribe_should_add_transient_subscriber()
        {
            // Arrange
            var mediatorEvent = new MediatorEventMock();
            var mediatorSubscriber = new MediatorEventHandlerMock();

            using var sut = new AsyncMediator(new AsyncMediatorOptions(), new LoggerMock(), new ServiceCollection().BuildServiceProvider());

            // Act
            sut.Subscribe(typeof(MediatorEventMock), mediatorSubscriber);
            sut.Publish(mediatorEvent);

            WaitForPublisherThread();

            // Assert
            Assert.AreEqual(1, mediatorSubscriber.EventsReceived);
            Assert.AreEqual(mediatorEvent, mediatorSubscriber.LastEvent);
        }

        [TestMethod]
        public void Unsubscribe_should_remove_singleton_subscriber()
        {
            // Arrange
            var mediatorEvent = new MediatorEventMock();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(typeof(IMediatorEventHandlerMock), typeof(MediatorEventHandlerMock));
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var mediatorSubscriber = (MediatorEventHandlerMock)serviceProvider.GetRequiredService(typeof(IMediatorEventHandlerMock));

            using var sut = new AsyncMediator(new AsyncMediatorOptions(), new LoggerMock(), serviceCollection.BuildServiceProvider());

            // Act
            sut.Subscribe(typeof(MediatorEventMock), typeof(IMediatorEventHandlerMock));
            sut.Unsubscribe(typeof(MediatorEventMock), typeof(IMediatorEventHandlerMock));
            sut.Publish(mediatorEvent);

            WaitForPublisherThread();

            // Assert
            Assert.AreEqual(0, mediatorSubscriber.EventsReceived);
            Assert.IsNull(mediatorSubscriber.LastEvent);
        }

        [TestMethod]
        public void Unsubscribe_should_remove_transient_subscriber()
        {
            // Arrange
            var mediatorEvent = new MediatorEventMock();
            var mediatorSubscriber = new MediatorEventHandlerMock();

            using var sut = new AsyncMediator(new AsyncMediatorOptions(), new LoggerMock(), new ServiceCollection().BuildServiceProvider());

            // Act
            sut.Subscribe(typeof(MediatorEventMock), mediatorSubscriber);
            sut.Unsubscribe(typeof(MediatorEventMock), mediatorSubscriber);
            sut.Publish(mediatorEvent);

            WaitForPublisherThread();

            // Assert
            Assert.AreEqual(0, mediatorSubscriber.EventsReceived);
            Assert.IsNull(mediatorSubscriber.LastEvent);
        }

        private static void WaitForPublisherThread()
        {
            Thread.Sleep(PublishDelay);
        }
    }
}
