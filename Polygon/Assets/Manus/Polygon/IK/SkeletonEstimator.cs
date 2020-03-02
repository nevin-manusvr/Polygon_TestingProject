using System.Collections;
using System.Collections.Generic;
using Manus.Polygon;
using UnityEngine;

namespace Manus.ToBeHermes.IK
{
	using System;

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
			public float spineHeight = .6f;
			public float armLength = .55f;
			public float legLength = .80f;
			public float shoulderWidth = .35f;
			public float legWidth = 0.2f;
		}

		public BodyMeasurements measurements;
		public IKTargets_TMP targets;

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

			EstimateNeckPosition(t_trackers[VRTrackerType.Head], t_trackers[VRTrackerType.Waist], out vec3 neck);
			EstimateSpine(t_trackers[VRTrackerType.Head], t_trackers[VRTrackerType.Waist], neck, out vec3 spine);

			EstimateArm(t_trackers[VRTrackerType.Head], t_trackers[VRTrackerType.Waist], t_trackers[VRTrackerType.LeftHand], neck, spine, true, out vec3 leftShoulder, out vec3 leftUpperArm, out vec3 leftElbow);
			EstimateArm(t_trackers[VRTrackerType.Head], t_trackers[VRTrackerType.Waist], t_trackers[VRTrackerType.RightHand], neck, spine, false, out vec3 rightShoulder, out vec3 rightUpperArm, out vec3 rightElbow);

			EstimateLeg(t_trackers[VRTrackerType.Waist], t_trackers[VRTrackerType.LeftFoot], true, out vec3 leftUpperLeg, out vec3 leftKnee);
			EstimateLeg(t_trackers[VRTrackerType.Waist], t_trackers[VRTrackerType.RightFoot], false, out vec3 rightUpperLeg, out vec3 rightKnee);
		}

		private void EstimateNeckPosition(Tracker _head, Tracker _hip, out vec3 neckPos)
		{
			vec3 t_directionToNeck = _head.rotation * -vec3.UnitY + _head.rotation * -vec3.UnitZ;
			vec3 t_directionToHip = _hip.position - _head.position;
			neckPos = _head.position + (t_directionToNeck.Normalized + t_directionToHip.Normalized).Normalized * measurements.neckLength;

#if UNITY_EDITOR
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(_head.position.ToUnityVector3(), neckPos.ToUnityVector3());
#endif
		}

		private void EstimateSpine(Tracker _head, Tracker _hip, vec3 _neckPos, out vec3 spine)
		{
			vec3 t_spineDirection = (_hip.rotation * -vec3.UnitZ * 3f + _head.rotation * -vec3.UnitZ * 1f).Normalized;
			spine = IK(_hip.position, _neckPos, (_hip.position + _neckPos) / 2f + t_spineDirection, measurements.spineHeight);

#if UNITY_EDITOR
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(_hip.position.ToUnityVector3(), spine.ToUnityVector3());
			Gizmos.DrawLine(spine.ToUnityVector3(), _neckPos.ToUnityVector3());
#endif
		}

		private void EstimateArm(Tracker _head, Tracker _hip, Tracker _hand, vec3 _neckPos, vec3 _spinePos, bool _left, out vec3 shoulder, out vec3 upperArm, out vec3 elbow)
		{
			vec3 t_downDirection = ((_spinePos - _neckPos) + (_neckPos - _head.position)).Normalized;
			vec3 t_sideDirection = (_hip.rotation * vec3.UnitX * 2f + _head.rotation * vec3.UnitX).Normalized * (_left ? -1f : 1f);

			shoulder = _neckPos + t_downDirection * 0.05f + t_sideDirection * (measurements.shoulderWidth * .2f / 2f);
			upperArm = shoulder + t_downDirection * 0.05f + t_sideDirection * (measurements.shoulderWidth * .8f / 2f) +
			                  (_hip.rotation * -vec3.UnitZ * 2f + _head.rotation * -vec3.UnitZ).Normalized * 0.025f;

			vec3 t_handInfluence = new vec3(0.1f, 0.1f, 1f);
			float t_totalHandInfluence = 1f;

			vec3 t_armDirection = upperArm - _hand.position;
			vec3 t_elbowAimDirection = vec3.Cross(t_armDirection, vec3.UnitY).Normalized * (_left ? -1 : 1);
			vec3 t_modifiedElbowAimDirection = (t_elbowAimDirection + 
			                           (_hand.rotation * -vec3.UnitX * t_handInfluence.x +
			                            _hand.rotation * -vec3.UnitY * t_handInfluence.y +
			                            _hand.rotation * -vec3.UnitZ * t_handInfluence.z).Normalized *
			                           t_totalHandInfluence).Normalized;
			t_modifiedElbowAimDirection = ProjectOnPlane(t_modifiedElbowAimDirection, t_armDirection).Normalized * 
			                              (1f - glm.Clamp(vec3.Distance(_hand.position, upperArm) / measurements.armLength, 0.3f, .7f));

			elbow = (_hand.position + upperArm) / 2f + t_modifiedElbowAimDirection;
			elbow = IK(upperArm, _hand.position, elbow, measurements.armLength);

#if UNITY_EDITOR
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(_spinePos.ToUnityVector3(), shoulder.ToUnityVector3());
			Gizmos.DrawLine(_neckPos.ToUnityVector3(), shoulder.ToUnityVector3());
			Gizmos.DrawLine(shoulder.ToUnityVector3(), upperArm.ToUnityVector3());
			Gizmos.DrawLine(upperArm.ToUnityVector3(), elbow.ToUnityVector3());
			Gizmos.DrawLine(elbow.ToUnityVector3(), _hand.position.ToUnityVector3());

			Gizmos.color = Color.red;
			Gizmos.DrawRay((upperArm.ToUnityVector3() + _hand.position.ToUnityVector3()) / 2f + t_modifiedElbowAimDirection.ToUnityVector3(), Vector3.up * 0.01f);
#endif
		}

		private void EstimateLeg(Tracker _hip, Tracker _foot, bool _left, out vec3 upperLeg, out vec3 knee) 
		{
			vec3 t_footPosition = _foot.position + _foot.rotation * vec3.UnitY * 0.12f;
			upperLeg = _hip.position + _hip.rotation * vec3.UnitX * (_left ? -1 : 1) * measurements.legWidth * 0.5f;

			float t_totalFootInfluence = 2f;
			vec3 t_footInfluence = new vec3(0.1f, 1f, .8f);
			
			float _feetWidth = Project(t_footPosition - upperLeg, _hip.rotation * -vec3.UnitX).Length;
			vec3 t_kneeAimDirection = vec3.Cross((upperLeg - t_footPosition).Normalized, _hip.rotation * -vec3.UnitX + _hip.rotation * -vec3.UnitZ * _feetWidth * 0.3f).Normalized;
			vec3 t_modifiedKneeAimPosition = (t_kneeAimDirection + 
			                                  (_foot.rotation * vec3.UnitX * (_left ? -1 : 1) * t_footInfluence.x +
			                                  _foot.rotation * vec3.UnitY * t_footInfluence.y +
			                                  _foot.rotation * vec3.UnitZ * t_footInfluence.z).Normalized * 
			                                  t_totalFootInfluence).Normalized;
			t_modifiedKneeAimPosition = ProjectOnPlane(t_modifiedKneeAimPosition, t_footPosition - upperLeg).Normalized 
			                            * (1f - glm.Clamp(vec3.Distance(t_footPosition, upperLeg) / measurements.legLength, 0.3f, .7f));

			knee = (upperLeg + t_footPosition) / 2f + t_modifiedKneeAimPosition;
			knee = IK(upperLeg, t_footPosition, knee, measurements.legLength);

#if UNITY_EDITOR
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(t_footPosition.ToUnityVector3(), _foot.position.ToUnityVector3());
			Gizmos.DrawLine(upperLeg.ToUnityVector3(), _hip.position.ToUnityVector3());
			Gizmos.DrawLine(upperLeg.ToUnityVector3(), knee.ToUnityVector3());
			Gizmos.DrawLine(knee.ToUnityVector3(), t_footPosition.ToUnityVector3());

			Gizmos.color = Color.red;
			Gizmos.DrawRay((upperLeg.ToUnityVector3() + t_footPosition.ToUnityVector3()) / 2f + t_modifiedKneeAimPosition.ToUnityVector3(), Vector3.up * 0.01f);
#endif
		}

		#region Math extensions

		public vec3 Project(vec3 vector, vec3 onNormal)
		{
			float num1 = vec3.Dot(onNormal, onNormal);
			if ((double)num1 < (double)Mathf.Epsilon)
				return vec3.Zero;
			float num2 = vec3.Dot(vector, onNormal);
			return new vec3(onNormal.x * num2 / num1, onNormal.y * num2 / num1, onNormal.z * num2 / num1);
		}

		public vec3 ProjectOnPlane(vec3 vector, vec3 planeNormal)
		{
			float num1 = vec3.Dot(planeNormal, planeNormal);
			if ((double)num1 < (double)Mathf.Epsilon)
				return vector;
			float num2 = vec3.Dot(vector, planeNormal);
			return new vec3(vector.x - planeNormal.x * num2 / num1, vector.y - planeNormal.y * num2 / num1, vector.z - planeNormal.z * num2 / num1);
		}

		private vec3 IK(vec3 _root, vec3 _target, vec3 _aimPosition, float totalLength)
		{
			if (vec3.Distance(_root, _target) > totalLength)
			{
				return (_root + _target) / 2f;
			}

			vec3 t_aimPoint = _aimPosition;
			float t_maxDistance = totalLength / 2f;

			int t_iteration = 0;
			while ((glm.Abs(vec3.Distance(_root, t_aimPoint) - t_maxDistance) > 0.001f || glm.Abs(vec3.Distance(t_aimPoint, _target) - t_maxDistance) > 0.001f) && t_iteration < 150)
			{
				MoveTowards(_root);
				MoveTowards(_target);

				t_iteration++;
			}

			return t_aimPoint;

			void MoveTowards(vec3 pos)
			{
				float t_distance = vec3.Distance(t_aimPoint, pos);
				vec3 t_moveDir = (pos - t_aimPoint).Normalized;

				t_aimPoint += t_moveDir * (t_distance - t_maxDistance);
			}
		}

		#endregion
	}
}