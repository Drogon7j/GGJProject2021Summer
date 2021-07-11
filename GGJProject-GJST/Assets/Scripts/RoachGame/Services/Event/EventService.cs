/// File create date:8/3/2018
using RoachGame.Interfaces;
using RoachGame.Services.Events;
using RoachGame.Utils;
using System;
using System.Collections.Generic;
/// Created By Yu.Liu
namespace RoachGame.Services {
    /// <summary>
    /// 事件服务，部分定义，可在它处拓展定义
    /// </summary>
    public partial class EventService : EmptyService {

        public const string SERVICE_NAME = "EventService";

        private Dictionary<string, IEventReceiver> dispatchTable;

        public EventService() {
            dispatchTable = new Dictionary<string, IEventReceiver>();
        }

        public override void InitService() {
            LogUtils.LogNotice("Event Service Initiated!");
        }

        public override void KillService() {
            LogUtils.LogNotice("Event Service Destoryed!");
        }

        /// <summary>
        /// 发送事件
        /// </summary>
        /// <param name="e">事件对象</param>
        public void SendEvent(EventMessage e) {
            if (dispatchTable.ContainsKey(e.dispatchTarget)) {
                dispatchTable[e.dispatchTarget].ReceiveEvent(e);
            } else {
                LogUtils.LogWarning("Cannot Find Event Target: " + e.dispatchTarget);
            }
        }

        /// <summary>
        /// 注册分发入口
        /// </summary>
        /// <param name="tag">分发入口标签</param>
        /// <param name="obj">分发入口对象</param>
        public void RegisterEventReceiver(string tag, IEventReceiver obj) {
            dispatchTable[tag] = obj;
        }
        /// <summary>
        /// 反注册分发入口
        /// </summary>
        /// <param name="tag">分发入口标签</param>
        /// <param name="obj">分发入口对象</param>
        public void UnregisterEventReceiver(string tag) {
            dispatchTable.Remove(tag);
        }

        /// <summary>
        /// 获取一个通用事件构造器
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="what">事件标签</param>
        /// <param name="target">事件目标</param>
        /// <param name="src">事件源</param>
        /// <returns>构造器</returns>
        protected EventBuilder GenerateCommonBuilder(int type, string target, string src, string content) {
            var builder = EventMessage.GetBuilder(type);
            builder.SetContent(content);
            if (TextUtils.HasData(src)) {
                builder.SetTarget(target, src);
            } else {
                builder.SetTarget(target);
            }
            return builder;
        }
        /// <summary>
        /// 发送一个通用事件
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="what">事件标签</param>
        /// <param name="target">事件目标</param>
        /// <param name="src">事件源</param>
        /// <param name="args">事件参数</param>
        public void SubmitCommonEvent(int type, string target, string src = null, string content = "Empty", Dictionary<string, object> args = null) {
            var builder = GenerateCommonBuilder(type, target, src, content);
            if (args != null) {
                foreach (var key in args.Keys) {
                    builder.PutExtra(key, args[key]);
                }
            }
            SendEvent(builder.BuildEvent());
        }
        /// <summary>
        /// 发送一个回调事件
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="what">事件标签</param>
        /// <param name="target">事件目标</param>
        /// <param name="callback">事件回调</param>
        /// <param name="src">事件源</param>
        /// <param name="args">事件参数</param>
        public void SubmitCommonCallbackEvent(int type, string target, Action<EventMessage> callback, string content = "Empty", string src = null, Dictionary<string, object> args = null) {
            var builder = GenerateCommonBuilder(type, target, src, content);
            builder.SetCallback(callback);
            if (args != null) {
                foreach (var key in args.Keys) {
                    builder.PutExtra(key, args[key]);
                }
            }
            SendEvent(builder.BuildEvent());
        }
    }
}
