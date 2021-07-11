// File create date:2019/8/12
using System;
using System.Collections.Generic;
// Created By Yu.Liu
namespace RoachGame.Machines.Automata {
    /// <summary>
    /// 一个可外部配置的，只有简单状态维护功能的状态机
    /// </summary>
    public class SimpleStateMachine {
        public Enum CurrentState { get; protected set; }
        public Enum PreviousState { get; protected set; }
        protected Enum entryState; // 入口状态缓存，用于重置状态机
        protected Dictionary<string, MachineState> machineStates;

        public SimpleStateMachine() {
            machineStates = new Dictionary<string, MachineState>();
            SetupMachine();
        }

        protected virtual void SetupMachine() { }

        public void ChangeState(Enum state) {
            if (CurrentState != null) {
                var currStateName = CurrentState.ToString();
                var nextStateName = state.ToString();
                if (machineStates.ContainsKey(nextStateName)) {
                    PreviousState = CurrentState;
                    CurrentState = state;
                    if (machineStates[nextStateName].stateAction != null) {
                        machineStates[nextStateName].stateAction.Invoke();
                    }
                } else {
                    throw new NotSupportedException("Certain state not configurated!");
                }
            }
        }

        public void EntryState(Enum state) {
            var stateName = state.ToString();
            if (machineStates.ContainsKey(stateName)) {
                if (!state.Equals(entryState)) {
                    entryState = state;
                }
                CurrentState = state;
                PreviousState = null;
            } else {
                throw new NotSupportedException("Entry state not configurated!");
            }
        }

        public void FireTrigger(Enum trigger) {
            var currStateName = CurrentState.ToString();
            if (machineStates.ContainsKey(currStateName)) {
                machineStates[currStateName].FireTrigger(trigger);
            } else {
                throw new NotSupportedException("Current state not configurated!");
            }
        }

        public bool CheckTransition(Enum srcState, Enum dstState) {
            var srcStateName = srcState.ToString();
            if (machineStates.ContainsKey(srcStateName)) {
                return machineStates[srcStateName].CheckTransition(dstState);
            }
            return false;
        }

        public bool CheckTrigger(Enum trigger) {
            var currStateName = CurrentState.ToString();
            if (machineStates.ContainsKey(currStateName)) {
                return machineStates[currStateName].CheckTrigger(trigger);
            }
            return false;
        }

        public MachineState SetupState(Enum state) {
            var stateName = state.ToString();
            if (!machineStates.ContainsKey(stateName)) {
                var s = new MachineState(state, this);
                machineStates[stateName] = s;
            }
            return machineStates[stateName];
        }

        public void ResetMachine() {
            EntryState(entryState);
        }

        public class MachineState {
            public string stateName;
            public Enum state;
            public Action stateAction;
            private SimpleStateMachine stateMachine;
            private Dictionary<string, StateTransition> transitionSet;

            public MachineState(Enum state, SimpleStateMachine machine) {
                this.state = state;
                stateName = state.ToString();
                stateMachine = machine;
                transitionSet = new Dictionary<string, StateTransition>();
            }

            public MachineState OnStateAction(Action action) {
                stateAction = action;
                return this;
            }

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
                    var transition = new StateTransition(state, dstState, stateMachine);
                    foreach (var trigger in triggers) {
                        transition.GrantTrigger(trigger);
                    }
                    transitionSet[transitionName] = transition;
                }
                return this;
            }

            public MachineState TransitionMode(Enum dstState, bool isAnyTransition) {
                var transitionName = string.Format("{0}-{1}", stateName, dstState.ToString());
                if (transitionSet.ContainsKey(transitionName)) {
                    transitionSet[transitionName].isAnyTransitition = isAnyTransition;
                } else {
                    throw new NotSupportedException("Transition not found!");
                }
                return this;
            }

            public bool CheckTransition(Enum dstState) {
                var transitionName = string.Format("{0}-{1}", stateName, dstState.ToString());
                return transitionSet.ContainsKey(transitionName);
            }

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

            public bool CheckTrigger(Enum trigger) {
                var result = (transitionSet.Count > 0);
                foreach (var transition in transitionSet.Values) {
                    result &= transition.CheckTrigger(trigger);
                }
                return result;
            }

            public void FireTrigger(Enum trigger) {
                foreach (var transition in transitionSet.Values) {
                    transition.FireTrigger(trigger);
                }
            }
        }

        public class StateTransition {
            public Enum srcState;
            public Enum dstState;
            public string transitionName;
            public bool isAnyTransitition = true;
            private SimpleStateMachine stateMachine;
            private Dictionary<string, bool> conditions;
            private bool conditionAny = true;
            private bool conditionAll = true;

            public StateTransition(Enum src, Enum dst, SimpleStateMachine machine) {
                srcState = src;
                dstState = dst;
                transitionName = string.Format("{0}-{1}", src.ToString(), dst.ToString());
                stateMachine = machine;
                conditions = new Dictionary<string, bool>();
            }

            public void GrantTrigger(Enum trigger) {
                conditions[trigger.ToString()] = false;
            }

            public bool CheckTrigger(Enum trigger) {
                return conditions.ContainsKey(trigger.ToString());
            }

            public void IgnoreTrigger(Enum trigger) {
                conditions.Remove(trigger.ToString());
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
                    stateMachine.ChangeState(dstState);
                }
            }

            public void AllTransit() {
                if (conditionAll) {
                    // 所有条件都满足才转移
                    stateMachine.ChangeState(dstState);
                }
            }
        }
    }
}
