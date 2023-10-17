using System;

namespace CCSWE.nanoFramework.Mediator.Test.Shared
{
    public interface IMediatorEventMock
    {
    }

    public class MediatorEventMock: IMediatorEvent, IMediatorEventMock
    {
        public Guid Id { get; } = Guid.NewGuid();

        public override bool Equals(object other)
        {
            return other is MediatorEventMock otherEvent && Id.Equals(otherEvent.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{nameof(MediatorEventMock)}: {Id}";
        }
    }
}
