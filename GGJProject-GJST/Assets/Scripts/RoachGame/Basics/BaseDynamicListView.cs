// File create date:2021/4/4
using RoachGame.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Created By Yu.Liu
namespace RoachGame.Basics {
    /// <summary>
    /// 动态列表抽象基类，不支持多种子项类型，使用Adapter机制
    /// </summary>
    public abstract class BaseDynamicListView : BaseWidget {

        public const int DYNAMIC_LIST_INNER_UPDATE_RATE = 30;
        public RectOffset paddingOffset = new RectOffset();

        protected BaseAdapter baseAdapter; // 列表适配器
        protected ScrollRect scrollRect; // 滑动组件
        protected RectTransform contentRoot; // 内容根对象
        protected Transform cacheRoot; // 缓存区域根对象

        protected List<GameObject> itemList;
        protected Queue<GameObject> itemCache;

        protected bool isListUpdate = false;

        protected override void LoadViews() {
            scrollRect = GetComponent<ScrollRect>();
            if (scrollRect != null) {
                contentRoot = scrollRect.content;
                InitializeContainer();
            } else {
                exceptionFlag = true;
                LogUtils.LogError("Cannot Find Scroll Rect Component on List Object - " + gameObject.name);
            }
        }

        protected abstract void InitializeContainer();

        protected override void LoadMembers() {
            itemList = new List<GameObject>();
            itemCache = new Queue<GameObject>();
        }

        public override sealed void UpdateViews() {
            if (!isListUpdate) {
                StartCoroutine(UpdateContent());
            }
        }

        protected virtual IEnumerator UpdateContent() {
            isListUpdate = true;
            DeleteExtraContent();
            var splitPoint = GetCurrentPosition();
            var cusor = splitPoint;
            var innerCount = 0;
            while (cusor < baseAdapter.GetCount()) {
                if (cusor >= itemList.Count) {
                    itemList.Add(TakeItemFromCache());
                }
                itemList[cusor] = baseAdapter.GetObject(itemList[cusor], cusor);
                SetItemIntoContent(itemList[cusor]);
                cusor++;
                innerCount++;
                if (innerCount >= DYNAMIC_LIST_INNER_UPDATE_RATE) {
                    innerCount = 0;
                    yield return null;
                }
            }
            cusor = 0;
            while (cusor < splitPoint) {
                itemList[cusor] = baseAdapter.GetObject(itemList[cusor], cusor);
                SetItemIntoContent(itemList[cusor]);
                cusor++;
                innerCount++;
                if (innerCount >= DYNAMIC_LIST_INNER_UPDATE_RATE) {
                    innerCount = 0;
                    yield return null;
                }
            }
            isListUpdate = false;
        }

        protected void DeleteExtraContent() {
            if (itemList.Count > 0) {
                var delta = itemList.Count - baseAdapter.GetCount();
                if (delta > 0) {
                    // 表示需要回收一部分
                    while (delta > 0) {
                        PutItemIntoCache(itemList.GetLast());
                        delta--;
                    }
                }
            }
        }

        protected abstract int GetCurrentPosition();

        public virtual void SetAdapter(BaseAdapter adapter) {
            baseAdapter = adapter;
            baseAdapter.SetupUpdateReference(this); // 装载列表引用，观察者模式成立
            NotifyUpdate();
        }

        public virtual BaseAdapter GetAdapter() {
            return baseAdapter;
        }

        protected void SetItemIntoContent(GameObject item) {
            item.transform.SetParent(contentRoot);
            item.transform.localScale = Vector3.one;
            if (item.transform.localPosition.z != 0f) {
                var pos = item.transform.localPosition;
                pos.z = 0f;
                item.transform.localPosition = pos;
            }
        }

        protected GameObject TakeItemFromCache() {
            GameObject item = null;
            if (itemCache.Count > 0) {
                item = itemCache.Dequeue();
                item.SetActive(true);
            }
            return item;
        }

        protected void PutItemIntoCache(GameObject item) {
            item.transform.SetParent(cacheRoot);
            item.SetActive(false);
            itemList.Remove(item);
            itemCache.Enqueue(item);
        }
    }
}
