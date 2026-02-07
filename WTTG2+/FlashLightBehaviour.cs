using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Light))]
[RequireComponent(typeof(AudioHubObject))]
public class FlashLightBehaviour : MonoBehaviour
{
	public static FlashLightBehaviour Ins;

	[SerializeField]
	private float defaultFlashLightBrightness;

	[SerializeField]
	private float flashLightLifeInMins = 1f;

	[SerializeField]
	private AudioFileDefinition flashLightOnSFX;

	[SerializeField]
	private AudioFileDefinition flashLightOffSFX;

	private float batteryLifeUsage;

	private float flashCheckTimeStamp;

	private Light flashLight;

	private bool flashlightHuntMode;

	private bool flashLightIsDead;

	private bool flashLightIsOn;

	public CustomEvent<bool> FlashLightWentOn = new CustomEvent<bool>();

	private bool lockedOut;

	private float maxBatteryLife;

	private AudioHubObject myAudioHub;

	private FlashLightBehData myFlashLightData;

	private bool test;

	public bool LightOn => flashLightIsOn && !flashLightIsDead;

	public Light getFlashlight => flashLight;

	private void Awake()
	{
		Ins = this;
		flashLight = GetComponent<Light>();
		myAudioHub = GetComponent<AudioHubObject>();
		GameManager.StageManager.Stage += stageMe;
		maxBatteryLife = flashLightLifeInMins * 60f;
		flashCheckTimeStamp = Time.time;
		flashLight.intensity = defaultFlashLightBrightness;
	}

	private void Update()
	{
		if (TimeKeeper.NightmareEndingTriggered && !test)
		{
			test = true;
			flashLight.enabled = true;
			flashLight.intensity = 1.2f;
			DiscoRainbowMode(Color.red, 0.5f);
			LockOut();
		}
		if (!test && !lockedOut && CrossPlatformInputManager.GetButtonDown("FlashLight"))
		{
			Ins.TriggerFlashLight();
		}
		if (!test && flashLightIsOn && !flashlightHuntMode)
		{
			HuntFlashLight(EnemyStateManager.HasEnemyState(ENEMY_STATE.CULT) || (EnemyStateManager.HasEnemyState(ENEMY_STATE.NEWNOIR) && (StateManager.PlayerLocation == PLAYER_LOCATION.DEAD_DROP || StateManager.PlayerLocation == PLAYER_LOCATION.DEAD_DROP_ROOM)));
		}
	}

	private void OnDestroy()
	{
		test = false;
		CancelInvoke("saveData");
	}

	public void TriggerFlashLight()
	{
		if (InventoryManager.OwnsFlashlight)
		{
			flashLightIsOn = !flashLightIsOn;
			flashLight.enabled = flashLightIsOn;
			if (flashLightIsOn)
			{
				myAudioHub.PlaySound(flashLightOnSFX);
			}
			else
			{
				myAudioHub.PlaySound(flashLightOffSFX);
			}
			if (flashLightIsOn && !flashLightIsDead)
			{
				FlashLightWentOn.Execute(value: true);
			}
			else
			{
				FlashLightWentOn.Execute(value: false);
			}
		}
	}

	public void LockOut()
	{
		lockedOut = true;
		if (!test)
		{
			flashLightIsOn = false;
			flashLight.enabled = false;
		}
	}

	public void UnLock()
	{
		lockedOut = false;
	}

	private void saveData()
	{
		DataManager.Save(myFlashLightData);
	}

	private void stageMe()
	{
		GameManager.StageManager.Stage -= stageMe;
		myFlashLightData = DataManager.Load<FlashLightBehData>(78);
		if (myFlashLightData == null)
		{
			myFlashLightData = new FlashLightBehData(78);
			myFlashLightData.BatteryLifeUsage = 1f;
		}
		batteryLifeUsage = myFlashLightData.BatteryLifeUsage;
		InvokeRepeating("saveData", 0f, 30f);
	}

	private void HuntFlashLight(bool enabled)
	{
		if (!test)
		{
			if (!enabled)
			{
				flashLight.enabled = flashLightIsOn;
				flashlightHuntMode = false;
			}
			else
			{
				flashlightHuntMode = true;
				GameManager.TimeSlinger.FireTimer(Random.Range(0.25f, 0.75f), ToggleHuntFlashLight);
			}
		}
	}

	private void ToggleHuntFlashLight()
	{
		if (!test)
		{
			if (flashLightIsOn)
			{
				flashLight.enabled = !flashLight.enabled;
			}
			if (EnemyStateManager.HasEnemyState(ENEMY_STATE.CULT) || (EnemyStateManager.HasEnemyState(ENEMY_STATE.NEWNOIR) && (StateManager.PlayerLocation == PLAYER_LOCATION.DEAD_DROP || StateManager.PlayerLocation == PLAYER_LOCATION.DEAD_DROP_ROOM)))
			{
				flashlightHuntMode = true;
			}
			else
			{
				flashLight.enabled = flashLightIsOn;
				flashlightHuntMode = false;
			}
			if (flashlightHuntMode)
			{
				GameManager.TimeSlinger.FireTimer(Random.Range(0.2f, 0.6f), ToggleHuntFlashLight);
			}
		}
	}

	private void DiscoRainbowMode(Color beginningColor, float delay)
	{
		if (delay < 0.1f)
		{
			delay = 0.1f;
		}
		flashLight.color = beginningColor;
		if (beginningColor == Color.red)
		{
			beginningColor = Color.yellow;
		}
		else if (beginningColor == Color.yellow)
		{
			beginningColor = Color.green;
		}
		else if (beginningColor == Color.green)
		{
			beginningColor = Color.blue;
		}
		else if (beginningColor == Color.blue)
		{
			beginningColor = Color.magenta;
		}
		else if (beginningColor == Color.magenta)
		{
			beginningColor = Color.red;
		}
		GameManager.TimeSlinger.FireTimer(delay, DiscoRainbowMode, beginningColor, delay);
	}
}
