// File create date:2021/5/27
using System;
using System.Collections.Generic;
using UnityEngine;
// Created By Yu.Liu
namespace RoachGame.Components {
	/// <summary>
	/// 倒计时器，用于倒数计时
	/// </summary>
	public class CountdownTimer : BaseTimer {
		
		public CountdownTimer(float length, bool startDone = false) : base(length, startDone) { }

		public override void UpdateTimer(float deltaTime) {
			if (!Done) {
				timer -= deltaTime;
				if (timer <= 0f) {
					Done = true;
					finishCallback?.Invoke();
				}
			}
		}

		public override void ResetTimer() {
			timer = TargetTime;
			Done = false;
		}

		public override void FinishTimer() {
			timer = 0f;
			Done = true;
		}
	}
}