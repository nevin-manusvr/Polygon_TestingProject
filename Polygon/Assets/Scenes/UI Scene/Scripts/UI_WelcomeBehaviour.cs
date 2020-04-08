using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Manus.Polygon;
using DG.Tweening;
using TMPro;

public class UI_WelcomeBehaviour : MonoBehaviour
{

    [Header("Welcome Canvas")]
    [SerializeField]
    private CanvasGroup m_WelcomeCanvas;
    [SerializeField]
    private GameObject m_WelcomeButton;
    [SerializeField]
    private CanvasGroup m_BeginButton;
    [SerializeField]
    private bool m_ButtonIsActive;
    [SerializeField]
    public bool m_WelcomeIsActive;





    private Camera m_Camera;

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = Camera.main;

        m_WelcomeCanvas = gameObject.GetComponentInChildren<CanvasGroup>();
        m_WelcomeButton = transform.GetChild(0).GetChild(3).gameObject;
        m_BeginButton = m_WelcomeButton.GetComponentInChildren<CanvasGroup>();
        m_WelcomeButton.SetActive(false);
        m_ButtonIsActive = false;
        m_WelcomeIsActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(m_Camera.transform.position.x, m_Camera.transform.position.y / 1.1f, m_Camera.transform.position.z + 0.38f);
    }

    //toggles button when standing in/out of the red circle
    public void ToggleButton(bool isActive)
    {
        m_WelcomeButton.SetActive(isActive);
    }


    //shows button with animation
    public void ShowButton()
    {
        m_BeginButton.DOFade(1f, .2f).SetEase(Ease.InOutCubic);
    }

    //hides button with animation
    public void HideButton()
    {
        m_BeginButton.DOFade(0f, .2f).SetEase(Ease.InOutCubic);
    }


    public void CloseWelcome()
    {
        m_WelcomeCanvas.DOFade(0f, .4f).SetEase(Ease.InOutCubic).OnComplete(() => 
        {
            ToggleButton(false);
            m_WelcomeIsActive = false;
        });
    }
}
