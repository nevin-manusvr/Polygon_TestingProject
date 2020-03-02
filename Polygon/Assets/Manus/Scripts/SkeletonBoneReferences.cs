using System.Collections;
using System.Collections.Generic;
using Manus.ToBeHermes.Skeleton;

namespace Manus.Polygon.Skeleton
{
	using UnityEngine;

	#region BoneOrganization

	[System.Serializable]
	public class Body
	{
		public Bone hip;
		// public Bone[] spine;
		public Bone spine;
		public OptionalBone chest;
		public OptionalBone upperChest;

		public ControlBone hipControl;
		public ControlBone upperBodyControl;

		public bool IsValid
		{
			get { return hip.bone && spine.bone; }
		}

		public Body()
		{
			hip = new Bone(BoneType.Hips);
			spine = new Bone(BoneType.Spine);
			chest = new OptionalBone(BoneType.Chest);
			upperChest = new OptionalBone(BoneType.UpperChest);
		}

		public Dictionary<BoneType, Bone> GatherBones()
		{
			var bones = new Dictionary<BoneType, Bone>();
			bones.Add(hip.type, hip);
			bones.Add(spine.type, spine);

			if (chest.bone) bones.Add(chest.type, chest);
			if (upperChest.bone) bones.Add(upperChest.type, upperChest);
			
			return bones;
		}

		public void AssignBones(Transform hip, Transform spine, Transform chest, Transform upperChest)
		{
			this.hip.AssignTransform(hip);
			this.spine.AssignTransform(spine);
			if (chest) this.chest.AssignTransform(chest);
			if (upperChest) this.upperChest.AssignTransform(upperChest);
		}
	}

	[System.Serializable]
	public class Head
	{
		public Bone neck;
		public Bone head;

		public ControlBone modelHeight;

		public bool IsValid
		{
			get { return neck.bone && head.bone; }
		}

		public Head()
		{
			neck = new Bone(BoneType.Neck);
			head = new Bone(BoneType.Head);

			modelHeight = new ControlBone(ControlPointType.Height);
		}

		public Dictionary<BoneType, Bone> GatherBones()
		{
			var bones = new Dictionary<BoneType, Bone>();
			bones.Add(neck.type, neck);
			bones.Add(head.type, head);

			return bones;
		}

		public void AssignBones(Transform neck, Transform head, Transform eyeLeft, Transform eyeRight)
		{
			this.neck.AssignTransform(neck);
			this.head.AssignTransform(head);

			if (eyeLeft && eyeRight)
			{
				//this.eyeLeft = new OptionalBone(eyeLeft);
				//this.eyeRight = new OptionalBone(eyeRight);
			}
		}
	}

	[System.Serializable]
	public class Arm
	{
		private bool left;

		public Bone shoulder;
		public Bone upperArm;
		public Bone lowerArm;
		public HandBoneReferences hand;

		public Arm(bool left)
		{
			this.left = left;

			shoulder = new Bone(left ? BoneType.LeftShoulder : BoneType.RightShoulder);
			upperArm = new Bone(left ? BoneType.LeftUpperArm : BoneType.RightUpperArm);
			lowerArm = new Bone(left ? BoneType.LeftLowerArm : BoneType.RightLowerArm);

			hand = new HandBoneReferences(left);
		}

		public bool IsValid
		{
			get { return shoulder.bone && upperArm.bone && lowerArm.bone && hand.IsValid; }
		}

		public Dictionary<BoneType, Bone> GatherBones()
		{
			var bones = new Dictionary<BoneType, Bone>();
			bones.Add(shoulder.type, shoulder);
			bones.Add(upperArm.type, upperArm);
			bones.Add(lowerArm.type, lowerArm);
			if (hand?.wrist?.bone != null) bones.Add(hand.wrist.type, hand.wrist);

			return bones;
		}

		public void AssignBones(Transform shoulder, Transform upperArm, Transform lowerArm)
		{
			this.shoulder.AssignTransform(shoulder);
			this.upperArm.AssignTransform(upperArm);
			this.lowerArm.AssignTransform(lowerArm);
		}

		public void AssignHandBones(Transform lowerArm, Animator animator)
		{
			this.hand.PopulateBones(animator, lowerArm, left);
		}
	}

	[System.Serializable]
	public class Leg
	{
		private bool left = false;

		public Bone upperLeg;
		public Bone lowerLeg;
		public Bone foot;
		public OptionalBone toes;
		public OptionalBone toesEnd;

		public ControlBone heel;

		public Leg(bool left)
		{
			this.left = left;

			upperLeg = new Bone(left ? BoneType.LeftUpperLeg : BoneType.RightUpperLeg);
			lowerLeg = new Bone(left ? BoneType.LeftLowerLeg : BoneType.RightLowerLeg);
			foot = new Bone(left ? BoneType.LeftFoot : BoneType.RightFoot);

			toes = new OptionalBone(left ? BoneType.LeftToes : BoneType.RightToes);
			toesEnd = new OptionalBone(left ? BoneType.LeftToesEnd : BoneType.RightToesEnd);

			heel = new ControlBone(ControlPointType.Ground);
		}

		public bool IsValid
		{
			get { return upperLeg.bone && lowerLeg.bone && foot.bone; }
		}

		public Dictionary<BoneType, Bone> GatherBones(GatherType gatherType)
		{
			var bones = new Dictionary<BoneType, Bone>();

			bones.Add(upperLeg.type, upperLeg);
			bones.Add(lowerLeg.type, lowerLeg);
			bones.Add(foot.type, foot);
			if (gatherType == GatherType.All && toes.bone != null) bones.Add(toes.type, toes);
			if (gatherType == GatherType.All && toesEnd.bone != null) bones.Add(toesEnd.type, toesEnd);

			return bones;
		}

		public void AssignBones(Transform upperLeg, Transform lowerLeg, Transform foot, Transform toes, Transform toesEnd)
		{
			this.upperLeg.AssignTransform(upperLeg);
			this.lowerLeg.AssignTransform(lowerLeg);
			this.foot.AssignTransform(foot);

			if (toes == null)
			{
				Debug.LogWarning($"No toe bone assigned in the avatar for {foot.name}, consider assigning the foot for a better connection to the ground", foot);
				return;
			}
			
			this.toes.AssignTransform(toes);

			if (toesEnd == null && toes.childCount == 0)
			{
				Debug.LogWarning($"No toe end bone assigned in the avatar for {toes.name}, consider assigning the foot for a better connection to the ground", toes);
				return;
			}

			this.toesEnd.AssignTransform(toesEnd ?? this.toes.bone.GetChild(0));
		}
}

	public enum GatherType
	{
		All,
		Retargeted
	}

	#endregion

	[System.Serializable]
	public class SkeletonBoneReferences
	{
		public Bone root;

		public Head head;
		public Body body;

		public Arm armLeft;
		public Arm armRight;

		public Leg legLeft = new Leg(true);
		public Leg legRight = new Leg(false);

		public bool IsValid
		{
			get { return head.IsValid && body.IsValid && armLeft.IsValid && armRight.IsValid && legLeft.IsValid && legRight.IsValid; }
		}

		public SkeletonBoneReferences()
		{
			Clear();
		}

		public Dictionary<BoneType, Bone> GatherBones(GatherType gatherType)
		{
			var bones = new Dictionary<BoneType, Bone>();
			
			bones.Add(root.type, root);
			AddToDictionary(body.GatherBones());
			AddToDictionary(head.GatherBones());
			AddToDictionary(armLeft.GatherBones());
			AddToDictionary(armRight.GatherBones());
			AddToDictionary(legLeft.GatherBones(gatherType));
			AddToDictionary(legRight.GatherBones(gatherType));

			return bones;

			void AddToDictionary(Dictionary<BoneType, Bone> bonesToAdd)
			{
				foreach (var bone in bonesToAdd)
				{
					if (!bones.ContainsKey(bone.Key))
					{
						bones.Add(bone.Key, bone.Value);
					}
				}
			}
		}

		public void Populate(Animator animator)
		{
			this.root = new Bone(BoneType.Root, animator.GetBoneTransform(HumanBodyBones.Hips).parent);
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
			this.armLeft.AssignHandBones(animator.GetBoneTransform(HumanBodyBones.LeftLowerArm), animator);

			this.armRight.AssignBones(
				animator.GetBoneTransform(HumanBodyBones.RightShoulder),
				animator.GetBoneTransform(HumanBodyBones.RightUpperArm),
				animator.GetBoneTransform(HumanBodyBones.RightLowerArm));
			this.armRight.AssignHandBones(animator.GetBoneTransform(HumanBodyBones.RightLowerArm), animator);

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
			root = new Bone(BoneType.Root);

			head = new Head();
			body = new Body();
			armLeft = new Arm(true);
			armRight = new Arm(false);
			legLeft = new Leg(true);
			legRight = new Leg(false);
		}
	}
}