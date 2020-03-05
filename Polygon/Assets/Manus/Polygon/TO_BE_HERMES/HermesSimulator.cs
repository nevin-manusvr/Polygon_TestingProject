using UnityEngine;
using HProt = Hermes.Protocol;

namespace Manus.ToBeHermes
{
	using System.Collections.Generic;
	using GlmSharp;
	using Google.Protobuf.Collections;
	using Hermes.Protocol.Polygon;
	using Hermes.Tools;

	public class HermesSimulator : MonoBehaviour
	{
		public static HermesSimulator instance;

		public Graveyard graveyard;

		private void Awake()
		{
			if (instance == null)
				instance = this;

			graveyard = new Graveyard();
		}

		public void OnNewSkeletonDefinition(HProt.Polygon.InternalData _Poly)
		{
			Debug.Log("Received polygon definitions");

			foreach (var skeleton in _Poly.Skeleton.Skeletons)
			{
				graveyard.AddGrave(skeleton);
			}
			
			//for (var i = 0; i < _Poly.Skeleton.Skeletons.Count; i++)
			//{
			//	if (i != 0) return;

			//	// Debug.Log($"Skeleton is valid : {IsDefinitionValid(_Poly.Skeleton.Skeletons[i].Bones)}");

			//	var bones = new Dictionary<BoneType, Bone>();
			//	var skeleton = _Poly.Skeleton.Skeletons[i];
				
			//	foreach (Bone bone in skeleton.Bones)
			//	{
			//		bones.Add(bone.Type, bone);
			//	}

			//	float armLength = glm.Distance(bones[BoneType.LeftHand].Position.toGlmVec3(), bones[BoneType.LeftLowerArm].Position.toGlmVec3()) +
			//	                  glm.Distance(bones[BoneType.LeftLowerArm].Position.toGlmVec3(), bones[BoneType.LeftUpperArm].Position.toGlmVec3());
			//	Debug.Log($"left arm length = {armLength}");
			//}
		}
	}
}