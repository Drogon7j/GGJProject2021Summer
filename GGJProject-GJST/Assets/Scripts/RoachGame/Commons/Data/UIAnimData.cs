// File create date:2020/10/17
using System;
using UnityEngine;
// Created By Yu.Liu
namespace RoachGame.Basics {

    public enum AnimEase {
        Linear,
        Sin,
        LinearBounceIn,
        LinearBounceOut
    }
    [Serializable]
    public class UIAnimData {

        public bool hasMove;
        public MoveAnim moveAnim;
        public bool hasRotate;
        public RotateAnim rotateAnim;
        public bool hasScale;
        public ScaleAnim scaleAnim;
        public bool hasFade;
        public FadeAnim fadeAnim;

        public UIAnimData() {
            hasMove = false;
            hasRotate = false;
            hasScale = false;
            hasFade = false;
            moveAnim = new MoveAnim();
            rotateAnim = new RotateAnim();
            scaleAnim = new ScaleAnim();
            fadeAnim = new FadeAnim();
        }
    }
    [Serializable]
    public abstract class BaseAnimData {

        public float startDelay;
        public float animDuration;
        public bool relativeAnim;
        public AnimEase animEase;
    }
    [Serializable]
    public class MoveAnim : BaseAnimData {

        public Vector2 moveFrom;
        public Vector2 moveTo;

        public MoveAnim() {
            startDelay = 0f;
            animDuration = 0.25f;
            relativeAnim = true;
            animEase = AnimEase.Linear;
            moveFrom = Vector2.zero;
            moveTo = Vector2.zero;
        }
    }
    [Serializable]
    public class RotateAnim : BaseAnimData {

        public Vector3 rotateFrom;
        public Vector3 rotateTo;

        public RotateAnim() {
            startDelay = 0f;
            animDuration = 0.25f;
            relativeAnim = true;
            animEase = AnimEase.Linear;
            rotateFrom = Vector3.zero;
            rotateTo = Vector3.zero;
        }
    }
    [Serializable]
    public class ScaleAnim : BaseAnimData {

        public Vector3 scaleFrom;
        public Vector3 scaleTo;

        public ScaleAnim() {
            startDelay = 0f;
            animDuration = 0.25f;
            relativeAnim = true;
            animEase = AnimEase.Linear;
            scaleFrom = Vector3.zero;
            scaleTo = Vector3.zero;
        }
    }
    [Serializable]
    public class FadeAnim : BaseAnimData {

        public float fadeFrom;
        public float fadeTo;

        public FadeAnim() {
            startDelay = 0f;
            animDuration = 0.25f;
            relativeAnim = true;
            animEase = AnimEase.Linear;
            fadeFrom = 0f;
            fadeTo = 1f;
        }
    }
}
