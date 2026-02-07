using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BotnetAppBTN : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public Color hoverStateBTNColor;

	private Image buttonIMG;

	private Color defaultStateBTNColor;

	private bool iAmLocked;

	private bool iWasFired;

	private Action myCallBackAction;

	private void Start()
	{
		buttonIMG = GetComponent<Image>();
		defaultStateBTNColor = buttonIMG.color;
	}

	private void Update()
	{
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!iAmLocked && !iWasFired)
		{
			iWasFired = true;
			myCallBackAction();
			DOTween.To(() => buttonIMG.color, delegate(Color x)
			{
				buttonIMG.color = x;
			}, defaultStateBTNColor, 0.3f).SetEase(Ease.Linear);
			GameManager.TimeSlinger.FireTimer(0.5f, resetFire);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!iAmLocked)
		{
			DOTween.To(() => buttonIMG.color, delegate(Color x)
			{
				buttonIMG.color = x;
			}, hoverStateBTNColor, 0.15f).SetEase(Ease.Linear);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!iAmLocked)
		{
			DOTween.To(() => buttonIMG.color, delegate(Color x)
			{
				buttonIMG.color = x;
			}, defaultStateBTNColor, 0.15f).SetEase(Ease.Linear);
		}
	}

	public void setMyAction(Action setAction)
	{
		myCallBackAction = setAction;
	}

	private void resetFire()
	{
		iWasFired = false;
	}
}
