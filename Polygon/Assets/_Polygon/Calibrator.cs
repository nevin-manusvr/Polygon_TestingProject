using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.Core.VR;

namespace Manus.Polygon
{
	[RequireComponent(typeof(CalibrationControllerEventListener))]
	public class Calibrator : MonoBehaviour
	{
		private CalibrationControllerEventListener controllerEvents;

		public CalibrationProfile profile;
		public CalibrationSequence sequence;

		private TrackerReference trackers;

		private void Awake()
		{
			trackers = FindObjectOfType<TrackerReference>();
			controllerEvents = GetComponent<CalibrationControllerEventListener>();

			// TMP:
			profile.Reset();

			sequence.SetupCalibrationSequence(profile, trackers, this);
		}

		private void OnEnable()
		{
			controllerEvents.nextCalibrationStepResponse += sequence.NextCalibrationStep;
		}

		private void OnDisable()
		{
			controllerEvents.nextCalibrationStepResponse -= sequence.NextCalibrationStep;
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