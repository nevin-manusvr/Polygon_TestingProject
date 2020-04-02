using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LightSwitch_Behaviour : MonoBehaviour
{

    GameObject m_UI_PhysicalButtons;
    UI_Behaviour m_UIBehviour;

    Vector3 m_StartingPos;
    public Vector3 dist;

    bool m_IsToggled;
    bool m_IsColliding;


    // Start is called before the first frame update
    void Start()
    {
        m_UI_PhysicalButtons = GameObject.Find("UI_PhysicalButtons");
        m_UIBehviour = m_UI_PhysicalButtons.GetComponent<UI_Behaviour>();

        m_StartingPos = transform.position;

        m_IsToggled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < 2.5f)
        {
            Debug.Log("IS LOWER NOW TOGGLE");
            m_UIBehviour.ToggleUI(m_IsToggled);
            SwitchBool();
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (m_IsColliding) return;
        m_IsColliding = true;
        dist = transform.position - collider.transform.position;
    }

    private void OnTriggerStay(Collider collider)
    {
        transform.position = new Vector3(transform.position.x, collider.transform.position.y + dist.y, transform.position.z);
        
        if (transform.position.y < 2.45f)
        {
            transform.localPosition = new Vector3(transform.position.x, 2.45f, transform.position.z);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (!m_IsColliding) return;
        m_IsColliding = false;
        transform.DOMoveY(m_StartingPos.y, 1f).SetEase(Ease.InOutCubic);
    }


    void SwitchBool()
    {
        m_IsToggled = !m_IsToggled;
    }
}
