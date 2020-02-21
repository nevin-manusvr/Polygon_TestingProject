using UnityEngine;
using Manus.ToBeHermes.Skeleton;

namespace Manus.Polygon.Skeleton
{
	[System.Serializable]
	public class Bone
	{
		public BoneType type;
		public Transform bone;

		public Vector3 controlPoint;
		public Quaternion desiredRotation;

		public Bone(BoneType type, Transform bone)
		{
			if (bone == null) return;

			this.type = type;
			this.bone = bone;
		}
	}

	[System.Serializable]
	public class OptionalBone : Bone
	{
		public OptionalBone(BoneType type, Transform bone) : base(type, bone) { }
	}
}