using System;
using UnityEngine;

namespace Pandora
{
    public class GameState : BaseState
    {
                
        public override void SetParam(object param)
        {
           
        }

        protected override void OnInitStage()
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();

            UIManager.Instance.OpenWindow("UI_Login");
        }

      
        protected override void OnLeaveStage()
        {
            TimerManager.Instance.RemoveAllMonitors();    
        }
    }
}