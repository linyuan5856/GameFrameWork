using UnityEngine;

namespace Pandora
{
    /// <summary>
    /// 该类会读取txt文本后 反射字段赋值 不要乱加字段
    /// </summary>
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

        public string GetCdnBundleUrl()
        {
             string bundleUrl = cdnUrl + "/AssetBundles/v" + VersionManager.Instance.LocalVersion + "/";
           
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


        public  string GetGameConfigPath()
        {
            string configPath = string.Empty;

            string fileName = string.Empty;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            fileName = GameDefine.EDITOR_CONFIG;
#else
            fileName = GameDefine.RELEASE_CONFIG;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
           configPath =  Application.streamingAssetsPath+"/"+fileName;
#else
            configPath = "file:///" + Application.streamingAssetsPath + "/" + fileName;
#endif
            return configPath;
        }
    }
}
