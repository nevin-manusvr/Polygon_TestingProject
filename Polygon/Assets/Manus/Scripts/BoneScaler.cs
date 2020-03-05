using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Polygon
{
	public enum ScaleMode
	{
		Length,
		Percentage
	}

	public enum ScaleAxis
	{
		Length,
		Width,
		Height,
		Thickness,
		All
	}

	public class BoneScaler
	{
		private Transform scaleBone;
		private Transform[] scaleFixBones;

		private float defaultLength = 1;

		public float DefaultLength
		{
			get { return defaultLength; }
		}

		public BoneScaler(Transform bone, Quaternion lookRotation, Transform[] childBones)
		{
			// Instantiate scale bone
			//scaleBone = new GameObject(bone.name + "_scaleBone").transform;
			//scaleBone.SetParent(bone.parent);
			//scaleBone.position = bone.position;
			//scaleBone.rotation = lookRotation;

			//scaleBone.SetParent(bone, true);
			scaleBone = bone;

			// Instantiate scale fix bones
			scaleFixBones = new Transform[] { };

			if (childBones != null && childBones.Length > 0)
			{
				var scaleFixList = new List<Transform>();

				foreach (Transform child in childBones)
				{
					Transform scaleFixBone = new GameObject(child.name + "_scaleFixBone").transform;

					scaleFixBone.SetParent(scaleBone, true);
					scaleFixBone.position = child.position;
					scaleFixBone.rotation = lookRotation;

					scaleFixList.Add(scaleFixBone);

					child.SetParent(scaleFixBone);
				}

				scaleFixBones = scaleFixList.ToArray();
			}

			// Calculate the default bone length
			float smallestAngle = float.MaxValue;
			Transform lookAtChild = null;

			if (childBones != null)
			{
				foreach (Transform child in childBones)
				{
					float angleToChild = Vector3.Angle(child.position - bone.position, lookRotation * Vector3.forward);

					if (angleToChild < smallestAngle && angleToChild < 20)
					{
						smallestAngle = angleToChild;
						lookAtChild = child;
					}
				}

				if (lookAtChild != null)
				{
					defaultLength = Vector3.Project(lookAtChild.position - bone.position, lookRotation * Vector3.forward).magnitude;
				}
			}
		}

		public void ScaleBone(float scale, ScaleAxis axis, ScaleMode mode)
		{
			float size = scale;

			if (axis == ScaleAxis.Length && mode == ScaleMode.Length)
			{
				size = scale / defaultLength;
			}

			switch (axis)
			{
				case ScaleAxis.Length:
					scaleBone.localScale = new Vector3(scaleBone.localScale.x, scaleBone.localScale.y, size);
					foreach (Transform scaleFixBone in scaleFixBones)
					{
						scaleFixBone.localScale = new Vector3(scaleFixBone.localScale.x, scaleFixBone.localScale.y, 1 / size);
					}
					break;
				case ScaleAxis.Width:
					scaleBone.localScale = new Vector3(size, scaleBone.localScale.y, scaleBone.localScale.z);
					foreach (Transform scaleFixBone in scaleFixBones)
					{
						scaleFixBone.localScale = new Vector3(1f / size, scaleFixBone.localScale.y, scaleFixBone.localScale.z);
					}
					break;
				case ScaleAxis.Height:
					scaleBone.localScale = new Vector3(scaleBone.localScale.x, size, scaleBone.localScale.z);
					foreach (Transform scaleFixBone in scaleFixBones)
					{
						scaleFixBone.localScale = new Vector3(scaleFixBone.localScale.x, 1f / size, scaleFixBone.localScale.z);
					}
					break;
				case ScaleAxis.Thickness:
					scaleBone.localScale = new Vector3(size, size, scaleBone.localScale.z);
					foreach (Transform scaleFixBone in scaleFixBones)
					{
						scaleFixBone.localScale = new Vector3(1f / size, 1f / size, scaleFixBone.localScale.z);
					}
					break;
				case ScaleAxis.All:
					scaleBone.localScale = new Vector3(size, size, size);
					foreach (Transform scaleFixBone in scaleFixBones)
					{
						scaleFixBone.localScale = new Vector3(1f / size, 1f / size, 1f / size);
					}
					break;
			}
		}
	}
}