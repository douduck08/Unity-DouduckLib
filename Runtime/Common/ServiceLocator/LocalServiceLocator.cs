using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public sealed class LocalServiceLocator : LocalSingletonComponent<LocalServiceLocator>, IServiceLocator
    {
        [SerializeField] List<Component> preloadServices = new();

        private readonly Dictionary<Type, object> _serviceDictionary = new();
        private readonly List<IUpdatable> _updatableList = new();
        private readonly List<IFixedUpdatable> _fixedUpdatableList = new();
        private readonly List<ILateUpdatable> _lateUpdatableList = new();

        GameObject IServiceLocator.Owner => gameObject;
        Dictionary<Type, object> IServiceLocator.ServiceDictionary => _serviceDictionary;
        List<IUpdatable> IServiceLocator.UpdatableList => _updatableList;
        List<IFixedUpdatable> IServiceLocator.FixedUpdatableList => _fixedUpdatableList;
        List<ILateUpdatable> IServiceLocator.LateUpdatableList => _lateUpdatableList;

        protected override void OnSingletonAwake()
        {
            foreach (var preloadService in preloadServices)
            {
                if (!this.HasService(preloadService.GetType()))
                {
                    this.AddService(preloadService);
                }
            }
        }

        void Update()
        {
            for (int i = _updatableList.Count - 1; i >= 0; i--)
            {
                _updatableList[i].OnUpdate();
            }
        }

        void FixedUpdate()
        {
            for (int i = _fixedUpdatableList.Count - 1; i >= 0; i--)
            {
                _fixedUpdatableList[i].OnFixedUpdate();
            }
        }

        void LateUpdate()
        {
            for (int i = _lateUpdatableList.Count - 1; i >= 0; i--)
            {
                _lateUpdatableList[i].OnLateUpdate();
            }
        }
    }
}
