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

    Color32 m_CalibratedMat;
    Color32 m_BlankMat;



    // Start is called before the first frame update
    void Start()
    {
        m_UI_PhysicalButtons = GameObject.Find("UI_PhysicalButtons");
        m_UIBehviour = m_UI_PhysicalButtons.GetComponent<UI_Behaviour>();

        m_CalibratedMat = new Color32(86, 173, 214, 255);
        m_BlankMat = new Color32(224, 224, 224, 255);


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
                    m_Materials[i].color = m_BlankMat;
                }
                break;
            case 3:
                m_Materials[1].color = m_CalibratedMat;
                m_Materials[2].color = m_CalibratedMat;
                break;
            case 4:
                m_Materials[7].color = m_BlankMat;
                m_Materials[8].color = m_BlankMat;
                m_Materials[10].color = m_BlankMat;
                m_Materials[11].color = m_BlankMat;
                break;
            case 5:
                m_Materials[7].color = m_CalibratedMat;
                m_Materials[8].color = m_CalibratedMat;
                m_Materials[10].color = m_CalibratedMat;
                m_Materials[11].color = m_CalibratedMat;

                m_Materials[3].color = m_BlankMat;
                m_Materials[5].color = m_BlankMat;
                break;
            case 6:
                m_Materials[3].color = m_CalibratedMat;
                m_Materials[5].color = m_CalibratedMat;

                m_Materials[4].color = m_BlankMat;
                m_Materials[6].color = m_BlankMat;
                break;
            case 7:
                m_Materials[4].color = m_CalibratedMat;
                m_Materials[6].color = m_CalibratedMat;

                m_Materials[9].color = m_BlankMat;
                m_Materials[12].color = m_BlankMat;
                break;
            case 8:
                m_Materials[9].color = m_CalibratedMat;
                m_Materials[12].color = m_CalibratedMat;
                break;
            default:
                break;
        }
    }
}
