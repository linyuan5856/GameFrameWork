using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using Excel;
using MySql.Data.MySqlClient;
using UnityEngine;

namespace GFW
{
    public class ConfigEditor
    {

        public const string Path_GenerateExcel = "Assets/Script/GameFrameWork/Config/CSVScriptObject";
        public const string Path_EditorConfig = "Assets\\Script\\GameFrameWork\\Config\\EditorConfig.asset";


        [MenuItem("program/Config/GenerateConfig", false, 0)]
        private static void GenerateConfig()
        {
            CSVPathEditor csvPathEditor = AssetDatabase.LoadAssetAtPath<CSVPathEditor>(Path_EditorConfig);
            if (csvPathEditor == null)
            {
                csvPathEditor = ScriptableObject.CreateInstance<CSVPathEditor>();
                AssetDatabase.CreateAsset(csvPathEditor, Path_EditorConfig);
            }
            Selection.activeObject = csvPathEditor;
        }

        [MenuItem("program/Config/ConvertAllTable", false, 0)]
        private static void BuildAllTable()
        {
            if (!Directory.Exists(Path_GenerateExcel))
                Directory.CreateDirectory(Path_GenerateExcel);

            //
            TableConfig tableConfig = null;
            for (int i = 0; i < CSVPathEditor.Instance.tableList.Count; i++)
            {
                tableConfig = CSVPathEditor.Instance.tableList[i];
                if (!string.IsNullOrEmpty(tableConfig.rowClassName))
                {
                    BuildOneTable(tableConfig.tableClassName, tableConfig.rowClassName,
                        CSVPathEditor.Instance.Path_ExcelInputDir + tableConfig.shortPath);
                }
                else
                {
                    BuildOneTable(tableConfig.tableClassName,
                        CSVPathEditor.Instance.Path_ExcelInputDir + tableConfig.shortPath);
                }
            }

            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
            Logger.Log("Convert Complete!");
        }

        private static void BuildOneServerTable(MySqlConnection conn, string path, string tableName)
        {
            EditorUtility.DisplayProgressBar("ConvertTable", path, UnityEngine.Random.value);
            Logger.Log("ConvertServerTable : " + tableName + " " + path);

            FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            excelReader.Read();

            Dictionary<int, string> fieldDict = new Dictionary<int, string>();
            Dictionary<int, string> fieldTypeDict = new Dictionary<int, string>();

            for (int i = 0; i < excelReader.FieldCount; i++)
            {
                if (!excelReader.IsDBNull(i))
                {
                    string fieldName = excelReader.GetString(i);
                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        fieldDict.Add(i, fieldName);
                    }
                }
            }

            excelReader.Read();
            //类型
            excelReader.Read();
            for (int i = 0; i < excelReader.FieldCount; i++)
            {
                if (!excelReader.IsDBNull(i))
                {
                    string value = excelReader.GetString(i);
                    if (value == "int")
                        fieldTypeDict.Add(i, "int");
                    else if (value == "float")
                        fieldTypeDict.Add(i, "float");
                    else
                        fieldTypeDict.Add(i, "varchar(255)");
                }
            }

            //删除客户端字段
            excelReader.Read();
            for (int i = 0; i < excelReader.FieldCount; i++)
            {
                if (!excelReader.IsDBNull(i))
                {
                    string ABC = excelReader.GetString(i);
                    if (ABC != "A" && ABC != "S")
                    {
                        fieldDict.Remove(i);
                    }
                }
            }
            if (fieldDict.Count == 0)
            {
                Logger.LogWarn(tableName + " no field is server , ingore");
                return;
            }

            //create table 
            string createsql = "create table `{0}` ({1} )";
            string param1 = tableName;
            string param2 = "";


            foreach (var f in fieldDict)
            {
                if (!string.IsNullOrEmpty(param2)) param2 += " ,";
                param2 += "`" + f.Value + "`" + " " + fieldTypeDict[f.Key];
                if (f.Value.ToUpper() == "ID")
                    param2 += " PRIMARY KEY";
            }

            ExcecuteCMD(conn, "drop table if exists " + tableName);
            ExcecuteCMD(conn, string.Format(createsql, param1, param2));



            int rowCount = 5;
            while (excelReader.Read())
            {
                string insertsql = "insert into `{0}` ({1}) values ({2})";
                string p1 = tableName;
                string p2 = "";
                string p3 = "";
                if (!excelReader.IsDBNull(0) && !string.IsNullOrEmpty(excelReader.GetString(0)))
                {
                    foreach (var pair in fieldDict)
                    {
                        int index = pair.Key;

                        string value = excelReader.IsDBNull(index) ? "" : excelReader.GetString(index);

                        if (!string.IsNullOrEmpty(p2)) p2 += ",";
                        if (!string.IsNullOrEmpty(p3)) p3 += ",";

                        p2 += "`" + pair.Value + "`";
                        p3 += "\"" + value + "\"";
                    }
                    insertsql = string.Format(insertsql, p1, p2, p3);
                    ExcecuteCMD(conn, insertsql);
                }
                else
                {
                    Logger.LogError(string.Format("rowRead Error：{0} {1}", tableName, excelReader.GetString(1)));
                }
                rowCount++;
            }
        }


        //建立MySql数据库连接
        public static MySqlConnection openConn()
        {
            string M_str_sqlcon = "server=192.168.1.119;user id=root;password=123456;database=orcwarconf";
            MySqlConnection myCon = new MySqlConnection(M_str_sqlcon);
            myCon.Open();
            return myCon;
        }

        //执行MySqlCommand命令
        public static void ExcecuteCMD(MySqlConnection con, string M_str_sqlstr)
        {
            //Logger.Log("execute sql :" + M_str_sqlstr);
            MySqlCommand mysqlcom = new MySqlCommand(M_str_sqlstr, con);
            mysqlcom.ExecuteNonQuery();
            mysqlcom.Dispose();
        }

        public static void CloseConn(MySqlConnection mysqlcon)
        {
            mysqlcon.Close();
            mysqlcon.Dispose();
        }

        [MenuItem("program/Config/ConvertOneTable", false, 0)]
        private static void BuildOneTable()
        {
            if (!Directory.Exists(Path_GenerateExcel))
                Directory.CreateDirectory(Path_GenerateExcel);

            string path = EditorUtility.OpenFilePanel("Select Excel", CSVPathEditor.Instance.Path_ExcelInputDir, "xlsx");
            path = path.Replace("\\", "/");
            if (string.IsNullOrEmpty(path)) return;
            TableConfig tableConfig = null;
            for (int i = 0; i < CSVPathEditor.Instance.tableList.Count; i++)
            {
                string shortPath = CSVPathEditor.Instance.tableList[i].shortPath;
                shortPath = shortPath.Replace("\\", "/");

                if (path.IndexOf(shortPath) > -1)
                {
                    tableConfig = CSVPathEditor.Instance.tableList[i];
                    break;
                }
            }
            if (tableConfig != null)
            {
                if (!string.IsNullOrEmpty(tableConfig.rowClassName))
                {
                    BuildOneTable(tableConfig.tableClassName, tableConfig.rowClassName, path);
                }
                else
                {
                    BuildOneTable(tableConfig.tableClassName, path);
                }
            }
            else
            {
                Logger.LogError("cannot find table config");
            }
            EditorUtility.ClearProgressBar();
            Logger.Log("Convert Complete!");
        }

        private static void BuildOneTable(string tableTypeName, string rowTypeName, string path)
        {
            Assembly amb = System.AppDomain.CurrentDomain.Load("Assembly-CSharp");
            Type tableType = amb.GetType(tableTypeName);
            Type rowType = amb.GetType(rowTypeName);

            if (tableType == null || rowType == null)
            {
                Logger.LogError(string.Format("table type not find :{0}", tableTypeName));
                return;
            }

            EditorUtility.DisplayProgressBar("ConvertTable", path, UnityEngine.Random.value);

            FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            //do  //只读第一个sheet
            //{
            excelReader.Read();

            string outputPath = Path_GenerateExcel + "/" + tableTypeName + ".asset";

            ITable newHeroTable = AssetDatabase.LoadAssetAtPath<ITable>(outputPath);
            if (newHeroTable == null)
            {
                newHeroTable = (ITable)Activator.CreateInstance(tableType);
                AssetDatabase.CreateAsset(newHeroTable, outputPath);
            }
            newHeroTable.Clear();

            Dictionary<int, string> fieldDict = new Dictionary<int, string>();
            Dictionary<int, FieldInfo> fieldInfoList = new Dictionary<int, FieldInfo>();

            for (int i = 0; i < excelReader.FieldCount; i++)
            {
                if (!excelReader.IsDBNull(i))
                {
                    string fieldName = excelReader.GetString(i);

                    FieldInfo fieldInfo = rowType.GetField(fieldName,
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (fieldInfo != null)
                    {
                        fieldDict.Add(i, fieldName);
                        fieldInfoList.Add(i, fieldInfo);
                    }
                }
            }

            excelReader.Read();
            excelReader.Read();
            excelReader.Read();
            for (int i = 0; i < excelReader.FieldCount; i++)
            {
                if (!excelReader.IsDBNull(i))
                {
                    string ABC = excelReader.GetString(i);
                    if (ABC != "A" && ABC != "C")
                    {
                        fieldDict.Remove(i);
                        fieldInfoList.Remove(i);
                    }
                }
            }

            int rowCount = 5;
            while (excelReader.Read())
            {
                if (!excelReader.IsDBNull(0) && !string.IsNullOrEmpty(excelReader.GetString(0)))
                {
                    IRow rowCVO = (IRow)Activator.CreateInstance(rowType);
                    foreach (var pair in fieldDict)
                    {
                        int index = pair.Key;

                        string value = excelReader.IsDBNull(index) ? "" : excelReader.GetString(index);
                        if (!ConvertUtil.SetFieldValue(fieldInfoList[index], rowCVO, value))
                        {
                            Logger.LogError(tableTypeName + " row:" + rowCount);
                        }
                    }
                    newHeroTable.AddRow(rowCVO);
                }
                else
                {
                    Logger.LogError(string.Format("rowRead Error：{0} {1}", tableTypeName, excelReader.GetString(1)));
                }
                rowCount++;
            }
            //} while (excelReader.NextResult());

            //Debug.Log(path + " Complete!");

            EditorUtility.SetDirty(newHeroTable);
            AssetDatabase.SaveAssets();
        }

        private static void BuildOneTable(string tableName, string path)
        {
            EditorUtility.DisplayProgressBar("ConvertTable", path, UnityEngine.Random.value);

            FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            //do  //只读第一个sheet
            //{
            excelReader.Read();

            string outputPath = Path_GenerateExcel + "/" + tableName + ".asset";
            if (path.IndexOf("LanguageLocal.xlsx") > -1)
            {
                outputPath = "Assets/Resources/" + tableName + ".asset";
            }
            else
            {
                outputPath = Path_GenerateExcel + "/" + tableName + ".asset";
            }

            NewCSVFile newCSVFile = AssetDatabase.LoadAssetAtPath<NewCSVFile>(outputPath);

            if (newCSVFile == null)
            {
                newCSVFile = new NewCSVFile();
                AssetDatabase.CreateAsset(newCSVFile, outputPath);
            }
            newCSVFile.Clear();

            newCSVFile.tableName = tableName;

            Dictionary<int, string> fieldDict = new Dictionary<int, string>();
            for (int i = 0; i < excelReader.FieldCount; i++)
            {
                if (!excelReader.IsDBNull(i))
                {
                    string fieldName = excelReader.GetString(i);
                    fieldDict.Add(i, fieldName);
                }
            }

            excelReader.Read();
            excelReader.Read();
            excelReader.Read();
            for (int i = 0; i < excelReader.FieldCount; i++)
            {
                if (!excelReader.IsDBNull(i))
                {
                    string ABC = excelReader.GetString(i);
                    if (ABC != "A" && ABC != "C")
                    {
                        fieldDict.Remove(i);
                    }
                }
            }
            /////
            string retField = string.Empty;
            foreach (var pair in fieldDict)
            {
                if (!string.IsNullOrEmpty(retField))
                    retField += "^";
                retField += pair.Value;
            }
            newCSVFile.dataList.Add(retField);
            /////
            while (excelReader.Read())
            {
                string ret = string.Empty;
                foreach (var pair in fieldDict)
                {
                    int index = pair.Key;
                    string value = excelReader.IsDBNull(index) ? "" : excelReader.GetString(index);

                    if (!string.IsNullOrEmpty(ret))
                        ret += "^";
                    ret += value;
                }
                newCSVFile.dataList.Add(ret);
            }
            //} while (excelReader.NextResult());
            //Debug.Log(path + " Complete!");
            EditorUtility.SetDirty(newCSVFile);
            AssetDatabase.SaveAssets();
        }


        [MenuItem("program/Config/BuildAllServerTable", false, 0)]
        private static void BuildAllServerTable()
        {
            if (!Directory.Exists(Path_GenerateExcel))
                Directory.CreateDirectory(Path_GenerateExcel);

            try
            {
                Logger.Log(CSVPathEditor.Instance.Path_ExcelInputDir);
                List<string> list = new List<string>();
                GetAllFile(CSVPathEditor.Instance.Path_ExcelInputDir, list);

                for (int i = 0; i < list.Count; i++)
                {
                    string path = list[i];
                    path = path.Replace("\\", "/");
                    int tableNameIndex = path.LastIndexOf('/');
                    int tableNameIndex2 = path.LastIndexOf('.');
                    string tableName = path.Substring(tableNameIndex + 1, tableNameIndex2 - tableNameIndex - 1);
                    if (string.IsNullOrEmpty(path)) return;

                    MySqlConnection conn = openConn();
                    BuildOneServerTable(conn, path, "df_" + tableName.ToLower());
                    CloseConn(conn);
                }
                Logger.Log("Convert Complete!");
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("提示", "打表成功", "好的");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("提示", "打表失败！！！！！", "好的");
            }
        }

        public static void GetAllFile(string path, List<string> FileList)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] fil = dir.GetFiles();
            DirectoryInfo[] dii = dir.GetDirectories();
            foreach (FileInfo f in fil)
            {
                FileList.Add(f.FullName); //添加文件路径到列表中  
            }
            //获取子文件夹内的文件列表，递归遍历  
            foreach (DirectoryInfo d in dii)
            {
                GetAllFile(d.FullName, FileList);
            }
        }

        [MenuItem("program/Config/ConfigBuildOneServerTable", false, 0)]
        private static void BuildOneServerTable()
        {
            if (!Directory.Exists(Path_GenerateExcel))
                Directory.CreateDirectory(Path_GenerateExcel);

            try
            {
                string path = EditorUtility.OpenFilePanel("Select Excel", CSVPathEditor.Instance.Path_ExcelInputDir, "xlsx");
                path = path.Replace("\\", "/");
                int tableNameIndex = path.LastIndexOf('/');
                int tableNameIndex2 = path.LastIndexOf('.');
                string tableName = path.Substring(tableNameIndex + 1, tableNameIndex2 - tableNameIndex - 1);
                if (string.IsNullOrEmpty(path)) return;

                MySqlConnection conn = openConn();
                BuildOneServerTable(conn, path, "df_" + tableName.ToLower());
                CloseConn(conn);

                Logger.Log("Convert Complete!");
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("提示", "打表成功", "好的");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("提示", "打表失败！！！！！", "好的");
            }
        }



    }
}

