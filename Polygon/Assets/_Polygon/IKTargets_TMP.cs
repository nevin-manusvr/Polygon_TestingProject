using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.Core.Utility;
using Accord.Extensions.Statistics.Filters;
using Accord.Math;

namespace Manus.Polygon
{
	public class IKTargets_TMP : MonoBehaviour
	{
		private int lastFramerate;
		public int updateFramerate = 90;

		[Space]
		public CalibrationProfile profile;
		private TrackerReference trackers;

		public bool updateTrackerPositions;
		public bool calculateAimPositions;

		private Coroutine update;

		[Header("Kalman")]
		public bool useKalmanOnTrackerData;
		public float measurementNoise = 10f;
		public float measurementReliability = 1.3f;
		private Dictionary<VRTrackerType, TransformFilter> filters;

		[Space]
		[Header("Trackers")]
		public Transform head;
		public Transform hip;

		[Space]
		public Transform leftHand;
		public Transform rightHand;

		[Space]
		public Transform leftFoot;
		public Transform rightFoot;

		[Header("IK Aims")]
		public Transform spine;

		[Space]
		public Transform leftKnee;
		public Transform rightKnee;

		[Space]
		public Transform leftElbow;
		public Transform rightElbow;

		// framerate
		private int m_frameCounter = 0;
		private float m_timeCounter = 0.0f;
		private float m_lastFramerate = 0.0f;
		private float m_refreshTime = 1f;

		#region Monobehaviour Callbacks

		private void Start()
		{
			trackers = FindObjectOfType<TrackerReference>();

			filters = new Dictionary<VRTrackerType, TransformFilter>();
			foreach (VRTrackerType type in trackers.RequiredTrackers)
			{
				filters.Add(type, new TransformFilter(Vector3.zero, Quaternion.identity, updateFramerate, measurementNoise, measurementReliability));
			}

			update = StartCoroutine(UpdateCoroutine());
		}

		private void Update()
		{
			if (m_timeCounter < m_refreshTime)
			{
				m_timeCounter += Time.deltaTime;
				m_frameCounter++;
			}
			else
			{
				m_lastFramerate = (float)m_frameCounter / m_timeCounter;
				m_frameCounter = 0;
				m_timeCounter = 0.0f;
			}
		}

		private IEnumerator UpdateCoroutine()
		{
			while (true)
			{
				int framerate = updateFramerate < m_lastFramerate ? updateFramerate : (int)m_lastFramerate;
				if (framerate == 0 || framerate < 1) 
					framerate = updateFramerate;

				if (lastFramerate != framerate)
				{
					foreach (VRTrackerType type in filters.Keys)
					{
						filters[type].UpdateNoiseSettings(measurementNoise, measurementReliability, framerate);
					}
					lastFramerate = framerate;
				}


				yield return new WaitForSeconds(1f / framerate);

				if (updateTrackerPositions)
					UpdateTrackerTargets();
			}
		}

		private void UpdateTrackerTargets()
		{
			foreach (VRTrackerType tracker in trackers.RequiredTrackers)
			{
				TransformValues? trackerTransform = trackers.GetTracker(tracker);

				Transform obj = null;

				switch (tracker)
				{
					case VRTrackerType.Head:
						obj = head;

						if (profile.trackerOffsets.ContainsKey(OffsetsToTrackers.HeadTrackerToHead))
						{
							TrackerOffset offset = profile.trackerOffsets[OffsetsToTrackers.HeadTrackerToHead];
							trackerTransform = trackers.GetTrackerWithOffset(VRTrackerType.Head, offset.Position, offset.Rotation);
						}

						break;
					case VRTrackerType.LeftHand:
						obj = leftHand;

						if (profile.trackerOffsets.ContainsKey(OffsetsToTrackers.LeftHandTrackerToWrist))
						{
							TrackerOffset offset = profile.trackerOffsets[OffsetsToTrackers.LeftHandTrackerToWrist];
							trackerTransform = trackers.GetTrackerWithOffset(VRTrackerType.LeftHand, offset.Position, offset.Rotation);
						}

						break;
					case VRTrackerType.RightHand:
						obj = rightHand;

						if (profile.trackerOffsets.ContainsKey(OffsetsToTrackers.RightHandTrackerToWrist))
						{
							TrackerOffset offset = profile.trackerOffsets[OffsetsToTrackers.RightHandTrackerToWrist];
							trackerTransform = trackers.GetTrackerWithOffset(VRTrackerType.RightHand, offset.Position, offset.Rotation);
						}

						break;
					case VRTrackerType.LeftFoot:
						obj = leftFoot;

						if (profile.trackerOffsets.ContainsKey(OffsetsToTrackers.LeftFootTrackerToAnkle))
						{
							TrackerOffset offset = profile.trackerOffsets[OffsetsToTrackers.LeftFootTrackerToAnkle];
							trackerTransform = trackers.GetTrackerWithOffset(VRTrackerType.LeftFoot, offset.Position, offset.Rotation);
						}

						break;
					case VRTrackerType.RightFoot:
						obj = rightFoot;

						if (profile.trackerOffsets.ContainsKey(OffsetsToTrackers.RightFootTrackerToAnkle))
						{
							TrackerOffset offset = profile.trackerOffsets[OffsetsToTrackers.RightFootTrackerToAnkle];
							trackerTransform = trackers.GetTrackerWithOffset(VRTrackerType.RightFoot, offset.Position, offset.Rotation);
						}

						break;
				}

				if (trackerTransform == null || obj == null) continue;

				filters[tracker].UpdateFilter(trackerTransform.Value.position, trackerTransform.Value.rotation);
				
				obj.position = useKalmanOnTrackerData ? filters[tracker].Position : trackerTransform.Value.position;
				obj.rotation = useKalmanOnTrackerData ? filters[tracker].Rotation : trackerTransform.Value.rotation;
			}
		}

		#endregion


		[ContextMenu("Initialize")]
		private void Initialize()
		{
			head = head != null ? head : CreateTarget("head");
			hip = hip != null ? hip : CreateTarget("hip");
			leftHand = leftHand != null ? leftHand : CreateTarget("leftHand");
			rightHand = rightHand != null ? rightHand : CreateTarget("rightHand");
			leftFoot = leftFoot != null ? leftFoot : CreateTarget("leftFoot");
			rightFoot = rightFoot != null ? rightFoot : CreateTarget("rightFoot");

			spine = spine != null ? spine : CreateTarget("spine");
			leftKnee = leftKnee != null ? leftKnee : CreateTarget("leftKnee");
			rightKnee = rightKnee != null ? rightKnee : CreateTarget("rightKnee");
			leftElbow = leftElbow != null ? leftElbow : CreateTarget("leftElbow");
			rightElbow = rightElbow != null ? rightElbow : CreateTarget("rightElbow");
		}

		[ContextMenu("Reset")]
		private void Reset()
		{
			if (head != null) DestroyImmediate(head.gameObject);
			if (hip != null) DestroyImmediate(hip.gameObject);
			if (leftHand != null) DestroyImmediate(leftHand.gameObject);
			if (rightHand != null) DestroyImmediate(rightHand.gameObject);
			if (leftFoot != null) DestroyImmediate(leftFoot.gameObject);
			if (rightFoot != null) DestroyImmediate(rightFoot.gameObject);

			if (spine != null) DestroyImmediate(spine.gameObject);
			if (leftKnee != null) DestroyImmediate(leftKnee.gameObject);
			if (rightKnee != null) DestroyImmediate(rightKnee.gameObject);
			if (leftElbow != null) DestroyImmediate(leftElbow.gameObject);
			if (rightElbow != null) DestroyImmediate(rightElbow.gameObject);

			head = null;
			hip = null;
			leftHand = null;
			rightHand = null;
			leftFoot = null;
			rightFoot = null;

			spine = null;
			leftKnee = null;
			rightKnee = null;
			leftElbow = null;
			rightElbow = null;
		}

		private Transform CreateTarget(string name)
		{
			GameObject obj = new GameObject(name);
			obj.transform.SetParent(transform);

			return obj.transform;
		}
	}

	public class TransformFilter
	{
		private int framerate;

		public DiscreteKalmanFilter<ConstantVelocity3DModel, Vector3> positionFilter;
		public DiscreteKalmanFilter<ConstantVelocity3DModel, Vector3> forwardRotationFilter;
		public DiscreteKalmanFilter<ConstantVelocity3DModel, Vector3> upRotationFilter;

		public Vector3 Position
		{
			get { return positionFilter.State.Position; }
		}

		public Quaternion Rotation
		{
			get { return Quaternion.LookRotation(forwardRotationFilter.State.Position, upRotationFilter.State.Position); }
		}

		public TransformFilter(Vector3 initialPosition, Quaternion initialRotation, int framerate, float noise = 10f, float reliability = 1.3f)
		{
			this.framerate = framerate;

			positionFilter = KalmanGenerator.Generate3DimensionalFilter(initialPosition, framerate, noise, reliability);
			forwardRotationFilter = KalmanGenerator.Generate3DimensionalFilter(initialRotation * Vector3.forward, framerate, noise, reliability);
			upRotationFilter = KalmanGenerator.Generate3DimensionalFilter(initialRotation * Vector3.up, framerate, noise, reliability);
		}

		public void UpdateFilter(Vector3 position, Quaternion rotation)
		{
			UpdateFilter();

			positionFilter.Correct(position);
			forwardRotationFilter.Correct(rotation * Vector3.forward);
			upRotationFilter.Correct(rotation * Vector3.up);
		}

		public void UpdateFilter()
		{
			positionFilter.Predict();
			forwardRotationFilter.Predict();
			upRotationFilter.Predict();
		}

		public void UpdateNoiseSettings(float noise = 10f, float reliability = 1.3f, float framerate = 90)
		{
			positionFilter.ProcessNoise = ConstantVelocity3DModel.GetProcessNoise(noise, 1f / framerate);
			positionFilter.MeasurementNoise = Matrix.Diagonal<double>(positionFilter.MeasurementVectorDimension, reliability);
			forwardRotationFilter.ProcessNoise = ConstantVelocity3DModel.GetProcessNoise(noise, 1f / framerate);
			forwardRotationFilter.MeasurementNoise = Matrix.Diagonal<double>(forwardRotationFilter.MeasurementVectorDimension, reliability);
			upRotationFilter.ProcessNoise = ConstantVelocity3DModel.GetProcessNoise(noise, 1f / framerate);
			upRotationFilter.MeasurementNoise = Matrix.Diagonal<double>(upRotationFilter.MeasurementVectorDimension, reliability);
		}
	}
}