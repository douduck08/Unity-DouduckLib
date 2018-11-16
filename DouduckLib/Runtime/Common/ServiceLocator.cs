using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DouduckLib;

namespace DouduckLib {
    public interface IUpdatable {
        void OnUpdate ();
    }

    public interface IFixedUpdatable {
        void OnFixedUpdate ();
    }

    public interface ILateUpdatable {
        void OnLateUpdate ();
    }

    public sealed class ServiceLocator : SingletonMono<ServiceLocator> {

        private Dictionary<Type, object> _serviceDictionary = new Dictionary<Type, object> ();
        private List<IUpdatable> _updatableList = new List<IUpdatable> ();
        private List<IFixedUpdatable> _fixedUpdatableList = new List<IFixedUpdatable> ();
        private List<ILateUpdatable> _lateUpdatableList = new List<ILateUpdatable> ();

        public static void AddService<T> (T service) where T : class {
            if (instance._serviceDictionary.ContainsKey (typeof (T))) {
                throw new InvalidOperationException (string.Format ("[ServiceLocator] There was a service <{0}>", typeof (T).Name));
            }
            instance._serviceDictionary.Add (typeof (T), service);

            IUpdatable updatable_ = service as IUpdatable;
            if (updatable_ != null) {
                instance._updatableList.Add (updatable_);
            }
            IFixedUpdatable fixedUpdatable_ = service as IFixedUpdatable;
            if (fixedUpdatable_ != null) {
                instance._fixedUpdatableList.Add (fixedUpdatable_);
            }
            ILateUpdatable lateUpdatable_ = service as ILateUpdatable;
            if (lateUpdatable_ != null) {
                instance._lateUpdatableList.Add (lateUpdatable_);
            }
        }

        public static void AddService<T> () where T : class,
        new () {
            if (instance._serviceDictionary.ContainsKey (typeof (T))) {
                throw new InvalidOperationException (string.Format ("[ServiceLocator] There was a service <{0}>", typeof (T).Name));
            }
            if (typeof (T).IsSubclassOf (typeof (MonoBehaviour))) {
                throw new ArgumentException (string.Format ("[ServiceLocator] Service <{0}> is a MonoBehaviour, use 'RegisterMono ()' instead", typeof (T).Name));
            }
            ServiceLocator.AddService<T> (new T ());
        }

        public static void AddMonoService<T> () where T : MonoBehaviour {
            Type serviceType_ = typeof (T);
            if (instance._serviceDictionary.ContainsKey (typeof (T))) {
                throw new InvalidOperationException (string.Format ("[ServiceLocator] There was a service <{0}>", serviceType_.Name));
            }
            T service_ = instance.gameObject.AddComponent<T> ();
            ServiceLocator.AddService<T> (service_);
        }

        public static T GetService<T> () where T : class {
            if (instance._serviceDictionary.ContainsKey (typeof (T))) {
                return (T) instance._serviceDictionary[typeof (T)];
            } else {
                return null;
            }
        }

        void Update () {
            for (int i = _updatableList.Count - 1; i >= 0; i--) {
                _updatableList[i].OnUpdate ();
            }
        }

        void FixedUpdate () {
            for (int i = _fixedUpdatableList.Count - 1; i >= 0; i--) {
                _fixedUpdatableList[i].OnFixedUpdate ();
            }
        }

        void LateUpdate () {
            for (int i = _lateUpdatableList.Count - 1; i >= 0; i--) {
                _lateUpdatableList[i].OnLateUpdate ();
            }
        }
    }
}