using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Manus.Core.VR;
using Manus.Core.Utility;

namespace Manus.Polygon
{

	public class TrackerReference : MonoBehaviour
	{
		#region Fields

		[SerializeField] private VRTrackerType[] requiredTrackers =
			{
				VRTrackerType.Head,
				VRTrackerType.LeftHand,
				VRTrackerType.RightHand
			};

		[ReadOnly, SerializeField] private bool areAllRequiredTrackersConnected = false;
		
		private TrackerManager trackerManager;
		private Dictionary<VRTrackerType, Tracker> trackers;

		#endregion

		#region Properties

		public bool AreAllRequiredTrackersConnected
		{
			get { return areAllRequiredTrackersConnected; }
		}

		#endregion

		#region MonoBehaviour Callbacks

		private void OnEnable()
		{
			trackerManager = TrackerManager.instance;
			trackerManager.TrackersChanged += OnTrackersChanged;

			UpdateTrackerReferences();
		}

		private void OnDisable()
		{
			trackerManager.TrackersChanged -= OnTrackersChanged;
		}

		private void OnDrawGizmos()
		{
			if (trackers == null) return;
			
			Vector3 size = new Vector3(1f, 1f, 1f);

			Matrix4x4 parentMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
			
			foreach (Tracker tracker in trackers.Values)
			{
				// Draw tracker
				Matrix4x4 trackerMatrix = Matrix4x4.TRS(parentMatrix.MultiplyPoint3x4(tracker.position), parentMatrix.rotation * tracker.rotation, Vector3.one);
				Gizmos.matrix = trackerMatrix;

				Gizmos.color = Color.cyan;
				Gizmos.DrawWireCube(Vector3.zero, size * 0.03f);

				// Draw directions
				Gizmos.color = Color.blue;
				Gizmos.DrawRay(Vector3.zero, Quaternion.identity * Vector3.forward * size.z * 0.1f);
				Gizmos.color = Color.green;
				Gizmos.DrawRay(Vector3.zero, Quaternion.identity * Vector3.up * size.y * 0.1f);
				Gizmos.color = Color.red;
				Gizmos.DrawRay(Vector3.zero, Quaternion.identity * Vector3.left * size.x * 0.1f);
			}

			Matrix4x4 gizmoMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
			Gizmos.matrix = gizmoMatrix;
		}

		#endregion

		#region Public Methods

		public void SetRequiredTrackers(VRTrackerType[] requiredTrackers)
		{
			this.requiredTrackers = requiredTrackers;
			UpdateTrackerReferences();
		}

		public Tracker GetTracker(VRTrackerType type)
		{
			return trackers[type];
		}

		#endregion

		#region Private Methods

		private void OnTrackersChanged()
		{
			UpdateTrackerReferences();
		}

		private void UpdateTrackerReferences()
		{
			areAllRequiredTrackersConnected = true;
			trackers = new Dictionary<VRTrackerType, Tracker>();

			foreach (VRTrackerType type in requiredTrackers)
			{
				if (trackerManager.m_TrackersType[(int)type]?.Count > 0 && trackerManager.m_TrackersType[(int)type][0] != null)
				{
					trackers.Add(type, trackerManager.m_TrackersType[(int)type][0]);
				}
				else
				{
					areAllRequiredTrackersConnected = false;
				}
			}
		}

		#endregion
	}
}

