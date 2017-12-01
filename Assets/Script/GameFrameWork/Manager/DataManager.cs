using System;
using System.Collections.Generic;

namespace Pandora
{
    public class DataManager : Singleton<DataManager>
    {

        private Dictionary<string, object> classDefineDict = new Dictionary<string, object>();

        T GetObj<T>()
        {
            string name = typeof(T).Name;
            if (classDefineDict.ContainsKey(name))
            {
                return (T)classDefineDict[name];
            }

            T t = Activator.CreateInstance<T>();
            classDefineDict[name] = t;
            return t;
        }

        public static T GetTable<T>()
        {
            return Instance.GetObj<T>();
        }


        public void Clear()
        {
            classDefineDict.Clear();
        }
    }
}
