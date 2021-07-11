using UnityEngine;

namespace RoachGame.Utils {
    /// <summary>
    /// 通用日志工具，可以通过三个开关来启用/禁用三种日志
    /// </summary>
    public static class LogUtils {
        public const string LOG_PREFIX_NOTICE = "NOTICE:";
        public const string LOG_PREFIX_WARNING = "WARNING:";
        public const string LOG_PREFIX_ERROR = "ERROR:";
        public static bool isNoticeLogEnable = true;
        public static bool isWarningLogEnable = true;
        public static bool isErrorLogEnable = true;
        /// <summary>
        /// 打印通知日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void LogNotice(string msg) {
            if (isNoticeLogEnable) {
                if (TextUtils.HasData(msg)) {
                    Debug.Log(LOG_PREFIX_NOTICE + msg);
                } else {
                    Debug.Log(LOG_PREFIX_NOTICE + "No Message!");
                }
            }
        }
        /// <summary>
        /// 打印警告日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void LogWarning(string msg) {
            if (isWarningLogEnable) {
                if (TextUtils.HasData(msg)) {
                    Debug.LogWarning(LOG_PREFIX_WARNING + msg);
                } else {
                    Debug.LogWarning(LOG_PREFIX_WARNING + "No Message!");
                }
            }
        }
        /// <summary>
        /// 打印错误日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void LogError(string msg) {
            if (isErrorLogEnable) {
                if (TextUtils.HasData(msg)) {
                    Debug.LogError(LOG_PREFIX_ERROR + msg);
                } else {
                    Debug.LogError(LOG_PREFIX_ERROR + "No Message!");
                }
            }
        }
    }
}
