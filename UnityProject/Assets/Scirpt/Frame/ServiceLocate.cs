using System;
using UnityEngine;

namespace GameFrameWork
{
    public class ServiceLocate : Singleton<ServiceLocate>, IService
    {
        private MapList<Type, IService> map;
        private bool isInit;

        public void Create()
        {
            if (map == null)
                map = new MapList<Type, IService>();
        }

        public void CreateAllServices()
        {
            if (!isInit)
                ForEachService(service => service.Create());
            isInit = true;
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
            isInit = false;
        }


        public T RegisterService<T>() where T : class, IService
        {
            Type type = typeof(T);
            if (map.ContainsKey(type))
                return map[type] as T;

            var service = Activator.CreateInstance<T>();
            map.Add(type, service);
            return service;
        }

        public T RegisterServiceAutoCreate<T>() where T : class, IService
        {
            var service = RegisterService<T>();
            service.Create();
            return service;
        }

        public void UnRegisterService<T>() where T : class, IService
        {
            Type type = typeof(T);
            if (TryGetService(out T t))
            {
                t.Release();
                map.Remove(type);
            }
        }


        public T GetService<T>() where T : class, IService
        {
            Type type = typeof(T);
            return Internal_GetService(type) as T;
        }

        public bool TryGetService<T>(out T t) where T : class, IService
        {
            Type type = typeof(T);
            t = Internal_GetService(type, false) as T;
            return t != null;
        }

        private IService Internal_GetService(Type type, bool isDebug = true)
        {
            if (map.ContainsKey(type))
                return map[type];
            if (isDebug)
                Debuger.LogWarning($"{type} Service 尚未注册");
            return null;
        }

        void ForEachService(Action<IService> ac)
        {
            var list = map.AsList();
            for (int i = 0; i < list.Count; i++)
            {
                var service = list[i];
                ac(service);
            }
        }
    }
}