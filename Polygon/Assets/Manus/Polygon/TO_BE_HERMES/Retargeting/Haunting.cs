using System.Collections;
using System.Collections.Generic;
using Hermes.Protocol.Polygon;

namespace Manus.ToBeHermes 
{

	public class Haunting
	{
		private Skeleton m_Skeleton;

		private Dictionary<BoneType, Bone> m_Bones;

		public Haunting(Skeleton _Skeleton)
		{
			m_Bones = new Dictionary<BoneType, Bone>();
		}

		public void SetTargetSkeleton(Skeleton _TargetSkeleton)
		{

		}
	}
}

