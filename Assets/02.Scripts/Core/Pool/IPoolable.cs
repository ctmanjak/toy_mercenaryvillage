namespace Core
{
    public interface IPoolable
    {
        void OnGetFromPool();
        void OnReleaseToPool();
    }
}