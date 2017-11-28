using UnityEngine;

namespace GFW
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
            this.InitGameState();
        }

       
        void InitGameSetting()
        {
            GameLogger.Log("MainGame Init");
            DontDestroyOnLoad(this.gameObject);
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Screen.orientation = ScreenOrientation.Landscape;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            QualitySettings.vSyncCount = 0;
            Caching.maximumAvailableDiskSpace = 600 * 1024 * 1024;
            Instance.SetGameFrame(LowFrame);

        }

        void InitGameState()
        {
            stateMachine = new StateMachine();
            stateMachine.RegisterState(new PreLoadState());
            stateMachine.RegisterState(new GameState());
            Instance.ChangeState<PreLoadState>();
        }


        void Update()
        {
            stateMachine.OnUpdate();
        }

        public void ChangeState<T>(object param = null)
        {
            stateMachine.ChangeState<T>(param);
        }

        public BaseState CurrentState
        {
            get { return stateMachine.curState as BaseState; }
        }

        public void SetGameFrame(int frame)
        {
            if (_gameFrame == frame) return;
            _gameFrame = frame;
            Application.targetFrameRate = _gameFrame;
        }

    }
}