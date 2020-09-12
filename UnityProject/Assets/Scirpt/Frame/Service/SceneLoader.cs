using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameFrameWork;

public class AsyncSceneCallBack
{
    public Action LoadedCallBack;
    public Action<float> ProgressCallBack;
    public float Progress;
}

public class SceneLoader
{
    public void RegisterSceneCallBack()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnLoad;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    public void UnRegisterSceneCallBack()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnLoad;
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    public void Release()
    {
        _asyncRecordDic.Clear();
        _removeSceneList.Clear();
    }

    public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        SceneManager.LoadScene(sceneName, mode);
    }


    readonly Dictionary<string, AsyncSceneCallBack> _asyncRecordDic = new Dictionary<string, AsyncSceneCallBack>();
    readonly List<string> _removeSceneList = new List<string>();

    public void LoadSceneAsync(string sceneName, AsyncSceneCallBack data = null,
        LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (_asyncRecordDic.ContainsKey(sceneName))
            Debuger.LogError($"repeat load scene  {sceneName}");
        else
            _asyncRecordDic.Add(sceneName, data);

        var async = SceneManager.LoadSceneAsync(sceneName, mode);
        async.completed += operation =>
        {
            if (_asyncRecordDic.ContainsKey(sceneName))
            {
                var recordData = _asyncRecordDic[sceneName];
                recordData.Progress = 0.9f;
            }

            _removeSceneList.Add(sceneName);
        };
        MonoHelper.GlobalStartCoroutine(LoadSceneAsync(sceneName, async));
    }


    public void DoUpdate()
    {
        if (_removeSceneList.Count <= 0) return;
        for (int i = _removeSceneList.Count - 1; i >= 0; i--)
        {
            var name = _removeSceneList[i];
            if (_asyncRecordDic.ContainsKey(name))
            {
                var data = _asyncRecordDic[name];
                if (Mathf.Approximately(data.Progress, 1.1f))
                {
                    data.LoadedCallBack?.Invoke();
                    _asyncRecordDic.Remove(name);
                    _removeSceneList.RemoveAt(i);
                }

                data.Progress = Mathf.Lerp(data.Progress, 1.1f, Time.deltaTime * 10);
                data.ProgressCallBack?.Invoke(data.Progress);
            }
            else
            {
                Debuger.LogError($"Error->加载的场景 {name} 找不到对应的参数");
                _removeSceneList.RemoveAt(i);
            }
        }
    }

    IEnumerator LoadSceneAsync(string sceneName, AsyncOperation async)
    {
        Action<float> progressAction = null;
        if (_asyncRecordDic.ContainsKey(sceneName))
        {
            var data = _asyncRecordDic[sceneName];
            progressAction = data.ProgressCallBack;
        }

        while (!async.isDone || async.progress <= 0.9f)
        {
            progressAction?.Invoke(async.progress);
            yield return null;
        }
    }


    private void OnActiveSceneChanged(Scene lastScene, Scene activeScene)
    {
        //Log($"[SceneManager  OnActiveSceneChanged] [LastScene] ->{lastScene.name} " +
        //    $" [ActiveScene]-> {activeScene.name} ");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Log($"[SceneManager] [Loaded] ->{scene.name}");
    }

    private void OnSceneUnLoad(Scene scene)
    {
        //Log($"[SceneManager] [UnLoaded] ->{scene.name} ");
    }


    void Log(string msg)
    {
        Debuger.Log(msg);
    }
}