using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RootMotion.FinalIK;
using DG.Tweening;

namespace Manus.Polygon
{
	[System.Serializable]
	public class PolygonIK_TMP : IDisposable
	{
		#region fields

		private bool isInitialized = false;
		private bool ikGenerated = false;

		private GameObject ikContainer;

		private Animator animator;
		private SkeletonBoneReferences bones;
		private IKTargets_TMP targets;

		// Settings
		[Header("Spine")]
		public float hipInfluence = 1f;
		public float headInfluence = 1f;

		[Header("Knee")]
		public Vector2 kneeMinMax = new Vector2(-40, 50);
		public Vector3 footInfluenceOnKneeRotation = new Vector3(.1f, 1f, 1f);
		public float totalFootInfluenceOnKneeRotation = 1f;

		[Header("Elbow")]
		public Vector2 elbowMinMax = new Vector2(-30, 80);
		public Vector3 handInfluenceOnElbowRotation = new Vector3(1f, .3f, -1f);
		public float totalHandInfluenceOnElbowRotation = 2f;

		// IK values
		[HideInInspector] public SingleBoneIK hipIK;
		[HideInInspector] public LimbIK[] spineIK;
		[HideInInspector] public LimbIK neckIK;
		[HideInInspector] public LimbIK leftArmIK;
		[HideInInspector] public LimbIK rightArmIK;
		[HideInInspector] public LimbIK leftLegIK;
		[HideInInspector] public LimbIK rightLegIK;

		private float[] spineIKWeight;
		private Tweener ikWeightTween;

		#endregion

		public void InitializeIK(Transform root, Animator animator, SkeletonBoneReferences bones, IKTargets_TMP targets)
		{
			if (isInitialized) return;
			isInitialized = true;

			ikContainer = new GameObject("IK");
			ikContainer.transform.SetParent(root);
			ikContainer.transform.localPosition = Vector3.zero;
			ikContainer.transform.localRotation = Quaternion.identity;

			this.animator = animator;
			this.bones = bones;
			this.targets = targets;
		}

		public void CreateCharacterIK()
		{
			if (ikGenerated) return;
			ikGenerated = true;

			IKExecutionOrder order = ikContainer.AddComponent<IKExecutionOrder>();
			order.animator = animator;

			List<IK> ikComponents = new List<IK>();

			ikComponents.Add(hipIK = ikContainer.AddComponent<SingleBoneIK>());
			hipIK.solver.SetChain(bones.main.bone, bones.main.bone);
			hipIK.solver.target = targets.hip;

			spineIK = new LimbIK[bones.body.spine.Length];
			spineIKWeight = new float[bones.body.spine.Length];
			for (int i = 0; i < bones.body.spine.Length; i++)
			{
				ikComponents.Add(spineIK[i] = CreateIKChain(bones.main.bone, bones.body.spine[i].bone, bones.head.head.bone, bones.main.bone, ikContainer, targets.head.GetChild(0), targets.spine));
				spineIK[i].solver.IKPositionWeight = (1 - 0.2f) / bones.body.spine.Length * (i + 1);
				spineIKWeight[i] = (1 - 0.2f) / bones.body.spine.Length * (i + 1);
			}

			ikComponents.Add(neckIK = CreateIKChain(bones.main.bone, bones.head.neck.bone, bones.head.head.bone, bones.main.bone, ikContainer, targets.head, targets.spine));

			ikComponents.Add(leftArmIK = CreateIKChain(bones.armLeft.upperArm.bone, bones.armLeft.lowerArm.bone, bones.armLeft.hand.wrist.bone, bones.main.bone, ikContainer, targets.leftHand, targets.leftElbow));
			ikComponents.Add(rightArmIK = CreateIKChain(bones.armRight.upperArm.bone, bones.armRight.lowerArm.bone, bones.armRight.hand.wrist.bone, bones.main.bone, ikContainer, targets.rightHand, targets.rightElbow));
			ikComponents.Add(leftLegIK = CreateIKChain(bones.legLeft.upperLeg.bone, bones.legLeft.lowerLeg.bone, bones.legLeft.foot.bone, bones.main.bone, ikContainer, targets.leftFoot, targets.leftKnee));
			ikComponents.Add(rightLegIK = CreateIKChain(bones.legRight.upperLeg.bone, bones.legRight.lowerLeg.bone, bones.legRight.foot.bone, bones.main.bone, ikContainer, targets.rightFoot, targets.rightKnee));

			order.IKComponents = ikComponents.ToArray();

			// Body functions
			hipIK.solver.OnPreUpdate += OnHipPreIK;
			hipIK.solver.OnPostUpdate += OnHipPostIK;

			// Leg functions
			leftLegIK.solver.OnPostUpdate += OnLeftLegPostIK;
			rightLegIK.solver.OnPostUpdate += OnRightLegPostIK;

			// Arm functions
			leftArmIK.solver.OnPostUpdate += OnLeftArmPostIK;
			rightArmIK.solver.OnPostUpdate += OnRightArmPostIK;
		}

		public void SetIKWeigth(float value, float duration = 0f)
		{
			ikWeightTween.Kill();

			value = Mathf.Clamp01(value);
			duration = Mathf.Abs(duration);

			if (!Mathf.Approximately(duration, 0f))
			{
				ikWeightTween = DOVirtual.Float(hipIK.solver.IKPositionWeight, value, duration, SetWeightOfAllIK);
			}
			else
			{
				SetWeightOfAllIK(value);
			}
		}

		#region Private Methods

		private LimbIK CreateIKChain(Transform bone1, Transform bone2, Transform bone3, Transform root, GameObject container, Transform target, Transform bendTarget)
		{
			LimbIK ik = container.AddComponent<LimbIK>();
			ik.solver.SetChain(bone1, bone2, bone3, root);
			ik.solver.bendModifier = IKSolverLimb.BendModifier.Goal;

			ik.solver.target = target;
			ik.solver.bendGoal = bendTarget;

			return ik;
		}

		private void SetWeightOfAllIK(float value)
		{
			hipIK.solver.IKPositionWeight = value;
			hipIK.solver.IKRotationWeight = value;
			neckIK.solver.IKPositionWeight = value;
			neckIK.solver.IKRotationWeight = value;
			leftArmIK.solver.IKPositionWeight = value;
			leftArmIK.solver.IKRotationWeight = value;
			rightArmIK.solver.IKPositionWeight = value;
			rightArmIK.solver.IKRotationWeight = value;
			leftLegIK.solver.IKPositionWeight = value;
			leftLegIK.solver.IKRotationWeight = value;
			rightLegIK.solver.IKPositionWeight = value;
			rightLegIK.solver.IKRotationWeight = value;

			for (var i = 0; i < spineIK.Length; i++)
			{
				LimbIK ik = spineIK[i];
				ik.solver.IKPositionWeight = value * spineIKWeight[i];
				ik.solver.IKRotationWeight = value * spineIKWeight[i];
			}
		}

		private void EstimateSpineAim(Bone hip, Bone head, Transform target)
		{
			// Spine aim
			Vector3 hipRight = hip.bone.right;
			Vector3 headRight = head.bone.right;

			Vector3 averageRight = (hipRight * hipInfluence + headRight * headInfluence).normalized;
			Vector3 lookAt = hip.bone.position - head.bone.position;
			Vector3 aimDirection = Vector3.Cross(averageRight, lookAt).normalized;

			// Position spine
			Vector3 spineAimPosition = hip.bone.position + aimDirection * 1.5f;
			target.position = spineAimPosition;
		}

		private void EstimateElbowAndShoulderPosition(Bone chest, Bone shoulder, Bone hand, Transform target, bool left)
		{
			// Shoulder
			//Vector3 shoulderAim = -(shoulder.bone.position - hand.bone.position).normalized;
			//Vector3 chestForward = BoneReferences.GetConvertedBoneDirection(chest.bone, chest.forward);
			//Vector3 chestUp = BoneReferences.GetConvertedBoneDirection(chest.bone, chest.up);
			//Vector3 chestSide = BoneReferences.GetConvertedBoneDirection(chest.bone, chest.right) * (left ? -1 : 1);
			//float handDistanceToShoulder = Mathf.Clamp01(bones.armLength / shoulderAim.magnitude);

			//float shoulderForwardValue = Vector3.Project(shoulderAim, chestUp).magnitude * Mathf.Sign(Vector3.Dot(chestUp, shoulderAim)) * handDistanceToShoulder;
			//float shoulderUpValue = Vector3.Project(shoulderAim, chestUp).magnitude * Mathf.Sign(Vector3.Dot(chestUp, shoulderAim)) * handDistanceToShoulder;
			//float shoulderSideValue = Vector3.Project(shoulderAim, chestUp).magnitude * Mathf.Sign(Vector3.Dot(chestUp, shoulderAim)) * handDistanceToShoulder;

			//shoulder.bone.localRotation = shoulder.defaultRotation * Quaternion.Euler(0, upCurve.Evaluate(shoulderUpValue) * (left ? 1 : -1), (sideCurve.Evaluate(shoulderSideValue) + forwardCurve.Evaluate(shoulderForwardValue)) * (left ? -1 : 1)); // TODO: fix for other models

			// Elbow
			Vector3 aimDirection = (shoulder.bone.position - hand.bone.position).normalized;
			Vector3 elbowAimRawDirection = Vector3.Cross(aimDirection, Vector3.up).normalized * (left ? -1 : 1);
			Vector3 elbowAimDirection = (elbowAimRawDirection +
								(hand.bone.right * (left ? 1 : -1) * handInfluenceOnElbowRotation.x +
								-hand.bone.up * handInfluenceOnElbowRotation.y +
								hand.bone.forward * handInfluenceOnElbowRotation.z)
								.normalized * totalHandInfluenceOnElbowRotation).normalized;

			Vector3 elbowUpDirection = Vector3.Cross(aimDirection, elbowAimDirection).normalized;
			Vector3 elbowUpRawDirection = Vector3.Cross(aimDirection, elbowAimRawDirection).normalized;
			float elbowAngle = Vector3.Angle(-Vector3.Cross(aimDirection, elbowUpRawDirection), -Vector3.Cross(aimDirection, elbowUpDirection)) * -Mathf.Sign(-Vector3.Cross(aimDirection, elbowUpDirection).y);
			elbowAngle = left ? Mathf.Clamp(elbowAngle, elbowMinMax.x, elbowMinMax.y) :
							 Mathf.Clamp(-elbowAngle, -elbowMinMax.y, -elbowMinMax.x);

			Quaternion lookRotation = Quaternion.LookRotation(elbowAimRawDirection, elbowUpRawDirection) * Quaternion.AngleAxis(elbowAngle, Vector3.right);

			target.position = (shoulder.bone.position + hand.bone.position) / 2f + lookRotation * Vector3.forward;

			Debug.DrawRay((shoulder.bone.position + hand.bone.position) / 2f, lookRotation * Vector3.forward, Color.red);
			Debug.DrawRay((shoulder.bone.position + hand.bone.position) / 2f, -Vector3.Cross(aimDirection, elbowUpRawDirection), Color.green);
			Debug.DrawRay((shoulder.bone.position + hand.bone.position) / 2f, -Vector3.Cross(aimDirection, elbowUpDirection), Color.cyan);
		}

		private void EstimateKneePosition(Bone hip, Bone foot, Transform target, bool left)
		{
			Vector3 aimDirection = hip.bone.position - foot.bone.position;
			Vector3 hipForward = hip.bone.forward;
			Vector3 hipUp = hip.bone.up;
			Vector3 hipRight = hip.bone.right;
			Vector3 footForward = foot.bone.forward;

			Vector3 averageForward = (hipForward + footForward).normalized + hipRight * (left ? -1 : 1) * 0.1f;
			Vector3 kneeForward = -Vector3.Cross(aimDirection, hipRight).normalized;
			Vector3 modifiedKneeForward = (kneeForward +
			                               (foot.bone.right * (left ? -1 : 1) * footInfluenceOnKneeRotation.x +
			                                foot.bone.up * footInfluenceOnKneeRotation.y +
			                                foot.bone.forward * footInfluenceOnKneeRotation.z)
			                               .normalized * totalFootInfluenceOnKneeRotation).normalized;

			target.position = (hip.bone.position + foot.bone.position) / 2f + modifiedKneeForward;
		}

		private void FixFootToGround(Transform footTarget, Bone footBone, Bone toeBone = null, Transform toeEndBone = null, bool isLeft = true)
		{
			if (toeBone == null) return;

			float _footHeight = 0.1346572f;
			float _toeHeight = 0.02811695f;
			float _toeEndHeight = 0.02812012f;

			//float footHeight = _footHeight * bones.legLeft.foot.bone.lossyScale.x;
			//if (footTarget.position.y < footHeight)
			//{
			//	footTarget.position = new Vector3(footTarget.position.x, footHeight, footTarget.position.z);
			//}


			float toeHeight = _toeHeight * bones.legLeft.foot.bone.lossyScale.x;
			if (toeBone.bone.position.y < toeHeight)
			{
				float normalAngle = Mathf.Asin(Mathf.Abs(toeBone.bone.position.y - footBone.bone.position.y) / Vector3.Distance(footBone.bone.position, toeBone.bone.position)) * Mathf.Rad2Deg;
				float newAngle = Mathf.Asin((Mathf.Abs(toeBone.bone.position.y - footBone.bone.position.y) - Mathf.Abs(toeBone.bone.position.y - toeHeight)) / Vector3.Distance(footBone.bone.position, toeBone.bone.position)) * Mathf.Rad2Deg;

				if (!float.IsNaN(normalAngle - newAngle))
				{
					float angle = -(normalAngle - newAngle);
					footBone.bone.localRotation *= Quaternion.Euler(angle, 0, 0);
				}
			}

			if (toeEndBone == null) return;

			float toeEndHeight = _toeEndHeight * bones.legLeft.foot.bone.lossyScale.x;
			toeBone.bone.localRotation = isLeft ? Quaternion.Euler(87.291f, -15.725f, 75.14101f) : Quaternion.Euler(-92.71f, -15.72601f, -75.13998f);

			if (toeEndBone.position.y < toeEndHeight)
			{
				float normalAngle = Mathf.Asin(Mathf.Abs(toeEndBone.position.y - toeBone.bone.position.y) / Vector3.Distance(toeBone.bone.position, toeEndBone.position)) * Mathf.Rad2Deg;
				float newAngle = Mathf.Asin((Mathf.Abs(toeEndBone.position.y - toeBone.bone.position.y) - Mathf.Abs(toeEndBone.position.y - toeEndHeight)) / Vector3.Distance(toeBone.bone.position, toeEndBone.position)) * Mathf.Rad2Deg;
				if (!float.IsNaN(normalAngle - newAngle))
				{
					float angle = -(normalAngle - newAngle);
					toeBone.bone.localRotation *= Quaternion.Euler(angle, 0, 0);
				}
			}
		}

		#endregion

		#region IK Callbacks
		private void OnHipPreIK()
		{
		}

		private void OnHipPostIK()
		{
			EstimateSpineAim(bones.main, bones.head.head, targets.spine);
		}

		private void OnLeftLegPostIK()
		{
			EstimateKneePosition(bones.main, bones.legLeft.foot, targets.leftKnee, true);
			FixFootToGround(targets.leftFoot, bones.legLeft.foot, bones.legLeft.toes, bones.legLeft.toesEnd.bone, true);
		}

		private void OnRightLegPostIK()
		{
			EstimateKneePosition(bones.main, bones.legRight.foot, targets.rightKnee, false);
			FixFootToGround(targets.rightFoot, bones.legRight.foot, bones.legRight.toes, bones.legRight.toesEnd.bone, false);
		}

		private void OnLeftArmPostIK()
		{
			EstimateElbowAndShoulderPosition(bones.body.spine[bones.body.spine.Length - 1], bones.armLeft.shoulder, bones.armLeft.hand.wrist, targets.leftElbow, true);
		}

		private void OnRightArmPostIK()
		{
			EstimateElbowAndShoulderPosition(bones.body.spine[bones.body.spine.Length - 1], bones.armRight.shoulder, bones.armRight.hand.wrist, targets.rightElbow, false);
		}
		#endregion

		public void Dispose()
		{
			MonoBehaviour.Destroy(ikContainer);

			if (ikGenerated)
			{
				// Body functions
				hipIK.solver.OnPreUpdate -= OnHipPreIK;
				hipIK.solver.OnPostUpdate -= OnHipPostIK;

				// Leg functions
				leftLegIK.solver.OnPostUpdate -= OnLeftLegPostIK;
				rightLegIK.solver.OnPostUpdate -= OnRightLegPostIK;

				// Arm functions
				leftArmIK.solver.OnPostUpdate -= OnLeftArmPostIK;
				rightArmIK.solver.OnPostUpdate -= OnRightArmPostIK;
			}
		}
	}
}


