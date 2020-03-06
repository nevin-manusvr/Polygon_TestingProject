using System.Collections;
using System.Collections.Generic;
using Manus.Polygon;

namespace Manus.ToBeHermes.IK
{
	using GlmSharp;

	using Hermes.Protocol.Polygon;
	using Hermes.Tools;

	using Manus.Core.Hermes;
	using Manus.ToBeHermes.Tracking;

	using UnityEngine;

	using HProt = Hermes.Protocol;

	[System.Serializable]
	public class SkeletonEstimator
	{
		#region Data Container

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

		#endregion

		// Fields
		public BodyMeasurements measurements;
		private Dictionary<BoneType, Bone> bones;

		// Properties
		public int ID { get { return (int)Skeleton.DeviceID; } }

		public Skeleton Skeleton { get; internal set; }

		public SkeletonEstimator(int _ID)
		{
			Skeleton = new Skeleton { DeviceID = (uint)_ID };

			// Create all bones that are going to be estimated
			Skeleton.Bones.Add(new Bone { Type = BoneType.Head, Position = new HProt.Translation(), Rotation = new HProt.Orientation() });
			Skeleton.Bones.Add(new Bone { Type = BoneType.Neck, Position = new HProt.Translation(), Rotation = new HProt.Orientation() });
			Skeleton.Bones.Add(new Bone { Type = BoneType.Hips, Position = new HProt.Translation(), Rotation = new HProt.Orientation() });
			Skeleton.Bones.Add(new Bone { Type = BoneType.Spine, Position = new HProt.Translation(), Rotation = new HProt.Orientation() });
			
			Skeleton.Bones.Add(new Bone { Type = BoneType.LeftShoulder, Position = new HProt.Translation(), Rotation = new HProt.Orientation() });
			Skeleton.Bones.Add(new Bone { Type = BoneType.LeftUpperArm, Position = new HProt.Translation(), Rotation = new HProt.Orientation() });
			Skeleton.Bones.Add(new Bone { Type = BoneType.LeftLowerArm, Position = new HProt.Translation(), Rotation = new HProt.Orientation() });
			Skeleton.Bones.Add(new Bone { Type = BoneType.LeftHand, Position = new HProt.Translation(), Rotation = new HProt.Orientation() });

			Skeleton.Bones.Add(new Bone { Type = BoneType.RightShoulder, Position = new HProt.Translation(), Rotation = new HProt.Orientation() });
			Skeleton.Bones.Add(new Bone { Type = BoneType.RightUpperArm, Position = new HProt.Translation(), Rotation = new HProt.Orientation() });
			Skeleton.Bones.Add(new Bone { Type = BoneType.RightLowerArm, Position = new HProt.Translation(), Rotation = new HProt.Orientation() });
			Skeleton.Bones.Add(new Bone { Type = BoneType.RightHand, Position = new HProt.Translation(), Rotation = new HProt.Orientation() });

			Skeleton.Bones.Add(new Bone { Type = BoneType.LeftUpperLeg, Position = new HProt.Translation(), Rotation = new HProt.Orientation() });
			Skeleton.Bones.Add(new Bone { Type = BoneType.LeftLowerLeg, Position = new HProt.Translation(), Rotation = new HProt.Orientation() });
			Skeleton.Bones.Add(new Bone { Type = BoneType.LeftFoot, Position = new HProt.Translation(), Rotation = new HProt.Orientation() });

			Skeleton.Bones.Add(new Bone { Type = BoneType.RightUpperLeg, Position = new HProt.Translation(), Rotation = new HProt.Orientation() });
			Skeleton.Bones.Add(new Bone { Type = BoneType.RightLowerLeg, Position = new HProt.Translation(), Rotation = new HProt.Orientation() });
			Skeleton.Bones.Add(new Bone { Type = BoneType.RightFoot, Position = new HProt.Translation(), Rotation = new HProt.Orientation() });

			// Add all bones to a dictionary for easy access
			bones = new Dictionary<BoneType, Bone>();
			foreach (Bone t_Bone in Skeleton.Bones)
			{
				bones.Add(t_Bone.Type, t_Bone);
			}

			//TMP:
			measurements = new BodyMeasurements();
			measurements.playerHeight = 1.83f;
			measurements.neckLength = .1f;
			measurements.spineHeight = .6f;
			measurements.armLength = .55f;
			measurements.legLength = .8f;
			measurements.shoulderWidth = .35f;
			measurements.legWidth = .2f;
		}
		public void EstimateBody(Tracker _Head, Tracker _LeftHand, Tracker _RightHand, Tracker _Hip, Tracker _LeftFoot, Tracker _RightFoot)
		{
			EstimateNeckPosition(_Head, _Hip);
			EstimateSpinePosition(_Head, _Hip);

			EstimateArmPositions(_Head, _Hip, _LeftHand, true);
			EstimateArmPositions(_Head, _Hip, _RightHand, false);

			EstimateLegPositions(_Hip, _LeftFoot, true);
			EstimateLegPositions(_Hip, _RightFoot, false);

			EstimateNeckRotation(_Head, _Hip);
		}

		//		public IKTargets_TMP targets;

		//		private Dictionary<VRTrackerType, Tracker> GatherTrackerData()
		//		{
		//			var t_trackers = new Dictionary<VRTrackerType, Tracker>();

		//			t_trackers.Add(VRTrackerType.Head, new Tracker(VRTrackerType.Head, targets.head.position.ToGlmVec3(), targets.head.rotation.ToGlmQuat()));
		//			t_trackers.Add(VRTrackerType.Waist, new Tracker(VRTrackerType.Waist, targets.hip.position.ToGlmVec3(), targets.hip.rotation.ToGlmQuat()));
		//			t_trackers.Add(VRTrackerType.LeftHand, new Tracker(VRTrackerType.LeftHand, targets.leftHand.position.ToGlmVec3(), targets.leftHand.rotation.ToGlmQuat()));
		//			t_trackers.Add(VRTrackerType.RightHand, new Tracker(VRTrackerType.RightHand, targets.rightHand.position.ToGlmVec3(), targets.rightHand.rotation.ToGlmQuat()));
		//			t_trackers.Add(VRTrackerType.LeftFoot, new Tracker(VRTrackerType.LeftFoot, targets.leftFoot.position.ToGlmVec3(), targets.leftFoot.rotation.ToGlmQuat()));
		//			t_trackers.Add(VRTrackerType.RightFoot, new Tracker(VRTrackerType.RightFoot, targets.rightFoot.position.ToGlmVec3(), targets.rightFoot.rotation.ToGlmQuat()));

		//			// TODO: for when using trackers instead of transforms, add the profile data (tracker position and rotation offsets)
		//			return t_trackers;
		//		}

		//		private void EstimateBody()
		//		{
		//			var t_trackers = GatherTrackerData();

		//			// Head
		//			if (!t_trackers.ContainsKey(VRTrackerType.Head) || 
		//			    !t_trackers.ContainsKey(VRTrackerType.Waist) || 
		//			    !t_trackers.ContainsKey(VRTrackerType.LeftHand) || 
		//			    !t_trackers.ContainsKey(VRTrackerType.RightHand) || 
		//			    !t_trackers.ContainsKey(VRTrackerType.LeftFoot) ||
		//			    !t_trackers.ContainsKey(VRTrackerType.RightFoot))
		//			{
		//				Debug.LogError("NO");
		//				return;
		//			}

		//			// Limit Tracker Distances

		//			// Estimate
		//			EstimateNeckPosition(t_trackers[VRTrackerType.Head], t_trackers[VRTrackerType.Waist], out vec3 neck);
		//			EstimateSpine(t_trackers[VRTrackerType.Head], t_trackers[VRTrackerType.Waist], neck, out vec3 spine);

		//			EstimateArm(t_trackers[VRTrackerType.Head], t_trackers[VRTrackerType.Waist], t_trackers[VRTrackerType.LeftHand], neck, spine, true, out vec3 leftShoulder, out vec3 leftUpperArm, out vec3 leftElbow);
		//			EstimateArm(t_trackers[VRTrackerType.Head], t_trackers[VRTrackerType.Waist], t_trackers[VRTrackerType.RightHand], neck, spine, false, out vec3 rightShoulder, out vec3 rightUpperArm, out vec3 rightElbow);

		//			EstimateLeg(t_trackers[VRTrackerType.Waist], t_trackers[VRTrackerType.LeftFoot], true, out vec3 leftUpperLeg, out vec3 leftKnee);
		//			EstimateLeg(t_trackers[VRTrackerType.Waist], t_trackers[VRTrackerType.RightFoot], false, out vec3 rightUpperLeg, out vec3 rightKnee);
		//		}

		private void EstimateNeckPosition(Tracker _Head, Tracker _Hip)
		{
			vec3 t_DirectionToNeck = _Head.rotation * -vec3.UnitY + _Head.rotation * -vec3.UnitZ;
			vec3 t_DirectionToHip = _Hip.position - _Head.position;
			vec3 t_Position = _Head.position + (t_DirectionToNeck.Normalized + t_DirectionToHip.Normalized).Normalized * measurements.neckLength;
			
			bones[BoneType.Neck].Position.Full = t_Position.toProtoVec3();
			bones[BoneType.Head].Position.Full = _Head.position.toProtoVec3();

			//#if UNITY_EDITOR
			//			Gizmos.color = Color.cyan;
			//			Gizmos.DrawLine(_Head.position.ToUnityVector3(), neckPos.ToUnityVector3());
			//#endif
		}

		private void EstimateNeckRotation(Tracker _Head, Tracker _Hip)
		{
			vec3 t_DirectionToHead = bones[BoneType.Head].Position.toGlmVec3() - bones[BoneType.Neck].Position.toGlmVec3();
			quat test = LookRotationLH(t_DirectionToHead.Normalized, -vec3.UnitZ);

			// Debug.DrawRay(bones[BoneType.Neck].Position.ToVector3(), t_DirectionToHead.ToUnityVector3(), Color.blue);
			Debug.DrawRay(bones[BoneType.Neck].Position.ToVector3(), (test * vec3.UnitZ).ToUnityVector3(), Color.blue);
			Debug.DrawRay(bones[BoneType.Neck].Position.ToVector3(), (test * vec3.UnitX).ToUnityVector3(), Color.red);
			Debug.DrawRay(bones[BoneType.Neck].Position.ToVector3(), (test * vec3.UnitY).ToUnityVector3(), Color.green);


			//bones[BoneType.Neck].Position.Full = quatLookAt
			bones[BoneType.Head].Rotation.Full = _Head.rotation.toProtoQuat();
		}

		private void EstimateSpinePosition(Tracker _Head, Tracker _Hip)
		{
			vec3 t_NeckPos = bones[BoneType.Neck].Position.toGlmVec3();
			vec3 t_SpineDirection = (_Hip.rotation * -vec3.UnitZ * 3f + _Head.rotation * -vec3.UnitZ * 1f).Normalized;
			vec3 t_SpinePos = IK(_Hip.position, t_NeckPos, (_Hip.position + t_NeckPos) / 2f + t_SpineDirection, measurements.spineHeight);

			bones[BoneType.Spine].Position.Full = t_SpinePos.toProtoVec3();

			//#if UNITY_EDITOR
			//			Gizmos.color = Color.cyan;
			//			Gizmos.DrawLine(_Hip.position.ToUnityVector3(), spine.ToUnityVector3());
			//			Gizmos.DrawLine(spine.ToUnityVector3(), _neckPos.ToUnityVector3());
			//#endif
		}

		private void EstimateArmPositions(Tracker _Head, Tracker _Hip, Tracker _Hand, bool _Left)
		{
			vec3 t_NeckPos = bones[BoneType.Neck].Position.toGlmVec3();
			vec3 t_SpinePos = bones[BoneType.Spine].Position.toGlmVec3();

			vec3 t_DownDirection = ((t_SpinePos - t_NeckPos) + (t_NeckPos - _Head.position)).Normalized;
			vec3 t_SideDirection = (_Hip.rotation * vec3.UnitX * 2f + _Head.rotation * vec3.UnitX).Normalized * (_Left ? -1f : 1f);

			vec3 t_ShoulderPos = t_NeckPos + t_DownDirection * 0.05f + t_SideDirection * (measurements.shoulderWidth * .2f / 2f);
			vec3 t_UpperArmPos = t_ShoulderPos + t_DownDirection * 0.05f + t_SideDirection * (measurements.shoulderWidth * .8f / 2f) +
							  (_Hip.rotation * -vec3.UnitZ * 2f + _Head.rotation * -vec3.UnitZ).Normalized * 0.025f;

			vec3 t_HandInfluence = new vec3(0.1f, 0.3f, 1f);
			float t_TotalHandInfluence = 1f;

			vec3 t_ArmDirection = t_UpperArmPos - _Hand.position;
			vec3 t_ElbowAimDirection = vec3.Cross(t_ArmDirection, vec3.UnitY).Normalized * (_Left ? -1 : 1);
			vec3 t_ModifiedElbowAimDirection = (t_ElbowAimDirection +
									   (_Hand.rotation * -vec3.UnitX * t_HandInfluence.x +
									    _Hand.rotation * -vec3.UnitY * t_HandInfluence.y +
									    _Hand.rotation * -vec3.UnitZ * t_HandInfluence.z).Normalized *
									   t_TotalHandInfluence).Normalized;
			t_ModifiedElbowAimDirection = ProjectOnPlane(t_ModifiedElbowAimDirection, t_ArmDirection).Normalized *
										  (1f - glm.Clamp(vec3.Distance(_Hand.position, t_UpperArmPos) / measurements.armLength, 0.3f, .7f));

			vec3 t_ElbowPos = (_Hand.position + t_UpperArmPos) / 2f + t_ModifiedElbowAimDirection;
			t_ElbowPos = IK(t_UpperArmPos, _Hand.position, t_ElbowPos, measurements.armLength);

			bones[_Left ? BoneType.LeftShoulder : BoneType.RightShoulder].Position.Full = t_ShoulderPos.toProtoVec3();
			bones[_Left ? BoneType.LeftUpperArm : BoneType.RightUpperArm].Position.Full = t_UpperArmPos.toProtoVec3();
			bones[_Left ? BoneType.LeftLowerArm : BoneType.RightLowerArm].Position.Full = t_ElbowPos.toProtoVec3();
			bones[_Left ? BoneType.LeftHand : BoneType.RightHand].Position.Full = _Hand.position.toProtoVec3();

			//#if UNITY_EDITOR
			//			Gizmos.color = Color.cyan;
			//			Gizmos.DrawLine(_spinePos.ToUnityVector3(), shoulder.ToUnityVector3());
			//			Gizmos.DrawLine(_neckPos.ToUnityVector3(), shoulder.ToUnityVector3());
			//			Gizmos.DrawLine(shoulder.ToUnityVector3(), upperArm.ToUnityVector3());
			//			Gizmos.DrawLine(upperArm.ToUnityVector3(), elbow.ToUnityVector3());
			//			Gizmos.DrawLine(elbow.ToUnityVector3(), _hand.position.ToUnityVector3());

			//			Gizmos.color = Color.red;
			//			Gizmos.DrawRay((upperArm.ToUnityVector3() + _hand.position.ToUnityVector3()) / 2f + t_modifiedElbowAimDirection.ToUnityVector3(), Vector3.up * 0.01f);
			//#endif
		}

		private void EstimateLegPositions(Tracker _Hip, Tracker _Foot, bool _Left)
		{
			vec3 t_FootPosition = _Foot.position + _Foot.rotation * vec3.UnitY * 0.12f;
			vec3 t_UpperLegPosition = _Hip.position + _Hip.rotation * vec3.UnitX * (_Left ? -1 : 1) * measurements.legWidth * 0.5f;

			float t_totalFootInfluence = 2f;
			vec3 t_footInfluence = new vec3(0.1f, 1f, .8f);

			float _feetWidth = Project(t_FootPosition - t_UpperLegPosition, _Hip.rotation * -vec3.UnitX).Length;
			vec3 t_kneeAimDirection = vec3.Cross((t_UpperLegPosition - t_FootPosition).Normalized, _Hip.rotation * -vec3.UnitX + _Hip.rotation * -vec3.UnitZ * _feetWidth * 0.3f).Normalized;
			vec3 t_modifiedKneeAimPosition = (t_kneeAimDirection +
											  (_Foot.rotation * vec3.UnitX * (_Left ? -1 : 1) * t_footInfluence.x +
											   _Foot.rotation * vec3.UnitY * t_footInfluence.y +
											   _Foot.rotation * vec3.UnitZ * t_footInfluence.z).Normalized *
											  t_totalFootInfluence).Normalized;
			t_modifiedKneeAimPosition = ProjectOnPlane(t_modifiedKneeAimPosition, t_FootPosition - t_UpperLegPosition).Normalized
										* (1f - glm.Clamp(vec3.Distance(t_FootPosition, t_UpperLegPosition) / measurements.legLength, 0.3f, .7f));

			vec3 t_KneePosition = (t_UpperLegPosition + t_FootPosition) / 2f + t_modifiedKneeAimPosition;
			t_KneePosition = IK(t_UpperLegPosition, t_FootPosition, t_KneePosition, measurements.legLength);

			bones[_Left ? BoneType.LeftUpperLeg : BoneType.RightUpperLeg].Position.Full = t_UpperLegPosition.toProtoVec3();
			bones[_Left ? BoneType.LeftLowerLeg : BoneType.RightLowerLeg].Position.Full = t_KneePosition.toProtoVec3();
			bones[_Left ? BoneType.LeftFoot : BoneType.RightFoot].Position.Full = _Foot.position.toProtoVec3();


			//#if UNITY_EDITOR
			//			Gizmos.color = Color.cyan;
			//			Gizmos.DrawLine(t_footPosition.ToUnityVector3(), _foot.position.ToUnityVector3());
			//			Gizmos.DrawLine(upperLeg.ToUnityVector3(), _hip.position.ToUnityVector3());
			//			Gizmos.DrawLine(upperLeg.ToUnityVector3(), knee.ToUnityVector3());
			//			Gizmos.DrawLine(knee.ToUnityVector3(), t_footPosition.ToUnityVector3());

			//			Gizmos.color = Color.red;
			//			Gizmos.DrawRay((upperLeg.ToUnityVector3() + t_footPosition.ToUnityVector3()) / 2f + t_modifiedKneeAimPosition.ToUnityVector3(), Vector3.up * 0.01f);
			//#endif
		}

		#region Math extensions

		public vec3 Project(vec3 vector, vec3 onNormal)
		{
			float num1 = vec3.Dot(onNormal, onNormal);
			if ((double)num1 < double.Epsilon)
				return vec3.Zero;
			float num2 = vec3.Dot(vector, onNormal);
			return new vec3(onNormal.x * num2 / num1, onNormal.y * num2 / num1, onNormal.z * num2 / num1);
		}

		public vec3 ProjectOnPlane(vec3 vector, vec3 planeNormal)
		{
			float num1 = vec3.Dot(planeNormal, planeNormal);
			if ((double)num1 < double.Epsilon)
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

		public quat LookRotationLH(vec3 direction, vec3 up)
		{
			mat3 result = new mat3();

			result.Column2 = direction;
			result.Column0 = vec3.Cross(up, result.Column2).Normalized;
			result.Column1 = vec3.Cross(result.Column2, result.Column0);
			
			return Cast(result);
		}

		public quat Cast(mat3 m)
		{
			float fourXSquaredMinus1 = m[0, 0] - m[1, 1] - m[2, 2];
			float fourYSquaredMinus1 = m[1, 1] - m[0, 0] - m[2, 2];
			float fourZSquaredMinus1 = m[2, 2] - m[0, 0] - m[1, 1];
			float fourWSquaredMinus1 = m[0, 0] + m[1, 1] + m[2, 2];

			int biggestIndex = 0;
			float fourBiggestSquaredMinus1 = fourWSquaredMinus1;

			if (fourXSquaredMinus1 > fourBiggestSquaredMinus1)
			{
				fourBiggestSquaredMinus1 = fourXSquaredMinus1;
				biggestIndex = 1;
			}

			if (fourYSquaredMinus1 > fourBiggestSquaredMinus1)
			{
				fourBiggestSquaredMinus1 = fourYSquaredMinus1;
				biggestIndex = 2;
			}

			if (fourZSquaredMinus1 > fourBiggestSquaredMinus1)
			{
				fourBiggestSquaredMinus1 = fourZSquaredMinus1;
				biggestIndex = 3;
			}

			float biggestVal = glm.Sqrt(fourBiggestSquaredMinus1 + 1f) * 0.5f;
			float mult = 0.25f / biggestVal;

			switch (biggestIndex)
			{
				case 0:
					return new quat(biggestVal, (m[1, 2] - m[2, 1]) * mult, (m[2, 0] - m[0, 2]) * mult, (m[0, 1] - m[1, 0]) * mult);
				case 1:
					return new quat((m[1, 2] - m[2, 1]) * mult, biggestVal, (m[0, 1] + m[1, 0]) * mult, (m[2, 0] + m[0, 2]) * mult);
				case 2:
					return new quat((m[2, 0] - m[0, 2]) * mult, (m[0, 1] + m[1, 0]) * mult, biggestVal, (m[1, 2] + m[2, 1]) * mult);
				case 3:
					return new quat((m[0, 1] - m[1, 0]) * mult, (m[2, 0] + m[0, 2]) * mult, (m[1, 2] + m[2, 1]) * mult, biggestVal);
				default:
					return new quat(1, 0, 0, 0);
			}
		}

		#endregion
	}
}