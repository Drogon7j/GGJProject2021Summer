// File create date:2021/1/24
using System;
using System.Collections;
using UnityEngine;
// Created By Yu.Liu
namespace RoachGame.Basics {
    
    /// <summary>
    /// 协程方法封装基类
    /// </summary>
    public abstract class BaseCoroutineFunction {

        protected MonoBehaviour sourceObject;
        protected Coroutine _coroutine;
        protected Action coroutineFinishAction;
        protected bool isCoroutineStart = false;

        public BaseCoroutineFunction(MonoBehaviour src) {
            sourceObject = src;
        }

        public virtual void SetupCoroutineFinishAction(Action action) {
            coroutineFinishAction = action;
        }

        public virtual void StartCoroutine() {
            if (!isCoroutineStart) {
                _coroutine = sourceObject.StartCoroutine(UpdateCoroutine());
            }
        }

        public virtual void StopCoroutine() {
            if (isCoroutineStart) {
                sourceObject.StopCoroutine(_coroutine);
                isCoroutineStart = false;
            }
        }

        protected IEnumerator UpdateCoroutine() {
            isCoroutineStart = true;
            yield return CoroutineExecute();
            coroutineFinishAction?.Invoke();
            isCoroutineStart = false;
        }

        protected abstract IEnumerator CoroutineExecute();
    }
}