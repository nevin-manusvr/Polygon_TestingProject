using UnityEditor;
using UnityEngine;

namespace ManusVR.Polygon
{
	[CustomEditor(typeof(HandSkeleton))]
	public class HandSkeletonEditor : Editor
	{
		private float size = 0.03f;

		private Color handlesColor = Color.cyan;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			HandSkeleton script = target as HandSkeleton;

			if (GUILayout.Button("Populate Bones"))
			{
				script.PopulateBoneReferences();
			}

			if (GUILayout.Button("Clear Bone References"))
			{
				script.ClearBoneReferences();
			}
		}

		void OnSceneGUI()
		{
			// TODO: add undo/redo functionality
			HandSkeleton skeleton = target as HandSkeleton;

			if (skeleton == null || !skeleton.handBoneReferences.IsValid) return;

			HandBoneReferences bones = skeleton.handBoneReferences;

			if (bones.IsValid) 
				DrawHand(bones);
		}

		#region drawing the skeleton bones

		private void DrawHand(HandBoneReferences hand)
		{
			DrawBone(hand.wrist, size);

			DrawFinger(hand.index, hand);
			DrawFinger(hand.middle, hand);
			DrawFinger(hand.ring, hand);
			DrawFinger(hand.pinky, hand);
			DrawFinger(hand.thumb, hand);
		}

		private void DrawFinger(Finger finger, HandBoneReferences hand)
		{
			// Draw Skeleton
			ConnectBones(hand.wrist, finger.proximal);
			ConnectBones(finger.proximal, finger.middle);
			ConnectBones(finger.middle, finger.distal);
			ConnectBones(finger.distal, finger.tip);

			// Draw Bones
			DrawBone(finger.proximal, size / 3f);
			DrawBone(finger.middle, size / 3f);
			DrawBone(finger.distal, size / 3f);
			DrawBone(finger.tip, size / 3f);
		}

		private void ConnectBones(Bone bone1, Bone bone2)
		{
			Handles.color = handlesColor;
			Handles.DrawLine(bone1.bone.position, bone2.bone.position);
		}

		private void DrawBone(Bone bone, float size)
		{
			Handles.color = handlesColor;
			if (Handles.Button(bone.bone.position, Quaternion.identity, size, size, Handles.SphereHandleCap))
			{
				if (Event.current.control)
				{
					Selection.activeGameObject = bone.bone.gameObject;
				}
				else
				{
					Selection.activeGameObject = (target as HandSkeleton)?.gameObject;
				}
			}
		}

		#endregion
	}
}

