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
    public UI_Behaviour m_UIBehaviour;

    [Header("Calibration step")]
    public string calibrationStepName;
    [TextArea] public string discriptionCalibration;


    [Header("Animations")]
    [SerializeField]
    private GameObject model;
    private Animator modelAnimator;
    private Vector3 m_CentrePos;
    private Vector3 m_StartPos;
	[SerializeField] private string animationPoseTrigger;
	[SerializeField] private string animationCalibrationTrigger;

	void Awake()
    {
        eventListener = GetComponent<CalibrationStepEventListener>();

        m_UIBehaviour = FindObjectOfType<UI_Behaviour>();

        model = GameObject.Find("CalibrationInstructionModel_" + this.gameObject.name.ToString());
        modelAnimator = model.GetComponent<Animator>();
        m_StartPos = model.transform.position;
        m_CentrePos = new Vector3(1, 0, 3);


        
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
        m_UIBehaviour.UpdateCurrentStep(calibrationStepName);
        TriggerPoseAnimation(calibrationStepName);
        MoveModelToCenter();
    }

    public void OnStartCalibration()
    {
        TriggerStartAnimation(calibrationStepName);
    }

    public void OnUpdateCalibration(float percentage)
    {
    }

    public void OnFinishCalibration()
    {
        ResetTrigger();
        MoveBackToPlace();
    }

    //Set trigger for animations
    void TriggerPoseAnimation(string step)
    {
        string currentStep = step + "_CalibrationStartingPose";
        modelAnimator.SetTrigger(animationPoseTrigger);
    }
    void TriggerStartAnimation(string step)
    {
        string currentStep = step + "_CalibrationStart";
        modelAnimator.SetTrigger(animationCalibrationTrigger);        
    }
    void ResetTrigger()
    {
        modelAnimator.SetTrigger("IsDone");
    }


    void MoveModelToCenter()
    {
        model.transform.DOMove(m_CentrePos, 2f);
    } 

    void MoveBackToPlace()
    {
        model.transform.DOMove(m_StartPos, 2f);
    }
}
