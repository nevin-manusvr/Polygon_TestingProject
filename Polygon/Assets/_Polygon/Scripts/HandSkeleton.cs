using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Polygon
{
	public class HandSkeleton : MonoBehaviour
	{
		public HandBoneReferences handBoneReferences;

		#region Monobehaviour Callbacks



		#endregion

		#region Private Methods

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


		public void PopulateBoneReferences()
		{
			handBoneReferences.PopulateBones(transform);
		}

		public void ClearBoneReferences()
		{
			handBoneReferences.ClearBoneReferences();
		}
	}
}

