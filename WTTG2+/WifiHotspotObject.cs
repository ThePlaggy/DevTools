using System.Collections.Generic;
using UnityEngine;

public class WifiHotspotObject : MonoBehaviour
{
	public Vector3 DonglePlacedPOS;

	public Vector3 DonglePlacedROT;

	public MeshRenderer DonglePreviewLevel1;

	public MeshRenderer DonglePreviewLevel2;

	public MeshRenderer DonglePreviewLevel3;

	public MeshRenderer HighlightBase;

	public GameObject Trigger;

	public List<WifiNetworkDefinition> myWifiNetworks;

	private MeshRenderer currentDonglePreview;

	private static readonly string[] BeerList = new string[28]
	{
		"budweiser", "heineken", "guinness", "hoegaarden", "pilsner", "staropramen", "lobkowicz", "zichovec", "starobrno", "breznak",
		"bakalar", "kozel", "budvar", "bruncvik", "chodovar", "gambrinus", "svijany", "bernard", "holba", "ostravar",
		"radegast", "pardal", "krusnohor", "birell", "louny", "primator", "zatec", "zlatopramen"
	};

	private void Awake()
	{
		currentDonglePreview = DonglePreviewLevel1;
		HighlightBase.enabled = false;
		Trigger.SetActive(value: false);
		if (base.gameObject.name == "WifiDongleHotspot4")
		{
			base.transform.position = new Vector3(1f, 1.008f, -21f);
			DonglePlacedPOS = new Vector3(1f, 1.008f, -21f);
		}
		GameManager.StageManager.Stage += stageMe;
		GameManager.StageManager.TheGameIsLive += gameLive;
	}

	public void ActivateMe()
	{
		HighlightBase.enabled = true;
		Trigger.SetActive(value: true);
	}

	public void DeactivateMe()
	{
		HighlightBase.enabled = false;
		Trigger.SetActive(value: false);
	}

	public void ShowPreview()
	{
		currentDonglePreview.enabled = true;
	}

	public void HidePreview()
	{
		currentDonglePreview.enabled = false;
	}

	public void PlaceDongleHere()
	{
		HidePreview();
		GameManager.ManagerSlinger.WifiManager.ExitWifiDonglePlacementMode(this);
	}

	public void RefreshPreviewDongle()
	{
		currentDonglePreview.enabled = false;
		switch (InventoryManager.WifiDongleLevel)
		{
		case 1:
			currentDonglePreview = DonglePreviewLevel2;
			break;
		case 2:
			currentDonglePreview = DonglePreviewLevel3;
			break;
		default:
			currentDonglePreview = DonglePreviewLevel1;
			break;
		}
	}

	public int GetWifiNetworkIndex(WifiNetworkDefinition TheNetwork)
	{
		int result = 0;
		for (int i = 0; i < myWifiNetworks.Count; i++)
		{
			if (myWifiNetworks[i] == TheNetwork)
			{
				result = i;
				i = myWifiNetworks.Count;
			}
		}
		return result;
	}

	public WifiNetworkDefinition GetWifiNetworkDefByIndex(int setIndex)
	{
		if (myWifiNetworks[setIndex] != null)
		{
			return myWifiNetworks[setIndex];
		}
		return null;
	}

	private void prepWifiNetworks()
	{
		List<string> list = new List<string>(GameManager.ManagerSlinger.WifiManager.PasswordList.Keys);
		for (int i = 0; i < myWifiNetworks.Count; i++)
		{
			switch (myWifiNetworks[i].networkName.ToLower())
			{
			case "hidden network":
				myWifiNetworks[i].networkStrength = -1;
				break;
			case "donaldswifi":
				myWifiNetworks[i].networkStrength = 0;
				myWifiNetworks[i].networkSignal = WIFI_SIGNAL_TYPE.W80211N;
				break;
			case "big dave network":
			case "mycci7471":
			case "tedata":
			case "joneslaw":
				myWifiNetworks[i].networkStrength = 0;
				break;
			case "freewifinovirus":
				myWifiNetworks[i].affectedByDosDrainer = true;
				break;
			case "d59709":
				myWifiNetworks[i].networkTrackRate = 477f;
				break;
			case "sswifi":
				myWifiNetworks[i].networkTrackRate = 543f;
				break;
			case "bring beer to 504":
				myWifiNetworks[i].networkSignal = WIFI_SIGNAL_TYPE.W80211N;
				break;
			}
			WifiNetworkData wifiNetworkData = new WifiNetworkData(myWifiNetworks[i].networkName.GetHashCode());
			wifiNetworkData.NetworkBSSID = MagicSlinger.GenerateRandomHexCode(2, 5, ":");
			if (myWifiNetworks[i].networkSecurity > WIFI_SECURITY.NONE)
			{
				int index = Random.Range(0, list.Count);
				wifiNetworkData.NetworkPassword = GameManager.ManagerSlinger.WifiManager.PasswordList[list[index]];
				list.RemoveAt(index);
			}
			else
			{
				wifiNetworkData.NetworkPassword = string.Empty;
			}
			if (myWifiNetworks[i].networkSecurity == WIFI_SECURITY.WEP)
			{
				wifiNetworkData.NetworkOpenPort = (short)Random.Range(myWifiNetworks[i].networkRandPortStart, myWifiNetworks[i].networkRandPortEnd);
			}
			else
			{
				wifiNetworkData.NetworkOpenPort = 0;
			}
			if (myWifiNetworks[i].networkSecurity == WIFI_SECURITY.WPA || myWifiNetworks[i].networkSecurity == WIFI_SECURITY.WPA2)
			{
				wifiNetworkData.NetworkInjectionAmount = (short)Random.Range(myWifiNetworks[i].networkInjectionRandStart, myWifiNetworks[i].networkInjectionRandEnd);
			}
			myWifiNetworks[i].networkBSSID = ((myWifiNetworks[i].networkName == "TheProgrammingChair") ? "AM:PE:RC:Z1:54" : wifiNetworkData.NetworkBSSID);
			switch (myWifiNetworks[i].networkName)
			{
			case "furrycon":
				myWifiNetworks[i].networkPassword = "fierce" + Random.Range(10000, 99999);
				break;
			case "Test Network":
				myWifiNetworks[i].networkPassword = "NothingThatShines";
				break;
			case "TheProgrammingChair":
				myWifiNetworks[i].networkPassword = MagicSlinger.MD5It(wifiNetworkData.NetworkPassword).Substring(0, 12);
				break;
			case "Bring Beer to 504":
			{
				bool flag = Random.Range(0, 100) >= 75;
				string text = string.Empty;
				for (int j = 1; j <= Random.Range(3, 10); j++)
				{
					text += j;
				}
				myWifiNetworks[i].networkPassword = BeerList[Random.Range(0, BeerList.Length)];
				if (flag)
				{
					myWifiNetworks[i].networkPassword += text;
				}
				break;
			}
			default:
				myWifiNetworks[i].networkPassword = wifiNetworkData.NetworkPassword;
				break;
			}
			myWifiNetworks[i].networkOpenPort = wifiNetworkData.NetworkOpenPort;
			myWifiNetworks[i].networkInjectionAmount = wifiNetworkData.NetworkInjectionAmount;
		}
	}

	public void gameLive()
	{
		RefreshPreviewDongle();
		GameManager.StageManager.TheGameIsLive -= gameLive;
	}

	private void stageMe()
	{
		prepWifiNetworks();
		GameManager.StageManager.Stage -= stageMe;
	}
}
