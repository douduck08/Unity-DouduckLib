using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DouduckLib.UIManagement
{
    [RequireComponent(typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler))]
    [DisallowMultipleComponent]
    public class UIManager : LocalSingletonComponent<UIManager>, ISceneSaveProcessing, IPrefabSaveProcessing
    {
        [SerializeField, ReadOnly] List<UIPage> _preloadUIPages;

        struct PageStackData
        {
            public UIPage page;
            public bool clone;
        }

        List<PageStackData> _pageStackDatas = new();

        public void OnSceneSave() => OnWillSave();
        public void OnPrefabSave() => OnWillSave();

        void OnWillSave()
        {
            _preloadUIPages = new();
            _preloadUIPages.AddRange(GetComponentsInChildren<UIPage>(true));
        }

        protected override void OnSingletonAwake()
        {
            Initialize();
        }

        void Initialize()
        {
            if (_preloadUIPages != null)
            {
                foreach (var page in _preloadUIPages)
                {
                    page.SetPageActive(false);
                    page.Initialize();
                }
            }
        }

        public void SetRenderMode(RenderMode renderMode, Camera worldCamera = null)
        {
            var canvas = GetComponent<Canvas>();
            canvas.renderMode = renderMode;
            if (renderMode == RenderMode.ScreenSpaceCamera)
            {
                canvas.worldCamera = worldCamera;
            }
        }

        public bool IsTopPage(UIPage page)
        {
            return _pageStackDatas.Count > 0 && _pageStackDatas[_pageStackDatas.Count - 1].page == page;
        }

        public T Push<T>(T page) where T : UIPage
        {
            if (page != null)
            {
                if (UnityUtil.IsPrefab(page))
                {
                    var instance = Instantiate(page, transform);
                    instance.Initialize();
                    return PushInternal(instance, true);
                }
                else
                {
                    return PushInternal(page, false);
                }
            }
            return null;
        }

        T PushInternal<T>(T page, bool clone) where T : UIPage
        {
            var pageStackData = new PageStackData() {
                page = page,
                clone = clone
            };

            var lastOrder = 0;
            if (_pageStackDatas.Count > 0)
            {
                var lastPageData = _pageStackDatas[_pageStackDatas.Count - 1];
                lastOrder = lastPageData.page.GetSortingOrder();
                lastPageData.page.PostCover(page); // Pass the page causing the cover
            }

            _pageStackDatas.Add(pageStackData);
            pageStackData.page.SetPageActive(true);
            pageStackData.page.OverrideSortingOrder(lastOrder + 1);
            pageStackData.page.PostPush();
            return page;
        }

        public void Pop()
        {
            if (_pageStackDatas.Count > 0)
            {
                var index = _pageStackDatas.Count - 1;
                var pageStackData = _pageStackDatas[index];
                _pageStackDatas.RemoveAt(index);

                pageStackData.page.SetPageActive(false);
                pageStackData.page.PostPop();
                if (pageStackData.clone)
                {
                    Destroy(pageStackData.page.gameObject);
                }

                if (_pageStackDatas.Count > 0)
                {
                    _pageStackDatas[_pageStackDatas.Count - 1].page.PostUncover(pageStackData.page); // Pass the popped page causing the uncover
                }
            }
        }

        public void Remove(UIPage page)
        {
            if (page == null)
            {
                return;
            }

            var index = -1;
            for (int i = 0; i < _pageStackDatas.Count; i++)
            {
                if (_pageStackDatas[i].page == page)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                return;
            }

            var pageStackData = _pageStackDatas[index];
            var isTop = (index == _pageStackDatas.Count - 1);

            _pageStackDatas.RemoveAt(index);

            // Clean up the page if this is an explicit call (not from its OnDestroy)
            if (pageStackData.page.IsPushed)
            {
                pageStackData.page.SetPageActive(false);
                pageStackData.page.PostPop();
                if (pageStackData.clone)
                {
                    Destroy(pageStackData.page.gameObject);
                }
            }

            if (isTop && _pageStackDatas.Count > 0)
            {
                _pageStackDatas[_pageStackDatas.Count - 1].page.PostUncover(page); // Pass the removed page causing the uncover
            }
        }
    }
}
