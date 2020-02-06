using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.Core.VR;

namespace Manus.Polygon
{
	public class Calibrator : MonoBehaviour
	{
		public CalibrationProfile profile;
		public CalibrationSequence sequence;

		private TrackerReference trackers;

		private void Start()
		{
			trackers = FindObjectOfType<TrackerReference>();

			// TMP:
			profile.Reset();

			sequence.SetupCalibrationSequence(profile, trackers, this);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.C))
			{
				sequence.NextCalibrationStep();
			}
		}
	}
}