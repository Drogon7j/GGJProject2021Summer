// File create date:2020/10/22
using RoachGame.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Created By Yu.Liu
namespace RoachGame.Basics {
    /// <summary>
    /// 简单列表抽象基类，不适用Adapter机制，子项自动更新数据，无重用，不适合大型列表
    /// </summary>
    public abstract class BaseSimpleListView : BaseWidget {

        public static int INNER_ITEM_UPDATE_RATE = 60;

        protected int prevIndex; // 刷新前所处位置索引
        protected int overIndex; // 全局索引
        protected int currCount; // 当前列表的项目数量
        protected int prevCount; // 缓存上一次的项目数量
        protected Coroutine itemUpdateCoroutine;

        protected ScrollRect scrollRect; // 滑动组件
        protected RectTransform contentRoot; // 内容根对象
        protected List<BaseItem> objectCache; // 列表项目缓存，改善效率

        public event Action<int> OnItemClick;

        // 子项Prefab配置
        public GameObject itemPrefab;
        // 公用配置项
        public RectOffset paddingOffset = new RectOffset();

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
            objectCache = new List<BaseItem>();
        }

        public override void UpdateViews() {
            prevIndex = GetCurrentPosition();
            overIndex = prevIndex;
            itemUpdateCoroutine = StartCoroutine(ExecuteItemUpdate());
        }

        /// <summary>
        /// 启动更新过程
        /// </summary>
        /// <param name="itemCount">列表最新的项目数</param>
        public virtual void UpdateItems(int itemCount) {
            currCount = itemCount;
            NotifyUpdate();
        }

        /// <summary>
        /// 获取当前滑动到的索引位置
        /// </summary>
        /// <returns>位置索引</returns>
        protected abstract int GetCurrentPosition();

        protected virtual IEnumerator ExecuteItemUpdate() {
            // 第一刷新循环
            while (overIndex < currCount) {
                for (var i = 0; i < INNER_ITEM_UPDATE_RATE; i++) { // 内循环
                    if (overIndex >= currCount) {
                        break;
                    }
                    FromCache(overIndex);
                    overIndex++;
                }
                yield return null;
            }
            // 第一循环结束，准备第二循环
            overIndex = currCount;
            while (overIndex < prevCount) { // 更新项目少于已有的项目
                ToCache(overIndex++);
            }
            prevCount = currCount;
            overIndex = 0;
            // 第二刷新循环
            while (overIndex < currCount && overIndex < prevIndex) {
                for (var i = 0; i < INNER_ITEM_UPDATE_RATE; i++) { // 内循环
                    if (overIndex >= currCount || overIndex >= prevIndex) {
                        break;
                    }
                    FromCache(overIndex);
                    overIndex++;
                }
                yield return null;
            }
            // 第二循环结束，收尾工作
            UpdateNotifier = false;
        }

        /// <summary>
        /// 从缓存中取出子项进行设置
        /// </summary>
        /// <param name="index">总体索引</param>
        protected void FromCache(int index) {
            if (index < objectCache.Count) {
                var item = objectCache[index];
                var e = item.GetComponent<LayoutElement>();
                e.ignoreLayout = false;
                item.gameObject.SetActive(true);
                item.NotifyUpdate();
            } else {
                var obj = Instantiate(itemPrefab, contentRoot);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.zero;
                var item = obj.GetComponent<BaseItem>();
                item.SetIndex(index);
                item.OnClick(OnItemClick);
                objectCache.Add(item);
                item.NotifyUpdate();
            }
        }
        /// <summary>
        /// 将子项放回缓存
        /// </summary>
        /// <param name="index">总体索引</param>
        protected void ToCache(int index) {
            if (index < objectCache.Count) {
                var item = objectCache[index];
                var e = item.GetComponent<LayoutElement>();
                e.ignoreLayout = true;
                item.gameObject.SetActive(false);
            }
        }
    }
}
