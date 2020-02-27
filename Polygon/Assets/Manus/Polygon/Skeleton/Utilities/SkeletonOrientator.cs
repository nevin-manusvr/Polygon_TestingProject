using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Manus.ToBeHermes.Skeleton;
using System;

namespace Manus.Polygon.Skeleton.Utilities
{

	public static class SkeletonOrientator
	{
		public static void UpdateBoneOrientations(this SkeletonBoneReferences bones, SkinnedMeshRenderer[] renderers)
		{
			var allBones = bones.GatherBones(GatherType.All).Values.ToArray();

			var rendData = new Tuple<SkinnedMeshRenderer, Mesh>[renderers.Length];

			// Save renderer data
			for (var i = 0; i < renderers.Length; i++)
			{
				SkinnedMeshRenderer rend = renderers[i];

				Mesh mesh = new Mesh();
				rend.BakeMesh(mesh);
				mesh.boneWeights = rend.sharedMesh.boneWeights;

				rendData[i] = new Tuple<SkinnedMeshRenderer, Mesh>(rend, mesh);
			}

			// Rotate bones
			foreach (Bone bone in allBones)
			{
				bone.bone.ReorientTransform(bone.desiredRotation);
			}

			// Assign renderer data
			foreach (Tuple<SkinnedMeshRenderer, Mesh> data in rendData)
			{
				SkinnedMeshRenderer rend = data.Item1;
				Mesh mesh = data.Item2;
				Transform[] joints = rend.bones;
				var bindposes = new Matrix4x4[rend.bones.Length];

				for (int i = 0; i < joints.Length; i++)
				{
					bindposes[i] = joints[i].worldToLocalMatrix * rend.transform.localToWorldMatrix;
				}

				mesh.bindposes = bindposes;
				rend.rootBone = bones.root.bone;
				rend.sharedMesh = mesh;
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
			
			// Un-parent all children
			foreach (Transform child in children)
			{
				child.SetParent(null);
			}

			// Change rotation
			transform.rotation = rotation;

			// Re-parent all children
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


