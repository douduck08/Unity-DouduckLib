namespace DouduckLib
{
    public abstract class Singleton<T> where T : class, new()
    {
        static readonly object _lock = new();
        static T _instance = null;

        public static T Get()
        {
            lock (_lock)
            {
                _instance ??= new T();
                return _instance;
            }
        }
    }
}
