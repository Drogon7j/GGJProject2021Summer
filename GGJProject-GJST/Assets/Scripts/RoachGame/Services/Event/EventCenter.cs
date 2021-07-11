using RoachGame.Interfaces;
using RoachGame.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace RoachGame.Services.Events {
    /// <summary>
    /// 事件中心，负责管理注册的事件监听者以及分发事件
    /// </summary>
    public class EventCenter : MonoBehaviour {
        /// <summary>
        /// 静态参数，表示每帧分发多少事件
        /// </summary>
        public static int DISPATCH_PER_FRAME = 30;
        /// <summary>
        /// 事件分发队列
        /// </summary>
        private Queue<EventMessage> dispatchQueue = new Queue<EventMessage>();
        /// <summary>
        /// 事件分发表
        /// </summary>
        private Dictionary<string, IEventReceiver> dispatchTable = new Dictionary<string, IEventReceiver>();

        public bool IsDispatching { get; private set; } = true;

        private void Update() {
            if (IsDispatching) {
                for (var i = 0; i < DISPATCH_PER_FRAME; i++) {
                    var dispatchEvent = DispatchDequeue();
                    if (dispatchEvent != null) {
                        var receiver = dispatchTable.TryGetElement(dispatchEvent.dispatchTarget);
                        if (receiver != null) {
                            receiver.ReceiveEvent(dispatchEvent);
                        } else {
                            LogUtils.LogWarning($"Cannot find event target in dispatch table with id[{dispatchEvent.dispatchTarget}]");
                        }
                    } else {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 注册分发入口
        /// </summary>
        /// <param name="tag">分发入口标签</param>
        /// <param name="obj">分发入口对象</param>
        public void RegisterEventObject(string tag, IEventReceiver obj) {
            dispatchTable[tag] = obj;
        }
        /// <summary>
        /// 反注册分发入口
        /// </summary>
        /// <param name="tag">分发入口标签</param>
        /// <param name="obj">分发入口对象</param>
        public void UnregisterEventObject(string tag) {
            dispatchTable.Remove(tag);
        }
        /// <summary>
        /// 分发事件入队
        /// </summary>
        /// <param name="e">事件对象</param>
        public void DispatchEnqueue(EventMessage e) {
            lock (dispatchQueue) {
                dispatchQueue.Enqueue(e);
            }
        }
        /// <summary>
        /// 分发事件出队
        /// </summary>
        /// <returns>事件对象</returns>
        private EventMessage DispatchDequeue() {
            EventMessage result = null;
            lock (dispatchQueue) {
                if (dispatchQueue.Count > 0) {
                    result = dispatchQueue.Dequeue();
                }
            }
            return result;
        }
        /// <summary>
        /// 分发开关
        /// </summary>
        /// <param name="isOn">是否启用分发</param>
        public void DispatchSwitch(bool isOn) {
            IsDispatching = isOn;
        }
    }
}
