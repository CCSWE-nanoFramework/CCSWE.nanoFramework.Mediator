using CCSWE.nanoFramework.Mediator.Internal;
using System.Collections;
using System;

namespace CCSWE.nanoFramework.Mediator
{
    public class AsyncMediatorOptions
    {
        /// <summary>
        /// Controls whether the <see cref="AsyncMediator"/> is started immediately or is delayed until the first message is published.
        /// </summary>
        public bool DelayedStart { get; set; } = false;

        internal ArrayList Subscribers { get; } = new();

        /// <summary>
        /// Adds a singleton subscriber to an event.
        /// </summary>
        /// <param name="eventType">Type of the event. The event must implement <see cref="IMediatorEvent"/>.</param>
        /// <param name="subscriberType">Type of the subscriber (as registered in DI). The subscriber must implement <see cref="IMediatorSubscriber"/>.</param>
        public void AddSubscriber(Type eventType, Type subscriberType)
        {
            MediatorTypeUtils.RequireMediatorEvent(eventType);

            Subscribers.Add(new MediatorOptionsSubscriber(eventType, subscriberType));
        }
    }
}
