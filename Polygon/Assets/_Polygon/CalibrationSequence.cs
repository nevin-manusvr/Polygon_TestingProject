using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Polygon
{
	using Manus.Core.VR;

	[CreateAssetMenu(fileName = "new Calibration Sequence", menuName = "ManusVR/Polygon/Calibration/Calibration Sequence", order = 0)]
	public class CalibrationSequence : ScriptableObject
	{
		public List<CalibrationStep> calibrationSteps;

		private CalibrationProfile profile;
		private TrackerReference trackers;

		private MonoBehaviour mono;

		public void SetupCalibrationSequence(CalibrationProfile profile, TrackerReference trackers, MonoBehaviour mono)
		{
			this.profile = profile;
			this.trackers = trackers;
			this.mono = mono;

			// TMP:
			currentIndex = 0;
			Debug.Log("Setup Sequence");
		}

		private int currentIndex;

		public void NextCalibrationStep()
		{
			mono.StartCoroutine(CalibrateThings(currentIndex));

			currentIndex++;
		}

		private IEnumerator CalibrateThings(int index)
		{
			Debug.Log("Setup Step");
			calibrationSteps[index].Setup(profile, trackers);
			Debug.Log("Start Countdown");
			yield return new WaitForSeconds(1f);
			Debug.Log("Start Calibration");

			mono.StartCoroutine(calibrationSteps[index].Start());
		}
	}
}
