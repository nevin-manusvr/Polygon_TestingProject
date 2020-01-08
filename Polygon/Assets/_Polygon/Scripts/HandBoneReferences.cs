using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManusVR.Polygon
{
	[System.Serializable]
	public struct Finger
	{
		public Bone proximal;
		public Bone middle;
		public Bone distal;
		public Bone tip;
	}

	[System.Serializable]
	public class HandBoneReferences
	{
		public Bone wrist;

		public Finger index;
		public Finger middle;
		public Finger ring;
		public Finger pinky;
		public Finger thumb;
	}
}

