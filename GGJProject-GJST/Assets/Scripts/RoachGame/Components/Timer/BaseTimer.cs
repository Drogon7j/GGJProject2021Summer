// File create date:2021/5/27
using System;
using System.Collections.Generic;
using UnityEngine;
// Created By Yu.Liu
namespace RoachGame.Components {
	/// <summary>
	/// 基础计时器，仅有计时功能
	/// </summary>
	public class BaseTimer {

		public float TargetTime { get; protected set; }
		protected float timer = 0f;
		protected Action finishCallback;

		public virtual float Perentage {
			get {
				if (TargetTime <= 0f) {
					return 1;
				} else {
					return timer / TargetTime;
				}
			}
		}

		public bool Done { get; protected set; }

		public BaseTimer(float length, bool startDone = false) {
			TargetTime = length;
			if (!startDone) {
				timer = 0f;
				Done = false;
			} else {
				timer = TargetTime;
				Done = true;
			}
		}

		public virtual void SetupFinishCallback(Action callback) {
			finishCallback = callback;
		}

		public virtual void UpdateTimer(float deltaTime) {
			if (!Done) {
				timer += deltaTime;
				if (timer >= TargetTime) {
					Done = true;
					finishCallback?.Invoke();
				}
			}
		}

		public virtual void ChangeTargetTime(float time) {
			TargetTime = time;
			if (timer > TargetTime) {
				timer = TargetTime;
			}
		}

		public virtual void ResetTimer() {
			timer = 0f;
			Done = false;
		}

		public virtual void FinishTimer() {
			timer = TargetTime;
			Done = true;
			finishCallback?.Invoke();
		}
	}
}