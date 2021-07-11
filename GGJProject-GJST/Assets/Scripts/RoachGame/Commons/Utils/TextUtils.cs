using System.Collections.Generic;
using System.Text;

namespace RoachGame.Utils {
    /// <summary>
    /// 字符串工具
    /// </summary>
    public static class TextUtils {
        /// <summary>
        /// 封装字符串数据判定，即检查该字符串对象是否有数据
        /// </summary>
        /// <param name="str">待判定字符串</param>
        /// <returns>是否有数据</returns>
        public static bool HasData(string str) {
            return str != null && str.Length > 0;
        }

        /// <summary>
        /// 将字符串列表转化为分隔的字符串，可自定义分隔符
        /// </summary>
        /// <param name="list">待转化列表</param>
        /// <param name="seperator">分隔符，默认为逗号</param>
        /// <returns>转化后的字符串</returns>
        public static string ListToString(List<string> list, char seperator = ',') {
            if (list != null && list.Count > 0) {
                var builder = new StringBuilder(list[0]);
                for (var i = 1; i < list.Count; i++) {
                    builder.Append(seperator);
                    builder.Append(list[i]);
                }
                return builder.ToString();
            } else {
                return "";
            }
        }

        /// <summary>
        /// 将字符串集合转化为分隔的字符串，可自定义分隔符
        /// </summary>
        /// <param name="set">待转化集合</param>
        /// <param name="seperator">分隔符，默认为逗号</param>
        /// <returns>转化后的字符串</returns>
        public static string SetToString(HashSet<string> set, char seperator = ',') {
            if (set != null && set.Count > 0) {
                var builder = new StringBuilder();
                var index = 0;
                foreach (var str in set) {
                    if (index > 0) {
                        builder.Append(seperator);
                    }
                    builder.Append(str);
                    index++;
                }
                return builder.ToString();
            } else {
                return "";
            }
        }

        /// <summary>
        /// 分隔字符串转化为列表，可自定义分隔符
        /// </summary>
        /// <param name="str">待转化字符串</param>
        /// <param name="seperator">分隔符，默认为逗号</param>
        /// <returns>字符串列表</returns>
        public static List<string> StringToList(string str, char seperator = ',') {
            var list = new List<string>();
            if (HasData(str)) {
                var sep = str.Split(seperator);
                foreach (var s in sep) {
                    list.Add(s);
                }
            }
            return list;
        }

        /// <summary>
        /// 分隔字符串转化为集合，可自定义分隔符
        /// </summary>
        /// <param name="str">待转化字符串</param>
        /// <param name="seperator">分隔符，默认为逗号</param>
        /// <returns>字符串集合</returns>
        public static HashSet<string> StringToSet(string str, char seperator = ',') {
            var set = new HashSet<string>();
            if (HasData(str)) {
                var sep = str.Split(seperator);
                foreach (var s in sep) {
                    set.Add(s);
                }
            }
            return set;
        }
    }
}
