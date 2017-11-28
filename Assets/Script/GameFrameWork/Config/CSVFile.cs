using UnityEngine;
using System.Collections.Generic;


namespace GFW
{
    [System.Serializable]
    public class CSVFile : ScriptableObject
    {
        public List<string> dataList = new List<string>();
        public string tableName;

        private Dictionary<string, int> keyIndexDict = new Dictionary<string, int>();
        private Dictionary<string, CSVRow> rowDict = new Dictionary<string, CSVRow>();
        private List<CSVRow> rowList = new List<CSVRow>();

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

                CSVRow row = new CSVRow(valueArr, this);

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

        public string GetValue(CSVRow row, string key)
        {
            int index = keyIndexDict[key];
            return row.rowArr[index];
        }

        public CSVRow GetRowByKey(string value)
        {
            return rowDict[value];
        }

        public CSVRow[] GetRow(string key, string value)
        {
            List<CSVRow> list = new List<CSVRow>();
            for (int i = 0; i < rowList.Count; i++)
            {
                CSVRow row = rowList[i];
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

        public Dictionary<string, CSVRow> GetAllRows()
        {
            return rowDict;
        }



        public int Count
        {
            get { return dataList.Count - 1; }
        }
    }

    public class CSVRow
    {
        public CSVRow(string[] arr, CSVFile csvFile)
        {
            this.rowArr = arr;
            this.csvFile = csvFile;
        }

        public string[] rowArr;
        private CSVFile csvFile;

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
