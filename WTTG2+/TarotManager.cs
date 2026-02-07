using UnityEngine;

public class TarotManager : MonoBehaviour
{
	public static bool HermitActive;

	public static int TimeController = 30;

	public static bool BreatherUndertaker;

	public static TarotManager Ins;

	public static playerSpeedMode CurSpeed = playerSpeedMode.NORMAL;

	public static string[] tappedSites;

	public static bool DeafActive;

	public static bool InDenial;

	public static int Scripted;

	private Timer sunmoonTimer;

	private Timer quickweakTimer;

	private void Awake()
	{
		Ins = this;
		TarotCardPullAnim.MAX_TAROT_CARDS = 30;
		TarotCardPullAnim.AlreadyGotTarot = new bool[TarotCardPullAnim.MAX_TAROT_CARDS];
	}

	private void Update()
	{
		if (HermitActive)
		{
			HermitTrap();
		}
	}

	private void HermitTrap()
	{
		if (StateManager.PlayerLocation != PLAYER_LOCATION.MAIN_ROON && StateManager.PlayerLocation != PLAYER_LOCATION.BATH_ROOM && StateManager.PlayerLocation != PLAYER_LOCATION.OUTSIDE && StateManager.PlayerLocation != PLAYER_LOCATION.UNKNOWN)
		{
			ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).transform.position = new Vector3(0.10953f, 40.51757f, -1.304061f);
		}
	}

	public void PullTarotCard(TAROT_CARDS card)
	{
		switch (card)
		{
		case TAROT_CARDS.THE_PRO:
			if (Random.Range(0, 2) == 0)
			{
				if (KeyPoll.keyManipulatorData == KEY_CUE_MODE.DEFAULT)
				{
					KeyPoll.DevEnableManipulator(KEY_CUE_MODE.ENABLED);
					break;
				}
			}
			else if (!SpeedPoll.speedManipulatorActive)
			{
				SpeedPoll.DevEnableManipulator(TWITCH_NET_SPEED.FAST);
				break;
			}
			if (Random.Range(0, 2) == 0)
			{
				WifiInteractionManager.WiFiPassword(-2);
				GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.KeyFound);
			}
			else
			{
				WifiInteractionManager.UnlockTheWiFi(-2);
				WifiMenuBehaviour.Ins.refreshNetworks();
			}
			break;
		case TAROT_CARDS.THE_NOOB:
			if (Random.Range(0, 2) == 0)
			{
				if (KeyPoll.keyManipulatorData == KEY_CUE_MODE.DEFAULT)
				{
					KeyPoll.DevEnableManipulator(KEY_CUE_MODE.DISABLED);
					break;
				}
			}
			else if (!SpeedPoll.speedManipulatorActive)
			{
				SpeedPoll.DevEnableManipulator(TWITCH_NET_SPEED.SLOW);
				break;
			}
			if (Random.Range(0, 2) == 0)
			{
				if (!WiFiPoll.lastConnectedWifi.affectedByDosDrainer)
				{
					WiFiPoll.lastConnectedWifi.affectedByDosDrainer = true;
				}
				else
				{
					WifiInteractionManager.LockTheWiFi(-2);
				}
			}
			else
			{
				WifiInteractionManager.LockTheWiFi(-1);
				WifiMenuBehaviour.Ins.refreshNetworks();
			}
			break;
		case TAROT_CARDS.THE_SUN:
			if (TimeController == 30)
			{
				TimeController = 60;
			}
			else if (TimeController == 5)
			{
				TimeController = 30;
			}
			GameManager.TimeSlinger.KillTimer(sunmoonTimer);
			GameManager.TimeSlinger.FireHardTimer(out sunmoonTimer, 300f, delegate
			{
				TimeController = 30;
			});
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.timechange);
			break;
		case TAROT_CARDS.THE_MOON:
			if (TimeController == 30)
			{
				TimeController = 5;
			}
			else if (TimeController == 60)
			{
				TimeController = 30;
			}
			GameManager.TimeSlinger.KillTimer(sunmoonTimer);
			GameManager.TimeSlinger.FireHardTimer(out sunmoonTimer, 300f, delegate
			{
				TimeController = 30;
			});
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.timechange);
			break;
		case TAROT_CARDS.THE_RICH:
			DOSCoinsCurrencyManager.AddCurrency(Random.Range(3.33f, 166.6f));
			GameManager.HackerManager.WhiteHatSound();
			break;
		case TAROT_CARDS.THE_POOR:
			DOSCoinsCurrencyManager.RemoveCurrencyBypassNegative(Random.Range(3.33f, 99.9f));
			GameManager.HackerManager.BlackHatSound();
			break;
		case TAROT_CARDS.THE_CURSED:
		{
			if (Random.Range(0, 100) < 10 && !GameManager.HackerManager.theSwan.isActivatedBefore)
			{
				GameManager.HackerManager.theSwan.ActivateTheSwan();
				break;
			}
			for (int j = 0; j < Random.Range(1, 5); j++)
			{
				GameManager.HackerManager.virusManager.ForceVirus();
			}
			break;
		}
		case TAROT_CARDS.THE_GAMBLER:
			if (Random.Range(0, 2) == 0)
			{
				DOSCoinsCurrencyManager.AddCurrency(DOSCoinsCurrencyManager.CurrentCurrency);
				GameManager.HackerManager.WhiteHatSound();
			}
			else
			{
				DOSCoinsCurrencyManager.RemoveCurrency(DOSCoinsCurrencyManager.CurrentCurrency);
				GameManager.HackerManager.BlackHatSound();
			}
			break;
		case TAROT_CARDS.THE_HACKER:
			if (Random.Range(0, 2) == 0)
			{
				GameManager.HackerManager.ForcePogHack();
			}
			else
			{
				GameManager.HackerManager.ForceNormalHack();
			}
			break;
		case TAROT_CARDS.THE_DEVIL:
			if (Random.Range(0, 2) == 0)
			{
				WindowManager.Get(SOFTWARE_PRODUCTS.ZERODAY).Launch();
				if (!ZeroDayProductObject.isDiscountOn)
				{
					for (int num2 = 0; num2 < GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts.Count; num2++)
					{
						GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[num2].myProductObject.DiscountMe();
					}
				}
				break;
			}
			WindowManager.Get(SOFTWARE_PRODUCTS.SHADOW_MARKET).Launch();
			if (!ShadowProductObject.isDiscountOn)
			{
				for (int num3 = 0; num3 < GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts.Count; num3++)
				{
					GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[num3].myProductObject.DiscountMe();
				}
			}
			break;
		case TAROT_CARDS.THE_UNDERTAKER:
			if (!BreatherUndertaker)
			{
				BreatherUndertaker = true;
				GameManager.TimeSlinger.FireTimer(600f, delegate
				{
					BreatherUndertaker = false;
				});
			}
			else
			{
				BreatherManager.InvisiblePerson = false;
			}
			break;
		case TAROT_CARDS.THE_QUICK:
			if (CurSpeed == playerSpeedMode.WEAK)
			{
				CurSpeed = playerSpeedMode.NORMAL;
				break;
			}
			CurSpeed = playerSpeedMode.QUICK;
			GameManager.TimeSlinger.KillTimer(quickweakTimer);
			GameManager.TimeSlinger.FireHardTimer(out quickweakTimer, 180f, delegate
			{
				CurSpeed = playerSpeedMode.NORMAL;
			});
			break;
		case TAROT_CARDS.THE_WEAK:
			if (CurSpeed == playerSpeedMode.QUICK)
			{
				CurSpeed = playerSpeedMode.NORMAL;
				break;
			}
			CurSpeed = playerSpeedMode.WEAK;
			GameManager.TimeSlinger.KillTimer(quickweakTimer);
			GameManager.TimeSlinger.FireHardTimer(out quickweakTimer, 180f, delegate
			{
				CurSpeed = playerSpeedMode.NORMAL;
			});
			break;
		case TAROT_CARDS.THE_DRUNK:
			GameManager.BehaviourManager.NotesBehaviour.ClearNotes();
			break;
		case TAROT_CARDS.THE_IMMUNE:
			EnemyStateManager.LockEnemyState(STATE_LOCK_OCCASION.TAROT);
			GameManager.TimeSlinger.FireTimer(600f, delegate
			{
				EnemyStateManager.UnlockEnemyState(STATE_LOCK_OCCASION.TAROT);
			});
			break;
		case TAROT_CARDS.THE_POPULAR:
		{
			for (int k = 0; k < Random.Range(1, 3); k++)
			{
				GameManager.TheCloud.ForceKeyDiscover();
			}
			break;
		}
		case TAROT_CARDS.THE_HERMIT:
			if (!HermitActive)
			{
				HermitActive = true;
				BotnetBehaviour.SetHermit();
				GameManager.TimeSlinger.FireTimer(300f, delegate
				{
					HermitActive = false;
					BotnetBehaviour.SetHermit();
				});
			}
			break;
		case TAROT_CARDS.THE_DIZZY:
			MainCameraHook.Ins.AddHeadHit();
			GameManager.TimeSlinger.FireTimer(300f, MainCameraHook.Ins.RemoveHeadHit);
			break;
		case TAROT_CARDS.THE_DEAF:
			if (!DeafActive)
			{
				DeafActive = true;
				GameManager.TimeSlinger.FireTimer(60f, delegate
				{
					DeafActive = false;
				});
			}
			break;
		case TAROT_CARDS.THE_BLIND:
			EnvironmentManager.PowerBehaviour.ForceTwitchPowerOff();
			break;
		case TAROT_CARDS.THE_ARTIST:
			GameManager.ManagerSlinger.TextDocManager.CreateTextDoc("keys.txt", string.Concat(new object[8]
			{
				"- " + tappedSites[0] + "\n",
				"- " + tappedSites[1] + "\n",
				"- " + tappedSites[2] + "\n",
				"- " + tappedSites[3] + "\n",
				"- " + tappedSites[4] + "\n",
				"- " + tappedSites[5] + "\n",
				"- " + tappedSites[6] + "\n",
				"- " + tappedSites[7]
			}));
			GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.KeyFound);
			break;
		case TAROT_CARDS.THE_DEAD:
			EnemyManager.CultManager.triggerCloseJump();
			break;
		case TAROT_CARDS.THE_CALLER:
			if (Random.Range(0, 2) == 0)
			{
				HitmanProxyBehaviour.FromElevator = Random.Range(0, 2) == 0;
				EnemyManager.HitManManager.HitmanEventDone(15f);
			}
			else if (Random.Range(0, 2) == 0)
			{
				TannerManager.Ins.TheCallerTanner();
			}
			else
			{
				DelfalcoBehaviour.Ins.CalledDelfalco();
			}
			break;
		case TAROT_CARDS.THE_INVISIBLE:
			BreatherManager.InvisiblePerson = true;
			break;
		case TAROT_CARDS.THE_PROSPERITY:
		{
			GameManager.ManagerSlinger.WifiManager.DisconnectFromWifi();
			EnemyManager.PoliceManager.hotNetworks.Clear();
			WifiMenuBehaviour.Ins.refreshNetworks();
			for (int num = 0; num < GameManager.ManagerSlinger.WifiManager.GetAllWifiNetworks().Count; num++)
			{
				if (GameManager.ManagerSlinger.WifiManager.GetAllWifiNetworks()[num].networkName.ToLower() != "freewifinovirus")
				{
					GameManager.ManagerSlinger.WifiManager.GetAllWifiNetworks()[num].affectedByDosDrainer = false;
				}
				GameManager.ManagerSlinger.WifiManager.GetAllWifiNetworks()[num].interaction = WiFiInteractionType.NONE;
				GameManager.ManagerSlinger.WifiManager.GetAllWifiNetworks()[num].howLongConnected = 0f;
			}
			break;
		}
		case TAROT_CARDS.THE_DENIAL:
			HackerManager.DenialHacks = 0;
			HackerManager.HackRandom = Random.Range(0, 100);
			InDenial = true;
			GameManager.HackerManager.ForceTarotHack();
			break;
		case TAROT_CARDS.THE_OUTLAW:
			EnemyStateManager.LockEnemyState(STATE_LOCK_OCCASION.OUTLAW);
			PowerStateManager.AddPowerStateLock(STATE_LOCK_OCCASION.OUTLAW);
			HackerManager.Outlaw = true;
			GameManager.TimeSlinger.FireTimer(1800f, delegate
			{
				EnemyStateManager.UnlockEnemyState(STATE_LOCK_OCCASION.OUTLAW);
				PowerStateManager.RemovePowerStateLock(STATE_LOCK_OCCASION.OUTLAW);
				HackerManager.Outlaw = false;
			});
			break;
		case TAROT_CARDS.THE_EMPRESS:
		{
			WindowManager.Get(SOFTWARE_PRODUCTS.SHADOW_MARKET).Launch();
			if (!EnemyManager.DollMakerManager.IsDollMakerActive)
			{
				ItemSlinger.LOLPYDisc.myProductObject.shipItem();
			}
			if (!EnemyManager.BombMakerManager.IsBombMakerActive)
			{
				ItemSlinger.Sulfur.myProductObject.shipItem();
			}
			GameManager.TimeSlinger.FireTimer(2f, EnemyManager.DollMakerManager.ForceMarker);
			GameManager.TimeSlinger.FireTimer(2f, EnemyManager.BombMakerManager.ReleaseTheBombMakerInstantly);
			for (int i = 0; i < 8; i++)
			{
				GameManager.TheCloud.ForceKeyDiscover();
			}
			VoIPManager.ShowVoIP();
			DelfalcoBehaviour.Ins.SetKnowsApartment();
			break;
		}
		case TAROT_CARDS.VENGEANCE:
			TarotVengeance.KillActiveEnemy();
			break;
		case TAROT_CARDS.THE_SCRIPT:
			Scripted = Random.Range(0, 4) + 1;
			break;
		}
	}

	public void PullCardAtLoc()
	{
		PullTarotCard((TAROT_CARDS)TarotCardPullAnim.currentCardTex);
		TAROT_CARDS currentCardTex = (TAROT_CARDS)TarotCardPullAnim.currentCardTex;
		Debug.Log("Pulling... It's " + currentCardTex);
	}
}
