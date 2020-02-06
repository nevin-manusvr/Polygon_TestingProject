using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Polygon
{
	public abstract class CalibrationStep : ScriptableObject
	{
		public CalibrationStepEvent calibrationEvents;

		[Space]
		[TextArea] public string description;
		[Range(1, 10)] public float time = 1f;


		protected CalibrationProfile profile;
		protected TrackerReference trackers;

		public virtual void Setup(CalibrationProfile profile, TrackerReference trackers)
		{
			if (calibrationEvents != null)
				calibrationEvents.RaisePrepareCalibration();

			this.profile = profile;
			this.trackers = trackers;

			// Create all data needed for the calibration step
		}

		public virtual IEnumerator Start()
		{
			if (calibrationEvents != null)
				calibrationEvents.RaiseStartCalibration();

			float timer = 0;

			while (timer < time)
			{
				timer += Time.deltaTime;

				if (calibrationEvents != null)
					calibrationEvents.RaiseUpdateCalibration(timer / time);

				Update();
				yield return new WaitForEndOfFrame();
			}

			if (calibrationEvents != null)
			{
				calibrationEvents.RaiseUpdateCalibration(1f);
				calibrationEvents.RaiseFinishedCalibration();
			}

			End();
		}

		protected virtual void Update() {} // Accumulate data for the calibration step

		protected abstract void End(); // Apply calibration step data to the profile
	}
}

