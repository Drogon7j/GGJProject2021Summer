// File create date:2021/5/27
using System;
using System.Collections.Generic;
using UnityEngine;
// Created By Yu.Liu
namespace RoachGame.Components {
	/// <summary>
	/// 带检查点的倒数计时器
	/// </summary>
	public class CheckPointCountdownTimer : CountdownTimer {
		
		protected List<CheckPoint> checkPointList;
		
		public CheckPointCountdownTimer(float length, bool startDone = false) :
			base(length, startDone) {
			checkPointList = new List<CheckPoint>();
		}
		
		public override void UpdateTimer(float deltaTime) {
			if (!Done) {
				timer -= deltaTime;
				if (timer <= 0f) {
					Done = true;
					finishCallback?.Invoke();
				} else {
					EvaluateCheckPoints(timer);
				}
			}
		}

		protected virtual void EvaluateCheckPoints(float current) {
			for (var i = 0; i < checkPointList.Count; i++) {
				if (!checkPointList[i].isChecked) {
					if (checkPointList[i].timePoint >= current) {
						checkPointList[i].checkCallback?.Invoke(current);
						checkPointList[i].isChecked = true;
					}
				}
			}
		}

		protected virtual void ResetCheckPoints() {
			for (var i = 0; i < checkPointList.Count; i++) {
				checkPointList[i].isChecked = false;
			}
		}

		public virtual void
			RegisterCheckPoint(float tp, Action<float> callback) {
			CheckPoint point = new CheckPoint() {
				timePoint = tp,
				checkCallback = callback
			};
			bool isInserted = false;
			for (var i = 0; i < checkPointList.Count; i++) {
				if (checkPointList[i].timePoint < point.timePoint) {
					checkPointList.Insert(i, point);
					isInserted = true;
					break;
				}
			}

			if (!isInserted) {
				checkPointList.Add(point);
			}
		}

		public virtual void UnregisterCheckPoint(float tp,
			Action<float> callback) {
			for (var i = 0; i < checkPointList.Count; i++) {
				if (checkPointList[i].timePoint.Equals(tp) && checkPointList[i]
					.checkCallback == callback) {
					checkPointList.RemoveAt(i);
					break;
				}
			}
		}

		public override void ResetTimer() {
			base.ResetTimer();
			ResetCheckPoints();
		}
		
		/// <summary>
		/// 检查点回调方法类
		/// </summary>
		protected class CheckPoint {
			public float timePoint;
			public Action<float> checkCallback;
			public bool isChecked = false;
		}
	}
}