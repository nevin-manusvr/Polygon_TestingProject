using System.Collections;
using System.Collections.Generic;
using Manus.Networking.Sync;
using LidNet = Lidgren.Network;
using UnityEngine;
using Hermes.Protocol.Polygon;

namespace Manus.Polygon.Skeleton.Networking
{
	[RequireComponent(typeof(PolygonSkeleton))]
	public class SkeletonNetworkSync : BaseSync
	{
		private PolygonSkeleton m_PolygonSkeleton;
		private Dictionary<BodyMeasurements, float> m_BodyMeasurements = new Dictionary<BodyMeasurements, float>();

		#region Private Methods

		private bool HasSkeletonComponent()
		{
			if (m_PolygonSkeleton == null)
				if (GetComponent<PolygonSkeleton>())
					m_PolygonSkeleton = GetComponent<PolygonSkeleton>();
			
			return m_PolygonSkeleton != null;
		}

		#endregion

		public override void Clean()
		{
			//m_Position = transform.localPosition;
			//m_Rotation = transform.localRotation;
			//m_Scale = transform.localScale;
		}

		public override bool IsDirty()
		{
			return true;
			//if (m_SmoothRoutine != null) return false; //still busy, probably
			//if (m_Position != transform.localPosition) return true;
			//if (m_Rotation != transform.localRotation) return true;
			//if (m_Scale != transform.localScale) return true;
			return false;
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
			if (!HasSkeletonComponent())
				return;

			var t_Bones = m_PolygonSkeleton.boneReferences.GatherBones(GatherType.Networked);

			if (_Msg.ReadBoolean())
			{
				int t_Count = _Msg.ReadInt32();
				
				for (int i = 0; i < t_Count; i++)
				{
					var t_Type = (BoneType)_Msg.ReadInt32();
					var t_Rotation = _Msg.ReadQuaternion();

					if (t_Bones.ContainsKey(t_Type))
					{
						t_Bones[t_Type].bone.rotation = t_Rotation;
					}
				}
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
			if (!HasSkeletonComponent())
				return;

			if (m_PolygonSkeleton == null)
			{
				_Msg.Write(false);
			}
			else
			{
				_Msg.Write(true);

				var bones = m_PolygonSkeleton.boneReferences.GatherBones(GatherType.Networked);
				_Msg.Write(bones.Count);

				foreach (var bone in bones)
				{
					_Msg.Write((int)bone.Key);
					_Msg.Write(bone.Value.bone.rotation);
				}
			}
		}
	}
}

