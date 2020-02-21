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
    [TextArea] public string discriptionCalibration;


    [Header("Animations")]
    private GameObject model;
    private Animator modelAnimator;
	[SerializeField] private string animationPoseTrigger;
	[SerializeField] private string animationCalibrationTrigger;

	void Awake()
    {
        eventListener = GetComponent<CalibrationStepEventListener>();

        uiManager = FindObjectOfType<UIManager>();

		model = GameObject.Find("CalibrationInstructionModel");
        modelAnimator = model.GetComponent<Animator>();
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.Keypad1))
        {
            eventListener.CalibrationPrepareRaised();
        }
        else if (Input.GetKey(KeyCode.Keypad2))
        {
            eventListener.CalibrationStartRaised();
        }
        else if (Input.GetKey(KeyCode.Keypad3))
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
        //uiManager.UpdateText(calibrationStepName, discriptionCalibration);
        TriggerPoseAnimation(calibrationStepName);
    }

    public void OnStartCalibration()
    {
        TriggerStartAnimation(calibrationStepName);
    }

    public void OnUpdateCalibration(float percentage)
    {
		//uiManager.UpdateTimer(percentage);
    }

    public void OnFinishCalibration()
    {
        ResetTrigger();
		//uiManager.Continue();
    }

    //Set trigger for animations
    void TriggerPoseAnimation(string step)
    {
        //Debug.Log("set trigger");
        string currentStep = step + "_CalibrationStartingPose";
        modelAnimator.SetTrigger(animationPoseTrigger);
    }
    void TriggerStartAnimation(string step)
    {
        //Debug.Log("set trigger");
        string currentStep = step + "_CalibrationStart";
        modelAnimator.SetTrigger(animationCalibrationTrigger);        
    }
    void ResetTrigger()
    {
        //Debug.Log("Reset trigger");
        modelAnimator.SetTrigger("IsDone");
    }
}
