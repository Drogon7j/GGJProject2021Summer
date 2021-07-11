using RoachGame.Interfaces;
using RoachGame.Services.Events;
using System.Collections.Generic;
using UnityEngine;

namespace RoachGame.Basics {
    /// <summary>
    /// 事件系统中游戏对象的公共基类，提供PreInit，Init，Execute和Release等方法可供重载
    /// </summary>
    [RequireComponent(typeof(ObjectIdentifier))]
    public abstract class BaseEventObject : BaseObject, IEventObject {

        // 事件队列
        protected Queue<EventMessage> eventQueue;

        /// <summary>
        /// 事件入队操作
        /// </summary>
        /// <param name="e">事件对象</param>
        public void EnqueueEvent(EventMessage e) {
            if (eventQueue != null && e != null) {
                eventQueue.Enqueue(e);
            }
        }

        /// <summary>
        /// 本来的初始化过程，为防止继承出错将其封闭
        /// </summary>
        protected sealed override void OnStart() {
            if (identifier != null && identifier.UseEventSystem) {
                eventQueue = new Queue<EventMessage>();
            }
            RegisterUpdateFunction(0, ProcessEvent);
            OnStartExtra();
        }

        /// <summary>
        /// 更多初始化，避免因重载造成的事件系统注册失效
        /// </summary>
        protected virtual void OnStartExtra() { }

        /// <summary>
        /// 处理事件
        /// </summary>
        private void ProcessEvent() {
            var e = DequeueEvent(); // 取出一个待处理事件
            if (e != null && e.consumeStatus != EventMessage.ConsumeStatus.Consumed) {
                HandleEvent(e);
                e.ConsumeEvent();
                PostEventConsume(e);
            }
        }

        /// <summary>
        /// 事件处理虚方法，子类可以覆盖
        /// </summary>
        /// <param name="e">待处理事件</param>
        protected virtual void HandleEvent(EventMessage e) { }

        /// <summary>
        /// 事件消费后续，可用于转发事件的计数器重置等功能
        /// </summary>
        /// <param name="e">被消费事件</param>
        protected virtual void PostEventConsume(EventMessage e) { }

        /// <summary>
        /// 事件出队操作
        /// </summary>
        /// <returns>事件对象</returns>
        protected EventMessage DequeueEvent() {
            EventMessage result = null;
            if (eventQueue != null && eventQueue.Count > 0) {
                result = eventQueue.Dequeue();
            }
            return result;
        }
        /// <summary>
        /// 发送事件
        /// </summary>
        /// <param name="e">事件对象</param>
        protected void SendEvent(EventMessage e) {
            if (eventService != null) {
                eventService.SendEvent(e);
            }
        }

        /// <summary>
        /// 对象回收时释放资源
        /// </summary>
        protected override void Release() {
            // 清理事件队列
            if (eventQueue != null) {
                eventQueue.Clear();
            }
        }
    }
}
