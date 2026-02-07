using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CultManager : MonoBehaviour
{
	public static bool HuntingLights;

	public static bool StagedEnd;

	[SerializeField]
	private CultDataDefinition data;

	[SerializeField]
	private CultMaleBehaviour cultMale;

	[SerializeField]
	private CultFemaleBehaviour cultFemale;

	[SerializeField]
	private Mesh debugMesh;

	[SerializeField]
	private LightTrigger[] lightsToCheck = new LightTrigger[0];

	[SerializeField]
	private CultSpawnDefinition[] normalSpawns = new CultSpawnDefinition[0];

	[SerializeField]
	private CultSpawnDefinition[] darkSpawns = new CultSpawnDefinition[0];

	public int keyDiscoveryCount;

	private bool closeJumpActive;

	private CultLooker cultLooker;

	private Dictionary<PLAYER_LOCATION, List<CultSpawnDefinition>> darkSpawnLookUp = new Dictionary<PLAYER_LOCATION, List<CultSpawnDefinition>>();

	private Timer forceNormalSpawnTimer;

	private Timer lightCheckTimer;

	private bool lightsOffModeActivated;

	private CultData myCultData;

	private bool normalSpawnActivated;

	private float normalSpawnFireWindow;

	private bool normalSpawnFireWindowActive;

	private Dictionary<PLAYER_LOCATION, List<CultSpawnDefinition>> normalSpawnLookUp = new Dictionary<PLAYER_LOCATION, List<CultSpawnDefinition>>();

	private float normalSpawnTimeStamp;

	private bool powerOffAttackModeActivated;

	private CultSpawnDefinition spawnData;

	public string NoirDebug => (normalSpawnFireWindow - (Time.time - normalSpawnTimeStamp) > 0f) ? ((int)(normalSpawnFireWindow - (Time.time - normalSpawnTimeStamp))).ToString() : "-1";

	private void Awake()
	{
		EnemyManager.CultManager = this;
		GameManager.StageManager.Stage += stageMe;
		GameManager.StageManager.ThreatsNowActivated += threatsActivated;
		GameManager.TheCloud.KeyDiscoveredEvent.Event += keyWasDiscovered;
		for (int i = 0; i < normalSpawns.Length; i++)
		{
			if (!normalSpawnLookUp.ContainsKey(normalSpawns[i].Location))
			{
				normalSpawnLookUp.Add(normalSpawns[i].Location, new List<CultSpawnDefinition>());
			}
			normalSpawnLookUp[normalSpawns[i].Location].Add(normalSpawns[i]);
		}
		CultSpawnDefinition cultSpawnDefinition = Object.Instantiate(darkSpawns[0]);
		cultSpawnDefinition.Location = PLAYER_LOCATION.MAIN_ROON;
		cultSpawnDefinition.Position = new Vector3(26.35f, 39.6f, -2.8f);
		cultSpawnDefinition.Rotation = new Vector3(0f, 180f, 0f);
		cultSpawnDefinition.DistanceThreshold = 2.2f;
		cultSpawnDefinition.RotateSpawnTowardsPlayer = false;
		List<CultSpawnDefinition> list = darkSpawns.ToList();
		list.Add(cultSpawnDefinition);
		darkSpawns = list.ToArray();
		for (int j = 0; j < darkSpawns.Length; j++)
		{
			if (!darkSpawnLookUp.ContainsKey(darkSpawns[j].Location))
			{
				darkSpawnLookUp.Add(darkSpawns[j].Location, new List<CultSpawnDefinition>());
			}
			darkSpawnLookUp[darkSpawns[j].Location].Add(darkSpawns[j]);
		}
	}

	private void Update()
	{
		if (normalSpawnFireWindow - (Time.time - normalSpawnTimeStamp) > data.NormalSpawnFireWindowMax / (float)(howManyLightsAreOff() + 1))
		{
			generateNormalSpawnWindow();
		}
		if (normalSpawnFireWindowActive && Time.time - normalSpawnTimeStamp >= normalSpawnFireWindow)
		{
			normalSpawnFireWindowActive = false;
			attemptSpawn();
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		for (int i = 0; i < normalSpawns.Length; i++)
		{
			if (normalSpawns[i] != null)
			{
				Gizmos.DrawWireMesh(debugMesh, normalSpawns[i].Position, Quaternion.Euler(normalSpawns[i].Rotation));
			}
		}
		Gizmos.color = Color.red;
		for (int j = 0; j < darkSpawns.Length; j++)
		{
			if (darkSpawns[j] != null)
			{
				Gizmos.DrawWireMesh(debugMesh, darkSpawns[j].Position, Quaternion.Euler(darkSpawns[j].Rotation));
			}
		}
	}

	public void StageSpawn(Definition SpawnData)
	{
		spawnData = (CultSpawnDefinition)SpawnData;
	}

	public void IsPlayerSeen(bool Seen)
	{
		if (spawnData.SeePlayer && Seen && closeJumpActive)
		{
			closeJumpActive = false;
			triggerCloseJump();
		}
	}

	public void StageCloseJump()
	{
		DataManager.LockSave = true;
		DataManager.ClearGameData();
		if (spawnData.SeePlayer)
		{
			closeJumpActive = true;
		}
		else
		{
			triggerCloseJump();
		}
	}

	public void StageDeskJump()
	{
		DataManager.LockSave = true;
		DataManager.ClearGameData();
		cultMale.StageDeskJump();
		cultFemale.StageDeskJump();
	}

	public void TriggerDeskJump()
	{
		ComputerChairObject.Ins.SetToNotInUsePosition();
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.JumpHit4);
		cultMale.TriggerDeskJump();
		cultFemale.TriggerDeskJump();
		GameManager.TimeSlinger.FireTimer(5f, delegate
		{
			MainCameraHook.Ins.ClearARF(0.45f);
		});
		GameManager.TimeSlinger.FireTimer(5.4f, delegate
		{
			UIManager.TriggerGameOver("KILLED");
		});
	}

	public void DeSpawn()
	{
		EnemyStateManager.RemoveEnemyState(ENEMY_STATE.CULT);
		PowerStateManager.RemovePowerStateLock(STATE_LOCK_OCCASION.CULT);
		DataManager.LockSave = false;
		normalSpawnFireWindowActive = false;
		generateNormalSpawnWindow();
	}

	public void StageEndJump()
	{
		StagedEnd = true;
		cultMale.StageEndJump();
		CultMaleCamHelper.Ins.StageEndJump();
		LookUp.Doors.Door1.DoorWasOpenedEvent.AddListener(endDoorWasOpened);
	}

	public void attemptSpawn()
	{
		StateManager.PlayerLocationChangeEvents.Event -= attemptSpawn;
		if (EnemyStateManager.IsInEnemyStateOrLocked() || EnvironmentManager.PowerState != POWER_STATE.ON)
		{
			normalSpawnFireWindow /= 2f;
			normalSpawnTimeStamp = Time.time;
			normalSpawnFireWindowActive = true;
			return;
		}
		if (StateManager.PlayerLocation == PLAYER_LOCATION.UNKNOWN)
		{
			StateManager.PlayerLocationChangeEvents.Event += attemptSpawn;
			return;
		}
		StateManager.PlayerLocationChangeEvents.Event -= attemptSpawn;
		if (normalSpawnLookUp.ContainsKey(StateManager.PlayerLocation))
		{
			EnemyStateManager.AddEnemyState(ENEMY_STATE.CULT);
			PowerStateManager.AddPowerStateLock(STATE_LOCK_OCCASION.CULT);
			DataManager.LockSave = true;
			List<CultSpawnDefinition> list = normalSpawnLookUp[StateManager.PlayerLocation];
			int index = Random.Range(0, list.Count);
			list[index].InvokeSpawnEvent();
		}
		else
		{
			StateManager.PlayerLocationChangeEvents.Event += attemptSpawn;
		}
	}

	public void triggerCloseJump()
	{
		cultFemale.ValidSpawnLocationEvent.Event += performCloseJump;
		cultFemale.InValidSpawnLocationEvent.Event += reRollCloseJump;
		cultFemale.AttemptSpawnBehindPlayer();
	}

	private void reRollCloseJump()
	{
		cultFemale.ValidSpawnLocationEvent.Event -= performCloseJump;
		cultFemale.InValidSpawnLocationEvent.Event -= reRollCloseJump;
		GameManager.TimeSlinger.FireTimer(0.2f, triggerCloseJump);
	}

	private void performCloseJump()
	{
		DataManager.LockSave = true;
		DataManager.ClearGameData();
		cultFemale.ValidSpawnLocationEvent.Event -= performCloseJump;
		cultFemale.InValidSpawnLocationEvent.Event -= reRollCloseJump;
		CultRoamJumper.Ins.TriggerHammerJump();
		GameManager.AudioSlinger.PlaySoundWithCustomDelay(LookUp.SoundLookUp.JumpHit4, 0.25f);
		GameManager.TimeSlinger.FireTimer(4.7f, delegate
		{
			MainCameraHook.Ins.ClearARF(0.45f);
		});
		GameManager.TimeSlinger.FireTimer(5f, delegate
		{
			UIManager.TriggerGameOver("KILLED");
		});
		GameManager.TimeSlinger.FireTimer(6f, delegate
		{
			CultRoamJumper.Ins.ClearDOF();
		});
	}

	private void powerWentOff()
	{
		if (powerOffAttackModeActivated && !EnemyStateManager.IsInEnemyStateOrLocked() && darkSpawnLookUp.ContainsKey(StateManager.PlayerLocation))
		{
			int num = Random.Range(0, 10);
			if (keyDiscoveryCount switch
			{
				4 => num < 5, 
				5 => num < 6, 
				6 => num < 6, 
				7 => num < 8, 
				8 => num < 9, 
				_ => num < 3, 
			} || DifficultyManager.Nightmare)
			{
				EnemyStateManager.AddEnemyState(ENEMY_STATE.CULT);
				PowerStateManager.AddPowerStateLock(STATE_LOCK_OCCASION.CULT);
				DataManager.LockSave = true;
				List<CultSpawnDefinition> list = darkSpawnLookUp[StateManager.PlayerLocation];
				int index = Random.Range(0, list.Count);
				list[index].InvokeSpawnEvent();
			}
		}
	}

	private void forceActivateNormalSpawns()
	{
		normalSpawnActivated = true;
		generateNormalSpawnWindow();
	}

	private void generateNormalSpawnWindow()
	{
		int num = howManyLightsAreOff();
		normalSpawnFireWindow = Random.Range(data.NormalSpawnFireWindowMin, data.NormalSpawnFireWindowMax) / (float)(num + 1);
		normalSpawnTimeStamp = Time.time;
		normalSpawnFireWindowActive = true;
	}

	private void endDoorWasOpened()
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.JumpHit4);
		LookUp.Doors.Door1.DoorWasOpenedEvent.RemoveListener(endDoorWasOpened);
		LookUp.Doors.Door1.CancelAutoClose();
		CultRoamJumper.Ins.TriggerEndJump();
		CultMaleCamHelper.Ins.TriggerEndJump();
		cultMale.TriggerAnim("triggerEndJump");
	}

	private void stageMe()
	{
		GameManager.StageManager.Stage -= stageMe;
		myCultData = DataManager.Load<CultData>(99);
		if (myCultData == null)
		{
			myCultData = new CultData(99);
			myCultData.KeysDiscoveredCount = 0;
			myCultData.NormalSpawnActivated = false;
			myCultData.PowerOffAttackActivated = false;
			myCultData.LightsOffAttackActivated = false;
		}
		keyDiscoveryCount = myCultData.KeysDiscoveredCount;
	}

	private void threatsActivated()
	{
		GameManager.StageManager.ThreatsNowActivated -= threatsActivated;
		normalSpawnActivated = myCultData.NormalSpawnActivated;
		if (normalSpawnActivated)
		{
			generateNormalSpawnWindow();
		}
		else
		{
			GameManager.TimeSlinger.FireHardTimer(out forceNormalSpawnTimer, data.TimeRequredForNormalSpawn, forceActivateNormalSpawns);
		}
		powerOffAttackModeActivated = myCultData.PowerOffAttackActivated;
		if (powerOffAttackModeActivated)
		{
			EnvironmentManager.PowerBehaviour.PowerOffEvent.Event += powerWentOff;
		}
		lightsOffModeActivated = myCultData.LightsOffAttackActivated;
	}

	private void keyWasDiscovered()
	{
		keyDiscoveryCount++;
		if (!normalSpawnActivated && keyDiscoveryCount >= data.KeysRequiredForNormalSpawn)
		{
			GameManager.TimeSlinger.KillTimer(forceNormalSpawnTimer);
			normalSpawnActivated = true;
			generateNormalSpawnWindow();
			myCultData.NormalSpawnActivated = true;
		}
		if (!powerOffAttackModeActivated && keyDiscoveryCount >= data.KeysRequiredForPowerSpawn)
		{
			EnvironmentManager.PowerBehaviour.PowerOffEvent.Event += powerWentOff;
			powerOffAttackModeActivated = true;
			myCultData.PowerOffAttackActivated = true;
		}
		if (!lightsOffModeActivated && keyDiscoveryCount >= data.KeysRequiredForLightTrigger)
		{
			lightsOffModeActivated = true;
			myCultData.LightsOffAttackActivated = true;
		}
		myCultData.KeysDiscoveredCount = keyDiscoveryCount;
		EnemyManager.KidnapperManager.ReleaseKidnapper();
		DataManager.Save(myCultData);
	}

	public int howManyLightsAreOn()
	{
		int num = 0;
		for (int i = 0; i < lightsToCheck.Length; i++)
		{
			if (lightsToCheck[i].LightsAreOn)
			{
				num++;
			}
		}
		return num;
	}

	public int howManyLightsAreOff()
	{
		int num = 0;
		for (int i = 0; i < lightsToCheck.Length; i++)
		{
			if (!lightsToCheck[i].LightsAreOn)
			{
				num++;
			}
		}
		return num;
	}

	public void turnOffRandomLight(int i)
	{
		if (howManyLightsAreOn() > 0)
		{
			if (i == -1)
			{
				i = Random.Range(0, lightsToCheck.Length);
			}
			if (lightsToCheck[i].LightsAreOn)
			{
				lightsToCheck[i].triggerLightsNoSound();
				return;
			}
			i += Random.Range(0, lightsToCheck.Length - 1);
			i %= lightsToCheck.Length;
			turnOffRandomLight(i);
		}
	}

	public void turnOffAllLights()
	{
		if (HuntingLights || EnvironmentManager.PowerState == POWER_STATE.OFF)
		{
			return;
		}
		HuntingLights = true;
		for (int i = 0; i < lightsToCheck.Length; i++)
		{
			if (lightsToCheck[i].LightsAreOn)
			{
				lightsToCheck[i].triggerLightsNoSound();
			}
		}
	}

	public void TurnOffAllLightsDWA()
	{
		for (int i = 0; i < lightsToCheck.Length; i++)
		{
			if (lightsToCheck[i].LightsAreOn)
			{
				lightsToCheck[i].triggerLightsNoSound();
			}
		}
	}

	public void turnOffRandomLightSound(int i)
	{
		if (howManyLightsAreOn() > 0)
		{
			if (i == -1)
			{
				i = Random.Range(0, lightsToCheck.Length);
			}
			if (lightsToCheck[i].LightsAreOn)
			{
				lightsToCheck[i].triggerLights();
				return;
			}
			i += Random.Range(0, lightsToCheck.Length - 1);
			i %= lightsToCheck.Length;
			turnOffRandomLightSound(i);
		}
	}

	public void DevEnemyTimerChange(float time)
	{
		if (normalSpawnFireWindowActive)
		{
			normalSpawnFireWindow = time;
			normalSpawnTimeStamp = Time.time;
		}
	}

	public void BREAKLIGHT(int i)
	{
		if (howManyLightsAreOn() > 0)
		{
			if (i == -1)
			{
				i = Random.Range(0, lightsToCheck.Length);
			}
			if (lightsToCheck[i].LightsAreOn)
			{
				Debug.Log($"Light {i} is broken");
				lightsToCheck[i].KILLLIGHT(i);
			}
			else
			{
				i += Random.Range(0, lightsToCheck.Length - 1);
				i %= lightsToCheck.Length;
				BREAKLIGHT(i);
			}
		}
	}
}
