using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WifiManager : MonoBehaviour
{
	public delegate void NewNetworksActions(List<WifiNetworkDefinition> NewNetworks);

	public delegate void OnlineOfflineActions();

	public delegate void OnlineWithNetworkActions(WifiNetworkDefinition TheNetwork);

	public WifiNetworkDefinition defaultWifiNetwork;

	public PasswordListDefinition PList;

	[SerializeField]
	private List<WifiHotspotObject> wifiHotSpots = new List<WifiHotspotObject>(6);

	[SerializeField]
	private WifiDongleBehaviour theWifiDongle;

	private WifiHotspotObject activeWifiHotSpot;

	private WifiNetworkDefinition currentWifiNetwork;

	private DOSDrainer dOSDrainer;

	[NonSerialized]
	public RandomDOSDrainer randDOSDrainer;

	private float godSpeed;

	private bool inWifiPlacementMode;

	private WifiManagerData myData;

	private int myID;

	private GameObject wifiIcon;

	private GameObject wifiMenu;

	private bool wifiMenuActive;

	private bool wifiMenuAniActive;

	private Vector2 wifiMenuPOS = Vector2.zero;

	private List<Sprite> wifiSprites = new List<Sprite>();

	private AudioSource wifiRage;

	public bool IsOnline { get; private set; }

	public Dictionary<string, string> PasswordList { get; } = new Dictionary<string, string>();

	public event OnlineOfflineActions WentOnline;

	public event OnlineOfflineActions WentOffline;

	public event OnlineWithNetworkActions OnlineWithNetwork;

	public event NewNetworksActions NewNetworksAvailable;

	public WifiHotspotObject GetCurrentHotspot()
	{
		return activeWifiHotSpot;
	}

	private void Awake()
	{
		for (int i = 0; i < GetAllWifiNetworks().Count; i++)
		{
			GetAllWifiNetworks()[i].networkIsOffline = false;
		}
		if (!DifficultyManager.HackerMode)
		{
			AddNewWiFi();
		}
		myID = base.transform.position.GetHashCode();
		activeWifiHotSpot = wifiHotSpots[0];
		GameManager.ManagerSlinger.WifiManager = this;
		string[] array = PList.PasswordList.Split(new string[1] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
		for (int j = 0; j < array.Length; j++)
		{
			PasswordList.Add(MagicSlinger.MD5It(array[j]), array[j]);
		}
		wifiIcon = LookUp.DesktopUI.WIFI_ICON;
		wifiSprites = LookUp.DesktopUI.WIFI_SPRITES;
		wifiMenu = LookUp.DesktopUI.WIFI_MENU;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProductWasActivated.Event += productWasPickedUp;
		GameManager.StageManager.Stage += stageMe;
		godSpeed = UnityEngine.Random.Range(0f, 100f);
		wifiRage = UnityEngine.Object.Instantiate(CustomObjectLookUp.WiFiRage).GetComponent<AudioSource>();
		if (!DifficultyManager.HackerMode)
		{
			MoveWiFi();
		}
	}

	private void MoveWiFi()
	{
		List<WifiNetworkDefinition> myWifiNetworks = wifiHotSpots[0].myWifiNetworks;
		List<WifiNetworkDefinition> myWifiNetworks2 = wifiHotSpots[1].myWifiNetworks;
		List<WifiNetworkDefinition> myWifiNetworks3 = wifiHotSpots[2].myWifiNetworks;
		List<WifiNetworkDefinition> myWifiNetworks4 = wifiHotSpots[3].myWifiNetworks;
		for (int i = 0; i < myWifiNetworks4.Count; i++)
		{
			switch (myWifiNetworks4[i].networkName)
			{
			case "FreeWifiNoVirus":
				myWifiNetworks3.Insert(1, myWifiNetworks4[i]);
				break;
			case "WIFIAF1A5D":
				myWifiNetworks3.Insert(4, myWifiNetworks4[i]);
				break;
			case "ali":
				myWifiNetworks3.Insert(9, myWifiNetworks4[i]);
				break;
			case "SENDemo":
				myWifiNetworks3.Insert(10, myWifiNetworks4[i]);
				break;
			case "Big Dave Network":
				myWifiNetworks3.Insert(15, myWifiNetworks4[i]);
				break;
			case "mycci7471":
				myWifiNetworks2.Insert(6, myWifiNetworks4[i]);
				break;
			case "tedata":
				myWifiNetworks2.Insert(7, myWifiNetworks4[i]);
				break;
			case "DonaldsWiFi":
				myWifiNetworks2.Insert(12, myWifiNetworks4[i]);
				break;
			case "JonesLaw":
				myWifiNetworks2.Insert(14, myWifiNetworks4[i]);
				break;
			case "WINSLOWS":
				myWifiNetworks2.Insert(15, myWifiNetworks4[i]);
				break;
			}
		}
		for (int j = 0; j < myWifiNetworks2.Count; j++)
		{
			if (myWifiNetworks2[j].networkName == "FreeWiFi7899")
			{
				WifiNetworkDefinition item = myWifiNetworks2[j];
				myWifiNetworks2.Remove(item);
				myWifiNetworks.Insert(1, item);
			}
		}
		myWifiNetworks4.Clear();
		WifiNetworkDefinition wifiNetworkDefinition = ScriptableObject.CreateInstance<WifiNetworkDefinition>();
		wifiNetworkDefinition.id = 199;
		wifiNetworkDefinition.networkChannel = 999;
		wifiNetworkDefinition.networkCoolOffTime = 9999f;
		wifiNetworkDefinition.networkInjectionCoolOffTime = 9f;
		wifiNetworkDefinition.networkInjectionRandEnd = 888;
		wifiNetworkDefinition.networkInjectionRandStart = 999;
		wifiNetworkDefinition.networkMaxInjectionAmount = 61;
		wifiNetworkDefinition.networkName = "Test Network";
		wifiNetworkDefinition.networkPower = 1;
		wifiNetworkDefinition.networkStrength = 3;
		wifiNetworkDefinition.networkSecurity = WIFI_SECURITY.WPA2;
		wifiNetworkDefinition.networkSignal = WIFI_SIGNAL_TYPE.W80211AC;
		wifiNetworkDefinition.networkTrackProbability = 0f;
		wifiNetworkDefinition.networkTrackRate = 99999f;
		wifiHotSpots[3].myWifiNetworks.Add(wifiNetworkDefinition);
	}

	private void Start()
	{
		dOSDrainer = new DOSDrainer();
		randDOSDrainer = base.gameObject.AddComponent<RandomDOSDrainer>();
	}

	private void Update()
	{
		if (currentWifiNetwork != null && currentWifiNetwork.affectedByDosDrainer)
		{
			dOSDrainer.tryConsume();
		}
	}

	private void FixedUpdate()
	{
		if (IsOnline && currentWifiNetwork != null)
		{
			currentWifiNetwork.howLongConnected += 1f;
		}
	}

	private void OnDestroy()
	{
	}

	public void EnterWifiDonglePlacementMode()
	{
		UIInventoryManager.ShowWifiDongle();
		inWifiPlacementMode = true;
		DisconnectFromWifi();
		ShowWifiHotSpots();
		StateManager.PlayerState = PLAYER_STATE.WIFI_DONGLE_PLACEMENT;
	}

	public void ExitWifiDonglePlacementMode(WifiHotspotObject newWifiHotSpot)
	{
		UIInventoryManager.HideWifiDongle();
		inWifiPlacementMode = false;
		HideWifiHotSpots();
		activeWifiHotSpot = newWifiHotSpot;
		theWifiDongle.PlaceDongle(activeWifiHotSpot.DonglePlacedPOS, activeWifiHotSpot.DonglePlacedROT);
		StateManager.PlayerState = PLAYER_STATE.ROAMING;
		for (int i = 0; i < wifiHotSpots.Count; i++)
		{
			if (wifiHotSpots[i] == newWifiHotSpot)
			{
				myData.ActiveWifiHotSpotIndex = i;
				i = wifiHotSpots.Count;
			}
		}
		DataManager.Save(myData);
		this.NewNetworksAvailable?.Invoke(GetCurrentWifiNetworks());
		if (BotnetBehaviour.Ins != null)
		{
			BotnetBehaviour.Ins.LookForDevices();
		}
	}

	public void ShowWifiHotSpots()
	{
		for (int i = 0; i < wifiHotSpots.Count; i++)
		{
			wifiHotSpots[i].ActivateMe();
		}
	}

	public void HideWifiHotSpots()
	{
		for (int i = 0; i < wifiHotSpots.Count; i++)
		{
			wifiHotSpots[i].DeactivateMe();
		}
	}

	public void ConnectToWifi(WifiNetworkDefinition wifiNetwork, bool byPassSecuirty = false)
	{
		if (IsOnline)
		{
			DisconnectFromWifi();
		}
		int wifiBarAmount = Mathf.Min(wifiNetwork.networkStrength + InventoryManager.WifiDongleLevel, 3);
		if (!wifiNetwork.networkIsOffline)
		{
			if (byPassSecuirty)
			{
				WiFiPoll.lastConnectedWifi = wifiNetwork;
				IsOnline = true;
				currentWifiNetwork = wifiNetwork;
				changeWifiBars(wifiBarAmount);
				myData.CurrentWifiNetworkIndex = activeWifiHotSpot.GetWifiNetworkIndex(wifiNetwork);
				myData.IsConnected = true;
				DataManager.Save(myData);
				this.WentOnline?.Invoke();
				this.OnlineWithNetwork?.Invoke(wifiNetwork);
			}
			else if (wifiNetwork.networkSecurity != WIFI_SECURITY.NONE && wifiNetwork.interaction != WiFiInteractionType.UNLOCKED)
			{
				UIDialogManager.NetworkDialog.Present(wifiNetwork);
			}
			else
			{
				WiFiPoll.lastConnectedWifi = wifiNetwork;
				IsOnline = true;
				currentWifiNetwork = wifiNetwork;
				changeWifiBars(wifiBarAmount);
				myData.CurrentWifiNetworkIndex = activeWifiHotSpot.GetWifiNetworkIndex(wifiNetwork);
				myData.IsConnected = true;
				DataManager.Save(myData);
				this.WentOnline?.Invoke();
				this.OnlineWithNetwork?.Invoke(wifiNetwork);
			}
		}
	}

	public void DisconnectFromWifi()
	{
		if (IsOnline)
		{
			IsOnline = false;
			currentWifiNetwork = null;
			changeWifiBars(0);
			myData.IsConnected = false;
			DataManager.Save(myData);
			this.WentOffline?.Invoke();
		}
	}

	public void TakeNetworkOffLine(WifiNetworkDefinition wifiNetwork)
	{
		if (currentWifiNetwork == wifiNetwork)
		{
			DisconnectFromWifi();
		}
		wifiNetwork.networkIsOffline = true;
		this.NewNetworksAvailable?.Invoke(GetCurrentWifiNetworks());
		GameManager.TimeSlinger.FireTimer(wifiNetwork.networkCoolOffTime, PutNetworkBackOnline, wifiNetwork);
		if (wifiNetwork.networkName == "GameNight At 602")
		{
			wifiRage.Play();
		}
	}

	public void TakeNetworkOffLineForever(string wifiNetworkName)
	{
		WifiNetworkDefinition wifiNetworkDefinition = GetAllWifiNetworks().FirstOrDefault((WifiNetworkDefinition network) => network.networkName == wifiNetworkName);
		if (wifiNetworkDefinition == null)
		{
			Debug.LogFormat("Error taking {0} WiFi down forever.", wifiNetworkName);
			return;
		}
		if (currentWifiNetwork == wifiNetworkDefinition)
		{
			DisconnectFromWifi();
		}
		wifiNetworkDefinition.networkIsOffline = true;
		this.NewNetworksAvailable?.Invoke(GetCurrentWifiNetworks());
		Debug.LogFormat("{0} WiFi taken down forever.", wifiNetworkDefinition.networkName);
	}

	public void PutNetworkBackOnline(WifiNetworkDefinition wifiNetwork)
	{
		wifiNetwork.networkIsOffline = false;
		this.NewNetworksAvailable?.Invoke(GetCurrentWifiNetworks());
	}

	public List<WifiNetworkDefinition> GetAllWifiNetworks()
	{
		List<WifiNetworkDefinition> list = new List<WifiNetworkDefinition>();
		for (int i = 0; i < wifiHotSpots.Count; i++)
		{
			for (int j = 0; j < wifiHotSpots[i].myWifiNetworks.Count; j++)
			{
				list.Add(wifiHotSpots[i].myWifiNetworks[j]);
			}
		}
		return list;
	}

	public List<WifiNetworkDefinition> GetCurrentWifiNetworks()
	{
		List<WifiNetworkDefinition> list = new List<WifiNetworkDefinition>();
		int wifiDongleLevel = InventoryManager.WifiDongleLevel;
		for (int i = 0; i < activeWifiHotSpot.myWifiNetworks.Count; i++)
		{
			if (RouterBehaviour.Ins.Owned && RouterBehaviour.Ins.RouterIsActive && !RouterBehaviour.Ins.IsJammed)
			{
				if (!activeWifiHotSpot.myWifiNetworks[i].networkIsOffline && activeWifiHotSpot.myWifiNetworks[i].networkStrength + wifiDongleLevel > 0)
				{
					list.Add(activeWifiHotSpot.myWifiNetworks[i]);
				}
			}
			else if (!activeWifiHotSpot.myWifiNetworks[i].networkIsOffline && activeWifiHotSpot.myWifiNetworks[i].networkStrength + wifiDongleLevel > 0 && !activeWifiHotSpot.myWifiNetworks[i].routerOnlyWiFi)
			{
				list.Add(activeWifiHotSpot.myWifiNetworks[i]);
			}
		}
		return list;
	}

	public List<WifiNetworkDefinition> GetWiFiAlt(bool All)
	{
		List<WifiNetworkDefinition> list = (All ? GetAllWifiNetworks() : GetCurrentWifiNetworks());
		List<WifiNetworkDefinition> list2 = new List<WifiNetworkDefinition>();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].interaction == WiFiInteractionType.LOCKED)
			{
				list2.Add(list[i]);
			}
			else if (list[i].networkSecurity != WIFI_SECURITY.NONE && list[i].interaction != WiFiInteractionType.UNLOCKED)
			{
				list2.Add(list[i]);
			}
		}
		return list2;
	}

	public List<WifiNetworkDefinition> GetSecureNetworks(WIFI_SECURITY SecuirtyType)
	{
		List<WifiNetworkDefinition> list = new List<WifiNetworkDefinition>();
		List<WifiNetworkDefinition> myWifiNetworks = activeWifiHotSpot.myWifiNetworks;
		int wifiDongleLevel = InventoryManager.WifiDongleLevel;
		for (int i = 0; i < myWifiNetworks.Count; i++)
		{
			if (RouterBehaviour.Ins.Owned && RouterBehaviour.Ins.RouterIsActive && !RouterBehaviour.Ins.IsJammed)
			{
				if (!myWifiNetworks[i].networkIsOffline && myWifiNetworks[i].networkSecurity == SecuirtyType && myWifiNetworks[i].networkStrength + wifiDongleLevel > 0)
				{
					list.Add(myWifiNetworks[i]);
				}
			}
			else if (!myWifiNetworks[i].networkIsOffline && myWifiNetworks[i].networkSecurity == SecuirtyType && myWifiNetworks[i].networkStrength + wifiDongleLevel > 0 && !myWifiNetworks[i].routerOnlyWiFi)
			{
				list.Add(myWifiNetworks[i]);
			}
		}
		return list;
	}

	public bool GetCurrentConnectedNetwork(out WifiNetworkDefinition currentNetwork)
	{
		currentNetwork = currentWifiNetwork;
		return IsOnline;
	}

	public bool CheckBSSID(string bssidToCheck, out WifiNetworkDefinition targetedWEP, WIFI_SECURITY crackerUsed)
	{
		targetedWEP = null;
		bool result = false;
		for (int i = 0; i < activeWifiHotSpot.myWifiNetworks.Count; i++)
		{
			if (!activeWifiHotSpot.myWifiNetworks[i].networkIsOffline && activeWifiHotSpot.myWifiNetworks[i].networkBSSID == bssidToCheck && activeWifiHotSpot.myWifiNetworks[i].networkSecurity == crackerUsed)
			{
				result = true;
				targetedWEP = activeWifiHotSpot.myWifiNetworks[i];
				i = activeWifiHotSpot.myWifiNetworks.Count;
			}
		}
		return result;
	}

	public void SafeDeactivateWiFiMenu()
	{
		if (wifiMenuAniActive)
		{
			TriggerWifiMenu();
		}
	}

	public void TriggerWifiMenu()
	{
		if (wifiMenuAniActive)
		{
			return;
		}
		wifiMenuAniActive = true;
		if (wifiMenuActive)
		{
			wifiMenuActive = false;
			wifiMenuPOS.x = wifiMenu.GetComponent<RectTransform>().anchoredPosition.x;
			wifiMenuPOS.y = wifiMenu.GetComponent<RectTransform>().sizeDelta.y;
			DOTween.To(() => wifiMenu.GetComponent<RectTransform>().anchoredPosition, delegate(Vector2 x)
			{
				wifiMenu.GetComponent<RectTransform>().anchoredPosition = x;
			}, wifiMenuPOS, 0.25f).SetEase(Ease.InQuad).OnComplete(delegate
			{
				wifiMenuAniActive = false;
			});
		}
		else
		{
			wifiMenuActive = true;
			wifiMenuPOS.x = wifiMenu.GetComponent<RectTransform>().anchoredPosition.x;
			wifiMenuPOS.y = -41f;
			DOTween.To(() => wifiMenu.GetComponent<RectTransform>().anchoredPosition, delegate(Vector2 x)
			{
				wifiMenu.GetComponent<RectTransform>().anchoredPosition = x;
			}, wifiMenuPOS, 0.25f).SetEase(Ease.OutQuad).OnComplete(delegate
			{
				wifiMenuAniActive = false;
			});
		}
	}

	private int getMinuteWearoutForSignal()
	{
		return currentWifiNetwork.networkSignal switch
		{
			WIFI_SIGNAL_TYPE.W80211B => (InventoryManager.WifiDongleLevel == 2) ? 5 : ((InventoryManager.WifiDongleLevel == 1) ? 10 : 15), 
			WIFI_SIGNAL_TYPE.W80211BP => (InventoryManager.WifiDongleLevel == 2) ? 4 : ((InventoryManager.WifiDongleLevel == 1) ? 7 : 10), 
			WIFI_SIGNAL_TYPE.W80211G => (InventoryManager.WifiDongleLevel == 2) ? 3 : ((InventoryManager.WifiDongleLevel == 1) ? 5 : 7), 
			WIFI_SIGNAL_TYPE.W80211N => (InventoryManager.WifiDongleLevel == 2) ? 2 : ((InventoryManager.WifiDongleLevel == 1) ? 3 : 5), 
			WIFI_SIGNAL_TYPE.W80211AC => (InventoryManager.WifiDongleLevel == 2) ? 1 : ((InventoryManager.WifiDongleLevel == 1) ? 2 : 3), 
			_ => 0, 
		};
	}

	public float GenereatePageLoadingTime()
	{
		int num = (int)(currentWifiNetwork.howLongConnected / 50f);
		int num2 = ((num > 0) ? (num / 60 * getMinuteWearoutForSignal()) : 0);
		if ((float)num2 > 0f)
		{
			Debug.Log("[WiFi Wearout] Current WiFi wearout: " + num2);
		}
		float num3 = (float)(currentWifiNetwork.networkPower + num2) * 0.2f;
		int num4 = Mathf.Min(currentWifiNetwork.networkStrength + InventoryManager.WifiDongleLevel, 3);
		num3 *= 1f - (float)num4 * 20f / 100f;
		num3 *= 1f - (float)InventoryManager.WifiDongleLevel * 10f / 100f;
		switch (currentWifiNetwork.networkSignal)
		{
		case WIFI_SIGNAL_TYPE.W80211B:
			num3 *= 0.95f;
			break;
		case WIFI_SIGNAL_TYPE.W80211BP:
			num3 *= 0.9f;
			break;
		case WIFI_SIGNAL_TYPE.W80211G:
			num3 *= 0.85f;
			break;
		case WIFI_SIGNAL_TYPE.W80211N:
			num3 *= 0.8f;
			break;
		case WIFI_SIGNAL_TYPE.W80211AC:
			num3 *= 0.75f;
			break;
		}
		num3 = Mathf.Max(num3, 0.5f);
		if (SpeedPoll.speedManipulatorActive)
		{
			switch (SpeedPoll.speedManipulatorData)
			{
			case TWITCH_NET_SPEED.FAST:
				num3 /= 3f;
				break;
			case TWITCH_NET_SPEED.SLOW:
				num3 *= 3f;
				break;
			}
		}
		return num3 + UnityEngine.Random.Range(0.25f, 0.75f);
	}

	private void changeWifiBars(int wifiBarAmount)
	{
		wifiIcon.GetComponent<Image>().sprite = wifiSprites[wifiBarAmount];
	}

	private void productWasPickedUp(ShadowMarketProductDefinition TheProduct)
	{
		HARDWARE_PRODUCTS productID = TheProduct.productID;
		if (productID == HARDWARE_PRODUCTS.WIFI_DONGLE_LEVEL2 || productID == HARDWARE_PRODUCTS.WIFI_DONGLE_LEVEL3)
		{
			InventoryManager.WifiDongleLevel++;
			if (IsOnline)
			{
				int wifiBarAmount = Mathf.Min(currentWifiNetwork.networkStrength + InventoryManager.WifiDongleLevel, 3);
				changeWifiBars(wifiBarAmount);
			}
			this.NewNetworksAvailable?.Invoke(GetCurrentWifiNetworks());
			theWifiDongle.RefreshActiveWifiDongleLevel();
			for (int i = 0; i < wifiHotSpots.Count; i++)
			{
				wifiHotSpots[i].RefreshPreviewDongle();
			}
			if (myData != null)
			{
				myData.OwnedWifiDongleLevel = InventoryManager.WifiDongleLevel;
				DataManager.Save(myData);
			}
		}
	}

	private void stageMe()
	{
		myData = DataManager.Load<WifiManagerData>(myID);
		if (myData == null)
		{
			myData = new WifiManagerData(myID)
			{
				ActiveWifiHotSpotIndex = 0,
				CurrentWifiNetworkIndex = wifiHotSpots[0].GetWifiNetworkIndex(defaultWifiNetwork),
				IsConnected = true,
				OwnedWifiDongleLevel = 0
			};
		}
		InventoryManager.WifiDongleLevel = myData.OwnedWifiDongleLevel;
		activeWifiHotSpot = wifiHotSpots[myData.ActiveWifiHotSpotIndex];
		theWifiDongle.PlaceDongle(activeWifiHotSpot.DonglePlacedPOS, activeWifiHotSpot.DonglePlacedROT, PlaySound: false);
		if (myData.IsConnected)
		{
			currentWifiNetwork = activeWifiHotSpot.GetWifiNetworkDefByIndex(myData.CurrentWifiNetworkIndex);
			ConnectToWifi(currentWifiNetwork, byPassSecuirty: true);
		}
		this.NewNetworksAvailable?.Invoke(GetCurrentWifiNetworks());
		SteamSlinger.Ins.AddWifiNetworks(GetAllWifiNetworks());
		EnvironmentManager.PowerBehaviour.PowerOffEvent.Event += DisconnectFromWifi;
		for (int i = 0; i < GetAllWifiNetworks().Count; i++)
		{
			if (GetAllWifiNetworks()[i].networkName.ToLower() != "freewifinovirus")
			{
				GetAllWifiNetworks()[i].affectedByDosDrainer = false;
			}
			GetAllWifiNetworks()[i].interaction = WiFiInteractionType.NONE;
			GetAllWifiNetworks()[i].injectWindowActive = false;
			GetAllWifiNetworks()[i].injectFireWindow = 0f;
			GetAllWifiNetworks()[i].injectTimeStamp = 0f;
			GetAllWifiNetworks()[i].howLongConnected = 0f;
		}
		WifiMenuBehaviour.Ins.refreshNetworks();
		GameManager.StageManager.Stage -= stageMe;
	}

	public WifiNetworkDefinition getCurrentWiFi()
	{
		return currentWifiNetwork;
	}

	public static string GetCurrentWifi2()
	{
		return GameManager.ManagerSlinger.WifiManager.getCurrentWiFi()?.networkName;
	}

	private void AddNewWiFi()
	{
		WifiNetworkDefinition wifiNetworkDefinition = ScriptableObject.CreateInstance<WifiNetworkDefinition>();
		WifiNetworkDefinition wifiNetworkDefinition2 = ScriptableObject.CreateInstance<WifiNetworkDefinition>();
		WifiNetworkDefinition wifiNetworkDefinition3 = ScriptableObject.CreateInstance<WifiNetworkDefinition>();
		WifiNetworkDefinition wifiNetworkDefinition4 = ScriptableObject.CreateInstance<WifiNetworkDefinition>();
		WifiNetworkDefinition wifiNetworkDefinition5 = ScriptableObject.CreateInstance<WifiNetworkDefinition>();
		WifiNetworkDefinition wifiNetworkDefinition6 = ScriptableObject.CreateInstance<WifiNetworkDefinition>();
		WifiNetworkDefinition wifiNetworkDefinition7 = ScriptableObject.CreateInstance<WifiNetworkDefinition>();
		WifiNetworkDefinition wifiNetworkDefinition8 = ScriptableObject.CreateInstance<WifiNetworkDefinition>();
		WifiNetworkDefinition wifiNetworkDefinition9 = ScriptableObject.CreateInstance<WifiNetworkDefinition>();
		wifiNetworkDefinition.id = 101;
		wifiNetworkDefinition.networkChannel = 6;
		wifiNetworkDefinition.networkName = "JackPott";
		wifiNetworkDefinition.networkPower = 55;
		wifiNetworkDefinition.networkStrength = -1;
		wifiNetworkDefinition.networkTrackProbability = 0.55f;
		wifiNetworkDefinition.networkTrackRate = 555f;
		wifiNetworkDefinition2.id = 102;
		wifiNetworkDefinition2.networkChannel = 6;
		wifiNetworkDefinition2.networkCoolOffTime = 83f;
		wifiNetworkDefinition2.networkInjectionCoolOffTime = 10f;
		wifiNetworkDefinition2.networkInjectionRandEnd = 400;
		wifiNetworkDefinition2.networkInjectionRandStart = 810;
		wifiNetworkDefinition2.networkMaxInjectionAmount = 43;
		wifiNetworkDefinition2.networkName = "MADP1NG";
		wifiNetworkDefinition2.networkPower = 7;
		wifiNetworkDefinition2.networkSecurity = WIFI_SECURITY.WPA;
		wifiNetworkDefinition2.networkSignal = WIFI_SIGNAL_TYPE.W80211G;
		wifiNetworkDefinition2.networkTrackProbability = 0.19f;
		wifiNetworkDefinition2.networkTrackRate = 584f;
		wifiNetworkDefinition3.id = 103;
		wifiNetworkDefinition3.networkChannel = 12;
		wifiNetworkDefinition3.networkName = "furrycon";
		wifiNetworkDefinition3.networkPower = 69;
		wifiNetworkDefinition3.networkRandPortEnd = 180;
		wifiNetworkDefinition3.networkRandPortStart = 420;
		wifiNetworkDefinition3.networkSecurity = WIFI_SECURITY.WEP;
		wifiNetworkDefinition3.networkSignal = WIFI_SIGNAL_TYPE.W80211G;
		wifiNetworkDefinition3.networkStrength = 3;
		wifiNetworkDefinition3.networkTrackProbability = 0.62f;
		wifiNetworkDefinition3.networkTrackRate = 621f;
		wifiNetworkDefinition4.id = 104;
		wifiNetworkDefinition4.networkChannel = 6;
		wifiNetworkDefinition4.networkName = "TheProgrammingChair";
		wifiNetworkDefinition4.networkPower = 54;
		wifiNetworkDefinition4.networkRandPortEnd = 110;
		wifiNetworkDefinition4.networkRandPortStart = 150;
		wifiNetworkDefinition4.networkSecurity = WIFI_SECURITY.WEP;
		wifiNetworkDefinition4.networkSignal = WIFI_SIGNAL_TYPE.W80211AC;
		wifiNetworkDefinition4.networkStrength = 1;
		wifiNetworkDefinition4.networkTrackProbability = 0.16f;
		wifiNetworkDefinition4.networkTrackRate = 1454f;
		wifiNetworkDefinition4.routerOnlyWiFi = true;
		wifiNetworkDefinition5.id = 105;
		wifiNetworkDefinition5.networkChannel = 2;
		wifiNetworkDefinition5.networkCoolOffTime = 11f;
		wifiNetworkDefinition5.networkInjectionCoolOffTime = 6f;
		wifiNetworkDefinition5.networkInjectionRandEnd = 420;
		wifiNetworkDefinition5.networkInjectionRandStart = 960;
		wifiNetworkDefinition5.networkMaxInjectionAmount = 70;
		wifiNetworkDefinition5.networkName = "GameNight At 602";
		wifiNetworkDefinition5.networkPower = 31;
		wifiNetworkDefinition5.networkSecurity = WIFI_SECURITY.WPA;
		wifiNetworkDefinition5.networkSignal = WIFI_SIGNAL_TYPE.W80211BP;
		wifiNetworkDefinition5.networkStrength = 3;
		wifiNetworkDefinition5.networkTrackProbability = 0.61f;
		wifiNetworkDefinition5.networkTrackRate = 411f;
		wifiNetworkDefinition6.id = 106;
		wifiNetworkDefinition6.networkChannel = 9;
		wifiNetworkDefinition6.networkName = "BillWiTheScienceFi";
		wifiNetworkDefinition6.networkPower = 76;
		wifiNetworkDefinition6.networkSignal = WIFI_SIGNAL_TYPE.W80211BP;
		wifiNetworkDefinition6.networkStrength = -1;
		wifiNetworkDefinition6.networkTrackProbability = 0.8f;
		wifiNetworkDefinition6.networkTrackRate = 499f;
		wifiNetworkDefinition7.id = 107;
		wifiNetworkDefinition7.networkChannel = 9;
		wifiNetworkDefinition7.networkCoolOffTime = 49f;
		wifiNetworkDefinition7.networkInjectionCoolOffTime = 4f;
		wifiNetworkDefinition7.networkInjectionRandEnd = 700;
		wifiNetworkDefinition7.networkInjectionRandStart = 790;
		wifiNetworkDefinition7.networkMaxInjectionAmount = 90;
		wifiNetworkDefinition7.networkName = "Definitely Not WiFi";
		wifiNetworkDefinition7.networkPower = (short)UnityEngine.Random.Range(17, 30);
		Debug.Log("[Randomized WiFi] WIFI Definitely Not Wifi - PWR: " + wifiNetworkDefinition7.networkPower);
		wifiNetworkDefinition7.networkSecurity = WIFI_SECURITY.WPA2;
		wifiNetworkDefinition7.networkSignal = WIFI_SIGNAL_TYPE.W80211N;
		wifiNetworkDefinition7.networkStrength = -1;
		wifiNetworkDefinition7.networkTrackProbability = 0.25f;
		wifiNetworkDefinition7.networkTrackRate = 911f;
		wifiNetworkDefinition8.id = 108;
		wifiNetworkDefinition8.networkChannel = 2;
		wifiNetworkDefinition8.networkCoolOffTime = 200f;
		wifiNetworkDefinition8.networkInjectionCoolOffTime = 20f;
		wifiNetworkDefinition8.networkInjectionRandEnd = 869;
		wifiNetworkDefinition8.networkInjectionRandStart = 920;
		wifiNetworkDefinition8.networkMaxInjectionAmount = 138;
		wifiNetworkDefinition8.networkName = "PipeLovers";
		wifiNetworkDefinition8.networkPower = 9;
		wifiNetworkDefinition8.networkSecurity = WIFI_SECURITY.WPA2;
		wifiNetworkDefinition8.networkSignal = WIFI_SIGNAL_TYPE.W80211AC;
		wifiNetworkDefinition8.networkStrength = 0;
		wifiNetworkDefinition8.networkTrackProbability = 0.14f;
		wifiNetworkDefinition8.networkTrackRate = 1805f;
		wifiNetworkDefinition8.routerOnlyWiFi = true;
		wifiNetworkDefinition9.id = 109;
		wifiNetworkDefinition9.networkChannel = 5;
		wifiNetworkDefinition9.networkName = "DonaldsWifi2";
		wifiNetworkDefinition9.networkPower = 49;
		wifiNetworkDefinition9.networkTrackProbability = 0.86f;
		wifiNetworkDefinition9.networkTrackRate = 412f;
		wifiHotSpots[0].myWifiNetworks.Insert(1, wifiNetworkDefinition);
		wifiHotSpots[0].myWifiNetworks.Insert(7, wifiNetworkDefinition2);
		wifiHotSpots[0].myWifiNetworks.Insert(8, wifiNetworkDefinition5);
		wifiHotSpots[0].myWifiNetworks.Insert(2, wifiNetworkDefinition6);
		wifiHotSpots[0].myWifiNetworks.Add(wifiNetworkDefinition8);
		wifiHotSpots[0].myWifiNetworks.Add(wifiNetworkDefinition7);
		wifiHotSpots[1].myWifiNetworks.Insert(4, wifiNetworkDefinition3);
		wifiHotSpots[1].myWifiNetworks.Insert(5, wifiNetworkDefinition4);
		wifiHotSpots[1].myWifiNetworks.Insert(2, wifiNetworkDefinition9);
		WifiNetworkDefinition wifiNetworkDefinition10 = ScriptableObject.CreateInstance<WifiNetworkDefinition>();
		WifiNetworkDefinition wifiNetworkDefinition11 = ScriptableObject.CreateInstance<WifiNetworkDefinition>();
		WifiNetworkDefinition wifiNetworkDefinition12 = ScriptableObject.CreateInstance<WifiNetworkDefinition>();
		wifiNetworkDefinition10.networkName = "Byte Me";
		wifiNetworkDefinition11.networkName = "Get Off My LAN";
		wifiNetworkDefinition12.networkName = "PanicAtTheCisco";
		wifiNetworkDefinition12.networkStrength = -1;
		List<WifiNetworkDefinition> list = new List<WifiNetworkDefinition> { wifiNetworkDefinition10, wifiNetworkDefinition11, wifiNetworkDefinition12 };
		for (int i = 0; i < list.Count; i++)
		{
			list[i].id = 121 + i;
			list[i].networkChannel = (short)UnityEngine.Random.Range(1, 13);
			list[i].networkPower = (short)UnityEngine.Random.Range(51, 81);
			list[i].networkRandPortEnd = 998;
			list[i].networkRandPortStart = 2;
			list[i].networkSecurity = WIFI_SECURITY.WEP;
			list[i].networkSignal = (WIFI_SIGNAL_TYPE)UnityEngine.Random.Range(0, 3);
			int num = UnityEngine.Random.Range(59, 82);
			int num2 = UnityEngine.Random.Range(408, 706);
			list[i].networkTrackProbability = (float)num / 100f;
			list[i].networkTrackRate = num2;
			Debug.LogFormat("[Randomized WiFi] WIFI {0} - CH: {1} | PWR: {2} | SIG: {3} | PROB: {4} | RATE: {5}", list[i].networkName, list[i].networkChannel, list[i].networkPower, list[i].networkSignal.ToString(), list[i].networkTrackProbability.ToString(), list[i].networkTrackRate.ToString());
		}
		wifiHotSpots[0].myWifiNetworks.Insert(5, wifiNetworkDefinition11);
		wifiHotSpots[0].myWifiNetworks.Insert(5, wifiNetworkDefinition10);
		wifiHotSpots[2].myWifiNetworks.Insert(3, wifiNetworkDefinition12);
	}
}
