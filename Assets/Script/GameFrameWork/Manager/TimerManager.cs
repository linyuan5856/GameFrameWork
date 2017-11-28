using System;
using System.Collections.Generic;
using System.Linq;
using GFW;
using UnityEngine;
namespace GFW
{
    public interface ICalculateTime
    {
        void CallBack_SyncTime(TimeSpan span);

        void CallBack_TimeOut();
    }

    public sealed class TimerDefine
    {
        public const int CARNIVAL = 101;
        public const int HERORECRUIT = 102;
        public const int VIPFREE = 103;
        public const int TASK = 104;
        public const int BATTLE = 105;
        public const int STAMINA = 106;
        public const int ARMYHELP = 107;
        public const int BATTLECARD = 108;
        public const int LADDER = 109;

    }

    public class TimerManager : MonoSingleton<TimerManager>
    {
        class TimeData
        {
            int timeType;        //计时器类型

            float startTime;     // 开始计时时间/  

            float endTime;       // 计时时间 

            float lastSyncTime;//上次进行同步的时间

            int interval;//多少时间间隔同步一次

            public float EndTime
            {
                get { return endTime; }
                set { endTime = value; }
            }

            public float StartTime
            {
                get { return startTime; }
            }

            public int TimeType
            {
                get { return timeType; }
            }

            public float LastSyncTime
            {
                get { return lastSyncTime; }

                set { lastSyncTime = value; }
            }

            public int Interval
            {
                get { return interval; }
            }

            public TimeData(int timeType, float startTime, double endTime, int interval)
            {
                this.timeType = timeType;
                this.endTime = (float)endTime;
                this.startTime = startTime;
                this.interval = interval;
                this.lastSyncTime = 0;
            }

            public void UpdateData(float start, double end)
            {
                this.startTime = start;
                this.endTime = (float)end;
                this.lastSyncTime = 0;
            }
        }
     
        Dictionary<int, TimeData> _timerDic = new Dictionary<int, TimeData>();
        List<TimeData> _timerList=new List<TimeData>();
        Dictionary<int, List<ICalculateTime>> _monitorDic = new Dictionary<int, List<ICalculateTime>>();
        List<int> _expiredTimerLists = new List<int>();
       
        protected override void OnUpdate()
        {
            base.OnUpdate();
           
            for (int i = 0; i < _timerList.Count; i++)//用 foreash 每帧24B ..
            {
                this.UpdateTimer(_timerList[i]);
            }       

            this.RemoveExpiredTimer();
        }   

        void UpdateTimer(TimeData data)
        {
            if (Time.realtimeSinceStartup - data.LastSyncTime > data.Interval)
            {
                this.UpdateMonitor_TimerSync(data, false);
            }

            float hasPassedTime = Time.realtimeSinceStartup - data.StartTime;

            if (hasPassedTime > data.EndTime)
            {
                this.UpdateMonitor_TimeOut(data.TimeType);

                if (data.TimeType != TimerDefine.STAMINA)//体力恢复 永动机
                {
                    _expiredTimerLists.Add(data.TimeType);
                }
            }
        }

        void UpdateMonitor_TimerSync(TimeData data, bool immediate)
        {
            float leftTime = GetLeftTime(data);
            int leftTimeInt = Mathf.CeilToInt(leftTime);
            TimeSpan span = new TimeSpan(0, 0, 0, leftTimeInt);
            data.LastSyncTime = Time.realtimeSinceStartup;

            if (this._monitorDic.ContainsKey(data.TimeType))
            {
                List<ICalculateTime> monitors = this._monitorDic[data.TimeType];

                for (int i = 0; i < monitors.Count; i++)
                {
                    monitors[i].CallBack_SyncTime(span);
                }
            }
        }

        void UpdateMonitor_TimeOut(int timerType)
        {
            if (this._monitorDic.ContainsKey(timerType))
            {
                List<ICalculateTime> monitors = this._monitorDic[timerType];

                for (int i = 0; i < monitors.Count; i++)
                {
                    monitors[i].CallBack_TimeOut();
                }
            }
        }

        void RemoveExpiredTimer()
        {
            for (int i = 0; i < this._expiredTimerLists.Count; i++)
            {
                int timerType = this._expiredTimerLists[i];
                if (_timerDic.ContainsKey(timerType))
                {
                    TimeData data = _timerDic[timerType];
                    if (this._timerList.Contains(data))
                    {
                        this._timerList.Remove(data);
                    }

                    this._timerDic.Remove(timerType);
                }
               
                this.RemoveMonitors(timerType);
            }

            this._expiredTimerLists.Clear();
        }


        float GetLeftTime(TimeData data)
        {
            return Mathf.Clamp(Mathf.CeilToInt(data.EndTime - (Time.realtimeSinceStartup - data.StartTime)), 0, data.EndTime);
        }

        public bool HasTimer(int type)
        {
            return this._timerDic.ContainsKey(type);
        }

        public void RegisterTimer(int type, double endTime, int interval = 1)
        {
            if (endTime <= 0)
            {
                return;
            }

            float startTime = Time.realtimeSinceStartup;
            if (!this._timerDic.ContainsKey(type))
            {
                TimeData data = new TimeData(type, startTime, endTime, interval);
                this._timerDic.Add(type, data);
                this._timerList.Add(data);
            }
            else
            {
                this._timerDic[type].UpdateData(startTime, endTime);
            }

            //Debug.LogWarning(Time.time+string.Format("  注册 计时器 Type {0}...Time {1} ",type,endTime));
        }

        bool HasMonitor(int type, ICalculateTime newTimer)
        {
            return newTimer != null
                   &&
                   this._monitorDic.ContainsKey(type)
                   &&
                   this._monitorDic[type].Contains(newTimer);
        }

        public void AddNewMonitor(int type, ICalculateTime newTimer)
        {
            if (!HasMonitor(type, newTimer))
            {
                //Debug.LogWarning(Time.time + string.Format("  Type {0}  添加 监听器", type));
                if (!this._monitorDic.ContainsKey(type))
                {
                    this._monitorDic.Add(type, new List<ICalculateTime>());
                }

                this._monitorDic[type].Add(newTimer);
            }

            if (HasTimer(type))
            {
                this.UpdateMonitor_TimerSync(this._timerDic[type], true);
            }
        }

        /// <summary>
        /// 1个View 对多个 Timer的情况  --eg:军团事件 军团建筑
        /// 先移除旧的 再添加新的timer
        /// </summary>
        /// <param name="type"></param>
        public void RemoveMonitors(int type)
        {
            if (this._monitorDic.ContainsKey(type))
            {
                //Debug.LogWarning(string.Format("Type {0} 移除全部监听器", type));
                this._monitorDic[type].Clear();
            }
            else
            {
                UnityEngine.Debug.LogError(string.Format("要移除监听器的 定时器类型 Type =={0}  不存在", type));
            }
        }

        public void RemoveAllMonitors()
        {
            foreach (var data in this._monitorDic)
            {
                data.Value.Clear();
            }
        }


        public void ForceEnd(int type)
        {
            if (this._timerDic.ContainsKey(type))
            {
                this._expiredTimerLists.Add(type);
            }
        }
    }
}
