using UnityEngine;
using Manus.ToBeHermes.Skeleton;

namespace Manus.Polygon.Skeleton
{
	[System.Serializable]
	public class Bone
	{
		public BoneType type;
		public Transform bone;

		public Quaternion desiredRotation;

		public Bone(BoneType type)
		{
			this.type = type;
		}

		public Bone(BoneType type, Transform bone)
		{
			this.type = type;
			this.bone = bone;
		}

		public void AssignTransform(Transform bone)
		{
			this.bone = bone;
		}
	}

	[System.Serializable]
	public class OptionalBone : Bone
	{
		public OptionalBone(BoneType type) : base(type) { }

		public OptionalBone(BoneType type, Transform bone) : base(type, bone) { }
	}

	[System.Serializable]
	public class ControlBone
	{
		public ControlPointType type;
		public Vector3 position;
		public Quaternion rotation;

		public ControlBone(ControlPointType type)
		{
			this.type = type;
		}
	}

	public enum ControlPointType
	{
		Ground,
		Height,
		Group
	}
}