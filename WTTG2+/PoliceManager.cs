using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PoliceManager : MonoBehaviour
{
	public delegate void WarningActions();

	private static int PoliceDebugFreeze;

	[SerializeField]
	private EnemyHotZoneTrigger[] roomRaidTriggers = new EnemyHotZoneTrigger[0];

	[SerializeField]
	private EnemyHotZoneTrigger[] powerTripTriggers = new EnemyHotZoneTrigger[0];

	[SerializeField]
	private DoorTrigger Floor8DoorTrigger;

	[SerializeField]
	private DoorTrigger MainDoorTrigger;

	[SerializeField]
	private float warningTime = 30f;

	[SerializeField]
	private float networkHotTime = 60f;

	[SerializeField]
	private int SWAT_POOL_COUNT = 4;

	[SerializeField]
	private Transform swatParent;

	[SerializeField]
	private GameObject swatManObject;

	[SerializeField]
	private Vector3[] floor8SpawnPOS = new Vector3[0];

	[SerializeField]
	private Vector3[] floor8SpwanROT = new Vector3[0];

	[SerializeField]
	private Vector3[] stairWayPOS = new Vector3[0];

	[SerializeField]
	private Vector3[] stairWayROT = new Vector3[0];

	[SerializeField]
	private Vector3[] roomRaidPOS = new Vector3[0];

	[SerializeField]
	private Vector3[] roomRaidROT = new Vector3[0];

	[SerializeField]
	private Vector3[] swatEndPOS = new Vector3[0];

	private List<SwatManBehaviour> currentActiveSwatMen = new List<SwatManBehaviour>(4);

	private WifiNetworkDefinition currentActiveWifiNetwork;

	public Dictionary<WifiNetworkDefinition, HotWifiNetwork> hotNetworks = new Dictionary<WifiNetworkDefinition, HotWifiNetwork>(10);

	private Queue<WifiNetworkDefinition> hotNetworksToRemove = new Queue<WifiNetworkDefinition>(10);

	private bool powerWasTripped;

	private PooledStack<SwatManBehaviour> swatManPool;

	private bool triggerActive;

	private float triggerTimeStamp;

	private float triggerTimeWindow;

	private bool warningActive;

	public string PoliceDebug
	{
		get
		{
			if (triggerTimeWindow - (Time.time - triggerTimeStamp) > 0f)
			{
				if (triggerActive)
				{
					PoliceDebugFreeze = (int)(triggerTimeWindow - (Time.time - triggerTimeStamp));
				}
				return PoliceDebugFreeze.ToString();
			}
			return PoliceDebugFreeze.ToString();
		}
	}

	public event WarningActions FireWarning;

	private void Awake()
	{
		EnemyManager.PoliceManager = this;
		if (!DifficultyManager.CasualMode)
		{
			if (!DifficultyManager.LeetMode && !DifficultyManager.Nightmare)
			{
				networkHotTime = 180f;
				Debug.Log("Playing Normal Mode, WiFi De-Track time changed to: " + networkHotTime);
			}
			else if (DifficultyManager.LeetMode && !DifficultyManager.Nightmare)
			{
				networkHotTime = 300f;
				Debug.Log("Playing 1337 Mode, WiFi De-Track time changed to: " + networkHotTime);
			}
			else if (DifficultyManager.Nightmare)
			{
				networkHotTime = 900f;
				Debug.Log("Playing Nightmare Mode, WiFi De-Track time changed to: " + networkHotTime);
			}
		}
		else
		{
			networkHotTime = 90f;
			Debug.Log("Playing Casual Mode, WiFi De-Track time changed to: " + networkHotTime);
		}
		swatManPool = new PooledStack<SwatManBehaviour>(delegate
		{
			SwatManBehaviour component = Object.Instantiate(swatManObject, swatParent).GetComponent<SwatManBehaviour>();
			component.Build();
			return component;
		}, SWAT_POOL_COUNT);
		if (!DifficultyManager.HackerMode)
		{
			GameManager.StageManager.Stage += stageMe;
			GameManager.StageManager.TheGameIsLive += gameLive;
			GameManager.StageManager.ThreatsNowActivated += threatsActivated;
			GameManager.ManagerSlinger.WifiManager.OnlineWithNetwork += userWentOnline;
			GameManager.ManagerSlinger.WifiManager.WentOffline += userWentOffline;
		}
	}

	private void Update()
	{
		if (triggerActive)
		{
			if (warningActive && Time.time - triggerTimeStamp >= triggerTimeWindow - warningTime)
			{
				warningActive = false;
				triggerWarning();
			}
			if (Time.time - triggerTimeStamp >= triggerTimeWindow)
			{
				triggerActive = false;
				attemptAttack();
			}
		}
		foreach (KeyValuePair<WifiNetworkDefinition, HotWifiNetwork> hotNetwork in hotNetworks)
		{
			if (Time.time - hotNetwork.Value.TimeStamp >= hotNetwork.Value.HotTime)
			{
				hotNetworksToRemove.Enqueue(hotNetwork.Key);
			}
		}
		while (hotNetworksToRemove.Count > 0)
		{
			hotNetworks.Remove(hotNetworksToRemove.Dequeue());
		}
	}

	private void OnDestroy()
	{
		Floor8DoorTrigger.DoorOpenEvent.RemoveListener(triggerStairJumpPre);
		Floor8DoorTrigger.DoorWasOpenedEvent.RemoveListener(triggerStairJumpPost);
		Floor8DoorTrigger.DoorOpenEvent.RemoveListener(triggerFloor8PreJump);
		Floor8DoorTrigger.DoorWasOpenedEvent.RemoveListener(triggerFloor8DoorJump);
		Floor8DoorTrigger.DoorOpenEvent.RemoveListener(triggerStairJumpBre);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		for (int i = 0; i < swatEndPOS.Length; i++)
		{
			Gizmos.DrawWireCube(swatEndPOS[i], new Vector3(0.2f, 0.2f, 0.2f));
		}
	}

	private void TriggerRaidRoomRoam()
	{
		bool flag = false;
		if (powerWasTripped)
		{
			EnvironmentManager.PowerBehaviour.ForcePowerOn();
		}
		currentActiveSwatMen[1].TriggerVoiceCommand(SWAT_VOICE_COMMANDS.POLICE_DEPT);
		KeypadManager.EnemyChangedKeypad();
		PoliceRoamJumper.Ins.TriggerRoomRaid(MainDoorTrigger.DoorMeshTransform.position);
		performRoomRaid();
	}

	public void TriggerPowerTrip()
	{
		if (!powerWasTripped)
		{
			powerWasTripped = true;
			EnvironmentManager.PowerBehaviour.ForcePowerOff();
		}
	}

	private void performRoomRaid()
	{
		DataManager.LockSave = true;
		DataManager.ClearGameData();
		currentActiveSwatMen[0].GunLight.enabled = false;
		currentActiveSwatMen[0].TriggerAnim("doorKick");
		if (MainDoorTrigger.SetDistanceLODs != null)
		{
			for (int i = 0; i < MainDoorTrigger.SetDistanceLODs.Length; i++)
			{
				MainDoorTrigger.SetDistanceLODs[i].OverwriteCulling = true;
			}
		}
		MainDoorTrigger.AudioHub.PlaySound(LookUp.SoundLookUp.SlamOpenDoor);
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.JumpHit2);
		MainDoorTrigger.KickDoorOpen();
		GameManager.TimeSlinger.FireTimer(0.2f, delegate
		{
			currentActiveSwatMen[1].FlashBangMesh.enabled = true;
			currentActiveSwatMen[1].TriggerAnim("flashBang");
		});
		GameManager.TimeSlinger.FireTimer(2f, delegate
		{
			currentActiveSwatMen[1].NavMeshAgent.enabled = true;
			currentActiveSwatMen[2].TriggerAnim("walk");
			Sequence sequence = DOTween.Sequence().OnComplete(delegate
			{
				currentActiveSwatMen[2].EndWalkCycle();
			});
			currentActiveSwatMen[2].TriggerVoiceCommand(SWAT_VOICE_COMMANDS.GO_GO, 1.5f);
			sequence.Insert(0f, DOTween.To(() => currentActiveSwatMen[2].transform.position, delegate(Vector3 x)
			{
				currentActiveSwatMen[2].transform.position = x;
			}, new Vector3(-2.249f, 39.599f, -6.46f), 0.5f).SetEase(Ease.Linear));
			sequence.Insert(0f, DOTween.To(() => currentActiveSwatMen[2].transform.rotation, delegate(Quaternion x)
			{
				currentActiveSwatMen[2].transform.rotation = x;
			}, Vector3.zero, 0.5f).SetEase(Ease.Linear).SetOptions());
			sequence.Insert(0.5f, DOTween.To(() => currentActiveSwatMen[2].transform.position, delegate(Vector3 x)
			{
				currentActiveSwatMen[2].transform.position = x;
			}, new Vector3(-2.249f, 39.599f, -3.561f), 1f).SetEase(Ease.Linear));
			sequence.Insert(1f, DOTween.To(() => currentActiveSwatMen[2].transform.rotation, delegate(Quaternion x)
			{
				currentActiveSwatMen[2].transform.rotation = x;
			}, new Vector3(0f, -45f, 0f), 0.5f).SetEase(Ease.Linear).SetOptions());
			sequence.Insert(1.5f, DOTween.To(() => currentActiveSwatMen[2].transform.position, delegate(Vector3 x)
			{
				currentActiveSwatMen[2].transform.position = x;
			}, swatEndPOS[0], 1.5f).SetEase(Ease.Linear));
			sequence.Insert(1.5f, DOTween.To(() => currentActiveSwatMen[2].transform.rotation, delegate(Quaternion x)
			{
				currentActiveSwatMen[2].transform.rotation = x;
			}, new Vector3(0f, -90f, 0f), 0.75f).SetEase(Ease.Linear).SetOptions());
			sequence.Play();
		});
		GameManager.TimeSlinger.FireTimer(3.5f, delegate
		{
			currentActiveSwatMen[3].TriggerAnim("walk");
			Sequence sequence = DOTween.Sequence().OnComplete(delegate
			{
				currentActiveSwatMen[3].EndWalkCycle();
			});
			sequence.Insert(0f, DOTween.To(() => currentActiveSwatMen[3].transform.position, delegate(Vector3 x)
			{
				currentActiveSwatMen[3].transform.position = x;
			}, new Vector3(-2.249f, 39.599f, -6.46f), 0.5f).SetEase(Ease.Linear));
			sequence.Insert(0f, DOTween.To(() => currentActiveSwatMen[3].transform.rotation, delegate(Quaternion x)
			{
				currentActiveSwatMen[3].transform.rotation = x;
			}, Vector3.zero, 0.5f).SetEase(Ease.Linear).SetOptions());
			sequence.Insert(0.5f, DOTween.To(() => currentActiveSwatMen[3].transform.position, delegate(Vector3 x)
			{
				currentActiveSwatMen[3].transform.position = x;
			}, new Vector3(-2.249f, 39.599f, -3.561f), 1f).SetEase(Ease.Linear));
			sequence.Insert(1f, DOTween.To(() => currentActiveSwatMen[3].transform.rotation, delegate(Quaternion x)
			{
				currentActiveSwatMen[3].transform.rotation = x;
			}, new Vector3(0f, 45f, 0f), 0.5f).SetEase(Ease.Linear).SetOptions());
			sequence.Insert(1.5f, DOTween.To(() => currentActiveSwatMen[3].transform.position, delegate(Vector3 x)
			{
				currentActiveSwatMen[3].transform.position = x;
			}, swatEndPOS[1], 2f).SetEase(Ease.Linear));
			sequence.Insert(1.5f, DOTween.To(() => currentActiveSwatMen[3].transform.rotation, delegate(Quaternion x)
			{
				currentActiveSwatMen[3].transform.rotation = x;
			}, new Vector3(0f, 90f, 0f), 0.75f).SetEase(Ease.Linear).SetOptions());
			sequence.Play();
		});
		GameManager.TimeSlinger.FireTimer(5f, delegate
		{
			currentActiveSwatMen[0].TriggerAnim("charge");
			DOTween.To(() => currentActiveSwatMen[0].transform.position, delegate(Vector3 x)
			{
				currentActiveSwatMen[0].transform.position = x;
			}, new Vector3(-2.249f, 39.599f, -6.46f), 0.25f).SetEase(Ease.Linear);
			DOTween.To(() => currentActiveSwatMen[0].transform.rotation, delegate(Quaternion x)
			{
				currentActiveSwatMen[0].transform.rotation = x;
			}, Vector3.zero, 0.25f).SetEase(Ease.Linear).SetOptions()
				.OnComplete(delegate
				{
					currentActiveSwatMen[1].GunLight.enabled = true;
					currentActiveSwatMen[0].ChargePlayer();
				});
		});
		GameManager.TimeSlinger.FireTimer(6f, delegate
		{
			currentActiveSwatMen[1].GunLight.enabled = false;
			currentActiveSwatMen[1].NavMeshAgent.enabled = false;
			currentActiveSwatMen[1].TriggerAnim("walk");
			Sequence s = DOTween.Sequence().OnComplete(delegate
			{
				currentActiveSwatMen[1].EndWalkCycle();
			});
			s.Insert(0f, DOTween.To(() => currentActiveSwatMen[1].transform.position, delegate(Vector3 x)
			{
				currentActiveSwatMen[1].transform.position = x;
			}, new Vector3(-2.249f, 39.599f, -6.46f), 0.5f).SetEase(Ease.Linear));
			s.Insert(0f, DOTween.To(() => currentActiveSwatMen[1].transform.rotation, delegate(Quaternion x)
			{
				currentActiveSwatMen[1].transform.rotation = x;
			}, Vector3.zero, 0.5f).SetEase(Ease.Linear).SetOptions());
			s.Insert(0.5f, DOTween.To(() => currentActiveSwatMen[1].transform.position, delegate(Vector3 x)
			{
				currentActiveSwatMen[1].transform.position = x;
			}, new Vector3(-2.249f, 39.599f, -3.561f), 1f).SetEase(Ease.Linear));
			s.Insert(1f, DOTween.To(() => currentActiveSwatMen[1].transform.rotation, delegate(Quaternion x)
			{
				currentActiveSwatMen[1].transform.rotation = x;
			}, new Vector3(0f, 90f, 0f), 0.5f).SetEase(Ease.Linear).SetOptions());
		});
		currentActiveSwatMen[2].TriggerVoiceCommand(SWAT_VOICE_COMMANDS.CLEAR, 11f);
		GameManager.TimeSlinger.FireTimer(12.5f, delegate
		{
			MainCameraHook.Ins.ClearARF();
			UIManager.TriggerGameOver("ARRESTED");
		});
	}

	private void triggerFloor8PreJump()
	{
		currentActiveSwatMen[0].SpawnMe(new Vector3(24.173f, currentActiveSwatMen[0].transform.position.y, currentActiveSwatMen[0].transform.position.z), currentActiveSwatMen[0].transform.localRotation.eulerAngles);
		GameManager.TimeSlinger.FireTimer(0.45f, delegate
		{
			GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.JumpHit2);
			roamController.Ins.KillOutOfWay();
			currentActiveSwatMen[0].TakeOverCamera();
			PoliceRoamJumper.Ins.TriggerFloor8DoorJump();
			currentActiveSwatMen[0].TriggerAnim("hallTackle");
		});
	}

	private void triggerFloor8DoorJump()
	{
		DataManager.LockSave = true;
		DataManager.ClearGameData();
		Floor8DoorTrigger.CancelAutoClose();
		currentActiveSwatMen[2].TriggerVoiceCommand(SWAT_VOICE_COMMANDS.POLICE_DEPT);
		currentActiveSwatMen[0].TriggerVoiceCommand(SWAT_VOICE_COMMANDS.GOT_YOU, 1.5f);
		GameManager.TimeSlinger.FireTimer(3.3f, delegate
		{
			UIManager.TriggerGameOver("ARRESTED");
		});
	}

	private void triggerStairJumpPre()
	{
		DataManager.LockSave = true;
		DataManager.ClearGameData();
		if (powerWasTripped)
		{
			EnvironmentManager.PowerBehaviour.ForcePowerOn();
		}
		GameManager.AudioSlinger.PlaySoundWithCustomDelay(LookUp.SoundLookUp.HeadHit, 0.5f);
		PoliceRoamJumper.Ins.TriggerStairWayJump();
		currentActiveSwatMen[1].TriggerVoiceCommand(SWAT_VOICE_COMMANDS.POLICE_DEPT, 0.35f);
		currentActiveSwatMen[0].TriggerVoiceCommand(SWAT_VOICE_COMMANDS.GOT_YOU, 3f);
		GameManager.TimeSlinger.FireTimer(0.25f, delegate
		{
			GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.JumpHit2);
			currentActiveSwatMen[0].TriggerAnim("headSmash");
			currentActiveSwatMen[0].TriggerFootSteps(1.3f, 4, 0.4f);
		});
		GameManager.TimeSlinger.FireTimer(5f, delegate
		{
			UIManager.TriggerGameOver("ARRESTED");
		});
	}

	private void triggerStairJumpBre()
	{
		PoliceRoamJumper.Ins.TriggerStairWayJump();
		BreatherFloor8Trigger.me.ActivateJumpscare();
		GameManager.AudioSlinger.PlaySoundWithCustomDelay(LookUp.SoundLookUp.JumpHit7, 0.25f);
		GameManager.AudioSlinger.PlaySoundWithCustomDelay(CustomSoundLookUp.wttg1_laugh, 0.7f);
		GameManager.TimeSlinger.FireTimer(4.3f, BreatherBehaviour.Ins.KnifeStab);
		GameManager.TimeSlinger.FireTimer(4.5f, delegate
		{
			MainCameraHook.Ins.ClearARF();
			UIManager.TriggerGameOver("YOU WERE KILLED");
		});
	}

	private void triggerStairJumpPost()
	{
		Floor8DoorTrigger.CancelAutoClose();
	}

	private void userWentOnline(WifiNetworkDefinition TheNetwork)
	{
		currentActiveWifiNetwork = TheNetwork;
		triggerTimeWindow = TheNetwork.networkTrackRate;
		if (DifficultyManager.LeetMode)
		{
			triggerTimeWindow *= 0.7f;
		}
		else if (DifficultyManager.Nightmare && triggerTimeWindow > 333f)
		{
			triggerTimeWindow = 333f;
		}
		else if (RouterBehaviour.Ins.Owned && RouterBehaviour.Ins.RouterIsActive && !RouterBehaviour.Ins.IsJammed && triggerTimeWindow > 120f)
		{
			triggerTimeWindow = TheNetwork.networkTrackRate;
			switch (RouterBehaviour.Ins.routerHubSwitch)
			{
			case 1:
				triggerTimeWindow = (DifficultyManager.Nightmare ? 666f : (triggerTimeWindow * 2f));
				break;
			case 2:
				triggerTimeWindow = (DifficultyManager.Nightmare ? 500f : (triggerTimeWindow * 1.5f));
				break;
			case 3:
				triggerTimeWindow = (DifficultyManager.Nightmare ? 333f : (triggerTimeWindow * 1f));
				break;
			case 4:
				triggerTimeWindow = (DifficultyManager.Nightmare ? 166f : (triggerTimeWindow * 0.5f));
				break;
			default:
				triggerTimeWindow = (DifficultyManager.Nightmare ? 166f : (triggerTimeWindow * 0.5f));
				break;
			}
		}
		if (hotNetworks.ContainsKey(TheNetwork))
		{
			float num = triggerTimeWindow;
			triggerTimeWindow = hotNetworks[TheNetwork].TimeLeft;
			if (triggerTimeWindow > num)
			{
				Debug.LogFormat("[HotWifiNetwork] A hot Wifi network was above the max calculated track range, Red {0}, expected less than {1}", triggerTimeWindow, num);
				triggerTimeWindow = num;
			}
			triggerTimeStamp = Time.time;
			triggerActive = true;
			warningActive = true;
			hotNetworks.Remove(TheNetwork);
		}
		else
		{
			triggerTimeStamp = Time.time;
			triggerActive = true;
			warningActive = true;
		}
	}

	private void userWentOffline()
	{
		triggerActive = false;
		warningActive = false;
		float setTimeLeft = triggerTimeWindow - (Time.time - triggerTimeStamp);
		if (currentActiveWifiNetwork != null)
		{
			if (DifficultyManager.Nightmare)
			{
				networkHotTime = ((RouterBehaviour.Ins.Owned && RouterBehaviour.Ins.RouterIsActive && !RouterBehaviour.Ins.IsJammed) ? 450f : 900f);
			}
			HotWifiNetwork value = new HotWifiNetwork(currentActiveWifiNetwork.GetHashCode(), networkHotTime, Time.time, setTimeLeft);
			hotNetworks.Remove(currentActiveWifiNetwork);
			hotNetworks.Add(currentActiveWifiNetwork, value);
		}
	}

	private void attemptAttack()
	{
		int num = Random.Range(0, 101);
		int num2 = Mathf.RoundToInt(currentActiveWifiNetwork.networkTrackProbability * 100f);
		float num3 = 0f;
		if (RouterBehaviour.Ins.Owned && RouterBehaviour.Ins.RouterIsActive && !RouterBehaviour.Ins.IsJammed)
		{
			num3 = RouterBehaviour.Ins.routerHubSwitch switch
			{
				1 => 40f, 
				2 => 30f, 
				3 => 20f, 
				4 => 10f, 
				_ => 4f, 
			};
		}
		if (RouterBehaviour.Ins.Owned && RouterBehaviour.Ins.RouterIsActive && !RouterBehaviour.Ins.IsJammed && DifficultyManager.CasualMode && currentActiveWifiNetwork.networkTrackProbability * 100f < num3)
		{
			triggerTimeWindow = currentActiveWifiNetwork.networkTrackRate / 2f;
			triggerTimeStamp = Time.time;
			triggerActive = true;
			warningActive = true;
		}
		else if (num < num2)
		{
			triggerAttack();
		}
		else if (DifficultyManager.Nightmare)
		{
			triggerAttack();
		}
		else
		{
			triggerTimeWindow = currentActiveWifiNetwork.networkTrackRate / 2f;
			triggerTimeStamp = Time.time;
			triggerActive = true;
			warningActive = true;
		}
	}

	public void triggerAttack()
	{
		if (!EnemyStateManager.IsInEnemyStateOrLocked() && EnvironmentManager.PowerState == POWER_STATE.ON)
		{
			processAttack();
			return;
		}
		triggerTimeWindow = currentActiveWifiNetwork.networkTrackRate / 2f;
		triggerTimeStamp = Time.time;
		triggerActive = true;
		warningActive = true;
	}

	private void processAttack()
	{
		if (!StateManager.BeingHacked && StateManager.PlayerState != PLAYER_STATE.PEEPING)
		{
			if (StateManager.PlayerState != PLAYER_STATE.BUSY)
			{
				if (StateManager.PlayerLocation != PLAYER_LOCATION.UNKNOWN)
				{
					if (StateManager.PlayerLocation == PLAYER_LOCATION.MAIN_ROON || StateManager.PlayerLocation == PLAYER_LOCATION.OUTSIDE || StateManager.PlayerLocation == PLAYER_LOCATION.BATH_ROOM)
					{
						if (StateManager.PlayerState == PLAYER_STATE.COMPUTER || StateManager.PlayerState == PLAYER_STATE.DESK)
						{
							int num = Random.Range(0, 100);
							if (num < (DifficultyManager.LeetMode ? 50 : 25) || DifficultyManager.Nightmare)
							{
								powerWasTripped = true;
								EnvironmentManager.PowerBehaviour.ForcePowerOff();
							}
						}
						spawnForRaid();
						for (int i = 0; i < roomRaidTriggers.Length; i++)
						{
							roomRaidTriggers[i].SetActive();
						}
						EnemyStateManager.AddEnemyState(ENEMY_STATE.POILCE);
						PowerStateManager.AddPowerStateLock(STATE_LOCK_OCCASION.POILCE);
						DataManager.LockSave = true;
					}
					else if (DifficultyManager.CasualMode)
					{
						triggerTimeWindow = 20f;
						triggerTimeStamp = Time.time;
						triggerActive = true;
					}
					else if (StateManager.PlayerLocation == PLAYER_LOCATION.HALL_WAY8)
					{
						for (int j = 0; j < powerTripTriggers.Length; j++)
						{
							powerTripTriggers[j].SetActive();
						}
						Floor8DoorTrigger.DoorOpenEvent.AddListener(triggerStairJumpPre);
						Floor8DoorTrigger.DoorWasOpenedEvent.AddListener(triggerStairJumpPost);
						spawnInStairWay();
						EnemyStateManager.AddEnemyState(ENEMY_STATE.POILCE);
						PowerStateManager.AddPowerStateLock(STATE_LOCK_OCCASION.POILCE);
						DataManager.LockSave = true;
					}
					else
					{
						Floor8DoorTrigger.DoorOpenEvent.AddListener(triggerFloor8PreJump);
						Floor8DoorTrigger.DoorWasOpenedEvent.AddListener(triggerFloor8DoorJump);
						Floor8DoorTrigger.SetCustomOpenDoorTime(0.45f);
						spawnSwatInHallWay8();
						EnemyStateManager.AddEnemyState(ENEMY_STATE.POILCE);
						PowerStateManager.AddPowerStateLock(STATE_LOCK_OCCASION.POILCE);
						DataManager.LockSave = true;
					}
				}
				else
				{
					triggerTimeWindow = 10f;
					triggerTimeStamp = Time.time;
					triggerActive = true;
				}
			}
			else
			{
				triggerTimeWindow = 5f;
				triggerTimeStamp = Time.time;
				triggerActive = true;
			}
		}
		else
		{
			triggerTimeWindow = 30f;
			triggerTimeStamp = Time.time;
			triggerActive = true;
		}
	}

	private void triggerWarning()
	{
		if (this.FireWarning != null)
		{
			this.FireWarning();
		}
	}

	private void spawnSwatInHallWay8()
	{
		for (int i = 0; i < floor8SpawnPOS.Length; i++)
		{
			SwatManBehaviour swatManBehaviour = swatManPool.Pop();
			if (i == 0)
			{
				swatManBehaviour.TriggerAnim("hallTackleIdle");
			}
			swatManBehaviour.SpawnMe(floor8SpawnPOS[i], floor8SpwanROT[i]);
			currentActiveSwatMen.Add(swatManBehaviour);
		}
		currentActiveSwatMen[2].TriggerAnim("standIdle2");
		currentActiveSwatMen[1].TriggerAnim("crouchIdle2");
	}

	private void spawnInStairWay()
	{
		for (int i = 0; i < stairWayPOS.Length; i++)
		{
			SwatManBehaviour swatManBehaviour = swatManPool.Pop();
			swatManBehaviour.SpawnMe(stairWayPOS[i], stairWayROT[i]);
			currentActiveSwatMen.Add(swatManBehaviour);
		}
		currentActiveSwatMen[0].TriggerAnim("headSmashIdle");
		currentActiveSwatMen[1].TriggerAnim("crouchIdle1");
		currentActiveSwatMen[3].TriggerAnim("crouchIdle2");
	}

	private void spawnForRaid()
	{
		for (int i = 0; i < roomRaidPOS.Length; i++)
		{
			SwatManBehaviour swatManBehaviour = swatManPool.Pop();
			swatManBehaviour.SpawnMe(roomRaidPOS[i], roomRaidROT[i]);
			currentActiveSwatMen.Add(swatManBehaviour);
		}
		currentActiveSwatMen[0].TriggerAnim("doorKickIdle");
		currentActiveSwatMen[1].TriggerAnim("flashBangIdle");
		CamHookBehaviour.Interruptions = true;
		CamHookBehaviour.SwitchCameraStatus(enabled: false);
		GameManager.TimeSlinger.FireTimer(3f, delegate
		{
			CamHookBehaviour.Interruptions = false;
			CamHookBehaviour.SwitchCameraStatus(enabled: true);
		});
		doorlogBehaviour.MayAddDoorlog("Floor 8 Hallway", mode: true);
	}

	private void stageMe()
	{
		PoliceDebugFreeze = 0;
		GameManager.StageManager.Stage -= stageMe;
	}

	private void gameLive()
	{
		GameManager.StageManager.TheGameIsLive -= gameLive;
	}

	private void threatsActivated()
	{
		GameManager.StageManager.ThreatsNowActivated -= threatsActivated;
	}

	public void triggerDevSwat()
	{
		if (!EnemyStateManager.IsInEnemyStateOrLocked() && EnvironmentManager.PowerState == POWER_STATE.ON)
		{
			spawnForRaid();
			for (int i = 0; i < roomRaidTriggers.Length; i++)
			{
				roomRaidTriggers[i].SetActive();
			}
		}
	}

	public void TrollPoliceScanner()
	{
		if (this.FireWarning != null)
		{
			this.FireWarning();
		}
	}

	public void NightmarePolice()
	{
		Floor8DoorTrigger.DoorOpenEvent.AddListener(triggerFloor8PreJump);
		Floor8DoorTrigger.DoorWasOpenedEvent.AddListener(triggerFloor8DoorJump);
		Floor8DoorTrigger.SetCustomOpenDoorTime(0.45f);
		spawnSwatInHallWay8();
	}

	public void StageBreatherFloor8()
	{
		Floor8DoorTrigger.DoorOpenEvent.AddListener(triggerStairJumpBre);
		Floor8DoorTrigger.DoorWasOpenedEvent.AddListener(triggerStairJumpPost);
		GameManager.TimeSlinger.FireTimer(120f, delegate
		{
			if (EnvironmentManager.PowerState == POWER_STATE.ON)
			{
				EnvironmentManager.PowerBehaviour.ForcePowerOff();
			}
		});
	}
}
