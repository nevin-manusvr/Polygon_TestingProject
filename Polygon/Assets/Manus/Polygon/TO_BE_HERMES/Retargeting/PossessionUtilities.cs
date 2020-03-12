using System.Collections;
using System.Collections.Generic;
using Hermes.Protocol.Polygon;

namespace Manus.ToBeHermes
{
	using System;
	using System.Linq;

	public static class PossessionUtilities
	{
		public static readonly BoneType[] DefaultRetargetTypes =
			{ 
				// Body
				BoneType.Hips,
				BoneType.Spine,
				BoneType.Chest,
				BoneType.UpperChest,
				BoneType.Neck,
				BoneType.Head,

				// Left arm
				BoneType.LeftShoulder,
				BoneType.LeftUpperArm,
				BoneType.LeftLowerArm,
				BoneType.LeftHand,

				// Right arm
				BoneType.RightShoulder,
				BoneType.RightUpperArm,
				BoneType.RightLowerArm,
				BoneType.RightHand,

				// Left leg
				BoneType.LeftUpperLeg,
				BoneType.LeftLowerLeg,
				BoneType.LeftFoot,
				
				// Right leg
				BoneType.RightUpperLeg,
				BoneType.RightLowerLeg,
				BoneType.RightFoot,
			};

		public static int GetBonePriority(BoneType _Type)
		{
			switch (_Type)
			{
				// Main
				case BoneType.Root:
					return 0;
				case BoneType.Head:
					return 100;
				case BoneType.Neck:
					return 90;
				case BoneType.Hips:
					return 30;
				case BoneType.Spine:
				case BoneType.Chest:
				case BoneType.UpperChest:
					return 50;

				// Legs
				case BoneType.LeftUpperLeg:
				case BoneType.RightUpperLeg:
					return 50;
				case BoneType.LeftLowerLeg:
				case BoneType.RightLowerLeg:
					return 0;
				case BoneType.LeftFoot:
				case BoneType.RightFoot:
					return 70;
				case BoneType.LeftToes:
				case BoneType.RightToes:
					return 5;
				case BoneType.LeftToesEnd:
				case BoneType.RightToesEnd:
					return 2;

				// Arms
				case BoneType.LeftShoulder:
				case BoneType.RightShoulder:
					return 50;
				case BoneType.LeftUpperArm:
				case BoneType.RightUpperArm:
					return 10;
				case BoneType.LeftLowerArm:
				case BoneType.RightLowerArm:
					return 0;
				case BoneType.LeftHand:
				case BoneType.RightHand:
					return 90;

				// Hands
				case BoneType.LeftIndexProximal:
				case BoneType.LeftMiddleProximal:
				case BoneType.LeftRingProximal:
				case BoneType.LeftPinkyProximal:
				case BoneType.LeftThumbProximal:
				case BoneType.RightIndexProximal:
				case BoneType.RightMiddleProximal:
				case BoneType.RightRingProximal:
				case BoneType.RightPinkyProximal:
				case BoneType.RightThumbProximal:
					return 3;
				case BoneType.LeftIndexMiddle:
				case BoneType.LeftMiddleMiddle:
				case BoneType.LeftRingMiddle:
				case BoneType.LeftPinkyMiddle:
				case BoneType.LeftThumbMiddle:
				case BoneType.RightIndexMiddle:
				case BoneType.RightMiddleMiddle:
				case BoneType.RightRingMiddle:
				case BoneType.RightPinkyMiddle:
				case BoneType.RightThumbMiddle:
					return 5;
				case BoneType.LeftIndexDistal:
				case BoneType.LeftMiddleDistal:
				case BoneType.LeftRingDistal:
				case BoneType.LeftPinkyDistal:
				case BoneType.LeftThumbDistal:
				case BoneType.RightIndexDistal:
				case BoneType.RightMiddleDistal:
				case BoneType.RightRingDistal:
				case BoneType.RightPinkyDistal:
				case BoneType.RightThumbDistal:
					return 7;
				case BoneType.LeftIndexTip:
				case BoneType.LeftMiddleTip:
				case BoneType.LeftRingTip:
				case BoneType.LeftPinkyTip:
				case BoneType.LeftThumbTip:
				case BoneType.RightIndexTip:
				case BoneType.RightMiddleTip:
				case BoneType.RightRingTip:
				case BoneType.RightPinkyTip:
				case BoneType.RightThumbTip:
					return 10;
				default:
					return 0;

			}
			return 0;
		}

		public static BoneType GetParentType(BoneType _Type, Bone[] _Bones)
		{
			switch (_Type)
			{
				case BoneType.Root:
					return BoneType.Root;
				case BoneType.Head:
					return BoneType.Neck;
				case BoneType.Neck:

					{
						Bone t_highestBones =
							_Bones.FirstOrDefault(item => item.Type == BoneType.UpperChest)
							?? _Bones.FirstOrDefault(item => item.Type == BoneType.Chest) 
							?? _Bones.FirstOrDefault(item => item.Type == BoneType.Spine);
						return t_highestBones.Type;
					}

				case BoneType.Hips:
					return BoneType.Root;
				case BoneType.Spine:
					return BoneType.Hips;
				case BoneType.Chest:
					return BoneType.Spine;
				case BoneType.UpperChest:
					return BoneType.Chest;
				case BoneType.LeftUpperLeg:
					break;
				case BoneType.RightUpperLeg:
					break;
				case BoneType.LeftLowerLeg:
					break;
				case BoneType.RightLowerLeg:
					break;
				case BoneType.LeftFoot:
					break;
				case BoneType.RightFoot:
					break;
				case BoneType.LeftToes:
					break;
				case BoneType.RightToes:
					break;
				case BoneType.LeftToesEnd:
					break;
				case BoneType.RightToesEnd:
					break;
				case BoneType.LeftShoulder:
					break;
				case BoneType.RightShoulder:
					break;
				case BoneType.LeftUpperArm:
					break;
				case BoneType.RightUpperArm:
					break;
				case BoneType.LeftLowerArm:
					break;
				case BoneType.RightLowerArm:
					break;
				case BoneType.LeftHand:
					break;
				case BoneType.RightHand:
					break;
				case BoneType.LeftThumbProximal:
					break;
				case BoneType.LeftThumbMiddle:
					break;
				case BoneType.LeftThumbDistal:
					break;
				case BoneType.LeftThumbTip:
					break;
				case BoneType.RightThumbProximal:
					break;
				case BoneType.RightThumbMiddle:
					break;
				case BoneType.RightThumbDistal:
					break;
				case BoneType.RightThumbTip:
					break;
				case BoneType.LeftIndexProximal:
					break;
				case BoneType.LeftIndexMiddle:
					break;
				case BoneType.LeftIndexDistal:
					break;
				case BoneType.LeftIndexTip:
					break;
				case BoneType.RightIndexProximal:
					break;
				case BoneType.RightIndexMiddle:
					break;
				case BoneType.RightIndexDistal:
					break;
				case BoneType.RightIndexTip:
					break;
				case BoneType.LeftMiddleProximal:
					break;
				case BoneType.LeftMiddleMiddle:
					break;
				case BoneType.LeftMiddleDistal:
					break;
				case BoneType.LeftMiddleTip:
					break;
				case BoneType.RightMiddleProximal:
					break;
				case BoneType.RightMiddleMiddle:
					break;
				case BoneType.RightMiddleDistal:
					break;
				case BoneType.RightMiddleTip:
					break;
				case BoneType.LeftRingProximal:
					break;
				case BoneType.LeftRingMiddle:
					break;
				case BoneType.LeftRingDistal:
					break;
				case BoneType.LeftRingTip:
					break;
				case BoneType.RightRingProximal:
					break;
				case BoneType.RightRingMiddle:
					break;
				case BoneType.RightRingDistal:
					break;
				case BoneType.RightRingTip:
					break;
				case BoneType.LeftPinkyProximal:
					break;
				case BoneType.LeftPinkyMiddle:
					break;
				case BoneType.LeftPinkyDistal:
					break;
				case BoneType.LeftPinkyTip:
					break;
				case BoneType.RightPinkyProximal:
					break;
				case BoneType.RightPinkyMiddle:
					break;
				case BoneType.RightPinkyDistal:
					break;
				case BoneType.RightPinkyTip:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(_Type), _Type, null);
			}

			return BoneType.Root;
		}
	}
}

