using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Polygon
{
	public abstract class CalibrationStep : ScriptableObject
	{
		public string name;
		[Range(0, 5)] public float time = 1f;

		public abstract void Start();

		//public virtual void Update();

		//public virtual void End();
	}
}

