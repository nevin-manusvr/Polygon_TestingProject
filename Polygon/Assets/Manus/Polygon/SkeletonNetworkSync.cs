using System.Collections;
using System.Collections.Generic;
using Manus.Networking.Sync;
using LidNet = Lidgren.Network;
using UnityEngine;
using Hermes.Protocol.Polygon;

namespace Manus.Polygon.Skeleton.Networking
{
	[UnityEngine.AddComponentMenu("Manus/Networking/Sync/Polygon Skeleton (Sync)")]
	[RequireComponent(typeof(PolygonSkeleton))]
	public class SkeletonNetworkSync : BaseSync
	{
		public class NetworkedBone
		{
			public Bone bone;
			public Quaternion targetRotation;
		}

		private PolygonSkeleton m_PolygonSkeleton;
		private Dictionary<BodyMeasurements, float> m_BodyMeasurements = new Dictionary<BodyMeasurements, float>();
		
		public bool smooth = true;
		public float smoothFactor = 20.0f;
		Coroutine m_SmoothRoutine = null;

		Vector3 m_HipPositionTarget;

		Dictionary<BoneType, NetworkedBone> m_Bones = null;

		bool m_Owned = false;

		public override void Initialize(Manus.Networking.NetObject _Object)
		{
			m_PolygonSkeleton = GetComponent<PolygonSkeleton>();

			var t_Bones = m_PolygonSkeleton.boneReferences.GatherBones(GatherType.Networked);

			m_Bones = new Dictionary<BoneType, NetworkedBone>();
			foreach (var t_Bone in t_Bones)
			{
				m_Bones.Add(t_Bone.Key, new NetworkedBone() { bone = t_Bone.Value, targetRotation = t_Bone.Value.bone.localRotation });
			}
		}

		public override void Clean()
		{
		}

		public override bool IsDirty()
		{
			return true;
		}

		public override void ReceiveData(LidNet.NetBuffer _Msg)
		{
			
			/* TODO: reenable this
			if (_Msg.ReadBoolean())
			{
				m_BodyMeasurements = new Dictionary<BodyMeasurements, float>();

				int t_Count = _Msg.ReadInt32();
				for (int i = 0; i < t_Count; i++)
				{
					m_BodyMeasurements.Add((BodyMeasurements)_Msg.ReadInt32(), _Msg.ReadFloat());
				}
			}
			*/

			// Sync rotations

			if (_Msg.ReadBoolean())
			{
				m_HipPositionTarget = _Msg.ReadVector3();

				int t_Count = _Msg.ReadInt32();
				
				for (int i = 0; i < t_Count; i++)
				{
					var t_Type = (BoneType)_Msg.ReadInt32();
					var t_Rotation = _Msg.ReadQuaternion();

					if (m_Bones.ContainsKey(t_Type))
					{
						m_Bones[t_Type].targetRotation = t_Rotation;
					}
				}
			}

			if (m_Owned) //don't apply any data if you own this skeletor
			{
				if (m_SmoothRoutine != null)
				{
					StopCoroutine(m_SmoothRoutine);
					m_SmoothRoutine = null;
				}
				return;
			}

			if (smooth)
			{
				if (m_SmoothRoutine == null) m_SmoothRoutine = StartCoroutine(SmoothTransform());
				return;
			}
			if (m_SmoothRoutine != null)
			{
				StopCoroutine(m_SmoothRoutine);
				m_SmoothRoutine = null;
			}
			
			m_Bones[BoneType.Hips].bone.bone.position = m_HipPositionTarget;
			foreach (var t_Bone in m_Bones)
			{
				t_Bone.Value.bone.bone.localRotation = t_Bone.Value.targetRotation;
			}
		}

		System.Collections.IEnumerator SmoothTransform()
		{
			while (true)
			{
				m_Bones[BoneType.Hips].bone.bone.position = Vector3.Lerp(m_Bones[BoneType.Hips].bone.bone.position, m_HipPositionTarget, smoothFactor * Time.deltaTime);

				foreach (var t_Bone in m_Bones)
				{
					t_Bone.Value.bone.bone.localRotation = Quaternion.Slerp(t_Bone.Value.bone.bone.localRotation, t_Bone.Value.targetRotation, smoothFactor * Time.deltaTime);
				}
				yield return new WaitForEndOfFrame();
			}
		}

		public override void WriteData(LidNet.NetBuffer _Msg)
		{
			/* TODO: reenable this
			if (m_BodyMeasurements == null)
			{
				_Msg.Write(false);
			}
			else
			{
				_Msg.Write(true);
				_Msg.Write(m_BodyMeasurements.Count);
				foreach (var t_Measurement in m_BodyMeasurements)
				{
					_Msg.Write((int)t_Measurement.Key);
					_Msg.Write(t_Measurement.Value);
				}
			}
			*/

			// Sync rotations

			if (m_PolygonSkeleton == null)
			{
				_Msg.Write(false);
			}
			else
			{
				_Msg.Write(true);

				_Msg.Write(m_Bones[BoneType.Hips].bone.bone.position);


				_Msg.Write(m_Bones.Count);

				foreach (var bone in m_Bones)
				{
					_Msg.Write((int)bone.Key);
					_Msg.Write(bone.Value.bone.bone.localRotation);
				}
			}
		}

		public override void OnGainOwnership(Manus.Networking.NetObject _Object)
		{
			m_Owned = true;
		}

		public override void OnLoseOwnership(Manus.Networking.NetObject _Object)
		{
			m_Owned = false;
		}
	}
}

