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

						Vector3 aimDirection = (skeleton.body.chest.bone ? skeleton.body.chest.bone.position : skeleton.head.neck.bone.position) - bone.bone.position;
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

						Vector3 aimDirection = (skeleton.body.upperChest.bone ? skeleton.body.upperChest.bone.position : skeleton.head.neck.bone.position) - bone.bone.position;
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
						HandBoneReferences hand = skeleton.armLeft.hand;

						// Forward Vector
						Vector3 aimDirection =
							((hand.index.proximal.bone.position - hand.wrist.bone.position
							  + hand.middle.proximal.bone.position - hand.wrist.bone.position
							  + hand.ring.proximal.bone.position - hand.wrist.bone.position
							  + hand.pinky.proximal.bone.position - hand.wrist.bone.position) / 4f).normalized;

						// Up Vector
						Vector3 upDirection =
							(Vector3.Cross(hand.middle.proximal.bone.position - hand.index.proximal.bone.position, aimDirection)
							 + Vector3.Cross(hand.ring.proximal.bone.position - hand.middle.proximal.bone.position, aimDirection)
							 + Vector3.Cross(hand.pinky.proximal.bone.position - hand.ring.proximal.bone.position, aimDirection)).normalized;

						bone.desiredRotation = Quaternion.LookRotation(aimDirection, upDirection);
					}

					break;
				case BoneType.RightHand:
					
					{
						HandBoneReferences hand = skeleton.armRight.hand;

						// Forward Vector
						Vector3 aimDirection =
							((hand.index.proximal.bone.position - hand.wrist.bone.position
							  + hand.middle.proximal.bone.position - hand.wrist.bone.position
							  + hand.ring.proximal.bone.position - hand.wrist.bone.position
							  + hand.pinky.proximal.bone.position - hand.wrist.bone.position) / 4f).normalized;

						// Up Vector
						Vector3 upDirection =
							(Vector3.Cross(hand.middle.proximal.bone.position - hand.index.proximal.bone.position, aimDirection)
							 + Vector3.Cross(hand.ring.proximal.bone.position - hand.middle.proximal.bone.position, aimDirection)
							 + Vector3.Cross(hand.pinky.proximal.bone.position - hand.ring.proximal.bone.position, aimDirection)).normalized * -1f;

						bone.desiredRotation = Quaternion.LookRotation(aimDirection, upDirection);
					}

					break;
				default:
					ErrorHandler.LogError(ErrorMessage.NotImplemented);
					break;
			}
		}

		public static Bone GetLookAtBone(this Bone bone, SkeletonBoneReferences skeleton)
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
					
					return null;

				case BoneType.Neck:

					return skeleton.head.head;

				case BoneType.Spine:

					return skeleton.body.chest.bone ? skeleton.body.chest : skeleton.head.neck;

				case BoneType.Chest:

					return skeleton.body.upperChest.bone ? skeleton.body.upperChest : skeleton.head.neck;
				
				case BoneType.UpperChest:

					return skeleton.head.neck;
				
				case BoneType.LeftUpperLeg:

					return skeleton.legLeft.lowerLeg;

				case BoneType.RightUpperLeg:

					return skeleton.legRight.lowerLeg;

				case BoneType.LeftLowerLeg:

					return skeleton.legLeft.foot;

				case BoneType.RightLowerLeg:

					return skeleton.legRight.foot;

				case BoneType.LeftShoulder:

					return skeleton.armLeft.upperArm;

				case BoneType.RightShoulder:

					return skeleton.armRight.upperArm;

				case BoneType.LeftUpperArm:

					return skeleton.armLeft.lowerArm;

				case BoneType.RightUpperArm:

					return skeleton.armRight.lowerArm;

				case BoneType.LeftLowerArm:

					return skeleton.armLeft.hand.wrist;

				case BoneType.RightLowerArm:

					return skeleton.armRight.hand.wrist;

				case BoneType.LeftHand:

					return skeleton.armLeft.hand.wrist;

				case BoneType.RightHand:

					return skeleton.armRight.hand.wrist;

				default:
					ErrorHandler.LogError(ErrorMessage.NotImplemented);
					break;
			}

			return null;
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

