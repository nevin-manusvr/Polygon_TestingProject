using System.Collections;
using System.Collections.Generic;
using Hermes.Protocol.Polygon;
using Google.Protobuf.Collections;
using HProt = Hermes.Protocol;

using UnityEngine;

namespace Manus.ToBeHermes
{

	[System.Serializable]
	public class Graveyard
	{
		public List<Grave> graves;

		public Graveyard()
		{
			graves = new List<Grave>();
		}

		public void AddGrave(HProt.Polygon.Skeleton _Skeleton)
		{
			if (!IsSkeletonValid(_Skeleton))
			{
				Debug.LogWarning("Skeleton not valid");
				return;
			}

			if (ContainsGrave((int)_Skeleton.DeviceID))
			{
				Debug.LogWarning("Skeleton already added, replace old one");
				RemoveGrave((int)_Skeleton.DeviceID);
			}

			graves.Add(new Grave(_Skeleton));
		}

		public bool RemoveGrave(int _ID)
		{
			for (int i = 0; i < graves.Count; i++)
			{
				if (graves[i].ID == _ID)
				{
					graves.RemoveAt(i);
					i--;
				}
			}

			return false;
		}

		public bool ContainsGrave(int _ID)
		{
			foreach (var grave in graves)
			{
				if (grave.ID == _ID)
				{
					return true;
				}
			}

			return false;
		}

		private static bool IsSkeletonValid(HProt.Polygon.Skeleton _Skeleton)
		{
			var t_RequiredBones = new BoneType[]
				                      {
					                      BoneType.Head,
					                      BoneType.Neck,
					                      BoneType.Hips,
					                      BoneType.Spine,
					                      BoneType.LeftUpperLeg,
					                      BoneType.RightUpperLeg,
					                      BoneType.LeftLowerLeg,
					                      BoneType.RightLowerLeg,
					                      BoneType.LeftFoot,
					                      BoneType.RightFoot,
					                      BoneType.LeftShoulder,
					                      BoneType.RightShoulder,
					                      BoneType.LeftUpperArm,
					                      BoneType.RightUpperArm,
					                      BoneType.LeftLowerArm,
					                      BoneType.RightLowerArm,
					                      BoneType.LeftHand,
					                      BoneType.RightHand
				                      };

			foreach (var t_Requirement in t_RequiredBones)
			{
				bool t_Found = false;
				foreach (var t_Bone in _Skeleton.Bones)
				{
					if (t_Bone.Type == t_Requirement)
					{
						t_Found = true;
						break;
					}
				}

				if (!t_Found)
					return false;
			}

			return true;
		}
	}
}
