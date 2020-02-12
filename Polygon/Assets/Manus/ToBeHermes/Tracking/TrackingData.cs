using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlmSharp;

namespace Manus.ToBeHermes.Tracking
{
	public class TrackingData : MonoBehaviour
	{
		private Dictionary<TrackerType, Tracker> trackers;

		public bool AddTracker(TrackerType trackerType)
		{
			if (trackers == null)
			{
				trackers = new Dictionary<TrackerType, Tracker>();
			}

			if (!trackers.ContainsKey(trackerType))
			{
				trackers.Add(trackerType, new Tracker());
				return true;
			}

			return false;
		}

		public bool RemoveTracker(TrackerType trackerType)
		{
			if (trackers != null && trackers.ContainsKey(trackerType))
			{
				if (trackers.Remove(trackerType))
				{
					if (trackers.Count == 0)
						trackers = null;

					return true;
				}
			}

			return false;
		}

		public bool ContainsTracker(TrackerType trackerType)
		{
			return trackers != null && trackers.ContainsKey(trackerType);
		}

		public void SetTrackerData(TrackerType trackerType, vec3 position, quat rotation)
		{
			if (ContainsTracker(trackerType))
			{
				trackers[trackerType].UpdateTracker(position, rotation);
			}
		}

		public void SetTrackerData(TrackerType trackerType, vec3 position)
		{
			if (ContainsTracker(trackerType))
			{
				trackers[trackerType].UpdateTracker(position);
			}
		}

		public void SetTrackerData(TrackerType trackerType, quat rotation)
		{
			if (ContainsTracker(trackerType))
			{
				trackers[trackerType].UpdateTracker(rotation);
			}
		}

		public Tracker GetTrackerData(TrackerType trackerType)
		{
			if (trackers != null && trackers.ContainsKey(trackerType))
			{
				return trackers[trackerType];
			}

			return null;
		}
	}
}