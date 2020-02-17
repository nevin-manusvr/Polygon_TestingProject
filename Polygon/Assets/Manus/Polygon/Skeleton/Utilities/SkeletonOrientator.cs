using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Manus.ToBeHermes.Skeleton;

namespace Manus.Polygon.Skeleton.Utilities
{
	public static class SkeletonOrientator
	{
		public static void SetToBindPose(SkinnedMeshRenderer[] meshes, Transform root)
		{
			Matrix4x4 rootMatrix = Matrix4x4.TRS(meshes[0].transform.position, meshes[0].transform.rotation, meshes[0].transform.lossyScale);
			Mesh mesh = MonoBehaviour.Instantiate(meshes[0].sharedMesh);
			meshes[0].BakeMesh(mesh);

			if (mesh == null) return;
			foreach (Vector3 vertex in mesh.vertices)
			{
				Debug.DrawRay(rootMatrix.MultiplyPoint3x4(vertex), Vector3.up * 0.01f, Color.red, 10f);
			}
			//for (var i = 0; i < meshes.Length; i++)
			//{
			//	SkinnedMeshRenderer mesh = meshes[i];

			//	for (var j = 0; j < mesh.bones.Length; j++)
			//	{
			//		Transform bone = mesh.bones[j];
			//		Matrix4x4 bindPose = mesh.sharedMesh.bindposes[j];

			//		// Recreate the local transform matrix of the bone
			//		Matrix4x4 localMatrix = bindPose;//.inverse;
			//		// Recreate local transform from that matrix
			//		bone.localPosition = localMatrix.GetPosition();
			//		bone.localRotation = localMatrix.rotation;
			//		bone.localScale = localMatrix.lossyScale;
			//	}
			//}
		}

		public static void UpdateBoneOrientations(this SkeletonBoneReferences bones, SkinnedMeshRenderer[] meshes)
		{
			var allBones = bones.GatherBones().Values.ToArray();

			// bones.root.bone.ReorientTransform(bones.root.desiredRotation);

			bones.root.bone.ReorientTransform(bones.root.desiredRotation);

			foreach (SkinnedMeshRenderer rend in meshes)
			{
				Mesh t_Mesh = new Mesh();
				rend.BakeMesh(t_Mesh);
				var t_Bones = rend.bones;
				t_Mesh.boneWeights = rend.sharedMesh.boneWeights;
				
				var t_BMats = new Matrix4x4[rend.bones.Length];

				for (int i = 0; i < t_Bones.Length; i++)
				{
					foreach (Bone bone in allBones)
					{
						if (bone.bone == t_Bones[i])
						{
							t_Bones[i].ReorientTransform(bone.desiredRotation);
						}
					}
				}

				for (int i = 0; i < t_Bones.Length; i++)
				{
					t_BMats[i] = t_Bones[i].worldToLocalMatrix * rend.transform.localToWorldMatrix;
				}

				t_Mesh.bindposes = t_BMats;
				rend.rootBone = bones.root.bone;
				rend.sharedMesh = t_Mesh;

				//Transform rootTransform = rend.transform;
				//Matrix4x4 vertexRoot = Matrix4x4.TRS(rend.transform.position, rend.transform.rotation, rend.transform.lossyScale);

				//Mesh bakedMesh = MonoBehaviour.Instantiate(rend.sharedMesh);
				//rend.BakeMesh(bakedMesh);

				//Matrix4x4[] bindposes = rend.sharedMesh.bindposes;

				//for (int i = 0; i < rend.bones.Length; i++)
				//{
				//	foreach (Bone bone in allBones)
				//	{
				//		if (rend.bones[i] == bone.bone)
				//		{
				//			if (!bone.desiredRotation.IsValid() || bone.type == BoneType.Root)
				//				continue;

				//			// bone.bone.ReorientTransform(bone.desiredRotation);
				//			Debug.DrawRay(bone.bone.position, Vector3.up * .11f, Color.magenta, 10f);

				//			Matrix4x4 boneMatrix = Matrix4x4.TRS(bone.bone.position, bone.bone.rotation, bone.bone.lossyScale);
				//			Matrix4x4 rootMatrix = Matrix4x4.TRS(rootTransform.position, rootTransform.rotation, rootTransform.lossyScale);
				//			bindposes[i] = bone.bone.worldToLocalMatrix * rend.transform.localToWorldMatrix;
				//		}
				//	}
				//}

				//// Debug vertices
				//var vertices = new List<Vector3>();

				//foreach (Vector3 vertex in bakedMesh.vertices)
				//{
				//	vertices.Add(vertexRoot.MultiplyPoint3x4(vertex));
				//	Debug.DrawRay(vertexRoot.MultiplyPoint3x4(vertex), Vector3.up * 0.01f, Color.red, 10f);
				//}

				//Mesh mesh = new Mesh();
				//mesh.vertices = vertices.ToArray();
				//mesh.triangles = rend.sharedMesh.triangles;

				//mesh.uv = rend.sharedMesh.uv;
				//mesh.normals = rend.sharedMesh.normals;

				//mesh.boneWeights = rend.sharedMesh.boneWeights;
				//mesh.bindposes = bindposes;

				//rend.bones = rend.bones;
				//rend.sharedMesh = mesh;
			}

			//foreach (var bone in allBones)
			//{
			//	if (!bone.desiredRotation.IsValid() || bone.type == BoneType.Root)
			//		continue;

			//	bone.bone.ReorientTransform(bone.desiredRotation);
			//}
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


