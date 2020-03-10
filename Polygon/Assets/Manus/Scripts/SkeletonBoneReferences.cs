using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hermes.Protocol.Polygon;

namespace Manus.Polygon.Skeleton
{
	using System;

	#region BoneOrganization

	[System.Serializable]
	public class Body : IBoneGroup
	{
		public Bone hip;
		// public Bone[] spine;
		public Bone spine;
		public Bone chest;
		public Bone upperChest;

		public ControlBone hipControl;
		public ControlBone upperBodyControl;

		public bool IsValid
		{
			get
			{
				foreach (var bone in GatherBones(GatherType.All))
				{
					if (!bone.Value.optional && bone.Value.bone == null)
						return false;
				}

				return true;
			}
		}

		public Body()
		{
			hip = new Bone(false, BoneType.Hips);
			spine = new Bone(false, BoneType.Spine);

			chest = new Bone(true, BoneType.Chest);
			upperChest = new Bone(true, BoneType.UpperChest);

			hipControl = new ControlBone(ControlBoneType.HipControl, new[] { hip, spine });
			upperBodyControl = new ControlBone(ControlBoneType.UpperBodyControl, new[] { spine });
		}

		public Dictionary<BoneType, Bone> GatherBones(GatherType gatherType)
		{
			var bones = new Dictionary<BoneType, Bone>();

			switch (gatherType)
			{
				case GatherType.All:
					bones.Add(hip.type, hip);
					bones.Add(spine.type, spine);
					if (chest.bone) bones.Add(chest.type, chest);
					if (upperChest.bone) bones.Add(upperChest.type, upperChest);
					break;

				case GatherType.Retargeted:
					bones.Add(hip.type, hip);
					bones.Add(spine.type, spine);
					if (chest.bone) bones.Add(chest.type, chest);
					if (upperChest.bone) bones.Add(upperChest.type, upperChest);
					break;

				case GatherType.Networked:
					bones.Add(hip.type, hip);
					bones.Add(spine.type, spine);
					if (chest.bone) bones.Add(chest.type, chest);
					if (upperChest.bone) bones.Add(upperChest.type, upperChest);
					break;
			}
			
			return bones;
		}
		
		public Dictionary<ControlBoneType, ControlBone> GatherControlBones(GatherType gatherType)
		{
			var bones = new Dictionary<ControlBoneType, ControlBone>();

			switch (gatherType)
			{
				case GatherType.All:
					bones.Add(hipControl.type, hipControl);
					bones.Add(upperBodyControl.type, upperBodyControl);
					break;

				case GatherType.Retargeted:
					bones.Add(hipControl.type, hipControl);
					bones.Add(upperBodyControl.type, upperBodyControl);
					break;

				case GatherType.Networked:
					break;
			}
			
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
	public class Head : IBoneGroup
	{
		public Bone neck;
		public Bone head;

		public ControlBone modelHeight;

		public bool IsValid
		{
			get
			{
				foreach (var bone in GatherBones(GatherType.All))
				{
					if (!bone.Value.optional && bone.Value.bone == null)
						return false;
				}

				return true;
			}
		}

		public Head()
		{
			neck = new Bone(false, BoneType.Neck);
			head = new Bone(false, BoneType.Head);

			modelHeight = new ControlBone(ControlBoneType.RootControl, null);
		}

		public Dictionary<BoneType, Bone> GatherBones(GatherType gatherType)
		{
			var bones = new Dictionary<BoneType, Bone>();
			
			switch (gatherType)
			{
				case GatherType.All:
					bones.Add(neck.type, neck);
					bones.Add(head.type, head);
					break;

				case GatherType.Retargeted:
					bones.Add(neck.type, neck);
					bones.Add(head.type, head);
					break;

				case GatherType.Networked:
					bones.Add(neck.type, neck);
					bones.Add(head.type, head);
					break;
			}

			return bones;
		}
		
		public Dictionary<ControlBoneType, ControlBone> GatherControlBones(GatherType gatherType)
		{
			var bones = new Dictionary<ControlBoneType, ControlBone>();
			
			switch (gatherType)
			{
				case GatherType.All:
					bones.Add(modelHeight.type, modelHeight);
					break;

				case GatherType.Retargeted:
					break;

				case GatherType.Networked:
					break;
			}

			return bones;
		}

		public void AssignBones(Transform neck, Transform head, Transform eyeLeft, Transform eyeRight)
		{
			this.neck.AssignTransform(neck);
			this.head.AssignTransform(head);
		}
	}

	[System.Serializable]
	public class Arm : IBoneGroup
	{
		private bool left;

		public Bone shoulder;
		public Bone upperArm;
		public Bone lowerArm;
		public HandBoneReferences hand;

		public Arm(bool left)
		{
			this.left = left;

			shoulder = new Bone(false, left ? BoneType.LeftShoulder : BoneType.RightShoulder);
			upperArm = new Bone(false, left ? BoneType.LeftUpperArm : BoneType.RightUpperArm);
			lowerArm = new Bone(false, left ? BoneType.LeftLowerArm : BoneType.RightLowerArm);

			hand = new HandBoneReferences(left);
		}

		public bool IsValid
		{
			get
			{
				foreach (var bone in GatherBones(GatherType.All))
				{
					if (!bone.Value.optional && bone.Value.bone == null)
						return false;
				}

				return true;
			}
		}

		public Dictionary<BoneType, Bone> GatherBones(GatherType gatherType)
		{
			var bones = new Dictionary<BoneType, Bone>();

			switch (gatherType)
			{
				case GatherType.All:
					bones.Add(shoulder.type, shoulder);
					bones.Add(upperArm.type, upperArm);
					bones.Add(lowerArm.type, lowerArm);
					if (hand.wrist.bone != null) bones.Add(hand.wrist.type, hand.wrist);
					break;

				case GatherType.Retargeted:
					bones.Add(shoulder.type, shoulder);
					bones.Add(upperArm.type, upperArm);
					bones.Add(lowerArm.type, lowerArm);
					if (hand.wrist.bone != null) bones.Add(hand.wrist.type, hand.wrist);
					break;

				case GatherType.Networked:
					bones.Add(shoulder.type, shoulder);
					bones.Add(upperArm.type, upperArm);
					bones.Add(lowerArm.type, lowerArm);
					if (hand.wrist.bone != null) bones.Add(hand.wrist.type, hand.wrist);
					break;
			}

			return bones;
		}
		
		public Dictionary<ControlBoneType, ControlBone> GatherControlBones(GatherType gatherType)
		{
			var bones = new Dictionary<ControlBoneType, ControlBone>();

			switch (gatherType)
			{
				case GatherType.All:
					break;

				case GatherType.Retargeted:
					break;

				case GatherType.Networked:
					break;
			}

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
	public class Leg : IBoneGroup
	{
		private bool m_Left = false;

		public Bone upperLeg;
		public Bone lowerLeg;
		public Bone foot;
		public Bone toes;
		public Bone toesEnd;

		public ControlBone heel;

		public Leg(bool _Left)
		{
			m_Left = _Left;

			upperLeg = new Bone(false, m_Left ? BoneType.LeftUpperLeg : BoneType.RightUpperLeg);
			lowerLeg = new Bone(false, m_Left ? BoneType.LeftLowerLeg : BoneType.RightLowerLeg);
			foot = new Bone(false, m_Left ? BoneType.LeftFoot : BoneType.RightFoot);

			toes = new Bone(true, m_Left ? BoneType.LeftToes : BoneType.RightToes);
			toesEnd = new Bone(true, m_Left ? BoneType.LeftToesEnd : BoneType.RightToesEnd);

			heel = new ControlBone(m_Left ? ControlBoneType.LeftHeelControl : ControlBoneType.RightHeelControl, new[] { foot });
		}

		public bool IsValid
		{
			get
			{
				foreach (var bone in GatherBones(GatherType.All))
				{
					if (!bone.Value.optional && bone.Value.bone == null)
						return false;
				}

				return true;
			}
		}

		public Dictionary<BoneType, Bone> GatherBones(GatherType gatherType)
		{
			var bones = new Dictionary<BoneType, Bone>();

			switch (gatherType)
			{
				case GatherType.All:
					bones.Add(upperLeg.type, upperLeg);
					bones.Add(lowerLeg.type, lowerLeg);
					bones.Add(foot.type, foot);
					if (toes.bone != null) bones.Add(toes.type, toes);
					if (toesEnd.bone != null) bones.Add(toesEnd.type, toesEnd);
					break;

				case GatherType.Retargeted:
					bones.Add(upperLeg.type, upperLeg);
					bones.Add(lowerLeg.type, lowerLeg);
					bones.Add(foot.type, foot);
					break;

				case GatherType.Networked:
					bones.Add(upperLeg.type, upperLeg);
					bones.Add(lowerLeg.type, lowerLeg);
					bones.Add(foot.type, foot);
					if (toes.bone != null) bones.Add(toes.type, toes);
					if (toesEnd.bone != null) bones.Add(toesEnd.type, toesEnd);
					break;
			}

			return bones;
		}
	
		public Dictionary<ControlBoneType, ControlBone> GatherControlBones(GatherType gatherType)
		{
			var bones = new Dictionary<ControlBoneType, ControlBone>();

			switch (gatherType)
			{
				case GatherType.All:
					bones.Add(heel.type, heel);
					break;

				case GatherType.Retargeted:
					bones.Add(heel.type, heel);
					break;

				case GatherType.Networked:
					break;
			}

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
		Retargeted,
		Networked
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
			AddToDictionary(body.GatherBones(gatherType));
			AddToDictionary(head.GatherBones(gatherType));
			AddToDictionary(armLeft.GatherBones(gatherType));
			AddToDictionary(armRight.GatherBones(gatherType));
			AddToDictionary(legLeft.GatherBones(gatherType));
			AddToDictionary(legRight.GatherBones(gatherType));

			AddToDictionary(armLeft.hand.GatherBones(gatherType));
			AddToDictionary(armRight.hand.GatherBones(gatherType));


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

		public Dictionary<ControlBoneType, ControlBone> GatherControlBones(GatherType gatherType)
		{
			var bones = new Dictionary<ControlBoneType, ControlBone>();

			AddToDictionary(body.GatherControlBones(gatherType));
			AddToDictionary(head.GatherControlBones(gatherType));
			AddToDictionary(armLeft.GatherControlBones(gatherType));
			AddToDictionary(armRight.GatherControlBones(gatherType));
			AddToDictionary(legLeft.GatherControlBones(gatherType));
			AddToDictionary(legRight.GatherControlBones(gatherType));

			return bones;

			void AddToDictionary(Dictionary<ControlBoneType, ControlBone> bonesToAdd)
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
			this.root.AssignTransform(animator.GetBoneTransform(HumanBodyBones.Hips).parent);
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
			root = new Bone(false, BoneType.Root);

			head = new Head();
			body = new Body();
			armLeft = new Arm(true);
			armRight = new Arm(false);
			legLeft = new Leg(true);
			legRight = new Leg(false);
		}
	}
}