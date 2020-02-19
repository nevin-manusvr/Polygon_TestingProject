using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Manus.Core.Hermes;
using GlmSharp;
using Hermes.Protocol;
using Hermes.Tools;
using Manus.Polygon;
using Manus.Polygon.Skeleton;
using Manus.ToBeHermes.Skeleton;
using Color = UnityEngine.Color;

namespace Manus.ToBeHermes.Retargeting
{
	using Manus.Polygon.Skeleton.Utilities;

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

		public Vector3 newPosition;

		public List<RetargetChain> chains;

		public RetargetContraint(int priority, Transform retargetBone, Transform targetBone)
		{
			this.priority = priority;
			this.retargetBone = retargetBone;
			this.targetBone = targetBone;

			newPosition = this.retargetBone.position;
		}

		public void PositionRetargetBone()
		{
			newPosition = targetBone.position;
		}

		public void SolvePosition(float quality)
		{
			quality = Mathf.Lerp(1, 0.001f, quality);

			if (chains == null) 
				return;

			foreach (RetargetChain chain in chains)
			{
				float distance = Vector3.Distance(newPosition, chain.endConstraint.newPosition);
				float difference = chain.maxDistance - distance;

				if (chain.directConnection || difference < 0)
				{
					Vector3 direction = (chain.endConstraint.newPosition - newPosition).normalized;
					newPosition = newPosition - direction * difference * (chain.endConstraint.priority > priority ? 1 : 0.5f) * quality;
				}
			}
		}

		public void ApplyRetargetingPosition()
		{
			retargetBone.position = newPosition;
		}

		public void ApplyRetargetingRotation()
		{
			retargetBone.rotation = targetBone.rotation;
		}

		public void ApplyRetargetingRotation(Vector3 aimDirection)
		{
			if (aimDirection == Vector3.zero)
			{
				ApplyRetargetingRotation();
				return;
			}

			retargetBone.rotation = Quaternion.LookRotation(aimDirection, targetBone.rotation * Vector3.up);
		}

		public void FindChains(Transform root, RetargetContraint[] allContraints)
		{
			if (retargetBone == root) return;

			chains = new List<RetargetChain>();
			TraverseHierachy(retargetBone, new List<Transform>(), root, allContraints);
		}

		private void TraverseHierachy(Transform currentNode, List<Transform> path, Transform limit, RetargetContraint[] allContraints)
		{
			path = new List<Transform>(path);
			path.Add(currentNode);

			foreach (RetargetContraint contraint in allContraints)
			{
				if (contraint.retargetBone == currentNode && currentNode != retargetBone)
				{
					if (contraint.priority >= priority)
					{
						chains.Add(new RetargetChain(contraint, path.ToArray(), allContraints));
						return;
					}
				}
			}

			// Continue Path
			if (!path.Contains(currentNode.parent) && currentNode.parent != limit)
				TraverseHierachy(currentNode.parent, path, limit, allContraints);

			for (int i = 0; i < currentNode.childCount; i++)
			{
				Transform child = currentNode.GetChild(i);
				if (!path.Contains(child) && child != limit)
					TraverseHierachy(child, path, limit, allContraints);
			}
		}
	}

	[System.Serializable]
	public class RetargetChain
	{
		public RetargetContraint endConstraint;
		public float maxDistance;

		public bool directConnection;

		public Transform[] chain;

		public RetargetChain(RetargetContraint constraint, Transform[] chain, RetargetContraint[] allContraints)
		{
			endConstraint = constraint;
			this.chain = chain;

			// Calculate max length
			float length = 0;
			for (int i = 0; i < this.chain.Length - 1; i++)
			{
				length += Vector3.Distance(this.chain[i].position, this.chain[i + 1].position);
			}

			int constraintCountInChain = 0;
			foreach (Transform transform in chain)
			{
				foreach (RetargetContraint contraint in allContraints)
				{
					if (contraint.retargetBone == transform)
					{
						constraintCountInChain++;
						break;
					}
				}
			}

			directConnection = constraintCountInChain < 3;
			maxDistance = length;
		}
	}

	public class SkeletonRetargeter : MonoBehaviour
	{
		public Priorities priorities;
		
		[Space]
		public PolygonSkeleton mainSkeleton;
		public PolygonSkeleton retargetSkeleton;

		private SkeletonBoneReferences mainBones;
		private SkeletonBoneReferences retargetBones;

		private Dictionary<BoneType, Bone> mainBonesCollection;
		private Dictionary<BoneType, Bone> retargetBonesCollection;

		[Space, Range(0, 1)] public float quality;
		public int iterations;

		[Space]
		[Space]
		[Space]
		[Space]
		[Space]
		public List<RetargetContraint> orderedRetargetConstraints;
		private List<RetargetContraint> retargetConstraints;

		private void Start()
		{
			mainBones = mainSkeleton.boneReferences;
			retargetBones = retargetSkeleton.boneReferences;

			mainBonesCollection = mainBones.GatherBones();
			retargetBonesCollection = retargetBones.GatherBones();

			retargetConstraints = new List<RetargetContraint>();

			GenerateRetargetConstraints();
			foreach (RetargetContraint contraint in retargetConstraints)
			{
				contraint.FindChains(retargetBones.root.bone, retargetConstraints.ToArray());
			}

			orderedRetargetConstraints = retargetConstraints.OrderBy(value => -value.priority).ToList();
		}

		private void Update()
		{

			PositionBones();
			SolveBoneLengths();
			ApplyRetargeting();


			//if (Input.GetKeyDown(KeyCode.V))
			//{
			//	PositionBones();
			//}

			//if (Input.GetKeyDown(KeyCode.B))
			//{
			//	SolveBoneLengths();
			//}

			//if (Input.GetKeyDown(KeyCode.N))
			//{
			//	ApplyRetargeting();
			//}
		}

		private void PositionBones()
		{
			foreach (RetargetContraint constraint in orderedRetargetConstraints)
			{
				constraint.PositionRetargetBone();
			}
		}

		private void SolveBoneLengths()
		{
			foreach (RetargetContraint constraint in orderedRetargetConstraints)
			{
				for (int i = 0; i < iterations; i++)
				{
					constraint.SolvePosition(quality);
				}
			}
		}

		private void ApplyRetargeting()
		{
			foreach (RetargetContraint constraint in retargetConstraints)
			{
				constraint.ApplyRetargetingPosition();

				Bone bone = null;
				foreach (var bones in retargetBonesCollection)
				{
					if (bones.Value.bone == constraint.retargetBone)
					{
						bone = bones.Value;
					}
				}

				bool set = false;

				if (bone != null)
				{
					foreach (var otherConstraints in retargetConstraints)
					{
						if (otherConstraints.retargetBone == bone.GetLookAtBone(retargetBones)?.bone)
						{
							set = true;
							constraint.ApplyRetargetingRotation(otherConstraints.newPosition - constraint.newPosition);
						}
					}
				}

				if (!set)
				{
					constraint.ApplyRetargetingRotation();
				}
			}
		}

		private void OnDrawGizmos()
		{
			if (retargetConstraints == null) return;

			foreach (var retargetConstraint in retargetConstraints)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawWireSphere(retargetConstraint.newPosition, .01f);
			}
		}

		private void GenerateRetargetConstraints()
		{
			foreach (KeyValuePair<BoneType, Bone> bone in retargetBonesCollection)
			{
				foreach (KeyValuePair<BoneType, Bone> mainBone in mainBonesCollection)
				{
					if (bone.Key == mainBone.Key)
					{
						retargetConstraints.Add(new RetargetContraint(priorities.BoneTypeToPriority(bone.Key), bone.Value.bone, mainBone.Value.bone));
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