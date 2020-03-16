using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Polygon
{
	using Hermes.Protocol.Polygon;
	using Manus.Polygon.Skeleton;

	public class SkeletonBoneScalers
	{
		public Dictionary<BoneType, BoneScaler> boneScalers;

		public SkeletonBoneScalers()
		{
			boneScalers = new Dictionary<BoneType, BoneScaler>();
		}

		#region Helper Functions

		public void ChangeHeight(float length)
		{

		}

		public void ChangeThickness(float thickness)
		{
			boneScalers[BoneType.Hips].ScaleBone(thickness, ScaleAxis.Height, ScaleMode.Percentage);
			boneScalers[BoneType.Spine].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.Chest)) boneScalers[BoneType.Chest].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.UpperChest)) boneScalers[BoneType.UpperChest].ScaleBone(thickness, ScaleAxis.Height, ScaleMode.Percentage);

			boneScalers[BoneType.Neck].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);

			boneScalers[BoneType.LeftShoulder].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[BoneType.LeftUpperArm].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[BoneType.LeftLowerArm].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);

			boneScalers[BoneType.RightShoulder].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[BoneType.RightUpperArm].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[BoneType.RightLowerArm].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);

			boneScalers[BoneType.LeftUpperLeg].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[BoneType.LeftLowerLeg].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);

			boneScalers[BoneType.RightUpperLeg].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			boneScalers[BoneType.RightLowerLeg].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
		}

		public void ChangeSpineLength(float length, ScaleMode mode)
		{
			float totalDefaultLength = boneScalers[BoneType.Spine].DefaultLength;
			//if (boneScalers.ContainsKey(BoneType.Chest)) totalDefaultLength 


				boneScalers[BoneType.Spine].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.Chest)) boneScalers[BoneType.Chest].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.UpperChest)) boneScalers[BoneType.UpperChest].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
		}

		public void ChangeHeadSize(float scale)
		{
			boneScalers[BoneType.Head].ScaleBone(scale, ScaleAxis.All, ScaleMode.Percentage);
		}

		public void ChangeArmLength(float upperArmLength, float lowerArmLength, ScaleMode mode)
		{
			boneScalers[BoneType.LeftUpperArm].ScaleBone(upperArmLength, ScaleAxis.Length, mode);
			boneScalers[BoneType.RightUpperArm].ScaleBone(upperArmLength, ScaleAxis.Length, mode);

			boneScalers[BoneType.LeftLowerArm].ScaleBone(lowerArmLength, ScaleAxis.Length, mode);
			boneScalers[BoneType.RightLowerArm].ScaleBone(lowerArmLength, ScaleAxis.Length, mode);
		}

		public void ChangeLegLength(float upperLegLength, float lowerLegLength, ScaleMode mode)
		{
			boneScalers[BoneType.LeftUpperLeg].ScaleBone(upperLegLength, ScaleAxis.Length, mode);
			boneScalers[BoneType.RightUpperLeg].ScaleBone(upperLegLength, ScaleAxis.Length, mode);

			boneScalers[BoneType.LeftLowerLeg].ScaleBone(lowerLegLength, ScaleAxis.Length, mode);
			boneScalers[BoneType.RightLowerLeg].ScaleBone(lowerLegLength, ScaleAxis.Length, mode);
		}

		public void ChangeFootSize(float scale)
		{
			boneScalers[BoneType.LeftFoot].ScaleBone(scale, ScaleAxis.All, ScaleMode.Percentage);
			boneScalers[BoneType.RightFoot].ScaleBone(scale, ScaleAxis.All, ScaleMode.Percentage);
		}

		public void ChangeHandSize(float scale)
		{
			boneScalers[BoneType.LeftHand].ScaleBone(scale, ScaleAxis.All, ScaleMode.Percentage);
			boneScalers[BoneType.RightHand].ScaleBone(scale, ScaleAxis.All, ScaleMode.Percentage);
		}

		public void ChangeFingerThickness(float thickness)
		{
			if (boneScalers.ContainsKey(BoneType.LeftIndexProximal)) boneScalers[BoneType.LeftIndexProximal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.LeftIndexMiddle)) boneScalers[BoneType.LeftIndexMiddle].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.LeftIndexDistal)) boneScalers[BoneType.LeftIndexDistal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.LeftMiddleProximal)) boneScalers[BoneType.LeftMiddleProximal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.LeftMiddleMiddle)) boneScalers[BoneType.LeftMiddleMiddle].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.LeftMiddleDistal)) boneScalers[BoneType.LeftMiddleDistal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.LeftRingProximal)) boneScalers[BoneType.LeftRingProximal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.LeftRingMiddle)) boneScalers[BoneType.LeftRingMiddle].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.LeftRingDistal)) boneScalers[BoneType.LeftRingDistal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.LeftPinkyProximal)) boneScalers[BoneType.LeftPinkyProximal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.LeftPinkyMiddle)) boneScalers[BoneType.LeftPinkyMiddle].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.LeftPinkyDistal)) boneScalers[BoneType.LeftPinkyDistal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.LeftThumbProximal)) boneScalers[BoneType.LeftThumbProximal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.LeftThumbMiddle)) boneScalers[BoneType.LeftThumbMiddle].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.LeftThumbDistal)) boneScalers[BoneType.LeftThumbDistal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);

			if (boneScalers.ContainsKey(BoneType.RightIndexProximal)) boneScalers[BoneType.RightIndexProximal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.RightIndexMiddle)) boneScalers[BoneType.RightIndexMiddle].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.RightIndexDistal)) boneScalers[BoneType.RightIndexDistal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.RightMiddleProximal)) boneScalers[BoneType.RightMiddleProximal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.RightMiddleMiddle)) boneScalers[BoneType.RightMiddleMiddle].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.RightMiddleDistal)) boneScalers[BoneType.RightMiddleDistal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.RightRingProximal)) boneScalers[BoneType.RightRingProximal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.RightRingMiddle)) boneScalers[BoneType.RightRingMiddle].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.RightRingDistal)) boneScalers[BoneType.RightRingDistal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.RightPinkyProximal)) boneScalers[BoneType.RightPinkyProximal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.RightPinkyMiddle)) boneScalers[BoneType.RightPinkyMiddle].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.RightPinkyDistal)) boneScalers[BoneType.RightPinkyDistal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.RightThumbProximal)) boneScalers[BoneType.RightThumbProximal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.RightThumbMiddle)) boneScalers[BoneType.RightThumbMiddle].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
			if (boneScalers.ContainsKey(BoneType.RightThumbDistal)) boneScalers[BoneType.RightThumbDistal].ScaleBone(thickness, ScaleAxis.Thickness, ScaleMode.Percentage);
		}

		public void ChangeFingerLength(float length)
		{
			//boneScalers[HumanBodyBones.LeftIndexProximal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.LeftIndexIntermediate].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.LeftIndexDistal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.LeftMiddleProximal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.LeftMiddleIntermediate].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.LeftMiddleDistal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.LeftRingProximal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.LeftRingIntermediate].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.LeftRingDistal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.LeftLittleProximal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.LeftLittleIntermediate].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.LeftLittleDistal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.LeftThumbProximal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.LeftThumbIntermediate].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.LeftThumbDistal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);

			//boneScalers[HumanBodyBones.RightIndexProximal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.RightIndexIntermediate].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.RightIndexDistal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.RightMiddleProximal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.RightMiddleIntermediate].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.RightMiddleDistal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.RightRingProximal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.RightRingIntermediate].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.RightRingDistal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.RightLittleProximal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.RightLittleIntermediate].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.RightLittleDistal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.RightThumbProximal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.RightThumbIntermediate].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
			//boneScalers[HumanBodyBones.RightThumbDistal].ScaleBone(length, ScaleAxis.Length, ScaleMode.Percentage);
		}

		#endregion

		public void GenerateScalerBonesForBody(SkeletonBoneReferences bones)
		{
			AddScalerBone(BoneType.Hips, bones.body.hip.bone, bones.body.hip.bone.rotation, new[] { bones.legLeft.upperLeg.bone, bones.legRight.upperLeg.bone, bones.body.spine.bone });
			AddScalerBone(BoneType.Spine, bones.body.spine.bone, bones.body.spine.bone.rotation, bones.body.chest.bone != null ? new[] { bones.body.chest.bone } : new [] { bones.head.neck.bone, bones.armLeft.shoulder.bone, bones.armRight.shoulder.bone });
			if (bones.body.chest.bone != null) AddScalerBone(BoneType.Chest, bones.body.chest.bone, bones.body.chest.bone.rotation, (bones.body.upperChest.bone != null) ? new[] { bones.body.upperChest.bone } : new[] { bones.head.neck.bone, bones.armLeft.shoulder.bone, bones.armRight.shoulder.bone });
			if (bones.body.upperChest.bone != null) AddScalerBone(BoneType.UpperChest, bones.body.upperChest.bone, bones.body.upperChest.bone.rotation, new[] { bones.head.neck.bone, bones.armLeft.shoulder.bone, bones.armRight.shoulder.bone });

			AddScalerBone(BoneType.Neck, bones.head.neck.bone, bones.head.neck.bone.rotation, new[] { bones.head.head.bone });
			AddScalerBone(BoneType.Head, bones.head.head.bone, bones.head.head.bone.rotation);

			// Arms
			AddScalerBone(BoneType.LeftShoulder, bones.armLeft.shoulder.bone, bones.armLeft.shoulder.bone.rotation, new[] { bones.armLeft.upperArm.bone });
			AddScalerBone(BoneType.LeftUpperArm, bones.armLeft.upperArm.bone, bones.armLeft.upperArm.bone.rotation, new[] { bones.armLeft.lowerArm.bone });
			AddScalerBone(BoneType.LeftLowerArm, bones.armLeft.lowerArm.bone, bones.armLeft.lowerArm.bone.rotation, new[] { bones.armLeft.hand.wrist.bone });

			AddScalerBone(BoneType.RightShoulder, bones.armRight.shoulder.bone, bones.armRight.shoulder.bone.rotation, new[] { bones.armRight.upperArm.bone });
			AddScalerBone(BoneType.RightUpperArm, bones.armRight.upperArm.bone, bones.armRight.upperArm.bone.rotation, new[] { bones.armRight.lowerArm.bone });
			AddScalerBone(BoneType.RightLowerArm, bones.armRight.lowerArm.bone, bones.armRight.lowerArm.bone.rotation, new[] { bones.armRight.hand.wrist.bone });

			// Legs
			AddScalerBone(BoneType.LeftUpperLeg, bones.legLeft.upperLeg.bone, bones.legLeft.upperLeg.bone.rotation, new[] { bones.legLeft.lowerLeg.bone });
			AddScalerBone(BoneType.LeftLowerLeg, bones.legLeft.lowerLeg.bone, bones.legLeft.lowerLeg.bone.rotation, new[] { bones.legLeft.foot.bone });
			AddScalerBone(BoneType.LeftFoot, bones.legLeft.foot.bone, bones.legLeft.foot.bone.rotation);
			//if (bones.legLeft.toes?.bone != null) AddScalerBone(BoneType.LeftToes, bones.legLeft.toes.bone, bones.legLeft.toes.bone.rotation);

			AddScalerBone(BoneType.RightUpperLeg, bones.legRight.upperLeg.bone, bones.legRight.upperLeg.bone.rotation, new[] { bones.legRight.lowerLeg.bone });
			AddScalerBone(BoneType.RightLowerLeg, bones.legRight.lowerLeg.bone, bones.legRight.lowerLeg.bone.rotation, new[] { bones.legRight.foot.bone });
			AddScalerBone(BoneType.RightFoot, bones.legRight.foot.bone, bones.legRight.foot.bone.rotation);
			//if (bones.legRight.toes?.bone != null) AddScalerBone(BoneType.RightToes, bones.legRight.toes.bone, bones.legRight.toes.bone.rotation);

			// Hands
			AddScalerBone(BoneType.LeftHand, bones.armLeft.hand.wrist.bone, bones.armLeft.hand.wrist.bone.rotation);

			//AddScalerBone(BoneType.LeftIndexProximal, bones.armLeft.hand.index.proximal.bone, bones.armLeft.hand.index.proximal.bone.rotation);
			//AddScalerBone(BoneType.LeftIndexMiddle, bones.armLeft.hand.index.middle.bone, bones.armLeft.hand.index.middle.bone.rotation);
			//AddScalerBone(BoneType.LeftIndexDistal, bones.armLeft.hand.index.distal.bone, bones.armLeft.hand.index.distal.bone.rotation);
			//AddScalerBone(BoneType.LeftMiddleProximal, bones.armLeft.hand.middle.proximal.bone, bones.armLeft.hand.middle.proximal.bone.rotation);
			//AddScalerBone(BoneType.LeftMiddleMiddle, bones.armLeft.hand.middle.middle.bone, bones.armLeft.hand.middle.middle.bone.rotation);
			//AddScalerBone(BoneType.LeftMiddleDistal, bones.armLeft.hand.middle.distal.bone, bones.armLeft.hand.middle.distal.bone.rotation);
			//AddScalerBone(BoneType.LeftRingProximal, bones.armLeft.hand.ring.proximal.bone, bones.armLeft.hand.ring.proximal.bone.rotation);
			//AddScalerBone(BoneType.LeftRingMiddle, bones.armLeft.hand.ring.middle.bone, bones.armLeft.hand.ring.middle.bone.rotation);
			//AddScalerBone(BoneType.LeftRingDistal, bones.armLeft.hand.ring.distal.bone, bones.armLeft.hand.ring.distal.bone.rotation);
			//AddScalerBone(BoneType.LeftPinkyProximal, bones.armLeft.hand.pinky.proximal.bone, bones.armLeft.hand.pinky.proximal.bone.rotation);
			//AddScalerBone(BoneType.LeftPinkyMiddle, bones.armLeft.hand.pinky.middle.bone, bones.armLeft.hand.pinky.middle.bone.rotation);
			//AddScalerBone(BoneType.LeftPinkyDistal, bones.armLeft.hand.pinky.distal.bone, bones.armLeft.hand.pinky.distal.bone.rotation);
			//AddScalerBone(BoneType.LeftThumbProximal, bones.armLeft.hand.thumb.proximal.bone, bones.armLeft.hand.thumb.proximal.bone.rotation);
			//AddScalerBone(BoneType.LeftThumbMiddle, bones.armLeft.hand.thumb.middle.bone, bones.armLeft.hand.thumb.middle.bone.rotation);
			//AddScalerBone(BoneType.LeftThumbDistal, bones.armLeft.hand.thumb.distal.bone, bones.armLeft.hand.thumb.distal.bone.rotation);

			AddScalerBone(BoneType.RightHand, bones.armRight.hand.wrist.bone, bones.armRight.hand.wrist.bone.rotation);
			//AddScalerBone(BoneType.RightIndexProximal, bones.armRight.hand.index.proximal.bone, bones.armRight.hand.index.proximal.bone.rotation);
			//AddScalerBone(BoneType.RightIndexMiddle, bones.armRight.hand.index.middle.bone, bones.armRight.hand.index.middle.bone.rotation);
			//AddScalerBone(BoneType.RightIndexDistal, bones.armRight.hand.index.distal.bone, bones.armRight.hand.index.distal.bone.rotation);
			//AddScalerBone(BoneType.RightMiddleProximal, bones.armRight.hand.middle.proximal.bone, bones.armRight.hand.middle.proximal.bone.rotation);
			//AddScalerBone(BoneType.RightMiddleMiddle, bones.armRight.hand.middle.middle.bone, bones.armRight.hand.middle.middle.bone.rotation);
			//AddScalerBone(BoneType.RightMiddleDistal, bones.armRight.hand.middle.distal.bone, bones.armRight.hand.middle.distal.bone.rotation);
			//AddScalerBone(BoneType.RightRingProximal, bones.armRight.hand.ring.proximal.bone, bones.armRight.hand.ring.proximal.bone.rotation);
			//AddScalerBone(BoneType.RightRingMiddle, bones.armRight.hand.ring.middle.bone, bones.armRight.hand.ring.middle.bone.rotation);
			//AddScalerBone(BoneType.RightRingDistal, bones.armRight.hand.ring.distal.bone, bones.armRight.hand.ring.distal.bone.rotation);
			//AddScalerBone(BoneType.RightPinkyProximal, bones.armRight.hand.pinky.proximal.bone, bones.armRight.hand.pinky.proximal.bone.rotation);
			//AddScalerBone(BoneType.RightPinkyMiddle, bones.armRight.hand.pinky.middle.bone, bones.armRight.hand.pinky.middle.bone.rotation);
			//AddScalerBone(BoneType.RightPinkyDistal, bones.armRight.hand.pinky.distal.bone, bones.armRight.hand.pinky.distal.bone.rotation);
			//AddScalerBone(BoneType.RightThumbProximal, bones.armRight.hand.thumb.proximal.bone, bones.armRight.hand.thumb.proximal.bone.rotation);
			//AddScalerBone(BoneType.RightThumbMiddle, bones.armRight.hand.thumb.middle.bone, bones.armRight.hand.thumb.middle.bone.rotation);
			//AddScalerBone(BoneType.RightThumbDistal, bones.armRight.hand.thumb.distal.bone, bones.armRight.hand.thumb.distal.bone.rotation);
		}

		public void AddScalerBone(BoneType humanBone, Transform bone, Quaternion boneLookRotation, Transform[] childBones = null)
		{
			if (!boneScalers.ContainsKey(humanBone))
			{
				boneScalers.Add(humanBone, new BoneScaler(bone, boneLookRotation, childBones));
			}
		}
	}
}

