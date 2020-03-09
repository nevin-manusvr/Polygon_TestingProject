using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Manus.Polygon;
using DG.Tweening;
using TMPro;

public class UI_Behaviour : MonoBehaviour
{

    public CalibrationControllerEvent controllerEvent;
	public CalibrationSequence sequence;

    [Header("Guide Canvas")]
    [SerializeField]
    private CanvasGroup m_GuideCanvas;
    [SerializeField]
    private List<CanvasGroup> m_List;
    private int m_GuideIndex;
    [SerializeField]
    CanvasGroup m_WelcomeText;
    [SerializeField]
    CanvasGroup m_ProgressText;
    [SerializeField]
    CanvasGroup m_ControllerText;
    [SerializeField]
    CanvasGroup m_ToggleText;
    [SerializeField]
    private GameObject m_GuideButtons;


    [Header("Progress Canvas")]
    [SerializeField]
    private CanvasGroup m_ProgressCanvas;

    [Header("Controller Canvas")]
    [SerializeField]
    private CanvasGroup m_ControllerCanvas;
    [SerializeField]
    private GameObject m_ControllerButtons;
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

    [Header("Toggle Canvas")]
    [SerializeField]
    private CanvasGroup m_ToggleCanvas;


    [Header("Buttons")]

    [SerializeField]
    private bool m_ButtonsAreActive;
    [SerializeField]
    private bool m_AreSwitched;

    [Header("Slider")]
    [SerializeField]
    private Image m_SliderImage;
    [SerializeField]
    private Image m_CurrentStepSliderImages;
    private int m_CurrentStep;

    [SerializeField]
    private TextMeshProUGUI m_CurrentStepText;

    [SerializeField]
    private CanvasGroup m_GetReadyText;
    [SerializeField]
    private CanvasGroup m_CalibratingText;

    private Camera m_Camera;


    

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = Camera.main;

        m_GuideIndex = 1;

        controllerEvent.StartCalibrationSequence();
        m_ButtonsAreActive = true;
        m_AreSwitched = false;
        ToggleUIButtons();
        m_GetReadyText.alpha = 0;
        m_CalibratingText.alpha = 0;

        m_CurrentStep = 0;


        m_PlayButtonObj = gameObject.transform.GetChild(0).GetChild(1).GetChild(0).gameObject;

        m_CheckButtonObj = gameObject.transform.GetChild(0).GetChild(1).GetChild(2).gameObject;
        m_CheckButtonObj.SetActive(false);
        m_CheckButton.alpha = 0;

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(m_Camera.transform.position.x, m_Camera.transform.position.y / 1.6f, m_Camera.transform.position.z + 0.35f);
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
                    m_CurrentStep += 1;
                    UpdateCurrentStep();
			    }

			    break;
		    case "Previous":

                controllerEvent.RaisePreviousStep();
                controllerEvent.RaiseSetupNextStep();
                Debug.Log(sequence.currentIndex);
                if(m_CurrentStep > 0)
                {
                    m_CurrentStep -= 1;
                }
                UpdateCurrentStep();
                Debug.Log(sequence.isFinished);
                if(sequence.isFinished)
                {
                    sequence.isFinished = false;
                    SwitchButtons();
                }
                
                break;
            case "Check":

                SwitchButtons();

                break;
            case "StartGuide":

                StartGuide(m_GuideIndex);

                break;
            case "SkipGuide":

                SkipGuide();

                break;
	    }
	}

    void StartSlider()
    {
        m_PlayButton.DOFade(0, 1f);
        m_PreviousButton.DOFade(0, 1f).OnComplete( () => ToggleUIButtons());
        m_GetReadyText.DOFade(1, .5f).SetEase(Ease.InOutCubic);

        
        m_SliderImage.DOFillAmount(1, 4f).SetEase(Ease.InOutCubic).OnComplete(() => {
                                                                                        controllerEvent.RaiseStartNextStep();
                                                                                        m_GetReadyText.DOFade(0, .5f).SetEase(Ease.InOutCubic);
                                                                                        UndoSlider(); 
                                                                                    });

        
    }

    void UndoSlider()
    {
        m_CalibratingText.DOFade(1, .5f).SetEase(Ease.InOutCubic).SetDelay(.2f);
        m_SliderImage.DOFillAmount(0, 4f).SetEase(Ease.InOutCubic).OnComplete(() => {   
                                                                                        if(sequence.currentIndex == sequence.calibrationSteps.Count)
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
        m_ControllerButtons.SetActive(m_ButtonsAreActive);
    }

    public void ToggleUI(bool isVisible)
    {
        if(isVisible)
        {
            m_ControllerCanvas.DOFade(1, 1f).SetEase(Ease.InOutCubic);
            m_ProgressCanvas.DOFade(1, 1f).SetEase(Ease.InOutCubic);
        }
        else
        {
            m_ControllerCanvas.DOFade(0, 1f).SetEase(Ease.InOutCubic);
            m_ProgressCanvas.DOFade(0, 1f).SetEase(Ease.InOutCubic);
        }
        
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

    private void UpdateCurrentStep()
    {
        float sliderValue = 1f / 8f * m_CurrentStep;
        m_CurrentStepSliderImages.DOFillAmount(sliderValue, .4f).SetEase(Ease.InOutCubic);
        m_CurrentStepText.text = m_CurrentStep.ToString();
    }

    private void StartGuide(int index)
    {
        if(index < 4)
        {
            m_List[index - 1].DOFade(0, .2f);
            m_List[index].DOFade(1, .2f);
        }

        switch (index)
        {
            case 1:
                m_ProgressCanvas.DOFade(1, .5f).SetEase(Ease.InOutCubic);
                break;
            case 2:
                ToggleUIButtons();
                m_ControllerCanvas.DOFade(1, 0.5f).SetEase(Ease.InOutCubic);
                break;
            case 3:
                m_ToggleCanvas.DOFade(1, 0.5f).SetEase(Ease.InOutCubic);              
                break;
            case 4:
                CloseGuide();
                break;
        }
        m_GuideIndex += 1;


        /*
        go through steps
        - show progress canvas and switch text
        - show controller canvas and switch text
        - show toggle canvas and switch text
        - finally close guide canvas 
        */
    }

    private void CloseGuide()
    {
        m_GuideCanvas.DOFade(0, 0.5f).SetEase(Ease.InOutCubic);
        m_GuideButtons.SetActive(false);

    }
    private void SkipGuide()
    {
        CloseGuide();

        m_ProgressCanvas.DOFade(1, .5f).SetEase(Ease.InOutCubic);

        m_ControllerCanvas.DOFade(1, 0.5f).SetEase(Ease.InOutCubic);
        
        m_ToggleCanvas.DOFade(1, 0.5f).SetEase(Ease.InOutCubic);
        /*
        close guide canvas
        open Progress, controller anf toggle canvas
        */
    }







}
