using System.Collections;
using UnityEngine;

public static class UnityExtension {
    public static T GetOrAddComponent<T> (this Component trans) where T : Component {
        T result = trans.GetComponent<T> ();
        if (result == null) {
            result = trans.gameObject.AddComponent<T> ();
        }
        return result;
    }

#if UNITY_5_3
    public static T Instantiate<T> (this GameObject go, GameObject origin, Transform parent) where T : GameObject {
        T go_ = GameObject.Instantiate<T> (origin);
        go_.transform.SetParent (parent);
    }
#endif
}