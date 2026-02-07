using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WifiMenuBehaviour : MonoBehaviour, IPointerExitHandler, IEventSystemHandler
{
	private const int MENU_OPT_START_POOL = 8;

	private const float OPT_SPACING = 3f;

	private const float OPT_SETX = 10f;

	private const float MENU_BOT = 13f;

	public static WifiMenuBehaviour Ins;

	public GameObject WifiMenuOption;

	public GameObject WifiMenuDisconnect;

	public GameObject WifiMenuSeperator;

	private List<WifiMenuNetworkOptionObject> currentOptions = new List<WifiMenuNetworkOptionObject>();

	private GameObject menuDisObject;

	private Vector2 menuOptionPOS = new Vector2(10f, 0f);

	private Vector2 menuOptOffScreenPOS = new Vector2(0f, 24f);

	private Vector2 menuPOS = Vector2.zero;

	private GameObject menuSepObject;

	private Vector2 menuSize = Vector2.zero;

	private PooledStack<WifiMenuNetworkOptionObject> wifiMenuOptionObjectPool;

	private void Awake()
	{
		Ins = this;
		menuSepObject = Object.Instantiate(WifiMenuSeperator, GetComponent<RectTransform>());
		menuDisObject = Object.Instantiate(WifiMenuDisconnect, GetComponent<RectTransform>());
		menuSepObject.GetComponent<RectTransform>().anchoredPosition = menuOptOffScreenPOS;
		menuDisObject.GetComponent<RectTransform>().anchoredPosition = menuOptOffScreenPOS;
		wifiMenuOptionObjectPool = new PooledStack<WifiMenuNetworkOptionObject>(delegate
		{
			WifiMenuNetworkOptionObject component = Object.Instantiate(WifiMenuOption, GetComponent<RectTransform>()).GetComponent<WifiMenuNetworkOptionObject>();
			component.SoftBuild();
			return component;
		}, 8);
		GameManager.ManagerSlinger.WifiManager.NewNetworksAvailable += buildNetworks;
	}

	private void Start()
	{
		GameManager.ManagerSlinger.WifiManager.WentOnline += refreshNetworks;
		GameManager.ManagerSlinger.WifiManager.WentOffline += refreshNetworks;
	}

	private void OnDestroy()
	{
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		GameManager.ManagerSlinger.WifiManager.TriggerWifiMenu();
	}

	public void refreshNetworks()
	{
		buildNetworks(GameManager.ManagerSlinger.WifiManager.GetCurrentWifiNetworks());
	}

	private void buildNetworks(List<WifiNetworkDefinition> Networks)
	{
		menuSepObject.GetComponent<RectTransform>().anchoredPosition = menuOptOffScreenPOS;
		menuDisObject.GetComponent<RectTransform>().anchoredPosition = menuOptOffScreenPOS;
		for (int i = 0; i < currentOptions.Count; i++)
		{
			currentOptions[i].Clear();
			wifiMenuOptionObjectPool.Push(currentOptions[i]);
		}
		currentOptions.Clear();
		float num = WifiMenuOption.GetComponent<RectTransform>().sizeDelta.y * (float)Networks.Count + 3f * (float)Networks.Count + 13f;
		float num2 = -3f;
		WifiNetworkDefinition currentNetwork;
		bool currentConnectedNetwork = GameManager.ManagerSlinger.WifiManager.GetCurrentConnectedNetwork(out currentNetwork);
		if (currentConnectedNetwork)
		{
			num = num + 6f + 1f + WifiMenuDisconnect.GetComponent<RectTransform>().sizeDelta.y;
		}
		menuSize.x = GetComponent<RectTransform>().sizeDelta.x;
		menuSize.y = num;
		menuPOS.x = GetComponent<RectTransform>().anchoredPosition.x;
		menuPOS.y = num;
		GetComponent<RectTransform>().sizeDelta = menuSize;
		GetComponent<RectTransform>().anchoredPosition = menuPOS;
		for (int j = 0; j < Networks.Count; j++)
		{
			WifiMenuNetworkOptionObject wifiMenuNetworkOptionObject = wifiMenuOptionObjectPool.Pop();
			menuOptionPOS.y = num2;
			bool connected = currentConnectedNetwork && currentNetwork.networkName == Networks[j].networkName;
			wifiMenuNetworkOptionObject.Build(connected, Networks[j], menuOptionPOS);
			currentOptions.Add(wifiMenuNetworkOptionObject);
			num2 = num2 - wifiMenuNetworkOptionObject.GetComponent<RectTransform>().sizeDelta.y - 3f;
		}
		if (currentConnectedNetwork)
		{
			menuOptionPOS.y = menuOptionPOS.y - WifiMenuOption.GetComponent<RectTransform>().sizeDelta.y - 3f;
			menuSepObject.GetComponent<RectTransform>().anchoredPosition = menuOptionPOS;
			menuOptionPOS.y = menuOptionPOS.y - 3f - 1f;
			menuDisObject.GetComponent<RectTransform>().anchoredPosition = menuOptionPOS;
		}
	}
}
