using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GFW
{
    public class GameState : BaseState
    {
                
        public override void SetParam(object param)
        {
           
        }

        protected override void OnInitStage()
        {
            UnityEngine.Resources.UnloadUnusedAssets();
            GC.Collect();

            UIManager.Instance.OpenWindow("UI_Login");
        }

      
        protected override void OnLeaveStage()
        {
            TimerManager.Instance.RemoveAllMonitors();    
        }
    }
}