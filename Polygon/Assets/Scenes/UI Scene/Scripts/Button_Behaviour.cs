using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Button_Behaviour : MonoBehaviour
{

    UI_Behaviour m_UIBehaviour;
    UI_WelcomeBehaviour m_UIWelcomeBehaviour;

    private Camera m_Camera;
    GameObject m_Child;
    Image m_ChildImage;

    Color32 m_PressedButtonColor;
    Color32 m_ButtonColor;

    [SerializeField]
    private bool m_IsColliding;

    [SerializeField]
    private bool m_IsLocked;
    [SerializeField]
    private bool m_WasLocked;

    [SerializeField]
    Sprite m_UnlockSprite;

    [SerializeField]
    Sprite m_LockSprite;

    private void Start()
    {
        m_IsLocked = false;
        m_WasLocked = true;

        m_PressedButtonColor = new Color32(86, 173, 214, 255);
        m_ButtonColor = new Color32(255,255,255,255);


        m_UIBehaviour = GetComponentInParent<UI_Behaviour>();
        m_UIWelcomeBehaviour = GameObject.Find("UI_WelcomeScreen").GetComponent<UI_WelcomeBehaviour>();
        m_Camera = Camera.main;
        m_Child = transform.GetChild(0).gameObject;
        m_ChildImage = m_Child.GetComponent<Image>();
    }
    void OnTriggerEnter(Collider collider)
    {
        //checks if "collider" is either the left or right hand.
        if(collider.gameObject.layer == 10 || collider.gameObject.layer == 11)
        {
            transform.GetComponent<Image>().color = m_PressedButtonColor;
            m_ChildImage.DOColor(m_PressedButtonColor, .1f);
        
            if(m_IsColliding) return;
            m_IsColliding = true;

            if(gameObject.tag == "Begin")
            {
                m_UIWelcomeBehaviour.CloseWelcome();
            }
            else
            {
                m_UIBehaviour.ButtonFunction(transform.tag);
            }
        }
        else
        {
            return;
        }        
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.layer == 10 || collider.gameObject.layer == 11)
        {
            //makes it visually look like you're pushing the button
            float dist = Vector3.Distance(m_Camera.transform.position, collider.transform.position);
            float m_handposition = dist * 100;

            m_Child.transform.localPosition = new Vector3(0, 0, m_handposition);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == 10 || collider.gameObject.layer == 11)
        {
            if (!m_IsColliding) return;
            m_IsColliding = false;

            m_ChildImage.DOColor(m_ButtonColor, .1f);
            Vector3 Startpos =  new Vector3(transform.localPosition.x, 0, 0);
            m_Child.transform.DOLocalMoveZ(0, 0.1f);

        }
    }
}
