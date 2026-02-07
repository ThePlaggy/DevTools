using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class SteamSlinger : MonoBehaviour
{
	public static SteamSlinger Ins;

	private Dictionary<int, bool> crackedWifiNetworks = new Dictionary<int, bool>(50);

	private bool ghostingA;

	private bool ghostingB;

	private Dictionary<int, bool> loreDocs = new Dictionary<int, bool>(25);

	private SteamSlingerData myData;

	private bool requestedCurrentStats;

	private Dictionary<int, bool> shadowMarketProducts = new Dictionary<int, bool>(10);

	private bool stalkerA;

	private bool stalkerB;

	private bool storeStats;

	private Dictionary<int, bool> tutDocs = new Dictionary<int, bool>(8);

	private Dictionary<int, bool> zerodayProducts = new Dictionary<int, bool>(10);

	private int zoneLostCount;

	private int zoneWonCount;

	private void Awake()
	{
		Ins = this;
	}

	private void Start()
	{
		myData = new SteamSlingerData(221445);
		if (myData == null)
		{
			myData = new SteamSlingerData(221445);
			myData.ProductPickUpCount = 0;
		}
	}

	private void Update()
	{
		SteamAPI.RunCallbacks();
		if (!SteamManager.Initialized)
		{
			return;
		}
		if (!requestedCurrentStats)
		{
			if (!SteamManager.Initialized)
			{
				requestedCurrentStats = true;
				return;
			}
			bool flag = SteamUserStats.RequestCurrentStats();
			requestedCurrentStats = flag;
		}
		if (storeStats)
		{
			SteamAPI.RunCallbacks();
			bool flag2 = SteamUserStats.StoreStats();
			storeStats = !flag2;
		}
	}

	public void UnlockSteamAchievement(STEAM_ACHIEVEMENT TheAchievement)
	{
	}

	public void ClearAchievements()
	{
		if (SteamManager.Initialized)
		{
			SteamUserStats.ResetAllStats(bAchievementsToo: true);
		}
	}

	public void ClearReset()
	{
		tutDocs.Clear();
		loreDocs.Clear();
		zerodayProducts.Clear();
		shadowMarketProducts.Clear();
		crackedWifiNetworks.Clear();
		tutDocs = new Dictionary<int, bool>(8);
		loreDocs = new Dictionary<int, bool>(25);
		zerodayProducts = new Dictionary<int, bool>(10);
		shadowMarketProducts = new Dictionary<int, bool>(10);
		crackedWifiNetworks = new Dictionary<int, bool>(50);
	}

	public void PlayerDeclinedStartCall()
	{
		ghostingA = true;
	}

	public void PlayerDeclinedProductCall()
	{
		ghostingB = true;
		if (ghostingA && ghostingB)
		{
			UnlockSteamAchievement(STEAM_ACHIEVEMENT.GHOSTING);
		}
	}

	public void AddTutDoc(int HashCode)
	{
		tutDocs.Add(HashCode, value: false);
	}

	public void ReadTutDoc(int HashCode)
	{
		if (tutDocs.ContainsKey(HashCode))
		{
			tutDocs[HashCode] = true;
		}
		bool flag = true;
		foreach (KeyValuePair<int, bool> tutDoc in tutDocs)
		{
			if (!tutDoc.Value)
			{
				flag = false;
			}
		}
		if (flag)
		{
			UnlockSteamAchievement(STEAM_ACHIEVEMENT.KNOWLEDGEISPOWER);
		}
	}

	public void AddLoreDoc(int HashCode)
	{
		loreDocs.Add(HashCode, value: false);
	}

	public void InspectLoreDoc(int HashCode)
	{
		if (loreDocs.ContainsKey(HashCode))
		{
			loreDocs[HashCode] = true;
		}
		bool flag = true;
		foreach (KeyValuePair<int, bool> loreDoc in loreDocs)
		{
			if (!loreDoc.Value)
			{
				flag = false;
			}
		}
		if (flag)
		{
			UnlockSteamAchievement(STEAM_ACHIEVEMENT.LOREJUNKY);
		}
	}

	public void CheckStalkerURL(string theURL)
	{
		if (theURL == "http://www.twitter.com/thewebpro")
		{
			stalkerA = true;
		}
		if (theURL == "http://www.youtube.com/c/ReflectStudios")
		{
			stalkerB = true;
		}
		if (stalkerA && stalkerB)
		{
			UnlockSteamAchievement(STEAM_ACHIEVEMENT.STALKER);
		}
	}

	public void PlayerLostZone()
	{
		zoneWonCount = 0;
		zoneLostCount++;
		if (zoneLostCount >= 20)
		{
			UnlockSteamAchievement(STEAM_ACHIEVEMENT.BUTTERFINGERS);
		}
	}

	public void PlayerBeatZone()
	{
		zoneLostCount = 0;
		zoneWonCount++;
		if (zoneWonCount >= 20)
		{
			UnlockSteamAchievement(STEAM_ACHIEVEMENT.CLICKMASTER);
		}
	}

	public void AddZeroDayProduct(int HashCode)
	{
		zerodayProducts.Add(HashCode, value: false);
	}

	public void ActivateZeroDayProduct(int HashCode)
	{
		if (zerodayProducts.ContainsKey(HashCode))
		{
			zerodayProducts[HashCode] = true;
		}
		bool flag = true;
		foreach (KeyValuePair<int, bool> zerodayProduct in zerodayProducts)
		{
			if (!zerodayProduct.Value)
			{
				flag = false;
			}
		}
		if (flag)
		{
			UnlockSteamAchievement(STEAM_ACHIEVEMENT.DIGITALCONSUMER);
		}
		ShoppingSpreeCheck();
	}

	public void AddShadowMarketProduct(int HashCode)
	{
		shadowMarketProducts.Add(HashCode, value: false);
	}

	public void ActivateShadowMarketProduct(int HashCode)
	{
		if (shadowMarketProducts.ContainsKey(HashCode))
		{
			shadowMarketProducts[HashCode] = true;
		}
		bool flag = true;
		foreach (KeyValuePair<int, bool> shadowMarketProduct in shadowMarketProducts)
		{
			if (!shadowMarketProduct.Value)
			{
				flag = false;
			}
		}
		if (flag)
		{
			UnlockSteamAchievement(STEAM_ACHIEVEMENT.SHIPPINGHANDLER);
		}
		ShoppingSpreeCheck();
	}

	public void ShoppingSpreeCheck()
	{
		bool flag = true;
		foreach (KeyValuePair<int, bool> zerodayProduct in zerodayProducts)
		{
			if (!zerodayProduct.Value)
			{
				flag = false;
			}
		}
		foreach (KeyValuePair<int, bool> shadowMarketProduct in shadowMarketProducts)
		{
			if (!shadowMarketProduct.Value)
			{
				flag = false;
			}
		}
		if (flag)
		{
			UnlockSteamAchievement(STEAM_ACHIEVEMENT.SHOPPINGSPREE);
		}
	}

	public void AddWifiNetworks(List<WifiNetworkDefinition> TheNetworks)
	{
		for (int i = 0; i < TheNetworks.Count; i++)
		{
			if (TheNetworks[i].networkSecurity != WIFI_SECURITY.NONE && !crackedWifiNetworks.ContainsKey(TheNetworks[i].GetHashCode()))
			{
				crackedWifiNetworks.Add(TheNetworks[i].GetHashCode(), value: false);
			}
		}
	}

	public void CrackWifiNetwork(int HashCode)
	{
		if (crackedWifiNetworks.ContainsKey(HashCode))
		{
			crackedWifiNetworks[HashCode] = true;
		}
		bool flag = true;
		foreach (KeyValuePair<int, bool> crackedWifiNetwork in crackedWifiNetworks)
		{
			if (!crackedWifiNetwork.Value)
			{
				flag = false;
			}
		}
		if (flag)
		{
			UnlockSteamAchievement(STEAM_ACHIEVEMENT.INFILTRATOR);
		}
	}

	public void AddPurchasedProduct()
	{
		if (myData != null)
		{
			myData.ProductPickUpCount += 1;
			DataManager.Save(myData);
		}
	}

	public void CheckForPro()
	{
		if (myData != null && myData.ProductPickUpCount <= 1)
		{
			UnlockSteamAchievement(STEAM_ACHIEVEMENT.THEPROFESSIONAL);
		}
	}
}
