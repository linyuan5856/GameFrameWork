/// <summary>
/// Generic Mono singleton.
/// </summary>
using UnityEngine;

namespace Pandora
{
    public abstract class MonoSingleton<T> : HMMonoBehaviour where T : MonoSingleton<T>
    {
      
        private static T m_Instance = null;

        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    GameLogger.LogError(string.Format("{0} 's Instance is Not Init",typeof(T).Name));
                }
                return m_Instance;
            }
        }

        void Awake()
        {
            if (m_Instance == null)
            {
                m_Instance = this as T;
                this.Init();
            }
        }

        protected virtual void Init()
        {

        }
    }
}