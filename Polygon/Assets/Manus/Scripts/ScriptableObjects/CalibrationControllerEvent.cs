using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Calibration Controller Event", menuName = "Manus/ScriptableObjects/Calibration Controller Event")]
public class CalibrationControllerEvent : ScriptableObject
{
	private readonly List<CalibrationControllerEventListener> eventListeners = new List<CalibrationControllerEventListener>();

	public void StartCalibrationSequence()
	{
		for (int i = eventListeners.Count - 1; i >= 0; i--)
			eventListeners[i].StartCalibrationSequenceRaised();
	}

	public void RaiseSetupNextStep()
	{
		for (int i = eventListeners.Count - 1; i >= 0; i--)
			eventListeners[i].SetupNextCalibrationStepRaised();
	}

	public void RaiseStartNextStep()
	{
		for (int i = eventListeners.Count - 1; i >= 0; i--)
			eventListeners[i].StartNextCalibrationStepRaised();
	}

	public void RaisePreviousStep()
	{
		for (int i = eventListeners.Count - 1; i >= 0; i--)
			eventListeners[i].PreviousCalibrationStepRaised();
	}

	// Register
	public void RegisterListener(CalibrationControllerEventListener listener)
	{
		if (!eventListeners.Contains(listener))
			eventListeners.Add(listener);
	}

	public void UnregisterListener(CalibrationControllerEventListener listener)
	{
		if (eventListeners.Contains(listener))
			eventListeners.Remove(listener);
	}
}