using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Manus.Polygon;

[RequireComponent(typeof(GameEventListener))]
public class CalibrationController : MonoBehaviour
{
    private GameEventListener eventListener;
    
    [Header("UI Script")]
    public UIManager uiManager;

    [Header("Calibration step")]
    public string calibrationStepName;
    public string discriptionCalibration;


    [Header("Animations")]
    [SerializeField] private GameObject model;
    [SerializeField] private Animator modelAnimator;


    
    // Start is called before the first frame update
    void Start()
    {
        eventListener = GetComponent<GameEventListener>();

        model = GameObject.Find("CalibrationInstructionModel");
        modelAnimator = model.GetComponent<Animator>();
        
        eventListener.startCountdownResponse += OnStartCountdown;
        eventListener.startCalibrationResponse += OnStartCalibration;
        eventListener.calibrationFinishedResponse += OnFinishCalibration;

    }

    public void Update()
    {
        if(Input.GetKey(KeyCode.Keypad1))
        {
            eventListener.StartCountDownRaised(5f);
        }
        else if (Input.GetKey(KeyCode.Keypad2))
        {
            eventListener.CalibrationStartRaised(8f);
        }
        else if(Input.GetKey(KeyCode.Keypad3))
        {
            eventListener.CalibrationFinishedRaised();
        }
    }

    private void OnEnable() 
    {
        eventListener.startCountdownResponse += OnStartCountdown;
        
        eventListener.startCalibrationResponse += OnStartCalibration;
      
        eventListener.calibrationFinishedResponse += OnFinishCalibration;
    }

    private void OnDisable() 
    {
        eventListener.startCountdownResponse -= OnStartCountdown;
       
        eventListener.startCalibrationResponse -= OnStartCalibration;
       
        eventListener.calibrationFinishedResponse -= OnFinishCalibration;
    }

    public void OnStartCountdown(float time)
    { 
        //shows ui and starts countdown
        uiManager.UpdateText(calibrationStepName, discriptionCalibration);
        TriggerPoseAnimation(calibrationStepName);
    }



    public void OnStartCalibration(float time)
    {
        Debug.Log("started calibration");
        //ResetTrigger();
        TriggerStartAnimation(calibrationStepName);
    }

    public void OnFinishCalibration()
    {
        ResetTrigger();
    }

    //Set trigger for animations
    void TriggerPoseAnimation(string step)
    {
        Debug.Log("set trigger");
        string currentStep = step + "_CalibrationStartingPose";
        modelAnimator.SetTrigger(currentStep);
    }
    void TriggerStartAnimation(string step)
    {
        Debug.Log("set trigger");
        string currentStep = step + "_CalibrationStart";
        modelAnimator.SetTrigger(currentStep);        
    }
    void ResetTrigger()
    {
        Debug.Log("Reset trigger");
        modelAnimator.SetTrigger("IsDone");
    }
}
