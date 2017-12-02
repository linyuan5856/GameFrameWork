using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pandora
{
    public class PreLoadState : BaseState
    {

        protected override void OnInitStage()
        {
            base.OnInitStage();

            this.LoadGameLocalConfig();
        }

        void LoadGameLocalConfig()
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

            GameLogger.Log("Extra Manager  Init OK");
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
                    VersionManager.Instance.SetLocalVersion(text);
                    this.LoadServerVersion();
                    GameLogger.Log(string.Format("LocalVersion:-->{0} \n Bundle Path:-->{1}", VersionManager.Instance.LocalVersion, GameConfigPVO.Instance.GetCdnBundleUrl()));
                }
            });

        }

        void LoadServerVersion()
        {
            string path = GameUtil.AddString(GameConfigPVO.Instance.GetCdnBundleUrl(), GameDefine.VERSIONTXT, "?r=", UnityEngine.Random.Range(0, 9999999));

            LoaderManager.HttpGetText(path, (text, error) =>
            {
                GameLogger.LogTest("ServerVersionPath:" + path);

                if (!string.IsNullOrEmpty(error))
                {
                    GameLogger.LogError(string.Format("LoadSeverVersion Error: {0} .. URL {1} ", error, path));
                    return;
                }

                VersionManager.Instance.SetServerVersion(text);
                GameLogger.Log(string.Format("SeverVersion:-->{0} ", VersionManager.Instance.SeverVersion));

                this.LoadSeverList();
            });
        }

        void LoadSeverList()
        {
            GameLogger.Log("Version Compare");

            if (VersionManager.Instance.LocalVersion != VersionManager.Instance.SeverVersion)
            {
                GameLogger.Log(string.Format("服务器 与 本地版本号不同 Sever -->{0}  .. Client-->{1}", VersionManager.Instance.SeverVersion, VersionManager.Instance.LocalVersion));
                //todo  版本不同的回调
            }
            else
            {
                GameLogger.Log("服务器列表 读取完毕");
            }

            GameLogger.Log("版本检测通过 开始读取服务器列表");
            string url = GameConfigPVO.Instance.GetCdnBundleUrl();
            LoaderManager.HttpGetText(GameUtil.AddString(url, GameDefine.SEVERLISTTXT, UnityEngine.Random.Range(0, 9999999)), (txt, err) =>
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
#if UNITY_EDITOR
                     StringBuilder sb = new StringBuilder();
                     for (int i = 0; i < row.rowArr.Length; i++)
                     {
                         sb.Append(row.rowArr[i].ToString()).Append("|");

                     }
                     GameLogger.LogWarn(sb.ToString());
#endif

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
            LoaderManager.Instance.PreLoadTable(this.StartLoadAsset);
        }

        void StartLoadAsset()
        {
            LoaderManager.Instance.PreLoadGameAssets(BeginGame);
        }

        void BeginGame()
        {
            GameLogger.Log("Preload Ready-->Enter Game");
            MainGame.Instance.ChangeState<TemplateState>();
        }

    }
}
