using System;
using UnityEngine;

public class HitmanPatrolBehaviour : MonoBehaviour
{
	public static HitmanPatrolBehaviour Ins;

	[SerializeField]
	public LightTrigger[] lightsToCheck = new LightTrigger[0];

	[SerializeField]
	public PatrolPointDefinition mainRoomExitPatrolPoint;

	[SerializeField]
	public PatrolPointDefinition[] mainRoomPatrolPoints = new PatrolPointDefinition[0];

	[SerializeField]
	public PatrolPointDefinition[] bathRoomPatrolPoints = new PatrolPointDefinition[0];

	private HitmanBathroomJump bathroomJump = new HitmanBathroomJump();

	private bool checkIfPlayerIsPeaking;

	private bool checkMicCheck;

	private Timer lockHideControllerTimer;

	private bool lockOutExit;

	private int micCheckCount;

	private float micCheckTimeStamp;

	private int micLoudHitCount;

	private int patrolCount;

	private Timer patrolTimer;

	private float peakingTimeStamp;

	private int peekCount;

	private bool playerPeaking;

	private HitmanShowerCaughtJump showerCaughtJump = new HitmanShowerCaughtJump();

	private HitmanShowerJump showerJump = new HitmanShowerJump();

	private HitmanWardrobeCaughtJump wardrobeCaughtJump = new HitmanWardrobeCaughtJump();

	private HitmanWardrobeJump wardrobeJump = new HitmanWardrobeJump();

	private void Awake()
	{
		Ins = this;
	}

	private void Update()
	{
		if (checkIfPlayerIsPeaking && Time.time - peakingTimeStamp >= EnemyManager.HitManManager.Data.CheckPeakingInterval)
		{
			peakingTimeStamp = Time.time;
			if (playerPeaking)
			{
				peekCount++;
			}
			if (peekCount >= EnemyManager.HitManManager.Data.MaxPeakCount)
			{
				checkIfPlayerIsPeaking = false;
				checkMicCheck = false;
				stageCaughtDoom();
			}
		}
		if (checkMicCheck && Time.time - micCheckTimeStamp >= EnemyManager.HitManManager.Data.MicCheckInterval)
		{
			micCheckTimeStamp = Time.time;
			if (micLoudHitCount >= EnemyManager.HitManManager.Data.AddMicCheckThreshold)
			{
				micCheckCount++;
			}
			if (micCheckCount >= EnemyManager.HitManManager.Data.MaxMicCheck)
			{
				checkIfPlayerIsPeaking = false;
				checkMicCheck = false;
				stageCaughtDoom();
			}
			micLoudHitCount = 0;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		for (int i = 0; i < mainRoomPatrolPoints.Length; i++)
		{
			Gizmos.DrawWireCube(mainRoomPatrolPoints[i].Position, new Vector3(0.2f, 0.2f, 0.2f));
		}
		Gizmos.color = Color.blue;
		for (int j = 0; j < bathRoomPatrolPoints.Length; j++)
		{
			Gizmos.DrawWireCube(bathRoomPatrolPoints[j].Position, new Vector3(0.2f, 0.2f, 0.2f));
		}
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(mainRoomExitPatrolPoint.Position, new Vector3(0.2f, 0.2f, 0.2f));
	}

	public void Patrol()
	{
		switch (StateManager.PlayerLocation)
		{
		case PLAYER_LOCATION.BATH_ROOM:
			mainRoomStageHunt();
			break;
		case PLAYER_LOCATION.MAIN_ROON:
			mainRoomStageHunt();
			break;
		}
	}

	public void PickNextPatrol()
	{
		patrolCount--;
		switch (UnityEngine.Random.Range(1, 4))
		{
		case 3:
			HitmanBehaviour.Ins.TriggerAnim("lookIdle3");
			break;
		case 2:
			HitmanBehaviour.Ins.TriggerAnim("lookIdle2");
			break;
		case 1:
			HitmanBehaviour.Ins.TriggerAnim("lookIdle1");
			LucasPlusManager.Ins.PlayComeOut();
			break;
		}
		if (patrolCount > 0)
		{
			GameManager.TimeSlinger.FireHardTimer(out patrolTimer, 8f, delegate
			{
				HitmanBehaviour.Ins.TriggerAnim("idle");
				pickPatrolPoint();
			});
		}
		else
		{
			GameManager.TimeSlinger.FireHardTimer(out patrolTimer, 8f, delegate
			{
				HitmanBehaviour.Ins.TriggerAnim("idle");
				patrolOut();
			});
		}
	}

	public void ReachedPatrolExitPoint()
	{
		checkIfPlayerIsPeaking = false;
		checkMicCheck = false;
		hideController.Ins.PlayerPeakingEvent.Event -= playerIsPeaking;
		hideController.Ins.HideTrigger.LeaveDoom = false;
		hideController.Ins.HideTrigger.StageLeaveDoomActions.Event -= stageLeftHidingJump;
		hideController.Ins.HideTrigger.LeaveDoomActions.Event -= leftHidingJump;
		switch (StateManager.PlayerLocation)
		{
		case PLAYER_LOCATION.BATH_ROOM:
			HitmanBehaviour.Ins.LeaveMainRoom();
			break;
		case PLAYER_LOCATION.MAIN_ROON:
			HitmanBehaviour.Ins.LeaveMainRoom();
			break;
		}
	}

	private void mainRoomStageHunt()
	{
		int num = 0;
		hideController.Ins.HideTrigger.LockedOut = true;
		hideController.Ins.HideTrigger.LeaveDoom = true;
		hideController.Ins.HideTrigger.StageLeaveDoomActions.Event += stageLeftHidingJump;
		hideController.Ins.HideTrigger.LeaveDoomActions.Event += leftHidingJump;
		if (PoliceScannerBehaviour.Ins != null && PoliceScannerBehaviour.Ins.ownPoliceScanner && PoliceScannerBehaviour.Ins.IsOn)
		{
			num++;
		}
		if (RouterBehaviour.Ins != null && RouterBehaviour.Ins.Owned && RouterBehaviour.Ins.RouterIsActive)
		{
			num++;
		}
		if (ComputerPowerHook.Ins.PowerOn)
		{
			GameManager.BehaviourManager.NotesBehaviour.ClearNotes();
			double num2 = DOSCoinsCurrencyManager.CurrentCurrency;
			float num3 = UnityEngine.Random.Range(0.5f, 0.9f);
			DOSCoinsCurrencyManager.RemoveCurrency((float)Math.Round(num2 * (double)num3, 3));
			num++;
		}
		for (int i = 0; i < lightsToCheck.Length; i++)
		{
			if (lightsToCheck[i].LightsAreOn)
			{
				num++;
			}
		}
		if (LookUp.Doors.MainDoor.SetDistanceLODs != null)
		{
			for (int j = 0; j < LookUp.Doors.MainDoor.SetDistanceLODs.Length; j++)
			{
				LookUp.Doors.MainDoor.SetDistanceLODs[j].OverwriteCulling = true;
			}
		}
		patrolCount = Mathf.Max(3, num * 2);
		HitmanBehaviour.Ins.SplineMove.PathIsCompleted += mainRoomHunt;
		HitmanBehaviour.Ins.EnterMainRoom();
		LucasPlusManager.Ins.playedComeOut = false;
	}

	private void mainRoomHunt()
	{
		hideController.Ins.PlayerPeakingEvent.Event += playerIsPeaking;
		peekCount = 0;
		peakingTimeStamp = Time.time;
		checkIfPlayerIsPeaking = true;
		hideController.Ins.HideTrigger.LockedOut = false;
		HitmanBehaviour.Ins.SplineMove.PathIsCompleted -= mainRoomHunt;
		LookUp.Doors.MainDoor.ForceDoorClose();
		if (FlashLightBehaviour.Ins.LightOn)
		{
			stageCaughtDoom();
		}
		else
		{
			FlashLightBehaviour.Ins.FlashLightWentOn.Event += flashLightWasTriggered;
		}
		pickPatrolPoint();
	}

	private void playerIsPeaking(float SetValue)
	{
		playerPeaking = SetValue >= 0.5f;
	}

	private void playerIsPeakingDoom(float SetValue)
	{
		if (SetValue <= 0.65f)
		{
			hideController.Ins.PlayerPeakingEvent.Event -= playerIsPeakingDoom;
			triggerCaughtDoom();
		}
	}

	private void playerVoiceLevel(float LoudLevel)
	{
		if (LoudLevel >= 0.65f)
		{
			micLoudHitCount++;
		}
	}

	private void pickPatrolPoint()
	{
		switch (StateManager.PlayerLocation)
		{
		case PLAYER_LOCATION.BATH_ROOM:
		{
			int num2 = UnityEngine.Random.Range(1, bathRoomPatrolPoints.Length);
			PatrolPointDefinition patrolPointDefinition2 = bathRoomPatrolPoints[num2];
			HitmanBehaviour.Ins.PatrolTo(patrolPointDefinition2);
			bathRoomPatrolPoints[num2] = bathRoomPatrolPoints[0];
			bathRoomPatrolPoints[0] = patrolPointDefinition2;
			break;
		}
		case PLAYER_LOCATION.MAIN_ROON:
		{
			int num = UnityEngine.Random.Range(1, mainRoomPatrolPoints.Length);
			PatrolPointDefinition patrolPointDefinition = mainRoomPatrolPoints[num];
			HitmanBehaviour.Ins.PatrolTo(patrolPointDefinition);
			mainRoomPatrolPoints[num] = mainRoomPatrolPoints[0];
			mainRoomPatrolPoints[0] = patrolPointDefinition;
			break;
		}
		}
	}

	private void patrolOut()
	{
		if (!lockOutExit)
		{
			switch (StateManager.PlayerLocation)
			{
			case PLAYER_LOCATION.BATH_ROOM:
				HitmanBehaviour.Ins.PatrolTo(mainRoomExitPatrolPoint);
				break;
			case PLAYER_LOCATION.MAIN_ROON:
				HitmanBehaviour.Ins.PatrolTo(mainRoomExitPatrolPoint);
				break;
			}
		}
		FlashLightBehaviour.Ins.FlashLightWentOn.Event -= flashLightWasTriggered;
	}

	private void stageLeftHidingJump()
	{
		hideController.Ins.PlayerPeakingEvent.Event -= playerIsPeaking;
		DataManager.LockSave = true;
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		GameManager.TimeSlinger.KillTimer(patrolTimer);
		HitmanBehaviour.Ins.KillPatrol();
		switch (StateManager.PlayerLocation)
		{
		case PLAYER_LOCATION.BATH_ROOM:
			if (HitmanBehaviour.Ins.InBathRoom)
			{
				showerJump.Stage();
				break;
			}
			PauseManager.UnLockPause();
			GameManager.InteractionManager.UnLockInteraction();
			hideController.Ins.HideTrigger.LockedOut = false;
			hideController.Ins.HideTrigger.LeaveDoom = false;
			bathroomJump.Stage();
			LookUp.Doors.BathroomDoor.DoorOpenEvent.AddListener(bathroomJump.Execute);
			LookUp.Doors.BathroomDoor.LockOutAutoClose = true;
			break;
		case PLAYER_LOCATION.MAIN_ROON:
			wardrobeJump.Stage();
			break;
		}
	}

	private void leftHidingJump()
	{
		HitmanBehaviour.Ins.GunFlashDoneEvents.Event += EnemyManager.HitManManager.GunFlashGameOver;
		hideController.Ins.HideTrigger.StageLeaveDoomActions.Event -= stageLeftHidingJump;
		hideController.Ins.HideTrigger.LeaveDoomActions.Event -= leftHidingJump;
		switch (StateManager.PlayerLocation)
		{
		case PLAYER_LOCATION.BATH_ROOM:
			if (HitmanBehaviour.Ins.InBathRoom)
			{
				showerJump.Execute();
			}
			break;
		case PLAYER_LOCATION.MAIN_ROON:
			wardrobeJump.Execute();
			break;
		}
	}

	private void stageCaughtDoom()
	{
		lockOutExit = true;
		hideController.Ins.PlayerPeakingEvent.Event -= playerIsPeaking;
		hideController.Ins.PlayerPeakingEvent.Event += playerIsPeakingDoom;
		hideController.Ins.HideTrigger.LockedOut = true;
		hideController.Ins.HideTrigger.LeaveDoom = false;
	}

	private void triggerCaughtDoom()
	{
		DataManager.LockSave = true;
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		GameManager.TimeSlinger.KillTimer(patrolTimer);
		HitmanBehaviour.Ins.KillPatrol();
		hideController.Ins.HideTrigger.LockedOut = true;
		hideController.Ins.HideTrigger.LeaveDoom = false;
		hideController.Ins.HideTrigger.StageLeaveDoomActions.Event -= stageLeftHidingJump;
		hideController.Ins.HideTrigger.LeaveDoomActions.Event -= leftHidingJump;
		switch (StateManager.PlayerLocation)
		{
		case PLAYER_LOCATION.BATH_ROOM:
			if (HitmanBehaviour.Ins.InBathRoom)
			{
				showerCaughtJump.Execute();
				break;
			}
			PauseManager.UnLockPause();
			GameManager.InteractionManager.UnLockInteraction();
			hideController.Ins.HideTrigger.LockedOut = false;
			hideController.Ins.HideTrigger.LeaveDoom = false;
			bathroomJump.Stage();
			LookUp.Doors.BathroomDoor.DoorOpenEvent.AddListener(bathroomJump.Execute);
			GameManager.TimeSlinger.FireTimer(5f, LookUp.Doors.MainDoor.ForceOpenDoor);
			GameManager.TimeSlinger.FireTimer(8f, LookUp.Doors.MainDoor.ForceDoorClose);
			break;
		case PLAYER_LOCATION.MAIN_ROON:
			wardrobeCaughtJump.Execute();
			break;
		}
	}

	private void flashLightWasTriggered(bool IsOn)
	{
		if (IsOn)
		{
			FlashLightBehaviour.Ins.FlashLightWentOn.Event -= flashLightWasTriggered;
			stageCaughtDoom();
		}
	}
}
