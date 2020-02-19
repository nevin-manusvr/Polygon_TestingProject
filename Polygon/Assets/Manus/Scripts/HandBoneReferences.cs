using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.ToBeHermes.Skeleton;

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
			get { return proximal?.bone && middle?.bone && distal?.bone && tip?.bone; }
		}

		public void AssignBones(Transform proximal, Transform middle, Transform distal, Transform tip)
		{
			//this.proximal = new Bone(proximal);
			//this.middle = new Bone(middle);
			//this.distal = new Bone(distal);
			//this.tip = new Bone(tip);
		}
	}

	[System.Serializable]
	public class HandBoneReferences
	{
		public Bone wrist;

		public Finger index;
		public Finger middle;
		public Finger ring;
		public Finger pinky;
		public Finger thumb;

		public bool IsValid
		{
			get { return wrist?.bone; } // && index.IsValid && middle.IsValid && ring.IsValid && pinky.IsValid && thumb.IsValid; }
		}

		public Dictionary<BoneType, Bone> GatherBones()
		{
			var bones = new Dictionary<BoneType, Bone>();

			bones.Add(wrist.type, wrist);

			return bones;
		}

		public void PopulateBones(Transform lowerArm)
		{
			PopulateBones(null, lowerArm, true);
		}

		public void PopulateBones(Animator animator, Transform lowerArm, bool left)
		{
			bool isLeft = left;

			wrist = new Bone( isLeft ? BoneType.LeftHand : BoneType.RightHand,
				animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand)
				?? Utils.FindDeepChildTransform(lowerArm, new string[] { "wrist" })
				?? Utils.FindDeepChildTransform(lowerArm, new string[] { "hand" })
				?? lowerArm.childCount > 0 ? lowerArm.GetChild(0) : null);

			//thumb.AssignBones(
			//	animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftThumbProximal : HumanBodyBones.RightThumbProximal)
			//	?? Utils.FindDeepChildTransform(lowerArm, new string[] { "thumb", "1" }),
			//	animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftThumbIntermediate : HumanBodyBones.RightThumbIntermediate)
			//	?? Utils.FindDeepChildTransform(lowerArm, new string[] { "thumb", "2" }),
			//	animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftThumbDistal : HumanBodyBones.RightThumbDistal)
			//	?? Utils.FindDeepChildTransform(lowerArm, new string[] { "thumb", "3" }),
			//	animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftThumbDistal : HumanBodyBones.RightThumbDistal)?.childCount > 0
			//		? animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftThumbDistal : HumanBodyBones.RightThumbDistal).GetChild(0)
			//		: Utils.FindDeepChildTransform(lowerArm, new string[] { "thumb", "4" }));
			//index.AssignBones(
			//	animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftIndexProximal : HumanBodyBones.RightIndexProximal)
			//	?? Utils.FindDeepChildTransform(lowerArm, new string[] { "index", "1" }),
			//	animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftIndexIntermediate : HumanBodyBones.RightIndexIntermediate)
			//	?? Utils.FindDeepChildTransform(lowerArm, new string[] { "index", "2" }),
			//	animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftIndexDistal : HumanBodyBones.RightIndexDistal)
			//	?? Utils.FindDeepChildTransform(lowerArm, new string[] { "index", "3" }),
			//	animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftIndexDistal : HumanBodyBones.RightIndexDistal)?.childCount > 0
			//		? animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftIndexDistal : HumanBodyBones.RightIndexDistal).GetChild(0)
			//		: Utils.FindDeepChildTransform(lowerArm, new string[] { "index", "4" }));
			//middle.AssignBones(
			//	animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftMiddleProximal : HumanBodyBones.RightMiddleProximal)
			//	?? Utils.FindDeepChildTransform(lowerArm, new string[] { "middle", "1" }),
			//	animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftMiddleIntermediate : HumanBodyBones.RightMiddleIntermediate)
			//	?? Utils.FindDeepChildTransform(lowerArm, new string[] { "middle", "2" }),
			//	animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftMiddleDistal : HumanBodyBones.RightMiddleDistal)
			//	?? Utils.FindDeepChildTransform(lowerArm, new string[] { "middle", "3" }),
			//	animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftMiddleDistal : HumanBodyBones.RightMiddleDistal)?.childCount > 0
			//		? animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftMiddleDistal : HumanBodyBones.RightMiddleDistal).GetChild(0)
			//		: Utils.FindDeepChildTransform(lowerArm, new string[] { "middle", "4" }));
			//ring.AssignBones(
			//	animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftRingProximal : HumanBodyBones.RightRingProximal)
			//	?? Utils.FindDeepChildTransform(lowerArm, new string[] { "ring", "1" }),
			//	animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftRingIntermediate : HumanBodyBones.RightRingIntermediate)
			//	?? Utils.FindDeepChildTransform(lowerArm, new string[] { "ring", "2" }),
			//	animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftRingDistal : HumanBodyBones.RightRingDistal)
			//	?? Utils.FindDeepChildTransform(lowerArm, new string[] { "ring", "3" }),
			//	animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftRingDistal : HumanBodyBones.RightRingDistal)?.childCount > 0
			//		? animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftRingDistal : HumanBodyBones.RightRingDistal).GetChild(0)
			//		: Utils.FindDeepChildTransform(lowerArm, new string[] { "ring", "4" }));
			//pinky.AssignBones(
			//	animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftLittleProximal : HumanBodyBones.RightLittleProximal)
			//	?? Utils.FindDeepChildTransform(lowerArm, new string[] { "pinky", "1" }),
			//	animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftLittleIntermediate : HumanBodyBones.RightLittleIntermediate)
			//	?? Utils.FindDeepChildTransform(lowerArm, new string[] { "pinky", "2" }),
			//	animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftLittleDistal : HumanBodyBones.RightLittleDistal)
			//	?? Utils.FindDeepChildTransform(lowerArm, new string[] { "pinky", "3" }),
			//	animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftLittleDistal : HumanBodyBones.RightLittleDistal)?.childCount > 0
			//		? animator?.GetBoneTransform(isLeft ? HumanBodyBones.LeftLittleDistal : HumanBodyBones.RightLittleDistal).GetChild(0)
			//		: Utils.FindDeepChildTransform(lowerArm, new string[] { "pinky", "4" }));

			if (!index.IsValid || !middle.IsValid || !ring.IsValid || !pinky.IsValid || !thumb.IsValid)
			{
				Debug.LogWarning("Not all finger bones found. Please assign missing bones manually");
			}
		}

		public void ClearBoneReferences()
		{
			wrist = null;
			thumb = new Finger();
			index = new Finger();
			middle = new Finger();
			ring = new Finger();
			pinky = new Finger();
		}
	}
}

