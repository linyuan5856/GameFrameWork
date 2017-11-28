using System;
using UnityEngine;
using UnityEngine.UI;

namespace GFW
{
    public class UIWindow : MonoBehaviour
    {
        protected UIParentEnum mParentType = UIParentEnum.BaseCameraCanvas;
        private bool mInited = false;

        protected GameObject mCloseBtn;
        public Action<UIWindow> actionOpen;
        public Action<UIWindow> actionClose;

        private UIType mUIType = UIType.none;

        public UIType uiType
        {
            get { return mUIType; }
            set { mUIType = value; }
        }

        public UIParentEnum parentType
        {
            get { return mParentType; }
        }

        void OnDestroy()
        {
            try
            {
                OnUnRegistWindow();
                OnDestoryWindow();
            }
            catch (Exception ex)
            {
               Logger.LogError(ex.ToString());
            }
        }

        public void SetParamList(params object[] objects)
        {
            OnSetParamList(objects);
        }

        public void InitWindow()
        {
            if (mInited) return;
            mInited = true;
            Transform closeTrans = transform.Find("button__close");
            if (closeTrans != null)
            {
                mCloseBtn = closeTrans.gameObject;
                UGUI_EventListener.Get(mCloseBtn).onClick = OnClickClose;
            }

            OnSetParent();
            OnInitWindow();
        }
        public void OpenWindow()
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            transform.SetAsLastSibling();

            if (actionOpen != null)
                actionOpen(this);

            OnRegistWindow();    
            OnOpenWindow();
        }

        public void CloseWindow()
        {
           
            if (actionClose != null)
                actionClose(this);

            OnUnRegistWindow();
            OnCloseWindow();

            gameObject.SetActive(false);
        }


        protected virtual void OnSetParamList(params object[] objects)
        {
        }

        protected virtual void OnSetParent()
        {
            Transform parentTrans = null;
            if (parentType == UIParentEnum.BaseCanvas)
            {
                parentTrans = UIManager.OverLayCanvas.transform;
            }
            else if (parentType == UIParentEnum.BaseCameraCanvas)
            {
                parentTrans = UIManager.CameraCanvas.transform;
            }
            else
            {
                parentTrans = UIManager.cacheUIRoot.transform;
            }
            gameObject.transform.SetParent(parentTrans, false);
        }

        protected virtual void OnInitWindow()
        {
        }

        protected virtual void OnRegistWindow()
        {

        }

        protected virtual void OnUnRegistWindow()
        {

        }

        protected virtual void OnOpenWindow()
        {
        }

        protected virtual void OnCloseWindow()
        {

        }

        protected virtual void OnDestoryWindow()
        {

        }

        protected virtual void OnClickClose(GameObject go)
        {        
            this.CloseWindow();           
        }

    }

    public enum UIParentEnum
    {
        BaseCanvas,
        BaseCameraCanvas,
        Root,
    }

    public enum UIType
    {
        none = 0,
        fullScreen = 0,
        popUP = 1,

        other = 2,
    }


}
