using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class HackerManager : MonoBehaviour
{
	public static int DenialHacks;

	public static int HackRandom;

	private static int HackDebugFreeze;

	public static bool Outlaw;

	public Canvas DesktopCanvas;

	public GameObject HackerOverlay;

	public GameObject HackedBlocked;

	public GameObject Hacked;

	public GameObject InstaHackBlocked;

	public CanvasGroup DOSCoinLostCG;

	public TextMeshProUGUI DOSCoinLostText;

	public CanvasGroup NotesLostCG;

	public TextMeshProUGUI NotesLostText;

	public CanvasGroup DOSCoinGainedCG;

	public TextMeshProUGUI DOSCoinGainedText;

	public AudioFileDefinition HackingTypeSFX;

	public AudioFileDefinition HackingIntroBedSFX;

	public AudioFileDefinition HackBlockSFX;

	public AudioFileDefinition HackedSFX;

	[SerializeField]
	private HackerManagerDataDefinition normalHackerData;

	[SerializeField]
	private HackerManagerDataDefinition leetHackerData;

	public VirusManager virusManager;

	public vapeAttack myVapeAttack;

	public DOSAttack myDosAttack;

	public bool denial;

	private float backdoorDOSCoinMax;

	private float backdoorDOSCoinMin;

	private bool bypassOnlineCheck;

	private bool customHack;

	private int customHackId;

	private int customHackLevel;

	private EvilSkullBehavior evilSkull;

	private float fireWindow;

	private bool fireWindowActive;

	private float fireWindowMax;

	private float fireWindowMin;

	private float fireWindowTimeStamp;

	private float freezeAddTime;

	private float freezeTimeStamp;

	private Tweener hacked1Tween;

	private Tweener hacked2Tween;

	private CanvasGroup hackedBlockedCG;

	private Tweener hackedBlockedTween;

	private CanvasGroup hackedCG;

	private Vector3 hackedDefaultScale = new Vector3(0.25f, 0.25f, 0.25f);

	private Vector3 hackedFullScale = Vector3.one;

	private RectTransform hackedRT;

	private CanvasGroup hackerOverlayCG;

	private Tweener instaHackBlockedTween;

	private CanvasGroup instaHackedBlockedCG;

	private bool isInTestMode;

	private int lastHackDupCount;

	private int lastHackPicked;

	private int rollCount;

	private bool rollHackFroze;

	private bool shouldSkipZonewall;

	private float smallFireWindowMax;

	private float smallFireWindowMin;

	private ComputerCameraManager myComputerCameraManager;

	private NodeHexerHack nodeHexerHack;

	private StackPusherHack stackPusherHack;

	private SweeperHack sweeperHack;

	public TheSwan theSwan;

	private bool twitchGodHack;

	private bool userIsOffLine;

	private AudioSource hackerMusicAS;

	private static bool NightmareFirstHack;

	public static bool GameEnded;

	public HackingTimerBehaviour HackingTimer { get; private set; }

	public HackingTerminalBehaviour HackingTerminal { get; private set; }

	public bool OpenForHacks { get; set; }

	public bool isInGodHack { get; private set; }

	public string HackDebug
	{
		get
		{
			if (fireWindow - (Time.time - freezeAddTime - fireWindowTimeStamp) > 0f)
			{
				if (fireWindowActive)
				{
					HackDebugFreeze = (int)(fireWindow - (Time.time - freezeAddTime - fireWindowTimeStamp));
				}
				return HackDebugFreeze.ToString();
			}
			return HackDebugFreeze.ToString();
		}
	}

	public string HackFreezeDebug => freezeAddTime + " : " + freezeTimeStamp;

	public int GetChainLevel => HackerModeManager.currenthax switch
	{
		HACK_TYPE.STACKPUSHER => stackPusherHack.HackerModeSetSkill(), 
		HACK_TYPE.NODEHEXER => nodeHexerHack.HackerModeSetSkill(), 
		HACK_TYPE.DOSBLOCK => myDosAttack.HackerModeSetSkill(), 
		HACK_TYPE.CLOUDGRID => myVapeAttack.HackerModeSetSkill(), 
		_ => 0, 
	};

	public List<StackPusherLevelDefinition> StackPusherLevelRef => stackPusherHack.StackPusherLevels;

	public List<NodeHexerLevelDefinition> NodeHexerLevelRef => nodeHexerHack.NodeHexerLevels;

	private void Awake()
	{
		GameManager.HackerManager = this;
		theSwan = new TheSwan();
		sweeperHack = GetComponent<SweeperHack>();
		stackPusherHack = GetComponent<StackPusherHack>();
		nodeHexerHack = GetComponent<NodeHexerHack>();
		virusManager = GetComponent<VirusManager>();
		HackingTerminal = GetComponent<HackingTerminalBehaviour>();
		evilSkull = GetComponent<EvilSkullBehavior>();
		HackingTimer = GetComponent<HackingTimerBehaviour>();
		hackerOverlayCG = HackerOverlay.GetComponent<CanvasGroup>();
		hackedBlockedCG = HackedBlocked.GetComponent<CanvasGroup>();
		hackedCG = Hacked.GetComponent<CanvasGroup>();
		instaHackedBlockedCG = InstaHackBlocked.GetComponent<CanvasGroup>();
		hackedRT = Hacked.GetComponent<RectTransform>();
		instaHackBlockedTween = DOTween.To(() => instaHackedBlockedCG.alpha, delegate(float x)
		{
			instaHackedBlockedCG.alpha = x;
		}, 1f, 0.2f).SetEase(Ease.Linear);
		instaHackBlockedTween.Pause();
		instaHackBlockedTween.SetAutoKill(autoKillOnCompletion: false);
		hackedBlockedTween = DOTween.To(() => hackedBlockedCG.alpha, delegate(float x)
		{
			hackedBlockedCG.alpha = x;
		}, 1f, 0.2f).SetEase(Ease.Linear);
		hackedBlockedTween.Pause();
		hackedBlockedTween.SetAutoKill(autoKillOnCompletion: false);
		hacked1Tween = DOTween.To(() => hackedCG.alpha, delegate(float x)
		{
			hackedCG.alpha = x;
		}, 1f, 0.3f).SetEase(Ease.Linear).OnComplete(genereateLostHackStats);
		hacked1Tween.Pause();
		hacked1Tween.SetAutoKill(autoKillOnCompletion: false);
		hacked2Tween = DOTween.To(() => hackedRT.localScale, delegate(Vector3 x)
		{
			hackedRT.localScale = x;
		}, hackedFullScale, 0.3f).SetEase(Ease.InCirc);
		hacked2Tween.Pause();
		hacked2Tween.SetAutoKill(autoKillOnCompletion: false);
		HackingTerminal.DumpDone += triggerSkull;
		if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			fireWindowMin = leetHackerData.FireWindowMin;
			fireWindowMax = leetHackerData.FireWindowMax;
			smallFireWindowMin = leetHackerData.SmallFireWindowMin;
			smallFireWindowMax = leetHackerData.SmallFireWindowMax;
			backdoorDOSCoinMin = leetHackerData.BackDoorDOSCoinMin;
			backdoorDOSCoinMax = leetHackerData.BackDoorDOSCoinMax;
		}
		else
		{
			fireWindowMin = normalHackerData.FireWindowMin;
			fireWindowMax = normalHackerData.FireWindowMax;
			smallFireWindowMin = normalHackerData.SmallFireWindowMin;
			smallFireWindowMax = normalHackerData.SmallFireWindowMax;
			backdoorDOSCoinMin = normalHackerData.BackDoorDOSCoinMin;
			backdoorDOSCoinMax = normalHackerData.BackDoorDOSCoinMax;
		}
		NightmareFirstHack = DifficultyManager.Nightmare;
		GameManager.StageManager.Stage += stageMe;
		GameManager.StageManager.ThreatsNowActivated += activateThreats;
	}

	private void Start()
	{
		myComputerCameraManager = ComputerCameraManager.Ins;
		hackerMusicAS = GameObject.Find("computerAudioHub").AddComponent<AudioSource>();
		hackerMusicAS.spatialBlend = 1f;
		hackerMusicAS.dopplerLevel = 0f;
		hackerMusicAS.clip = CustomSoundLookUp.hackermode;
		hackerMusicAS.loop = true;
	}

	private void PlayHackerModeSong()
	{
		hackerMusicAS.Play();
	}

	private void KillHackerModeSong()
	{
		hackerMusicAS.Stop();
	}

	private void Update()
	{
		if (fireWindowActive && Time.time - freezeAddTime - fireWindowTimeStamp >= fireWindow)
		{
			fireWindowActive = false;
			triggerHack();
		}
	}

	private void OnDestroy()
	{
		HackingTerminal.DumpDone -= triggerSkull;
		hackerMusicAS = null;
	}

	public void PresentHackGame()
	{
		HackingTimerObject.IsWTTG1Hack = false;
		myComputerCameraManager.ClearPostFXs();
		if (!shouldSkipZonewall)
		{
			sweeperHack.ActivateMe();
			sweeperHack.PrepSweepAttack(isInGodHack);
		}
		else
		{
			ProcessSweepAttack(HACK_SWEEPER_SKILL_TIER.GOD_TIER);
		}
	}

	public void ProcessSweepAttack(HACK_SWEEPER_SKILL_TIER SetTier)
	{
		if (GameEnded)
		{
			return;
		}
		if (!shouldSkipZonewall)
		{
			sweeperHack.DeActivateMe();
		}
		if (isInTestMode)
		{
			clearTestHack();
			return;
		}
		if (customHack)
		{
			LaunchCustomHack(customHackId, customHackLevel);
			return;
		}
		if (SetTier == HACK_SWEEPER_SKILL_TIER.INSTABLOCK && !twitchGodHack)
		{
			SteamSlinger.Ins.PlayerBeatZone();
			presentInstaHackBlocked();
			return;
		}
		SteamSlinger.Ins.PlayerLostZone();
		if (TarotManager.InDenial)
		{
			pickHackTarot();
			Debug.Log("In Denial - Picking up the same hack");
		}
		else
		{
			pickHack(SetTier);
		}
	}

	public void PlayerWon(int LevelIndex = 0)
	{
		if (isInTestMode)
		{
			if (HMCustomHack.IsCustomHack)
			{
				clearTestHack();
			}
			else
			{
				LaunchTestHack(HackerModeManager.currenthax);
			}
			return;
		}
		if (!TarotManager.InDenial)
		{
			presentHackBlocked(LevelIndex);
			StatisticsManager.Ins.HackBeaten();
			return;
		}
		DenialHacks++;
		if ((float)DenialHacks >= (float)GetMaxHackLevel(HackRandom))
		{
			presentHackBlocked(LevelIndex);
		}
		else
		{
			pickHackTarot();
		}
	}

	public void PlayerLost()
	{
		if (!isInTestMode)
		{
			presentHacked();
			return;
		}
		if (!HMCustomHack.IsCustomHack)
		{
			HackerModeManager.Ins.ShowGameOver();
		}
		clearTestHack();
	}

	public void LaunchTestHack(HACK_TYPE SetType)
	{
		if (GameEnded)
		{
			return;
		}
		if (DifficultyManager.HackerMode && HackerModeManager.Ins != null && !HMCustomHack.IsCustomHack)
		{
			HackerModeManager.Ins.chainCount++;
		}
		isInTestMode = true;
		StateManager.BeingHacked = true;
		GameManager.AudioSlinger.MuteAudioLayer(AUDIO_LAYER.WEBSITE);
		LookUp.DesktopUI.DesktopGraphicRaycaster.enabled = false;
		hackerOverlayCG.blocksRaycasts = true;
		hackerOverlayCG.ignoreParentGroups = true;
		DOTween.To(() => hackerOverlayCG.alpha, delegate(float x)
		{
			hackerOverlayCG.alpha = x;
		}, 1f, 0.3f).SetEase(Ease.Linear).OnComplete(delegate
		{
			switch (SetType)
			{
			case HACK_TYPE.STACKPUSHER:
				HackingTimerObject.IsWTTG1Hack = false;
				if (HMCustomHack.IsCustomHack)
				{
					CustomStackPusher();
				}
				else
				{
					stackPusherHack.PrepStackPusherAttack(HACK_SWEEPER_SKILL_TIER.HACKER_MODE);
				}
				break;
			case HACK_TYPE.NODEHEXER:
				HackingTimerObject.IsWTTG1Hack = false;
				if (HMCustomHack.IsCustomHack)
				{
					CustomNodeHexer();
				}
				else
				{
					nodeHexerHack.PrepNodeHexAttack(HACK_SWEEPER_SKILL_TIER.HACKER_MODE);
				}
				break;
			case HACK_TYPE.CLOUDGRID:
				HackingTimerObject.IsWTTG1Hack = true;
				if (HMCustomHack.IsCustomHack)
				{
					CustomVapeAttack();
				}
				else
				{
					StartVapeAttack(HACK_SWEEPER_SKILL_TIER.HACKER_MODE);
				}
				break;
			case HACK_TYPE.DOSBLOCK:
				HackingTimerObject.IsWTTG1Hack = true;
				if (HMCustomHack.IsCustomHack)
				{
					CustomDOSAttack();
				}
				else
				{
					StartDOSAttack(HACK_SWEEPER_SKILL_TIER.HACKER_MODE);
				}
				break;
			}
		});
	}

	public void RollHack()
	{
		if (!(UnityEngine.Random.Range(0f, 100f) <= 10f) || rollHackFroze)
		{
			return;
		}
		if (!DifficultyManager.LeetMode || !DifficultyManager.Nightmare)
		{
			rollCount++;
			if (rollCount >= 3)
			{
				rollCount = 0;
				GameManager.TheCloud.ForceKeyDiscover();
			}
		}
		triggerHack();
		rollHackFroze = true;
		GameManager.TimeSlinger.FireTimer(240f, delegate
		{
			rollHackFroze = false;
		});
	}

	private void clearTestHack()
	{
		HackerModeManager.Ins.ChangeMusic(HackerModeMusicType.menu);
		DOTween.To(() => hackerOverlayCG.alpha, delegate(float x)
		{
			hackerOverlayCG.alpha = x;
		}, 0f, 0.3f).SetEase(Ease.Linear).OnComplete(delegate
		{
			isInTestMode = false;
			StateManager.BeingHacked = false;
			GameManager.AudioSlinger.UnMuteAudioLayer(AUDIO_LAYER.WEBSITE);
			LookUp.DesktopUI.DesktopGraphicRaycaster.enabled = true;
			hackerOverlayCG.blocksRaycasts = false;
			hackerOverlayCG.ignoreParentGroups = false;
		});
	}

	private void presentHackAni()
	{
		if (!GameEnded)
		{
			if (DifficultyManager.Nightmare)
			{
				isInGodHack = true;
				twitchGodHack = true;
				shouldSkipZonewall = true;
			}
			StatisticsManager.Ins.HackLaunched();
			CursorManager.Ins.SwitchToHackerCursor();
			GameManager.AudioSlinger.MuteAudioLayer(AUDIO_LAYER.WEBSITE);
			LookUp.DesktopUI.DesktopGraphicRaycaster.enabled = false;
			hackerOverlayCG.blocksRaycasts = true;
			hackerOverlayCG.ignoreParentGroups = true;
			myComputerCameraManager.BeginHackAni();
			GameManager.TimeSlinger.FireTimer(1.65f, triggerTerminalDump);
			GameManager.AudioSlinger.PlaySound(HackingIntroBedSFX);
		}
	}

	private void triggerTerminalDump()
	{
		myComputerCameraManager.ClearPostFXs();
		hackerOverlayCG.alpha = 1f;
		HackingTerminal.DoDump();
	}

	private void triggerSkull()
	{
		myComputerCameraManager.TriggerHackingTerminalDumpGlitch();
		GameManager.TimeSlinger.FireTimer(0.75f, delegate
		{
			myComputerCameraManager.TriggerHackingTerminalSkullEFXs();
			HackingTerminal.TerminalHelper.ClearTerminal();
			HackingTerminal.TerminalHelper.UpdateTerminalContentScrollHeight();
			evilSkull.PresentMe();
		});
	}

	private void pickHack(HACK_SWEEPER_SKILL_TIER SetTier)
	{
		switch ((TarotManager.Scripted > 0) ? TarotManager.Scripted : (UnityEngine.Random.Range(0, 4) + 1))
		{
		case 1:
			HackingTimerObject.IsWTTG1Hack = true;
			if (isInGodHack)
			{
				StartVapeAttack(HACK_SWEEPER_SKILL_TIER.GOD_TIER);
			}
			else
			{
				StartVapeAttack(SetTier);
			}
			break;
		case 2:
			HackingTimerObject.IsWTTG1Hack = false;
			if (isInGodHack)
			{
				stackPusherHack.PrepStackPusherAttack(HACK_SWEEPER_SKILL_TIER.GOD_TIER);
			}
			else
			{
				stackPusherHack.PrepStackPusherAttack(SetTier);
			}
			break;
		case 3:
			HackingTimerObject.IsWTTG1Hack = false;
			if (isInGodHack)
			{
				nodeHexerHack.PrepNodeHexAttack(HACK_SWEEPER_SKILL_TIER.GOD_TIER);
			}
			else
			{
				nodeHexerHack.PrepNodeHexAttack(SetTier);
			}
			break;
		case 4:
			HackingTimerObject.IsWTTG1Hack = true;
			if (isInGodHack)
			{
				StartDOSAttack(HACK_SWEEPER_SKILL_TIER.GOD_TIER);
			}
			else
			{
				StartDOSAttack(SetTier);
			}
			break;
		}
	}

	private void presentHacked()
	{
		if (twitchGodHack && (DifficultyManager.LeetMode || DifficultyManager.Nightmare || TarotManager.InDenial))
		{
			EnvironmentManager.PowerBehaviour.ForcePowerOff();
			EnvironmentManager.PowerBehaviour.ResetPowerTripTime();
		}
		GameManager.AudioSlinger.KillSound(HackingIntroBedSFX);
		CursorManager.Ins.SwitchToHackerCursor();
		GameManager.AudioSlinger.PlaySound(HackedSFX);
		myComputerCameraManager.TriggerHackedEFXs();
		evilSkull.HackedLaugh();
		hacked1Tween.Restart();
		hacked2Tween.Restart();
		GameManager.TimeSlinger.FireTimer(3f, dismissHackMode);
	}

	public void presentHackBlocked(int LevelIndex = 0)
	{
		GameManager.AudioSlinger.KillSound(HackingIntroBedSFX);
		GameManager.AudioSlinger.PlaySound(HackBlockSFX);
		myComputerCameraManager.TriggerHackBlockedEFXs();
		hackedBlockedTween.Restart();
		genereateBlockHackedStats(LevelIndex);
		GameManager.TimeSlinger.FireTimer(2.3f, dismissHackMode);
	}

	private void presentInstaHackBlocked()
	{
		GameManager.AudioSlinger.KillSound(HackingIntroBedSFX);
		GameManager.AudioSlinger.PlaySound(HackBlockSFX);
		myComputerCameraManager.TriggerHackBlockedEFXs();
		instaHackBlockedTween.Restart();
		GameManager.TimeSlinger.FireTimer(2.3f, dismissHackMode);
	}

	private void dismissHackMode()
	{
		if (EnemyStateManager.HasEnemyState(ENEMY_STATE.KIDNAPPER))
		{
			GameManager.TimeSlinger.FireTimer(3f, delegate
			{
				StateManager.BeingHacked = false;
			});
		}
		else
		{
			StateManager.BeingHacked = false;
		}
		generateFireWindow();
		hackedCG.alpha = 0f;
		hackedBlockedCG.alpha = 0f;
		instaHackedBlockedCG.alpha = 0f;
		hackerOverlayCG.alpha = 0f;
		hackedRT.localScale = hackedDefaultScale;
		hackerOverlayCG.alpha = 0f;
		hackerOverlayCG.blocksRaycasts = false;
		hackerOverlayCG.ignoreParentGroups = false;
		DOSCoinLostCG.alpha = 0f;
		NotesLostCG.alpha = 0f;
		DOSCoinGainedCG.alpha = 0f;
		myComputerCameraManager.ClearPostFXs();
		isInGodHack = false;
		twitchGodHack = false;
		bypassOnlineCheck = false;
		shouldSkipZonewall = false;
		customHack = false;
		LookUp.DesktopUI.DesktopGraphicRaycaster.enabled = true;
		if (!ComputerMuteBehaviour.Ins.Muted)
		{
			GameManager.AudioSlinger.UnMuteAudioLayer(AUDIO_LAYER.WEBSITE);
		}
		if (!DifficultyManager.HackerMode)
		{
			CursorManager.Ins.ClearHackerCursor();
		}
	}

	private void generateFireWindow()
	{
		freezeAddTime = 0f;
		freezeTimeStamp = (GameManager.ManagerSlinger.WifiManager.IsOnline ? 0f : Time.time);
		fireWindow = UnityEngine.Random.Range(fireWindowMin, fireWindowMax);
		fireWindowTimeStamp = Time.time;
		fireWindowActive = GameManager.ManagerSlinger.WifiManager.IsOnline;
	}

	private void generateSmallFireWindow()
	{
		freezeAddTime = 0f;
		freezeTimeStamp = (GameManager.ManagerSlinger.WifiManager.IsOnline ? 0f : Time.time);
		fireWindow = UnityEngine.Random.Range(smallFireWindowMin, smallFireWindowMax);
		fireWindowTimeStamp = Time.time;
		fireWindowActive = GameManager.ManagerSlinger.WifiManager.IsOnline;
	}

	private void triggerHack()
	{
		if (!GameEnded)
		{
			if (!OpenForHacks || StateManager.BeingHacked || theSwan.SwanFailure || KAttack.IsInAttack || Outlaw)
			{
				generateSmallFireWindow();
			}
			else if (EnemyStateManager.IsInEnemyState())
			{
				generateSmallFireWindow();
			}
			else if (!ComputerPowerHook.Ins.FullyPoweredOn)
			{
				Debug.Log("Why do you want to hack when the PC is off?");
				generateSmallFireWindow();
			}
			else if (!GameManager.ManagerSlinger.WifiManager.IsOnline)
			{
				Debug.Log("Hacked when offline? WTF");
				generateSmallFireWindow();
			}
			else
			{
				GameManager.AudioSlinger.UnMuteAudioHub(AUDIO_HUB.COMPUTER_HUB);
				StateManager.BeingHacked = true;
				presentHackAni();
			}
		}
	}

	private void playerPausedGame()
	{
		if (!userIsOffLine && !StateManager.BeingHacked)
		{
			fireWindowActive = false;
			freezeTimeStamp = Time.time;
		}
	}

	private void playerUnPausedGame()
	{
		if (!userIsOffLine && !StateManager.BeingHacked)
		{
			freezeAddTime += Time.time - freezeTimeStamp;
			fireWindowActive = true;
		}
	}

	private void networkWentOffLine()
	{
		userIsOffLine = true;
		fireWindowActive = false;
		freezeTimeStamp = Time.time;
	}

	private void networkWentOnline()
	{
		userIsOffLine = false;
		freezeAddTime += Time.time - freezeTimeStamp;
		fireWindowActive = true;
	}

	private void genereateBlockHackedStats(int levelIndex)
	{
		if (TarotManager.InDenial)
		{
			TarotManager.InDenial = false;
			KillHackerModeSong();
			DOSCoinsCurrencyManager.AddCurrency(1337f);
			DOSCoinGainedText.text = "GAINED: 1337";
			DOTween.To(() => DOSCoinGainedCG.alpha, delegate(float x)
			{
				DOSCoinGainedCG.alpha = x;
			}, 1f, 0.2f).SetEase(Ease.Linear);
			return;
		}
		if (InventoryManager.GetBackdoorCount() > 0)
		{
			float num = (float)Math.Round(UnityEngine.Random.Range(backdoorDOSCoinMin, backdoorDOSCoinMax) * (float)levelIndex, 3);
			if (twitchGodHack)
			{
				if (DifficultyManager.LeetMode)
				{
					num = 33.37f;
				}
				else if (!DifficultyManager.Nightmare)
				{
					num = ((!DifficultyManager.CasualMode) ? 13.37f : 6.9f);
				}
				else
				{
					num = (NightmareFirstHack ? 80.4f : 50.2f);
					NightmareFirstHack = false;
				}
			}
			if (num <= 0f)
			{
				num = 3.5f;
			}
			if (DifficultyManager.LeetMode && num >= 75f)
			{
				num = 78.385f;
			}
			DOSCoinsCurrencyManager.AddCurrency(num);
			DOSCoinGainedText.text = "GAINED: " + num;
			DOTween.To(() => DOSCoinGainedCG.alpha, delegate(float x)
			{
				DOSCoinGainedCG.alpha = x;
			}, 1f, 0.2f).SetEase(Ease.Linear);
		}
		InventoryManager.RemoveBackdoorHack();
	}

	private void genereateLostHackStats()
	{
		TarotManager.Scripted = 0;
		if (!TarotManager.InDenial)
		{
			InventoryManager.RemoveBackdoorHack();
		}
		double num = DOSCoinsCurrencyManager.CurrentCurrency;
		int num2 = UnityEngine.Random.Range(1, 21);
		float num3 = ((num2 >= 8 && num2 < 12) ? 0.25f : ((num2 >= 5 && num2 < 8) ? 0.35f : ((num2 >= 3 && num2 < 5) ? 0.45f : ((num2 != 2) ? 0.2f : 0.75f))));
		if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			num3 = 0.95f;
		}
		float num4 = (TarotManager.InDenial ? (DOSCoinsCurrencyManager.CurrentCurrency - 0.56f) : ((float)Math.Round(num * (double)num3, 3)));
		if (num4 <= 0f)
		{
			num4 = 0f;
		}
		DOSCoinsCurrencyManager.RemoveCurrency(num4);
		DOSCoinLostText.SetText("LOST: " + num4);
		num2 = UnityEngine.Random.Range(1, 101);
		if ((num2 >= 0 && num2 <= 20) || TarotManager.InDenial)
		{
			GameManager.BehaviourManager.NotesBehaviour.ClearNotes();
			NotesLostText.SetText("NOTES: LOST");
		}
		else
		{
			NotesLostText.SetText("NOTES: KEPT");
		}
		if (DifficultyManager.LeetMode || DifficultyManager.Nightmare || TarotManager.InDenial)
		{
			if (DifficultyManager.LeetMode || DifficultyManager.Nightmare || TarotManager.InDenial)
			{
				virusManager.ForceVirus();
			}
			else
			{
				virusManager.AddVirus();
			}
		}
		TarotManager.InDenial = false;
		KillHackerModeSong();
		DOTween.To(() => DOSCoinLostCG.alpha, delegate(float x)
		{
			DOSCoinLostCG.alpha = x;
		}, 1f, 0.5f).SetEase(Ease.Linear);
		DOTween.To(() => NotesLostCG.alpha, delegate(float x)
		{
			NotesLostCG.alpha = x;
		}, 1f, 0.5f).SetEase(Ease.Linear);
	}

	private void stageMe()
	{
		GameManager.PauseManager.GamePaused += playerPausedGame;
		GameManager.PauseManager.GameUnPaused += playerUnPausedGame;
		GameManager.ManagerSlinger.WifiManager.WentOnline += networkWentOnline;
		GameManager.ManagerSlinger.WifiManager.WentOffline += networkWentOffLine;
		HackDebugFreeze = 0;
		GameManager.StageManager.Stage -= stageMe;
	}

	private void activateThreats()
	{
		OpenForHacks = true;
		generateFireWindow();
		GameManager.StageManager.ThreatsNowActivated -= activateThreats;
	}

	public void ForceTwitchHack()
	{
		if (StateManager.BeingHacked || theSwan.SwanFailure || !ComputerPowerHook.Ins.FullyPoweredOn || EnvironmentManager.PowerState == POWER_STATE.OFF || EnemyStateManager.IsInEnemyState() || KAttack.IsInAttack)
		{
			GameManager.TimeSlinger.FireTimer(20f, ForceTwitchHack);
			return;
		}
		StateManager.BeingHacked = true;
		isInGodHack = true;
		twitchGodHack = true;
		bypassOnlineCheck = true;
		shouldSkipZonewall = true;
		presentHackAni();
	}

	public void ForceNormalHack()
	{
		if (StateManager.BeingHacked || theSwan.SwanFailure || !ComputerPowerHook.Ins.FullyPoweredOn || EnvironmentManager.PowerState == POWER_STATE.OFF || EnemyStateManager.IsInEnemyState() || KAttack.IsInAttack)
		{
			GameManager.TimeSlinger.FireTimer(20f, ForceNormalHack);
			return;
		}
		StateManager.BeingHacked = true;
		bypassOnlineCheck = true;
		presentHackAni();
	}

	public void WhiteHatSound()
	{
		GameManager.AudioSlinger.PlaySound(HackBlockSFX);
	}

	public void BlackHatSound()
	{
		GameManager.AudioSlinger.PlaySound(HackedSFX);
	}

	public void ForcePogHack()
	{
		if (StateManager.BeingHacked || theSwan.SwanFailure || !ComputerPowerHook.Ins.FullyPoweredOn || EnvironmentManager.PowerState == POWER_STATE.OFF || EnemyStateManager.IsInEnemyState() || KAttack.IsInAttack)
		{
			GameManager.TimeSlinger.FireTimer(20f, ForcePogHack);
			return;
		}
		StateManager.BeingHacked = true;
		isInGodHack = true;
		bypassOnlineCheck = true;
		presentHackAni();
	}

	public void ForceTarotHack()
	{
		StateManager.BeingHacked = true;
		twitchGodHack = true;
		bypassOnlineCheck = true;
		shouldSkipZonewall = true;
		presentHackAni();
		GameManager.TimeSlinger.FireTimer(11.2f, PlayHackerModeSong);
	}

	private void pickHackTarot()
	{
		if (HackRandom < 25)
		{
			HackingTimerObject.IsWTTG1Hack = true;
			myVapeAttack.CreateNewVapeAttackTarot(DenialHacks);
		}
		else if ((HackRandom >= 25) & (HackRandom < 50))
		{
			HackingTimerObject.IsWTTG1Hack = false;
			stackPusherHack.PrepStackPusherAttackTarot(DenialHacks);
		}
		else if ((HackRandom >= 50) & (HackRandom < 75))
		{
			HackingTimerObject.IsWTTG1Hack = false;
			nodeHexerHack.PrepNodeHexAttackTarot(DenialHacks);
		}
		else
		{
			HackingTimerObject.IsWTTG1Hack = true;
			myDosAttack.CreateNewDOSAttackTarot(DenialHacks);
		}
	}

	public void HackerModeHack()
	{
		StateManager.BeingHacked = true;
		bypassOnlineCheck = true;
		presentHackAni();
	}

	public void StartVapeAttack(HACK_SWEEPER_SKILL_TIER SetTier)
	{
		GameManager.AudioSlinger.PlaySound(HackingTypeSFX);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out myVapeAttack.termLine1, TERMINAL_LINE_TYPE.TYPE, "> ./CLOUDGRID", 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out myVapeAttack.termLine2, TERMINAL_LINE_TYPE.TYPE, "  Loading CLOUDGRID v.AP3nA710N...", 0.2f, 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out myVapeAttack.termLine3, TERMINAL_LINE_TYPE.TYPE, "  Initialzing...", 0.2f, 0.4f);
		myVapeAttack.CreateNewVapeAttack(SetTier);
		GameManager.TimeSlinger.FireTimer(0.8f, myVapeAttack.warmVapeAttack);
	}

	public void LaunchCustomHack(int hackID, int difficulty)
	{
		switch (hackID)
		{
		case 0:
			HackingTimerObject.IsWTTG1Hack = false;
			nodeHexerHack.PrepNodeHexAttackTarot(difficulty);
			break;
		case 1:
			HackingTimerObject.IsWTTG1Hack = false;
			stackPusherHack.PrepStackPusherAttackTarot(difficulty);
			break;
		case 2:
			HackingTimerObject.IsWTTG1Hack = true;
			myVapeAttack.CreateNewVapeAttackTarot(difficulty);
			break;
		default:
			HackingTimerObject.IsWTTG1Hack = true;
			myDosAttack.CreateNewDOSAttackTarot(difficulty);
			break;
		}
	}

	private int GetMaxHackLevel(int hackrandom)
	{
		if (hackrandom >= 50)
		{
			return 10;
		}
		return 12;
	}

	public void StartDOSAttack(HACK_SWEEPER_SKILL_TIER SetTier)
	{
		GameManager.AudioSlinger.PlaySound(HackingTypeSFX);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out myDosAttack.termLine1, TERMINAL_LINE_TYPE.TYPE, "> ./DOS_Blocker", 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out myDosAttack.termLine2, TERMINAL_LINE_TYPE.TYPE, "  Loading DOS Blocker v0.6b...", 0.2f, 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out myDosAttack.termLine3, TERMINAL_LINE_TYPE.TYPE, "  Initialzing...", 0.2f, 0.4f);
		GameManager.TimeSlinger.FireTimer(0.8f, myDosAttack.CreateNewDOSAttack, SetTier);
		GameManager.TimeSlinger.FireTimer(1f, myDosAttack.warmDOSAttack);
	}

	public void ForceCustomHack(int hack, int level)
	{
		if (StateManager.BeingHacked || theSwan.SwanFailure || !ComputerPowerHook.Ins.FullyPoweredOn || EnvironmentManager.PowerState == POWER_STATE.OFF || EnemyStateManager.IsInEnemyState() || KAttack.IsInAttack)
		{
			GameManager.TimeSlinger.FireTimer(20f, ForceCustomHack, hack, level);
			return;
		}
		StateManager.BeingHacked = true;
		bypassOnlineCheck = true;
		shouldSkipZonewall = true;
		customHack = true;
		customHackId = hack;
		customHackLevel = level;
		presentHackAni();
	}

	private void CustomStackPusher()
	{
		HMCustomStackPusher stackPusher = HackerModeManager.Ins.lookUp.customHack.stackPusher;
		int num = stackPusher.stackPieces;
		int num2 = stackPusher.deadPieces;
		int num3 = stackPusher.matrixSize * stackPusher.matrixSize - 2;
		int num4 = num3;
		if (num + num2 > num3)
		{
			if (num > num4)
			{
				num = num4;
			}
			if (num + num2 > num3)
			{
				num2 = num3 - num;
			}
		}
		stackPusherHack.buildCustomStackPusherAttack(stackPusher.matrixSize, num, num2, stackPusher.timePerPiece, 3, stackPusher.randomExit);
	}

	private void CustomNodeHexer()
	{
		HMCustomNodeHexer nodeHexer = HackerModeManager.Ins.lookUp.customHack.nodeHexer;
		int num = nodeHexer.matrixSize * nodeHexer.matrixSize / 2 - 1;
		int num2 = nodeHexer.tagPieces;
		float startTime = nodeHexer.startTime + (float)(nodeHexer.tagPieces / 4);
		if (num2 > num)
		{
			num2 = num;
		}
		nodeHexerHack.buildCustomNodeHexAttack(nodeHexer.matrixSize, num2, nodeHexer.timeBoost, startTime);
	}

	private void CustomVapeAttack()
	{
		HMCustomVapeAttack hMCustomVapeAttack = HackerModeManager.Ins.lookUp.customHack.vapeAttack;
		myVapeAttack.prepCustomVapeAttack((short)hMCustomVapeAttack.matrixSize, hMCustomVapeAttack.freeCountPer, (short)hMCustomVapeAttack.groupSize, (short)hMCustomVapeAttack.deadNoteSize, hMCustomVapeAttack.timePerBlock);
	}

	private void CustomDOSAttack()
	{
		HMCustomDOSBlocker dosBlocker = HackerModeManager.Ins.lookUp.customHack.dosBlocker;
		short num = (short)dosBlocker.actionBlockSize;
		short num2 = (short)dosBlocker.matrixSize;
		if (num > num2)
		{
			num = num2;
		}
		myDosAttack.prepCustomDOSAttack((short)dosBlocker.matrixSize, num, dosBlocker.hotTime, dosBlocker.gameTimeModifier, dosBlocker.trollNodesActive);
	}

	public void dismissCustomMode(AudioFileDefinition sound)
	{
		GameManager.AudioSlinger.PlaySound(sound);
		hackedBlockedTween.Restart();
		GameManager.TimeSlinger.FireTimer(2.3f, dismissHackMode);
	}
}
