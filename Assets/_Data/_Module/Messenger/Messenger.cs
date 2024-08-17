using System;
using System.Collections.Generic;

namespace MessageEvent
{
    public static class Messenger
    {
        private static readonly Dictionary<string, Delegate> EventData = new Dictionary<string, Delegate>();

        public static void AddListener(string key, Action callback)
        {
            if (EventData.ContainsKey(key))
            {
                EventData[key] = Delegate.Combine(EventData[key], callback);
            }
            else
            {
                EventData[key] = callback;
            }
        }

        public static void AddListener<T>(string key, Action<T> callback)
        {
            if (EventData.ContainsKey(key))
            {
                EventData[key] = Delegate.Combine(EventData[key], callback);
            }
            else
            {
                EventData[key] = callback;
            }
        }

        public static void AddListener<T, V>(string key, Action<T, V> callback)
        {
            if (EventData.ContainsKey(key))
            {
                EventData[key] = Delegate.Combine(EventData[key], callback);
            }
            else
            {
                EventData[key] = callback;
            }
        }

        public static void Broadcast(string key)
        {
            if (EventData.TryGetValue(key, out var callback))
            {
                callback.DynamicInvoke();
            }
        }

        public static void Broadcast<T>(string key, T parameter)
        {
            if (EventData.TryGetValue(key, out var callback))
            {
                callback.DynamicInvoke(parameter);
            }
        }

        public static void Broadcast<T, V>(string key, T parameter1, V parameter2)
        {
            if (EventData.TryGetValue(key, out var callback))
            {
                callback.DynamicInvoke(parameter1, parameter2);  
            }
        }

        public static void RemoveListener(string key, Action callback)
        {
            if (EventData.TryGetValue(key, out var value))
            {
                EventData[key] = Delegate.Remove(value, callback);
                if (EventData[key] == null)
                {
                    EventData.Remove(key);
                }
            }
        }

        public static void RemoveListener<T>(string key, Action<T> callback)
        {
            if (EventData.TryGetValue(key, out var value))
            {
                EventData[key] = Delegate.Remove(value, callback);
                if (EventData[key] == null)
                {
                    EventData.Remove(key);
                }
            }
        }

        public static void RemoveListener<T, V>(string key, Action<T, V> callback)
        {
            if (EventData.TryGetValue(key, out var delegates))
            {
                EventData[key] = Delegate.Remove(delegates, callback);
                if (EventData[key] == null)
                {
                    EventData.Remove(key);
                }
            }
        }
    }
}