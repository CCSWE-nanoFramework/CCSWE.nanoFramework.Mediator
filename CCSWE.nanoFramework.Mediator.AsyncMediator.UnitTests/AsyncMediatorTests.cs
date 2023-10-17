using CCSWE.nanoFramework.Mediator.Test.Shared;
using nanoFramework.TestFramework;
using System;
using System.Threading;
using nanoFramework.DependencyInjection;

namespace CCSWE.nanoFramework.Mediator.UnitTests
{
    [TestClass]
    public class AsyncMediatorTests
    {
        public static TimeSpan PublishDelay = TimeSpan.FromMilliseconds(500);

        [TestMethod]
        public void Subscribe_should_add_singleton_subscriber()
        {
            // Arrange
            var mediatorEvent = new MediatorEventMock();
            var mediatorSubscriber = new MediatorSubscriberMock();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(typeof(IMediatorSubscriberMock), mediatorSubscriber);

            using var sut = new AsyncMediator(serviceCollection.BuildServiceProvider());
            sut.Start();

            // Act
            sut.Subscribe(typeof(MediatorEventMock), typeof(IMediatorSubscriberMock));
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
            var mediatorSubscriber = new MediatorSubscriberMock();

            using var sut = new AsyncMediator(new ServiceCollection().BuildServiceProvider());
            sut.Start();

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
            serviceCollection.AddSingleton(typeof(IMediatorSubscriberMock), typeof(MediatorSubscriberMock));
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var mediatorSubscriber = (MediatorSubscriberMock)serviceProvider.GetRequiredService(typeof(IMediatorSubscriberMock));

            using var sut = new AsyncMediator(serviceCollection.BuildServiceProvider());
            sut.Start();

            // Act
            sut.Subscribe(typeof(MediatorEventMock), typeof(IMediatorSubscriberMock));
            sut.Unsubscribe(typeof(MediatorEventMock), typeof(IMediatorSubscriberMock));
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
            var mediatorSubscriber = new MediatorSubscriberMock();

            using var sut = new AsyncMediator(new ServiceCollection().BuildServiceProvider());
            sut.Start();

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
