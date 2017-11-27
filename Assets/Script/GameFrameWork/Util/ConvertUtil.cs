using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using GFW;
using UnityEngine;
using Logger = UnityEngine.Logger;

namespace GFW
{
    public class ConvertUtil
    {
        public static List<int> ConvertToIntList(string value, char splite)
        {
            string[] arr = value.Split(splite);

            int[] ret = Array.ConvertAll(arr, Int32.Parse);
            return ret.ToList();
        }

        public static string[] ConvertToArrary(string value, char splite)
        {
            string[] arr = value.Split(splite);
            return arr;
        }

        public static MapStruct.Map<K, V> ConvertToMap<K, V>(string value, char splite)
        {
            string[] arr = value.Split(splite);
            MapStruct.Map<K, V> map = new MapStruct.Map<K, V>();

            if (arr.Length == 1)
            {
                map.Key = (K)ConvertType(map.Key, arr[0]);
                map.Value = default(V);
            }
            else
            {
                map.Key = (K)ConvertType(map.Key, arr[0]);
                map.Value = (V)ConvertType(map.Key, arr[1]);
            }
            return map;
        }

        static object ConvertType<T>(T t, string content)
        {
            if (t is int)
            {
                return int.Parse(content);
            }
            if (t is float)
            {
                return float.Parse(content);
            }
            if (t is double)
            {
                return double.Parse(content);
            }

            return null;
        }

        public static List<MapStruct.Map<K, V>> ConvertToMapList<K, V>(string value, char splite)
        {
            string[] arr = value.Split(splite);

            List<MapStruct.Map<K, V>> teampList = new List<MapStruct.Map<K, V>>();
            for (int i = 0; i < arr.Length; i++)
            {
                MapStruct.Map<K, V> map = ConvertToMap<K, V>(arr[i], ':');
                teampList.Add(map);
            }

            return teampList;

        }

        public static MapStruct.MapList<int> ConvertToMapList_Int(string value, char splite, char secondSplite)
        {
            if (value == "-1")
            {
                return new MapStruct.MapList<int>(null, null);

            }

            string[] arr = value.Split(splite);
            List<int> firstList = null;
            List<int> secondList = null;
            string[] tempArrary;

            try
            {
                if (arr.Length > 0)
                {
                    firstList = new List<int>();
                    tempArrary = arr[0].Split(secondSplite);
                    for (int i = 0; i < tempArrary.Length; i++)
                    {
                        firstList.Add(Int32.Parse(tempArrary[i]));
                    }
                }
                if (arr.Length > 1)
                {
                    secondList = new List<int>();
                    tempArrary = arr[1].Split(secondSplite);
                    for (int i = 0; i < tempArrary.Length; i++)
                    {
                        secondList.Add(Int32.Parse(tempArrary[i]));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                Logger.LogError("convert error:" + value);
            }


            MapStruct.MapList<int> _map = new MapStruct.MapList<int>(firstList, secondList);
            return _map;
        }

        public static MapStruct.MapList<string> ConvertToMapList_String(string value, char splite, char secondSplite)
        {
            if (value == "-1")
            {
                return new MapStruct.MapList<string>(null, null);

            }

            string[] arr = value.Split(splite);
            List<string> firstList = null;
            List<string> secondList = null;
            string[] tempArrary;

            if (arr.Length > 0)
            {
                firstList = new List<string>();
                tempArrary = arr[0].Split(secondSplite);
                for (int i = 0; i < tempArrary.Length; i++)
                {
                    firstList.Add(tempArrary[i]);
                }
            }
            if (arr.Length > 1)
            {
                secondList = new List<string>();
                tempArrary = arr[1].Split(secondSplite);
                for (int i = 0; i < tempArrary.Length; i++)
                {
                    secondList.Add(tempArrary[i]);
                }
            }

            MapStruct.MapList<string> _map = new MapStruct.MapList<string>(firstList, secondList);
            return _map;
        }

      

        public static void ToPVO(String configStr, System.Object target)
        {
            System.IO.StringReader _reader = new System.IO.StringReader(configStr);
            Type targetType = target.GetType();
            while (_reader.Peek() >= 0)
            {
                string line = _reader.ReadLine();
                line = line.Trim();
                if (line.IndexOf("//") == 0) continue;
                int splitIndex = line.IndexOf('=');
                if (splitIndex != -1)
                {
                    string key = line.Substring(0, splitIndex);
                    string value = line.Substring(splitIndex + 1);
                    //
                    //Logger.LogTest(string.Format("{0}:{1}", key, value));

                    FieldInfo fieldInfo = targetType.GetField(key,
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (fieldInfo == null)
                    {
                        //Logger.LogWarn("cannot find property:" + key);
                        continue;
                    }
                    SetFieldValue(fieldInfo, target, value);
                }
            }
            _reader.Close();
        }

        public static void NewCSVToStaticClass(NewCSVFile csvFile, Type targetType)
        {
            FieldInfo[] fieldList = targetType.GetFields(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static);

            for (int i = 0; i < fieldList.Length; i++)
            {
                FieldInfo field = fieldList[i];

                if (csvFile.ContainsKey(field.Name))
                {
                    SetFieldValue(field, targetType, csvFile.GetRowByKey(field.Name).GetValue("Value"));
                }
                else
                {
                   Logger.LogError(string.Format("can not find field:{0},{1}", targetType.Name, field.Name));
                }
            }
        }

        public static NewCSVFile ToNewCSVFile(string configStr)
        {
            System.IO.StringReader _reader = new System.IO.StringReader(configStr);

            NewCSVFile newCSVFile = ScriptableObject.CreateInstance<NewCSVFile>();

            while (_reader.Peek() >= 0)
            {
                string line = _reader.ReadLine();
                line = line.Trim();
                if (!string.IsNullOrEmpty(line))
                    newCSVFile.dataList.Add(line);
            }
            _reader.Close();

            return newCSVFile;
        }

        public static Dictionary<string, string> ToPDict(String configStr)
        {
            System.IO.StringReader _reader = new System.IO.StringReader(configStr);

            Dictionary<string, string> dict = new Dictionary<string, string>();

            while (_reader.Peek() >= 0)
            {
                string line = _reader.ReadLine();
                line = line.Trim();
                if (line.IndexOf("//") == 0) continue;
                int splitIndex = line.IndexOf('=');
                if (splitIndex != -1)
                {
                    string key = line.Substring(0, splitIndex);
                    string value = line.Substring(splitIndex + 1);
                    //
                    //Logger.LogTest(string.Format("{0}:{1}", key, value));
                    dict.Add(key, value);
                }
            }
            _reader.Close();
            return dict;
        }

        //static public List<T> ToTVO<T>(CSVSheet sheet) where T:IRow,new()
        //{
        //    Type targetType = typeof(T);
        //    //
        //    List<T> list = new List<T>();
        //    string[] heads = sheet.Heads;
        //    //fieldInfo.Name
        //    List<KeyValuePair<string, FieldInfo>> fieldList = new List<KeyValuePair<string, FieldInfo>>();
        //    for (int i = 0; i < heads.Length; i++)
        //    {
        //        string fieldName = heads[i];
        //        FieldInfo fieldInfo = targetType.GetField(fieldName,
        //            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        //        if (fieldInfo == null)
        //        {
        //            //Logger.LogWarn("cannot find field : " + fieldName);
        //            continue;
        //        }
        //        fieldList.Add(new KeyValuePair<string, FieldInfo>(fieldName, fieldInfo));
        //    }


        //    foreach (var r in sheet)
        //    {
        //        CSVRow row = r.Value;

        //        T t = new T();
        //        //for (int i = 0; i < fieldList.Count;i++)
        //        //{
        //        //    string fieldName = fieldList[i].Key;
        //        //    FieldInfo fieldInfo = fieldList[i].Value;
        //        //    SetFieldValue(fieldInfo,t, row.Get(fieldName));
        //        //}
        //        t.SetRow(row, fieldList);
        //        list.Add(t);
        //    }
        //    return list;
        //}

        public static bool SetFieldValue(FieldInfo fieldInfo, System.Object target, string value)
        {
            try
            {
                if (fieldInfo.FieldType == typeof(int))
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        fieldInfo.SetValue(target, 0);
                    }
                    else
                    {                      
                        try
                        {
                            fieldInfo.SetValue(target, int.Parse(value));
                        }
                        catch (Exception ex)
                        {
                            float tValue = float.Parse(value);
                            fieldInfo.SetValue(target, Mathf.RoundToInt(tValue));
                            Logger.LogError(ex.ToString());
                        }
                    }
                }
                else if (fieldInfo.FieldType == typeof(string))
                {
                    fieldInfo.SetValue(target, value);
                }
                else if (fieldInfo.FieldType == typeof(float))
                {
                    fieldInfo.SetValue(target, float.Parse(value));
                }
                else if (fieldInfo.FieldType == typeof(bool))
                {
                    fieldInfo.SetValue(target, (value == "true" || value == "1") ? true : false);
                }
                else if (fieldInfo.FieldType == typeof(long))
                {
                    fieldInfo.SetValue(target, long.Parse(value));
                }
                else
                {
                   Logger.LogError("field type not supported:" + target.GetType().ToString() + " " + fieldInfo.FieldType);
                    return false;
                }
            }
            catch (Exception)
            {
               Logger.LogError(string.Format("field setValue failed:{0},{1},{2},{3},", target.GetType(), fieldInfo.FieldType, fieldInfo.Name, value));
                return false;
            }
            return true;
        }

        public static string Join<T>(string sep, List<T> _list)
        {
            string ret = string.Empty;
            for (int i = 0, len = _list.Count; i < len; i++)
            {
                if (i == 0)
                    ret = _list[i].ToString();
                else
                    ret += sep + _list[i].ToString();
            }
            return ret;
        }
    }

}
