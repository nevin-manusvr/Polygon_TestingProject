using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Polygon
{
	public struct TransformValues
	{
		public Vector3 position;
		public Quaternion rotation;

		public TransformValues(Transform transform)
		{
			this.position = transform.position;
			this.rotation = transform.rotation;
		}

		public TransformValues(Vector3 position, Quaternion rotation)
		{
			this.position = position;
			this.rotation = rotation;
		}
	}
}
