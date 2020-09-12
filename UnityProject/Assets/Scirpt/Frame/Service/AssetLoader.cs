using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameFrameWork;
using UnityEngine;
using Object = UnityEngine.Object;

public class AssetLoader
{
    private class LoaderData
    {
        private readonly string _path;
        private readonly Type _assetType;
        private readonly Action<Object> _callBack;
        private ResourceRequest _request;

        public LoaderData(string path, Action<Object> callBack, Type assetType)
        {
            _path = path;
            _callBack = callBack;
            _request = null;
            _assetType = assetType;
#if UNITY_EDITOR
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
#endif
        }

        public void SetAsyncRequest(ResourceRequest request)
        {
            _request = request;
        }

        public string Path => _path;

        public Action<Object> CallBack => _callBack;

        public ResourceRequest Request => _request;

        public Type AssetType => _assetType;

#if UNITY_EDITOR
        private Stopwatch _stopwatch;

        public Stopwatch Stopwatch => _stopwatch;
#endif
    }

    private readonly Queue<LoaderData> _waitForLoadQueue = new Queue<LoaderData>();
    private readonly List<LoaderData> _onLoadingList = new List<LoaderData>(MAXNUM);
    private int _currentLoadNum; //当前正在异步加载的obj数量
    private const int MAXNUM = 8; //最大加载数量

    public void Release()
    {
        _currentLoadNum = 0;
        _waitForLoadQueue.Clear();
        _onLoadingList.Clear();
    }

    public void DoUpdate()
    {
        while (_waitForLoadQueue.Count > 0 && _currentLoadNum <= MAXNUM)
        {
            var data = _waitForLoadQueue.Dequeue();
            var request = data.AssetType != null
                ? Resources.LoadAsync(data.Path, data.AssetType)
                : Resources.LoadAsync(data.Path);
            data.SetAsyncRequest(request);
            _onLoadingList.Add(data);
            _currentLoadNum++;
        }

        for (int i = _onLoadingList.Count - 1; i >= 0; i--)
        {
            var data = _onLoadingList[i];
            var loader = data.Request;

            if (loader.isDone)
            {
                data.CallBack?.Invoke(loader.asset);
                _onLoadingList.RemoveAt(i);
                _currentLoadNum--;
#if UNITY_EDITOR
                data.Stopwatch.Stop();
                var time = data.Stopwatch.ElapsedMilliseconds;
                if (loader.asset == null)
                    Log($" Load Async Asset-> {data.Path}  asset is Null", Color.red);
                else
                    Log($" Load Async Asset-> {data.Path}  cost Time->{time} Milliseconds", Color.green);
#endif
            }
        }
    }

    public Object LoadAsset(string path)
    {
        return LoadAsset<Object>(path);
    }

    public T LoadAsset<T>(string path) where T : Object
    {
#if UNITY_EDITOR
        Stopwatch stopwatch = Stopwatch.StartNew();
#endif
        var asset = Resources.Load(path, typeof(T)) as T;
        UnityEngine.Debug.Assert(asset != null, $"Load Asset {path} is Null");
#if UNITY_EDITOR
        var time = stopwatch.ElapsedMilliseconds;
        Log($" Load Asset-> {path}  cost Time->{time} Milliseconds", Color.magenta);
#endif
        return asset;
    }

    public void LoadAssetAsync(string path, Action<Object> afterLoaded)
    {
        LoadAssetAsync<Object>(path, afterLoaded);
    }

    public void LoadAssetAsync<T>(string path, Action<Object> afterLoaded)
    {
        if (afterLoaded == null)
        {
            Debuger.LogError($"try to load async but set a null action,asset path-> {path}");
            return;
        }

        LoaderData data = new LoaderData(path, afterLoaded, typeof(T));
        _waitForLoadQueue.Enqueue(data);
    }

    public GameObject Instantiate(string path)
    {
        var asset = LoadAsset(path);

        return Internal_Instantiate(asset);
    }

    public GameObject Instantiate(string path, Vector3 pos, Quaternion rotate)
    {
        var asset = LoadAsset(path);
        return Internal_Instantiate(asset, pos, rotate);
    }

    public GameObject Instantiate(string path, Transform partent, bool isInWorldSpace)
    {
        var asset = LoadAsset(path);
        return Object.Instantiate(asset, partent, isInWorldSpace) as GameObject;
    }

    private GameObject Internal_Instantiate(Object asset)
    {
        return Object.Instantiate(asset) as GameObject;
    }

    private GameObject Internal_Instantiate(Object asset, Vector3 pos, Quaternion rotate)
    {
        return Object.Instantiate(asset, pos, rotate) as GameObject;
    }

    public void InstantiateAsync(string path, Action<GameObject> afterLoaded)
    {
        if (afterLoaded == null)
        {
            Debuger.LogWarning($"Load Object Async With a null callBack");
        }

        LoadAssetAsync(path, asset =>
        {
            var go = Internal_Instantiate(asset);
            afterLoaded?.Invoke(go);
        });
    }


    private void Log(string msg, Color color)
    {
        //Debuger.Log(newMsg);
    }
}