using Colorful;
using UnityEngine;
using UnityEngine.UI;

public static class AppCreator
{
	public static bool AppsCreated;

	public static GameObject TheSwanAppObject;

	public static GameObject VoIPGameObject;

	public static GameObject VWipeIconObject;

	public static GameObject VWipeCloseBTN;

	public static GameObject CamHooks;

	public static GameObject SecCamsIconObject;

	public static GameObject SignalInterruptionOverlay;

	public static GameObject CamHider;

	public static GameObject CamHider2;

	public static GameObject SecCamera;

	public static Image CamBattery;

	public static GameObject CamTroll;

	public static Image CamImg;

	public static GameObject BotnetAppObject;

	public static GameObject BotNetAppIcon;

	public static GameObject DoorlogAppIcon;

	public static GameObject doorlogApp;

	public static GameObject RouterDoc;

	public static GameObject RouterDocIcon;

	public static GameObject EventPosterObject;

	public static void CreateApps()
	{
		if (!AppsCreated)
		{
			AppsCreated = true;
			WindowManager.Get(SOFTWARE_PRODUCTS.SHADOW_MARKET).Launch();
			WindowManager.Get(SOFTWARE_PRODUCTS.ZERODAY).Launch();
			CreateSwanApp();
			CreateVOIP();
			CreateCamHook();
			CreateSecCamsIcon();
			CreateVWipeIcon();
			VWipeApp.Ins.AddVWipeIcon();
			CreateBotnetApp();
			CreateBotnetAppIcon();
			CreateDoorlogApp();
			CreateDoorlogIcon();
			CreateRouterInfo();
			CreateRouterDocIcon();
			if ((EventSlinger.ChristmasEvent || EventSlinger.EasterEvent || EventSlinger.HalloweenEvent) && !EventSlinger.AprilFoolsEvent)
			{
				SpawnEventWindow();
			}
			if (Themes.selected == THEME.WTTG2BETA)
			{
				GameObject.Find("DOSCoinBTN").transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
				GameObject.Find("BackDoorIMG").transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
				GameObject.Find("MotionSensorButton").transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
				GameObject.Find("SoundButton").transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
				GameObject.Find("PowerButton").transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
			}
			GameObject gameObject = GameObject.Find("ZeroDayMarket");
			GameObject gameObject2 = GameObject.Find("ShadowMarket");
			GameObject gameObject3 = gameObject.transform.Find("ProductsHolder").gameObject;
			GameObject gameObject4 = gameObject2.transform.Find("ProductsHolder").gameObject;
			gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(500f, 660f);
			gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2(500f, 660f);
			gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2(-2f, 616f);
			gameObject3.transform.localPosition = new Vector2(250f, -350.9f);
			gameObject4.GetComponent<RectTransform>().sizeDelta = new Vector2(-2f, 616f);
			gameObject4.transform.localPosition = new Vector2(250f, -350.9f);
			gameObject.AddComponent<GraphicsRayCasterCatcher>();
			gameObject2.AddComponent<GraphicsRayCasterCatcher>();
			gameObject.SetActive(value: false);
			gameObject2.SetActive(value: false);
		}
	}

	public static void CreateSwanApp()
	{
		GameObject gameObject = (TheSwanAppObject = Object.Instantiate(GameObject.Find("ZeroDayMarket")));
		gameObject.transform.SetParent(GameObject.Find("WindowHolder").transform);
		gameObject.name = "TH3SW4NApp";
		gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		gameObject.GetComponent<RectTransform>().sizeDelta = new Vector3(350f, 350f);
		gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(65f, (float)Screen.width * 0.75f), Random.Range(-75f, 0f - (float)Screen.height + 360f));
		Object.Destroy(gameObject.transform.Find("OfflineHolder").gameObject);
		Object.Destroy(gameObject.transform.Find("ProductsHolder").gameObject);
		Object.Destroy(gameObject.transform.Find("TopBar/zeroDayTitle").gameObject);
		Object.Destroy(gameObject.transform.Find("TopBar/TopRightIcons/MinBTN").gameObject);
		Object.Destroy(gameObject.transform.Find("TopBar/TopRightIcons/CloseBTN").gameObject);
		gameObject.transform.Find("TopBar/dragBar").gameObject.GetComponent<DragWindowBehaviour>().MyProduct = SOFTWARE_PRODUCTS.THE_SWAN;
		gameObject.transform.Find("TopBar/dragBar").gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 3f, 1f);
		gameObject.transform.Find("TopBar/dragBar").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(-50f, -60f);
		gameObject.transform.Find("TopBar/dragBar").gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(-80f, 40f);
		new GameObject("SwanAppBehaviour").AddComponent<TheSwanAppBehaviour>();
		gameObject.transform.Find("TopBar/dragBar").gameObject.GetComponent<DragWindowBehaviour>().ReInstantiate();
		TheSwanAppObject = gameObject;
		gameObject.SetActive(value: false);
	}

	public static void CreateVOIP()
	{
		GameObject gameObject = (VoIPGameObject = Object.Instantiate(GameObject.Find("ZeroDayMarket")));
		gameObject.transform.SetParent(GameObject.Find("WindowHolder").transform);
		gameObject.name = "VoIPManagerApp";
		gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		gameObject.GetComponent<RectTransform>().sizeDelta = new Vector3(550f, 709f);
		gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2((float)(Screen.width / 2) - 275f, -115f);
		gameObject.GetComponent<Image>().sprite = CustomSpriteLookUp.voipmanager;
		Object.Destroy(gameObject.transform.Find("OfflineHolder").gameObject);
		Object.Destroy(gameObject.transform.Find("ProductsHolder").gameObject);
		Object.Destroy(gameObject.transform.Find("TopBar/zeroDayTitle").gameObject);
		Object.Destroy(gameObject.transform.Find("TopBar/TopRightIcons/MinBTN").gameObject);
		VWipeCloseBTN = Object.Instantiate(gameObject.transform.Find("TopBar/TopRightIcons/CloseBTN").gameObject, LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.transform);
		VWipeCloseBTN.GetComponent<CloseWindowBehaviour>().MyProduct = SOFTWARE_PRODUCTS.VWIPE;
		VWipeCloseBTN.GetComponent<RectTransform>().anchoredPosition = new Vector2(-27f, 140f);
		VWipeCloseBTN.name = "VWipeClose";
		gameObject.transform.Find("TopBar/TopRightIcons/CloseBTN").gameObject.GetComponent<CloseWindowBehaviour>().MyProduct = SOFTWARE_PRODUCTS.VOIP;
		gameObject.transform.Find("TopBar/dragBar").gameObject.GetComponent<DragWindowBehaviour>().MyProduct = SOFTWARE_PRODUCTS.VOIP;
		gameObject.transform.Find("TopBar/dragBar").gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 3f, 1f);
		gameObject.transform.Find("TopBar/dragBar").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -60f);
		new GameObject("VoIPBehaviour").AddComponent<VoIPBehaviour>();
		gameObject.transform.Find("TopBar/dragBar").gameObject.GetComponent<DragWindowBehaviour>().ReInstantiate();
		gameObject.AddComponent<GraphicsRayCasterCatcher>();
		VoIPGameObject = gameObject;
		gameObject.SetActive(value: false);
	}

	public static void CreateRouterInfo()
	{
		GameObject gameObject = (RouterDoc = Object.Instantiate(GameObject.Find("ZeroDayMarket")));
		gameObject.transform.SetParent(GameObject.Find("WindowHolder").transform);
		gameObject.name = "RouterGamingInfo";
		gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		gameObject.GetComponent<RectTransform>().sizeDelta = new Vector3(704f, 755f);
		gameObject.GetComponent<Image>().sprite = CustomSpriteLookUp.RouterDoc;
		Object.Destroy(gameObject.transform.Find("OfflineHolder").gameObject);
		Object.Destroy(gameObject.transform.Find("ProductsHolder").gameObject);
		Object.Destroy(gameObject.transform.Find("TopBar/zeroDayTitle").gameObject);
		Object.Destroy(gameObject.transform.Find("TopBar/TopRightIcons/MinBTN").gameObject);
		gameObject.transform.Find("TopBar/TopRightIcons/CloseBTN").gameObject.GetComponent<CloseWindowBehaviour>().MyProduct = SOFTWARE_PRODUCTS.ROUTERDOC;
		gameObject.transform.Find("TopBar/dragBar").gameObject.GetComponent<DragWindowBehaviour>().MyProduct = SOFTWARE_PRODUCTS.ROUTERDOC;
		gameObject.transform.Find("TopBar/dragBar").gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 3f, 1f);
		gameObject.transform.Find("TopBar/dragBar").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -60f);
		new GameObject("RouterDocBehaviour").AddComponent<RouterDocBehaviour>();
		gameObject.transform.Find("TopBar/dragBar").gameObject.GetComponent<DragWindowBehaviour>().ReInstantiate();
		gameObject.AddComponent<GraphicsRayCasterCatcher>();
		RouterDoc = gameObject;
		gameObject.SetActive(value: false);
	}

	public static void CreateVWipeIcon()
	{
		GameObject gameObject = Object.Instantiate(GameObject.Find("zeroDayIcon"));
		GameObject gameObject2 = gameObject.transform.Find("titleText1").gameObject;
		GameObject gameObject3 = gameObject.transform.Find("titleText2").gameObject;
		gameObject.transform.SetParent(GameObject.Find("IconsHolder").transform);
		gameObject.name = "vAppIcon";
		gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(-120f, -186f);
		gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		gameObject.GetComponent<iconBehavior>().MyProduct = SOFTWARE_PRODUCTS.VWIPE;
		gameObject.GetComponent<iconBehavior>().DefaultIMG.sprite = ((Themes.selected == THEME.WTTG2BETA) ? ThemesLookUp.WTTG2Beta.vwipe : CustomSpriteLookUp.vwipeIdle);
		gameObject.GetComponent<iconBehavior>().HoverIMG.sprite = ((Themes.selected == THEME.WTTG2BETA) ? ThemesLookUp.WTTG2Beta.vwipe : CustomSpriteLookUp.vwipeActive);
		gameObject2.name = "vAppText1";
		gameObject3.name = "vAppText2";
		gameObject2.GetComponent<Text>().text = "VWipe";
		gameObject3.GetComponent<Text>().text = "VWipe";
		VWipeIconObject = gameObject;
	}

	public static void CreateRouterDocIcon()
	{
		GameObject gameObject = Object.Instantiate(GameObject.Find("NotesIcon"));
		GameObject gameObject2 = gameObject.transform.Find("notesText1").gameObject;
		GameObject gameObject3 = gameObject.transform.Find("notesText2").gameObject;
		gameObject.transform.SetParent(GameObject.Find("IconsHolder").transform);
		gameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
		gameObject.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
		gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
		gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		gameObject.GetComponent<iconBehavior>().MyProduct = SOFTWARE_PRODUCTS.ROUTERDOC;
		gameObject.GetComponent<iconBehavior>().DefaultIMG.sprite = ((Themes.selected == THEME.WTTG2BETA) ? ThemesLookUp.WTTG2Beta.docIcon : CustomSpriteLookUp.routerDocIcon);
		gameObject.GetComponent<iconBehavior>().HoverIMG.sprite = ((Themes.selected == THEME.WTTG2BETA) ? ThemesLookUp.WTTG2Beta.docIconActive : CustomSpriteLookUp.routerDocIconActive);
		gameObject.GetComponent<iconBehavior>().DefaultIMG.GetComponent<RectTransform>().sizeDelta = new Vector2(77f, 77f);
		gameObject.GetComponent<iconBehavior>().HoverIMG.GetComponent<RectTransform>().sizeDelta = new Vector2(77f, 77f);
		gameObject2.GetComponent<Text>().text = " DWR-921";
		gameObject3.GetComponent<Text>().text = " DWR-921";
		RouterDocIcon = gameObject;
		RouterDocIcon.transform.position = new Vector3(Random.Range(15f, (float)Screen.width * 0.9f), 0f - Random.Range(56f, (float)Screen.height - 40f - 15f), 0f);
		bool flag = true;
		gameObject.SetActive(value: false);
	}

	public static void CreateCamHook()
	{
		GameObject gameObject = (CamHooks = Object.Instantiate(GameObject.Find("ZeroDayMarket")));
		gameObject.transform.SetParent(GameObject.Find("WindowHolder").transform);
		gameObject.name = "CamHookApp";
		gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		gameObject.GetComponent<RectTransform>().sizeDelta = new Vector3(600f, 500f);
		gameObject.transform.position = new Vector3(50f, -50f, 0f);
		RenderTexture renderTexture = new RenderTexture(1024, 1024, 16, RenderTextureFormat.ARGB32);
		GameObject gameObject2 = new GameObject("CamHookCamera");
		gameObject2.transform.position = new Vector3(-4.3527f, 41.7599f, -7.5946f);
		gameObject2.transform.rotation = Quaternion.Euler(20.5818f, 42.0364f, 0f);
		gameObject2.AddComponent<Camera>().targetTexture = renderTexture;
		AnalogTV analogTV = gameObject2.AddComponent<AnalogTV>();
		analogTV.ConvertToGrayscale = true;
		analogTV.CubicDistortion = 0f;
		analogTV.Distortion = 0f;
		analogTV.NoiseIntensity = 1f;
		gameObject2.AddComponent<Glitch>().Mode = Glitch.GlitchingMode.Tearing;
		SecCamera = gameObject2;
		gameObject2.SetActive(value: false);
		GameObject gameObject3 = new GameObject("CamRaw");
		gameObject3.AddComponent<RawImage>().texture = renderTexture;
		gameObject3.transform.SetParent(gameObject.transform);
		gameObject3.transform.localPosition = new Vector3(300f, -268f, 0f);
		gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2(597f, 451f);
		Image image = new GameObject("CamBattery").AddComponent<Image>();
		image.sprite = CustomSpriteLookUp.BatteryMeter4;
		image.transform.SetParent(gameObject.transform);
		image.transform.localPosition = new Vector3(40f, -65f, 0f);
		image.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
		image.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 50f);
		CamBattery = image;
		GameObject gameObject4 = new GameObject("CamInterrupted");
		gameObject4.AddComponent<Image>().sprite = CustomSpriteLookUp.NoSignal;
		gameObject4.transform.SetParent(gameObject.transform);
		gameObject4.transform.localPosition = new Vector3(300f, -268f, 0f);
		gameObject4.GetComponent<RectTransform>().sizeDelta = new Vector2(597f, 451f);
		GameObject gameObject5 = new GameObject("CamHider");
		gameObject5.AddComponent<Image>().sprite = CustomSpriteLookUp.NoSignal;
		gameObject5.transform.SetParent(gameObject.transform);
		gameObject5.transform.localPosition = new Vector3(300f, -268f, 0f);
		gameObject5.GetComponent<RectTransform>().sizeDelta = new Vector2(597f, 451f);
		GameObject gameObject6 = new GameObject("CamHider2");
		gameObject6.AddComponent<Image>().sprite = CustomSpriteLookUp.NoSignal;
		gameObject6.transform.SetParent(gameObject.transform);
		gameObject6.transform.localPosition = new Vector3(300f, -268f, 0f);
		gameObject6.GetComponent<RectTransform>().sizeDelta = new Vector2(597f, 451f);
		GameObject gameObject7 = new GameObject("CamTroll");
		CamImg = gameObject7.AddComponent<Image>();
		gameObject7.transform.SetParent(gameObject.transform);
		gameObject7.transform.localPosition = new Vector3(300f, -268f, 0f);
		gameObject7.GetComponent<RectTransform>().sizeDelta = new Vector2(597f, 451f);
		SignalInterruptionOverlay = gameObject4;
		CamHider = gameObject5;
		CamHider2 = gameObject6;
		CamTroll = gameObject7;
		gameObject5.SetActive(value: false);
		gameObject6.SetActive(value: false);
		gameObject7.SetActive(value: false);
		Object.Destroy(gameObject.transform.Find("OfflineHolder").gameObject);
		Object.Destroy(gameObject.transform.Find("ProductsHolder").gameObject);
		Object.Destroy(gameObject.transform.Find("TopBar/TopRightIcons/MinBTN").gameObject);
		gameObject.transform.Find("TopBar/TopRightIcons/CloseBTN").GetComponent<CloseWindowBehaviour>().IgnoreProduct = false;
		gameObject.transform.Find("TopBar/TopRightIcons/CloseBTN").GetComponent<CloseWindowBehaviour>().MyProduct = SOFTWARE_PRODUCTS.CAMHOOK;
		gameObject.transform.Find("TopBar/dragBar").gameObject.GetComponent<DragWindowBehaviour>().MyProduct = SOFTWARE_PRODUCTS.CAMHOOK;
		gameObject.transform.Find("TopBar/zeroDayTitle").GetComponent<Image>().sprite = CustomSpriteLookUp.secksCamsTitle;
		new GameObject("CamHookBehaviour").AddComponent<CamHookBehaviour>();
		gameObject.transform.Find("TopBar/dragBar").gameObject.GetComponent<DragWindowBehaviour>().ReInstantiate();
		gameObject.AddComponent<GraphicRaycaster>();
		gameObject.AddComponent<GraphicsRayCasterCatcher>();
		CamHooks = gameObject;
		CamHookBehaviour.appLaunched = false;
		CamHookBehaviour.Interruptions = false;
		gameObject.SetActive(value: false);
	}

	public static void CreateSecCamsIcon()
	{
		GameObject gameObject = Object.Instantiate(GameObject.Find("NotesIcon"));
		GameObject gameObject2 = gameObject.transform.Find("notesText1").gameObject;
		GameObject gameObject3 = gameObject.transform.Find("notesText2").gameObject;
		gameObject.transform.SetParent(GameObject.Find("IconsHolder").transform);
		gameObject.name = "secCamsIcon";
		gameObject.GetComponent<RectTransform>().anchorMin = Vector2.zero;
		gameObject.GetComponent<RectTransform>().anchorMax = Vector2.zero;
		gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(12f, 192f);
		gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		gameObject.GetComponent<iconBehavior>().MyProduct = SOFTWARE_PRODUCTS.CAMHOOK;
		gameObject.GetComponent<iconBehavior>().DefaultIMG.sprite = ((Themes.selected == THEME.WTTG2BETA) ? ThemesLookUp.WTTG2Beta.seccams : CustomSpriteLookUp.CamHookIdle);
		gameObject.GetComponent<iconBehavior>().HoverIMG.sprite = ((Themes.selected == THEME.WTTG2BETA) ? ThemesLookUp.WTTG2Beta.seccams : CustomSpriteLookUp.CamHookActive);
		gameObject2.name = "SCAppText1";
		gameObject3.name = "SCAppText2";
		gameObject2.GetComponent<Text>().text = "SecCams";
		gameObject3.GetComponent<Text>().text = "SecCams";
		SecCamsIconObject = gameObject;
		bool flag = false;
		gameObject.SetActive(value: false);
	}

	public static void CreateBotnetApp()
	{
		GameObject gameObject = (BotnetAppObject = Object.Instantiate(GameObject.Find("ZeroDayMarket")));
		gameObject.transform.SetParent(GameObject.Find("WindowHolder").transform);
		gameObject.name = "botnetApp";
		gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		gameObject.GetComponent<RectTransform>().sizeDelta = new Vector3(1245f, 800f);
		gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(100f, -100f);
		Object.Destroy(gameObject.transform.Find("OfflineHolder").gameObject);
		Object.Destroy(gameObject.transform.Find("ProductsHolder").gameObject);
		gameObject.transform.Find("TopBar/zeroDayTitle").GetComponent<Image>().sprite = CustomSpriteLookUp.botnetTitle;
		Object.Destroy(gameObject.transform.Find("TopBar/TopRightIcons/MinBTN").gameObject);
		gameObject.transform.Find("TopBar/dragBar").gameObject.GetComponent<DragWindowBehaviour>().MyProduct = SOFTWARE_PRODUCTS.BOTNET;
		new GameObject("BotnetAppBehaviour").AddComponent<BotnetAppBehaviour>();
		gameObject.transform.Find("TopBar/dragBar").gameObject.GetComponent<DragWindowBehaviour>().ReInstantiate();
		gameObject.transform.Find("TopBar/TopRightIcons/CloseBTN").gameObject.GetComponent<CloseWindowBehaviour>().MyProduct = SOFTWARE_PRODUCTS.BOTNET;
		BotnetAppObject = gameObject;
		GameObject gameObject2 = Object.Instantiate(CustomObjectLookUp.botnetAppContent);
		gameObject2.transform.SetParent(gameObject.transform);
		gameObject2.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, -21f, 0f);
		gameObject2.GetComponent<RectTransform>().rotation = Quaternion.Euler(Vector3.zero);
		gameObject2.GetComponent<RectTransform>().localScale = Vector3.one;
		gameObject.AddComponent<GraphicRaycaster>();
		gameObject.AddComponent<GraphicsRayCasterCatcher>();
		gameObject.SetActive(value: false);
	}

	public static void CreateBotnetAppIcon()
	{
		GameObject gameObject = Object.Instantiate(GameObject.Find("NotesIcon"));
		GameObject gameObject2 = gameObject.transform.Find("notesText1").gameObject;
		GameObject gameObject3 = gameObject.transform.Find("notesText2").gameObject;
		gameObject.transform.SetParent(GameObject.Find("IconsHolder").transform);
		gameObject.name = "botNetAppIcon";
		gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(130f, -186f);
		gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		gameObject.GetComponent<iconBehavior>().MyProduct = SOFTWARE_PRODUCTS.BOTNET;
		gameObject.GetComponent<iconBehavior>().DefaultIMG.sprite = ((Themes.selected == THEME.WTTG2BETA) ? ThemesLookUp.WTTG2Beta.botnet : CustomSpriteLookUp.botnetIdle);
		gameObject.GetComponent<iconBehavior>().HoverIMG.sprite = ((Themes.selected == THEME.WTTG2BETA) ? ThemesLookUp.WTTG2Beta.botnet : CustomSpriteLookUp.botnetActive);
		gameObject2.name = "BNAppText1";
		gameObject3.name = "BNAppText2";
		gameObject2.GetComponent<Text>().text = "botnetMNGR";
		gameObject3.GetComponent<Text>().text = "botnetMNGR";
		BotNetAppIcon = gameObject;
		gameObject.SetActive(value: false);
	}

	public static void CreateDoorlogApp()
	{
		GameObject window = WindowManager.Get(SOFTWARE_PRODUCTS.SKYBREAK).Window;
		GameObject gameObject = (doorlogApp = Object.Instantiate(window));
		gameObject.transform.SetParent(window.transform.parent);
		gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(700f, 800f);
		gameObject.transform.Find("topBar/TopRightIcons/CloseBTN").gameObject.GetComponent<CloseWindowBehaviour>().MyProduct = SOFTWARE_PRODUCTS.DOORLOG;
		gameObject.transform.Find("topBar/dragBar").gameObject.GetComponent<DragWindowBehaviour>().MyProduct = SOFTWARE_PRODUCTS.DOORLOG;
		doorlogBehaviour doorlogBehaviour2 = new GameObject("doorlogBehaviour").AddComponent<doorlogBehaviour>();
		gameObject.transform.Find("topBar/dragBar").gameObject.GetComponent<DragWindowBehaviour>().ReInstantiate();
		if (Themes.selected != THEME.WTTG2BETA)
		{
			gameObject.transform.Find("terminalWindowHolder/bg").GetComponent<Image>().color = new Color(0f, 0f, 0.1f, 1f);
		}
		gameObject.transform.Find("topBar/skyBREAKTitle").GetComponent<Image>().sprite = CustomSpriteLookUp.doorLogTitle;
		gameObject.transform.Find("topBar/skyBREAKTitle").GetComponent<Image>().rectTransform.sizeDelta = new Vector2(100f, 21f);
		Object.Destroy(gameObject.transform.Find("topBar/TopRightIcons/MaxBTN").gameObject);
		Object.Destroy(gameObject.transform.Find("topBar/TopRightIcons/MinBTN").gameObject);
		Object.Destroy(gameObject.transform.Find("Resize").gameObject);
		GameObject gameObject2 = gameObject.transform.Find("terminalWindowHolder/TerminalContentHolder/Viewport/ContentBox/terminalLine(Clone)/terminalText").gameObject;
		gameObject2.transform.SetParent(gameObject.transform);
		gameObject2.transform.localPosition = new Vector3(10f, -48f, 0f);
		gameObject2.GetComponent<Text>().text = "";
		gameObject2.GetComponent<Text>().color = Color.white;
		doorlogBehaviour2.theLine = gameObject2;
		Object.Destroy(gameObject.transform.Find("terminalWindowHolder/TerminalContentHolder").gameObject);
		gameObject.AddComponent<GraphicsRayCasterCatcher>();
		if (Themes.selected != THEME.WTTG2BETA)
		{
			window.transform.Find("terminalWindowHolder/bg").GetComponent<Image>().color = ((Themes.selected <= THEME.TWR) ? new Color(0f, 0.05f, 0.1f, 1f) : new Color(0.1f, 0f, 0.1f, 1f));
		}
		window.GetComponent<RectTransform>().sizeDelta = new Vector2(950f, 500f);
		GameObject gameObject3 = new GameObject("lineHolder");
		gameObject3.transform.SetParent(gameObject.transform);
		doorlogBehaviour2.lineHolder = gameObject3;
		VerticalLayoutGroup verticalLayoutGroup = gameObject3.AddComponent<VerticalLayoutGroup>();
		verticalLayoutGroup.padding = new RectOffset(0, 0, 0, 0);
		verticalLayoutGroup.spacing = 0f;
		verticalLayoutGroup.childAlignment = TextAnchor.UpperLeft;
		verticalLayoutGroup.childControlHeight = false;
		verticalLayoutGroup.childControlWidth = true;
		verticalLayoutGroup.childForceExpandHeight = false;
		verticalLayoutGroup.childForceExpandWidth = true;
		gameObject3.transform.localPosition = new Vector3(358f, -98f, 0f);
		gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2(700f, 100f);
	}

	public static void CreateDoorlogIcon()
	{
		GameObject gameObject = Object.Instantiate(GameObject.Find("zeroDayIcon"));
		GameObject gameObject2 = gameObject.transform.Find("titleText1").gameObject;
		GameObject gameObject3 = gameObject.transform.Find("titleText2").gameObject;
		gameObject.transform.SetParent(GameObject.Find("IconsHolder").transform);
		gameObject.name = "doorLog0";
		gameObject.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 0f);
		gameObject.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 0f);
		gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-112f, 192f);
		gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		gameObject.GetComponent<iconBehavior>().MyProduct = SOFTWARE_PRODUCTS.DOORLOG;
		gameObject.GetComponent<iconBehavior>().DefaultIMG.sprite = ((Themes.selected == THEME.WTTG2BETA) ? ThemesLookUp.WTTG2Beta.doorlog : CustomSpriteLookUp.doorLogIdle);
		gameObject.GetComponent<iconBehavior>().HoverIMG.sprite = ((Themes.selected == THEME.WTTG2BETA) ? ThemesLookUp.WTTG2Beta.doorlog : CustomSpriteLookUp.doorLogActive);
		gameObject2.name = "doorLog1";
		gameObject3.name = "doorLog2";
		gameObject2.GetComponent<Text>().text = "doorL0G";
		gameObject3.GetComponent<Text>().text = "doorL0G";
		DoorlogAppIcon = gameObject;
		gameObject.SetActive(value: false);
	}

	public static void SpawnEventWindow()
	{
		GameObject gameObject = (EventPosterObject = Object.Instantiate(GameObject.Find("ZeroDayMarket")));
		gameObject.transform.SetParent(GameObject.Find("WindowHolder").transform);
		gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		gameObject.GetComponent<RectTransform>().sizeDelta = new Vector3(525f, 750f);
		gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2((float)(Screen.width / 2) - 275f, -115f);
		gameObject.GetComponent<Image>().sprite = GetCurrentEventPoster();
		Object.Destroy(gameObject.transform.Find("OfflineHolder").gameObject);
		Object.Destroy(gameObject.transform.Find("ProductsHolder").gameObject);
		Object.Destroy(gameObject.transform.Find("TopBar/zeroDayTitle").gameObject);
		Object.Destroy(gameObject.transform.Find("TopBar/TopRightIcons/MinBTN").gameObject);
		gameObject.transform.Find("TopBar/TopRightIcons/CloseBTN").gameObject.GetComponent<CloseWindowBehaviour>().MyProduct = SOFTWARE_PRODUCTS.EVENTPOSTER;
		gameObject.transform.Find("TopBar/dragBar").gameObject.GetComponent<DragWindowBehaviour>().MyProduct = SOFTWARE_PRODUCTS.EVENTPOSTER;
		gameObject.transform.Find("TopBar/dragBar").gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 3f, 1f);
		gameObject.transform.Find("TopBar/dragBar").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -60f);
		new GameObject("EventPoster").AddComponent<EventPosterBehaviour>();
		gameObject.transform.Find("TopBar/dragBar").gameObject.GetComponent<DragWindowBehaviour>().ReInstantiate();
		gameObject.AddComponent<GraphicsRayCasterCatcher>();
		EventPosterObject = gameObject;
		gameObject.SetActive(EventSlinger.ChristmasEvent || EventSlinger.EasterEvent || EventSlinger.HalloweenEvent);
	}

	private static Sprite GetCurrentEventPoster()
	{
		if (EventSlinger.ChristmasEvent)
		{
			return CustomSpriteLookUp.posterX;
		}
		if (EventSlinger.EasterEvent)
		{
			return CustomSpriteLookUp.posterE;
		}
		if (EventSlinger.HalloweenEvent)
		{
			return CustomSpriteLookUp.posterH;
		}
		return CustomSpriteLookUp.NoSignal;
	}
}
