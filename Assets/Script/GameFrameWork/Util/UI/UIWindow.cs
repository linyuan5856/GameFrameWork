using System;
using UnityEngine;
using UnityEngine.UI;

namespace Pandora
{
    public class UIWindow : MonoBehaviour
    {     
        private bool _isInit;
       
        void OnDestroy()
        {
            try
            {
                OnUnRegisterWindow();
                OnDestroyWindow();
            }
            catch (Exception ex)
            {
               GameLogger.LogError(ex.ToString());
            }
        }

        public void SetParamList(params object[] objects)
        {
            OnSetParamList(objects);
        }

        public void InitWindow()
        {
            if (_isInit) return;
            _isInit = true;
           
            OnInitWindow();
        }

        public void OpenWindow()
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            transform.SetAsLastSibling();

            OnRegisterWindow();    
            OnOpenWindow();
        }

        public void CloseWindow()
        {        
            OnUnRegisterWindow();
            OnCloseWindow();         
        }


        protected virtual void OnSetParamList(params object[] objects)
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

        protected virtual void OnCloseWindow()
        {

        }

        protected virtual void OnDestroyWindow()
        {

        }

    }
}
