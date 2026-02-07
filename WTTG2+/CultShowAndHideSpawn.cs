using UnityEngine;

public class CultShowAndHideSpawn : CultSpawner
{
	[SerializeField]
	private DefinitionUnityEvent stageEvents;

	private Timer autoDespawnTimer;

	private CultSpawnDefinition currentSpawnData;

	private bool lookAwayTimeActive;

	private float lookAwayTimeStamp;

	private Camera mainCamera;

	private bool debugMSG;

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
			Debug.Log("[CultShowAndHideSpawn] Noir outside range despawned");
			lookAwayTimeActive = false;
			CultLooker.Ins.CheckForVisible = false;
			CultLooker.Ins.VisibleActions.Event -= resetLookAway;
			DeSpawn();
		}
		if (IAmSpawned && StateManager.PlayerLocation == PLAYER_LOCATION.DEAD_DROP && currentSpawnData.Location == PLAYER_LOCATION.LOBBY)
		{
			IAmSpawned = false;
			Debug.Log("[CultShowAndHideSpawn] Noir in lobby despawned");
			lookAwayTimeActive = false;
			CultLooker.Ins.CheckForVisible = false;
			CultLooker.Ins.VisibleActions.Event -= resetLookAway;
			DeSpawn();
		}
		if (IAmSpawned && StateManager.PlayerLocation == PLAYER_LOCATION.MAINTENANCE_ROOM && currentSpawnData.Location == PLAYER_LOCATION.HALL_WAY8)
		{
			IAmSpawned = false;
			Debug.Log("[CultShowAndHideSpawn] Noir near maintenance despawned");
			lookAwayTimeActive = false;
			CultLooker.Ins.CheckForVisible = false;
			CultLooker.Ins.VisibleActions.Event -= resetLookAway;
			DeSpawn();
		}
		if (IAmSpawned && CustomElevatorManager.Elevating)
		{
			IAmSpawned = false;
			Debug.Log("[CultShowAndHideSpawn] Using elevator, Despawning noir");
			lookAwayTimeActive = false;
			CultLooker.Ins.CheckForVisible = false;
			CultLooker.Ins.VisibleActions.Event -= resetLookAway;
			DeSpawn();
		}
		if (IAmSpawned && windowNoir && windowNoirDespawnTime && StateManager.PlayerState == PLAYER_STATE.COMPUTER)
		{
			windowNoir = false;
			windowNoirDespawnTime = false;
			Debug.Log("[CultShowAndHideSpawn] Noir stood on the Window too much, Despawning noir");
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
		triggerSpawn();
	}

	private void triggerSpawn()
	{
		float magnitude = (currentSpawnData.Position - mainCamera.transform.position).magnitude;
		if (magnitude >= 4f)
		{
			debugMSG = false;
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
			CultLooker.Ins.VisibleActions.Event += stageDeSpawn;
			CultLooker.Ins.CheckForVisible = true;
		}
		else
		{
			if (!debugMSG)
			{
				Debug.Log("Cannot spawn noir because you're too close to the spawn location");
			}
			debugMSG = true;
			GameManager.TimeSlinger.FireTimer(0.5f, triggerSpawn);
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
}
