using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManusVR.Polygon
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

		private float defaultLength;

		public BoneScaler(Transform bone, Quaternion lookRotation)
		{
			// Instantiate scale bone
			scaleBone = new GameObject(bone.name + "_scaleBone").transform;
			scaleBone.SetParent(bone.parent);
			scaleBone.position = bone.position;
			scaleBone.rotation = lookRotation;

			bone.SetParent(scaleBone, true);

			// Instantiate scale fix bones
			var childs = new Transform[bone.childCount];
			for (int i = 0; i < bone.childCount; i++)
			{
				childs[i] = bone.GetChild(i);
			}

			var scaleFixList = new List<Transform>();

			foreach (Transform child in childs)
			{
				Transform scaleFixBone = new GameObject(child.name + "_scaleFixBone").transform;

				scaleFixBone.SetParent(bone, true);
				scaleFixBone.position = child.position;
				scaleFixBone.rotation = lookRotation;

				scaleFixList.Add(scaleFixBone);

				child.SetParent(scaleFixBone);
			}

			scaleFixBones = scaleFixList.ToArray();
		}

		public void ScaleBone(float scale, ScaleMode mode, ScaleAxis axis)
		{
			float size = scale;

			scaleBone.localScale = new Vector3(scaleBone.localScale.x, scaleBone.localScale.y, size);
			foreach (Transform scaleFixBone in scaleFixBones)
			{
				scaleFixBone.localScale = new Vector3(scaleBone.localScale.x, scaleBone.localScale.y, 1 / size);
			}
		}
	}
}