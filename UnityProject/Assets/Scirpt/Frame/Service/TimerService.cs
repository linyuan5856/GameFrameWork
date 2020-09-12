using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameFrameWork;
using UnityEngine;

public class TimerService : BaseService
{
    public const float INFINITY = -9999;
    public const int NONE = -1;
    private bool openLog = false;

    private class Timer
    {
        private int identifyId;
        private float interval;
        private float life;
        private object[] param;
        private Action<bool, object[]> callBack;
        private bool isStop;
        private bool isDelete;
        private bool isIgnoreTimeScale;

        private float startTime;
        private float lastUpdateTime;

        public float StartTime
        {
            get => startTime;
            set => startTime = value;
        }

        public bool IsStop
        {
            get => isStop;
            set => isStop = value;
        }

        public bool IsDelete
        {
            get => isDelete;
            set => isDelete = value;
        }

        public float Life
        {
            get => life;
            set => life = value;
        }

        public int IdentifyId => identifyId;

        public bool IsIgnoreTimeScale => isIgnoreTimeScale;

        public void SetTimer(int id, Action<bool, object[]> callBack, float interval, float life, bool ignoreTimeScale,
            object[] param = null)
        {
            identifyId = id;
            this.interval = interval;
            this.life = life;
            this.callBack = callBack;
            this.param = param;
            isIgnoreTimeScale = ignoreTimeScale;
        }


        public bool Tick(float time)
        {
            if (time - lastUpdateTime >= interval)
            {
                callBack(false, param);
                lastUpdateTime = time;
            }

            if (Math.Abs(life - INFINITY) > 1)
            {
                if (time - startTime >= life)
                    isDelete = true;
            }

            return isDelete;
        }

        public void Reset()
        {
            callBack(true, param);
            startTime = 0;
            lastUpdateTime = 0;
            life = INFINITY;
            callBack = null;
            param = null;
            isStop = false;
            isDelete = false;
        }
    }

    private int identifyId;
    public float Interval { get; set; } = 0.01f;


    private readonly MapList<int, Timer> _timeMap = new MapList<int, Timer>();
    private readonly Queue<Timer> _pool = new Queue<Timer>();

    public int CreateTimer(Action<bool, object[]> callBack, float life = INFINITY, float interval = 0.1f,
        bool ignoreTimeScale = false, object[] param = null)
    {
        Timer timer;
        identifyId++;

        if (_pool.Count > 0)
        {
            timer = _pool.Dequeue();
            DebugTime(LogType.Log, $"[{GetType().Name}]  Create Timer From Pool-> {identifyId} Life->{life}");
        }
        else
        {
            timer = new Timer();
            DebugTime(LogType.Log, $"[{GetType().Name}]  Create Timer Use NEW-> {identifyId} Life->{life}");
        }

        timer.SetTimer(identifyId, callBack, interval, life, ignoreTimeScale, param);
        timer.StartTime = GetTimeNow(ignoreTimeScale);
        this._timeMap.Add(identifyId, timer);
        return identifyId;
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        _timeMap.Clear();
        willDeleteTimerList.Clear();
    }


    public void RefreshTimerLife(int id, float life)
    {
        var timer = GetTimer(id);
        if (timer != null)
        {
            timer.StartTime = GetTimeNow(timer.IsIgnoreTimeScale);
            timer.Life = life;
            DebugTime(LogType.Log,
                $"[{GetType().Name}]  Refresh Timer ID->{id} Life->{life}");
        }
    }

    public void StopTimer(int id)
    {
        var timer = GetTimer(id);
        if (timer != null)
            timer.IsStop = true;
        DebugTime(LogType.Log, $"[{GetType().Name}]  Stop Timer ID->{id}");
    }

    public void ResumeTimer(int id)
    {
        var timer = GetTimer(id);
        if (timer != null)
            timer.IsStop = false;
        DebugTime(LogType.Log, $"[{GetType().Name}]  Resume Timer ID->{id}");
    }


    public void DeleteTimer(int id)
    {
        var timer = GetTimer(id);
        if (timer != null)
            timer.IsDelete = true;
        DebugTime(LogType.Log, $"[{GetType().Name}]  Delete Timer ID->{id}");
    }

    private float GetTimeNow(bool ignoreTimeScale)
    {
        return ignoreTimeScale ? unscaleCurrentTime : currentTime;
    }

    private Timer GetTimer(int id)
    {
        if (_timeMap.ContainsKey(id))
        {
            var time = _timeMap[id];
            return time;
        }

        DebugTime(LogType.Error, $"[{GetType().Name}] ID->{id} is not Exist");
        return null;
    }

    private float unscaleCurrentTime;
    private float tempTimeUnScaled;

    private float currentTime;
    private float tempTime;

    private readonly List<int> willDeleteTimerList = new List<int>();

    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        unscaleCurrentTime += Time.unscaledDeltaTime;
        tempTimeUnScaled += Time.unscaledDeltaTime;

        currentTime += deltaTime;
        tempTime += deltaTime;

        if (tempTime >= Interval)
            tempTime = 0;

        if (tempTimeUnScaled >= Interval)
            tempTimeUnScaled = 0;

        if (_timeMap.Count > 0) // && (canTickTimer || canTickUnScaledTimer))
        {
            willDeleteTimerList.Clear();
            var list =_timeMap.AsList();
            for (int i = 0; i < list.Count; i++)
            {
                var timer = list[i];
                if (timer == null)
                {
                    DebugTime(LogType.Error, $"[{GetType().Name}] List Index->{i}  Timer is null");
                    continue;
                }

                //timer.Tick(float time) return true,this timer will be deleted（life is over）
                if (timer.IsDelete || !timer.IsStop && timer.Tick(GetTimeNow(timer.IsIgnoreTimeScale)))
                    willDeleteTimerList.Add(timer.IdentifyId);
            }

            for (int i = 0; i < willDeleteTimerList.Count; i++)
            {
                int id = willDeleteTimerList[i];
                var timer = GetTimer(id);
                timer.Reset();
                _timeMap.Remove(id);
                _pool.Enqueue(timer);
                DebugTime(LogType.Log, $"[{GetType().Name}]  Timer Life Is End ID->{id}");
            }
        }
    }

    [Conditional("UNITY_EDITOR")]
    private void DebugTime(LogType type, object obj)
    {
        if (!openLog)
            return;

        switch (type)
        {
            case LogType.Error:
                UnityEngine.Debug.LogError(obj);
                break;
            case LogType.Warning:
                UnityEngine.Debug.LogWarning(obj);
                break;
            case LogType.Log:
                UnityEngine.Debug.Log(obj);
                break;
        }
    }
}