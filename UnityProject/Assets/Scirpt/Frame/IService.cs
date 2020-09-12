namespace GameFrameWork
{
    public interface IService
    {
        void Create(IFacade facade);
        void Release();

        void DoUpdate(float deltaTime);
        void OnApplicationQuit();
    }
}