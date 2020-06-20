using UnityEngine;

namespace DouduckLib {
    public abstract class GameObjectPoolSetting<TObject> where TObject : Component {
        [SerializeField] TObject _prefab;
        public TObject prefab {
            get {
                return _prefab;
            }
        }

        [SerializeField] ObjectPoolSettings _poolSetting;
        public ObjectPoolSettings poolSetting {
            get {
                return _poolSetting;
            }
        }
    }
}