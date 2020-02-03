using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.Core.Utility;

namespace Manus.Polygon
{
	[CreateAssetMenu(fileName = "new Align Calibration Step", menuName = "ManusVR/Polygon/Calibration/Align Calibration Step", order = 10)]
	public class AlignCalibrationStep : CalibrationStep
	{
		public Settings[] settings;

		//public override void Setup(CalibrationProfile profile, TrackerReference trackers)
		//{
		//	base.Setup(profile, trackers);
		//}

		protected override void Update()
		{
		}

		protected override void End()
		{
			foreach (Settings setting in settings)
			{
				switch (setting.dataType)
				{
					case AlignType.GetAxis:

						foreach (Data axisData in setting.getAxisData)
						{
							TransformValues fromTrackerTransform = trackers.GetTracker(axisData.fromTracker)
							                                       ?? new TransformValues(Vector3.zero, Quaternion.identity);
							TransformValues toTrackerTransform = trackers.GetTracker(axisData.ToTracker)
							                            ?? new TransformValues(Vector3.zero, Quaternion.identity);

							if (axisData.useFromLocal)
							{
								if (profile.trackerOffsets.ContainsKey(axisData.fromLocalOffset))
								{
									TrackerOffset trackerOffset = profile.trackerOffsets[axisData.fromLocalOffset];
									fromTrackerTransform = trackers.GetTrackerWithOffset(axisData.fromTracker, trackerOffset.Position, Quaternion.identity) 
									                       ?? new TransformValues(Vector3.zero, Quaternion.identity);
								}
								else
								{
									Debug.LogWarning($"{axisData.fromLocalOffset} not found in profile, use without local offset");
								}
							}

							if (axisData.useToLocal)
							{
								if (profile.trackerOffsets.ContainsKey(axisData.toLocalOffset))
								{
									TrackerOffset trackerOffset = profile.trackerOffsets[axisData.toLocalOffset];
									toTrackerTransform = trackers.GetTrackerWithOffset(axisData.ToTracker, trackerOffset.Position, Quaternion.identity)
												?? new TransformValues(Vector3.zero, Quaternion.identity);
								}
								else
								{
									Debug.LogWarning($"{axisData.toLocalOffset} not found in profile, use without local offset");
								}
							}

							TransformValues trackerTransform = trackers.GetTracker(axisData.tracker) 
							                                   ?? new TransformValues(Vector3.zero, Quaternion.identity);

							Vector3 direction = toTrackerTransform.position - fromTrackerTransform.position;
							Matrix4x4 trackerMatrix = Matrix4x4.TRS(trackerTransform.position, trackerTransform.rotation, Vector3.one);
							trackerMatrix = trackerMatrix.inverse;

							profile.AddTrackerDirection(axisData.tracker, axisData.axis, trackerMatrix.MultiplyVector(direction));
						}

						break;
					case AlignType.AlignAxis:
						break;
					case AlignType.CalculateAxis:
						break;
					default:
						Debug.LogError("Selected align setting type is not implemented");
						break;
				}
			}
		}

		#region enums

		public enum AlignType
		{
			GetAxis,
			AlignAxis,
			CalculateAxis
		}

		#endregion

		#region classes

		[System.Serializable]
		public class Settings
		{
			public AlignType dataType;

			[Header("Get Axis")]
			public Data[] getAxisData;

			[Header("Align Axis")]
			public Axis axisToAlign;

			public VRTrackerType alignTracker1;
			public bool invertTracker1Axis;
			public VRTrackerType alignTracker2;

			[Header("Calculate axis")]
			public Axis axisToCalculate;

		}
		
		[System.Serializable]
		public class Data
		{
			public VRTrackerType tracker;
			public Axis axis;
			
			[Space]

			public VRTrackerType fromTracker;
			public bool useFromLocal;
			public OffsetsToTrackers fromLocalOffset;

			[Space]

			public VRTrackerType ToTracker;
			public bool useToLocal;
			public OffsetsToTrackers toLocalOffset;
		}

		#endregion
	}
}