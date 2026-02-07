using System;
using Colorful;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class DelfalcoBehaviour : MonoBehaviour
{
	public static DelfalcoBehaviour Ins;

	[SerializeField]
	private Animator myAnimator;

	[SerializeField]
	private GameObject saw;

	[SerializeField]
	private Light helperLight;

	public AudioHubObject myAho;

	public NavMeshAgent myAgent;

	private DelfalcoTrigger myTrigger;

	private DelfalcoTrigger killTrigger;

	private DelfalcoTrigger lobbyKillTrigger;

	private DelfalcoFootstepController myFootstepController;

	private DelfalcoTrigger[] safeSpots;

	public bool released;

	private bool stageChase;

	private float fireTimeStamp;

	private float shortFireTimeStamp;

	private float fireWindow;

	private float shortFireWindow;

	private bool fireWindowActive;

	private bool shortFireWindowActive;

	private DoorTrigger latestDoor;

	private Transform cameraBone;

	private bool lockDespawn;

	private bool boughtCams;

	private bool boughtKeypad;

	private bool stagedCamsTrap;

	private bool hasAppearedOnCams;

	private bool chasingPlayer;

	private bool patrolStarted;

	private bool hadPreviousFrame;

	private bool destInProgress;

	private Action destinationReached;

	private PatrolPointDefinition previousPatrolPoint;

	private int currentSpots;

	private int maxSpots;

	private bool stageKill;

	private bool knowsPlayersApartment;

	private bool peepJumpFix;

	private bool inApartment;

	private bool checkIfPlayerIsPeaking;

	private bool playerPeeking;

	private float peakingTimeStamp;

	private int peekCount;

	private bool cancelRunDespawn;

	private bool delfalcoApartmentDoorCheck;

	private int doorOpenCount = 0;

	private bool bathroomKill = false;

	private bool duringChaseKill;

	private int inSafeZone;

	private bool lobbyTriggerAllowedToKill;

	private bool playerInLobbyTrigger;

	private bool duringKill;

	private bool stagedKill;

	public bool alleywayChase;

	private bool jumpOnLeaveRoom;

	private bool alleywayPatrolActive;

	private bool noticedPlayerAlleyway;

	private bool doorJumpStaged;

	public bool canStageDumpsterJump;

	private AudioHubObject alleywayCueAHO;

	private bool alleyDoorOpening;

	public string DelfalcoDebug
	{
		get
		{
			if (TarotVengeance.Killed(ENEMY_STATE.DELFALCO))
			{
				return "-2";
			}
			string text = ((fireWindow - (Time.time - fireTimeStamp) > 0f) ? ((int)(fireWindow - (Time.time - fireTimeStamp))).ToString() : "-1");
			string text2 = ((shortFireWindow - (Time.time - shortFireTimeStamp) > 0f) ? ((int)(shortFireWindow - (Time.time - shortFireTimeStamp))).ToString() : "-1");
			return text + " | " + text2;
		}
	}

	private void Awake()
	{
		if (SceneManager.GetActiveScene().name.ToLower() == "titlescreen")
		{
			return;
		}
		Ins = this;
		myFootstepController = GetComponentInChildren<DelfalcoFootstepController>();
		myAho = base.gameObject.AddComponent<AudioHubObject>();
		alleywayCueAHO = new GameObject("DelfalcoAlleywayAHO").AddComponent<AudioHubObject>();
		alleywayCueAHO.transform.position = new Vector3(0.03f, 1.61f, 215.25f);
		myAgent = base.gameObject.AddComponent<NavMeshAgent>();
		myAgent.speed = 1f;
		myAgent.enabled = false;
		new GameObject("CustomElevatorManager").AddComponent<CustomElevatorManager>();
		myTrigger = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<DelfalcoTrigger>();
		myTrigger.name = "DelfalcoTrigger";
		myTrigger.GetComponent<BoxCollider>().isTrigger = true;
		myTrigger.GetComponent<MeshRenderer>().enabled = false;
		killTrigger = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<DelfalcoTrigger>();
		killTrigger.GetComponent<BoxCollider>().isTrigger = true;
		killTrigger.GetComponent<MeshRenderer>().enabled = false;
		killTrigger.name = "DelfalcoKillTrigger";
		lobbyKillTrigger = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<DelfalcoTrigger>();
		lobbyKillTrigger.GetComponent<BoxCollider>().isTrigger = true;
		lobbyKillTrigger.GetComponent<MeshRenderer>().enabled = false;
		lobbyKillTrigger.name = "DelfalcoLobbyKillTrigger";
		Transform transform;
		(transform = killTrigger.transform).SetParent(base.transform);
		transform.localScale = new Vector3(1.6134f, 1.2223f, 0.5396801f);
		transform.localPosition = new Vector3(0f, 1.003f, 0.476f);
		Transform transform2;
		(transform2 = lobbyKillTrigger.transform).localScale = new Vector3(3.15f, 1f, 1f);
		transform2.localPosition = new Vector3(-1.5731f, 0.5132f, -7.7955f);
		myTrigger.playerEnterEvent.Event += PlayerEnteredTrigger;
		killTrigger.playerEnterEvent.Event += TriggerChaseJump;
		lobbyKillTrigger.playerEnterEvent.Event += LobbyTriggerEntered;
		lobbyKillTrigger.playerExitEvent.Event += LobbyTriggerExited;
		DelfalcoTrigger delfalcoTrigger = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<DelfalcoTrigger>();
		delfalcoTrigger.name = "DelfalcoAlleywayPackageTrigger";
		delfalcoTrigger.GetComponent<BoxCollider>().isTrigger = true;
		delfalcoTrigger.GetComponent<MeshRenderer>().enabled = false;
		delfalcoTrigger.playerEnterEvent.Event += PlayeredEnteredPackageTrigger;
		delfalcoTrigger.transform.position = new Vector3(31f, 0.5f, 200f);
		delfalcoTrigger.transform.localScale = new Vector3(1f, 1f, 4f);
		safeSpots = new DelfalcoTrigger[3];
		safeSpots[0] = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<DelfalcoTrigger>();
		safeSpots[0].GetComponent<BoxCollider>().isTrigger = true;
		safeSpots[0].GetComponent<MeshRenderer>().enabled = false;
		safeSpots[0].name = "DelfalcoSafeSpot1";
		safeSpots[0].transform.position = new Vector3(-3.8913f, 0.6605f, -16.4719f);
		safeSpots[0].transform.localScale = new Vector3(1.8f, 1f, 4.2f);
		safeSpots[1] = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<DelfalcoTrigger>();
		safeSpots[1].GetComponent<BoxCollider>().isTrigger = true;
		safeSpots[1].GetComponent<MeshRenderer>().enabled = false;
		safeSpots[1].name = "DelfalcoSafeSpot2";
		safeSpots[1].transform.position = new Vector3(4.2287f, 0.6605f, -21.5774f);
		safeSpots[1].transform.localScale = new Vector3(7f, 1f, 15f);
		safeSpots[2] = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<DelfalcoTrigger>();
		safeSpots[2].GetComponent<BoxCollider>().isTrigger = true;
		safeSpots[2].GetComponent<MeshRenderer>().enabled = false;
		safeSpots[2].name = "DelfalcoSafeSpot3";
		safeSpots[2].transform.position = new Vector3(0.6142f, 0.6605f, -15.2865f);
		safeSpots[2].transform.localScale = new Vector3(1.5f, 1f, 2f);
		for (int i = 0; i < safeSpots.Length; i++)
		{
			safeSpots[i].playerEnterEvent.Event += delegate
			{
				inSafeZone++;
				bool flag = false;
			};
			safeSpots[i].playerExitEvent.Event += delegate
			{
				inSafeZone--;
				bool flag = false;
			};
		}
		myTrigger.transform.localScale = new Vector3(1f, 1f, 1.3564f);
		LookUp.Doors.Door1.DoorOpenEvent.AddListener(Floor1Opened);
		LookUp.Doors.Door3.DoorOpenEvent.AddListener(Floor3Opened);
		LookUp.Doors.Door5.DoorOpenEvent.AddListener(Floor5Opened);
		LookUp.Doors.Door6.DoorOpenEvent.AddListener(Floor6Opened);
		LookUp.Doors.Door8.DoorOpenEvent.AddListener(Floor8Opened);
		LookUp.Doors.Door10.DoorOpenEvent.AddListener(Floor10Opened);
		cameraBone = GameObject.Find("SM_MrDelfalco.ao/Root/UN_j_PropB_Output/CameraHolder").transform;
		StateManager.PlayerStateChangeEvents.Event += PlayerStateChanged;
		LookUp.Doors.MainDoor.DoorOpenEvent.AddListener(MainDoorOpened);
		helperLight.enabled = false;
		if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			ReleaseMe();
		}
	}

	private void PlayeredEnteredPackageTrigger()
	{
		Debug.Log("[Delfalco] Player entered pick up location");
		if (canStageDumpsterJump && EnemyStateManager.HasEnemyState(ENEMY_STATE.DELFALCO))
		{
			StageDumpsterJump();
		}
	}

	public bool CanSpawnInAlleyway()
	{
		return released && !TarotVengeance.Killed(ENEMY_STATE.DELFALCO);
	}

	public void DevEnemyTimerChange(float time)
	{
		if (fireWindowActive)
		{
			fireWindow = time;
			fireTimeStamp = Time.time;
		}
	}

	private void Update()
	{
		if (SceneManager.GetActiveScene().name.ToLower() == "titlescreen" || DifficultyManager.CasualMode || TarotVengeance.Killed(ENEMY_STATE.DELFALCO))
		{
			return;
		}
		if (alleywayChase)
		{
			myAgent.destination = roamController.Ins.transform.position;
			myAgent.speed = 4f;
		}
		else
		{
			myAgent.speed = 1f;
		}
		if (fireWindowActive && Time.time - fireTimeStamp >= fireWindow)
		{
			fireWindowActive = false;
			StageSpawn();
		}
		if (shortFireWindowActive && Time.time - shortFireTimeStamp >= shortFireWindow)
		{
			shortFireWindowActive = false;
			if (boughtCams)
			{
				SpawnCameraTrap();
				return;
			}
			StageSpawn();
		}
		if (destInProgress && myAgent.enabled)
		{
			if (myAgent.hasPath)
			{
				hadPreviousFrame = true;
			}
			else if (hadPreviousFrame)
			{
				hadPreviousFrame = false;
				destinationReached?.Invoke();
			}
		}
		if (delfalcoApartmentDoorCheck && base.transform.position.x > -5f)
		{
			delfalcoApartmentDoorCheck = false;
			DelfalcoPassedDoor();
		}
		if (checkIfPlayerIsPeaking && Time.time - peakingTimeStamp >= 1f)
		{
			peakingTimeStamp = Time.time;
			if (playerPeeking)
			{
				peekCount++;
			}
			if (peekCount >= 5)
			{
				checkIfPlayerIsPeaking = false;
				myAgent.enabled = false;
				DeSpawn(noFireWindow: true);
				StageBehindJump();
			}
		}
	}

	private void StageSpawn()
	{
		if (DifficultyManager.CasualMode || TarotVengeance.Killed(ENEMY_STATE.DELFALCO))
		{
			return;
		}
		if (EnemyStateManager.IsInEnemyStateOrLocked() || StateManager.BeingHacked)
		{
			fireWindowActive = true;
			fireTimeStamp = Time.time;
			fireWindow = 10f;
		}
		else if (knowsPlayersApartment)
		{
			if (StateManager.PlayerLocation == PLAYER_LOCATION.OUTSIDE || EnvironmentManager.PowerState == POWER_STATE.OFF)
			{
				fireWindowActive = true;
				fireTimeStamp = Time.time;
				fireWindow = 10f;
			}
			else if (StateManager.PlayerLocation == PLAYER_LOCATION.MAIN_ROON || StateManager.PlayerLocation == PLAYER_LOCATION.BATH_ROOM)
			{
				if (EnvironmentManager.PowerState != POWER_STATE.ON || LookUp.Doors.MainDoor.IsOpen)
				{
					fireWindowActive = true;
					fireTimeStamp = Time.time;
					fireWindow = 10f;
				}
				else
				{
					Debug.Log("[Delfalco] Staged Apartment Spawn");
					TriggerHomePatrol();
				}
			}
			else
			{
				Debug.Log("[Delfalco] I already know the player's apartment. Waiting for player to come back");
				fireWindowActive = true;
				fireTimeStamp = Time.time;
				fireWindow = 15f;
			}
		}
		else
		{
			Debug.Log("[Delfalco] Staged Spawn");
			stageChase = true;
		}
	}

	private void DeSpawn(bool noFireWindow = false)
	{
		if (!lockDespawn)
		{
			myAgent.enabled = false;
			if (!noFireWindow)
			{
				generateFireWindow();
				EnemyStateManager.RemoveEnemyState(ENEMY_STATE.DELFALCO);
				PowerStateManager.RemovePowerStateLock(STATE_LOCK_OCCASION.DELFALCO);
			}
			myAnimator.SetBool("Run", value: false);
			myFootstepController.running = false;
			myAnimator.SetBool("Walk", value: false);
			patrolStarted = false;
			base.transform.position = Vector3.zero;
			chasingPlayer = false;
			myTrigger.DeSpawnMe();
			if (EventManager.ElevatorOpened)
			{
				CustomElevatorManager.Ins.OpenMyDoor();
				CustomElevatorManager.Ins.MoveMeToFloor(8);
			}
			else
			{
				CustomElevatorManager.Ins.CloseMyDoor();
			}
		}
	}

	public void ReleaseMe()
	{
		if (!DifficultyManager.CasualMode && !released)
		{
			released = true;
			generateShortFireWindow();
			Debug.Log("[Delfalco] I was released");
			TarotVengeance.ActivateEnemy(ENEMY_STATE.DELFALCO);
		}
	}

	public void SetKnowsApartment()
	{
		ReleaseMe();
		knowsPlayersApartment = true;
	}

	public void CalledDelfalco()
	{
		ReleaseMe();
		knowsPlayersApartment = true;
		fireWindow = 15f;
		fireTimeStamp = Time.time;
		fireWindowActive = true;
	}

	private void generateFireWindow()
	{
		if (!DifficultyManager.CasualMode && !TarotVengeance.Killed(ENEMY_STATE.DELFALCO))
		{
			fireWindow = (knowsPlayersApartment ? UnityEngine.Random.Range(660f, 970f) : UnityEngine.Random.Range(280f, 440f));
			if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
			{
				fireWindow *= 0.9f;
			}
			fireTimeStamp = Time.time;
			fireWindowActive = true;
		}
	}

	private void generateShortFireWindow()
	{
		if (!DifficultyManager.CasualMode && !TarotVengeance.Killed(ENEMY_STATE.DELFALCO))
		{
			Debug.Log("[Delfalco] Generated short fire window");
			shortFireWindow = UnityEngine.Random.Range(30f, 55f);
			shortFireTimeStamp = Time.time;
			shortFireWindowActive = true;
		}
	}

	private void OnDestroy()
	{
		if (!(SceneManager.GetActiveScene().name.ToLower() == "titlescreen"))
		{
			Ins = null;
			LookUp.Doors.Door1.DoorOpenEvent.RemoveListener(Floor1Opened);
			LookUp.Doors.Door3.DoorOpenEvent.RemoveListener(Floor3Opened);
			LookUp.Doors.Door5.DoorOpenEvent.RemoveListener(Floor5Opened);
			LookUp.Doors.Door6.DoorOpenEvent.RemoveListener(Floor6Opened);
			LookUp.Doors.Door8.DoorOpenEvent.RemoveListener(Floor8Opened);
			LookUp.Doors.Door10.DoorOpenEvent.RemoveListener(Floor10Opened);
		}
	}

	private void PlayerEnteredTrigger()
	{
		if (stageChase)
		{
			ComeOutTheElevator();
		}
	}

	public void ComeOutTheElevator()
	{
		stageChase = false;
		doorOpenCount = 0;
		PlaySoundThroughMe(CustomSoundLookUp.delfalcoChaseLine);
		CustomElevatorManager.Ins.OpenMyDoor();
		StateManager.PlayerLocationChangeEvents.Event += PlayerLocationChanged;
		GameManager.TimeSlinger.FireTimer(0.4f, delegate
		{
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.delfalcoChaseJump);
			base.transform.DOMoveX(23.7f, 7f).SetEase(Ease.Linear);
			myAnimator.SetBool("Run", value: true);
			myFootstepController.running = true;
		});
		chasingPlayer = true;
		if (latestDoor == LookUp.Doors.Door1)
		{
			GameManager.TimeSlinger.FireTimer(3.75f, DelfalcoLookAtLobby);
		}
		GameManager.TimeSlinger.FireTimer(7.4f, delegate
		{
			lobbyTriggerAllowedToKill = false;
			StateManager.PlayerLocationChangeEvents.Event -= PlayerLocationChanged;
			if (latestDoor.IsOpen)
			{
				TriggerChaseJump();
			}
			else if (!cancelRunDespawn)
			{
				DeSpawn();
			}
		});
	}

	private void PlayerLocationChanged()
	{
		if (bathroomKill && StateManager.PlayerLocation == PLAYER_LOCATION.MAIN_ROON)
		{
			StageBehindJump();
		}
	}

	public void Floor1Opened()
	{
		StairCaseDoorOpened(1);
		latestDoor = LookUp.Doors.Door1;
	}

	public void Floor3Opened()
	{
		StairCaseDoorOpened(3);
		latestDoor = LookUp.Doors.Door3;
	}

	public void Floor5Opened()
	{
		StairCaseDoorOpened(5);
		latestDoor = LookUp.Doors.Door5;
	}

	public void Floor6Opened()
	{
		StairCaseDoorOpened(6);
		latestDoor = LookUp.Doors.Door6;
	}

	public void Floor8Opened()
	{
		StairCaseDoorOpened(8);
		latestDoor = LookUp.Doors.Door8;
	}

	public void Floor10Opened()
	{
		StairCaseDoorOpened(10);
		latestDoor = LookUp.Doors.Door10;
	}

	public void StairCaseDoorOpened(int floor)
	{
		if (chasingPlayer)
		{
			doorOpenCount++;
			if (doorOpenCount == 2)
			{
				cancelRunDespawn = true;
				base.transform.DOPause();
				DeSpawn(noFireWindow: true);
				GameManager.TimeSlinger.FireTimer(1f, StageBehindJump);
			}
		}
		if (stageChase && (!EnemyStateManager.IsInEnemyState() || EnemyStateManager.HasEnemyState(ENEMY_STATE.DELFALCO)))
		{
			delfalcoApartmentDoorCheck = floor == 8;
			EnemyStateManager.AddEnemyState(ENEMY_STATE.DELFALCO);
			CustomElevatorManager.Ins.MoveMeToFloor(floor);
			CustomElevatorManager.Ins.CloseMyDoor();
			moveMeToFloor(floor);
			moveTriggerToFloor(floor);
			if (TarotManager.CurSpeed == playerSpeedMode.WEAK)
			{
				TarotManager.CurSpeed = playerSpeedMode.NORMAL;
			}
		}
	}

	private void moveMeToFloor(int floor)
	{
		base.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
		switch (floor)
		{
		case 10:
			base.transform.position = new Vector3(-27.7f, 50.92f, -6.3f);
			break;
		case 8:
			base.transform.position = new Vector3(-27.7f, 39.62f, -6.3f);
			break;
		case 6:
			base.transform.position = new Vector3(-27.7f, 28.32f, -6.3f);
			break;
		case 5:
			base.transform.position = new Vector3(-27.7f, 22.65f, -6.3f);
			break;
		case 3:
			base.transform.position = new Vector3(-27.7f, 11.34f, -6.3f);
			break;
		case 1:
			base.transform.position = new Vector3(-27.7f, 0.03f, -6.3f);
			break;
		case 2:
		case 4:
		case 7:
		case 9:
			break;
		}
	}

	private void moveTriggerToFloor(int floor)
	{
		switch (floor)
		{
		case 10:
			myTrigger.MoveMe(new Vector3(1.13f, 51.65f, -6.3f));
			break;
		case 8:
			myTrigger.MoveMe(new Vector3(1.13f, 40.52f, -6.3f));
			break;
		case 6:
			myTrigger.MoveMe(new Vector3(1.13f, 29.15f, -6.3f));
			break;
		case 5:
			myTrigger.MoveMe(new Vector3(1.13f, 23.15f, -6.3f));
			break;
		case 3:
			myTrigger.MoveMe(new Vector3(1.13f, 12.15f, -6.3f));
			break;
		case 1:
			myTrigger.MoveMe(new Vector3(1.13f, 0.5f, -6.3f));
			break;
		case 2:
		case 4:
		case 7:
		case 9:
			break;
		}
	}

	public void PlaySoundThroughMe(AudioFileDefinition AFD)
	{
		myAho.PlaySound(AFD);
	}

	private void TriggerChaseJump()
	{
		if ((!chasingPlayer && !alleywayChase) || duringChaseKill)
		{
			return;
		}
		duringChaseKill = true;
		helperLight.enabled = true;
		GameObject.Find("SM_MrDelfalco.mo").GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen = true;
		lockDespawn = true;
		base.transform.DOPause();
		FlashLightBehaviour.Ins.LockOut();
		GameManager.BehaviourManager.CrossHairBehaviour.HideCrossHairGroup();
		GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.delfalcoChaseJumpB);
		Debug.Log("[Delfalco] Chase Jump Triggered.");
		roamController.Ins.LoseControl();
		PauseManager.LockPause();
		CameraManager.Get(roamController.Ins.CameraIControl, out var ReturnCamera);
		Transform cameraTransform = ReturnCamera.transform;
		GameManager.TimeSlinger.FireTimer(0.05f, delegate
		{
			cameraTransform.SetParent(cameraBone);
			cameraTransform.DOLocalMove(new Vector3(0f, 0f, 0f), 0.2f).SetEase(Ease.Linear);
			cameraTransform.DOLocalRotate(new Vector3(0f, 180f, 0f), 0.2f).SetEase(Ease.Linear);
		});
		myAnimator.SetBool("ChaseJump", value: true);
		GameManager.TimeSlinger.FireTimer(3.49f, delegate
		{
			DoubleVision doubleVision = cameraTransform.gameObject.GetComponent<DoubleVision>();
			LensDistortionBlur lensDistortionBlur = cameraTransform.gameObject.GetComponent<LensDistortionBlur>();
			doubleVision.enabled = true;
			lensDistortionBlur.enabled = true;
			DOTween.To(() => doubleVision.Displace.x, delegate(float x)
			{
				doubleVision.Displace.x = x;
			}, 3f, 0.1f);
			DOTween.To(() => lensDistortionBlur.Distortion, delegate(float x)
			{
				lensDistortionBlur.Distortion = x;
			}, 0.5f, 0.1f);
		});
		GameManager.TimeSlinger.FireTimer(6.1f, delegate
		{
			UIManager.TriggerGameOver("YOU WERE CAUGHT");
		});
	}

	public void BoughtCams()
	{
		if (!boughtCams)
		{
			boughtCams = true;
		}
	}

	public void BoughtKeypad()
	{
		if (!boughtKeypad)
		{
			boughtKeypad = true;
		}
	}

	private void SpawnCameraTrap()
	{
		if (!DifficultyManager.CasualMode && !TarotVengeance.Killed(ENEMY_STATE.DELFALCO))
		{
			Debug.Log("[Delfalco] Staged camera trap");
			stagedCamsTrap = true;
			myAnimator.SetBool("Camera", value: true);
		}
	}

	public void CamsOpened()
	{
		if (stagedCamsTrap)
		{
			hasAppearedOnCams = true;
			base.transform.position = new Vector3(-2.819f, 39.6183f, -5.955f);
			base.transform.rotation = Quaternion.Euler(0f, 220f, 0f);
		}
	}

	public void CamsClosed()
	{
		if (stagedCamsTrap && hasAppearedOnCams)
		{
			Debug.Log("[Delfalco] Removed camera trap");
			stagedCamsTrap = false;
			myAnimator.SetBool("Camera", value: false);
			DeSpawn();
			generateFireWindow();
		}
	}

	private void TriggerHomePatrol()
	{
		if (!DifficultyManager.CasualMode && !TarotVengeance.Killed(ENEMY_STATE.DELFALCO) && !patrolStarted)
		{
			myFootstepController.dismissFootsteps = true;
			EnemyStateManager.AddEnemyState(ENEMY_STATE.DELFALCO);
			PowerStateManager.AddPowerStateLock(STATE_LOCK_OCCASION.DELFALCO);
			base.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
			patrolStarted = true;
			base.transform.position = new Vector3(15f, 39.62f, -6.3f);
			base.transform.DOMoveX(-2.2f, 15f).SetEase(Ease.Linear);
			myAnimator.SetBool("Walk", value: true);
			myAho.PlaySound(CustomSoundLookUp.delfalcoWhistle);
			GameManager.TimeSlinger.FireTimer(15f, delegate
			{
				myAnimator.SetBool("Walk", value: false);
				EnterMainRoom();
			});
			doorlogBehaviour.MayAddDoorlog("Floor 8 Hallway", mode: true);
		}
	}

	private void EnterMainRoom()
	{
		if (StateManager.PlayerState != PLAYER_STATE.HIDING)
		{
			CamHookBehaviour.Interruptions = true;
			CamHookBehaviour.SwitchCameraStatus(enabled: false);
			Debug.Log("NOT HIDING");
			if (StateManager.PlayerState != PLAYER_STATE.COMPUTER && StateManager.PlayerState != PLAYER_STATE.PEEPING)
			{
				DeSpawn(noFireWindow: true);
			}
			if (StateManager.PlayerState == PLAYER_STATE.PEEPING)
			{
				peepJumpFix = true;
			}
			StageBehindJump();
			return;
		}
		inApartment = true;
		myFootstepController.dismissFootsteps = false;
		base.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 1f);
		base.transform.DOMoveZ(HitmanPatrolBehaviour.Ins.mainRoomExitPatrolPoint.Position.z, 2f).SetEase(Ease.Linear);
		myAnimator.SetBool("Walk", value: true);
		currentSpots = 0;
		int num = 0;
		for (int i = 0; i < HitmanPatrolBehaviour.Ins.lightsToCheck.Length; i++)
		{
			if (HitmanPatrolBehaviour.Ins.lightsToCheck[i].LightsAreOn)
			{
				num++;
			}
		}
		maxSpots = ((num <= 3) ? UnityEngine.Random.Range(4, 9) : UnityEngine.Random.Range(7, 13));
		if (ComputerPowerHook.Ins.PowerOn)
		{
			GameManager.BehaviourManager.NotesBehaviour.ClearNotes();
			double num2 = DOSCoinsCurrencyManager.CurrentCurrency;
			float num3 = UnityEngine.Random.Range(0.5f, 0.9f);
			DOSCoinsCurrencyManager.RemoveCurrency((float)Math.Round(num2 * (double)num3, 3));
		}
		else
		{
			maxSpots--;
		}
		if (PoliceScannerBehaviour.Ins != null && PoliceScannerBehaviour.Ins.ownPoliceScanner && PoliceScannerBehaviour.Ins.IsOn)
		{
			maxSpots++;
		}
		if (RouterBehaviour.Ins != null && RouterBehaviour.Ins.Owned && RouterBehaviour.Ins.RouterIsActive)
		{
			maxSpots++;
		}
		LookUp.Doors.MainDoor.NPCOpenDoor();
		hideController.Ins.PlayerPeakingEvent.Event += playerIsPeaking;
		if (FlashLightBehaviour.Ins.LightOn)
		{
			StageBehindJump();
		}
		else
		{
			FlashLightBehaviour.Ins.FlashLightWentOn.Event += flashLightWasTriggered;
		}
		hideController.Ins.HideTrigger.PlayerLeftHidingActions.Event += playerLeftHiding;
		checkIfPlayerIsPeaking = true;
		GameManager.TimeSlinger.FireTimer(3f, EnteredRoom);
	}

	private void EnteredRoom()
	{
		if (!stageKill)
		{
			myAgent.enabled = true;
			destInProgress = false;
			myAnimator.SetBool("Walk", value: false);
			destinationReached = null;
			LookUp.Doors.MainDoor.ForceDoorClose();
			GameManager.TimeSlinger.FireTimer(1f, pickPatrolSpot);
		}
	}

	private void flashLightWasTriggered(bool obj)
	{
		FlashLightBehaviour.Ins.FlashLightWentOn.Event -= flashLightWasTriggered;
		DeSpawn(noFireWindow: true);
		LookUp.Doors.MainDoor.ForceOpenDoor();
		StageBehindJump();
	}

	private void pickPatrolSpot()
	{
		myAnimator.SetBool("LookAround", value: false);
		currentSpots++;
		if (currentSpots >= maxSpots)
		{
			PrepareToWalkOut();
			return;
		}
		myAnimator.SetBool("Walk", value: true);
		bool flag = true;
		while (flag)
		{
			PatrolPointDefinition patrolPointDefinition = HitmanPatrolBehaviour.Ins.mainRoomPatrolPoints[UnityEngine.Random.Range(0, HitmanPatrolBehaviour.Ins.mainRoomPatrolPoints.Length)];
			if (patrolPointDefinition != previousPatrolPoint)
			{
				flag = false;
				previousPatrolPoint = patrolPointDefinition;
			}
		}
		myAgent.destination = previousPatrolPoint.Position;
		destInProgress = true;
		destinationReached = delegate
		{
			destInProgress = false;
			myAnimator.SetBool("Walk", value: false);
			myAnimator.SetBool("LookAround", value: true);
			int num = UnityEngine.Random.Range(0, 100);
			if (num < 10)
			{
				AudioFileDefinition audioFile = UnityEngine.Random.Range(0, 4) switch
				{
					1 => CustomSoundLookUp.delfalcoPatrolVoice2, 
					2 => CustomSoundLookUp.delfalcoPatrolVoice3, 
					3 => CustomSoundLookUp.delfalcoPatrolVoice4, 
					_ => CustomSoundLookUp.delfalcoPatrolVoice1, 
				};
				myAho.PlaySound(audioFile);
			}
			GameManager.TimeSlinger.FireTimer(UnityEngine.Random.Range(3, 6), pickPatrolSpot);
		};
	}

	private void PrepareToWalkOut()
	{
		myAnimator.SetBool("Walk", value: true);
		destInProgress = true;
		myAgent.destination = HitmanPatrolBehaviour.Ins.mainRoomExitPatrolPoint.Position;
		destinationReached = WalkOut;
	}

	private void WalkOut()
	{
		hideController.Ins.PlayerPeakingEvent.Event -= playerIsPeaking;
		FlashLightBehaviour.Ins.FlashLightWentOn.Event -= flashLightWasTriggered;
		hideController.Ins.HideTrigger.PlayerLeftHidingActions.Event -= playerLeftHiding;
		peekCount = 0;
		checkIfPlayerIsPeaking = false;
		destInProgress = false;
		myAnimator.SetBool("Walk", value: false);
		LookUp.Doors.MainDoor.NPCOpenDoor();
		GameManager.TimeSlinger.FireTimer(1f, delegate
		{
			myAnimator.SetBool("Walk", value: true);
			myAgent.enabled = false;
			base.transform.DOLocalRotate(new Vector3(0f, 180f, 0f), 1f);
			base.transform.DOMoveZ(-6.2f, 2f).SetEase(Ease.Linear);
			base.transform.DOMoveX(-2f, 2f).SetEase(Ease.Linear);
			GameManager.TimeSlinger.FireTimer(2f, delegate
			{
				WalkedOut();
				doorlogBehaviour.MayAddDoorlog("Floor 8 Hallway", mode: false);
			});
		});
	}

	private void WalkedOut()
	{
		LookUp.Doors.MainDoor.ForceDoorClose();
		inApartment = false;
		base.transform.DOLocalRotate(new Vector3(0f, 90f, 0f), 1f);
		base.transform.DOMoveX(15f, 15f).SetEase(Ease.Linear);
		GameManager.TimeSlinger.FireTimer(2f, delegate
		{
			DeSpawn();
		});
	}

	public void StageBehindJump()
	{
		Debug.Log("[Delfalco] Staged Behind Jump");
		myAnimator.SetBool("Walk", value: false);
		GameManager.TimeSlinger.FireTimer(300f, delegate
		{
			if (EnvironmentManager.PowerState == POWER_STATE.ON)
			{
				EnvironmentManager.PowerBehaviour.ForcePowerOff();
			}
		});
		if (StateManager.PlayerState != PLAYER_STATE.ROAMING)
		{
			stageKill = true;
			return;
		}
		stageKill = false;
		myAnimator.SetBool("StageBehindJump", value: true);
		if (Physics.Raycast(roamController.Ins.transform.position, roamController.Ins.transform.forward * -1f, 1f))
		{
			GameManager.TimeSlinger.FireTimer(1f, StageBehindJump);
		}
		else
		{
			TriggerBehindJump();
		}
	}

	public void TriggerBehindJump()
	{
		if (stagedKill)
		{
			return;
		}
		stagedKill = true;
		Debug.Log("[Delfalco] Triggering Behind Jump");
		GameManager.TimeSlinger.FireTimer(7.5f, delegate
		{
			UIManager.TriggerGameOver("YOU WERE FOUND");
		});
		myAgent.enabled = false;
		helperLight.enabled = true;
		lockDespawn = true;
		base.transform.DOPause();
		FlashLightBehaviour.Ins.LockOut();
		GameManager.BehaviourManager.CrossHairBehaviour.HideCrossHairGroup();
		GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.delfalcoBehindJump);
		base.transform.position = roamController.Ins.transform.position + roamController.Ins.transform.forward * -1f - new Vector3(0f, 0.95f, 0f);
		Vector3 eulerAngles = Quaternion.LookRotation(roamController.Ins.transform.position - base.transform.position).eulerAngles;
		PauseManager.LockPause();
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		base.transform.rotation = Quaternion.Euler(eulerAngles);
		roamController.Ins.LoseControl();
		CameraManager.Get(roamController.Ins.CameraIControl, out var ReturnCamera);
		Transform cameraTransform = ReturnCamera.transform;
		cameraTransform.SetParent(cameraBone);
		cameraTransform.DOLocalMove(new Vector3(0f, 0f, 0f), 0.2f).SetEase(Ease.Linear);
		cameraTransform.DOLocalRotate(new Vector3(0f, 180f, 0f), 0.2f).SetEase(Ease.Linear);
		myAnimator.SetBool("BehindJump", value: true);
		GameManager.TimeSlinger.FireTimer(4.32f, delegate
		{
			DoubleVision doubleVision = cameraTransform.gameObject.GetComponent<DoubleVision>();
			LensDistortionBlur lensDistortionBlur = cameraTransform.gameObject.GetComponent<LensDistortionBlur>();
			doubleVision.enabled = true;
			lensDistortionBlur.enabled = true;
			DOTween.To(() => doubleVision.Displace.x, delegate(float x)
			{
				doubleVision.Displace.x = x;
			}, 3f, 0.1f);
			DOTween.To(() => lensDistortionBlur.Distortion, delegate(float x)
			{
				lensDistortionBlur.Distortion = x;
			}, 0.5f, 0.1f);
		});
	}

	private void PlayerStateChanged()
	{
		if (StateManager.PlayerState == PLAYER_STATE.ROAMING && peepJumpFix)
		{
			TriggerBehindJump();
		}
		else
		{
			if (StateManager.PlayerState != PLAYER_STATE.ROAMING)
			{
				return;
			}
			GameManager.TimeSlinger.FireTimer(1f, delegate
			{
				if (StateManager.PlayerState == PLAYER_STATE.ROAMING)
				{
					if (stageKill)
					{
						StageBehindJump();
					}
					if (peepJumpFix)
					{
						TriggerBehindJump();
					}
				}
			});
		}
	}

	public void MainDoorOpened()
	{
		if (chasingPlayer)
		{
			knowsPlayersApartment = true;
			if (!delfalcoApartmentDoorCheck)
			{
				cancelRunDespawn = true;
				base.transform.DOPause();
				DeSpawn(noFireWindow: true);
				myAho.MuteHub();
				GameManager.TimeSlinger.FireTimer(1f, StageBehindJump);
				return;
			}
		}
		if (patrolStarted && !inApartment)
		{
			base.transform.DOPause();
			DeSpawn(noFireWindow: true);
			myAho.MuteHub();
			GameManager.TimeSlinger.FireTimer(1f, StageBehindJump);
		}
	}

	private void playerIsPeaking(float SetValue)
	{
		playerPeeking = SetValue >= 0.5f;
	}

	private void playerLeftHiding()
	{
		if (LookUp.Doors.MainDoor.IsOpen)
		{
			LookUp.Doors.MainDoor.ForceDoorClose();
		}
		if (StateManager.PlayerLocation == PLAYER_LOCATION.BATH_ROOM)
		{
			bathroomKill = true;
			myAgent.enabled = false;
			DeSpawn(noFireWindow: true);
			StateManager.PlayerLocationChangeEvents.Event += PlayerLocationChanged;
		}
		else
		{
			myAgent.enabled = false;
			DeSpawn(noFireWindow: true);
			StageBehindJump();
		}
	}

	private void LobbyTriggerEntered()
	{
		playerInLobbyTrigger = true;
		if (chasingPlayer && lobbyTriggerAllowedToKill)
		{
			cancelRunDespawn = true;
			base.transform.DOPause();
			DeSpawn(noFireWindow: true);
			GameManager.TimeSlinger.FireTimer(0.5f, StageBehindJump);
		}
	}

	private void LobbyTriggerExited()
	{
		playerInLobbyTrigger = false;
	}

	private void DelfalcoLookAtLobby()
	{
		Debug.Log("[Delfalco] Checking in lobby");
		if (inSafeZone == 0 && StateManager.PlayerLocation == PLAYER_LOCATION.LOBBY)
		{
			Debug.Log("[Delfalco] Caught in lobby");
			GameManager.TimeSlinger.FireTimer(0.75f, delegate
			{
				cancelRunDespawn = true;
				DeSpawn(noFireWindow: true);
				GameManager.TimeSlinger.FireTimer(0.5f, StageBehindJump);
			});
			return;
		}
		Debug.Log("[Delfalco] No one in the lobby");
		lobbyTriggerAllowedToKill = true;
		if (playerInLobbyTrigger)
		{
			Debug.Log("[Delfalco] Player inside lobby trigger");
			cancelRunDespawn = true;
			DeSpawn(noFireWindow: true);
			TriggerBehindJump();
		}
	}

	private void DelfalcoPassedDoor()
	{
		if (LookUp.Doors.MainDoor.IsOpen)
		{
			cancelRunDespawn = true;
			base.transform.DOPause();
			DeSpawn(noFireWindow: true);
			GameManager.TimeSlinger.FireTimer(0.5f, StageBehindJump);
		}
	}

	public void StartAlleywayPatrol()
	{
		if (!TarotVengeance.Killed(ENEMY_STATE.DELFALCO))
		{
			Debug.Log("[Delfalco] Starting alleyway patrol");
			EnemyStateManager.AddEnemyState(ENEMY_STATE.DELFALCO);
			canStageDumpsterJump = true;
			base.transform.position = new Vector3(0.5455f, 0f, 205.71f);
			GameManager.TimeSlinger.FireTimer(2f, delegate
			{
				alleywayCueAHO.PlaySound(CustomSoundLookUp.delfalcoWhistle);
			});
			GameManager.TimeSlinger.FireTimer(17f, TryMoveIntoAlleyway);
			alleyDoorOpening = false;
		}
	}

	private void TryMoveIntoAlleyway()
	{
		if (!doorJumpStaged)
		{
			if (StateManager.PlayerLocation != PLAYER_LOCATION.DEAD_DROP_ROOM && StateManager.PlayerLocation != PLAYER_LOCATION.UNKNOWN)
			{
				StageBehindJump();
			}
			else if (LookUp.Doors.DeadDropDoor.DoingSomething)
			{
				GameManager.TimeSlinger.FireTimer(0.5f, TryMoveIntoAlleyway);
			}
			else
			{
				MoveIntoAlleyway();
			}
		}
	}

	private void MoveIntoAlleyway()
	{
		LookUp.Doors.DeadDropDoor.DoorOpenEvent.AddListener(DeadDropDoorOpen);
		LookUp.Doors.DeadDropDoor.DoorWasOpenedEvent.AddListener(DeadDropDoorDoneOpen);
		noticedPlayerAlleyway = false;
		alleywayPatrolActive = true;
		jumpOnLeaveRoom = true;
		base.transform.position = new Vector3(0.5455f, 0f, 205.71f);
		myAgent.enabled = true;
		myAnimator.SetBool("Walk", value: true);
		myAgent.destination = new Vector3(2.813f, 0f, 199.884f);
		destInProgress = true;
		canStageDumpsterJump = false;
		destinationReached = delegate
		{
			myAgent.destination = new Vector3(23.711f, 0f, 199.884f);
			destinationReached = delegate
			{
				destInProgress = false;
				myAnimator.SetBool("Walk", value: false);
				destinationReached = null;
				GameManager.TimeSlinger.FireTimer(5f, delegate
				{
					UnlockChase();
					myAgent.destination = new Vector3(32.596f, 0f, 199.884f);
					destInProgress = true;
					myAnimator.SetBool("Walk", value: true);
					destinationReached = delegate
					{
						destInProgress = false;
						myAnimator.SetBool("Walk", value: false);
						GameManager.TimeSlinger.FireTimer(5f, delegate
						{
							if (!noticedPlayerAlleyway && alleywayPatrolActive && !alleyDoorOpening)
							{
								StageDoorJump(new Vector3(24.282f, 0.455f, 198.43f), new Vector3(0f, 180f, 0f), new Vector3(23.577f, 0.455f, 198.43f), new Vector3(23.576f, 0.942f, 197.879f), new Vector3(0.95608f, 1f, 0.1525f));
							}
						});
					};
				});
			};
		};
	}

	private void DeadDropDoorOpen()
	{
		if (alleywayPatrolActive)
		{
			alleyDoorOpening = true;
		}
		if (alleywayPatrolActive && jumpOnLeaveRoom)
		{
			StageDoorJump(new Vector3(24.282f, 0.455f, 198.43f), new Vector3(0f, 180f, 0f), new Vector3(23.577f, 0.455f, 198.43f), new Vector3(23.576f, 0.942f, 197.879f), new Vector3(0.95608f, 1f, 0.1525f));
		}
	}

	private void DeadDropDoorDoneOpen()
	{
		if (alleywayPatrolActive && !jumpOnLeaveRoom && !doorJumpStaged)
		{
			AlleywayNoticePlayer();
		}
	}

	private void UnlockChase()
	{
		jumpOnLeaveRoom = false;
	}

	private void AlleywayNoticePlayer()
	{
		if (noticedPlayerAlleyway)
		{
			return;
		}
		noticedPlayerAlleyway = true;
		myAgent.enabled = false;
		destInProgress = false;
		destinationReached = null;
		myAnimator.SetBool("Walk", value: false);
		Vector3 eulerAngles = Quaternion.LookRotation(roamController.Ins.transform.position - base.transform.position).eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		base.transform.DORotate(eulerAngles, 0.5f);
		GameManager.TimeSlinger.FireTimer(0.5f, delegate
		{
			PlaySoundThroughMe(CustomSoundLookUp.delfalcoChaseLine);
			GameManager.TimeSlinger.FireTimer(0.4f, delegate
			{
				GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.delfalcoChaseJump);
				myAnimator.SetBool("Run", value: true);
				myFootstepController.running = true;
				myAgent.enabled = true;
				alleywayChase = true;
			});
		});
	}

	public void PlayerRanAway()
	{
		EnemyStateManager.RemoveEnemyState(ENEMY_STATE.DELFALCO);
		DeSpawn(noFireWindow: true);
		myAgent.enabled = false;
		destinationReached = null;
		destInProgress = false;
		alleywayChase = false;
		alleywayPatrolActive = false;
		noticedPlayerAlleyway = false;
	}

	public void StageDumpsterJump()
	{
		Debug.Log("[Delfalco] Staging Dumpster Jump");
		StageDoorJump(new Vector3(15.341f, 0f, 200.92f), new Vector3(0f, 90f, 0f), new Vector3(15.341f, 0f, 199.354f), new Vector3(15.876f, 0.676f, 199.101f), new Vector3(0.1130756f, 1f, 1.974651f), dontCheckDoor: true);
	}

	private void StageDoorJump(Vector3 Spawn, Vector3 rot, Vector3 MoveTo, Vector3 triggerPos, Vector3 triggerSize, bool dontCheckDoor = false)
	{
		if (doorJumpStaged)
		{
			return;
		}
		doorJumpStaged = true;
		DeSpawn(noFireWindow: true);
		base.transform.position = Spawn;
		base.transform.rotation = Quaternion.Euler(rot);
		myAnimator.SetBool("StageDoorJump", value: true);
		DelfalcoTrigger delfalcoTrigger = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<DelfalcoTrigger>();
		delfalcoTrigger.GetComponent<BoxCollider>().isTrigger = true;
		delfalcoTrigger.GetComponent<MeshRenderer>().enabled = false;
		delfalcoTrigger.name = "DelfalcoKillTrigger";
		delfalcoTrigger.transform.position = triggerPos;
		delfalcoTrigger.transform.localScale = triggerSize;
		delfalcoTrigger.playerEnterEvent.Event += delegate
		{
			if (dontCheckDoor)
			{
				TriggerDoorJump(MoveTo);
			}
			else if (LookUp.Doors.DeadDropDoor.IsOpen && StateManager.PlayerState == PLAYER_STATE.ROAMING)
			{
				TriggerDoorJump(MoveTo);
			}
		};
	}

	private void TriggerDoorJump(Vector3 moveTo)
	{
		if (duringKill)
		{
			return;
		}
		duringKill = true;
		myAnimator.SetBool("DoorJump", value: true);
		if (moveTo != Vector3.zero)
		{
			base.transform.DOMove(moveTo, 0.1f);
		}
		LookUp.Doors.DeadDropDoor.LockOutAutoClose = true;
		helperLight.enabled = true;
		lockDespawn = true;
		FlashLightBehaviour.Ins.LockOut();
		GameManager.BehaviourManager.CrossHairBehaviour.HideCrossHairGroup();
		GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.delfalcoJumpA);
		GameManager.TimeSlinger.FireTimer(0.625f, delegate
		{
			myAho.PlaySound(CustomSoundLookUp.delfalcoSawHit);
		});
		GameManager.TimeSlinger.FireTimer(2.9f, delegate
		{
			myAho.PlaySound(CustomSoundLookUp.delfalcoSawFlesh);
		});
		GameManager.TimeSlinger.FireTimer(4.17f, delegate
		{
			myAho.PlaySound(CustomSoundLookUp.delfalcoSawOut);
		});
		GameManager.TimeSlinger.FireTimer(5.28f, delegate
		{
			myAho.PlaySound(CustomSoundLookUp.delfalcoFall);
		});
		PauseManager.LockPause();
		roamController.Ins.LoseControl();
		CameraManager.Get(roamController.Ins.CameraIControl, out var ReturnCamera);
		Transform cameraTransform = ReturnCamera.transform;
		GameManager.TimeSlinger.FireTimer(0.05f, delegate
		{
			cameraTransform.SetParent(GameObject.Find("SM_MrDelfalco.ao/Root/UN_j_PropB_Output/CameraHolder/CameraHolder_end").transform);
			cameraTransform.DOLocalMove(new Vector3(0f, 0f, 0f), 0.1f).SetEase(Ease.Linear);
			cameraTransform.DOLocalRotate(new Vector3(0f, 180f, 0f), 0.1f).SetEase(Ease.Linear);
		});
		GameManager.TimeSlinger.FireTimer(4.25f, delegate
		{
			DoubleVision doubleVision = cameraTransform.gameObject.GetComponent<DoubleVision>();
			LensDistortionBlur lensDistortionBlur = cameraTransform.gameObject.GetComponent<LensDistortionBlur>();
			doubleVision.enabled = true;
			lensDistortionBlur.enabled = true;
			DOTween.To(() => doubleVision.Displace.x, delegate(float x)
			{
				doubleVision.Displace.x = x;
			}, 3f, 0.1f);
			DOTween.To(() => lensDistortionBlur.Distortion, delegate(float x)
			{
				lensDistortionBlur.Distortion = x;
			}, 0.5f, 0.1f);
		});
		GameManager.TimeSlinger.FireTimer(6.5f, delegate
		{
			UIManager.TriggerGameOver("YOU WERE FOUND");
		});
	}
}
