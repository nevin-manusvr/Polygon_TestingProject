using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Button_Behaviour : MonoBehaviour
{

    UI_Behaviour m_UIBehaviour;

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
    Sprite m_UnlockSprite;

    [SerializeField]
    Sprite m_LockSprite;

    private void Start()
    {
        m_IsLocked = false;

        m_PressedButtonColor = new Color32(140,24,18,255);
        m_ButtonColor = new Color32(25,30,34,255);


        m_UIBehaviour = GetComponentInParent<UI_Behaviour>();
        m_Camera = Camera.main;
        m_Child = transform.GetChild(0).gameObject;
        m_ChildImage = m_Child.GetComponent<Image>();
    }
    void OnTriggerEnter(Collider collider)
    {

        transform.GetComponent<Image>().color = new Color32(140,24,18,255);
        m_ChildImage.DOColor(m_PressedButtonColor, .1f);
        
        if(m_IsColliding) return;
        m_IsColliding = true;

        Debug.Log("colliding with: " + collider);
        if(this.gameObject.CompareTag("Start"))
        {
            Debug.Log("Continue");
            m_UIBehaviour.ButtonFunction(transform.tag);
            //next step
        }
        else if(this.gameObject.CompareTag("Previous"))
        {
            Debug.Log("Previous");
            m_UIBehaviour.ButtonFunction(transform.tag);
            //previous step
        }
        else if(this.gameObject.CompareTag("Check"))
        {
            m_UIBehaviour.ButtonFunction(transform.tag);
        }
        
    }

    void OnTriggerStay(Collider collider)
    {
        float dist = Vector3.Distance(m_Camera.transform.position, collider.transform.position);
        float m_handposition = dist * 100;
    
        m_Child.transform.localPosition = new Vector3(0, 0, m_handposition);

        if(this.gameObject.CompareTag("Slider"))
        {
            transform.position = new Vector3(collider.transform.position.x, transform.position.y ,transform.position.z);
            
            if(transform.localPosition.x < -125)
            {
                transform.localPosition = new Vector3(-125, transform.position.y, transform.position.z);
            }
            else if (transform.localPosition.x > 125)
            {
                transform.localPosition = new Vector3(125, transform.position.y, transform.position.z);
            }
        }

    }

    void OnTriggerExit(Collider collider)
    {
        if(!m_IsColliding) return;
        m_IsColliding = false;

        m_ChildImage.DOColor(m_ButtonColor, .1f);
        Vector3 Startpos =  new Vector3(transform.localPosition.x, 0, 0);
        m_Child.transform.DOLocalMoveZ(0, 0.1f);
        if(this.gameObject.CompareTag("Slider"))
        {
            Debug.Log("exits");
           if (transform.localPosition.x >= 40)
            {
                m_IsLocked = true;
                transform.DOLocalMoveX(125, .2f).OnComplete(() => SwitchImage());
            }
            else if (transform.localPosition.x <= -40)
            {
                m_IsLocked = false;
                transform.DOLocalMoveX(-125, .2f).OnComplete(() => SwitchImage());
            }
            else if(transform.localPosition.x < 40 && transform.localPosition.x > -40)
            {
                if(m_IsLocked)
                {
                    transform.DOLocalMoveX(125, .2f);
                }
                else
                {
                    transform.DOLocalMoveX(-125, .2f);
                }
            }
        }
    }

    void SwitchImage()
    {
        if(m_IsLocked)
        {
            m_ChildImage.sprite = m_LockSprite;
        } 
        else
        {
            m_ChildImage.sprite = m_UnlockSprite;
        }
        m_UIBehaviour.ToggleUI(m_IsLocked);
        //switches slider image to either lock or unlock
        //toggles ui
    }
}
