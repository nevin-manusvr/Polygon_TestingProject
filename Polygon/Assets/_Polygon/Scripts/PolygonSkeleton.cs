using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManusVR.Polygon
{
	public class PolygonSkeleton : MonoBehaviour
	{
		public Animator animator;

		public SkeletonBoneReferences boneReferences;

		[ContextMenu("Auto Populate Bones")]
		public void Populate()
		{
			if (animator == null) Debug.LogError("make sure you assign a animator");
			boneReferences.Populate(animator);
		}
	}
}

