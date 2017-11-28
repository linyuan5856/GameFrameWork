using System.Collections.Generic;

namespace GFW
{
    public class State
    {
        protected string mName;
        protected object mParam;

        public State()
        {
            mName = this.ToString();
        }

        public string stateName
        {
            get
            {
                return mName;
            }
        }

        public virtual void SetParam(object param)
        {
            mParam = param;
        }

        public virtual void EnterState()
        {

        }

        public virtual void LeaveState()
        {

        }


        public virtual void OnUpdate()
        {

        }
    }

    public class StateMachine
    {
        protected Dictionary<string, State> mDicState = new Dictionary<string, State>();
        protected State mCurState;
        State FindState(string _name)
        {
            State state = null;
            mDicState.TryGetValue(_name, out state);
            return state;
        }

        public void RegisterState(State _state)
        {
            if (FindState(_state.stateName) == null)
            {
                mDicState.Add(_state.stateName, _state);
            }
        }

        public void ChangeState<T>(object param = null)
        {
            string t = typeof(T).ToString();
            ChangeState(t, param);
        }
        private void ChangeState(string stateName, object param)
        {
            Logger.Log("Change State:" + stateName);
            State state = FindState(stateName);
            if (state != null)
            {
                if (mCurState != null)
                {
                    mCurState.LeaveState();
                    LoaderManager.Instance.ClearCache(CacheType.Scene);
                }
                mCurState = state;
                mCurState.SetParam(param);
                mCurState.EnterState();
            }
        }

        public void OnUpdate()
        {
            if (mCurState != null)
            {
                mCurState.OnUpdate();
            }
        }

        public State curState
        {
            get
            {
                return mCurState;
            }
        }

    }
}
