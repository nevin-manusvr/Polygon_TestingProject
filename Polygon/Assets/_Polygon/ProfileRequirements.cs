using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.Core.Utility;

namespace Manus.Polygon
{
	[CreateAssetMenu(fileName = "new Requirements", menuName = "ManusVR/Polygon/Calibration/Requirements")]
	public class ProfileRequirements : ScriptableObject
	{
		public VRTrackerType[] requiredTrackerOffsets =
			{
				VRTrackerType.Head, 
				VRTrackerType.LeftHand, 
				VRTrackerType.RightHand, 
				VRTrackerType.LeftFoot,
				VRTrackerType.RightFoot
			};
		public BodyMeasurements[] requiredBoneMeasurements =
			{
				BodyMeasurements.PlayerHeight,
				BodyMeasurements.SpineLength,
				BodyMeasurements.NeckLength,
				BodyMeasurements.ShoulderLength,
				BodyMeasurements.ArmLength,
				BodyMeasurements.LegLength,
				BodyMeasurements.HipWidth,
				BodyMeasurements.ShoulderWidth
			};
	}
}

