using RoachGame.Interfaces;
using RoachGame.Services.Broadcast;
using RoachGame.Utils;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RoachGame.Basics {
    /// <summary>
    /// UI视图抽象基类，所有的视图组件均由此派生
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(RectTransform))]
    public abstract class BaseView : BaseEventObject, IUpdateViews {

        public string viewName = "Unanmed";
        public string viewCategory = "Default";

        public ViewBehavior showBehavior = new ViewBehavior();
        public ViewBehavior hideBehavior = new ViewBehavior();
        public bool independentDeltaTime = false;
        public bool deactivateRootObject = false;

        protected Vector2 showMoveFrom;
        protected Vector2 showMoveTo;
        protected Vector2 hideMoveFrom;
        protected Vector2 hideMoveTo;

        protected Vector3 showRotateFrom;
        protected Vector3 showRotateTo;
        protected Vector3 hideRotateFrom;
        protected Vector3 hideRotateTo;

        protected Vector3 showScaleFrom;
        protected Vector3 showScaleTo;
        protected Vector3 hideScaleFrom;
        protected Vector3 hideScaleTo;

        protected float showFadeFrom;
        protected float showFadeTo;
        protected float hideFadeFrom;
        protected float hideFadeTo;

        protected bool UpdateNotifier = false; // 数据刷新标志位
        protected bool isShowing = false;
        protected bool isVisible = true;
        protected bool isHiding = false;

        protected float activeDeltaTime {
            get {
                if (independentDeltaTime) {
                    return Time.unscaledDeltaTime;
                } else {
                    return Time.deltaTime;
                }
            }
        }

        public Canvas viewCanvas { get; protected set; }
        public CanvasGroup viewCanvasGroup { get; protected set; }
        public RectTransform viewTransform { get; protected set; }
        public bool IsAnimating {
            get {
                return isShowing || isHiding;
            }
        }

        /// <summary>
        /// 实现预初始化
        /// </summary>
        protected sealed override void OnAwake() {
            try {
                PreLoad();
                viewCanvas = GetComponent<Canvas>();
                viewCanvasGroup = GetComponent<CanvasGroup>();
                viewTransform = GetComponent<RectTransform>();
                viewCanvas.overrideSorting = true;
                LoadMembers();
                LoadViews();
            } catch (Exception e) {
                LogUtils.LogNotice("ERROR in - " + gameObject.name);
                LogUtils.LogError(e.Message + "\n" + e.StackTrace);
            }
        }
        /// <summary>
        /// 实现初始化
        /// </summary>
        protected sealed override void OnStartExtra() {
            InitializeBehavior();
            PostLoad();
        }

        protected void InitializeBehavior() {
            showMoveFrom = showBehavior.anim.moveAnim.moveFrom;
            showMoveTo = showBehavior.anim.moveAnim.moveTo;
            hideMoveFrom = hideBehavior.anim.moveAnim.moveFrom;
            hideMoveTo = hideBehavior.anim.moveAnim.moveTo;
            if (showBehavior.anim.moveAnim.relativeAnim) {
                showMoveFrom += viewTransform.anchoredPosition;
                showMoveTo += viewTransform.anchoredPosition;
            }
            if (hideBehavior.anim.moveAnim.relativeAnim) {
                hideMoveFrom += viewTransform.anchoredPosition;
                hideMoveTo += viewTransform.anchoredPosition;
            }

            showRotateFrom = showBehavior.anim.rotateAnim.rotateFrom;
            showRotateTo = showBehavior.anim.rotateAnim.rotateTo;
            hideRotateFrom = hideBehavior.anim.rotateAnim.rotateFrom;
            hideRotateTo = hideBehavior.anim.rotateAnim.rotateTo;
            if (showBehavior.anim.rotateAnim.relativeAnim) {
                showRotateFrom += viewTransform.localEulerAngles;
                showRotateTo += viewTransform.localEulerAngles;
            }
            if (hideBehavior.anim.rotateAnim.relativeAnim) {
                hideRotateFrom += viewTransform.localEulerAngles;
                hideRotateTo += viewTransform.localEulerAngles;
            }

            showScaleFrom = showBehavior.anim.scaleAnim.scaleFrom;
            showScaleTo = showBehavior.anim.scaleAnim.scaleTo;
            hideScaleFrom = hideBehavior.anim.scaleAnim.scaleFrom;
            hideScaleTo = hideBehavior.anim.scaleAnim.scaleTo;
            if (showBehavior.anim.scaleAnim.relativeAnim) {
                showScaleFrom += viewTransform.localScale;
                showScaleTo += viewTransform.localScale;
            }
            if (hideBehavior.anim.scaleAnim.relativeAnim) {
                hideScaleFrom += viewTransform.localScale;
                hideScaleTo += viewTransform.localScale;
            }

            showFadeFrom = showBehavior.anim.fadeAnim.fadeFrom;
            showFadeTo = showBehavior.anim.fadeAnim.fadeTo;
            hideFadeFrom = hideBehavior.anim.fadeAnim.fadeFrom;
            hideFadeTo = hideBehavior.anim.fadeAnim.fadeTo;
            if (showBehavior.anim.fadeAnim.relativeAnim) {
                showFadeFrom += viewCanvasGroup.alpha;
                showFadeTo += viewCanvasGroup.alpha;
            }
            if (hideBehavior.anim.fadeAnim.relativeAnim) {
                hideFadeFrom += viewCanvasGroup.alpha;
                hideFadeTo += viewCanvasGroup.alpha;
            }
        }

        /// <summary>
        /// 预加载
        /// </summary>
        protected virtual void PreLoad() { }
        /// <summary>
        /// 加载成员数据
        /// </summary>
        protected virtual void LoadMembers() { }
        /// <summary>
        /// 加载视图组件
        /// </summary>
        protected virtual void LoadViews() { }
        /// <summary>
        /// 后处理
        /// </summary>
        protected virtual void PostLoad() { }
        /// <summary>
        /// 手动刷新，无延时
        /// </summary>
        public virtual void UpdateViews() { }

        /// <summary>
        /// 通知界面进行刷新，延时一帧
        /// </summary>
        public void NotifyUpdate() {
            if (gameObject.activeInHierarchy && !UpdateNotifier) {
                UpdateNotifier = true;
                BeforeUpdate();
                ExecuteNextGlobal(ViewUpdate);
            }
        }

        protected virtual void BeforeUpdate() { }

        /// <summary>
        /// 更新方法
        /// </summary>
        private void ViewUpdate() {
            try {
                UpdateViews();
            } catch (Exception e) {
                LogUtils.LogNotice("ERROR in - " + gameObject.name);
                LogUtils.LogError(e.Message + "\n" + e.StackTrace);
            }
            UpdateNotifier = false;
        }

        protected virtual void OnBeforeShow() { }

        protected virtual void OnAfterShow() { }

        protected virtual void OnBeforeHide() { }

        protected virtual void OnAfterHide() { }

        public virtual void SetupSortingLayer(string name) {
            viewCanvas.sortingLayerName = name;
        }

        public virtual void SetupSortingOrder(int order) {
            viewCanvas.sortingOrder = order;
        }

        public virtual void ShowView(bool instantFlag = false) {
            if (!isVisible && !IsAnimating) {
                if (deactivateRootObject) {
                    gameObject.SetActive(true);
                } else {
                    viewCanvasGroup.blocksRaycasts = true;
                    viewCanvasGroup.interactable = true;
                }
                StartCoroutine(ExecuteViewShow(instantFlag));
                NotifyUpdate();
            }
        }

        public virtual void HideView(bool instantFlag = false) {
            if (isVisible && !IsAnimating) {
                StartCoroutine(ExecuteViewHide(instantFlag));
            }
        }

        protected virtual IEnumerator ExecuteViewShow(bool instant) {
            isShowing = true;
            if (!instant && showBehavior.isAnimate) {
                var timer = 0f;
                OnBeforeShow();
                if (showBehavior.startBroadcast) {
                    ViewBehaviorBroadcast(CommonConfigs.BROADCAST_FILTER_VIEW_SHOW, CommonConfigs.BROADCAST_CONTENT_VIEW_BEHAVIOR_START);
                }
                while (CheckBehaviorTimer(timer, showBehavior)) {
                    timer += activeDeltaTime;
                    // -- Move
                    var moveAnim = showBehavior.anim.moveAnim;
                    if (showBehavior.anim.hasMove) {
                        UpdateMoveAnimation(moveAnim.startDelay, moveAnim.animDuration, true, moveAnim.animEase, timer);
                    }
                    // -- Roatate
                    var rotateAnim = showBehavior.anim.rotateAnim;
                    if (showBehavior.anim.hasRotate) {
                        UpdateRotateAnimation(rotateAnim.startDelay, rotateAnim.animDuration, true, rotateAnim.animEase, timer);
                    } else {
                        viewTransform.localEulerAngles = Vector3.zero;
                    }
                    // -- Scale
                    var scaleAnim = showBehavior.anim.scaleAnim;
                    if (showBehavior.anim.hasScale) {
                        UpdateScaleAnimation(scaleAnim.startDelay, scaleAnim.animDuration, true, scaleAnim.animEase, timer);
                    } else {
                        viewTransform.localScale = Vector3.one;
                    }
                    // -- Fade
                    var fadeAnim = showBehavior.anim.fadeAnim;
                    if (showBehavior.anim.hasFade) {
                        UpdateFadeAnimation(fadeAnim.startDelay, fadeAnim.animDuration, true, fadeAnim.animEase, timer);
                    } else {
                        viewCanvasGroup.alpha = 1f;
                    }
                    yield return null;
                }
                // Animation Finish
            }
            OnAfterShow();
            if (showBehavior.finishBroadcast) {
                ViewBehaviorBroadcast(CommonConfigs.BROADCAST_FILTER_VIEW_SHOW, CommonConfigs.BROADCAST_CONTENT_VIEW_BEHAVIOR_FINISH);
            }
            viewCanvasGroup.alpha = 1f;
            isShowing = false;
            isVisible = true;
        }

        protected virtual IEnumerator ExecuteViewHide(bool instant) {
            isHiding = true;
            if (!instant && hideBehavior.isAnimate) {
                var timer = 0f;
                OnBeforeHide();
                if (hideBehavior.startBroadcast) {
                    ViewBehaviorBroadcast(CommonConfigs.BROADCAST_FILTER_VIEW_HIDE, CommonConfigs.BROADCAST_CONTENT_VIEW_BEHAVIOR_START);
                }
                while (CheckBehaviorTimer(timer, hideBehavior)) {
                    timer += activeDeltaTime;
                    // -- Move
                    var moveAnim = hideBehavior.anim.moveAnim;
                    if (hideBehavior.anim.hasMove) {
                        UpdateMoveAnimation(moveAnim.startDelay, moveAnim.animDuration, false, moveAnim.animEase, timer);
                    }
                    // -- Roatate
                    var rotateAnim = hideBehavior.anim.rotateAnim;
                    if (hideBehavior.anim.hasRotate) {
                        UpdateRotateAnimation(rotateAnim.startDelay, rotateAnim.animDuration, false, rotateAnim.animEase, timer);
                    } else {
                        viewTransform.localEulerAngles = Vector3.zero;
                    }
                    // -- Scale
                    var scaleAnim = hideBehavior.anim.scaleAnim;
                    if (hideBehavior.anim.hasScale) {
                        UpdateScaleAnimation(scaleAnim.startDelay, scaleAnim.animDuration, false, scaleAnim.animEase, timer);
                    } else {
                        viewTransform.localScale = Vector3.one;
                    }
                    // -- Fade
                    var fadeAnim = hideBehavior.anim.fadeAnim;
                    if (hideBehavior.anim.hasFade) {
                        UpdateFadeAnimation(fadeAnim.startDelay, fadeAnim.animDuration, false, fadeAnim.animEase, timer);
                    } else {
                        viewCanvasGroup.alpha = 1f;
                    }
                    yield return null;
                }
                // Animation Finish
            }
            OnAfterHide();
            if (hideBehavior.finishBroadcast) {
                ViewBehaviorBroadcast(CommonConfigs.BROADCAST_FILTER_VIEW_HIDE, CommonConfigs.BROADCAST_CONTENT_VIEW_BEHAVIOR_FINISH);
            }
            viewCanvasGroup.alpha = 0f;
            isHiding = false;
            isVisible = false;
            if (deactivateRootObject) {
                gameObject.SetActive(false);
            } else {
                viewCanvasGroup.blocksRaycasts = false;
                viewCanvasGroup.interactable = false;
            }
        }

        protected void ViewBehaviorBroadcast(string filter, string content) {
            var msg = new BroadcastInformation(filter, content);
            msg.PutStringExtra(CommonConfigs.EXTRA_TAG_VIEW_NAME, viewName);
            msg.PutStringExtra(CommonConfigs.EXTRA_TAG_VIEW_CATEGORY, viewCategory);
            SetupBehaviorBroadcast(msg);
            broadcastService.BroadcastInformation(msg);
        }

        protected virtual void SetupBehaviorBroadcast(BroadcastInformation msg) { }

        private void UpdateMoveAnimation(float delay, float duration, bool isShow, AnimEase ease, float timer) {
            var animTime = timer - delay;
            if (animTime > 0f) {
                var startPos = isShow ? showMoveFrom : hideMoveFrom;
                var finishPos = isShow ? showMoveTo : hideMoveTo;
                if (animTime <= duration) {
                    var amount = CalculateAmountByEase(animTime / duration, ease);
                    viewTransform.anchoredPosition = Vector2.LerpUnclamped(startPos, finishPos, amount);
                } else {
                    viewTransform.anchoredPosition = finishPos;
                }
            }
        }

        private void UpdateRotateAnimation(float delay, float duration, bool isShow, AnimEase ease, float timer) {
            var animTime = timer - delay;
            if (animTime > 0f) {
                var startAngle = isShow ? showRotateFrom : hideRotateFrom;
                var finishAngle = isShow ? showRotateTo : hideRotateTo;
                if (animTime <= duration) {
                    var amount = CalculateAmountByEase(animTime / duration, ease);
                    viewTransform.localEulerAngles = Vector3.LerpUnclamped(startAngle, finishAngle, amount);
                } else {
                    viewTransform.localEulerAngles = finishAngle;
                }
            }
        }

        private void UpdateScaleAnimation(float delay, float duration, bool isShow, AnimEase ease, float timer) {
            var animTime = timer - delay;
            if (animTime > 0f) {
                var startScale = isShow ? showScaleFrom : hideScaleFrom;
                var finishScale = isShow ? showScaleTo : hideScaleTo;
                if (animTime <= duration) {
                    var amount = CalculateAmountByEase(animTime / duration, ease);
                    viewTransform.localScale = Vector3.LerpUnclamped(startScale, finishScale, amount);
                } else {
                    viewTransform.localScale = finishScale;
                }
            }
        }

        private void UpdateFadeAnimation(float delay, float duration, bool isShow, AnimEase ease, float timer) {
            var animTime = timer - delay;
            if (animTime > 0f) {
                var startFade = isShow ? showFadeFrom : hideFadeFrom;
                var finishFade = isShow ? showFadeTo : hideFadeTo;
                if (animTime <= duration) {
                    var amount = CalculateAmountByEase(animTime / duration, ease);
                    viewCanvasGroup.alpha = Mathf.Lerp(startFade, finishFade, amount);
                } else {
                    viewCanvasGroup.alpha = finishFade;
                }
            }
        }

        protected bool CheckBehaviorTimer(float time, ViewBehavior behavior) {
            var result = false;
            if (behavior.anim.hasMove) {
                result |= (time < behavior.anim.moveAnim.startDelay + behavior.anim.moveAnim.animDuration);
            }
            if (behavior.anim.hasRotate) {
                result |= (time < behavior.anim.rotateAnim.startDelay + behavior.anim.rotateAnim.animDuration);
            }
            if (behavior.anim.hasScale) {
                result |= (time < behavior.anim.scaleAnim.startDelay + behavior.anim.scaleAnim.animDuration);
            }
            if (behavior.anim.hasFade) {
                result |= (time < behavior.anim.fadeAnim.startDelay + behavior.anim.fadeAnim.animDuration);
            }
            return result;
        }

        protected float CalculateAmountByEase(float normalizeTime, AnimEase animEase) {
            switch (animEase) {
                case AnimEase.Sin:
                    return (Mathf.Sin((normalizeTime - 0.5f) * Mathf.PI) + 1f) / 2f;
                case AnimEase.LinearBounceIn: // 开头Bounce
                    if (normalizeTime < 0.2f) {
                        return -normalizeTime;
                    } else {
                        return normalizeTime * 1.5f - 0.5f;
                    }
                case AnimEase.LinearBounceOut:
                    if (normalizeTime < 0.8f) {
                        return normalizeTime * 1.5f;
                    } else {
                        return -normalizeTime + 2f;
                    }
            }
            return normalizeTime;
        }
    }

    [Serializable]
    public class ViewBehavior {

        public bool isAnimate;
        public UIAnimData anim;
        public bool startBroadcast;
        public bool finishBroadcast;

        public ViewBehavior() {
            isAnimate = false;
            anim = new UIAnimData();
            startBroadcast = false;
            finishBroadcast = false;
        }
    }
}
