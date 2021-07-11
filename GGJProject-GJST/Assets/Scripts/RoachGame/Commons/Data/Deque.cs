using System;
using System.Collections;
using System.Collections.Generic;

namespace RoachGame.Data {
    /// <summary>
    /// 自定义双端队列
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    public class Deque<T> : IEnumerable {

        private List<T> elementList;

        public int Count {
            get {
                return elementList.Count;
            }
        }

        public T this[int i] {
            get {
                return elementList[i];
            }

            set {
                elementList[i] = value;
            }
        }

        public Deque() {
            elementList = new List<T>();
        }
        /// <summary>
        /// 前端入队
        /// </summary>
        /// <param name="ele">前端元素</param>
        public void EnqueueHead(T ele) {
            if (elementList.Count == 0) {
                elementList.Add(ele);
            } else {
                elementList.Insert(0, ele);
            }
        }
        /// <summary>
        /// 访问前端第一个元素
        /// </summary>
        /// <returns>前端元素</returns>
        public T PeekHead() {
            if (elementList.Count > 0) {
                return elementList[0];
            }
            throw new IndexOutOfRangeException();
        }
        /// <summary>
        /// 前端出队
        /// </summary>
        /// <returns>前端元素</returns>
        public T DequeueHead() {
            if (elementList.Count > 0) {
                var ele = elementList[0];
                elementList.RemoveAt(0);
                return ele;
            }
            throw new IndexOutOfRangeException();
        }
        /// <summary>
        /// 后端入队
        /// </summary>
        /// <param name="ele">后端元素</param>
        public void EnqueueTail(T ele) {
            elementList.Add(ele);
        }
        /// <summary>
        /// 访问后端第一个元素
        /// </summary>
        /// <returns>后端元素</returns>
        public T PeekTail() {
            if (elementList.Count > 0) {
                return elementList[Count - 1];
            }
            throw new IndexOutOfRangeException();
        }
        /// <summary>
        /// 后端出队
        /// </summary>
        /// <returns>后端元素</returns>
        public T DequeueTail() {
            if (elementList.Count > 0) {
                var ele = elementList[Count - 1];
                elementList.RemoveAt(Count - 1);
                return ele;
            }
            throw new IndexOutOfRangeException();
        }

        public void Clear() {
            elementList.Clear();
        }

        public IEnumerator GetEnumerator() {
            for (var i = 0; i < elementList.Count; i++) {
                yield return elementList[i];
            }
        }
    }
}
