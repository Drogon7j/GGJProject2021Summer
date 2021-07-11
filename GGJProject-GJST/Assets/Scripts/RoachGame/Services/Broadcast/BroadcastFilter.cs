// File create date:1/27/2019
using System.Collections;
using System.Collections.Generic;
// Created By Yu.Liu
namespace RoachGame.Services.Broadcast {
    /// <summary>
    /// 广播行为过滤器
    /// </summary>
    public class BroadcastFilter : IEnumerable<string> {

        private HashSet<string> filters; // 过滤器标签集合

        /// <summary>
        /// 初始化过滤器标签
        /// </summary>
        /// <param name="args">所有可用标签</param>
        public BroadcastFilter(params string[] args) {
            filters = new HashSet<string>();
            foreach (var arg in args) {
                filters.Add(arg);
            }
        }

        /// <summary>
        /// 添加一个过滤器标签
        /// </summary>
        /// <param name="filter"></param>
        public void AddFilter(string filter) {
            filters.Add(filter);
        }

        /// <summary>
        /// 添加一系列过滤器标签
        /// </summary>
        /// <param name="filters"></param>
        public void AddFilter(params string[] filters) {
            foreach (var filter in filters) {
                this.filters.Add(filter);
            }
        }

        /// <summary>
        /// 获取过滤器标签枚举器
        /// </summary>
        /// <returns>枚举器</returns>
        public IEnumerator<string> GetEnumerator() {
            return filters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return filters.GetEnumerator();
        }
    }
}
