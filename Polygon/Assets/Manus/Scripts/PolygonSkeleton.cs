using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Manus.Polygon.Skeleton.Utilities;
using Manus.Core;
using Manus.Core.Hermes;
using HProt = Hermes.Protocol;

using Manus.Core.Utility;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Manus.Polygon.Skeleton
{
	

	public class PolygonSkeleton : MonoBehaviour
	{
		public bool useIK;
		public bool updateFromHermes;

		public int deviceID;

		public Animator animator;

		public SkeletonBoneReferences boneReferences;
		public SkeletonBoneScalers boneScalers;

		// TODO: remove IK from plugin
		public PolygonIK_TMP ik;

		public Action CalibrateBody;

		#region Properties

		public bool IsAnimatorValid
		{
			get
			{
				if (animator == null || animator.avatar == null || !animator.avatar.isValid || !animator.avatar.isHuman)
				{
					Debug.LogWarning((animator == null ? "No animator assigned" : "Assigned animator does not have a valid human avatar") + ", trying to find one");

					animator = FindValidAnimatorInHierarchy() ?? animator;

					if (animator == null) return false;
				}

				return true;
			}
		}

		#endregion

		#region Monobehaviour Callbacks

		private void Awake()
		{
			if (boneReferences.IsValid)
			{
				boneScalers = new SkeletonBoneScalers();
				boneScalers.GenerateScalerBonesForBody(boneReferences);
			}

			ManusManager.instance.communicationHub.polygonUpdate += TMP_UpdateFromHermes;
		}

		private void Update()
		{
			// TODO: remove IK from plugin
			if (useIK && !ik.isInitialized)
				ik.InitializeIK(transform, animator, boneReferences, FindObjectOfType<IKTargets_TMP>());
			if (useIK && !ik.ikGenerated)
				ik.CreateCharacterIK();
			if (ik.ikGenerated && ik.isInitialized)
				ik.SetIKWeigth(useIK ? 1 : 0);

			if (Input.GetKeyDown(KeyCode.P))
			{
				TMP_SendSkeletonDefinitions();
			}
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

		#region Skeleton Setup

		public void PopulateBoneReferences()
		{
			if (IsAnimatorValid)
			{
				boneReferences.Populate(animator);
			}
		}

		public void ClearBoneReferences()
		{
			boneReferences.Clear();
		}

		public void CalculateBoneOrientations()
		{
			foreach (var bone in boneReferences.GatherBones(GatherType.All).Values)
			{
				bone.CalculateOrientation(boneReferences);
			}

			Tuple<Transform, Transform>[] bones =
				{
					Tuple.Create(boneReferences.armLeft.hand.wrist.bone, boneReferences.armRight.hand.wrist.bone),
					Tuple.Create(boneReferences.legLeft.foot.bone, boneReferences.legRight.foot.bone)
				};

			// Left Heel
			{
				Vector3 heelPosition = boneReferences.legLeft.foot.bone.position;
				heelPosition.y = 0;
				boneReferences.legLeft.heel.UpdateTransformation(heelPosition, Quaternion.identity);
			}

			// Right Heel
			{
				Vector3 heelPosition = boneReferences.legRight.foot.bone.position;
				heelPosition.y = 0;
				boneReferences.legLeft.heel.UpdateTransformation(heelPosition, Quaternion.identity);
			}

			// Model Height
			{
				Vector3 rootPos = boneReferences.head.head.bone.position;
				rootPos.y = 0;

				Matrix4x4 rootMatrix = Matrix4x4.TRS(rootPos, boneReferences.root.bone.rotation, Vector3.one).inverse;
				boneReferences.head.modelHeight.position = rootMatrix.MultiplyPoint3x4(new Vector3(rootPos.x, 1.8f, rootPos.z));
			}

			// Hip Control
			{
				boneReferences.body.hipControl = new ControlBone(HProt.Polygon.ControlBoneType.HipControl, new[] { boneReferences.body.hip, boneReferences.body.spine, boneReferences.legLeft.upperLeg, boneReferences.legRight.upperLeg });

				Matrix4x4 hipMatrix = Matrix4x4.TRS(
					boneReferences.body.hip.bone.position,
					boneReferences.body.hip.desiredRotation,
					boneReferences.body.hip.bone.lossyScale).inverse;

				Vector3 hipCenterPos = (boneReferences.legLeft.upperLeg.bone.position + boneReferences.legRight.upperLeg.bone.position) / 2f;
				boneReferences.body.hipControl.position = hipMatrix.MultiplyPoint3x4(hipCenterPos);

				Vector3 aimDirection = hipMatrix.MultiplyVector(SkeletonOrientationCalculator.CalculateForward(bones));
				Vector3 upDirection = hipMatrix.MultiplyVector(Vector3.up);
				boneReferences.body.hipControl.rotation = Quaternion.LookRotation(aimDirection, upDirection);
			}

			// UpperBody Control
			{
				Bone highestSpine = boneReferences.body.spine;
				if (boneReferences.body.chest.bone != null) highestSpine = boneReferences.body.chest;
				if (boneReferences.body.upperChest.bone != null) highestSpine = boneReferences.body.upperChest;
				boneReferences.body.upperBodyControl = new ControlBone(HProt.Polygon.ControlBoneType.UpperBodyControl, new[] { highestSpine, boneReferences.head.neck, boneReferences.armLeft.shoulder, boneReferences.armRight.shoulder });

				Matrix4x4 upperBodyMatrix = Matrix4x4.TRS(
					highestSpine.bone.position,
					highestSpine.desiredRotation,
					highestSpine.bone.lossyScale).inverse;

				Vector3 upperBodyCenterPos = ((boneReferences.armLeft.upperArm.bone.position + boneReferences.armRight.upperArm.bone.position) / 2f +
											 (boneReferences.armLeft.shoulder.bone.position + boneReferences.armRight.shoulder.bone.position) / 2f) / 2f;
				boneReferences.body.upperBodyControl.position = upperBodyMatrix.MultiplyPoint3x4(upperBodyCenterPos);

				Vector3 aimDirection = upperBodyMatrix.MultiplyVector(-SkeletonOrientationCalculator.CalculateForward(bones));
				Vector3 upDirection = upperBodyMatrix.MultiplyVector(Vector3.up);
				boneReferences.body.upperBodyControl.rotation = Quaternion.LookRotation(aimDirection, upDirection);
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

			string t_ID = "PolygonMesh_" + gameObject.GetInstanceID().ToString();
			string t_Path = "Assets/" + t_ID + ".asset";
			PolygonMeshes t_Asset = new PolygonMeshes();
			t_Asset.name = t_ID;
			AssetDatabase.CreateAsset(t_Asset, t_Path);
			AssetDatabase.SetMainObject(t_Asset, t_Path);
			SkinnedMeshRenderer[] skinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();
			boneReferences.UpdateBoneOrientations(skinnedMeshes);

			t_Asset.m_ID = t_ID;
			t_Asset.m_Meshes = new PolygonMeshes.MeshInfo[skinnedMeshes.Length];

			for (int i = 0; i < skinnedMeshes.Length; i++)
			{
				Mesh t_Mesh = skinnedMeshes[i].sharedMesh;
				t_Mesh.name = skinnedMeshes[i].name + "_Mesh";
				AssetDatabase.AddObjectToAsset(t_Mesh, t_Asset);
				skinnedMeshes[i].sharedMesh = t_Mesh;
				t_Asset.m_Meshes[i].m_Mesh = t_Mesh;
				t_Asset.m_Meshes[i].m_Hierarchy = skinnedMeshes[i].transform.GetPath();
#if UNITY_EDITOR
				EditorUtility.SetDirty(skinnedMeshes[i]);
#endif
			}

			AssetDatabase.SaveAssets();
		}

		public void SetToBindPose()
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

			SkeletonOrientator.SampleBindPose(gameObject);
		}

		#endregion

		#region ProfileData

		public void ApplyProfileData()
		{
			CalibrateBody?.Invoke();
		}

		#endregion

		private void TMP_UpdateFromHermes(HProt.Polygon.Data _Poly)
		{
			if (!updateFromHermes) 
				return;

			UnityMainThreadDispatcher.Instance().Enqueue(
				() =>
					{
						foreach (HProt.Polygon.Skeleton polySkeleton in _Poly.Skeletons)
						{
							if (polySkeleton.DeviceID != (uint)deviceID)
								continue;

							var localBones = boneReferences.GatherBones(GatherType.Retargeted);

							foreach (var bone in polySkeleton.Bones)
							{
								if (localBones.ContainsKey(bone.Type))
								{
									localBones[bone.Type].bone.position = bone.Position.ToVector3();
									localBones[bone.Type].bone.rotation = bone.Rotation.ToUnityQuat();
								}
							}
						}
					});
		}

		public void TMP_ToggleIK(bool tf)
		{
			useIK = tf;
		}

		public void TMP_SendSkeletonDefinitions()
		{
			HProt.Polygon.SkeletalDefinition t_Skeletors = new HProt.Polygon.SkeletalDefinition();

			HProt.Polygon.Skeleton t_Skele = new HProt.Polygon.Skeleton();
			t_Skele.DeviceID = (uint)deviceID;

			foreach (var bone in boneReferences.GatherBones(GatherType.All))
			{
				t_Skele.Bones.Add(bone.Value);
			}

			t_Skeletors.Skeletons.Add(t_Skele);

			if (ManusManager.instance.communicationHub.careTaker == null || !ManusManager.instance.communicationHub.careTaker.connected)
				return;

			ManusManager.instance.communicationHub.careTaker.Hermes.PolygonSkeletalDefinitionSetAsync(t_Skeletors);
		}
	}
}

