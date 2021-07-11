// File create date:1/27/2019
using RoachGame.Services.Broadcast;
using RoachGame.Utils;
using System;
using System.Collections.Generic;
using UniRx;
// Created By Yu.Liu
namespace RoachGame.Services {
    /// <summary>
    /// 广播服务，提供基础的广播注册和发送方法，部分定义，可拓展
    /// </summary>
    public partial class BroadcastService : EmptyService {

        public const string SERVICE_NAME = "BroadcastService";

        protected Dictionary<string, HashSet<Action<BroadcastInformation>>> broadcastReceivers;

        public BroadcastService() {
            broadcastReceivers = new Dictionary<string, HashSet<Action<BroadcastInformation>>>();
        }

        public override void InitService() {
            LogUtils.LogNotice("Broadcast Service Initiated!");
            MessageBroker.Default.Receive<BroadcastInformation>().Subscribe(ReceiveInformation);
        }

        public override void KillService() {
            LogUtils.LogNotice("Broadcast Service Destoryed!");
        }

        public void BroadcastInformation(BroadcastInformation msg) {
            MessageBroker.Default.Publish(msg);
        }

        protected void ReceiveInformation(BroadcastInformation msg) {
            if (broadcastReceivers.ContainsKey(msg.BroadcastAction)) {
                var actionSet = broadcastReceivers[msg.BroadcastAction];
                foreach (var action in actionSet) {
                    action(msg);
                }
            }
        }

        public void RegisterBroadcastReceiver(BroadcastFilter filter, Action<BroadcastInformation> action) {
            foreach (var filterStr in filter) {
                if (broadcastReceivers.ContainsKey(filterStr)) {
                    broadcastReceivers[filterStr].Add(action);
                } else {
                    var actionSet = new HashSet<Action<BroadcastInformation>>();
                    actionSet.Add(action);
                    broadcastReceivers[filterStr] = actionSet;
                }
            }
        }

        public void UnregisterBroadcastReceiver(Action<BroadcastInformation> action) {
            var filters = new BroadcastFilter();
            foreach (var filter in broadcastReceivers.Keys) {
                var actionSet = broadcastReceivers[filter];
                if (actionSet.Contains(action)) {
                    filters.AddFilter(filter);
                }
            }
            foreach (var filter in filters) {
                broadcastReceivers[filter].Remove(action);
            }
        }
    }
}
