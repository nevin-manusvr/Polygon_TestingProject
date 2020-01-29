using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.Core.Utility;

namespace Manus.Polygon
{
	[CreateAssetMenu(fileName = "new Calibration Profile", menuName = "ManusVR/Polygon/Calibration/Profile", order = 1)]
	public class CalibrationProfile : ScriptableObject
	{
		public Dictionary<OffsetsToTrackers, TrackerOffset> trackerOffsets;
		public Dictionary<BodyMeasurements, float> bodyMeasurements;

		// Tracker offset
		public void AddTrackerOffset(OffsetsToTrackers type, Vector3 positionValue)
		{
			Debug.LogWarning($"Add tracker position offset: {type}");

			//if (!trackerOffsets.ContainsKey(type))
			//{
			//	trackerOffsets.Add(type, new TrackerOffset(positionValue));
			//}
			//else
			//{
			//	trackerOffsets[type].SetPositionOffset(positionValue);
			//}
		}

		public void AddTrackerOffset(OffsetsToTrackers type, Quaternion rotationValue)
		{
			Debug.LogWarning($"Add tracker rotation offset: {type}");


			//if (!trackerOffsets.ContainsKey(type))
			//{
			//	trackerOffsets.Add(type, new TrackerOffset(rotationValue));
			//}
			//else
			//{
			//	trackerOffsets[type].SetRotationOffset(rotationValue);
			//}
		}

		public void AddTrackerOffset(OffsetsToTrackers type, Vector3 positionValue, Quaternion rotationValue)
		{
			Debug.LogWarning($"Add tracker position and rotation offset: {type}");


			//if (!trackerOffsets.ContainsKey(type))
			//{
			//	trackerOffsets.Add(type, new TrackerOffset(positionValue, rotationValue));
			//}
			//else
			//{
			//	trackerOffsets[type].SetPositionOffset(positionValue);
			//	trackerOffsets[type].SetRotationOffset(rotationValue);
			//}
		}

		public void RemoveTrackerOffset(OffsetsToTrackers type, bool removePositionOffset, bool removeRotationOffset)
		{
			Debug.LogWarning($"Removed tracker position and rotation offset: {type}");

			//if (trackerOffsets[type].RemoveValue(removePositionOffset, removeRotationOffset))
			//{
			//	trackerOffsets.Remove(type);
			//}
		}

		// Body measurement
		public void AddBodyMeasurement(BodyMeasurements type, float value)
		{
			Debug.LogWarning($"Add body measurement: {type}");
		}

		public void RemoveBodyMeasurement(BodyMeasurements type)
		{
			Debug.LogWarning($"Remove body measurement: {type}");
		}

		#region Serialization



		#endregion
	}

	[System.Serializable]
	public struct TrackerOffset
	{
		public Vector3? positionOffset;
		public Quaternion? rotationOffset;

		#region Properties

		public bool IsEmpty
		{
			get { return positionOffset == null && rotationOffset == null; }
		}

		public Vector3 Position
		{
			get { return positionOffset ?? Vector3.zero; }
		}

		public Quaternion Rotation
		{
			get { return rotationOffset ?? Quaternion.identity; }
		}

		#endregion

		#region Constructor

		public TrackerOffset(Vector3 positionOffset, Quaternion rotationOffset)
		{
			this.positionOffset = null;
			this.rotationOffset = null;

			SetPositionOffset(positionOffset);
			SetRotationOffset(rotationOffset);
		}

		public TrackerOffset(Vector3 positionOffset)
		{
			this.positionOffset = null;
			this.rotationOffset = null;

			SetPositionOffset(positionOffset);
		}

		public TrackerOffset(Quaternion rotationOffset)
		{
			this.positionOffset = null;
			this.rotationOffset = null;

			SetRotationOffset(rotationOffset);
		}

		#endregion

		#region Public Methods

		public void SetPositionOffset(Vector3 positionOffset)
		{
			this.positionOffset = positionOffset;
		}

		public void SetRotationOffset(Quaternion rotatationOffset)
		{
			this.rotationOffset = rotatationOffset;
		}

		/// <summary>
		/// Function to remove the position or rotation offset. Returns true if the offset is completely empty and can be destroyed
		/// </summary>
		/// <param name="removePositionOffset">Should the position offset be removed</param>
		/// <param name="removeRotationOffset">Should the rotation offset be removed</param>
		/// <returns></returns>
		public bool RemoveValue(bool removePositionOffset, bool removeRotationOffset)
		{
			return true;
		}

		#endregion
	}
}
