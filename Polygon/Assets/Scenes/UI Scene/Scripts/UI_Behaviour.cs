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

    [Header("Progress Canvas")]
    [SerializeField]
    private CanvasGroup m_ProgressCanvas;
    [SerializeField]
    private Image m_CurrentStepSliderImage;
    private int m_CurrentStep;

    [SerializeField]
    public TextMeshProUGUI m_CurrentStepText;

    [SerializeField]
    private TextMeshProUGUI m_CurrentStepIndexText;    
   
    [Header("Second Progress Canvas")]
    [SerializeField]
    private CanvasGroup m_SecondProgressCanvas;
    [SerializeField]
    private Image m_SliderImage;
    [SerializeField]
    private TextMeshProUGUI m_GetReadyText;

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
    [SerializeField]
    private Image m_FocusImage;

    [Header("Buttons")]

    [SerializeField]
    public bool m_ButtonsAreActive;
    [SerializeField]
    private bool m_AreSwitched;

    [Header("Instructor")]
    [SerializeField]
    private Image m_InstructorFocusImage;

    private Camera m_Camera;
    private GameObject m_UI_Platform;


    

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = Camera.main;
        m_UI_Platform = GameObject.Find("UI_Platform");


        controllerEvent.StartCalibrationSequence();
        m_ButtonsAreActive = false;
        m_AreSwitched = false;
       

        m_CurrentStep = 0;


        m_PlayButtonObj = gameObject.transform.GetChild(2).GetChild(2).GetChild(0).gameObject;

        m_CheckButtonObj = gameObject.transform.GetChild(2).GetChild(2).GetChild(2).gameObject;
        m_CheckButtonObj.SetActive(false);
        m_CheckButton.alpha = 0;

        ToggleUI(false);

    }

    // Update is called once per frame
    void Update()
    {
        //change to fixed position of the red circle
        //transform.position = new Vector3(m_Camera.transform.position.x - 0.1f, m_Camera.transform.position.y / 1.1f, m_Camera.transform.position.z + 0.35f);
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
                UpdateCurrentStep(null);
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
	    }
	}

    void StartSlider()
    {
        m_PlayButton.DOFade(0, .3f);
        m_PreviousButton.DOFade(0, .3f).OnComplete( () => ToggleUIButtons());
        //m_GetReadyText.DOFade(1, .5f).SetEase(Ease.InOutCubic);
        FocusInstructor();
        m_GetReadyText.text = "Asume pose";


        m_SliderImage.DOFillAmount(.5f, 4f).SetEase(Ease.InOutCubic).OnComplete(() => {
                                                                                        controllerEvent.RaiseStartNextStep();
                                                                                        //m_GetReadyText.DOFade(0, .5f).SetEase(Ease.InOutCubic);
                                                                                        UndoSlider(); 
                                                                                    });

        
    }

    void UndoSlider()
    {
        m_GetReadyText.text = "Calibrating";
        //m_CalibratingText.DOFade(1, .5f).SetEase(Ease.InOutCubic).SetDelay(.2f);
        m_SliderImage.DOFillAmount(1, 4f).SetEase(Ease.InOutCubic).OnComplete(() => {   
                                                                                        if(sequence.currentIndex == sequence.calibrationSteps.Count)
			                                                                            {
                                                                                            SwitchButtons();
			                                                                            }
                                                                                        //m_CalibratingText.DOFade(0, .5f).SetEase(Ease.InOutCubic);
                                                                                        ToggleUIButtons(); 
                                                                                        m_PlayButton.DOFade(1, .3f).SetEase(Ease.InOutCubic);
                                                                                        m_PreviousButton.DOFade(1, .3f).SetEase(Ease.InOutCubic);
                                                                                        m_GetReadyText.text = "";
                                                                                        m_SliderImage.DOFillAmount(0, 0f);
                                                                                        UpdateCurrentStep(null);
                                                                                        FocusUI();
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
            m_SecondProgressCanvas.DOFade(1, 1f).SetEase(Ease.InOutCubic);
        }
        else
        {
            m_ControllerCanvas.DOFade(0, 1f).SetEase(Ease.InOutCubic);
            m_ProgressCanvas.DOFade(0, 1f).SetEase(Ease.InOutCubic);
            m_SecondProgressCanvas.DOFade(0, 1f).SetEase(Ease.InOutCubic);
        }
        
    }
    //sets ui hight to hight of user
    private bool m_UIHightSet = false;
    public void SetUIHight()
    {
        if (m_UIHightSet) return;
        m_UIHightSet = true;
        transform.position = new Vector3(m_UI_Platform.transform.position.x, m_Camera.transform.position.y / 1.1f, m_UI_Platform.transform.position.z + .5f);
    }

    private void SwitchButtons()
    {
        m_AreSwitched = !m_AreSwitched;
        
        if (m_AreSwitched)
        {
            m_PlayButton.DOFade(0, .1f).OnComplete(() => {  m_PlayButtonObj.SetActive(false); 
                                                            m_CheckButtonObj.SetActive(true);
                                                            });
            m_CheckButton.DOFade(1, .1f).SetDelay(.3f);
        }
        else
        {
            m_CheckButton.DOFade(0, .1f).OnComplete(() => { m_PlayButtonObj.SetActive(true); 
                                                            m_CheckButtonObj.SetActive(false);
                                                        });
            m_PlayButton.DOFade(1, .1f).SetDelay(.3f);
        }

    }

    public void UpdateCurrentStep(string currentStepText)
    {
        float sliderValue = 1f / 8f * m_CurrentStep;
        m_CurrentStepSliderImage.DOFillAmount(sliderValue, .4f).SetEase(Ease.InOutCubic);
        m_CurrentStepIndexText.text = m_CurrentStep.ToString();
        if(currentStepText != null)
        {
            m_CurrentStepText.text = currentStepText;
        }
    }

    private void FocusInstructor() 
    {
        Vector3 minScale = new Vector3(1, 1, 1);
        Vector3 maxScale = new Vector3(2, 2, 2);
        for (int i = 0; i < 3; i++)
        {
            m_InstructorFocusImage.transform.DOScale(minScale, .5f).SetDelay(i).SetEase(Ease.InOutCubic).OnComplete(() => { m_InstructorFocusImage.transform.localScale = maxScale ; });
            m_InstructorFocusImage.DOFade(1, .25f).SetDelay(i).SetEase(Ease.InOutCubic).OnComplete(() => { m_InstructorFocusImage.DOFade(0, .25f); });
        }

    }

    private void FocusUI() 
    {
        for (int i = 0; i < 3; i++)
        {
            m_FocusImage.DOFade(1, .4f).SetDelay(i).OnComplete(() => { m_FocusImage.DOFade(0, .4f); });
        }
    }
}
