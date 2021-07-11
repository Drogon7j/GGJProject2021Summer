// File create date:2021/7/10
using System;
using System.Collections;
using System.Collections.Generic;
using Pixelplacement;
using RoachGame.Basics;
using Spine.Unity;
using UnityEngine;
using UnityEngine.U2D;
using Spline = Pixelplacement.Spline;

// Created By Yu.Liu
public class TrainController : BaseObject {

	public Spline trainTrail;
	public SpriteShapeController trailSprite;
	
	public GameObject cargoPrefab;
	public SkeletonAnimation[] thrusterList;

	[SpineAnimation] public string thrusterStopAnim;
	[SpineAnimation] public string thrusterWorkAnim;
	[SpineAnimation] public string thrusterWarnAnim;

	public float OffsetDistance { private set; get; }

	private List<TrainCargo> cargoList;

	private float cargoDelay;

	private Rigidbody2D trainBody;
	private float trainTimer = 0f;

	private Vector3 trailDirection;
	private Vector3 offsetToLeft;
	private Vector3 offsetToRight;

	private float offsetDir;
	
	private float offsetPower;

	private float offsetTimePoint;
	private float returnTimePoint;
	private float backTimePoint;
	private bool isOffset;
	private bool isReturn;

	private Vector3 trainTrailSpeed;
	private Vector3 trainRealtimeSpeed;
	private float trailLength;

	private float applyForce;
	private float applyPower;
	private float slowDownRatio;

	private float trainFuel;

	protected override void OnAwake() {
		base.OnAwake();
		trainBody = GetComponent<Rigidbody2D>();
		offsetPower = 0f;
		applyForce = 0f;
		applyPower = 0f;
		offsetDir = 0f;
		trainFuel = 10f;
		slowDownRatio = 0f;
		cargoList = new List<TrainCargo>();
	}

	protected override void OnStart() {
		base.OnStart();
		if (trailSprite != null) {
			trailSprite.spline.Clear();
			for (var i = trainTrail.Anchors.Length - 1; i >= 0; i--) {
				trailSprite.spline.InsertPointAt(0, trainTrail.Anchors[i]
					.Anchor.position);
			}

			for (var i = 0; i < trainTrail.Anchors.Length; i++) {
				SplineAnchor anchor = trainTrail.Anchors[i];
				trailSprite.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
				var position = anchor.transform
					.position;
				trailSprite.spline.SetLeftTangent(i, anchor.InTangent.position -
				                                     position);
				trailSprite.spline.SetRightTangent(i, anchor.OutTangent
					.position - position);
			}
		}
		if (trainTrail != null) {
			trainTrail.CalculateLength();
			trailLength = trainTrail.Length;
			cargoDelay = 2.2f / trailLength;
			trainTimer = 0f;
			StartCoroutine(UpdateTrainMove());
		}

		if (thrusterList != null && thrusterList.Length > 0)
		{
			for (int i = 0; i < thrusterList.Length; i++)
			{
				thrusterList[i].state.SetAnimation(0, thrusterWorkAnim, false);
			}
		}
		
		AppendCargo();
		AppendCargo();
		AppendCargo();
		AppendCargo();
		AppendCargo();
	}

	protected override void OnUpdate() {
		base.OnUpdate();
		trainFuel -= Time.deltaTime;
		if (trainFuel <= 0f) {
			// Game Over
		}
		// if (Input.GetMouseButtonDown(0)) {
		// 	AppendCargo();
		// }
		Accelerate(Time.deltaTime  * 5f);//trainspeed
		// if (Input.GetKey(KeyCode.W))
		// {
		// }
		//  else if (Input.GetKey(KeyCode.S)) {
		// 	SlowDown(Time.deltaTime);
		// }

		
	}

	public void AppendCargo() {
		GameObject cargoObj = Instantiate(cargoPrefab, Vector3.zero,
			Quaternion.identity);
		TrainCargo c = new TrainCargo();
		c.cargoIndex = cargoList.Count;
		c.cargoBody = cargoObj.GetComponent<Rigidbody2D>();
		c.cargoPower = 0f;
		cargoList.Add(c);
	}

	public void GetFuel(float fuel) {
		trainFuel += fuel;
	}

	public float TrainSpeed => 4f * slowDownRatio;

	private IEnumerator UpdateTrainMove() {
		if (trainTrail != null) {
			while (trainTimer < 1f) {
				var deltaTime = Time.deltaTime * 4f / trailLength * slowDownRatio;
				trailDirection = trainTrail.GetDirection(trainTimer);
				offsetToLeft = Vector3.Cross(trailDirection, Vector3.back);
				offsetToRight = -offsetToLeft;
				Vector3 predictPos = trainTrail.GetPosition(trainTimer);
				Vector3 cachePos = predictPos;
				if (!isOffset && !isReturn) {
					// if (Input.GetKeyDown(KeyCode.A)) {
					// 	GoOffset(1f);
					// }
					//
					// if (Input.GetKeyDown(KeyCode.D)) {
					// 	GoOffset(-1f);
					// }
				}

				if (isOffset) {

					// if (Input.GetKey(KeyCode.A)) {
					// 	ApplyPower(1f);
					// }
					//
					// if (Input.GetKey(KeyCode.D)) {
					// 	ApplyPower(-1f);
					// }
					
					offsetPower += Time.deltaTime * offsetDir * applyForce;
					predictPos += offsetToLeft * offsetPower;
				}

				if (isReturn) {
					
					// if (Input.GetKey(KeyCode.A)) {
					// 	ApplyPower(1f);
					// }
					//
					// if (Input.GetKey(KeyCode.D)) {
					// 	ApplyPower(-1f);
					// }
					
					float nPow =
						Mathf.Sign(offsetPower - Time.deltaTime * offsetDir * applyForce);
					if (Mathf.Approximately(nPow, offsetDir)) {
						offsetPower -= Time.deltaTime * offsetDir * applyForce;
						predictPos += offsetToLeft * offsetPower;
					} else {
						isReturn = false;
						offsetPower = 0f;
						backTimePoint = trainTimer;
						for (int i = 0; i < thrusterList.Length; i++)
						{
							if (thrusterList[i].AnimationName != thrusterWorkAnim)
							{
								thrusterList[i].state.SetAnimation(0, thrusterWorkAnim, true);
							}
						}
					}
				}
				trainBody.MovePosition(predictPos);
				OffsetDistance = (predictPos - cachePos).magnitude;
				transform.up = trailDirection;

				foreach (var cargo in cargoList) {
					UpdateCargo(cargo);
				}

				trainTimer += deltaTime;
				yield return null;
			}
		}
	}

	public void GoOffset(float dir) {
		if (!isOffset && !isReturn) {
			isOffset = true;
			isReturn = false;
			offsetDir = dir;
			applyForce = 0.6f;
			offsetTimePoint = trainTimer;
			if (Mathf.Sign(dir) > 0f)
			{
				// Left
				thrusterList[0].state.SetAnimation(0, thrusterWarnAnim, true);
				thrusterList[1].state.SetAnimation(0, thrusterWarnAnim, true);
			}
			else
			{
				// Right
				thrusterList[2].state.SetAnimation(0, thrusterWarnAnim, true);
				thrusterList[3].state.SetAnimation(0, thrusterWarnAnim, true);
			}
		}
	}

	public void ApplyPower(float pow)
	{
		// isOffset = true;//debug test kill this after;
		if (isOffset) {
			if (!Mathf.Approximately(Mathf.Sign(pow), Mathf.Sign(offsetDir))) {
				isReturn = true;
				isOffset = false;
				returnTimePoint = trainTimer;
				applyForce = 0.25f;
			} else {
				applyForce += Time.deltaTime / 2f;
			}
		}

		if (isReturn) {
			if (!Mathf.Approximately(Mathf.Sign(pow), -Mathf.Sign(offsetDir))) {
				isOffset = true;
				isReturn = false;
				offsetTimePoint = trainTimer;
				applyForce = 0.25f;
			} else {
				applyForce += Time.deltaTime / 2f;
			}
		}
	}

	public void SlowDown(float delta) {
		slowDownRatio = Mathf.Clamp(slowDownRatio - delta, 0, 1);
		if (slowDownRatio <= 0f)
		{
			for (int i = 0; i < thrusterList.Length; i++)
			{
				if (thrusterList[i].AnimationName != thrusterStopAnim)
				{
					thrusterList[i].state.SetAnimation(0, thrusterStopAnim, false);
				}
			}
		}
	}

	public void Accelerate(float delta) {
		slowDownRatio = Mathf.Clamp(slowDownRatio + delta, 0, 5);
		// if (slowDownRatio > 0f)
		// {
		// 	for (int i = 0; i < thrusterList.Length; i++)
		// 	{
		// 		if (thrusterList[i].AnimationName != thrusterWorkAnim)
		// 		{
		// 			thrusterList[i].state.SetAnimation(0, thrusterWorkAnim, false);
		// 		}
		// 	}
		// }
	}

	private void UpdateCargo(TrainCargo cargo) {
		float delay = cargoDelay * (cargo.cargoIndex + 1);
		float cargoTimer = trainTimer - delay;
		if (cargoTimer > 0f) {
			Vector3 cargoPos = trainTrail.GetPosition(cargoTimer);
			Vector3 cargoDir = trainTrail.GetDirection(cargoTimer);
			Vector3 cargoLeft = Vector3.Cross(cargoDir, Vector3.back);
			bool chkOffset = cargo.cargoIndex == 0
				? isOffset
				: cargoList[cargo.cargoIndex - 1].isOffset;
			if (chkOffset && !cargo.isOffset && cargoTimer >= offsetTimePoint) {
				cargo.isOffset = true;
				cargo.isReturn = false;
			}

			if (cargo.isOffset) {
				bool chkReturn = cargo.cargoIndex == 0
					? isReturn
					: cargoList[cargo.cargoIndex - 1].isReturn;
				if (chkReturn && !cargo.isReturn &&
				    cargoTimer >= returnTimePoint) {
					cargo.isOffset = false;
					cargo.isReturn = true;
				} else {
					cargo.cargoPower += offsetDir * Time.deltaTime * applyForce;
					cargoPos += cargoLeft * cargo.cargoPower;
				}
			}

			if (cargo.isReturn) {
				float nPow =
					Mathf.Sign(cargo.cargoPower - Time.deltaTime * offsetDir * applyForce);
				if (Mathf.Approximately(nPow, offsetDir)) {
					cargo.cargoPower -= offsetDir * Time.deltaTime * applyForce;
					cargoPos += cargoLeft * cargo.cargoPower;
				} else {
					cargo.isReturn = false;
				}
			}

			cargo.cargoBody.MovePosition(cargoPos);
			cargo.cargoBody.transform.right = cargoDir;
		}
	}
}

public class TrainCargo {
	public int cargoIndex;
	public Rigidbody2D cargoBody;
	public float cargoPower;
	public bool isOffset;
	public bool isReturn;
}