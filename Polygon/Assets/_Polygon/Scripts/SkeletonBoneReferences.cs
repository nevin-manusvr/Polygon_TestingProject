using System.Collections;
using System.Collections.Generic;

namespace ManusVR.Polygon
{
	using UnityEngine;

	#region BoneOrganization
	[System.Serializable]
	public struct Arm
	{
		public Bone shoulder;
		public Bone upperArm;
		public Bone lowerArm;
		public HandBoneReferences hand;
	}

	[System.Serializable]
	public struct Leg
	{
		public Bone upperLeg;
		public Bone lowerLeg;
		public Bone foot;
		public Bone toes;
		public Bone toesTip;
	}

	[System.Serializable]
	public struct Body
	{
		public Bone hip;
		public Bone[] spine;
	}

	[System.Serializable]
	public struct Head
	{
		public Bone neck;
		public Bone head;
		public Bone eyeLeft;
		public Bone eyeRight;
	}
	#endregion

	[System.Serializable]
	public class SkeletonBoneReferences
	{
		public Body body;
		public Arm armLeft;
		public Arm armRight;

		public Leg legLeft;
		public Leg legRight;

		public void Populate(Animator animator)
		{
			body.hip = new Bone(animator.GetBoneTransform(HumanBodyBones.Hips));
		}
	}
}