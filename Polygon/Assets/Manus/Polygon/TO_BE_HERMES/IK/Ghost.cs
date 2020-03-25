using System.Collections.Generic;
using GlmSharp;
using GlmMathAddons;
using Hermes.Protocol.Polygon;
using Hermes.Tools;
using Manus.ToBeHermes.Tracking;
using HProt = Hermes.Protocol;

namespace Manus.ToBeHermes.IK
{
	[System.Serializable]
	public class Ghost
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
			public float ankleHeight = 0.13f;
		}

		#endregion

		// Fields
		private uint m_ID;
		public BodyMeasurements measurements;

		private Dictionary<BoneType, GlmBone> bones;
		private Dictionary<ControlBoneType, GlmControl> controls;

		// Properties
		public uint ID { get { return Skeleton.id; } }

		public GlmSkeleton Skeleton { get; internal set; }

		public Ghost(uint _ID)
		{
			// TMP:
			measurements = new BodyMeasurements();
			measurements.playerHeight = 1.83f;
			measurements.neckLength = .1f;
			measurements.spineHeight = .8f;
			measurements.armLength = .8f;
			measurements.legLength = 1.2f;
			measurements.shoulderWidth = .35f;
			measurements.legWidth = .2f;
			// END TMP

			m_ID = _ID;

			GenerateSkeleton(measurements);
		}

		public void GenerateSkeleton(BodyMeasurements _Measurements)
		{
			Skeleton = new GlmSkeleton { id = m_ID };
			
			// Create all bones that are going to be estimated
			Skeleton.bones.Add(new GlmBone { type = BoneType.Head });
			Skeleton.bones.Add(new GlmBone { type = BoneType.Neck });
			Skeleton.bones.Add(new GlmBone { type = BoneType.Hips });
			Skeleton.bones.Add(new GlmBone { type = BoneType.Spine });
			Skeleton.bones.Add(new GlmBone { type = BoneType.Chest });
			Skeleton.bones.Add(new GlmBone { type = BoneType.UpperChest });

			Skeleton.bones.Add(new GlmBone { type = BoneType.LeftShoulder });
			Skeleton.bones.Add(new GlmBone { type = BoneType.LeftUpperArm });
			Skeleton.bones.Add(new GlmBone { type = BoneType.LeftLowerArm });
			Skeleton.bones.Add(new GlmBone { type = BoneType.LeftHand });

			Skeleton.bones.Add(new GlmBone { type = BoneType.RightShoulder });
			Skeleton.bones.Add(new GlmBone { type = BoneType.RightUpperArm });
			Skeleton.bones.Add(new GlmBone { type = BoneType.RightLowerArm });
			Skeleton.bones.Add(new GlmBone { type = BoneType.RightHand });

			Skeleton.bones.Add(new GlmBone { type = BoneType.LeftUpperLeg });
			Skeleton.bones.Add(new GlmBone { type = BoneType.LeftLowerLeg });
			Skeleton.bones.Add(new GlmBone { type = BoneType.LeftFoot });

			Skeleton.bones.Add(new GlmBone { type = BoneType.RightUpperLeg });
			Skeleton.bones.Add(new GlmBone { type = BoneType.RightLowerLeg });
			Skeleton.bones.Add(new GlmBone { type = BoneType.RightFoot });

			// Add all bones to a dictionary for easy access
			bones = new Dictionary<BoneType, GlmBone>();
			foreach (var t_Bone in Skeleton.bones)
			{
				bones.Add(t_Bone.type, t_Bone);
			}

			// Create all control bones
			var t_LeftHeelControl = new GlmControl { type = ControlBoneType.LeftHeelControl };
			t_LeftHeelControl.localBones.Add(new GlmLocalBone { bone = bones[BoneType.LeftFoot], localPosition = new vec3(0, _Measurements.ankleHeight, 0), localRotation = quat.Identity });
			Skeleton.controls.Add(t_LeftHeelControl);

			var t_RightHeelControl = new GlmControl { type = ControlBoneType.RightHeelControl };
			t_RightHeelControl.localBones.Add(new GlmLocalBone { bone = bones[BoneType.RightFoot], localPosition = new vec3(0, _Measurements.ankleHeight, 0), localRotation = quat.Identity });
			Skeleton.controls.Add(t_RightHeelControl);

			var t_HipControl = new GlmControl { type = ControlBoneType.HipControl };
			t_HipControl.localBones.Add(new GlmLocalBone { bone = bones[BoneType.Hips], localPosition = new vec3(0, _Measurements.ankleHeight, 0), localRotation = quat.Identity });
			t_HipControl.localBones.Add(new GlmLocalBone { bone = bones[BoneType.LeftUpperLeg], localPosition = new vec3(0, _Measurements.ankleHeight, 0), localRotation = quat.Identity });
			t_HipControl.localBones.Add(new GlmLocalBone { bone = bones[BoneType.RightUpperLeg], localPosition = new vec3(0, _Measurements.ankleHeight, 0), localRotation = quat.Identity });
			Skeleton.controls.Add(t_HipControl);

			// Add all bones to a dictionary for easy access
			controls = new Dictionary<ControlBoneType, GlmControl>();
			foreach (var t_Control in Skeleton.controls)
			{
				controls.Add(t_Control.type, t_Control);
			}
		}

		public void EstimateBody(Tracker _Head, Tracker _LeftHand, Tracker _RightHand, Tracker _Hip, Tracker _LeftFoot, Tracker _RightFoot)
		{
			// Position
			EstimateNeckPosition(_Head, _Hip);
			EstimateSpinePosition(_Head, _Hip);

			EstimateArmPositions(_Head, _Hip, _LeftHand, true);
			EstimateArmPositions(_Head, _Hip, _RightHand, false);

			EstimateLegPositions(_Hip, _LeftFoot, true);
			EstimateLegPositions(_Hip, _RightFoot, false);


			// Rotation
			//EstimateNeckRotation(_Head, _Hip);
			//EstimateSpineRotation(_Hip);

			//EstimateArmRotations(_Head, _Hip, _LeftHand, true);
			//EstimateArmRotations(_Head, _Hip, _RightHand, false);

			//EstimateLegRotations(_Hip, _LeftFoot, true);
			//EstimateLegRotations(_Hip, _RightFoot, false);
		}

		// Position
		private void EstimateNeckPosition(Tracker _Head, Tracker _Hip)
		{
			vec3 t_DirectionToNeck = _Head.rotation * -vec3.UnitY + _Head.rotation * -vec3.UnitZ;
			vec3 t_DirectionToHip = _Hip.position - _Head.position;
			vec3 t_Position = _Head.position + (t_DirectionToNeck.Normalized + t_DirectionToHip.Normalized).Normalized * measurements.neckLength;
			
			bones[BoneType.Neck].position = t_Position;
			bones[BoneType.Head].position = _Head.position;

			bones[BoneType.Head].rotation = _Head.rotation;

			//#if UNITY_EDITOR
			//			Gizmos.color = Color.cyan;
			//			Gizmos.DrawLine(_Head.position.ToUnityVector3(), neckPos.ToUnityVector3());
			//#endif
		}

		private void EstimateSpinePosition(Tracker _Head, Tracker _Hip)
		{
			vec3 t_HipPosition = _Hip.position;

			vec3 t_NeckPos = bones[BoneType.Neck].position;
			vec3 t_SpineDirection = (_Hip.rotation * -vec3.UnitZ * 3f + _Head.rotation * -vec3.UnitZ * 1f).Normalized; // TODO: fix this so it doesn't flip around
			vec3 t_SpinePos = IK(t_HipPosition, t_NeckPos, (t_HipPosition + t_NeckPos) / 2f + t_SpineDirection, measurements.spineHeight);

			controls[ControlBoneType.HipControl].position = _Hip.position;
			controls[ControlBoneType.HipControl].rotation = _Hip.rotation;

			bones[BoneType.Hips].position = t_HipPosition;
			bones[BoneType.Spine].position = t_SpinePos;
			bones[BoneType.Chest].position = t_SpinePos;
			bones[BoneType.UpperChest].position = t_SpinePos;
		}

		private void EstimateArmPositions(Tracker _Head, Tracker _Hip, Tracker _Hand, bool _Left)
		{
			vec3 t_NeckPos = bones[BoneType.Neck].position;
			vec3 t_SpinePos = bones[BoneType.Spine].position;

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
			t_ModifiedElbowAimDirection = GlmMathExtensions.ProjectOnPlane(t_ModifiedElbowAimDirection, t_ArmDirection).Normalized *
										  (1f - glm.Clamp(vec3.Distance(_Hand.position, t_UpperArmPos) / measurements.armLength, 0.3f, .7f));

			vec3 t_ElbowPos = (_Hand.position + t_UpperArmPos) / 2f + t_ModifiedElbowAimDirection;
			t_ElbowPos = IK(t_UpperArmPos, _Hand.position, t_ElbowPos, measurements.armLength);

			bones[_Left ? BoneType.LeftShoulder : BoneType.RightShoulder].position = t_ShoulderPos;
			bones[_Left ? BoneType.LeftUpperArm : BoneType.RightUpperArm].position = t_UpperArmPos;
			bones[_Left ? BoneType.LeftLowerArm : BoneType.RightLowerArm].position = t_ElbowPos;
			bones[_Left ? BoneType.LeftHand : BoneType.RightHand].position = _Hand.position;

			bones[_Left ? BoneType.LeftHand : BoneType.RightHand].rotation = _Hand.rotation;

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

			float _feetWidth = GlmMathExtensions.Project(t_FootPosition - t_UpperLegPosition, _Hip.rotation * -vec3.UnitX).Length;
			vec3 t_kneeAimDirection = vec3.Cross((t_UpperLegPosition - t_FootPosition).Normalized, _Hip.rotation * -vec3.UnitX + _Hip.rotation * -vec3.UnitZ * _feetWidth * 0.3f).Normalized;
			vec3 t_modifiedKneeAimPosition = (t_kneeAimDirection +
											  (_Foot.rotation * vec3.UnitX * (_Left ? -1 : 1) * t_footInfluence.x +
											   _Foot.rotation * vec3.UnitY * t_footInfluence.y +
											   _Foot.rotation * vec3.UnitZ * t_footInfluence.z).Normalized *
											  t_totalFootInfluence).Normalized;
			t_modifiedKneeAimPosition = GlmMathExtensions.ProjectOnPlane(t_modifiedKneeAimPosition, t_FootPosition - t_UpperLegPosition).Normalized
										* (1f - glm.Clamp(vec3.Distance(t_FootPosition, t_UpperLegPosition) / measurements.legLength, 0.3f, .7f));

			vec3 t_KneePosition = (t_UpperLegPosition + t_FootPosition) / 2f + t_modifiedKneeAimPosition;
			t_KneePosition = IK(t_UpperLegPosition, t_FootPosition, t_KneePosition, measurements.legLength);

			controls[_Left ? ControlBoneType.LeftHeelControl : ControlBoneType.RightHeelControl].position = _Foot.position;
			controls[_Left ? ControlBoneType.LeftHeelControl : ControlBoneType.RightHeelControl].rotation = _Foot.rotation;

			bones[_Left ? BoneType.LeftUpperLeg : BoneType.RightUpperLeg].position = t_UpperLegPosition;
			bones[_Left ? BoneType.LeftLowerLeg : BoneType.RightLowerLeg].position = t_KneePosition;

			bones[_Left ? BoneType.LeftFoot : BoneType.RightFoot].position = _Foot.position;
			bones[_Left ? BoneType.LeftFoot : BoneType.RightFoot].rotation = _Foot.rotation;


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


		// Rotation
		private void EstimateNeckRotation(Tracker _Head, Tracker _Hip)
		{
			quat t_HeadRotation = _Head.rotation;
			quat t_HipRotatation = _Hip.rotation;

			vec3 t_DirectionToHead = bones[BoneType.Head].position - bones[BoneType.Neck].position;
			vec3 t_BackDirection = (t_HeadRotation * -vec3.UnitZ * 2f + t_HipRotatation * -vec3.UnitZ).Normalized;
			quat t_NeckRotation = GlmMathExtensions.LookRotation(t_DirectionToHead.Normalized, t_BackDirection);

			bones[BoneType.Neck].rotation = t_NeckRotation;
			bones[BoneType.Head].rotation = t_HeadRotation;
		}

		private void EstimateSpineRotation(Tracker _Hip)
		{
			quat t_HipTrackerRotation = _Hip.rotation;

			vec3 t_HipAim = (bones[BoneType.Spine].position - bones[BoneType.Hips].position).Normalized;
			vec3 t_HipUp = _Hip.rotation * -vec3.UnitZ;
			quat t_HipRotation = GlmMathExtensions.LookRotation(t_HipAim, t_HipUp);

			vec3 t_AimDirection = (bones[BoneType.Neck].position - bones[BoneType.Spine].position).Normalized;
			vec3 t_SpineDirection = (t_HipTrackerRotation * -vec3.UnitZ * 3f + bones[BoneType.Head].rotation * -vec3.UnitZ * 1f) / 4f; // TODO: fix this so it doesn't flip around
			quat t_SpineRotation = GlmMathExtensions.LookRotation(t_AimDirection, t_SpineDirection);

			bones[BoneType.Hips].rotation = t_HipRotation;
			bones[BoneType.Spine].rotation = t_SpineRotation;
			bones[BoneType.Chest].rotation = t_SpineRotation;
			bones[BoneType.UpperChest].rotation = t_SpineRotation;
		}

		private void EstimateArmRotations(Tracker _Head, Tracker _Hip, Tracker _Hand, bool _Left)
		{
			vec3 t_HipPosition = _Hip.position;
			vec3 t_HeadPosition = _Head.position;
			quat t_HandRotation = _Hand.rotation;
			
			// Bone positions
			vec3 t_ShoulderPosition = bones[_Left ? BoneType.LeftShoulder : BoneType.RightShoulder].position;
			vec3 t_UpperArmPosition = bones[_Left ? BoneType.LeftUpperArm : BoneType.RightUpperArm].position;
			vec3 t_LowerArmPosition = bones[_Left ? BoneType.LeftLowerArm : BoneType.RightLowerArm].position;
			vec3 t_HandPosition = bones[_Left ? BoneType.LeftHand : BoneType.RightHand].position;

			// Shoulder
			vec3 t_ShoulderDirection = (t_UpperArmPosition - t_ShoulderPosition).Normalized;
			quat t_ShoulderRotation = GlmMathExtensions.LookRotation(t_ShoulderDirection, t_HeadPosition - t_HipPosition);

			// UpperArm
			vec3 t_UpperArmDirection = (t_LowerArmPosition - t_UpperArmPosition).Normalized;
			vec3 t_UpperArmUp = t_ShoulderRotation * vec3.UnitY;
			quat t_UpperArmRotation = GlmMathExtensions.LookRotation(t_UpperArmDirection, t_UpperArmUp);

			// LowerArm
			vec3 t_LowerArmDirection = (t_HandPosition - t_LowerArmPosition).Normalized;
			vec3 t_LowerArmUp = vec3.Cross(vec3.Cross(t_LowerArmPosition - t_UpperArmPosition, vec3.UnitY), t_LowerArmPosition - t_UpperArmPosition);
			quat t_lowerArmRotation = GlmMathExtensions.LookRotation(t_LowerArmDirection, t_LowerArmUp);

			bones[_Left ? BoneType.LeftShoulder : BoneType.RightShoulder].rotation = t_ShoulderRotation;
			bones[_Left ? BoneType.LeftUpperArm : BoneType.RightUpperArm].rotation = t_UpperArmRotation;
			bones[_Left ? BoneType.LeftLowerArm : BoneType.RightLowerArm].rotation = t_lowerArmRotation;
			bones[_Left ? BoneType.LeftHand : BoneType.RightHand].rotation = t_HandRotation;
		}

		private void EstimateLegRotations(Tracker _Hip, Tracker _Foot, bool _Left)
		{
			quat t_FootRotation = _Foot.rotation;
			quat t_HipRotation = _Hip.rotation;

			vec3 t_FootPosition = bones[_Left ? BoneType.LeftFoot : BoneType.RightFoot].position;
			vec3 t_LowerLegPosition = bones[_Left ? BoneType.LeftLowerLeg : BoneType.RightLowerLeg].position;
			vec3 t_UpperLegPosition = bones[_Left ? BoneType.LeftUpperLeg : BoneType.RightUpperLeg].position;

			// Upper leg, TODO: fix this
			vec3 t_UpperLegDirection = (t_LowerLegPosition - t_UpperLegPosition).Normalized;
			vec3 t_UpperLegUp = -vec3.Cross(t_FootPosition - t_UpperLegPosition, vec3.Cross(t_FootPosition - t_UpperLegPosition, t_HipRotation * (t_HipRotation * vec3.UnitZ * 3f + t_FootRotation * vec3.UnitZ) / 4f).Normalized);
			quat t_upperLegRotation = GlmMathExtensions.LookRotation(t_UpperLegDirection, t_UpperLegUp);

			// Lower leg, TODO: fix this
			vec3 t_LowerLegDirection = (t_FootPosition - t_LowerLegPosition).Normalized;
			vec3 t_LowerLegUp = t_upperLegRotation * vec3.UnitZ + t_upperLegRotation * vec3.UnitY;
			quat t_LowerLegRotation = GlmMathExtensions.LookRotation(t_LowerLegDirection, t_LowerLegUp);

			bones[_Left ? BoneType.LeftUpperLeg : BoneType.RightUpperLeg].rotation = t_upperLegRotation;
			bones[_Left ? BoneType.LeftLowerLeg : BoneType.RightLowerLeg].rotation = t_LowerLegRotation;
			bones[_Left ? BoneType.LeftFoot : BoneType.RightFoot].rotation = t_FootRotation;
		}

		#region Math extensions

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