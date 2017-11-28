
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GFW
{
    public abstract class ITable : ScriptableObject
    {
        public abstract void InitTable();
        public abstract void AddRow(IRow row);
        public abstract void Clear();
    }

    public abstract class BaseTable<T> : ITable where T : IRow, new()
    {
        public List<T> dataList = new List<T>();
        public Dictionary<int, T> rowIndexDict = new Dictionary<int, T>();
        public BaseTable()
        {
        }

        public override void InitTable()
        {
            for (int i = 0; i < dataList.Count; i++)
            {
                T row = dataList[i];
                try
                {
                    int key = row.getKey();
                    if (!rowIndexDict.ContainsKey(key))
                    {
                        rowIndexDict.Add(key, row);
                        row.FliterData();
                    }
                    else
                    {
                        GFW.GameLogger.LogError(string.Format("主键重复 Table名称 {0}  主键 {1} ", this.GetType().Name, key));
                    }

                }
                catch (Exception ex)
                {
                    GFW.GameLogger.Log("RowKey is:" + row.getKey());
                    GFW.GameLogger.LogError(ex.ToString());
                }
            }
        }
        public override void Clear()
        {
            dataList.Clear();
        }
        public override void AddRow(IRow row)
        {
            dataList.Add((T)row);
        }

        public virtual T Get(int id)
        {
            if (rowIndexDict.ContainsKey(id))
                return rowIndexDict[id];
            else
                GFW.GameLogger.LogError(string.Format("can not find table data: {0} ,{1}", this.GetType().Name, id));
            return default(T);
        }

        public bool Contains(int id)
        {
            return rowIndexDict.ContainsKey(id);
        }

        public List<T> GetAll()
        {
            return dataList;
        }


    }

    public abstract class IRow
    {
        public IRow() { }
        public abstract int getKey();

        public virtual void FliterData() { }

    }


}
