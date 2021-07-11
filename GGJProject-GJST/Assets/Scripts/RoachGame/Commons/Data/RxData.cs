// File create date:2019/8/2
using System;
using UniRx;
using UnityEngine;
// Created By Yu.Liu
namespace RoachGame.DataBinding {
    /// <summary>
    /// 自动绑定属性封装接口
    /// </summary>
    /// <typeparam name="T">属性类型</typeparam>
    public interface IRxData<T> {
        void BindCallback(Action<T> callback);
        void ChangeValue(T value);
    }
    /// <summary>
    /// 自动绑定属性封装基类
    /// </summary>
    /// <typeparam name="T">属性类型</typeparam>
    public class RxData<T> : IRxData<T> {

        protected ReactiveProperty<T> reactiveValue;
        protected IDisposable reactiveDisposable;

        public T Value {
            get {
                return reactiveValue.Value;
            }
        }

        public RxData() {
            reactiveValue = new ReactiveProperty<T>();
        }

        /// <summary>
        /// 绑定属性变化回调
        /// </summary>
        /// <param name="callback"></param>
        public void BindCallback(Action<T> callback) {
            reactiveDisposable = reactiveValue.Subscribe(callback);
        }

        /// <summary>
        /// 改变属性值
        /// </summary>
        /// <param name="value"></param>
        public void ChangeValue(T value) {
            reactiveValue.Value = value;
        }

        /// <summary>
        /// 取消当前的回调绑定
        /// </summary>
        public void UnbindCallback() {
            if(reactiveDisposable != null) {
                reactiveDisposable.Dispose();
            }
        }
    }
    /// <summary>
    /// 浮点型自动属性封装，提供自动转换和运算符重载
    /// </summary>
    public class RxFloat : RxData<float> {

        public RxFloat() {
            reactiveValue = new ReactiveProperty<float>();
        }

        public RxFloat(float value) {
            reactiveValue = new ReactiveProperty<float>(value);
        }

        public static implicit operator float(RxFloat rxValue) {
            return rxValue.reactiveValue.Value;
        }

        public static RxFloat operator +(RxFloat left, float right) {
            left.reactiveValue.Value += right;
            return left;
        }

        public static RxFloat operator -(RxFloat left, float right) {
            left.reactiveValue.Value -= right;
            return left;
        }

        public static RxFloat operator *(RxFloat left, float right) {
            left.reactiveValue.Value *= right;
            return left;
        }

        public static RxFloat operator /(RxFloat left, float right) {
            left.reactiveValue.Value /= right;
            return left;
        }
    }
    /// <summary>
    /// 整型自动属性封装，提供自动转换和运算符重载
    /// </summary>
    public class RxInteger : RxData<int> {

        public RxInteger() {
            reactiveValue = new ReactiveProperty<int>();
        }

        public RxInteger(int value) {
            reactiveValue = new ReactiveProperty<int>(value);
        }

        public static implicit operator int(RxInteger rxValue) {
            return rxValue.reactiveValue.Value;
        }

        public static RxInteger operator +(RxInteger left, int right) {
            left.reactiveValue.Value += right;
            return left;
        }

        public static RxInteger operator -(RxInteger left, int right) {
            left.reactiveValue.Value -= right;
            return left;
        }

        public static RxInteger operator *(RxInteger left, int right) {
            left.reactiveValue.Value *= right;
            return left;
        }

        public static RxInteger operator /(RxInteger left, int right) {
            left.reactiveValue.Value /= right;
            return left;
        }
    }
    /// <summary>
    /// 字符串自动属性封装，提供自动转换和运算符重载
    /// </summary>
    public class RxString : RxData<string> {

        public RxString() {
            reactiveValue = new ReactiveProperty<string>();
        }

        public RxString(string value) {
            reactiveValue = new ReactiveProperty<string>(value);
        }

        public static implicit operator string(RxString rxValue) {
            return rxValue.reactiveValue.Value;
        }

        public static RxString operator +(RxString left, object right) {
            left.reactiveValue.Value += right;
            return left;
        }
    }
    /// <summary>
    /// Vector2型自动属性封装，提供自动转换和运算符重载
    /// </summary>
    public class RxVector2 : RxData<Vector2> {

        public RxVector2() {
            reactiveValue = new ReactiveProperty<Vector2>();
        }

        public RxVector2(Vector2 value) {
            reactiveValue = new ReactiveProperty<Vector2>(value);
        }

        public RxVector2(float x, float y) {
            reactiveValue = new ReactiveProperty<Vector2>(new Vector2(x, y));
        }

        public static implicit operator Vector2(RxVector2 rxValue) {
            return rxValue.reactiveValue.Value;
        }

        public static RxVector2 operator +(RxVector2 left, Vector2 right) {
            left.reactiveValue.Value += right;
            return left;
        }

        public static RxVector2 operator -(RxVector2 left, Vector2 right) {
            left.reactiveValue.Value -= right;
            return left;
        }

        public static RxVector2 operator *(RxVector2 left, float right) {
            left.reactiveValue.Value *= right;
            return left;
        }

        public static RxVector2 operator /(RxVector2 left, float right) {
            left.reactiveValue.Value /= right;
            return left;
        }
    }
    /// <summary>
    /// Vector3型自动属性封装，提供自动转换和运算符重载
    /// </summary>
    public class RxVector3 : RxData<Vector3> {

        public RxVector3() {
            reactiveValue = new ReactiveProperty<Vector3>();
        }

        public RxVector3(Vector3 value) {
            reactiveValue = new ReactiveProperty<Vector3>(value);
        }

        public RxVector3(float x, float y, float z) {
            reactiveValue = new ReactiveProperty<Vector3>(new Vector3(x, y, z));
        }

        public static implicit operator Vector3(RxVector3 rxValue) {
            return rxValue.reactiveValue.Value;
        }

        public static RxVector3 operator +(RxVector3 left, Vector3 right) {
            left.reactiveValue.Value += right;
            return left;
        }

        public static RxVector3 operator -(RxVector3 left, Vector3 right) {
            left.reactiveValue.Value -= right;
            return left;
        }

        public static RxVector3 operator *(RxVector3 left, float right) {
            left.reactiveValue.Value *= right;
            return left;
        }

        public static RxVector3 operator /(RxVector3 left, float right) {
            left.reactiveValue.Value /= right;
            return left;
        }
    }
}