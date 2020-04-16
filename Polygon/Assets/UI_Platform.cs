using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Platform : MonoBehaviour
{
    [SerializeField]
    private GameObject m_UIPhysicalButtons;
    [SerializeField]
    private UI_Behaviour m_UIBehaviour;

    [SerializeField]
    private GameObject m_UIWelcomeScreen;
    [SerializeField]
    private UI_WelcomeBehaviour m_UIWelcomeBehaviour;

    [SerializeField]
    private GameObject m_Navi;
    [SerializeField]
    private Navi_Behaviour m_NaviBehaviour;

    private bool isStandingIn;
    bool isCalled;

    // Start is called before the first frame update
    void Start()
    {
        m_UIPhysicalButtons = GameObject.Find("UI_PhysicalButtons");
        m_UIBehaviour = m_UIPhysicalButtons.GetComponent<UI_Behaviour>();
        
        m_UIWelcomeScreen = GameObject.Find("UI_WelcomeScreen");
        m_UIWelcomeBehaviour = m_UIWelcomeScreen.GetComponent<UI_WelcomeBehaviour>();
        
        m_Navi = GameObject.Find("Navi");
        m_NaviBehaviour = m_Navi.GetComponent<Navi_Behaviour>();

        isStandingIn = false;
        isCalled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.layer == 9) 
        {
            if (isCalled) return;
            isCalled = true;
            if (m_UIWelcomeBehaviour.m_WelcomeIsActive)
            {
                m_UIWelcomeBehaviour.ToggleButton(true);
                m_UIWelcomeBehaviour.ShowButton();
                m_NaviBehaviour.m_CurrentState = Navi_Behaviour.State.Welcome;
                m_UIBehaviour.SetUIHight();

            }
            else
            {
                m_UIBehaviour.ToggleUI(true);
                m_UIBehaviour.ToggleUIButtons(true);
                m_NaviBehaviour.m_CurrentState = Navi_Behaviour.State.Controlls;
            }
        }
        else
        {
            return;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_UIBehaviour.isActive) return;

        if (other.gameObject.layer == 9)
        {
            if (!m_UIWelcomeBehaviour.m_WelcomeIsActive)
            {
                if (isStandingIn) return;
                isStandingIn = true;
                m_UIBehaviour.ToggleUI(true);
                m_UIBehaviour.ToggleUIButtons(true);

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == 9)
        {
            if (m_UIWelcomeBehaviour.m_WelcomeIsActive)
            {
                m_UIWelcomeBehaviour.HideButton();
                m_UIWelcomeBehaviour.ToggleButton(false);
                
            }
            else
            {
                m_UIBehaviour.ToggleUI(false);
                m_UIBehaviour.ToggleUIButtons(false);
            }
            isCalled = false;
            m_NaviBehaviour.m_CurrentState = Navi_Behaviour.State.Stand;
        }
    }
}
