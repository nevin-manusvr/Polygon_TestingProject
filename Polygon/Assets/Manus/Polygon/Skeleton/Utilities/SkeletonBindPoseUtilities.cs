using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Manus.Polygon.Skeleton.Utilities
{
	using Hermes.Protocol.Polygon;

	using Bone = Manus.Polygon.Skeleton.Bone;

	public static class SkeletonBindPoseUtilities
	{
		public static void SampleBindPose(GameObject go)
		{
			List<SkinnedMeshRenderer> skins = new List<SkinnedMeshRenderer>(go.GetComponentsInChildren<SkinnedMeshRenderer>());
			skins.Sort(new SkinTransformHierarchySorter());
			foreach (SkinnedMeshRenderer skin in skins)
			{
				//Debug.Log ("Sampling skinning of SkinnedMeshRenderer "+skin.name);
				Matrix4x4 goMatrix = skin.transform.localToWorldMatrix;
				List<Transform> bones = new List<Transform>(skin.bones);
				Vector3[] backupLocalPosition = new Vector3[bones.Count];

				// backup local position of bones. Only use rotation given by bind pose
				for (int i = 0; i < bones.Count; i++)
				{
					backupLocalPosition[i] = bones[i].localPosition;
				}

				// Set all parents to be null to be able to set global alignments of bones without affecting their children.
				Dictionary<Transform, Transform> parents = new Dictionary<Transform, Transform>();
				foreach (Transform bone in bones)
				{
					parents[bone] = bone.parent;
					bone.parent = null;
				}

				// Set global space position and rotation of each bone
				for (int i = 0; i < bones.Count; i++)
				{
					Vector3 position;
					Quaternion rotation;
					GetBindPoseBonePositionRotation(goMatrix, skin.sharedMesh.bindposes[i], bones[i], out position, out rotation);
					bones[i].position = position;
					bones[i].rotation = rotation;
				}

				// Reconnect bones in their original hierarchy
				foreach (Transform bone in bones)
					bone.parent = parents[bone];

				// put back local postion of bones
				for (int i = 0; i < bones.Count; i++)
				{
					bones[i].localPosition = backupLocalPosition[i];
				}
			}
		}

		public static void GetBindPoseBonePositionRotation(Matrix4x4 skinMatrix, Matrix4x4 boneMatrix, Transform bone, out Vector3 position, out Quaternion rotation)
		{
			// Get global matrix for bone
			Matrix4x4 bindMatrixGlobal = skinMatrix * boneMatrix.inverse;

			// Get local X, Y, Z, and position of matrix
			Vector3 mX = new Vector3(bindMatrixGlobal.m00, bindMatrixGlobal.m10, bindMatrixGlobal.m20);
			Vector3 mY = new Vector3(bindMatrixGlobal.m01, bindMatrixGlobal.m11, bindMatrixGlobal.m21);
			Vector3 mZ = new Vector3(bindMatrixGlobal.m02, bindMatrixGlobal.m12, bindMatrixGlobal.m22);
			Vector3 mP = new Vector3(bindMatrixGlobal.m03, bindMatrixGlobal.m13, bindMatrixGlobal.m23);

			// Set position
			// Adjust scale of matrix to compensate for difference in binding scale and model scale
			float bindScale = mZ.magnitude;
			float modelScale = Mathf.Abs(bone.lossyScale.z);
			position = mP * (modelScale / bindScale);

			// Set rotation
			// Check if scaling is negative and handle accordingly
			if (Vector3.Dot(Vector3.Cross(mX, mY), mZ) >= 0)
				rotation = Quaternion.LookRotation(mZ, mY);
			else
				rotation = Quaternion.LookRotation(-mZ, -mY);
		}

		private class SkinTransformHierarchySorter : IComparer<SkinnedMeshRenderer>
		{
			public int Compare(SkinnedMeshRenderer skinA, SkinnedMeshRenderer skinB)
			{
				Transform a = skinA.transform;
				Transform b = skinB.transform;
				while (true)
				{
					if (a == null)
						if (b == null)
							return 0;
						else
							return -1;
					if (b == null)
						return 1;
					a = a.parent;
					b = b.parent;
				}
			}
		}

		public static void SetBindPose(this SkeletonBoneReferences bones, SkinnedMeshRenderer[] renderers)
		{
			var allBones = bones.GatherBones(GatherType.All).Values.ToArray();

			foreach (var renderer in renderers)
			{
				renderer.transform.localScale = Vector3.one;
			}

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
				if (bone.type == BoneType.Root)
				{
					Vector3 rootPosition = bone.bone.position;
					rootPosition.y = 0;

					bone.bone.MoveTransform(rootPosition);
				}
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

		public static void MoveTransform(this Transform transform, Vector3 position)
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
			transform.position = position;

			// Re-parent all children
			foreach (Transform child in children)
			{
				child.SetParent(transform);
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
	}
}


