using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIToggle : MonoBehaviour
{
	private CanvasGroup canvas;
	private bool isVisible;

    private void Start()
    {
	    canvas = GetComponent<CanvasGroup>();

		canvas.alpha = 0;
	    isVisible = false;
    }

    private void Update()
    {
	    if (Input.GetKeyDown(KeyCode.F1))
	    {
		    isVisible = !isVisible;
		    canvas.alpha = isVisible ? 1 : 0;
	    }
    }
}
