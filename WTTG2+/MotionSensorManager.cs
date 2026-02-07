using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MotionSensorManager : MonoBehaviour
{
	public delegate void MotionSensorVoidActions(MotionSensorObject TheMotionSensor);

	[SerializeField]
	private int MOTION_SENSOR_POOL_COUNT = 5;

	[SerializeField]
	private GameObject motionSensorObject;

	[SerializeField]
	private Transform motionSensorParent;

	[SerializeField]
	private Vector3[] motionSensorSpawnLocations = new Vector3[0];

	[SerializeField]
	private Vector3[] motionSensorSpawnRotations = new Vector3[0];

	[SerializeField]
	private float triggerWarningFadeTime = 0.1f;

	[SerializeField]
	private AudioFileDefinition motionSensorAlertSFX;

	private MotionSensorObject currentActiveMotionSensor;

	private Stack<Vector3> currentAvaibleMotionSensorSpawnLocations = new Stack<Vector3>();

	private Stack<Vector3> currentAvaibleMotionSensorSpawnRotations = new Stack<Vector3>();

	private List<MotionSensorObject> currentlyOwnedMotionSensors = new List<MotionSensorObject>(5);

	private bool motionSensorMenuActive;

	private bool motionSensorMenuAniActive;

	private PooledStack<MotionSensorObject> motionSensorPool;

	private MotionSensorManagerData myData;

	private int myID;

	private bool triggerWarningActive;

	private float triggerWarningTimeStamp;

	public Vector3 CurrentMotionSensorSpawnLocation
	{
		get
		{
			if (currentActiveMotionSensor != null)
			{
				return currentActiveMotionSensor.SpawnLocation;
			}
			return motionSensorSpawnLocations[0];
		}
	}

	public Vector3 CurrentMotionSensorSpawRotat
	{
		get
		{
			if (currentActiveMotionSensor != null)
			{
				return currentActiveMotionSensor.SpawnRotation;
			}
			return motionSensorSpawnRotations[0];
		}
	}

	public event MotionSensorVoidActions EnteredPlacementMode;

	public event MotionSensorVoidActions MotionSensorPlaced;

	public event MotionSensorVoidActions MotionSensorWasReturned;

	private void Awake()
	{
		myID = base.transform.position.GetHashCode();
		GameManager.ManagerSlinger.MotionSensorManager = this;
		GameManager.StageManager.Stage += stageMe;
		GameManager.StageManager.TheGameIsLive += gameIsLive;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProductWasActivated.Event += productWasPickedUp;
		motionSensorPool = new PooledStack<MotionSensorObject>(delegate
		{
			MotionSensorObject component = Object.Instantiate(motionSensorObject, motionSensorParent).GetComponent<MotionSensorObject>();
			component.EnteredPlacementMode += motionSensorWasPickedUp;
			component.IWasPlaced += motionSensorWasPlaced;
			component.IWasTripped += motionSensorWasTripped;
			component.SoftBuild();
			return component;
		}, MOTION_SENSOR_POOL_COUNT);
		for (int num = motionSensorSpawnLocations.Length - 1; num >= 0; num--)
		{
			currentAvaibleMotionSensorSpawnLocations.Push(motionSensorSpawnLocations[num]);
		}
		for (int num2 = motionSensorSpawnRotations.Length - 1; num2 >= 0; num2--)
		{
			currentAvaibleMotionSensorSpawnRotations.Push(motionSensorSpawnRotations[num2]);
		}
		motionSensorAlertSFX.MyAudioHub = AUDIO_HUB.COMPUTER_HUB;
		motionSensorAlertSFX.MyAudioLayer = AUDIO_LAYER.WEBSITE;
	}

	private void Update()
	{
		if (triggerWarningActive)
		{
			LookUp.DesktopUI.MOTION_SENSOR_MENU_ICON_ACTIVE.alpha = Mathf.Lerp(1f, 0f, (Time.time - triggerWarningTimeStamp) / triggerWarningFadeTime);
			if (Time.time - triggerWarningTimeStamp >= triggerWarningFadeTime)
			{
				triggerWarningActive = false;
				LookUp.DesktopUI.MOTION_SENSOR_MENU_ICON_ACTIVE.alpha = 0f;
				LookUp.DesktopUI.MOTION_SENSOR_MENU_ICON_IDLE.alpha = 1f;
			}
		}
	}

	private void OnDestroy()
	{
		clearMotionSensors();
	}

	public void ReturnMotionSensor()
	{
		if (currentActiveMotionSensor != null)
		{
			StateManager.PlayerState = PLAYER_STATE.ROAMING;
			if (this.MotionSensorWasReturned != null)
			{
				this.MotionSensorWasReturned(currentActiveMotionSensor);
			}
			currentActiveMotionSensor.SpawnMe(currentActiveMotionSensor.SpawnLocation, currentActiveMotionSensor.SpawnRotation);
			currentActiveMotionSensor = null;
		}
	}

	public void TriggerMotionSensorMenu()
	{
		if (motionSensorMenuAniActive)
		{
			return;
		}
		motionSensorMenuAniActive = true;
		if (motionSensorMenuActive)
		{
			motionSensorMenuActive = false;
			DOTween.To(endValue: new Vector2(LookUp.DesktopUI.MOTION_SENSOR_MENU.anchoredPosition.x, LookUp.DesktopUI.MOTION_SENSOR_MENU.sizeDelta.y), getter: () => LookUp.DesktopUI.MOTION_SENSOR_MENU.anchoredPosition, setter: delegate(Vector2 x)
			{
				LookUp.DesktopUI.MOTION_SENSOR_MENU.anchoredPosition = x;
			}, duration: 0.25f).SetEase(Ease.InQuad).OnComplete(delegate
			{
				motionSensorMenuAniActive = false;
			});
		}
		else
		{
			motionSensorMenuActive = true;
			DOTween.To(endValue: new Vector2(LookUp.DesktopUI.MOTION_SENSOR_MENU.anchoredPosition.x, -41f), getter: () => LookUp.DesktopUI.MOTION_SENSOR_MENU.anchoredPosition, setter: delegate(Vector2 x)
			{
				LookUp.DesktopUI.MOTION_SENSOR_MENU.anchoredPosition = x;
			}, duration: 0.25f).SetEase(Ease.OutQuad).OnComplete(delegate
			{
				motionSensorMenuAniActive = false;
			});
		}
	}

	public void getSpawnMotionSensor()
	{
		spawnMotionSensor();
	}

	private void spawnMotionSensor()
	{
		if (currentAvaibleMotionSensorSpawnLocations.Count > 0 && currentAvaibleMotionSensorSpawnRotations.Count > 0)
		{
			Vector3 setPosition = currentAvaibleMotionSensorSpawnLocations.Pop();
			Vector3 setRotation = currentAvaibleMotionSensorSpawnRotations.Pop();
			MotionSensorObject motionSensorObject = motionSensorPool.Pop();
			motionSensorObject.SpawnMe(setPosition, setRotation);
			currentlyOwnedMotionSensors.Add(motionSensorObject);
		}
	}

	private void productWasPickedUp(ShadowMarketProductDefinition TheProduct)
	{
		HARDWARE_PRODUCTS productID = TheProduct.productID;
		if (productID == HARDWARE_PRODUCTS.MOTION_SENSOR)
		{
			spawnMotionSensor();
		}
	}

	private void motionSensorWasPickedUp(MotionSensorObject TheMotionSensor)
	{
		currentActiveMotionSensor = TheMotionSensor;
		StateManager.PlayerState = PLAYER_STATE.MOTION_SENSOR_PLACEMENT;
		myData.CurrentlyPlacedMotionSensors.Remove(TheMotionSensor.Transform.position.GetHashCode());
		DataManager.Save(myData);
		if (this.EnteredPlacementMode != null)
		{
			this.EnteredPlacementMode(TheMotionSensor);
		}
	}

	private void motionSensorWasPlaced(MotionSensorObject TheMotionSensor)
	{
		currentActiveMotionSensor = null;
		StateManager.PlayerState = PLAYER_STATE.ROAMING;
		SerTrans setPosition = SerTrans.Convert(TheMotionSensor.Transform.position, TheMotionSensor.Transform.rotation.eulerAngles);
		myData.CurrentlyPlacedMotionSensors.Add(TheMotionSensor.transform.position.GetHashCode(), new MotionSensorPlacementData(TheMotionSensor.Location, setPosition));
		DataManager.Save(myData);
		if (this.MotionSensorPlaced != null)
		{
			this.MotionSensorPlaced(TheMotionSensor);
		}
		int num = 0;
		for (int i = 0; i < currentlyOwnedMotionSensors.Count; i++)
		{
			if (currentlyOwnedMotionSensors[i].Placed)
			{
				num++;
			}
		}
		if (num >= 4 && BlueWhisperManager.Ins.Owns)
		{
			SteamSlinger.Ins.UnlockSteamAchievement(STEAM_ACHIEVEMENT.PARANOID);
		}
	}

	private void motionSensorWasTripped(MotionSensorObject TheMotionSensor)
	{
		if (!triggerWarningActive)
		{
			if (InventoryManager.OwnsMotionSensorAudioCue)
			{
				GameManager.AudioSlinger.PlaySound(motionSensorAlertSFX);
				BlueWhisperManager.Ins.ProcessSound(motionSensorAlertSFX);
			}
			LookUp.DesktopUI.MOTION_SENSOR_MENU_ICON_ACTIVE.alpha = 1f;
			LookUp.DesktopUI.MOTION_SENSOR_MENU_ICON_IDLE.alpha = 0f;
			triggerWarningTimeStamp = Time.time;
			triggerWarningActive = true;
		}
	}

	private void clearMotionSensors()
	{
		for (int i = 0; i < currentlyOwnedMotionSensors.Count; i++)
		{
			motionSensorPool.Push(currentlyOwnedMotionSensors[i]);
		}
		foreach (MotionSensorObject item in motionSensorPool)
		{
			item.EnteredPlacementMode -= motionSensorWasPickedUp;
			item.IWasPlaced -= motionSensorWasPlaced;
			item.IWasTripped -= motionSensorWasTripped;
		}
		currentlyOwnedMotionSensors.Clear();
		motionSensorPool.Clear();
	}

	private void stageMe()
	{
		myData = DataManager.Load<MotionSensorManagerData>(myID);
		if (myData == null)
		{
			myData = new MotionSensorManagerData(myID);
		}
		GameManager.StageManager.Stage -= stageMe;
	}

	private void gameIsLive()
	{
		int num = 0;
		foreach (KeyValuePair<int, MotionSensorPlacementData> currentlyPlacedMotionSensor in myData.CurrentlyPlacedMotionSensors)
		{
			if (currentlyOwnedMotionSensors[num] != null)
			{
				currentlyOwnedMotionSensors[num].SetPlaceMe(currentlyPlacedMotionSensor.Value.LocationPoisition, (PLAYER_LOCATION)currentlyPlacedMotionSensor.Value.LocationName);
				this.MotionSensorPlaced(currentlyOwnedMotionSensors[num]);
			}
			num++;
		}
		GameManager.StageManager.TheGameIsLive -= gameIsLive;
	}
}
