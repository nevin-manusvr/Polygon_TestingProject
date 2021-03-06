﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hermes.Protocol.Polygon;

namespace Manus.Polygon.Skeleton
{
	[System.Serializable]
	public class Finger
	{
		public Bone proximal;
		public Bone middle;
		public Bone distal;
		public Bone tip;

		public bool IsValid
		{
			get { return proximal.bone && middle.bone && distal.bone; }
		}

		public Finger(bool left, int index)
		{
			string enumName = left ? "Left" : "Right";

			if (index == 0) enumName += "Thumb";
			else if (index == 1) enumName += "Index";
			else if (index == 2) enumName += "Middle";
			else if (index == 3) enumName += "Ring";
			else if (index == 4) enumName += "Pinky";

			proximal = new Bone(true, (BoneType)System.Enum.Parse(typeof(BoneType), enumName + "Proximal"));
			middle = new Bone(true, (BoneType)System.Enum.Parse(typeof(BoneType), enumName + "Middle"));
			distal = new Bone(true, (BoneType)System.Enum.Parse(typeof(BoneType), enumName + "Distal"));
			tip = new Bone(true, (BoneType)System.Enum.Parse(typeof(BoneType), enumName + "Tip"));
		}

		public Dictionary<BoneType, Bone> GatherBones(GatherType gatherType)
		{
			var bones = new Dictionary<BoneType, Bone>();

			switch (gatherType)
			{
				case GatherType.All:
					if (proximal.bone) bones.Add(proximal.type, proximal);
					if (middle.bone) bones.Add(middle.type, middle);
					if (distal.bone) bones.Add(distal.type, distal);
					if (tip.bone) bones.Add(tip.type, tip);

					break;
				case GatherType.Retargeted:
					break;

				case GatherType.Networked:
					break;
			}

			return bones;
		}

		public void AssignBones(Transform proximal, Transform middle, Transform distal, Transform tip)
		{
			this.proximal.AssignTransform(proximal);
			this.middle.AssignTransform(middle);
			this.distal.AssignTransform(distal);
			this.tip.AssignTransform(tip);
		}
	}

	[System.Serializable]
	public class HandBoneReferences
	{
		private bool left;

		public Bone wrist;

		public Finger index;
		public Finger middle;
		public Finger ring;
		public Finger pinky;
		public Finger thumb;

		public bool IsValid
		{
			get { return wrist?.bone && index.IsValid && middle.IsValid && ring.IsValid && pinky.IsValid && thumb.IsValid; }
		}

		public HandBoneReferences(bool left)
		{
			this.left = left;
			ClearBoneReferences();
		}

		public Dictionary<BoneType, Bone> GatherBones(GatherType gatherType)
		{
			var bones = new Dictionary<BoneType, Bone>();

			switch (gatherType)
			{
				case GatherType.All:
					bones.Add(wrist.type, wrist);
					AddToDictionary(index.GatherBones(gatherType));
					AddToDictionary(middle.GatherBones(gatherType));
					AddToDictionary(ring.GatherBones(gatherType));
					AddToDictionary(pinky.GatherBones(gatherType));
					AddToDictionary(thumb.GatherBones(gatherType));
					break;

				case GatherType.Retargeted:
					bones.Add(wrist.type, wrist);
					AddToDictionary(index.GatherBones(gatherType));
					AddToDictionary(middle.GatherBones(gatherType));
					AddToDictionary(ring.GatherBones(gatherType));
					AddToDictionary(pinky.GatherBones(gatherType));
					AddToDictionary(thumb.GatherBones(gatherType));
					break;

				case GatherType.Networked:
					bones.Add(wrist.type, wrist);
					AddToDictionary(index.GatherBones(gatherType));
					AddToDictionary(middle.GatherBones(gatherType));
					AddToDictionary(ring.GatherBones(gatherType));
					AddToDictionary(pinky.GatherBones(gatherType));
					AddToDictionary(thumb.GatherBones(gatherType));
					break;
			}

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

		public void PopulateBones(Transform lowerArm)
		{
			PopulateBones(null, lowerArm, true);
		}

		public void PopulateBones(Animator animator, Transform lowerArm, bool left)
		{
			bool isLeft = left;

			wrist.AssignTransform(
				animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand)
				?? Utils.FindDeepChildTransform(lowerArm, new string[] { "wrist" })
				?? Utils.FindDeepChildTransform(lowerArm, new string[] { "hand" })
				?? lowerArm.childCount > 0 ? lowerArm.GetChild(0) : null);

			thumb.AssignBones(
				animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftThumbProximal : HumanBodyBones.RightThumbProximal)
				?? Utils.FindDeepChildTransform(lowerArm, new string[] { "thumb", "1" }),
				animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftThumbIntermediate : HumanBodyBones.RightThumbIntermediate)
				?? Utils.FindDeepChildTransform(lowerArm, new string[] { "thumb", "2" }),
				animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftThumbDistal : HumanBodyBones.RightThumbDistal)
				?? Utils.FindDeepChildTransform(lowerArm, new string[] { "thumb", "3" }),
				animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftThumbDistal : HumanBodyBones.RightThumbDistal)?.childCount > 0
					? animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftThumbDistal : HumanBodyBones.RightThumbDistal).GetChild(0)
					: Utils.FindDeepChildTransform(lowerArm, new string[] { "thumb", "4" }));
			index.AssignBones(
				animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftIndexProximal : HumanBodyBones.RightIndexProximal)
				?? Utils.FindDeepChildTransform(lowerArm, new string[] { "index", "1" }),
				animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftIndexIntermediate : HumanBodyBones.RightIndexIntermediate)
				?? Utils.FindDeepChildTransform(lowerArm, new string[] { "index", "2" }),
				animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftIndexDistal : HumanBodyBones.RightIndexDistal)
				?? Utils.FindDeepChildTransform(lowerArm, new string[] { "index", "3" }),
				animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftIndexDistal : HumanBodyBones.RightIndexDistal)?.childCount > 0
					? animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftIndexDistal : HumanBodyBones.RightIndexDistal).GetChild(0)
					: Utils.FindDeepChildTransform(lowerArm, new string[] { "index", "4" }));
			middle.AssignBones(
				animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftMiddleProximal : HumanBodyBones.RightMiddleProximal)
				?? Utils.FindDeepChildTransform(lowerArm, new string[] { "middle", "1" }),
				animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftMiddleIntermediate : HumanBodyBones.RightMiddleIntermediate)
				?? Utils.FindDeepChildTransform(lowerArm, new string[] { "middle", "2" }),
				animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftMiddleDistal : HumanBodyBones.RightMiddleDistal)
				?? Utils.FindDeepChildTransform(lowerArm, new string[] { "middle", "3" }),
				animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftMiddleDistal : HumanBodyBones.RightMiddleDistal)?.childCount > 0
					? animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftMiddleDistal : HumanBodyBones.RightMiddleDistal).GetChild(0)
					: Utils.FindDeepChildTransform(lowerArm, new string[] { "middle", "4" }));
			ring.AssignBones(
				animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftRingProximal : HumanBodyBones.RightRingProximal)
				?? Utils.FindDeepChildTransform(lowerArm, new string[] { "ring", "1" }),
				animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftRingIntermediate : HumanBodyBones.RightRingIntermediate)
				?? Utils.FindDeepChildTransform(lowerArm, new string[] { "ring", "2" }),
				animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftRingDistal : HumanBodyBones.RightRingDistal)
				?? Utils.FindDeepChildTransform(lowerArm, new string[] { "ring", "3" }),
				animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftRingDistal : HumanBodyBones.RightRingDistal)?.childCount > 0
					? animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftRingDistal : HumanBodyBones.RightRingDistal).GetChild(0)
					: Utils.FindDeepChildTransform(lowerArm, new string[] { "ring", "4" }));
			pinky.AssignBones(
				animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftLittleProximal : HumanBodyBones.RightLittleProximal)
				?? Utils.FindDeepChildTransform(lowerArm, new string[] { "pinky", "1" }),
				animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftLittleIntermediate : HumanBodyBones.RightLittleIntermediate)
				?? Utils.FindDeepChildTransform(lowerArm, new string[] { "pinky", "2" }),
				animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftLittleDistal : HumanBodyBones.RightLittleDistal)
				?? Utils.FindDeepChildTransform(lowerArm, new string[] { "pinky", "3" }),
				animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftLittleDistal : HumanBodyBones.RightLittleDistal)?.childCount > 0
					? animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftLittleDistal : HumanBodyBones.RightLittleDistal).GetChild(0)
					: Utils.FindDeepChildTransform(lowerArm, new string[] { "pinky", "4" }));

			//if (!index.IsValid || !middle.IsValid || !ring.IsValid || !pinky.IsValid || !thumb.IsValid)
			//{
			//	Debug.LogWarning("Not all finger bones found. Please assign missing bones manually");
			//}
		}

		public void ClearBoneReferences()
		{
			wrist = new Bone(false, left ? BoneType.LeftHand : BoneType.RightHand);
			thumb = new Finger(left, 0);
			index = new Finger(left, 1);
			middle = new Finger(left, 2);
			ring = new Finger(left, 3);
			pinky = new Finger(left, 4);
		}
	}
}

