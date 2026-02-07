using UnityEngine;

public class PhoneManager : MonoBehaviour
{
	public static PhoneManager Ins;

	private static readonly int maxIgnoreCallsNormal = 4;

	private static readonly int maxIgnoreCallsLeet = 2;

	private static readonly int maxIgnoreCallsNightmare = 1;

	[HideInInspector]
	public bool phoneKilled;

	private float fireTimeStamp;

	private float fireWindow;

	private bool fireWindowActive;

	private int ignoredCalls;

	private bool IsActive;

	private int keyDiscoverCount;

	public static bool HarbingerHappened;

	public string PhoneDebug => $"(NextCall:{phoneDebugPart2()},Ignored:{phoneDebugPart1()})";

	public string PhoneDebugNext => phoneDebugPart2();

	public string PhoneDebugIgnored => phoneDebugPart1();

	private void Awake()
	{
		Ins = this;
		BreatherNightNightTrigger.SpawnBreatherNightNight();
		GameManager.StageManager.ThreatsNowActivated += threatsActivated;
		GameManager.TheCloud.KeyDiscoveredEvent.Event += keyWasDiscovered;
	}

	private void Update()
	{
		if (!DifficultyManager.CasualMode && !TarotVengeance.Killed(ENEMY_STATE.BREATHER))
		{
			bool flag = false;
			if (fireWindowActive && Time.time - fireTimeStamp >= fireWindow)
			{
				fireWindowActive = false;
				RingRing();
			}
		}
	}

	private void OnDestroy()
	{
		Ins = null;
	}

	public void generateFireWindow()
	{
		if (!DifficultyManager.CasualMode && !TarotVengeance.Killed(ENEMY_STATE.BREATHER))
		{
			fireWindow = Random.Range(420f, DifficultyManager.Nightmare ? 610f : 1260f);
			fireTimeStamp = Time.time;
			fireWindowActive = true;
		}
	}

	private void RingRing()
	{
		if (!DifficultyManager.CasualMode && !TarotVengeance.Killed(ENEMY_STATE.BREATHER) && !IsActive)
		{
			if (EnemyStateManager.IsInEnemyStateOrLocked())
			{
				fireWindow = 5f;
				fireTimeStamp = Time.time;
				fireWindowActive = true;
			}
			else if (EnvironmentManager.PowerState != POWER_STATE.ON)
			{
				fireWindow = 30f;
				fireTimeStamp = Time.time;
				fireWindowActive = true;
			}
			else if (StateManager.PlayerLocation != PLAYER_LOCATION.MAIN_ROON)
			{
				fireWindow = 80f;
				fireTimeStamp = Time.time;
				fireWindowActive = true;
			}
			else if (GameManager.HackerManager.theSwan.SwanError)
			{
				fireWindow = 10f;
				fireTimeStamp = Time.time;
				fireWindowActive = true;
			}
			else
			{
				IsActive = true;
				PowerStateManager.AddPowerStateLock(STATE_LOCK_OCCASION.BREATHER);
				EnemyStateManager.AddEnemyState(ENEMY_STATE.BREATHER);
				RingPhone();
			}
		}
	}

	private void RingPhone()
	{
		if (!DifficultyManager.CasualMode && !TarotVengeance.Killed(ENEMY_STATE.BREATHER))
		{
			Debug.Log("[Phone] Attempting call...");
			PhoneBehaviour.Ins.AttemptCall();
		}
	}

	public void DespawnMe()
	{
		if (!phoneKilled)
		{
			Debug.Log("[Phone] Soft Despawn");
			IsActive = false;
			PowerStateManager.RemovePowerStateLock(STATE_LOCK_OCCASION.BREATHER);
			EnemyStateManager.RemoveEnemyState(ENEMY_STATE.BREATHER);
			generateFireWindow();
		}
	}

	private void keyWasDiscovered()
	{
		keyDiscoverCount++;
	}

	private void threatsActivated()
	{
		GameManager.StageManager.ThreatsNowActivated -= threatsActivated;
		generateFireWindow();
		TarotVengeance.ActivateEnemy(ENEMY_STATE.BREATHER);
	}

	public bool TalkAudio()
	{
		if (keyDiscoverCount == 3)
		{
			return Random.Range(0, 2) == 0;
		}
		return keyDiscoverCount >= 4;
	}

	public void PlayerIgnoredCall()
	{
		Debug.Log("[Phone] Player Ignored a call");
		ignoredCalls++;
		int num = maxIgnoreCallsNormal;
		if (DifficultyManager.LeetMode)
		{
			num = maxIgnoreCallsLeet;
		}
		if (DifficultyManager.Nightmare)
		{
			num = maxIgnoreCallsNightmare;
		}
		if (ignoredCalls >= num)
		{
			phoneKilled = true;
			PrepBreatherKill();
		}
	}

	public void generateSmallFireWindow()
	{
		if (!DifficultyManager.CasualMode && !TarotVengeance.Killed(ENEMY_STATE.BREATHER))
		{
			fireWindow = Random.Range(60f, 120f);
			fireTimeStamp = Time.time;
			fireWindowActive = true;
		}
	}

	public void ForceDespawnMe()
	{
		if (!phoneKilled)
		{
			Debug.Log("[Phone] Hard Despawn");
			IsActive = false;
			PowerStateManager.RemovePowerStateLock(STATE_LOCK_OCCASION.BREATHER);
			EnemyStateManager.RemoveEnemyState(ENEMY_STATE.BREATHER);
			generateSmallFireWindow();
		}
	}

	private string phoneDebugPart1()
	{
		return ignoredCalls.ToString();
	}

	private string phoneDebugPart2()
	{
		if (TarotVengeance.Killed(ENEMY_STATE.BREATHER))
		{
			return "-2";
		}
		return (fireWindow - (Time.time - fireTimeStamp) > 0f) ? ((int)(fireWindow - (Time.time - fireTimeStamp))).ToString() : "-1";
	}

	public void StageBreatherKill()
	{
		if (StateManager.BeingHacked)
		{
			GameManager.TimeSlinger.FireTimer(10f, StageBreatherKill);
			return;
		}
		if (KeypadManager.Locked)
		{
			BreatherFloor8Trigger.SpawnBreatherFloor8();
			return;
		}
		Debug.Log("[Phone] Stage Breather Kill");
		if (StateManager.PlayerState == PLAYER_STATE.COMPUTER)
		{
			GameManager.AudioSlinger.PlaySound((Random.Range(0, 2) == 0) ? CustomSoundLookUp.breather_hearyou : CustomSoundLookUp.breather_seeyou);
		}
		base.gameObject.AddComponent<BreatherNightNightJumper>().AddComputerJump();
	}

	private void PrepBreatherKill()
	{
		bool flag = false;
		GameManager.TimeSlinger.FireTimer(Random.Range(60f, 120f), StageBreatherKill);
	}

	public void ScheduleDevCall()
	{
		fireWindow = 3f;
		fireTimeStamp = Time.time;
		fireWindowActive = true;
	}

	public void DevEnemyTimerChange(float time)
	{
		if (fireWindowActive)
		{
			fireWindow = time;
			fireTimeStamp = Time.time;
		}
	}

	public void StageHarbinger()
	{
		ignoredCalls--;
		PhoneBehaviour.DevCall = true;
		PhoneBehaviour.DevAudioClip = CustomSoundLookUp.harbinger;
		fireWindow = 6f;
		fireTimeStamp = Time.time;
		fireWindowActive = true;
	}
}
