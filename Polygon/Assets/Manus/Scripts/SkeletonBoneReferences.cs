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
		public Bone[] spine;

		public bool IsValid
		{
			get { return hip?.bone && spine?.Length > 0; }
		}

		public Dictionary<BoneType, Bone> GatherBones()
		{
			var bones = new Dictionary<BoneType, Bone>();
			bones.Add(hip.type, hip);
			foreach (Bone spineBone in spine)
			{
				bones.Add(spineBone.type, spineBone);
			}

			return bones;
		}

		public void AssignBones(Transform hip, Transform spine, Transform chest, Transform upperChest)
		{
			this.hip = new Bone(BoneType.Hips, hip);

			var spineBones = new List<Bone>();
			if (spine) spineBones.Add(new Bone(BoneType.Spine, spine));
			if (chest) spineBones.Add(new Bone(BoneType.Chest, chest));
			if (upperChest) spineBones.Add(new Bone(BoneType.UpperChest, upperChest));

			this.spine = spineBones.ToArray();
		}
	}

	[System.Serializable]
	public class Head
	{
		public Bone neck;
		public Bone head;
		//public OptionalBone eyeLeft;
		//public OptionalBone eyeRight;

		public bool IsValid
		{
			get { return neck?.bone && head?.bone; }
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
			this.neck = new Bone(BoneType.Neck, neck);
			this.head = new Bone(BoneType.Head, head);

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
		public Bone shoulder;
		public Bone upperArm;
		public Bone lowerArm;
		public HandBoneReferences hand;

		public bool IsValid
		{
			get { return shoulder?.bone && upperArm?.bone && lowerArm?.bone && hand.IsValid; }
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

		public void AssignBones(Transform shoulder, Transform upperArm, Transform lowerArm, bool left)
		{
			this.shoulder = new Bone(left ? BoneType.LeftShoulder : BoneType.RightShoulder, shoulder);
			this.upperArm = new Bone(left ? BoneType.LeftUpperArm : BoneType.RightUpperArm, upperArm);
			this.lowerArm = new Bone(left ? BoneType.LeftLowerArm : BoneType.RightLowerArm, lowerArm);
		}

		public void AssignHandBones(Transform lowerArm, Animator animator, bool left)
		{
			this.hand.PopulateBones(animator, lowerArm, left);
		}
	}

	[System.Serializable]
	public class Leg
	{
		public Bone upperLeg;
		public Bone lowerLeg;
		public Bone foot;
		public OptionalBone toes;
		public OptionalBone toesEnd;

		public bool IsValid
		{
			get { return upperLeg?.bone && lowerLeg?.bone && foot?.bone; }
		}

		public Dictionary<BoneType, Bone> GatherBones()
		{
			var bones = new Dictionary<BoneType, Bone>();
			bones.Add(upperLeg.type, upperLeg);
			bones.Add(lowerLeg.type, lowerLeg);
			bones.Add(foot.type, foot);
			if (toes?.bone != null) bones.Add(toes.type, toes);
			if (toesEnd?.bone != null) bones.Add(toesEnd.type, toesEnd);

			return bones;
		}

		public void AssignBones(Transform upperLeg, Transform lowerLeg, Transform foot, Transform toes, Transform toesEnd, bool left)
		{
			this.upperLeg = new Bone(left ? BoneType.LeftUpperLeg : BoneType.RightUpperLeg, upperLeg);
			this.lowerLeg = new Bone(left ? BoneType.LeftLowerLeg : BoneType.RightLowerLeg, lowerLeg);
			this.foot = new Bone(left ? BoneType.LeftFoot : BoneType.RightFoot, foot);

			if (toes == null)
			{
				Debug.LogWarning($"No toe bone assigned in the avatar for {foot.name}, consider assigning the foot for a better connection to the ground", foot);
				return;
			}
			
			this.toes = new OptionalBone(left ? BoneType.LeftToes : BoneType.RightToes, toes);

			if (toesEnd == null && toes.childCount == 0)
			{
				Debug.LogWarning($"No toe end bone assigned in the avatar for {toes.name}, consider assigning the foot for a better connection to the ground", toes);
				return;
			}

			this.toesEnd = new OptionalBone(left ? BoneType.LeftToesEnd : BoneType.RightToesEnd, toesEnd ?? this.toes.bone.GetChild(0));

			Vector3 pos = this.foot.bone.position;
			pos.y = 0;
			Debug.DrawRay(pos, Vector3.forward, Color.red, 100f);
		}
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

		public Leg legLeft;
		public Leg legRight;

		public bool IsValid
		{
			get { return head.IsValid && body.IsValid && armLeft.IsValid && armRight.IsValid && legLeft.IsValid && legRight.IsValid; }
		}

		public Dictionary<BoneType, Bone> GatherBones()
		{
			var bones = new Dictionary<BoneType, Bone>();
			
			bones.Add(root.type, root);
			AddToDictionary(body.GatherBones());
			AddToDictionary(head.GatherBones());
			AddToDictionary(armLeft.GatherBones());
			AddToDictionary(armRight.GatherBones());
			AddToDictionary(legLeft.GatherBones());
			AddToDictionary(legRight.GatherBones());

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
				animator.GetBoneTransform(HumanBodyBones.LeftLowerArm), true);
			this.armLeft.AssignHandBones(animator.GetBoneTransform(HumanBodyBones.LeftLowerArm), animator, true);

			this.armRight.AssignBones(
				animator.GetBoneTransform(HumanBodyBones.RightShoulder),
				animator.GetBoneTransform(HumanBodyBones.RightUpperArm),
				animator.GetBoneTransform(HumanBodyBones.RightLowerArm), false);
			this.armRight.AssignHandBones(animator.GetBoneTransform(HumanBodyBones.RightLowerArm), animator, false);

			this.legLeft.AssignBones(
				animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg),
				animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg),
				animator.GetBoneTransform(HumanBodyBones.LeftFoot),
				animator.GetBoneTransform(HumanBodyBones.LeftToes),
				null, true);
			this.legRight.AssignBones(
				animator.GetBoneTransform(HumanBodyBones.RightUpperLeg),
				animator.GetBoneTransform(HumanBodyBones.RightLowerLeg),
				animator.GetBoneTransform(HumanBodyBones.RightFoot),
				animator.GetBoneTransform(HumanBodyBones.RightToes),
				null, false);
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