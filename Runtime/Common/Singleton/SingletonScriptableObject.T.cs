using System.Linq;
using UnityEngine;

namespace DouduckLib
{
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        static T instance_ = null;

        public static T Get()
        {
            if (!instance_)
            {
                instance_ = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
            }
            return instance_;
        }
    }
}
