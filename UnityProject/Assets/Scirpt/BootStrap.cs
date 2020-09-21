using GameFrameWork;
using UnityEngine;

public class BootStrap : MonoBehaviour
{
    private ServiceLocate _locate;

    // Start is called before the first frame update
    void Start()
    {
        Debuger.Init();
        Facade facade = Facade.CreateFacade();
        _locate = new ServiceLocate(facade);
        facade.Locate = _locate;
        RegisterAllService(_locate);
        _locate.CreateAllServices();
        var uiContext = new UiServiceContext(facade);
        uiContext.Create();
        _locate.GetService<UIService>().SetContext(uiContext);
    }

    void RegisterAllService(ServiceLocate locate)
    {
        locate.RegisterService<LoaderService>();
        locate.RegisterService<TimerService>();
        locate.RegisterService<UIService>();
    }


    void Update()
    {
        _locate.DoUpdate(Time.deltaTime);
        
        // if(Input.GetKeyDown(KeyCode.Q))
        //     _locate.GetService<UIService>().OpenWindow("UI_Welcome");
    }

    void OnApplicationQuit()
    {
        _locate.OnApplicationQuit();
    }
}