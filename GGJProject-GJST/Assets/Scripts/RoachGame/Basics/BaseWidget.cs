using RoachGame.Interfaces;
using RoachGame.Utils;
using System;
using UnityEngine;

namespace RoachGame.Basics {
    /// <summary>
    /// UI控件的抽象基类，所有的UI控件均由此派生
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public abstract class BaseWidget : BaseObject, IUpdateViews {

        public RectTransform viewTransform { get; protected set; }

        protected bool UpdateNotifier = false; // 数据刷新标志位

        /// <summary>
        /// 实现预初始化
        /// </summary>
        protected sealed override void OnAwake() {
            try {
                PreLoad();
                viewTransform = GetComponent<RectTransform>();
                LoadMembers();
                LoadViews();
            } catch (Exception e) {
                LogUtils.LogNotice("ERROR in - " + gameObject.name);
                LogUtils.LogError(e.Message + "\n" + e.StackTrace);
            }
        }
        /// <summary>
        /// 实现初始化
        /// </summary>
        protected sealed override void OnStart() {
            PostLoad();
        }
        /// <summary>
        /// 预加载
        /// </summary>
        protected virtual void PreLoad() { }
        /// <summary>
        /// 加载成员数据
        /// </summary>
        protected virtual void LoadMembers() { }
        /// <summary>
        /// 加载视图组件
        /// </summary>
        protected virtual void LoadViews() { }
        /// <summary>
        /// 后处理
        /// </summary>
        protected virtual void PostLoad() { }
        /// <summary>
        /// 手动刷新，无延时
        /// </summary>
        public virtual void UpdateViews() { }

        /// <summary>
        /// 通知界面进行刷新，延时一帧
        /// </summary>
        public void NotifyUpdate() {
            if (gameObject.activeInHierarchy && !UpdateNotifier) {
                UpdateNotifier = true;
                BeforeUpdate();
                ExecuteNextGlobal(ViewUpdate);
            }
        }

        protected virtual void BeforeUpdate() { }

        /// <summary>
        /// 更新方法
        /// </summary>
        private void ViewUpdate() {
            try {
                UpdateViews();
            } catch (Exception e) {
                LogUtils.LogNotice("ERROR in - " + gameObject.name);
                LogUtils.LogError(e.Message + "\n" + e.StackTrace);
            }
            UpdateNotifier = false;
        }
    }
}
