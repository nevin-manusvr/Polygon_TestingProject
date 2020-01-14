using UnityEditor;
using UnityEngine;

namespace ManusVR.Polygon
{
	[CustomEditor(typeof(PolygonSkeleton))]
	public class PolygonSkeletonEditor : Editor
	{
		private float size = 0.03f;

		private Color handlesColor = Color.cyan;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			PolygonSkeleton script = target as PolygonSkeleton;

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
			
			PolygonSkeleton skeleton = target as PolygonSkeleton;
			if (skeleton == null) return;

			if (skeleton.boneReferences.IsValid) 
				DrawHumanoidSkeletonBones(skeleton.boneReferences);

			if (skeleton.newSkeleton.IsValid)
				DrawHumanoidSkeletonDirections(skeleton.newSkeleton);
		}

		private void DrawHumanoidSkeletonBones(SkeletonBoneReferences bones)
		{
			Handles.color = handlesColor;

			DrawBodyAndHead(bones.body, bones.head);

			DrawLeg(bones.legLeft, bones.body);
			DrawLeg(bones.legRight, bones.body);

			DrawArm(bones.armLeft, bones.body);
			DrawArm(bones.armRight, bones.body);
		}

		private void DrawHumanoidSkeletonDirections(SkeletonBoneReferences bones)
		{
			DrawDirectionBone(bones.body.hip, size * 2f);
		}

		private void DrawDirectionBone(Bone bone, float size)
		{
			//HandleUtility.

			Handles.color = Handles.zAxisColor;
			Handles.ArrowHandleCap(0, bone.bone.position, bone.bone.rotation, size, EventType.Repaint);

			Handles.color = Handles.yAxisColor;
			Handles.DrawLine(bone.bone.position, bone.bone.position + bone.bone.up * size);
			Handles.CylinderHandleCap(0, bone.bone.position + bone.bone.up * size, bone.bone.rotation * Quaternion.Euler(0, 90, 0), size / 3f, EventType.Repaint);
		}

		#region drawing the skeleton bones

		private void DrawBodyAndHead(Body body, Head head)
		{
			// Draw Skeleton
			ConnectBones(body.hip, body.spine[0]);
			for (var i = 0; i < body.spine.Length; i++)
			{
				if (i == 0) continue;

				ConnectBones(body.spine[i - 1], body.spine[i]);
			}

			ConnectBones(body.spine[body.spine.Length - 1], head.neck);
			ConnectBones(head.neck, head.head);
			ConnectBones(head.head, head.eyeLeft);
			ConnectBones(head.head, head.eyeRight);

			// Draw Bones
			DrawBone(body.hip, size);
			foreach (Bone bone in body.spine)
			{
				DrawBone(bone, size);
			}

			DrawBone(head.neck, size);
			DrawBone(head.head, size);
			DrawBone(head.eyeLeft, size);
			DrawBone(head.eyeRight, size);
		}

		private void DrawArm(Arm arm, Body body)
		{
			// Draw Skeleton
			ConnectBones(arm.lowerArm, arm.hand.wrist);
			ConnectBones(body.spine[body.spine.Length - 1], arm.shoulder);
			ConnectBones(arm.shoulder, arm.upperArm);
			ConnectBones(arm.upperArm, arm.lowerArm);

			// Draw Bones
			DrawHand(arm.hand);

			DrawBone(arm.shoulder, size);
			DrawBone(arm.upperArm, size);
			DrawBone(arm.lowerArm, size);
		}
		
		private void DrawLeg(Leg leg, Body body)
		{
			// Draw Skeleton
			ConnectBones(body.hip, leg.upperLeg);
			ConnectBones(leg.upperLeg, leg.lowerLeg);
			ConnectBones(leg.lowerLeg, leg.foot);
			ConnectBones(leg.foot, leg.toes);
			ConnectBones(leg.toes, leg.toesEnd);

			// Draw Bones
			DrawBone(leg.upperLeg, size);
			DrawBone(leg.lowerLeg, size);
			DrawBone(leg.foot, size);
			DrawBone(leg.toes, size);
			DrawBone(leg.toesEnd, size);
		}

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
			Handles.DrawLine(bone1.bone.position, bone2.bone.position);
		}

		private void DrawBone(Bone bone, float size)
		{
			Handles.color = handlesColor;
			if (Handles.Button(bone.bone.position, Quaternion.identity, size, size, Handles.SphereHandleCap))
			{
				Selection.activeGameObject = bone.bone.gameObject;
			}

			//if (Handles.Button(bone.bone.position + bone.bone.up * size, bone.bone.rotation * Quaternion.Euler(0, 90, 0), size / 3f, size / 3f, Handles.CylinderHandleCap))
			//{
			//	Handles.DrawWireDisc(bone.bone.position, bone.bone.forward, size);

			//}

			//Handles.color = Color.blue;
			//Handles.ArrowHandleCap(0, bone.bone.position, bone.bone.rotation, size * 3f, EventType.Repaint);

			//Handles.color = Color.green;
			////Handles.CylinderHandleCap(0, bone.bone.position + bone.bone.up * size, bone.bone.rotation * Quaternion.Euler(0, 90, 0), size / 3f, EventType.Repaint);

			//Handles.color = handlesColor;
		}

		#endregion
	}
}

