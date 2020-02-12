using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CalibrationStepEventListener : MonoBehaviour
{
	[Tooltip("Event to register with.")]
	[SerializeField] private CalibrationStepEvent calibrationEvent;

	[Tooltip("Response to invoke when Event is raised.")]
	public Action prepareCalibrationResponse;
	public Action startCalibrationResponse;
	public Action<float> updateCalibrationResponse;
	public Action finishCalibrationResponse;

	#region Monobehaviour Callbacks

	private void OnEnable()
	{
		calibrationEvent?.RegisterListener(this);
	}

	private void OnDisable()
	{
		calibrationEvent?.UnregisterListener(this);
	}

	#endregion

	public void CalibrationPrepareRaised()
	{
		prepareCalibrationResponse?.Invoke();
	}

	public void CalibrationStartRaised()
	{
		startCalibrationResponse?.Invoke();
	}

	public void CalibrationUpdateRaised(float percentage)
	{
		updateCalibrationResponse?.Invoke(percentage);
	}

	public void CalibrationFinishedRaised()
	{
		finishCalibrationResponse?.Invoke();
	}
}
