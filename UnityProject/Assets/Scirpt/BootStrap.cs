using GameFrameWork;
using UnityEngine;

public class BootStrap : MonoBehaviour
{
    private ServiceLocate _locate;
    private Facade _facade;

    void Start()
    {
        Debuger.Init();
        _facade = Facade.CreateFacade();
        _locate = new ServiceLocate(_facade);
        _facade.Locate = _locate;
        RegisterAllService(_locate);
        _locate.CreateAllServices();
    }

    void RegisterAllService(ServiceLocate locate)
    {
        locate.RegisterService<LoaderService>();
        locate.RegisterService<TimerService>();
        var service = locate.RegisterService<UIService>();
        var uiContext = new UiServiceContext(_facade);
        service.SetContext(uiContext);
    }


    void Update()
    {
        _locate.DoUpdate(Time.deltaTime);
    }

    void OnApplicationQuit()
    {
        _locate.OnApplicationQuit();
    }
}