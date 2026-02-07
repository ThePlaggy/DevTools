using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NetworkDialog : MonoBehaviour
{
	private const float BG_FADE_TIME = 0.25f;

	public GameObject NetworkDialogObject;

	public GameObject ConnectBTN;

	public GameObject CancelBTN;

	public GameObject WifiNameType;

	public GameObject PasswordInput;

	private WifiNetworkDefinition curNetworkToJoin;

	private DialogBTNBehaviour dialogBTNCancel;

	private DialogBTNBehaviour dialogBTNConnect;

	private Sequence noSeq;

	private Vector3 scaleMaxSize = Vector3.one;

	private Vector3 scaleMinSize = new Vector3(0.1f, 0.1f, 0.1f);

	private void Awake()
	{
		UIDialogManager.NetworkDialog = this;
		dialogBTNConnect = ConnectBTN.GetComponent<DialogBTNBehaviour>();
		dialogBTNCancel = CancelBTN.GetComponent<DialogBTNBehaviour>();
		dialogBTNConnect.OnPress += connectToNetwork;
		dialogBTNCancel.OnPress += cancelConnect;
	}

	private void OnDestroy()
	{
		dialogBTNConnect.OnPress -= connectToNetwork;
		dialogBTNCancel.OnPress -= cancelConnect;
	}

	public void Present(WifiNetworkDefinition NetworkToJoin)
	{
		curNetworkToJoin = NetworkToJoin;
		LookUp.DesktopUI.DIALOG_HOLDER.SetActive(value: true);
		DOTween.To(() => LookUp.DesktopUI.DIALOG_BG_OBJECT.GetComponent<CanvasGroup>().alpha, delegate(float x)
		{
			LookUp.DesktopUI.DIALOG_BG_OBJECT.GetComponent<CanvasGroup>().alpha = x;
		}, 1f, 0.25f).SetEase(Ease.Linear).OnComplete(delegate
		{
			NetworkDialogObject.SetActive(value: true);
			PasswordInput.GetComponent<InputField>().text = string.Empty;
			WifiNameType.GetComponent<Text>().text = "The WiFi network " + NetworkToJoin.networkName + " requires a  " + MagicSlinger.GetNetworkSecurityType(NetworkToJoin.networkSecurity) + " password.";
			DOTween.To(() => NetworkDialogObject.GetComponent<RectTransform>().localScale, delegate(Vector3 x)
			{
				NetworkDialogObject.GetComponent<RectTransform>().localScale = x;
			}, scaleMaxSize, 0.35f).SetEase(Ease.OutCirc);
		});
	}

	private void dismiss()
	{
		DOTween.To(() => NetworkDialogObject.GetComponent<RectTransform>().localScale, delegate(Vector3 x)
		{
			NetworkDialogObject.GetComponent<RectTransform>().localScale = x;
		}, scaleMinSize, 0.35f).SetEase(Ease.InCirc).OnComplete(delegate
		{
			curNetworkToJoin = null;
			PasswordInput.GetComponent<InputField>().text = string.Empty;
			WifiNameType.GetComponent<Text>().text = string.Empty;
			NetworkDialogObject.SetActive(value: false);
			DOTween.To(() => LookUp.DesktopUI.DIALOG_BG_OBJECT.GetComponent<CanvasGroup>().alpha, delegate(float x)
			{
				LookUp.DesktopUI.DIALOG_BG_OBJECT.GetComponent<CanvasGroup>().alpha = x;
			}, 0f, 0.25f).OnComplete(delegate
			{
				LookUp.DesktopUI.DIALOG_HOLDER.SetActive(value: false);
			});
		});
	}

	private void connectToNetwork()
	{
		if (PasswordInput.GetComponent<InputField>().text.Equals(curNetworkToJoin.networkPassword))
		{
			GameManager.ManagerSlinger.WifiManager.ConnectToWifi(curNetworkToJoin, byPassSecuirty: true);
			dismiss();
			return;
		}
		PasswordInput.GetComponent<InputField>().text = string.Empty;
		noSeq = DOTween.Sequence();
		noSeq.Insert(0f, DOTween.To(() => NetworkDialogObject.GetComponent<RectTransform>().anchoredPosition, delegate(Vector2 x)
		{
			NetworkDialogObject.GetComponent<RectTransform>().anchoredPosition = x;
		}, new Vector2(-30f, 0f), 0.15f).SetEase(Ease.InSine).SetRelative(isRelative: true));
		noSeq.Insert(0.15f, DOTween.To(() => NetworkDialogObject.GetComponent<RectTransform>().anchoredPosition, delegate(Vector2 x)
		{
			NetworkDialogObject.GetComponent<RectTransform>().anchoredPosition = x;
		}, new Vector2(60f, 0f), 0.15f).SetEase(Ease.OutSine).SetRelative(isRelative: true));
		noSeq.Insert(0.3f, DOTween.To(() => NetworkDialogObject.GetComponent<RectTransform>().anchoredPosition, delegate(Vector2 x)
		{
			NetworkDialogObject.GetComponent<RectTransform>().anchoredPosition = x;
		}, new Vector2(-50f, 0f), 0.15f).SetEase(Ease.InSine).SetRelative(isRelative: true));
		noSeq.Insert(0.45f, DOTween.To(() => NetworkDialogObject.GetComponent<RectTransform>().anchoredPosition, delegate(Vector2 x)
		{
			NetworkDialogObject.GetComponent<RectTransform>().anchoredPosition = x;
		}, new Vector2(40f, 0f), 0.15f).SetEase(Ease.OutSine).SetRelative(isRelative: true));
		noSeq.Insert(0.6f, DOTween.To(() => NetworkDialogObject.GetComponent<RectTransform>().anchoredPosition, delegate(Vector2 x)
		{
			NetworkDialogObject.GetComponent<RectTransform>().anchoredPosition = x;
		}, new Vector2(-30f, 0f), 0.15f).SetEase(Ease.InSine).SetRelative(isRelative: true));
		noSeq.Insert(0.75f, DOTween.To(() => NetworkDialogObject.GetComponent<RectTransform>().anchoredPosition, delegate(Vector2 x)
		{
			NetworkDialogObject.GetComponent<RectTransform>().anchoredPosition = x;
		}, new Vector2(20f, 0f), 0.15f).SetEase(Ease.OutSine).SetRelative(isRelative: true));
		noSeq.Insert(0.9f, DOTween.To(() => NetworkDialogObject.GetComponent<RectTransform>().anchoredPosition, delegate(Vector2 x)
		{
			NetworkDialogObject.GetComponent<RectTransform>().anchoredPosition = x;
		}, new Vector2(-10f, 0f), 0.15f).SetEase(Ease.InSine).SetRelative(isRelative: true));
		noSeq.Play();
	}

	private void cancelConnect()
	{
		dismiss();
	}
}
