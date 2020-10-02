using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DemoBootStrap : MonoBehaviour
{
    public AssetReference go;
    private GameObject _prefab;

    void Start()
    {
        go.InstantiateAsync(new Vector3(0, 1, 0), Quaternion.identity).Completed += OnLoadAssetDone;
    }

    private void OnLoadAssetDone(AsyncOperationHandle<GameObject> obj)
    {
        Debug.LogWarning($"load {obj.Result} done");
        _prefab = obj.Result;
        _prefab.AddComponent<PlayerMoveController>();
    }

    void Update()
    {
        if (Keyboard.current.qKey.isPressed)
            go.ReleaseInstance(_prefab);
    }
}