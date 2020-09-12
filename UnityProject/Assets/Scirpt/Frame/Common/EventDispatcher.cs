using System;
using System.Collections.Generic;

namespace GameFrameWork.Utils
{
    public class EventDispatcher<S, T>
    {
        private readonly Dictionary<S, Action<T>> _eventDict = new Dictionary<S, Action<T>>();

        public virtual void AddListener(S eventCode, Action<T> callback)
        {
            if (callback == null) Debuger.LogError("can not add null callback");
            if (_eventDict.ContainsKey(eventCode))
            {
                Action<T> callbacks = _eventDict[eventCode];
                if (callbacks != null)
                {
                    Delegate[] invocationList = callbacks.GetInvocationList();
                    int index = Array.IndexOf(invocationList, callback);
                    if (index != -1)
                    {
                        Debuger.LogWarning("repeated listener regist,eventcode:" + eventCode);
                        return;
                    }
                }

                callbacks += callback;
                _eventDict[eventCode] = callbacks;
            }
            else
            {
                Action<T> cb = null;
                cb += callback;
                _eventDict[eventCode] = cb;
            }
        }

        public virtual void RemoveListener(S eventCode, Action<T> callback)
        {
            if (_eventDict.ContainsKey(eventCode))
                _eventDict[eventCode] -= callback;
        }

        public virtual bool DispatchEvent(S eventCode, T param = default(T))
        {
            if (_eventDict.ContainsKey(eventCode))
            {
                Action<T> callback = _eventDict[eventCode];
                if (callback != null)
                {
                    callback(param);
                    return true;
                }
            }

            return false;
        }

        public virtual void ClearEvent()
        {
            _eventDict.Clear();
        }
    }
}