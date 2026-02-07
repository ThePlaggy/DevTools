using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HMBTNObject : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public Image buttonIMG;

	public Text buttonText;

	public Color defaultStateBTNColor;

	public Color defaultStateTextColor;

	public Color hoverStateBTNColor;

	public Color hoverStateTextColor;

	public Color lockStateBTNColor;

	public Color lockStateTextColor;

	public AudioClip hoverSFX;

	private string defaultTextValue;

	private bool holdAction;

	private string holdActionText;

	private bool iAmLocked;

	private bool iWasFired;

	private AudioSource myAS;

	private Action myCallBackAction;

	public void Start()
	{
		if (buttonText != null)
		{
			defaultTextValue = buttonText.text;
		}
		myAS = base.gameObject.AddComponent<AudioSource>();
		myAS.volume = 0.6f;
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
			DOTween.To(() => buttonIMG.color, delegate(Color x)
			{
				buttonIMG.color = x;
			}, defaultStateBTNColor, 0.3f).SetEase(Ease.Linear);
			if (buttonText != null)
			{
				DOTween.To(() => buttonText.color, delegate(Color x)
				{
					buttonText.color = x;
				}, defaultStateTextColor, 0.3f).SetEase(Ease.Linear);
			}
			GameManager.TimeSlinger.FireTimer(0.5f, resetFire);
		}
		else if (holdActionText != null && buttonText != null)
		{
			buttonText.text = holdActionText;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (iAmLocked)
		{
			return;
		}
		if (holdAction)
		{
			if (iWasFired)
			{
				return;
			}
			if (HackerModeManager.Ins != null && !HackerModeManager.Ins.sfxMuted)
			{
				myAS.PlayOneShot(hoverSFX);
			}
			DOTween.To(() => buttonIMG.color, delegate(Color x)
			{
				buttonIMG.color = x;
			}, hoverStateBTNColor, 0.3f).SetEase(Ease.Linear);
			if (buttonText != null)
			{
				DOTween.To(() => buttonText.color, delegate(Color x)
				{
					buttonText.color = x;
				}, hoverStateTextColor, 0.3f).SetEase(Ease.Linear);
			}
			return;
		}
		if (HackerModeManager.Ins != null && !HackerModeManager.Ins.sfxMuted)
		{
			myAS.PlayOneShot(hoverSFX);
		}
		DOTween.To(() => buttonIMG.color, delegate(Color x)
		{
			buttonIMG.color = x;
		}, hoverStateBTNColor, 0.3f).SetEase(Ease.Linear);
		if (buttonText != null)
		{
			DOTween.To(() => buttonText.color, delegate(Color x)
			{
				buttonText.color = x;
			}, hoverStateTextColor, 0.3f).SetEase(Ease.Linear);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (iAmLocked)
		{
			return;
		}
		if (holdAction)
		{
			if (iWasFired)
			{
				return;
			}
			DOTween.To(() => buttonIMG.color, delegate(Color x)
			{
				buttonIMG.color = x;
			}, defaultStateBTNColor, 0.3f).SetEase(Ease.Linear);
			if (buttonText != null)
			{
				DOTween.To(() => buttonText.color, delegate(Color x)
				{
					buttonText.color = x;
				}, defaultStateTextColor, 0.3f).SetEase(Ease.Linear);
			}
			return;
		}
		DOTween.To(() => buttonIMG.color, delegate(Color x)
		{
			buttonIMG.color = x;
		}, defaultStateBTNColor, 0.3f).SetEase(Ease.Linear);
		if (buttonText != null)
		{
			DOTween.To(() => buttonText.color, delegate(Color x)
			{
				buttonText.color = x;
			}, defaultStateTextColor, 0.3f).SetEase(Ease.Linear);
		}
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

	public void releaseHold()
	{
		if (!iWasFired)
		{
			return;
		}
		if (buttonText != null)
		{
			buttonText.text = defaultTextValue;
			DOTween.To(() => buttonText.color, delegate(Color x)
			{
				buttonText.color = x;
			}, defaultStateTextColor, 0.3f).SetEase(Ease.Linear);
		}
		DOTween.To(() => buttonIMG.color, delegate(Color x)
		{
			buttonIMG.color = x;
		}, defaultStateBTNColor, 0.3f).SetEase(Ease.Linear);
		GameManager.TimeSlinger.FireTimer(0.3f, resetFire);
	}

	public void lockMe()
	{
		if (!iWasFired)
		{
			iAmLocked = true;
			buttonIMG.color = lockStateBTNColor;
			if (buttonText != null)
			{
				buttonText.color = lockStateTextColor;
				buttonText.fontStyle = FontStyle.Italic;
			}
		}
	}

	public void unLockMe()
	{
		if (!iWasFired)
		{
			iAmLocked = false;
			buttonIMG.color = defaultStateBTNColor;
			if (buttonText != null)
			{
				buttonText.color = defaultStateTextColor;
				buttonText.fontStyle = FontStyle.Normal;
			}
		}
	}

	public void resetFire()
	{
		iWasFired = false;
	}
}
