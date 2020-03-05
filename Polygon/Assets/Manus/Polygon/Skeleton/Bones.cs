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

		public Bone[] bonesToControl;

		public ControlBone(Bone[] _BonesToControl)
		{
			bonesToControl = _BonesToControl;
		}
	}

	public enum ControlPointType
	{
		Ground,
		Height,
		Group
	}
}