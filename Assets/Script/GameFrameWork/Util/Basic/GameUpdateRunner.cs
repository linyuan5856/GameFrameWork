using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pandora
{
    public class GameUpdateRunner : MonoBehaviour, ISerializationCallbackReceiver
    {

        static List<HMMonoBehaviour> monoList = new List<HMMonoBehaviour>();

        public static void Register(HMMonoBehaviour mono)
        {
            monoList.Add(mono);
        }

        void Update()
        {
            for (int i = 0; i < monoList.Count; i++)
            {
                monoList[i].UpdateMono();
            }
        }


        [SerializeField]
        private List<HMMonoBehaviour>serializMonoList=new List<HMMonoBehaviour>();
        public void OnBeforeSerialize()
        {
            serializMonoList.Clear();
            for (int i = 0; i < monoList.Count; i++)
            {
                serializMonoList.Add(monoList[i]);
            }
        }

        public void OnAfterDeserialize()
        {             
        }

    }
}
