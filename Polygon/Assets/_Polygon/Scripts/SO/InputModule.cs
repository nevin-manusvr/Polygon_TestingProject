using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class InputModule : BaseInputModule
{

    public UIManager m_UIManager;

    public Camera m_Camera;

    [SerializeField] private GameObject m_CurrentObject = null;
    [SerializeField] private GameObject m_LastObject = null;
    private PointerEventData m_Data = null;





    protected override void Awake()
    {
        base.Awake();

        m_Data = new PointerEventData(eventSystem);
    }
    public override void Process()
    {
        // Reset data, set camera
        m_Data.Reset();
        m_Data.position = new Vector2(m_Camera.pixelWidth / 2, m_Camera.pixelHeight / 2);
        
        // Raycast
        eventSystem.RaycastAll(m_Data, m_RaycastResultCache);
        m_Data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        m_CurrentObject = m_Data.pointerCurrentRaycast.gameObject;
        Debug.DrawRay(m_Camera.transform.position , m_Camera.transform.forward * 100, Color.red);

        // Clear
        m_RaycastResultCache.Clear();

        // Hover
        HandlePointerExitAndEnter(m_Data, m_CurrentObject);

        if(m_CurrentObject != null)
        {
            Debug.Log(m_CurrentObject);
            m_LastObject = m_CurrentObject;
            if(m_CurrentObject.CompareTag("Button"))
            {
                m_UIManager.LookAtButton(m_CurrentObject);
            }
        } 
        else
        {
            m_UIManager.ResetButton(m_LastObject);
        }



        // Press
        
        // Release


    }

    public PointerEventData GetData()
    {
        return m_Data;
    }
}
