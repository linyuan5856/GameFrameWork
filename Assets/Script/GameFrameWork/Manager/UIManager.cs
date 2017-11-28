using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GFW
{
    public class UINewManager : MonoBehaviour
    {
        static private UINewManager _Instance;

        static private UINewManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    string path = "";//todo
                    GameObject cacheRootCanvas = (GameObject)GameObject.Instantiate(Resources.Load(path));
                    GameObject.DontDestroyOnLoad(cacheRootCanvas);
                    cacheRootCanvas.name = path;

                    _Instance = cacheRootCanvas.GetComponent<UINewManager>();
                    _Instance.overLayCanvas = cacheRootCanvas.transform.Find("Canvas").gameObject;
                    _Instance.mCacheRootGO = cacheRootCanvas;
                    _Instance.mEventSystem = cacheUIRoot.GetComponentInChildren<EventSystem>();                  
                    Logger.LogTest("UINewManager Create New Instance");
                }
                return _Instance;
            }
        }

        static public UINewManager CreateUIManager()
        {
            return Instance;
        }

        static public UINewManager TryGetInstance
        {
            get { return _Instance; }
        }

        static public GameObject OverLayCanvas
        {
            get
            {
                if (TryGetInstance != null)
                {
                    return TryGetInstance.overLayCanvas;
                }
                return null;
            }
        }

        static public GameObject CameraCanvas
        {
            get
            {
                if (TryGetInstance != null)
                {
                    return TryGetInstance.carmeraCanvas;
                }
                return null;
            }
        }

        static public Vector2 GetScreenSize()
        {
            RectTransform canvasTrans = (RectTransform)OverLayCanvas.transform;
            return canvasTrans.sizeDelta;
        }

        static public GameObject cacheUIRoot
        {
            get
            {
                if (TryGetInstance != null)
                {
                    return TryGetInstance.mCacheRootGO;
                }
                return null;
            }
        }

        void OnDestroy()
        {
            animatedMainUI = null;
            _Instance = null;
        }

        private GameObject mCacheRootGO;
        private GameObject overLayCanvas;
        private GameObject carmeraCanvas;
        private EventSystem mEventSystem;

        private Dictionary<string, GameObject> uiCacheDict = new Dictionary<string, GameObject>();
        private Dictionary<string, Boolean> loadingUIDict = new Dictionary<string, bool>();
        private Dictionary<string, UIWindow> openedUIDict = new Dictionary<string, UIWindow>();
        private Dictionary<string, UIWindow> allUIDict = new Dictionary<string, UIWindow>();

        static private Type animatedMainUI = null;

       

        static public void OpenWindow<T>(params object[] _param) where T : UIWindow
        {
           
            Instance.OpenWindowByParam(typeof(T).Name, _param);
        }
        static public void OpenWindow(string uiName, params object[] _param)
        {
          
            Instance.OpenWindowByParam(uiName, _param);
        }
        static public void JumpWindow<T>(params object[] _param) where T : UIWindow
        {         
            Instance.OpenWindowByParam(typeof(T).Name, _param);
        }

        static public bool ContainWindow(string uiName)
        {
            if (TryGetInstance != null)
            {
                return TryGetInstance.allUIDict.ContainsKey(uiName);
            }
            return false;
        }


        static public bool HasWindowOpened<T>()
        {
            if (TryGetInstance != null)
            {
                String ui = typeof(T).Name;
                return TryGetInstance.openedUIDict.ContainsKey(ui);
            }
            return false;
        }

        static public void CloseAllWindow()
        {
            if (TryGetInstance != null)
            {
                animatedMainUI = null;
                TryGetInstance.closeAllPopWindow();
            }
        }

        static public void CloseWindow<T>() where T : UIWindow
        {
            if (TryGetInstance != null)
                TryGetInstance.Close<T>();
            else
            {
               Logger.LogWarn("close failed because uinewmanager root donot exist");
            }
        }

        static public T FindWindow<T>() where T : UIWindow
        {
            if (TryGetInstance != null)
                return TryGetInstance.Find<T>();
            return null;
        }
        static public void SetEventSystemActive(bool active)
        {
            if (TryGetInstance != null)
                TryGetInstance.SetEventSystem(active);
        }

        private T Find<T>() where T : UIWindow
        {
            string uiName = typeof(T).Name;
            if (uiCacheDict.ContainsKey(uiName))
            {
                GameObject go = uiCacheDict[uiName];
                T t = go.GetComponent<T>();
                return t;
            }
            return null;
        }

        private void closeAllPopWindow()
        {
            if (openedUIDict.Count > 0)
            {
                foreach (var value in new List<UIWindow>(openedUIDict.Values))
                {
                    if (value.uiType != UIType.other)
                    {
                        value.CloseWindow();
                    }
                }
            }
        }
        private void OpenWindowByParam(string uiName, params object[] _param)
        {
           Logger.Log("OpenWindow:" + uiName);
            if (loadingUIDict.ContainsKey(uiName) && loadingUIDict[uiName] == true)
            {
                Logger.LogError("ignore repeated ui load:" + uiName);
                return;
            }

            if (uiCacheDict.ContainsKey(uiName))
            {
                DirectOpenWindow(uiName, uiCacheDict[uiName], _param);
                return;
            }

            loadingUIDict[uiName] = true;
            //todo
            //LoaderManager.Instance.LoadUIPrefabAysnc(uiName, (asset) =>
            //{
            //    loadingUIDict[uiName] = false;
            //    if (asset != null)
            //    {
            //        GameObject go = (GameObject)GameObject.Instantiate(asset);
            //        go.name = uiName;

            //        uiCacheDict[uiName] = go;
            //        DirectOpenWindow(uiName, go, _param);
            //    }
            //});
        }

        private void DirectOpenWindow(string uiName, GameObject go, params object[] _param)
        {
            //
            if (_Instance == null) return;
            Type type = Type.GetType(uiName);
            /////////
            UIWindow t = (UIWindow)go.GetComponent(type);
            t.SetParamList(_param);
            t.actionOpen = OnWindowOpen;
            t.actionClose = OnWindowClose;
            //
            t.InitWindow();
            t.OpenWindow();
        }

        private void OnWindowOpen(UIWindow window)
        {
            string windowName = window.name;
                 
            if (openedUIDict.ContainsKey(windowName))
            {
               Logger.LogWarn(windowName + " has been opened");
            }
            else
            {
                openedUIDict.Add(window.name, window);
            }
            if (!allUIDict.ContainsKey(windowName))
            {
                allUIDict.Add(windowName, window);
            }
        }

        private void OnWindowClose(UIWindow window)
        {
            openedUIDict.Remove(window.name);
        }

       

        private void Close<T>() where T : UIWindow
        {
            string uiName = typeof(T).Name;
            if (uiCacheDict.ContainsKey(uiName))
            {
                GameObject go = uiCacheDict[uiName];
                T t = go.GetComponent<T>();
                t.CloseWindow();
            }
        }

        private void DestoryWindow<T>()
        {
            string uiName = typeof(T).Name;
            if (uiCacheDict.ContainsKey(uiName))
            {
                GameObject go = uiCacheDict[uiName];
                uiCacheDict.Remove(uiName);

                GameObject.DestroyImmediate(go);
            }
        }

        private void SetEventSystem(bool active)
        {
            if (mEventSystem.enabled != active)
                mEventSystem.enabled = active;
        }

        //private void LoadUIAysnc<T>(Action<GameObject> callback)
        //{
        //    string uiName = typeof(T).Name;

        //}

        //


    }

}
