using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Area : MonoBehaviour
{
    GameObject m_UI_PhysicalButtons;
    UI_Behaviour m_UIBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        m_UI_PhysicalButtons = GameObject.Find("UI_PhysicalButtons");
        m_UIBehaviour = m_UI_PhysicalButtons.GetComponent<UI_Behaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            m_UIBehaviour.ToggleUI(true);
        }
        else
        {
            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            m_UIBehaviour.ToggleUI(false);
        }
        else
        {
            return;
        }
    }
}
