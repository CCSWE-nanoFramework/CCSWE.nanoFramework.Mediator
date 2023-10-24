using System;
using System.Collections;
using System.Threading;
using CCSWE.nanoFramework.Mediator.Internal;
using Microsoft.Extensions.Logging;

namespace CCSWE.nanoFramework.Mediator
{
    /// <summary>
    /// Threaded implementation of Mediator pattern
    /// </summary>
    public class AsyncMediator : IMediator, IDisposable
    {
        private bool _disposed;
        private readonly Queue _eventQueue = new();
        private readonly AutoResetEvent _eventWaiting = new(false);
        private Thread? _publishThread;
        private readonly ILogger _logger;
        private readonly LogLevel _logLevel;
        private readonly IServiceProvider _serviceProvider;
        private readonly Hashtable _subscribers = new();
        private readonly Hashtable _subscriberTypes = new();
        private readonly object _syncLock = new();

        /// <summary>
        /// Create a new instance of <see cref="AsyncMediator"/>
        /// </summary>
        /// <param name="options">The <see cref="AsyncMediatorOptions"/> used to configure this <see cref="AsyncMediator"/>.</param>
        /// <param name="logger">The <see cref="ILogger"/> to log events to.</param>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> use to location singleton subscribers.</param>
        public AsyncMediator(AsyncMediatorOptions options, ILogger logger, IServiceProvider serviceProvider)
        {
            _logLevel = options.LogLevel;
            _logger = logger;
            _serviceProvider = serviceProvider;

            foreach (MediatorOptionsSubscriber subscriber in options.Subscribers)
            {
                Subscribe(subscriber.EventType, subscriber.SubscriberType);
            }

            if (!options.DelayedStart)
            {
                Start();
            }
        }

        /// <summary>
        /// No summary needed...
        /// </summary>
        ~AsyncMediator()
        {
            Dispose(false);
        }

        private CancellationToken CancellationToken => CancellationTokenSource.Token;

        private CancellationTokenSource CancellationTokenSource { get; } = new();

        private void CheckPublishThread()
        {
            if (_publishThread is null && !CancellationToken.IsCancellationRequested)
            {
                Start();
            }
        }

        private void DebugLog(string message)
        {
            if (string.IsNullOrEmpty(message) || !_logger.IsEnabled(_logLevel))
            {
                return;
            }

            _logger.Log(_logLevel, $"[{nameof(AsyncMediator)}] {message}");
        }

        private IMediatorEvent? DequeueEvent()
        {
            lock (_syncLock)
            {
                return _eventQueue.Count > 0 ? _eventQueue.Dequeue() as IMediatorEvent : null;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            lock (_syncLock)
            {
                if (_disposed)
                {
                    return;
                }

                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Stop();
            }

            _disposed = true;
        }

        /// <inheritdoc />
        public void Publish(IMediatorEvent mediatorEvent)
        {
            CheckPublishThread();

            lock (_syncLock)
            {
                DebugLog("Queueing event {mediatorEvent.GetType().Name}");

                _eventQueue.Enqueue(mediatorEvent);
                _eventWaiting.Set();
            }
        }

        private void PublishInternal(IMediatorEvent mediatorEvent)
        {
            var eventName = mediatorEvent.GetType().FullName;

            if (_subscribers.Contains(eventName))
            {
                foreach (IMediatorEventHandler subscriber in (ArrayList)_subscribers[eventName])
                {
                    subscriber.HandleEvent(mediatorEvent);
                }
            }

            if (!_subscriberTypes.Contains(eventName))
            {
                return;
            }

            foreach (Type subscriberType in (ArrayList)_subscriberTypes[eventName])
            {
                var service = _serviceProvider.GetService(subscriberType);
                if (service is not IMediatorEventHandler subscriber)
                {
                    // Should I just log an error here instead?
                    throw new InvalidOperationException($"{service.GetType().FullName} registered as {subscriberType.FullName} does not implement {nameof(IMediatorEventHandler)}");
                }
                subscriber.HandleEvent(mediatorEvent);
            }
        }

        private void PublishThread()
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                _eventWaiting.WaitOne();



                var eventToPublish = DequeueEvent();

                while (eventToPublish is not null)
                {
                    DebugLog("Publishing event {eventToPublish.GetType().Name}");

                    PublishInternal(eventToPublish);

                    eventToPublish = DequeueEvent();
                }
            }
        }

        internal void Start()
        {
            // ReSharper disable once InvertIf
            if (_publishThread is null)
            {
                lock (_syncLock)
                {
                    if (_publishThread is not null)
                    {
                        return;
                    }

                    DebugLog("Starting publisher thread");

                    _publishThread = new Thread(PublishThread);
                    _publishThread.Start();
                }
            }
        }

        internal void Stop()
        {
            CancellationTokenSource.Cancel();

            _eventWaiting.Set();

            if (_publishThread is null)
            {
                return;
            }

            DebugLog("Stopping publisher thread");

            try
            {
                if (Thread.CurrentThread != _publishThread)
                {
                    _publishThread.Join(10_000);
                }
            }
            finally
            {
                _publishThread = null;
            }
        }

        /// <inheritdoc />
        public void Subscribe(Type eventType, IMediatorEventHandler eventHandler)
        {
            MediatorTypeUtils.RequireMediatorEvent(eventType);

            DebugLog("Adding subscriber: {eventType.Name} - {eventHandler.GetType().Name}");
         
            var eventName = eventType.FullName;
            if (!_subscribers.Contains(eventName))
            {
                _subscribers.Add(eventName, new ArrayList { eventHandler });
                return;
            }

            var subscribers = (ArrayList)_subscribers[eventName];
            if (!subscribers.Contains(eventHandler))
            {
                subscribers.Add(eventHandler);
            }

        }

        /// <inheritdoc />
        public void Subscribe(Type eventType, Type subscriberType)
        {
            MediatorTypeUtils.RequireMediatorEvent(eventType);

            DebugLog("Adding subscriber: {eventType.Name} - {subscriberType.Name}");

            var eventName = eventType.FullName;
            if (!_subscriberTypes.Contains(eventName))
            {
                _subscriberTypes.Add(eventName, new ArrayList { subscriberType });
                return;
            }

            var subscribers = (ArrayList)_subscriberTypes[eventName];
            if (!subscribers.Contains(subscriberType))
            {
                subscribers.Add(subscriberType);
            }
        }

        /// <inheritdoc />
        public void Unsubscribe(Type eventType, IMediatorEventHandler eventHandler)
        {
            DebugLog("Removing subscriber: {eventType.Name} - {eventHandler.GetType().Name}");
         
            var eventName = eventType.FullName;
            if (!_subscribers.Contains(eventName))
            {
                return;
            }

            var subscribers = (ArrayList)_subscribers[eventName];
            if (subscribers.Contains(eventHandler))
            {
                subscribers.Remove(eventHandler);
            }
        }

        /// <inheritdoc />
        public void Unsubscribe(Type eventType, Type subscriberType)
        {
            DebugLog("Removing subscriber: {eventType.Name} - {subscriberType.Name}");
          
            var eventName = eventType.FullName;
            if (!_subscriberTypes.Contains(eventName))
            {
                return;
            }

            var subscribers = (ArrayList)_subscriberTypes[eventName];
            if (subscribers.Contains(subscriberType))
            {
                subscribers.Remove(subscriberType);
            }
        }
    }
}
