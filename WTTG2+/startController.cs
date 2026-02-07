using DG.Tweening;
using UnityEngine;

public class startController : baseController
{
	public static startController Ins;

	[SerializeField]
	private Vector3 startingCameraPOS = Vector3.zero;

	[SerializeField]
	private Vector3 startingCameraROT = Vector3.zero;

	public CustomEvent TriggerTutorialEvents = new CustomEvent(2);

	protected new void Awake()
	{
		base.Awake();
		ControllerManager.Add(this);
		Ins = this;
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
	}

	protected new void OnDestroy()
	{
		ControllerManager.Remove(Controller);
		base.OnDestroy();
	}

	private void loseControl()
	{
		Active = false;
		SetMasterLock(setLock: true);
		MyState = GAME_CONTROLLER_STATE.LOCKED;
		StateManager.PlayerState = PLAYER_STATE.BUSY;
	}

	private void postBaseStage()
	{
		PostStage.Event -= postBaseStage;
		if (Active)
		{
			LookUp.PlayerUI.BlackScreenCG.alpha = 1f;
			CameraManager.GetCameraHook(CameraIControl).SetMyParent(base.transform);
			MyCamera.transform.localPosition = startingCameraPOS;
		}
	}

	private void postBaseLive()
	{
		PostLive.Event -= postBaseLive;
		if (!DifficultyManager.HackerMode)
		{
			AppCreator.CreateApps();
		}
		if (!Active)
		{
			return;
		}
		MyCamera.transform.localPosition = startingCameraPOS;
		MyCamera.transform.localRotation = Quaternion.Euler(startingCameraROT);
		Sequence sequence = DOTween.Sequence().OnComplete(delegate
		{
			loseControl();
			deskController.Ins.TakeOverFromStart();
			if (!DifficultyManager.LeetMode)
			{
				TutorialProductBehaviour.Ins.AddRemoteVPNSpawn();
				TutorialStartBehaviour.Ins.InitializeDesktop();
			}
			else
			{
				TutorialStartBehaviour.Ins.InitializeDesktop();
			}
		});
		sequence.Insert(1.75f, DOTween.To(() => LookUp.PlayerUI.BlackScreenCG.alpha, delegate(float x)
		{
			LookUp.PlayerUI.BlackScreenCG.alpha = x;
		}, 0f, 3f).SetEase(Ease.Linear));
		sequence.Insert(4.75f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
		{
			MyCamera.transform.localPosition = x;
		}, new Vector3(-0.2071f, 0.035f, 0f), 2f).SetEase(Ease.Linear));
		sequence.Insert(5f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
		{
			MyCamera.transform.localRotation = x;
		}, new Vector3(15f, 90f, 0f), 1f).SetEase(Ease.Linear).SetOptions());
		sequence.Play();
	}
}
