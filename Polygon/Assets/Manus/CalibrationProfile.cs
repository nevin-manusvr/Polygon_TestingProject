using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.Core.Utility;

namespace Manus.Polygon
{
	[CreateAssetMenu(fileName = "new Calibration Profile", menuName = "ManusVR/Polygon/Calibration/Profile", order = 1)]
	public class CalibrationProfile : ScriptableObject
	{
		public ProfileRequirements profileRequirements;

		public Dictionary<OffsetsToTrackers, TrackerOffset> trackerOffsets = new Dictionary<OffsetsToTrackers, TrackerOffset>();
		public Dictionary<VRTrackerType, TrackerDirection> trackerDirections = new Dictionary<VRTrackerType, TrackerDirection>();
		public Dictionary<BodyMeasurements, float> bodyMeasurements = new Dictionary<BodyMeasurements, float>();

		#region Public Methods

		// Main
		public void Reset()
		{
			trackerOffsets = new Dictionary<OffsetsToTrackers, TrackerOffset>();
			trackerDirections = new Dictionary<VRTrackerType, TrackerDirection>();
			bodyMeasurements = new Dictionary<BodyMeasurements, float>();
		}

		public void Reset(ProfileData newProfile)
		{
			profileRequirements = newProfile.profileRequirements;
			trackerOffsets = newProfile.trackerOffsets;
			trackerDirections = newProfile.trackerDirections;
			bodyMeasurements = newProfile.bodyMeasurements;
		}

		// Tracker offset
		public void AddTrackerOffset(OffsetsToTrackers type, Vector3 positionValue)
		{
			Debug.LogWarning($"Add tracker position offset: {type}");

			if (!trackerOffsets.ContainsKey(type))
			{
				trackerOffsets.Add(type, new TrackerOffset(positionValue));
			}
			else
			{
				trackerOffsets[type].SetPositionOffset(positionValue);
			}
		}

		public void AddTrackerOffset(OffsetsToTrackers type, Quaternion rotationValue)
		{
			Debug.LogWarning($"Add tracker rotation offset: {type}");


			if (!trackerOffsets.ContainsKey(type))
			{
				trackerOffsets.Add(type, new TrackerOffset(rotationValue));
			}
			else
			{
				trackerOffsets[type].SetRotationOffset(rotationValue);
			}
		}

		public void AddTrackerOffset(OffsetsToTrackers type, Vector3 positionValue, Quaternion rotationValue)
		{
			Debug.LogWarning($"Add tracker position and rotation offset: {type}");


			if (!trackerOffsets.ContainsKey(type))
			{
				trackerOffsets.Add(type, new TrackerOffset(positionValue, rotationValue));
			}
			else
			{
				trackerOffsets[type].SetPositionOffset(positionValue);
				trackerOffsets[type].SetRotationOffset(rotationValue);
			}
		}

		public void RemoveTrackerOffset(OffsetsToTrackers type, bool removePositionOffset, bool removeRotationOffset)
		{
			Debug.LogWarning($"Removed tracker position and rotation offset: {type}");

			if (trackerOffsets[type].RemoveValue(removePositionOffset, removeRotationOffset))
			{
				trackerOffsets.Remove(type);
			}
		}

		// Tracker Direction
		public void AddTrackerDirection(VRTrackerType type, Axis axis, Vector3 direction)
		{
			Debug.LogWarning($"Add tracker direction: {type} - {axis}");

			if (!trackerDirections.ContainsKey(type))
			{
				trackerDirections.Add(type, new TrackerDirection(axis, direction.normalized));
			}
			else
			{
				trackerDirections[type].SetAxis(axis, direction.normalized);
			}

			if (trackerDirections?[type].GetAxis(Axis.Z) != null && trackerDirections?[type].GetAxis(Axis.Y) != null)
			{
				AddTrackerOffset((OffsetsToTrackers)GetMatchingTrackerOffsetForTracker(type), Quaternion.LookRotation(trackerDirections[type].Z, trackerDirections[type].Y));
			}
		}

		public void RemoveTrackerDirection(VRTrackerType trackerType, Axis type)
		{
			Debug.LogWarning($"Remove tracker direction: {trackerType} - {type}");

		}

		// Body measurement
		public void AddBodyMeasurement(BodyMeasurements type, float value)
		{
			Debug.LogWarning($"Add body measurement: {type}");

			if (!bodyMeasurements.ContainsKey(type))
			{
				bodyMeasurements.Add(type, value);
			}
			else
			{
				bodyMeasurements[type] = value;
			}
		}

		public void RemoveBodyMeasurement(BodyMeasurements type)
		{
			Debug.LogWarning($"Remove body measurement: {type}");
		}

		#endregion

		#region Serialization



		#endregion

		public static VRTrackerType? GetMatchingTrackerFromOffset(OffsetsToTrackers offsetType)
		{
			switch (offsetType)
			{
				case OffsetsToTrackers.HeadTrackerToHead:
					return VRTrackerType.Head;
				case OffsetsToTrackers.LeftHandTrackerToWrist:
					return VRTrackerType.LeftHand;
				case OffsetsToTrackers.RightHandTrackerToWrist:
					return VRTrackerType.RightHand;
				case OffsetsToTrackers.LeftElbowTrackerToElbow:
					return null;
				case OffsetsToTrackers.RightElbowTrackerToElbow:
					return null;
				case OffsetsToTrackers.LeftElbowTrackerToShoulder:
					return null;
				case OffsetsToTrackers.RightElbowTrackerToShoulder:
					return null;
				case OffsetsToTrackers.HipTrackerToHip:
					return VRTrackerType.Waist;
				case OffsetsToTrackers.HipTrackerToLeftLeg:
					return VRTrackerType.Waist;
				case OffsetsToTrackers.HipTrackerToRightLeg:
					return VRTrackerType.Waist;
				case OffsetsToTrackers.LeftFootTrackerToAnkle:
					return VRTrackerType.LeftFoot;
				case OffsetsToTrackers.RightFootTrackerToAnkle:
					return VRTrackerType.RightFoot;
				default:
					return null;
			}
		}

		public static OffsetsToTrackers? GetMatchingTrackerOffsetForTracker(VRTrackerType trackerType)
		{
			switch (trackerType)
			{
				case VRTrackerType.Head:
					return OffsetsToTrackers.HeadTrackerToHead;
				case VRTrackerType.LeftHand:
					return OffsetsToTrackers.LeftHandTrackerToWrist;
				case VRTrackerType.RightHand:
					return OffsetsToTrackers.RightHandTrackerToWrist;
				case VRTrackerType.Waist:
					return OffsetsToTrackers.HipTrackerToHip;
				case VRTrackerType.LeftFoot:
					return OffsetsToTrackers.LeftFootTrackerToAnkle;
				case VRTrackerType.RightFoot:
					return OffsetsToTrackers.RightFootTrackerToAnkle;
				default:
					Debug.LogError($"Tracking type: {trackerType} is currently not supported, please add this");
					return null;
			}
		}
	}

	[System.Serializable]
	public class TrackerOffset
	{
		public Vector3? position;
		public Quaternion? rotation;

		#region Properties

		public bool IsEmpty
		{
			get { return position == null && rotation == null; }
		}

		public Vector3 Position
		{
			get { return position ?? Vector3.zero; }
		}

		public Quaternion Rotation
		{
			get { return rotation ?? Quaternion.identity; }
		}

		#endregion

		#region Constructor

		public TrackerOffset(TrackerOffset offset)
		{
			this.position = offset.position;
			this.rotation = offset.rotation;
		}

		public TrackerOffset(Vector3 positionOffset, Quaternion rotationOffset)
		{
			this.position = null;
			this.rotation = null;

			SetPositionOffset(positionOffset);
			SetRotationOffset(rotationOffset);
		}

		public TrackerOffset(Vector3 positionOffset)
		{
			this.position = null;
			this.rotation = null;

			SetPositionOffset(positionOffset);
		}

		public TrackerOffset(Quaternion rotationOffset)
		{
			this.position = null;
			this.rotation = null;

			SetRotationOffset(rotationOffset);
		}

		#endregion

		#region Public Methods

		public void SetPositionOffset(Vector3 positionOffset)
		{
			this.position = positionOffset;
		}

		public void SetRotationOffset(Quaternion rotatationOffset)
		{
			this.rotation = rotatationOffset;
		}

		/// <summary>
		/// Function to remove the position or rotation offset.
		/// </summary>
		/// <param name="removePositionOffset">Should the position offset be removed</param>
		/// <param name="removeRotationOffset">Should the rotation offset be removed</param>
		/// <returns>Returns true if the offset is completely empty and can be destroyed</returns>
		public bool RemoveValue(bool removePositionOffset, bool removeRotationOffset)
		{
			if (removePositionOffset)
				position = null;

			if (removeRotationOffset)
				rotation = null;

			return IsEmpty;
		}

		#endregion
	}

	[System.Serializable]
	public class TrackerDirection
	{
		private Vector3? x;
		private Vector3? y;
		private Vector3? z;

		#region Properties

		public bool IsEmpty
		{
			get { return x == null && y == null && z == null; }
		}

		public Vector3 X
		{
			get { return x ?? Vector3.zero; }
		}

		public Vector3 Y
		{
			get { return y ?? Vector3.zero; }
		}

		public Vector3 Z
		{
			get { return z ?? Vector3.zero; }
		}

		#endregion

		#region Constructor

		public TrackerDirection(TrackerDirection direction)
		{
			this.x = direction.x;
			this.y = direction.y;
			this.z = direction.z;
		}

		public TrackerDirection(Vector3 x, Vector3 y, Vector3 z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public TrackerDirection(Axis axis, Vector3 direction)
		{
			this.x = null;
			this.y = null;
			this.z = null;

			switch (axis)
			{
				case Axis.X:
					x = direction;
					break;
				case Axis.Y:
					y = direction;
					break;
				case Axis.Z:
					z = direction;
					break;
			}
		}

		#endregion

		#region Public Methods

		public Vector3? GetAxis(Axis axis)
		{
			switch (axis)
			{
				case Axis.X:
					return x;
				case Axis.Y:
					return y;
				case Axis.Z:
					return z;
			}

			return null;
		}

		public void SetAxis(Axis axis, Vector3 direction)
		{
			switch (axis)
			{
				case Axis.X:
					this.x = direction;
					break;
				case Axis.Y:
					this.y = direction;
					break;
				case Axis.Z:
					this.z = direction;
					break;
			}
		}

		public bool RemoveAxis(Axis axis)
		{
			switch (axis)
			{
				case Axis.X:
					x = null;
					break;
				case Axis.Y:
					y = null;
					break;
				case Axis.Z:
					z = null;
					break;
			}

			return IsEmpty;
		}

		#endregion
	}

	[System.Serializable]
	public class ProfileData
	{
		public ProfileRequirements profileRequirements;

		public Dictionary<OffsetsToTrackers, TrackerOffset> trackerOffsets = new Dictionary<OffsetsToTrackers, TrackerOffset>();
		public Dictionary<VRTrackerType, TrackerDirection> trackerDirections = new Dictionary<VRTrackerType, TrackerDirection>();
		public Dictionary<BodyMeasurements, float> bodyMeasurements = new Dictionary<BodyMeasurements, float>();

		public ProfileData(CalibrationProfile newProfile)
		{
			profileRequirements = new ProfileRequirements(
				newProfile.profileRequirements.requiredTrackerOffsets,
				newProfile.profileRequirements.requiredBoneMeasurements);

			foreach (var offset in newProfile.trackerOffsets)
			{
				trackerOffsets.Add(offset.Key, new TrackerOffset(offset.Value));
			}

			foreach (var dir in newProfile.trackerDirections)
			{
				trackerDirections.Add(dir.Key, new TrackerDirection(dir.Value));
			}

			foreach (var measurement in newProfile.bodyMeasurements)
			{
				bodyMeasurements.Add(measurement.Key, measurement.Value);
			}
		}
	}
}
