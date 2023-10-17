namespace CCSWE.nanoFramework.Mediator
{
    public interface IMediatorSubscriber
    {
        void HandleEvent(IMediatorEvent mediatorEvent);
    }
}
