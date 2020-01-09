using System.Collections;
using System.Collections.Generic;

namespace ManusVR.Polygon
{
	using UnityEngine;

	#region BoneOrganization
	[System.Serializable]
	public struct Body
	{
		public Bone hip;
		public Bone[] spine;

		public void AssignBones(Transform hip, Transform spine, Transform chest, Transform upperChest)
		{
			this.hip = new Bone(hip);

			var spineBones = new List<Bone>();
			if (spine) spineBones.Add(new Bone(spine));
			if (chest) spineBones.Add(new Bone(chest));
			if (upperChest) spineBones.Add(new Bone(upperChest));

			this.spine = spineBones.ToArray();
		}
	}

	[System.Serializable]
	public struct Head
	{
		public Bone neck;
		public Bone head;
		public Bone eyeLeft;
		public Bone eyeRight;

		public void AssignBones(Transform neck, Transform head, Transform eyeLeft, Transform eyeRight)
		{
			this.neck = new Bone(neck);
			this.head = new Bone(head);

			if (eyeLeft && eyeRight)
			{
				this.eyeLeft = new Bone(eyeLeft);
				this.eyeRight = new Bone(eyeRight);
			}
		}
	}

	[System.Serializable]
	public struct Arm
	{
		public Bone shoulder;
		public Bone upperArm;
		public Bone lowerArm;
		public HandBoneReferences hand;

		public void AssignBones(Transform shoulder, Transform upperArm, Transform lowerArm, Animator animator, bool left)
		{
			this.shoulder = new Bone(shoulder);
			this.upperArm = new Bone(upperArm);
			this.lowerArm = new Bone(lowerArm);

			//this.hand.PopulateBones(animator, lowerArm, left);
			this.hand.PopulateBones(lowerArm);
		}
	}

	[System.Serializable]
	public struct Leg
	{
		public Bone upperLeg;
		public Bone lowerLeg;
		public Bone foot;
		public Bone toes;
		public Bone toesEnd;

		public void AssignBones(Transform upperLeg, Transform lowerLeg, Transform foot, Transform toes)
		{
			this.upperLeg = new Bone(upperLeg);
			this.lowerLeg = new Bone(lowerLeg);
			this.foot = new Bone(foot);

			if (toes == null)
			{
				Debug.LogWarning($"No toe bone assigned in the avatar for {foot.name}, consider assigning the foot for a better connection to the ground", foot);
				return;
			}
			
			this.toes = new Bone(toes);

			if (toes.childCount == 0)
			{
				Debug.LogWarning($"No toe end bone assigned in the avatar for {toes.name}, consider assigning the foot for a better connection to the ground", toes);
				return;
			}

			this.toesEnd = new Bone(this.toes.bone.GetChild(0));
		}
}

	#endregion

	[System.Serializable]
	public class SkeletonBoneReferences
	{
		public Head head;
		public Body body;

		public Arm armLeft;
		public Arm armRight;

		public Leg legLeft;
		public Leg legRight;

		public void Populate(Animator animator)
		{
			this.head.AssignBones(
				animator.GetBoneTransform(HumanBodyBones.Neck),
				animator.GetBoneTransform(HumanBodyBones.Head),
				animator.GetBoneTransform(HumanBodyBones.LeftEye),
				animator.GetBoneTransform(HumanBodyBones.RightEye));
			this.body.AssignBones(
				animator.GetBoneTransform(HumanBodyBones.Hips),
				animator.GetBoneTransform(HumanBodyBones.Spine),
				animator.GetBoneTransform(HumanBodyBones.Chest),
				animator.GetBoneTransform(HumanBodyBones.UpperChest));
			this.armLeft.AssignBones(
				animator.GetBoneTransform(HumanBodyBones.LeftShoulder), 
				animator.GetBoneTransform(HumanBodyBones.LeftUpperArm),
				animator.GetBoneTransform(HumanBodyBones.LeftLowerArm), 
				animator, true);
			this.armRight.AssignBones(
				animator.GetBoneTransform(HumanBodyBones.RightShoulder),
				animator.GetBoneTransform(HumanBodyBones.RightUpperArm),
				animator.GetBoneTransform(HumanBodyBones.RightLowerArm),
				animator, false);
			this.legLeft.AssignBones(
				animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg),
				animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg),
				animator.GetBoneTransform(HumanBodyBones.LeftFoot),
				animator.GetBoneTransform(HumanBodyBones.LeftToes));
			this.legRight.AssignBones(
				animator.GetBoneTransform(HumanBodyBones.RightUpperLeg),
				animator.GetBoneTransform(HumanBodyBones.RightLowerLeg),
				animator.GetBoneTransform(HumanBodyBones.RightFoot),
				animator.GetBoneTransform(HumanBodyBones.RightToes));
		}

		public void Clear()
		{
			head = new Head();
			body = new Body();
			armLeft = new Arm();
			armRight = new Arm();
			legLeft = new Leg();
			legRight = new Leg();
		}
	}
}