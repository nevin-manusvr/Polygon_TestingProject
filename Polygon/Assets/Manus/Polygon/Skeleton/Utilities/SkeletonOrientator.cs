using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Manus.Polygon.Skeleton.Utilities
{
	public static class SkeletonOrientator
	{
		public static void UpdateBoneOrientations(this SkeletonBoneReferences bones, SkinnedMeshRenderer meshes)
		{
			var allBones = bones.GatherBones().Values.ToArray();

			Transform[] joints = meshes.bones;
			Matrix4x4[] bindposes = meshes.sharedMesh.bindposes;

			for (int i = 0; i < allBones.Length; i++)
			{
				if (!allBones[i].desiredRotation.IsValid())
					continue;

				allBones[i].bone.ReorientTransform(allBones[i].desiredRotation);
			}
		}

		public static void ReorientTransform(this Transform transform, Quaternion rotation)
		{
			// Find all children
			var children = new List<Transform>();
			for (int i = 0; i < transform.childCount; i++)
			{
				children.Add(transform.GetChild(i));
			}
			
			// Unparent all children
			foreach (Transform child in children)
			{
				child.SetParent(null);
			}

			// Change rotation
			transform.rotation = rotation;

			// Reparent all children
			foreach (Transform child in children)
			{
				child.SetParent(transform);
			}
		}


		public static bool IsValid(this Quaternion rotation)
		{
			return !(Mathf.Approximately(rotation.x, 0)
			         && Mathf.Approximately(rotation.y, 0)
			         && Mathf.Approximately(rotation.z, 0)
			         && Mathf.Approximately(rotation.w, 0));
		}
	}
}


