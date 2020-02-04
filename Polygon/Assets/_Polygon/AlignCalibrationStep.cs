using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.Core.Utility;

namespace Manus.Polygon
{
	[CreateAssetMenu(fileName = "new Align Calibration Step", menuName = "ManusVR/Polygon/Calibration/Align Calibration Step", order = 10)]
	public class AlignCalibrationStep : CalibrationStep
	{
		public AxisData[] axisData;
		public OffsetData[] offsetData;

		//public override void Setup(CalibrationProfile profile, TrackerReference trackers)
		//{
		//	base.Setup(profile, trackers);
		//}

		protected override void End()
		{
			foreach (AxisData data in axisData)
			{
				switch (data.dataType)
				{
					case AlignType.GetAxis:

						Vector3 axisDirection = DirectionClosestTo.GetDirection(data.axisDirectionToGet, trackers, profile);

						TransformValues? getAxisTracker = trackers.GetTracker(data.getAxisTracker);

						if (getAxisTracker == null)
						{
							Debug.LogError("Not all trackers are connected");
							continue;
						}

						Matrix4x4 getAxisTrackerMatrix = Matrix4x4.TRS(getAxisTracker.Value.position, getAxisTracker.Value.rotation, Vector3.one);
						getAxisTrackerMatrix = getAxisTrackerMatrix.inverse;

						profile.AddTrackerDirection(data.getAxisTracker, data.axisToGet, getAxisTrackerMatrix.MultiplyVector(axisDirection));

						break;
					case AlignType.AverageTwoAxis:

						TransformValues? averageTracker1 = trackers.GetTracker(data.averageTracker1);
						TransformValues? averageTracker2 = trackers.GetTracker(data.averageTracker2);

						if (averageTracker1 == null || averageTracker2 == null)
						{
							Debug.LogError("Not all trackers are connected");
							continue;
						}

						if (!profile.trackerDirections.ContainsKey(data.averageTracker1) || !profile.trackerDirections.ContainsKey(data.averageTracker2)
						    || profile.trackerDirections[data.averageTracker1].GetAxis(data.averageAxis1) == null || profile.trackerDirections[data.averageTracker2].GetAxis(data.averageAxis2) == null)
						{
							Debug.LogError("Not all trackers have required directions");
							continue;
						}

						Vector3 averageTracker1Dir = (Vector3)profile.trackerDirections[data.averageTracker1].GetAxis(data.averageAxis1) * (data.averageAxisInvert1 ? -1 : 1);
						Vector3 averageTracker2Dir = (Vector3)profile.trackerDirections[data.averageTracker2].GetAxis(data.averageAxis2) * (data.averageAxisInvert2 ? -1 : 1);

						Matrix4x4 averageTracker1Matrix = Matrix4x4.TRS(averageTracker1.Value.position, averageTracker1.Value.rotation, Vector3.one);
						Matrix4x4 averageTracker2Matrix = Matrix4x4.TRS(averageTracker2.Value.position, averageTracker2.Value.rotation, Vector3.one);

						averageTracker1Dir = averageTracker1Matrix.MultiplyVector(averageTracker1Dir);
						averageTracker2Dir = averageTracker2Matrix.MultiplyVector(averageTracker2Dir);

						Vector3 averageTrackerDir = (averageTracker1Dir + averageTracker2Dir) / 2f;

						profile.AddTrackerDirection(data.averageTracker1, data.averageAxis1, averageTracker1Matrix.inverse.MultiplyVector(averageTrackerDir));
						profile.AddTrackerDirection(data.averageTracker2, data.averageAxis2, averageTracker2Matrix.inverse.MultiplyVector(averageTrackerDir));

						break;
					case AlignType.CalculateAxis:

						TransformValues? calculateTrackerTransform = trackers.GetTracker(data.calculateTracker);
						
						if (calculateTrackerTransform == null)
						{
							Debug.LogError("Not all trackers are connected");
							continue;
						}

						Axis[] axisToUse = { };
						switch (data.axisToCalculate)
						{
							case Axis.X:
								axisToUse = new[] { Axis.Y, Axis.Z };
								break;
							case Axis.Y:
								axisToUse = new[] { Axis.X, Axis.Z };
								break;
							case Axis.Z:
								axisToUse = new[] { Axis.X, Axis.Y };
								break;
						}

						if (!profile.trackerDirections.ContainsKey(data.calculateTracker)
						    || profile.trackerDirections[data.calculateTracker].GetAxis(axisToUse[0]) == null || profile.trackerDirections[data.calculateTracker].GetAxis(axisToUse[1]) == null)
						{
							Debug.LogError("Tracker doesn't have required directions");
							continue;
						}

						Matrix4x4 calucalteTrackerMatrix = Matrix4x4.TRS(calculateTrackerTransform.Value.position, calculateTrackerTransform.Value.rotation, Vector3.one);
						Vector3 calculatedAxis = Vector3.Cross(
							profile.trackerDirections[data.calculateTracker].GetAxis(axisToUse[0]).Value,
							profile.trackerDirections[data.calculateTracker].GetAxis(axisToUse[1]).Value);

						if (DirectionClosestTo.CheckIfDirectionShouldBeInverted(data.calculateDirectionClosestTo, trackers, profile, calucalteTrackerMatrix.MultiplyVector(calculatedAxis)))
						{
							calculatedAxis *= -1;
						}

						profile.AddTrackerDirection(data.calculateTracker, data.axisToCalculate, calculatedAxis);

						break;
					default:
						Debug.LogError("Selected align setting type is not implemented");
						break;
				}
			}

			foreach (OffsetData data in offsetData)
			{
				switch (data.type)
				{
					case OffsetType.AverageOffsetsOnPlane:

						if (!profile.trackerOffsets.ContainsKey(data.offsetTracker1) || !profile.trackerOffsets.ContainsKey(data.offsetTracker2)
						    || profile.trackerOffsets[data.offsetTracker1].position == null || profile.trackerOffsets[data.offsetTracker2].position == null)
						{
							Debug.LogError("Not all trackers have required offsets");
						}

						TrackerOffset offsetTracker1 = profile.trackerOffsets[data.offsetTracker1];
						TrackerOffset offsetTracker2 = profile.trackerOffsets[data.offsetTracker2];

						TransformValues? offsetTrackerTransform1 = trackers.GetTrackerWithOffset(data.offsetFromTracker1, offsetTracker1.Position, Quaternion.identity);
						TransformValues? offsetTrackerTransform2 = trackers.GetTrackerWithOffset(data.offsetFromTracker2, offsetTracker2.Position, Quaternion.identity);

						if (offsetTrackerTransform1 == null || offsetTrackerTransform2 == null)
						{
							Debug.LogError("Not all trackers have required directions");
							continue;
						}



						break;
					default:
						Debug.LogError("Implement your shit");
						break;
				}
			}
		}

		#region enums

		// Axis enums
		public enum AlignType
		{
			GetAxis,
			AverageTwoAxis,
			CalculateAxis
		}

		// Offset enums
		public enum OffsetType
		{
			AverageOffsetsOnPlane
		}

		public enum DirectionClosestType
		{
			SingleDirection,
			Cross,
		}

		public enum DirectionType
		{
			WorldDirection,
			TrackerDirection
		}

		#endregion

		#region classes

		[System.Serializable]
		public class AxisData
		{
			public AlignType dataType;

			[Header("Get Axis")]
			public VRTrackerType getAxisTracker;
			public Axis axisToGet;
			public DirectionClosestTo axisDirectionToGet;

			[Header("Average Two Axis")]
			public VRTrackerType averageTracker1;
			public Axis averageAxis1;
			public bool averageAxisInvert1;
			public VRTrackerType averageTracker2;
			public Axis averageAxis2;
			public bool averageAxisInvert2;

			[Header("Calculate axis")]
			public VRTrackerType calculateTracker;
			public Axis axisToCalculate;
			public DirectionClosestTo calculateDirectionClosestTo;
		}

		// Offset classes

		[System.Serializable]
		public class OffsetData
		{
			public OffsetType type;

			[Header("Average offsets on plane")]
			public VRTrackerType offsetFromTracker1;
			public OffsetsToTrackers offsetTracker1;
			public VRTrackerType offsetFromTracker2;
			public OffsetsToTrackers offsetTracker2;
			public Axis offsetPlaneNormal;
		}

		[System.Serializable]
		public class DirectionClosestTo // TODO: make editor script
		{
			public DirectionClosestType type;

			[Header("Single Direction")]
			public Direction singleDirection;

			[Header("Cross")]
			public Direction crossDirection1;
			public Direction crossDirection2;

			public static Vector3 GetDirection(DirectionClosestTo settings, TrackerReference trackers, CalibrationProfile profile)
			{
				switch (settings.type)
				{
					case DirectionClosestType.SingleDirection:
						return Direction.GetDirection(settings.singleDirection, trackers, profile);
					case DirectionClosestType.Cross:
						Vector3 crossDir1 = Direction.GetDirection(settings.crossDirection1, trackers, profile);
						Vector3 crossDir2 = Direction.GetDirection(settings.crossDirection2, trackers, profile);
						return Vector3.Cross(crossDir1, crossDir2);
					default:
						Debug.LogError("Implement your shit");
						break;
				}

				return Vector3.zero;
			}

			public static bool CheckIfDirectionShouldBeInverted(DirectionClosestTo settings, TrackerReference trackers, CalibrationProfile profile, Vector3 worldDirection)
			{
				switch (settings.type)
				{
					case DirectionClosestType.SingleDirection:
						Vector3 dir = Direction.GetDirection(settings.singleDirection, trackers, profile);

						if (Vector3.Distance(dir, worldDirection) > Vector3.Distance(dir, worldDirection * -1))
						{
							return true;
						}

						break;
					case DirectionClosestType.Cross:

						Vector3 crossDir1 = Direction.GetDirection(settings.crossDirection1, trackers, profile);
						Vector3 crossDir2 = Direction.GetDirection(settings.crossDirection2, trackers, profile);
						Vector3 cross = Vector3.Cross(crossDir1, crossDir2);

						if (Vector3.Distance(cross, worldDirection) > Vector3.Distance(cross, worldDirection * -1))
						{
							return true;
						}

						break;
					default:
						Debug.LogError("Implement your shit");
						break;
				}

				return false;
			}
		}

		[System.Serializable]
		public class Direction
		{
			public DirectionType type;

			[Header("WorldDirection")]
			public Vector3 worldDirection;

			[Header("TrackerDirection")]
			public VRTrackerType trackerFrom;
			public bool trackerFromLocal;
			public OffsetsToTrackers trackerFromLocalOffset;
			public VRTrackerType trackerTo;
			public bool trackerToLocal;
			public OffsetsToTrackers trackerToLocalOffset;

			public static Vector3 GetDirection(Direction settings, TrackerReference trackers, CalibrationProfile profile)
			{
				switch (settings.type)
				{
					case DirectionType.WorldDirection:
						return settings.worldDirection;
					case DirectionType.TrackerDirection:
						TransformValues? trackerFrom = trackers.GetTracker(settings.trackerFrom);
						if (settings.trackerFromLocal)
						{
							if (profile.trackerOffsets.ContainsKey(settings.trackerFromLocalOffset)
							    && profile.trackerOffsets[settings.trackerFromLocalOffset].position != null)
							{
								trackerFrom = trackers.GetTrackerWithOffset(settings.trackerFrom, profile.trackerOffsets[settings.trackerFromLocalOffset].Position, Quaternion.identity);
							}
						}

						TransformValues? trackerTo = trackers.GetTracker(settings.trackerTo);
						if (settings.trackerToLocal)
						{
							if (profile.trackerOffsets.ContainsKey(settings.trackerToLocalOffset)
							    && profile.trackerOffsets[settings.trackerToLocalOffset].position != null)
							{
								trackerTo = trackers.GetTrackerWithOffset(settings.trackerTo, profile.trackerOffsets[settings.trackerToLocalOffset].Position, Quaternion.identity);
							}
						}

						if (trackerFrom == null || trackerTo == null)
						{
							Debug.LogError("Not all trackers are connected");
							break;
						}

						return trackerTo.Value.position - trackerFrom.Value.position;
				}

				return Vector3.zero;
			}
		}

		#endregion
	}
}