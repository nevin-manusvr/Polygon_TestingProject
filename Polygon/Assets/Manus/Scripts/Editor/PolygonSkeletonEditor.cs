using UnityEditor;
using UnityEngine;

namespace Manus.Polygon.Skeleton.Editor
{
	using Accord;

	using Manus.Polygon.Skeleton.Utilities;

	[CustomEditor(typeof(PolygonSkeleton))]
	public class PolygonSkeletonEditor : UnityEditor.Editor
	{
		private float size = 0.03f;

		private Color handlesColor = Color.cyan;
		private Bone selectedBone;

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

			if (GUILayout.Button("Calculatettteette"))
			{
				script.Calculateeee();
			}
		}

		void OnSceneGUI()
		{
			// TODO: add undo/redo functionality
			PolygonSkeleton skeleton = target as PolygonSkeleton;

			if (skeleton == null || !skeleton.boneReferences.IsValid) return;

			SkeletonBoneReferences bones = skeleton.boneReferences;
			// if (skeleton.newSkeleton.IsValid) bones = skeleton.newSkeleton;

			if (skeleton.boneReferences.IsValid) 
				DrawHumanoidSkeletonBones(bones);

			if (Selection.activeGameObject != skeleton.gameObject) selectedBone = null;
			if (selectedBone != null) DrawRotationGizmo(selectedBone, size);
		}

		private void DrawHumanoidSkeletonBones(SkeletonBoneReferences bones)
		{
			// Draw whole skeleton
			DrawBodyAndHead(bones.body, bones.head);

			DrawLeg(bones.legLeft, bones.body);
			DrawLeg(bones.legRight, bones.body);

			DrawArm(bones.armLeft, bones.body);
			DrawArm(bones.armRight, bones.body);
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
			//if (head.eyeLeft?.bone != null) ConnectBones(head.head, head.eyeLeft);
			//if (head.eyeRight?.bone != null) ConnectBones(head.head, head.eyeRight);

			// Draw Bones
			DrawBone(body.hip, size);
			foreach (Bone bone in body.spine)
			{
				DrawBone(bone, size);
			}

			DrawBone(head.neck, size);
			DrawBone(head.head, size);
			//if (head.eyeLeft?.bone != null) DrawBone(head.eyeLeft, size);
			//if (head.eyeRight?.bone != null) DrawBone(head.eyeRight, size);
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

			//DrawFinger(hand.index, hand);
			//DrawFinger(hand.middle, hand);
			//DrawFinger(hand.ring, hand);
			//DrawFinger(hand.pinky, hand);
			//DrawFinger(hand.thumb, hand);
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

			if (selectedBone == bone)
			{
				Handles.SphereHandleCap(0, bone.bone.position, Quaternion.identity, size, EventType.Repaint);
				DrawDirectionBone(bone, size);
				return;
			}

			if (Handles.Button(bone.bone.position, Quaternion.identity, size, size, Handles.SphereHandleCap))
			{
				if (Event.current.control)
				{
					Selection.activeGameObject = bone.bone.gameObject;
				}
				else
				{
					Selection.activeGameObject = (target as PolygonSkeleton)?.gameObject;
					selectedBone = bone;
				}
			}

			DrawDirectionBone(bone, size);
		}

		private void DrawDirectionBone(Bone bone, float size)
		{
			
			if (!bone.desiredRotation.IsValid()) return;

			Handles.color = Handles.zAxisColor;
			Handles.ArrowHandleCap(0, bone.bone.position, bone.desiredRotation, size, EventType.Repaint);

			Handles.color = Handles.yAxisColor;
			Handles.ArrowHandleCap(0, bone.bone.position, bone.desiredRotation * Quaternion.Euler(-90f, 0f, 0f), size, EventType.Repaint);
		}

		private void DrawRotationGizmo(Bone bone, float size)
		{
			if (!bone.desiredRotation.IsValid() || bone.bone == null) return;
			
			Handles.color = Handles.yAxisColor;
			Handles.CylinderHandleCap(0, bone.bone.position + bone.desiredRotation * Vector3.up * size, bone.desiredRotation * Quaternion.Euler(0, 90, 0), size / 3f, EventType.Repaint);

			EditorGUI.BeginChangeCheck();
			
			Quaternion rot = Handles.Disc(bone.desiredRotation, bone.bone.position, bone.desiredRotation * Vector3.forward, size, false, 0.01f);
			if (EditorGUI.EndChangeCheck())
			{
				Debug.Log("Record");
				Undo.RecordObject(target, "Rotated Bone");
				bone.desiredRotation = rot;
			}
		}

		#endregion
	}
}

