using UnityEngine;
using UnityEngine.Events;

public class HitManManager : MonoBehaviour
{
	[SerializeField]
	private HitmanDataDefinition data;

	public UnityEvent BeginPatrolEvents = new UnityEvent();

	[SerializeField]
	private HitmanSpawnTrigger failSafeTrigger;

	[SerializeField]
	private EnemyHotZoneTrigger[] hotZoneTriggers = new EnemyHotZoneTrigger[0];

	private HitmanBathroomJump bathroomJump = new HitmanBathroomJump();

	private float fireTimeStamp;

	private float fireWindow;

	private bool fireWindowActive;

	private HitmanFloor10Jump floor10Jump = new HitmanFloor10Jump();

	private HitmanFloor1Jump floor1Jump = new HitmanFloor1Jump();

	private HitmanFloor3Jump floor3Jump = new HitmanFloor3Jump();

	private HitmanFloor5Jump floor5Jump = new HitmanFloor5Jump();

	private HitmanFloor6Jump floor6Jump = new HitmanFloor6Jump();

	private HitmanFloor8Jump floor8Jump = new HitmanFloor8Jump();

	private bool hitmanActivated;

	private int keyDiscoverCount;

	private HitmanLobbyComputerJump lobbyComputerJump = new HitmanLobbyComputerJump();

	private Timer lockPickTimer;

	private HitmanMainDoorJump mainDoorJump = new HitmanMainDoorJump();

	private HitmanMainDoorOutsideJump mainDoorOutsideJump = new HitmanMainDoorOutsideJump();

	private HitManData myHitmanData;

	private bool proxyHitSpawn;

	private HitmanStairWayDoor10Jump stairwayDoor10Jump = new HitmanStairWayDoor10Jump();

	private HitmanStairWayDoor1Jump stairwayDoor1Jump = new HitmanStairWayDoor1Jump();

	private HitmanStairWayDoor3Jump stairwayDoor3Jump = new HitmanStairWayDoor3Jump();

	private HitmanStairWayDoor5Jump stairwayDoor5Jump = new HitmanStairWayDoor5Jump();

	private HitmanStairWayDoor6Jump stairwayDoor6Jump = new HitmanStairWayDoor6Jump();

	private HitmanStairWayDoor8Jump stairwayDoor8Jump = new HitmanStairWayDoor8Jump();

	private float leeywayWindow;

	private float leeywayStamp;

	private bool jumpIsAdded;

	private LucasLaserManager laser;

	public Timer LockPickTimer => lockPickTimer;

	public HitmanDataDefinition Data => data;

	public string LucasDebug
	{
		get
		{
			if (TarotVengeance.Killed(ENEMY_STATE.HITMAN))
			{
				return "-2";
			}
			return (fireWindow - (Time.time - fireTimeStamp) > 0f) ? ((int)(fireWindow - (Time.time - fireTimeStamp))).ToString() : "-1";
		}
	}

	public string LeeyWayDebug => getLeeyway();

	private string getLeeyway()
	{
		return (leeywayWindow - (Time.time - leeywayStamp) >= 0f && leeywayWindow - (Time.time - leeywayStamp) <= data.NotInMainRoomLeeyWayTime) ? (leeywayWindow - (Time.time - leeywayStamp)).ToString() : "NaN";
	}

	private void Awake()
	{
		EnemyManager.HitManManager = this;
		GameManager.StageManager.Stage += stageMe;
		GameManager.StageManager.ThreatsNowActivated += threatsActivated;
		GameManager.TheCloud.KeyDiscoveredEvent.Event += keyWasDiscovered;
	}

	private void Update()
	{
		if (!DifficultyManager.CasualMode && !TarotVengeance.Killed(ENEMY_STATE.HITMAN) && fireWindowActive)
		{
			myHitmanData.TimeLeftOnWindow = fireWindow - (Time.time - fireTimeStamp);
			if (Time.time - fireTimeStamp >= fireWindow)
			{
				fireWindowActive = false;
				stageHitmanPatrol();
			}
		}
	}

	private void OnDestroy()
	{
		if (jumpIsAdded)
		{
			jumpIsAdded = false;
			LookUp.Doors.MainDoor.DoorOpenEvent.RemoveListener(mainDoorJump.Stage);
			LookUp.Doors.MainDoor.DoorWasOpenedEvent.RemoveListener(mainDoorJump.Execute);
			Debug.Log("[HitmanManager] Removed Jump");
		}
		CancelInvoke("saveMyData");
	}

	public void SpawnHitman(HitmanSpawnDefinition SpawnData)
	{
		Debug.Log("Unused now: SpawnHitman");
	}

	public void GunFlashGameOver()
	{
		DataManager.ClearGameData();
		MainCameraHook.Ins.ClearARF();
		HitmanBehaviour.Ins.GunFlashDoneEvents.Event -= GunFlashGameOver;
		UIManager.TriggerHardGameOver("ASSASSINATED");
		HitmanRoamJumper.Ins.ClearPPVol();
	}

	public void ProxyHitEnd()
	{
		Debug.Log("Unused now: ProxyHitEnd");
	}

	public void DeSpawn()
	{
		EnemyStateManager.RemoveEnemyState(ENEMY_STATE.HITMAN);
		PowerStateManager.RemovePowerStateLock(STATE_LOCK_OCCASION.HITMAN);
		DataManager.LockSave = false;
		CamHookBehaviour.Interruptions = false;
		CamHookBehaviour.SwitchCameraStatus(enabled: true, silently: true);
		generateFireWindow();
	}

	public void ExecuteLobbyComputerJump()
	{
		lobbyComputerJump.Execute();
	}

	private void DoHitmanEvent()
	{
		Debug.Log("[Lucas+MEGA] Spawning proxy event...");
		HitmanProxyBehaviour.Ins.TriggerPath();
		GameManager.TimeSlinger.FireTimer(10f, delegate
		{
			if ((StateManager.PlayerLocation == PLAYER_LOCATION.BATH_ROOM || StateManager.PlayerLocation == PLAYER_LOCATION.OUTSIDE || StateManager.PlayerLocation == PLAYER_LOCATION.MAIN_ROON) && !LookUp.Doors.MainDoor.IsOpen && !jumpIsAdded)
			{
				jumpIsAdded = true;
				LookUp.Doors.MainDoor.DoorOpenEvent.AddListener(mainDoorJump.Stage);
				LookUp.Doors.MainDoor.DoorWasOpenedEvent.AddListener(mainDoorJump.Execute);
				Debug.Log("[HitmanManager] Added Jump");
			}
			if (!HitmanProxyBehaviour.FromElevator)
			{
				doorlogBehaviour.MayAddDoorlog("Floor 8 Hallway", mode: true);
				GameManager.TimeSlinger.FireTimer(4f, delegate
				{
					doorlogBehaviour.MayAddDoorlog("Floor 8 Hallway", mode: false);
				});
			}
		});
	}

	public void stageHitmanPatrol()
	{
		if (DifficultyManager.CasualMode || TarotVengeance.Killed(ENEMY_STATE.HITMAN))
		{
			return;
		}
		TarotVengeance.ActivateEnemy(ENEMY_STATE.HITMAN);
		if (EnemyStateManager.IsInEnemyStateOrLocked())
		{
			fireWindow /= 2f;
			fireWindowActive = true;
		}
		else if (EnvironmentManager.PowerState != POWER_STATE.ON)
		{
			fireWindow *= 0.35f;
			fireWindowActive = true;
		}
		else if (!StateManager.BeingHacked)
		{
			if (!hitmanActivated)
			{
				hitmanActivated = true;
			}
			EnemyStateManager.AddEnemyState(ENEMY_STATE.HITMAN);
			PowerStateManager.AddPowerStateLock(STATE_LOCK_OCCASION.HITMAN);
			DataManager.LockSave = true;
			triggerHitmanPatrol();
		}
		else
		{
			fireWindow = 30f;
			fireWindowActive = true;
		}
	}

	private void triggerHitmanPatrol()
	{
		if (!DifficultyManager.CasualMode && !TarotVengeance.Killed(ENEMY_STATE.HITMAN))
		{
			DoHitmanEvent();
		}
	}

	public void HitmanEventDone(float delay = 0f)
	{
		EnemyStateManager.AddEnemyState(ENEMY_STATE.HITMAN);
		PowerStateManager.AddPowerStateLock(STATE_LOCK_OCCASION.HITMAN);
		if (delay <= 0.5f)
		{
			spawn();
		}
		else
		{
			GameManager.TimeSlinger.FireTimer(delay, spawn);
		}
	}

	private void spawn()
	{
		if (DifficultyManager.CasualMode || TarotVengeance.Killed(ENEMY_STATE.HITMAN))
		{
			return;
		}
		if (LookUp.Doors.MainDoor.DoingSomething && !LookUp.Doors.MainDoor.dollmakerHeadOn)
		{
			GameManager.TimeSlinger.FireTimer(0.5f, spawn);
			return;
		}
		if (StateManager.PlayerState == PLAYER_STATE.BUSY)
		{
			StateManager.PlayerStateChangeEvents.Event += spawn;
			return;
		}
		StateManager.PlayerStateChangeEvents.Event -= spawn;
		if (StateManager.PlayerLocation == PLAYER_LOCATION.UNKNOWN)
		{
			StateManager.PlayerLocationChangeEvents.Event += spawn;
			return;
		}
		StateManager.PlayerLocationChangeEvents.Event -= spawn;
		Debug.Log("[Lucas+] Spawning Lucas");
		if (StateManager.PlayerLocation == PLAYER_LOCATION.BATH_ROOM || StateManager.PlayerLocation == PLAYER_LOCATION.OUTSIDE || StateManager.PlayerLocation == PLAYER_LOCATION.MAIN_ROON)
		{
			if (!jumpIsAdded)
			{
				jumpIsAdded = true;
				LookUp.Doors.MainDoor.DoorOpenEvent.AddListener(mainDoorJump.Stage);
				LookUp.Doors.MainDoor.DoorWasOpenedEvent.AddListener(mainDoorJump.Execute);
				Debug.Log("[HitmanManager] Added Jump");
			}
			HitmanBehaviour.Ins.ReachedEndPath.Event += mainDoorSpawnAction;
			HitmanBehaviour.Ins.Spawn();
			Debug.Log("[Lucas+] Spawning Lucas 2");
		}
		else
		{
			GameManager.TimeSlinger.FireTimer(data.NotInMainRoomLeeyWayTime, subSpawnCheck);
			leeywayWindow = data.NotInMainRoomLeeyWayTime;
			leeywayStamp = Time.time;
		}
	}

	private void subSpawnCheck()
	{
		if (LookUp.Doors.MainDoor.DoingSomething && !LookUp.Doors.MainDoor.dollmakerHeadOn)
		{
			GameManager.TimeSlinger.FireTimer(0.5f, subSpawnCheck);
			return;
		}
		if (StateManager.PlayerState == PLAYER_STATE.BUSY)
		{
			StateManager.PlayerStateChangeEvents.Event += subSpawnCheck;
			return;
		}
		StateManager.PlayerStateChangeEvents.Event -= subSpawnCheck;
		if (StateManager.PlayerLocation == PLAYER_LOCATION.UNKNOWN)
		{
			StateManager.PlayerLocationChangeEvents.Event += subSpawnCheck;
			return;
		}
		StateManager.PlayerLocationChangeEvents.Event -= subSpawnCheck;
		Debug.Log("[Lucas+] Sub Spawn Check");
		if (StateManager.PlayerLocation == PLAYER_LOCATION.BATH_ROOM || StateManager.PlayerLocation == PLAYER_LOCATION.OUTSIDE || StateManager.PlayerLocation == PLAYER_LOCATION.MAIN_ROON)
		{
			if (!jumpIsAdded)
			{
				jumpIsAdded = true;
				LookUp.Doors.MainDoor.DoorOpenEvent.AddListener(mainDoorJump.Stage);
				LookUp.Doors.MainDoor.DoorWasOpenedEvent.AddListener(mainDoorJump.Execute);
				Debug.Log("[HitmanManager] Added Jump");
			}
			HitmanBehaviour.Ins.ReachedEndPath.Event += mainDoorSpawnAction;
			HitmanBehaviour.Ins.Spawn();
		}
		else
		{
			outSideMainRoomAction();
		}
	}

	private void mainDoorSpawnAction()
	{
		Debug.Log("[Lucas+] Main door spawn action Lucas");
		HitmanBehaviour.Ins.ReachedEndPath.Event -= mainDoorSpawnAction;
		GameManager.TimeSlinger.FireTimer(1f, mainDoorTriggerAction);
	}

	private void Start()
	{
		laser = new GameObject("LaserManager").AddComponent<LucasLaserManager>();
	}

	private void mainRoomAttackAction()
	{
		Debug.Log("[Lucas+] Main door attack action Lucas");
		if (StateManager.PlayerState == PLAYER_STATE.BUSY)
		{
			Debug.Log("[Lucas+] State busy, try again later");
			StateManager.PlayerStateChangeEvents.Event += mainRoomAttackAction;
			return;
		}
		if (KeypadManager.Locked)
		{
			Debug.Log("[Lucas+] Keypad despawn");
			if (jumpIsAdded)
			{
				jumpIsAdded = false;
				LookUp.Doors.MainDoor.DoorOpenEvent.RemoveListener(mainDoorJump.Stage);
				LookUp.Doors.MainDoor.DoorWasOpenedEvent.RemoveListener(mainDoorJump.Execute);
				Debug.Log("[HitmanManager] Removed Jump");
			}
			EnemyStateManager.RemoveEnemyState(ENEMY_STATE.HITMAN);
			PowerStateManager.RemovePowerStateLock(STATE_LOCK_OCCASION.HITMAN);
			DataManager.LockSave = false;
			HitmanBehaviour.Ins.TriggerAnim("idle");
			HitmanBehaviour.Ins.WalkAwayFromMainDoor();
			StateManager.PlayerStateChangeEvents.Event -= mainRoomAttackAction;
			GameManager.TimeSlinger.FireTimer(4f, HitmanBehaviour.Ins.DeSpawn);
			Debug.Log("[Lucas+] Staging Laser");
			GameManager.TimeSlinger.FireTimer(Random.Range(25f, 65f), LaserEntryPoint);
			return;
		}
		Debug.Log("[Lucas+] No keypad");
		CamHookBehaviour.Interruptions = true;
		CamHookBehaviour.SwitchCameraStatus(enabled: false);
		HitmanBehaviour.Ins.TriggerAnim("idle");
		StateManager.PlayerStateChangeEvents.Event -= mainRoomAttackAction;
		if (StateManager.PlayerLocation == PLAYER_LOCATION.OUTSIDE)
		{
			LucasPlusManager.Ins.StageBalconyDoorJump();
		}
		if (StateManager.PlayerState == PLAYER_STATE.HIDING)
		{
			Debug.Log("[Lucas+] Start Patrol");
			if (jumpIsAdded)
			{
				jumpIsAdded = false;
				LookUp.Doors.MainDoor.DoorOpenEvent.RemoveListener(mainDoorJump.Stage);
				LookUp.Doors.MainDoor.DoorWasOpenedEvent.RemoveListener(mainDoorJump.Execute);
				Debug.Log("[HitmanManager] Removed Jump");
			}
			HitmanPatrolBehaviour.Ins.Patrol();
		}
		else if (StateManager.PlayerLocation == PLAYER_LOCATION.BATH_ROOM && !LookUp.Doors.BathroomDoor.DoingSomething)
		{
			bathroomJump.Stage();
			LookUp.Doors.BathroomDoor.DoorOpenEvent.AddListener(bathroomJump.Execute);
		}
		else if (StateManager.PlayerState == PLAYER_STATE.COMPUTER)
		{
			HitmanComputerJumper.Ins.AddComputerJump();
		}
		else
		{
			if (StateManager.PlayerState == PLAYER_STATE.PEEPING)
			{
				HitmanBehaviour.Ins.WalkAwayFromMainDoor();
			}
			HitmanPeepJumper.Ins.AddPeepJump();
			LookUp.Doors.BathroomDoor.DoorWasClosedEvent.AddListener(checkBathroomDoorJump);
			HitmanComputerJumper.Ins.AddDelayComputerJump();
		}
	}

	public void LaserDespawned()
	{
		EnemyStateManager.RemoveEnemyState(ENEMY_STATE.HITMAN);
		PowerStateManager.RemovePowerStateLock(STATE_LOCK_OCCASION.HITMAN);
		DataManager.LockSave = false;
		if (jumpIsAdded)
		{
			jumpIsAdded = false;
			LookUp.Doors.MainDoor.DoorOpenEvent.RemoveListener(mainDoorJump.Stage);
			LookUp.Doors.MainDoor.DoorWasOpenedEvent.RemoveListener(mainDoorJump.Execute);
			Debug.Log("[HitmanManager] Removed Jump");
		}
		generateFireWindow();
	}

	private void LaserEntryPoint()
	{
		if ((StateManager.PlayerLocation == PLAYER_LOCATION.OUTSIDE || StateManager.PlayerLocation == PLAYER_LOCATION.BATH_ROOM || StateManager.PlayerLocation == PLAYER_LOCATION.MAIN_ROON) && StateManager.PlayerState == PLAYER_STATE.COMPUTER)
		{
			laser.SpawnLaser();
		}
		else
		{
			GameManager.TimeSlinger.FireTimer(10f, LaserEntryPoint);
		}
	}

	private void checkBathroomDoorJump()
	{
		if (StateManager.PlayerLocation == PLAYER_LOCATION.BATH_ROOM)
		{
			LookUp.Doors.BathroomDoor.DoorWasClosedEvent.RemoveListener(checkBathroomDoorJump);
			bathroomJump.Stage();
			LookUp.Doors.BathroomDoor.DoorOpenEvent.AddListener(bathroomJump.Execute);
		}
	}

	private void generateFireWindow()
	{
		if (!DifficultyManager.CasualMode && !TarotVengeance.Killed(ENEMY_STATE.HITMAN))
		{
			fireWindow = Random.Range(data.FireWindowMin, data.FireWindowMax);
			if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
			{
				fireWindow *= 0.4f;
			}
			fireTimeStamp = Time.time;
			fireWindowActive = true;
		}
	}

	public void StageLucasHolmes()
	{
		HitmanLobbyComputerJump.ATApartment = true;
		lobbyComputerJump.Stage();
	}

	private void outSideMainRoomAction()
	{
		LookUp.Doors.MainDoor.DoorOpenEvent.AddListener(mainDoorOutsideJump.Stage);
		LookUp.Doors.MainDoor.DoorWasOpenedEvent.AddListener(mainDoorOutsideJump.Execute);
		if (StateManager.PlayerState == PLAYER_STATE.LOBBY_COMPUTER)
		{
			lobbyComputerJump.Stage();
			HitmanRoamJumper.Ins.AddLobbyComputerJump();
			return;
		}
		switch (StateManager.PlayerLocation)
		{
		case PLAYER_LOCATION.HALL_WAY10:
			LookUp.Doors.Door10.DoorOpenEvent.AddListener(floor10Jump.Stage);
			LookUp.Doors.Door10.DoorWasOpenedEvent.AddListener(floor10Jump.Execute);
			break;
		case PLAYER_LOCATION.HALL_WAY8:
			LookUp.Doors.Door8.DoorOpenEvent.AddListener(floor8Jump.Stage);
			LookUp.Doors.Door8.DoorWasOpenedEvent.AddListener(floor8Jump.Execute);
			break;
		case PLAYER_LOCATION.HALL_WAY6:
			LookUp.Doors.Door6.DoorOpenEvent.AddListener(floor6Jump.Stage);
			LookUp.Doors.Door6.DoorWasOpenedEvent.AddListener(floor6Jump.Execute);
			break;
		case PLAYER_LOCATION.HALL_WAY5:
			LookUp.Doors.Door5.DoorOpenEvent.AddListener(floor5Jump.Stage);
			LookUp.Doors.Door5.DoorWasOpenedEvent.AddListener(floor5Jump.Execute);
			break;
		case PLAYER_LOCATION.HALL_WAY3:
			LookUp.Doors.Door3.DoorOpenEvent.AddListener(floor3Jump.Stage);
			LookUp.Doors.Door3.DoorWasOpenedEvent.AddListener(floor3Jump.Execute);
			break;
		case PLAYER_LOCATION.HALL_WAY1:
			LookUp.Doors.Door1.DoorOpenEvent.AddListener(floor1Jump.Stage);
			LookUp.Doors.Door1.DoorWasOpenedEvent.AddListener(floor1Jump.Execute);
			break;
		case PLAYER_LOCATION.STAIR_WAY:
			LookUp.Doors.Door1.DoorOpenEvent.AddListener(stairwayDoor1Jump.Stage);
			LookUp.Doors.Door1.DoorWasOpenedEvent.AddListener(stairwayDoor1Jump.Execute);
			LookUp.Doors.Door3.DoorOpenEvent.AddListener(stairwayDoor3Jump.Stage);
			LookUp.Doors.Door3.DoorWasOpenedEvent.AddListener(stairwayDoor3Jump.Execute);
			LookUp.Doors.Door5.DoorOpenEvent.AddListener(stairwayDoor5Jump.Stage);
			LookUp.Doors.Door5.DoorWasOpenedEvent.AddListener(stairwayDoor5Jump.Execute);
			LookUp.Doors.Door6.DoorOpenEvent.AddListener(stairwayDoor6Jump.Stage);
			LookUp.Doors.Door6.DoorWasOpenedEvent.AddListener(stairwayDoor6Jump.Execute);
			LookUp.Doors.Door8.DoorOpenEvent.AddListener(stairwayDoor8Jump.Stage);
			LookUp.Doors.Door8.DoorWasOpenedEvent.AddListener(stairwayDoor8Jump.Execute);
			LookUp.Doors.Door10.DoorOpenEvent.AddListener(stairwayDoor10Jump.Stage);
			LookUp.Doors.Door10.DoorWasOpenedEvent.AddListener(stairwayDoor10Jump.Execute);
			break;
		case PLAYER_LOCATION.MAINTENANCE_ROOM:
			LookUp.Doors.Door1.DoorOpenEvent.AddListener(stairwayDoor1Jump.Stage);
			LookUp.Doors.Door1.DoorWasOpenedEvent.AddListener(stairwayDoor1Jump.Execute);
			LookUp.Doors.Door3.DoorOpenEvent.AddListener(stairwayDoor3Jump.Stage);
			LookUp.Doors.Door3.DoorWasOpenedEvent.AddListener(stairwayDoor3Jump.Execute);
			LookUp.Doors.Door5.DoorOpenEvent.AddListener(stairwayDoor5Jump.Stage);
			LookUp.Doors.Door5.DoorWasOpenedEvent.AddListener(stairwayDoor5Jump.Execute);
			LookUp.Doors.Door6.DoorOpenEvent.AddListener(stairwayDoor6Jump.Stage);
			LookUp.Doors.Door6.DoorWasOpenedEvent.AddListener(stairwayDoor6Jump.Execute);
			LookUp.Doors.Door8.DoorOpenEvent.AddListener(stairwayDoor8Jump.Stage);
			LookUp.Doors.Door8.DoorWasOpenedEvent.AddListener(stairwayDoor8Jump.Execute);
			LookUp.Doors.Door10.DoorOpenEvent.AddListener(stairwayDoor10Jump.Stage);
			LookUp.Doors.Door10.DoorWasOpenedEvent.AddListener(stairwayDoor10Jump.Execute);
			break;
		case PLAYER_LOCATION.LOBBY:
			LookUp.Doors.Door1.DoorOpenEvent.AddListener(floor1Jump.Stage);
			LookUp.Doors.Door1.DoorWasOpenedEvent.AddListener(floor1Jump.Execute);
			break;
		case PLAYER_LOCATION.DEAD_DROP:
			LookUp.Doors.Door1.DoorOpenEvent.AddListener(floor1Jump.Stage);
			LookUp.Doors.Door1.DoorWasOpenedEvent.AddListener(floor1Jump.Execute);
			break;
		case PLAYER_LOCATION.LOBBY_COMPUTER:
			LookUp.Doors.Door1.DoorOpenEvent.AddListener(floor1Jump.Stage);
			LookUp.Doors.Door1.DoorWasOpenedEvent.AddListener(floor1Jump.Execute);
			break;
		case PLAYER_LOCATION.DEAD_DROP_ROOM:
			LookUp.Doors.Door1.DoorOpenEvent.AddListener(floor1Jump.Stage);
			LookUp.Doors.Door1.DoorWasOpenedEvent.AddListener(floor1Jump.Execute);
			break;
		case PLAYER_LOCATION.OUTSIDE:
			break;
		}
	}

	private void saveMyData()
	{
		DataManager.Save(myHitmanData);
	}

	private void stageMe()
	{
		GameManager.StageManager.Stage -= stageMe;
		myHitmanData = DataManager.Load<HitManData>(7);
		if (myHitmanData == null)
		{
			myHitmanData = new HitManData(7);
			myHitmanData.IsActivated = false;
			myHitmanData.KeysDiscoveredCount = 0;
			myHitmanData.TimeLeftOnWindow = 0f;
		}
		keyDiscoverCount = myHitmanData.KeysDiscoveredCount;
		InvokeRepeating("saveMyData", 0f, 30f);
	}

	private void threatsActivated()
	{
		GameManager.StageManager.ThreatsNowActivated -= threatsActivated;
		if (myHitmanData.IsActivated)
		{
			hitmanActivated = true;
			if (myHitmanData.TimeLeftOnWindow >= 10f)
			{
				fireTimeStamp = Time.time;
				fireWindow = myHitmanData.TimeLeftOnWindow;
				fireWindowActive = true;
			}
			else
			{
				generateFireWindow();
				TarotVengeance.ActivateEnemy(ENEMY_STATE.HITMAN);
			}
		}
	}

	private void keyWasDiscovered()
	{
		if (!hitmanActivated)
		{
			keyDiscoverCount++;
			myHitmanData.KeysDiscoveredCount++;
			if (keyDiscoverCount >= data.KeysRequiredToTrigger)
			{
				hitmanActivated = true;
				myHitmanData.IsActivated = true;
				generateFireWindow();
				TarotVengeance.ActivateEnemy(ENEMY_STATE.HITMAN);
			}
			DataManager.Save(myHitmanData);
		}
	}

	public void ReleaseTheHitman()
	{
		if (!hitmanActivated)
		{
			hitmanActivated = true;
			myHitmanData.IsActivated = true;
			generateFireWindow();
			TarotVengeance.ActivateEnemy(ENEMY_STATE.HITMAN);
		}
	}

	public void LucassedJump()
	{
		EnemyStateManager.AddEnemyState(ENEMY_STATE.HITMAN);
		PowerStateManager.AddPowerStateLock(STATE_LOCK_OCCASION.HITMAN);
		LookUp.Doors.MainDoor.DoorOpenEvent.AddListener(mainDoorOutsideJump.Stage);
		LookUp.Doors.MainDoor.DoorWasOpenedEvent.AddListener(mainDoorOutsideJump.Execute);
	}

	private void mainDoorTriggerAction()
	{
		if (!EnemyStateManager.HasEnemyState(ENEMY_STATE.HITMAN))
		{
			Debug.Log("[Lucas+] Something weird is going on...");
			Debug.Log(EnemyStateManager.EnemyStateDebug);
			Debug.Log(EnemyStateManager.LockedStateDebug);
			Debug.Log(EnvironmentManager.PowerState);
			EnemyStateManager.AddEnemyState(ENEMY_STATE.HITMAN);
			PowerStateManager.AddPowerStateLock(STATE_LOCK_OCCASION.HITMAN);
			DataManager.LockSave = true;
		}
		if (LookUp.Doors.MainDoor.Locked)
		{
			LookUp.Doors.MainDoor.AudioHub.PlaySound(LookUp.SoundLookUp.DoorKnobSFX);
			HitmanBehaviour.Ins.TriggerAnim("lockPick");
			GameManager.TimeSlinger.FireHardTimer(out lockPickTimer, data.LockPickingTime, mainRoomAttackAction);
		}
		else
		{
			mainRoomAttackAction();
		}
	}

	public void DevEnemyTimerChange(float time)
	{
		if (fireWindowActive)
		{
			fireWindow = time;
			fireTimeStamp = Time.time;
		}
	}
}
