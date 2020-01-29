using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.Core.Utility;

namespace Manus.Polygon
{
	using System.Linq;

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

		public override IEnumerator Start()
		{
			float timer = 0;

			while (timer < time)
			{
				timer += Time.deltaTime;
				Update();
				yield return new WaitForEndOfFrame();
			}

			End();
		}

		protected override void Update()
		{
			for (int i = 0; i < arcSettings.Length; i++)
			{
				arcArray[i].AddMeasurement(trackers.GetTracker(arcSettings[i].tracker).position);
			}
		}

		protected override void End()
		{
			for (int i = 0; i < arcSettings.Length; i++)
			{
				arcArray[i].CalculateArc();
				GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphere.transform.position = arcArray[i].IntersectionPoint;
				sphere.transform.localScale = new Vector3(.05f, .05f, .05f);
			}

			foreach (ArcDataToSave data in arcData)
			{
				switch (data.dataType)
				{
					case ArcDataType.OffsetToTracker:

						profile.AddTrackerOffset(data.trackerOffset, arcArray[data.arcPositionIndex].GetOffsetToTracker());

						break;
					case ArcDataType.Length:

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

						//switch (data.directionType)
						//{
						//	case ArcDirectionType.ArcNormal:

						//		Vector3 arcNormal = data.arcDirectionIndices.Aggregate(Vector3.zero, (current, index) => current + arcArray[index].planeNormal)
						//		                    / data.arcDirectionIndices.Length;

								
						//		break;
						//}

						break;
				}
			}
		}

		public override void Revert()
		{
			// TODO: implement this
		}
		
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
			public VRTrackerType trackerType;
			public ArcDirection arcDirection;
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

		[System.Serializable]
		public class ArcDirection
		{
			public ArcDirectionType directionType;

			[Header("Arc normal")]
			public int[] arcDirectionIndices;
		}

		public enum ArcPointType
		{
			ArcPoint,
			Tracker
		}

		public enum ArcDirectionType
		{
			ArcNormal
		}

		public enum ArcDataType
		{
			OffsetToTracker,
			Length,
			Distance,
			Direction
		}
	}
}