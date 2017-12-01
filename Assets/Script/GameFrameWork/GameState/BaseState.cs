﻿using UnityEngine.SceneManagement;

namespace Pandora
{
    public class BaseState : State
    {
        protected bool mIsStageLoaded;

        public bool IsStageLoaded
        {
            get { return mIsStageLoaded; }
        }

        public sealed override void EnterState()
        {
            mIsStageLoaded = false;
            GameLogger.Log(string.Format("Scene [{0}] Loaded Complete：", SceneManager.GetActiveScene().name));
            OnInitStage();    
            mIsStageLoaded = true;
        }

        public sealed override void LeaveState()
        {
            OnLeaveStage();
        }
       
        protected virtual void OnInitStage()
        {
           
           
        }

        protected virtual void OnLeaveStage()
        {

        }
    }
}
