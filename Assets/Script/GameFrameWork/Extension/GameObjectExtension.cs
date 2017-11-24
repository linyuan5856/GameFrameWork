using UnityEngine;

public static class GameObjectExtension
{
    //path
    public static string GetPath(this GameObject go)
    {
        Transform transRoot = go.transform;
        string path = transRoot.name;

        while (transRoot.parent != null)
        {
            path = transRoot.parent.name + "/" + path;
            transRoot = transRoot.parent;
        }

        return path;
    }

    //zorder
    public static int GetZOrder(this GameObject go)
    {
        return go.transform.GetSiblingIndex();
    }

    public static void SetZOrder(this GameObject go, int index)
    {
        if (index < 0)
            index = 0;
        if (index >= go.transform.parent.childCount)
            index = go.transform.parent.childCount - 1;

        go.transform.SetSiblingIndex(index);
    }

    public static void IncZOrder(this GameObject go)
    {
        int index = go.transform.GetSiblingIndex() - 1;
        if (index < 0)
            index = 0;
        if (index >= go.transform.parent.childCount)
            index = go.transform.parent.childCount - 1;
        go.transform.SetSiblingIndex(index);
    }

    public static void DecZOrder(this GameObject go)
    {
        int index = go.transform.GetSiblingIndex() + 1;
        if (index < 0)
            index = 0;
        if (index >= go.transform.parent.childCount)
            index = go.transform.parent.childCount - 1;
        go.transform.SetSiblingIndex(index);
    }

    //Child
    public static bool HasChild(this GameObject go, GameObject child)
    {
        return child.transform.parent == (go.transform);
    }

    public static void SetParent(this GameObject go, GameObject parent, bool worldPositionStays = true)
    {
        go.transform.SetParent(parent.transform, worldPositionStays);
    }

    public static void SetParent(this GameObject go, Transform parent, bool worldPositionStays = true)
    {
        go.transform.SetParent(parent, worldPositionStays);
    }

    public static void AddChild(this GameObject go, GameObject child, bool worldPositionStays = true)
    {
        child.transform.SetParent(go.transform, worldPositionStays);
    }

    public static void AddChild(this GameObject go, Transform child, bool worldPositionStays = true)
    {
        child.SetParent(go.transform, worldPositionStays);
    }

    public static int RemoveAllChildren(this GameObject go)
    {
        int num = go.transform.childCount;
        for (int i = go.transform.childCount - 1; i >= 0; i--)
        {
            UnityEngine.Object.Destroy(go.transform.GetChild(i).gameObject);
        }
        return num;
    }

    public static void ResetTransform(this GameObject go)
    {
        go.transform.localPosition = new Vector3(0, 0, 0);
        go.transform.localRotation = new Quaternion(0, 0, 0, 1);
        go.transform.localScale = new Vector3(1, 1, 1);
    }

    //Componet
    public static bool HasComponent<T>(this GameObject go) where T : Component
    {
        return go.GetComponent<T>() != null;
    }

    public static bool RemoveComponent<T>(this GameObject go) where T : Component
    {
        Component component = go.GetComponent<T>();
        if (component != null)
        {
            UnityEngine.Object.Destroy(component);
            return true;
        }
        return false;
    }

    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        return component != null ? component : go.AddComponent<T>();
    }
}