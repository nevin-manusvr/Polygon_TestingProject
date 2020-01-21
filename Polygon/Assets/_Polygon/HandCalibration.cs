using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManusVR.Polygon
{
	using DG.Tweening;

	public class HandCalibration : MonoBehaviour
	{
		public HandSkeleton leftHand;
		public HandSkeleton rightHand;

		public CalibrationUI calibrationUI;

		public bool debug = false;

		[Header("Calibration Settings")]
		[Range(-45f, 45f)] public float fingerAngleOffset = 0;

		private void Start()
		{
			Arc leftHandArc = new Arc(leftHand.transform);
			Arc rightHandArc = new Arc(rightHand.transform);

			calibrationUI.Calibrate("Hand Arc", 3f,
				() =>
					{
						//AlignHands(leftHand, rightHand);
					}, 
				() =>
					{
						leftHandArc.AddMeasurement(leftHand.transform.position);
						rightHandArc.AddMeasurement(rightHand.transform.position);
					}, 
				() =>
					{
						leftHandArc.CalculateArc();
						rightHandArc.CalculateArc();

						Vector3 offset = (leftHandArc.GetOffsetToTracker() + rightHandArc.GetOffsetToTracker()) / 2f;

						leftHand.transform.GetChild(0).localPosition = offset;
						rightHand.transform.GetChild(0).localPosition = offset;

						DOVirtual.DelayedCall(1, () => { calibrationUI.Calibrate("Hand Offsets", 1f, () => { AlignHands(leftHand, rightHand); }); });
					});
		}

		private void Update()
		{
			// GetHandForward(rightHand.handBoneReferences);
			// GetHandForward(leftHand.handBoneReferences);
			   
			// CalculatePalmNormal(rightHand.handBoneReferences, false);
			// CalculatePalmNormal(leftHand.handBoneReferences, true);


			Vector3 rotationPlane = Vector3.Cross(GetHandForward(rightHand.handBoneReferences), CalculatePalmNormal(rightHand.handBoneReferences, false));
			// Debug.DrawRay(rightHand.handBoneReferences.wrist.bone.position, rotationPlane, Color.cyan, 1f);
		}

		private void AlignHands(HandSkeleton leftHand, HandSkeleton rightHand)
		{
			Vector3 rightHandNormal = CalculatePalmNormal(rightHand.handBoneReferences, false);
			Vector3 leftHandNormal = CalculatePalmNormal(leftHand.handBoneReferences, true);
			Vector3 averageNormal = (rightHandNormal * -1f + leftHandNormal) / 2f;

			if (debug)
			{
				Vector3 palmPosition = (leftHand.handBoneReferences.wrist.bone.position + leftHand.handBoneReferences.index.proximal.bone.position + leftHand.handBoneReferences.pinky.proximal.bone.position) / 3f;
				Debug.DrawRay(palmPosition, averageNormal, Color.magenta, 1f);

				palmPosition = (rightHand.handBoneReferences.wrist.bone.position + rightHand.handBoneReferences.index.proximal.bone.position + rightHand.handBoneReferences.pinky.proximal.bone.position) / 3f;
				Debug.DrawRay(palmPosition, averageNormal * -1f, Color.magenta, 1f);
			}
			
			RotateHandToNewPalmNormal(leftHand, true, averageNormal);
			RotateHandToNewPalmNormal(rightHand, false, averageNormal * -1f);
		}

		private void RotateHandToNewPalmNormal(HandSkeleton hand, bool isLeft, Vector3 newPalmNormal)
		{
			Vector3 rotationPlane = Vector3.Cross(GetHandForward(hand.handBoneReferences), CalculatePalmNormal(hand.handBoneReferences, isLeft));
		
			Vector3 newHandForward = Vector3.Cross(CalculatePalmNormal(hand.handBoneReferences, isLeft), rotationPlane);
			bool inverseRotation = Vector3.Dot(newHandForward, Vector3.ProjectOnPlane(newPalmNormal, rotationPlane)) > 0f;

			float angleToRotate = Vector3.Angle(CalculatePalmNormal(hand.handBoneReferences, isLeft), Vector3.ProjectOnPlane(newPalmNormal, rotationPlane));
			//angleToRotate += fingerAngleOffset / 2f;
			angleToRotate *= inverseRotation ? -1f : 1f;
			angleToRotate += fingerAngleOffset;

			hand.transform.GetChild(0).rotation *= Quaternion.AngleAxis(angleToRotate, hand.transform.GetChild(0).InverseTransformDirection(rotationPlane));
		}

		private Vector3 CalculatePalmNormal(HandBoneReferences hand, bool left)
		{
			Vector3 palmNormal = Vector3.Cross(hand.index.proximal.bone.position - hand.wrist.bone.position, hand.pinky.proximal.bone.position - hand.wrist.bone.position).normalized * (left ? 1f : -1f);
			//palmNormal = Vector3.Cross(hand.index.proximal.bone.position - palmNormal * fingerOffset / 2f - hand.wrist.bone.position, hand.pinky.proximal.bone.position - palmNormal * fingerOffset / 2f - hand.wrist.bone.position).normalized * (left ? 1f : -1f);

			if (debug)
			{
				Vector3 palmPosition = (hand.wrist.bone.position + hand.index.proximal.bone.position + hand.pinky.proximal.bone.position) / 3f;
				Debug.DrawRay(palmPosition, palmNormal, Color.blue);
			}

			return palmNormal;
		}

		private Vector3 GetHandForward(HandBoneReferences hand)
		{
			Vector3 handForward = (hand.middle.proximal.bone.position - hand.wrist.bone.position).normalized;

			if (debug)
			{
				Vector3 palmPosition = (hand.wrist.bone.position + hand.index.proximal.bone.position + hand.pinky.proximal.bone.position) / 3f;
				Debug.DrawRay(palmPosition, handForward, Color.red);
			}

			return handForward;
		}

		//private void OnDrawGizmos()
		//{
		//	Vector3 palmNormal = Vector3.Cross(rightHand.handBoneReferences.index.proximal.bone.position - rightHand.handBoneReferences.wrist.bone.position, rightHand.handBoneReferences.pinky.proximal.bone.position - rightHand.handBoneReferences.wrist.bone.position).normalized * (false ? 1f : -1f);
		//	Gizmos.color = Color.cyan;
		//	Gizmos.DrawWireSphere(rightHand.handBoneReferences.index.proximal.bone.position, .01f);
		//	Gizmos.DrawWireSphere(rightHand.handBoneReferences.index.proximal.bone.position + palmNormal * fingerOffset * 0.5f, .01f);
		//}
	}
}
