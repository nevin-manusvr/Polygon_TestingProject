﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.Core.Utility;
using System.Linq;
using System;
using Hermes.Protocol.Polygon;

namespace Manus.Polygon
{

	[CreateAssetMenu(fileName = "new Arc Calibration Step", menuName = "ManusVR/Polygon/Calibration/Arc Calibration Step", order = 10)]
	public class ArcCalibrationStep : CalibrationStep
	{
		public Settings[] settings;
		public Data[] allData;
		private Arc[] arcArray;

		public override void Setup(CalibrationProfile profile, TrackerReference trackers, Action finishCallback = null)
		{
			base.Setup(profile, trackers, finishCallback);

			arcArray = new Arc[settings.Length];

			for (var i = 0; i < settings.Length; i++)
			{
				arcArray[i] = settings[i].useParentTracker
								  ? new Arc(trackers, settings[i].parentTracker, false, name)
					              : arcArray[i] = new Arc();
			}
		}

		protected override void Update()
		{
			for (int i = 0; i < settings.Length; i++)
			{
				Vector3 offset = Vector3.zero;
				if (settings[i].useTrackerLocal)
				{
					if (profile.trackerOffsets.ContainsKey(settings[i].localOffset))
					{
						offset = profile.trackerOffsets[settings[i].localOffset].position ?? Vector3.zero; 
					}
				}
				
				if (!trackers.GetTrackerWithOffset(settings[i].tracker, offset, Quaternion.identity, out TransformValues trackerTransform))
					return;

				arcArray[i].AddMeasurement(trackerTransform.position);
			}
		}

		protected override void End()
		{
			for (int i = 0; i < settings.Length; i++)
			{
				arcArray[i].CalculateArc();
			}

			// Order data to save and process it
			Data[] data = allData.OrderBy(value => (int)value.dataType).ToArray();
			ProcessData(data);
		}

		private void ProcessData(Data[] arcData)
		{
			foreach (Data data in arcData)
			{
				switch (data.dataType)
				{
					case DataType.OffsetToTracker:

						// Add offset on local plane
						if (data.onLocalPlane)
						{
							// if profile doesn't contain a offset yet, just add it normally
							if (profile.trackerOffsets.ContainsKey(data.trackerOffset) && profile.trackerOffsets[data.trackerOffset].position != null)
							{
								TrackerOffset offset = profile.trackerOffsets[data.trackerOffset];
								VRTrackerType trackerType = (VRTrackerType)CalibrationProfile.GetMatchingTrackerFromOffset(data.trackerOffset);
								
								// if profile doesn't contain the required axis ignore
								if (profile.trackerDirections.ContainsKey(trackerType) && profile.trackerDirections[trackerType].GetAxis(data.localPlane) != null)
								{
									if (!trackers.GetTracker(trackerType, out TransformValues trackerTransform))
									{
										ErrorHandler.LogError(ErrorMessage.NoTrackerData);
										continue;
									}
									Matrix4x4 trackerMatrix = Matrix4x4.TRS(trackerTransform.position, trackerTransform.rotation, Vector3.one);
									Matrix4x4 inverseTrackerMatrix = trackerMatrix.inverse;

									Vector3 localAxis = trackerMatrix.MultiplyVector(profile.trackerDirections[trackerType].GetAxis(data.localPlane) ?? Vector3.zero);
									Vector3 newPositionDirection = trackerMatrix.MultiplyPoint3x4(arcArray[data.arcPositionIndex].GetOffsetToTracker()) - trackerMatrix.MultiplyPoint3x4((Vector3)offset.position);
									Vector3 newPosition = trackerMatrix.MultiplyPoint3x4((Vector3)offset.position) + Vector3.ProjectOnPlane(newPositionDirection, localAxis);
									
									newPosition = inverseTrackerMatrix.MultiplyPoint3x4(newPosition);

									arcArray[data.arcPositionIndex].GetOffsetToTracker();

									profile.AddTrackerOffset(data.trackerOffset, newPosition);

									continue;
								}

								Debug.LogError($"Could not apply local offset to {trackerType}, plane axis is not applied");
							}
						}

						profile.AddTrackerOffset(data.trackerOffset, arcArray[data.arcPositionIndex].GetOffsetToTracker());

						break;
					case DataType.Length:

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
					case DataType.Distance:

						Vector3 point1 = Vector3.zero;
						Vector3 point2 = Vector3.zero;

						// TODO: make this a function
						switch (data.point1.pointType)
						{
							case PointType.ArcPoint:
								point1 = arcArray[data.point1.arcIndex].IntersectionPoint;
								break;
							case PointType.Tracker:
								point1 = Vector3.zero; // TODO: calculate local offset to tracker
								break;
						}
						switch (data.point2.pointType)
						{
							case PointType.ArcPoint:
								point2 = arcArray[data.point2.arcIndex].IntersectionPoint;
								break;
							case PointType.Tracker:
								point2 = Vector3.zero; // TODO: calculate local offset to tracker
								break;
						}

						float distance = Vector3.Distance(point1, point2);

						profile.AddBodyMeasurement(data.distanceMeasurement, distance);

						break;
					case DataType.Direction:

						Vector3 normal = arcArray[data.arcDirectionIndex].GetArcNormalFromTracker();
						VRTrackerType trackerDirectionType = settings[data.arcDirectionIndex].parentTracker;

						if (!trackers.GetTracker(trackerDirectionType, out TransformValues trackerDirectionTransform))
						{
							ErrorHandler.LogError(ErrorMessage.NoTrackerData);
							continue;
						}

						Matrix4x4 trackerDirectionMatrix = Matrix4x4.TRS(trackerDirectionTransform.position, trackerDirectionTransform.rotation, Vector3.one);
						Vector3 worldNormal = trackerDirectionMatrix.MultiplyVector(normal);

						switch (data.directionClosest.type)
						{
							case DirectionClosestType.SingleDirection:
								Vector3 dir = Vector3.zero;

								// TODO: make this a function
								switch (data.directionClosest.singleDirection.type)
								{
									case DirectionType.WorldDirection:
										dir = data.directionClosest.singleDirection.worldDirection;
										dir = dir.normalized;
										break;
									case DirectionType.TrackerDirection:

										if (!trackers.GetTracker(data.directionClosest.singleDirection.trackerFrom, out TransformValues trackerFrom) ||
										    !trackers.GetTracker(data.directionClosest.singleDirection.trackerTo, out TransformValues trackerTo))
										{
											ErrorHandler.LogError(ErrorMessage.NoTrackerData);
											continue;
										}

										dir = trackerTo.position - trackerFrom.position;
										dir = dir.normalized;

										break;
								}

								//Debug.DrawRay(trackers.GetTracker(VRTrackerType.LeftHand).position, worldNormal, Color.magenta, 10f);
								//Debug.DrawRay(trackers.GetTracker(VRTrackerType.LeftHand).position, dir, Color.cyan, 10f);

								if (Vector3.Distance(dir, worldNormal) > Vector3.Distance(dir, worldNormal * -1))
								{
									normal *= -1f;
								}

								break;
							case DirectionClosestType.Cross:

								Vector3 crossDir1 = Vector3.zero;
								Vector3 crossDir2 = Vector3.zero;

								// TODO: make this a function
								switch (data.directionClosest.crossDirection1.type)
								{
									case DirectionType.WorldDirection:
										crossDir1 = data.directionClosest.crossDirection1.worldDirection;
										break;
									case DirectionType.TrackerDirection:

										if (!trackers.GetTracker(data.directionClosest.crossDirection1.trackerFrom, out TransformValues trackerFrom) ||
											!trackers.GetTracker(data.directionClosest.crossDirection1.trackerTo, out TransformValues trackerTo))
										{
											ErrorHandler.LogError(ErrorMessage.NoTrackerData);
											return;
										}

										crossDir1 = trackerTo.position - trackerFrom.position;
										break;
								}

								switch (data.directionClosest.crossDirection2.type)
								{
									case DirectionType.WorldDirection:
										crossDir2 = data.directionClosest.crossDirection2.worldDirection;
										break;
									case DirectionType.TrackerDirection:
										if (!trackers.GetTracker(data.directionClosest.crossDirection2.trackerFrom, out TransformValues trackerFrom) ||
										    !trackers.GetTracker(data.directionClosest.crossDirection2.trackerTo, out TransformValues trackerTo))
										{
											ErrorHandler.LogError(ErrorMessage.NoTrackerData);
											return;
										}

										crossDir2 = trackerTo.position - trackerFrom.position;
										break;
								}

								Vector3 cross = Vector3.Cross(crossDir1, crossDir2);

								if (Vector3.Distance(cross, worldNormal) > Vector3.Distance(cross, worldNormal * -1))
								{
									normal *= -1f;
								}

								//Debug.DrawRay(trackers.GetTracker(VRTrackerType.LeftHand).Value.position, worldNormal, Color.magenta, 10f);
								//Debug.DrawRay(trackers.GetTracker(VRTrackerType.LeftHand).Value.position, cross, Color.cyan, 10f);

								break;
							default:
								ErrorHandler.LogError(ErrorMessage.NotImplemented);
								break;
						}

						profile.AddTrackerDirection(trackerDirectionType, data.directionAxis, normal);

						break;
				}
			}
		}

		// TODO: make all enums and classes local to the class, so you won't have to use Arc for every class name :/

		#region enums

		public enum DataType
		{
			OffsetToTracker = 1,
			Length = 2,
			Distance = 3,
			Direction = 0
		}

		public enum PointType
		{
			ArcPoint,
			Tracker
		}

		public enum DirectionClosestType
		{
			Tracker,
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
		public class Settings
		{
			public VRTrackerType tracker = VRTrackerType.Waist;
			public bool useTrackerLocal = false;
			public OffsetsToTrackers localOffset;

			public bool useParentTracker = false;
			public VRTrackerType parentTracker;
		}

		[System.Serializable]
		public class Data
		{
			public DataType dataType;

			// OffsetToTracker
			public OffsetsToTrackers trackerOffset;
			public int arcPositionIndex;
			public bool onLocalPlane;
			public Axis localPlane;

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
			public DirectionClosestTo directionClosest;
		}

		[System.Serializable]
		public class ArcPoint
		{
			public PointType pointType;

			// Arc point
			public int arcIndex;

			// Tracker
			public VRTrackerType tracker;
			public bool useLocalOffset;
			public OffsetsToTrackers offset;
		}

		[System.Serializable]
		public class DirectionClosestTo // TODO: make editor script
		{
			public DirectionClosestType type;

			[Header("Tracker")]
			public VRTrackerType trackerType;
			public bool inverseTracker;

			[Header("Tracker Direction")]
			public Direction singleDirection;

			[Header("Cross")]
			public Direction crossDirection1;
			public Direction crossDirection2;
		}

		[System.Serializable]
		public class Direction
		{
			public DirectionType type;

			[Header("WorldDirection")]
			public Vector3 worldDirection;

			[Header("TrackerDirection")]
			public VRTrackerType trackerFrom;
			public VRTrackerType trackerTo;
		}

		#endregion
	}
}