using System.Collections;
using System.Collections.Generic;
using Manus.Networking.Sync;
using LidNet = Lidgren.Network;
using UnityEngine;

namespace Manus.Polygon.Skeleton.Networking
{
	public class SkeletonNetworkSync : BaseSync
	{


		public override void Clean()
		{
			m_Position = transform.localPosition;
			m_Rotation = transform.localRotation;
			m_Scale = transform.localScale;
		}

		public override bool IsDirty()
		{
			if (m_SmoothRoutine != null) return false; //still busy, probably
			if (m_Position != transform.localPosition) return true;
			if (m_Rotation != transform.localRotation) return true;
			if (m_Scale != transform.localScale) return true;
			return false;
		}

		public override void ReceiveData(LidNet.NetBuffer _Msg)
		{
			m_Position.x = _Msg.ReadFloat();
			m_Position.y = _Msg.ReadFloat();
			m_Position.z = _Msg.ReadFloat();

			m_Rotation.x = _Msg.ReadFloat();
			m_Rotation.y = _Msg.ReadFloat();
			m_Rotation.z = _Msg.ReadFloat();
			m_Rotation.w = _Msg.ReadFloat();

			m_Scale.x = _Msg.ReadFloat();
			m_Scale.y = _Msg.ReadFloat();
			m_Scale.z = _Msg.ReadFloat();

			if (m_Smooth)
			{
				if (m_SmoothRoutine != null) StopCoroutine(m_SmoothRoutine);
				m_SmoothRoutine = StartCoroutine("SmoothTransform");
				return;
			}
			transform.localPosition = m_Position;
			transform.localRotation = m_Rotation;
			transform.localScale = m_Scale;
		}

		public override void WriteData(LidNet.NetBuffer _Msg)
		{
			_Msg.Write(m_Position.x);
			_Msg.Write(m_Position.y);
			_Msg.Write(m_Position.z);

			_Msg.Write(m_Rotation.x);
			_Msg.Write(m_Rotation.y);
			_Msg.Write(m_Rotation.z);
			_Msg.Write(m_Rotation.w);

			_Msg.Write(m_Scale.x);
			_Msg.Write(m_Scale.y);
			_Msg.Write(m_Scale.z);
		}
	}
}

