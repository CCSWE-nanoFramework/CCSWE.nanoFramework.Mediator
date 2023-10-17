using System;

namespace CCSWE.nanoFramework.Mediator.Internal
{
    internal static class MediatorTypeUtils
    {
        public static bool IsMediatorEvent(Type type)
        {
            var interfaces = type.GetInterfaces();
            foreach (var current in interfaces)
            {
                if (current == typeof(IMediatorEvent))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsMediatorSubscriber(Type type)
        {
            var interfaces = type.GetInterfaces();
            foreach (var current in interfaces)
            {
                if (current == typeof(IMediatorSubscriber))
                {
                    return true;
                }
            }

            return false;
        }

        public static void RequireMediatorEvent(Type eventType)
        {
            if (!IsMediatorEvent(eventType))
            {
                throw new ArgumentException($"{eventType.Name} does not implement {nameof(IMediatorEvent)}");
            }
        }

        public static void RequireMediatorSubscriber(Type subscriberType)
        {
            if (!IsMediatorSubscriber(subscriberType))
            {
                throw new ArgumentException($"{subscriberType.Name} does not implement {nameof(IMediatorSubscriber)}");
            }
        }
    }
}
