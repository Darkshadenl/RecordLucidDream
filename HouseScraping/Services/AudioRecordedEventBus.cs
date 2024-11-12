using System;
using Interfaces;

namespace HouseScraping.Services;

public class AudioRecordedEventBus : IAudioRecordedEventBus
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();
    private readonly object _lock = new();

    public void Publish<T>(T @event)
    {
        if (@event == null) return;

        lock (_lock)
        {
            if (_handlers.TryGetValue(typeof(T), out var delegates))
            {
                foreach (var handler in delegates.ToList())
                {
                    if (handler is Action<T> action)
                    {
                        action(@event);
                    }
                }
            }
        }
    }

    public void Subscribe<T>(Action<T> handler)
    {
        lock (_lock)
        {
            var type = typeof(T);
            if (!_handlers.ContainsKey(type))
            {
                _handlers[type] = new List<Delegate>();
            }
            _handlers[type].Add(handler);
        }
    }

    public void Unsubscribe<T>(Action<T> handler)
    {
        lock (_lock)
        {
            var type = typeof(T);
            if (_handlers.ContainsKey(type))
            {
                _handlers[type].Remove(handler);
            }
        }
    }
}
