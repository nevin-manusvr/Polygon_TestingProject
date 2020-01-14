using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManusVR.Polygon
{
	public class PolygonSkeleton : MonoBehaviour
	{
		public Animator animator;

		public SkeletonBoneReferences boneReferences;

		public SkeletonBoneReferences newSkeleton;

		public float spineLength = 0.1f;

		public List<Transform> scaleBones;
		public List<Transform> scaleFixBones;

		private Transform newSkeletonParent;

		#region Monobehaviour Callbacks

		private void Start()
		{

			if (boneReferences.IsValid)
			{
				newSkeleton = CopySkeleton(boneReferences, newSkeletonParent);
			}
		}

		private void Update()
		{
			//foreach (Transform bone in scaleBones)
			//{
			//	bone.localScale = new Vector3(1, 1, spineLength);
			//}

			//foreach (Transform bone in scaleFixBones)
			//{
			//	bone.localScale = new Vector3(1, 1, 1f / spineLength);
			//}
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
				CreateDirectionBone("foot_left", skeleton.legLeft.foot.bone.position, skeleton.legLeft.toes != null ? skeleton.legLeft.toes.bone.position - skeleton.legLeft.foot.bone.position : bodyForward, Vector3.up, parent),
				skeleton.legLeft.toes == null ? null : CreateDirectionBone("toes_left", skeleton.legLeft.toes.bone.position, skeleton.legLeft.toesEnd != null ? skeleton.legLeft.toesEnd.bone.position - skeleton.legLeft.toes.bone.position : bodyForward, Vector3.up, parent),
				skeleton.legLeft.toesEnd == null ? null : CreateDirectionBone("toesEnd_left", skeleton.legLeft.toesEnd.bone.position, bodyForward, Vector3.up, parent));

			skeletonCopy.legRight.AssignBones(
				CreateDirectionBone("upperLeg_right", skeleton.legRight.upperLeg.bone.position, skeleton.legRight.lowerLeg.bone.position - skeleton.legRight.upperLeg.bone.position, bodyForward, parent),
				CreateDirectionBone("lowerLeg_right", skeleton.legRight.lowerLeg.bone.position, skeleton.legRight.foot.bone.position - skeleton.legRight.lowerLeg.bone.position, bodyForward, parent),
				CreateDirectionBone("foot_right", skeleton.legRight.foot.bone.position, skeleton.legRight.toes != null ? skeleton.legRight.toes.bone.position - skeleton.legRight.foot.bone.position : bodyForward, Vector3.up, parent),
				skeleton.legRight.toes == null ? null : CreateDirectionBone("toes_right", skeleton.legRight.toes.bone.position, skeleton.legRight.toesEnd != null ? skeleton.legRight.toesEnd.bone.position - skeleton.legRight.toes.bone.position : bodyForward, Vector3.up, parent),
				skeleton.legRight.toesEnd == null ? null : CreateDirectionBone("toesEnd_right", skeleton.legRight.toesEnd.bone.position, bodyForward, Vector3.up, parent));

			skeletonCopy.armLeft.hand = CopyHandSkeleton(skeleton.armLeft.hand, parent);
			skeletonCopy.armRight.hand = CopyHandSkeleton(skeleton.armRight.hand, parent);

			return skeletonCopy;
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
		// TODO: end to do

		private Transform CreateDirectionBone(string name, Vector3 position, Vector3 lookForward, Vector3 lookUp, Transform parent = null)
		{
			Transform newBone = new GameObject(name).transform;
			if (parent != null) newBone.SetParent(parent);
			newBone.position = position;
			newBone.rotation = Quaternion.LookRotation(lookForward, lookUp);

			return newBone;
		}

		private void InsertScaleBone(Transform bone, Transform target)
		{
			Quaternion lookRotation = Quaternion.LookRotation(target.position - bone.position); 

			Transform scaleBone = new GameObject(bone.name + "_scaleBone").transform;
			scaleBone.SetParent(bone.parent);
			scaleBone.position = bone.position;
			scaleBone.rotation = lookRotation;

			scaleBones.Add(scaleBone); // TODO: temp

			bone.SetParent(scaleBone, true);

			var childs = new Transform[bone.childCount];
			for (int i = 0; i < bone.childCount; i++)
			{
				childs[i] = bone.GetChild(i);
			}

			foreach (Transform child in childs)
			{
				Transform scaleFixBone = new GameObject(child.name + "_scaleFixBone").transform;

				scaleFixBone.SetParent(bone, true);
				scaleFixBone.position = child.position;
				scaleFixBone.rotation = lookRotation;

				scaleFixBones.Add(scaleFixBone); // TODO: temp

				child.SetParent(scaleFixBone);
			}
		}

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

