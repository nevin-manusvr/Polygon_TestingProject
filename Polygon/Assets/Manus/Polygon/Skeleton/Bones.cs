using UnityEngine;

namespace Manus.Polygon.Skeleton
{
	[System.Serializable]
	public class Bone
	{
		public Transform bone;

		public Bone(Transform bone)
		{
			if (bone == null) return;
			this.bone = bone;
		}
	}

	[System.Serializable]
	public class OptionalBone : Bone
	{
		public OptionalBone(Transform bone) : base(bone)
		{

		}
	}
}