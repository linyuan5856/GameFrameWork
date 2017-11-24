using System;
using System.Collections.Generic;

public interface IEvent
{
    void SetManager(IManager manager);

    void Action();
}
public interface IManager
{
    void DoNext();

    void Register(IEvent _event);
}

public abstract class BaseEvent : IEvent
{
    private IManager _manager;

    void IEvent.SetManager(IManager manager)
    {
        this._manager = manager;
    }

    void IEvent.Action()
    {
        OnAction();
    }

    protected abstract void OnAction();

    protected virtual void OnActionEnd()
    {
        this._manager.DoNext();
    }
}


public class EventQueueManager : IManager
{
    private Action _endAction;
    readonly Queue<IEvent> _eventQueue = new Queue<IEvent>();

    public EventQueueManager(Action endAction)
    {
        _endAction = endAction;
    }

    public void Register(IEvent _event)
    {
        _event.SetManager(this);
        _eventQueue.Enqueue(_event);
    }

    void IManager.DoNext()
    {
        bool end = _eventQueue.Count == 0;

        if (end)
        {
            if (this._endAction != null)
            {
                this._endAction();
            }
            return;
        }
        this.TryInvoke();
    }

    void TryInvoke()
    {
        if (_eventQueue.Count > 0)
        {
            IEvent _event = _eventQueue.Dequeue();

            _event.Action();
        }
    }
}

public class EventparallelManager : IManager
{
    private Action _endAction;
    private int totalEventSum;
    private List<IEvent> _events = new List<IEvent>();

    public EventparallelManager(Action endAction)
    {
        _endAction = endAction;
    }

    public void Register(IEvent _event)
    {
        _event.SetManager(this);
        _events.Add(_event);
        totalEventSum++;
    }

    public void BeginParallelEvent()
    {

        if (_events.Count==0)
        {
            this._endAction();
        }

        for (int i = 0; i < _events.Count; i++)
        {
            _events[i].Action();
        }
    }

    void IManager.DoNext()
    {
        totalEventSum--;

        if (totalEventSum == 0 && this._endAction != null)
        {
            _endAction();
        }
    }
}