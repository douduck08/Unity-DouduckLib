using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace DouduckLib {
	public class GameObjectPool<TObject> : GameObjectPoolBase<TObject>, IObjectPool<TObject> where TObject : Component {

        Action<TObject> m_setter;

        public GameObjectPool (ObjectPoolSettings settings, TObject prefab) {
			Initialize (settings, prefab);
		}

        public GameObjectPool (ObjectPoolSettings settings, TObject prefab, Action<TObject> setter) {
            if (setter == null) {
                throw new ArgumentNullException ("[ObjectPool] setter cannot be null");
            }
            m_setter = setter;
            Initialize (settings, prefab);
        }

        public TObject Spawn () {
            var item = GetInternal ();
            if (m_setter != null) {
                m_setter.Invoke (item);
            }
            return item;
        }
	}

	public class GameObjectPool<TObject, TParam1> : GameObjectPoolBase<TObject>, IObjectPool<TObject, TParam1> where TObject : Component {

		Action<TObject, TParam1> m_setter;

		public GameObjectPool (ObjectPoolSettings settings, TObject prefab, Action<TObject, TParam1> setter) {
			if (setter == null) {
				throw new ArgumentNullException ("[ObjectPool] setter cannot be null");
			}
			m_setter = setter;
			Initialize (settings, prefab);
		}

		public TObject Spawn (TParam1 param1) {
			var item = GetInternal ();
			m_setter.Invoke(item, param1);
			return item;
		}
	}

	public class GameObjectPool<TObject, TParam1, TParam2> : GameObjectPoolBase<TObject>, IObjectPool<TObject, TParam1, TParam2> where TObject : Component {

		Action<TObject, TParam1, TParam2> m_setter;

		public GameObjectPool (ObjectPoolSettings settings, TObject prefab, Action<TObject, TParam1, TParam2> setter) {
			if (setter == null) {
				throw new ArgumentNullException ("[ObjectPool] setter cannot be null");
			}
			m_setter = setter;
			Initialize (settings, prefab);
		}

		public TObject Spawn (TParam1 param1, TParam2 param2) {
			var item = GetInternal ();
			m_setter.Invoke(item, param1, param2);
			return item;
		}
	}

	public class GameObjectPool<TObject, TParam1, TParam2, TParam3> : GameObjectPoolBase<TObject>, IObjectPool<TObject, TParam1, TParam2, TParam3> where TObject : Component {

		Action<TObject, TParam1, TParam2, TParam3> m_setter;

		public GameObjectPool (ObjectPoolSettings settings, TObject prefab, Action<TObject, TParam1, TParam2, TParam3> setter) {
			if (setter == null) {
				throw new ArgumentNullException ("[ObjectPool] setter cannot be null");
			}
			m_setter = setter;
			Initialize (settings, prefab);
		}

		public TObject Spawn (TParam1 param1, TParam2 param2, TParam3 param3) {
			var item = GetInternal ();
			m_setter.Invoke(item, param1, param2, param3);
			return item;
		}
	}
}