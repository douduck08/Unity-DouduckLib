using UnityEngine;
/// Ref: http://wiki.unity3d.com/index.php/Singleton
namespace DouduckLib {
    public abstract class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour {
        private static T _instance;
        private static GameObject _gameObject;
        private static object _lock = new object ();

        protected SingletonMono () { }

        public static T instance {
            get {
                if (applicationIsQuitting) {
                    return null;
                }

                lock (_lock) {
                    if (_instance == null) {
                        _instance = (T) FindObjectOfType (typeof (T));
                        if (FindObjectsOfType (typeof (T)).Length > 1) {
                            Debug.LogError ("[Singleton] There should never be more than 1 singleton!");
                            return _instance;
                        }

                        if (_instance == null) {
                            _gameObject = new GameObject ();
                            _instance = _gameObject.AddComponent<T> ();
                            _gameObject.name = "[singleton] " + typeof (T).Name;

                            DontDestroyOnLoad (_gameObject);
                            Debug.Log ("[Singleton] An instance of " + typeof (T) + "was created with DontDestroyOnLoad.");
                        }
                    }
                    return _instance;
                }
            }
        }

        private static bool applicationIsQuitting = false;
        public void OnDestroy () {
            applicationIsQuitting = true;
        }
    }
}