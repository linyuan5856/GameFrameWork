using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Pandora
{
    public enum CacheType
    {
        Scene,
        Always,
        NotCache,
    }

    public enum AssetType
    {
        UI,
        Music,
        Table,
        Scene,
        Effect,
        AssetBundle,
    }
   
    public class LoaderManager : MonoSingleton<LoaderManager>, ISerializationCallbackReceiver
    {

        #region 序列化
        [SerializeField]
        List<string> _assetkey = new List<string>();
        [SerializeField]
        List<CacheAssetInfo> _assetvalue = new List<CacheAssetInfo>();
        [SerializeField]
        List<string> _abKey = new List<string>();
        [SerializeField]
        List<AssetBundle> _abValue = new List<AssetBundle>();

        public void OnBeforeSerialize()
        {
            _assetkey.Clear();
            _assetvalue.Clear();
            _abKey.Clear();
            _abValue.Clear();

            foreach (var asset in _assetCacheDict)
            {
                this._assetkey.Add(asset.Key);
                this._assetvalue.Add(asset.Value);
            }

            foreach (var ab in _abDict)
            {
                this._abKey.Add(ab.Key);
                this._abValue.Add(ab.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            this._assetCacheDict.Clear();
            this._abDict.Clear();

            for (int i = 0; i < this._assetkey.Count; i++)
            {
                this._assetCacheDict[this._assetkey[i]] = this._assetvalue[i];
            }

            for (int i = 0; i < this._abKey.Count; i++)
            {
                this._abDict[this._abKey[i]] = this._abValue[i];
            }

        }
        #endregion

        [Serializable]
        class CacheAssetInfo
        {
            public CacheAssetInfo(string name, Object asset, CacheType cacheType)
            {
                this.pathName = name;
                this.asset = asset;
                this.cacheType = cacheType;
                cacheTime = Time.time;
                lastUseTime = Time.time;
            }
            public string pathName;
            public Object asset;
            public CacheType cacheType;
            public float cacheTime;
            public float lastUseTime;
        }

        private readonly AssetDownLoader assetDownLoader = new AssetDownLoader();
        private readonly AssetBundleDownLoader abDownLoader = new AssetBundleDownLoader();


        private Dictionary<string, CacheAssetInfo> _assetCacheDict = new Dictionary<string, CacheAssetInfo>();
        private Dictionary<string, Object> _tableDict = new Dictionary<string, Object>();
        private Dictionary<string, AssetBundle> _abDict = new Dictionary<string, AssetBundle>();

      

        #region 加载 读取 Table
        public Object LoadTable(string tableName, AssetBundle ab = null, string key = "ID")
        {
            if (_tableDict.ContainsKey(tableName) && _tableDict[tableName] != null)
            {
                return _tableDict[tableName];
            }
            ScriptableObject asset = null;

            if (ab != null)
            {
                asset = (ScriptableObject)ab.LoadAsset(tableName);
            }

#if UNITY_EDITOR
            asset = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>(String.Format(GameDefine.TABLE_PATH, tableName));
            asset = Object.Instantiate(asset);//编辑下下直接修改数值会被编辑器保存
#endif

            if (asset is ITable)
            {
                ((ITable)asset).InitTable();
            }
            else if (asset is CSVFile)
            {
                ((CSVFile)asset).InitTable(key);
            }
            else
            {
                GameLogger.LogError(String.Format("load table {0} error", tableName));
            }
            if (_tableDict.ContainsKey(tableName))
            {
                _tableDict[tableName] = asset;
            }
            else
            {
                _tableDict.Add(tableName, asset);
            }
            return asset;
        }

        public T GetTable<T>() where T : Object
        {
            string tableName = typeof(T).ToString();
            if (_tableDict.ContainsKey(tableName))
            {
                return (T)_tableDict[tableName];
            }

            GameLogger.LogError("can not find table data:" + tableName);
            return default(T);
        }
        #endregion

        #region 加载资源
        string GetAssetPath(string name, AssetType assettype)
        {
            string path;
            if (assettype == AssetType.UI)
            {
                path = GameUtil.AddString("UI/", name);
            }
            else
            {
                path = name;
            }
            return path;
        }

        public T LoadAsset<T>(string name, AssetType assettype) where T : UnityEngine.Object
        {
            string path = GetAssetPath(name, assettype);

            return LoadAsset<T>(path);
        }

        public void LoadAssetAsync<T>(string name, AssetType assettype, object extraObject, Action<Object, object> onComplete = null)
        {
            string path = GetAssetPath(name, assettype);
            LoadAssetAsync<T>(path, null, extraObject, onComplete, null);
        }


        T LoadAsset<T>(string name, AssetBundle bundle = null, CacheType cacheType = CacheType.Scene) where T : UnityEngine.Object
        {
            string cacheName = bundle != null ? bundle.GetInstanceID() + "+" + name : name;
            Object target = TryGetFromCache(cacheName);
            if (target != null)
            {
                return (T)target;
            }

            GameLogger.LogTest("LoadAsset:" + cacheName);

            T prefab = assetDownLoader.LoadAsset<T>(name, bundle);

            this.PushToCache(cacheName, prefab, cacheType);

            return prefab;
        }

        void LoadAssetAsync<T>(string name, AssetBundle ab, object extraObject = null, Action<Object, object> onComplete = null,
          Action<float, object> onLoading = null,
          CacheType cacheType = CacheType.Scene)
        {
            string cacheName = ab != null ? ab.GetInstanceID() + "+" + name : name;
            Object target = TryGetFromCache(cacheName);
            if (target != null)
            {
                if (onComplete != null)
                {
                    onComplete(target, extraObject);
                }
                return;
            }

            GameLogger.LogTest("LoadAssetAsync:" + name);
            assetDownLoader.LoadAsync<T>(ab, name, extraObject, (asset, extraObj2) =>
            {
                this.PushToCache(cacheName, asset, cacheType);
                if (onComplete != null)
                {
                    onComplete(asset, extraObj2);
                }
            }, onLoading);
        }

        void PushToCache(string cacheName, Object asset, CacheType cacheType)
        {
            if (asset != null && cacheType != CacheType.NotCache)
                _assetCacheDict[cacheName] = new CacheAssetInfo(cacheName, asset, cacheType);

            if (asset == null)
            {
                GameLogger.LogError("LoadAsset error : can't found prefab: " + cacheName);
            }
        }

        Object TryGetFromCache(string name)
        {
            if (_assetCacheDict.ContainsKey(name))
            {
                CacheAssetInfo cacheAssetInfo = _assetCacheDict[name];
                cacheAssetInfo.lastUseTime = Time.time;
                return cacheAssetInfo.asset;
            }

            return null;
        }
        #endregion

        #region 登陆游戏预加载
        private int abLoadedCount = 0;
        private int abLoadedError = 0;

        private int assetLoadedCount = 0;
        private Action _afterAbLoaded;
        private Action _afterPreAssetLoaded;
        private Action _afterTableLoaded;

        //*******预载AssetBundle ***********//
        public void PreLoadGameAssetBundles(Action onComplete)
        {
            GameLogger.Log("Begin Load AssetBundles");

            this._afterAbLoaded = onComplete;

#if UNITY_EDITOR
            if (GameConfigPVO.Instance.EditorUseBundleConfig)
            {
                PreLoadAssetCatalog.AssetBundleList.Add(new PreLoadSourceInfo("configs", AssetType.AssetBundle));
            }
#else      
            PreLoadAssetCatalog.AssetBundleList.Add(new PreLoadSourceInfo("configs", AssetType.AssetBundle));
#endif

            abLoadedCount = 0;
            abLoadedError = 0;
            this.abDownLoader.onCompleteEvent.AddListener(this.OnAbLoadComplete);
            this.abDownLoader.onLoadEvent.AddListener(this.OnLoadingAb);
            foreach (PreLoadSourceInfo si in PreLoadAssetCatalog.AssetBundleList)
            {
                abDownLoader.DownLoadAB(si.AssetName, si);
            }
        }

        private void OnLoadingAb(float progress, System.Object extraParam)
        {
              PreLoadSourceInfo info = (PreLoadSourceInfo)extraParam;

            _logProgressDict[info.AssetName] = progress;
            //if (_logProgressDict.ContainsKey(info.AssetName))
            //{
                
            //}

            // GameLogger.Log(string.Format("Load Bundle Name--<{0}  ... Progress --> {1}",info.AssetName,progress));
        }
        Dictionary<string,float>_logProgressDict=new Dictionary<string, float>();

        void OnGUI()
        {
            List<string>hasDoneAbName=new List<string>();
            foreach (var progress in _logProgressDict)
            {
                GUILayout.TextField("Loading ... "+progress.Key + " --> " + (progress.Value*100).ToString("F1")+"%");
                if (Mathf.Approximately(progress.Value,1f))
                {
                    hasDoneAbName.Add(progress.Key);
                }
            }

            for (int i = 0; i < hasDoneAbName.Count; i++)
            {
                _logProgressDict.Remove(hasDoneAbName[i]);
            }          
        }

        private void OnAbLoadComplete(AssetBundle ab, AssetBundleDownLoader.EResult result, System.Object _param)
        {
            abLoadedCount++;
            PreLoadSourceInfo preLoadSourceInfo = (PreLoadSourceInfo)_param;
            //Logger.LogTest(string.Format("DonwLoad {0} Complete Count -->{1}",preLoadSourceInfo.AssetName,abLoadedCount));

            if (result == AssetBundleDownLoader.EResult.Success)
            {
                //if (!this.bundleDict.ContainsKey(preLoadSourceInfo.AssetName))
                //{
                this._abDict[preLoadSourceInfo.AssetName] = ab;
                //}

                if (abLoadedCount == PreLoadAssetCatalog.AssetBundleList.Count)
                {
                    if (abLoadedError > 0)
                    {
                        GameLogger.LogError("not enough memory or storage space.");
                        PreLoadGameAssetBundles(this._afterAbLoaded);
                    }
                    else
                    {
                        GameLogger.Log("AssetBundles DownLoad Done");
                        this.abDownLoader.onCompleteEvent.RemoveAllListeners();
                        this.abDownLoader.onLoadEvent.RemoveAllListeners();
                        if (this._afterAbLoaded != null)
                        {
                            this._afterAbLoaded();
                            this._afterAbLoaded = null;
                        }
                    }
                }
            }
            else
            {
                abLoadedError++;
            }
        }

        //*******预载游戏资源 ***********//
        public void PreLoadGameAssets(Action callback)
        {
            GameLogger.Log("Begin Load PreLoadAsset");
            this._afterPreAssetLoaded = callback;
            if (PreLoadAssetCatalog.AssetList.Count == 0 && this._afterPreAssetLoaded != null)
            {
                _afterPreAssetLoaded();
                _afterPreAssetLoaded = null;
            }

            for (int i = 0; i < PreLoadAssetCatalog.AssetList.Count; i++)
            {
                PreLoadSourceInfo preLoadSourceInfo = PreLoadAssetCatalog.AssetList[i];
                LoadAssetAsync<GameObject>(preLoadSourceInfo.AssetName, null, preLoadSourceInfo,
                    OnAssetLoadComplete, null, CacheType.Always);
            }
        }

        private void OnAssetLoadComplete(UnityEngine.Object o, object extraParam)
        {
            assetLoadedCount++;
            if (assetLoadedCount == PreLoadAssetCatalog.AssetList.Count)
            {
                GameLogger.Log("Load PreLoadAsset Done");
                assetLoadedCount = 0;
                if (_afterPreAssetLoaded != null)
                {
                    _afterPreAssetLoaded();
                    _afterPreAssetLoaded = null;
                }
            }
        }

        //*******预载CSV配置文件 ***********//
        public void PreLoadTable(Action onComplete)
        {
            this._afterTableLoaded = onComplete;
            StartCoroutine(CO_PreLoadTable());
        }

        IEnumerator CO_PreLoadTable()
        {
            int totalCount = PreLoadAssetCatalog.TableList.Count;
            for (int i = 0; i < totalCount; i++)
            {
                string tableName = PreLoadAssetCatalog.TableList[i];
                GameLogger.Log("load table:" + tableName);
                long time = DateTime.Now.Ticks;
                LoaderManager.Instance.LoadTable(tableName);
                long costTime = (DateTime.Now.Ticks - time) / 10000;
                if (costTime > 10)
                    GameLogger.LogWarn(tableName + " cost time:" + costTime);
                yield return null;
            }

            // ConvertUtil.NewCSVToStaticClass((NewCSVFile)LoaderManager.Instance.LoadTable("ActivityBase", "Key"), typeof(CSV_ActivityBase));
            GameLogger.Log("Load Table Done");

            if (this._afterTableLoaded != null)
            {
                this._afterTableLoaded();
                this._afterTableLoaded = null;
            }
        }

        #endregion

        #region 加载Scene

        private string _loadSceneName;
        private Action _loadSceneComplete;
        private Action<float> _loadingScene;
        public void LoadSceneAsync(string scene, Action<float> onLoading, Action onComplete)
        {
            _loadSceneName = scene;
            _loadingScene = onLoading;
            _loadSceneComplete = onComplete;
            this.StartCoroutine(LoadScene());
        }

        IEnumerator LoadScene()
        {
            GameLogger.LogWarn(string.Format("Load Scene --> {0}", _loadSceneName));

            AsyncOperation ao = SceneManager.LoadSceneAsync(_loadSceneName);
            if (ao != null)
            {
                ao.allowSceneActivation = false;

                while (true)
                {
                    if (_loadingScene != null)
                    {
                        _loadingScene(ao.progress);
                    }
                    yield return ao.isDone;
                    break;
                }

                GameLogger.LogWarn(string.Format("Scene --> {0} is Loaded", _loadSceneName));

                if (_loadSceneComplete != null)
                {
                    _loadSceneComplete();
                }

                ao.allowSceneActivation = true;
                _loadingScene = null;
                _loadSceneComplete = null;
            }
        }

        #endregion

        bool HasLoadedAb(string abName)
        {
            return this._abDict.ContainsKey(abName);
        }     

        public void ClearCache(string name, AssetBundle assetBundle)
        {
            string cacheName = assetBundle != null ? assetBundle.name + "+" + name : name;
            if (_assetCacheDict.ContainsKey(cacheName))
            {
                _assetCacheDict.Remove(cacheName);
            }
        }

        public void ClearCache(CacheType cacheType)
        {
            CacheAssetInfo[] cacheAssetArr = _assetCacheDict.Values.ToArray();
            int assetCount = cacheAssetArr.Length;
            for (int i = 0; i < assetCount; i++)
            {
                if (cacheAssetArr[i].cacheType == cacheType)
                {
                    _assetCacheDict.Remove(cacheAssetArr[i].pathName);
                }
            }
        }

        public static void HttpGetText(string url, Action<string, string> callback)
        {
            MainGame.Instance.StartCoroutine(http_get_text(url, callback));
        }

        static IEnumerator http_get_text(string url, Action<string, string> callback)
        {
            WWW www = new WWW(url);
            while (!www.isDone)
            {
                yield return www;

                var result = www.text;

                if (callback != null)
                    callback(result, www.error);
            }
            www.Dispose();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            this.assetDownLoader.UpdateLoad();
            this.abDownLoader.UpdateLoad();
        }


    }

    public sealed class AssetBundleDownLoader
    {
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
            public bool Stop;
            public System.Object ExtraParam;

            public ABDownloadInfo(string abName, System.Object extraParam)
            {
                ABName = abName;
                ExtraParam = extraParam;
            }
        }

        private const int MAXCOUNT = 3;

        private List<ABDownloadInfo> processingDownloads = new List<ABDownloadInfo>();
        private List<ABDownloadInfo> pendingDownloads = new List<ABDownloadInfo>();

        public class OnLoadEvent : UnityEvent<float, System.Object> { }
        public class OnCompleteEvent : UnityEvent<AssetBundle, EResult, System.Object> { }

        public readonly OnLoadEvent onLoadEvent = new OnLoadEvent();
        public readonly OnCompleteEvent onCompleteEvent = new OnCompleteEvent();


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

        public void DownLoadAB(string abName, System.Object extraObj = null)
        {
            ABDownloadInfo info = processingDownloads.Find(a => a.ABName == abName) ?? pendingDownloads.Find(a => a.ABName == abName);

            if (info == null)
            {
                info = new ABDownloadInfo(abName, extraObj);
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


        private void StartDownLoadAB(ABDownloadInfo info)
        {
            string url = VersionManager.Instance.GetBundlePath(info.ABName);
            GameLogger.Log("StartDownLoadAB:" + info.ABName + ":" + url);


            if (info.www != null)
            {
                info.www.Dispose();
            }


            info.www = WWW.LoadFromCacheOrDownload(url,0);

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
            GameLogger.Log("download success:" + info.ABName);
            try
            {
                if (onCompleteEvent != null)
                    onCompleteEvent.Invoke(info.www.assetBundle, EResult.Success, info.ExtraParam);

                info.www.Dispose();
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
                GameLogger.LogError("Failed to download " + info.ABName + ", " + info.www.error + " retry" + info.Retry);
                return;
            }
            GameLogger.LogError("Failed to download " + info.ABName + ", " + info.www.error);
            onCompleteEvent.Invoke(null, EResult.Failed, info.ExtraParam);
        }

        private void DownLoadStop(ABDownloadInfo info)
        {
            GameLogger.LogTest("Failed to download " + info.ABName + ", Stop.");
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
                            GameLogger.LogError(string.Format("asset load error {0}", loadInfo.AssetName));

                        try
                        {
                            loadInfo.OnComplete(asset, loadInfo.ExtraObj);
                        }
                        catch (Exception ex)
                        {
                            GameLogger.LogError("asset load callback error:" + loadInfo.AssetName + " \n" + ex.ToString());
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
