using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.CrossPlatformInput;

public class peepHoleController : mouseableController
{
	public static peepHoleController Ins;

	private Camera computerCamera;

	private Camera mainCamera;

	public CustomEvent TakingOverEvents = new CustomEvent(2);

	private AutoExposure tmpAE;

	private Bloom tmpBloom;

	private PostProcessVolume tmpPPVol;

	public CustomEvent TookOverEvents = new CustomEvent(2);

	public bool LockOutLeave { get; set; }

	protected new void Awake()
	{
		Ins = this;
		base.Awake();
		ControllerManager.Add(this);
		CameraManager.Get(CAMERA_ID.MAIN, out mainCamera);
		CameraManager.Get(CAMERA_ID.COMPUTER, out computerCamera);
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

	public void DoTakeOver()
	{
		TakingOverEvents.Execute();
		ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).SwitchToPeepHoleController();
	}

	public void TakeOver()
	{
		FlashLightBehaviour.Ins.LockOut();
		tmpAE = ScriptableObject.CreateInstance<AutoExposure>();
		tmpAE.enabled.Override(x: true);
		tmpAE.minLuminance.Override(-9f);
		tmpAE.maxLuminance.Override(-9f);
		tmpBloom = ScriptableObject.CreateInstance<Bloom>();
		tmpBloom.enabled.Override(x: true);
		tmpBloom.intensity.Override(70f);
		tmpPPVol = PostProcessManager.instance.QuickVolume(base.gameObject.layer, 100f, tmpAE, tmpBloom);
		tmpPPVol.weight = 1f;
		MyCamera.enabled = true;
		computerCamera.enabled = false;
		mainCamera.enabled = false;
		if (!MouseCaptureInit)
		{
			Init();
		}
		Active = true;
		SetMasterLock(setLock: false);
		MyState = GAME_CONTROLLER_STATE.IDLE;
		StateManager.PlayerState = PLAYER_STATE.PEEPING;
		DOTween.To(() => tmpPPVol.weight, delegate(float x)
		{
			tmpPPVol.weight = x;
		}, 0f, 0.75f).SetEase(Ease.Linear).OnComplete(delegate
		{
			RuntimeUtilities.DestroyVolume(tmpPPVol, destroyProfile: false);
		});
		TookOverEvents.Execute();
	}

	public void ForceOut()
	{
		leavePeepMode();
	}

	private void leavePeepMode()
	{
		FlashLightBehaviour.Ins.UnLock();
		Active = false;
		SetMasterLock(setLock: true);
		MyState = GAME_CONTROLLER_STATE.LOCKED;
		StateManager.PlayerState = PLAYER_STATE.BUSY;
		mainCamera.enabled = true;
		computerCamera.enabled = true;
		MyCamera.enabled = false;
		ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).TakeOverFromPeep();
	}

	private void takeInput()
	{
		if (!lockControl && !LockOutLeave && CrossPlatformInputManager.GetButtonDown("RightClick"))
		{
			leavePeepMode();
		}
	}
}
