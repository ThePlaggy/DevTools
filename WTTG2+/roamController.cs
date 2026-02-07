using DG.Tweening;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class roamController : moveableController
{
	public static roamController Ins;

	public Transform HeadTiltHolder;

	public float HeadTiltValue;

	public float HeadTitleXValue;

	private Vector3 defaultHeadTiltPOS;

	private Sequence getOutOfTheWaySeq;

	private Vector3 lastCameraPOS;

	private Vector3 lastCameraROT;

	private PLAYER_STATE lastPlayerState;

	private Vector3 lastPOS;

	private Vector3 lastROT;

	private bool lockFromDoorOpen;

	private Sequence switchToPeepHoleSequence;

	public CustomEvent TookControlActions = new CustomEvent(5);

	protected new void Awake()
	{
		Ins = this;
		base.Awake();
		ControllerManager.Add(this);
		defaultHeadTiltPOS = HeadTiltHolder.transform.localPosition;
		PostStage.Event += postBaseStage;
		PostLive.Event += postBaseLive;
	}

	protected new void Start()
	{
		base.Start();
	}

	protected new void Update()
	{
		base.Update();
		takeInput();
	}

	protected new void OnDestroy()
	{
		ControllerManager.Remove(Controller);
		base.OnDestroy();
	}

	private void OnDrawGizmos()
	{
		if (MyCharcterController != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(base.transform.position, MyCharcterController.radius);
			Gizmos.DrawLine(base.transform.position, base.transform.position + Vector3.down * (MyCharcterController.height / 2f + MyCharcterController.radius));
		}
	}

	public void LoseControl()
	{
		Active = false;
		SetMasterLock(setLock: true);
		MyState = GAME_CONTROLLER_STATE.LOCKED;
		StateManager.PlayerState = PLAYER_STATE.BUSY;
	}

	public void TakeControl()
	{
		if (!MoveableControllerInit)
		{
			Init();
		}
		Active = true;
		SetMasterLock(setLock: false);
		MyState = GAME_CONTROLLER_STATE.IDLE;
		StateManager.PlayerState = PLAYER_STATE.ROAMING;
		TookControlActions.Execute();
	}

	public void GlobalTakeOver()
	{
		MyMouseCapture.setCameraTargetRot(MyCamera.transform.localRotation.x);
		CameraManager.GetCameraHook(CameraIControl).SetMyParent(HeadTiltHolder);
		Sequence sequence = DOTween.Sequence().OnComplete(TakeControl);
		sequence.Insert(0f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
		{
			MyCamera.transform.localPosition = x;
		}, DefaultCameraPOS, 0.75f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
		{
			MyCamera.transform.localRotation = x;
		}, DefaultCameraPOS, 0.5f).SetEase(Ease.Linear).SetOptions());
		sequence.Play();
	}

	public void GlobalTakeOver(float POSTime, float ROTTime, float DelayTime)
	{
		MyMouseCapture.setCameraTargetRot(MyCamera.transform.localRotation.x);
		CameraManager.GetCameraHook(CameraIControl).SetMyParent(HeadTiltHolder);
		Sequence sequence = DOTween.Sequence().OnComplete(TakeControl);
		sequence.Insert(0f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
		{
			MyCamera.transform.localPosition = x;
		}, DefaultCameraPOS, POSTime).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
		{
			MyCamera.transform.localRotation = x;
		}, DefaultCameraPOS, ROTTime).SetEase(Ease.Linear).SetOptions());
		sequence.SetDelay(DelayTime);
		sequence.Play();
	}

	public void SwitchToPeepHoleController()
	{
		DataManager.LockSave = true;
		LoseControl();
		GameManager.InteractionManager.LockInteraction();
		GameManager.BehaviourManager.CrossHairBehaviour.HideCrossHairGroup();
		lastPOS = base.transform.position;
		lastROT = base.transform.rotation.eulerAngles;
		lastCameraPOS = MyCamera.transform.localPosition;
		lastCameraROT = MyCamera.transform.localRotation.eulerAngles;
		Sequence sequence = DOTween.Sequence().OnComplete(delegate
		{
			ControllerManager.Get<peepHoleController>(GAME_CONTROLLER.PEEP_HOLE).TakeOver();
		});
		sequence.Insert(0f, DOTween.To(() => base.transform.position, delegate(Vector3 x)
		{
			base.transform.position = x;
		}, new Vector3(-2.194f, 40.5183f, -4.827071f), 0.75f).SetEase(Ease.OutQuart));
		sequence.Insert(0f, DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, new Vector3(0f, 180f, 0f), 0.75f).SetEase(Ease.OutQuart).SetOptions());
		sequence.Insert(0f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
		{
			MyCamera.transform.localPosition = x;
		}, DefaultCameraPOS, 0.45f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
		{
			MyCamera.transform.localRotation = x;
		}, DefaultCameraROT, 0.45f).SetEase(Ease.Linear).SetOptions());
		sequence.Insert(0.45f, DOTween.To(() => MyCamera.fieldOfView, delegate(float x)
		{
			MyCamera.fieldOfView = x;
		}, 15.5f, 0.4f).SetEase(Ease.Linear));
		sequence.Insert(0.45f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
		{
			MyCamera.transform.localPosition = x;
		}, new Vector3(0.0042f, 0.2592f, 0.1175f), 0.6f).SetEase(Ease.Linear));
		sequence.Play();
	}

	public void TakeOverFromPeep()
	{
		Sequence sequence = DOTween.Sequence().OnComplete(delegate
		{
			DataManager.LockSave = false;
			GameManager.InteractionManager.UnLockInteraction();
			GameManager.BehaviourManager.CrossHairBehaviour.ShowCrossHairGroup();
			TakeControl();
		});
		sequence.Insert(0f, DOTween.To(() => MyCamera.fieldOfView, delegate(float x)
		{
			MyCamera.fieldOfView = x;
		}, 60f, 0.4f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
		{
			MyCamera.transform.localPosition = x;
		}, DefaultCameraPOS, 0.4f).SetEase(Ease.Linear));
		sequence.Insert(0.4f, DOTween.To(() => base.transform.position, delegate(Vector3 x)
		{
			base.transform.position = x;
		}, lastPOS, 0.75f).SetEase(Ease.InQuart));
		sequence.Insert(0.4f, DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, lastROT, 0.75f).SetEase(Ease.InQuart).SetOptions());
		sequence.Insert(0.4f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
		{
			MyCamera.transform.localPosition = x;
		}, lastCameraPOS, 0.4f).SetEase(Ease.Linear));
		sequence.Insert(0.4f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
		{
			MyCamera.transform.localRotation = x;
		}, lastCameraROT, 0.4f).SetEase(Ease.Linear));
		sequence.Play();
	}

	public void JumpRailingFromBalcoy()
	{
		SetMasterLock(setLock: true);
		GameManager.InteractionManager.LockInteraction();
		GameManager.BehaviourManager.CrossHairBehaviour.HideCrossHairGroup();
		MyMouseCapture.setFullCameraTargetRot(Vector3.zero);
		Sequence sequence = DOTween.Sequence().OnComplete(delegate
		{
			CameraManager.GetCameraHook(CAMERA_ID.MAIN).SwitchToGlobalParent();
			base.transform.position = new Vector3(-1.973f, 40.138f, 4.16f);
			CameraManager.GetCameraHook(CameraIControl).SetMyParent(HeadTiltHolder);
			Sequence sequence2 = DOTween.Sequence().OnComplete(delegate
			{
				GameManager.InteractionManager.UnLockInteraction();
				GameManager.BehaviourManager.CrossHairBehaviour.ShowCrossHairGroup();
				SetMasterLock(setLock: false);
			});
			sequence2.Insert(0f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
			{
				MyCamera.transform.localPosition = x;
			}, new Vector3(-0.05f, 0.2764f, -0.401f), 0.3f).SetEase(Ease.Linear));
			sequence2.Insert(0f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
			{
				MyCamera.transform.localRotation = x;
			}, new Vector3(31.414f, -12.121f, -6.387f), 0.3f).SetEase(Ease.Linear));
			sequence2.Insert(0.3f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
			{
				MyCamera.transform.localPosition = x;
			}, new Vector3(-0.4608f, 0.218f, -0.181f), 0.3f).SetEase(Ease.Linear));
			sequence2.Insert(0.3f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
			{
				MyCamera.transform.localRotation = x;
			}, new Vector3(44.885f, -19.738f, 2.839f), 0.3f).SetEase(Ease.Linear));
			sequence2.Insert(0.6f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
			{
				MyCamera.transform.localPosition = x;
			}, new Vector3(-0.628f, 0.109f, 0.061f), 0.3f).SetEase(Ease.Linear));
			sequence2.Insert(0.6f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
			{
				MyCamera.transform.localRotation = x;
			}, new Vector3(38.745f, 1.447f, 34.416f), 0.3f).SetEase(Ease.Linear));
			sequence2.Insert(0.9f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
			{
				MyCamera.transform.localPosition = x;
			}, new Vector3(-0.328f, 0.078f, 0.107f), 0.3f).SetEase(Ease.Linear));
			sequence2.Insert(0.9f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
			{
				MyCamera.transform.localRotation = x;
			}, new Vector3(21.487f, -7.096f, 15.127f), 0.3f).SetEase(Ease.Linear));
			sequence2.Insert(1.2f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
			{
				MyCamera.transform.localPosition = x;
			}, DefaultCameraPOS, 0.6f).SetEase(Ease.Linear));
			sequence2.Insert(1.2f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
			{
				MyCamera.transform.localRotation = x;
			}, DefaultCameraROT, 0.6f).SetEase(Ease.Linear));
			sequence2.Play();
		});
		sequence.Insert(0f, DOTween.To(() => base.transform.position, delegate(Vector3 x)
		{
			base.transform.position = x;
		}, new Vector3(-2.92f, 40.138f, 4.16f), 0.3f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, new Vector3(0f, 90f, 0f), 0.3f).SetEase(Ease.Linear).SetOptions());
		sequence.Insert(0f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
		{
			MyCamera.transform.localPosition = x;
		}, DefaultCameraPOS, 0.3f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
		{
			MyCamera.transform.localRotation = x;
		}, DefaultCameraROT, 0.3f).SetEase(Ease.Linear));
		sequence.Play();
	}

	public void JumpRailingFromStairWell()
	{
		SetMasterLock(setLock: true);
		GameManager.InteractionManager.LockInteraction();
		GameManager.BehaviourManager.CrossHairBehaviour.HideCrossHairGroup();
		MyMouseCapture.setFullCameraTargetRot(Vector3.zero);
		Sequence sequence = DOTween.Sequence().OnComplete(delegate
		{
			CameraManager.GetCameraHook(CAMERA_ID.MAIN).SwitchToGlobalParent();
			base.transform.position = new Vector3(-2.92f, 40.138f, 4.16f);
			CameraManager.GetCameraHook(CameraIControl).SetMyParent(HeadTiltHolder);
			Sequence sequence2 = DOTween.Sequence().OnComplete(delegate
			{
				GameManager.InteractionManager.UnLockInteraction();
				GameManager.BehaviourManager.CrossHairBehaviour.ShowCrossHairGroup();
				SetMasterLock(setLock: false);
			});
			sequence2.Insert(0f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
			{
				MyCamera.transform.localPosition = x;
			}, new Vector3(-0.0636f, 0.2971f, -0.7757f), 0.3f).SetEase(Ease.Linear));
			sequence2.Insert(0f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
			{
				MyCamera.transform.localRotation = x;
			}, new Vector3(27.067f, 4.084f, 8.391f), 0.3f).SetEase(Ease.Linear));
			sequence2.Insert(0.3f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
			{
				MyCamera.transform.localPosition = x;
			}, new Vector3(-0.212f, 0.247f, -0.544f), 0.3f).SetEase(Ease.Linear));
			sequence2.Insert(0.3f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
			{
				MyCamera.transform.localRotation = x;
			}, new Vector3(30.671f, 8.478f, 17.479f), 0.3f).SetEase(Ease.Linear));
			sequence2.Insert(0.6f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
			{
				MyCamera.transform.localPosition = x;
			}, new Vector3(-0.3255f, 0.1413f, -0.2621f), 0.3f).SetEase(Ease.Linear));
			sequence2.Insert(0.6f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
			{
				MyCamera.transform.localRotation = x;
			}, new Vector3(30.268f, 15.123f, 34.3f), 0.3f).SetEase(Ease.Linear));
			sequence2.Insert(0.9f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
			{
				MyCamera.transform.localPosition = x;
			}, new Vector3(-0.2697f, 0.1515f, -0.1587f), 0.3f).SetEase(Ease.Linear));
			sequence2.Insert(0.9f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
			{
				MyCamera.transform.localRotation = x;
			}, new Vector3(23.563f, 8.448f, 15.347f), 0.3f).SetEase(Ease.Linear));
			sequence2.Insert(1.2f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
			{
				MyCamera.transform.localPosition = x;
			}, DefaultCameraPOS, 0.6f).SetEase(Ease.Linear));
			sequence2.Insert(1.2f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
			{
				MyCamera.transform.localRotation = x;
			}, DefaultCameraROT, 0.6f).SetEase(Ease.Linear));
			sequence2.Play();
		});
		sequence.Insert(0f, DOTween.To(() => base.transform.position, delegate(Vector3 x)
		{
			base.transform.position = x;
		}, new Vector3(-2.122f, 40.138f, 4.16f), 0.3f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, new Vector3(0f, -90f, 0f), 0.3f).SetEase(Ease.Linear).SetOptions());
		sequence.Insert(0f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
		{
			MyCamera.transform.localPosition = x;
		}, DefaultCameraPOS, 0.3f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
		{
			MyCamera.transform.localRotation = x;
		}, DefaultCameraROT, 0.3f).SetEase(Ease.Linear));
		sequence.Play();
	}

	public void PutMe(Vector3 SetPOS, Vector3 SetROT, bool RememberLastLocation = false)
	{
		if (RememberLastLocation)
		{
			lastPOS = base.transform.position;
			lastROT = base.transform.rotation.eulerAngles;
		}
		base.transform.position = SetPOS;
		base.transform.rotation = Quaternion.Euler(SetROT);
	}

	public void ReturnMe()
	{
		base.transform.position = lastPOS;
		base.transform.rotation = Quaternion.Euler(lastROT);
	}

	public void ReturnMe(Vector3 CustomROT)
	{
		MyMouseCapture.setRotatingObjectRotation(CustomROT);
		MyMouseCapture.setRotatingObjectTargetRot(CustomROT);
		base.transform.position = lastPOS;
		base.transform.rotation = Quaternion.Euler(CustomROT);
	}

	public void SpawnMeTo(Vector3 ToLocation, Vector3 ToRotation, float ReSpawnDelay)
	{
		UIManager.FadeScreen(1f, 0.5f);
		SetMasterLock(setLock: true);
		lastPlayerState = StateManager.PlayerState;
		MyState = GAME_CONTROLLER_STATE.IDLE;
		StateManager.PlayerState = PLAYER_STATE.BUSY;
		GameManager.InteractionManager.LockInteraction();
		DataManager.LockSave = true;
		PauseManager.LockPause();
		GameManager.TimeSlinger.FireTimer(0.5f, delegate
		{
			base.transform.position = ToLocation;
			base.transform.rotation = Quaternion.Euler(ToRotation);
			MyMouseCapture.setRotatingObjectRotation(ToRotation);
			MyMouseCapture.setRotatingObjectTargetRot(ToRotation);
			GameManager.TimeSlinger.FireTimer(ReSpawnDelay, delegate
			{
				UIManager.FadeScreen(0f, 0.5f);
				GameManager.InteractionManager.UnLockInteraction();
				SetMasterLock(setLock: false);
				StateManager.PlayerState = lastPlayerState;
				DataManager.LockSave = false;
				PauseManager.UnLockPause();
			});
		});
	}

	public void MoveOutOfTheWay(float LookDir, Vector3 Destination, CARDINAL_DIR Direction)
	{
		SetMasterLock(setLock: true);
		lastPlayerState = StateManager.PlayerState;
		MyState = GAME_CONTROLLER_STATE.IDLE;
		StateManager.PlayerState = PLAYER_STATE.BUSY;
		Vector3 endValue = Vector3.zero;
		Vector3 endValue2 = Vector3.zero;
		Vector3 vector = new Vector3(0f, LookDir, 0f);
		switch (Direction)
		{
		case CARDINAL_DIR.FORWARD:
			endValue = Destination + base.transform.forward * MyCharcterController.radius;
			endValue2 = Destination + base.transform.forward * (MyCharcterController.radius + 0.5f);
			endValue2.x = endValue.x;
			break;
		case CARDINAL_DIR.BACK:
			endValue = Destination + -base.transform.forward * MyCharcterController.radius;
			endValue2 = Destination + -base.transform.forward * (MyCharcterController.radius + 0.5f);
			endValue2.x = endValue.x;
			break;
		case CARDINAL_DIR.LEFT:
			endValue = Destination + -base.transform.right * MyCharcterController.radius;
			endValue2 = Destination + -base.transform.right * (MyCharcterController.radius + 0.5f);
			endValue2.z = endValue.z;
			break;
		case CARDINAL_DIR.RIGHT:
			endValue = Destination + base.transform.right * MyCharcterController.radius;
			endValue2 = Destination + base.transform.right * (MyCharcterController.radius + 0.5f);
			endValue2.z = endValue.z;
			break;
		}
		endValue.y = base.transform.position.y;
		endValue2.y = base.transform.position.y;
		MyMouseCapture.setCameraTargetRot();
		MyMouseCapture.setRotatingObjectTargetRot(vector);
		getOutOfTheWaySeq = DOTween.Sequence().OnComplete(delegate
		{
			if (!lockFromDoorOpen)
			{
				StateManager.PlayerState = lastPlayerState;
				SetMasterLock(setLock: false);
			}
		});
		getOutOfTheWaySeq.Insert(0f, DOTween.To(() => base.transform.position, delegate(Vector3 x)
		{
			base.transform.position = x;
		}, endValue2, 0.45f).SetEase(Ease.OutCubic));
		getOutOfTheWaySeq.Insert(0f, DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, vector, 0.45f).SetEase(Ease.OutCubic));
		getOutOfTheWaySeq.Insert(0f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
		{
			MyCamera.transform.localPosition = x;
		}, DefaultCameraPOS, 0.65f).SetEase(Ease.OutSine));
		getOutOfTheWaySeq.Insert(0f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
		{
			MyCamera.transform.localRotation = x;
		}, DefaultCameraPOS, 0.65f).SetEase(Ease.OutSine).SetOptions());
		getOutOfTheWaySeq.Insert(0.65f, DOTween.To(() => base.transform.position, delegate(Vector3 x)
		{
			base.transform.position = x;
		}, endValue, 0.3f).SetEase(Ease.Linear));
		getOutOfTheWaySeq.Play();
	}

	public void HardMove(Vector3 Destination)
	{
		Vector3 zero = Vector3.zero;
		zero = Destination;
		zero.y = base.transform.position.y;
		base.transform.position = zero;
	}

	public void LockFromDoorRecovry()
	{
		lockFromDoorOpen = true;
	}

	public void KillOutOfWay()
	{
		if (getOutOfTheWaySeq != null)
		{
			getOutOfTheWaySeq.Pause();
			getOutOfTheWaySeq.Kill();
		}
	}

	private void takeInput()
	{
		if (!lockControl)
		{
			float num = CrossPlatformInputManager.GetAxis("HeadTilt") * HeadTiltValue;
			float x = CrossPlatformInputManager.GetAxis("HeadTilt") * HeadTitleXValue;
			DisableRun = Mathf.Abs(num) > 0f;
			HeadTiltHolder.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, num));
			HeadTiltHolder.transform.localPosition = new Vector3(x, defaultHeadTiltPOS.y, defaultHeadTiltPOS.z);
		}
	}

	private void postBaseStage()
	{
		PostStage.Event -= postBaseStage;
		if (DataManager.ContinuedGame)
		{
			MyState = GAME_CONTROLLER_STATE.IDLE;
			Active = true;
			CameraManager.GetCameraHook(CameraIControl).SetMyParent(HeadTiltHolder);
			MyCamera.transform.localPosition = DefaultCameraPOS;
			MyCamera.transform.localRotation = Quaternion.Euler(DefaultCameraROT);
			CameraManager.GetCameraHook(CameraIControl).ManualPushDataUpdate();
		}
	}

	private void postBaseLive()
	{
		PostLive.Event -= postBaseLive;
		if (Active)
		{
			TakeControl();
		}
	}
}
