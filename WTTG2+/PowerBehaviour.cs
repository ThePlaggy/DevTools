using DG.Tweening;
using UnityEngine;

public class PowerBehaviour : MonoBehaviour
{
	private static Color[] hardColors;

	private static Color[] lightColors;

	private static bool AlreadySetDisco;

	public bool LockedOut;

	[SerializeField]
	private float fireWindowMin = 60f;

	[SerializeField]
	private float fireWindowMax = 120f;

	[SerializeField]
	private InteractiveLight[] Lights;

	[SerializeField]
	private AudioHubObject powerOutageHub;

	[SerializeField]
	private AudioHubObject breakerBoxHub;

	[SerializeField]
	private AudioFileDefinition powerOutSFX;

	[SerializeField]
	private AudioFileDefinition powerOnSFX;

	[SerializeField]
	private AudioFileDefinition breakerSwitchOffSFX;

	[SerializeField]
	private AudioFileDefinition breakerSwitchOnSFX;

	[SerializeField]
	private Material computerMaterial;

	[SerializeField]
	private Material[] HardEmissiveLights;

	[SerializeField]
	private InteractionHook[] TriggerColliders;

	[SerializeField]
	private MeshRenderer ComputerScreen;

	[SerializeField]
	private Transform BreakerSwitchTransform;

	[SerializeField]
	private Vector3 SwitchOnPOS;

	[SerializeField]
	private Vector3 SwitchOffPOS;

	private float fireWindow;

	private bool fireWindowActive;

	private float fireWindowTimeStamp;

	private PowerBehaviourData myData;

	private int myID;

	public CustomEvent PowerOffEvent = new CustomEvent(2);

	public CustomEvent PowerOnEvent = new CustomEvent(2);

	private Tweener switchOffTween;

	private Tweener switchOnTween;

	private bool kidnapperRolled;

	private bool FemaleNoir;

	public AudioHubObject PowerOutageHub => powerOutageHub;

	public string PowerDebug
	{
		get
		{
			if (fireWindow - (Time.time - fireWindowTimeStamp) > 0f)
			{
				return ((int)(fireWindow - (Time.time - fireWindowTimeStamp))).ToString();
			}
			return 0.ToString();
		}
	}

	private void Awake()
	{
		myID = base.transform.position.GetHashCode();
		EnvironmentManager.PowerBehaviour = this;
		switchOnTween = DOTween.To(() => SwitchOffPOS, delegate(Vector3 x)
		{
			BreakerSwitchTransform.transform.localPosition = x;
		}, SwitchOnPOS, 0.3f).SetEase(Ease.InCirc);
		switchOnTween.Pause();
		switchOnTween.SetAutoKill(autoKillOnCompletion: false);
		switchOffTween = DOTween.To(() => SwitchOnPOS, delegate(Vector3 x)
		{
			BreakerSwitchTransform.transform.localPosition = x;
		}, SwitchOffPOS, 0.15f).SetEase(Ease.Linear);
		switchOffTween.Pause();
		switchOffTween.SetAutoKill(autoKillOnCompletion: false);
		GameManager.StageManager.Stage += stageMe;
		GameManager.StageManager.TheGameIsLive += gameLive;
		GameManager.StageManager.ThreatsNowActivated += threatsActivated;
	}

	private void Update()
	{
		if (!kidnapperRolled && fireWindowActive && Time.time - fireWindowTimeStamp >= fireWindow - 30f)
		{
			if (EnemyManager.CultManager.keyDiscoveryCount < 3)
			{
				Debug.Log("PowerBehaviour: Kidnapper, not enough keys to do this event");
				kidnapperRolled = true;
				return;
			}
			if (TarotVengeance.Killed(ENEMY_STATE.KIDNAPPER))
			{
				Debug.Log("PowerBehaviour: Kidnapper got tarotted' lol");
				kidnapperRolled = true;
				return;
			}
			if (StateManager.PlayerState != PLAYER_STATE.COMPUTER || !ComputerPowerHook.Ins.FullyPoweredOn)
			{
				Debug.Log("PowerBehaviour: Not in computer mode, No kidnapper");
				kidnapperRolled = true;
				return;
			}
			if (EnvironmentManager.PowerState == POWER_STATE.OFF)
			{
				Debug.Log("PowerBehaviour: wtf state is already off, No kidnapper");
				kidnapperRolled = true;
				return;
			}
			if (EnemyStateManager.IsInEnemyStateOrLocked())
			{
				Debug.Log("PowerBehaviour: Enemy state or locked, No kidnapper");
				kidnapperRolled = true;
				return;
			}
			if (StateManager.PlayerLocation != PLAYER_LOCATION.MAIN_ROON && StateManager.PlayerLocation != PLAYER_LOCATION.BATH_ROOM && StateManager.PlayerLocation != PLAYER_LOCATION.OUTSIDE)
			{
				Debug.Log("PowerBehaviour: Player not in apartment, No kidnapper");
				kidnapperRolled = true;
				return;
			}
			if (LookUp.Doors.MainDoor.IsOpen)
			{
				Debug.Log("PowerBehaviour: Main door opened, No kidnapper");
				kidnapperRolled = true;
				return;
			}
			if (DifficultyManager.CasualMode || EventSlinger.HalloweenEvent)
			{
				Debug.Log("PowerBehaviour: Kidnapper is not in casual");
				kidnapperRolled = true;
				return;
			}
			Debug.Log("PowerBehaviour: Time for kidnapper?");
			kidnapperRolled = true;
			if (Random.Range(0, 100) <= EnemyManager.CultManager.keyDiscoveryCount * 5 + 30)
			{
				Debug.Log("PowerBehaviour: Yes");
				EnemyManager.KidnapperManager.AddKidnapperPowerEvent();
			}
		}
		if (fireWindowActive && Time.time - fireWindowTimeStamp >= fireWindow)
		{
			fireWindowActive = false;
			if (!PowerStateManager.IsLocked() && !StateManager.BeingHacked && !KAttack.IsInAttack && EnvironmentManager.PowerState == POWER_STATE.ON)
			{
				powerOff();
				return;
			}
			fireWindow = 30f;
			fireWindowTimeStamp = Time.time;
			fireWindowActive = true;
		}
	}

	public void SwitchPowerOn()
	{
		generateFireWindow();
		breakerBoxHub.PlaySound(breakerSwitchOnSFX);
		powerOutageHub.PlaySound(powerOnSFX);
		switchOnTween.Restart();
		PowerOnEvent.Execute();
		powerOn();
	}

	public void PowerOnNewNoir()
	{
		generateFireWindow();
		switchOnTween.Restart();
		PowerOnEvent.Execute();
		powerOn();
	}

	public void ForcePowerOff()
	{
		powerOff();
	}

	public void ForcePowerOn()
	{
		powerOn();
	}

	public void ResetPowerTripTime()
	{
		generateFireWindow();
	}

	private void powerOff(bool PlaySound = true)
	{
		if (PlaySound)
		{
			powerOutageHub.PlaySound(powerOutSFX);
			breakerBoxHub.PlaySound(breakerSwitchOffSFX);
		}
		TarotManager.HermitActive = false;
		BotnetBehaviour.SetHermit();
		switchToComputerController.Ins.Lock();
		EnvironmentManager.PowerState = POWER_STATE.OFF;
		switchOffTween.Restart();
		PowerOffEvent.Execute();
		if (StateManager.PlayerState == PLAYER_STATE.COMPUTER)
		{
			ControllerManager.Get<computerController>(GAME_CONTROLLER.COMPUTER).LeaveMe();
		}
		for (int i = 0; i < Lights.Length; i++)
		{
			Lights[i].ForceOff();
		}
		for (int j = 0; j < HardEmissiveLights.Length; j++)
		{
			HardEmissiveLights[j].DisableKeyword("_EMISSION");
		}
		for (int k = 0; k < TriggerColliders.Length; k++)
		{
			TriggerColliders[k].ForceLock = true;
		}
		computerMaterial.DisableKeyword("_EMISSION");
		ComputerScreen.enabled = false;
		myData.LightsAreOff = true;
		DataManager.Save(myData);
		if (KidnapperManager.KidnapperTime)
		{
			KidnapperManager.KidnapperTime = false;
			TrackerManager.ClearTrackState();
			EnemyManager.KidnapperManager.ExecutePowerOffEvent();
			return;
		}
		FemaleNoir = Random.Range(0, 100) < FemaleNoirBehavior.GetPowerOffChances() && !EnemyStateManager.IsInEnemyStateOrLocked();
		if (FemaleNoir)
		{
			FemaleNoirBehavior.Ins.Stage8thFloor();
		}
	}

	private void powerOn()
	{
		EnvironmentManager.PowerState = POWER_STATE.ON;
		kidnapperRolled = false;
		Debug.Log("Kidnapper rolled = false;");
		if (FemaleNoir)
		{
			FemaleNoir = false;
			FemaleNoirBehavior.Ins.Spawn8thFloor();
		}
		for (int i = 0; i < Lights.Length; i++)
		{
			Lights[i].ReturnFromForceOff();
		}
		for (int j = 0; j < HardEmissiveLights.Length; j++)
		{
			HardEmissiveLights[j].EnableKeyword("_EMISSION");
		}
		for (int k = 0; k < TriggerColliders.Length; k++)
		{
			TriggerColliders[k].ForceLock = false;
		}
		if (ComputerPowerHook.Ins.PowerOn)
		{
			computerMaterial.EnableKeyword("_EMISSION");
			ComputerScreen.enabled = true;
			switchToComputerController.Ins.UnLock();
		}
		myData.LightsAreOff = false;
		DataManager.Save(myData);
	}

	private void generateFireWindow()
	{
		fireWindow = Random.Range(fireWindowMin, fireWindowMax);
		if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			fireWindow *= 0.4f;
		}
		fireWindowTimeStamp = Time.time;
		fireWindowActive = true;
	}

	private void stageMe()
	{
		myData = DataManager.Load<PowerBehaviourData>(myID);
		if (myData == null)
		{
			myData = new PowerBehaviourData(myID);
			myData.LightsAreOff = false;
		}
		GameManager.StageManager.Stage -= stageMe;
	}

	private void gameLive()
	{
		if (myData.LightsAreOff)
		{
			powerOff(PlaySound: false);
		}
		else
		{
			for (int i = 0; i < HardEmissiveLights.Length; i++)
			{
				HardEmissiveLights[i].EnableKeyword("_EMISSION");
			}
		}
		if (!AlreadySetDisco)
		{
			setLightDefaults();
		}
		else
		{
			RevertDiscoLightsOnAwake();
		}
		if (EventSlinger.HalloweenEvent)
		{
			EnvironmentManager.PowerBehaviour.DOSetNewLightColor(new Color(1f, 0.4348f, 0.0047f), 1f);
		}
		if (EventSlinger.AprilFoolsEvent)
		{
			EnvironmentManager.PowerBehaviour.DOSetNewLightColor(Rickroller.VACation ? new Color(0.2471f, 0.72551f, 1f) : new Color(0.6706f, 0.6431f, 0.8157f), 0.1f);
		}
		GameManager.StageManager.TheGameIsLive -= gameLive;
	}

	private void threatsActivated()
	{
		GameManager.StageManager.ThreatsNowActivated -= threatsActivated;
		generateFireWindow();
	}

	public void ForceTwitchPowerOff()
	{
		if (!PowerStateManager.IsLocked() && !StateManager.BeingHacked && !KAttack.IsInAttack && EnvironmentManager.PowerState == POWER_STATE.ON)
		{
			powerOff();
			ResetPowerTripTime();
		}
		else
		{
			GameManager.TimeSlinger.FireTimer(30f, ForceTwitchPowerOff);
		}
	}

	private void setLightDefaults()
	{
		hardColors = new Color[HardEmissiveLights.Length];
		lightColors = new Color[Lights.Length];
		for (int i = 0; i < hardColors.Length; i++)
		{
			hardColors[i] = HardEmissiveLights[i].color;
		}
		for (int j = 0; j < lightColors.Length; j++)
		{
			lightColors[j] = Lights[j].MyLight.color;
		}
		AlreadySetDisco = true;
	}

	public void DOSetNewLightColor(Color color, float delay)
	{
		if (!TheSwanBehaviour.SwanInControlOfDisco)
		{
			for (int i = 0; i < HardEmissiveLights.Length; i++)
			{
				HardEmissiveLights[i].DOColor(color, delay);
			}
			for (int j = 0; j < Lights.Length; j++)
			{
				Lights[j].MyLight.DOColor(color, delay);
			}
		}
	}

	public void SwanDisco(Color color)
	{
		if (!EventSlinger.HalloweenEvent && !EasterEggManager.EventCompleted)
		{
			for (int i = 0; i < HardEmissiveLights.Length; i++)
			{
				HardEmissiveLights[i].DOColor(color, 1f);
			}
			for (int j = 0; j < Lights.Length; j++)
			{
				Lights[j].MyLight.DOColor(color, 1f);
			}
		}
	}

	public void ResetDefaultLights()
	{
		for (int i = 0; i < HardEmissiveLights.Length; i++)
		{
			HardEmissiveLights[i].DOColor(hardColors[i], 1f);
		}
		for (int j = 0; j < Lights.Length; j++)
		{
			Lights[j].MyLight.DOColor(lightColors[j], 1f);
		}
	}

	private void RevertDiscoLightsOnAwake()
	{
		for (int i = 0; i < HardEmissiveLights.Length; i++)
		{
			HardEmissiveLights[i].color = hardColors[i];
		}
		for (int j = 0; j < Lights.Length; j++)
		{
			Lights[j].MyLight.color = lightColors[j];
		}
	}

	public void DevEnemyTimerChange(float time)
	{
		if (fireWindowActive)
		{
			fireWindow = time;
			fireWindowTimeStamp = Time.time;
		}
	}
}
