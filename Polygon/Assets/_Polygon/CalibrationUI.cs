using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Manus.Polygon
{
	using System;

	public class CalibrationUI : MonoBehaviour
	{
		[Header("Main")]
		[SerializeField] private Text currentStep;
		[SerializeField] private CanvasGroup currentStepGroup;
		[SerializeField] private CanvasGroup mainTitleGroup;

		[SerializeField] private CanvasGroup subtitleGroup;

		[SerializeField] private CanvasGroup ui;

		[Header("Countdown")]
		[SerializeField] private CanvasGroup number3;
		[SerializeField] private CanvasGroup number2;
		[SerializeField] private CanvasGroup number1;

		[Header("Slider")]
		[SerializeField] private CanvasGroup timeCanvas;
		[SerializeField] private CanvasGroup timeTitleGroup;
		[SerializeField] private Slider timer;

		[Header("Settings")]
		[SerializeField] private float countdownTime = 3f;

		private void Awake()
		{
			ui.alpha = 1;

			mainTitleGroup.alpha = 0;
			subtitleGroup.alpha = 0;
			currentStepGroup.alpha = 0;
			currentStep.text = string.Empty;

			number1.alpha = 0;
			number2.alpha = 0;
			number3.alpha = 0;

			timeCanvas.alpha = 0;
			timeTitleGroup.alpha = 0;
			timer.value = 0;

			// Test code
			Calibrate("Wrists", 5f, () => { Debug.Log("Start"); }, () => { Debug.Log("Calibrating"); }, () => { Debug.Log("Done"); });
		}

		public void ToggleUI(bool tf)
		{
			ui.DOFade(tf ? 1 : 0, .3f).SetEase(Ease.InOutCubic);
		}

		public void Calibrate(string step, float time, Action startCallback = null, Action updateCallback = null, Action endCallback = null)
		{
			// TODO: put this mess inside of a coroutine or a sequence

			mainTitleGroup.DOFade(1, .5f).SetEase(Ease.InCubic);
			subtitleGroup.DOFade(1, .5f).SetEase(Ease.InCubic);
			currentStepGroup.DOFade(1, .5f).SetDelay(0.1f).SetEase(Ease.InCubic);
			currentStep.text = step;

			DOVirtual.DelayedCall(.5f, () =>
				{
					CountDownAnimation(number3, () => CountDownAnimation(number2, () => CountDownAnimation(number1,() =>
					{
						startCallback?.Invoke();

						timeCanvas.DOFade(1, 0.5f).SetEase(Ease.InCubic).OnComplete(() => { timeCanvas.DOFade(0, 0.5f).SetDelay(time - 0.5f).SetEase(Ease.OutCubic); });
						subtitleGroup.DOFade(0, .5f).SetEase(Ease.OutCubic);
						timeTitleGroup.DOFade(1, 0.5f).SetEase(Ease.OutCubic);
						//Animator.setTrigger("Trigger name")

						DOVirtual.Float(0, 1, time,
							value =>
							{
								updateCallback?.Invoke();
								timer.value = value;
							}).SetEase(Ease.Linear).OnComplete(
							() =>
							{
								endCallback?.Invoke();

								mainTitleGroup.DOFade(0, .5f).SetEase(Ease.OutCubic);
								currentStepGroup.DOFade(0, .5f).SetDelay(0.1f).SetEase(Ease.OutCubic);
							});
					})));
				});
		}

		private void CountDownAnimation(CanvasGroup number, Action callback = null)
		{
			number.alpha = 0;
			number.transform.localScale = Vector3.zero;

			float time = countdownTime / 3f;

			number.DOFade(1, time * 0.3f).SetEase(Ease.InCubic).OnComplete(
				() => { number.DOFade(0, time * 0.2f).SetEase(Ease.OutCubic).SetDelay(time * 0.5f); });
			number.transform.DOScale(1.5f, time).SetEase(Ease.InCubic).OnComplete(() => { callback?.Invoke(); });
		}
	}
}

