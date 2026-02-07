using DG.Tweening;
using UnityEngine;

public class TannerManager : MonoBehaviour
{
	public static TannerManager Ins;

	public static bool ForcefullyReleased;

	public AudioFileDefinition JumpSFX;

	public AudioFileDefinition NeedleSFX;

	public AudioFileDefinition HeadHitSFX;

	public AudioFileDefinition TackleHitSFX;

	public AudioFileDefinition WindowKnockSFX;

	public bool IsHunting;

	public bool IsActive;

	public bool tannerPopActivated;

	private Sequence doorSequence = DOTween.Sequence();

	private float fireTimeStamp;

	private float fireWindow;

	private bool fireWindowActive;

	private TannerHeadLightHelper HeadLightHelper;

	private int keyDiscoverCount;

	private Timer mainRoomAttackActionTimer;

	private InteractionHook peepHoleInteraction;

	private TannerHotZoneTrigger RaidTrigger;

	private TannerBehaviour TannerBehaviour;

	private TannerLooker TannerLooker;

	private TannerProxy tannerVPN;

	private bool windowPeaked;

	public string TannerDebug
	{
		get
		{
			if (TarotVengeance.Killed(ENEMY_STATE.TANNER))
			{
				return "-2";
			}
			return (fireWindow - (Time.time - fireTimeStamp) > 0f) ? ((int)(fireWindow - (Time.time - fireTimeStamp))).ToString() : "-1";
		}
	}

	private void KeyWasDiscovered()
	{
		if (!DifficultyManager.CasualMode)
		{
			keyDiscoverCount++;
		}
	}

	private void StageTannerPatrol()
	{
		if (DifficultyManager.CasualMode || TarotVengeance.Killed(ENEMY_STATE.TANNER) || IsActive)
		{
			return;
		}
		Debug.Log("[Tanner] StageTannerPatrol - Init");
		if (EnemyStateManager.IsInEnemyStateOrLocked())
		{
			Debug.Log("[Tanner] StageTannerPatrol - EnemyState is not Idle");
			fireWindow = 45f;
			fireTimeStamp = Time.time;
			fireWindowActive = true;
			return;
		}
		if (EnvironmentManager.PowerState != POWER_STATE.ON)
		{
			Debug.Log("[Tanner] StageTannerPatrol - Power is tripped");
			fireWindow = 50f;
			fireTimeStamp = Time.time;
			fireWindowActive = true;
			return;
		}
		if (StateManager.PlayerState != PLAYER_STATE.COMPUTER)
		{
			Debug.Log("[Tanner] StageTannerPatrol - Player Is not in Computer Mode");
			fireWindow = 10f;
			fireTimeStamp = Time.time;
			fireWindowActive = true;
			return;
		}
		if (StateManager.BeingHacked)
		{
			Debug.Log("[Tanner] stageTannerPatrol - BeingHacked");
			fireWindow = 25f;
			fireTimeStamp = Time.time;
			fireWindowActive = true;
			return;
		}
		if (GameManager.HackerManager.theSwan.SwanError)
		{
			Debug.Log("[Tanner] stageTannerPatrol - SwanError");
			fireWindow = 15f;
			fireTimeStamp = Time.time;
			fireWindowActive = true;
			return;
		}
		Debug.Log("[Tanner] StageTannerPatrol - Spawn");
		PowerStateManager.AddPowerStateLock(STATE_LOCK_OCCASION.TANNER);
		EnemyStateManager.AddEnemyState(ENEMY_STATE.TANNER);
		IsActive = true;
		if (ForcefullyReleased || keyDiscoverCount >= 4 || (keyDiscoverCount >= 2 && Random.Range(0, 100) >= 20))
		{
			PrepSpawn();
		}
		else
		{
			StageWindowPeak();
		}
	}

	private void generateFireWindow()
	{
		if (!DifficultyManager.CasualMode)
		{
			fireWindow = Random.Range(750f, 1000f);
			if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
			{
				fireWindow *= 0.4f;
			}
			fireTimeStamp = Time.time;
			fireWindowActive = true;
		}
	}

	public void DeSpawn()
	{
		Debug.Log("[Tanner] DeSpawn - DeSpawn Tanner");
		IsHunting = false;
		IsActive = false;
		PowerStateManager.RemovePowerStateLock(STATE_LOCK_OCCASION.TANNER);
		EnemyStateManager.RemoveEnemyState(ENEMY_STATE.TANNER);
		DataManager.LockSave = false;
		TannerBehaviour.Spawn(new Vector3(0f, -20f, 0f), Vector3.zero);
		TannerBehaviour.ToggleFootSteps(playing: false);
		TannerBehaviour.TriggerAnim("Despawn");
		TannerBehaviour.SwitchToNormal();
		HeadLightHelper.DisableLight();
		peepHoleInteraction.ForceLock = false;
		generateFireWindow();
		tannerVPN.ToggleDoorProxy(state: false);
		tannerVPN.ToggleBalconyProxy(state: false);
	}

	private void PrepSpawn()
	{
		DataManager.LockSave = true;
		IsHunting = true;
		Spawn();
		doorlogBehaviour.MayAddDoorlog("Floor 8 Hallway", mode: true);
	}

	private void Spawn()
	{
		peepHoleController.Ins.TookOverEvents.Event += PlayerEnteredPeepHole;
		TannerBehaviour.SwitchToNormal();
		TannerBehaviour.AddMainDoorJump();
		TannerBehaviour.Spawn(new Vector3(-15.28f, 39.586f, -6.5f), Vector3.zero);
		TannerBehaviour.voiceHub.PlaySound(TannerBehaviour.laughSFX);
		TannerBehaviour.ToggleFootSteps(playing: true);
		DoorSequence();
	}

	public void StageWindowPeak()
	{
		if (!DifficultyManager.CasualMode)
		{
			Debug.Log("[Tanner] StageWindowPeak - Init");
			TannerBehaviour.Spawn(new Vector3(1.17f, 39.475f, 4f), new Vector3(0f, 173.5f, 0f));
			TannerBehaviour.SwitchToCustom();
			HeadLightHelper.DisableLight();
			TannerBehaviour.voiceHub.PlaySound(WindowKnockSFX);
			GameManager.TimeSlinger.FireTimer(1f, tannerVPN.ToggleBalconyProxy, callBackValue: true);
			TannerLooker.VisibleActions.Event += TriggerWindowPeak;
			TannerLooker.TargetLocation = TannerBehaviour.transform.position;
			TannerLooker.CheckForVisible = true;
			windowPeaked = false;
			GameManager.TimeSlinger.FireTimer(120f, BoringDespawnTanner);
		}
	}

	private void TriggerWindowPeak()
	{
		TannerLooker.VisibleActions.Event -= TriggerWindowPeak;
		windowPeaked = true;
		TannerBehaviour.voiceHub.KillSound(WindowKnockSFX.AudioClip);
		GameManager.TimeSlinger.FireTimer(0.15f, delegate
		{
			GameManager.TimeSlinger.FireTimer(1f, tannerVPN.ToggleBalconyProxy, callBackValue: true);
			TannerBehaviour.TriggerAnim("WindowPeak");
			GameManager.TimeSlinger.FireTimer(6.5f, DeSpawn);
		});
	}

	private void ExitWindowPeak()
	{
		TannerLooker.VisibleActions.Event -= TriggerWindowPeak;
		TannerLooker.CheckForVisible = false;
		windowPeaked = true;
		TannerBehaviour.voiceHub.KillSound(WindowKnockSFX.AudioClip);
		TannerBehaviour.TriggerAnim("WindowExit");
		GameManager.TimeSlinger.FireTimer(4.35f, DeSpawn);
	}

	private void MainRoomAttackAction()
	{
		tannerVPN.ToggleDoorProxy(state: false);
		CamHookBehaviour.Interruptions = true;
		CamHookBehaviour.SwitchCameraStatus(enabled: false);
		peepHoleController.Ins.TookOverEvents.Event -= PlayerEnteredPeepHole;
		StateManager.PlayerStateChangeEvents.Event -= MainRoomAttackAction;
		if (DifficultyManager.CasualMode)
		{
			return;
		}
		if (KeypadManager.Locked)
		{
			GameManager.TimeSlinger.FireTimer(2.5f, delegate
			{
				CamHookBehaviour.Interruptions = false;
				CamHookBehaviour.SwitchCameraStatus(enabled: true);
			});
			KeypadDespawnTanner();
		}
		else
		{
			if (!IsHunting)
			{
				return;
			}
			if (StateManager.PlayerState == PLAYER_STATE.BUSY)
			{
				Debug.Log("[Tanner] mainRoomAttackAction - Player is BUSY, " + StateManager.PlayerState);
				StateManager.PlayerStateChangeEvents.Event += MainRoomAttackAction;
				return;
			}
			peepHoleInteraction.ForceLock = true;
			TannerBehaviour.TriggerAnim("DeadlyIdleState");
			TannerBehaviour.RemoveMainDoorJump();
			if (StateManager.PlayerState == PLAYER_STATE.COMPUTER)
			{
				StageComputerJump();
				return;
			}
			StageKickMainDoorJump();
			GameManager.TimeSlinger.FireTimer(300f, delegate
			{
				if (EnvironmentManager.PowerState == POWER_STATE.ON)
				{
					EnvironmentManager.PowerBehaviour.ForcePowerOff();
				}
			});
		}
	}

	private void PlayerEnteredPeepHole()
	{
		if (IsHunting)
		{
			Debug.Log("[Tanner] CheckForPeepWhileHunting - Got Caught by Peep");
			peepHoleController.Ins.TookOverEvents.Event -= PlayerEnteredPeepHole;
			GameManager.TimeSlinger.KillTimer(mainRoomAttackActionTimer);
			peepHoleController.Ins.LockOutLeave = true;
			PauseManager.LockPause();
			HeadLightHelper.EnableLight();
			TannerBehaviour.Spawn(new Vector3(-2.35f, 39.7f, -5.75f), Vector3.zero);
			TannerBehaviour.SwitchToCustom();
			IsHunting = false;
			TannerBehaviour.RemoveMainDoorJump();
			GameManager.TimeSlinger.FireTimer(0.5f, delegate
			{
				tannerVPN.ToggleDoorProxy(state: false);
				TannerBehaviour.RunAwayFromDoor();
			});
			GameManager.TimeSlinger.FireTimer(4.3f, delegate
			{
				peepHoleController.Ins.LockOutLeave = false;
				peepHoleController.Ins.ForceOut();
				PauseManager.UnLockPause();
			});
		}
	}

	public void StageComputerJump()
	{
		Debug.Log("[Tanner] StageComputerJump - Stagging...");
		TannerBehaviour.SwitchToNormal();
		TannerBehaviour.Spawn(new Vector3(3.2f, 39.675f, -2.75f), new Vector3(0f, -180f, 0f));
		TannerBehaviour.TriggerAnim("StageHavingFun");
		HeadLightHelper.DisableLight();
		computerController.Ins.LeaveEvents.Event += TannerBehaviour.TriggerComputerJump;
	}

	public void StageKickMainDoorJump()
	{
		Debug.Log("[Tanner] StageKickMainDoorJump - Stagging...");
		TannerBehaviour.SwitchToNormal();
		TannerBehaviour.TriggerSyringeAnim("StageClosetJump");
		TannerBehaviour.TriggerAnim("StageClosetJump");
		RaidTrigger.SetActive();
	}

	private void BoringDespawnTanner()
	{
		if (IsActive && TannerLooker.CheckForVisible && !windowPeaked)
		{
			GameManager.TimeSlinger.FireTimer(1f, tannerVPN.ToggleBalconyProxy, callBackValue: true);
			ExitWindowPeak();
			Debug.Log("[Tanner] ExitWindowPeak (Boring Despawn)");
		}
	}

	private void DoorSequence()
	{
		CamHookBehaviour.Interruptions = true;
		CamHookBehaviour.SwitchCameraStatus(enabled: false);
		TannerBehaviour.transform.DOMove(new Vector3(-2.28f, 39.586f, -5.455f), 2.5f).SetEase(Ease.Linear).OnComplete(delegate
		{
			Debug.Log("[Tanner] Spawn - Doors reached");
			tannerVPN.ToggleDoorProxy(state: true);
			TannerBehaviour.ToggleFootSteps(playing: false);
			if (LookUp.Doors.MainDoor.Locked)
			{
				CamHookBehaviour.Interruptions = false;
				CamHookBehaviour.SwitchCameraStatus(enabled: true);
				Debug.Log("[Tanner] mainDoorSpawnAction - Doors Locked");
				GameManager.TimeSlinger.FireHardTimer(out mainRoomAttackActionTimer, 15f, MainRoomAttackAction);
			}
			else
			{
				MainRoomAttackAction();
			}
		});
	}

	public void DevStageKill()
	{
		if (StateManager.PlayerState == PLAYER_STATE.COMPUTER)
		{
			StageComputerJump();
		}
		else
		{
			StageKickMainDoorJump();
		}
	}

	public void TheCallerTanner()
	{
		ForcefullyReleased = true;
		fireWindow = 15f;
		fireTimeStamp = Time.time;
		fireWindowActive = true;
		TarotVengeance.ActivateEnemy(ENEMY_STATE.TANNER);
	}

	private void KeypadDespawnTanner()
	{
		Debug.Log("[Tanner] Keypad Despawn");
		peepHoleController.Ins.TookOverEvents.Event -= PlayerEnteredPeepHole;
		TannerBehaviour.RemoveMainDoorJump();
		DeSpawn();
	}

	private void Awake()
	{
		Ins = this;
		GameManager.TheCloud.KeyDiscoveredEvent.Event += KeyWasDiscovered;
		peepHoleInteraction = GameObject.Find("switchToPeepHoleController").GetComponent<InteractionHook>();
		BuildAudio();
		BuildEnemyPool();
		BuildRaidTrigger();
		TannerDOSPopper.TannerMAXDOSPop = ((DifficultyManager.Nightmare || DifficultyManager.LeetMode) ? 40 : 120);
		tannerVPN = new GameObject("TannerProxyManager").AddComponent<TannerProxy>();
	}

	private void Update()
	{
		if (TannerDOSPopper.DOS >= TannerDOSPopper.TannerMAXDOSPop && !tannerPopActivated)
		{
			tannerPopActivated = true;
			generateFireWindow();
			TarotVengeance.ActivateEnemy(ENEMY_STATE.TANNER);
		}
		if (fireWindowActive && Time.time - fireTimeStamp >= fireWindow)
		{
			fireWindowActive = false;
			StageTannerPatrol();
		}
		if (IsActive && StateManager.PlayerState == PLAYER_STATE.ROAMING && TannerLooker.CheckForVisible && !windowPeaked)
		{
			ExitWindowPeak();
			Debug.Log("[Tanner] ExitWindowPeak (Forced)");
			GameManager.TimeSlinger.FireTimer(1f, tannerVPN.ToggleBalconyProxy, callBackValue: false);
		}
		bool flag = false;
	}

	private void OnDestroy()
	{
		try
		{
			computerController.Ins.EnterEvents.Event -= StageComputerJump;
			computerController.Ins.LeaveEvents.Event -= TriggerWindowPeak;
			computerController.Ins.LeaveEvents.Event -= TannerBehaviour.TriggerComputerJump;
			peepHoleController.Ins.TookOverEvents.Event -= PlayerEnteredPeepHole;
			RaidTrigger.TriggerActions.Event -= TannerBehaviour.TriggerKickDoorJump;
		}
		catch
		{
		}
		Ins = null;
	}

	private void BuildAudio()
	{
		JumpSFX = CustomSoundLookUp.TannerJump;
		NeedleSFX = CustomSoundLookUp.NeedleInject;
		HeadHitSFX = CustomSoundLookUp.HeadFloorHit;
		TackleHitSFX = CustomSoundLookUp.BodyHit;
		WindowKnockSFX = CustomSoundLookUp.Window2;
		HeadHitSFX.Delay = true;
		TackleHitSFX.Delay = true;
		WindowKnockSFX.Volume = 0.9f;
		HeadHitSFX.DelayAmount = 0.65f;
		TackleHitSFX.DelayAmount = 0.38f;
	}

	private void BuildEnemyPool()
	{
		GameObject gameObject = Object.Instantiate(CustomObjectLookUp.TheTanner, GameObject.Find("EnemyPool").transform, worldPositionStays: true);
		gameObject.gameObject.name = "Tanner";
		TannerBehaviour = gameObject.AddComponent<TannerBehaviour>();
		HeadLightHelper = gameObject.AddComponent<TannerHeadLightHelper>();
		roamController.Ins.gameObject.AddComponent<TannerRoamJumper>();
		TannerLooker = CameraManager.GetCameraHook(CAMERA_ID.MAIN).gameObject.AddComponent<TannerLooker>();
	}

	private void BuildRaidTrigger()
	{
		GameObject gameObject = new GameObject("TannerRaidTrigger");
		BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
		gameObject.transform.SetParent(GameObject.Find("EnemyHotZones").transform);
		gameObject.transform.position = new Vector3(-2.236f, 40.25f, -1.985f);
		boxCollider.isTrigger = true;
		boxCollider.center = new Vector3(-0.0025f, -0.1f, 0.15f);
		boxCollider.size = new Vector3(1.867f, 1.2f, 0.2f);
		BoxCollider boxCollider2 = gameObject.AddComponent<BoxCollider>();
		boxCollider2.isTrigger = true;
		boxCollider2.center = new Vector3(-0.85f, -0.1f, -1.3f);
		boxCollider2.size = new Vector3(0.15f, 1.2f, 3f);
		BoxCollider boxCollider3 = gameObject.AddComponent<BoxCollider>();
		boxCollider3.isTrigger = true;
		boxCollider3.center = new Vector3(0.85f, -0.1f, -0.85f);
		boxCollider3.size = new Vector3(0.15f, 1.2f, 3.9f);
		BoxCollider boxCollider4 = gameObject.AddComponent<BoxCollider>();
		boxCollider4.isTrigger = true;
		boxCollider4.center = new Vector3(0.02f, 0f, -2.3f);
		boxCollider4.size = new Vector3(0.85f, 1f, 1f);
		RaidTrigger = gameObject.AddComponent<TannerHotZoneTrigger>();
		RaidTrigger.TriggerActions.Event += TannerBehaviour.TriggerKickDoorJump;
	}
}
