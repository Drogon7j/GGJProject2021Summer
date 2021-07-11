using System;
using System.Collections.Generic;

namespace RoachGame.Services.Events {
    /// <summary>
    /// 通用事件类，可继承拓展，受保护的构造函数，必须使用静态方法ObtainEvent获取新事件
    /// </summary>
    public class EventMessage {

        public static Queue<EventMessage> recycleEventQueue = new Queue<EventMessage>();
        /// <summary>
        /// 事件ID
        /// </summary>
        protected long eventID;
        /// <summary>
        /// 事件类型
        /// </summary>
        public int Type { get; protected set; }
        /// <summary>
        /// 事件携带的内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 事件可携带的回调方法，仅在被消费掉时触发，其它回调要通过额外回调列表进行
        /// </summary>
        public Action<EventMessage> callback;
        /// <summary>
        /// 事件优先级，最高优先会直接唤醒目标且率插队，最低优先会排到队列最后
        /// </summary>
        public EventPriority priority;
        /// <summary>
        /// 事件源（来自对象TagName）
        /// </summary>
        public string eventSource;
        /// <summary>
        /// 事件目标（来自对象TagName）
        /// </summary>
        public string dispatchTarget;
        /// <summary>
        /// 事件携带的额外信息
        /// </summary>
        protected Dictionary<string, object> extra;
        /// <summary>
        /// 额外信息的关键字枚举
        /// </summary>
        public IEnumerable<string> ExtraKeys {
            get {
                return extra == null ? null : extra.Keys;
            }
        }
        /// <summary>
        /// 事件携带的额外回调
        /// </summary>
        protected Dictionary<string, Action> extraCallbacks;
        /// <summary>
        /// 额外回调的关键字枚举
        /// </summary>
        public IEnumerable<string> CallbackKeys {
            get {
                return extraCallbacks == null ? null : extraCallbacks.Keys;
            }
        }
        /// <summary>
        /// 消费状态说明
        /// </summary>
        public ConsumeStatus consumeStatus = ConsumeStatus.Consumable;
        /// <summary>
        /// 消费计数器，用于判定是否已经被完全消费
        /// </summary>
        public int consumerCounter = 0;

        public enum EventPriority {
            Significant,
            Standard,
            Unimportant
        }

        public enum ConsumeStatus {
            NotConsumable,
            Consumable,
            Consumed
        }

        /// <summary>
        /// 静态方法获取一个空事件，如果回收队列中有可用事件则重用，否则新建一个
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <returns>事件对象</returns>
        public static EventMessage ObtainEvent(int type) {
            EventMessage ev;
            if (recycleEventQueue.Count > 0) {
                ev = recycleEventQueue.Dequeue();
                ev.eventID = DateTime.UtcNow.Ticks;
                ev.Type = type;
                ev.consumeStatus = ConsumeStatus.Consumable;
                ev.consumerCounter = 0;
                ev.Content = "Empty";
                ev.priority = EventPriority.Standard;
                ev.dispatchTarget = null;
                ev.callback = null;
                ev.extra = null;
                ev.extraCallbacks = null;
            } else {
                ev = new EventMessage(type);
            }
            return ev;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="type">标明当前事件类型</param>
        protected EventMessage(int type) {
            eventID = DateTime.UtcNow.Ticks; // 打上时间戳
            Type = type;
        }
        /// <summary>
        /// 放入额外信息
        /// </summary>
        /// <param name="key">额外信息关键字</param>
        /// <param name="value">额外信息内容</param>
        public void PutExtra(string key, object value) {
            if (extra == null) {
                extra = new Dictionary<string, object>();
            }
            extra[key] = value;
        }
        /// <summary>
        /// 获取额外信息
        /// </summary>
        /// <param name="key">额外信息关键字</param>
        /// <returns>额外信息内容</returns>
        public T GetExtra<T>(string key) {
            T result = default;
            if (extra != null && key != null) {
                result = (T)extra.TryGetElement(key);
            }
            return result;
        }
        /// <summary>
        /// 放入额外回调
        /// </summary>
        /// <param name="key">额外回调关键字</param>
        /// <param name="action">额外回调方法</param>
        public void PutCallback(string key, Action action) {
            if (extraCallbacks == null) {
                extraCallbacks = new Dictionary<string, Action>();
            }
            extraCallbacks[key] = action;
        }
        /// <summary>
        /// 获取额外回调
        /// </summary>
        /// <param name="key">额外回调关键字</param>
        /// <returns>额外回调方法</returns>
        public Action GetCallback(string key) {
            Action action = null;
            if (extraCallbacks != null) {
                action = extraCallbacks.TryGetElement(key);
            }
            return action;
        }
        /// <summary>
        /// 设置事件的来源和目的地
        /// </summary>
        /// <param name="src">来源</param>
        /// <param name="target">目的</param>
        public void SetupTarget(string src, string target) {
            eventSource = src;
            dispatchTarget = target;
        }
        /// <summary>
        /// 消费当前事件，调用一次回调并将该事件放入回收队列
        /// </summary>
        /// <param name="intercept">是否拦截当前事件，如果拦截事件会导致后续处理被截断</param>
        public void ConsumeEvent(bool intercept = false) {
            callback?.Invoke(this);
            if (intercept) {
                consumerCounter = 0;
            } else {
                consumerCounter--;
            }
            if (consumeStatus == ConsumeStatus.Consumable && consumerCounter <= 0) {
                consumeStatus = ConsumeStatus.Consumed;
                recycleEventQueue.Enqueue(this);
            }
        }

        /// <summary>
        /// 创建一个事件构建工具
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <returns>工具对象</returns>
        public static EventBuilder GetBuilder(int type) {
            return new EventBuilder(type);
        }
    }

    /// <summary>
    /// 事件对象创建工具
    /// </summary>
    public class EventBuilder {

        private EventMessage baseEvent;

        public EventBuilder(int type) {
            baseEvent = EventMessage.ObtainEvent(type);
        }

        /// <summary>
        /// 设置事件目标
        /// </summary>
        /// <param name="target">目标标识</param>
        /// <param name="src">来源标识</param>
        /// <returns></returns>
        public EventBuilder SetTarget(string target, string src = CommonConfigs.TAG_SYSTEM_OBJ) {
            baseEvent.SetupTarget(src, target);
            return this;
        }

        /// <summary>
        /// 设置事件目标，根据ID和Domain自行编译
        /// </summary>
        /// <param name="targetID">目标ID</param>
        /// <param name="targetDomain">目标Domain</param>
        /// <param name="src">来源标识</param>
        /// <returns></returns>
        public EventBuilder SetTarget(string targetID, string targetDomain, string src = CommonConfigs.TAG_SYSTEM_OBJ) {
            baseEvent.SetupTarget(src, ObjectManagement.CompileTagName(targetID, targetDomain));
            return this;
        }

        /// <summary>
        /// 设置事件是否可被消费
        /// </summary>
        /// <param name="isConsumable">是否可以消费</param>
        /// <returns></returns>
        public EventBuilder SetConsumable(bool isConsumable) {
            baseEvent.consumeStatus = isConsumable ? EventMessage.ConsumeStatus.Consumable : EventMessage.ConsumeStatus.NotConsumable;
            return this;
        }

        /// <summary>
        /// 设置事件优先度
        /// </summary>
        /// <param name="priority">优先度枚举</param>
        /// <returns></returns>
        public EventBuilder SetPriority(EventMessage.EventPriority priority) {
            baseEvent.priority = priority;
            return this;
        }

        /// <summary>
        /// 设置事件内容
        /// </summary>
        /// <param name="content">内容字符串</param>
        /// <returns></returns>
        public EventBuilder SetContent(string content) {
            baseEvent.Content = content;
            return this;
        }

        /// <summary>
        /// 设置事件结束回调
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public EventBuilder SetCallback(Action<EventMessage> callback) {
            baseEvent.callback = callback;
            return this;
        }

        /// <summary>
        /// 储存一个事件额外数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public EventBuilder PutExtra(string key, object value) {
            baseEvent.PutExtra(key, value);
            return this;
        }

        /// <summary>
        /// 保存一个事件额外回调
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public EventBuilder PutCallback(string key, Action action) {
            baseEvent.PutCallback(key, action);
            return this;
        }

        /// <summary>
        /// 装配一个事件实例
        /// </summary>
        /// <returns>事件对象</returns>
        public EventMessage BuildEvent() {
            return baseEvent;
        }
    }
}
