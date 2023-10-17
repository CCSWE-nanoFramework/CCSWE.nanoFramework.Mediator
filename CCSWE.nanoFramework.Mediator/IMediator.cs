using System;

namespace CCSWE.nanoFramework.Mediator
{
    /// <summary>
    /// A simple mediator pattern interface
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Publishes an event and calls each subscriber.
        /// </summary>
        /// <param name="mediatorEvent">The event.</param>
        void Publish(IMediatorEvent mediatorEvent);

        /// <summary>
        /// Subscribes event handler to an event.
        /// </summary>
        /// <param name="eventType">Type of the event. The event must implement <see cref="IMediatorEvent"/>.</param>
        /// <param name="subscriber">The subscriber.</param>
        void Subscribe(Type eventType, IMediatorSubscriber subscriber);

        /// <summary>
        /// Subscribes event handler to an event.
        /// </summary>
        /// <param name="eventType">Type of the event. The event must implement <see cref="IMediatorEvent"/>.</param>
        /// <param name="subscriberType">Type of the subscriber (as registered in DI). The subscriber must implement <see cref="IMediatorSubscriber"/>.</param>
        void Subscribe(Type eventType, Type subscriberType);

        /// <summary>
        /// Unsubscribes event handler from an event.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="subscriber">The subscriber.</param>
        public void Unsubscribe(Type eventType, IMediatorSubscriber subscriber);

        /// <summary>
        /// Unsubscribes event handler from an event.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="subscriberType">Type of the subscriber (as registered in DI).</param>
        void Unsubscribe(Type eventType, Type subscriberType);
    }
}
