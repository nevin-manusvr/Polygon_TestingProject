using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.ToBeHermes.Skeleton;

namespace Manus.Polygon.Skeleton.Utilities
{
	using System;

	public static class SkeletonOrientationCalculator
	{
		public static void CalculateOrientation(this Bone bone, SkeletonBoneReferences skeleton)
		{
			switch (bone.type)
			{
				case BoneType.Unknown:
					ErrorHandler.LogError(ErrorMessage.NoRequiredData);
					break;
				case BoneType.Root:
				case BoneType.Head:
				case BoneType.Hips:
				case BoneType.LeftFoot:
				case BoneType.RightFoot:
				case BoneType.LeftToes:
				case BoneType.RightToes:
				case BoneType.LeftToesEnd:
				case BoneType.RightToesEnd:

					{
						Tuple<Transform, Transform>[] bones =
							{
								Tuple.Create(skeleton.armLeft.hand.wrist.bone, skeleton.armRight.hand.wrist.bone),
								Tuple.Create(skeleton.legLeft.foot.bone, skeleton.legRight.foot.bone)
							};

						Vector3 aimDirection = CalculateForward(bones);
						Vector3 upDirection = Vector3.up;

						bone.desiredRotation = Quaternion.LookRotation(aimDirection, upDirection);
					}

					break;
				case BoneType.Neck:

					{
						Tuple<Transform, Transform>[] bones =
							{
								Tuple.Create(skeleton.armLeft.hand.wrist.bone, skeleton.armRight.hand.wrist.bone),
								Tuple.Create(skeleton.legLeft.foot.bone, skeleton.legRight.foot.bone)
							};

						Vector3 aimDirection = skeleton.head.head.bone.position - bone.bone.position;
						Vector3 upDirection = -CalculateForward(bones);

						bone.desiredRotation = Quaternion.LookRotation(aimDirection, upDirection);
					}

					break;
				case BoneType.Spine:

					{
						Tuple<Transform, Transform>[] bones =
							{
								Tuple.Create(skeleton.armLeft.hand.wrist.bone, skeleton.armRight.hand.wrist.bone),
								Tuple.Create(skeleton.legLeft.foot.bone, skeleton.legRight.foot.bone)
							};

						Vector3 aimDirection = (skeleton.body.spine[1]?.bone.position ?? skeleton.head.neck.bone.position) - bone.bone.position;
						Vector3 upDirection = -CalculateForward(bones);

						bone.desiredRotation = Quaternion.LookRotation(aimDirection, upDirection);
					}

					break;
				case BoneType.Chest:

					{
						Tuple<Transform, Transform>[] bones =
							{
								Tuple.Create(skeleton.armLeft.hand.wrist.bone, skeleton.armRight.hand.wrist.bone),
								Tuple.Create(skeleton.legLeft.foot.bone, skeleton.legRight.foot.bone)
							};

						Vector3 aimDirection = (skeleton.body.spine[2]?.bone.position ?? skeleton.head.neck.bone.position) - bone.bone.position;
						Vector3 upDirection = -CalculateForward(bones);

						bone.desiredRotation = Quaternion.LookRotation(aimDirection, upDirection);
					}

					break;
				case BoneType.UpperChest:

					{
						Tuple<Transform, Transform>[] bones =
							{
								Tuple.Create(skeleton.armLeft.hand.wrist.bone, skeleton.armRight.hand.wrist.bone),
								Tuple.Create(skeleton.legLeft.foot.bone, skeleton.legRight.foot.bone)
							};

						Vector3 aimDirection = skeleton.head.neck.bone.position - bone.bone.position;
						Vector3 upDirection = -CalculateForward(bones);

						bone.desiredRotation = Quaternion.LookRotation(aimDirection, upDirection);
					}

					break;
				case BoneType.LeftUpperLeg:

					{
						Tuple<Transform, Transform>[] bones =
							{
								Tuple.Create(skeleton.armLeft.hand.wrist.bone, skeleton.armRight.hand.wrist.bone),
								Tuple.Create(skeleton.legLeft.foot.bone, skeleton.legRight.foot.bone)
							};

						Vector3 aimDirection = skeleton.legLeft.lowerLeg.bone.position - bone.bone.position;
						Vector3 upDirection = CalculateForward(bones);

						bone.desiredRotation = Quaternion.LookRotation(aimDirection, upDirection);
					}

					break;
				case BoneType.RightUpperLeg:

					{
						Tuple<Transform, Transform>[] bones =
							{
								Tuple.Create(skeleton.armLeft.hand.wrist.bone, skeleton.armRight.hand.wrist.bone),
								Tuple.Create(skeleton.legLeft.foot.bone, skeleton.legRight.foot.bone)
							};

						Vector3 aimDirection = skeleton.legRight.lowerLeg.bone.position - bone.bone.position;
						Vector3 upDirection = CalculateForward(bones);

						bone.desiredRotation = Quaternion.LookRotation(aimDirection, upDirection);
					}

					break;
				case BoneType.LeftLowerLeg:

					{
						Tuple<Transform, Transform>[] bones =
							{
								Tuple.Create(skeleton.armLeft.hand.wrist.bone, skeleton.armRight.hand.wrist.bone),
								Tuple.Create(skeleton.legLeft.foot.bone, skeleton.legRight.foot.bone)
							};

						Vector3 aimDirection = skeleton.legLeft.foot.bone.position - bone.bone.position;
						Vector3 upDirection = CalculateForward(bones);

						bone.desiredRotation = Quaternion.LookRotation(aimDirection, upDirection);
					}

					break;
				case BoneType.RightLowerLeg:

					{
						Tuple<Transform, Transform>[] bones =
							{
								Tuple.Create(skeleton.armLeft.hand.wrist.bone, skeleton.armRight.hand.wrist.bone),
								Tuple.Create(skeleton.legLeft.foot.bone, skeleton.legRight.foot.bone)
							};

						Vector3 aimDirection = skeleton.legRight.foot.bone.position - bone.bone.position;
						Vector3 upDirection = CalculateForward(bones);

						bone.desiredRotation = Quaternion.LookRotation(aimDirection, upDirection);
					}

					break;
				case BoneType.LeftShoulder:

					{
						Vector3 aimDirection = skeleton.armLeft.upperArm.bone.position - bone.bone.position;
						Vector3 upDirection = Vector3.up;

						bone.desiredRotation = Quaternion.LookRotation(aimDirection, upDirection);
					}

					break;
				case BoneType.RightShoulder:

					{
						Vector3 aimDirection = skeleton.armRight.upperArm.bone.position - bone.bone.position;
						Vector3 upDirection = Vector3.up;

						bone.desiredRotation = Quaternion.LookRotation(aimDirection, upDirection);
					}

					break;
				case BoneType.LeftUpperArm:

					{
						Vector3 aimDirection = skeleton.armLeft.lowerArm.bone.position - bone.bone.position;
						Vector3 upDirection = Vector3.up;

						bone.desiredRotation = Quaternion.LookRotation(aimDirection, upDirection);
					}

					break;
				case BoneType.RightUpperArm:

					{
						Vector3 aimDirection = skeleton.armRight.lowerArm.bone.position - bone.bone.position;
						Vector3 upDirection = Vector3.up;

						bone.desiredRotation = Quaternion.LookRotation(aimDirection, upDirection);
					}

					break;
				case BoneType.LeftLowerArm:

					{
						Vector3 aimDirection = skeleton.armLeft.hand.wrist.bone.position - bone.bone.position;

						Vector3 armSide = Vector3.Cross(bone.bone.position - skeleton.armLeft.upperArm.bone.position, Vector3.up).normalized;
						Vector3 upDirection = Vector3.Cross(armSide, bone.bone.position - skeleton.armLeft.upperArm.bone.position).normalized;

						bone.desiredRotation = Quaternion.LookRotation(aimDirection, upDirection);
					}

					break;
				case BoneType.RightLowerArm:

					{
						Vector3 aimDirection = skeleton.armRight.hand.wrist.bone.position - bone.bone.position;

						Vector3 armSide = Vector3.Cross(bone.bone.position - skeleton.armRight.upperArm.bone.position, Vector3.up).normalized;
						Vector3 upDirection = Vector3.Cross(armSide, bone.bone.position - skeleton.armRight.upperArm.bone.position).normalized;

						bone.desiredRotation = Quaternion.LookRotation(aimDirection, upDirection);
					}

					break;
				case BoneType.LeftHand:
					
					{
						Vector3 aimDirection = skeleton.armLeft.hand.wrist.bone.position - skeleton.armLeft.lowerArm.bone.position;
						Vector3 upDirection = Vector3.up;

						bone.desiredRotation = Quaternion.LookRotation(aimDirection, upDirection);
					}

					break;
				case BoneType.RightHand:
					
					{
						Vector3 aimDirection = skeleton.armRight.hand.wrist.bone.position - skeleton.armRight.lowerArm.bone.position;
						Vector3 upDirection = Vector3.up;

						bone.desiredRotation = Quaternion.LookRotation(aimDirection, upDirection);
					}

					break;
				default:
					ErrorHandler.LogError(ErrorMessage.NotImplemented);
					break;
			}
		}

		/// <summary>
		/// Calculate the forward direction
		/// </summary>
		/// <param name="direction">collection of transform pairs, every pair should be a left and right transform</param>
		public static Vector3 CalculateForward(Tuple<Transform, Transform>[] directions)
		{
			Vector3 forward = Vector3.zero;
			foreach (Tuple<Transform, Transform> directionPairs in directions)
			{
				Vector3 cross = Vector3.Cross(directionPairs.Item2.position - directionPairs.Item1.position, Vector3.up);
				cross.y = 0;

				forward += cross.normalized;
			}

			return forward / directions.Length;
		}
	}
}

