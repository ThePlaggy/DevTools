using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HMBTNOptions : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public Text buttonText;

	public Color onColor;

	public Color offColor;

	public AudioClip hoverSFX;

	[HideInInspector]
	public bool off;

	private string defaultTextValue;

	private bool holdAction;

	private string holdActionText;

	private bool iAmLocked;

	private bool iWasFired;

	private Action myCallBackAction;

	public void Start()
	{
		if (buttonText != null)
		{
			defaultTextValue = buttonText.text;
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (iAmLocked || iWasFired)
		{
			return;
		}
		iWasFired = true;
		myCallBackAction();
		if (!holdAction)
		{
			off = !off;
			Color endValue = (off ? offColor : onColor);
			string text = (off ? "OFF" : "ON");
			if (buttonText != null)
			{
				DOTween.To(() => buttonText.color, delegate(Color x)
				{
					buttonText.color = x;
				}, endValue, 0.3f).SetEase(Ease.Linear);
				buttonText.text = text;
			}
			Invoke("resetFire", 0.5f);
		}
		else if (holdActionText != null && buttonText != null)
		{
			buttonText.text = holdActionText;
		}
	}

	public void SetActive(bool active)
	{
		off = active;
		Color endValue = (off ? offColor : onColor);
		string text = (off ? "OFF" : "ON");
		if (buttonText != null)
		{
			DOTween.To(() => buttonText.color, delegate(Color x)
			{
				buttonText.color = x;
			}, endValue, 0.3f).SetEase(Ease.Linear);
			buttonText.text = text;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!iAmLocked && holdAction)
		{
			bool flag = iWasFired;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
	}

	public void setMyAction(Action setAction)
	{
		myCallBackAction = setAction;
	}

	public void setMyAction(Action setAction, bool hasHoldAction)
	{
		myCallBackAction = setAction;
		holdAction = hasHoldAction;
	}

	public void setMyAction(Action setAction, bool hasHoldAction, string setHoldActionText)
	{
		myCallBackAction = setAction;
		holdAction = hasHoldAction;
		holdActionText = setHoldActionText;
	}

	private void resetFire()
	{
		iWasFired = false;
	}
}
