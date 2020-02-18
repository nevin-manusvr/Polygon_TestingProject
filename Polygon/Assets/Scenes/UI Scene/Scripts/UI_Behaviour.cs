﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Manus.Polygon;
using DG.Tweening;

public class UI_Behaviour : MonoBehaviour
{

    public CalibrationControllerEvent controllerEvent;
	public CalibrationSequence sequence;

    [Header("Canvas")]
    [SerializeField]
    private CanvasGroup m_Canvas;

    [Header("Buttons")]

    [SerializeField]
    private GameObject m_Buttons;
    [SerializeField]
    private bool m_ButtonsAreActive;
    [SerializeField]
    private CanvasGroup m_PlayButton;
    [SerializeField]
    private CanvasGroup m_PreviousButton;

    [Header("Slider")]
    [SerializeField]
    private Image m_SliderImage;

    private Camera m_Camera;

    

    // Start is called before the first frame update
    void Start()
    {
        controllerEvent.StartCalibrationSequence();
        m_Camera = Camera.main;
        m_ButtonsAreActive = true;

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(m_Camera.transform.position.x, m_Camera.transform.position.y / 1.5f, m_Camera.transform.position.z + 0.35f);
    }


    public void ButtonFunction(string buttonTag)
    {

	    switch (buttonTag)
	    {
		    case "Start":
			    
			    if (!sequence.isFinished)
			    {
				    controllerEvent.RaiseSetupNextStep();
                    StartSlider();
			    }
			    else
			    {
                    HideUI();
			    }
			    break;
		    case "Previous":
                controllerEvent.RaisePreviousStep();
                StartSlider();
			    break;
	    }
	}

    void StartSlider()
    {
        m_PlayButton.DOFade(0, 1f);
        m_PreviousButton.DOFade(0, 1f).OnComplete( () => ToggleUIButtons());

        m_SliderImage.DOFillAmount(1, 6f).OnComplete(() => {controllerEvent.RaiseStartNextStep(); UndoSlider(); });

        
    }

    public void UndoSlider()
    {
        m_SliderImage.DOFillAmount(0, 6f).OnComplete(() => {    controllerEvent.RaiseSetupNextStep();
                                                                ToggleUIButtons(); 
                                                                m_PlayButton.DOFade(1, 1f);
                                                                m_PlayButton.interactable = true;
                                                                m_PreviousButton.DOFade(1, 1f);
                                                                m_PreviousButton.interactable = true;});
    }


    public void NextStep()
    {
        if(sequence.isFinished)
        {
            HideUI();
        }
    }
    public void ToggleUIButtons()
    {
        m_ButtonsAreActive = !m_ButtonsAreActive;
        m_Buttons.SetActive(m_ButtonsAreActive);
    }

    public void HideUI()
    {
        m_Canvas.DOFade(0, 8f).SetEase(Ease.InOutCubic);
    }
        
}