using System;
using System.Collections;
using UnityEngine;

namespace GFW
{
    public static class GameUtil
    {
        public static void StartCoroutine(IEnumerator enumerator)
        {
            MainGame.Instance.StartCoroutine(enumerator);
        }

        public static String GetDeviceId()
        {
            String deviceId = PlayerPrefs.GetString("New_deviceUniqueIdentifier", string.Empty);
            if (string.IsNullOrEmpty(deviceId))
            {
#if UNITY_ANDROID && !UNITY_EDITOR
		deviceId = activity.Call<String>("GetDeviceId");
#else
                deviceId = SystemInfo.deviceUniqueIdentifier + ":" + DateTime.Now.Ticks;
#endif
                PlayerPrefs.SetString("New_deviceUniqueIdentifier", deviceId);
            }
            return deviceId;
        }


        public static string to16string(int code)
        {
            return (code < 0 ? "-0x" : "0x") + Convert.ToString(Math.Abs(code), 16);
        }

    }
}