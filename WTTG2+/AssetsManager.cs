using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AssetsManager : MonoBehaviour
{
	public static Dictionary<string, ASoftLoader> CurrentBundles = new Dictionary<string, ASoftLoader>();

	public static AssetsManager Ins;

	public static Action<string> AssetsFailed;

	public static bool AssetsReady;

	private bool Initialized;

	private bool Failed;

	public static event Action AssetsLoading;

	public static event Action AssetsLoaded;

	public AssetsManager()
	{
		AddLoader(BUNDLE.ASOFT, 50, 414, 26);
		AddLoader(BUNDLE.BOMBMAKER, 13, 454, 568);
		AddLoader(BUNDLE.BREATHERPLUS, 14, 310, 237);
		AddLoader(BUNDLE.EVENTS, 14, 61, 564);
		AddLoader(BUNDLE.EXECUTIONER, 18, 119, 581);
		AddLoader(BUNDLE.GAME_THEMES, 2, 990, 119);
		AddLoader(BUNDLE.HACKERMODE, 20, 510, 131);
		AddLoader(BUNDLE.KIDNAPPER, 12, 382, 10);
		AddLoader(BUNDLE.LUCASPLUS, 11, 123, 517);
		AddLoader(BUNDLE.MISCAUDIO, 5, 794, 387);
		AddLoader(BUNDLE.NASKO, 9, 991, 506);
		AddLoader(BUNDLE.OTREXDEV, 63, 295, 281);
		AddLoader(BUNDLE.SPRITE, 0, 515, 543);
		AddLoader(BUNDLE.TROPHIES, 36, 551, 677);
		AddLoader(BUNDLE.DELFALCO, 47, 593, 238);
		AddLoader(BUNDLE.PROPS, 12, 182, 104);
		AddLoader(BUNDLE.RADIO, 40, 546, 343);
		AddLoader(BUNDLE.NOIRPLUS, 32, 996, 251);
		AddLoader(BUNDLE.MENU, 1, 12, 892);
		AddLoader(BUNDLE.MENU_MUSIC, 31, 969, 445);
		AddLoader(BUNDLE.MENU_THEMES, 61, 138, 572);
		AddLoader(BUNDLE.WEBSITEDATA, 28, 91, 260);
		AddLoader(BUNDLE.WEBSITES_ROOT, 4, 206, 420);
		AddLoader(BUNDLE.WEBSITES_VANILLA, 95, 783, 557);
		AddLoader(BUNDLE.WEBSITES_WTTG1, 68, 276, 790);
		AddLoader(BUNDLE.WEBSITES_NASKO222, 75, 459, 374);
		AddLoader(BUNDLE.WEBSITES_KOTZWURST, 54, 892, 458);
		AddLoader(BUNDLE.WEBSITES_FIERCE, 31, 938, 965);
		AddLoader(BUNDLE.WEBSITES_OTREX, 100, 526, 24);
		AddLoader(BUNDLE.WEBSITES_OTHERS, 22, 967, 377);
		AddLoader(BUNDLE.POSTCREDITS, 35, 24, 367);
		AddLoader(BUNDLE.CASUAL, 0, 418, 782);
	}

	public static void AddLoader(BUNDLE bundle, int a0, int a1, int a2)
	{
		long fileSize = a2 + a1 * 1000 + a0 * 1000000;
		AddLoader(bundle, fileSize);
	}

	public static void AddLoader(BUNDLE Bundle, long FileSize)
	{
		AddLoader(Bundle.ToString(), FileSize);
	}

	public static void AddLoader(string BundleName, long FileSize)
	{
		ASoftLoader value = new ASoftLoader(BundleName, FileSize);
		if (CurrentBundles.ContainsKey(BundleName))
		{
			CurrentBundles[BundleName] = value;
		}
		else
		{
			CurrentBundles.Add(BundleName, value);
		}
	}

	public static ASoftLoader GetLoader(BUNDLE Bundle)
	{
		return GetLoader(Bundle.ToString());
	}

	public static ASoftLoader GetLoader(string BundleName)
	{
		ASoftLoader value;
		return CurrentBundles.TryGetValue(BundleName, out value) ? value : null;
	}

	public void LoadAssets()
	{
		Debug.Log("[AssetsManager] Loading Assets");
		AssetsManager.AssetsLoading?.Invoke();
		foreach (ASoftLoader value in CurrentBundles.Values)
		{
			StartCoroutine(value.LoadLocalAsset());
		}
	}

	private void AssetsLoadingFailed(string reason)
	{
		Debug.Log("[AssetsManager] Assets Failed to Download, reason: " + reason);
		Failed = true;
	}

	private void Awake()
	{
		if (AssetsReady)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		UnityEngine.Object.DontDestroyOnLoad(this);
		Ins = this;
		AssetsFailed = (Action<string>)Delegate.Remove(AssetsFailed, new Action<string>(AssetsLoadingFailed));
	}

	private void Update()
	{
		if (!Failed && !Initialized && CurrentBundles.All((KeyValuePair<string, ASoftLoader> pair) => pair.Value.iAmReady))
		{
			Initialized = true;
			AssetsReady = true;
			AssetsManager.AssetsLoaded?.Invoke();
			Debug.Log("[AssetsManager] Asset bundles Loaded");
		}
	}

	private void OnDestroy()
	{
		Ins = null;
		AssetsFailed = (Action<string>)Delegate.Remove(AssetsFailed, new Action<string>(AssetsLoadingFailed));
		Debug.Log("[AssetsManager] Unloading Bundles...");
		AssetBundle.UnloadAllAssetBundles(unloadAllObjects: true);
	}
}
