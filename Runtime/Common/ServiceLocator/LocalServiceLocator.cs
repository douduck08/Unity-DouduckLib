using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public sealed class LocalServiceLocator : LocalSingletonComponent<LocalServiceLocator>, IServiceLocator
    {
        private readonly Dictionary<Type, object> serviceDictionary_ = new();
        private readonly List<IUpdatable> updatableList_ = new();
        private readonly List<IFixedUpdatable> fixedUpdatableList_ = new();
        private readonly List<ILateUpdatable> lateUpdatableList_ = new();

        GameObject IServiceLocator.Owner => gameObject;
        Dictionary<Type, object> IServiceLocator.ServiceDictionary => serviceDictionary_;
        List<IUpdatable> IServiceLocator.UpdatableList => updatableList_;
        List<IFixedUpdatable> IServiceLocator.FixedUpdatableList => fixedUpdatableList_;
        List<ILateUpdatable> IServiceLocator.LateUpdatableList => lateUpdatableList_;

        void Update()
        {
            for (int i = updatableList_.Count - 1; i >= 0; i--)
            {
                updatableList_[i].OnUpdate();
            }
        }

        void FixedUpdate()
        {
            for (int i = fixedUpdatableList_.Count - 1; i >= 0; i--)
            {
                fixedUpdatableList_[i].OnFixedUpdate();
            }
        }

        void LateUpdate()
        {
            for (int i = lateUpdatableList_.Count - 1; i >= 0; i--)
            {
                lateUpdatableList_[i].OnLateUpdate();
            }
        }
    }
}
