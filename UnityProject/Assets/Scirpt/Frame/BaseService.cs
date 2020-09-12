using System;
using GameFrameWork.Utils;

namespace GameFrameWork
{
    public abstract class BaseService : IService
    {
        public virtual void Create()
        {
        }

        public virtual void Release()
        {
            _eventDispatcher?.ClearEvent();
            _eventDispatcher = null;
#if UNITY_EDITOR
            Debuger.Log($"{GetType().Name} is Release");
#endif
        }

        public virtual void DoUpdate(float deltaTime)
        {
        }

        public virtual void OnApplicationQuit()
        {
        }


        private EventDispatcher<int, object> _eventDispatcher;

        private EventDispatcher<int, object> GetDispatcher()
        {
            return _eventDispatcher ?? (_eventDispatcher = new EventDispatcher<int, object>());
        }

        public void AddEventListener(int eventId, Action<object> callback)
        {
            GetDispatcher().AddListener(eventId, callback);
        }

        public void RemoveEventListener(int eventId, Action<object> callback)
        {
            GetDispatcher().RemoveListener(eventId, callback);
        }

        public void DispatchEvent(int eventId, object obj = null)
        {
            GetDispatcher().DispatchEvent(eventId, obj);
        }
    }
}