using UnityStandardAssets.CrossPlatformInput;

public class computerController : baseController
{
	public static computerController Ins;

	public CustomEvent EnterEvents = new CustomEvent(3);

	public CustomEvent LeaveEvents = new CustomEvent(3);

	private ComputerCameraManager myComputerCameraManager;

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
		if (!lockControl && CrossPlatformInputManager.GetButtonDown("RightClick") && !DifficultyManager.HackerMode)
		{
			LeaveMe();
		}
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
		myComputerCameraManager.BecomeSlave();
		GameManager.ManagerSlinger.CursorManager.SetOverwrite(setValue: false);
		GameManager.ManagerSlinger.CursorManager.SwitchToDefaultCursor();
		if (AppCreator.CamHider != null)
		{
			AppCreator.CamHider.SetActive(value: true);
		}
		if (CamHookBehaviour.cam != null)
		{
			CamHookBehaviour.cam.SetActive(value: true);
		}
		if (CamHookBehaviour.appLaunched)
		{
			DelfalcoBehaviour.Ins.CamsClosed();
		}
	}

	public void TakeControl()
	{
		EnterEvents.Execute();
		FlashLightBehaviour.Ins.LockOut();
		Active = true;
		SetMasterLock(setLock: false);
		MyState = GAME_CONTROLLER_STATE.IDLE;
		StateManager.PlayerState = PLAYER_STATE.COMPUTER;
		myComputerCameraManager.BecomeMaster();
		LookUp.DesktopUI.DesktopGraphicRaycaster.enabled = true;
		GameManager.InteractionManager.LockInteraction();
		GameManager.BehaviourManager.CrossHairBehaviour.HideCrossHairGroup();
		GameManager.ManagerSlinger.CursorManager.SwitchToCustomCursor();
		GameManager.ManagerSlinger.CursorManager.SetOverwrite(setValue: true);
		if (AppCreator.CamHider != null)
		{
			AppCreator.CamHider.SetActive(value: false);
		}
		if (CamHookBehaviour.cam != null)
		{
			CamHookBehaviour.cam.SetActive(value: false);
		}
		if (CamHookBehaviour.appLaunched)
		{
			DelfalcoBehaviour.Ins.CamsOpened();
		}
	}

	public void LeaveMe()
	{
		GameManager.TimeSlinger.FireTimer(0.3f, delegate
		{
			FlashLightBehaviour.Ins.UnLock();
			LeaveEvents.Execute();
		});
		LookUp.DesktopUI.DesktopGraphicRaycaster.enabled = false;
		LoseControl();
		ControllerManager.Get<deskController>(GAME_CONTROLLER.DESK).LeaveComputerMode();
	}

	private void postBaseStage()
	{
		PostStage.Event -= postBaseStage;
		myComputerCameraManager = ComputerCameraManager.Ins;
		if (Active)
		{
			ControllerManager.Get<deskController>(GAME_CONTROLLER.DESK).TakeOverMainCamera();
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

	public void LeaveMeForced()
	{
		GameManager.TimeSlinger.FireTimer(0.3f, delegate
		{
			FlashLightBehaviour.Ins.UnLock();
			LeaveEvents.Execute();
		});
		LookUp.DesktopUI.DesktopGraphicRaycaster.enabled = false;
		LoseControl();
		ControllerManager.Get<deskController>(GAME_CONTROLLER.DESK).LeaveComputerMode();
	}
}
