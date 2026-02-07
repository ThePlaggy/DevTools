using DG.Tweening;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class deskController : mouseableController
{
	public static deskController Ins;

	public float MaxLookLeft = -70f;

	public float MaxLookRight = 70f;

	public Vector3 DefaultCameraHolderRotation;

	private Vector3 horzVector = Vector3.zero;

	private bool lockOutFromRecovery;

	protected new void Awake()
	{
		Ins = this;
		base.Awake();
		ControllerManager.Add(this);
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
		getInput();
	}

	protected new void OnDestroy()
	{
		ControllerManager.Remove(Controller);
		base.OnDestroy();
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
		if (!MouseCaptureInit)
		{
			Init();
		}
		Active = true;
		SetMasterLock(setLock: false);
		MyState = GAME_CONTROLLER_STATE.IDLE;
		StateManager.PlayerState = PLAYER_STATE.DESK;
	}

	public void TakeOverFromRoam()
	{
		if (MouseCaptureInit)
		{
			MyMouseCapture.setCameraTargetRot();
			MyMouseCapture.setRotatingObjectTargetRot(DefaultCameraHolderRotation);
			MyMouseCapture.setRotatingObjectRotation(DefaultCameraHolderRotation);
		}
		CameraManager.GetCameraHook(CameraIControl).SetMyParent(MouseRotatingObject.transform);
		Sequence sequence = DOTween.Sequence().OnComplete(TakeControl);
		sequence.Insert(0f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
		{
			MyCamera.transform.localPosition = x;
		}, Vector3.zero, 0.75f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
		{
			MyCamera.transform.localRotation = x;
		}, Vector3.zero, 0.5f).SetEase(Ease.Linear).SetOptions());
		sequence.Play();
	}

	public void TakeOverFromStart()
	{
		if (MouseCaptureInit)
		{
			MyMouseCapture.setCameraTargetRot();
			MyMouseCapture.setRotatingObjectTargetRot(DefaultCameraHolderRotation);
			MyMouseCapture.setRotatingObjectRotation(DefaultCameraHolderRotation);
		}
		CameraManager.GetCameraHook(CameraIControl).SetMyParent(MouseRotatingObject.transform);
		Sequence sequence = DOTween.Sequence().OnComplete(TakeControl);
		sequence.Insert(0f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
		{
			MyCamera.transform.localPosition = x;
		}, Vector3.zero, 1f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
		{
			MyCamera.transform.localRotation = x;
		}, Vector3.zero, 0.75f).SetEase(Ease.Linear).SetOptions());
		sequence.Play();
	}

	public void SwitchToComputerController()
	{
		LoseControl();
		GameManager.BehaviourManager.CrossHairBehaviour.HideCrossHairGroup();
		GameManager.AudioSlinger.UnMuffleAudioHub(AUDIO_HUB.COMPUTER_HUB, 0.3f);
		Sequence sequence = DOTween.Sequence().OnComplete(ControllerManager.Get<computerController>(GAME_CONTROLLER.COMPUTER).TakeControl);
		sequence.Insert(0f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
		{
			MyCamera.transform.localRotation = x;
		}, Vector3.zero, 0.3f).SetEase(Ease.OutSine).SetOptions());
		sequence.Insert(0f, DOTween.To(() => MouseRotatingObject.transform.localRotation, delegate(Quaternion x)
		{
			MouseRotatingObject.transform.localRotation = x;
		}, new Vector3(0f, 90f, 0f), 0.3f).SetEase(Ease.OutSine).SetOptions());
		sequence.Insert(0f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
		{
			MyCamera.transform.localPosition = x;
		}, new Vector3(-0.03f, -0.13f, 0.014f), 0.3f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => MyCamera.fieldOfView, delegate(float x)
		{
			MyCamera.fieldOfView = x;
		}, 30.9f, 0.3f).SetEase(Ease.InSine));
		sequence.Play();
	}

	public void LeaveComputerMode()
	{
		GameManager.AudioSlinger.MuffleAudioHub(AUDIO_HUB.COMPUTER_HUB, 0.6f, 0.3f);
		Sequence sequence = DOTween.Sequence().OnComplete(delegate
		{
			if (!lockOutFromRecovery)
			{
				TakeControl();
				MyMouseCapture.setCameraTargetRot();
				MyMouseCapture.setRotatingObjectTargetRot(DefaultCameraHolderRotation);
				MyMouseCapture.setRotatingObjectRotation(DefaultCameraHolderRotation);
				GameManager.InteractionManager.UnLockInteraction();
				GameManager.BehaviourManager.CrossHairBehaviour.ShowCrossHairGroup();
			}
		});
		sequence.Insert(0f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
		{
			MyCamera.transform.localPosition = x;
		}, Vector3.zero, 0.3f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => MyCamera.fieldOfView, delegate(float x)
		{
			MyCamera.fieldOfView = x;
		}, 60f, 0.3f).SetEase(Ease.InSine));
		sequence.Play();
	}

	public void LockRecovery()
	{
		lockOutFromRecovery = true;
	}

	public void TakeOverMainCamera()
	{
		CameraManager.GetCameraHook(CameraIControl).SetMyParent(MouseRotatingObject.transform);
	}

	private void getInput()
	{
		if (!lockControl && CrossPlatformInputManager.GetButtonDown("RightClick") && !DifficultyManager.HackerMode)
		{
			switchToRoamController();
		}
	}

	private void switchToRoamController()
	{
		LoseControl();
		ComputerChairObject.Ins.SetToNotInUsePosition();
		ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).GlobalTakeOver();
	}

	private void postBaseStage()
	{
		PostStage.Event -= postBaseStage;
		if (Active)
		{
			CameraManager.GetCameraHook(CameraIControl).SetMyParent(MouseRotatingObject.transform);
			if (!DataManager.ContinuedGame)
			{
				MyCamera.transform.localPosition = Vector3.zero;
				MyCamera.transform.localRotation = Quaternion.Euler(Vector3.zero);
			}
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

	public void UnLockRecovery()
	{
		lockOutFromRecovery = false;
	}

	public void flattenCamera()
	{
		Sequence s = DOTween.Sequence();
		s.Insert(0f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
		{
			MyCamera.transform.localRotation = x;
		}, Vector3.zero, 0.3f).SetEase(Ease.OutSine).SetOptions());
		s.Insert(0f, DOTween.To(() => MouseRotatingObject.transform.localRotation, delegate(Quaternion x)
		{
			MouseRotatingObject.transform.localRotation = x;
		}, new Vector3(0f, 90f, 0f), 0.3f).SetEase(Ease.OutSine).SetOptions());
	}
}
