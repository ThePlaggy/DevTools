using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AssetBundleManager : MonoBehaviour
{
	private static AssetBundle ASOFT;

	private static AssetBundle EVENTS;

	private static AssetBundle WEBSITEDATA;

	private static AssetBundle MISCAUDIO;

	private static AssetBundle BOMBMAKER;

	private static AssetBundle KIDNAPPER;

	private static AssetBundle PROPS;

	private static AssetBundle SPRITE;

	private static AssetBundle RADIO;

	private static AssetBundle LUCASPLUS;

	private static AssetBundle EXECUTIONER;

	private static AssetBundle BREATHERPLUS;

	private static AssetBundle OTREXDEV;

	private static AssetBundle TROPHIES;

	private static AssetBundle DELFALCO;

	private static AssetBundle HACKERMODE;

	private static AssetBundle MENU;

	private static AssetBundle MENU_MUSIC;

	private static AssetBundle MENU_THEMES;

	private static AssetBundle NASKO;

	private static AssetBundle GAME_THEMES;

	private static AssetBundle NOIRPLUS;

	private static AssetBundle CHIPFLAKE;

	public static List<BUNDLE> LoadedBundles = new List<BUNDLE>();

	public static AssetBundleManager Ins;

	public static bool loaded;

	private int AssetBundlesLeftToLoad;

	public static event Action AssetsCached;

	private void Awake()
	{
		Ins = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public void LoadAssetBundles()
	{
		if (!loaded)
		{
			loaded = true;
			ASOFT = AssetsManager.GetLoader(BUNDLE.ASOFT).myProps;
			EVENTS = AssetsManager.GetLoader(BUNDLE.EVENTS).myProps;
			WEBSITEDATA = AssetsManager.GetLoader(BUNDLE.WEBSITEDATA).myProps;
			MISCAUDIO = AssetsManager.GetLoader(BUNDLE.MISCAUDIO).myProps;
			BOMBMAKER = AssetsManager.GetLoader(BUNDLE.BOMBMAKER).myProps;
			KIDNAPPER = AssetsManager.GetLoader(BUNDLE.KIDNAPPER).myProps;
			PROPS = AssetsManager.GetLoader(BUNDLE.PROPS).myProps;
			SPRITE = AssetsManager.GetLoader(BUNDLE.SPRITE).myProps;
			RADIO = AssetsManager.GetLoader(BUNDLE.RADIO).myProps;
			MENU = AssetsManager.GetLoader(BUNDLE.MENU).myProps;
			MENU_THEMES = AssetsManager.GetLoader(BUNDLE.MENU_THEMES).myProps;
			MENU_MUSIC = AssetsManager.GetLoader(BUNDLE.MENU_MUSIC).myProps;
			LUCASPLUS = AssetsManager.GetLoader(BUNDLE.LUCASPLUS).myProps;
			EXECUTIONER = AssetsManager.GetLoader(BUNDLE.EXECUTIONER).myProps;
			BREATHERPLUS = AssetsManager.GetLoader(BUNDLE.BREATHERPLUS).myProps;
			OTREXDEV = AssetsManager.GetLoader(BUNDLE.OTREXDEV).myProps;
			TROPHIES = AssetsManager.GetLoader(BUNDLE.TROPHIES).myProps;
			DELFALCO = AssetsManager.GetLoader(BUNDLE.DELFALCO).myProps;
			NOIRPLUS = AssetsManager.GetLoader(BUNDLE.NOIRPLUS).myProps;
			HACKERMODE = AssetsManager.GetLoader(BUNDLE.HACKERMODE).myProps;
			NASKO = AssetsManager.GetLoader(BUNDLE.NASKO).myProps;
			GAME_THEMES = AssetsManager.GetLoader(BUNDLE.GAME_THEMES).myProps;
			CHIPFLAKE = AssetsManager.GetLoader(BUNDLE.GAME_THEMES).myProps;
			AssetBundlesLeftToLoad = 23;
			StartCoroutine(AssetUnpackingComplete());
			StartCoroutine(LoadASoft());
			StartCoroutine(LoadEvents());
			StartCoroutine(LoadWebsiteData());
			StartCoroutine(LoadMiscAudio());
			StartCoroutine(LoadBombmaker());
			StartCoroutine(LoadKidnapper());
			StartCoroutine(LoadProps());
			StartCoroutine(LoadSprite());
			StartCoroutine(LoadRadio());
			StartCoroutine(LoadMenu());
			StartCoroutine(LoadGameThemes());
			StartCoroutine(LoadMenuMusic());
			StartCoroutine(LoadMenuThemes());
			StartCoroutine(LoadLucasPlus());
			StartCoroutine(LoadExecutioner());
			StartCoroutine(LoadBreatherPlus());
			StartCoroutine(LoadOtrexDev());
			StartCoroutine(LoadTrophies());
			StartCoroutine(LoadDelfalco());
			StartCoroutine(LoadNoirPlus());
			StartCoroutine(LoadHackerMode());
			StartCoroutine(LoadNasko());
            StartCoroutine(LoadChipflake());
        }
	}

	private IEnumerator AssetUnpackingComplete()
	{
		Debug.Log("[AssetBundleManager] Unpacking bundles...");
		while (AssetBundlesLeftToLoad > 0)
		{
			yield return null;
			if (AssetBundlesLeftToLoad <= 0)
			{
				Debug.Log("[AssetBundleManager] All asset bundles are unpacked!");
				Debug.Log("[AssetBundleManager] Unloading asset bundles...");
				AssetBundle.UnloadAllAssetBundles(unloadAllObjects: false);
				Debug.Log("[AssetBundleManager] Asset bundles unloaded");
				TitleManager.Ins.LoadMusic();
				yield return new WaitForSeconds(1f);
				AssetBundleManager.AssetsCached?.Invoke();
				Debug.Log("[AssetsManager] Assets Loaded");
			}
		}
	}

	private IEnumerator LoadAssetAsync<T>(AssetBundle AB, string assetName, Action<T> callback)
	{
		AssetBundleRequest req = AB.LoadAssetAsync<T>(assetName);
		yield return req;
		callback?.Invoke((T)(object)req.asset);
	}

	private IEnumerator LoadASoft()
	{
		yield return LoadAssetAsync(ASOFT, "TwitchPopUp", delegate(GameObject result)
		{
			CustomObjectLookUp.TwitchPopUp = result;
		});
		yield return LoadAssetAsync(ASOFT, "CultMaleDance", delegate(GameObject result)
		{
			CustomObjectLookUp.CultMaleDance = result;
		});
		yield return LoadAssetAsync(ASOFT, "CultFemaleDance", delegate(GameObject result)
		{
			CustomObjectLookUp.CultFemaleDance = result;
		});
		yield return LoadAssetAsync(ASOFT, "TheBombMaker", delegate(GameObject result)
		{
			CustomObjectLookUp.TheBombMaker = result;
		});
		yield return LoadAssetAsync(ASOFT, "TheTanner", delegate(GameObject result)
		{
			CustomObjectLookUp.TheTanner = result;
		});
		yield return LoadAssetAsync(ASOFT, "NotifyCard", delegate(GameObject result)
		{
			CustomObjectLookUp.NotifyCard = result;
		});
		yield return LoadAssetAsync(ASOFT, "TheSyringe", delegate(GameObject result)
		{
			CustomObjectLookUp.TheSyringe = result;
		});
		yield return LoadAssetAsync(ASOFT, "ManipulatorHolder", delegate(GameObject result)
		{
			CustomObjectLookUp.ManipulatorHolder = result;
		});
		yield return LoadAssetAsync(ASOFT, "BM_TitleAC", delegate(RuntimeAnimatorController result)
		{
			CustomObjectLookUp.BM_TitleAC = result;
		});
		yield return LoadAssetAsync(ASOFT, "TannerTitleAC", delegate(RuntimeAnimatorController result)
		{
			CustomObjectLookUp.TannerTitleAC = result;
		});
		yield return LoadAssetAsync(ASOFT, "SyringeTitleAC", delegate(RuntimeAnimatorController result)
		{
			CustomObjectLookUp.SyringeTitleAC = result;
		});
		yield return LoadAssetAsync(ASOFT, "CustomTannerAC", delegate(RuntimeAnimatorController result)
		{
			CustomObjectLookUp.CustomTannerAC = result;
		});
		yield return LoadAssetAsync(ASOFT, "Tanner@T-PoseAvatar", delegate(Avatar result)
		{
			CustomObjectLookUp.TannerTPoseAvatar = result;
		});
		yield return LoadAssetAsync(ASOFT, "WebsiteBoostIdle", delegate(Sprite result)
		{
			CustomSpriteLookUp.WebBoostIdle = result;
		});
		yield return LoadAssetAsync(ASOFT, "WebsiteBoostNegative", delegate(Sprite result)
		{
			CustomSpriteLookUp.WebBoostNegative = result;
		});
		yield return LoadAssetAsync(ASOFT, "WebsiteBoostPositive", delegate(Sprite result)
		{
			CustomSpriteLookUp.WebBoostPositive = result;
		});
		yield return LoadAssetAsync(ASOFT, "KeyCueIdle", delegate(Sprite result)
		{
			CustomSpriteLookUp.KeyCueIdle = result;
		});
		yield return LoadAssetAsync(ASOFT, "KeyCueNegative", delegate(Sprite result)
		{
			CustomSpriteLookUp.KeyCueNegative = result;
		});
		yield return LoadAssetAsync(ASOFT, "KeyCuePositive", delegate(Sprite result)
		{
			CustomSpriteLookUp.KeyCuePositive = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(ASOFT, "TannerJump", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.TannerJump = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(ASOFT, "NeedleInject", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.NeedleInject = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(ASOFT, "HeadFloorHit", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.HeadFloorHit = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(ASOFT, "BodyHit", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.BodyHit = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(ASOFT, "Window2", AFD_TYPE.ENEMY, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.Window2 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(ASOFT, "Tanner_Laugh", AFD_TYPE.ENEMY, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.Tanner_Laugh = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(ASOFT, "Having", AFD_TYPE.ENEMY, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.Having = result;
		});
		Debug.Log("[AssetBundleManager] ASOFT unpacking completed");
		AssetBundlesLeftToLoad--;
	}

	private IEnumerator LoadEvents()
	{
		yield return LoadAssetAsync(EVENTS, "PumpkinJack", delegate(GameObject result)
		{
			CustomObjectLookUp.PumpkinJack = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(EVENTS, "FireworksMain", AFD_TYPE.OUTSIDE, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.FireworksMain = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(EVENTS, "FireworksLong1", AFD_TYPE.OUTSIDE, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.FireworksLong1 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(EVENTS, "FireworksQuick1", AFD_TYPE.OUTSIDE, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.FireworksQuick1 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(EVENTS, "FireworksQuick2", AFD_TYPE.OUTSIDE, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.FireworksQuick2 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(EVENTS, "FireworksQuick3", AFD_TYPE.OUTSIDE, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.FireworksQuick3 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(EVENTS, "FireworksQuick4", AFD_TYPE.OUTSIDE, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.FireworksQuick4 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(EVENTS, "FireworksQuick5", AFD_TYPE.OUTSIDE, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.FireworksQuick5 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(EVENTS, "FireworksQuick6", AFD_TYPE.OUTSIDE, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.FireworksQuick6 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(EVENTS, "WitchLaugh", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.WitchLaugh = result;
		});
		Debug.Log("[AssetBundleManager] EVENTS unpacking completed");
		AssetBundlesLeftToLoad--;
	}

	private IEnumerator LoadWebsiteData()
	{
		AssetBundleRequest webpageRequest = WEBSITEDATA.LoadAllAssetsAsync<WebPageDefinition>();
		AssetBundleRequest websiteRequest = WEBSITEDATA.LoadAllAssetsAsync<WebSiteDefinition>();
		yield return webpageRequest;
		WebsiteLookUp.webpageDefinitions = webpageRequest.allAssets.Cast<WebPageDefinition>().ToList();
		yield return websiteRequest;
		WebsiteLookUp.websiteDefinitions = websiteRequest.allAssets.Cast<WebSiteDefinition>().ToList();
		yield return LoadAssetAsync(WEBSITEDATA, "bug1", delegate(Sprite result)
		{
			CustomSpriteLookUp.bug1 = result;
		});
		yield return LoadAssetAsync(WEBSITEDATA, "bug2", delegate(Sprite result)
		{
			CustomSpriteLookUp.bug2 = result;
		});
		yield return LoadAssetAsync(WEBSITEDATA, "bug3", delegate(Sprite result)
		{
			CustomSpriteLookUp.bug3 = result;
		});
		yield return LoadAssetAsync(WEBSITEDATA, "bug4", delegate(Sprite result)
		{
			CustomSpriteLookUp.bug4 = result;
		});
		yield return LoadAssetAsync(WEBSITEDATA, "bug5", delegate(Sprite result)
		{
			CustomSpriteLookUp.bug5 = result;
		});
		yield return LoadAssetAsync(WEBSITEDATA, "bug6", delegate(Sprite result)
		{
			CustomSpriteLookUp.bug6 = result;
		});
		Debug.Log("[AssetBundleManager] WEBSITEDATA unpacking completed");
		AssetBundlesLeftToLoad--;
	}

	private IEnumerator LoadMiscAudio()
	{
		yield return AFDManager.Ins.CreateAFDAsync(MISCAUDIO, "alarm", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.alarm = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(MISCAUDIO, "beep", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.beep = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(MISCAUDIO, "disappear", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.disappear = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(MISCAUDIO, "failsafe", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.failsafe = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(MISCAUDIO, "fool", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.fool = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(MISCAUDIO, "thefool2", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.fool2 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(MISCAUDIO, "thefool3", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.fool3 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(MISCAUDIO, "glyphClick", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.glyphClick = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(MISCAUDIO, "KeyFound2", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.keyFound2 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(MISCAUDIO, "keypad_Click", AFD_TYPE.PLAYER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.keypad_Click = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(MISCAUDIO, "keypad_Correct", AFD_TYPE.PLAYER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.keypad_Correct = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(MISCAUDIO, "keypad_Unlock", AFD_TYPE.PLAYER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.keypad_Unlock = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(MISCAUDIO, "keypad_Wrong", AFD_TYPE.PLAYER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.keypad_Wrong = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(MISCAUDIO, "manipulator", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.manipulator = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(MISCAUDIO, "pull", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.pull = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(MISCAUDIO, "reset", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.reset = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(MISCAUDIO, "routerjammed", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.routerjammed = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(MISCAUDIO, "routerreset", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.routerreset = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(MISCAUDIO, "strike", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.strike = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(MISCAUDIO, "swanKeyPress", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.swanKeyPress = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(MISCAUDIO, "systemFailure", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.systemFailure = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(MISCAUDIO, "timechange", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.timechange = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(MISCAUDIO, "party", AFD_TYPE.PLAYER, 1f, Loop: true, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.party = result;
		});
		Debug.Log("[AssetBundleManager] MISCAUDIO unpacking completed");
		AssetBundlesLeftToLoad--;
	}

	private IEnumerator LoadBombmaker()
	{
		yield return LoadAssetAsync(BOMBMAKER, "BombMakerHallwayJump.prefab", delegate(GameObject result)
		{
			CustomObjectLookUp.BombMakerHallwayJump = result;
		});
		yield return LoadAssetAsync(BOMBMAKER, "BombMakerApartmentJump.prefab", delegate(GameObject result)
		{
			CustomObjectLookUp.BombMakerApartmentJump = result;
		});
		yield return LoadAssetAsync(BOMBMAKER, "BombMakerPCKill.prefab", delegate(GameObject result)
		{
			CustomObjectLookUp.BombMakerPCKill = result;
		});
		yield return LoadAssetAsync(BOMBMAKER, "BombMakerPresence.prefab", delegate(GameObject result)
		{
			CustomObjectLookUp.BombMakerPresence = result;
		});
		yield return LoadAssetAsync(BOMBMAKER, "BombMakerRecolorer.prefab", delegate(GameObject result)
		{
			CustomObjectLookUp.BombMakerRecolorer = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(BOMBMAKER, "bombmaker", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.bombmaker = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(BOMBMAKER, "bombmakertalk", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.bombmakertalk = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(BOMBMAKER, "explosion", AFD_TYPE.PLAYER, 0.65f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.explosion = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(BOMBMAKER, "youreuseless", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.youreuseless = result;
		});
		Debug.Log("[AssetBundleManager] BOMBMAKER unpacking completed");
		AssetBundlesLeftToLoad--;
	}

	private IEnumerator LoadKidnapper()
	{
		yield return LoadAssetAsync(KIDNAPPER, "Kidnapper", delegate(GameObject result)
		{
			CustomObjectLookUp.Kidnapper = result;
		});
		yield return LoadAssetAsync(KIDNAPPER, "KidnapperTitle", delegate(GameObject result)
		{
			CustomObjectLookUp.KidnapperTitle = result;
		});
		yield return LoadAssetAsync(KIDNAPPER, "motionAlert 1", delegate(Sprite result)
		{
			CustomSpriteLookUp.locationServicesBuyIcon = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(KIDNAPPER, "kidnapperjump", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.kidnapperjump = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(KIDNAPPER, "kidnapperline", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.kidnapperline = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(KIDNAPPER, "punch", AFD_TYPE.PLAYER, 0.45f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.punch = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(KIDNAPPER, "footStepMR1", AFD_TYPE.PLAYER, 0.25f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.KidnapperWarning1 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(KIDNAPPER, "footStepMR2", AFD_TYPE.PLAYER, 0.25f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.KidnapperWarning2 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(KIDNAPPER, "muffle1", AFD_TYPE.PLAYER, 0.25f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.KidnapperWarning3 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(KIDNAPPER, "OutSideFootStep1", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.KidnapperFootstep1 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(KIDNAPPER, "OutSideFootStep2", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.KidnapperFootstep2 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(KIDNAPPER, "OutSideFootStep3", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.KidnapperFootstep3 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(KIDNAPPER, "theSource", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.TheSource = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(KIDNAPPER, "nothingHere", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.NothingHere = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(KIDNAPPER, "locationservices", AFD_TYPE.COMPUTER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.LocationServices = result;
		});
		Debug.Log("[AssetBundleManager] KIDNAPPER unpacking completed");
		AssetBundlesLeftToLoad--;
	}

	private IEnumerator LoadProps()
	{
		yield return LoadAssetAsync(PROPS, "PackageBox.prefab", delegate(GameObject result)
		{
			CustomObjectLookUp.PackageBox = result;
		});
		yield return LoadAssetAsync(PROPS, "Router.prefab", delegate(GameObject result)
		{
			CustomObjectLookUp.Router = result;
		});
		yield return LoadAssetAsync(PROPS, "TarotCards.prefab", delegate(GameObject result)
		{
			CustomObjectLookUp.TarotCards = result;
		});
		yield return LoadAssetAsync(PROPS, "TH3SW4N.prefab", delegate(GameObject result)
		{
			CustomObjectLookUp.TH3SW4N = result;
		});
		yield return LoadAssetAsync(PROPS, "BeerFor504.prefab", delegate(GameObject result)
		{
			CustomObjectLookUp.BeerFor504 = result;
		});
		yield return LoadAssetAsync(PROPS, "OverrideInfo.prefab", delegate(GameObject result)
		{
			CustomObjectLookUp.OverrideInfo = result;
		});
		yield return LoadAssetAsync(PROPS, "OverrideInfoNM.prefab", delegate(GameObject result)
		{
			CustomObjectLookUp.OverrideInfoNM = result;
		});
		yield return LoadAssetAsync(PROPS, "ElevatorCanvas.prefab", delegate(GameObject result)
		{
			CustomObjectLookUp.ElevatorCanvas = result;
		});
		yield return LoadAssetAsync(PROPS, "Elevator Buttons Holder.prefab", delegate(GameObject result)
		{
			CustomObjectLookUp.EBH = result;
		});
		yield return LoadAssetAsync(PROPS, "ElectricLightManager.prefab", delegate(GameObject result)
		{
			CustomObjectLookUp.ELM = result;
		});
		yield return LoadAssetAsync(PROPS, "dude", delegate(GameObject result)
		{
			CustomObjectLookUp.dude = result;
		});
		yield return LoadAssetAsync(PROPS, "Gamer Rage AS", delegate(GameObject result)
		{
			CustomObjectLookUp.GamerRage = result;
		});
		yield return LoadAssetAsync(PROPS, "WiFi Crash Rage AS", delegate(GameObject result)
		{
			CustomObjectLookUp.WiFiRage = result;
		});
		yield return LoadAssetAsync(PROPS, "harbinger", delegate(AudioClip result)
		{
			CustomSoundLookUp.harbinger = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(PROPS, "SFX_GlassBreak", AFD_TYPE.PLAYER, 5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.booooom = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(PROPS, "blipp", AFD_TYPE.COMPUTER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.newDOSDrainer = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(PROPS, "diing", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.elevatorDing = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(PROPS, "elestart", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.elevatorStart = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(PROPS, "elevatorded", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.elevatorBreak = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(PROPS, "coffeetime", AFD_TYPE.PLAYER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.coffeetime = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(PROPS, "beer504", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.nyam = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(PROPS, "welcometothegametwoplus.wav", AFD_TYPE.COMPUTER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.welcometothegametwoplus = result;
		});
		yield return LoadAssetAsync(PROPS, "wttg2PlusLogo", delegate(Sprite result)
		{
			CustomSpriteLookUp.wttg2PlusLogo = result;
		});
		Debug.Log("[AssetBundleManager] PROPS unpacking completed");
		AssetBundlesLeftToLoad--;
	}

	private IEnumerator LoadSprite()
	{
		yield return LoadAssetAsync(SPRITE, "remoteVPNlvl2.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.remoteVPNlvl2 = result;
		});
		yield return LoadAssetAsync(SPRITE, "remoteVPNlvl3.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.remoteVPNlvl3 = result;
		});
		yield return LoadAssetAsync(SPRITE, "speeditem.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.speeditem = result;
		});
		yield return LoadAssetAsync(SPRITE, "sulphur.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.sulphur = result;
		});
		yield return LoadAssetAsync(SPRITE, "router.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.router = result;
		});
		yield return LoadAssetAsync(SPRITE, "tarotcard.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.tarotcard = result;
		});
		yield return LoadAssetAsync(SPRITE, "voipmanager.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.voipmanager = result;
		});
		yield return LoadAssetAsync(SPRITE, "adosicon.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.adosicon = result;
		});
		yield return LoadAssetAsync(SPRITE, "NoSignal", delegate(Sprite result)
		{
			CustomSpriteLookUp.NoSignal = result;
		});
		yield return LoadAssetAsync(SPRITE, "CamIconIdle.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.CamHookIdle = result;
		});
		yield return LoadAssetAsync(SPRITE, "CamIconActive.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.CamHookActive = result;
		});
		yield return LoadAssetAsync(SPRITE, "secCamsTitle.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.secksCamsTitle = result;
		});
		yield return LoadAssetAsync(SPRITE, "seccamsStore.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.seckscamsStore = result;
		});
		yield return LoadAssetAsync(SPRITE, "BatteryMeter0.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.BatteryMeter0 = result;
		});
		yield return LoadAssetAsync(SPRITE, "BatteryMeter1.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.BatteryMeter1 = result;
		});
		yield return LoadAssetAsync(SPRITE, "BatteryMeter2.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.BatteryMeter2 = result;
		});
		yield return LoadAssetAsync(SPRITE, "BatteryMeter3.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.BatteryMeter3 = result;
		});
		yield return LoadAssetAsync(SPRITE, "BatteryMeter4.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.BatteryMeter4 = result;
		});
		yield return LoadAssetAsync(SPRITE, "binclosed", delegate(Sprite result)
		{
			CustomSpriteLookUp.binclosed = result;
		});
		yield return LoadAssetAsync(SPRITE, "binopened", delegate(Sprite result)
		{
			CustomSpriteLookUp.binopened = result;
		});
		yield return LoadAssetAsync(SPRITE, "dicidle.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.dicidle = result;
		});
		yield return LoadAssetAsync(SPRITE, "dicoutline.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.dicoutline = result;
		});
		yield return LoadAssetAsync(SPRITE, "megalock.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.megalock = result;
		});
		yield return LoadAssetAsync(SPRITE, "megaunlock.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.megaunlock = result;
		});
		yield return LoadAssetAsync(SPRITE, "doorLogIcon.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.doorLogIdle = result;
		});
		yield return LoadAssetAsync(SPRITE, "doorLogIcon_active.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.doorLogActive = result;
		});
		yield return LoadAssetAsync(SPRITE, "doorLogTitle.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.doorLogTitle = result;
		});
		yield return LoadAssetAsync(SPRITE, "RouterDoc.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.RouterDoc = result;
		});
		yield return LoadAssetAsync(SPRITE, "routerDocIcon.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.routerDocIcon = result;
		});
		yield return LoadAssetAsync(SPRITE, "routerDocIconActive.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.routerDocIconActive = result;
		});
		yield return LoadAssetAsync(SPRITE, "strongflashlight.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.strongflashlight = result;
		});
		yield return LoadAssetAsync(SPRITE, "doorLogStoreIcon.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.doorLogStore = result;
		});
		yield return LoadAssetAsync(SPRITE, "KeypadStore.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.KeypadStore = result;
		});
		yield return LoadAssetAsync(SPRITE, "botnetStore.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.botnetStore = result;
		});
		yield return LoadAssetAsync(SPRITE, "DOSCoin", delegate(Sprite result)
		{
			SpriteLookUp.DOSCoin = result;
		});
		yield return LoadAssetAsync(SPRITE, "skyBreak", delegate(Sprite result)
		{
			SpriteLookUp.skyBreak = result;
		});
		yield return LoadAssetAsync(SPRITE, "skyBreakWPA", delegate(Sprite result)
		{
			SpriteLookUp.skyBreakWPA = result;
		});
		yield return LoadAssetAsync(SPRITE, "skyBreakWPA2", delegate(Sprite result)
		{
			SpriteLookUp.skyBreakWPA2 = result;
		});
		yield return LoadAssetAsync(SPRITE, "backDoorHack", delegate(Sprite result)
		{
			SpriteLookUp.backDoorHack = result;
		});
		yield return LoadAssetAsync(SPRITE, "motionAlert", delegate(Sprite result)
		{
			SpriteLookUp.motionAlert = result;
		});
		yield return LoadAssetAsync(SPRITE, "keyCue", delegate(Sprite result)
		{
			SpriteLookUp.keyCue = result;
		});
		yield return LoadAssetAsync(SPRITE, "wifiDongleLevel2", delegate(Sprite result)
		{
			SpriteLookUp.wifiDongleLevel2 = result;
		});
		yield return LoadAssetAsync(SPRITE, "wifiDongleLevel3", delegate(Sprite result)
		{
			SpriteLookUp.wifiDongleLevel3 = result;
		});
		yield return LoadAssetAsync(SPRITE, "remoteVPN", delegate(Sprite result)
		{
			SpriteLookUp.remoteVPN = result;
		});
		yield return LoadAssetAsync(SPRITE, "motionSensor", delegate(Sprite result)
		{
			SpriteLookUp.motionSensor = result;
		});
		yield return LoadAssetAsync(SPRITE, "policeScanner", delegate(Sprite result)
		{
			SpriteLookUp.policeScanner = result;
		});
		yield return LoadAssetAsync(SPRITE, "lolpyDisc", delegate(Sprite result)
		{
			SpriteLookUp.lolpyDisc = result;
		});
		yield return LoadAssetAsync(SPRITE, "orangeBG.png", delegate(Sprite result)
		{
			ThemesLookUp.WTTG2.orangeBG = result;
		});
		yield return LoadAssetAsync(SPRITE, "orangeBGF.png", delegate(Sprite result)
		{
			ThemesLookUp.WTTG2.orangeBGF = result;
		});
		yield return LoadAssetAsync(SPRITE, "blueBG.png", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.blueBG = result;
		});
		yield return LoadAssetAsync(SPRITE, "blueBGF.png", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.blueBGF = result;
		});
		yield return LoadAssetAsync(SPRITE, "annBackActive", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.AnnBackBTNActive = result;
		});
		yield return LoadAssetAsync(SPRITE, "annForwardActive", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.AnnForwardBTNActive = result;
		});
		yield return LoadAssetAsync(SPRITE, "annBookmarksActive", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.AnnBookmarksBTNActive = result;
		});
		yield return LoadAssetAsync(SPRITE, "annRefreshActive", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.AnnRefreshBTNActive = result;
		});
		yield return LoadAssetAsync(SPRITE, "annCodeActive", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.AnnSourceBTNActive = result;
		});
		yield return LoadAssetAsync(SPRITE, "annHomeActive", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.AnnHomeBTNActive = result;
		});
		Debug.Log("[AssetBundleManager] SPRITE unpacking completed");
		AssetBundlesLeftToLoad--;
	}

	private IEnumerator LoadRadio()
	{
		yield return LoadAssetAsync(RADIO, "1.ogg", delegate(AudioClip result)
		{
			RadioLookUp.anonyjazz[0] = result;
		});
		yield return LoadAssetAsync(RADIO, "2.ogg", delegate(AudioClip result)
		{
			RadioLookUp.anonyjazz[1] = result;
		});
		yield return LoadAssetAsync(RADIO, "3.ogg", delegate(AudioClip result)
		{
			RadioLookUp.anonyjazz[2] = result;
		});
		yield return LoadAssetAsync(RADIO, "4.ogg", delegate(AudioClip result)
		{
			RadioLookUp.anonyjazz[3] = result;
		});
		yield return LoadAssetAsync(RADIO, "5.ogg", delegate(AudioClip result)
		{
			RadioLookUp.anonyjazz[4] = result;
		});
		yield return LoadAssetAsync(RADIO, "1b.ogg", delegate(AudioClip result)
		{
			RadioLookUp.dsbm[0] = result;
		});
		yield return LoadAssetAsync(RADIO, "2b.ogg", delegate(AudioClip result)
		{
			RadioLookUp.dsbm[1] = result;
		});
		yield return LoadAssetAsync(RADIO, "3b.ogg", delegate(AudioClip result)
		{
			RadioLookUp.dsbm[2] = result;
		});
		yield return LoadAssetAsync(RADIO, "4b.ogg", delegate(AudioClip result)
		{
			RadioLookUp.dsbm[3] = result;
		});
		yield return LoadAssetAsync(RADIO, "5b.ogg", delegate(AudioClip result)
		{
			RadioLookUp.dsbm[4] = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(RADIO, "cameraSwitch", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.cameraSwitch = result;
		});
		Debug.Log("[AssetBundleManager] RADIO unpacking completed");
		AssetBundlesLeftToLoad--;
	}

	private IEnumerator LoadMenu()
	{
		yield return LoadAssetAsync(MENU, "NewMenuCanvas", delegate(GameObject result)
		{
			CustomObjectLookUp.newMenuCanvas = result;
		});
		Debug.Log("[AssetBundleManager] MENU unpacking completed");
		AssetBundlesLeftToLoad--;
	}

    private IEnumerator LoadChipflake()
    {
        yield return LoadAssetAsync(CHIPFLAKE, "chipflake", delegate (GameObject result)
        {
            CustomObjectLookUp.chipflake = result;
        });
        yield return AFDManager.Ins.CreateAFDAsync(CHIPFLAKE, "wegsound", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate (AudioFileDefinition result)
        {
            CustomSoundLookUp.wegsound = result;
        });
        yield return AFDManager.Ins.CreateAFDAsync(CHIPFLAKE, "spawnsound", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate (AudioFileDefinition result)
        {
            CustomSoundLookUp.spawnsound = result;
        });
        Debug.Log("[AssetBundleManager] CHIPFLAKE unpacking completed");
        AssetBundlesLeftToLoad--;
    }

    private IEnumerator LoadMenuThemes()
	{
		yield return LoadAssetAsync(MENU_THEMES, "WTTG2Scenery", delegate(GameObject result)
		{
			CustomObjectLookUp.wttg2TitleScenery = result;
		});
		yield return LoadAssetAsync(MENU_THEMES, "WTTG1Scenery", delegate(GameObject result)
		{
			CustomObjectLookUp.wttg1TitleScenery = result;
		});
		Debug.Log("[AssetBundleManager] MENU_THEMES unpacking completed");
		AssetBundlesLeftToLoad--;
	}

	private IEnumerator LoadMenuMusic()
	{
		yield return LoadAssetAsync(MENU_MUSIC, "wttg_title", delegate(AudioClip result)
		{
			MusicLookUp.WTTG1Music = result;
		});
		yield return LoadAssetAsync(MENU_MUSIC, "twr_title", delegate(AudioClip result)
		{
			MusicLookUp.TWRMusic = result;
		});
		yield return LoadAssetAsync(MENU_MUSIC, "twr_easter_title", delegate(AudioClip result)
		{
			MusicLookUp.TWRSecretMusic = result;
		});
		yield return LoadAssetAsync(MENU_MUSIC, "scrutinized_title", delegate(AudioClip result)
		{
			MusicLookUp.ScrutinizedMusic = result;
		});
		yield return LoadAssetAsync(MENU_MUSIC, "deadSignal_title", delegate(AudioClip result)
		{
			MusicLookUp.DeadSignalMusic = result;
		});
		yield return LoadAssetAsync(MENU_MUSIC, "tls_title", delegate(AudioClip result)
		{
			MusicLookUp.TheLastSightMusic = result;
		});
		Debug.Log("[AssetBundleManager] MENU_MUSIC unpacking completed");
		AssetBundlesLeftToLoad--;
	}

	private IEnumerator LoadGameThemes()
	{
		yield return LoadAssetAsync(GAME_THEMES, "botnet", delegate(Sprite result)
		{
			ThemesLookUp.WTTG2Beta.botnet = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "doorLog", delegate(Sprite result)
		{
			ThemesLookUp.WTTG2Beta.doorlog = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "seccams", delegate(Sprite result)
		{
			ThemesLookUp.WTTG2Beta.seccams = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "skyBreak", delegate(Sprite result)
		{
			ThemesLookUp.WTTG2Beta.skybreak = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "wttg2betaWallpaper", delegate(Sprite result)
		{
			ThemesLookUp.WTTG2Beta.wallpaper = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "vwipe", delegate(Sprite result)
		{
			ThemesLookUp.WTTG2Beta.vwipe = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "wttg2betaNightmare", delegate(Sprite result)
		{
			ThemesLookUp.WTTG2Beta.nightmareWallpaper = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "shadowMarket", delegate(Sprite result)
		{
			ThemesLookUp.WTTG2Beta.shadowMarket = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "zeroDay", delegate(Sprite result)
		{
			ThemesLookUp.WTTG2Beta.zeroDay = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "assets/themes/beta/docicon.png", delegate(Sprite result)
		{
			ThemesLookUp.WTTG2Beta.docIcon = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "docIconActive", delegate(Sprite result)
		{
			ThemesLookUp.WTTG2Beta.docIconActive = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "baseAppTransBG", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.baseapptrans = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "botBarBG", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.botBar = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "topBarBG", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.topBar = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "redd", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.wttg1nightmare = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "heartbeat3", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.twr_nightmare = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "twr", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.twr_wallpaper = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "minAppTabHover", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.MinAppTabHover = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "minBTNInactive", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.minBTNinactive = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "minBTNActive", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.minBTNactive = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "MaxBTNInactive", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.maxBTNinactive = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "MaxBTNActive", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.maxBTNactive = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "UnMaxBTNInactive", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.unmaxBTNinactive = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "UnMaxBTNActive", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.unmaxBTNactive = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "closeBTNInactive", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.closeBTNinactive = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "closeBTNActive", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.closeBTNactive = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "terminalIcon", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.terminal = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "terminalIconActive", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.terminalActive = result;
		});
		yield return LoadAssetAsync(GAME_THEMES, "baseAppBotnetBG", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.botnetapp = result;
		});
		Debug.Log("[AssetBundleManager] GAME_THEMES unpacking completed");
		AssetBundlesLeftToLoad--;
	}

	private IEnumerator LoadLucasPlus()
	{
		yield return LoadAssetAsync(LUCASPLUS, "HitmanTest", delegate(GameObject result)
		{
			CustomObjectLookUp.HitmanTest = result;
		});
		yield return LoadAssetAsync(LUCASPLUS, "BalconyJumpAnimator", delegate(RuntimeAnimatorController result)
		{
			CustomObjectLookUp.BalconyJumpAnimator = result;
		});
		yield return LoadAssetAsync(LUCASPLUS, "idiot", delegate(AudioClip result)
		{
			CustomSoundLookUp.idiot_1 = result;
		});
		yield return LoadAssetAsync(LUCASPLUS, "comeout", delegate(AudioClip result)
		{
			CustomSoundLookUp.comeout = result;
		});
		Debug.Log("[AssetBundleManager] LUCASPLUS unpacking completed");
		AssetBundlesLeftToLoad--;
	}

	private IEnumerator LoadExecutioner()
	{
		yield return LoadAssetAsync(EXECUTIONER, "ExecutionerPrefab", delegate(GameObject result)
		{
			CustomObjectLookUp.ExecutionerPrefab = result;
		});
		yield return LoadAssetAsync(EXECUTIONER, "ExecutionerCustomRig", delegate(GameObject result)
		{
			CustomObjectLookUp.ExecutionerCustomRig = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(EXECUTIONER, "jump", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.exeJump = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(EXECUTIONER, "Punch1", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.exePunch1 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(EXECUTIONER, "hallDoorOpenClose", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.hallDoorOpenClose = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(EXECUTIONER, "whistlingReverb", AFD_TYPE.PLAYER, 0.85f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.whistlingReverb = result;
		});
		Debug.Log("[AssetBundleManager] EXECUTIONER unpacking completed");
		AssetBundlesLeftToLoad--;
	}

	private IEnumerator LoadBreatherPlus()
	{
		yield return LoadAssetAsync(BREATHERPLUS, "BreatherPhone.prefab", delegate(GameObject result)
		{
			CustomObjectLookUp.BreatherPhone = result;
		});
		yield return LoadAssetAsync(BREATHERPLUS, "NightNightBreather.prefab", delegate(GameObject result)
		{
			CustomObjectLookUp.NightNightBreather = result;
		});
		yield return LoadAssetAsync(BREATHERPLUS, "DancingBreatherSecret.prefab", delegate(GameObject result)
		{
			CustomObjectLookUp.BreatherSecret = result;
		});
		yield return LoadAssetAsync(BREATHERPLUS, "Floor8Breather.prefab", delegate(GameObject result)
		{
			CustomObjectLookUp.Floor8Breather = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(BREATHERPLUS, "answerPhone", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.answerPhone = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(BREATHERPLUS, "cellPhoneRing", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.cellPhoneRing = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(BREATHERPLUS, "hangUpPhone", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.hangUpPhone = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(BREATHERPLUS, "phoneVibrate", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.phoneVibrate = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(BREATHERPLUS, "breathe1", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.breather_breathe1 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(BREATHERPLUS, "breathe2", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.breather_breathe2 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(BREATHERPLUS, "gettingcloser", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.breather_gettingcloser = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(BREATHERPLUS, "ifoundyou", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.breather_ifoundyou = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(BREATHERPLUS, "imcomingforyou", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.breather_imcomingforyou = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(BREATHERPLUS, "knockknock", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.breather_knockknock = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(BREATHERPLUS, "ryhme", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.breather_rhyme = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(BREATHERPLUS, "hearyou", AFD_TYPE.PLAYER, 0.3f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.breather_hearyou = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(BREATHERPLUS, "seeyou", AFD_TYPE.PLAYER, 0.3f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.breather_seeyou = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(BREATHERPLUS, "NightNight", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.breather_nightnight = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(BREATHERPLUS, "wttg1_laugh", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.wttg1_laugh = result;
		});
		Debug.Log("[AssetBundleManager] BREATHERPLUS unpacking completed");
		AssetBundlesLeftToLoad--;
	}

	private IEnumerator LoadOtrexDev()
	{
		yield return LoadAssetAsync(OTREXDEV, "NoirChips.prefab", delegate(GameObject result)
		{
			CustomObjectLookUp.NoirChips = result;
		});
		yield return LoadAssetAsync(OTREXDEV, "Shitman.prefab", delegate(GameObject result)
		{
			CustomObjectLookUp.Shitman = result;
		});
		yield return LoadAssetAsync(OTREXDEV, "Chipman.prefab", delegate(GameObject result)
		{
			CustomObjectLookUp.Chipman = result;
		});
		yield return LoadAssetAsync(OTREXDEV, "BotnetAppContent", delegate(GameObject result)
		{
			CustomObjectLookUp.botnetAppContent = result;
		});
		yield return LoadAssetAsync(OTREXDEV, "SecksDevKeypad", delegate(GameObject result)
		{
			CustomObjectLookUp.SecksDevKeypad = result;
		});
		yield return LoadAssetAsync(OTREXDEV, "SecksDevKeypadCanvas", delegate(GameObject result)
		{
			CustomObjectLookUp.gaypadCanvas = result;
		});
		yield return LoadAssetAsync(OTREXDEV, "CollectReward", delegate(GameObject result)
		{
			CustomObjectLookUp.christmasReward = result;
		});
		yield return LoadAssetAsync(OTREXDEV, "ChristmasStuff", delegate(GameObject result)
		{
			CustomObjectLookUp.christmasStuff = result;
		});
		yield return LoadAssetAsync(OTREXDEV, "SecCam", delegate(GameObject result)
		{
			CustomObjectLookUp.SecCam = result;
		});
		yield return LoadAssetAsync(OTREXDEV, "FireworksBox", delegate(GameObject result)
		{
			CustomObjectLookUp.fireworkBox = result;
		});
		yield return LoadAssetAsync(OTREXDEV, "botnet_active", delegate(Sprite result)
		{
			CustomSpriteLookUp.botnetActive = result;
		});
		yield return LoadAssetAsync(OTREXDEV, "botnet_idle", delegate(Sprite result)
		{
			CustomSpriteLookUp.botnetIdle = result;
		});
		yield return LoadAssetAsync(OTREXDEV, "botnetTitle.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.botnetTitle = result;
		});
		yield return LoadAssetAsync(OTREXDEV, "computerIcon", delegate(Sprite result)
		{
			CustomSpriteLookUp.computerIcon = result;
		});
		yield return LoadAssetAsync(OTREXDEV, "laptopIcon", delegate(Sprite result)
		{
			CustomSpriteLookUp.laptopIcon = result;
		});
		yield return LoadAssetAsync(OTREXDEV, "KeypadIcon.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.I_Keypad = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(OTREXDEV, "gamePass", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.GamePass = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(OTREXDEV, "gameFail", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.GameFail = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(OTREXDEV, "validCodeInput", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.ValidInput = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(OTREXDEV, "invalidCodeInput", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.InvalidInput = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(OTREXDEV, "highLightCode", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.highLightCode = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(OTREXDEV, "KeyReturn", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.KeyReturn = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(OTREXDEV, "KAPowerUp", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.KernelPowerUp = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(OTREXDEV, "omnomnomnomnom", AFD_TYPE.PLAYER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.omnom = result;
		});
		Debug.Log("[AssetBundleManager] OTREXDEV unpacking completed");
		AssetBundlesLeftToLoad--;
	}

	private IEnumerator LoadTrophies()
	{
		yield return LoadAssetAsync(TROPHIES, "TrophyCase", delegate(GameObject result)
		{
			CustomObjectLookUp.TrophyCase = result;
		});
		yield return LoadAssetAsync(TROPHIES, "egg", delegate(Sprite result)
		{
			CustomSpriteLookUp.eggIcon = result;
		});
		yield return LoadAssetAsync(TROPHIES, "pumpkin", delegate(Sprite result)
		{
			CustomSpriteLookUp.pumpkinIcon = result;
		});
		yield return LoadAssetAsync(TROPHIES, "gift", delegate(Sprite result)
		{
			CustomSpriteLookUp.giftIcon = result;
		});
		Debug.Log("[AssetBundleManager] TROPHIES unpacking completed");
		AssetBundlesLeftToLoad--;
	}

	private IEnumerator LoadDelfalco()
	{
		yield return AFDManager.Ins.CreateAFDAsync(DELFALCO, "SFX_DelfalcoJumpB", AFD_TYPE.PLAYER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.delfalcoChaseJump = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(DELFALCO, "SFX_MrDelfalco_ThereYouAre", AFD_TYPE.PLAYER, 2f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.delfalcoChaseLine = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(DELFALCO, "SFX_DelfalcoChaseJump", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.delfalcoChaseJumpB = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(DELFALCO, "SFX_DelfalcoWhistle", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.delfalcoWhistle = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(DELFALCO, "delfalcoBehindJump", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.delfalcoBehindJump = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(DELFALCO, "SFX_Delfalco_IKnowYouAreHere", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.delfalcoPatrolVoice1 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(DELFALCO, "SFX_Delfalco_IWillFindYou", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.delfalcoPatrolVoice2 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(DELFALCO, "SFX_Delfalco_WhereAreYou", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.delfalcoPatrolVoice3 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(DELFALCO, "SFX_Delfalco_YouCanHide", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.delfalcoPatrolVoice4 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(DELFALCO, "SFX_DelfalcoJumpA", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.delfalcoJumpA = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(DELFALCO, "SFX_Delfalco_SawFleshA", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.delfalcoSawFlesh = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(DELFALCO, "SFX_Delfalco_SawHit", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.delfalcoSawHit = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(DELFALCO, "SFX_Delfalco_SawFlip", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.delfalcoSawFlip = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(DELFALCO, "SFX_Delfalco_SawOut", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.delfalcoSawOut = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(DELFALCO, "SFX_Delfalco_Swipe", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.delfalcoSawSwipe = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(DELFALCO, "SFX_KneeFall", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.delfalcoFall = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(DELFALCO, "SFX_BluntHit", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.delfalcoHit = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(DELFALCO, "SFX_Delfalco_Shh", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.delfalcoShh = result;
		});
		yield return LoadAssetAsync(DELFALCO, "Delfalco", delegate(GameObject result)
		{
			CustomObjectLookUp.Delfalco = result;
		});
		yield return LoadAssetAsync(DELFALCO, "CustomElevator", delegate(GameObject result)
		{
			CustomObjectLookUp.CustomElevator = result;
		});
		Debug.Log("[AssetBundleManager] DELFALCO unpacking completed");
		AssetBundlesLeftToLoad--;
	}

	private IEnumerator LoadNoirPlus()
	{
		yield return AFDManager.Ins.CreateAFDAsync(NOIRPLUS, "SFX_NoirFemale_ThatsIt", AFD_TYPE.ENEMY, 0.3f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.ComeCloser = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(NOIRPLUS, "SFX_NoirFemale_NotThatClose", AFD_TYPE.ENEMY, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.NotThatClose = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(NOIRPLUS, "SFX_NoirFemale_DontMove", AFD_TYPE.PLAYER, 0.8f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.DontMove = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(NOIRPLUS, "SFX_NoirFemale_Boo", AFD_TYPE.ENEMY, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.Boo = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(NOIRPLUS, "SFX_noirFemale_Laugh", AFD_TYPE.ENEMY, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.NoirLaugh = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(NOIRPLUS, "SFX_Noir_JumpHitB", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.MaleNoirJump = result;
		});
		yield return LoadAssetAsync(NOIRPLUS, "FemaleNoir", delegate(GameObject result)
		{
			CustomObjectLookUp.NewFemaleNoir = result;
		});
		yield return LoadAssetAsync(NOIRPLUS, "MaleNoir", delegate(GameObject result)
		{
			CustomObjectLookUp.NewMaleNoir = result;
		});
		Debug.Log("[AssetBundleManager] NOIRPLUS unpacking completed");
		AssetBundlesLeftToLoad--;
	}

	private IEnumerator LoadHackerMode()
	{
		yield return LoadAssetAsync(HACKERMODE, "HMMenu", delegate(GameObject result)
		{
			CustomObjectLookUp.HackerModeMenu = result;
		});
		yield return LoadAssetAsync(HACKERMODE, "vapeAttack", delegate(GameObject result)
		{
			CustomObjectLookUp.vapeAttack = result;
		});
		yield return LoadAssetAsync(HACKERMODE, "vapeHolder", delegate(GameObject result)
		{
			CustomObjectLookUp.vapeHolder = result;
		});
		yield return LoadAssetAsync(HACKERMODE, "DOSAttack", delegate(GameObject result)
		{
			CustomObjectLookUp.DOSAttack = result;
		});
		yield return LoadAssetAsync(HACKERMODE, "DOSHolder", delegate(GameObject result)
		{
			CustomObjectLookUp.DOSHolder = result;
		});
		yield return LoadAssetAsync(HACKERMODE, "HMGameOver", delegate(GameObject result)
		{
			CustomObjectLookUp.hackerModeGameOver = result;
		});
		yield return LoadAssetAsync(HACKERMODE, "VWipeButton", delegate(GameObject result)
		{
			CustomObjectLookUp.VWipeButton = result;
		});
		yield return LoadAssetAsync(HACKERMODE, "adios", delegate(Sprite result)
		{
			CustomSpriteLookUp.adios = result;
		});
		yield return LoadAssetAsync(HACKERMODE, "locIcon", delegate(Sprite result)
		{
			CustomSpriteLookUp.locIcon = result;
		});
		yield return LoadAssetAsync(HACKERMODE, "vwipeActive", delegate(Sprite result)
		{
			CustomSpriteLookUp.vwipeActive = result;
		});
		yield return LoadAssetAsync(HACKERMODE, "vwipeIdle", delegate(Sprite result)
		{
			CustomSpriteLookUp.vwipeIdle = result;
		});
		yield return LoadAssetAsync(HACKERMODE, "HackerModeMenuMusic", delegate(AudioClip result)
		{
			CustomSoundLookUp.HackerModeMenuMusic = result;
		});
		yield return LoadAssetAsync(HACKERMODE, "Hackermode", delegate(AudioClip result)
		{
			CustomSoundLookUp.hackermode = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "vapeBlockHover", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.boxNodeHoverClip = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "vapeBlockActive", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.boxNodeActiveClip = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "vapeBlankHover", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.blankNodeHoverClip = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "vapeBlockGood", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.goodNodeActiveClip = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "DOSCountDownTick1", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.CountDownTick1 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "DOSCountDownTick2", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.CountDownTick2 = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "KAlmostUp", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.ClockAlmostUp = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "DOSNodeHot", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.NodeHot = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "DOSNodeCold", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.NodeCold = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "DOSActionNodeActive", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.NodeActive = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "DOSExitNodeActive", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.ExitNodeActive = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "DOSAttackClick", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.ActionNodeClick = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "gameOver", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.gameOver = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "gameOverTrigger", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.gameOverTrigger = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "TotalPointSlide", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.totalPointSlide = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "TimeShow", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.timeShow = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "FinalPointsImpact", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.finalPointsImpact = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "newHighScoreSFX", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.newHighScore = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "Selection", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.Selection = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "denied", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.Denied = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "1piano-a", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			ClaffisLookUp.piano[0] = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "2piano-b", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			ClaffisLookUp.piano[1] = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "3piano-c", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			ClaffisLookUp.piano[2] = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "4piano-d", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			ClaffisLookUp.piano[3] = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "5piano-e", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			ClaffisLookUp.piano[4] = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "6piano-f", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			ClaffisLookUp.piano[5] = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "7piano-g", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			ClaffisLookUp.piano[6] = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "8piano-f", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			ClaffisLookUp.piano[7] = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "9piano-eb", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			ClaffisLookUp.piano[8] = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "10piano-c", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			ClaffisLookUp.piano[9] = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "11piano-bb", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			ClaffisLookUp.piano[10] = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "12piano-g", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			ClaffisLookUp.piano[11] = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "pianoFail", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			ClaffisLookUp.pianoFail = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(HACKERMODE, "MysteryPiano", AFD_TYPE.COMPUTER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			ClaffisLookUp.MysteryPiano = result;
		});
		yield return LoadAssetAsync(HACKERMODE, "annWindowBG", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.annapp = result;
		});
		yield return LoadAssetAsync(HACKERMODE, "baseAppBG", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.baseapp = result;
		});
		yield return LoadAssetAsync(HACKERMODE, "wallPaper", delegate(Sprite result)
		{
			ThemesLookUp.WTTG1TWR.wttg1wallpaper = result;
		});
		Debug.Log("[AssetBundleManager] HACKERMODE unpacking completed");
		AssetBundlesLeftToLoad--;
	}

	private IEnumerator LoadNasko()
	{
		yield return LoadAssetAsync(NASKO, "EggHolder", delegate(GameObject result)
		{
			CustomObjectLookUp.EggHolder = result;
		});
		yield return LoadAssetAsync(NASKO, "CandyBox", delegate(GameObject result)
		{
			CustomObjectLookUp.CandyBox = result;
		});
		yield return LoadAssetAsync(NASKO, "Magic fire pro orange.prefab", delegate(GameObject result)
		{
			CustomObjectLookUp.MagicFireOrangeXProMax = result;
		});
		yield return LoadAssetAsync(NASKO, "halloweenposter.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.posterH = result;
		});
		yield return LoadAssetAsync(NASKO, "easterposter.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.posterE = result;
		});
		yield return LoadAssetAsync(NASKO, "xmasposter.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.posterX = result;
		});
		yield return LoadAssetAsync(NASKO, "LOLpaper.png", delegate(Sprite result)
		{
			CustomSpriteLookUp.LOLpaper = result;
		});
		yield return LoadAssetAsync(NASKO, "rickastleymyboy.png", delegate(Texture2D result)
		{
			CustomSpriteLookUp.rickastleymaboi = result;
		});
		yield return LoadAssetAsync(NASKO, "VACation.png", delegate(Texture2D result)
		{
			CustomSpriteLookUp.TheRealVacation = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(NASKO, "getcalculated", AFD_TYPE.PLAYER, 0.47f, Loop: true, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.getcalculated = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(NASKO, "tadaaa", AFD_TYPE.PLAYER, 1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.tadaaa = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(NASKO, "owl", AFD_TYPE.PLAYER, 0.5f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.owl = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(NASKO, "packagePickUp", AFD_TYPE.PLAYER, 0.3f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.candyPickup = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(NASKO, "ignited", AFD_TYPE.PLAYER, 0.1f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.ignited = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(NASKO, "WoodClick", AFD_TYPE.PLAYER, 0.2f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.WoodClick = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(NASKO, "YoutubeAudioLibrary.ogg", AFD_TYPE.PLAYER, 0.55f, Loop: false, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.jingleBELLS = result;
		});
		yield return AFDManager.Ins.CreateAFDAsync(NASKO, "realVAC.wav", AFD_TYPE.PLAYER, 0.55f, Loop: true, delegate(AudioFileDefinition result)
		{
			CustomSoundLookUp.TheRealVacation = result;
		});
		yield return LoadAssetAsync(NASKO, "deadsignal", delegate(AudioClip result)
		{
			CustomSoundLookUp.deadsignal = result;
		});
		yield return LoadAssetAsync(NASKO, "fireplace", delegate(AudioClip result)
		{
			CustomSoundLookUp.fireplace = result;
		});
		Debug.Log("[AssetBundleManager] NASKO unpacking completed");
		AssetBundlesLeftToLoad--;
	}
}