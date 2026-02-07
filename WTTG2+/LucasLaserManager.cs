using System;
using DG.Tweening;
using UnityEngine;

public class LucasLaserManager : MonoBehaviour
{
	public static LucasLaserManager Ins;

	[NonSerialized]
	public GameObject myLaser;

	[NonSerialized]
	public int currentLaserPoint;

	[NonSerialized]
	public int maxLaserRotations;

	private GameObject Failsafe1;

	private GameObject Failsafe2;

	private Vector3[] LaserPoints = new Vector3[6]
	{
		new Vector3(358f, 357.9014f, 0f),
		new Vector3(358f, 354.4868f, 0f),
		new Vector3(359.1277f, 354.4868f, 0f),
		new Vector3(359.1277f, 357.9014f, 0f),
		new Vector3(0.7477f, 357.9014f, 0f),
		new Vector3(0.7477f, 354.4868f, 0f)
	};

	private bool laserActive;

	private bool addedBathroomJump;

	private void Awake()
	{
		Ins = this;
		LucasLaserTrigger.stood = 0;
		LucasLaserTrigger.frozen = false;
		LucasLaserTrigger.jumpmode = 0;
	}

	private void OnDestroy()
	{
		Ins = null;
		LookUp.Doors.BathroomDoor.DoorOpenEvent.RemoveListener(BathroomDoorWasOpened);
		Debug.Log("[LucasLaser] Unloaded");
	}

	private void Start()
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		gameObject.name = "HitmanLaser";
		gameObject.GetComponent<BoxCollider>().isTrigger = true;
		Material material = new Material(Shader.Find("Standard"));
		material.color = new Color(1f, 0.35f, 0.4f, 0.8f);
		material.SetColor("_EmissionColor", new Color(4f, 1f, 1f, 1f));
		material.EnableKeyword("_EMISSION");
		gameObject.GetComponent<Renderer>().castShadows = false;
		gameObject.GetComponent<Renderer>().material = material;
		gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(4f, 1f, 1f, 1f));
		myLaser = gameObject;
		myLaser.transform.position = new Vector3(0.5262f, 41.3528f, 20f);
		myLaser.transform.localScale = new Vector3(0.01f, 0.01f, 50f);
		myLaser.transform.rotation = Quaternion.Euler(LaserPoints[currentLaserPoint]);
		myLaser.AddComponent<LucasLaserTrigger>();
		LucasLaserTrigger.stood = 0;
		LucasLaserTrigger.frozen = false;
		LucasLaserTrigger.jumpmode = 0;
		myLaser.SetActive(value: false);
		GameObject gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
		gameObject2.transform.position = new Vector3(-1.0833f, 40.5293f, -2.6302f);
		gameObject2.transform.localScale = new Vector3(0.1109f, 0.86f, 3.8564f);
		gameObject2.GetComponent<BoxCollider>().isTrigger = true;
		gameObject2.GetComponent<MeshRenderer>().enabled = false;
		gameObject2.AddComponent<LucasLaserFailTrigger>().SetEndPoint(new Vector3(7.7262f, 41.5528f, 20f), new Vector3(359.2525f, 21.7613f, 0f));
		Failsafe1 = gameObject2;
		Failsafe1.SetActive(value: false);
		GameObject gameObject3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
		gameObject3.transform.position = new Vector3(-3.0833f, 40.5293f, -2.6302f);
		gameObject3.transform.localScale = new Vector3(0.1109f, 0.86f, 3.8564f);
		gameObject3.GetComponent<BoxCollider>().isTrigger = true;
		gameObject3.GetComponent<MeshRenderer>().enabled = false;
		gameObject3.AddComponent<LucasLaserFailTrigger>().SetEndPoint(new Vector3(13.7625f, 41.6382f, 15.1636f), new Vector3(359.1303f, 44.9905f, 0f));
		Failsafe2 = gameObject3;
		Failsafe2.SetActive(value: false);
		Debug.Log("[LucasLaser] Ready for entry");
	}

	public void SpawnLaser()
	{
		Debug.Log("[LucasLaser] Spawning laser");
		LucasLaserTrigger.stood = 0;
		LucasLaserTrigger.frozen = false;
		LucasLaserTrigger.jumpmode = 0;
		myLaser.SetActive(value: true);
		Failsafe1.SetActive(value: true);
		Failsafe2.SetActive(value: true);
		maxLaserRotations = UnityEngine.Random.Range(7, 12);
		moveLaser();
		laserActive = true;
	}

	private void FixedUpdate()
	{
		if (laserActive && !addedBathroomJump && StateManager.PlayerLocation == PLAYER_LOCATION.BATH_ROOM && !LookUp.Doors.BathroomDoor.DoingSomething)
		{
			Debug.Log("[LucasLaser] Added bathroom jump");
			addedBathroomJump = true;
			LookUp.Doors.BathroomDoor.DoorOpenEvent.AddListener(BathroomDoorWasOpened);
			GameManager.TimeSlinger.FireTimer(Mathf.Max(10f - (float)EnemyManager.CultManager.howManyLightsAreOff() * 1.5f, 5f), DespawnLucasLaser);
		}
	}

	private void moveLaser()
	{
		if (maxLaserRotations <= 0 && StateManager.PlayerState == PLAYER_STATE.COMPUTER)
		{
			LucasLaserTrigger.frozen = true;
			ExplodeGameOver();
			return;
		}
		maxLaserRotations--;
		currentLaserPoint++;
		if (currentLaserPoint >= LaserPoints.Length)
		{
			currentLaserPoint = 0;
		}
		myLaser.transform.DORotate(LaserPoints[currentLaserPoint], 10f).SetEase(Ease.Linear).OnComplete(moveLaser);
		Debug.Log("[LucasLaser] Laser Stage: " + currentLaserPoint);
	}

	private void DespawnLucasLaser()
	{
		CancelTweener();
		laserActive = false;
		myLaser.SetActive(value: false);
		Failsafe1.SetActive(value: false);
		Failsafe2.SetActive(value: false);
		maxLaserRotations = 0;
		currentLaserPoint = 0;
		myLaser.transform.position = new Vector3(0.5262f, 41.3528f, 20f);
		myLaser.transform.localScale = new Vector3(0.01f, 0.01f, 50f);
		myLaser.transform.rotation = Quaternion.Euler(LaserPoints[currentLaserPoint]);
		LucasLaserTrigger.stood = 0;
		LucasLaserTrigger.frozen = false;
		LucasLaserTrigger.jumpmode = 0;
		LookUp.Doors.BathroomDoor.DoorOpenEvent.RemoveListener(BathroomDoorWasOpened);
		Debug.Log("[LucasLaser] Despawned");
		EnemyManager.HitManManager.LaserDespawned();
	}

	public void CancelTweener()
	{
		Debug.Log("[LucasLaser] Cancel tweener");
		myLaser.transform.DOPause();
		LucasLaserTrigger.frozen = true;
		Failsafe1.SetActive(value: false);
		Failsafe2.SetActive(value: false);
	}

	public void BathroomDoorWasOpened()
	{
		LookUp.Doors.BathroomDoor.DoorOpenEvent.RemoveListener(BathroomDoorWasOpened);
		LucasLaserTrigger.jumpmode = 1;
		PauseManager.LockPause();
		CancelTweener();
		myLaser.transform.position = new Vector3(-27.0484f, 41.6492f, 20f);
		myLaser.transform.rotation = Quaternion.Euler(359.3436f, 300.52f, 0f);
		LookUp.Doors.BathroomDoor.CancelAutoClose();
		GameManager.TimeSlinger.FireTimer(0.4f, delegate
		{
			myLaser.transform.DOScale(new Vector3(0.01f, 0.01f, 100f), 0.15f);
			myLaser.transform.DORotate(new Vector3(359.3436f, 299.4634f, 0f), 0.4f);
			GameManager.TimeSlinger.FireTimer(0.45f, delegate
			{
				if (LucasLaserTrigger.jumpmode == 1)
				{
					ExplodeGameOver();
				}
			});
		});
	}

	public static void ExplodeGameOver()
	{
		LucasLaserTrigger.jumpmode = 2;
		LookUp.PlayerUI.BlackScreenCG.alpha = 1f;
		PauseManager.LockPause();
		GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.booooom);
		GameManager.AudioSlinger.MuffleAudioLayer(AUDIO_LAYER.UNIVERSAL, 0.1f);
		GameManager.AudioSlinger.MuffleAudioLayer(AUDIO_LAYER.PLAYER, 0.1f);
		GameManager.AudioSlinger.MuffleAudioLayer(AUDIO_LAYER.MUSIC, 0.1f);
		GameManager.AudioSlinger.MuffleAudioLayer(AUDIO_LAYER.COMPUTER_SFX, 0.1f);
		GameManager.AudioSlinger.MuffleAudioLayer(AUDIO_LAYER.HACKING_SFX, 0.1f);
		GameManager.AudioSlinger.MuffleAudioLayer(AUDIO_LAYER.ENVIRONMENT, 0.1f);
		GameManager.AudioSlinger.MuffleAudioLayer(AUDIO_LAYER.OUTSIDE, 0.1f);
		GameManager.AudioSlinger.MuffleAudioLayer(AUDIO_LAYER.DEAD_DROP, 0.1f);
		GameManager.AudioSlinger.MuffleAudioLayer(AUDIO_LAYER.ENEMY, 0.1f);
		GameManager.TimeSlinger.FireTimer(0.75f, UIManager.TriggerHardGameOver, "ASSASSINATED");
	}
}
