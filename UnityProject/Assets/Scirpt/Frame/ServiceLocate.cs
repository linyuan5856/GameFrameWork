using System;
using UnityEngine;

namespace GameFrameWork
{
    public class ServiceLocate
    {
        private readonly MapList<Type, IService> _map;
        private bool _isInit;
        private readonly IFacade _facade;

        public ServiceLocate(IFacade facade)
        {
            _facade = facade;
            if (_map == null)
                _map = new MapList<Type, IService>();
        }

        public void CreateAllServices()
        {
            if (_isInit)
                return;
            ForEachService(service => service.Create(_facade));
            _isInit = true;
        }

        public void DoUpdate(float deltaTime)
        {
            ForEachService(service => service.DoUpdate(Time.deltaTime));
        }

        public void OnApplicationQuit()
        {
            ForEachService(service => service.OnApplicationQuit());
        }


        public void Release()
        {
            ForEachService(service => service.Release());
            _isInit = false;
        }


        public T RegisterService<T>() where T : class, IService
        {
            Type type = typeof(T);
            if (_map.ContainsKey(type))
                return _map[type] as T;

            var service = Activator.CreateInstance<T>();
            _map.Add(type, service);
            return service;
        }

        public T RegisterServiceAutoCreate<T>() where T : class, IService
        {
            var service = RegisterService<T>();
            service.Create(_facade);
            return service;
        }

        public void UnRegisterService<T>() where T : class, IService
        {
            Type type = typeof(T);
            if (TryGetService(out T t))
            {
                t.Release();
                _map.Remove(type);
            }
        }


        public T GetService<T>() where T : IService
        {
            Type type = typeof(T);
            return (T) Internal_GetService(type);
        }

        public bool TryGetService<T>(out T t) where T : IService
        {
            Type type = typeof(T);
            t = (T) Internal_GetService(type, false);
            return t != null;
        }

        private IService Internal_GetService(Type type, bool isDebug = true)
        {
            if (_map.ContainsKey(type))
                return _map[type];
            if (isDebug)
                Debuger.LogWarning($"{type} Service 尚未注册");
            return null;
        }

        void ForEachService(Action<IService> ac)
        {
            var list = _map.AsList();
            foreach (var service in list)
                ac(service);
        }
    }
}