using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace GFW
{
    public enum CacheType
    {
        Scene,
        Always,
        NotCache,
    }

    public class LoaderManager : MonoSingleton<LoaderManager>
    {
        class AssetInfo
        {
            public AssetInfo(string name, UnityEngine.Object asset, CacheType cacheType)
            {
                this.pathName = name;
                this.asset = asset;
                this.cacheType = cacheType;
                cacheTime = Time.time;
                lastUseTime = Time.time;
            }
            public string pathName;
            public UnityEngine.Object asset;
            public CacheType cacheType;
            public float cacheTime;
            public float lastUseTime;
        }

        private readonly AssetDownLoader assetDownLoader = new AssetDownLoader();
        private readonly AssetBundleDownLoader abDownLoader = new AssetBundleDownLoader();

        private Dictionary<string, AssetInfo> assetCacheDict = new Dictionary<string, AssetInfo>();
        private Dictionary<string, UnityEngine.Object> tableDict = new Dictionary<string, UnityEngine.Object>();
        private const string TABLEPATH = "Assets/_Generate/_Excel/{0}.asset";

        public UnityEngine.Object LoadTable(string tableName, AssetBundle ab = null, string key = "ID")
        {
            if (tableDict.ContainsKey(tableName) && tableDict[tableName] != null)
            {
                return tableDict[tableName];
            }
            UnityEngine.ScriptableObject asset = null;
            if (tableName == "LanguageLocal")
            {
                asset = (ScriptableObject)Resources.Load(tableName);
            }
            else
            {
                if (ab != null)
                {
                    asset = (ScriptableObject)ab.LoadAsset(tableName);
                }
                else
                {
#if UNITY_EDITOR
               asset=UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>(string.Format(TABLEPATH,tableName));
#endif
                }
            }

#if UNITY_EDITOR//编辑下下直接修改数值会被编辑器保存
        asset = UnityEngine.Object.Instantiate(asset);
#endif

            if (asset is ITable)
            {
                ((ITable)asset).InitTable();
            }
            else if (asset is NewCSVFile)
            {
                ((NewCSVFile)asset).InitTable(key);
            }
            else
            {
                Logger.LogError(string.Format("load table {0} error", tableName));
            }
            if (tableDict.ContainsKey(tableName))
            {
                tableDict[tableName] = asset;
            }
            else
            {
                tableDict.Add(tableName, asset);
            }
            return asset;
        }

        public T GetTable<T>() where T : UnityEngine.Object
        {
            string tableName = typeof(T).ToString();
            if (tableDict.ContainsKey(tableName))
            {
                return (T)tableDict[tableName];
            }

            Logger.LogError("can not find table data:" + tableName);
            return default(T);
        }

        #region 加载资源
        T LoadAsset<T>(string name, AssetBundle bundle = null, CacheType cacheType = CacheType.Scene) where T : UnityEngine.Object
        {
            string cacheName = bundle != null ? bundle.GetInstanceID() + "+" + name : name;
            UnityEngine.Object target = TryGetFromCache(cacheName);
            if (target != null)
            {
                return (T)target;
            }

            Logger.LogTest("LoadAsset:" + cacheName);

            T prefab = assetDownLoader.LoadAsset<T>(name, bundle);

            this.PushToCache(cacheName, prefab, cacheType);

            return prefab;
        }

        void LoadAssetAsync<T>(string name, AssetBundle ab, object extraObject = null, Action<UnityEngine.Object, object> onComplete = null,
          Action<float, object> onLoading = null,
          CacheType cacheType = CacheType.Scene)
        {
            string cacheName = ab != null ? ab.GetInstanceID() + "+" + name : name;
            UnityEngine.Object target = TryGetFromCache(cacheName);
            if (target != null)
            {
                if (onComplete != null)
                {
                    onComplete(target, extraObject);
                }
                return;
            }

            Logger.LogTest("LoadAssetAsync:" + name);
            assetDownLoader.LoadAsync<T>(ab, name, extraObject, (asset, extraObj2) =>
            {
                this.PushToCache(cacheName, asset, cacheType);
                if (onComplete != null)
                {
                    onComplete(asset, extraObj2);
                }
            }, onLoading);
        }

        void PushToCache(string cacheName, UnityEngine.Object asset, CacheType cacheType)
        {
            if (asset != null && cacheType != CacheType.NotCache)
                assetCacheDict[cacheName] = new AssetInfo(cacheName, asset, cacheType);

            if (asset == null)
            {
                Logger.LogError("LoadAsset error : can't found prefab: " + cacheName);
            }
        }

        UnityEngine.Object TryGetFromCache(string name)
        {
            if (assetCacheDict.ContainsKey(name))
            {
                AssetInfo assetInfo = assetCacheDict[name];
                assetInfo.lastUseTime = Time.time;
                return assetInfo.asset;
            }

            return null;
        }
        #endregion

        public void ClearCache(string name, AssetBundle assetBundle)
        {
            string cacheName = assetBundle != null ? assetBundle.name + "+" + name : name;
            if (assetCacheDict.ContainsKey(cacheName))
            {
                assetCacheDict.Remove(cacheName);
            }
        }

        public void ClearCache(CacheType cacheType)
        {
            AssetInfo[] assetArr = assetCacheDict.Values.ToArray();
            int assetCount = assetArr.Length;
            for (int i = 0; i < assetCount; i++)
            {
                if (assetArr[i].cacheType == cacheType)
                {
                    assetCacheDict.Remove(assetArr[i].pathName);
                }
            }
        }

        public void HttpGetText(string url, System.Action<string, string> callback)
        {
            this.StartCoroutine(this.http_get_text(url, callback));
        }

        IEnumerator http_get_text(string url, System.Action<string, string> callback)
        {
            WWW www = new WWW(url);
            while (!www.isDone)
            {
                yield return www;

                var result = www.text;
                if (callback != null)
                    callback(result, www.error);
            }
        }


        void Update()
        {
            this.assetDownLoader.UpdateLoad();
            this.abDownLoader.UpdateLoad();
        }
    }

    public sealed class AssetBundleDownLoader
    {
        private class WWWExist
        {
            public WWWExistType ExistType;
            public WWW www;
        }

        public enum WWWExistType
        {
            NotExist,
            AlwaysExist,
            ExistTime,
        }

        public enum EResult
        {
            Unknown = -1,
            Success,
            Failed,
            TimeOut,
            NotFound,
        }

        private class ABDownloadInfo
        {
            public string ABName;
            public WWW www = null;
            public int Retry = 10;
            public float BeginTick = 0;
            public int Version;
            public bool Stop;
            public System.Object ExtraParam;
            public WWWExistType ExistType = WWWExistType.NotExist;

            public ABDownloadInfo(string abName, WWWExistType existType, System.Object extraParam)
            {
                abName = abName;
                ExistType = existType;
                ExtraParam = extraParam;
            }
        }

        private const int MAXCOUNT = 3;

        private List<ABDownloadInfo> processingDownloads = new List<ABDownloadInfo>();
        private List<ABDownloadInfo> pendingDownloads = new List<ABDownloadInfo>();
        private Dictionary<string, WWWExist> abCache = new Dictionary<string, WWWExist>();

        private class OnLoadEvent : UnityEvent<float, System.Object> { }
        private class OnCompleteEvent : UnityEvent<WWW, EResult, System.Object> { }

        readonly OnLoadEvent onLoadEvent = new OnLoadEvent();
        readonly OnCompleteEvent onCompleteEvent = new OnCompleteEvent();


        public void UpdateLoad()
        {
            for (int i = 0; i < processingDownloads.Count;)
            {
                ABDownloadInfo info = processingDownloads[i];
                if (info.Stop)
                {
                    processingDownloads.Remove(info);
                    DownLoadStop(info);
                    continue;
                }

                if (!string.IsNullOrEmpty(info.www.error))
                {
                    processingDownloads.Remove(info);
                    onLoadEvent.Invoke(0, info.ExtraParam);
                    DownLoadFailed(info);
                    continue;
                }

                if (info.www.isDone)
                {
                    processingDownloads.Remove(info);
                    onLoadEvent.Invoke(1, info.ExtraParam);
                    DownLoadSuccess(info);
                    continue;
                }

                onLoadEvent.Invoke(info.www.progress, info.ExtraParam);
                i++;
            }
            if (pendingDownloads.Count > 0 && processingDownloads.Count < MAXCOUNT)
            {
                ABDownloadInfo info = pendingDownloads[0];
                pendingDownloads.RemoveAt(0);
                processingDownloads.Add(info);
                StartDownLoadAB(info);
            }
        }

        public void DownLoadAB(string shortURL, WWWExistType existType = WWWExistType.NotExist, System.Object extraObj = null)
        {
            WWWExist wwwExist;
            if (abCache.TryGetValue(shortURL, out wwwExist))
            {            
                onCompleteEvent.Invoke(wwwExist.www, EResult.Success, extraObj);                
                return;
            }
            ABDownloadInfo info = processingDownloads.Find(a => a.ABName == shortURL) ?? pendingDownloads.Find(a => a.ABName == shortURL);

            if (info != null)
            {
                if (info.www != null)
                {
                    info.www.threadPriority = ThreadPriority.Normal;
                }
            }
            else
            {
                info = new ABDownloadInfo(shortURL, existType, extraObj);
                if (processingDownloads.Count < MAXCOUNT)
                {
                    processingDownloads.Add(info);
                    StartDownLoadAB(info);
                }
                else
                {
                    pendingDownloads.Add(info);
                }
            }
        }

        public void UnloadCache(string path)
        {
            WWWExist wwwExist = null;
            if (abCache.TryGetValue(path, out wwwExist))
            {
                wwwExist.www.assetBundle.Unload(false);
                wwwExist.www.Dispose();
                abCache.Remove(path);
            }
        }

        public AssetBundle GetAssetBundle(string _path)
        {
            WWWExist wwwExist = null;
            if (abCache.TryGetValue(_path, out wwwExist))
            {
                return wwwExist.www.assetBundle;
            }
            return null;
        }

        private void StartDownLoadAB(ABDownloadInfo info)
        {
            string url = "";//VersionManager.Instance.getBundldPath(_info.shortURL);
            Logger.Log("StartDownLoadAB:" + info.ABName + ":" + url);

            info.BeginTick = Time.realtimeSinceStartup;

            if (info.www != null)
            {
                info.www.Dispose();
            }

            info.www = WWW.LoadFromCacheOrDownload(url, info.Version);

            if (info.www == null)
            {
                info.www = new WWW(url);
            }
            if (info.www != null)
            {
                info.www.threadPriority = ThreadPriority.Normal;
            }
        }

        private void DownLoadSuccess(ABDownloadInfo info)
        {
            Logger.Log("download success:" + info.ABName);
            try
            {
                if (info.ExistType == WWWExistType.AlwaysExist
                    || info.ExistType == WWWExistType.ExistTime)
                {
                    WWWExist wwwExist = new WWWExist();
                    wwwExist.ExistType = info.ExistType;
                    wwwExist.www = info.www;
                    abCache[info.ABName] = wwwExist;
                }

                try
                {
                    if (onCompleteEvent != null)
                        onCompleteEvent.Invoke(info.www, EResult.Success, info.ExtraParam);
                }
                catch (Exception ex)
                {
                    Logger.LogError("asset load callback error:" + info.ABName + " \n" + ex.ToString());
                }

                if (info.ExistType == WWWExistType.NotExist)
                {
                    if (info.www.assetBundle != null)
                    {
                        info.www.assetBundle.Unload(false);
                    }
                    info.www.Dispose();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        private void DownLoadFailed(ABDownloadInfo info)
        {
            if (info.Retry > 0)
            {
                info.Retry -= 1;
                pendingDownloads.Add(info);
                Logger.LogError("Failed to download " + info.ABName + ", " + info.www.error + " retry" + info.Retry);
                return;
            }
            Logger.LogError("Failed to download " + info.ABName + ", " + info.www.error);
            onCompleteEvent.Invoke(info.www, EResult.Failed, info.ExtraParam);
        }

        private void DownLoadStop(ABDownloadInfo info)
        {
            Logger.LogTest("Failed to download " + info.ABName + ", Stop.");
            info.www.Dispose();
        }
    }

    public sealed class AssetDownLoader
    {
        private List<AssetLoadInfo> processingAssetLoads = new List<AssetLoadInfo>();
        private Queue<AssetLoadInfo> pendingAssetLoads = new Queue<AssetLoadInfo>();
        private const int MAXCOUNT = 5;

        private class AssetLoadInfo
        {
            public Action<UnityEngine.Object, object> OnComplete = null;
            public Action<float, object> OnLoad = null;
            public AsyncOperation AsyncOperation;
            public String AssetName;
            public object ExtraObj;

            private AssetBundle _ab;
            private Type _type;

            public AssetLoadInfo(AssetBundle ab, Type type, Action<UnityEngine.Object, object> onComplete = null, Action<float, object> onLoad = null, object extraObj = null)
            {
                this._ab = ab;
                this._type = type;
                this.OnComplete = onComplete;
                this.OnLoad = onLoad;
                this.ExtraObj = extraObj;
            }

            public void StartLoad()
            {
                if (_ab != null)
                    AsyncOperation = _ab.LoadAssetAsync(AssetName, _type);
                else
                    AsyncOperation = Resources.LoadAsync(AssetName, _type);
            }
        }

        public T LoadAsset<T>(string name, AssetBundle bundle = null) where T : UnityEngine.Object
        {
            T prefab = bundle == null ? UnityEngine.Resources.Load<T>(name) : bundle.LoadAsset<T>(name);

            return prefab;
        }

        public void LoadAsync<T>(AssetBundle ab, string name, object extraObj = null,
        Action<UnityEngine.Object, object> onComplete = null,
        Action<float, object> onProgressUpdate = null)
        {
            AssetLoadInfo loadInfo = new AssetLoadInfo(ab, typeof(T), onComplete, onProgressUpdate, extraObj);
            loadInfo.AssetName = name;

            pendingAssetLoads.Enqueue(loadInfo);
        }

        public void UpdateLoad()
        {
            for (int i = 0; i < processingAssetLoads.Count;)
            {
                AssetLoadInfo loadInfo = processingAssetLoads[i];
                if (loadInfo.OnLoad != null)
                    loadInfo.OnLoad(loadInfo.AsyncOperation.progress, loadInfo.ExtraObj);

                if (loadInfo.AsyncOperation.isDone)
                {
                    processingAssetLoads.RemoveAt(i);
                    if (loadInfo.OnComplete != null)
                    {
                        UnityEngine.Object asset = null;
                        if (loadInfo.AsyncOperation is AssetBundleRequest)
                            asset = ((AssetBundleRequest)loadInfo.AsyncOperation).asset;
                        else if (loadInfo.AsyncOperation is ResourceRequest)
                            asset = ((ResourceRequest)loadInfo.AsyncOperation).asset;

                        if (asset == null)
                            Logger.LogError(string.Format("asset load error {0}", loadInfo.AssetName));

                        try
                        {
                            loadInfo.OnComplete(asset, loadInfo.ExtraObj);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("asset load callback error:" + loadInfo.AssetName + " \n" + ex.ToString());
                        }
                    }
                    continue;
                }
                i++;
            }
            while (pendingAssetLoads.Count > 0 && processingAssetLoads.Count < MAXCOUNT)
            {
                AssetLoadInfo loadInfo = pendingAssetLoads.Dequeue();
                loadInfo.StartLoad();
                processingAssetLoads.Add(loadInfo);
            }
        }
    }
}
