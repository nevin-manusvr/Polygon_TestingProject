﻿using System.Collections;
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

		public virtual IEnumerator Start()
		{
			float timer = 0;

			while (timer < time)
			{
				timer += Time.deltaTime;
				Update();
				yield return new WaitForEndOfFrame();
			}

			End();
		}

		protected virtual void Update() {} // Accumulate data for the calibration step

		protected abstract void End(); // Apply calibration step data to the profile
	}
}

