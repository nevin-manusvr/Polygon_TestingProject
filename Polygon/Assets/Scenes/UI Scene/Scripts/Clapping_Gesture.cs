using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clapping_Gesture : MonoBehaviour
{

    GameObject m_UI_PhysicalButtons;
    UI_Behaviour m_UIBehviour;

    private bool isVisible;
    [SerializeField]
    private GameObject m_LeftHand;
    [SerializeField]
    private GameObject m_RightHand;

    [Header("Distance")]
    public float m_Dist;

    public float m_MinRangeDist;
    public float m_MaxRangeDist;

    [Header("Time")]
    public float endTime;
    public float time;
    public bool m_timerHasStarted;

    [Header("bools")]
    public bool m_HasClappedOnce;




    // Start is called before the first frame update
    void Start()
    {

        m_UI_PhysicalButtons = GameObject.Find("UI_PhysicalButtons");
        m_UIBehviour = m_UI_PhysicalButtons.GetComponent<UI_Behaviour>();

        isVisible = true;

        m_MinRangeDist = .10f;
        m_MaxRangeDist = .20f;

        //m_timer = 2f;
        m_timerHasStarted = false;

        m_HasClappedOnce = false;
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.time;


        //finds both hands
        if (m_LeftHand == null || m_RightHand == null)
        {
            m_LeftHand = GameObject.FindGameObjectWithTag("LeftHand");
            m_RightHand = GameObject.FindGameObjectWithTag("RightHand");
        }
        //gets distance between both hands
        if(m_LeftHand != null && m_RightHand != null)
        {
            m_Dist = Vector3.Distance(m_LeftHand.transform.position, m_RightHand.transform.position);
        }


        if(m_Dist < m_MaxRangeDist && m_Dist > m_MinRangeDist)
        {
            if (m_timerHasStarted) return;
            m_timerHasStarted = true;
            
            //start timer
            endTime = Mathf.RoundToInt(Time.time + 1.5f);
            
            //see if dist goes higher than max dist and if gets back lower than max dist within time, it does? close/open ui.
        } 

        //check if timer has started and is user has clapped once

        if(m_timerHasStarted && m_Dist > m_MaxRangeDist)
        {
            m_HasClappedOnce = true;
        }
        if(m_HasClappedOnce && m_Dist < m_MaxRangeDist)
        {
            Debug.Log("SESAM OPEN THE UI!!!!");
            m_UIBehviour.ToggleUI(isVisible);
            SwitchBool();
        }

        //checks if timer has ended
        if (Mathf.RoundToInt(Time.time) == endTime)
        {
            if (!m_timerHasStarted) return;
            m_timerHasStarted = false;
            m_HasClappedOnce = false;


            Debug.Log("HEEEEEEEEEEEEEELLLLOOOOOOOOOOOOOO");
        }
    }

    void SwitchBool()
    {
        isVisible = !isVisible;
    }



}
