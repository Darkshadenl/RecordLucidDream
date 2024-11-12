namespace Interfaces
{
    public interface IAudioRecordedEventBus
    {
        void Publish<T>(T @event);
        void Subscribe<T>(Action<T> handler);
    }
}