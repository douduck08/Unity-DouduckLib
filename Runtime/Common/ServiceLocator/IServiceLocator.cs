using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public interface IUpdatable
    {
        void OnUpdate();
    }

    public interface IFixedUpdatable
    {
        void OnFixedUpdate();
    }

    public interface ILateUpdatable
    {
        void OnLateUpdate();
    }

    public interface IServiceLocator
    {
        internal GameObject Owner { get; }
        internal Dictionary<Type, object> ServiceDictionary { get; }
        internal List<IUpdatable> UpdatableList { get; }
        internal List<IFixedUpdatable> FixedUpdatableList { get; }
        internal List<ILateUpdatable> LateUpdatableList { get; }
    }

    public static class ServiceLocatorFunction
    {
        public static bool HasService<T>(this IServiceLocator serviceLocator) where T : class
        {
            return serviceLocator.ServiceDictionary.ContainsKey(typeof(T));
        }

        public static bool HasService(this IServiceLocator serviceLocator, Type type)
        {
            return serviceLocator.ServiceDictionary.ContainsKey(type);
        }

        public static T GetService<T>(this IServiceLocator serviceLocator) where T : class
        {
            return serviceLocator.ServiceDictionary.ContainsKey(typeof(T)) ? (T)serviceLocator.ServiceDictionary[typeof(T)] : null;
        }

        public static bool CreateService<T>(this IServiceLocator serviceLocator) where T : class, new()
        {
            if (serviceLocator.HasService<T>())
            {
                return false;
            }
            return serviceLocator.AddServiceInternal(new T());
        }

        public static bool AddService<T>(this IServiceLocator serviceLocator, T service) where T : class
        {
            if (serviceLocator.HasService(service.GetType()))
            {
                return false;
            }
            return serviceLocator.AddServiceInternal(service);
        }

        public static bool CreateServiceComponent<T>(this IServiceLocator serviceLocator) where T : Component
        {
            if (serviceLocator.HasService<T>())
            {
                return false;
            }
            return serviceLocator.AddServiceInternal(serviceLocator.Owner.AddComponent<T>());
        }

        public static bool AddServiceComponent<T>(this IServiceLocator serviceLocator, T component) where T : Component
        {
            if (serviceLocator.HasService(component.GetType()))
            {
                return false;
            }
            return serviceLocator.AddServiceInternal(component);
        }

        static bool AddServiceInternal<T>(this IServiceLocator serviceLocator, T service) where T : class
        {
            serviceLocator.ServiceDictionary.Add(service.GetType(), service);
            if (service is IUpdatable updatable)
            {
                serviceLocator.UpdatableList.Add(updatable);
            }
            if (service is IFixedUpdatable fixedUpdatable)
            {
                serviceLocator.FixedUpdatableList.Add(fixedUpdatable);
            }
            if (service is ILateUpdatable lateUpdatable)
            {
                serviceLocator.LateUpdatableList.Add(lateUpdatable);
            }
            return true;
        }
    }
}
