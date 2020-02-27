using System.Collections;
using System.Collections.Generic;
using Manus.Polygon;
using UnityEngine;

namespace Manus.ToBeHermes.IK
{
	using GlmSharp;
	using Manus.Core.Hermes;
	using Manus.Core.Utility;
	using Manus.ToBeHermes.Tracking;

	public class SkeletonEstimator : MonoBehaviour
	{
		[System.Serializable] // TODO: change this to the calibration profile
		public class BodyMeasurements
		{
			public float playerHeight = 1.83f;
			public float neckLength = .1f;
			public float armLength = .55f;
			public float legLength = .80f;
			public float shoulderWidth = .35f;
			public float legWidth = 0.2f;
		}

		public BodyMeasurements measurements;
		public IKTargets_TMP targets;

		//public Vector3 footInfluence;
		//public float totalFootInfluence;

		//private void OnDrawGizmos()
		//{
		//	float radius = 0.05f;
		//	Gizmos.color = Color.cyan;

		//	Gizmos.DrawSphere(targets.head.position, radius);
		//	Gizmos.DrawRay(targets.head.position, targets.head.rotation * Vector3.forward * radius * 3f);

		//	Gizmos.DrawSphere(targets.hip.position, radius);
		//	Gizmos.DrawRay(targets.hip.position, targets.hip.rotation * Vector3.forward * radius * 3f);

		//	Gizmos.DrawSphere(targets.leftHand.position, radius);
		//	Gizmos.DrawRay(targets.leftHand.position, targets.leftHand.rotation * Vector3.forward * radius * 3f);

		//	Gizmos.DrawSphere(targets.rightHand.position, radius);
		//	Gizmos.DrawRay(targets.rightHand.position, targets.rightHand.rotation * Vector3.forward * radius * 3f);

		//	Gizmos.DrawSphere(targets.leftFoot.position, radius);
		//	Gizmos.DrawRay(targets.leftFoot.position, targets.leftFoot.rotation * Vector3.forward * radius * 3f);

		//	Gizmos.DrawSphere(targets.rightFoot.position, radius);
		//	Gizmos.DrawRay(targets.rightFoot.position, targets.rightFoot.rotation * Vector3.forward * radius * 3f);

		//	Gizmos.color = Color.blue;
		//	radius *= 0.5f;

		//	Gizmos.DrawSphere(targets.hip.position + targets.hip.rotation * Vector3.right * measurements.legWidth * 0.5f, radius);
		//	Gizmos.DrawSphere(targets.hip.position - targets.hip.rotation * Vector3.right * measurements.legWidth * 0.5f, radius);

		//	bool left = true;
		//	Vector3 aimDirection = targets.hip.position - targets.leftFoot.position;
		//	Vector3 averageForward = (targets.hip.rotation * Vector3.forward + targets.leftFoot.rotation * Vector3.forward).normalized + targets.hip.rotation * Vector3.right * (left ? -1 : 1) * 0.1f;
		//	Vector3 kneeForward = -Vector3.Cross(aimDirection, targets.hip.rotation * Vector3.right).normalized;
		//	Vector3 modifiedKneeForward = (kneeForward +
		//	                               (targets.leftFoot.rotation * Vector3.right * (left ? -1 : 1) * footInfluence.x +
		//	                                targets.leftFoot.rotation * Vector3.up * footInfluence.y +
		//	                                targets.leftFoot.rotation * Vector3.forward * footInfluence.z)
		//	                               .normalized * totalFootInfluence).normalized;

		//	float multiplier = measurements.legLength / aimDirection.magnitude;
		//	Gizmos.DrawSphere((targets.hip.position + targets.leftFoot.position) / 2f + modifiedKneeForward * multiplier * 0.5f, radius);
		//}

		#region MonoBehaviour callbacks

		private void OnDrawGizmos()
		{
			EstimateBody();
		}

		#endregion

		private Dictionary<VRTrackerType, Tracker> GatherTrackerData()
		{
			var t_trackers = new Dictionary<VRTrackerType, Tracker>();

			t_trackers.Add(VRTrackerType.Head, new Tracker(VRTrackerType.Head, targets.head.position.ToGlmVec3(), targets.head.rotation.ToGlmQuat()));
			t_trackers.Add(VRTrackerType.Waist, new Tracker(VRTrackerType.Waist, targets.hip.position.ToGlmVec3(), targets.hip.rotation.ToGlmQuat()));
			t_trackers.Add(VRTrackerType.LeftHand, new Tracker(VRTrackerType.LeftHand, targets.leftHand.position.ToGlmVec3(), targets.leftHand.rotation.ToGlmQuat()));
			t_trackers.Add(VRTrackerType.RightHand, new Tracker(VRTrackerType.RightHand, targets.rightHand.position.ToGlmVec3(), targets.rightHand.rotation.ToGlmQuat()));
			t_trackers.Add(VRTrackerType.LeftFoot, new Tracker(VRTrackerType.LeftFoot, targets.leftFoot.position.ToGlmVec3(), targets.leftFoot.rotation.ToGlmQuat()));
			t_trackers.Add(VRTrackerType.RightFoot, new Tracker(VRTrackerType.RightFoot, targets.rightFoot.position.ToGlmVec3(), targets.rightFoot.rotation.ToGlmQuat()));

			// TODO: for when using trackers instead of transforms, add the profile data (tracker position and rotation offsets)
			return t_trackers;
		}

		private void EstimateBody()
		{
			var t_trackers = GatherTrackerData();

			// Head
			if (!t_trackers.ContainsKey(VRTrackerType.Head) || 
			    !t_trackers.ContainsKey(VRTrackerType.Waist) || 
			    !t_trackers.ContainsKey(VRTrackerType.LeftHand) || 
			    !t_trackers.ContainsKey(VRTrackerType.RightHand) || 
			    !t_trackers.ContainsKey(VRTrackerType.LeftFoot) ||
			    !t_trackers.ContainsKey(VRTrackerType.RightFoot))
			{
				Debug.LogError("NO");
				return;
			}

			EstimateNeckPosition(t_trackers[VRTrackerType.Head], t_trackers[VRTrackerType.Waist]);
			EstimateLeg(t_trackers[VRTrackerType.Waist], t_trackers[VRTrackerType.LeftFoot], true);
		}

		private void EstimateNeckPosition(Tracker _head, Tracker _hip)
		{
			vec3 t_directionToNeck = _head.rotation * -vec3.UnitY + _head.rotation * -vec3.UnitZ * 0.5f;
			vec3 t_directionToHip = _hip.position - _head.position;
			vec3 t_neckPos = _head.position + (t_directionToNeck.Normalized + t_directionToHip.Normalized).Normalized * measurements.neckLength;

			Debug.DrawRay(t_neckPos.ToUnityVector3(), Vector3.up * .01f, Color.red);
		}

		private void EstimateLeg(Tracker _hip, Tracker _foot, bool left)
		{
			vec3 t_upperLegPosition = _hip.position + _hip.rotation * vec3.UnitX * (left ? -1 : 1) * measurements.legWidth * 0.5f;
			
			
			Debug.DrawRay(t_upperLegPosition.ToUnityVector3(), Vector3.up * .01f, Color.red);
		}
	}
}