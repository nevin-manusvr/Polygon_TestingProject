using UnityEngine;
using Hermes.Protocol.Polygon;
using Hermes.Protocol;
using Manus.Core.Hermes;

namespace Manus.Polygon.Skeleton
{
	[System.Serializable]
	public class Bone
	{
		public bool optional = false;
		public bool retarget = false;

		public BoneType type;
		public Transform bone;

		public Quaternion desiredRotation;

		public Bone(bool _Optional, BoneType _Type)
		{
			optional = _Optional;
			type = _Type;
		}

		public void AssignTransform(Transform _Bone)
		{
			bone = _Bone;
		}

		public static implicit operator Hermes.Protocol.Polygon.Bone(Bone _Bone)
		{
			return new Hermes.Protocol.Polygon.Bone()  
			{ 
				Position = new Translation() { Full = _Bone.bone.position.ToProto() }, 
				Rotation = new Orientation() { Full = _Bone.bone.rotation.ToProto() },
				Type = _Bone.type
			};
		}
	}

	[System.Serializable]
	public class ControlBone
	{
		public Vector3 position;
		public Quaternion rotation;

		public BoneLocalOffset[] bonesToControl;

		public ControlBone(Bone[] _BonesToControl)
		{
			var t_Offsets = new BoneLocalOffset[_BonesToControl.Length];

			for (int i = 0; i < _BonesToControl.Length; i++)
			{
				t_Offsets[i] = new BoneLocalOffset(_BonesToControl[i], this);
			}

			bonesToControl = t_Offsets;
		}

		public void Update(Vector3 _Position)
		{
			Update(_Position, rotation);
		}

		public void Update(Quaternion _Rotation)
		{
			Update(position, _Rotation);
		}

		public void Update(Vector3 _Position, Quaternion _Rotation)
		{
			foreach (var t_BoneLocalOffset in bonesToControl)
			{
				t_BoneLocalOffset.Update(this);
			}
		}

		// TODO: add implicit converter
	}

	[System.Serializable]
	public class BoneLocalOffset
	{
		public Bone bone;

		public Vector3 localPosition;
		public Quaternion localRotation;

		public BoneLocalOffset(Bone _Bone, ControlBone _Parent)
		{
			bone = _Bone;
			Update(_Parent);
		}

		public void Update(ControlBone _Parent)
		{
			Matrix4x4 t_ParentMatrix = Matrix4x4.TRS(_Parent.position, _Parent.rotation, Vector3.one).inverse;
			localPosition = t_ParentMatrix.MultiplyPoint3x4(bone.bone.position);
			Vector3 t_Forward = t_ParentMatrix.MultiplyVector(bone.bone.rotation * Vector3.forward);
			Vector3 t_Up = t_ParentMatrix.MultiplyVector(bone.bone.rotation * Vector3.up);
			localRotation = Quaternion.LookRotation(t_Forward, t_Up);
		}
	}
}