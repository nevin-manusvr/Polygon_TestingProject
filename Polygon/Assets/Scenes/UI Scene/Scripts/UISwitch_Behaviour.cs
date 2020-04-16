using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UISwitch_Behaviour : MonoBehaviour
{
    [SerializeField]
    GameObject m_UI_PhysicalButtons;
    [SerializeField]
    UI_Behaviour m_UIBehviour;

    [SerializeField]
    private GameObject m_UIWelcomeScreen;
    [SerializeField]
    private UI_WelcomeBehaviour m_UIWelcomeBehaviour;

    Vector3 m_StartingPos;
    public Vector3 dist;

    [SerializeField]
    bool m_IsToggled;
    [SerializeField]
    bool m_IsColliding;


    // Start is called before the first frame update
    void Start()
    {
        m_UI_PhysicalButtons = GameObject.Find("UI_PhysicalButtons");
        m_UIBehviour = m_UI_PhysicalButtons.GetComponent<UI_Behaviour>();

        m_UIWelcomeScreen = GameObject.Find("UI_WelcomeScreen");
        m_UIWelcomeBehaviour = m_UIWelcomeScreen.GetComponent<UI_WelcomeBehaviour>();

        m_StartingPos = transform.position;

        m_IsToggled = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckToToggle();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == 10 || collider.gameObject.layer == 11)
        {
            if (m_IsColliding) return;
            m_IsColliding = true;
            dist = transform.position - collider.transform.position;
        }
        else return;
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.layer == 10 || collider.gameObject.layer == 11)
        {
            transform.position = new Vector3(transform.position.x, collider.transform.position.y + dist.y, transform.position.z);

            if (transform.position.y < 2.45f)
            {
                transform.localPosition = new Vector3(transform.position.x, 2.45f, transform.position.z);
            }
        }
        else return;
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == 10 || collider.gameObject.layer == 11)
        {
            if (!m_IsColliding) return;
            m_IsColliding = false;
            transform.DOMoveY(m_StartingPos.y, 1f).SetEase(Ease.InOutCubic);
        }
        else return;
    }

    private void CheckToToggle()
    {
        if (m_UIWelcomeBehaviour.m_WelcomeIsActive) return;

        if (transform.position.y < 2.5f)
        {
            Debug.Log("IS LOWER NOW TOGGLE");
            m_UIBehviour.ToggleUI(m_IsToggled);
            SwitchBool();
        }
    }


    void SwitchBool()
    {
        m_IsToggled = !m_IsToggled;
    }
}
