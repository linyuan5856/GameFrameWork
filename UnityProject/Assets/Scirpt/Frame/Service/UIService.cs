using System;
using System.Collections.Generic;
using System.Linq;
using GameFrameWork.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace GameFrameWork
{
    public class UIService : BaseService
    {
        public override void Create()
        {
            base.Create();
            if (_cacheRootCanvas == null)
            {
                _cacheRootCanvas = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("UI/UIRoot"));
                _cacheRootCanvas.name = "UIRoot";
                UnityEngine.Object.DontDestroyOnLoad(_cacheRootCanvas);
                _canvas = _cacheRootCanvas.transform.Find("Canvas").GetComponent<Canvas>();
                _canvas.GetComponent<GraphicRaycaster>().ignoreReversedGraphics = false;
                _msgCanvas = _cacheRootCanvas.transform.Find("MsgCanvas").GetComponent<Canvas>();
                _hud = _cacheRootCanvas.transform.Find("Canvas/Huds");
                _spaceCanvas = _cacheRootCanvas.transform.Find("Canvas_SpaceCamera").GetComponent<Canvas>();
            }
        }

        private const int OPEN_EVENT = 0;
        private const int CLOSE_EVENT = 1;

        private readonly Dictionary<string, GameObject> _uiCacheDict = new Dictionary<string, GameObject>();
        private readonly Dictionary<string, Boolean> _loadingUiDict = new Dictionary<string, bool>();
        private readonly List<UIWindow> _openedUiList = new List<UIWindow>();
        private readonly EventDispatcher<int, object> _eventDispatcher = new EventDispatcher<int, object>();
        private Canvas _canvas;
        private Canvas _msgCanvas;
        private Canvas _spaceCanvas;
        private Transform _hud;
        private GameObject _cacheRootCanvas;
        public Canvas Canvas => _canvas;

        public Canvas MsgCanvas => _msgCanvas;

        public Transform Hud => _hud;


        public void AddOpenListener(Action<object> callback)
        {
            _eventDispatcher.AddListener(OPEN_EVENT, callback);
        }

        public void RemoveOpenListener(Action<object> callback)
        {
            _eventDispatcher.RemoveListener(OPEN_EVENT, callback);
        }

        public void AddCloseListener(Action<object> callback)
        {
            _eventDispatcher.AddListener(CLOSE_EVENT, callback);
        }

        public void RemoveCloseListener(Action<object> callback)
        {
            _eventDispatcher.RemoveListener(CLOSE_EVENT, callback);
        }

        #region public Interface

        public void JumpWindow<T>(object _params = null)
        {
            InternalJumpWindow(typeof(T).Name, _params);
        }

        public void JumpWindow(string uiName, object _params = null)
        {
            InternalJumpWindow(uiName, _params);
        }

        public void GoBackWindow(string uiName = null)
        {
            InternalGoBackWindow(uiName);
        }

        public void OpenWindow<T>(object _param = null) where T : UIWindow
        {
            InternalOpenWindowByParam(typeof(T).Name, _param);
        }

        public void OpenWindow(string uiName, object _param = null)
        {
            InternalOpenWindowByParam(uiName, _param);
        }

        public bool IsWindowOpened<T>()
        {
            String uiName = typeof(T).Name;
            return InternalIsWindowOpened(uiName);
        }

        public void CloseAllWindow(bool record = false)
        {
            InternalCloseAllWindow(record);
        }

        public void CloseWindow<T>() where T : UIWindow
        {
            InternalClose(typeof(T).Name);
        }

        public void CloseWindow(string uiName)
        {
            InternalClose(uiName);
        }

        #endregion

        private bool InternalIsWindowOpened(string uiName)
        {
            return FindWindow(uiName) != null;
        }

        private void InternalCloseAllWindow(bool record = false, string toWindow = null)
        {
            if (record)
                RecordAllWindow(toWindow);

            for (int i = _openedUiList.Count - 1; i >= 0; i--)
            {
                UIWindow window = _openedUiList[i];
                if (window.UiType != UIType.other)
                    window.CloseWindow();
            }
        }

        private void RecordAllWindow(string toWindow = null)
        {
            if (_openedUiList.Count > 0)
            {
                HistoryNode node = new HistoryNode {ToWindow = toWindow};

                for (int i = _openedUiList.Count - 1; i >= 0; i--)
                {
                    UIWindow window = _openedUiList[i];
                    if (window.UiType != UIType.other)
                    {
                        node.WindowList.Add(window.name);
                        node.ParamList.Add(window.GetParam());
                    }
                }

                uiHistory.AddHistoryNode(node);
            }
        }

        private void InternalOpenWindowByParam(string uiName, object _param = null)
        {
            Debuger.Log("OpenWindow:" + uiName);
            if (_loadingUiDict.ContainsKey(uiName) && _loadingUiDict[uiName])
            {
                Debuger.LogWarning("ignore repeated ui load:" + uiName);
                return;
            }

            UIWindow openedWindow = FindWindow(uiName);
            if (openedWindow != null)
            {
                Debuger.Log("window opened ,just UpdateWindow:" + uiName);
                openedWindow.SetParam(_param);
                openedWindow.UpdateWindow();
                return;
            }

            if (_uiCacheDict.ContainsKey(uiName))
            {
                DirectOpenWindow(uiName, _uiCacheDict[uiName], _param);
                return;
            }

            _loadingUiDict[uiName] = true;
            LoaderService loader = ServiceLocate.Instance.GetService<LoaderService>();
            string path = "UI/" + uiName;
            GameObject go = loader.Instantiate(path, _canvas.transform, false);
            _loadingUiDict[uiName] = false;
            go.name = uiName;
            _uiCacheDict[uiName] = go;
            DirectOpenWindow(uiName, go, _param);
        }

        private UIWindow FindWindow(string uiName)
        {
            foreach (var t in _openedUiList)
            {
                if (t.name == uiName)
                    return t;
            }

            return null;
        }

        private void DirectOpenWindow(string uiName, GameObject go, object _param)
        {
            UIWindow t = go.GetComponent<UIWindow>();
            Debug.Assert(t != null, "ui script must inherit from UIWindow");
            t.ActionOpen = OnWindowOpen;
            t.ActionClose = OnWindowClose;
            t.InitWindow();
            t.SetParam(_param);
            t.OpenWindow();
        }

        private void OnWindowOpen(UIWindow window)
        {
            string windowName = window.name;
            if (_openedUiList.IndexOf(window) > -1)
            {
                Debuger.LogError(windowName + " has been opened");
            }
            else
            {
                _openedUiList.Add(window);
            }

            _eventDispatcher.DispatchEvent(OPEN_EVENT, windowName);
        }

        private void OnWindowClose(UIWindow window)
        {
            _openedUiList.Remove(window);
            _eventDispatcher.DispatchEvent(CLOSE_EVENT, window.name);
        }

        private void InternalClose(string uiName)
        {
            if (!string.IsNullOrEmpty(uiName)
                && _uiCacheDict.ContainsKey(uiName))
            {
                GameObject go = _uiCacheDict[uiName];
                UIWindow t = go.GetComponent<UIWindow>();
                t.CloseWindow();
            }
        }

        private void DestroyWindow<T>()
        {
            string uiName = typeof(T).Name;
            DestroyWindow(uiName);
        }

        private void DestroyWindow(string uiName)
        {
            if (_uiCacheDict.ContainsKey(uiName))
            {
                GameObject go = _uiCacheDict[uiName];
                _uiCacheDict.Remove(uiName);
                UnityEngine.Object.DestroyImmediate(go);
            }
        }

        public void DestroyAllWindow()
        {
            var windows = _uiCacheDict.Keys.ToList();
            for (int i = windows.Count - 1; i >= 0; i--)
            {
                DestroyWindow(windows[i]);
            }
        }

        private readonly UiHistory uiHistory = new UiHistory();

        private void InternalJumpWindow(string uiName, object _params)
        {
            if (_openedUiList.Count > 0)
            {
                if (_openedUiList[_openedUiList.Count - 1].name == uiName)
                {
                    Debuger.LogWarning("window opened at top,can not jump to :" + uiName);
                    return;
                }

                InternalCloseAllWindow(true, uiName);
            }

            InternalOpenWindowByParam(uiName, _params);
        }

        private void InternalGoBackWindow(string uiName)
        {
            HistoryNode node = uiHistory.PopNode(uiName);

            InternalClose(uiName);
            if (node != null)
            {
                for (int i = node.WindowList.Count - 1; i >= 0; i--)
                {
                    string windowName = node.WindowList[i];
                    object param = node.ParamList[i];
                    Debuger.Log("go back to window:" + windowName);
                    InternalOpenWindowByParam(windowName, param);
                }
            }
        }

        private class UiHistory
        {
            private readonly Stack<HistoryNode> _stack = new Stack<HistoryNode>();

            public void AddHistoryNode(HistoryNode node)
            {
                _stack.Push(node);
            }

            public HistoryNode PopNode(string uiName)
            {
                if (_stack.Count <= 0) return null;
                HistoryNode node = _stack.Peek();
                if (string.IsNullOrEmpty(uiName))
                    return _stack.Pop();
                if (node.ToWindow == uiName)
                    return _stack.Pop();
                Debuger.LogError("history node not match,there must be some logic mistake:" + uiName);
                //Logger.LogWarn("there is no history node");
                //_stack.Clear();
                return null;
            }

            public void Clear()
            {
                _stack.Clear();
            }
        }

        private class HistoryNode
        {
            public readonly List<string> WindowList = new List<string>();
            public readonly List<object> ParamList = new List<object>();
            public string ToWindow;
        }
    }
}