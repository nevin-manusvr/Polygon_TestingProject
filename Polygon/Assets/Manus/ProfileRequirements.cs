using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.Core.Utility;
using Hermes.Protocol.Polygon;

namespace Manus.Polygon
{
	[System.Serializable]
	public class ProfileRequirements
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

		public ProfileRequirements(VRTrackerType[] trackers, BodyMeasurements[] measurements)
		{
			requiredTrackerOffsets = trackers;
			requiredBoneMeasurements = measurements;
		}
	}
}

