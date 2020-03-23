using System.Collections;
using System.Collections.Generic;
using Hermes.Protocol.Polygon;
using System.Linq;
using GlmSharp;

namespace Manus.ToBeHermes
{
	public static class PossessionUtilities
	{
		public class Node
		{
			public BoneType parentNode;
			public BoneType[] childNodes;

			public Node(BoneType _Parent, BoneType[] _ChildNodes)
			{
				parentNode = _Parent;
				childNodes = _ChildNodes ?? new BoneType[] { };
			}
		}

		public static readonly Dictionary<BoneType, Node> DefaultBoneStructure = new Dictionary<BoneType, Node>
		 {
		     {BoneType.Root, new Node(BoneType.Root, new []{BoneType.Hips})},
		     {BoneType.Hips, new Node(BoneType.Root, new []{BoneType.LeftUpperLeg, BoneType.RightUpperLeg, BoneType.Spine})},
			 {BoneType.Spine, new Node(BoneType.Hips, new []{BoneType.Chest})},
			 {BoneType.Chest, new Node(BoneType.Spine, new []{BoneType.UpperChest})},
			 {BoneType.UpperChest, new Node(BoneType.Chest, new []{BoneType.Neck, BoneType.LeftShoulder, BoneType.RightShoulder})},
			 {BoneType.Neck, new Node(BoneType.UpperChest, new []{BoneType.Head})},
			 {BoneType.Head, new Node(BoneType.Neck, null)},
			 // Legs
		     {BoneType.LeftUpperLeg, new Node(BoneType.Hips, new []{BoneType.LeftLowerLeg})},
		     {BoneType.LeftLowerLeg, new Node(BoneType.LeftUpperLeg, new []{BoneType.LeftFoot})},
		     {BoneType.LeftFoot, new Node(BoneType.LeftLowerLeg, new []{BoneType.LeftToes})},
		     {BoneType.LeftToes, new Node(BoneType.LeftFoot, new []{BoneType.LeftToesEnd})},
		     {BoneType.LeftToesEnd, new Node(BoneType.LeftToes, null)},

		     {BoneType.RightUpperLeg, new Node(BoneType.Hips, new []{BoneType.RightLowerLeg})},
		     {BoneType.RightLowerLeg, new Node(BoneType.RightUpperLeg, new []{BoneType.RightFoot})},
		     {BoneType.RightFoot, new Node(BoneType.RightLowerLeg, new []{BoneType.RightToes})},
		     {BoneType.RightToes, new Node(BoneType.RightFoot, new []{BoneType.RightToesEnd})},
		     {BoneType.RightToesEnd, new Node(BoneType.RightToes, null)},
		     // Arms
		     {BoneType.LeftShoulder, new Node(BoneType.UpperChest, new []{BoneType.LeftUpperArm})},
		     {BoneType.LeftUpperArm, new Node(BoneType.LeftShoulder, new []{BoneType.LeftLowerArm})},
		     {BoneType.LeftLowerArm, new Node(BoneType.LeftUpperArm, new []{BoneType.LeftHand})},
		     {BoneType.LeftHand, new Node(BoneType.LeftLowerArm, new []{BoneType.LeftThumbProximal ,BoneType.LeftIndexProximal, BoneType.LeftMiddleProximal, BoneType.LeftRingProximal, BoneType.LeftPinkyProximal})},

		     {BoneType.RightShoulder, new Node(BoneType.UpperChest, new []{BoneType.RightUpperArm})},
		     {BoneType.RightUpperArm, new Node(BoneType.RightShoulder, new []{BoneType.RightLowerArm})},
		     {BoneType.RightLowerArm, new Node(BoneType.RightUpperArm, new []{BoneType.RightHand})},
		     {BoneType.RightHand, new Node(BoneType.RightLowerArm, new []{BoneType.RightThumbProximal ,BoneType.RightIndexProximal, BoneType.RightMiddleProximal, BoneType.RightRingProximal, BoneType.RightPinkyProximal})},
		     // Hands
		     //{BoneType.LeftThumbProximal, new Node(BoneType.LeftHand, new []{BoneType.LeftThumbMiddle})},
		     //{BoneType.LeftThumbMiddle, new Node(BoneType.LeftThumbProximal, new []{BoneType.LeftThumbDistal})},
		     //{BoneType.LeftThumbDistal, new Node(BoneType.LeftThumbMiddle, new []{BoneType.LeftThumbTip})},
		     //{BoneType.LeftThumbTip, new Node(BoneType.LeftThumbDistal, null)},
		     //{BoneType.LeftIndexProximal, new Node(BoneType.LeftHand, new []{BoneType.LeftIndexMiddle})},
		     //{BoneType.LeftIndexMiddle, new Node(BoneType.LeftIndexProximal, new []{BoneType.LeftIndexDistal})},
		     //{BoneType.LeftIndexDistal, new Node(BoneType.LeftIndexMiddle, new []{BoneType.LeftIndexTip})},
		     //{BoneType.LeftIndexTip, new Node(BoneType.LeftIndexDistal, null)},
		     //{BoneType.LeftMiddleProximal, new Node(BoneType.LeftHand, new []{BoneType.LeftMiddleMiddle})},
		     //{BoneType.LeftMiddleMiddle, new Node(BoneType.LeftMiddleProximal, new []{BoneType.LeftMiddleDistal})},
		     //{BoneType.LeftMiddleDistal, new Node(BoneType.LeftMiddleMiddle, new []{BoneType.LeftMiddleTip})},
		     //{BoneType.LeftMiddleTip, new Node(BoneType.LeftMiddleDistal, null)},
		     //{BoneType.LeftRingProximal, new Node(BoneType.LeftHand, new []{BoneType.LeftRingMiddle})},
		     //{BoneType.LeftRingMiddle, new Node(BoneType.LeftRingProximal, new []{BoneType.LeftRingDistal})},
		     //{BoneType.LeftRingDistal, new Node(BoneType.LeftRingMiddle, new []{BoneType.LeftRingTip})},
		     //{BoneType.LeftRingTip, new Node(BoneType.LeftRingDistal, null)},
		     //{BoneType.LeftPinkyProximal, new Node(BoneType.LeftHand, new []{BoneType.LeftPinkyMiddle})},
		     //{BoneType.LeftPinkyMiddle, new Node(BoneType.LeftPinkyProximal, new []{BoneType.LeftPinkyDistal})},
		     //{BoneType.LeftPinkyDistal, new Node(BoneType.LeftPinkyMiddle, new []{BoneType.LeftPinkyTip})},
		     //{BoneType.LeftPinkyTip, new Node(BoneType.LeftPinkyDistal, null)},

		     //{BoneType.RightThumbProximal, new Node(BoneType.RightHand, new []{BoneType.RightThumbMiddle})},
		     //{BoneType.RightThumbMiddle, new Node(BoneType.RightThumbProximal, new []{BoneType.RightThumbDistal})},
		     //{BoneType.RightThumbDistal, new Node(BoneType.RightThumbMiddle, new []{BoneType.RightThumbTip})},
		     //{BoneType.RightThumbTip, new Node(BoneType.RightThumbDistal, null)},
		     //{BoneType.RightIndexProximal, new Node(BoneType.RightHand, new []{BoneType.RightIndexMiddle})},
		     //{BoneType.RightIndexMiddle, new Node(BoneType.RightIndexProximal, new []{BoneType.RightIndexDistal})},
		     //{BoneType.RightIndexDistal, new Node(BoneType.RightIndexMiddle, new []{BoneType.RightIndexTip})},
		     //{BoneType.RightIndexTip, new Node(BoneType.RightIndexDistal, null)},
		     //{BoneType.RightMiddleProximal, new Node(BoneType.RightHand, new []{BoneType.RightMiddleMiddle})},
		     //{BoneType.RightMiddleMiddle, new Node(BoneType.RightMiddleProximal, new []{BoneType.RightMiddleDistal})},
		     //{BoneType.RightMiddleDistal, new Node(BoneType.RightMiddleMiddle, new []{BoneType.RightMiddleTip})},
		     //{BoneType.RightMiddleTip, new Node(BoneType.RightMiddleDistal, null)},
		     //{BoneType.RightRingProximal, new Node(BoneType.RightHand, new []{BoneType.RightRingMiddle})},
		     //{BoneType.RightRingMiddle, new Node(BoneType.RightRingProximal, new []{BoneType.RightRingDistal})},
		     //{BoneType.RightRingDistal, new Node(BoneType.RightRingMiddle, new []{BoneType.RightRingTip})},
		     //{BoneType.RightRingTip, new Node(BoneType.RightRingDistal, null)},
		     //{BoneType.RightPinkyProximal, new Node(BoneType.RightHand, new []{BoneType.RightPinkyMiddle})},
		     //{BoneType.RightPinkyMiddle, new Node(BoneType.RightPinkyProximal, new []{BoneType.RightPinkyDistal})},
		     //{BoneType.RightPinkyDistal, new Node(BoneType.RightPinkyMiddle, new []{BoneType.RightPinkyTip})},
		     //{BoneType.RightPinkyTip, new Node(BoneType.RightPinkyDistal, null)},

		 };

		public static readonly BoneType[] DefaultReturnTypes =
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
				BoneType.LeftToes,
				BoneType.LeftToesEnd,
				
				// Right leg
				BoneType.RightUpperLeg,
				BoneType.RightLowerLeg,
				BoneType.RightFoot,
				BoneType.RightToes,
				BoneType.RightToesEnd
			};

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
					return 40;

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
					return 80;

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

		public static Possession.Constraint GetParentForType(this Possession.Constraint[] _Constraints, BoneType _Type)
		{
			if (!DefaultBoneStructure.ContainsKey(_Type) || _Type == DefaultBoneStructure[_Type].parentNode)
				return null;

			BoneType _ParentType = DefaultBoneStructure[_Type].parentNode;
			
			Possession.Constraint t_Parent = null;

			foreach (var t_Constraint in _Constraints)
			{
				if (t_Constraint.bone.type == _ParentType)
				{
					t_Parent = t_Constraint;
					break;
				}
			}

			if (t_Parent == null)
				t_Parent = _Constraints.GetParentForType(DefaultBoneStructure[DefaultBoneStructure[_Type].parentNode].parentNode);

			return t_Parent;
		}

		public static Possession.Constraint[] GetChildrenForType(this Possession.Constraint[] _Constraints, BoneType _Type)
		{
			if (!DefaultBoneStructure.ContainsKey(_Type) || DefaultBoneStructure[_Type].childNodes.Length < 1)
				return null;

			var t_Children = new List<Possession.Constraint>();

			foreach (var t_ChildType in DefaultBoneStructure[_Type].childNodes)
			{
				foreach (var t_Constraint in _Constraints)
				{
					if (t_Constraint.bone.type == t_ChildType)
					{
						t_Children.Add(t_Constraint);
						break;
					}
				}
			}

			if (t_Children.Count == 0 && DefaultBoneStructure[_Type].childNodes.Length == 1)
			{
				var t_NewChildren = GetChildrenForType(_Constraints, DefaultBoneStructure[_Type].childNodes[0]);
				if (t_NewChildren == null) 
					return null;

				t_Children = t_NewChildren.ToList();
			}

			return t_Children.ToArray();
		}

		public static quat CalculateBoneRotation()
		{
			return quat.Identity;
		}
	}
}