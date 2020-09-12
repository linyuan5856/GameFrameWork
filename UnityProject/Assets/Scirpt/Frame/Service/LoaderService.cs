using System;
using GameFrameWork;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
#if UNITY_EDITOR
using UnityEditor;

#endif
public partial class LoaderService : BaseService
{
    private const string AtlasPathPrefix = "Atlas/";
    private const string SpritePathPrefix = "Sprite/";

    private readonly SceneLoader _sceneLoader = new SceneLoader();
    private readonly AssetLoader _assetLoader = new AssetLoader();

    public override void Create()
    {
        base.Create();
        _sceneLoader.RegisterSceneCallBack();
        SpriteAtlasManager.atlasRequested += OnAtlasRequested;
    }

    public override void Release()
    {
        base.Release();
        _assetLoader.Release();
        _sceneLoader.Release();
        _sceneLoader.UnRegisterSceneCallBack();
        SpriteAtlasManager.atlasRequested -= OnAtlasRequested;
    }

    public override void DoUpdate(float deltaTime)
    {
        base.DoUpdate(deltaTime);
        _sceneLoader.DoUpdate();
        _assetLoader.DoUpdate();
    }

    private UnityEngine.Object LoadAsset(string path)
    {
        return _assetLoader.LoadAsset(path);
    }

    private T LoadAsset<T>(string path) where T : UnityEngine.Object
    {
        return _assetLoader.LoadAsset<T>(path);
    }

    private void LoadAssetAsync(string path, Action<UnityEngine.Object> afterLoaded)
    {
        _assetLoader.LoadAssetAsync(path, afterLoaded);
    }

    private void LoadAssetAsync<T>(string path, Action<UnityEngine.Object> afterLoaded)
    {
        _assetLoader.LoadAssetAsync<T>(path, afterLoaded);
    }

    public GameObject Instantiate(string path)
    {
        return _assetLoader.Instantiate(path);
    }

    public GameObject Instantiate(string path, Vector3 pos, Quaternion rotate)
    {
        return _assetLoader.Instantiate(path, pos, rotate);
    }

    public GameObject Instantiate(string path, Transform parent, bool isInWorldSpace = false)
    {
        return _assetLoader.Instantiate(path, parent, isInWorldSpace);
    }

    public void InstantiateAsync(string path, Action<GameObject> afterLoaded)
    {
        _assetLoader.InstantiateAsync(path, afterLoaded);
    }

    public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        _sceneLoader.LoadScene(sceneName, mode);
    }

    public void LoadSceneAsync(string sceneName, AsyncSceneCallBack data = null,
        LoadSceneMode mode = LoadSceneMode.Single)
    {
        _sceneLoader.LoadSceneAsync(sceneName, data, mode);
    }
}