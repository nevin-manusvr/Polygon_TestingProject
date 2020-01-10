using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManusVR.Polygon
{
	public class PolygonSkeleton : MonoBehaviour
	{
		public Animator animator;

		public SkeletonBoneReferences boneReferences;

		#region Monobehaviour Callbacks

		private void Start()
		{
			GameObject scaleBone = new GameObject("shoulderScaleBone");
			scaleBone.transform.position = boneReferences.armLeft.shoulder.bone.position;
			scaleBone.transform.LookAt(boneReferences.armLeft.upperArm.bone);

			scaleBone.transform.SetParent(boneReferences.armLeft.shoulder.bone);
			boneReferences.armLeft.upperArm.bone.SetParent(scaleBone.transform);

			Debug.Log("New scalebone", scaleBone);
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

				if (animator != null) return false;
			}

			return true;
		}

		[ContextMenu("Auto Populate Bones")]
		public void PopulateBoneReferences()
		{
			if (IsAnimatorValid())
			{
				boneReferences.Populate(animator);
			}
		}

		[ContextMenu("Clear Bones")]
		public void ClearBoneReferences()
		{
			boneReferences.Clear();
		}
	}
}

