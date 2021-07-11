// File create date:2020/10/26
using RoachGame.Basics;
using System;
using System.Collections.Generic;
using System.Text;
// Created By Yu.Liu
public class HierarchyStateMachine : BaseObject, IStateMachine {

    /// <summary>
    /// 当前状态
    /// </summary>
    public Enum CurrentState { get; protected set; }
    /// <summary>
    /// 前驱状态
    /// </summary>
    public Enum PreviousState { get; protected set; }

    protected Enum entryState;
    /// <summary>
    /// 状态机的所有层级
    /// </summary>
    protected Dictionary<string, MachineState> machineStates;
    /// <summary>
    /// 状态机是否激活
    /// </summary>
    protected bool isMachineActive = true;

    protected sealed override void OnAwake() {
        CurrentState = null;
        PreviousState = null;
        entryState = null;
        machineStates = new Dictionary<string, MachineState>();
        OnMachineAwake();
    }

    protected virtual void OnMachineAwake() { }

    protected sealed override void OnStart() {
        OnMachineStart();
    }

    protected virtual void OnMachineStart() { }

    protected sealed override void OnUpdate() {
        if (isMachineActive) {
            if (CurrentState != null) {
                var stateName = CurrentState.ToString();
                machineStates[stateName].UpdateState();
            }
            OnMachineUpdate();
        }
    }
    protected virtual void OnMachineUpdate() { }

    public MachineState SetupState(Enum state) {
        var stateName = state.ToString();
        if (!machineStates.ContainsKey(stateName)) {
            var subState = new MachineState(state, this);
            machineStates[stateName] = subState;
        }
        return machineStates[stateName];
    }

    public MachineState GetCurrentState() {
        var stateName = CurrentState.ToString();
        return machineStates.TryGetElement(stateName);
    }

    public MachineState GetLeafState() {
        if (CurrentState != null) {
            var stateName = CurrentState.ToString();
            var cursor = machineStates[stateName];
            var state = cursor.CurrentSubState;
            while (state != null) {
                cursor = cursor.GetSubState(state);
                state = cursor.CurrentSubState;
            }
            return cursor;
        }
        return null;
    }

    public string GetLeafStatePath() {
        var pathBuilder = new StringBuilder();
        if (CurrentState != null) {
            var stateName = CurrentState.ToString();
            pathBuilder.Append(stateName);
            var cursor = machineStates[stateName];
            var state = cursor.CurrentSubState;
            while (state != null) {
                pathBuilder.Append($".{state.ToString()}");
                cursor = cursor.GetSubState(state);
                state = cursor.CurrentSubState;
            }
        }
        return pathBuilder.ToString();
    }

    public MachineState GetState(Enum state) {
        var stateName = state.ToString();
        return machineStates.TryGetElement(stateName);
    }

    public void ChangeState(Enum state) {
        if (isMachineActive && CurrentState != null) {
            var currStateName = CurrentState.ToString();
            var nextStateName = state.ToString();
            if (machineStates.ContainsKey(nextStateName)) {
                machineStates[currStateName].ExitState();
                PreviousState = CurrentState;
                CurrentState = state;
                machineStates[nextStateName].EnterState();
            } else {
                throw new NotSupportedException("Certain state not configurated!");
            }
        }
    }

    public void FireTrigger(Enum trigger) {
        if (isMachineActive) {
            var currStateName = CurrentState.ToString();
            if (machineStates.ContainsKey(currStateName)) {
                machineStates[currStateName].FireTrigger(trigger);
            } else {
                throw new NotSupportedException("Current state not configurated!");
            }
        }
    }

    public void RevertState() {
        if (isMachineActive && PreviousState != null) {
            ChangeState(PreviousState);
            PreviousState = null;
        }
    }

    protected virtual void SetupEntryState(Enum state) {
        var stateName = state.ToString();
        if (machineStates.ContainsKey(stateName)) {
            entryState = state;
        } else {
            throw new NotSupportedException("No Entry State Set, Please Choose a configurated state!");
        }
    }

    /// <summary>
    /// 开始整个状态机的运行
    /// </summary>
    public void StartMachine() {
        isMachineActive = true;
        var stateName = entryState?.ToString();
        if (!string.IsNullOrEmpty(stateName) && machineStates.ContainsKey(stateName)) {
            CurrentState = entryState;
            PreviousState = null;
            machineStates[stateName].EnterState();
        } else {
            throw new NotSupportedException("Entry state not configurated!");
        }
    }

    /// <summary>
    /// 重置状态机
    /// </summary>
    public void ResetMachine() {
        StopMachine();
        StartMachine();
    }

    /// <summary>
    /// 停止状态机
    /// </summary>
    public void StopMachine() {
        if (isMachineActive) {
            var stateName = CurrentState.ToString();
            machineStates[stateName].ExitState();
        }
        isMachineActive = false;
        CurrentState = null;
        PreviousState = null;
    }

    /// <summary>
    /// 暂停状态机
    /// </summary>
    public void SuspendMachine() {
        isMachineActive = false;
    }

    /// <summary>
    /// 恢复状态机
    /// </summary>
    public void ResumeMachine() {
        isMachineActive = true;
    }

    /// <summary>
    /// 内部状态结构
    /// </summary>
    public class MachineState : IState, IStateMachine {

        public string stateName; // 状态名称，也就是状态枚举的字符串表达
        public Enum state; // 外部状态枚举
        public Action entryAction;
        public Action updateAction;
        public Action exitAction;
        private IStateMachine parentState;
        private Dictionary<string, StateTransition> transitionSet;
        private Dictionary<string, Func<bool>> externalConditions;
        #region 子状态相关
        private Dictionary<string, MachineState> subStates; // 包含的子状态集合
        private Enum subEntryState;
        public Enum CurrentSubState { get; private set; }
        public Enum LastSubState { get; private set; }
        #endregion

        public MachineState(Enum state, IStateMachine parent) {
            this.state = state;
            stateName = state.ToString();
            parentState = parent;
            subEntryState = null;
            CurrentSubState = null;
            LastSubState = null;
            transitionSet = new Dictionary<string, StateTransition>();
            externalConditions = new Dictionary<string, Func<bool>>();
            subStates = new Dictionary<string, MachineState>();
        }

        public MachineState OnEntry(Action action) {
            entryAction = action;
            return this;
        }

        public MachineState OnUpdate(Action action) {
            updateAction = action;
            return this;
        }

        public MachineState OnExit(Action action) {
            exitAction = action;
            return this;
        }

        /// <summary>
        /// 配置状态的转移路径
        /// </summary>
        /// <param name="dstState">目标状态枚举</param>
        /// <param name="triggers">触发器枚举</param>
        /// <returns>状态对象</returns>
        public MachineState Grant(Enum dstState, params Enum[] triggers) {
            var transitionName = string.Format("{0}-{1}", stateName, dstState.ToString());
            if (transitionSet.ContainsKey(transitionName)) {
                // 已经有了这个转移，将触发器包含进去
                var transition = transitionSet[transitionName];
                foreach (var trigger in triggers) {
                    transition.GrantTrigger(trigger);
                }
            } else {
                // 新建一个转移
                var transition = new StateTransition(state, dstState, parentState);
                foreach (var trigger in triggers) {
                    transition.GrantTrigger(trigger);
                }
                transitionSet[transitionName] = transition;
            }
            return this;
        }

        /// <summary>
        /// 配置指定的子状态
        /// </summary>
        /// <param name="state">状态枚举</param>
        /// <returns>子状态对象</returns>
        public MachineState SetupSubState(Enum state) {
            var stateName = state.ToString();
            if (!subStates.ContainsKey(stateName)) {
                var subState = new MachineState(state, this);
                subStates[stateName] = subState;
            }
            return subStates[stateName];
        }

        /// <summary>
        /// 获取指定的子状态，不会新建
        /// </summary>
        /// <param name="state">状态枚举</param>
        /// <returns>子状态对象</returns>
        public MachineState GetSubState(Enum state) {
            var stateName = state.ToString();
            return subStates.TryGetElement(stateName);
        }

        /// <summary>
        /// 配置入口子状态
        /// </summary>
        /// <param name="state">状态枚举</param>
        public void SetupSubEntryState(Enum state) {
            var stateName = state.ToString();
            if (subStates.ContainsKey(stateName)) {
                subEntryState = state;
            } else {
                subEntryState = null;
                throw new NotSupportedException("Entry state not configurated!");
            }
        }

        /// <summary>
        /// 配置状态的外部条件
        /// </summary>
        /// <param name="trigger">触发器枚举</param>
        /// <param name="func">外部条件方法</param>
        /// <returns>状态对象</returns>
        public MachineState ExternalTrigger(Enum trigger, Func<bool> func) {
            externalConditions[trigger.ToString()] = func;
            return this;
        }

        /// <summary>
        /// 配置特定转移的条件聚合类型
        /// </summary>
        /// <param name="dstState">目标状态枚举</param>
        /// <param name="isAnyTransition">聚合类型（与、或）</param>
        /// <returns>状态对象</returns>
        public MachineState TransitionMode(Enum dstState, bool isAnyTransition) {
            var transitionName = string.Format("{0}-{1}", stateName, dstState.ToString());
            if (transitionSet.ContainsKey(transitionName)) {
                transitionSet[transitionName].isAnyTransitition = isAnyTransition;
            } else {
                throw new NotSupportedException("Transition not found!");
            }
            return this;
        }

        /// <summary>
        /// 检查特定转移是否存在
        /// </summary>
        /// <param name="dstState">目标状态枚举</param>
        /// <returns>是否存在</returns>
        public bool CheckTransition(Enum dstState) {
            var transitionName = string.Format("{0}-{1}", stateName, dstState.ToString());
            return transitionSet.ContainsKey(transitionName);
        }

        /// <summary>
        /// 配置指定转移的触发器黑名单
        /// </summary>
        /// <param name="dstState">目标状态枚举</param>
        /// <param name="trigger">触发器枚举</param>
        /// <returns>状态对象</returns>
        public MachineState Ignore(Enum dstState, Enum trigger = null) {
            var transitionName = string.Format("{0}-{1}", stateName, dstState.ToString());
            if (trigger != null) {
                if (transitionSet.ContainsKey(transitionName)) {
                    // 已经有了这个转移，将触发器包含进去
                    var transition = transitionSet[transitionName];
                    transition.IgnoreTrigger(trigger);
                }
            } else {
                transitionSet.Remove(transitionName);
            }
            return this;
        }

        /// <summary>
        /// 配置指定转移的拦截回调
        /// </summary>
        /// <param name="dstState">目标状态枚举</param>
        /// <param name="interceptCall">拦截回调方法</param>
        /// <returns>状态对象</returns>
        public MachineState Intercept(Enum dstState, Action interceptCall) {
            var transitionName = string.Format("{0}-{1}", stateName, dstState.ToString());
            if (transitionSet.ContainsKey(transitionName)) {
                var transition = transitionSet[transitionName];
                transition.InterceptTransition(interceptCall);
            } else {
                throw new NotSupportedException("Transition not found!");
            }
            return this;
        }

        /// <summary>
        /// 启动指定触发器
        /// </summary>
        /// <param name="trigger">触发器枚举</param>
        public void FireTrigger(Enum trigger) {
            var triggerName = trigger.ToString();
            if (!externalConditions.ContainsKey(triggerName) || externalConditions[triggerName].Invoke()) {
                foreach (var transition in transitionSet.Values) {
                    transition.FireTrigger(trigger);
                }
                FireSubTrigger(trigger);
            }
        }
        /// <summary>
        /// 启动子状态的触发器
        /// </summary>
        /// <param name="trigger">触发器枚举</param>
        protected void FireSubTrigger(Enum trigger) {
            if (CurrentSubState != null) {
                var subStateName = CurrentSubState.ToString();
                subStates.TryGetElement(subStateName)?.FireTrigger(trigger);
            }
        }
        /// <summary>
        /// 改变状态
        /// </summary>
        /// <param name="state">状态枚举</param>
        public void ChangeState(Enum state) {
            if (CurrentSubState != null) {
                var currStateName = CurrentSubState.ToString();
                var nextStateName = state.ToString();
                if (subStates.ContainsKey(nextStateName)) {
                    if (subStates[currStateName].exitAction != null) {
                        subStates[currStateName].exitAction.Invoke();
                    }
                    LastSubState = CurrentSubState;
                    CurrentSubState = state;
                    if (subStates[nextStateName].entryAction != null) {
                        subStates[nextStateName].entryAction.Invoke();
                    }
                } else {
                    throw new NotSupportedException("Certain sub state not configurated!");
                }
            }
        }

        public void RevertState() {
            if (LastSubState != null) {
                ChangeState(LastSubState);
                LastSubState = null;
            }
        }

        public void EnterState() {
            entryAction?.Invoke();
            if (subEntryState != null) {
                CurrentSubState = subEntryState;
                var stateName = CurrentSubState.ToString();
                subStates[stateName].EnterState();
            }
        }

        public void UpdateState() {
            updateAction?.Invoke();
            if (CurrentSubState != null) {
                var stateName = CurrentSubState.ToString();
                subStates[stateName].UpdateState();
            }
        }

        public void ExitState() {
            if (CurrentSubState != null) {
                var stateName = CurrentSubState.ToString();
                subStates[stateName].ExitState();
            }
            exitAction?.Invoke();
        }
    }

    /// <summary>
    /// 状态转移结构
    /// </summary>
    public class StateTransition {

        public Enum srcState;
        public Enum dstState;
        public string transitionName;
        public List<Action> interceptCalls;
        public bool isAnyTransitition = true;

        private IStateMachine relativeMachine;
        private Dictionary<string, bool> conditions;
        private HashSet<string> conditionKeySet;
        private bool conditionAny = true;
        private bool conditionAll = true;

        public StateTransition(Enum src, Enum dst, IStateMachine relative) {
            srcState = src;
            dstState = dst;
            transitionName = string.Format("{0}-{1}", src.ToString(), dst.ToString());
            interceptCalls = new List<Action>();
            relativeMachine = relative;
            conditions = new Dictionary<string, bool>();
            conditionKeySet = new HashSet<string>();
        }

        public void GrantTrigger(Enum trigger) {
            var triggerName = trigger.ToString();
            conditionKeySet.Add(triggerName);
            conditions[triggerName] = false;
        }

        public bool CheckTrigger(Enum trigger) {
            return conditions.ContainsKey(trigger.ToString());
        }

        public void IgnoreTrigger(Enum trigger) {
            var triggerName = trigger.ToString();
            conditionKeySet.Remove(triggerName);
            conditions.Remove(triggerName);
        }

        public void InterceptTransition(Action callback) {
            if (!interceptCalls.Contains(callback)) {
                interceptCalls.Add(callback);
            }
        }

        public void FireTrigger(Enum trigger) {
            var triggerName = trigger.ToString();
            if (conditions.ContainsKey(triggerName)) {
                conditions[triggerName] = true;
                conditionAny = true;
                conditionAll = true;
                foreach (var flag in conditions.Values) {
                    conditionAll &= flag;
                }
                if (isAnyTransitition) {
                    AnyTransit();
                } else {
                    AllTransit();
                }
            }
        }

        public bool CheckAny() {
            return conditionAny;
        }

        public bool CheckAll() {
            return conditionAll;
        }

        public void AnyTransit() {
            if (conditionAny) {
                // 任何条件满足都进行转移
                for (var i = 0; i < interceptCalls.Count; i++) {
                    interceptCalls[i].Invoke();
                }
                foreach (var key in conditionKeySet) {
                    conditions[key] = false;
                }
                relativeMachine.ChangeState(dstState);
            }
        }

        public void AllTransit() {
            if (conditionAll) {
                // 所有条件都满足才转移
                for (var i = 0; i < interceptCalls.Count; i++) {
                    interceptCalls[i].Invoke();
                }
                foreach (var key in conditionKeySet) {
                    conditions[key] = false;
                }
                relativeMachine.ChangeState(dstState);
            }
        }
    }
}

public interface IStateMachine {
    void FireTrigger(Enum trigger);
    void ChangeState(Enum state);
    void RevertState();
}

public interface IState {
    void EnterState();
    void UpdateState();
    void ExitState();
}
