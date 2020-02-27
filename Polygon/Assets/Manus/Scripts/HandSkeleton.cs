using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Polygon.Skeleton
{
	public class HandSkeleton : MonoBehaviour
	{
		public HandBoneReferences handBoneReferences;

		#region Monobehaviour Callbacks



		#endregion

		public void PopulateBoneReferences()
		{
			handBoneReferences.PopulateBones(transform);
		}

		public void ClearBoneReferences()
		{
			handBoneReferences.ClearBoneReferences();
		}
	}
}

