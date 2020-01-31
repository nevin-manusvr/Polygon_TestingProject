using System.Collections;
using System.Collections.Generic;
using Manus.Polygon;
using UnityEngine;
using Manus.Core.Utility;

namespace Manus.Polygon
{
	using System.Linq;

	public class ProfileDebugger : MonoBehaviour
	{
		public CalibrationProfile profile;
		private TrackerReference trackers;

		private List<GameObject> trackerVisuals;
		private List<GameObject> trackerOffsetVisuals;
		private List<GameObject> trackerDirectionVisuals;

		[Header("Models")]
		public GameObject trackerModel;
		public GameObject leftHandModel, rightHandModel;
		public GameObject leftFootModel, rightFootModel;

		private void Start()
		{
			// Find references
			trackers = FindObjectOfType<TrackerReference>();

			// Initializing
			trackerVisuals = new List<GameObject>();
			trackerOffsetVisuals = new List<GameObject>();
			trackerDirectionVisuals = new List<GameObject>();
		}

		private void Update()
		{
			VisualizeTrackers();
			VisualizeTrackerOffsets();
		}

		private void VisualizeTrackers()
		{
			for (int i = 0; i < trackers.RequiredTrackers.Length; i++)
			{
				// Spawn new visual if there are not enough
				if (trackerVisuals.Count < i + 1)
				{
					trackerVisuals.Add(Instantiate(trackerModel, transform));
				}

				VRTrackerType trackerType = trackers.RequiredTrackers[i];

				// Hide tracker when there is no tracker data
				TransformValues? trackerTransform = trackers.GetTracker(trackerType);
				if (trackerTransform == null)
				{
					if (trackerVisuals[i].activeSelf) trackerVisuals[i].SetActive(false);
					continue;
				}

				// Position tracker model to the right position
				if (!trackerVisuals[i].activeSelf) trackerVisuals[i].SetActive(true);

				trackerVisuals[i]?.transform.SetPositionAndRotation(
					trackerTransform.Value.position,
					trackerTransform.Value.rotation);
			}
		}

		private void VisualizeTrackerOffsets()
		{
			for (int i = 0; i < profile.trackerOffsets.Keys.Count; i++)
			{
				if (trackerOffsetVisuals.Count < i + 1)
				{
					trackerOffsetVisuals.Add(GameObject.CreatePrimitive(PrimitiveType.Sphere));
					trackerOffsetVisuals[i].transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
					trackerOffsetVisuals[i].transform.SetParent(transform);
				}



				OffsetsToTrackers offsetType = profile.trackerOffsets.Keys.ToArray()[i];
				
				VRTrackerType type = VRTrackerType.Other;
				switch (offsetType)
				{
					case OffsetsToTrackers.LeftHandTrackerToWrist:
						type = VRTrackerType.LeftHand;
						break;
					case OffsetsToTrackers.RightHandTrackerToWrist:
						type = VRTrackerType.RightHand;
						break;
					default:
						Debug.LogError($"OffsetsToTracker type : {offsetType} not implemented");
						break;
				}

				TrackerOffset offset = profile.trackerOffsets[offsetType];

				TransformValues? trackerWithOffset = trackers.GetTrackerWithOffset(type, offset.Position, offset.Rotation);

				if (trackerWithOffset == null)
				{
					if (trackerOffsetVisuals[i].activeSelf) trackerOffsetVisuals[i].SetActive(false);
					continue;
				}

				if (!trackerOffsetVisuals[i].activeSelf) trackerOffsetVisuals[i].SetActive(true);

				trackerOffsetVisuals[i]?.transform.SetPositionAndRotation(
					trackerWithOffset.Value.position,
					trackerWithOffset.Value.rotation);
			}
		}
	}
}

