using System.Collections.Generic;

namespace Pandora
{
    public class STable : SVO
    {
        public STable()
            : base()
        {
            childList = new List<STable>();
            viewDict = new Dictionary<string, object>();
        }

        private List<STable> childList;

        private Dictionary<string, object> viewDict;

        public STable parent { get; set; }

        public int Count
        {
            get { return childList.Count; }
        }

        public void addChild(STable sTable)
        {
            STable affectSO = sTable;
            if (sTable.parent != null)
            {
                sTable.parent.removeChild(sTable);
            }
            sTable.parent = this;
            childList.Add(sTable);
            DispatchEvent(DataEvent.Event_AddChild, affectSO);
        }

        public void addChildAt(int pos, STable sTable)
        {
            STable affectSO = sTable;
            if (sTable.parent != null)
            {
                sTable.parent.removeChild(sTable);
            }
            sTable.parent = this;
            if (childList[pos] != null)
            {
                GameLogger.LogWarn("addChildAt pos had an exist STable:" + pos);
            }
            childList[pos] = sTable;
            DispatchEvent(DataEvent.Event_AddChild, affectSO);
        }

        public void removeChild(STable sTable)
        {
            STable affectSO = sTable;
            if (childList.Remove(sTable) == false)
            {
                GameLogger.LogWarn("removeChild failed");
            }
            DispatchEvent(DataEvent.Event_RemoveChild, affectSO);
        }

        public void removeChildAt(int pos)
        {
            STable affectSO = null;
            if (childList.Count > pos)
            {
                if (childList[pos] == null)
                {
                    GameLogger.LogWarn("removeChildAt pos already null STable:" + pos);
                }
                else
                {
                    affectSO = childList[pos];
                    childList[pos] = null;
                }
            }
            else
            {
                GameLogger.LogWarn("remove child at a not exist pos:" + pos + " this: " + this.ToString());
            }
            DispatchEvent(DataEvent.Event_RemoveChild, affectSO);
        }

        public STable getChildAt(int i)
        {
            return childList[i];
        }

        public void addView<T>(T dataSvo)
        {
            viewDict.Add(typeof(T).Name, dataSvo);
        }

        public T getView<T>()
        {
            object stateObj;
            viewDict.TryGetValue(typeof(T).Name, out stateObj);

            return (T) stateObj;
        }
    }
}