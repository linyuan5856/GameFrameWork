﻿public abstract class Singleton<T> where T : Singleton<T>, new()
{
    protected static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
                _instance.InitSingleton();
            }

            return _instance;
        }
    }

    protected virtual void InitSingleton()
    {
    }
}