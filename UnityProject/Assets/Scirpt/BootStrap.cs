using GameFrameWork;
using UnityEngine;

public class BootStrap : MonoBehaviour
{
    private ServiceLocate _locate;

    // Start is called before the first frame update
    void Start()
    {
        Facade facade = Facade.CreateFacade();
        _locate = new ServiceLocate(facade);
        facade.Locate = _locate;
        RegisterAllService(_locate);
        _locate.CreateAllServices();
        _locate.GetService<UIService>().SetContext(new UiServiceContext(facade));
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
    }

    void OnApplicationQuit()
    {
        _locate.OnApplicationQuit();
    }
}