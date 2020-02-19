using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.Core.Hermes;
using GlmSharp;
using Hermes.Protocol;
using Hermes.Tools;
using Manus.Polygon;
using Manus.Polygon.Skeleton;
using Manus.ToBeHermes.Skeleton;

namespace Manus.ToBeHermes.Retargeting
{

	[System.Serializable]
	public struct Priorities
	{
		[Header("Main")]
		public int rootPriority;
		public int hipPriority;
		public int spinePriority;
		public int neckPriority;
		public int headPriority;

		[Header("Arm")]
		public int shoulderPriority;
		public int upperArmPriority;
		public int lowerArmPriority;
		public int handPriority;

		[Header("Leg")]
		public int upperLegPriority;
		public int lowerLegPriority;
		public int footPriority;
		public int toesPriority;
		public int toesEndPriority;

		public int BoneTypeToPriority(BoneType type)
		{
			switch (type)
			{
				case BoneType.Root:

					return rootPriority;
				
				case BoneType.Head:
					
					return headPriority;

				case BoneType.Neck:

					return neckPriority;

				case BoneType.Hips:

					return hipPriority;

				case BoneType.Spine:
				case BoneType.Chest:
				case BoneType.UpperChest:

					return spinePriority;
				
				case BoneType.LeftUpperLeg:
				case BoneType.RightUpperLeg:

					return upperLegPriority;

				case BoneType.LeftLowerLeg:
				case BoneType.RightLowerLeg:

					return lowerLegPriority;

				case BoneType.LeftFoot:
				case BoneType.RightFoot:

					return footPriority;

				case BoneType.LeftToes:
				case BoneType.RightToes:

					return toesPriority;

				case BoneType.LeftToesEnd:
				case BoneType.RightToesEnd:

					return toesEndPriority;

				case BoneType.LeftShoulder:
				case BoneType.RightShoulder:

					return shoulderPriority;

				case BoneType.LeftUpperArm:
				case BoneType.RightUpperArm:

					return upperArmPriority;

				case BoneType.LeftLowerArm:
				case BoneType.RightLowerArm:

					return lowerArmPriority;

				case BoneType.LeftHand:
				case BoneType.RightHand:

					return handPriority;
				
				default:
					ErrorHandler.LogError(ErrorMessage.NotImplemented);
					break;
			}

			return -1;
		}
	}	[System.Serializable]
	public class RetargetContraint
	{
		public int priority = 0;

		public Transform retargetBone;
		public Transform targetBone;

		public RetargetContraint(int priority, Transform retargetBone, Transform targetBone)
		{
			this.priority = priority;
			this.retargetBone = retargetBone;
			this.targetBone = targetBone;

			//Vector3 newPosition
		}
	}

	public class SkeletonRetargeter : MonoBehaviour
	{
		public Priorities priorities;

		public PolygonSkeleton mainSkeleton;
		public PolygonSkeleton retargetSkeleton;

		private SkeletonBoneReferences mainBones;
		private SkeletonBoneReferences retargetBones;

		private Dictionary<BoneType, Bone> mainBonesCollection;
		private Dictionary<BoneType, Bone> retargetBonesCollection;

		public List<RetargetContraint> retargetContraints;

		private void Start()
		{
			mainBones = mainSkeleton.boneReferences;
			retargetBones = retargetSkeleton.boneReferences;

			mainBonesCollection = mainBones.GatherBones();
			retargetBonesCollection = retargetBones.GatherBones();

			retargetContraints = new List<RetargetContraint>();
			GenerateRetargetConstraints();
		}

		private void Update()
		{
			MatchRotations();
		}

		private void GenerateRetargetConstraints()
		{
			foreach (KeyValuePair<BoneType, Bone> bone in retargetBonesCollection)
			{
				foreach (KeyValuePair<BoneType, Bone> mainBone in mainBonesCollection)
				{
					if (bone.Key == mainBone.Key)
					{
						retargetContraints.Add(new RetargetContraint(priorities.BoneTypeToPriority(bone.Key), bone.Value.bone, mainBone.Value.bone));
					}
				}
			}
		}

		private void MatchRotations()
		{
			foreach (KeyValuePair<BoneType, Bone> bone in retargetBonesCollection)
			{
				foreach (KeyValuePair<BoneType, Bone> mainBone in mainBonesCollection)
				{
					if (bone.Key == mainBone.Key)
					{
						bone.Value.bone.rotation = mainBone.Value.bone.rotation;
					}
				}
			}
		}
	}
}