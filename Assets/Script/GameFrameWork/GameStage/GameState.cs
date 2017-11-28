using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GFW
{
    public class GameState : BaseState
    {
        private string sceneName;           

        public override void SetParam(object param)
        {
            sceneName = (string)param;
        }

        protected override void OnInitStage()
        {
            MainGame.Instance.StartCoroutine(this.start_loading_scene());
        }

        private IEnumerator start_loading_scene()
        {      
            AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
           // float LoadMaxProgress = 80f;
            if (ao != null)
            {
                while (!ao.isDone)
                {
                   // int newprogress = (int)(ao.progress * LoadMaxProgress);
                   
                    yield return null;
                }
            }
           
            yield return ao.isDone;

            if (SceneManager.GetActiveScene().name != sceneName)
            {
                Logger.LogError(sceneName + " load compelte but there is some unexpected error");
                MainGame.Instance.StartCoroutine(start_loading_scene());             
                yield break;
            }

         
            mIsStageLoaded = false;
            UnityEngine.Resources.UnloadUnusedAssets();
            GC.Collect();          
            
            mIsStageLoaded = true;
        }

        protected override void OnLeaveStage()
        {
            TimerManager.Instance.RemoveAllMonitors();    
        }
    }
}