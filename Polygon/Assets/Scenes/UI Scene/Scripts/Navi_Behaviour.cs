using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Navi_Behaviour : MonoBehaviour
{
    Vector3 m_IdlePos;
    Vector3 m_UIPos;
    Vector3 m_CharacterPos;

    GameObject m_UI;


    // Start is called before the first frame update
    void Start()
    {
        m_UI = GameObject.Find("UI_PhysicalButtons");


        m_CharacterPos = new Vector3(1.187f, 1.663f, 0.428f);


        

    }

    // Update is called once per frame
    void Update()
    {
        m_UIPos = new Vector3(m_UI.transform.position.x, m_UI.transform.position.y + .1f, m_UI.transform.position.z);
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            MoveToUI();
        }
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            MoveToCharacter();
        }
    }


    public void MoveToUI()
    {
        transform.DOMove(m_UIPos, 3f).SetEase(Ease.InOutBounce);
    }
    public void MoveToCharacter()
    {
        transform.DOMove(m_CharacterPos, 3f).SetEase(Ease.InOutBounce);
    }
}
