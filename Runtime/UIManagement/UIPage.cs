using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DouduckLib.UIManagement
{
    [RequireComponent(typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster))]
    [DisallowMultipleComponent]
    public abstract class UIPage : MonoBehaviour, ISceneSaveProcessing, IPrefabSaveProcessing
    {
        [SerializeField, ReadOnly] Canvas _canvas;
        [SerializeField, ReadOnly] GraphicRaycaster _raycaster;

        bool _inputEnabled = true;
        bool _isPushed;
        bool _isCovered;

        internal bool IsPushed => _isPushed;

        public void OnSceneSave() => OnWillSave();
        public void OnPrefabSave() => OnWillSave();

        void OnWillSave()
        {
            _canvas = GetComponent<Canvas>();
            _raycaster = GetComponent<GraphicRaycaster>();
        }

        public bool GetPageActive() => _canvas != null && _canvas.enabled;
        
        public void SetPageActive(bool pageActive)
        {
            this.enabled = pageActive;
            if (_canvas != null)
            {
                _canvas.enabled = pageActive;
            }
            if (_raycaster != null)
            {
                _raycaster.enabled = pageActive && _inputEnabled;
            }
        }

        public bool GetInputEnabled() => _inputEnabled;
        
        public void SetInputEnabled(bool inputEnabled)
        {
            _inputEnabled = inputEnabled;
            if (_raycaster != null)
            {
                _raycaster.enabled = inputEnabled;
            }
        }

        public bool IsTop()
        {
            var manager = UIManager.Get(this);
            return manager != null && manager.IsTopPage(this);
        }

        public int GetSortingOrder()
        {
            if (_canvas != null && _canvas.overrideSorting)
            {
                return _canvas.sortingOrder;
            }
            return 0;
        }

        internal void OverrideSortingOrderInternal(int sortingOrder)
        {
            if (_canvas != null)
            {
                _canvas.overrideSorting = true;
                _canvas.sortingOrder = sortingOrder;
            }
        }

        internal void Initialize()
        {
            OnInitialized();
        }

        internal void PostPush()
        {
            _isPushed = true;
            OnPushed();
        }

        internal void PostPop()
        {
            if (_isPushed)
            {
                _isPushed = false;
                _isCovered = false;
                OnPopped();
            }
        }

        protected void OnDestroy()
        {
            if (_isPushed)
            {
                _isPushed = false;
                _isCovered = false;
                OnPopped();
                var manager = UIManager.Get(this);
                if (manager != null)
                {
                    manager.Remove(this);
                }
            }
        }

        internal void PostCover(UIPage coveringPage)
        {
            if (!_isCovered)
            {
                _isCovered = true;
                OnCovered(coveringPage);
            }
        }

        internal void PostUncover(UIPage uncoveringPage)
        {
            if (_isCovered)
            {
                _isCovered = false;
                OnUncovered(uncoveringPage);
            }
        }

        protected virtual void OnInitialized() { }
        protected virtual void OnPushed() { }
        protected virtual void OnPopped() { }
        protected virtual void OnCovered(UIPage coveringPage) { }
        protected virtual void OnUncovered(UIPage uncoveringPage) { }
    }

    public static class UIPageExtension
    {
        public static T OverrideSortingOrder<T>(this T page, int sortingOrder) where T : UIPage
        {
            page.OverrideSortingOrderInternal(sortingOrder);
            return page;
        }
    }
}
