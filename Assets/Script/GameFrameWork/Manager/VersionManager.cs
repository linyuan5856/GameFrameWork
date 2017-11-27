using System;
using System.Collections.Generic;
using UnityEngine;

namespace GFW
{
    public class VersionManager : Singleton<VersionManager>
    {
        //版本信息
        public int SeverVersion;

        public int LocalVersion;
        //
        private Dictionary<string, string> localVersionDict;
        private Dictionary<string, string> serverVersionDict;
        public void SetLocalVersion(string config)
        {
            localVersionDict = ConvertUtil.ToPDict(config);
            LocalVersion = int.Parse(localVersionDict["Version"]);
        }

        public void SetServerVersion(string config)
        {
            serverVersionDict = ConvertUtil.ToPDict(config);
            SeverVersion = int.Parse(serverVersionDict["Version"]);
        }

        public string getBundleMd5(string name)
        {
            return serverVersionDict[name];
        }

        public string GetBundlePath(string name)
        {
            string serverMd5 = serverVersionDict[name];

            string localMd5 = string.Empty;

            string pathUrl = string.Empty;
            if (localVersionDict.ContainsKey(name))
            {
                localMd5 = localVersionDict[name];

                if (localMd5 == serverMd5)
                {
                    pathUrl = GetStreamingPath();
                }
            }

            if (string.IsNullOrEmpty(pathUrl))
            {
                pathUrl = GameConfigPVO.Instance.getCDNBundleUrl();
            }

            pathUrl += "/" + name + "_" + serverMd5 + ".bundle";
            return pathUrl;
        }

        private string mPathUrl;
        public string GetStreamingPath()
        {
            if (string.IsNullOrEmpty(mPathUrl))
            {
#if UNITY_ANDROID && !UNITY_EDITOR
            mPathUrl = Application.streamingAssetsPath;
#else
                mPathUrl = "file:///" + Application.streamingAssetsPath;
#endif
            }

            return mPathUrl;
        }
    }


}
