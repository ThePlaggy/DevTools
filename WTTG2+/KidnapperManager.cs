using System;
using DG.Tweening;
using UnityEngine;

public class KidnapperManager : MonoBehaviour
{
	private bool kidnapperReleased;

	private bool kidnapperTimerActive;

	private float kidnapperTimeStamp;

	private float kidnapperWindow;

	public static bool KidnapperTime;

	public const float TIME_BEFORE_DOOM = 30f;

	public const int KEYS_FOR_POWER_EVENT = 3;

	private static readonly Vector3 stairwayVector = new Vector3(18.368f, 40.8183f, -4.509f);

	private static readonly Vector3 elevatorVector = new Vector3(0.411f, 40.8183f, -4.509f);

	[NonSerialized]
	public AudioHubObject myAho;

	private GameObject theAho;

	private AudioFileDefinition[] myFootsteps;

	public static bool CanDoom;

	public static bool LeftHiding;

	public string KidnapperDebug
	{
		get
		{
			if (TarotVengeance.Killed(ENEMY_STATE.KIDNAPPER))
			{
				return "-2";
			}
			return (kidnapperWindow - (Time.time - kidnapperTimeStamp) > 0f) ? ((int)(kidnapperWindow - (Time.time - kidnapperTimeStamp))).ToString() : "-1";
		}
	}

	private void Awake()
	{
		UnityEngine.Object.Instantiate(CustomObjectLookUp.Kidnapper);
		EnemyManager.KidnapperManager = this;
	}

	private void Start()
	{
		theAho = new GameObject();
		theAho.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		myAho = theAho.AddComponent<AudioHubObject>();
		myFootsteps = new AudioFileDefinition[3];
		myFootsteps[0] = CustomSoundLookUp.KidnapperFootstep1;
		myFootsteps[1] = CustomSoundLookUp.KidnapperFootstep2;
		myFootsteps[2] = CustomSoundLookUp.KidnapperFootstep3;
	}

	private void Update()
	{
		if (!DifficultyManager.CasualMode && !EventSlinger.HalloweenEvent)
		{
			if (kidnapperTimerActive && Time.time - kidnapperTimeStamp >= kidnapperWindow)
			{
				kidnapperTimerActive = false;
				timesUpCheck();
			}
			if (CanDoom && (StateManager.PlayerState != PLAYER_STATE.HIDING || FlashLightBehaviour.Ins.LightOn))
			{
				CanDoom = false;
				Debug.Log("[Kidnapper Mod] Destroyed Walking AHO");
				LeftHiding = true;
				theAho.SetActive(value: false);
			}
		}
	}

	private void timesUp()
	{
		if (DifficultyManager.CasualMode || EventSlinger.HalloweenEvent || TarotVengeance.Killed(ENEMY_STATE.KIDNAPPER))
		{
			return;
		}
		int num = EnemyManager.CultManager.howManyLightsAreOn();
		if (StateManager.PlayerState == PLAYER_STATE.COMPUTER && num < 3)
		{
			switch (num)
			{
			default:
				return;
			case 2:
				if (UnityEngine.Random.Range(0, 100) > 25 && !DifficultyManager.Nightmare)
				{
					break;
				}
				goto case 0;
			case 1:
				if (UnityEngine.Random.Range(0, 100) > 75 && !DifficultyManager.Nightmare)
				{
					break;
				}
				goto case 0;
			case 0:
				KidnapperFail();
				return;
			}
			KidnapperPass();
		}
		else if (num == (DifficultyManager.Nightmare ? 1 : 0) && StateManager.PlayerLocation != PLAYER_LOCATION.MAIN_ROON && StateManager.PlayerLocation != PLAYER_LOCATION.BATH_ROOM && StateManager.PlayerLocation != PLAYER_LOCATION.OUTSIDE)
		{
			KidnapperFail(outside: true);
		}
		else
		{
			KidnapperPass();
		}
	}

	private void KidnapperPass()
	{
		EnemyManager.CultManager.turnOffRandomLight(-1);
		EnemyStateManager.RemoveEnemyState(ENEMY_STATE.KIDNAPPER);
		activateKidnapperTime();
	}

	private void KidnapperFail(bool outside = false)
	{
		EnemyStateManager.AddEnemyState(ENEMY_STATE.KIDNAPPER);
		if (outside)
		{
			Jumps_POKI();
		}
		else
		{
			Jumps_PCJump();
		}
	}

	public void activateKidnapperTime()
	{
		if (!DifficultyManager.CasualMode && !EventSlinger.HalloweenEvent)
		{
			kidnapperWindow = ((DifficultyManager.Nightmare || DifficultyManager.LeetMode) ? UnityEngine.Random.Range(200f, 400f) : UnityEngine.Random.Range(300f, 600f));
			kidnapperTimeStamp = Time.time;
			kidnapperTimerActive = true;
		}
	}

	public void ReleaseKidnapper()
	{
		if (!DifficultyManager.CasualMode && !EventSlinger.HalloweenEvent && !kidnapperReleased)
		{
			kidnapperReleased = true;
			activateKidnapperTime();
			TarotVengeance.ActivateEnemy(ENEMY_STATE.KIDNAPPER);
		}
	}

	private void timesUpCheck()
	{
		if (!DifficultyManager.CasualMode && !EventSlinger.HalloweenEvent && !TarotVengeance.Killed(ENEMY_STATE.KIDNAPPER))
		{
			if (EnemyStateManager.IsInEnemyStateOrLocked() || EnvironmentManager.PowerState != POWER_STATE.ON || StateManager.BeingHacked)
			{
				kidnapperWindow = 50f;
				kidnapperTimeStamp = Time.time;
				kidnapperTimerActive = true;
			}
			else if (StateManager.PlayerState == PLAYER_STATE.COMPUTER)
			{
				EnemyStateManager.AddEnemyState(ENEMY_STATE.KIDNAPPER);
				timesUp();
			}
			else if (StateManager.PlayerLocation != PLAYER_LOCATION.MAIN_ROON && StateManager.PlayerLocation != PLAYER_LOCATION.BATH_ROOM)
			{
				EnemyStateManager.AddEnemyState(ENEMY_STATE.KIDNAPPER);
				timesUp();
			}
			else
			{
				kidnapperWindow = 5f;
				kidnapperTimeStamp = Time.time;
				kidnapperTimerActive = true;
			}
		}
	}

	public void DevEnemyTimerChange(float time)
	{
		if (kidnapperTimerActive)
		{
			kidnapperWindow = time;
			kidnapperTimeStamp = Time.time;
		}
	}

	public void AddKidnapperPowerEvent()
	{
		if (!DifficultyManager.CasualMode && !EventSlinger.HalloweenEvent && !TarotVengeance.Killed(ENEMY_STATE.KIDNAPPER))
		{
			EnemyStateManager.AddEnemyState(ENEMY_STATE.KIDNAPPER);
			KidnapperTime = true;
			TrackerManager.NotifyUserBeingTracked(-1f);
			Jumps_PIKO();
		}
	}

	public void ExecutePowerOffEvent()
	{
		if (DifficultyManager.CasualMode || EventSlinger.HalloweenEvent || TarotVengeance.Killed(ENEMY_STATE.KIDNAPPER))
		{
			return;
		}
		theAho.transform.position = stairwayVector;
		GameManager.TimeSlinger.FireTimer(DifficultyManager.Nightmare ? 10f : 15f, delegate
		{
			if (StateManager.PlayerState != PLAYER_STATE.HIDING || FlashLightBehaviour.Ins.LightOn)
			{
				Debug.Log("[Kidnapper Mod] Failed to hide in time");
			}
			else
			{
				CanDoom = true;
				Debug.Log("[Kidnapper Mod] Starting walk event");
				doorlogBehaviour.MayAddDoorlog("Floor 8 Hallway", mode: true);
				myAho.PlaySound(CustomSoundLookUp.TheSource);
				GameManager.TimeSlinger.FireTimer(5f, delegate
				{
					GameManager.TimeSlinger.FireTimer(1f, (Action)delegate
					{
						myAho.PlaySound(myFootsteps[UnityEngine.Random.Range(0, myFootsteps.Length)]);
					}, 20);
				});
				theAho.transform.DOMove(elevatorVector, 30f).OnComplete(delegate
				{
					myAho.PlaySound(CustomSoundLookUp.NothingHere);
					GameManager.TimeSlinger.FireTimer(0.25f, delegate
					{
						if (!LeftHiding)
						{
							CanDoom = false;
							EnemyStateManager.RemoveEnemyState(ENEMY_STATE.KIDNAPPER);
							KidnapperNewBehaviour.Ins.UnstageInsideJumpscare();
							Debug.Log("[Kidnapper Mod] I am gone");
							doorlogBehaviour.MayAddDoorlog("Floor 8 Hallway", mode: false);
							GameManager.TimeSlinger.FireTimer(5f, delegate
							{
								theAho.transform.position = Vector3.zero;
							});
						}
					});
				});
			}
		});
	}

	public void Jumps_PCJump()
	{
		KidnapperNewBehaviour.Ins.Spawn(new Vector3(3.05f, 39.82f, -2.96f), new Vector3(0f, -190f, 0f));
		KidnapperNewBehaviour.Ins.AddJump();
	}

	public void Jumps_PIKO()
	{
		KidnapperNewBehaviour.Ins.Spawn(new Vector3(-13.186f, 39.78f, -5.5f), Vector3.zero);
		KidnapperNewBehaviour.Ins.StageInsideJumpscare();
		KidnapperNewBehaviour.Ins.AddOutsideJump();
	}

	public void Jumps_POKI()
	{
		KidnapperNewBehaviour.Ins.Spawn(new Vector3(-3.256f, 39.78f, -4.7f), new Vector3(0f, -180f, 0f));
		KidnapperNewBehaviour.Ins.StageInsideJumpscare();
		KidnapperNewBehaviour.Ins.AddOutsideJump();
	}
}
