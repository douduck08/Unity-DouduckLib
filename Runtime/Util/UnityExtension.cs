using System.Collections;
using UnityEngine;

namespace DouduckLib
{
    public static class UnityExtension
    {
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            if (!component.TryGetComponent<T>(out var result))
            {
                result = component.gameObject.AddComponent<T>();
            }
            return result;
        }

        public static bool IsPrefab(this GameObject go)
        {
            return go.scene.rootCount == 0;
        }
    }
}
