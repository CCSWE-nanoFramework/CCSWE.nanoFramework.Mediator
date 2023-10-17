using System;
using System.Collections;
using CCSWE.nanoFramework.Mediator.Test.Shared;
using nanoFramework.TestFramework;

namespace CCSWE.nanoFramework.Mediator.UnitTests
{
    [TestClass]
    public class AsyncMediatorOptionsTests
    {
        [TestMethod]
        public void AddSubscription_should_add_subscriber()
        {
            // Arrange
            var sut = new AsyncMediatorOptions();

            // Act
            sut.AddSubscriber(typeof(MediatorEventMock), typeof(IMediatorSubscriberMock));

            // Assert
            Assert.AreEqual(1, sut.Subscribers.Count);
        }

        [TestMethod]
        public void AddSubscription_should_throw_exception_for_invalid_event_type()
        {
            // Arrange
            var sut = new AsyncMediatorOptions();

            // Act
            Assert.ThrowsException(typeof(ArgumentException), () => sut.AddSubscriber(typeof(ArrayList), typeof(MediatorSubscriberMock)));

            // Assert
            Assert.AreEqual(0, sut.Subscribers.Count);
        }
    }
}
