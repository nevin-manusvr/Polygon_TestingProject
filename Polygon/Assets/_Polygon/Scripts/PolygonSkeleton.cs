using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Polygon
{
	public class PolygonSkeleton : MonoBehaviour
	{
		public Animator animator;

		public SkeletonBoneReferences boneReferences;

		public SkeletonBoneReferences newSkeleton;

		private Transform newSkeletonParent;

		public SkeletonBoneScalers boneScalers;

		// TMP
		public PolygonIK_TMP ik;

		#region Monobehaviour Callbacks

		private void Awake()
		{
			if (boneReferences.IsValid && newSkeleton.IsValid)
			{
				ReparentSkeleton(newSkeleton);
				ParentSkeletonToAnotherSkeleton(newSkeleton, boneReferences);

				boneScalers = new SkeletonBoneScalers();
				boneScalers.GenerateScalerBonesForBody(boneReferences, newSkeleton);
			}
		}

		#endregion

		#region Private Methods

		private Animator FindValidAnimatorInHierarchy()
		{
			Animator[] animatorsInHierarchy = GetComponentsInChildren<Animator>();
			foreach (Animator ac in animatorsInHierarchy)
			{
				if (ac.avatar != null && ac.avatar.isValid && ac.avatar.isHuman)
				{
					Debug.Log("Valid animator found");
					return ac;
				}
			}

			Debug.LogWarning(animatorsInHierarchy.Length == 0 ? "No animators found in hierarchy" : "No animator found with a valid human avatar, go to the settings and set the 'Animation Type' to 'Humanoid'");

			return null;
		}

		// TODO: place this in another script, maybe the SkeletonBoneReferences or a new one
		#region TEMP (put in another file)

		private SkeletonBoneReferences CopySkeleton(SkeletonBoneReferences skeleton, Transform parent)
		{
			if (!skeleton.IsValid)
			{
				return null;
			}
			
			SkeletonBoneReferences skeletonCopy = new SkeletonBoneReferences();

			Vector3 bodyForward =
				(Vector3.Cross(skeleton.legRight.foot.bone.position - skeleton.legLeft.foot.bone.position, Vector3.up).normalized + 
				 Vector3.Cross(skeleton.armRight.shoulder.bone.position - skeleton.armLeft.shoulder.bone.position, Vector3.up).normalized) / 2f;


			Debug.DrawRay(transform.position, bodyForward, Color.red, 10);

			skeletonCopy.main = new Bone(CreateDirectionBone("main", (skeleton.legLeft.upperLeg.bone.position + skeleton.legRight.upperLeg.bone.position) / 2f, bodyForward, Vector3.up, parent));

			skeletonCopy.head.AssignBones(
				CreateDirectionBone("neck", skeleton.head.neck.bone.position, skeleton.head.head.bone.position - skeleton.head.neck.bone.position, -bodyForward, parent),
				CreateDirectionBone("head", skeleton.head.head.bone.position, bodyForward, Vector3.up, parent),
				null, null);

			skeletonCopy.body.AssignBones(
				CreateDirectionBone("hip", skeleton.body.hip.bone.position, skeleton.body.spine[0].bone.position - skeleton.body.hip.bone.position, -bodyForward, parent),
				CreateDirectionBone("spine", skeleton.body.spine[0].bone.position, (skeleton.body.spine[1]?.bone.position ?? skeleton.head.neck.bone.position) - skeleton.body.spine[0].bone.position, -bodyForward, parent),
				skeleton.body.spine[1] == null ? null : CreateDirectionBone("chest", skeleton.body.spine[1].bone.position, (skeleton.body.spine[2]?.bone.position ?? skeleton.head.neck.bone.position) - skeleton.body.spine[1].bone.position, -bodyForward, parent),
				skeleton.body.spine[2] == null ? null : CreateDirectionBone("upperchest", skeleton.body.spine[2].bone.position, skeleton.head.neck.bone.position - skeleton.body.spine[2].bone.position, -bodyForward, parent));

			skeletonCopy.armLeft.AssignBones(
				CreateDirectionBone("shoulder_left", skeleton.armLeft.shoulder.bone.position, skeleton.armLeft.upperArm.bone.position - skeleton.armLeft.shoulder.bone.position, Vector3.up, parent),
				CreateDirectionBone("upperArm_left", skeleton.armLeft.upperArm.bone.position, skeleton.armLeft.lowerArm.bone.position - skeleton.armLeft.upperArm.bone.position, Vector3.up, parent),
				CreateDirectionBone("lowerArm_left", skeleton.armLeft.lowerArm.bone.position, skeleton.armLeft.hand.wrist.bone.position - skeleton.armLeft.lowerArm.bone.position, Vector3.up, parent));

			skeletonCopy.armRight.AssignBones(
				CreateDirectionBone("shoulder_right", skeleton.armRight.shoulder.bone.position, skeleton.armRight.upperArm.bone.position - skeleton.armRight.shoulder.bone.position, Vector3.up, parent),
				CreateDirectionBone("upperArm_right", skeleton.armRight.upperArm.bone.position, skeleton.armRight.lowerArm.bone.position - skeleton.armRight.upperArm.bone.position, Vector3.up, parent),
				CreateDirectionBone("lowerArm_right", skeleton.armRight.lowerArm.bone.position, skeleton.armRight.hand.wrist.bone.position - skeleton.armRight.lowerArm.bone.position, Vector3.up, parent));

			skeletonCopy.legLeft.AssignBones(
				CreateDirectionBone("upperLeg_left", skeleton.legLeft.upperLeg.bone.position, skeleton.legLeft.lowerLeg.bone.position - skeleton.legLeft.upperLeg.bone.position, bodyForward, parent),
				CreateDirectionBone("lowerLeg_left", skeleton.legLeft.lowerLeg.bone.position, skeleton.legLeft.foot.bone.position - skeleton.legLeft.lowerLeg.bone.position, bodyForward, parent),
				CreateDirectionBone("foot_left", skeleton.legLeft.foot.bone.position, bodyForward, Vector3.up, parent),
				skeleton.legLeft.toes == null ? null : CreateDirectionBone("toes_left", skeleton.legLeft.toes.bone.position, bodyForward, Vector3.up, parent),
				skeleton.legLeft.toesEnd == null ? null : CreateDirectionBone("toesEnd_left", skeleton.legLeft.toesEnd.bone.position, bodyForward, Vector3.up, parent));

			skeletonCopy.legRight.AssignBones(
				CreateDirectionBone("upperLeg_right", skeleton.legRight.upperLeg.bone.position, skeleton.legRight.lowerLeg.bone.position - skeleton.legRight.upperLeg.bone.position, bodyForward, parent),
				CreateDirectionBone("lowerLeg_right", skeleton.legRight.lowerLeg.bone.position, skeleton.legRight.foot.bone.position - skeleton.legRight.lowerLeg.bone.position, bodyForward, parent),
				CreateDirectionBone("foot_right", skeleton.legRight.foot.bone.position, bodyForward, Vector3.up, parent),
				skeleton.legRight.toes == null ? null : CreateDirectionBone("toes_right", skeleton.legRight.toes.bone.position, bodyForward, Vector3.up, parent),
				skeleton.legRight.toesEnd == null ? null : CreateDirectionBone("toesEnd_right", skeleton.legRight.toesEnd.bone.position, bodyForward, Vector3.up, parent));

			skeletonCopy.armLeft.hand = CopyHandSkeleton(skeleton.armLeft.hand, parent);
			skeletonCopy.armRight.hand = CopyHandSkeleton(skeleton.armRight.hand, parent);

			return skeletonCopy;
		}

		// Parent Skeleton to another skeleton
		public void ParentSkeletonToAnotherSkeleton(SkeletonBoneReferences parent, SkeletonBoneReferences child)
		{
			InsertBoneParent(child.main.bone, parent.main.bone);
			InsertBoneParent(child.body.hip.bone, parent.body.hip.bone);
			for (var index = 0; index < child.body.spine.Length; index++)
			{
				InsertBoneParent(child.body.spine[index].bone, parent.body.spine[index].bone);
			}

			InsertBoneParent(child.head.neck.bone, parent.head.neck.bone);
			InsertBoneParent(child.head.head.bone, parent.head.head.bone);

			ParentArmToArm(parent.armLeft, child.armLeft);
			ParentArmToArm(parent.armRight, child.armRight);

			ParentLegToLeg(parent.legLeft, child.legLeft);
			ParentLegToLeg(parent.legRight, child.legRight);

			ParentHandSkeletonToAnotherHandSkeleton(parent.armLeft.hand, child.armLeft.hand);
			ParentHandSkeletonToAnotherHandSkeleton(parent.armRight.hand, child.armRight.hand);
		}

		public void ParentHandSkeletonToAnotherHandSkeleton(HandBoneReferences parent, HandBoneReferences child)
		{
			InsertBoneParent(child.wrist.bone, parent.wrist.bone);

			ParentFingerToFinger(parent.index, child.index);
			ParentFingerToFinger(parent.middle, child.middle);
			ParentFingerToFinger(parent.ring, child.ring);
			ParentFingerToFinger(parent.pinky, child.pinky);
			ParentFingerToFinger(parent.thumb, child.thumb);
		}

		void ParentArmToArm(Arm parent, Arm child)
		{
			InsertBoneParent(child.shoulder.bone, parent.shoulder.bone);
			InsertBoneParent(child.upperArm.bone, parent.upperArm.bone);
			InsertBoneParent(child.lowerArm.bone, parent.lowerArm.bone);
		}

		void ParentLegToLeg(Leg parent, Leg child)
		{
			InsertBoneParent(child.upperLeg.bone, parent.upperLeg.bone);
			InsertBoneParent(child.lowerLeg.bone, parent.lowerLeg.bone);
			InsertBoneParent(child.foot.bone, parent.foot.bone);
			if (child.toes?.bone != null) InsertBoneParent(child.toes.bone, parent.toes.bone);
			if (child.toesEnd?.bone != null) InsertBoneParent(child.toesEnd.bone, parent.toesEnd.bone);
		}

		private void ParentFingerToFinger(Finger parent, Finger child)
		{
			InsertBoneParent(child.proximal.bone, parent.proximal.bone);
			InsertBoneParent(child.middle.bone, parent.middle.bone);
			InsertBoneParent(child.distal.bone, parent.distal.bone);
			InsertBoneParent(child.tip.bone, parent.tip.bone);
		}

		private void InsertBoneParent(Transform child, Transform parent)
		{
			parent.SetParent(child.parent);
			child.SetParent(parent);
		}

		// Fix parenting of a single skeleton
		public void ReparentSkeleton(SkeletonBoneReferences skeleton)
		{
			skeleton.body.hip.bone.SetParent(skeleton.body.hip.bone);

			skeleton.body.spine[0].bone.SetParent(skeleton.body.hip.bone);
			for (int i = 0; i < skeleton.body.spine.Length; i++)
			{
				skeleton.body.spine[i].bone.SetParent(i == 0 ? skeleton.body.hip.bone : skeleton.body.spine[i - 1].bone);
			}
			skeleton.armLeft.shoulder.bone.SetParent(skeleton.body.spine[skeleton.body.spine.Length - 1].bone);

			skeleton.head.neck.bone.SetParent(skeleton.body.spine[skeleton.body.spine.Length - 1].bone);
			skeleton.head.head.bone.SetParent(skeleton.head.neck.bone);

			ReparentArm(skeleton.armLeft, skeleton.body);
			ReparentArm(skeleton.armRight, skeleton.body);

			ReparentLeg(skeleton.legLeft, skeleton.body);
			ReparentLeg(skeleton.legRight, skeleton.body);

			ReparentHand(skeleton.armLeft.hand);
			ReparentHand(skeleton.armRight.hand);
		}

		public void ReparentHand(HandBoneReferences hand)
		{
			ReparentFinger(hand.index, hand.wrist);
			ReparentFinger(hand.middle, hand.wrist);
			ReparentFinger(hand.ring, hand.wrist);
			ReparentFinger(hand.pinky, hand.wrist);
			ReparentFinger(hand.thumb, hand.wrist);
		}

		private void ReparentArm(Arm arm, Body body)
		{
			arm.shoulder.bone.SetParent(body.spine[body.spine.Length - 1].bone);
			arm.upperArm.bone.SetParent(arm.shoulder.bone);
			arm.lowerArm.bone.SetParent(arm.upperArm.bone);
			arm.hand.wrist.bone.SetParent(arm.lowerArm.bone);
		}

		private void ReparentLeg(Leg leg, Body body)
		{
			leg.upperLeg.bone.SetParent(body.hip.bone);
			leg.lowerLeg.bone.SetParent(leg.upperLeg.bone);
			leg.foot.bone.SetParent(leg.lowerLeg.bone);
			if (leg.toes?.bone != null) leg.toes.bone.SetParent(leg.foot.bone);
			if (leg.toesEnd?.bone != null && leg.toes?.bone != null) leg.toesEnd.bone.SetParent(leg.toes.bone);
		}

		private void ReparentFinger(Finger finger, Bone wrist)
		{
			finger.proximal.bone.SetParent(wrist.bone);
			finger.middle.bone.SetParent(finger.proximal.bone);
			finger.distal.bone.SetParent(finger.middle.bone);
			finger.tip.bone.SetParent(finger.distal.bone);
		}

		private HandBoneReferences CopyHandSkeleton(HandBoneReferences hand, Transform parent)
		{
			var newHand = new HandBoneReferences();
			Vector3 handUp = Vector3.up;

			newHand.wrist = new Bone(CreateDirectionBone("wrist", hand.wrist.bone.position, hand.middle.proximal.bone.position - hand.wrist.bone.position, handUp, parent));

			newHand.index.AssignBones(
				CreateDirectionBone("index_prox", hand.index.proximal.bone.position, hand.index.middle.bone.position - hand.index.proximal.bone.position, handUp, parent),
				CreateDirectionBone("index_mid", hand.index.middle.bone.position, hand.index.distal.bone.position - hand.index.middle.bone.position, handUp, parent),
				CreateDirectionBone("index_dis", hand.index.distal.bone.position, hand.index.tip.bone.position - hand.index.distal.bone.position, handUp, parent),
				CreateDirectionBone("index_tip", hand.index.tip.bone.position, hand.index.tip.bone.position - hand.index.distal.bone.position, handUp, parent));

			newHand.middle.AssignBones(
				CreateDirectionBone("middle_prox", hand.middle.proximal.bone.position, hand.middle.middle.bone.position - hand.middle.proximal.bone.position, handUp, parent),
				CreateDirectionBone("middle_mid", hand.middle.middle.bone.position, hand.middle.distal.bone.position - hand.middle.middle.bone.position, handUp, parent),
				CreateDirectionBone("middle_dis", hand.middle.distal.bone.position, hand.middle.tip.bone.position - hand.middle.distal.bone.position, handUp, parent),
				CreateDirectionBone("middle_tip", hand.middle.tip.bone.position, hand.middle.tip.bone.position - hand.middle.distal.bone.position, handUp, parent));

			newHand.ring.AssignBones(
				CreateDirectionBone("ring_prox", hand.ring.proximal.bone.position, hand.ring.middle.bone.position - hand.ring.proximal.bone.position, handUp, parent),
				CreateDirectionBone("ring_mid", hand.ring.middle.bone.position, hand.ring.distal.bone.position - hand.ring.middle.bone.position, handUp, parent),
				CreateDirectionBone("ring_dis", hand.ring.distal.bone.position, hand.ring.tip.bone.position - hand.ring.distal.bone.position, handUp, parent),
				CreateDirectionBone("ring_tip", hand.ring.tip.bone.position, hand.ring.tip.bone.position - hand.ring.distal.bone.position, handUp, parent));

			newHand.pinky.AssignBones(
				CreateDirectionBone("pinky_prox", hand.pinky.proximal.bone.position, hand.pinky.middle.bone.position - hand.pinky.proximal.bone.position, handUp, parent),
				CreateDirectionBone("pinky_mid", hand.pinky.middle.bone.position, hand.pinky.distal.bone.position - hand.pinky.middle.bone.position, handUp, parent),
				CreateDirectionBone("pinky_dis", hand.pinky.distal.bone.position, hand.pinky.tip.bone.position - hand.pinky.distal.bone.position, handUp, parent),
				CreateDirectionBone("pinky_tip", hand.pinky.tip.bone.position, hand.pinky.tip.bone.position - hand.pinky.distal.bone.position, handUp, parent));

			newHand.thumb.AssignBones(
				CreateDirectionBone("thumb_prox", hand.thumb.proximal.bone.position, hand.thumb.middle.bone.position - hand.thumb.proximal.bone.position, handUp, parent),
				CreateDirectionBone("thumb_mid", hand.thumb.middle.bone.position, hand.thumb.distal.bone.position - hand.thumb.middle.bone.position, handUp, parent),
				CreateDirectionBone("thumb_dis", hand.thumb.distal.bone.position, hand.thumb.tip.bone.position - hand.thumb.distal.bone.position, handUp, parent),
				CreateDirectionBone("thumb_tip", hand.thumb.tip.bone.position, hand.thumb.tip.bone.position - hand.thumb.distal.bone.position, handUp, parent));

			return newHand;
		}

		private Transform CreateDirectionBone(string name, Vector3 position, Vector3 lookForward, Vector3 lookUp, Transform parent = null)
		{
			Transform newBone = new GameObject(name).transform;
			if (parent != null) newBone.SetParent(parent);
			newBone.position = position;
			newBone.rotation = Quaternion.LookRotation(lookForward, lookUp);

			return newBone;
		}
		#endregion

		#endregion

		public bool IsAnimatorValid()
		{
			if (animator == null || animator.avatar == null || !animator.avatar.isValid || !animator.avatar.isHuman)
			{
				Debug.LogWarning((animator == null ? "No animator assigned" : "Assigned animator does not have a valid human avatar") + ", trying to find one");

				animator = FindValidAnimatorInHierarchy() ?? animator;

				if (animator != null) return false;
			}

			return true;
		}

		public void PopulateBoneReferences()
		{
			if (IsAnimatorValid())
			{
				boneReferences.Populate(animator);
			}

			if (boneReferences.IsValid)
			{
				if (newSkeletonParent != null)
					DestroyImmediate(newSkeletonParent.gameObject);

				newSkeletonParent = new GameObject("newSkeleton").transform;
				newSkeletonParent.SetParent(transform);

				newSkeleton = CopySkeleton(boneReferences, newSkeletonParent);
			}
		}

		public void ClearBoneReferences()
		{
			boneReferences.Clear();
		}
	}
}

