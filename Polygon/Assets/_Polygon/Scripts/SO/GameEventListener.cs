using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
	[Tooltip("Event to register with.")]
	public GameEvent CalibrationEvent;

	[Tooltip("Response to invoke when Event is raised.")]
	public UnityEvent<float> StartCountdownResponse;
	public UnityEvent<float> StartCalibrationResponse;
	public UnityEvent CalibrationFinishedResponse;

	private void OnEnable()
	{
		CalibrationEvent.RegisterListener(this);
	}

	private void OnDisable()
	{
		CalibrationEvent.UnregisterListener(this);
	}

	public void StartCountDownRaised(float countdownTime)
	{
		// DoStuff
		StartCountdownResponse?.Invoke(countdownTime);
	}

	public void CalibrationStartRaised(float time)
	{
		StartCalibrationResponse?.Invoke(time);
	}

	public void CalibrationFinishedRaised()
	{
		CalibrationFinishedResponse?.Invoke();
	}
}
