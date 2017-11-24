using System;
using System.Collections.Generic;

namespace GFW
{
    public class EventDispatcher<T>
    {
        private Dictionary<int, Action<T>> eventDict = new Dictionary<int, Action<T>>();

        public void AddListener(int _eventCode, Action<T> _callback)
        {
            if (eventDict.ContainsKey(_eventCode))
            {
                Action<T> callbacks = eventDict[_eventCode];
                if (callbacks != null)
                {
                    Delegate[] invocationList = callbacks.GetInvocationList();
                    int index = Array.IndexOf(invocationList, _callback);
                    if (index != -1)
                    {
                        Logger.LogWarn("repeated listener regist,eventcode:" + _eventCode);
                        return;
                    }
                }
                callbacks += _callback;
                eventDict[_eventCode] = callbacks;
            }
            else
            {
                Action<T> callback = null;
                callback += _callback;
                eventDict[_eventCode] = callback;
            }
        }

        public void RemoveListener(int _eventCode, Action<T> _callback)
        {
            if (eventDict.ContainsKey(_eventCode))
            {
                eventDict[_eventCode] -= _callback;
            }

        }

        public bool DispatchEvent(int _eventCode, T _param = default(T))
        {
            if (eventDict.ContainsKey(_eventCode))
            {
                Action<T> callback = eventDict[_eventCode];
                if (callback != null)
                {
                    callback(_param);
                    return true;
                }
            }
            return false;
        }

        public void ClearEvent()
        {
            eventDict.Clear();
        }
    }
}