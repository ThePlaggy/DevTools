using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VWipeApp : MonoBehaviour
{
	private const float largeDelta = 333f;

	public static VWipeApp Ins;

	public static bool ownApp;

	public static bool Premium;

	[HideInInspector]
	public CanvasGroup InfoText;

	[HideInInspector]
	public CanvasGroup PBarBG;

	[HideInInspector]
	public CanvasGroup PBarFill;

	[HideInInspector]
	public CanvasGroup VirusFound;

	[HideInInspector]
	public Text Title1;

	[HideInInspector]
	public Text Title2;

	private float deltaHeight;

	private TextMeshProUGUI introLabel;

	private VWipeButton ProBTN;

	private VWipeButton ScanBTN;

	private VWipeButton WipeBTN;

	private void Awake()
	{
		Ins = this;
	}

	private void Start()
	{
		LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.transform.SetParent(LookUp.DesktopUI.WINDOW_HOLDER);
		LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.GetComponent<RectTransform>().localScale = Vector3.one;
		LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.GetComponent<CanvasGroup>().alpha = 1f;
		LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.AddComponent<GraphicsRayCasterCatcher>();
		LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.AddComponent<BringWindowToFrontBehaviour>().parentTrans = LookUp.DesktopUI.WINDOW_HOLDER;
		InfoText = LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.transform.Find("InfoText").gameObject.AddComponent<CanvasGroup>();
		PBarBG = LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.transform.Find("PBarBG").gameObject.AddComponent<CanvasGroup>();
		PBarFill = LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.transform.Find("PBarFill").gameObject.AddComponent<CanvasGroup>();
		VirusFound = LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.transform.Find("VirusFound").gameObject.AddComponent<CanvasGroup>();
		Title1 = LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.transform.Find("Title").gameObject.GetComponent<Text>();
		Title2 = LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.transform.Find("Title2").gameObject.GetComponent<Text>();
		deltaHeight = LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.GetComponent<RectTransform>().sizeDelta.y;
		ChangeTitleText("VWipe Lite");
		ScanBTN = UnityEngine.Object.Instantiate(CustomObjectLookUp.VWipeButton, LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.transform).GetComponent<VWipeButton>();
		ScanBTN.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		ScanBTN.text.text = "Scan for viruses";
		ScanBTN.ClickAction.Event += ScanButton;
		WipeBTN = UnityEngine.Object.Instantiate(CustomObjectLookUp.VWipeButton, LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.transform).GetComponent<VWipeButton>();
		WipeBTN.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -50f);
		WipeBTN.text.text = "Wipe viruses";
		WipeBTN.priceText.text = "10";
		WipeBTN.ClickAction.Event += WipeButton;
		ProBTN = UnityEngine.Object.Instantiate(CustomObjectLookUp.VWipeButton, LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.transform).GetComponent<VWipeButton>();
		ProBTN.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -100f);
		ProBTN.text.text = "Buy Premium";
		ProBTN.priceText.text = "80";
		ProBTN.ClickAction.Event += ProButton;
		introLabel = UnityEngine.Object.Instantiate(ProBTN.text, LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.transform).GetComponent<TextMeshProUGUI>();
		introLabel.fontStyle = FontStyles.Bold;
		introLabel.fontSize = 24f;
		introLabel.text = "Welcome to VWipe, the best antivirus software" + Environment.NewLine + "on the deep web.";
		introLabel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 85f);
	}

	private void OnDestroy()
	{
		Ins = null;
		ScanBTN.ClickAction.Event -= ScanButton;
		WipeBTN.ClickAction.Event -= WipeButton;
		ProBTN.ClickAction.Event -= ProButton;
	}

	public void AddVWipeIcon()
	{
		if (!ownApp)
		{
			AppCreator.VWipeIconObject.SetActive(value: true);
			ownApp = true;
		}
	}

	public void ResizeToSmall()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.GetComponent<RectTransform>().sizeDelta.y, delegate(float x)
		{
			LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.GetComponent<RectTransform>().sizeDelta = new Vector2(414f, x);
		}, deltaHeight, 0.5f).SetEase(Ease.Linear));
		sequence.Play();
	}

	public void ResizeToLarge()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.GetComponent<RectTransform>().sizeDelta.y, delegate(float x)
		{
			LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.GetComponent<RectTransform>().sizeDelta = new Vector2(414f, x);
		}, 333f, 0.5f).SetEase(Ease.Linear));
		sequence.Play();
	}

	public void AlphaVirusCleaningContent(float alpha)
	{
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => InfoText.alpha, delegate(float x)
		{
			InfoText.alpha = x;
			PBarBG.alpha = x;
			PBarFill.alpha = x;
			VirusFound.alpha = x;
		}, alpha, 1f).SetEase(Ease.Linear));
		sequence.Play();
	}

	public void VirusCleaningContent(float alpha)
	{
		InfoText.alpha = alpha;
		PBarBG.alpha = alpha;
		PBarFill.alpha = alpha;
		VirusFound.alpha = alpha;
	}

	public void ChangeTitleText(string text)
	{
		Title1.text = text;
		Title2.text = text;
	}

	public void SetToLarge()
	{
		LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.GetComponent<RectTransform>().sizeDelta = new Vector2(414f, 333f);
	}

	public void GoPremium()
	{
		Premium = true;
		ChangeTitleText("VWipe Pro");
		WipeBTN.priceText.text = "FREE";
		ProBTN.text.text = "PURCHASED";
		ProBTN.text.color = Color.green;
		Title1.color = new Color(1f, 0.1f, 0f);
		UnityEngine.Object.Destroy(ProBTN.priceText);
		UnityEngine.Object.Destroy(ProBTN.priceText.transform.parent.gameObject);
	}

	public void ScanButton()
	{
		if (!TutorialAnnHook.YAAIRunning)
		{
			UIDialogManager.VWipeDialog.PerformScan(GameManager.HackerManager.virusManager.getVirusCount);
		}
	}

	public void WipeButton()
	{
		if (!TutorialAnnHook.YAAIRunning)
		{
			if (Premium)
			{
				UIDialogManager.VWipeDialog.PerformScanRemove(GameManager.HackerManager.virusManager.getVirusCount);
			}
			else if (DOSCoinsCurrencyManager.CurrentCurrency >= 10f)
			{
				DOSCoinsCurrencyManager.RemoveCurrency(10f);
				TannerDOSPopper.PopDOS(10);
				UIDialogManager.VWipeDialog.PerformScanRemove(GameManager.HackerManager.virusManager.getVirusCount);
			}
			else
			{
				GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.CantBuyItem);
			}
		}
	}

	public void ProButton()
	{
		if (!TutorialAnnHook.YAAIRunning)
		{
			if (DOSCoinsCurrencyManager.CurrentCurrency >= 80f)
			{
				DOSCoinsCurrencyManager.RemoveCurrency(80f);
				TannerDOSPopper.PopDOS(80);
				GoPremium();
				ProBTN.SetHardLock(value: true);
				GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: false);
			}
			else
			{
				GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.CantBuyItem);
			}
		}
	}

	public void AlphaButtons(float alpha)
	{
		if (alpha == 0f)
		{
			introLabel.alpha = alpha;
		}
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => ScanBTN.CG.alpha, delegate(float x)
		{
			ScanBTN.CG.alpha = x;
			WipeBTN.CG.alpha = x;
			ProBTN.CG.alpha = x;
			if (alpha == 1f)
			{
				introLabel.alpha = x;
			}
		}, alpha, 0.25f).SetEase(Ease.Linear));
		sequence.Play();
	}

	public void SetButtonLock(bool value)
	{
		ScanBTN.SetHardLock(value);
		WipeBTN.SetHardLock(value);
		if (!Premium)
		{
			ProBTN.SetHardLock(value);
		}
		GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: false);
	}
}
