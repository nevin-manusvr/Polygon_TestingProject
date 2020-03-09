using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class debugslider : MonoBehaviour
{

    Image m_Image;
    // Start is called before the first frame update
    void Start()
    {
        m_Image = GetComponent<Image>();
        DoFillAmount();
    }

    void DoFillAmount()
    {
        m_Image.DOFillAmount(1, 4f).SetEase(Ease.InOutCubic).OnComplete(() => {  m_Image.DOFillAmount(0, 4f).SetEase(Ease.InOutCubic).OnComplete(() => {  DoFillAmount();});});
    }
}
