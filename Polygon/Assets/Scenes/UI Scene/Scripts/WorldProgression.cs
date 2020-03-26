using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WorldProgression : MonoBehaviour
{

    UI_Behaviour m_UIBehaviour;

    GameObject[] m_PoolsArray = new GameObject[8];
    [SerializeField]
    List<GameObject> m_PoolsList = new List<GameObject>();

    public int m_PoolInt;
    // Start is called before the first frame update
    void Start()
    {
        GameObject m_UI_PhysicalButtons = GameObject.Find("UI_PhysicalButtons");
        m_UIBehaviour = m_UI_PhysicalButtons.GetComponent<UI_Behaviour>();

        m_PoolInt = 0;


        
        foreach (Transform child in this.gameObject.transform)
        {
            m_PoolsList.Add(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (m_PoolInt < m_UIBehaviour.m_CurrentStep)
        {
            var m_PoolRenderer = m_PoolsList[m_PoolInt].GetComponent<Renderer>();
            m_PoolRenderer.material.SetColor("_Color", Color.red);
            m_PoolInt++;
        }
        else if (m_PoolInt > m_UIBehaviour.m_CurrentStep)
        {
            var m_PoolRenderer = m_PoolsList[m_PoolInt - 1].GetComponent<Renderer>();
            m_PoolRenderer.material.SetColor("_Color", Color.blue);
            m_PoolInt -= 1;
        }
    }
}
