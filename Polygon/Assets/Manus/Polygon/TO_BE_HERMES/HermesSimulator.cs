using UnityEngine;
using HProt = Hermes.Protocol;
using Hermes.Protocol.Polygon;
using Manus.Core;
using Manus.Core.Hermes;
using Manus.Core.Utility;
using Manus.Polygon;
using Manus.ToBeHermes.IK;
using Manus.ToBeHermes.Tracking;
namespace Manus.ToBeHermes
{
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

			UnityMainThreadDispatcher.Instance().Enqueue(
				() =>
				{
					foreach (Grave t_Grave in graveyard.graves)
					{
						t_Grave.possession.Move();
					}
				});
			

			UpdatePolygonSkeletons();
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;
			if (IK != null && IK.Skeleton != null)
			{
				foreach (var t_Bone in IK.Skeleton.bones)
				{
					Vector3 t_Pos = t_Bone.position.ToUnityVector3();
					Quaternion t_Rot = t_Bone.rotation.ToUnityQuat();

					Gizmos.color = Color.cyan;
					Gizmos.DrawWireSphere(t_Pos, .02f);
					Gizmos.color = Color.blue;
					Gizmos.DrawRay(t_Pos, t_Bone.rotation.ToUnityQuat() * Vector3.forward * .05f);
					Gizmos.color = Color.green;
					Gizmos.DrawRay(t_Pos, t_Bone.rotation.ToUnityQuat() * Vector3.up * .05f);
					Gizmos.color = Color.red;
					Gizmos.DrawRay(t_Pos, t_Bone.rotation.ToUnityQuat() * Vector3.right * .05f);

				}

				foreach (var t_Control in IK.Skeleton.controls)
				{
					Vector3 t_Pos = t_Control.position.ToUnityVector3();
					Quaternion t_Rot = t_Control.rotation.ToUnityQuat();
					Gizmos.matrix = Matrix4x4.TRS(t_Pos, t_Rot, Vector3.one);
					Gizmos.color = Color.magenta;
					Gizmos.DrawWireCube(Vector3.zero, new Vector3(.1f, .03f, 0.1f));
					Gizmos.color = Color.blue;
					Gizmos.DrawRay(Vector3.zero, Vector3.forward * .05f);
					Gizmos.color = Color.green;
					Gizmos.DrawRay(Vector3.zero, Vector3.up * .05f);
					Gizmos.color = Color.red;
					Gizmos.DrawRay(Vector3.zero, Vector3.right * .05f);
					Gizmos.matrix = Matrix4x4.identity;
				}
			}

			foreach (var t_Grave in graveyard.graves)
			{
				t_Grave.possession.DrawGizmos();
			}
		}

		public void OnNewSkeletonDefinition(HProt.Polygon.InternalData _Poly)
		{
			//TODO : remove
			UnityMainThreadDispatcher.Instance().Enqueue(
				() =>
				{
					foreach (var t_Skeleton in _Poly.Skeleton.Skeletons)
					{
						graveyard.AddGrave(t_Skeleton);
						graveyard.GetGrave(t_Skeleton.DeviceID).possession.SetTargetSkeleton(IK.Skeleton);
					}
				});
		}

		public void UpdatePolygonSkeletons()
		{
			if (ManusManager.instance.communicationHub.careTaker == null) 
				return;

			var t_Data = new Data();

			foreach (var grave in graveyard.graves)
			{
				t_Data.Skeletons.Add(grave.possession.Skeleton);
			}

			ManusManager.instance.communicationHub.careTaker.Hermes?.UpdatePolygonAsync(t_Data);
		}
	}
}