namespace GameFrameWork
{
    public interface IService
    {
        void Create();

        void Release();

        void DoUpdate(float deltaTime);
        void OnApplicationQuit();
    }
}