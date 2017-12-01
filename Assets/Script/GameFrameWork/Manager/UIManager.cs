using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pandora
{
    public class UIManager : MonoSingleton<UIManager>
    {
        private readonly Dictionary<string, UIWindow> _cacheUiDict = new Dictionary<string, UIWindow>();
        private readonly Dictionary<string, Boolean> _loadingUiDict = new Dictionary<string, bool>();
        private readonly Dictionary<string, UIWindow> _openedUiDict = new Dictionary<string, UIWindow>();
        private readonly Dictionary<string, UICanvasController> _cacheCanvasDict = new Dictionary<string, UICanvasController>();

        private GameObject _uiRoot;

        private GameObject UiRoot
        {
            get
            {
                if (_uiRoot == null)
                {
                    _uiRoot = new GameObject("UiRoot");

                }
                return _uiRoot;
            }
        }

        private UIManager()
        {

        }

        public void OpenWindow<T>(params object[] param) where T : UIWindow
        {
            OpenWindowByParam(typeof(T).Name, param);
        }

        public void OpenWindow(string uiName, params object[] param)
        {
            OpenWindowByParam(uiName, param);
        }

        public bool HasWindowOpened<T>()
        {
            String ui = typeof(T).Name;
            return _openedUiDict.ContainsKey(ui);
        }

        public void CloseAllWindow()
        {
            CloseAllPopWindow();
        }

        public void CloseWindow<T>() where T : UIWindow
        {
            Instance.Close<T>();
        }

        public T FindWindow<T>() where T : UIWindow
        {
            string uiName = typeof(T).Name;
            if (_cacheUiDict.ContainsKey(uiName))
            {
                T t = _cacheUiDict[uiName] as T;
                if (t == null)
                {
                    GameLogger.LogError(String.Format("FindWindow {0} Exception", uiName));
                }
                return t;
            }
            return null;
        }


        private void OpenWindowByParam(string uiName, params object[] param)
        {
            GameLogger.Log("OpenWindow:" + uiName);
            if (_loadingUiDict.ContainsKey(uiName) && _loadingUiDict[uiName])
            {
                GameLogger.LogError("ignore repeated ui load:" + uiName);
                return;
            }

            if (_cacheUiDict.ContainsKey(uiName))
            {
                DirectOpenWindow(uiName, _cacheUiDict[uiName], param);
                return;
            }

            _loadingUiDict[uiName] = true;

            LoadWindow(uiName, window =>
            {
                _cacheUiDict[uiName] = window;
                DirectOpenWindow(uiName, window, param);
            }, param);
        }

        private void LoadWindow(string uiName, Action<UIWindow> callback, params object[] param)
        {
            LoaderManager.Instance.LoadAssetAsync<UnityEngine.Object>(uiName, AssetType.UI, null, (asset, extraObj) =>
            {
                _loadingUiDict[uiName] = false;
                if (asset != null)
                {
                    var canvasAsset = LoaderManager.Instance.LoadAsset<GameObject>(GameDefine.UI_Canvas, AssetType.UI);
                    var canvas = (GameObject)UnityEngine.Object.Instantiate(canvasAsset, UiRoot.transform);
#if UNITY_EDITOR
                    canvas.name = GameUtil.AddString("canvas_", uiName);
#endif
                    var controller = canvas.GetComponent<UICanvasController>();
                    this._cacheCanvasDict[uiName] = controller;

                    GameObject ui = (GameObject)UnityEngine.Object.Instantiate(asset, controller.transform);
                    ui.name = uiName;
                    var window = ui.GetComponent<UIWindow>();
                    window.transform.SetParent(controller.transform);
                    if (callback != null)
                    {
                        callback(window);
                    }
                }
            });
        }

        private void DirectOpenWindow(string name, UIWindow window, params object[] param)
        {
            SetCanvas(name, true);

            window.SetParamList(param);
            window.InitWindow();
            window.OpenWindow();
            if (_openedUiDict.ContainsKey(name))
            {
                GameLogger.LogWarn(name + " has been opened");
            }
            else
            {
                _openedUiDict.Add(window.name, window);
            }
        }

        private void DirectCloseWindow(UIWindow window)
        {
            window.CloseWindow();
            string uiname = window.name;
            SetCanvas(uiname, false);
            _openedUiDict.Remove(uiname);
        }

        private void CloseAllPopWindow()
        {
            if (_openedUiDict.Count > 0)
            {
                foreach (var window in new List<UIWindow>(_openedUiDict.Values))
                {
                    DirectCloseWindow(window);
                }
            }
        }

        private void SetCanvas(string name, bool enable)
        {
            if (this._cacheCanvasDict.ContainsKey(name))
            {
                this._cacheCanvasDict[name].SetEnable(enable);
            }
            else
            {
                GameLogger.LogError(string.Format("UI {0} 's Canvas Miss", name));
            }
        }


        private void Close<T>() where T : UIWindow
        {
            string uiName = typeof(T).Name;
            if (_cacheUiDict.ContainsKey(uiName))
            {
                var window = _cacheUiDict[uiName];
                DirectCloseWindow(window);
            }
        }

        private void DestroyWindow<T>()
        {
            string uiName = typeof(T).Name;
            if (_cacheUiDict.ContainsKey(uiName))
            {
                var window = _cacheUiDict[uiName];
                _cacheUiDict.Remove(uiName);

                DestroyImmediate(window.gameObject);
            }
        }

    }
}
