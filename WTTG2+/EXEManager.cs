using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class EXEManager : MonoBehaviour
{
	public static EXEManager Ins;

	private CustomEvent doorJumpStage = new CustomEvent(2);

	private float fireTimeStamp;

	private float fireWindow;

	private bool fireWindowActive;

	private bool gracePeriodOver;

	private bool IsActive;

	private bool jumpTriggered;

	private int keyDiscoverCount;

	private int maxPopSounds;

	public Animator myAnimator;

	public NavMeshAgent myNavMeshAgent;

	private roamController myRoamController;

	private bool PlayerMakesSound;

	private bool popListenerActive;

	private bool PUNCH;

	public string EXEDebug
	{
		get
		{
			if (TarotVengeance.Killed(ENEMY_STATE.EXECUTIONER))
			{
				return "-2";
			}
			return (fireWindow - (Time.time - fireTimeStamp) > 0f) ? ((int)(fireWindow - (Time.time - fireTimeStamp))).ToString() : "-1";
		}
	}

	private void Awake()
	{
		Ins = this;
		if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			ReleaseExecutor();
			TarotVengeance.ActivateEnemy(ENEMY_STATE.EXECUTIONER);
		}
	}

	private void Start()
	{
		GameObject gameObject = new GameObject("EXEDoorTrigger");
		gameObject.transform.position = new Vector3(-2.7143f, 40.5293f, -2.8887f);
		gameObject.transform.localScale = new Vector3(6f, 1f, 3.5f);
		gameObject.AddComponent<BoxCollider>().isTrigger = true;
		gameObject.AddComponent<EXEDoorTrigger>().playerEnterEvent.Event += PlayerEnteredDoorTrigger;
		myRoamController = GameObject.Find("roamController").GetComponent<roamController>();
	}

	private void Update()
	{
		if (!DifficultyManager.CasualMode && !TarotVengeance.Killed(ENEMY_STATE.EXECUTIONER))
		{
			if (fireWindowActive && Time.time - fireTimeStamp >= fireWindow)
			{
				fireWindowActive = false;
				StagePatrol();
			}
			bool flag = false;
			if (GameManager.BehaviourManager.AnnBehaviour.isPlayingAudio && !ComputerMuteBehaviour.Ins.Muted)
			{
				PlayerMakesSound = true;
			}
			else if (DeepWebRadioManager.RadioAS.volume >= 0.01f)
			{
				PlayerMakesSound = true;
			}
			else if (DeepWebRadioManager.RadioAS2.volume >= 0.01f)
			{
				PlayerMakesSound = true;
			}
			else
			{
				PlayerMakesSound = false;
			}
			if (PlayerMakesSound && gracePeriodOver)
			{
				TriggerJump();
			}
			if (popListenerActive && gracePeriodOver && EXESoundPopper.PoppedSounds > maxPopSounds)
			{
				TriggerJump();
			}
		}
	}

	private void OnDestroy()
	{
		Ins = null;
		HitmanComputerJumper.Ins.myComputerController.LeaveEvents.Event -= ComputerJump;
	}

	private void generateFireWindow()
	{
		if (!DifficultyManager.CasualMode && !TarotVengeance.Killed(ENEMY_STATE.EXECUTIONER))
		{
			fireWindow = Random.Range(300f, 600f);
			if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
			{
				fireWindow *= 0.9f;
			}
			fireTimeStamp = Time.time;
			fireWindowActive = true;
		}
	}

	public void ReleaseExecutor()
	{
		generateFireWindow();
	}

	private void StagePatrol()
	{
		if (!DifficultyManager.CasualMode && !TarotVengeance.Killed(ENEMY_STATE.EXECUTIONER) && !IsActive)
		{
			if (EnemyStateManager.IsInEnemyStateOrLocked())
			{
				fireWindow = 20f;
				fireTimeStamp = Time.time;
				fireWindowActive = true;
			}
			else if (EnvironmentManager.PowerState != POWER_STATE.ON)
			{
				fireWindow = 60f;
				fireTimeStamp = Time.time;
				fireWindowActive = true;
			}
			else if (StateManager.PlayerState != PLAYER_STATE.COMPUTER)
			{
				fireWindow = ((StateManager.PlayerLocation == PLAYER_LOCATION.MAIN_ROON || StateManager.PlayerLocation == PLAYER_LOCATION.BATH_ROOM || StateManager.PlayerLocation == PLAYER_LOCATION.OUTSIDE || StateManager.PlayerLocation == PLAYER_LOCATION.HALL_WAY8) ? 5f : 150f);
				fireTimeStamp = Time.time;
				fireWindowActive = true;
			}
			else if (GameManager.HackerManager.theSwan.SwanError)
			{
				fireWindow = 10f;
				fireTimeStamp = Time.time;
				fireWindowActive = true;
			}
			else if (StateManager.BeingHacked)
			{
				fireWindow = 20f;
				fireTimeStamp = Time.time;
				fireWindowActive = true;
			}
			else
			{
				IsActive = true;
				PowerStateManager.AddPowerStateLock(STATE_LOCK_OCCASION.EXECUTIONER);
				EnemyStateManager.AddEnemyState(ENEMY_STATE.EXECUTIONER);
				EXESoundPopper.PoppedSounds = 0;
				Spawn();
			}
		}
	}

	private void Spawn()
	{
		if (TarotVengeance.Killed(ENEMY_STATE.EXECUTIONER))
		{
			return;
		}
		float num = Random.Range(20, 35);
		GameManager.TimeSlinger.FireTimer(num / 5f, delegate
		{
			WalkingEXE.stageMe();
		});
		doorJumpStage.Event += DoorKickKill;
		EXEAHOManager aho = new GameObject("EXEAHOManager").AddComponent<EXEAHOManager>();
		if (EXEAHOManager.Ins != null)
		{
			EXEAHOManager.Ins.MoveMeToStairwayDoor(0f);
			EXEAHOManager.Ins.EXEPlaySound(CustomSoundLookUp.hallDoorOpenClose);
			doorlogBehaviour.MayAddDoorlog("Floor 8 Hallway", mode: true);
		}
		GameManager.TimeSlinger.FireTimer(4f, delegate
		{
			EXESoundPopper.PoppedSounds = 0;
			gracePeriodOver = true;
			maxPopSounds = Random.Range(16, 32);
			if (DifficultyManager.Nightmare)
			{
				maxPopSounds /= 2;
			}
			Debug.Log("[Executioner] Max pop sounds for this round: " + maxPopSounds);
			popListenerActive = true;
			if (!DifficultyManager.Nightmare)
			{
				Debug.Log("[Executioner] Not nightmare, playing whistle");
				if (EXEAHOManager.Ins != null)
				{
					EXEAHOManager.Ins.EXEPlaySound(CustomSoundLookUp.whistlingReverb);
					EXEAHOManager.Ins.MoveMeToElevator(num / 2f);
				}
			}
		});
		GameManager.TimeSlinger.FireTimer(num, delegate
		{
			if (!jumpTriggered)
			{
				gracePeriodOver = false;
				popListenerActive = false;
				maxPopSounds = 0;
				EXESoundPopper.PoppedSounds = 0;
				if (EXEAHOManager.Ins != null)
				{
					EXEAHOManager.Ins.MoveMeToStairwayDoor(0f);
					EXEAHOManager.Ins.EXEPlaySound(CustomSoundLookUp.hallDoorOpenClose);
					doorlogBehaviour.MayAddDoorlog("Floor 8 Hallway", mode: false);
				}
				doorJumpStage.Event -= DoorKickKill;
			}
		});
		GameManager.TimeSlinger.FireTimer(num + 2f, delegate
		{
			if (!jumpTriggered)
			{
				Object.Destroy(aho);
				DeSpawn();
			}
		});
	}

	private void DeSpawn()
	{
		IsActive = false;
		PowerStateManager.RemovePowerStateLock(STATE_LOCK_OCCASION.EXECUTIONER);
		EnemyStateManager.RemoveEnemyState(ENEMY_STATE.EXECUTIONER);
		generateFireWindow();
	}

	private void TriggerGameOver()
	{
		UIManager.TriggerHardGameOver("YOU'VE BEEN SENT TO THE WAITING ROOM");
	}

	private void PlayerEnteredDoorTrigger()
	{
		doorJumpStage.Execute();
	}

	public void TriggerJump()
	{
		if (!jumpTriggered)
		{
			jumpTriggered = true;
			Debug.Log("[Executioner] Jump Triggered");
			if (EXEAHOManager.Ins != null)
			{
				Object.Destroy(EXEAHOManager.Ins.gameObject);
				CamHookBehaviour.Interruptions = true;
				CamHookBehaviour.SwitchCameraStatus(enabled: false);
			}
			HitmanComputerJumper.Ins.myComputerController.LeaveEvents.Event += ComputerJump;
		}
	}

	public void ComputerJump()
	{
		BombMakerDeskJumper.Ins.EXERotator();
		HitmanComputerJumper.Ins.myComputerController.LeaveEvents.Event -= ComputerJump;
		ComputerChairObject.Ins.Hide();
		DataManager.LockSave = true;
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		BombMakerDeskJumper.Ins.myDeskController.LockRecovery();
		BombMakerDeskJumper.Ins.myDeskController.SetMasterLock(setLock: true);
		SpawnMesh(new Vector3(2.0714f, 39.59f, -3.26f), new Vector3(0f, 90f, 0f));
		myAnimator.SetBool("pcJump", value: true);
		GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.exeJump);
		GameManager.TimeSlinger.FireTimer(1.266f, delegate
		{
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.exePunch1);
			DataManager.ClearGameData();
			MainCameraHook.Ins.ClearARF();
			TriggerGameOver();
		});
	}

	public void DoorKickKill()
	{
		Debug.Log("[Executioner] Trigger Door Kick Jump");
		if (EXEAHOManager.Ins != null)
		{
			Object.Destroy(EXEAHOManager.Ins.gameObject);
			CamHookBehaviour.Interruptions = true;
			CamHookBehaviour.SwitchCameraStatus(enabled: false);
		}
		SpawnMesh(new Vector3(-2.2402f, 39.59f, -5.3121f), Vector3.zero);
		RotatePlayerToDoor();
		if (LookUp.Doors.MainDoor.SetDistanceLODs != null)
		{
			for (int i = 0; i < LookUp.Doors.MainDoor.SetDistanceLODs.Length; i++)
			{
				LookUp.Doors.MainDoor.SetDistanceLODs[i].OverwriteCulling = true;
			}
		}
		myAnimator.SetBool("kickDoor", value: true);
		KeypadManager.EnemyChangedKeypad();
		GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.exeJump);
		LookUp.Doors.MainDoor.KickDoorOpen();
		StartCoroutine(MoveToPlayerLoc());
	}

	private void RotatePlayerToDoor()
	{
		DataManager.LockSave = true;
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		roamController.Ins.SetMasterLock(setLock: true);
		StartCoroutine(LerpPosition(new Vector3(-2.2402f, 39.59f, -5.3121f), 1.5f));
	}

	private IEnumerator MoveToPlayerLoc()
	{
		yield return new WaitForSeconds(0.9f);
		Vector3 destination = roamController.Ins.transform.position + roamController.Ins.transform.forward;
		myNavMeshAgent.SetDestination(destination);
		PUNCH = true;
	}

	private IEnumerator LerpPosition(Vector3 targetPosition, float duration)
	{
		float time = 0f;
		Vector3 startPosition = roamController.Ins.transform.position + roamController.Ins.transform.forward;
		while (time < duration)
		{
			PoliceRoamJumper.Ins.TriggerConstantLookAt(Vector3.Lerp(startPosition, targetPosition, time / duration));
			time += Time.deltaTime;
			yield return null;
		}
		PoliceRoamJumper.Ins.TriggerConstantLookAt(targetPosition);
	}

	public void SpawnMesh(Vector3 location, Vector3 rotation)
	{
		myAnimator = Object.Instantiate(CustomObjectLookUp.ExecutionerPrefab, location, Quaternion.Euler(rotation)).GetComponent<Animator>();
		myNavMeshAgent = myAnimator.gameObject.AddComponent<NavMeshAgent>();
		BoxCollider boxCollider = myAnimator.gameObject.AddComponent<BoxCollider>();
		boxCollider.isTrigger = true;
		boxCollider.size = new Vector3(2f, 2f, 2f);
		boxCollider.gameObject.AddComponent<EXEDoorTrigger>().playerEnterEvent.Event += PunchPlayer;
	}

	private void PunchPlayer()
	{
		if (PUNCH)
		{
			PUNCH = false;
			Vector3 eulerAngles = Quaternion.LookRotation(roamController.Ins.transform.position - myAnimator.transform.position).eulerAngles;
			eulerAngles.x = 0f;
			eulerAngles.z = 0f;
			myAnimator.transform.DORotate(eulerAngles, 0.5f);
			myAnimator.SetBool("punch", value: true);
			GameManager.TimeSlinger.FireTimer(1.2f, delegate
			{
				GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.exePunch1);
				DataManager.ClearGameData();
				MainCameraHook.Ins.ClearARF();
				TriggerGameOver();
			});
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
