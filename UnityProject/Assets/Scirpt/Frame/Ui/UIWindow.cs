using System;
using UnityEngine;

public enum UIType
{
    none = 0,
    other = 2,
}

public class UIWindow : MonoBehaviour
{
    public Action<UIWindow> ActionOpen;
    public Action<UIWindow> ActionClose;
    private bool _isInit;
    private UIType uIType = UIType.none;
    private string _uiName;
    private object _param;

    public UIType UiType => uIType;
    public string UiName => _uiName;

    void OnDestroy()
    {
        OnUnRegisterWindow();
        OnDestroyWindow();
    }


    public void SetParam(object param)
    {
        _param = param;
        OnSetParam(param);
    }

    public object GetParam()
    {
        return _param;
    }

    public void InitWindow()
    {
        if (_isInit) return;
        _isInit = true;
        _uiName = gameObject.name;
        OnInitWindow();
    }

    public void OpenWindow()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        //transform.SetAsLastSibling();
        ActionOpen?.Invoke(this);
        OnRegisterWindow();
        OnOpenWindow();
    }

    public void UpdateWindow()
    {
        //transform.SetAsLastSibling();
        OnUpdateWindow();
    }

    public void CloseWindow()
    {
        ActionClose?.Invoke(this);

        OnUnRegisterWindow();
        OnCloseWindow();

        if (gameObject != null)
            gameObject.SetActive(false);
    }


    protected virtual void OnSetParam(object _param)
    {
    }

    protected virtual void OnInitWindow()
    {
    }

    protected virtual void OnRegisterWindow()
    {
    }

    protected virtual void OnUnRegisterWindow()
    {
    }

    protected virtual void OnOpenWindow()
    {
    }

    protected virtual void OnUpdateWindow()
    {
    }

    protected virtual void OnCloseWindow()
    {
    }

    protected virtual void OnDestroyWindow()
    {
    }
}