using CCSWE.nanoFramework.Mediator.Internal;
using System.Collections;
using System;
using Microsoft.Extensions.Logging;

namespace CCSWE.nanoFramework.Mediator
{
    /// <summary>
    /// Options used to configure <see cref="AsyncMediator"/>.
    /// </summary>
    public class AsyncMediatorOptions
    {
        /// <summary>
        /// Controls whether the <see cref="AsyncMediator"/> is started immediately or is delayed until the first message is published.
        /// </summary>
        public bool DelayedStart { get; set; } = false;

        /// <summary>
        /// The default <see cref="LogLevel"/> used for debug log messages.
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Trace;

        internal ArrayList Subscribers { get; } = new();

        /// <summary>
        /// Adds a singleton subscriber to an event.
        /// </summary>
        /// <param name="eventType">Type of the event. The event must implement <see cref="IMediatorEvent"/>.</param>
        /// <param name="subscriberType">Type of the subscriber (as registered in DI). The subscriber must implement <see cref="IMediatorEventHandler"/>.</param>
        public void AddSubscriber(Type eventType, Type subscriberType)
        {
            MediatorTypeUtils.RequireMediatorEvent(eventType);

            Subscribers.Add(new MediatorOptionsSubscriber(eventType, subscriberType));
        }
    }
}
