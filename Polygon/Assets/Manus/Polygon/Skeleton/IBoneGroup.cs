using System.Collections.Generic;
using Hermes.Protocol.Polygon;

namespace Manus.Polygon.Skeleton
{
	public interface IBoneGroup
	{
		Dictionary<BoneType, Bone> GatherBones(GatherType gatherType);

		Dictionary<ControlBoneType, ControlBone> GatherControlBones(GatherType gatherType);
	}
}

