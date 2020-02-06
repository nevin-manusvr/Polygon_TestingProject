using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Manus.Polygon;

[RequireComponent(typeof(CalibrationStepEventListener))]
public class CalibrationController : MonoBehaviour
{
    private CalibrationStepEventListener eventListener;
    
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
        eventListener = GetComponent<CalibrationStepEventListener>();

        model = GameObject.Find("CalibrationInstructionModel");
        modelAnimator = model.GetComponent<Animator>();
        
        eventListener.prepareCalibrationResponse += OnPrepareCalibration;
        eventListener.startCalibrationResponse += OnStartCalibration;
        eventListener.updateCalibrationResponse += OnUpdateCalibration;
        eventListener.finishCalibrationResponse += OnFinishCalibration;
    }

    public void Update()
    {
        if(Input.GetKey(KeyCode.Keypad1))
        {
            eventListener.CalibrationPrepareRaised();
        }
        else if (Input.GetKey(KeyCode.Keypad2))
        {
            eventListener.CalibrationStartRaised();
        }
        else if(Input.GetKey(KeyCode.Keypad3))
        {
            eventListener.CalibrationFinishedRaised();
        }
    }

    private void OnEnable() 
    {
        eventListener.prepareCalibrationResponse += OnPrepareCalibration;
        eventListener.startCalibrationResponse += OnStartCalibration;
        eventListener.updateCalibrationResponse += OnUpdateCalibration;
		eventListener.finishCalibrationResponse += OnFinishCalibration;
    }

    private void OnDisable() 
    {
        eventListener.prepareCalibrationResponse -= OnPrepareCalibration;
        eventListener.startCalibrationResponse -= OnStartCalibration;
        eventListener.updateCalibrationResponse -= OnUpdateCalibration;
        eventListener.finishCalibrationResponse -= OnFinishCalibration;
    }

    public void OnPrepareCalibration()
    { 
        //shows ui and starts countdown
        uiManager.UpdateText(calibrationStepName, discriptionCalibration);
        TriggerPoseAnimation(calibrationStepName);
    }

    public void OnStartCalibration()
    {
        Debug.Log("started calibration");
        //ResetTrigger();
        TriggerStartAnimation(calibrationStepName);
    }

    public void OnUpdateCalibration(float percentage)
    {

    }

    public void OnFinishCalibration()
    {
        ResetTrigger();
    }

    //Set trigger for animations
    void TriggerPoseAnimation(string step)
    {
        Debug.Log("set trigger");
        string currentStep = step + "CalibrationStartingPose";
        modelAnimator.SetTrigger(currentStep);
    }
    void TriggerStartAnimation(string step)
    {
        Debug.Log("set trigger");
        string currentStep = step + "CalibrationStart";
        modelAnimator.SetTrigger(currentStep);        
    }
    void ResetTrigger()
    {
        Debug.Log("Reset trigger");
        modelAnimator.SetTrigger("IsDone");
    }
}
