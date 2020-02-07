using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Manus.Core.VR;

namespace Manus.Polygon
{
	[CreateAssetMenu(fileName = "new Calibration Sequence", menuName = "ManusVR/Polygon/Calibration/Calibration Sequence", order = 0)]
	public class CalibrationSequence : ScriptableObject
	{
		public bool isFinished = false;
		public List<CalibrationStep> calibrationSteps;

		public Action calibrationFinished;

		private CalibrationProfile profile;
		private TrackerReference trackers;

		private MonoBehaviour mono;

		private Dictionary<CalibrationStep, ProfileData> profileHistory = new Dictionary<CalibrationStep, ProfileData>();

		public void SetupCalibrationSequence(CalibrationProfile profile, TrackerReference trackers, MonoBehaviour mono)
		{
			this.profile = profile;
			this.trackers = trackers;
			this.mono = mono;

			// TMP:
			isFinished = false;
			currentIndex = 0;
		}

		private int currentIndex;

		public void SetupNextCalibrationStep()
		{
			profileHistory[calibrationSteps[currentIndex]] = new ProfileData(profile);
			calibrationSteps[currentIndex].Setup(profile, trackers,
				() =>
					{
						Debug.Log(currentIndex);
						if (currentIndex >= calibrationSteps.Count)
						{
							isFinished = true;
							calibrationFinished?.Invoke();
						}
					});
		}

		public void StartCalibrationStep()
		{
			mono.StartCoroutine(calibrationSteps[currentIndex].Start());
			currentIndex++;
		}
	}
}
