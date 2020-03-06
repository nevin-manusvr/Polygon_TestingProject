﻿using UnityEngine;
using HProt = Hermes.Protocol;

namespace Manus.ToBeHermes
{
	using System.Collections.Generic;
	using GlmSharp;
	using Google.Protobuf.Collections;
	using Hermes.Protocol.Polygon;
	using Hermes.Tools;

	using Manus.Core;
	using Manus.Core.Hermes;
	using Manus.Core.Utility;
	using Manus.Polygon;
	using Manus.ToBeHermes.IK;
	using Manus.ToBeHermes.Tracking;

	public class HermesSimulator : MonoBehaviour
	{
		public static HermesSimulator instance;

		public Graveyard graveyard;

		public SkeletonEstimator IK;

		public IKTargets_TMP targets;

		private void Awake()
		{
			if (instance == null)
				instance = this;

			graveyard = new Graveyard();

			IK = new SkeletonEstimator(0);
		}

		private void Update()
		{
			IK.EstimateBody(
				new Tracker(VRTrackerType.Head, targets.head.position.ToGlmVec3(), targets.head.rotation.ToGlmQuat()),
				new Tracker(VRTrackerType.LeftHand, targets.leftHand.position.ToGlmVec3(), targets.leftHand.rotation.ToGlmQuat()),
				new Tracker(VRTrackerType.RightHand, targets.rightHand.position.ToGlmVec3(), targets.rightHand.rotation.ToGlmQuat()),
				new Tracker(VRTrackerType.Waist, targets.hip.position.ToGlmVec3(), targets.hip.rotation.ToGlmQuat()),
				new Tracker(VRTrackerType.LeftFoot, targets.leftFoot.position.ToGlmVec3(), targets.leftFoot.rotation.ToGlmQuat()),
				new Tracker(VRTrackerType.RightFoot, targets.rightFoot.position.ToGlmVec3(), targets.rightFoot.rotation.ToGlmQuat())
			);

			UpdatePolygonSkeletons();
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;
			if (IK != null && IK.Skeleton != null)
			{
				foreach (var bone in IK.Skeleton.Bones)
				{
					Gizmos.DrawWireSphere(bone.Position.ToVector3(), .05f);
				}
			}
		}

		public void OnNewSkeletonDefinition(HProt.Polygon.InternalData _Poly)
		{
			foreach (var skeleton in _Poly.Skeleton.Skeletons)
			{
				graveyard.AddGrave(skeleton);
			}
			
			//for (var i = 0; i < _Poly.Skeleton.Skeletons.Count; i++)
			//{
			//	if (i != 0) return;

			//	// Debug.Log($"Skeleton is valid : {IsDefinitionValid(_Poly.Skeleton.Skeletons[i].Bones)}");

			//	var bones = new Dictionary<BoneType, Bone>();
			//	var skeleton = _Poly.Skeleton.Skeletons[i];
				
			//	foreach (Bone bone in skeleton.Bones)
			//	{
			//		bones.Add(bone.Type, bone);
			//	}

			//	float armLength = glm.Distance(bones[BoneType.LeftHand].Position.toGlmVec3(), bones[BoneType.LeftLowerArm].Position.toGlmVec3()) +
			//	                  glm.Distance(bones[BoneType.LeftLowerArm].Position.toGlmVec3(), bones[BoneType.LeftUpperArm].Position.toGlmVec3());
			//	Debug.Log($"left arm length = {armLength}");
			//}
		}

		public void UpdatePolygonSkeletons()
		{
			if (ManusManager.instance.communicationHub.careTaker == null) 
				return;

			var t_Data = new HProt.Polygon.Data();

			foreach (var grave in graveyard.graves)
			{
				var t_Skeleton = new HProt.Polygon.Skeleton { DeviceID = (uint)grave.ID };

				foreach (var bone in grave.Skeleton.Bones)
				{
					t_Skeleton.Bones.Add(bone);
				}

				t_Data.Skeletons.Add(t_Skeleton);
			}

			ManusManager.instance.communicationHub.careTaker.Hermes?.UpdatePolygonAsync(t_Data);
		}
	}
}