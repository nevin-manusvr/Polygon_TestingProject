using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Navi_Behaviour : MonoBehaviour
{

    [Header("Tagets")]
    [SerializeField]
    private GameObject m_UIPhysicalButtons;
    [SerializeField]
    Vector3 m_UIPos;

    [SerializeField]
    private GameObject m_UIWelcomeScreen;
    [SerializeField]
    private Vector3 m_UIWelcomePos;

    [SerializeField]
    private GameObject m_UIPlatform;
    [SerializeField]
    private Vector3 m_PlatformPos;

    [SerializeField]
    private GameObject m_Character;
    [SerializeField]
    private Vector3 m_CharacterPos;


    MeshRenderer m_meshrenderer;
    public bool isCalled;
    public bool isVisible;



    [Header("States")]
    [SerializeField]
    public State m_CurrentState;
    [SerializeField]
    private State m_PreviousState;
    public enum State
    {
        Stand,
        Welcome,
        Controlls,
        Instruction,
    }

    // Start is called before the first frame update
    void Start()
    {

        m_UIPhysicalButtons = GameObject.Find("UI_PhysicalButtons");

        m_UIWelcomeScreen = GameObject.Find("UI_WelcomeScreen");
        
        m_UIPlatform = GameObject.Find("UI_Platform");
        m_PlatformPos = m_UIPlatform.transform.position;

        m_CharacterPos = new Vector3(1.187f, 1.663f, 0.428f);

        m_meshrenderer = transform.GetComponent<MeshRenderer>();
        isCalled = false;
        isVisible = true;

        transform.position = new Vector3(m_PlatformPos.x, m_PlatformPos.y + 1, m_PlatformPos.z);
        m_CurrentState = State.Stand;
        m_PreviousState = default;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_PreviousState != m_CurrentState)
        {
            CheckStates();
        }
    }


    void CheckStates()
    {
        switch (m_CurrentState)
        {
            case State.Stand:
                CheckPreviousState();
                StandInPlatform();

                UpdatePreviousState();
                break;
            case State.Welcome:
                CheckPreviousState();
                GetPos(m_UIWelcomeScreen, m_UIWelcomePos);
                FlyTo(m_UIWelcomePos);

                //FlyIntoWelcome();


                UpdatePreviousState();
                break;
            case State.Controlls:
                CheckPreviousState();
                GetPos(m_UIPhysicalButtons, m_UIPos);
                FlyTo(m_UIPos);
                //FlyIntoControlls();


                UpdatePreviousState();
                break;
            case State.Instruction:
                CheckPreviousState();
                GetPos(m_Character, m_CharacterPos);
                FlyTo(m_CharacterPos);

                //FlyIntoInstruction();


                UpdatePreviousState();
                break;
            default:
                break;
        }
    }

    void CheckPreviousState()
    {
        switch (m_PreviousState)
        {
            case State.Stand:
                break;
            case State.Welcome:
                ToggleVisibility(true);
                break;
            case State.Controlls:
                ToggleVisibility(true);
                break;
            case State.Instruction:
                ToggleVisibility(true);
                break;
            default:
                break;
        }
    }

    //when user is not standing in platform
    void StandInPlatform()
    {
        if (isCalled) return;
        isCalled = true;

        transform.position = new Vector3(m_PlatformPos.x, m_PlatformPos.y + 1, m_PlatformPos.z);

        transform.DOMoveY(transform.position.y + .4f, 1f).SetEase(Ease.InOutCubic).OnComplete(() => 
        { 
            transform.DOMoveY(transform.position.y - .4f, 1f).SetEase(Ease.InOutCubic).OnComplete(() => 
            { 
                isCalled = false;
                if (m_CurrentState == State.Stand) 
                {
                    StandInPlatform();
                }
            }); 
        });
    }


    private void GetPos(GameObject targetObject, Vector3 targetPos)
    {
        targetPos = targetObject.transform.position;
    }
    //when user is standing in platform but hasn't pressed button of welcome

    private void FlyTo(Vector3 targetPos)
    {
        transform.DOMove(targetPos, 1f).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            ToggleVisibility(false);
        });
    }

    void ToggleVisibility(bool isVisible)
    {
        m_meshrenderer.enabled = isVisible;
    }


    void UpdatePreviousState()
    {
        m_PreviousState = m_CurrentState;
    }


    /*
    public void FlyIntoWelcome()
    {
        Vector3 t_Welcome = m_UIWelcomeScreen.transform.position;
        transform.DOMove(t_Welcome, 1f).SetEase(Ease.InOutCubic).OnComplete(() => 
        {
            ToggleVisibility(false); 
        });
    }

    //when user is standing in platform and welcome is false or "same" + is not calibrating
    void FlyIntoControlls()
    {
        Vector3 t_Controlls = m_UIPhysicalButtons.transform.position;
        transform.DOMove(t_Controlls, 1f).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            ToggleVisibility(false);
        });
    }

    //when user pressed ui button and is calibrating
    void FlyIntoInstruction()
    {
        Vector3 t_Instruction = m_CharacterPos;
        transform.DOMove(t_Instruction, 1f).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            ToggleVisibility(false);
        });
    } */

}
