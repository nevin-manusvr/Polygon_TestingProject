using System.Collections;
using System.Collections.Generic;
using Hermes.Protocol.Polygon;
using Google.Protobuf.Collections;
using HProt = Hermes.Protocol;

using UnityEngine;
using Manus.ToBeHermes.IK;

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

		public void AddGrave(Skeleton _Skeleton)
		{
			if (!IsSkeletonValid(_Skeleton))
			{
				Debug.LogWarning("Skeleton not valid");
				return;
			}

			if (ContainsGrave(_Skeleton.DeviceID))
			{
				Debug.LogWarning("Skeleton already added, replace old one");
				RemoveGrave(_Skeleton.DeviceID);
			}

			graves.Add(new Grave(_Skeleton));
		}

		public bool RemoveGrave(uint _ID)
		{
			if (!ContainsGrave(_ID))
			{
				Debug.LogWarning("No skeleton to remove");
				return false;
			}

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

		public bool ContainsGrave(uint _ID)
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

		public Grave GetGrave(uint _ID)
		{
			foreach (var t_Grave in graves)
			{
				if (t_Grave.ID == _ID)
				{
					return t_Grave;
				}
			}

			Debug.LogWarning("No skeleton found");
			return null;
		}

		private static bool IsSkeletonValid(Skeleton _Skeleton)
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
