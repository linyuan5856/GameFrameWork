using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GFW
{
    public class PreLoadStage : BaseStage
    {

        protected override void OnInitStage()
        {
            base.OnInitStage();

            GameUtil.StartCoroutine(LoadScene());

        }

        protected override void OnLeaveStage()
        {
            base.OnLeaveStage();
        }


        IEnumerator LoadScene()
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(GameDefine.PreLoadScene);
            if (ao != null)
            {
                yield return ao.isDone;
                GameUtil.StartCoroutine(this.loadBaseConfig());
            }
        }


        IEnumerator loadBaseConfig()
        {
            string configPath = string.Empty;

            string fileName = string.Empty;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            fileName = GameDefine.EDITOR_CONFIG;
#else
            fileName = GameDefine.RELEASE_CONFIG;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
           configPath =  Application.streamingAssetsPath+"/"+fileName;
#else
            configPath = "file:///" + Application.streamingAssetsPath + "/" + fileName;
#endif

            WWW www = new WWW(configPath);
            yield return www.isDone;
            if (!string.IsNullOrEmpty(www.error))
            {
                Logger.LogError("LoadConfig Failed:  " + www.error);
                yield break;
            }

            ConvertUtil.ToPVO(www.text, GameConfigPVO.Instance);

            Logger.Log("CDN Path:" + GameConfigPVO.Instance.cdnUrl);
            Logger.Log("Load GameConfig OK");
            InitGame();
        }

        private void InitGame()
        {
            Logger.logLevel = GameConfigPVO.Instance.logLevel;

            GameObject mainGo = MainGame.Instance.gameObject;
            if (GameConfigPVO.Instance.usePerformance)
                mainGo.AddComponent<Performance>();

            AddManager<LoaderManager>(mainGo);
            AddManager<AudioManager>(mainGo);
            AddManager<TcpManager>(mainGo);
            AddManager<ChatTcpManager>(mainGo);
            AddManager<TimerManager>(mainGo);


            if (GameConfigPVO.Instance.useRemoteLog)
                mainGo.AddComponent<LogTcpManager>();

            MsgManager.Instance.Regist();

            Logger.Log("MainGame Init OK");
        }

        void AddManager<T>(GameObject parent) where T:Component
        {
            string name = typeof(T).Name;
            GameObject go = new GameObject(name);
            go.SetParent(parent);
            go.AddComponent<T>();
        }
    }
}
