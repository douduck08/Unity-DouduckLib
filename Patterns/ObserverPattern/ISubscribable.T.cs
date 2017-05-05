using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace DouduckGame {
    public interface ISubscribable<T> where T : struct {
        SubscribableObserver RegisterObserver (int iEventId, Action<int> observerCallback);
        void UnregisterObserver (int iEventId, Action<int, T> observerCallback);
    }
}
