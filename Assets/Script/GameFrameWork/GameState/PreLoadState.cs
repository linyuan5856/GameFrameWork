using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GFW
{
    public class PreLoadState : BaseState
    {

        protected override void OnInitStage()
        {
            base.OnInitStage();

            LoaderManager.Instance.LoadSceneAsync(GameDefine.PreLoadScene,null,this.LoadGameConfig);         
        }
    
        void LoadGameConfig()
        {
            string configPath = GameConfigPVO.Instance.GetGameConfigPath();
            LoaderManager.HttpGetText(configPath, (text, error) =>
             {
                 if (!string.IsNullOrEmpty(error))
                 {
                     GameLogger.LogError(string.Format("Load  {1} Failed -->Error:  {2} ", configPath, error));
                     return;
                 }
                 ConvertUtil.ToPVO(text, GameConfigPVO.Instance);
                 GameLogger.logLevel = GameConfigPVO.Instance.logLevel;
                 GameLogger.Log("Load GameConfig OK --> CDN Path:" + GameConfigPVO.Instance.cdnUrl);
                 InitExtraGameManager();
                 InitVersion();
             });
        }

        void InitExtraGameManager()
        {      
            GameObject mainGo = MainGame.Instance.gameObject;

            if (GameConfigPVO.Instance.useRemoteLog)
                GameUtil.AddComponentToP<LogTcpManager>(mainGo);
            if (GameConfigPVO.Instance.usePerformance)
                GameUtil.AddComponentToP<Performance>(mainGo);

            MsgManager.Instance.Regist();

            GameLogger.Log("GameManager  Init OK");
        }

        void InitVersion()
        {
            LoadLocalVersion();
        }

        void LoadLocalVersion()
        {
            string path = VersionManager.Instance.GetStreamingPath() + GameDefine.VERSIONTXT;
            LoaderManager.HttpGetText(path, (text, error) =>
            {
                if (!string.IsNullOrEmpty(error))
                {
                    GameLogger.LogError("LoadLocalVersion Error:" + error);
                }
                else
                {
                    GameLogger.LogTest("LocalVersion:" + text);
                    VersionManager.Instance.SetLocalVersion(text);
                    this.LoadServerVersion();
                }
            });

            GameLogger.Log("Bundle Path:" + GameConfigPVO.Instance.getCDNBundleUrl());
        }

        void LoadServerVersion()
        {
            string path = GameUtil.AddString(GameConfigPVO.Instance.getCDNBundleUrl(), GameDefine.VERSIONTXT, "?r=", UnityEngine.Random.Range(0, 9999999));

            LoaderManager.HttpGetText(path, (text, error) =>
            {
                GameLogger.LogTest("ServerVersionPath:" + path);

                if (!string.IsNullOrEmpty(error))
                {
                    GameLogger.LogError(string.Format("LoadSeverVersion Error: {0} .. URL {1} ", error, path));
                    return;
                }

                GameLogger.LogTest("SeverVersion:" + text);

                VersionManager.Instance.SetServerVersion(text);

                this.LoadSeverList();
            });
        }

        void LoadSeverList()
        {
            GameLogger.Log("Version Compare");

            if (VersionManager.Instance.LocalVersion != VersionManager.Instance.SeverVersion)
            {
                GameLogger.Log(string.Format("服务器 与 本地版本号不同 Sever -->{0}  .. Client-->{1}", VersionManager.Instance.SeverVersion, VersionManager.Instance.LocalVersion));
            }
            else
            {
                GameLogger.Log("服务器列表 读取完毕");
            }

            GameLogger.Log("版本检测通过 开始读取服务器列表");
            string url = GameConfigPVO.Instance.getCDNBundleUrl();
            LoaderManager.HttpGetText(url + "/ServerList.csv?" + UnityEngine.Random.Range(0, 9999999), (txt, err) =>
            {
                if (!string.IsNullOrEmpty(err))
                {
                    Debug.LogError("Load SeverList Error :" + err);
                    return;
                }


                CSVFile csvFile = ConvertUtil.ToNewCSVFile(txt);
                csvFile.InitTable("ID");
                for (int ID = 1; ID <= csvFile.Count; ID++)
                {//读表初始化服务器集合

                    if (!csvFile.ContainsKey(ID.ToString()))
                        continue;
                    CSVRow row = csvFile.GetRowByKey(ID.ToString());

                    StringBuilder sb = new StringBuilder();//todo
                    for (int i = 0; i < row.rowArr.Length; i++)
                    {
                        sb.Append(row.rowArr[i].ToString()).Append("|");

                    }
                    GameLogger.LogWarn(sb.ToString());
                }

                this.DownLoadAssetBundles();
            });
        }

        void DownLoadAssetBundles()
        {
            LoaderManager.Instance.PreLoadGameAssetBundles(this.DownLoadCSV_Table);
        }

        void DownLoadCSV_Table()
        {
            GameUtil.StartCoroutine(CO_LoadTable());
        }

        IEnumerator CO_LoadTable()
        {
            int totalCount = PreLoadAssetCatalog.TableList.Count;
            for (int i = 0; i < totalCount; i++)
            {
                string tableName = PreLoadAssetCatalog.TableList[i];
                GameLogger.Log("load table:" + tableName);
                long time = DateTime.Now.Ticks;
                LoaderManager.Instance.LoadTable(tableName);
                long costTime = (DateTime.Now.Ticks - time) / 10000;
                if (costTime > 10)
                    GameLogger.LogWarn(tableName + " cost time:" + costTime);
                yield return null;
            }

            // ConvertUtil.NewCSVToStaticClass((NewCSVFile)LoaderManager.Instance.LoadTable("ActivityBase", "Key"), typeof(CSV_ActivityBase));
            GameLogger.Log("Load Table Done");
            StartLoadAsset();
        }

        void StartLoadAsset()
        {
            LoaderManager.Instance.PreLoadGameAseets(BeginGame);
        }

        void BeginGame()
        {
            GameLogger.Log("Preload Ready-->Enter Game");
            MainGame.Instance.ChangeState<GameState>();
        }

    }
}
