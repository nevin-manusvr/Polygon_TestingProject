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

		public RetargetChain[] chains;

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
			if (chains == null) 
				return;

			foreach (RetargetChain chain in chains)
			{
				float distance = (newPosition - chain.endConstraint.newPosition).sqrMagnitude;
				float difference = chain.maxDistanceSqr - distance;

				if (difference < 0 || chain.directConnection)
				{
					difference = chain.maxDistance - Mathf.Sqrt(distance);

					Vector3 direction = (chain.endConstraint.newPosition - newPosition).normalized;
					newPosition -= direction * difference * (chain.endConstraint.priority > priority ? 1 : 0.5f) * quality;
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

			List<RetargetChain> allChains = new List<RetargetChain>();
			TraverseHierachy(retargetBone, new List<Transform>(), root, allContraints, allChains);

			chains = allChains.ToArray();
		}

		//public void AddToChain(Transform transform, RetargetContraint[] allContraints)
		//{
		//	RetargetContraint endConstraint = null;
		//	foreach (RetargetContraint contraint in allContraints)
		//	{
		//		if (contraint.retargetBone == transform)
		//			endConstraint = contraint;
		//	}

		//	if (endConstraint == null)
		//		return;

		//	chains = chains.Concat(
		//		new[]   {
		//					new RetargetChain(endConstraint, new List<Transform> { retargetBone, transform }, allContraints)
		//				}).ToArray();
			
		//	Debug.Log($"Added {transform.name} to chain of {retargetBone.name}");
		//}

		private void TraverseHierachy(Transform currentNode, List<Transform> path, Transform limit, RetargetContraint[] allContraints, List<RetargetChain> allChains)
		{
			path = new List<Transform>(path);
			path.Add(currentNode);

			foreach (RetargetContraint contraint in allContraints)
			{
				if (contraint.retargetBone == currentNode && currentNode != retargetBone)
				{
					if (contraint.priority >= priority)
					{
						allChains.Add(new RetargetChain(contraint, path, allContraints));
						return;
					}
				}
			}

			// Continue Path
			if (!path.Contains(currentNode.parent) && currentNode.parent != limit)
				TraverseHierachy(currentNode.parent, path, limit, allContraints, allChains);

			for (int i = 0; i < currentNode.childCount; i++)
			{
				Transform child = currentNode.GetChild(i);
				if (!path.Contains(child) && child != limit)
					TraverseHierachy(child, path, limit, allContraints, allChains);
			}
		}
	}

	[System.Serializable]
	public class RetargetChain
	{
		public RetargetContraint endConstraint;
		public float maxDistance;
		public float maxDistanceSqr;
		public bool directConnection;

		//public Transform[] chain;

		public RetargetChain(RetargetContraint constraint, List<Transform> chain, RetargetContraint[] allContraints)
		{
			endConstraint = constraint;
			
			// Take shortcuts to the shortest route
			if (chain.Count > 2)
			{
				var bonesToRemove = new List<Transform>();
				bool lastWasParent = false;
				for (var i = 0; i < chain.Count; i++)
				{
					if (i == chain.Count - 1) continue;

					if (chain[i].IsChildOf(chain[i + 1]))
					{
						lastWasParent = true;
					}
					else
					{
						if (lastWasParent)
							bonesToRemove.Add(chain[i]);

						lastWasParent = false;
					}
				}

				foreach (Transform boneToRemove in bonesToRemove)
				{
					chain.RemoveAt(chain.IndexOf(boneToRemove));
				}
			}
			
			// Calculate max length
			float length = 0;
			for (int i = 0; i < chain.Count - 1; i++)
			{
				length += Vector3.Distance(chain[i].position, chain[i + 1].position);
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
			maxDistanceSqr = maxDistance * maxDistance;
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
		private List<RetargetContraint> retargetConstraints;
		private Dictionary<int, List<RetargetContraint>> orderedRetargetConstraints;

		private void Start()
		{
			mainBones = mainSkeleton.boneReferences;
			retargetBones = retargetSkeleton.boneReferences;

			mainBonesCollection = mainBones.GatherBones();
			retargetBonesCollection = retargetBones.GatherBones();

			retargetConstraints = new List<RetargetContraint>();
			orderedRetargetConstraints = new Dictionary<int, List<RetargetContraint>>();

			GenerateRetargetConstraints();
			foreach (RetargetContraint contraint in retargetConstraints)
			{
				contraint.FindChains(retargetBones.root.bone, retargetConstraints.ToArray());
			}
		}

		private void Update()
		{
			PositionBones();
			SolveBoneLengths();
			ApplyRetargeting();
		}

		private void PositionBones()
		{
			foreach (int priority in orderedRetargetConstraints.Keys)
			{
				foreach (RetargetContraint contraint in orderedRetargetConstraints[priority])
				{
					contraint.PositionRetargetBone();
				}
			}
		}

		private void SolveBoneLengths()
		{
			float ClampedQuality = Mathf.Lerp(1, 0.001f, quality);

			foreach (int priority in orderedRetargetConstraints.Keys)
			{
				for (int i = 0; i < iterations; i++)
				{
					foreach (RetargetContraint contraint in orderedRetargetConstraints[priority])
					{
						contraint.SolvePosition(ClampedQuality);
					}
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
						int priority = priorities.BoneTypeToPriority(bone.Key);
						RetargetContraint constraint = new RetargetContraint(priority, bone.Value.bone, mainBone.Value.bone);

						retargetConstraints.Add(constraint);
						
						if (!orderedRetargetConstraints.ContainsKey(priority))
							orderedRetargetConstraints.Add(priority, new List<RetargetContraint>());

						orderedRetargetConstraints[priority].Add(constraint);
					}
				}
			}

			orderedRetargetConstraints = orderedRetargetConstraints.OrderByDescending(value => value.Key).ToDictionary(value => value.Key, value => value.Value);
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