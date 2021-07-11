using RoachGame.Interfaces;
using RoachGame.Services;
using RoachGame.Services.Events;
using RoachGame.Utils;
using UnityEngine;

namespace RoachGame.Basics {
    public sealed class ObjectIdentifier : MonoBehaviour, IEventReceiver {

        [Tooltip("组件唯一标识，如果置空则与对象名称相同")]
        public string ObjectID = "ManageObject";
        [Tooltip("组件作用域，可区分不同域下的同一ID")]
        public string ObjectDomain = "Global";
        [Tooltip("是否将对象注册到事件系统")]
        public bool UseEventSystem = true;
        // 对象标签，区别于GameObject的tag属性
        private string _tagName = "";
        // 事件服务
        private EventService eventService;
        /// <summary>
        /// 对象标签，用于识别该对象
        /// </summary>
        public string TagName {
            get {
                if (!TextUtils.HasData(_tagName)) {
                    _tagName = string.Format("({0}@{1})&[0x{2:X8}]", ObjectID, ObjectDomain, GetInstanceID());
                }
                return _tagName;
            }
        }

        public void ReceiveEvent(EventMessage e) {
            gameObject.SetActive(gameObject.activeSelf || e.priority == EventMessage.EventPriority.Significant);
            var handlers = GetComponents<IEventObject>();
            if (handlers != null && handlers.Length > 0) {
                e.consumerCounter += handlers.Length;
                foreach (var handler in handlers) {
                    handler.EnqueueEvent(e);
                }
            }
        }

        private void Awake() {
            UniverseController.Initialize();
            InitIdentity();
        }

        private void Start() {
            eventService = ServiceProvider.Instance.ProvideService<EventService>(EventService.SERVICE_NAME);
            if (UseEventSystem) {
                eventService.RegisterEventReceiver(TagName, this);
            }
        }

        private void InitIdentity() {
            ObjectID = TextUtils.HasData(ObjectID) ? ObjectID : name;
            var ID_Domain = ObjectID + "@" + ObjectDomain;
            if (ObjectManagement.IDMap.ContainsKey(ID_Domain)) {
                LogUtils.LogWarning("Duplicated ObjectID:" + ID_Domain + " detected, please advice!");
            } else {
                ObjectManagement.IDMap[ID_Domain] = GetInstanceID();
                ObjectManagement.ObjectMap[GetInstanceID()] = gameObject;
            }
        }

        private void OnDestroy() {
            // 反注册全局标识
            if (UseEventSystem && eventService != null) {
                eventService.UnregisterEventReceiver(TagName);
            }
            var ID_Domain = ObjectID + "@" + ObjectDomain;
            var id = ObjectManagement.IDMap.TryGetElement(ID_Domain);
            if (id == GetInstanceID()) {
                ObjectManagement.IDMap.Remove(ID_Domain);
                ObjectManagement.ObjectMap.Remove(id);
            }
        }
    }
}
