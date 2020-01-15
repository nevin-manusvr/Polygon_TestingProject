using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManusVR.Polygon
{
	public class SkeletonScaler
	{
		public Dictionary<HumanBodyBones, BoneScaler> boneScalers;

		public SkeletonScaler()
		{
			boneScalers = new Dictionary<HumanBodyBones, BoneScaler>();
		}

		public void GenerateScalerBonesForBody(SkeletonBoneReferences bones, SkeletonBoneReferences boneRotations)
		{

		}

		public void AddScalerBone(HumanBodyBones boneName, Transform bone, Quaternion boneLookRotation)
		{
			boneScalers.Add(boneName, new BoneScaler(bone, boneLookRotation));
		}
	}
}

