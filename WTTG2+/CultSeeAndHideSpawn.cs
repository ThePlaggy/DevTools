using UnityEngine;

public class CultSeeAndHideSpawn : CultSpawner
{
	[SerializeField]
	private DefinitionUnityEvent stageEvents;

	private Timer autoDespawnTimer;

	private CultSpawnDefinition currentSpawnData;

	private bool lookAwayTimeActive;

	private float lookAwayTimeStamp;

	private Camera mainCamera;

	private Timer reRollStageSpawn;

	protected new void Awake()
	{
		base.Awake();
		CameraManager.Get(CAMERA_ID.MAIN, out mainCamera);
	}

	private void Update()
	{
		if (lookAwayTimeActive && Time.time - lookAwayTimeStamp >= currentSpawnData.LookAwayTime)
		{
			lookAwayTimeActive = false;
			CultLooker.Ins.CheckForVisible = false;
			CultLooker.Ins.VisibleActions.Event -= resetLookAway;
			DeSpawn();
		}
		if (IAmSpawned && StateManager.PlayerState == PLAYER_STATE.COMPUTER && currentSpawnData.Location != PLAYER_LOCATION.MAIN_ROON)
		{
			IAmSpawned = false;
			Debug.Log("[CultSeeAndHideSpawn] Noir outside range despawned");
			lookAwayTimeActive = false;
			CultLooker.Ins.CheckForVisible = false;
			CultLooker.Ins.VisibleActions.Event -= resetLookAway;
			DeSpawn();
		}
		if (IAmSpawned && StateManager.PlayerLocation == PLAYER_LOCATION.DEAD_DROP && currentSpawnData.Location == PLAYER_LOCATION.LOBBY)
		{
			IAmSpawned = false;
			Debug.Log("[CultSeeAndHideSpawn] Noir in lobby despawned");
			lookAwayTimeActive = false;
			CultLooker.Ins.CheckForVisible = false;
			CultLooker.Ins.VisibleActions.Event -= resetLookAway;
			DeSpawn();
		}
		if (IAmSpawned && StateManager.PlayerLocation == PLAYER_LOCATION.MAINTENANCE_ROOM && currentSpawnData.Location == PLAYER_LOCATION.HALL_WAY8)
		{
			IAmSpawned = false;
			Debug.Log("[CultSeeAndHideSpawn] Noir near maintenance despawned");
			lookAwayTimeActive = false;
			CultLooker.Ins.CheckForVisible = false;
			CultLooker.Ins.VisibleActions.Event -= resetLookAway;
			DeSpawn();
		}
		if (IAmSpawned && CustomElevatorManager.Elevating)
		{
			IAmSpawned = false;
			Debug.Log("[CultSeeAndHideSpawn] Using elevator, Despawning noir");
			lookAwayTimeActive = false;
			CultLooker.Ins.CheckForVisible = false;
			CultLooker.Ins.VisibleActions.Event -= resetLookAway;
			DeSpawn();
		}
		if (IAmSpawned && windowNoir && windowNoirDespawnTime && StateManager.PlayerState == PLAYER_STATE.COMPUTER)
		{
			windowNoir = false;
			windowNoirDespawnTime = false;
			Debug.Log("[CultSeeAndHideSpawn] Noir stood on the Window too much, Despawning noir");
			lookAwayTimeActive = false;
			CultLooker.Ins.CheckForVisible = false;
			CultLooker.Ins.VisibleActions.Event -= resetLookAway;
			DeSpawn();
		}
	}

	public void StageSpawn(Definition SetSpawnData)
	{
		currentSpawnData = (CultSpawnDefinition)SetSpawnData;
		CultLooker.Ins.TargetLocation = currentSpawnData.Position;
		stageEvents.Invoke(SetSpawnData);
		stageSpawn();
	}

	private void stageSpawn()
	{
		GameManager.TimeSlinger.KillTimer(reRollStageSpawn);
		if (CultLooker.Ins.IsTargetVisible(currentSpawnData.Position))
		{
			CultLooker.Ins.NotVisibleActions.Event += triggerSpawn;
			CultLooker.Ins.CheckForNotVisible = true;
		}
		else
		{
			triggerSpawn();
		}
	}

	private void triggerSpawn()
	{
		CultLooker.Ins.NotVisibleActions.Event -= triggerSpawn;
		float magnitude = (currentSpawnData.Position - mainCamera.transform.position).magnitude;
		if (magnitude >= 4f)
		{
			if (currentSpawnData.RotateSpawnTowardsPlayer)
			{
				Vector3 forward = roamController.Ins.transform.position - currentSpawnData.Position;
				Vector3 eulerAngles = Quaternion.LookRotation(forward).eulerAngles;
				eulerAngles.x = 0f;
				eulerAngles.z = 0f;
				Spawn(currentSpawnData.Position, eulerAngles);
				bool flag = false;
				SetupWindowNoir(currentSpawnData);
				IAmSpawned = true;
			}
			else
			{
				Spawn(currentSpawnData.Position, currentSpawnData.Rotation);
				bool flag2 = false;
				SetupWindowNoir(currentSpawnData);
				IAmSpawned = true;
			}
			GameManager.TimeSlinger.FireHardTimer(out autoDespawnTimer, 60f, base.DeSpawn);
			CultLooker.Ins.VisibleActions.Event -= stageDeSpawn;
			CultLooker.Ins.VisibleActions.Event += stageDeSpawn;
			CultLooker.Ins.CheckForVisible = true;
		}
		else
		{
			GameManager.TimeSlinger.FireHardTimer(out reRollStageSpawn, 0.5f, stageSpawn);
		}
	}

	private void stageDeSpawn()
	{
		GameManager.TimeSlinger.KillTimer(autoDespawnTimer);
		CultLooker.Ins.VisibleActions.Event -= stageDeSpawn;
		CultLooker.Ins.NotVisibleActions.Event += triggerDeSpawn;
		CultLooker.Ins.CheckForNotVisible = true;
	}

	private void triggerDeSpawn()
	{
		CultLooker.Ins.NotVisibleActions.Event -= triggerDeSpawn;
		if (currentSpawnData.HasLookAwayTime)
		{
			CultLooker.Ins.VisibleActions.Event -= resetLookAway;
			CultLooker.Ins.VisibleActions.Event += resetLookAway;
			CultLooker.Ins.CheckForVisible = true;
			lookAwayTimeStamp = Time.time;
			lookAwayTimeActive = true;
		}
		else
		{
			DeSpawn();
		}
	}

	private void resetLookAway()
	{
		lookAwayTimeActive = false;
		CultLooker.Ins.VisibleActions.Event -= resetLookAway;
		CultLooker.Ins.NotVisibleActions.Event += triggerDeSpawn;
		CultLooker.Ins.CheckForNotVisible = true;
	}

	private void balcDoorFix()
	{
		Debug.Log("This line should never appear in the debug log. Balcony door glitch was fixed. If you see this, then there is something seriously wrong with the mod. Please report this bug immediately.");
	}
}
