using UnityEngine;
using System.Collections.Generic;


namespace GFW
{
    [System.Serializable]
    public class NewCSVFile : ScriptableObject
    {
        public List<string> dataList = new List<string>();
        public string tableName;


        private List<string[]> dataList2 = new List<string[]>();

        private Dictionary<string, int> keyIndexDict = new Dictionary<string, int>();
        private Dictionary<string, NewRow> rowDict = new Dictionary<string, NewRow>();
        private List<NewRow> rowList = new List<NewRow>();

        public void InitTable(string key)
        {
            if (keyIndexDict.Count > 0) return;

            string[] fieldArr = dataList[0].Split(new char[] {'^'});
            for (int i = 0; i < fieldArr.Length; i++)
            {
                keyIndexDict.Add(fieldArr[i], i);
            }

            if (!keyIndexDict.ContainsKey(key))
                Logger.LogError(string.Format("table {0} cannot find key {1}:", tableName, key));

            int keyIndex = keyIndexDict[key];
            for (int i = 1; i < dataList.Count; i++)
            {
                string[] valueArr = dataList[i].Split(new char[] {'^'});

                NewRow row = new NewRow(valueArr, this);

                string value = valueArr[keyIndex];
                if (rowDict.ContainsKey(value))
                    Logger.LogError(string.Format("table {0} add repeat key {1} {2}", tableName, key, value));

                rowDict.Add(value, row);
                rowList.Add(row);
            }
        }

        public void Clear()
        {
            dataList.Clear();
        }

        public string GetValue(NewRow row, string key)
        {
            int index = keyIndexDict[key];
            return row.rowArr[index];
        }

        public NewRow GetRowByKey(string value)
        {
            return rowDict[value];
        }

        public NewRow[] GetRow(string key, string value)
        {
            List<NewRow> list = new List<NewRow>();
            for (int i = 0; i < rowList.Count; i++)
            {
                NewRow row = rowList[i];
                if (row.GetValue(key) == value)
                {
                    list.Add(row);
                }
            }
            return list.ToArray();
        }

        public bool ContainsKey(string key)
        {
            return rowDict.ContainsKey(key);
        }

        public Dictionary<string, NewRow> GetAllRows()
        {
            return rowDict;
        }



        public int Count
        {
            get { return dataList.Count - 1; }
        }
    }

    public class NewRow
    {
        public NewRow(string[] arr, NewCSVFile csvFile)
        {
            this.rowArr = arr;
            this.csvFile = csvFile;
        }

        public string[] rowArr;
        private NewCSVFile csvFile;

        public string GetValue(string key)
        {
            return csvFile.GetValue(this, key);
        }

        public int GetInt(string key)
        {
            return int.Parse(GetValue(key));
        }
    }
}
