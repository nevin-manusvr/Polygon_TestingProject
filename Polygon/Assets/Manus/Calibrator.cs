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
		}

		private void OnEnable()
		{
			controllerEvents.startCalibrationSequenceResponse += SetupCalibrationSequence;
			controllerEvents.setupNextCalibrationStepResponse += sequence.SetupNextCalibrationStep;
			controllerEvents.startNextCalibrationStepResponse += sequence.StartCalibrationStep;
			controllerEvents.previousCalibrationStepResponse += sequence.PreviousStep;
		}

		private void OnDisable()
		{
			controllerEvents.startCalibrationSequenceResponse -= SetupCalibrationSequence;
			controllerEvents.setupNextCalibrationStepResponse -= sequence.SetupNextCalibrationStep;
			controllerEvents.startNextCalibrationStepResponse -= sequence.StartCalibrationStep;
			controllerEvents.previousCalibrationStepResponse -= sequence.PreviousStep;
		}

		private void SetupCalibrationSequence()
		{
			// TMP:
			profile.Reset();
			sequence.SetupCalibrationSequence(profile, trackers, this);
		}
	}
}