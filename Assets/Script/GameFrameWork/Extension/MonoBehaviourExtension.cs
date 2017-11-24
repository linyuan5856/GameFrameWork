using UnityEngine;

public static class MonoBehaviourExtension
{
    //path
    public static string GetPath(this MonoBehaviour behavior)
    {
        return behavior.gameObject.GetPath();
    }

    //order
    public static int GetZOrder(this MonoBehaviour behavior)
    {
        return behavior.transform.GetSiblingIndex();
    }

    public static void SetZOrder(this MonoBehaviour behavior, int index)
    {
        behavior.gameObject.SetZOrder(index);
    }

    public static void IncZOrder(this MonoBehaviour behavior)
    {
        behavior.gameObject.IncZOrder();
    }

    public static void DecZOrder(this MonoBehaviour behavior)
    {
        behavior.gameObject.DecZOrder();
    }

    public static void SetFirstZOrder(this MonoBehaviour behavior)
    {
        behavior.transform.SetAsFirstSibling();
    }

    public static void SetLastZOrder(this MonoBehaviour behavior)
    {
        behavior.transform.SetAsLastSibling();
    }


    public static void SetParent(this MonoBehaviour behavior, GameObject parent, bool worldPositionStays = true)
    {
        behavior.gameObject.SetParent(parent, worldPositionStays);
    }

    public static void SetParent(this MonoBehaviour behavior, Transform parent, bool worldPositionStays = true)
    {
        behavior.gameObject.SetParent(parent, worldPositionStays);
    }

    public static void AddChild(this MonoBehaviour behavior, GameObject child, bool worldPositionStays = true)
    {
        behavior.gameObject.AddChild(child, worldPositionStays);
    }

    public static void AddChild(this MonoBehaviour behavior, Transform child, bool worldPositionStays = true)
    {
        behavior.gameObject.AddChild(child, worldPositionStays);
    }

    public static int RemoveAllChildren(this MonoBehaviour behavior)
    {
        return behavior.gameObject.RemoveAllChildren();
    }

    public static void ResetTransform(this MonoBehaviour behavior)
    {
        behavior.gameObject.ResetTransform();
    }

    //Componet
    public static bool HasComponent<T>(this MonoBehaviour behavior) where T : Component
    {
        return behavior.gameObject.HasComponent<T>();
    }

    public static bool RemoveComponent<T>(this MonoBehaviour behavior) where T : Component
    {
        return behavior.gameObject.RemoveComponent<T>();
    }

    public static T GetOrAddComponent<T>(this MonoBehaviour behavior) where T : Component
    {
        return behavior.gameObject.GetOrAddComponent<T>();
    }

}