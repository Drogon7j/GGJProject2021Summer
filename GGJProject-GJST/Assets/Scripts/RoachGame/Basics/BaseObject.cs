using RoachGame.Services;
using RoachGame.Services.Broadcast;
using RoachGame.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace RoachGame.Basics {
    /// <summary>
    /// 基础游戏对象，包含常用工具方法合集以及更新代理等
    /// </summary>
    public class BaseObject : MonoBehaviour {
        // 对象标识组件，如果为空则表示没有标识
        protected ObjectIdentifier identifier;
        // 异常标识，用于避免异常情况下依然进行错误更新
        protected bool exceptionFlag = false;
        // 更新方法注册管理集合，将更新方法集与优先级对应，可实现有顺序的帧更新策略
        protected SortedDictionary<int, List<Action>> updateFunctionSet;
        // 对象销毁回调集合
        protected List<Action<GameObject>> destroyFunctionSet;
        // 事件服务
        protected EventService eventService;
        // 通用的广播发送接收服务
        protected BroadcastService broadcastService;
        // 当前对象内注册的广播处理方法集合，用于自动释放
        protected HashSet<Action<BroadcastInformation>> receiverSet;

        private void Awake() {
            UniverseController.Initialize();
            try {
                eventService = ServiceProvider.Instance.ProvideService<EventService>(EventService.SERVICE_NAME);
                broadcastService = ServiceProvider.Instance.ProvideService<BroadcastService>(BroadcastService.SERVICE_NAME);
                updateFunctionSet = new SortedDictionary<int, List<Action>>();
                destroyFunctionSet = new List<Action<GameObject>>();
                receiverSet = new HashSet<Action<BroadcastInformation>>();
                identifier = GetComponent<ObjectIdentifier>();
                OnAwake();
            } catch (Exception e) {
                LogUtils.LogNotice("ERROR in - " + gameObject.name + " Awake");
                LogUtils.LogError(e.Message + "\n" + e.StackTrace);
                exceptionFlag = true;
            }
        }

        private void Start() {
            try {
                ExecuteNext(OnLazyLoad);
                OnStart();
            } catch (Exception e) {
                LogUtils.LogNotice("ERROR in - " + gameObject.name + " Start");
                LogUtils.LogError(e.Message + "\n" + e.StackTrace);
                exceptionFlag = true;
            }
        }

        private void Update() {
            try {
                if (!exceptionFlag) {
                    CallFunctions();
                    OnUpdate();
                }
            } catch (Exception e) {
                LogUtils.LogNotice("ERROR in - " + gameObject.name + " Update");
                LogUtils.LogError(e.Message + "\n" + e.StackTrace);
                exceptionFlag = true;
            }
        }

        private void LateUpdate() {
            try {
                if (!exceptionFlag) {
                    OnLateUpdate();
                }
            } catch (Exception e) {
                LogUtils.LogNotice("ERROR in - " + gameObject.name + " LateUpdate");
                LogUtils.LogError(e.Message + "\n" + e.StackTrace);
                exceptionFlag = true;
            }
        }

        /// <summary>
        /// 初始化虚方法，子类可覆盖
        /// </summary>
        protected virtual void OnAwake() { }

        /// <summary>
        /// 初始化虚方法，子类可覆盖
        /// </summary>
        protected virtual void OnStart() { }

        /// <summary>
        /// 帧更新虚方法，子类可覆盖
        /// </summary>
        protected virtual void OnUpdate() { }

        /// <summary>
        /// 每帧的延迟更新方法，子类可覆盖
        /// </summary>
        protected virtual void OnLateUpdate() { }

        /// <summary>
        /// 延迟加载方法，该部分代码延迟到第一帧执行
        /// </summary>
        protected virtual void OnLazyLoad() { }

        /// <summary>
        /// 调用所有注册的更新方法
        /// </summary>
        private void CallFunctions() {
            if (updateFunctionSet.Count > 0) {
                foreach (var pair in updateFunctionSet) {
                    if (pair.Value != null && pair.Value.Count > 0) {
                        for (var i = 0; i < pair.Value.Count; i++) {
                            pair.Value[i].Invoke();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 注册帧更新方法到集合
        /// </summary>
        /// <param name="priority">方法优先级，数字越小越早运行</param>
        /// <param name="func">更新方法</param>
        protected void RegisterUpdateFunction(int priority, Action func) {
            List<Action> functions;
            if (updateFunctionSet.ContainsKey(priority)) {
                functions = updateFunctionSet[priority];
            } else {
                functions = new List<Action>();
            }
            functions.Add(func);
            updateFunctionSet[priority] = functions;
        }

        /// <summary>
        /// 反注册帧更新方法
        /// </summary>
        /// <param name="func">更新方法</param>
        protected void UnregisterUpdateFunction(Action func) {
            var priorityList = new List<int>(updateFunctionSet.Keys);
            foreach (var p in priorityList) {
                updateFunctionSet[p].Remove(func);
            }
        }

        /// <summary>
        /// 下一帧执行方法
        /// </summary>
        /// <param name="action">方法委托</param>
        protected void ExecuteNext(Action action) {
            StartCoroutine(NextFrameCoroutine(action));
        }

        /// <summary>
        /// 下一帧执行方法，UniRx全局代理，不受游戏对象激活状态影响
        /// </summary>
        /// <param name="action">方法委托</param>
        protected void ExecuteNextGlobal(Action action) {
            Observable.NextFrame()
                .Subscribe(_ => {
                    action.Invoke();
                });
        }

        /// <summary>
        /// 等待一定时间后执行方法
        /// </summary>
        /// <param name="time">等待时间，单位为秒</param>
        /// <param name="action">方法委托</param>
        protected Coroutine ExecuteDelay(float time, Action action) {
            return StartCoroutine(WaitTimeCoroutine(time, action));
        }

        /// <summary>
        /// 等待一定时间后执行方法，UniRx全局代理，不受游戏对象激活状态影响
        /// </summary>
        /// <param name="time">等待时间，单位为秒</param>
        /// <param name="action">方法委托</param>
        protected IDisposable ExecuteDelayGlobal(float time, Action action) {
            return Observable.Return(true, Scheduler.MainThread)
                .Delay(TimeSpan.FromSeconds(time))
                .Subscribe(_ => {
                    action.Invoke();
                });
        }

        /// <summary>
        /// 按照指定时间循环执行方法
        /// </summary>
        /// <param name="cycle">循环时间，单位为秒</param>
        /// <param name="action">方法委托</param>
        protected Coroutine ExecuteInterval(float cycle, Action action) {
            return StartCoroutine(IntervalCoroutine(cycle, action));
        }

        /// <summary>
        /// 按照指定时间循环执行方法，UniRx全局代理，不受游戏对象激活状态影响
        /// </summary>
        /// <param name="cycle">循环时间，单位为秒</param>
        /// <param name="action">方法委托</param>
        protected IDisposable ExecuteIntervalGlobal(float cycle, Action action) {
            return Observable.Interval(TimeSpan.FromSeconds(cycle), Scheduler.MainThread)
                .Subscribe(_ => {
                    action.Invoke();
                });
        }

        /// <summary>
        /// 下一帧执行方法的协程调用
        /// </summary>
        /// <param name="func">方法委托</param>
        /// <returns>枚举器</returns>
        private IEnumerator NextFrameCoroutine(Action func) {
            yield return null;
            func.Invoke();
        }

        /// <summary>
        /// 等待指定时间后执行方法的协程调用
        /// </summary>
        /// <param name="seconds">指定时间</param>
        /// <param name="func">方法委托</param>
        /// <returns>枚举器</returns>
        private IEnumerator WaitTimeCoroutine(float seconds, Action func) {
            yield return new WaitForSeconds(seconds);
            func.Invoke();
        }

        /// <summary>
        /// 以指定的时间间隔反复执行方法的协程调用
        /// </summary>
        /// <param name="cycle">循环时间</param>
        /// <param name="func">方法委托</param>
        /// <returns>枚举器</returns>
        private IEnumerator IntervalCoroutine(float cycle, Action func) {
            var wait = new WaitForSeconds(cycle);
            while (true) {
                yield return wait;
                func.Invoke();
            }
        }

        /// <summary>
        /// 注册一个对象销毁回调方法
        /// </summary>
        /// <param name="callback">方法委托</param>
        public void RegisterDestroyCallback(Action<GameObject> callback) {
            destroyFunctionSet.Add(callback);
        }

        /// <summary>
        /// 根据筛选器注册一个广播接收器
        /// </summary>
        /// <param name="filter">筛选器</param>
        /// <param name="receiver">接收器</param>
        protected virtual void RegisterBroadcastReceiver(BroadcastFilter filter, Action<BroadcastInformation> receiver) {
            if (broadcastService != null) {
                broadcastService.RegisterBroadcastReceiver(filter, receiver);
                receiverSet.Add(receiver);
            }
        }

        /// <summary>
        /// 通过完整路径寻找子物体组件的快捷方法
        /// </summary>
        /// <typeparam name="T">目标组件类型</typeparam>
        /// <param name="path">完整路径</param>
        /// <returns>目标组件</returns>
        protected T FindComponent<T>(string path) where T : Component {
            return gameObject.FindComponent<T>(path);
        }

        /// <summary>
        /// 通过完整路径寻找子物体的快捷方法
        /// </summary>
        /// <param name="path">完整路径</param>
        /// <returns>目标物体对象</returns>
        protected GameObject FindGameObject(string path) {
            return gameObject.FindObject(path);
        }

        /// <summary>
        /// 设置目标对象的激活状态
        /// </summary>
        /// <param name="active">是否激活</param>
        public void SetActive(bool active) {
            gameObject.SetActive(active);
        }

        /// <summary>
        /// 摧毁对象时释放资源
        /// </summary>
        protected virtual void Release() { }

        private void OnDestroy() {
            Release();
            if (broadcastService != null) {
                foreach (var receiver in receiverSet) {
                    broadcastService.UnregisterBroadcastReceiver(receiver);
                }
            }
            foreach (var act in destroyFunctionSet) {
                act.Invoke(gameObject);
            }
        }
    }
}
