// File create date:6/22/2018
using System;
using System.Collections.Generic;
using UnityEngine;
// Created By Yu.Liu
namespace RoachGame {
    /// <summary>
    /// 游戏对象配置
    /// </summary>
    public static class ObjectManagement {
        /// <summary>
        /// ObjectID和InstanceID的映射表
        /// </summary>
        public static Dictionary<string, int> IDMap = new Dictionary<string, int>();
        /// <summary>
        /// InstanceID和具体GameObject的映射表
        /// </summary>
        public static Dictionary<int, GameObject> ObjectMap = new Dictionary<int, GameObject>();
        /// <summary>
        /// 通过ObjectID寻找对应的组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="oid">ObjectID字符串</param>
        /// <param name="domain">组件所在域</param>
        /// <returns>对应组件</returns>
        public static T FindComponent<T>(string oid, string domain) {
            var result = default(T);
            var ID_Domain = oid + "@" + domain;
            if (IDMap.ContainsKey(ID_Domain)) {
                var id = IDMap[ID_Domain];
                result = ObjectMap[id].GetComponent<T>();
            }
            return result;
        }
        /// <summary>
        /// 通过ObjectID寻找对应的游戏对象
        /// </summary>
        /// <param name="oid">ObjectID字符串</param>
        /// <param name="domain">对象所在域</param>
        /// <returns>游戏对象</returns>
        public static GameObject FindGameObject(string oid, string domain) {
            GameObject result = null;
            var ID_Domain = oid + "@" + domain;
            if (IDMap.ContainsKey(ID_Domain)) {
                result = ObjectMap[IDMap[ID_Domain]];
            }
            return result;
        }
        /// <summary>
        /// 通过ID和Domain编译得到TagName，可用于发送事件消息
        /// </summary>
        /// <param name="oid">ObjectID字符串</param>
        /// <param name="domain">对象所在域</param>
        /// <returns>对象TagName</returns>
        public static string CompileTagName(string oid, string domain) {
            var result = "";
            var ID_Domain = oid + "@" + domain;
            if (IDMap.ContainsKey(ID_Domain)) {
                result = String.Format("({0}@{1})&[0x{2:X8}]", oid, domain, IDMap[ID_Domain]);
            }
            return result;
        }
    }
}
