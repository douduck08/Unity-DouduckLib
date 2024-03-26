namespace DouduckLib
{
    public abstract class Singleton<T> where T : class, new()
    {
        private static readonly object _lock = new();
        private static T _instance = null;

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
