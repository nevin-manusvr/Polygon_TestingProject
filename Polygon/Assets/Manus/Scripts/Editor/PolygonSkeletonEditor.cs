using UnityEditor;
using UnityEngine;
using Manus.Polygon.Skeleton.Utilities;

namespace Manus.Polygon.Skeleton.Editor
{
	using System.Linq;

	public enum ControlPointType
	{
		Ground,
		Height,
		Group
	}

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
				script?.PopulateBoneReferences();
			}

			if (GUILayout.Button("Clear Bone References"))
			{
				script?.ClearBoneReferences();
			}

			if (GUILayout.Button("Calculate Bone Orientations"))
			{
				script?.CalculateBoneOrientations();
			}

			if (GUILayout.Button("Set Bind Pose"))
			{
				script?.SetBindPose();
			}

			if (GUILayout.Button("Set To Bind Pose"))
			{
				script?.SetToBindPose();
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

			DrawControlPoint(bones.head.modelHeight, ControlPointType.Height);
			DrawControlPoint(bones.body.hipControl, ControlPointType.Group);

			Bone highestSpine = bones.body.spine;
			if (bones.body.chest.bone != null) highestSpine = bones.body.chest;
			if (bones.body.upperChest.bone != null) highestSpine = bones.body.upperChest;
			DrawControlPoint(bones.body.upperBodyControl, ControlPointType.Group);
		}

		#region drawing the skeleton bones

		private void DrawBodyAndHead(Body body, Head head)
		{
			// Draw Skeleton
			ConnectBones(body.hip, body.spine);
			if (body.chest.bone)
			{
				
				ConnectBones(body.spine, body.chest);

				if (body.upperChest.bone)
				{
					ConnectBones(body.chest, body.upperChest);
					ConnectBones(body.upperChest, head.neck);
				}
				else
				{
					ConnectBones(body.chest, head.neck);
				}
			}
			else
			{
				ConnectBones(body.spine, head.neck);
			}

			ConnectBones(head.neck, head.head);

			// Draw Bones
			DrawBone(body.hip, size);
			DrawBone(body.spine, size);
			if (body.chest.bone) DrawBone(body.chest, size);
			if (body.upperChest.bone) DrawBone(body.upperChest, size);

			DrawBone(head.neck, size);
			DrawBone(head.head, size);
		}

		private void DrawArm(Arm arm, Body body)
		{
			// Draw Skeleton
			ConnectBones(arm.lowerArm, arm.hand.wrist);

			Bone highestSpine = body.spine;
			if (body.chest.bone) highestSpine = body.chest;
			if (body.upperChest.bone) highestSpine = body.upperChest;

			ConnectBones(highestSpine, arm.shoulder);
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

			DrawControlPoint(leg.heel, ControlPointType.Ground);
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
			var bones = finger.GatherBones(GatherType.All).Values.ToArray();
			for (int i = 0; i < bones.Length; i++)
			{ 
				ConnectBones(bones[i], (i == 0) ? hand.wrist : bones[i - 1]);
				DrawBone(bones[i], size / 3f);
			}
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

		private void DrawControlPoint(ControlBone control, ControlPointType type)
		{
			switch (type)
			{
				case ControlPointType.Ground:

					if (control.rotation.IsValid())
					{
						EditorGUI.BeginChangeCheck();

						// Matrix4x4 rootMatrix = Matrix4x4.TRS(root.bone.position, root.desiredRotation, root.bone.lossyScale);
						
						//Quaternion controlRotation = Quaternion.LookRotation(rootMatrix.MultiplyVector(control.rotation * Vector3.forward), rootMatrix.MultiplyVector(control.rotation * Vector3.up));
						Vector3 point = Handles.Slider2D(control.position, control.rotation * Vector3.up, control.rotation* Vector3.forward, control.rotation * Vector3.right, size, Handles.CircleHandleCap, new Vector2(.001f, .001f));
						Handles.DrawLine(point, point + control.rotation * Vector3.forward * size);

						if (EditorGUI.EndChangeCheck())
						{
							Undo.RecordObject(target, "Moved Control point");
							control.UpdateTransformation(point);
						}
					}

					break;
				case ControlPointType.Height:

					if (control.position != Vector3.zero)
					{
						EditorGUI.BeginChangeCheck();

						Vector3 point = Handles.Slider(
							control.position + control.rotation * Vector3.up * size * 0.5f,
							control.rotation * Vector3.up,
							size,
							Handles.CubeHandleCap,
							.001f);

						if (EditorGUI.EndChangeCheck())
						{
							Undo.RecordObject(target, "Moved Control point");
							control.position = point;
						}
					}

					break;
				case ControlPointType.Group:

					if (control.position != Vector3.zero || control.rotation.IsValid())
					{
						Handles.CircleHandleCap(0, control.position, control.rotation * Quaternion.Euler(90f, 0f, 0f), size * 5, EventType.Repaint);
						Handles.DrawLine(control.position, control.position + control.rotation * Vector3.forward * size * 5);
					}

					break;
				default:
					ErrorHandler.LogError(ErrorMessage.NotImplemented);
					break;
			}
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
				Undo.RecordObject(target, "Rotated Bone");
				bone.desiredRotation = rot;
			}
		}

		#endregion
	}
}

