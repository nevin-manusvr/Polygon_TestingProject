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
	public Action nextCalibrationStepResponse;
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

	public void NextCalibrationStepRaised()
	{
		nextCalibrationStepResponse?.Invoke();
	}

	public void PreviousCalibrationStepRaised()
	{
		previousCalibrationStepResponse?.Invoke();
	}
}
