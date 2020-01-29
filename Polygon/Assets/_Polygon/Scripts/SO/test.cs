using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(GameEventListener))]
public class test : MonoBehaviour
{
    private GameEventListener eventListener;

    [Header("Calibration step")]
    public string calibrationStepName;
    [Header("Starting pose")]
    public Animation poseAnimation;
    [Header("Calibration animation")]
    public Animation calibrationAnimation;

    
    // Start is called before the first frame update
    void Start()
    {
        eventListener = GetComponent<GameEventListener>();
    }

    public void OnStartCountdown()
    {

    }

    public void OnStartCalibration()
    {

    }

    public void OnFinishCalibration()
    {

    }
}
