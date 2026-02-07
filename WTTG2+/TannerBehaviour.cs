using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class TannerBehaviour : MonoBehaviour
{
	public static TannerBehaviour Ins;

	public TannerManager TannerManager;

	public AudioFileDefinition[] WalkFootStepSFXS;

	public AudioFileDefinition havingFunSFX;

	public AudioFileDefinition laughSFX;

	public AudioHubObject voiceHub;

	public GameObject syringe;

	public bool TannerIsCustom;

	private SkinnedMeshRenderer syringeMesh;

	private TannerMainDoorJump mainDoorJump;

	private AudioHubObject footHub;

	private Transform cameraBone;

	private CameraHook myCamera;

	private Animator syringeAC;

	private Animator myAC;

	private bool processingFootSteps;

	private GameObject[] CustomTanner = new GameObject[2];

	private GameObject[] OriginalTanner = new GameObject[2];

	private RuntimeAnimatorController OriginalAC;

	private RuntimeAnimatorController CustomAC;

	private Avatar OriginalAvatar;

	private Avatar CustomAvatar;

	public void Spawn(Vector3 SpawnPos, Vector3 SpawnRot)
	{
		base.gameObject.transform.position = SpawnPos;
		base.gameObject.transform.rotation = Quaternion.Euler(SpawnRot);
	}

	public void TriggerSyringeAnim(string name)
	{
		syringeAC.SetTrigger(name);
	}

	public void TriggerAnim(string name)
	{
		myAC.SetTrigger(name);
	}

	public void SwitchToNormal()
	{
		SwitchModel(toCustom: false);
	}

	public void SwitchToCustom()
	{
		SwitchModel(toCustom: true);
	}

	public void TriggerKickDoorJump()
	{
		DoorTrigger mainDoor = LookUp.Doors.MainDoor;
		if (mainDoor.SetDistanceLODs != null)
		{
			DistanceLOD[] setDistanceLODs = mainDoor.SetDistanceLODs;
			DistanceLOD[] array = setDistanceLODs;
			foreach (DistanceLOD distanceLOD in array)
			{
				distanceLOD.OverwriteCulling = true;
			}
		}
		mainDoor.KickDoorOpen();
		GameManager.TimeSlinger.FireTimer(0.09f, TriggerRushPlayerJump);
	}

	public void TriggerRushPlayerJump()
	{
		Debug.Log("[Tanner] TriggerRushPlayerJump - Init");
		syringeMesh.enabled = true;
		syringe.transform.localPosition = new Vector3(0.009999645f, 0.03499988f, 0.15f);
		myCamera.GetComponent<Camera>().nearClipPlane = 0.001f;
		PauseManager.LockPause();
		roamController.Ins.LoseControl();
		TannerRoamJumper.Ins.SetJumpPPVol();
		TannerHeadLightHelper.Ins.EnableLight();
		GameManager.InteractionManager.LockInteraction();
		TriggerAnim("TriggerClosetJump");
		TriggerSyringeAnim("TriggerClosetJump");
		GameManager.TimeSlinger.FireTimer(0.22f, delegate
		{
			myCamera.SetMyParent(cameraBone);
			myCamera.GetComponent<Camera>().fieldOfView = 65f;
			GameManager.AudioSlinger.PlaySound(TannerManager.JumpSFX);
			GameManager.AudioSlinger.PlaySound(TannerManager.TackleHitSFX);
			GameManager.AudioSlinger.PlaySound(TannerManager.HeadHitSFX);
			GameManager.TimeSlinger.FireTimer(1.84f, delegate
			{
				TannerRoamJumper.Ins.SetDruggedPPVol();
				Debug.Log("[Tanner] TriggerRushPlayerJump - Drugged");
				GameManager.AudioSlinger.PlaySound(TannerManager.NeedleSFX);
			});
			DOTween.To(() => myCamera.transform.localPosition, delegate(Vector3 x)
			{
				myCamera.transform.localPosition = x;
			}, Vector3.zero, 0.3f).SetEase(Ease.Linear);
			DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
			{
				myCamera.transform.localRotation = x;
			}, Vector3.zero, 0.3f).SetEase(Ease.Linear);
		});
		GameManager.TimeSlinger.FireTimer(3.75f, delegate
		{
			DataManager.ClearGameData();
			UIManager.TriggerGameOver("YOU BECAME A STATISTIC");
			Debug.Log("[Tanner] TriggerRushPlayerJump - SoftGameOver");
		});
		GameManager.TimeSlinger.FireTimer(4.4f, delegate
		{
			MainCameraHook.Ins.ClearARF();
			TannerRoamJumper.Ins.ClearPPVol();
			Debug.Log("[Tanner] TriggerRushPlayerJump - ClearEffects");
		});
	}

	public void TriggerComputerJump()
	{
		Debug.Log("[Tanner] TriggerComputerJump - Init");
		computerController.Ins.LeaveEvents.Event -= TriggerComputerJump;
		syringe.transform.localPosition = new Vector3(0.011f, 0.092f, 0.031f);
		myCamera.GetComponent<Camera>().nearClipPlane = 0.001f;
		PauseManager.LockPause();
		DataManager.LockSave = true;
		deskController.Ins.LockRecovery();
		deskController.Ins.SetMasterLock(setLock: true);
		GameManager.InteractionManager.LockInteraction();
		TannerManager.JumpSFX.Volume = 0.6f;
		TannerRoamJumper.Ins.SetJumpPPVol();
		TriggerSyringeAnim("Jumpscare3");
		TriggerAnim("TriggerHavingFun");
		GameManager.TimeSlinger.FireTimer(0.1f, delegate
		{
			Debug.Log("[Tanner] TriggerComputerJump - MainCameraHook(TriggerTannerJump)");
			myCamera.transform.SetParent(cameraBone);
			AudioReverbFilter component = MainCameraHook.Ins.GetComponent<AudioReverbFilter>();
			component.reverbPreset = AudioReverbPreset.Livingroom;
			component.enabled = true;
			TannerHeadLightHelper.Ins.EnableLight();
			GameManager.AudioSlinger.PlaySound(TannerManager.JumpSFX);
			GameManager.TimeSlinger.FireTimer(havingFunSFX.DelayAmount, delegate
			{
				GameObject obj = new GameObject("HavingFunHUH");
				obj.transform.position = deskController.Ins.transform.position;
				AudioSource audioSource = obj.AddComponent<AudioSource>();
				audioSource.clip = havingFunSFX.AudioClip;
				audioSource.Play();
			});
			GameManager.TimeSlinger.FireTimer(1.84f, delegate
			{
				Debug.Log("[Tanner] TriggerComputerJump - Drugged");
				GameManager.AudioSlinger.PlaySound(TannerManager.NeedleSFX);
				TannerRoamJumper.Ins.SetDruggedPPVol();
			});
			DOTween.To(() => myCamera.transform.localPosition, delegate(Vector3 x)
			{
				myCamera.transform.localPosition = x;
			}, Vector3.zero, 0.25f).SetEase(Ease.Linear);
			DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
			{
				myCamera.transform.localRotation = x;
			}, Vector3.zero, 0.25f).SetEase(Ease.Linear);
			DOTween.To(() => myCamera.GetComponent<Camera>().fieldOfView, delegate(float x)
			{
				myCamera.GetComponent<Camera>().fieldOfView = x;
			}, 70f, 0.25f).SetEase(Ease.Linear);
		});
		GameManager.TimeSlinger.FireTimer(3.4f, delegate
		{
			DataManager.ClearGameData();
			UIManager.TriggerGameOver("YOU BECAME A STATISTIC");
			Debug.Log("[Tanner] TriggerComputerJump - SoftGameOver");
		});
		GameManager.TimeSlinger.FireTimer(4.1f, delegate
		{
			MainCameraHook.Ins.ClearARF();
			TannerRoamJumper.Ins.ClearPPVol();
			Debug.Log("[Tanner] TriggerComputerJump - ClearEffects");
		});
	}

	public void RemoveMainDoorJump()
	{
		Debug.Log("[Tanner] DestroyMainDoorJump - Removing...");
		LookUp.Doors.MainDoor.DoorOpenEvent.RemoveListener(mainDoorJump.Stage);
		LookUp.Doors.MainDoor.DoorWasOpenedEvent.RemoveListener(mainDoorJump.Execute);
	}

	public void AddMainDoorJump()
	{
		Debug.Log("[Tanner] AddMainDoorJump - Adding...");
		LookUp.Doors.MainDoor.DoorOpenEvent.AddListener(mainDoorJump.Stage);
		LookUp.Doors.MainDoor.DoorWasOpenedEvent.AddListener(mainDoorJump.Execute);
	}

	public void RunAwayFromDoor()
	{
		Debug.Log("[Tanner] RunAwayFromDoor - Trigger PeepholeShock");
		SwitchToCustom();
		TriggerAnim("TriggerPeepholeShock");
		GameManager.TimeSlinger.FireTimer(1.25f, delegate
		{
			GameManager.TimeSlinger.FireTimer(0.25f, delegate
			{
				ToggleFootSteps(playing: true);
			});
			TannerHeadLightHelper.Ins.DisableLight();
			base.transform.DOMove(new Vector3(10f, base.transform.position.y, -5f), 5f).SetEase(Ease.Linear).OnComplete(delegate
			{
				Debug.Log("[Tanner] Despawn me");
				TannerManager.DeSpawn();
				SwitchToNormal();
				doorlogBehaviour.MayAddDoorlog("Floor 8 Hallway", mode: false);
			});
		});
	}

	public void ToggleFootSteps(bool playing)
	{
		if (playing)
		{
			StartCoroutine(ProcessFootSteps());
		}
		processingFootSteps = playing;
	}

	private void SwitchModel(bool toCustom)
	{
		if (TannerIsCustom != toCustom)
		{
			TannerIsCustom = toCustom;
			Array.ForEach(OriginalTanner, delegate(GameObject x)
			{
				x.SetActive(!toCustom);
			});
			Array.ForEach(CustomTanner, delegate(GameObject x)
			{
				x.SetActive(toCustom);
			});
			myAC.runtimeAnimatorController = (toCustom ? CustomAC : OriginalAC);
			myAC.avatar = (toCustom ? CustomAvatar : OriginalAvatar);
			TannerHeadLightHelper.Ins.SwitchLightSource(toCustom);
			Debug.Log($"Switch Tanner to isCustom: {TannerIsCustom}");
		}
	}

	private IEnumerator ProcessFootSteps()
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(0.4f, 0.45f));
		int num = UnityEngine.Random.Range(1, WalkFootStepSFXS.Length);
		footHub.PlaySoundWithWildPitch(WalkFootStepSFXS[num], 0.85f, 1.1f);
		AudioFileDefinition audioFileDefinition = WalkFootStepSFXS[num];
		WalkFootStepSFXS[num] = WalkFootStepSFXS[0];
		WalkFootStepSFXS[0] = audioFileDefinition;
		if (processingFootSteps)
		{
			StartCoroutine(ProcessFootSteps());
		}
	}

	private void Awake()
	{
		Ins = this;
		TannerManager = TannerManager.Ins;
		myCamera = CameraManager.GetCameraHook(CAMERA_ID.MAIN);
		cameraBone = GameObject.Find("camera_bone").transform;
		myAC = GetComponent<Animator>();
		myAC.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		syringe = UnityEngine.Object.Instantiate(CustomObjectLookUp.TheSyringe);
		syringe.transform.SetParent(base.transform);
		syringeAC = syringe.GetComponent<Animator>();
		syringeAC.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		syringeMesh = GameObject.Find("SK_Syringe_01").GetComponent<SkinnedMeshRenderer>();
		syringeMesh.enabled = true;
		mainDoorJump = new TannerMainDoorJump();
		BuildAudio();
		BuildCustomPool();
	}

	private void OnDestroy()
	{
		Ins = null;
	}

	private void BuildAudio()
	{
		footHub = GameObject.Find("TannerFootHub").AddComponent<AudioHubObject>();
		voiceHub = GameObject.Find("TannerVoiceHub").AddComponent<AudioHubObject>();
		footHub.START_SO_POOL_COUNT = 2;
		voiceHub.START_SO_POOL_COUNT = 2;
		WalkFootStepSFXS = (AudioFileDefinition[])roamController.Ins.WalkFootStepSFXS.Clone();
		AudioFileDefinition[] walkFootStepSFXS = WalkFootStepSFXS;
		foreach (AudioFileDefinition audioFileDefinition in walkFootStepSFXS)
		{
			audioFileDefinition.MyAudioLayer = AUDIO_LAYER.ENEMY;
			audioFileDefinition.Volume = 0.925f;
		}
		laughSFX = CustomSoundLookUp.Tanner_Laugh;
		laughSFX.Volume = 0.75f;
		havingFunSFX = CustomSoundLookUp.Having;
		havingFunSFX.Delay = true;
		havingFunSFX.DelayAmount = 0.22f;
	}

	private void BuildCustomPool()
	{
		OriginalAC = myAC.runtimeAnimatorController;
		OriginalAvatar = myAC.avatar;
		CustomAC = CustomObjectLookUp.CustomTannerAC;
		CustomAvatar = CustomObjectLookUp.TannerTPoseAvatar;
		OriginalTanner[0] = base.transform.Find("SK_Tanner_01").gameObject;
		OriginalTanner[1] = base.transform.Find("WorldJoint").gameObject;
		CustomTanner[0] = base.transform.Find("SK_Tanner").gameObject;
		CustomTanner[1] = base.transform.Find("mixamorig:Hips").gameObject;
	}
}
