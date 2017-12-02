using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace Pandora
{
    public static class GameUtil
    {
       
        public static String GetDeviceId()
        {
            String deviceId = PlayerPrefs.GetString("New_deviceUniqueIdentifier", string.Empty);
            if (string.IsNullOrEmpty(deviceId))
            {
#if UNITY_ANDROID && !UNITY_EDITOR
		//deviceId = activity.Call<String>("GetDeviceId"); todo
#else
                deviceId = SystemInfo.deviceUniqueIdentifier + ":" + DateTime.Now.Ticks;
#endif
                PlayerPrefs.SetString("New_deviceUniqueIdentifier", deviceId);
            }
            return deviceId;
        }

        public static string To16String(int code)
        {
            return (code < 0 ? "-0x" : "0x") + Convert.ToString(Math.Abs(code), 16);
        }

        public static T AddComponentToP<T>(GameObject parent) where T : Component
        {
            string name = typeof(T).Name;
            GameObject go = new GameObject(name);
            go.SetParent(parent);         
            return go.AddComponent<T>();
        }

        public static void SetRenderQueue(GameObject go, int number)
        {
            Renderer[] renders = go.GetComponentsInChildren<Renderer>();
            if (renders != null && renders.Length > 0)
            {
                foreach (var renderer in renders)
                {
                    Material[] mats = renderer.materials;
                    foreach (var material in mats)
                    {
                        material.renderQueue = number;
                    }
                }
            }
        }

        public static void SetTransformZ(Transform trans, float z)
        {
            Vector3 pos = trans.localPosition;
            pos.z = z;
            trans.localPosition = pos;
        }


        private static StringBuilder sb = new StringBuilder();
        public static string AddString(object p1, object p2)
        {
            sb.Length = 0;
            sb.Append(p1);
            sb.Append(p2);
            return sb.ToString();
        }
        public static string AddString(object p1, object p2, object p3)
        {
            sb.Length = 0;
            sb.Append(p1);
            sb.Append(p2);
            sb.Append(p3);
            return sb.ToString();
        }
        public static string AddString(params object[] pList)
        {
            sb.Length = 0;
            for (int i = 0; i < pList.Length; i++)
            {
                sb.Append(pList[i]);
            }
            return sb.ToString();
        }
    }
}