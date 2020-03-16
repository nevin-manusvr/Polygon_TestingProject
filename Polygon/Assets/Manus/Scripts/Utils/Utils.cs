using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Polygon
{

	public static class Utils
	{
		#region stringUtils

		public static Transform FindDeepChildTransform(Transform trans, string[] childCriteria)
		{
			foreach (Transform child in trans)
			{
				if (StringContainsCriteria(child.name, childCriteria))
				{
					return child;
				}

				if (child.childCount > 0)
				{
					Transform foundTransform = FindDeepChildTransform(child, childCriteria);
					if (foundTransform != null)
					{
						return foundTransform;
					}
				}
			}
			return null;
		}

		public static bool StringContainsCriteria(string stringToCheck, string[] criteria)
		{
			int count = 0;
			foreach (var crit in criteria)
			{
				if (stringToCheck.ToUpper().Contains(crit.ToUpper()))
				{
					count++;
				}
			}
			return count == criteria.Length;
		}

		#endregion

		public static bool IsValid(this Quaternion rotation)
		{
			return !(Mathf.Approximately(rotation.x, 0)
			         && Mathf.Approximately(rotation.y, 0)
			         && Mathf.Approximately(rotation.z, 0)
			         && Mathf.Approximately(rotation.w, 0));
		}
	}
}