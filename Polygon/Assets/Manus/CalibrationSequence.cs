using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Manus.Core.VR;
using Manus.Core.Utility;

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

		private Dictionary<int, ProfileData> profileHistory = new Dictionary<int, ProfileData>();

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
			profileHistory[currentIndex] = new ProfileData(profile);
			calibrationSteps[currentIndex].Setup(profile, trackers,
				() =>
					{
						if (currentIndex == calibrationSteps.Count)
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
			currentIndex = Mathf.Clamp(currentIndex, 0, calibrationSteps.Count);
		}

		public void PreviousStep()
		{
			currentIndex--;
			currentIndex = Mathf.Clamp(currentIndex, 0, calibrationSteps.Count - 1);

			if (profileHistory == null || !profileHistory.ContainsKey(currentIndex)) return;

			profile.Reset(profileHistory[currentIndex]);
		}
	}
}
