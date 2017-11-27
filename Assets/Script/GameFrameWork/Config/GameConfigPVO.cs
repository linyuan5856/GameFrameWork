using System;
using UnityEngine;

namespace GFW
{
    public class GameConfigPVO : Singleton<GameConfigPVO>
    {
        public bool usePerformance;
        public bool useRemoteLog;

        public int logLevel;
        public string logServerIp;
        public int logServerPort;

        public string cdnUrl;
        public bool useSDKLogin;

        public bool EditorUseBundleConfig;

        public string getCDNBundleUrl()
        {
           // string bundleUrl = cdnUrl + "/AssetBundles/v" + VersionManager.Instance.LocalVersion + "/";
            string bundleUrl = cdnUrl + "/AssetBundles/";


            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                bundleUrl += "Windows";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                bundleUrl += "Android";
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                bundleUrl += "IOS";
            }
            else
            {
                bundleUrl += "Windows";
            }
            return bundleUrl;
        }
    }
}
