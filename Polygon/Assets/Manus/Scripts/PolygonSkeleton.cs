using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.Polygon.Skeleton.Utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Manus.Polygon.Skeleton
{
	public class PolygonSkeleton : MonoBehaviour
	{
		public bool useIK;
		public Animator animator;

		public SkeletonBoneReferences boneReferences;
		public SkeletonBoneScalers boneScalers;

		// TMP
		public PolygonIK_TMP ik;

		#region Monobehaviour Callbacks

		private void Awake()
		{
			if (boneReferences.IsValid)
			{
				boneScalers = new SkeletonBoneScalers();
				// boneScalers.GenerateScalerBonesForBody(boneReferences);
			}
		}

		private void Update()
		{
			if (useIK && !ik.isInitialized)
				ik.InitializeIK(transform, animator, boneReferences, FindObjectOfType<IKTargets_TMP>());
			if (useIK && !ik.ikGenerated)
				ik.CreateCharacterIK();
			if (ik.ikGenerated && ik.isInitialized)
				ik.SetIKWeigth(useIK ? 1 : 0);
		}

		#endregion

		#region Private Methods

		private Animator FindValidAnimatorInHierarchy()
		{
			Animator[] animatorsInHierarchy = GetComponentsInChildren<Animator>();
			foreach (Animator ac in animatorsInHierarchy)
			{
				if (ac.avatar != null && ac.avatar.isValid && ac.avatar.isHuman)
				{
					Debug.Log("Valid animator found");
					return ac;
				}
			}

			Debug.LogWarning(animatorsInHierarchy.Length == 0 ? "No animators found in hierarchy" : "No animator found with a valid human avatar, go to the settings and set the 'Animation Type' to 'Humanoid'");

			return null;
		}

		#endregion

		public bool IsAnimatorValid()
		{
			if (animator == null || animator.avatar == null || !animator.avatar.isValid || !animator.avatar.isHuman)
			{
				Debug.LogWarning((animator == null ? "No animator assigned" : "Assigned animator does not have a valid human avatar") + ", trying to find one");

				animator = FindValidAnimatorInHierarchy() ?? animator;

				if (animator == null) return false;
			}

			return true;
		}

		public void PopulateBoneReferences()
		{
			if (IsAnimatorValid())
			{
				boneReferences.Populate(animator);
			}

			//PolygonSkeleton newVersion = Instantiate(this, transform.position, transform.rotation, transform.parent);
			//newVersion.transform.SetSiblingIndex(transform.GetSiblingIndex());

			//if (boneReferences.IsValid)
			//{
			//	if (newSkeletonParent != null)
			//		DestroyImmediate(newSkeletonParent.gameObject);

			//	newSkeletonParent = new GameObject("newSkeleton").transform;
			//	newSkeletonParent.SetParent(transform);

			//	newSkeleton = CopySkeleton(boneReferences, newSkeletonParent);
			//}
		}

		public void ClearBoneReferences()
		{
			boneReferences.Clear();
		}

		public void CalculateBoneOrientations()
		{
			// Left Heel
			{
				Matrix4x4 footMatrix = Matrix4x4.TRS(
					boneReferences.legLeft.foot.bone.position,
					boneReferences.legLeft.foot.bone.rotation,
					boneReferences.legLeft.foot.bone.lossyScale).inverse;

				Vector3 heelPosition = boneReferences.legLeft.foot.bone.position;
				heelPosition.y = 0;
				boneReferences.legLeft.heel.position = footMatrix.MultiplyPoint3x4(heelPosition);
			}

			// Right Heel
			{
				Matrix4x4 footMatrix = Matrix4x4.TRS(
					boneReferences.legRight.foot.bone.position,
					boneReferences.legRight.foot.bone.rotation,
					boneReferences.legRight.foot.bone.lossyScale).inverse;

				Vector3 heelPosition = boneReferences.legRight.foot.bone.position;
				heelPosition.y = 0;
				boneReferences.legRight.heel.position = footMatrix.MultiplyPoint3x4(heelPosition);
			}

			// Model Height
			{
				Vector3 rootPos = boneReferences.body.hip.bone.position;
				rootPos.y = 0;

				Matrix4x4 rootMatrix = Matrix4x4.TRS(rootPos, boneReferences.root.bone.rotation, Vector3.one).inverse;
				boneReferences.modelHeight.position = rootMatrix.MultiplyPoint3x4(new Vector3(rootPos.x, 1.8f, rootPos.z));
			}

			foreach (var bone in boneReferences.GatherBones(GatherType.All).Values)
			{
				bone.CalculateOrientation(boneReferences);
			}
		}

		public void SetBindPose()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (PrefabUtility.GetPrefabAssetType(this.gameObject) != PrefabAssetType.NotAPrefab)
				{
					Debug.LogError("The object you are trying to edit is a prefab");
					return;
				}
			}
#endif

			SkinnedMeshRenderer[] skinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();
			boneReferences.UpdateBoneOrientations(skinnedMeshes);
		}
	}
}

