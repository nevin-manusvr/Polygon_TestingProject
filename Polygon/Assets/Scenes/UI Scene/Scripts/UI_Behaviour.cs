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
    private bool m_AreSwitched;
    [SerializeField]
    private GameObject m_PlayButtonObj;
    [SerializeField]
    private CanvasGroup m_PlayButton;
    [SerializeField]
    private CanvasGroup m_PreviousButton;
    [SerializeField]
    private GameObject m_CheckButtonObj;
    [SerializeField]
    private CanvasGroup m_CheckButton;

    [Header("Slider")]
    [SerializeField]
    private Image m_SliderImage;

    [SerializeField]
    private CanvasGroup m_GetReadyText;
    [SerializeField]
    private CanvasGroup m_CalibratingText;

    private Camera m_Camera;

    

    // Start is called before the first frame update
    void Start()
    {



        controllerEvent.StartCalibrationSequence();
        m_Camera = Camera.main;
        m_ButtonsAreActive = true;
        m_AreSwitched = false;
        m_GetReadyText.alpha = 0;
        m_CalibratingText.alpha = 0;


        m_PlayButtonObj = gameObject.transform.GetChild(0).GetChild(2).GetChild(0).gameObject;

        m_CheckButtonObj = gameObject.transform.GetChild(0).GetChild(2).GetChild(2).gameObject;
        m_CheckButtonObj.SetActive(false);
        m_CheckButton.alpha = 0;

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

			    break;
		    case "Previous":

                controllerEvent.RaisePreviousStep();
                controllerEvent.RaiseSetupNextStep();
                Debug.Log(sequence.currentIndex);
                if(sequence.isFinished)
                {
                    //sequence.currentIndex -= 1;
                    sequence.isFinished = false;
                    SwitchButtons();
                }
                
                break;
            case "Check":

                HideUI();
                ToggleUIButtons();

                break;
	    }
	}

    void StartSlider()
    {
        m_PlayButton.DOFade(0, 1f);
        m_PreviousButton.DOFade(0, 1f).OnComplete( () => ToggleUIButtons());
        m_GetReadyText.DOFade(1, .5f).SetEase(Ease.InOutCubic);
        m_SliderImage.DOFillAmount(1, 6f).SetEase(Ease.InOutCubic).OnComplete(() => {   controllerEvent.RaiseStartNextStep(); 
                                                                                        m_GetReadyText.DOFade(0, .5f).SetEase(Ease.InOutCubic);
                                                                                        UndoSlider(); 
                                                                                    });

        
    }

    void UndoSlider()
    {
        m_CalibratingText.DOFade(1, .5f).SetEase(Ease.InOutCubic).SetDelay(.2f);
        m_SliderImage.DOFillAmount(0, 6f).SetEase(Ease.InOutCubic).OnComplete(() => {   if(sequence.currentIndex == sequence.calibrationSteps.Count)
			                                                                            {
                                                                                            SwitchButtons();
			                                                                            }
                                                                                        m_CalibratingText.DOFade(0, .5f).SetEase(Ease.InOutCubic);
                                                                                        ToggleUIButtons(); 
                                                                                        m_PlayButton.DOFade(1, 1f).SetEase(Ease.InOutCubic);
                                                                                        m_PreviousButton.DOFade(1, 1f).SetEase(Ease.InOutCubic);
                                                                                    });
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

    private void SwitchButtons()
    {
        m_AreSwitched = !m_AreSwitched;
        
        if (m_AreSwitched)
        {
            m_PlayButton.DOFade(0, .2f).OnComplete(() => {  m_PlayButtonObj.SetActive(false); 
                                                            m_CheckButtonObj.SetActive(true);
                                                            });
            m_CheckButton.DOFade(1, .2f).SetDelay(.3f);
        }
        else
        {
            m_CheckButton.DOFade(0, .2f).OnComplete(() => { m_PlayButtonObj.SetActive(true); 
                                                            m_CheckButtonObj.SetActive(false);
                                                        });
            m_PlayButton.DOFade(1, .2f).SetDelay(.3f);
        }

    }
        
}