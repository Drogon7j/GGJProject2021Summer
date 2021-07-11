// File create date:2019/10/7
using System;
using System.Collections.Generic;
// Created By Yu.Liu
namespace RoachGame.Data {
    /// <summary>
    /// 优先队列，堆实现
    /// </summary>
    public class PriorityQueue<T> {

        protected IComparer<T> comparer;
        protected List<T> heap;

        public int Count { get; private set; }

        public PriorityQueue(int capacity, IComparer<T> comparer = null) {
            this.comparer = comparer ?? Comparer<T>.Default;
            heap = new List<T>(capacity);
        }

        public PriorityQueue(IComparer<T> comparer) : this(16, comparer) { }
        public PriorityQueue() : this(null) { }

        public void Enqueue(T e) {
            heap.Add(e);
            SiftUp(Count++);
        }

        public T Dequeue() {
            var e = Head();
            heap[0] = heap[--Count];
            heap.RemoveAt(Count);
            if (Count > 0) {
                SiftDown(0);
            }
            return e;
        }

        public T Head() {
            if (Count > 0) {
                return heap[0];
            }
            throw new IndexOutOfRangeException("队列为空，无法取得元素");
        }

        public bool Remove(T item) {
            if (heap.Contains(item)) {
                int i = heap.IndexOf(item);
                heap[i] = heap[--Count];
                heap.RemoveAt(Count);
                if (Count > 0 && i < Count) {
                    SiftDown(i);
                }
                
                return true;
            }

            return false;
        }

        public void Clear() {
            Count = 0;
            heap.Clear();
        }

        protected void SiftUp(int n) {
            var v = heap[n];
            for (var n2 = n / 2; n > 0 && comparer.Compare(v, heap[n2]) > 0; n = n2, n2 /= 2) {
                heap[n] = heap[n2];
            }
            heap[n] = v;
        }

        protected void SiftDown(int n) {
            var v = heap[n];
            for (var n2 = n * 2; n2 < Count; n = n2, n2 *= 2) {
                if (n2 + 1 < Count && comparer.Compare(heap[n2 + 1], heap[n2]) > 0) {
                    n2++;
                }
                if (comparer.Compare(v, heap[n2]) >= 0) {
                    break;
                }
                heap[n] = heap[n2];
            }
            heap[n] = v;
        }
    }
}
