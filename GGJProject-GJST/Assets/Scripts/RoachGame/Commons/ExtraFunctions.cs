using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 拓展方法类
/// </summary>
public static class ExtraFunctions {

    public const float COLOR_NORMALIZER = 255f; // 颜色通道归一化常量
    /// <summary>
    /// 安全获取Dictionary元素的拓展方法，在不存在对应Key的时候会返回默认值
    /// </summary>
    /// <typeparam name="K">Key的类型</typeparam>
    /// <typeparam name="V">Value的类型</typeparam>
    /// <param name="dict">拓展方法对应的Dictionary对象</param>
    /// <param name="key">键</param>
    /// <returns>值</returns>
    public static V TryGetElement<K, V>(this Dictionary<K, V> dict, K key) {
        V result = default;
        if (key != null) {
            dict.TryGetValue(key, out result);
        }
        return result;
    }

    /// <summary>
    /// 获取并删除Dictionary内指定元素的拓展方法
    /// </summary>
    /// <typeparam name="K">Key的类型</typeparam>
    /// <typeparam name="V">Value的类型</typeparam>
    /// <param name="dict">拓展方法对应的Dictionary对象</param>
    /// <param name="key">键</param>
    /// <returns>值</returns>
    public static V TakeAndRemove<K, V>(this Dictionary<K, V> dict, K key) {
        V result = default;
        if (key != null && dict.ContainsKey(key)) {
            result = dict.TryGetElement(key);
            dict.Remove(key);
        }
        return result;
    }

    /// <summary>
    /// 直接在GameObject之下按路径寻找子物体组件的拓展方法
    /// </summary>
    /// <typeparam name="T">所寻找组件类型</typeparam>
    /// <param name="obj">拓展方法对应的GameObject</param>
    /// <param name="path">路径</param>
    /// <returns>组件</returns>
    public static T FindComponent<T>(this GameObject obj, string path) where T : Component {
        T result = null;
        if (obj != null) {
            path = path.Replace('.', '/');
            var target = obj.transform.Find(path);
            if (target != null) {
                result = target.gameObject.GetComponent<T>();
                if (result == null) {
                    Debug.LogWarningFormat("Cannot Find Component[{0}] At: {1}", typeof(T).Name, path);
                }
            } else {
                Debug.LogWarningFormat("Cannot Find GameObject At: {0}", path);
            }
        }
        return result;
    }

    /// <summary>
    /// 在某个组件所绑定的GameObject之下按路径寻找子物体组件的拓展方法
    /// </summary>
    /// <typeparam name="T">所寻找组件类型</typeparam>
    /// <param name="obj">拓展方法对应的Component</param>
    /// <param name="path">路径</param>
    /// <returns>组件</returns>
    public static T FindComponent<T>(this Component com, string path) where T : Component {
        T result = null;
        if (com != null) {
            path = path.Replace('.', '/');
            var target = com.transform.Find(path);
            if (target != null) {
                result = target.gameObject.GetComponent<T>();
                if (result == null) {
                    Debug.LogWarningFormat("Cannot Find Component[{0}] At: {1}", typeof(T).Name, path);
                }
            } else {
                Debug.LogWarningFormat("Cannot Find GameObject At: {0}", path);
            }
        }
        return result;
    }

    /// <summary>
    /// 直接在GameObject之下按路径寻找子物体的拓展方法
    /// </summary>
    /// <param name="obj">拓展方法对应的GameObject</param>
    /// <param name="path">路径</param>
    /// <returns>子物体</returns>
    public static GameObject FindObject(this GameObject obj, string path) {
        GameObject result = null;
        if (obj != null) {
            path = path.Replace('.', '/');
            var target = obj.transform.Find(path);
            if (target != null) {
                result = target.gameObject;
            } else {
                Debug.LogWarningFormat("Cannot Find GameObject At: {0}", path);
            }
        }
        return result;
    }

    /// <summary>
    /// 在某个组件所绑定的GameObject之下按路径寻找子物体的拓展方法
    /// </summary>
    /// <param name="obj">拓展方法对应的Component</param>
    /// <param name="path">路径</param>
    /// <returns>子物体</returns>
    public static GameObject FindObject(this Component com, string path) {
        GameObject result = null;
        if (com != null) {
            path = path.Replace('.', '/');
            var target = com.transform.Find(path);
            if (target != null) {
                result = target.gameObject;
            } else {
                Debug.LogWarningFormat("Cannot Find GameObject At: {0}", path);
            }
        }
        return result;
    }

    /// <summary>
    /// 组件获取的合并方法，第一步检查传入组件是否为空（默认为空）
    /// 如果第一步得到为空则从GameObject获取组件，否则返回传入组件
    /// 如果获取结果依然为空则添加组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="obj">拓展方法对应的GameObject</param>
    /// <param name="origin">传入组件</param>
    /// <returns>组件</returns>
    public static T CheckAndGetComponent<T>(this GameObject obj, T origin = null) where T : Component {
        T component;
        if (origin == null) {
            component = obj.GetComponent<T>();
            if (component == null) {
                component = obj.AddComponent<T>();
            }
        } else {
            component = origin;
        }
        return component;
    }

    /// <summary>
    /// 为GameObject设置触发事件回调
    /// </summary>
    /// <param name="target">触发器所在对象</param>
    /// <param name="listener">回调方法</param>
    /// <param name="type">事件类型</param>
    public static void SetupEventTrigger(this GameObject target, UnityAction<BaseEventData> listener, EventTriggerType type) {
        var trigger = target.GetComponent<EventTrigger>() as EventTrigger;
        if (trigger == null) {
            trigger = target.AddComponent<EventTrigger>();
        }
        if (trigger.triggers == null) {
            trigger.triggers = new List<EventTrigger.Entry>();
        }
        var entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener(listener);
        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// 为UGUI的Button组件设置点击事件回调
    /// </summary>
    /// <param name="button">Button组件</param>
    /// <param name="listener">回调方法</param>
    public static void SetupButtonListener(this Button button, UnityAction listener) {
        button.onClick.AddListener(listener);
    }

    /// <summary>
    /// 对Image的Color属性进行赋值
    /// </summary>
    /// <param name="img">拓展方法对应的Image</param>
    /// <param name="colorStr">颜色字符串，要求符合"#FFFFFF"或者"#FFFFFFFF"格式</param>
    public static void SetColor(this Image img, string colorStr) {
        img.color = CompileColor(colorStr);
    }

    /// <summary>
    /// 对SpriteRenderer的Color属性进行赋值
    /// </summary>
    /// <param name="renderer">拓展方法对应的SpriteRenderer</param>
    /// <param name="colorStr">颜色字符串，要求符合"#FFFFFF"或者"#FFFFFFFF"格式</param>
    public static void SetColor(this SpriteRenderer renderer, string colorStr) {
        renderer.color = CompileColor(colorStr);
    }

    /// <summary>
    /// 对Text的Color属性进行赋值
    /// </summary>
    /// <param name="txt">拓展方法对应的Text</param>
    /// <param name="colorStr">颜色字符串，要求符合"#FFFFFF"或者"#FFFFFFFF"格式</param>
    public static void SetColor(this Text txt, string colorStr) {
        txt.color = CompileColor(colorStr);
    }

    /// <summary>
    /// 对Image的Color属性进行赋值
    /// </summary>
    /// <param name="img">拓展方法对应的Image</param>
    /// <param name="color">颜色值，以无符号整数类型给出，建议使用0xFFFFFFFF格式</param>
    public static void SetColor(this Image img, uint color) {
        img.color = CompileColor(color);
    }

    /// <summary>
    /// 对SpriteRenderer的Color属性进行赋值
    /// </summary>
    /// <param name="renderer">拓展方法对应的SpriteRenderer</param>
    /// <param name="color">颜色值，以无符号整数类型给出，建议使用0xFFFFFFFF格式</param>
    public static void SetColor(this SpriteRenderer renderer, uint color) {
        renderer.color = CompileColor(color);
    }

    /// <summary>
    /// 对Text的Color属性进行赋值
    /// </summary>
    /// <param name="txt">拓展方法对应的Text</param>
    /// <param name="color">颜色值，以无符号整数类型给出，建议使用0xFFFFFFFF格式</param>
    public static void SetColor(this Text txt, uint color) {
        txt.color = CompileColor(color);
    }
    /// <summary>
    /// 从颜色字符串中编译颜色对象
    /// </summary>
    /// <param name="colorStr">颜色字符串</param>
    /// <returns>颜色对象</returns>
    public static Color CompileColor(string colorStr) {
        var result = Color.clear;
        if (colorStr.Length > 0 && colorStr[0] == '#') {
            if (colorStr.Length == 7) {
                // RGB
                var rStr = colorStr.Substring(1, 2);
                var gStr = colorStr.Substring(3, 2);
                var bStr = colorStr.Substring(5, 2);
                var r = int.Parse(rStr, System.Globalization.NumberStyles.HexNumber);
                var g = int.Parse(gStr, System.Globalization.NumberStyles.HexNumber);
                var b = int.Parse(bStr, System.Globalization.NumberStyles.HexNumber);
                result = new Color(r / COLOR_NORMALIZER, g / COLOR_NORMALIZER, b / COLOR_NORMALIZER);
            } else if (colorStr.Length == 9) {
                // ARGB
                var aStr = colorStr.Substring(1, 2);
                var rStr = colorStr.Substring(3, 2);
                var gStr = colorStr.Substring(5, 2);
                var bStr = colorStr.Substring(7, 2);
                var a = int.Parse(aStr, System.Globalization.NumberStyles.HexNumber);
                var r = int.Parse(rStr, System.Globalization.NumberStyles.HexNumber);
                var g = int.Parse(gStr, System.Globalization.NumberStyles.HexNumber);
                var b = int.Parse(bStr, System.Globalization.NumberStyles.HexNumber);
                result = new Color(r / COLOR_NORMALIZER, g / COLOR_NORMALIZER, b / COLOR_NORMALIZER, a / COLOR_NORMALIZER);
            }
        }
        return result;
    }
    /// <summary>
    /// 从颜色值中编译颜色对象
    /// </summary>
    /// <param name="color">颜色值，以无符号整数类型给出，建议使用0xFFFFFFFF格式</param>
    /// <returns>颜色对象</returns>
    public static Color CompileColor(uint color) {
        var tmpColor = color;
        var b = tmpColor & 0xFF;
        tmpColor = tmpColor >> 8;
        var g = tmpColor & 0xFF;
        tmpColor = tmpColor >> 8;
        var r = tmpColor & 0xFF;
        tmpColor = tmpColor >> 8;
        var a = tmpColor & 0xFF;
        return new Color(r / COLOR_NORMALIZER, g / COLOR_NORMALIZER, b / COLOR_NORMALIZER, a / COLOR_NORMALIZER);
    }
    /// <summary>
    /// 设置GameObject的layer，使用layer名称作为参数
    /// </summary>
    /// <param name="obj">拓展方法对应的GameObject</param>
    /// <param name="layer">层级名称</param>
    public static void SetLayer(this GameObject obj, string layer) {
        if (obj != null) {
            obj.layer = LayerMask.NameToLayer(layer);
        }
    }
    /// <summary>
    /// 按指定长度裁剪字符串
    /// </summary>
    /// <param name="str">拓展方法对应的字符串</param>
    /// <param name="length">限定长度</param>
    /// <param name="suffix">裁剪后附加后缀</param>
    /// <returns>裁剪后的字符串</returns>
    public static string ShrinkString(this string str, int length, string suffix) {
        var result = str;
        if (str.Length >= length) {
            result = str.Substring(0, length - 1);
            result += suffix;
        }
        return result;
    }
    /// <summary>
    /// 运行指定方法，空参数表
    /// </summary>
    /// <param name="method">对应的方法信息</param>
    /// <param name="source">方法源</param>
    public static void InvokeEmpty(this MethodInfo method, object source) {
        method.Invoke(source, new object[0]);
    }

    /// <summary>
    /// 获取列表的最后一个元素，如果列表为空则返回默认值
    /// </summary>
    /// <typeparam name="T">列表元素类型</typeparam>
    /// <param name="list">对应的列表对象</param>
    /// <returns>最后一个元素或者默认</returns>
    public static T GetLast<T>(this List<T> list) {
        if (list.Count > 0) {
            return list[list.Count - 1];
        }
        return default(T);
    }

    /// <summary>
    /// 将可枚举的集合元素全部放入当前集合
    /// </summary>
    /// <typeparam name="T">集合元素类型</typeparam>
    /// <param name="set">对应的集合对象</param>
    /// <param name="src">待放入的可枚举集合</param>
    public static void PutSet<T>(this HashSet<T> set, IEnumerable<T> src) {
        if (src != null) {
            foreach (var item in src) {
                set.Add(item);
            }
        }
    }

    /// <summary>
    /// 获取集合里第一个元素（按枚举顺序）
    /// </summary>
    /// <typeparam name="T">集合元素类型</typeparam>
    /// <param name="set">对应的集合对象</param>
    /// <returns>第一个元素</returns>
    public static T PickFirst<T>(this HashSet<T> set) {
        if (set.Count > 0) {
            foreach (var item in set) {
                return item;
            }
        }
        return default;
    }

    /// <summary>
    /// 封装数据交换操作，使用引用类型保证交换成功，泛型支持
    /// </summary>
    /// <typeparam name="T">操作类型</typeparam>
    /// <param name="left">左操作对象</param>
    /// <param name="right">右操作对象</param>
    public static void SwapData<T>(ref T left, ref T right) {
        var temp = left;
        left = right;
        right = temp;
    }
}
