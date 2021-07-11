// File create date:1/27/2019
using Newtonsoft.Json;
using System.Collections.Generic;
// Created By Yu.Liu
namespace RoachGame.Services.Broadcast {
    /// <summary>
    /// 广播消息数据类
    /// </summary>
    public class BroadcastInformation {

        /// <summary>
        /// 广播行为标签
        /// </summary>
        public string BroadcastAction { get; private set; }

        /// <summary>
        /// 广播内容
        /// </summary>
        public string BroadcastContent { get; private set; }

        /// <summary>
        /// 广播额外数据，纯字符串表示
        /// </summary>
        private Dictionary<string, string> broadcastExtras;

        public BroadcastInformation(string action, string content = "Default Message") {
            BroadcastAction = action;
            BroadcastContent = content;
            broadcastExtras = new Dictionary<string, string>();
        }

        /// <summary>
        /// 放入一个布尔型数据
        /// </summary>
        /// <param name="key">数据标识</param>
        /// <param name="flag">布尔</param>
        public void PutBooleanExtra(string key, bool flag) {
            broadcastExtras[key] = flag.ToString();
        }

        /// <summary>
        /// 放入一个整型数据
        /// </summary>
        /// <param name="key">数据标识</param>
        /// <param name="num">数字</param>
        public void PutIntegerExtra(string key, int num) {
            broadcastExtras[key] = num.ToString();
        }

        /// <summary>
        /// 放入一个字符串数据
        /// </summary>
        /// <param name="key">数据标识</param>
        /// <param name="str">字符串</param>
        public void PutStringExtra(string key, string str) {
            broadcastExtras[key] = str;
        }

        /// <summary>
        /// 放入一个浮点型数据
        /// </summary>
        /// <param name="key">数据标识</param>
        /// <param name="flt">浮点数字</param>
        public void PutFloatExtra(string key, float flt) {
            broadcastExtras[key] = flt.ToString("F4");
        }

        /// <summary>
        /// 放入一个复杂对象数据，Json序列化
        /// </summary>
        /// <param name="key">数据标识</param>
        /// <param name="obj">对象</param>
        public void PutComplexExtra(string key, object obj) {
            broadcastExtras[key] = JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 获取一个储存的布尔数据
        /// </summary>
        /// <param name="key">标识</param>
        /// <returns>布尔</returns>
        public bool GetBooleanExtra(string key) {
            if (key != null && broadcastExtras.ContainsKey(key)) {
                return bool.Parse(broadcastExtras[key]);
            }
            return false;
        }

        /// <summary>
        /// 获取一个储存的数字
        /// </summary>
        /// <param name="key">标识</param>
        /// <returns>数字</returns>
        public int GetIntegerExtra(string key) {
            if (key != null && broadcastExtras.ContainsKey(key)) {
                return int.Parse(broadcastExtras[key]);
            }
            return int.MinValue;
        }

        /// <summary>
        /// 获取一个储存的字符串
        /// </summary>
        /// <param name="key">标识</param>
        /// <returns>字符串</returns>
        public string GetStringExtra(string key) {
            if (key != null && broadcastExtras.ContainsKey(key)) {
                return broadcastExtras[key];
            }
            return null;
        }

        /// <summary>
        /// 获取一个储存的浮点数字
        /// </summary>
        /// <param name="key">标识</param>
        /// <returns>浮点数</returns>
        public float GetFloatExtra(string key) {
            if (key != null && broadcastExtras.ContainsKey(key)) {
                return float.Parse(broadcastExtras[key]);
            }
            return float.NaN;
        }

        /// <summary>
        /// 获取一个储存的复杂类对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="key">标识</param>
        /// <returns>对象</returns>
        public T GetComplexExtra<T>(string key) {
            if (key != null && broadcastExtras.ContainsKey(key)) {
                return JsonConvert.DeserializeObject<T>(broadcastExtras[key]);
            }
            return default;
        }

        public static BroadcastInformation CreateInformation(string action, string content = "Default Message") {
            return new BroadcastInformation(action, content);
        }
    }
}
