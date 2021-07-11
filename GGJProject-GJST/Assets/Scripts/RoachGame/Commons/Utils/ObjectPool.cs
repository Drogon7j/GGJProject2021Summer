// File create date:1/6/2018
using System.Collections.Generic;
using UnityEngine;

// Created By Yu.Liu
namespace RoachGame.Utils {
    /// <summary>
    /// 对象池的泛型实现
    /// </summary>
    /// <typeparam name="T">泛型类型</typeparam>
    public class ObjectPool<T> {

        private HashSet<T> activeSet;
        private HashSet<T> idleSet;

        public ObjectPool() {
            activeSet = new HashSet<T>();
            idleSet = new HashSet<T>();
        }

        public void StoreObject(T obj) {
            if (activeSet.Contains(obj)) {
                if (obj is GameObject) {
                    (obj as GameObject).SetActive(false);
                }
                activeSet.Remove(obj);
            }
            idleSet.Add(obj);
        }

        public T PickObject() {
            var result = default(T);
            if (idleSet.Count > 0) {
                result = idleSet.GetEnumerator().Current;
                idleSet.Remove(result);
                activeSet.Add(result);
            }
            return result;
        }

        public HashSet<T>.Enumerator GetActiveEnumerator() {
            return activeSet.GetEnumerator();
        }
    }
}
