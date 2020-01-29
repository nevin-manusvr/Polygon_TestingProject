using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Game Event", menuName = "Manus/ScriptableObjects/Game Event")]
public class GameEvent : ScriptableObject
{
	private readonly List<GameEventListener> eventListeners = new List<GameEventListener>();

	public void RaiseStartCountdown(float time)
	{
		for (int i = eventListeners.Count - 1; i >= 0; i--)
			eventListeners[i].StartCountDownRaised(time);
	}

	public void RaiseStartCalibration(float time)
	{
		for (int i = eventListeners.Count - 1; i >= 0; i--)
			eventListeners[i].CalibrationStartRaised(time);
	}

	public void RaiseFinishedCalibration()
	{
		for (int i = eventListeners.Count - 1; i >= 0; i--)
			eventListeners[i].CalibrationFinishedRaised();
	}

	// Register
	public void RegisterListener(GameEventListener listener)
	{
		if (!eventListeners.Contains(listener))
			eventListeners.Add(listener);
	}

	public void UnregisterListener(GameEventListener listener)
	{
		if (eventListeners.Contains(listener))
			eventListeners.Remove(listener);
	}
}