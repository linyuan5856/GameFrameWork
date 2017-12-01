using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pandora
{
    public class HMMonoBehaviour : MonoBehaviour
    {

        // Update is called once per frame
        void Update()
        {
            this.OnUpdate();
        }

        protected virtual void OnUpdate()
        {

        }
    }
}

