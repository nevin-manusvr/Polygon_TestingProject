using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.Core.Utility;

namespace Manus.Polygon
{
	[CreateAssetMenu(fileName = "new Calibration Profile", menuName = "ManusVR/Polygon/Calibration/Profile", order = 1)]
	public class CalibrationProfile : ScriptableObject
	{
		public Dictionary<VRTrackerType, TrackerOffset> trackerOffsets;
		public Dictionary<BodyMeasurements, float> bodyMeasurements;

		public void AddTrackerOffset(VRTrackerType trackerType, Vector3 localPosition, Quaternion localRotation)
		{

		}

		public void RemoveTrackerOffset(VRTrackerType trackerType)
		{

		}
	}

	[System.Serializable]
	public class TrackerOffset
	{
		public Vector3 trackerOffset;
		public Quaternion rotationOffset;
	}
}
