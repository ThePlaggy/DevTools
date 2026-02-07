using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class desktopBehavior : MonoBehaviour
{
	public GameObject WifiIcon;

	public List<Sprite> WifiSprites;

	public GameObject WifiMenu;

	private bool wifiMenuActive;

	private bool wifiMenuAniActive;

	private void Awake()
	{
	}

	private void Start()
	{
		prepDesktopBehavior();
	}

	private void Update()
	{
	}

	public void ChangeWifiBars(int wifiBarAmount)
	{
		WifiIcon.GetComponent<Image>().sprite = WifiSprites[wifiBarAmount];
	}

	public void TriggerWifiMenu()
	{
		if (wifiMenuAniActive)
		{
			return;
		}
		wifiMenuAniActive = true;
		GameManager.TimeSlinger.FireTimer(0.25f, resetWifiMenuAniActive);
		if (wifiMenuActive)
		{
			wifiMenuActive = false;
			float y = Mathf.Floor(WifiMenu.GetComponent<RectTransform>().sizeDelta.y / 2f + 41f);
			DOTween.To(() => WifiMenu.GetComponent<RectTransform>().localPosition, delegate(Vector3 x)
			{
				WifiMenu.GetComponent<RectTransform>().localPosition = x;
			}, new Vector3(WifiMenu.GetComponent<RectTransform>().localPosition.x, y, 0f), 0.25f).SetEase(Ease.InQuad);
		}
		else
		{
			wifiMenuActive = true;
			float num = Mathf.Floor(WifiMenu.GetComponent<RectTransform>().sizeDelta.y / 2f + 41f);
			DOTween.To(() => WifiMenu.GetComponent<RectTransform>().localPosition, delegate(Vector3 x)
			{
				WifiMenu.GetComponent<RectTransform>().localPosition = x;
			}, new Vector3(WifiMenu.GetComponent<RectTransform>().localPosition.x, 0f - num, 0f), 0.25f).SetEase(Ease.OutQuad);
		}
	}

	private void prepDesktopBehavior()
	{
		wifiMenuActive = false;
	}

	private void updateClocks()
	{
	}

	private void resetWifiMenuAniActive()
	{
		wifiMenuAniActive = false;
	}
}
