using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WifiMenuNetworkOptionObject : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public GameObject NetworkName1;

	public GameObject NetworkName2;

	public GameObject NetworkConnected;

	public GameObject NetworkSecurity;

	public GameObject NetworkStrength;

	public List<Sprite> NetworkBarSprites;

	public Color hoverColor;

	private Color defaultColor;

	private Vector2 myPOS = new Vector2(10f, 24f);

	private WifiNetworkDefinition myWifiNetwork;

	[HideInInspector]
	private Image networkLock;

	[HideInInspector]
	private RectTransform networkLockRT;

	[HideInInspector]
	private MouseClickSoundScrub MCSS;

	private void Awake()
	{
		networkLock = NetworkSecurity.GetComponent<Image>();
		networkLock.sprite = CustomSpriteLookUp.megalock;
		networkLockRT = networkLock.GetComponent<RectTransform>();
		MCSS = GetComponent<MouseClickSoundScrub>();
	}

	private void Start()
	{
		defaultColor = GetComponent<Image>().color;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (myWifiNetwork.interaction != WiFiInteractionType.LOCKED)
		{
			GameManager.ManagerSlinger.WifiManager.TriggerWifiMenu();
			GameManager.ManagerSlinger.WifiManager.ConnectToWifi(myWifiNetwork);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		GetComponent<Image>().color = hoverColor;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		GetComponent<Image>().color = defaultColor;
	}

	public void Clear()
	{
		NetworkName1.GetComponent<Text>().text = string.Empty;
		NetworkName2.GetComponent<Text>().text = string.Empty;
		NetworkConnected.SetActive(value: false);
		NetworkSecurity.SetActive(value: false);
		GetComponent<RectTransform>().anchoredPosition = myPOS;
	}

	public void SoftBuild()
	{
		GetComponent<RectTransform>().anchoredPosition = myPOS;
	}

	public void Build(bool Connected, WifiNetworkDefinition MyNetwork, Vector2 SetPOS)
	{
		if (MyNetwork.networkName == "Hidden Network" && GameManager.ManagerSlinger.WifiManager.getCurrentWiFi() == MyNetwork)
		{
			NetworkName1.GetComponent<Text>().text = "Executing Reality";
		}
		else
		{
			NetworkName1.GetComponent<Text>().text = MyNetwork.networkName;
		}
		if (Connected)
		{
			NetworkConnected.SetActive(value: true);
		}
		NetworkSecurity.SetActive(value: true);
		switch (MyNetwork.interaction)
		{
		case WiFiInteractionType.NONE:
		{
			networkLock.color = Color.black;
			networkLock.sprite = CustomSpriteLookUp.megalock;
			networkLockRT.sizeDelta = new Vector2(16f, 16f);
			Vector3 localPosition3 = networkLockRT.localPosition;
			localPosition3.x = 180f;
			networkLockRT.localPosition = localPosition3;
			MCSS.enabled = true;
			break;
		}
		case WiFiInteractionType.LOCKED:
		{
			networkLock.color = Color.red;
			networkLock.sprite = CustomSpriteLookUp.megalock;
			networkLockRT.sizeDelta = new Vector2(16f, 16f);
			Vector3 localPosition2 = networkLockRT.localPosition;
			localPosition2.x = 180f;
			networkLockRT.localPosition = localPosition2;
			MCSS.enabled = false;
			break;
		}
		case WiFiInteractionType.UNLOCKED:
		{
			networkLock.color = new Color(0f, 0.75f, 0f, 1f);
			networkLock.sprite = CustomSpriteLookUp.megaunlock;
			networkLockRT.sizeDelta = new Vector2(23f, 16f);
			Vector3 localPosition = networkLockRT.localPosition;
			localPosition.x = 175f;
			networkLockRT.localPosition = localPosition;
			MCSS.enabled = true;
			break;
		}
		}
		if (MyNetwork.networkSecurity == WIFI_SECURITY.NONE && MyNetwork.interaction > WiFiInteractionType.LOCKED)
		{
			NetworkSecurity.SetActive(value: false);
		}
		int index = Mathf.Min(MyNetwork.networkStrength + InventoryManager.WifiDongleLevel, 3);
		NetworkStrength.GetComponent<Image>().sprite = NetworkBarSprites[index];
		myWifiNetwork = MyNetwork;
		GetComponent<RectTransform>().anchoredPosition = SetPOS;
	}
}
