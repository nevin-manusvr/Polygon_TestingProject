using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Progress_Manager : MonoBehaviour
{
    GameObject m_UI_PhysicalButtons;
    UI_Behaviour m_UIBehviour;

    public SkinnedMeshRenderer m_SkinnedMeshRenderer;

    public List<Material> m_Materials;

    public int m_FinishedStep;



    // Start is called before the first frame update
    void Start()
    {
        m_UI_PhysicalButtons = GameObject.Find("UI_PhysicalButtons");
        m_UIBehviour = m_UI_PhysicalButtons.GetComponent<UI_Behaviour>();

        m_FinishedStep = m_UIBehviour.m_CurrentStep;

        m_SkinnedMeshRenderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        m_SkinnedMeshRenderer.GetMaterials(m_Materials);
    }

    // Update is called once per frame
    void Update()
    {
        CheckCurrentStep();
    }

    void CheckCurrentStep()
    {
        switch (m_UIBehviour.m_CurrentStep)
        {
            case 2:
                for (int i = 0; i < m_Materials.Count; i++)
                {
                    m_Materials[i].color = Color.white;
                }
                break;
            case 3:
                m_Materials[1].color = Color.red;
                m_Materials[2].color = Color.red;
                break;
            case 4:
                m_Materials[7].color = Color.white;
                m_Materials[8].color = Color.white;
                m_Materials[10].color = Color.white;
                m_Materials[11].color = Color.white;
                break;
            case 5:
                m_Materials[7].color = Color.red;
                m_Materials[8].color = Color.red;
                m_Materials[10].color = Color.red;
                m_Materials[11].color = Color.red;

                m_Materials[3].color = Color.white;
                m_Materials[5].color = Color.white;
                break;
            case 6:
                m_Materials[3].color = Color.red;
                m_Materials[5].color = Color.red;

                m_Materials[4].color = Color.white;
                m_Materials[6].color = Color.white;
                break;
            case 7:
                m_Materials[4].color = Color.red;
                m_Materials[6].color = Color.red;

                m_Materials[9].color = Color.white;
                m_Materials[12].color = Color.white;
                break;
            case 8:
                m_Materials[9].color = Color.red;
                m_Materials[12].color = Color.red;
                break;
            default:
                break;
        }
    }
}
