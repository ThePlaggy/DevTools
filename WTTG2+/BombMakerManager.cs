using UnityEngine;

public class BombMakerManager : MonoBehaviour
{
	public static bool scheduledAutoLeave;

	[HideInInspector]
	public BombMakerBehaviour bombMakerBehaviour;

	[HideInInspector]
	public int SulphurTaken;

	[HideInInspector]
	public BombMakerDataDefinition bmData;

	private bool bombMakerActivated;

	private AudioHubObject explosionHub1;

	private AudioHubObject explosionHub2;

	private bool forced;

	private int myID;

	private AudioHubObject packageHub;

	private bool sulphurActive;

	private float sulphurTimeStamp;

	private float sulphurWindow;

	public string SulphurDebug
	{
		get
		{
			if (TarotVengeance.Killed(ENEMY_STATE.BOMB_MAKER))
			{
				return "-2";
			}
			return (sulphurWindow - (Time.time - sulphurTimeStamp) > 0f) ? ((int)(sulphurWindow - (Time.time - sulphurTimeStamp))).ToString() : "-1";
		}
	}

	public bool IsBombMakerActive { get; private set; }

	private void Awake()
	{
		EnemyManager.BombMakerManager = this;
		bmData = new BombMakerDataDefinition();
		myID = base.transform.position.GetHashCode();
	}

	private void Start()
	{
		new GameObject("BombMakerBehaviour").AddComponent<BombMakerBehaviour>();
		Object.Instantiate(CustomObjectLookUp.BombMakerRecolorer).transform.position = Vector3.zero;
		GameObject.Find("deskController").AddComponent<BombMakerDeskJumper>();
		GameObject.Find("deskController").AddComponent<BombMakerDeskPresence>();
		AudioHubObject audioHubObject = new GameObject().AddComponent<AudioHubObject>();
		audioHubObject.transform.position = new Vector3(-0.813f, 40.849f, -0.224f);
		packageHub = audioHubObject;
		AudioHubObject audioHubObject2 = new GameObject().AddComponent<AudioHubObject>();
		audioHubObject2.transform.position = new Vector3(-24.266f, 39.582f, -6.24f);
		explosionHub1 = audioHubObject2;
		AudioHubObject audioHubObject3 = new GameObject().AddComponent<AudioHubObject>();
		audioHubObject3.transform.position = new Vector3(24.266f, 39.582f, -12.24f);
		explosionHub2 = audioHubObject3;
	}

	private void Update()
	{
		if (!DifficultyManager.CasualMode && sulphurActive && Time.time - sulphurTimeStamp >= sulphurWindow)
		{
			sulphurActive = false;
			triggerSulphurTimesUp();
		}
	}

	private void OnDestroy()
	{
		Object.Destroy(packageHub);
		Object.Destroy(explosionHub1);
		Object.Destroy(explosionHub2);
		scheduledAutoLeave = false;
		Debug.Log("[BombMaker Mod] Unloaded successfully");
	}

	public void ReleaseTheBombMaker()
	{
		if (!DifficultyManager.CasualMode && !IsBombMakerActive)
		{
			SulphurTaken = 0;
			GameManager.StageManager.ManuallyActivateThreats();
			IsBombMakerActive = true;
			GameManager.TimeSlinger.FireTimer(Random.Range(210f, 390f), BombMakerPresence);
			TarotVengeance.ActivateEnemy(ENEMY_STATE.BOMB_MAKER);
		}
	}

	private void triggerBombMakerKill()
	{
		if (!DifficultyManager.CasualMode)
		{
			if ((EnemyStateManager.IsInEnemyStateOrLocked() || EnvironmentManager.PowerState != POWER_STATE.ON || StateManager.BeingHacked) && (!EnemyStateManager.HasEnemyState(ENEMY_STATE.BOMB_MAKER) || EnvironmentManager.PowerState != POWER_STATE.ON || StateManager.BeingHacked))
			{
				sulphurWindow = 40f;
				sulphurTimeStamp = Time.time;
				sulphurActive = true;
				return;
			}
			PowerStateManager.AddPowerStateLock(STATE_LOCK_OCCASION.BOMB_MAKER);
			DataManager.LockSave = true;
			DataManager.ClearGameData();
			EnemyStateManager.AddEnemyState(ENEMY_STATE.BOMB_MAKER);
			BombMakerDeskJumper.Ins.AddComputerJump();
			BombMakerBehaviour.Ins.StageBombMakerOutsideKill();
			CamHookBehaviour.Interruptions = true;
			CamHookBehaviour.SwitchCameraStatus(enabled: false);
		}
	}

	private void activateSulphurTime()
	{
		if (!DifficultyManager.CasualMode)
		{
			sulphurWindow = Random.Range(bmData.SulphurCoolTimeMin, bmData.SulphurCoolTimeMax);
			sulphurTimeStamp = Time.time;
			sulphurActive = true;
		}
	}

	private void triggerSulphurTimesUp()
	{
		if (DifficultyManager.CasualMode || TarotVengeance.Killed(ENEMY_STATE.BOMB_MAKER))
		{
			return;
		}
		if ((EnemyStateManager.IsInEnemyStateOrLocked() || EnvironmentManager.PowerState != POWER_STATE.ON || StateManager.BeingHacked) && (!EnemyStateManager.HasEnemyState(ENEMY_STATE.BOMB_MAKER) || EnvironmentManager.PowerState != POWER_STATE.ON || StateManager.BeingHacked))
		{
			sulphurWindow = 40f;
			sulphurTimeStamp = Time.time;
			sulphurActive = true;
			return;
		}
		if (StateManager.PlayerLocation == PLAYER_LOCATION.DEAD_DROP || StateManager.PlayerLocation == PLAYER_LOCATION.DEAD_DROP_ROOM)
		{
			sulphurWindow = 10f;
			sulphurTimeStamp = Time.time;
			sulphurActive = true;
			return;
		}
		EnemyStateManager.AddEnemyState(ENEMY_STATE.BOMB_MAKER);
		if (SulphurInventory.SulphurAmount <= 0)
		{
			if (KeypadManager.Locked)
			{
				EnemyStateManager.RemoveEnemyState(ENEMY_STATE.BOMB_MAKER);
				sulphurWindow = Random.Range(120f, 150f);
				sulphurTimeStamp = Time.time;
				sulphurActive = true;
			}
			else
			{
				triggerBombMakerKill();
			}
			return;
		}
		EnemyStateManager.RemoveEnemyState(ENEMY_STATE.BOMB_MAKER);
		SulphurInventory.RemoveSulphur(1);
		SulphurTaken++;
		if (!DifficultyManager.Nightmare)
		{
			DOSCoinsCurrencyManager.AddCurrency(65f);
		}
		if (SulphurTaken >= bmData.maxSulphurReq)
		{
			DOSCoinsCurrencyManager.AddCurrency(DifficultyManager.Nightmare ? 50f : 100f);
			return;
		}
		PlayLaugh();
		activateSulphurTime();
		if (DifficultyManager.Nightmare)
		{
			if (Random.Range(0, 2) == 0)
			{
				GameManager.TimeSlinger.FireTimer(Random.Range(80, 120), PlayExplosion);
			}
			else
			{
				GameManager.TimeSlinger.FireTimer(Random.Range(50, 90), PlayExplosion2);
			}
		}
		else if (Random.Range(0, 2) == 0)
		{
			GameManager.TimeSlinger.FireTimer(Random.Range(340f, 480f), PlayExplosion);
		}
		else
		{
			GameManager.TimeSlinger.FireTimer(Random.Range(280f, 360f), PlayExplosion2);
		}
	}

	public void PlayLaugh()
	{
		if (StateManager.PlayerLocation == PLAYER_LOCATION.MAIN_ROON || StateManager.PlayerLocation == PLAYER_LOCATION.HALL_WAY8)
		{
			packageHub.PlaySound(CustomSoundLookUp.bombmaker);
		}
	}

	public void PlayExplosion()
	{
		if (StateManager.PlayerLocation == PLAYER_LOCATION.MAIN_ROON || StateManager.PlayerLocation == PLAYER_LOCATION.HALL_WAY8)
		{
			explosionHub1.PlaySound(CustomSoundLookUp.explosion);
		}
	}

	public void PlayExplosion2()
	{
		if (StateManager.PlayerLocation == PLAYER_LOCATION.MAIN_ROON || StateManager.PlayerLocation == PLAYER_LOCATION.HALL_WAY8)
		{
			explosionHub2.PlaySound(CustomSoundLookUp.explosion);
		}
	}

	private void BombMakerPresence()
	{
		if (!DifficultyManager.CasualMode && !TarotVengeance.Killed(ENEMY_STATE.BOMB_MAKER))
		{
			if ((EnemyStateManager.IsInEnemyStateOrLocked() || EnvironmentManager.PowerState != POWER_STATE.ON || StateManager.BeingHacked) && (!EnemyStateManager.HasEnemyState(ENEMY_STATE.BOMB_MAKER) || EnvironmentManager.PowerState != POWER_STATE.ON || StateManager.BeingHacked))
			{
				GameManager.TimeSlinger.FireTimer(Random.Range(20f, 40f), BombMakerPresence);
				return;
			}
			EnemyStateManager.AddEnemyState(ENEMY_STATE.BOMB_MAKER);
			BombMakerDeskPresence.Ins.AddComputerPresence();
			PowerStateManager.AddPowerStateLock(STATE_LOCK_OCCASION.BOMB_MAKER);
			scheduledAutoLeave = true;
			GameManager.TimeSlinger.FireTimer(25f, TriggerAutoPCLeave);
		}
	}

	public void ClearPresenceState()
	{
		EnemyStateManager.RemoveEnemyState(ENEMY_STATE.BOMB_MAKER);
		PowerStateManager.RemovePowerStateLock(STATE_LOCK_OCCASION.BOMB_MAKER);
		activateSulphurTime();
	}

	public void ReleaseTheBombMakerInstantly()
	{
		if (!DifficultyManager.CasualMode && !IsBombMakerActive)
		{
			SulphurTaken = 0;
			GameManager.StageManager.ManuallyActivateThreats();
			IsBombMakerActive = true;
			activateSulphurTime();
			PlayExplosion();
			GameManager.TimeSlinger.FireTimer(3f, PlayLaugh);
			TarotVengeance.ActivateEnemy(ENEMY_STATE.BOMB_MAKER);
		}
	}

	private void TriggerAutoPCLeave()
	{
		if (scheduledAutoLeave)
		{
			if (StateManager.PlayerState == PLAYER_STATE.COMPUTER)
			{
				computerController.Ins.LeaveMe();
			}
			else
			{
				GameManager.TimeSlinger.FireTimer(20f, TriggerAutoPCLeave);
			}
		}
	}

	public void ReleaseTheBombMakerFastDBG()
	{
		if (!DifficultyManager.CasualMode && !IsBombMakerActive)
		{
			SulphurTaken = 0;
			GameManager.StageManager.ManuallyActivateThreats();
			IsBombMakerActive = true;
			GameManager.TimeSlinger.FireTimer(Random.Range(2f, 8f), BombMakerPresence);
		}
	}

	public void DevEnemyTimerChange(float time)
	{
		if (sulphurActive)
		{
			sulphurWindow = time;
			sulphurTimeStamp = Time.time;
		}
	}
}
