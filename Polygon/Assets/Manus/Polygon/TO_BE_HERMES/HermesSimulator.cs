using UnityEngine;
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
		public Ghost IK;

		public IKTargets_TMP targets;

		private void Awake()
		{
			if (instance == null)
				instance = this;

			graveyard = new Graveyard();

			IK = new Ghost(0);
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

			foreach (Grave grave in graveyard.graves)
			{
				grave.possession.Move();
			}

			UpdatePolygonSkeletons();
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;
			if (IK != null && IK.Skeleton != null)
			{
				foreach (var bone in IK.Skeleton.Bones)
				{
					Vector3 t_Pos = bone.Position.ToVector3();
					Quaternion t_Rot = bone.Rotation.ToUnityQuat();

					Gizmos.color = Color.cyan;
					Gizmos.DrawWireSphere(t_Pos, .02f);
					Gizmos.color = Color.blue;
					Gizmos.DrawRay(t_Pos, bone.Rotation.ToUnityQuat() * Vector3.forward * .05f);
					Gizmos.color = Color.green;
					Gizmos.DrawRay(t_Pos, bone.Rotation.ToUnityQuat() * Vector3.up * .05f);
					Gizmos.color = Color.red;
					Gizmos.DrawRay(t_Pos, bone.Rotation.ToUnityQuat() * Vector3.right * .05f);

				}
			}
		}

		public void OnNewSkeletonDefinition(HProt.Polygon.InternalData _Poly)
		{
			Debug.Log("Message from Unity");
			foreach (var t_Skeleton in _Poly.Skeleton.Skeletons)
			{
				graveyard.AddGrave(t_Skeleton);

				graveyard.GetGrave((int)t_Skeleton.DeviceID).possession.SetTargetSkeleton(IK.Skeleton);
			}
		}

		public void UpdatePolygonSkeletons()
		{
			if (ManusManager.instance.communicationHub.careTaker == null) 
				return;

			var t_Data = new Data();

			foreach (var grave in graveyard.graves)
			{
				foreach (var bone in grave.Skeleton.Bones)
				{
					foreach (var ikBone in IK.Skeleton.Bones)
					{
						if (bone.Type == ikBone.Type)
						{
							bone.Position = ikBone.Position;
							bone.Rotation = ikBone.Rotation;
						}
					}
				}

				t_Data.Skeletons.Add(grave.Skeleton);
			}

			ManusManager.instance.communicationHub.careTaker.Hermes?.UpdatePolygonAsync(t_Data);
		}
	}
}