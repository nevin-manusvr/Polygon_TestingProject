using System.Collections;
using System.Collections.Generic;
using Hermes.Protocol.Polygon;
using System.Linq;

using UnityEngine;
using GlmSharp;
using Hermes.Tools;

namespace Manus.ToBeHermes 
{
	using System;

	public class Possession
	{
		private static readonly BoneType[] m_RetargetType = 
			{ 
				// Body
				BoneType.Hips,
				BoneType.Spine,
				BoneType.Chest,
				BoneType.UpperChest,
				BoneType.Neck,
				BoneType.Head,

				// Left arm
				BoneType.LeftShoulder,
				BoneType.LeftUpperArm,
				BoneType.LeftLowerArm,
				BoneType.LeftHand,

				// Right arm
				BoneType.RightShoulder,
				BoneType.RightUpperArm,
				BoneType.RightLowerArm,
				BoneType.RightHand,

				// Left leg
				BoneType.LeftUpperLeg,
				BoneType.LeftLowerLeg,
				BoneType.LeftFoot,
				
				// Right leg
				BoneType.RightUpperLeg,
				BoneType.RightLowerLeg,
				BoneType.RightFoot,
			};

		private static int GetBonePriority(BoneType _Type)
		{
			switch (_Type)
			{
				// Main
				case BoneType.Root:
					return 0;
				case BoneType.Head:
					return 100;
				case BoneType.Neck:
					return 90;
				case BoneType.Hips:
					return 30;
				case BoneType.Spine:
				case BoneType.Chest:
				case BoneType.UpperChest:
					return 50;

				// Legs
				case BoneType.LeftUpperLeg:
				case BoneType.RightUpperLeg:
					return 50;
				case BoneType.LeftLowerLeg:
				case BoneType.RightLowerLeg:
					return 0;
				case BoneType.LeftFoot:
				case BoneType.RightFoot:
					return 70;
				case BoneType.LeftToes:
				case BoneType.RightToes:
					return 5;
				case BoneType.LeftToesEnd:
				case BoneType.RightToesEnd:
					return 2;

				// Arms
				case BoneType.LeftShoulder:
				case BoneType.RightShoulder:
					return 50;
				case BoneType.LeftUpperArm:
				case BoneType.RightUpperArm:
					return 10;
				case BoneType.LeftLowerArm:
				case BoneType.RightLowerArm:
					return 0;
				case BoneType.LeftHand:
				case BoneType.RightHand:
					return 90;

				// Hands
				case BoneType.LeftIndexProximal:
				case BoneType.LeftMiddleProximal:
				case BoneType.LeftRingProximal:
				case BoneType.LeftPinkyProximal:
				case BoneType.LeftThumbProximal:
				case BoneType.RightIndexProximal:
				case BoneType.RightMiddleProximal:
				case BoneType.RightRingProximal:
				case BoneType.RightPinkyProximal:
				case BoneType.RightThumbProximal:
					return 3;
				case BoneType.LeftIndexMiddle:
				case BoneType.LeftMiddleMiddle:
				case BoneType.LeftRingMiddle:
				case BoneType.LeftPinkyMiddle:
				case BoneType.LeftThumbMiddle:
				case BoneType.RightIndexMiddle:
				case BoneType.RightMiddleMiddle:
				case BoneType.RightRingMiddle:
				case BoneType.RightPinkyMiddle:
				case BoneType.RightThumbMiddle:
					return 5;
				case BoneType.LeftIndexDistal:
				case BoneType.LeftMiddleDistal:
				case BoneType.LeftRingDistal:
				case BoneType.LeftPinkyDistal:
				case BoneType.LeftThumbDistal:
				case BoneType.RightIndexDistal:
				case BoneType.RightMiddleDistal:
				case BoneType.RightRingDistal:
				case BoneType.RightPinkyDistal:
				case BoneType.RightThumbDistal:
					return 7;
				case BoneType.LeftIndexTip:
				case BoneType.LeftMiddleTip:
				case BoneType.LeftRingTip:
				case BoneType.LeftPinkyTip:
				case BoneType.LeftThumbTip:
				case BoneType.RightIndexTip:
				case BoneType.RightMiddleTip:
				case BoneType.RightRingTip:
				case BoneType.RightPinkyTip:
				case BoneType.RightThumbTip:
					return 10;
				default:
					return 0;

			}
			return 0;
		}

		private Skeleton m_Skeleton;
		private Dictionary<int, Constraint[]> m_Constraints;

		public Possession(Skeleton _Skeleton)
		{
			m_Skeleton = _Skeleton.Clone();
		}

		public void SetTargetSkeleton(Skeleton _TargetSkeleton)
		{
			GenerateHauntedSkeleton(_TargetSkeleton);
		}

		private void GenerateHauntedSkeleton(Skeleton _TargetSkeleton)
		{
			foreach (var t_Bone in m_Skeleton.Bones)
			{
				// This bone does not have to be retargeted
				if (!m_RetargetType.Contains(t_Bone.Type))
					continue;

				foreach (var t_TargetBone in _TargetSkeleton.Bones)
				{
					if (t_Bone.Type == t_TargetBone.Type)
					{
						new Constraint(GetBonePriority(t_Bone.Type), t_Bone, t_TargetBone);
					}
				}
			}
		}

		public void Move()
		{
			
		}

		#region Modules

		public class Constraint
		{
			public int priority;

			public Bone bone;
			public Bone targetBone;

			public Constraint(int _Priority, Bone _Bone, Bone _TargetBone)
			{
				priority = _Priority;
				bone = _Bone;
				targetBone = _TargetBone;
			}
		}

		#endregion
	}
}

