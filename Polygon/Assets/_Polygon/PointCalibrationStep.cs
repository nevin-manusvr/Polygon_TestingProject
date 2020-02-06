using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.Core.Utility;

namespace Manus.Polygon
{

	[CreateAssetMenu(fileName = "new Point Calibration Step", menuName = "ManusVR/Polygon/Calibration/Point Calibration Step", order = 10)]
	public class PointCalibrationStep : CalibrationStep
	{
		public Data[] pointData;

		protected override void End()
		{
			foreach (Data data in pointData)
			{
				switch (data.type)
				{
					case PointDataType.AverageTrackerPoint:

						Vector3 averagePoint = Vector3.zero;
						foreach (Point point in data.points)
						{
							averagePoint += Point.GetPoint(point, trackers, profile);
						}

						averagePoint /= data.points.Length;

						TransformValues? pointTrackerTransform = trackers.GetTracker(data.pointTracker);
						if (pointTrackerTransform == null)
						{
							Debug.LogError("tracker not connected");
							continue;
						}

						Matrix4x4 pointTrackerMatrix = Matrix4x4.TRS(pointTrackerTransform.Value.position, pointTrackerTransform.Value.rotation, Vector3.one);
						profile.AddTrackerOffset(data.pointTrackerOffset, pointTrackerMatrix.inverse.MultiplyPoint3x4(averagePoint));

						break;
					default:
						Debug.LogError("Implement your shit");
						break;
				}
			}
		}

		#region enums

		public enum PointDataType
		{
			AverageTrackerPoint
		}

		public enum PointType
		{
			Tracker
		}

		#endregion

		#region MyRegion

		[System.Serializable]
		public class Data
		{
			public PointDataType type;

			[Header("AverageTrackerPoint")]
			public VRTrackerType pointTracker;
			public OffsetsToTrackers pointTrackerOffset;
			public Point[] points;
		}

		[System.Serializable]
		public class Point
		{
			public PointType type;

			[Header("Tracker")]
			public VRTrackerType tracker;
			public bool useTrackerLocal;
			public OffsetsToTrackers trackerOffset;

			public static Vector3 GetPoint(Point settings, TrackerReference trackers, CalibrationProfile profile)
			{

				switch (settings.type)
				{
					case PointType.Tracker:

						TransformValues? trackerTransform = trackers.GetTracker(settings.tracker);

						if (settings.useTrackerLocal)
						{
							if (!profile.trackerOffsets.ContainsKey(settings.trackerOffset)
							    || profile.trackerOffsets[settings.trackerOffset].position == null)
								break;

							trackerTransform = trackers.GetTrackerWithOffset(settings.tracker, profile.trackerOffsets[settings.trackerOffset].Position, Quaternion.identity);
						}

						if (trackerTransform == null) break;

						return trackerTransform.Value.position;

						break;
					default:
						Debug.LogError("Not Implemented");
						break;
				}

				Debug.LogError("Something went wrong here");

				return Vector3.zero;
			}
		}

		#endregion

	}
}