using UnityEngine;

public static class WifiInteractionManager
{
	public static string DOSTwitchIns;

	public static void LockTheWiFi(int id)
	{
		WifiNetworkDefinition definition = GetDefinition(id);
		if (definition.interaction != WiFiInteractionType.LOCKED)
		{
			definition.interaction--;
		}
		if (GameManager.ManagerSlinger.WifiManager.IsOnline && GameManager.ManagerSlinger.WifiManager.getCurrentWiFi() == definition)
		{
			GameManager.ManagerSlinger.WifiManager.DisconnectFromWifi();
		}
		Debug.Log("[WIM] Locking WiFi " + definition.networkName);
	}

	public static void UnlockTheWiFi(int id)
	{
		if (id == -2 || id == -3)
		{
			id -= 10;
		}
		if (id == -12 && GameManager.ManagerSlinger.WifiManager.GetWiFiAlt(All: false).Count <= 0)
		{
			id--;
		}
		if (id == -13 && GameManager.ManagerSlinger.WifiManager.GetWiFiAlt(All: true).Count <= 0)
		{
			Debug.LogError("[WIM] No suitable network to unlock");
			DOSTwitchIns = "null";
			return;
		}
		WifiNetworkDefinition definition = GetDefinition(id);
		if (definition.networkSecurity == WIFI_SECURITY.NONE && definition.interaction != WiFiInteractionType.LOCKED)
		{
			Debug.LogFormat("[WIM] Cannot unlock a free WiFi {0}", definition.networkName);
			UnlockTheWiFi(-12);
			return;
		}
		if (definition.interaction == WiFiInteractionType.UNLOCKED)
		{
			Debug.LogFormat("[WIM] Cannot unlock an already unlocked WiFi {0}", definition.networkName);
			UnlockTheWiFi(-13);
			return;
		}
		if (definition.interaction != WiFiInteractionType.UNLOCKED)
		{
			definition.interaction++;
		}
		Debug.Log("[WIM] UnLocking WiFi " + definition.networkName);
	}

	public static void WiFiPassword(int id)
	{
		WifiNetworkDefinition definition = GetDefinition(id);
		if ((id == -2 || id == -3) && definition.networkSecurity == WIFI_SECURITY.NONE)
		{
			WiFiPassword(id);
		}
		else
		{
			GameManager.ManagerSlinger.TextDocManager.CreateTextDoc(definition.networkName, definition.networkPassword);
		}
	}

	public static void SetBeer504Password()
	{
		WifiNetworkDefinition definition = GetDefinition(17);
		BeerTrigger.SetWiFiPassword(definition.networkPassword);
	}

	private static WifiNetworkDefinition GetDefinition(int id)
	{
		WifiNetworkDefinition wifiNetworkDefinition = null;
        switch (id)
        {
            case -4:
                wifiNetworkDefinition = WiFiPoll.targetWifi;
                break;
            case -3:
                wifiNetworkDefinition = GetRandomWiFi();
                break;
            case -13:
                wifiNetworkDefinition = GetRandomWiFiAlt();
                break;
            case -2:
                wifiNetworkDefinition = GetRandomLocalWiFi();
                break;
            case -12:
                wifiNetworkDefinition = GetRandomLocalWiFiAlt();
                break;
            case -1:
                wifiNetworkDefinition = GetCurrentWiFi();
                break;
            default:
                wifiNetworkDefinition = GetWiFiByID(id);
                break;
        }
        DOSTwitchIns = wifiNetworkDefinition.networkName;
		return wifiNetworkDefinition;
	}

	public static void ChangeWiFiPassword(int id, string password)
	{
		WifiNetworkDefinition definition = GetDefinition(id);
		if (definition.networkSecurity != WIFI_SECURITY.NONE)
		{
			if (GameManager.ManagerSlinger.WifiManager.IsOnline && GameManager.ManagerSlinger.WifiManager.getCurrentWiFi() == definition)
			{
				GameManager.ManagerSlinger.WifiManager.DisconnectFromWifi();
			}
			definition.networkPassword = password;
			Debug.Log("[WIM] Changed WiFi Password to " + definition.networkName + " with the password " + password);
		}
	}

	private static WifiNetworkDefinition GetCurrentWiFi()
	{
		return (GameManager.ManagerSlinger.WifiManager.getCurrentWiFi() == null) ? WiFiPoll.lastConnectedWifi : GameManager.ManagerSlinger.WifiManager.getCurrentWiFi();
	}

	private static WifiNetworkDefinition GetWiFiByID(int id)
	{
		return (id >= GameManager.ManagerSlinger.WifiManager.GetAllWifiNetworks().Count) ? GetWiFiByID(0) : GameManager.ManagerSlinger.WifiManager.GetAllWifiNetworks()[id];
	}

	private static WifiNetworkDefinition GetRandomLocalWiFi()
	{
		return GameManager.ManagerSlinger.WifiManager.GetCurrentWifiNetworks()[Random.Range(0, GameManager.ManagerSlinger.WifiManager.GetCurrentWifiNetworks().Count)];
	}

	private static WifiNetworkDefinition GetRandomWiFi()
	{
		return GameManager.ManagerSlinger.WifiManager.GetAllWifiNetworks()[Random.Range(0, GameManager.ManagerSlinger.WifiManager.GetAllWifiNetworks().Count)];
	}

	private static WifiNetworkDefinition GetRandomLocalWiFiAlt()
	{
		return GameManager.ManagerSlinger.WifiManager.GetWiFiAlt(All: false)[Random.Range(0, GameManager.ManagerSlinger.WifiManager.GetWiFiAlt(All: false).Count)];
	}

	private static WifiNetworkDefinition GetRandomWiFiAlt()
	{
		return GameManager.ManagerSlinger.WifiManager.GetWiFiAlt(All: true)[Random.Range(0, GameManager.ManagerSlinger.WifiManager.GetWiFiAlt(All: true).Count)];
	}
}
