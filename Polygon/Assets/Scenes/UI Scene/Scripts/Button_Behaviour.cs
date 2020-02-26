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

    private void Start()
    {
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

    }

    void OnTriggerExit(Collider collider)
    {
        m_ChildImage.DOColor(m_ButtonColor, .1f);
        Vector3 Startpos =  new Vector3(transform.localPosition.x, 0, 0);
        m_Child.transform.DOLocalMoveZ(0, 0.1f);
        CanCollide();
    }


    void CanCollide()
    {
        m_IsColliding = false;
    }
}
