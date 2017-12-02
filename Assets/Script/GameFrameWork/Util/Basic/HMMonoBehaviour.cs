using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pandora
{
    public class HMMonoBehaviour : MonoBehaviour
    {
        void Awake()
        {          
            GameUpdateRunner.Register(this);
            OnAwake();
        }

        public void UpdateMono()
        {
            this.OnUpdate();
        }

        protected virtual void OnUpdate()
        {

        }

        protected virtual void OnAwake()
        {

        }
    }
}

