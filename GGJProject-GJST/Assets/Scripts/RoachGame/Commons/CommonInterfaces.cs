// File create date:4/18/2019
using RoachGame.Services.Events;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
// Created By Yu.Liu
namespace RoachGame.Interfaces {
    /// <summary>
    /// 列表数据适配器接口，提供适配器所需的基础方法集
    /// </summary>
    public interface IAdapter {
        void SetupUpdateReference(IUpdateViews v);
        int GetCount(); // 获取列表项数目
        int GetItemId(int index); // 获取项目ID
        object GetItem(int index); // 获取项目数据
        GameObject GetObject(GameObject prev, int index); // 生成项目对象
        int GetObjectType(int index); // 获取指定项目的类型
        int GetObjectTypeCount(); // 获取当前总共涉及多少类型的项目
    }

    /// <summary>
    /// 动画组件接口，提供基础动画方法
    /// </summary>
    public interface IAnimation {
        void Play(bool isForward, Action onAfter = null, Action onBefore = null);
        void Stop(bool isComplete = false);
    }

    /// <summary>
    /// 可拖拽组件接口，提供拖拽回调方法
    /// </summary>
    public interface IDragable {
        void OnDragBegin(PointerEventData pointer);
        void OnDragUpdate(PointerEventData pointer);
        void OnDragFinish(PointerEventData pointer);
    }

    /// <summary>
    /// 事件处理接口，实现该接口即拥有处理事件的权限
    /// </summary>
    public interface IEventObject {
        void EnqueueEvent(EventMessage e);
    }

    /// <summary>
    /// 事件接收器接口，实现该接口表示可接受事件
    /// </summary>
    public interface IEventReceiver {
        void ReceiveEvent(EventMessage e);
    }

    /// <summary>
    /// 界面刷新接口，提供同步和隔帧刷新方法
    /// </summary>
    public interface IUpdateViews {
        void UpdateViews();
        void NotifyUpdate();
    }
}
