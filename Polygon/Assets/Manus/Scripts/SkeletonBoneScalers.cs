using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Polygon
{
	using Manus.Polygon.Skeleton;

	public class SkeletonBoneScalers
	{
		public Dictionary<HumanBodyBones, BoneScaler> boneScalers;

		public SkeletonBoneScalers()
		{
			boneScalers = new Dictionary<HumanBodyBones, BoneScaler>();
		}

		#region Helper Functions

		public void ChangeHeight(float length)
		{

		}

		public void ChangeThickness(float thickness)
		{
			boneScalers[HumanBodyBones.Hips].ScaleBone(thickness, ScaleAxis.Height, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.Spine].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.Chest].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.UpperChest].ScaleBone(thickness, ScaleAxis.Height, ScaleMode.Percentage);

			boneScalers[HumanBodyBones.Neck].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);

			boneScalers[HumanBodyBones.LeftShoulder].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftUpperArm].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftLowerArm].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);

			boneScalers[HumanBodyBones.RightShoulder].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightUpperArm].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightLowerArm].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);

			boneScalers[HumanBodyBones.LeftUpperLeg].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftLowerLeg].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);

			boneScalers[HumanBodyBones.RightUpperLeg].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightLowerLeg].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
		}

		public void ChangeSpineLength(float length)
		{
			boneScalers[HumanBodyBones.Spine].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.Chest].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.UpperChest].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
		}

		public void ChangeHeadSize(float scale)
		{
			boneScalers[HumanBodyBones.Head].ScaleBone(scale, ScaleAxis.All, ScaleMode.Percentage);
		}

		public void ChangeArmLength(float upperArmLength, float lowerArmLength)
		{
			boneScalers[HumanBodyBones.LeftUpperArm].ScaleBone(upperArmLength, ScaleAxis.Length, ScaleMode.Length);
			boneScalers[HumanBodyBones.RightUpperArm].ScaleBone(upperArmLength, ScaleAxis.Length, ScaleMode.Length);

			boneScalers[HumanBodyBones.LeftLowerArm].ScaleBone(lowerArmLength, ScaleAxis.Length, ScaleMode.Length);
			boneScalers[HumanBodyBones.RightLowerArm].ScaleBone(lowerArmLength, ScaleAxis.Length, ScaleMode.Length);
		}

		public void ChangeLegLength(float upperLegLength, float lowerLegLength)
		{
			boneScalers[HumanBodyBones.LeftUpperLeg].ScaleBone(upperLegLength, ScaleAxis.Length, ScaleMode.Length);
			boneScalers[HumanBodyBones.RightUpperLeg].ScaleBone(upperLegLength, ScaleAxis.Length, ScaleMode.Length);

			boneScalers[HumanBodyBones.LeftLowerLeg].ScaleBone(lowerLegLength, ScaleAxis.Length, ScaleMode.Length);
			boneScalers[HumanBodyBones.RightLowerLeg].ScaleBone(lowerLegLength, ScaleAxis.Length, ScaleMode.Length);
		}

		public void ChangeFootSize(float scale)
		{
			boneScalers[HumanBodyBones.LeftFoot].ScaleBone(scale, ScaleAxis.All, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightFoot].ScaleBone(scale, ScaleAxis.All, ScaleMode.Percentage);
		}

		public void ChangeHandSize(float scale)
		{
			boneScalers[HumanBodyBones.LeftHand].ScaleBone(scale, ScaleAxis.All, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightHand].ScaleBone(scale, ScaleAxis.All, ScaleMode.Percentage);
		}

		public void ChangeFingerThickness(float thickness)
		{
			boneScalers[HumanBodyBones.LeftIndexProximal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftIndexIntermediate].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftIndexDistal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftMiddleProximal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftMiddleIntermediate].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftMiddleDistal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftRingProximal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftRingIntermediate].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftRingDistal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftLittleProximal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftLittleIntermediate].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftLittleDistal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftThumbProximal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftThumbIntermediate].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftThumbDistal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);

			boneScalers[HumanBodyBones.RightIndexProximal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightIndexIntermediate].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightIndexDistal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightMiddleProximal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightMiddleIntermediate].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightMiddleDistal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightRingProximal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightRingIntermediate].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightRingDistal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightLittleProximal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightLittleIntermediate].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightLittleDistal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightThumbProximal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightThumbIntermediate].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightThumbDistal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
		}

		public void ChangeFingerLength(float length)
		{
			boneScalers[HumanBodyBones.LeftIndexProximal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftIndexIntermediate].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftIndexDistal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftMiddleProximal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftMiddleIntermediate].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftMiddleDistal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftRingProximal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftRingIntermediate].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftRingDistal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftLittleProximal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftLittleIntermediate].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftLittleDistal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftThumbProximal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftThumbIntermediate].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.LeftThumbDistal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);

			boneScalers[HumanBodyBones.RightIndexProximal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightIndexIntermediate].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightIndexDistal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightMiddleProximal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightMiddleIntermediate].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightMiddleDistal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightRingProximal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightRingIntermediate].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightRingDistal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightLittleProximal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightLittleIntermediate].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightLittleDistal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightThumbProximal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightThumbIntermediate].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			boneScalers[HumanBodyBones.RightThumbDistal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
		}

		#endregion

		public void GenerateScalerBonesForBody(SkeletonBoneReferences bones, SkeletonBoneReferences boneRotations)
		{
			//AddScalerBone(HumanBodyBones.Hips, bones.body.hip.bone, boneRotations.body.hip.bone.rotation, new[] { boneRotations.legLeft.upperLeg.bone, boneRotations.legRight.upperLeg.bone, boneRotations.body.spine[0].bone });
			//AddScalerBone(HumanBodyBones.Spine, bones.body.spine[0].bone, boneRotations.body.spine[0].bone.rotation, (bones.body.spine[1]?.bone != null) ? new [] { boneRotations.body.spine[1].bone } : new [] { boneRotations.head.neck.bone, boneRotations.armLeft.shoulder.bone, boneRotations.armRight.shoulder.bone });
			//if (bones.body.spine[1]?.bone != null) AddScalerBone(HumanBodyBones.Chest, bones.body.spine[1].bone, boneRotations.body.spine[1].bone.rotation, (bones.body.spine[2]?.bone != null) ? new[] { boneRotations.body.spine[2].bone } : new[] { boneRotations.head.neck.bone, boneRotations.armLeft.shoulder.bone, boneRotations.armRight.shoulder.bone });
			//if (bones.body.spine[2]?.bone != null) AddScalerBone(HumanBodyBones.UpperChest, bones.body.spine[2].bone, boneRotations.body.spine[2].bone.rotation, new[] { boneRotations.head.neck.bone, boneRotations.armLeft.shoulder.bone, boneRotations.armRight.shoulder.bone });

			AddScalerBone(HumanBodyBones.Neck, bones.head.neck.bone, boneRotations.head.neck.bone.rotation, new[] { boneRotations.head.head.bone });
			AddScalerBone(HumanBodyBones.Head, bones.head.head.bone, boneRotations.head.head.bone.rotation);

			// Arms
			AddScalerBone(HumanBodyBones.LeftShoulder, bones.armLeft.shoulder.bone, boneRotations.armLeft.shoulder.bone.rotation, new[] { boneRotations.armLeft.upperArm.bone });
			AddScalerBone(HumanBodyBones.LeftUpperArm, bones.armLeft.upperArm.bone, boneRotations.armLeft.upperArm.bone.rotation, new[] { boneRotations.armLeft.lowerArm.bone });
			AddScalerBone(HumanBodyBones.LeftLowerArm, bones.armLeft.lowerArm.bone, boneRotations.armLeft.lowerArm.bone.rotation, new[] { boneRotations.armLeft.hand.wrist.bone });

			AddScalerBone(HumanBodyBones.RightShoulder, bones.armRight.shoulder.bone, boneRotations.armRight.shoulder.bone.rotation, new[] { boneRotations.armRight.upperArm.bone });
			AddScalerBone(HumanBodyBones.RightUpperArm, bones.armRight.upperArm.bone, boneRotations.armRight.upperArm.bone.rotation, new[] { boneRotations.armRight.lowerArm.bone });
			AddScalerBone(HumanBodyBones.RightLowerArm, bones.armRight.lowerArm.bone, boneRotations.armRight.lowerArm.bone.rotation, new[] { boneRotations.armRight.hand.wrist.bone });

			// Legs
			AddScalerBone(HumanBodyBones.LeftUpperLeg, bones.legLeft.upperLeg.bone, boneRotations.legLeft.upperLeg.bone.rotation, new[] { boneRotations.legLeft.lowerLeg.bone });
			AddScalerBone(HumanBodyBones.LeftLowerLeg, bones.legLeft.lowerLeg.bone, boneRotations.legLeft.lowerLeg.bone.rotation, new[] { boneRotations.legLeft.foot.bone });
			AddScalerBone(HumanBodyBones.LeftFoot, bones.legLeft.foot.bone, boneRotations.legLeft.foot.bone.rotation);
			//if (bones.legLeft.toes?.bone != null) AddScalerBone(HumanBodyBones.LeftToes, bones.legLeft.toes.bone, boneRotations.legLeft.toes.bone.rotation);

			AddScalerBone(HumanBodyBones.RightUpperLeg, bones.legRight.upperLeg.bone, boneRotations.legRight.upperLeg.bone.rotation, new[] { boneRotations.legRight.lowerLeg.bone });
			AddScalerBone(HumanBodyBones.RightLowerLeg, bones.legRight.lowerLeg.bone, boneRotations.legRight.lowerLeg.bone.rotation, new[] { boneRotations.legRight.foot.bone });
			AddScalerBone(HumanBodyBones.RightFoot, bones.legRight.foot.bone, boneRotations.legRight.foot.bone.rotation);
			//if (bones.legRight.toes?.bone != null) AddScalerBone(HumanBodyBones.RightToes, bones.legRight.toes.bone, boneRotations.legRight.toes.bone.rotation);

			// Hands
			AddScalerBone(HumanBodyBones.LeftHand, bones.armLeft.hand.wrist.bone, boneRotations.armLeft.hand.wrist.bone.rotation);
			//AddScalerBone(HumanBodyBones.LeftIndexProximal, bones.armLeft.hand.index.proximal.bone, boneRotations.armLeft.hand.index.proximal.bone.rotation);
			//AddScalerBone(HumanBodyBones.LeftIndexIntermediate, bones.armLeft.hand.index.middle.bone, boneRotations.armLeft.hand.index.middle.bone.rotation);
			//AddScalerBone(HumanBodyBones.LeftIndexDistal, bones.armLeft.hand.index.distal.bone, boneRotations.armLeft.hand.index.distal.bone.rotation);
			//AddScalerBone(HumanBodyBones.LeftMiddleProximal, bones.armLeft.hand.middle.proximal.bone, boneRotations.armLeft.hand.middle.proximal.bone.rotation);
			//AddScalerBone(HumanBodyBones.LeftMiddleIntermediate, bones.armLeft.hand.middle.middle.bone, boneRotations.armLeft.hand.middle.middle.bone.rotation);
			//AddScalerBone(HumanBodyBones.LeftMiddleDistal, bones.armLeft.hand.middle.distal.bone, boneRotations.armLeft.hand.middle.distal.bone.rotation);
			//AddScalerBone(HumanBodyBones.LeftRingProximal, bones.armLeft.hand.ring.proximal.bone, boneRotations.armLeft.hand.ring.proximal.bone.rotation);
			//AddScalerBone(HumanBodyBones.LeftRingIntermediate, bones.armLeft.hand.ring.middle.bone, boneRotations.armLeft.hand.ring.middle.bone.rotation);
			//AddScalerBone(HumanBodyBones.LeftRingDistal, bones.armLeft.hand.ring.distal.bone, boneRotations.armLeft.hand.ring.distal.bone.rotation);
			//AddScalerBone(HumanBodyBones.LeftLittleProximal, bones.armLeft.hand.pinky.proximal.bone, boneRotations.armLeft.hand.pinky.proximal.bone.rotation);
			//AddScalerBone(HumanBodyBones.LeftLittleIntermediate, bones.armLeft.hand.pinky.middle.bone, boneRotations.armLeft.hand.pinky.middle.bone.rotation);
			//AddScalerBone(HumanBodyBones.LeftLittleDistal, bones.armLeft.hand.pinky.distal.bone, boneRotations.armLeft.hand.pinky.distal.bone.rotation);
			//AddScalerBone(HumanBodyBones.LeftThumbProximal, bones.armLeft.hand.thumb.proximal.bone, boneRotations.armLeft.hand.thumb.proximal.bone.rotation);
			//AddScalerBone(HumanBodyBones.LeftThumbIntermediate, bones.armLeft.hand.thumb.middle.bone, boneRotations.armLeft.hand.thumb.middle.bone.rotation);
			//AddScalerBone(HumanBodyBones.LeftThumbDistal, bones.armLeft.hand.thumb.distal.bone, boneRotations.armLeft.hand.thumb.distal.bone.rotation);

			AddScalerBone(HumanBodyBones.RightHand, bones.armRight.hand.wrist.bone, boneRotations.armRight.hand.wrist.bone.rotation);
			//AddScalerBone(HumanBodyBones.RightIndexProximal, bones.armRight.hand.index.proximal.bone, boneRotations.armRight.hand.index.proximal.bone.rotation);
			//AddScalerBone(HumanBodyBones.RightIndexIntermediate, bones.armRight.hand.index.middle.bone, boneRotations.armRight.hand.index.middle.bone.rotation);
			//AddScalerBone(HumanBodyBones.RightIndexDistal, bones.armRight.hand.index.distal.bone, boneRotations.armRight.hand.index.distal.bone.rotation);
			//AddScalerBone(HumanBodyBones.RightMiddleProximal, bones.armRight.hand.middle.proximal.bone, boneRotations.armRight.hand.middle.proximal.bone.rotation);
			//AddScalerBone(HumanBodyBones.RightMiddleIntermediate, bones.armRight.hand.middle.middle.bone, boneRotations.armRight.hand.middle.middle.bone.rotation);
			//AddScalerBone(HumanBodyBones.RightMiddleDistal, bones.armRight.hand.middle.distal.bone, boneRotations.armRight.hand.middle.distal.bone.rotation);
			//AddScalerBone(HumanBodyBones.RightRingProximal, bones.armRight.hand.ring.proximal.bone, boneRotations.armRight.hand.ring.proximal.bone.rotation);
			//AddScalerBone(HumanBodyBones.RightRingIntermediate, bones.armRight.hand.ring.middle.bone, boneRotations.armRight.hand.ring.middle.bone.rotation);
			//AddScalerBone(HumanBodyBones.RightRingDistal, bones.armRight.hand.ring.distal.bone, boneRotations.armRight.hand.ring.distal.bone.rotation);
			//AddScalerBone(HumanBodyBones.RightLittleProximal, bones.armRight.hand.pinky.proximal.bone, boneRotations.armRight.hand.pinky.proximal.bone.rotation);
			//AddScalerBone(HumanBodyBones.RightLittleIntermediate, bones.armRight.hand.pinky.middle.bone, boneRotations.armRight.hand.pinky.middle.bone.rotation);
			//AddScalerBone(HumanBodyBones.RightLittleDistal, bones.armRight.hand.pinky.distal.bone, boneRotations.armRight.hand.pinky.distal.bone.rotation);
			//AddScalerBone(HumanBodyBones.RightThumbProximal, bones.armRight.hand.thumb.proximal.bone, boneRotations.armRight.hand.thumb.proximal.bone.rotation);
			//AddScalerBone(HumanBodyBones.RightThumbIntermediate, bones.armRight.hand.thumb.middle.bone, boneRotations.armRight.hand.thumb.middle.bone.rotation);
			//AddScalerBone(HumanBodyBones.RightThumbDistal, bones.armRight.hand.thumb.distal.bone, boneRotations.armRight.hand.thumb.distal.bone.rotation);
		}

		public void AddScalerBone(HumanBodyBones humanBone, Transform bone, Quaternion boneLookRotation, Transform[] childBones = null)
		{
			if (!boneScalers.ContainsKey(humanBone))
			{
				boneScalers.Add(humanBone, new BoneScaler(bone, boneLookRotation, childBones));
			}
		}
	}
}

