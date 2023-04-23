using System.Collections;
using UnityEngine;

namespace DouduckLib
{
    public static class UnityExtension
    {
        public static T GetOrAddComponent<T>(this Component trans) where T : Component
        {
            T result = trans.GetComponent<T>();
            if (result == null)
            {
                result = trans.gameObject.AddComponent<T>();
            }
            return result;
        }

        public static bool IsPrefab(this GameObject go)
        {
            return go.scene.rootCount == 0;
        }
    }
}
