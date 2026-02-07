using System;
using DG.Tweening;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class hideController : mouseableController
{
	public static hideController Ins;

	[SerializeField]
	private Vector3 defaultCameraHolderRotation;

	private Vector3 defaultCamreaHolderPOS;

	public CustomEvent<float> PlayerPeakingEvent = new CustomEvent<float>(4);

	public bool FullyHidden { get; set; }

	public Vector3 PeakPOS { get; set; }

	public GoHideTrigger HideTrigger { get; private set; }

	protected new void Awake()
	{
		base.Awake();
		Ins = this;
		ControllerManager.Add(this);
		defaultCamreaHolderPOS = MouseRotatingObject.transform.localPosition;
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

	public void TakeOverFromRoam(Action ReturnAction, GoHideTrigger SetHideTrigger)
	{
		ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).LoseControl();
		HideTrigger = SetHideTrigger;
		if (MouseCaptureInit)
		{
			MyMouseCapture.setCameraTargetRot();
			MyMouseCapture.setRotatingObjectTargetRot(defaultCameraHolderRotation);
			MyMouseCapture.setRotatingObjectRotation(defaultCameraHolderRotation);
		}
		CameraManager.GetCameraHook(CameraIControl).SetMyParent(MouseRotatingObject.transform);
		Sequence sequence = DOTween.Sequence().OnComplete(delegate
		{
			takeControl();
			ReturnAction();
		});
		sequence.Insert(0f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
		{
			MyCamera.transform.localPosition = x;
		}, Vector3.zero, 0.75f).SetEase(Ease.OutBack));
		sequence.Insert(0.65f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
		{
			MyCamera.transform.localRotation = x;
		}, Vector3.zero, 0.35f).SetEase(Ease.Linear).SetOptions());
		sequence.Play();
	}

	public void LoseControlToRoam()
	{
		loseControl();
		ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).ReturnMe();
		ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).GlobalTakeOver();
	}

	public void LoseControlToRoam(Vector3 CustomROT)
	{
		loseControl();
		ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).ReturnMe(CustomROT);
		ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).GlobalTakeOver();
	}

	public void PutMe(Vector3 SetPOS, Vector3 SetROT)
	{
		base.transform.position = SetPOS;
		base.transform.rotation = Quaternion.Euler(SetROT);
	}

	private void takeControl()
	{
		if (!MouseCaptureInit)
		{
			Init();
		}
		Active = true;
		SetMasterLock(setLock: false);
		MyState = GAME_CONTROLLER_STATE.IDLE;
		StateManager.PlayerState = PLAYER_STATE.HIDING;
	}

	private void loseControl()
	{
		Active = false;
		SetMasterLock(setLock: true);
		MyState = GAME_CONTROLLER_STATE.LOCKED;
		StateManager.PlayerState = PLAYER_STATE.BUSY;
		HideTrigger = null;
	}

	private void takeInput()
	{
		float num = ((!FullyHidden) ? 0f : CrossPlatformInputManager.GetAxis("LeftClickWeighted"));
		if (lockControl)
		{
			num = 0f;
		}
		if (FullyHidden)
		{
			PlayerPeakingEvent.Execute(num);
		}
		MouseRotatingObject.transform.localPosition = Vector3.Lerp(defaultCamreaHolderPOS, PeakPOS, num);
	}
}
