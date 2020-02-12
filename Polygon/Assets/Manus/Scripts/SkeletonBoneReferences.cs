using System.Collections;
using System.Collections.Generic;

namespace Manus.Polygon.Skeleton
{
	using UnityEngine;

	#region BoneOrganization
	[System.Serializable]
	public struct Body
	{
		public Bone hip;
		public Bone[] spine;

		public bool IsValid
		{
			get { return hip?.bone && spine?.Length > 0; }
		}

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
		public OptionalBone eyeLeft;
		public OptionalBone eyeRight;

		public bool IsValid
		{
			get { return neck?.bone && head?.bone; }
		}

		public void AssignBones(Transform neck, Transform head, Transform eyeLeft, Transform eyeRight)
		{
			this.neck = new Bone(neck);
			this.head = new Bone(head);

			if (eyeLeft && eyeRight)
			{
				this.eyeLeft = new OptionalBone(eyeLeft);
				this.eyeRight = new OptionalBone(eyeRight);
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

		public bool IsValid
		{
			get { return shoulder?.bone && upperArm?.bone && lowerArm?.bone && hand.IsValid; }
		}

		public void AssignBones(Transform shoulder, Transform upperArm, Transform lowerArm)
		{
			this.shoulder = new Bone(shoulder);
			this.upperArm = new Bone(upperArm);
			this.lowerArm = new Bone(lowerArm);

		}

		public void AssignHandBones(Transform lowerArm, Animator animator, bool left)
		{
			this.hand.PopulateBones(animator, lowerArm, left);
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

		public bool IsValid
		{
			get { return upperLeg?.bone && lowerLeg?.bone && foot?.bone; }
		}

		public void AssignBones(Transform upperLeg, Transform lowerLeg, Transform foot, Transform toes, Transform toesEnd)
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

			if (toesEnd == null && toes.childCount == 0)
			{
				Debug.LogWarning($"No toe end bone assigned in the avatar for {toes.name}, consider assigning the foot for a better connection to the ground", toes);
				return;
			}

			this.toesEnd = new Bone(toesEnd ?? this.toes.bone.GetChild(0));

			Vector3 pos = this.foot.bone.position;
			pos.y = 0;
			Debug.DrawRay(pos, Vector3.forward, Color.red, 100f);
		}
}

	#endregion

	[System.Serializable]
	public class SkeletonBoneReferences
	{
		public Bone main;

		public Head head;
		public Body body;

		public Arm armLeft;
		public Arm armRight;

		public Leg legLeft;
		public Leg legRight;

		public bool IsValid
		{
			get { return main?.bone != null && head.IsValid && body.IsValid && armLeft.IsValid && armRight.IsValid && legLeft.IsValid && legRight.IsValid; }
		}

		public void Populate(Animator animator)
		{
			this.main = new Bone(animator.GetBoneTransform(HumanBodyBones.Hips).parent);

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
				animator.GetBoneTransform(HumanBodyBones.LeftLowerArm));
			this.armLeft.AssignHandBones(animator.GetBoneTransform(HumanBodyBones.LeftLowerArm), animator, true);

			this.armRight.AssignBones(
				animator.GetBoneTransform(HumanBodyBones.RightShoulder),
				animator.GetBoneTransform(HumanBodyBones.RightUpperArm),
				animator.GetBoneTransform(HumanBodyBones.RightLowerArm));
			this.armRight.AssignHandBones(animator.GetBoneTransform(HumanBodyBones.RightLowerArm), animator, false);

			this.legLeft.AssignBones(
				animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg),
				animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg),
				animator.GetBoneTransform(HumanBodyBones.LeftFoot),
				animator.GetBoneTransform(HumanBodyBones.LeftToes),
				null);
			this.legRight.AssignBones(
				animator.GetBoneTransform(HumanBodyBones.RightUpperLeg),
				animator.GetBoneTransform(HumanBodyBones.RightLowerLeg),
				animator.GetBoneTransform(HumanBodyBones.RightFoot),
				animator.GetBoneTransform(HumanBodyBones.RightToes),
				null);
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