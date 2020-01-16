using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace ManusVR.Polygon
{
	using System;

	public class CalibrationUI : MonoBehaviour
	{
		[Header("Main")]
		[SerializeField] private Text currentStep;
		[SerializeField] private CanvasGroup currentStepGroup;
		[SerializeField] private CanvasGroup titleGroup;

		[SerializeField] private CanvasGroup ui;

		[Header("Countdown")]
		[SerializeField] private CanvasGroup number3;
		[SerializeField] private CanvasGroup number2;
		[SerializeField] private CanvasGroup number1;

		[Header("Slider")]
		[SerializeField] private CanvasGroup timeCanvas;
		[SerializeField] private Slider timer;

		private void Start()
		{
			ui.alpha = 1;

			titleGroup.alpha = 0;
			currentStepGroup.alpha = 0;
			currentStep.text = string.Empty;

			number1.alpha = 0;
			number2.alpha = 0;
			number3.alpha = 0;

			timeCanvas.alpha = 0;
			timer.value = 0;

			Calibrate("Mom's Spaghetti!", 5f, () => { Debug.Log("Start"); }, () => { Debug.Log("Done"); });
		}

		public void ToggleUI(bool tf)
		{
			ui.DOFade(tf ? 1 : 0, .3f).SetEase(Ease.InOutCubic);
		}

		public void SetCurrentStep(string step)
		{
			currentStep.text = step;
		}

		public void Calibrate(string step, float time, Action startCallback = null, Action endCallback = null)
		{
			titleGroup.DOFade(1, .5f).SetEase(Ease.InCubic);
			currentStepGroup.DOFade(1, .5f).SetDelay(0.1f).SetEase(Ease.InCubic);
			currentStep.text = step;

			DOVirtual.DelayedCall(.5f, () =>
				{
					CountDownAnimation(number3, () => CountDownAnimation(number2, () => CountDownAnimation(number1,() =>
					{
						startCallback?.Invoke();

						timeCanvas.DOFade(1, 0.5f).SetEase(Ease.InCubic).OnComplete(() => { timeCanvas.DOFade(0, 0.5f).SetDelay(time - 0.5f).SetEase(Ease.OutCubic); });

						DOVirtual.Float(0, 1, time, value => { timer.value = value; }).SetEase(Ease.Linear).OnComplete(() =>
						{
							endCallback?.Invoke();

							titleGroup.DOFade(0, .5f).SetEase(Ease.OutCubic);
							currentStepGroup.DOFade(0, .5f).SetDelay(0.1f).SetEase(Ease.OutCubic);
						});
					})));
				});
		}

		private void CountDownAnimation(CanvasGroup number, Action callback = null)
		{
			number.alpha = 0;
			number.transform.localScale = Vector3.zero;

			number.DOFade(1, 0.3f).SetEase(Ease.InCubic).OnComplete(
				() => { number.DOFade(0, 0.2f).SetEase(Ease.OutCubic).SetDelay(.6f); });
			number.transform.DOScale(1.5f, 1f).SetEase(Ease.InCubic).OnComplete(() => { callback?.Invoke(); });
		}
	}
}

