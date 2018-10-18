using UnityEngine;
using UnityEngine.EventSystems;

namespace Pandora
{
    public class MainGame : MonoSingleton<MainGame>
    {
        private StateMachine stateMachine;
        private const int LowFrame = 30;
        private const int HighFrame = 40;
        private int _gameFrame;

        protected override void Init()
        {
            base.Init();
            this.InitGameSetting();
            this.InitGameManager();
            this.InitGameState();
        }

       
        void InitGameSetting()
        {          
            DontDestroyOnLoad(this.gameObject);
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Screen.orientation = ScreenOrientation.Landscape;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            QualitySettings.vSyncCount = 0;  
            Instance.SetGameFrame(LowFrame);

        }

        void InitGameManager()
        {
            GameObject mainGo = Instance.gameObject;
            GameUtil.AddComponentToP<GameUpdateRunner>(mainGo);
            GameUtil.AddComponentToP<LoaderManager>(mainGo);
            GameUtil.AddComponentToP<AudioManager>(mainGo);
            GameUtil.AddComponentToP<TcpManager>(mainGo);
            GameUtil.AddComponentToP<ChatTcpManager>(mainGo);
            GameUtil.AddComponentToP<TimerManager>(mainGo);
            GameUtil.AddComponentToP<UIManager>(mainGo);
            MsgManager.Instance.Init();
        }

        void InitGameState()
        {
            stateMachine = new StateMachine();
            stateMachine.RegisterState(new PreLoadState());
            stateMachine.RegisterState(new GameState());
            stateMachine.RegisterState(new TemplateState());
            Instance.ChangeState<PreLoadState>();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            stateMachine.OnUpdate();
        }

        public void ChangeState<T>(object param = null)
        {
            stateMachine.ChangeState<T>(param);
        }

        public void SetGameFrame(int frame)
        {
            if (_gameFrame == frame) return;
            _gameFrame = frame;
            Application.targetFrameRate = _gameFrame;
        }

    }
}