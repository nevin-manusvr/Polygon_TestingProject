using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{

    [Header("MenuCanvas")]
    [SerializeField] private CanvasGroup m_MenuCanvas;
    
    
    [Header("Calibration Start Canvas")]
    [SerializeField] private CanvasGroup m_CalibrationStartCanvas;
    [SerializeField] private Text m_StartStep;
    [SerializeField] private Text m_StartDiscription;

	
	[Header("Calibration Update Canvas")]
	[SerializeField] private CanvasGroup m_CalibrationUpdateCanvas;
    [SerializeField] private Text m_UpdateStep;
    [SerializeField] private Text m_UpdateDiscription;


    [Header("Calibration Finish Canvas")]
    [SerializeField] private CanvasGroup m_CalibrationFinishCanvas;

    public List<CanvasGroup> m_CanvasList = new List<CanvasGroup>();

    [SerializeField] private CanvasGroup m_CurrentCanvas;
    [SerializeField] private int m_CurrentCanvasID;
    [SerializeField] private int m_NextCanvasID;


    private void Awake()
    {
        //add all different canvas to list
        m_CanvasList.Add(m_MenuCanvas);
        m_CanvasList.Add(m_CalibrationStartCanvas);
        m_CanvasList.Add(m_CalibrationUpdateCanvas);
        m_CanvasList.Add(m_CalibrationFinishCanvas);


        for (int i = 0; i < m_CanvasList.Count; i++)
        {
           int x = i + 1;
           GameObject m_CanvasObject = this.gameObject.transform.GetChild(x).gameObject;
           m_CanvasList[i] = m_CanvasObject.GetComponent<CanvasGroup>();
           
           if(i >= 1)
            {
                m_CanvasList[i].alpha = 0;
                m_CanvasList[i].blocksRaycasts = false;
            }
        }

    	m_CurrentCanvasID = 0;
    }

    void Start()
    {
        m_CurrentCanvas = m_CanvasList[m_CurrentCanvasID];
        
    }
    public void LookAtButton(GameObject button)
    {
        button.GetComponent<Image>().fillAmount += 0.3f * Time.deltaTime;
        if(button.GetComponent<Image>().fillAmount == 1)
        {
            m_NextCanvasID = m_CurrentCanvasID + 1;
            string m_buttonName = button.name;

            switch(m_buttonName)
            {
                case "Start":
                    Continue(m_CurrentCanvas);
                    //function that instantiates "gameobject" which has the data of which part you're calibrating
                    break;
                case "Continue":
                    Continue(m_CurrentCanvas);
                    break;
                case "Next":
                    ReturnToStart(m_CurrentCanvas);
                    break;
                
            }
            button.GetComponent<Image>().fillAmount = 0;
            
        }
    }

    public void ResetButton(GameObject button)
    {
        if(button == null)
        {
            return;
        }
        button.GetComponent<Image>().fillAmount = 0;
    }


    public void Continue(CanvasGroup current)
    {
        m_NextCanvasID = m_CurrentCanvasID + 1;
        CanvasGroup next = m_CanvasList[m_NextCanvasID];
        
        UpdateCanvas(current, next);
        
        m_CurrentCanvas = next;
        m_CurrentCanvasID += 1;
    }

    public void ReturnToStart(CanvasGroup current)
    {
        CanvasGroup next = m_CanvasList[1];
        m_CurrentCanvasID = 1;
        m_NextCanvasID = 1;
        UpdateCanvas(current, next);
    }

    public void ReturnToMenu(CanvasGroup current)
    {
        CanvasGroup next = m_CanvasList[0];
        m_CurrentCanvasID = 0;
        m_NextCanvasID = 0;
        UpdateCanvas(current, next);
    }

    public void UpdateCanvas(CanvasGroup current, CanvasGroup next)
    {
        current.DOFade(0, 0.8f).SetEase(Ease.OutCubic).OnComplete( () => next.DOFade(1, 0.8f).SetEase(Ease.InCubic));

        current.blocksRaycasts = false;
        next.blocksRaycasts = true;

        m_CurrentCanvas = next;
    }

    public void UpdateText(string currentStep, string description)
    {
        m_StartStep.text = currentStep;
        m_StartDiscription.text = description;

        m_UpdateStep.text = currentStep;
        m_UpdateDiscription.text = description;
    }

}
