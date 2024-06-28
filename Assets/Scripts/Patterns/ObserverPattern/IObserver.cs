namespace Patterns.ObserverPattern
{
    public interface IObserver
    {
        void OnNotify(EventKey key);
    }
}