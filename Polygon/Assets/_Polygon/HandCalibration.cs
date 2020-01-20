using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManusVR.Polygon
{
	public class HandCalibration : MonoBehaviour
	{
		public HandSkeleton leftHand;
		public HandSkeleton rightHand;

		public CalibrationUI calibrationUI;

		[Header("Calibration Settings")]
		[Range(0f, 1f)] public float fingerOffset = 0.1f;

		private void Start()
		{
			calibrationUI.Calibrate("Hand Offsets", 1f,
				() =>
					{
						// TODO: add calibration
						Vector3 rightHandNormal = CalculatePalmNormal(rightHand.handBoneReferences, false);
						Vector3 leftHandNormal = CalculatePalmNormal(leftHand.handBoneReferences, true);

						Vector3 rightHandForward = GetHandForward(rightHand.handBoneReferences);
						Vector3 leftHandForward = GetHandForward(leftHand.handBoneReferences);

						Vector3 averageNormal = (rightHandNormal * -1f + leftHandNormal) / 2f;
						//Vector3 averageForward = (leftHandForward + rightHandForward) / 2f;

						//leftHand.transform.GetChild(0).rotation *= leftRotationOffset;
						//rightHand.transform.GetChild(0).localRotation *= rightRotationOffset;

						Vector3 palmPosition = (leftHand.handBoneReferences.wrist.bone.position + leftHand.handBoneReferences.index.proximal.bone.position + leftHand.handBoneReferences.pinky.proximal.bone.position) / 3f;
						Debug.DrawRay(palmPosition, averageNormal, Color.cyan, 10f);
						//Debug.DrawRay(palmPosition, averageForward, Color.magenta, 10f);

						palmPosition = (rightHand.handBoneReferences.wrist.bone.position + rightHand.handBoneReferences.index.proximal.bone.position + rightHand.handBoneReferences.pinky.proximal.bone.position) / 3f;
						Debug.DrawRay(palmPosition, averageNormal * -1f, Color.cyan, 10f);
						//Debug.DrawRay(palmPosition, averageForward, Color.magenta, 10f);


						RotateHandToNewPalmNormal(leftHand, true, averageNormal);
						RotateHandToNewPalmNormal(rightHand, false, averageNormal * -1f);
					});
		}

		private void Update()
		{
			GetHandForward(rightHand.handBoneReferences);
			GetHandForward(leftHand.handBoneReferences);

			CalculatePalmNormal(rightHand.handBoneReferences, false);
			CalculatePalmNormal(leftHand.handBoneReferences, true);


			//Vector3 rotationPlane = Vector3.Cross(GetHandForward(rightHand.handBoneReferences), CalculatePalmNormal(rightHand.handBoneReferences, false));
			//Vector3 palmPosition = (rightHand.handBoneReferences.wrist.bone.position + rightHand.handBoneReferences.index.proximal.bone.position + rightHand.handBoneReferences.pinky.proximal.bone.position) / 3f;

			//bool inverse = Vector3.Dot(CalculatePalmNormal(rightHand.handBoneReferences, false) * -1f, Vector3.ProjectOnPlane(GetHandForward(leftHand.handBoneReferences), rotationPlane)) > 0f;
			//float angle = Vector3.Angle(GetHandForward(rightHand.handBoneReferences), Vector3.ProjectOnPlane(GetHandForward(leftHand.handBoneReferences), rotationPlane)) * (inverse ? -1f : 1f);

			//rightHand.transform.GetChild(0).rotation *= Quaternion.AngleAxis(angle, rightHand.transform.GetChild(0).InverseTransformDirection(rotationPlane));

			//Debug.DrawRay(palmPosition, rotationPlane, Color.yellow);
			//Debug.DrawRay(palmPosition, Vector3.ProjectOnPlane(GetHandForward(leftHand.handBoneReferences), rotationPlane), Color.magenta);
			//leftHand.transform.GetChild(0).rotation *= Quaternion.AngleAxis(180f * Time.deltaTime, leftHand.transform.GetChild(0).InverseTransformDirection(GetHandForward(leftHand.handBoneReferences)));
		}

		private void RotateHandToNewPalmNormal(HandSkeleton hand, bool isLeft, Vector3 newPalmNormal)
		{
			Vector3 rotationPlane = Vector3.Cross(GetHandForward(hand.handBoneReferences), CalculatePalmNormal(hand.handBoneReferences, isLeft));

			bool inverseRotation = Vector3.Dot(GetHandForward(hand.handBoneReferences), Vector3.ProjectOnPlane(newPalmNormal, rotationPlane)) > 0f;
			float angleToRotate = Vector3.Angle(CalculatePalmNormal(hand.handBoneReferences, isLeft), Vector3.ProjectOnPlane(newPalmNormal, rotationPlane)) * (inverseRotation ? -1f : 1f);

			hand.transform.GetChild(0).rotation *= Quaternion.AngleAxis(angleToRotate, hand.transform.GetChild(0).InverseTransformDirection(rotationPlane));
		}

		private Vector3 CalculatePalmNormal(HandBoneReferences hand, bool left)
		{
			Vector3 palmPosition = (hand.wrist.bone.position + hand.index.proximal.bone.position + hand.pinky.proximal.bone.position) / 3f;
			Vector3 palmNormal = Vector3.Cross(hand.index.proximal.bone.position - hand.wrist.bone.position, hand.pinky.proximal.bone.position - hand.wrist.bone.position).normalized * (left ? 1f : -1f);
			Debug.DrawRay(palmPosition, palmNormal, Color.blue);

			return palmNormal;
		}

		private Vector3 GetHandForward(HandBoneReferences hand)
		{
			Vector3 palmPosition = (hand.wrist.bone.position + hand.index.proximal.bone.position + hand.pinky.proximal.bone.position) / 3f;
			Vector3 handForward = (hand.middle.proximal.bone.position - hand.wrist.bone.position).normalized;
			Debug.DrawRay(palmPosition, handForward, Color.red);

			return handForward;
		}
	}
}
