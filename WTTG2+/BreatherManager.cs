using DG.Tweening;
using UnityEngine;

public class BreatherManager : MonoBehaviour
{
	private const float BREATHER_Y_OFFSET = 0.935f;

	public static bool InvisiblePerson;

	public static bool BreatherCooldown;

	[SerializeField]
	private BreatherDataDefinition breatherData;

	[SerializeField]
	private Transform deadDropDoor;

	[SerializeField]
	private BreatherBehaviour breatherBehaviour;

	[SerializeField]
	private SpawnToLobbyTrigger spawnToLobbyTrigger;

	[SerializeField]
	private EnemyHotZoneTrigger preLeaveTrigger;

	[SerializeField]
	private EnemyHotZoneTrigger pickUpTrigger;

	[SerializeField]
	private EnemyHotZoneTrigger dumpsterTrigger;

	[SerializeField]
	private EnemyHotZoneTrigger peekABooTrigger;

	[SerializeField]
	private AudioHubObject audioQueHub;

	[SerializeField]
	private AudioFileDefinition[] audioQueSFXs = new AudioFileDefinition[0];

	private Timer braceTimer;

	private bool breatherIsActive;

	private int currentDoorAttempts;

	private bool doorMechanicActive;

	private bool firstProductWasPickedUp;

	private int keyDiscoveryCount;

	private BreatherData myData;

	private bool openDoorActive;

	private bool openDoorAttemptActive;

	private float openDoorAttemptTimeStamp;

	private float openDoorAttemptWindow;

	private float openDoorTimeStamp;

	private float openDoorWindow;

	private bool playerLeavesBraceDoomActive;

	private bool threatsActive;

	private int lastIDper = -1;

	public static int SwanAdded;

	private void Awake()
	{
		LookUp.SoundLookUp.breatherLaugh = audioQueSFXs[0];
		EnemyManager.BreatherManager = this;
		GameManager.StageManager.Stage += stageMe;
		GameManager.StageManager.ThreatsNowActivated += threatsActivated;
		GameManager.TheCloud.KeyDiscoveredEvent.Event += keyWasDiscovered;
	}

	private void Update()
	{
		if (openDoorAttemptActive && Time.time - openDoorAttemptTimeStamp >= openDoorAttemptWindow)
		{
			openDoorAttemptActive = false;
			triggerOpenDoor();
		}
		if (openDoorActive)
		{
			if (Time.time - openDoorTimeStamp >= openDoorWindow)
			{
				openDoorActive = false;
				rollNextAttempt();
			}
			if (!braceController.Ins.BracingDoor)
			{
				openDoorActive = false;
				triggerDoorDoom();
			}
		}
	}

	public void PlayerSpawnedToDeadDrop()
	{
		DataManager.LockSave = true;
		if (DifficultyManager.CasualMode || !firstProductWasPickedUp || !threatsActive || EnemyStateManager.IsInEnemyStateOrLocked())
		{
			return;
		}
		bool flag;
		if (TarotManager.Ins != null && TarotManager.BreatherUndertaker)
		{
			flag = true;
		}
		else
		{
			int num = Random.Range(0, 100);
			num -= SwanAdded;
            switch (keyDiscoveryCount)
            {
                case 0:
                case 1:
                    flag = num < 35;
                    break;
                case 2:
                    flag = num < 40;
                    break;
                case 3:
                    flag = num < 45;
                    break;
                case 4:
                    flag = num < 50;
                    break;
                case 5:
                    flag = num < 55;
                    break;
                case 6:
                    flag = num < 60;
                    break;
                case 7:
                    flag = num < 65;
                    break;
                case 8:
                default:
                    flag = num < 90;
                    break;
            }
            if (DifficultyManager.Nightmare)
			{
				flag = num < 95;
			}
		}
		if (!flag)
		{
			return;
		}
		if (InvisiblePerson)
		{
			InvisiblePerson = false;
		}
		else
		{
			if (BreatherCooldown)
			{
				return;
			}
			BreatherCooldown = true;
			GameManager.TimeSlinger.FireTimer(GetBreatherCooldown(), delegate
			{
				BreatherCooldown = false;
			});
			int num2 = Random.Range(0, DelfalcoBehaviour.Ins.released ? 3 : 2);
			if (lastIDper == num2)
			{
				num2++;
			}
			switch (lastIDper = num2 % (DelfalcoBehaviour.Ins.released ? 3 : 2))
			{
			case 1:
				FemaleNoirBehavior.Ins.SpawnOnAlleywayEntry();
				return;
			case 2:
				DelfalcoBehaviour.Ins.StartAlleywayPatrol();
				return;
			}
			if (!TarotVengeance.Killed(ENEMY_STATE.BREATHER))
			{
				EnemyStateManager.AddEnemyState(ENEMY_STATE.BREATHER);
				int num3 = Random.Range(0, audioQueSFXs.Length);
				audioQueHub.PlaySoundCustomDelay(audioQueSFXs[num3], 2f);
				breatherIsActive = true;
				preLeaveTrigger.SetActive();
				pickUpTrigger.SetActive();
				braceController.Ins.PlayerEnteredEvent.Event += playerEnteredBraceMode;
				braceController.Ins.PlayerLeftEvent.Event += playerLeftBraceMode;
			}
		}
	}

	private float GetBreatherCooldown()
	{
		if (DifficultyManager.Nightmare)
		{
			return 150f;
		}
		if (DifficultyManager.LeetMode)
		{
			return 300f;
		}
		return 450f;
	}

	public void PlayerHitPreLeaveTrigger()
	{
		if (breatherIsActive)
		{
			DataManager.ClearGameData();
			PauseManager.LockPause();
			GameManager.InteractionManager.LockInteraction();
			spawnToLobbyTrigger.LockOut();
			GameManager.AudioSlinger.PlaySoundWithCustomDelay(LookUp.SoundLookUp.JumpHit7, 0.2f);
			breatherBehaviour.TriggerVoice(BREATHER_VOICE_COMMANDS.LAUGH1);
			breatherBehaviour.TriggerExitRush();
		}
	}

	public void PlayerEnteredPickUpLocation()
	{
		if (breatherIsActive)
		{
			DataManager.ClearGameData();
			PauseManager.LockPause();
			GameManager.InteractionManager.LockInteraction();
			spawnToLobbyTrigger.LockOut();
			if (roamController.Ins.transform.rotation.eulerAngles.y >= 60f && roamController.Ins.transform.rotation.eulerAngles.y <= 130f)
			{
				BreatherRoamJumper.Ins.StagePickUpJump();
				GameManager.AudioSlinger.PlaySoundWithCustomDelay(LookUp.SoundLookUp.JumpHit7, 0.2f);
				breatherBehaviour.TriggerVoice(BREATHER_VOICE_COMMANDS.LAUGH1);
				breatherBehaviour.TriggerPickUpRush();
			}
			else
			{
				dumpsterTrigger.SetActive();
			}
		}
	}

	public void PlayerEnteredDumpsterTrigger()
	{
		if (breatherIsActive)
		{
			breatherBehaviour.TriggerDumpsterJump();
			GameManager.AudioSlinger.PlaySoundWithCustomDelay(LookUp.SoundLookUp.JumpHit7, 0.1f);
			GameManager.TimeSlinger.FireTimer(0.1f, BreatherRoamJumper.Ins.TriggerDumpsterJump);
		}
	}

	public void PlayerLeftDeadDrop()
	{
		DataManager.LockSave = false;
		if (breatherIsActive)
		{
			EnemyStateManager.RemoveEnemyState(ENEMY_STATE.BREATHER);
		}
		breatherIsActive = false;
	}

	public void TriggerWalkToDoor()
	{
		breatherBehaviour.TriggerWalkToDoor();
	}

	public void TriggerDoorMech()
	{
		doorMechanicActive = true;
		currentDoorAttempts = Random.Range(breatherData.DoorAttemptsMin, breatherData.DoorAttemptsMax);
		openDoorAttemptWindow = Random.Range(breatherData.DoorAttemptWindowMin, breatherData.DoorAttemptWindowMax);
		openDoorAttemptTimeStamp = Time.time;
		openDoorAttemptActive = true;
	}

	public void TriggerSafeDeSpawn()
	{
		EnemyStateManager.RemoveEnemyState(ENEMY_STATE.BREATHER);
		breatherBehaviour.DeSpawn();
		braceController.Ins.PlayerLeftEvent.Event -= playerLeftBraceModeTooSoon;
	}

	public void ActivatePeekABooJump()
	{
		breatherBehaviour.CapsuleCollider.radius = 0.33f;
		breatherBehaviour.NotInMeshEvents.Event += triggerPeekABookJump;
		breatherBehaviour.InMeshEvents.Event += reRollPeekABooJump;
		breatherBehaviour.AttemptSpawnBehindPlayer(roamController.Ins.transform, 0.935f);
	}

	private void playerEnteredBraceMode()
	{
		if (!playerLeavesBraceDoomActive)
		{
			if (braceTimer != null)
			{
				braceTimer.UnPause();
				return;
			}
			float duration = Random.Range(breatherData.PatrolDelayMin, breatherData.PatrolDelayMax);
			GameManager.TimeSlinger.FireHardTimer(out braceTimer, duration, triggerDoorPatrol);
		}
	}

	private void playerLeftBraceMode()
	{
		if (braceTimer != null)
		{
			braceTimer.Pause();
		}
		if (!playerLeavesBraceDoomActive)
		{
			return;
		}
		if (doorMechanicActive)
		{
			DOTween.To(() => deadDropDoor.transform.localRotation, delegate(Quaternion x)
			{
				deadDropDoor.transform.localRotation = x;
			}, Vector3.zero, 0.5f).SetEase(Ease.Linear).SetOptions();
			openDoorAttemptActive = false;
			openDoorActive = false;
		}
		stagePeekABooJump();
	}

	private void playerLeftBraceModeTooSoon()
	{
		braceController.Ins.PlayerLeftEvent.Event -= playerLeftBraceModeTooSoon;
		stagePeekABooJump();
	}

	private void triggerDoorPatrol()
	{
		GameManager.TimeSlinger.KillTimer(braceTimer);
		braceTimer = null;
		playerLeavesBraceDoomActive = true;
		BreatherPatrolBehaviour.Ins.PatrolSpawn();
	}

	private void triggerOpenDoor()
	{
		DOTween.To(() => deadDropDoor.transform.localRotation, delegate(Quaternion x)
		{
			deadDropDoor.transform.localRotation = x;
		}, new Vector3(0f, -0.5f, 0f), 0.75f).SetEase(Ease.Linear).SetOptions();
		GameManager.TimeSlinger.FireTimer(0.55f, delegate
		{
			openDoorWindow = Random.Range(breatherData.OpeningDoorWindowMin, breatherData.OpeningDoorWindowMax);
			openDoorTimeStamp = Time.time;
			openDoorActive = true;
		});
	}

	private void rollNextAttempt()
	{
		DOTween.To(() => deadDropDoor.transform.localRotation, delegate(Quaternion x)
		{
			deadDropDoor.transform.localRotation = x;
		}, Vector3.zero, 0.5f).SetEase(Ease.Linear).SetOptions();
		currentDoorAttempts--;
		if (currentDoorAttempts > 0)
		{
			openDoorAttemptWindow = Random.Range(breatherData.DoorAttemptWindowMin, breatherData.DoorAttemptWindowMax);
			openDoorAttemptTimeStamp = Time.time;
			openDoorAttemptActive = true;
			return;
		}
		openDoorAttemptActive = false;
		openDoorActive = false;
		playerLeavesBraceDoomActive = false;
		preLeaveTrigger.SetInActive();
		pickUpTrigger.SetInActive();
		dumpsterTrigger.SetInActive();
		braceController.Ins.PlayerEnteredEvent.Event -= playerEnteredBraceMode;
		braceController.Ins.PlayerLeftEvent.Event -= playerLeftBraceMode;
		braceController.Ins.PlayerLeftEvent.Event += playerLeftBraceModeTooSoon;
		breatherBehaviour.TriggerWalkAwayFromDoor();
	}

	private void triggerDoorDoom()
	{
		DataManager.ClearGameData();
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		DOTween.To(() => deadDropDoor.transform.localRotation, delegate(Quaternion x)
		{
			deadDropDoor.transform.localRotation = x;
		}, new Vector3(0f, -131f, 0f), 0.3f).SetEase(Ease.Linear).SetOptions();
		BreatherBraceJumper.Ins.TriggerDoorJump();
		breatherBehaviour.TriggerDoorJump();
		LookUp.Doors.DeadDropDoor.AudioHub.PlaySound(LookUp.SoundLookUp.SlamOpenDoor2);
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.JumpHit7);
	}

	private void stagePeekABooJump()
	{
		DataManager.ClearGameData();
		openDoorAttemptActive = false;
		openDoorActive = false;
		playerLeavesBraceDoomActive = false;
		pickUpTrigger.SetInActive();
		dumpsterTrigger.SetInActive();
		braceController.Ins.PlayerLeftEvent.Event -= playerLeftBraceMode;
		braceController.Ins.PlayerLeftEvent.Event -= playerLeftBraceModeTooSoon;
		breatherBehaviour.HardDeSpawn();
		peekABooTrigger.SetActive();
	}

	private void triggerPeekABookJump()
	{
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		breatherBehaviour.NotInMeshEvents.Event -= triggerPeekABookJump;
		breatherBehaviour.InMeshEvents.Event -= reRollPeekABooJump;
		GameManager.AudioSlinger.PlaySoundWithCustomDelay(LookUp.SoundLookUp.JumpHit6, 0.3f);
		breatherBehaviour.SpawnBehindPlayer(roamController.Ins.transform, 0.935f);
		breatherBehaviour.TriggerPeekABooJump();
		BreatherRoamJumper.Ins.TriggerPeekABooJump();
	}

	private void reRollPeekABooJump()
	{
		breatherBehaviour.NotInMeshEvents.Event -= triggerPeekABookJump;
		breatherBehaviour.InMeshEvents.Event -= reRollPeekABooJump;
		GameManager.TimeSlinger.FireTimer(0.2f, ActivatePeekABooJump);
	}

	private void keyWasDiscovered()
	{
		keyDiscoveryCount++;
		myData.KeysDiscoveredCount = keyDiscoveryCount;
		DataManager.Save(myData);
	}

	private void productWasPickedUp()
	{
		GameManager.ManagerSlinger.ProductsManager.ProductWasPickedUp.Event -= productWasPickedUp;
		firstProductWasPickedUp = true;
		myData.ProductWasPickedUp = true;
		DataManager.Save(myData);
		TarotVengeance.ActivateEnemy(ENEMY_STATE.BREATHER);
	}

	private void stageMe()
	{
		GameManager.StageManager.Stage -= stageMe;
		myData = DataManager.Load<BreatherData>(55779);
		if (myData == null)
		{
			myData = new BreatherData(55779);
			myData.KeysDiscoveredCount = 0;
			myData.ProductWasPickedUp = false;
		}
		keyDiscoveryCount = myData.KeysDiscoveredCount;
		firstProductWasPickedUp = myData.ProductWasPickedUp;
		if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			firstProductWasPickedUp = true;
			TarotVengeance.ActivateEnemy(ENEMY_STATE.BREATHER);
		}
		else if (!myData.ProductWasPickedUp)
		{
			GameManager.ManagerSlinger.ProductsManager.ProductWasPickedUp.Event += productWasPickedUp;
		}
	}

	private void threatsActivated()
	{
		GameManager.StageManager.ThreatsNowActivated -= threatsActivated;
		threatsActive = true;
	}
}
