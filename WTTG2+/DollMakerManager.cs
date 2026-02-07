using System.Collections.Generic;
using UnityEngine;

public class DollMakerManager : MonoBehaviour
{
	private const float DOLLMAKER_Y_OFFSET = 0.93429f;

	[SerializeField]
	private DollMakerDataDefinition myData;

	[SerializeField]
	private DollMakerBehaviour dollMakerBehaviour;

	[SerializeField]
	private Light dollMakerDoorHelperLight;

	[SerializeField]
	private AudioFileDefinition dollMakerMusic;

	[SerializeField]
	private AudioFileDefinition helpMeSFX;

	[SerializeField]
	private EnemyHotZoneTrigger warningTrigger;

	[SerializeField]
	private DollMakerMarkerBehaviour theMaker;

	[SerializeField]
	private Vector3 markerDefaultSpawnPOS = Vector3.zero;

	[SerializeField]
	private Vector3 markerDefaultSpawnROT = Vector3.zero;

	[SerializeField]
	private AudioFileDefinition makerQueSFX;

	[SerializeField]
	private DollMakerMarkerTrigger[] markerTriggers = new DollMakerMarkerTrigger[0];

	private int activeUnitNumber;

	private bool delayActive;

	private float delayTimeStamp;

	private float delayWindow;

	private bool dollMakerActivated;

	private bool forced;

	private Timer forcePowerTripTimer;

	private bool markerActive;

	private float markerTimeStamp;

	private float markerWindow;

	private DollMakerData myDollMakerData;

	private int myID;

	private int PriceUnit;

	public static bool lucassed;

	private AudioSource gamerRage;

	public string MarkerDebug
	{
		get
		{
			if (TarotVengeance.Killed(ENEMY_STATE.DOLL_MAKER) || lucassed)
			{
				return "-2";
			}
			return (markerWindow - (Time.time - markerTimeStamp) > 0f) ? ((int)(markerWindow - (Time.time - markerTimeStamp))).ToString() : "-1";
		}
	}

	public DollMakerMarkerBehaviour TheMaker => theMaker;

	public bool IsDollMakerActive { get; private set; }

	public string tenantDebug => GameManager.ManagerSlinger.TenantTrackManager.CheckIfFemaleTenant(activeUnitNumber).ToString();

	private void Awake()
	{
		EnemyManager.DollMakerManager = this;
		myID = base.transform.position.GetHashCode();
		GameManager.StageManager.Stage += stageMe;
		GameManager.StageManager.ThreatsNowActivated += threatsActivated;
		theMaker.MarkerWasPickedUp.Event += markerWasPickedUp;
		gamerRage = Object.Instantiate(CustomObjectLookUp.GamerRage).GetComponent<AudioSource>();
	}

	private void Update()
	{
		if (DifficultyManager.CasualMode || lucassed)
		{
			return;
		}
		if (delayActive && Time.time - delayTimeStamp >= delayWindow)
		{
			delayActive = false;
			if (!forced)
			{
				triggerDollMakerSpawn();
			}
		}
		if (markerActive && Time.time - markerTimeStamp >= markerWindow)
		{
			markerActive = false;
			triggerMarkerTimesUp();
		}
	}

	private void OnDestroy()
	{
		theMaker.MarkerWasPickedUp.Event -= markerWasPickedUp;
	}

	public void ReleaseTheDollMaker()
	{
		if (!DifficultyManager.CasualMode && !IsDollMakerActive)
		{
			GameManager.StageManager.ManuallyActivateThreats();
			IsDollMakerActive = true;
			myDollMakerData.IsReleased = true;
			DataManager.Save(myDollMakerData);
			generateReleaseWindow(instantly: false);
			TarotVengeance.ActivateEnemy(ENEMY_STATE.DOLL_MAKER);
		}
	}

	public void ReleaseTheDollMakerFastDBG()
	{
		if (!DifficultyManager.CasualMode && !IsDollMakerActive)
		{
			GameManager.StageManager.ManuallyActivateThreats();
			IsDollMakerActive = true;
			myDollMakerData.IsReleased = true;
			DataManager.Save(myDollMakerData);
			generateReleaseWindow(instantly: true);
			TarotVengeance.ActivateEnemy(ENEMY_STATE.DOLL_MAKER);
		}
	}

	public void DoorPowerTrip()
	{
		PowerStateManager.RemovePowerStateLock(STATE_LOCK_OCCASION.DOLL_MAKER);
		EnvironmentManager.PowerBehaviour.ForcePowerOff();
		EnvironmentManager.PowerBehaviour.PowerOnEvent.Event += playerTurnedPowerBackOn;
		PowerStateManager.AddPowerStateLock(STATE_LOCK_OCCASION.DOLL_MAKER);
		GameManager.AudioSlinger.KillSound(dollMakerMusic);
		LookUp.Doors.MainDoor.EnableDoor();
		LookUp.Doors.BalconyDoor.EnableDoor();
		LookUp.Doors.BathroomDoor.EnableDoor();
		PauseManager.UnLockPause();
		dollMakerDoorHelperLight.enabled = false;
		dollMakerBehaviour.DeSpawn();
		peepHoleController.Ins.LockOutLeave = false;
		peepHoleController.Ins.ForceOut();
	}

	public void PlayerHitWarningTrigger()
	{
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		dollMakerBehaviour.StageSpeech();
		DollMakerRoamJumper.Ins.TriggerSpeechJump();
	}

	public void ClearWarningTrigger()
	{
		PauseManager.UnLockPause();
		GameManager.InteractionManager.UnLockInteraction();
		DataManager.LockSave = false;
		EnemyStateManager.RemoveEnemyState(ENEMY_STATE.DOLL_MAKER);
		LookUp.Doors.MainDoor.DisableDoor();
		theMaker.MarkerWasPickedUp.Event += markerWasPickedUpForTheFirstTime;
		theMaker.SpawnMeTo(markerDefaultSpawnPOS, markerDefaultSpawnROT);
		activateMarkerTime();
		GameManager.TimeSlinger.FireTimer(2f, delegate
		{
			MainCameraHook.Ins.ResetARF();
		});
	}

	public void MarkerWasPlaced(int UnitNumber, Vector3 SpawnPOS, Vector3 SpawnROT)
	{
		activeUnitNumber = UnitNumber;
		theMaker.SpawnMeTo(SpawnPOS, SpawnROT);
		myDollMakerData.ActiveUnitNumber = UnitNumber;
		DataManager.Save(myDollMakerData);
		for (int i = 0; i < markerTriggers.Length; i++)
		{
			markerTriggers[i].DeActivate();
		}
	}

	private void triggerDollMakerSpawn()
	{
		if (DifficultyManager.CasualMode || TarotVengeance.Killed(ENEMY_STATE.DOLL_MAKER) || lucassed)
		{
			return;
		}
		if (EnemyStateManager.IsInEnemyStateOrLocked() || StateManager.BeingHacked || EnvironmentManager.PowerState != POWER_STATE.ON)
		{
			delayWindow = 30f;
			delayTimeStamp = Time.time;
			delayActive = true;
			return;
		}
		DataManager.LockSave = true;
		EnemyStateManager.AddEnemyState(ENEMY_STATE.DOLL_MAKER);
		PowerStateManager.AddPowerStateLock(STATE_LOCK_OCCASION.DOLL_MAKER);
		if (StateManager.PlayerState == PLAYER_STATE.COMPUTER)
		{
			stageDoorSpawn();
		}
		else
		{
			computerController.Ins.EnterEvents.Event += stageDoorSpawn;
		}
	}

	private void stageDoorSpawn()
	{
		computerController.Ins.EnterEvents.Event -= stageDoorSpawn;
		DollCameraFlicker();
		dollMakerDoorHelperLight.enabled = true;
		dollMakerBehaviour.StageDoorSpawn();
		GameManager.AudioSlinger.MuffleAudioHub(AUDIO_HUB.MANIKIN_HUB, 0.25f);
		GameManager.AudioSlinger.PlaySound(dollMakerMusic);
		peepHoleController.Ins.TakingOverEvents.Event += playerIsEnteringPeepHole;
		peepHoleController.Ins.TookOverEvents.Event += playerEnteredPeepHole;
		LookUp.Doors.MainDoor.DisableDoor();
		LookUp.Doors.BalconyDoor.DisableDoor();
		LookUp.Doors.BathroomDoor.DisableDoor();
		dollMakerBehaviour.ManikinTransform.localPosition = new Vector3(-0.002f, 0f, 0f);
		GameManager.TimeSlinger.FireHardTimer(out forcePowerTripTimer, myData.ForcePowerTripTime, forcePowerTrip);
	}

	private void generateReleaseWindow()
	{
		if (!DifficultyManager.CasualMode)
		{
			delayWindow = Random.Range(myData.DelayTimeMin, myData.DelayTimeMax);
			delayTimeStamp = Time.time;
			delayActive = true;
		}
	}

	private void forcePowerTrip()
	{
		peepHoleController.Ins.LockOutLeave = false;
		peepHoleController.Ins.TakingOverEvents.Event -= playerIsEnteringPeepHole;
		peepHoleController.Ins.TookOverEvents.Event -= playerEnteredPeepHole;
		PowerStateManager.RemovePowerStateLock(STATE_LOCK_OCCASION.DOLL_MAKER);
		EnvironmentManager.PowerBehaviour.ForcePowerOff();
		EnvironmentManager.PowerBehaviour.PowerOnEvent.Event += playerTurnedPowerBackOn;
		PowerStateManager.AddPowerStateLock(STATE_LOCK_OCCASION.DOLL_MAKER);
		GameManager.AudioSlinger.KillSound(dollMakerMusic);
		LookUp.Doors.MainDoor.EnableDoor();
		LookUp.Doors.BalconyDoor.EnableDoor();
		LookUp.Doors.BathroomDoor.EnableDoor();
		dollMakerDoorHelperLight.enabled = false;
		dollMakerBehaviour.DeSpawn();
		dollMakerBehaviour.TriggerAnim("ExitDollGrabIdle");
	}

	private void playerTurnedPowerBackOn()
	{
		EnvironmentManager.PowerBehaviour.PowerOnEvent.Event -= playerTurnedPowerBackOn;
		EnvironmentManager.PowerBehaviour.ResetPowerTripTime();
		PowerStateManager.RemovePowerStateLock(STATE_LOCK_OCCASION.DOLL_MAKER);
		warningTrigger.SetActive();
	}

	public void markerWasPickedUpForTheFirstTime()
	{
		myDollMakerData.IsActivated = true;
		DataManager.Save(myDollMakerData);
		theMaker.MarkerWasPickedUp.Event -= markerWasPickedUpForTheFirstTime;
		LookUp.Doors.MainDoor.EnableDoor();
		LookUp.Doors.MainDoor.dollmakerHeadOn = false;
	}

	private void activateMarkerTime()
	{
		if (!DifficultyManager.CasualMode)
		{
			markerWindow = Random.Range(myData.MarkerCoolTimeMin, myData.MarkerCoolTimeMax);
			markerTimeStamp = Time.time;
			markerActive = true;
		}
	}

	private void triggerMarkerTimesUp()
	{
		if (DifficultyManager.CasualMode || TarotVengeance.Killed(ENEMY_STATE.DOLL_MAKER) || lucassed)
		{
			return;
		}
		StateManager.PlayerLocationChangeEvents.Event -= triggerMarkerTimesUp;
		StateManager.PlayerStateChangeEvents.Event -= triggerMarkerTimesUp;
		if ((EnemyStateManager.IsInEnemyStateOrLocked() || EnvironmentManager.PowerState != POWER_STATE.ON || StateManager.BeingHacked) && (!EnemyStateManager.HasEnemyState(ENEMY_STATE.DOLL_MAKER) || EnvironmentManager.PowerState != POWER_STATE.ON || StateManager.BeingHacked))
		{
			markerWindow = 60f;
			markerTimeStamp = Time.time;
			markerActive = true;
			return;
		}
		if (StateManager.PlayerState == PLAYER_STATE.BUSY)
		{
			StateManager.PlayerStateChangeEvents.Event += triggerMarkerTimesUp;
			return;
		}
		if (StateManager.PlayerLocation == PLAYER_LOCATION.UNKNOWN)
		{
			StateManager.PlayerLocationChangeEvents.Event += triggerMarkerTimesUp;
			return;
		}
		EnemyStateManager.AddEnemyState(ENEMY_STATE.DOLL_MAKER);
		doorlogBehaviour.MayAddDoorlog($"Apartment {activeUnitNumber}", mode: true);
		if (GameManager.ManagerSlinger.TenantTrackManager.CheckIfFemaleTenant(activeUnitNumber))
		{
			if (GameManager.ManagerSlinger.TenantTrackManager.CheckAyana(activeUnitNumber))
			{
				TenantTrackManager.DidAyana = true;
			}
			if (GameManager.ManagerSlinger.TenantTrackManager.CheckGG(activeUnitNumber))
			{
				gamerRage.Play();
				GameManager.ManagerSlinger.WifiManager.TakeNetworkOffLineForever("GameNight At 602");
			}
			nextMarkerCheck();
			return;
		}
		if (GameManager.ManagerSlinger.TenantTrackManager.CheckLucas(activeUnitNumber))
		{
			lucassed = true;
			doorlogBehaviour.MayAddDoorlog($"Apartment {activeUnitNumber}", mode: true);
			GameManager.TimeSlinger.FireTimer(1f, delegate
			{
				doorlogBehaviour.MayAddDoorlog($"Apartment {activeUnitNumber}", mode: false);
			});
			GameManager.TimeSlinger.FireTimer(2f, delegate
			{
				doorlogBehaviour.MayAddDoorlog($"Apartment {activeUnitNumber}", mode: true);
			});
			GameManager.TimeSlinger.FireTimer(3f, delegate
			{
				doorlogBehaviour.MayAddDoorlog($"Apartment {activeUnitNumber}", mode: false);
			});
			GameManager.TimeSlinger.FireTimer(4f, delegate
			{
				doorlogBehaviour.MayAddDoorlog($"Apartment {activeUnitNumber}", mode: true);
			});
			GameManager.TimeSlinger.FireTimer(5f, delegate
			{
				doorlogBehaviour.MayAddDoorlog($"Apartment {activeUnitNumber}", mode: false);
			});
			GameManager.TimeSlinger.FireTimer(10f, delegate
			{
				doorlogBehaviour.MayAddDoorlog($"Apartment {activeUnitNumber}", mode: true);
			});
			EnemyStateManager.RemoveEnemyState(ENEMY_STATE.DOLL_MAKER);
			PowerStateManager.RemovePowerStateLock(STATE_LOCK_OCCASION.DOLL_MAKER);
			DoLucasHolmes();
			return;
		}
		GameManager.TimeSlinger.FireTimer(5f, delegate
		{
			doorlogBehaviour.MayAddDoorlog($"Apartment {activeUnitNumber}", mode: false);
		});
		PowerStateManager.AddPowerStateLock(STATE_LOCK_OCCASION.DOLL_MAKER);
		DataManager.LockSave = true;
		DataManager.ClearGameData();
		CamHookBehaviour.Interruptions = true;
		CamHookBehaviour.SwitchCameraStatus(enabled: false);
		if (StateManager.PlayerState == PLAYER_STATE.DESK || StateManager.PlayerState == PLAYER_STATE.COMPUTER)
		{
			if (StateManager.PlayerState == PLAYER_STATE.COMPUTER)
			{
				dollMakerBehaviour.SpawnBehindDesk();
				computerController.Ins.LeaveEvents.Event += triggerComputerJump;
				return;
			}
			GameManager.InteractionManager.LockInteraction();
			dollMakerBehaviour.SpawnBehindDesk();
			triggerComputerJump();
			DollMakerDeskJumper.Ins.TriggerDeskJump();
		}
		else if (StateManager.PlayerLocation != PLAYER_LOCATION.DEAD_DROP && StateManager.PlayerLocation != PLAYER_LOCATION.DEAD_DROP_ROOM)
		{
			dollMakerBehaviour.TriggerAnim("triggerUniJumpIdle");
			dollMakerBehaviour.NotInMeshEvents.Event += triggerBehindPlayerJump;
			dollMakerBehaviour.InMeshEvents.Event += reRollBehindPlayerJump;
			dollMakerBehaviour.AttemptSpawnBehindPlayer(roamController.Ins.transform, 0.93429f);
		}
		else
		{
			StateManager.PlayerLocationChangeEvents.Event += triggerMarkerTimesUp;
		}
	}

	private void DoLucasHolmes()
	{
		if (StateManager.PlayerState != PLAYER_STATE.COMPUTER)
		{
			GameManager.TimeSlinger.FireTimer(5f, DoLucasHolmes);
		}
		else if (DifficultyManager.Nightmare || DifficultyManager.LeetMode)
		{
			EnemyStateManager.AddEnemyState(ENEMY_STATE.HITMAN);
			EnemyManager.HitManManager.StageLucasHolmes();
		}
		else
		{
			HitmanProxyBehaviour.FromElevator = Random.Range(0, 2) == 0;
			EnemyManager.HitManManager.HitmanEventDone(5f);
		}
	}

	private void triggerBehindPlayerJump()
	{
		dollMakerBehaviour.NotInMeshEvents.Event -= triggerBehindPlayerJump;
		dollMakerBehaviour.InMeshEvents.Event -= reRollBehindPlayerJump;
		GameManager.AudioSlinger.PlaySoundWithCustomDelay(LookUp.SoundLookUp.JumpHit5, 0.2f);
		dollMakerBehaviour.SpawnBehindPlayer(roamController.Ins.transform, 0.93429f);
		DollMakerRoamJumper.Ins.TriggerUniJump();
		dollMakerBehaviour.TriggerUniJump();
		GameManager.TimeSlinger.FireTimer(6.5f, delegate
		{
			MainCameraHook.Ins.ClearARF(1f);
			UIManager.TriggerGameOver("YOU DISAPPOINTED THE DOLL MAKER");
		});
	}

	private void reRollBehindPlayerJump()
	{
		dollMakerBehaviour.NotInMeshEvents.Event -= triggerBehindPlayerJump;
		dollMakerBehaviour.InMeshEvents.Event -= reRollBehindPlayerJump;
		GameManager.TimeSlinger.FireTimer(0.2f, triggerMarkerTimesUp);
	}

	private void triggerComputerJump()
	{
		ComputerChairObject.Ins.Hide();
		computerController.Ins.LeaveEvents.Event -= triggerComputerJump;
		DollMakerDeskJumper.Ins.TriggerDeskJump();
		GameManager.TimeSlinger.FireTimer(0.1f, delegate
		{
			GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.JumpHit5);
			dollMakerBehaviour.TriggerDeskJump();
		});
		GameManager.TimeSlinger.FireTimer(5.75f, delegate
		{
			UIManager.TriggerGameOver("YOU DISAPPOINTED THE DOLL MAKER");
		});
	}

	private void nextMarkerCheck()
	{
		if ((StateManager.PlayerLocation == PLAYER_LOCATION.MAIN_ROON || StateManager.PlayerLocation == PLAYER_LOCATION.BATH_ROOM) && activeUnitNumber >= 600 && !myDollMakerData.PlayedHelpMeSound)
		{
			EnvironmentManager.PowerBehaviour.PowerOutageHub.PlaySound(helpMeSFX);
			myDollMakerData.PlayedHelpMeSound = true;
			GameManager.TimeSlinger.FireTimer(30f, delegate
			{
				doorlogBehaviour.MayAddDoorlog($"Apartment {activeUnitNumber}", mode: false);
			});
		}
		EnemyStateManager.RemoveEnemyState(ENEMY_STATE.DOLL_MAKER);
		myDollMakerData.CurrentVictims++;
		myDollMakerData.UsedUnitNumbers.Add(activeUnitNumber);
		PriceUnit = activeUnitNumber;
		for (int num = 0; num < markerTriggers.Length; num++)
		{
			if (markerTriggers[num].UnitNumber == activeUnitNumber)
			{
				markerTriggers[num].LockOut();
				num = markerTriggers.Length;
			}
		}
		activeUnitNumber = 0;
		myDollMakerData.ActiveUnitNumber = 0;
		if (myDollMakerData.CurrentVictims < myData.TargetVictimCount)
		{
			float duration = Random.Range(myData.MarkerResetTimeMin / 2f, myData.MarkerResetTimeMax / 2f);
			GameManager.TimeSlinger.FireTimer(duration, rePlaceMarker);
			DOSCoinsCurrencyManager.AddCurrency(GameManager.ManagerSlinger.TenantTrackManager.CheckDollMakerPrice(PriceUnit));
		}
		else
		{
			if (SteamSlinger.Ins != null)
			{
				SteamSlinger.Ins.UnlockSteamAchievement(STEAM_ACHIEVEMENT.DOLLMAKERPET);
			}
			DOSCoinsCurrencyManager.AddCurrency(200f);
			myDollMakerData.IsSatisfied = true;
		}
		DataManager.Save(myDollMakerData);
	}

	private void rePlaceMarker()
	{
		if (!lucassed)
		{
			StateManager.PlayerStateChangeEvents.Event -= rePlaceMarker;
			if (StateManager.PlayerState != PLAYER_STATE.COMPUTER)
			{
				StateManager.PlayerStateChangeEvents.Event += rePlaceMarker;
				return;
			}
			LookUp.Doors.MainDoor.AudioHub.PlaySound(makerQueSFX);
			LookUp.Doors.MainDoor.DisableDoorDollmaker();
			DollCameraFlicker();
			theMaker.MarkerWasPickedUp.Event += markerWasPickedUpForTheFirstTime;
			theMaker.SpawnMeTo(new Vector3(-2.5611f, 40.6517f, -5.0414f), Vector3.zero);
			activateMarkerTime();
		}
	}

	private void playerIsEnteringPeepHole()
	{
		GameManager.TimeSlinger.KillTimer(forcePowerTripTimer);
		peepHoleController.Ins.TakingOverEvents.Event -= playerIsEnteringPeepHole;
		GameManager.AudioSlinger.UnMuffleAudioHub(AUDIO_HUB.MANIKIN_HUB, 0.75f);
	}

	private void playerEnteredPeepHole()
	{
		peepHoleController.Ins.TookOverEvents.Event -= playerEnteredPeepHole;
		PauseManager.LockPause();
		peepHoleController.Ins.LockOutLeave = true;
		GameManager.TimeSlinger.FireTimer(2f, delegate
		{
			dollMakerBehaviour.TriggerAnim("triggerDollGrab");
		});
	}

	private void stageMe()
	{
		GameManager.StageManager.Stage -= stageMe;
		myDollMakerData = DataManager.Load<DollMakerData>(myID);
		if (myDollMakerData == null)
		{
			myDollMakerData = new DollMakerData(myID);
			myDollMakerData.IsReleased = false;
			myDollMakerData.IsActivated = false;
			myDollMakerData.CurrentVictims = 0;
			myDollMakerData.IsSatisfied = false;
			myDollMakerData.UsedUnitNumbers = new List<int>();
			myDollMakerData.ActiveUnitNumber = 0;
			myDollMakerData.PlayedHelpMeSound = false;
		}
	}

	private void threatsActivated()
	{
		GameManager.StageManager.ThreatsNowActivated -= threatsActivated;
		if (DifficultyManager.CasualMode || myDollMakerData.IsSatisfied)
		{
			return;
		}
		IsDollMakerActive = myDollMakerData.IsReleased;
		dollMakerActivated = myDollMakerData.IsActivated;
		activeUnitNumber = myDollMakerData.ActiveUnitNumber;
		if (IsDollMakerActive && !dollMakerActivated)
		{
			generateReleaseWindow();
		}
		else
		{
			if (!IsDollMakerActive || !dollMakerActivated)
			{
				return;
			}
			for (int i = 0; i < myDollMakerData.UsedUnitNumbers.Count; i++)
			{
				for (int j = 0; j < markerTriggers.Length; j++)
				{
					if (myDollMakerData.UsedUnitNumbers[i] == markerTriggers[j].UnitNumber)
					{
						markerTriggers[j].LockOut();
						j = markerTriggers.Length;
					}
				}
			}
			if (activeUnitNumber > 0)
			{
				for (int k = 0; k < markerTriggers.Length; k++)
				{
					markerTriggers[k].DeActivate();
				}
			}
			else
			{
				UIInventoryManager.ShowDollMakerMarker();
				for (int l = 0; l < markerTriggers.Length; l++)
				{
					markerTriggers[l].Activate();
				}
			}
			activateMarkerTime();
		}
	}

	private void markerWasPickedUp()
	{
		activeUnitNumber = 0;
		for (int i = 0; i < markerTriggers.Length; i++)
		{
			markerTriggers[i].Activate();
		}
	}

	private void generateReleaseWindow(bool instantly)
	{
		if (!DifficultyManager.CasualMode)
		{
			if (instantly)
			{
				delayWindow = Random.Range(5, 10);
			}
			else
			{
				delayWindow = Random.Range(myData.DelayTimeMin, myData.DelayTimeMax);
			}
			delayTimeStamp = Time.time;
			delayActive = true;
		}
	}

	public void ForceMarker()
	{
		if (!DifficultyManager.CasualMode && !forced && !IsDollMakerActive)
		{
			IsDollMakerActive = true;
			forced = true;
			StateManager.PlayerStateChangeEvents.Event -= rePlaceMarker;
			if (StateManager.PlayerState == PLAYER_STATE.COMPUTER)
			{
				LookUp.Doors.MainDoor.AudioHub.PlaySound(makerQueSFX);
				LookUp.Doors.MainDoor.DisableDoorDollmaker();
				DollCameraFlicker();
				theMaker.MarkerWasPickedUp.Event += markerWasPickedUpForTheFirstTime;
				theMaker.SpawnMeTo(new Vector3(-2.5611f, 40.6517f, -5.0414f), Vector3.zero);
				activateMarkerTime();
				TarotVengeance.ActivateEnemy(ENEMY_STATE.DOLL_MAKER);
			}
			else
			{
				StateManager.PlayerStateChangeEvents.Event += rePlaceMarker;
			}
		}
	}

	private void DollCameraFlicker()
	{
		CamHookBehaviour.Interruptions = true;
		CamHookBehaviour.SwitchCameraStatus(enabled: false);
		GameManager.TimeSlinger.FireTimer(1.5f, delegate
		{
			CamHookBehaviour.Interruptions = false;
			CamHookBehaviour.SwitchCameraStatus(enabled: true);
		});
	}

	public void DevEnemyTimerChange(float time)
	{
		if (markerActive)
		{
			markerWindow = time;
			markerTimeStamp = Time.time;
		}
	}
}
