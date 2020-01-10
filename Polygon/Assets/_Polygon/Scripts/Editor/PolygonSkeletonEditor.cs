using UnityEditor;
using UnityEngine;

namespace ManusVR.Polygon
{
	[CustomEditor(typeof(PolygonSkeleton))]
	public class PolygonSkeletonEditor : Editor
	{
		private float size = 0.03f;

		private Color handlesColor = Color.cyan;

		void OnSceneGUI()
		{
			PolygonSkeleton skeleton = target as PolygonSkeleton;
			SkeletonBoneReferences bones = skeleton.boneReferences;

			DrawHumanoidSkeletonEditor(bones);
		}

		private void DrawHumanoidSkeletonEditor(SkeletonBoneReferences bones)
		{
			Handles.color = handlesColor;

			DrawBodyAndHead(bones.body, bones.head);

			DrawLeg(bones.legLeft, bones.body);
			DrawLeg(bones.legRight, bones.body);

			DrawArm(bones.armLeft, bones.body);
			DrawArm(bones.armRight, bones.body);
		}

		private void DrawBodyAndHead(Body body, Head head)
		{
			DrawBone(body.hip, size);
			foreach (Bone bone in body.spine)
			{
				DrawBone(bone, size);
			}

			DrawBone(head.neck, size);
			DrawBone(head.head, size);
			DrawBone(head.eyeLeft, size);
			DrawBone(head.eyeRight, size);

			// Connect Bones
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
		}

		private void DrawArm(Arm arm, Body body)
		{
			DrawHand(arm.hand);

			DrawBone(arm.shoulder, size);
			DrawBone(arm.upperArm, size);
			DrawBone(arm.lowerArm, size);

			// Connect the bones
			ConnectBones(arm.lowerArm, arm.hand.wrist);
			ConnectBones(body.spine[body.spine.Length - 1], arm.shoulder);
			ConnectBones(arm.shoulder, arm.upperArm);
			ConnectBones(arm.upperArm, arm.lowerArm);
		}
		
		private void DrawLeg(Leg leg, Body body)
		{
			DrawBone(leg.upperLeg, size);
			DrawBone(leg.lowerLeg, size);
			DrawBone(leg.foot, size);
			DrawBone(leg.toes, size);
			DrawBone(leg.toesEnd, size);

			// Connect the bones
			ConnectBones(body.hip, leg.upperLeg);
			ConnectBones(leg.upperLeg, leg.lowerLeg);
			ConnectBones(leg.lowerLeg, leg.foot);
			ConnectBones(leg.foot, leg.toes);
			ConnectBones(leg.toes, leg.toesEnd);
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
			DrawBone(finger.proximal, size / 3f);
			DrawBone(finger.middle, size / 3f);
			DrawBone(finger.distal, size / 3f);
			DrawBone(finger.tip, size / 3f);

			// Connect Bones
			ConnectBones(hand.wrist, finger.proximal);
			ConnectBones(finger.proximal, finger.middle);
			ConnectBones(finger.middle, finger.distal);
			ConnectBones(finger.distal, finger.tip);
		}


		private void DrawBone(Bone bone, float size)
		{
			if (Handles.Button(bone.bone.position, Quaternion.identity, size, size, Handles.SphereHandleCap))
			{
				Selection.activeGameObject = bone.bone.gameObject;
			}
		}

		private void ConnectBones(Bone bone1, Bone bone2)
		{
			Handles.DrawLine(bone1.bone.position, bone2.bone.position);
		}
	}
}

