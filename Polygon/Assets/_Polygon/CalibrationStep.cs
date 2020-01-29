using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Polygon
{
	public abstract class CalibrationStep : ScriptableObject
	{
		[TextArea] public string description;
		[Range(1, 10)] public float time = 1f;

		protected CalibrationProfile profile;
		protected TrackerReference trackers;

		public virtual void Setup(CalibrationProfile profile, TrackerReference trackers)
		{
			this.profile = profile;
			this.trackers = trackers;

			// Create all data needed for the calibration step
		}

		public abstract IEnumerator Start();

		protected abstract void Update(); // Accumulate data for the calibration step

		protected abstract void End(); // Apply calibration step data to the profile

		public abstract void Revert(); // Remove added profile date
	}
}

