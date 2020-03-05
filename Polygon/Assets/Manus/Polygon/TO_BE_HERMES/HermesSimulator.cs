using UnityEngine;
using HProt = Hermes.Protocol;

namespace Manus.ToBeHermes
{
	using System.Collections.Generic;

	using GlmSharp;

	using Hermes.Protocol.Polygon;
	using Hermes.Tools;

	public class HermesSimulator : MonoBehaviour
	{
		public static HermesSimulator instance;

		public SkeletonDefinitions[] m_SkeletonDefinitions;

		private void Awake()
		{
			if (instance == null)
				instance = this;
		}

		public void OnNewSkeletonDefinition(HProt.Polygon.InternalData _Poly)
		{
			for (var i = 0; i < _Poly.Skeleton.Skeletons.Count; i++)
			{
				if (i != 0) return;

				var bones = new Dictionary<BoneType, Bone>();
				var skeleton = _Poly.Skeleton.Skeletons[i];
				
				foreach (Bone bone in skeleton.Bones)
				{
					bones.Add(bone.Type, bone);
				}

				float armLength = glm.Distance(bones[BoneType.LeftHand].Position.toGlmVec3(), bones[BoneType.LeftLowerArm].Position.toGlmVec3()) +
				                  glm.Distance(bones[BoneType.LeftLowerArm].Position.toGlmVec3(), bones[BoneType.LeftUpperArm].Position.toGlmVec3());
				Debug.Log($"left arm length = {armLength}");
			}


			Debug.Log("Received polygon definitions");
		}
	}
}