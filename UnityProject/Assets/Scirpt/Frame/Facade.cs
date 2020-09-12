using GameFrameWork;


public interface IFacade
{
    T GetService<T>() where T : class, IService;
}

public class Facade : IFacade
{
    private Facade()
    {
    }

    private static Facade _facade;

    public static Facade CreateFacade()
    {
        if (_facade == null)
            _facade = new Facade();
        else
            Debuger.LogError("repeat create facade");
        return _facade;
    }

    public ServiceLocate Locate;

    public T GetService<T>() where T : class, IService
    {
        return Locate.GetService<T>();
    }
}