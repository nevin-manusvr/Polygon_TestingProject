using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManusVR.Polygon
{
	public class HandCalibration : MonoBehaviour
	{
		private PolygonSkeleton skeleton;

		private void Start()
		{
			skeleton = GetComponent<PolygonSkeleton>();
			HandBoneReferences hand = skeleton.boneReferences.armLeft.hand;

			Vector3 leftHandPalm = (hand.wrist.bone.position + hand.index.proximal.bone.position + hand.pinky.proximal.bone.position) / 3f;
			Vector3 palmNormal = Vector3.Cross(hand.index.proximal.bone.position - hand.wrist.bone.position, hand.pinky.proximal.bone.position - hand.wrist.bone.position).normalized;
			
			Debug.DrawRay(leftHandPalm, palmNormal, Color.red, 10f);
		}
	}
}
