using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Calibration Step Event", menuName = "Manus/ScriptableObjects/Calibration Step Event")]
public class CalibrationStepEvent : ScriptableObject
{
	private readonly List<CalibrationStepEventListener> eventListeners = new List<CalibrationStepEventListener>();

	public void RaiseStartCalibration()
	{
		for (int i = eventListeners.Count - 1; i >= 0; i--)
			eventListeners[i].CalibrationStartRaised();
	}

	public void RaisePrepareCalibration()
	{
		for (int i = eventListeners.Count - 1; i >= 0; i--)
			eventListeners[i].CalibrationPrepareRaised();
	}

	public void RaiseUpdateCalibration(float percentage)
	{
		for (int i = eventListeners.Count - 1; i >= 0; i--)
			eventListeners[i].CalibrationUpdateRaised(percentage);
	}

	public void RaiseFinishedCalibration()
	{
		for (int i = eventListeners.Count - 1; i >= 0; i--)
			eventListeners[i].CalibrationFinishedRaised();
	}

	// Register
	public void RegisterListener(CalibrationStepEventListener listener)
	{
		if (!eventListeners.Contains(listener))
			eventListeners.Add(listener);
	}

	public void UnregisterListener(CalibrationStepEventListener listener)
	{
		if (eventListeners.Contains(listener))
			eventListeners.Remove(listener);
	}
}