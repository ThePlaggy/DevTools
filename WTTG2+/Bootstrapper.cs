using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
	public static bool VersionIsExpired;

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		FireUp();
		AssetsManager.AssetsLoaded += AssetBundleManager.Ins.LoadAssetBundles;
		SceneManager.sceneLoaded += OnSceneLoaded;
		Debug.Log("[ASoftBootstrapper] Loaded successfully");
		Debug.Log("WTTG2+ Version v1.614");
	}

	private void OnDestroy()
	{
		AssetsManager.AssetsLoaded -= AssetBundleManager.Ins.LoadAssetBundles;
		SceneManager.sceneLoaded -= OnSceneLoaded;
		Debug.Log("[ASoftBootstrapper] Unloaded successfully");
	}

	private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		Debug.Log("[ASoftAPI] OnSceneLoaded: " + scene.name);
		switch (scene.name)
		{
		case "titleScreen":
			InitTitleScreen();
			break;
		case "PreLoadBox":
			if (DifficultyManager.HackerMode)
			{
				GameObject.Find("Content/LoadingScreenIMG").SetActive(value: false);
				GameObject gameObject = GameObject.Find("Content/skullLogo");
				gameObject.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, 120f, 0f);
				gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
				GameObject.Find("Content/Title1").GetComponent<TextMeshProUGUI>().text = "Loading Hacker Mode...";
				GameObject.Find("Content/Title1").GetComponent<TextMeshProUGUI>().fontSize = 48f;
				GameObject.Find("Content/Title2").SetActive(value: false);
			}
			else
			{
				HitmanProxyBehaviour.SetLucasHolmes();
			}
			break;
		case "loadBox":
			if (!DifficultyManager.HackerMode)
			{
				LookUp.DesktopUI.ANN_WINDOW_BROWSER_OBJECT.transform.parent.parent.gameObject.SetActive(value: true);
				Debug.Log("Loaded Game Browser");
				new GameObject("HalloweenEvent").AddComponent<HalloweenEvent>();
			}
			break;
		case "apartment":
			if (!DifficultyManager.HackerMode)
			{
				LookUp.DesktopUI.ANN_WINDOW_BROWSER_OBJECT.transform.parent.parent.gameObject.SetActive(value: false);
			}
			TimeSlinger.FireTimer(PostStageGame, 5f);
			break;
		}
	}

	private static void InitTitleScreen()
	{
		if (!AssetsManager.AssetsReady)
		{
			new GameObject("AssetsManager").AddComponent<AssetsManager>();
		}
		new GameObject("DLLTitleManager").AddComponent<DLLTitleManager>();
	}

	private static void PostStageGame()
	{
		if (!DifficultyManager.HackerMode)
		{
			new GameObject("NewYearEvent").AddComponent<NewYearEvent>();
			new GameObject("RickAstleyFromAliexpress").AddComponent<Rickroller>();
		}
		TwitchManager.Ins.InitDOSTwitch();
	}

	private void FireUp()
	{
		Debug.Log("[ASoftBootstrapper] Initializing client interpolation");
		TimeSlinger.Init();
		RareEvents.Roll();
		EventSlinger.SetEvents();
		new GameObject("UnityMainThreadDispatcher").AddComponent<UnityMainThreadDispatcher>();
		new GameObject("AssetBundleManager").AddComponent<AssetBundleManager>();
		new GameObject("AFDManager").AddComponent<AFDManager>();
		new GameObject("TwitchManager").AddComponent<TwitchManager>();
	}
}
