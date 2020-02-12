using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CalibrationControllerEventListener : MonoBehaviour
{
	[Tooltip("Event to register with.")]
	[SerializeField] private CalibrationControllerEvent controllerEvent;

	[Tooltip("Response to invoke when Event is raised.")]
	public Action startCalibrationSequenceResponse;
	public Action setupNextCalibrationStepResponse;
	public Action startNextCalibrationStepResponse;
	public Action previousCalibrationStepResponse;

	#region Monobehaviour Callbacks

	private void OnEnable()
	{
		controllerEvent?.RegisterListener(this);
	}

	private void OnDisable()
	{
		controllerEvent?.UnregisterListener(this);
	}

	#endregion

	public void StartCalibrationSequenceRaised()
	{
		startCalibrationSequenceResponse?.Invoke();
	}
	
	public void SetupNextCalibrationStepRaised()
	{
		setupNextCalibrationStepResponse?.Invoke();
	}

	public void StartNextCalibrationStepRaised()
	{
		startNextCalibrationStepResponse?.Invoke();
	}

	public void PreviousCalibrationStepRaised()
	{
		previousCalibrationStepResponse?.Invoke();
	}
}
