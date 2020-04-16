﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_Desktop_Behaviour : MonoBehaviour
{
    GameObject m_UI_PhysicalButtons;
    UI_Behaviour m_UIBehviour;

    public TextMeshProUGUI m_text;

    public Button m_StartCalibrationButton;
    public Button m_PreviousStepButton;
    public Button m_StartStepButton;


    // Start is called before the first frame update
    void Start()
    {
        m_UI_PhysicalButtons = GameObject.Find("UI_PhysicalButtons");
        m_UIBehviour = m_UI_PhysicalButtons.GetComponent<UI_Behaviour>();        
    }

    public void StartCalibration()
    {

    }

    public void NextStep()
    {
        m_UIBehviour.ButtonFunction("Next");
        m_text.text = "Current Step: " + m_UIBehviour.m_CurrentStepText.text;
    }

    public void PreviousStep()
    {
        m_UIBehviour.ButtonFunction("Previous");
        m_text.text = "Current Step: " + m_UIBehviour.m_CurrentStepText.text;
    }

    public void StartCalibrationStep()
    {
        m_UIBehviour.ButtonFunction("Start");
        m_text.text = "Current Step: " + m_UIBehviour.m_CurrentStepText.text;

    }

    private void Update()
    {
        if (!m_UIBehviour.m_ButtonsAreActive)
        {
            m_StartCalibrationButton.interactable = false;
            m_PreviousStepButton.interactable = false;
            m_StartStepButton.interactable = false;

        }
        else
        {
            m_StartCalibrationButton.interactable = true;
            m_PreviousStepButton.interactable = true;
            m_StartStepButton.interactable = true;
        }
    }




}