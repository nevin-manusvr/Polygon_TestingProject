using System.Collections;
using System.Collections.Generic;
using Manus.Polygon;
using UnityEngine;

public class SkeletonEstimator : MonoBehaviour
{
	[System.Serializable] // TODO: change this to the calibration profile
	public class BodyMeasurements
	{
		public float playerHeight = 1.83f;
		public float armLength = .55f;
		public float legLength = .80f;
		public float shoulderWidth = .35f;
		public float legWidth = 0.2f;
	}

	public BodyMeasurements measurements;
	public IKTargets_TMP targets;

	public Vector3 footInfluence;
	public float totalFootInfluence;

	private void OnDrawGizmos()
	{
		float radius = 0.05f;
		Gizmos.color = Color.cyan;

		Gizmos.DrawSphere(targets.head.position, radius);
		Gizmos.DrawRay(targets.head.position, targets.head.rotation * Vector3.forward * radius * 3f);

		Gizmos.DrawSphere(targets.hip.position, radius);
		Gizmos.DrawRay(targets.hip.position, targets.hip.rotation * Vector3.forward * radius * 3f);

		Gizmos.DrawSphere(targets.leftHand.position, radius);
		Gizmos.DrawRay(targets.leftHand.position, targets.leftHand.rotation * Vector3.forward * radius * 3f);

		Gizmos.DrawSphere(targets.rightHand.position, radius);
		Gizmos.DrawRay(targets.rightHand.position, targets.rightHand.rotation * Vector3.forward * radius * 3f);

		Gizmos.DrawSphere(targets.leftFoot.position, radius);
		Gizmos.DrawRay(targets.leftFoot.position, targets.leftFoot.rotation * Vector3.forward * radius * 3f);

		Gizmos.DrawSphere(targets.rightFoot.position, radius);
		Gizmos.DrawRay(targets.rightFoot.position, targets.rightFoot.rotation * Vector3.forward * radius * 3f);

		Gizmos.color = Color.blue;
		radius *= 0.5f;

		Gizmos.DrawSphere(targets.hip.position + targets.hip.rotation * Vector3.right * measurements.legWidth * 0.5f, radius);
		Gizmos.DrawSphere(targets.hip.position - targets.hip.rotation * Vector3.right * measurements.legWidth * 0.5f, radius);

		bool left = true;
		Vector3 aimDirection = targets.hip.position - targets.leftFoot.position;
		Vector3 averageForward = (targets.hip.rotation * Vector3.forward + targets.leftFoot.rotation * Vector3.forward).normalized + targets.hip.rotation * Vector3.right * (left ? -1 : 1) * 0.1f;
		Vector3 kneeForward = -Vector3.Cross(aimDirection, targets.hip.rotation * Vector3.right).normalized;
		Vector3 modifiedKneeForward = (kneeForward +
		                               (targets.leftFoot.rotation * Vector3.right * (left ? -1 : 1) * footInfluence.x +
		                                targets.leftFoot.rotation * Vector3.up * footInfluence.y +
		                                targets.leftFoot.rotation * Vector3.forward * footInfluence.z)
		                               .normalized * totalFootInfluence).normalized;

		float multiplier = measurements.legLength / aimDirection.magnitude;
		Gizmos.DrawSphere((targets.hip.position + targets.leftFoot.position) / 2f + modifiedKneeForward * multiplier * 0.5f, radius);
	}
}
