using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
	[Tooltip("Event to register with.")]
	[SerializeField] private GameEvent calibrationEvent;

	[Tooltip("Response to invoke when Event is raised.")]
	public Action<float> startCountdownResponse;
	public Action<float> startCalibrationResponse;
	public Action calibrationFinishedResponse;

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

	public void StartCountDownRaised(float countdownTime)
	{
		startCountdownResponse?.Invoke(countdownTime);
	}

	public void CalibrationStartRaised(float time)
	{
		startCalibrationResponse?.Invoke(time);
	}

	public void CalibrationFinishedRaised()
	{
		calibrationFinishedResponse?.Invoke();
	}
}
