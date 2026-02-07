using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
	public static EventManager Ins;

	public static bool ElevatorOpened;

	private int PresentsPickedUp;

	public static AudioSource DSAS;

	private Text xmasCounter;

	private void Awake()
	{
		Ins = this;
		if (!DifficultyManager.HackerMode)
		{
			CheckForEvents();
		}
	}

	private void XmasCounter()
	{
		GameObject gameObject = Object.Instantiate(GameObject.Find("UI/UIComputer/DesktopUI/TopBar/TopLeftIconHolder/BackDoorIMG"), GameObject.Find("DesktopUI/TopBar/TopLeftIconHolder").transform);
		gameObject.transform.position = new Vector3(273f, -8f, 19f);
		gameObject.GetComponent<Image>().sprite = CustomSpriteLookUp.giftIcon;
		GameObject gameObject2 = Object.Instantiate(GameObject.Find("DesktopUI/TopBar/TopLeftIconHolder/CurrentBackDoors"), GameObject.Find("DesktopUI/TopBar/TopLeftIconHolder").transform);
		gameObject2.transform.position = new Vector3(327f, -20.5f, 19f);
		gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 25f);
		Object.Destroy(gameObject2.GetComponent<backdoorTextHook>());
		xmasCounter = gameObject2.GetComponent<Text>();
		xmasCounter.text = "0/10";
	}

	public void PresentWasPickedUp()
	{
		PresentsPickedUp++;
		xmasCounter.text = $"{PresentsPickedUp}/10";
		if (PresentsPickedUp == 10)
		{
			Object.Instantiate(CustomObjectLookUp.christmasReward, new Vector3(0.162f, 40.536f, 1.193f), Quaternion.Euler(0f, 0f, 0f));
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.fool);
		}
	}

	public void AddEventRewards(string EventName, bool Money, bool WiFi, bool Discount, bool Powerups, bool RandomKey, bool ItemWhitehat, bool Disco = false, Color DiscoColor = default(Color))
	{
		if (Money)
		{
			DOSCoinsCurrencyManager.AddCurrency(150f);
			GameManager.HackerManager.WhiteHatSound();
		}
		if (WiFi)
		{
			for (int i = 0; i < 16; i++)
			{
				WifiInteractionManager.UnlockTheWiFi(-3);
			}
			WifiMenuBehaviour.Ins.refreshNetworks();
		}
		if (Discount)
		{
			WindowManager.Get(SOFTWARE_PRODUCTS.ZERODAY).Launch();
			if (!ZeroDayProductObject.isDiscountOn)
			{
				for (int j = 0; j < GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts.Count; j++)
				{
					GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[j].myProductObject.DiscountMe();
				}
			}
			WindowManager.Get(SOFTWARE_PRODUCTS.SHADOW_MARKET).Launch();
			if (!ShadowProductObject.isDiscountOn)
			{
				for (int k = 0; k < GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts.Count; k++)
				{
					GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[k].myProductObject.DiscountMe();
				}
			}
		}
		if (Disco)
		{
			EnvironmentManager.PowerBehaviour.DOSetNewLightColor(DiscoColor, 1f);
		}
		if (Powerups)
		{
			SpeedPoll.DevEnableManipulator(TWITCH_NET_SPEED.FAST);
			KeyPoll.DevEnableManipulator(KEY_CUE_MODE.ENABLED);
		}
		if (RandomKey)
		{
			int num = Random.Range(0, 8) + 1;
			GameManager.ManagerSlinger?.TextDocManager.CreateTextDoc($"Key{num}.txt", $"{num} - {GameManager.TheCloud.MasterKey.Substring(12 * (num - 1), 12)}");
			GameManager.AudioSlinger?.PlaySound(LookUp.SoundLookUp.KeyFound);
		}
		if (ItemWhitehat)
		{
			ItemWhitehats.GiveItemWhitehat(FromTwitch: false);
		}
		switch (EventName)
		{
		case "Christmas":
			TrophyManager.Ins.christmasTrophy.SetActive(value: true);
			PlayerPrefs.SetInt("EventXMASTrophy", 1);
			break;
		case "Easter":
			TrophyManager.Ins.easterTrophy.SetActive(value: true);
			PlayerPrefs.SetInt("EventEasterTrophy", 1);
			break;
		case "Halloween":
			TrophyManager.Ins.halloweenTrophy.SetActive(value: true);
			PlayerPrefs.SetInt("EventHalloTrophy", 1);
			break;
		}
	}

	public void CheckForEvents()
	{
		if (EventSlinger.EasterEvent)
		{
			ActivateEasterEvent();
		}
		if (EventSlinger.HalloweenEvent)
		{
			ActivateHalloweenEvent();
		}
		if (EventSlinger.ChristmasEvent)
		{
			ActivateChristmasEvent();
		}
		if (DifficultyManager.CasualMode || EventSlinger.EasterEvent)
		{
			Elevatorin();
		}
	}

	private void ActivateChristmasEvent()
	{
		Debug.Log("[Events] Christmas event active!");
		GameManager.TimeSlinger.FireTimer(5.5f, delegate
		{
			Object.Instantiate(CustomObjectLookUp.christmasStuff, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
			GameObject.Find("CoffeNotePad").GetComponent<InteractionHook>().ForceLock = true;
			string[] array = new string[10] { "ApartmentStructure/Static/MainRoom/coffeeTable", "Folder1", "Folder2", "Folder3", "paper", "picture", "pen4", "TvRemote", "Mug1", "ThumbDrive4" };
			foreach (string text in array)
			{
				GameObject.Find(text).SetActive(value: false);
			}
		});
		XmasCounter();
	}

	private void ActivateHalloweenEvent()
	{
		Debug.Log("[Events] Halloween event active!");
	}

	private void ActivateEasterEvent()
	{
		if (!EventSlinger.AprilFoolsEvent)
		{
			Debug.Log("[Events] Easter event active!");
			new GameObject("eggs").AddComponent<EasterEggManager>();
		}
	}

	public static void Elevatorin()
	{
		if (!ElevatorOpened)
		{
			CustomElevatorManager.Ins.OpenMyDoor();
			ElevatorOpened = true;
			GameObject gameObject = new GameObject("Dead Signal Audio Source");
			gameObject.transform.position = new Vector3(-27.935f, 40.93f, -6.31f);
			gameObject.transform.SetParent(CustomElevatorManager.Ins.myElevator.transform);
			DSAS = gameObject.AddComponent<AudioSource>();
			DSAS.clip = CustomSoundLookUp.deadsignal;
			DSAS.minDistance = 0.2f;
			DSAS.maxDistance = 0.2f;
			DSAS.dopplerLevel = 0f;
			DSAS.spatialBlend = 1f;
			DSAS.loop = true;
			DSAS.Play();
			GameManager.PauseManager.GamePaused += DSAS0;
			GameManager.PauseManager.GameUnPaused += DSAS1;
			StateManager.PlayerLocationChangeEvents.Event += DSAS1;
		}
	}

	private void OnDestroy()
	{
		GameManager.PauseManager.GamePaused -= DSAS0;
		GameManager.PauseManager.GameUnPaused -= DSAS1;
		StateManager.PlayerLocationChangeEvents.Event -= DSAS1;
	}

	private static void DSAS0()
	{
		DSAS.volume = 0f;
	}

	private static void DSAS1()
	{
		if (!CustomElevatorManager.KillDSAS)
		{
			DSAS.volume = ((!GameManager.PauseManager.Paused && (StateManager.PlayerLocation == PLAYER_LOCATION.UNKNOWN || StateManager.PlayerLocation == PLAYER_LOCATION.HALL_WAY1 || StateManager.PlayerLocation == PLAYER_LOCATION.HALL_WAY3 || StateManager.PlayerLocation == PLAYER_LOCATION.HALL_WAY5 || StateManager.PlayerLocation == PLAYER_LOCATION.HALL_WAY6 || StateManager.PlayerLocation == PLAYER_LOCATION.HALL_WAY8 || StateManager.PlayerLocation == PLAYER_LOCATION.HALL_WAY10 || StateManager.PlayerLocation == PLAYER_LOCATION.LOBBY)) ? 1f : 0f);
		}
	}
}
