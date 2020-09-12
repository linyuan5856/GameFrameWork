using System.Collections.Generic;
using System;
using GameFrameWork;

public abstract class State
{
    private object _param;
    public string StateName { get; }

    protected State()
    {
        StateName = GetType().Name;
    }

    public virtual void InitState()
    {
    }

    public virtual void SetParam(object param)
    {
        _param = param;
    }

    public object GetParam()
    {
        return _param;
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

public class SimpleState : State
{
    private readonly Action _onEnter;
    private readonly Action _onLeave;

    public SimpleState(Action onEnter, Action onLeave)
    {
        _onEnter = onEnter;
        _onLeave = onLeave;
    }

    public override void EnterState()
    {
        _onEnter?.Invoke();
    }

    public override void LeaveState()
    {
        _onLeave?.Invoke();
    }
}

public class StateMachine
{
    private readonly string _name;
    private readonly bool showDebugInfo = true;
    private Action<State> _enterStateCallbacks;
    private Action<State> _leaveStateCallbacks;
    private readonly Dictionary<string, State> _stateDic;
    private State _curState;
    public State CurState => _curState;

    public StateMachine(string name = null)
    {
        _name = name;
        _stateDic = new Dictionary<string, State>();
    }

    public void InitAllState()
    {
        foreach (var map in _stateDic)
            map.Value.InitState();
    }

    public void AddEnterStateListener(Action<State> callback)
    {
        if (_enterStateCallbacks != null)
        {
            Delegate[] invocationList = _enterStateCallbacks.GetInvocationList();
            int index = Array.IndexOf(invocationList, callback);
            if (index != -1)
            {
                Debuger.LogWarning("repeated state listener registed");
                return;
            }
        }

        _enterStateCallbacks += callback;
    }

    public void AddLeaveStateListener(Action<State> callback)
    {
        if (_leaveStateCallbacks != null)
        {
            Delegate[] invocationList = _leaveStateCallbacks.GetInvocationList();
            int index = Array.IndexOf(invocationList, callback);
            if (index != -1)
            {
                Debuger.LogWarning("repeated state listener registed");
                return;
            }
        }

        _leaveStateCallbacks += callback;
    }

    public void RemoveEnterStateListener(Action<State> callback)
    {
        if (callback != null)
            if (_enterStateCallbacks != null)
                _enterStateCallbacks = _enterStateCallbacks - callback;
    }

    public void RemoveLeaveStateListener(Action<State> callback)
    {
        if (callback != null)
            if (_leaveStateCallbacks != null)
                _leaveStateCallbacks -= callback;
    }


    public State FindState(string name)
    {
        _stateDic.TryGetValue(name, out var state);
        return state;
    }

    public void RegisterState(State state)
    {
        if (FindState(state.StateName) == null)
            _stateDic.Add(state.StateName, state);
    }

    public void ChangeState<T>(object param = null)
    {
        string t = typeof(T).ToString();
        ChangeState(t, param);
    }

    public void ChangeState(string stateName, object param = null)
    {
        if (showDebugInfo)
            Debuger.Log(_name + " Change State:" + stateName);

        if (_curState != null && _curState.StateName == stateName)
        {
            Debuger.LogWarning("re-enter to same state : " + stateName);
        }

        if (_curState != null)
        {
            State tempState = _curState;
            _curState = null;
            tempState.LeaveState();
            if (_leaveStateCallbacks != null)
                _leaveStateCallbacks(tempState);
        }

        if (_curState != null)
        {
            Debuger.LogError("Do not EnterState When LeaveState:" + _curState.StateName);
        }

        if (!string.IsNullOrEmpty(stateName))
        {
            State state = FindState(stateName);
            if (state != null)
            {
                _curState = state;
                _curState.SetParam(param);
                _curState.EnterState();
            }
            else
            {
                Debuger.LogError("can not find state:" + stateName);
            }
        }

        _enterStateCallbacks?.Invoke(_curState);
    }

    public void LeaveNow()
    {
        CurState?.LeaveState();
    }

    public void Destroy()
    {
        CurState?.LeaveState();
    }

    public void OnUpdate()
    {
        _curState?.OnUpdate();
    }
}