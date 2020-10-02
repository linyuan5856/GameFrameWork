using UnityEngine;

public class GameLogicManager : MonoBehaviour
{
    private Facade _facade;


    public GameLogicManager(Facade facade)
    {
        _facade = facade;
        DoNextStep();
    }

    //todo for test
    private void DoNextStep()
    {
        DemoBootStrap boot = GetComponent<DemoBootStrap>();

        if (boot != null)
            return;
        GameObject go = new GameObject("demoBootStrap");
        go.AddComponent<DemoBootStrap>();
    }
}