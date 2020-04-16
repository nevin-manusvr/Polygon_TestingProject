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

    public bool isStandingIn;
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
            }
            else
            {
               /*
                m_UIBehaviour.ToggleUI(true);
                m_UIBehaviour.ToggleUIButtons();
                m_UIBehaviour.SetUIHight();
                //show ui_physical buttons*/
            }
            m_NaviBehaviour.m_CurrentState = Navi_Behaviour.State.Welcome;
        }
        else
        {
            return;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_UIBehaviour.m_ButtonsAreActive) return;

        if (other.gameObject.layer == 9)
        {
            if (!m_UIWelcomeBehaviour.m_WelcomeIsActive)
            {
                m_UIBehaviour.ToggleUI(true);
                m_UIBehaviour.ToggleUIButtons();
                m_UIBehaviour.SetUIHight();
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
                isCalled = false;
            }
            else
            {
                m_UIBehaviour.ToggleUI(false);
                m_UIBehaviour.ToggleUIButtons();
                //show ui_physical buttons
            }
            m_NaviBehaviour.m_CurrentState = Navi_Behaviour.State.Stand;
        }
    }
}
