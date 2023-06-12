namespace DouduckLib
{
    public abstract class Singleton<T> where T : class, new()
    {
        private static readonly object lock_ = new();
        private static T instance_ = null;

        public static T Get()
        {
            lock (lock_)
            {
                instance_ ??= new T();
                return instance_;
            }
        }
    }
}
