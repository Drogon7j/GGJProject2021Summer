using RoachGame.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoachGame.Components {
    /// <summary>
    /// 拖拽响应范围，用于标定拖拽动作起效的范围
    /// </summary>
    public class DragableArea : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        protected IDragable dragableContent;
        private void Start() {
            Init();
        }

        protected virtual void Init() { }

        public void SetupDragable(IDragable content) {
            dragableContent = content;
        }

        public void OnBeginDrag(PointerEventData eventData) {
            dragableContent?.OnDragBegin(eventData);
        }

        public void OnDrag(PointerEventData eventData) {
            dragableContent?.OnDragUpdate(eventData);
        }

        public void OnEndDrag(PointerEventData eventData) {
            dragableContent?.OnDragFinish(eventData);
        }
    }
}
