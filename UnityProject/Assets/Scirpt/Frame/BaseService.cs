using System;
using GameFrameWork.Utils;

namespace GameFrameWork
{
    public abstract class BaseService : IService
    {
        private EventDispatcher<int, object> _eventDispatcher;
        private IFacade _facade;

        void IService.Create(IFacade facade)
        {
            _facade = facade;
            OnCreate();
        }

        void IService.Release()
        {
            OnRelease();
            _facade = null;
            _eventDispatcher?.ClearEvent();
            _eventDispatcher = null;
#if UNITY_EDITOR
            Debuger.Log($"{GetType().Name} is Release");
#endif
        }

        void IService.DoUpdate(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        void IService.OnApplicationQuit()
        {
            OnApplicationQuit();
        }

        protected virtual void OnCreate()
        {
        }

        protected virtual void OnRelease()
        {
        }

        protected virtual void OnUpdate(float deltaTime)
        {
        }

        protected virtual void OnApplicationQuit()
        {
        }

        protected IFacade GetFacade()
        {
            return _facade;
        }

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