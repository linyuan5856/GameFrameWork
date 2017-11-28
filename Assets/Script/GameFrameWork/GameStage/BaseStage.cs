using UnityEngine.SceneManagement;

namespace GFW
{
    public class BaseStage : State
    {
        protected bool mIsStageLoaded;

        public bool IsStageLoaded
        {
            get { return mIsStageLoaded; }
        }

        public sealed override void EnterState()
        {
            mIsStageLoaded = false;
            Logger.Log(string.Format("Scene [{0}] Loaded Complete：", SceneManager.GetActiveScene().name));
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
