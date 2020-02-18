using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Trigger : MonoBehaviour
{

    UI_Behaviour m_UIBehaviour;

    private Camera m_Camera;
    GameObject m_Child;

    private void Start()
    {
        m_UIBehaviour = GetComponentInParent<UI_Behaviour>();
        m_Camera = Camera.main;
        m_Child = transform.GetChild(0).gameObject;
    }
    void OnTriggerEnter(Collider collider)
    {
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
    }

    void OnTriggerStay(Collider collider)
    {
        float dist = Vector3.Distance(m_Camera.transform.position, collider.transform.position);
        float m_handposition = dist * 100;
    
        m_Child.transform.localPosition = new Vector3( m_handposition, transform.localPosition.y, transform.localPosition.z);
    }

    void OnTriggerExit(Collider collider)
    {
        Vector3 Startpos =  new Vector3(transform.localPosition.x, 0, 0);
        m_Child.transform.DOLocalMoveX(0, 0.1f);
    }
}
