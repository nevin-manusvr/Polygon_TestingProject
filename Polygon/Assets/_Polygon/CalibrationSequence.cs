using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Polygon
{
	using Manus.Core.VR;

	[CreateAssetMenu(fileName = "new Calibration Sequence", menuName = "ManusVR/Polygon/Calibration/Calibration Sequence", order = 0)]
	public class CalibrationSequence : ScriptableObject
	{
		public ProfileRequirements profileRequirements;
		public List<CalibrationStep> calibrationSteps;

		private CalibrationProfile profile;
		private TrackerReference trackers;

		private MonoBehaviour mono;

		public void StartCalibrationSequence(CalibrationProfile profile, TrackerReference trackers, MonoBehaviour mono)
		{
			this.profile = profile;
			this.trackers = trackers;
			this.mono = mono;

			Debug.Log("Setup Sequence");

			StartCalibrationStep();
		}

		private int currentIndex = 0;

		private void StartCalibrationStep()
		{
			mono.StartCoroutine(CalibrateThings());
		}

		private IEnumerator CalibrateThings()
		{
			Debug.Log("Setup Step");
			calibrationSteps[currentIndex].Setup(profile, trackers);
			Debug.Log("Start Countdown");
			yield return new WaitForSeconds(1f);
			Debug.Log("Start Calibration");

			mono.StartCoroutine(calibrationSteps[currentIndex].Start());
		}
	}
}
