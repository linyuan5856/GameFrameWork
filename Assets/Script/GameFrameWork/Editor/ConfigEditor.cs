using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using System.Runtime.CompilerServices;
using Excel;
using MySql.Data.MySqlClient;
using UnityEngine;

namespace GFW
{
    public class ConfigEditor
    {

        public const string Path_GenerateExcel = "Assets/Script/GameFrameWork/Config/CSVScriptObject";
        public const string Path_EditorConfig = "Assets\\Script\\GameFrameWork\\Config\\EditorConfig.asset";

        [NonSerialized]
        static List<TableConfig> tableList = new List<TableConfig>()
        {
        //new TableConfig("ActiveTaskReward","","\\Task\\ActiveTaskReward.xlsx"),
        //new TableConfig("PurchaseTimes","","\\PurchaseTimes\\PurchaseTimes.xlsx"),
        //new TableConfig("PurchasePrice","","\\PurchaseTimes\\PurchasePrice.xlsx"),
        //new TableConfig("Errorcode","","\\Resources\\Errorcode.xlsx"),
        //new TableConfig("LanguageSelection","","\\Resources\\LanguageSelection.xlsx"),
        //new TableConfig("LanguageLocal","","\\Resources\\LanguageLocal.xlsx"),
        //new TableConfig("Notice","","\\Notice\\Notice.xlsx"),
        //new TableConfig("Music","","\\Music\\Music.xlsx"),
        //new TableConfig("RandomName","","\\Role\\RandomName.xlsx"),
        //new TableConfig("Actor","","\\Map\\Actor.xlsx"),
        //new TableConfig("Dummy","","\\Resources\\Dummy.xlsx"),
        //new TableConfig("Language","","\\Resources\\Language.xlsx"),
        //new TableConfig("ActivityBase","","\\Activity\\ActivityBase.xlsx"),
        //new TableConfig("ArenaBase", "", "\\Arena\\ArenaBase.xlsx"),
        //new TableConfig("ArmyBase", "", "\\Army\\ArmyBase.xlsx"),
        //new TableConfig("DefendCityBase", "", "\\Map\\DefendCityBase.xlsx"),
        //new TableConfig("EquipBase", "", "\\Equip\\EquipBase.xlsx"),
        //new TableConfig("FightBase", "", "\\Fight\\FightBase.xlsx"),
        //new TableConfig("FriendBase", "", "\\Friend\\FriendBase.xlsx"),
        //new TableConfig("GuideBase", "", "\\Task\\GuideBase.xlsx"),
        //new TableConfig("HeroBase", "", "\\Hero\\HeroBase.xlsx"),
        //new TableConfig("JumpUIBase", "", "\\Task\\JumpUIBase.xlsx"),
        //new TableConfig("LadderMatchBase", "", "\\LadderMatch\\LadderMatchBase.xlsx"),
        //new TableConfig("MonsterBase", "", "\\Fight\\MonsterBase.xlsx"),
        //new TableConfig("RoleBase", "", "\\Role\\RoleBase.xlsx"),
        //new TableConfig("RuneBase", "", "\\Rune\\RuneBase.xlsx"),
        //new TableConfig("TavernBase", "", "\\Tavern\\TavernBase.xlsx"),
        //new TableConfig("TrialBase", "", "\\Trial\\TrialBase.xlsx"),
        //new TableConfig("VipBase", "", "\\Vip\\VipBase.xlsx"),
        //new TableConfig("ChapterBase", "", "\\HeroChapter\\ChapterBase.xlsx"),

        //new TableConfig("MailCTable","MailCVO","\\Mail\\Mail.xlsx"),
        //new TableConfig("RuneCTable", "RuneCVO","\\Rune\\Rune.xlsx"),
        //new TableConfig("RuneOpenCTable", "RuneOpenCVO","\\Rune\\RuneOpen.xlsx"),
        //new TableConfig("ShopCTable", "ShopCVO","\\Shop\\Shop.xlsx"),
        //new TableConfig("Welfare_ActivityCTable", "Welfare_ActivityCVO","\\Activity\\ActivityList.xlsx"),
        //new TableConfig("Welfare_SevenDayGiftCTable", "Welfare_SevenDayGiftCVO","\\Activity\\Login.xlsx"),
        //new TableConfig("ShineTowerCTable", "ShineTowerCVO","\\Trial\\Trial.xlsx"),
        //new TableConfig("VipMoneyTreeCTable", "VipMoneyTreeCVO","\\Vip\\MoneyTree.xlsx"),
        //new TableConfig("VipDiamondCTable", "VipDiamondCVO","\\Vip\\VipDiamond.xlsx"),
        //new TableConfig("VipLevelCTable","VipLevelCVO","\\Vip\\VipLevel.xlsx"),
        //new TableConfig("RechargeCTable", "RechargeCVO","\\Vip\\Recharge.xlsx"),
        //new TableConfig("FightMapCTable", "FightMapCVO","\\Fight\\FightMap.xlsx"),
        //new TableConfig("FightMapGridCTable","FightMapGridCVO","\\Fight\\FightMapGrid.xlsx"),
        //new TableConfig("ShopItemCTable", "ShopItemCVO","\\Shop\\ShopExchange.xlsx"),
        //new TableConfig("CoinCTable","CoinCVO","\\Item\\Coin.xlsx"),
        //new TableConfig("GemThomasTable", "GemThomasCVO","\\Arena\\GemThomas.xlsx"),
        //new TableConfig("DialogCTable", "DialogCVO","\\Task\\Dialog.xlsx"),
        //new TableConfig("GuideCTable", "GuideCVO","\\Task\\Guide.xlsx"),
        //new TableConfig("GuideOpenCTable", "GuideOpenCVO","\\FunctionOpen\\GuideOpen.xlsx"),
        //new TableConfig("RankConfigCTable", "RankConfigCVO","\\Rank\\Rank.xlsx"),
        //new TableConfig("RankRewardCTable", "RankRewardCVO","\\Rank\\RankReward.xlsx"),
        //new TableConfig("Army_BulidCTable", "ArmyBulidCVO","\\Army\\ArmyBuilding.xlsx"),
        //new TableConfig("Army_DonateCTable", "Army_DonateCVO","\\Army\\ArmyDonate.xlsx"),
        //new TableConfig("Army_OfficalCTable", "Army_OfficalCVO","\\Army\\ArmyMember.xlsx"),
        //new TableConfig("Army_RedCTable", "Army_RedCVO","\\Army\\ArmyRed.xlsx"),
        //new TableConfig("Army_HelpCTable", "Army_HelpCVO","\\Army\\ArmyHelp.xlsx"),
        //new TableConfig("Army_LevelCTable", "Army_LevelCVO","\\Army\\ArmyLevel.xlsx"),
        //new TableConfig("RoleExpCTable", "RoleExpCVO","\\Role\\RoleExp.xlsx"),
        new TableConfig("ItemCTable", "ItemCVO","\\Item\\Item.xlsx"),
        //new TableConfig("MonsterCTable","MonsterCVO","\\Fight\\Monster.xlsx"),
        //new TableConfig("EquipStrengthenCTable", "EquipStrengthenCVO","\\Equip\\EquipStrengthen.xlsx"),
        //new TableConfig("EquipCTable", "EquipCVO","\\Equip\\Equip.xlsx"),
        //new TableConfig("HeroCTable", "HeroCVO","\\Hero\\Hero.xlsx"),
        //new TableConfig("HeroBreakCTable", "HeroBreakCVO","\\Hero\\HeroBreak.xlsx"),
        //new TableConfig("TaskCTable", "TaskCVO","\\Task\\Task.xlsx"),
        //new TableConfig("StoryBoxCTable", "StoryBoxCVO","\\Story\\StoryBox.xlsx"),
        //new TableConfig("StoryEventCTable", "StoryEventCVO","\\Story\\StoryEvent.xlsx"),
        //new TableConfig("SectioInfoCTable", "SectioInfoCVO","\\Story\\Story.xlsx"),
        //new TableConfig("LadderRewardCTable", "DanRewardCVO","\\LadderMatch\\LadderMatch.xlsx"),
        //new TableConfig("DanRewardCTable", "DanRewardCVO","\\Arena\\ArenaReward.xlsx"),
        //new TableConfig("ScenceCTable", "ScenceCVO","\\Map\\Scene.xlsx"),
        //new TableConfig("HeroTalentCTable", "HeroTalentCVO","\\Hero\\HeroTalent.xlsx"),
        //new TableConfig("HeroExpCTable", "HeroExpCVO","\\Hero\\HeroExp.xlsx"),
        //new TableConfig("SkillInfoCTable", "SkillInFoCVO","\\Skill\\Skill.xlsx"),
        //new TableConfig("SkillElementCTable", "SkillElementCVO","\\Skill\\SkillElement.xlsx"),
        //new TableConfig("SkillShowCTable", "SkillShowCVO","\\Skill\\SkillShow.xlsx"),
        //new TableConfig("SkillUpCTable", "SkillUpCVO","\\Skill\\SkillUp.xlsx"),
        //new TableConfig("BattleHallCTable", "BattleHallCVO","\\BattleHall\\BattleHall.xlsx"),
        //new TableConfig("FunctionOpenCTable", "FunctionOpenCVO","\\FunctionOpen\\FunctionOpen.xlsx"),
        //new TableConfig("ScenceNpcCTable", "ScenceNpcCVO","\\Map\\SceneNpc.xlsx"),
        //new TableConfig("SignCTable", "SignCVO","\\Activity\\Sign.xlsx"),
        //new TableConfig("Welfare_RechargeCTable", "Welfare_RechargeCVO","\\Activity\\TotalRecharge.xlsx"),
        //new TableConfig("CarnivalCTable", "CarnivalCVO","\\Activity\\Carnival.xlsx"),
        //new TableConfig("SuccinctLibraryCTable", "SuccinctLibraryCVO","\\Equip\\SuccinctLibrary.xlsx"),
        //new TableConfig("HeroTavernCTable", "HeroTavernCVO","\\Tavern\\TavernWeek.xlsx"),
        //new TableConfig("NoticeCTable", "NoticeCVO","\\Notice\\Announcement.xlsx"),
        //new TableConfig("TeamFightCTable", "TeamFightCVO","\\Team\\TeamFight.xlsx"),
        //new TableConfig("SecretFieldCTable", "SecretFieldCVO","\\Map\\SecretField.xlsx"),
        //new TableConfig("EventCTable", "EventCVO","\\Map\\Event.xlsx"),
        //new TableConfig("DailyTaskCVOCTable", "DailyTaskCVO","\\Task\\ActiveScore.xlsx"),
        //new TableConfig("ChapterCTable","chapterCVO","\\HeroChapter\\Chapter.xlsx"),
        //new TableConfig("ChapterRewardCTable","ChapterRewardCVO","\\HeroChapter\\ChapterReward.xlsx"),
        //new TableConfig("Welfare_PlayerLvCTable","Welfare_PlayerLvCVO","\\Activity\\LevelReward.xlsx"),
        //new TableConfig("Welfare_FirstRechargeCTable","Welfare_FirstRechargeCVO","\\Activity\\FirstCharge.xlsx"),
        //new TableConfig("Welfare_FianceCTable","Welfare_FianceCVO","\\Activity\\FiancePlan.xlsx"),
        //new TableConfig("Welfare_ConsumeCTable","Welfare_ConsumeCVO","\\Activity\\TotalConsume.xlsx")
        };

        static CSVPathEditor _csvPathEditor;

        static CSVPathEditor CsvPathEditor
        {
            get
            { 
                GenerateCsvPathEditor();
                return _csvPathEditor;
            }         
        }

        [MenuItem("program/Config/GenerateCSVPathEditor", false, 0)]
        private static void GenerateCsvPathEditor()
        {
            if (_csvPathEditor!=null)
            {
                return;
            }

            _csvPathEditor = AssetDatabase.LoadAssetAtPath<CSVPathEditor>(Path_EditorConfig);
            if (_csvPathEditor == null)
            {
                _csvPathEditor = ScriptableObject.CreateInstance<CSVPathEditor>();
                AssetDatabase.CreateAsset(CsvPathEditor, Path_EditorConfig);
            }
            Selection.activeObject = CsvPathEditor;
        }

        [MenuItem("program/Config/ConvertAllTable", false, 0)]
        private static void BuildAllTable()
        {
            if (!Directory.Exists(Path_GenerateExcel))
                Directory.CreateDirectory(Path_GenerateExcel);

            //
            TableConfig tableConfig = null;
            for (int i = 0; i < tableList.Count; i++)
            {
                tableConfig =tableList[i];
                if (!string.IsNullOrEmpty(tableConfig.rowClassName))
                {
                    BuildOneTable(tableConfig.tableClassName, tableConfig.rowClassName,
                        CsvPathEditor.Path_ExcelInputDir + tableConfig.shortPath);
                }
                else
                {
                    BuildOneTable(tableConfig.tableClassName,
                        CsvPathEditor.Path_ExcelInputDir + tableConfig.shortPath);
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

            string path = EditorUtility.OpenFilePanel("Select Excel", CsvPathEditor.Path_ExcelInputDir, "xlsx");
            path = path.Replace("\\", "/");
            if (string.IsNullOrEmpty(path)) return;
            TableConfig tableConfig = null;
            for (int i = 0; i < tableList.Count; i++)
            {
                string shortPath = tableList[i].shortPath;
                shortPath = shortPath.Replace("\\", "/");

                if (path.IndexOf(shortPath) > -1)
                {
                    tableConfig =tableList[i];
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
                Logger.Log(CsvPathEditor.Path_ExcelInputDir);
                List<string> list = new List<string>();
                GetAllFile(CsvPathEditor.Path_ExcelInputDir, list);

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
                string path = EditorUtility.OpenFilePanel("Select Excel", CsvPathEditor.Path_ExcelInputDir, "xlsx");
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

