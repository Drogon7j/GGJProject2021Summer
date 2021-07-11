using RoachGame.Data;
using RoachGame.Interfaces;
using RoachGame.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoachGame.Basics {

    /// <summary>
    /// 抽象列表基类，包含各种具体列表通用的方法
    /// </summary>
    public abstract class BaseComplexListView : BaseWidget {

        public RectOffset paddingOffset = new RectOffset();

        protected BaseAdapter baseAdapter; // 列表适配器
        protected ScrollRect scrollRect; // 滑动组件
        protected RectTransform contentRoot; // 内容根对象
        protected Transform cacheRoot; // 缓存区域根对象
        protected RectTransform sliderRoot; // 滑动区根对象

        protected List<RecycleItemPlaceHolder> placeholderList; // 占位符列表
        protected Deque<RecycleItem> activeDeque; // 激活项目双端队列
        protected Dictionary<int, Queue<RecycleItem>> cacheDict; // 缓冲字典

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
            placeholderList = new List<RecycleItemPlaceHolder>();
            activeDeque = new Deque<RecycleItem>();
            cacheDict = new Dictionary<int, Queue<RecycleItem>>();
        }

        protected override void PostLoad() {
            RegisterUpdateFunction(1, UpdateRecycle);
        }

        protected abstract void UpdateSlider();

        protected abstract void UpdateRecycle();

        public sealed override void UpdateViews() {
            ExecuteNextGlobal(UpdateSlider);
        }

        /// <summary>
        /// 从缓存中取出一个列表项或者新建一个空的
        /// </summary>
        /// <returns>列表项</returns>
        protected RecycleItem TakeItemFromCache(int type) {
            RecycleItem itemData = null;
            var itemQueue = cacheDict.TryGetElement(type);
            if (itemQueue != null && itemQueue.Count > 0) {
                itemData = itemQueue.Dequeue();
            }
            if (itemData == null) {
                itemData = new RecycleItem();
                itemData.itemType = type;
            }
            itemData.itemObject?.SetActive(true);
            return itemData;
        }

        /// <summary>
        /// 将列表项放入滑动窗口内排列好
        /// </summary>
        /// <param name="itemData">列表项</param>
        protected void PutItemIntoSlider(RecycleItem itemData, bool isHead = false) {
            itemData.itemObject.transform.SetParent(sliderRoot);
            itemData.itemObject.transform.localScale = Vector3.one;
            if (itemData.itemObject.transform.localPosition.z != 0f) {
                var position = itemData.itemObject.transform.localPosition;
                position.z = 0f;
                itemData.itemObject.transform.localPosition = position;
            }
            if (isHead) {
                itemData.itemObject.transform.SetAsFirstSibling();
            }
            itemData.itemObject.GetComponent<IUpdateViews>()?.NotifyUpdate();
        }

        /// <summary>
        /// 将列表项放回缓存中以便重用
        /// </summary>
        /// <param name="itemData">列表项</param>
        protected void PutItemIntoCache(RecycleItem itemData) {
            itemData.itemObject.SetActive(false);
            itemData.itemObject.transform.SetParent(cacheRoot);
            var itemQueue = cacheDict.TryGetElement(itemData.itemType);
            if (itemQueue == null) {
                itemQueue = new Queue<RecycleItem>();
                cacheDict[itemData.itemType] = itemQueue;
            }
            itemQueue.Enqueue(itemData);
        }

        /// <summary>
        /// 通过偏移量寻找位置索引，二分法搜索
        /// </summary>
        /// <param name="position">位置偏移</param>
        /// <returns>位置索引</returns>
        protected int FindIndexByPosition(float position) {
            var l = 0;
            var r = placeholderList.Count - 1;
            var mid = (l + r) / 2;
            while (l < r - 1 && placeholderList[mid].itemOffset != position) {
                if (placeholderList[mid].itemOffset > position) {
                    r = mid;
                } else {
                    l = mid;
                }
                mid = (l + r) / 2;
            }
            if (placeholderList[mid].itemOffset == position) {
                return mid;
            } else {
                return l;
            }
        }

        /// <summary>
        /// 将滑动区移动到指定位置
        /// </summary>
        /// <param name="targetOffset">目标偏移量</param>
        protected abstract void PutSliderIntoPosition(float targetOffset);

        /// <summary>
        /// 设置适配器
        /// </summary>
        /// <param name="adapter">适配器</param>
        public virtual void SetAdapter(BaseAdapter adapter) {
            baseAdapter = adapter;
            baseAdapter.SetupUpdateReference(this); // 装载列表引用，观察者模式成立
            NotifyUpdate();
        }

        public virtual BaseAdapter GetAdapter() {
            return baseAdapter;
        }
    }

    public class RecycleItem {
        public int itemIndex; // 项目索引
        public int itemType; // 项目类型
        public GameObject itemObject; // 项目对应游戏GUI对象
        public float itemOffset; // 项目在滑动区内的坐标偏移量
    }

    public struct RecycleItemPlaceHolder {
        public int itemType; // 占位符代表的类型
        public float itemSize; // 占位符大小
        public float itemOffset; // 占位符偏移量
    }
}
