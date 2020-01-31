using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.Core.Utility;
using System.Linq;

namespace Manus.Polygon
{
	[CreateAssetMenu(fileName = "new Arc Calibration Step", menuName = "ManusVR/Polygon/Calibration/Arc Calibration Step", order = 10)]
	public class ArcCalibrationStep : CalibrationStep
	{
		public ArcSettings[] arcSettings;
		public ArcDataToSave[] arcData;
		private Arc[] arcArray;

		public override void Setup(CalibrationProfile profile, TrackerReference trackers)
		{
			base.Setup(profile, trackers);

			arcArray = new Arc[arcSettings.Length];

			for (var i = 0; i < arcSettings.Length; i++)
			{
				arcArray[i] = arcSettings[i].useParentTracker
								  ? new Arc(trackers, arcSettings[i].parentTracker)
					              : arcArray[i] = new Arc();
			}
		}

		protected override void Update()
		{
			for (int i = 0; i < arcSettings.Length; i++)
			{
				TransformValues? trackerTransform = trackers.GetTracker(arcSettings[i].tracker);
				if (arcSettings[i].useParentTracker)
				{
					if (profile.trackerOffsets.ContainsKey(arcSettings[i].localOffset))
					{
						Vector3 localOffset = profile.trackerOffsets[arcSettings[i].localOffset].positionOffset ?? Vector3.zero;
						trackerTransform = trackers.GetTrackerWithOffset(arcSettings[i].tracker, localOffset, Quaternion.identity);
					}
				}
				
				if (trackerTransform == null) 
					return;

				arcArray[i].AddMeasurement(trackerTransform.Value.position);
			}
		}

		protected override void End()
		{
			for (int i = 0; i < arcSettings.Length; i++)
			{
				arcArray[i].CalculateArc();
			}

			foreach (ArcDataToSave data in arcData)
			{
				switch (data.dataType)
				{
					case ArcDataType.OffsetToTracker:

						profile.AddTrackerOffset(data.trackerOffset, arcArray[data.arcPositionIndex].GetOffsetToTracker());

						break;
					case ArcDataType.Length:

						// TODO: TMP
						foreach (int index in data.arcMeasurementIndices)
						{
							GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
							sphere.transform.position = arcArray[index].IntersectionPoint;
							sphere.transform.localScale = new Vector3(.05f, .05f, .05f);
						}

						float value = data.arcMeasurementIndices.Sum(arc => arcArray[arc].GetArcRadius()) / data.arcMeasurementIndices.Length;
						profile.AddBodyMeasurement(data.measurement, value);

						break;
					case ArcDataType.Distance:

						Vector3 point1 = Vector3.zero;
						Vector3 point2 = Vector3.zero;

						switch (data.point1.pointType)
						{
							case ArcPointType.ArcPoint:
								point1 = arcArray[data.point1.arcIndex].IntersectionPoint;
								break;
							case ArcPointType.Tracker:
								point1 = Vector3.zero; // TODO: calculate local offset to tracker
								break;
						}
						switch (data.point2.pointType)
						{
							case ArcPointType.ArcPoint:
								point2 = arcArray[data.point2.arcIndex].IntersectionPoint;
								break;
							case ArcPointType.Tracker:
								point2 = Vector3.zero; // TODO: calculate local offset to tracker
								break;
						}

						float distance = Vector3.Distance(point1, point2);

						profile.AddBodyMeasurement(data.distanceMeasurement, distance);

						break;
					case ArcDataType.Direction:
						
						Vector3 normal = arcArray[data.arcDirectionIndex].GetArcNormalFromTracker();
						profile.AddTrackerDirection(arcSettings[data.arcDirectionIndex].parentTracker, data.directionAxis, normal);

						break;
				}
			}
		}

		#region enums

		public enum ArcDataType
		{
			OffsetToTracker,
			Length,
			Distance,
			Direction
		}

		public enum ArcPointType
		{
			ArcPoint,
			Tracker
		}

		#endregion

		#region classes

		[System.Serializable]
		public class ArcSettings
		{
			public VRTrackerType tracker = VRTrackerType.Waist;
			public bool useTrackerLocal = false;
			public OffsetsToTrackers localOffset;

			public bool useParentTracker = false;
			public VRTrackerType parentTracker;
		}

		[System.Serializable]
		public class ArcDataToSave
		{
			public ArcDataType dataType;

			// OffsetToTracker
			public OffsetsToTrackers trackerOffset;
			public int arcPositionIndex;

			// Measurement
			public BodyMeasurements measurement;
			public int[] arcMeasurementIndices;

			// Distance
			public BodyMeasurements distanceMeasurement;
			public ArcPoint point1;
			public ArcPoint point2;

			// Direction
			public Axis directionAxis;
			public int arcDirectionIndex;
		}

		[System.Serializable]
		public class ArcPoint
		{
			public ArcPointType pointType;

			// Arc point
			public int arcIndex;

			// Tracker
			public VRTrackerType tracker;
			public bool useLocalOffset;
			public OffsetsToTrackers offset;
		}

		#endregion
	}
}