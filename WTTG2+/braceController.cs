using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.CrossPlatformInput;

public class braceController : mouseableController
{
	public static braceController Ins;

	[SerializeField]
	private float maxRotationRight = 90f;

	[SerializeField]
	private float maxTopPeak = 0.5f;

	[SerializeField]
	private float maxRightPeak = 0.1f;

	[SerializeField]
	private float maxEnergyTime = 4f;

	[SerializeField]
	private float maxDoorHandleTime = 1f;

	[SerializeField]
	private Vector3 defaultCameraHolderRotation = Vector3.zero;

	[SerializeField]
	private GameObject PPLayerObject;

	[SerializeField]
	private InteractionHook braceInteractionTrigger;

	[SerializeField]
	private Transform doorHandle;

	private float currentDoorHandleValue;

	private float currentEnergy;

	private Vector3 defaultPOS;

	private float lastBraceValue;

	public CustomEvent PlayerEnteredEvent = new CustomEvent(2);

	public CustomEvent PlayerLeftEvent = new CustomEvent(2);

	private bool playerRanOutOfEnergy;

	private DepthOfField ppDOF;

	private PostProcessVolume ppVol;

	public bool BracingDoor { get; private set; }

	protected new void Awake()
	{
		Ins = this;
		base.Awake();
		ControllerManager.Add(this);
		defaultPOS = base.transform.position;
		currentEnergy = maxEnergyTime;
		currentDoorHandleValue = 0f;
		BracingDoor = false;
		ppDOF = ScriptableObject.CreateInstance<DepthOfField>();
		ppDOF.focusDistance.Override(0.34f);
		ppDOF.aperture.Override(20.4f);
		ppDOF.focalLength.Override(30f);
		ppVol = PostProcessManager.instance.QuickVolume(PPLayerObject.layer, 100f, ppDOF);
		ppVol.weight = 0f;
		PostStage.Event += postBaseStage;
		PostLive.Event += postBaseLive;
		braceInteractionTrigger.LeftAxisAction += playerIsBracingDoor;
	}

	protected new void Start()
	{
		base.Start();
	}

	protected new void Update()
	{
		base.Update();
		getInput();
		if (Active)
		{
			if (BracingDoor)
			{
				currentEnergy = Mathf.MoveTowards(currentEnergy, 0f, Time.deltaTime);
				currentDoorHandleValue = Mathf.MoveTowards(currentDoorHandleValue, maxDoorHandleTime, Time.deltaTime);
			}
			else
			{
				currentEnergy = Mathf.MoveTowards(currentEnergy, maxEnergyTime, Time.deltaTime);
				currentDoorHandleValue = Mathf.MoveTowards(currentDoorHandleValue, 0f, Time.deltaTime);
			}
			LookUp.PlayerUI.EBarFill.fillAmount = currentEnergy / maxEnergyTime;
			doorHandle.transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, new Vector3(0f, 0f, 20f), currentDoorHandleValue / maxDoorHandleTime));
		}
	}

	protected new void OnDestroy()
	{
		braceInteractionTrigger.LeftAxisAction -= playerIsBracingDoor;
		ControllerManager.Remove(Controller);
		base.OnDestroy();
	}

	public void LoseControl()
	{
		Active = false;
		SetMasterLock(setLock: true);
		MyState = GAME_CONTROLLER_STATE.LOCKED;
		StateManager.PlayerState = PLAYER_STATE.BUSY;
		DOTween.To(() => ppVol.weight, delegate(float x)
		{
			ppVol.weight = x;
		}, 0f, 0.75f).SetEase(Ease.Linear).OnComplete(delegate
		{
			base.transform.position = defaultPOS;
			base.transform.rotation = Quaternion.Euler(Vector3.zero);
		});
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
		StateManager.PlayerState = PLAYER_STATE.BRACE;
	}

	public void TakeOverFromRoam()
	{
		PlayerEnteredEvent.Execute();
		if (MouseCaptureInit)
		{
			MyMouseCapture.setCameraTargetRot();
			MyMouseCapture.setRotatingObjectTargetRot(defaultCameraHolderRotation);
			MyMouseCapture.setRotatingObjectRotation(defaultCameraHolderRotation);
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
		sequence.Insert(0f, DOTween.To(() => ppVol.weight, delegate(float x)
		{
			ppVol.weight = x;
		}, 1f, 0.75f).SetEase(Ease.Linear));
		sequence.Play();
	}

	private void getInput()
	{
		if (!lockControl)
		{
			float axis = CrossPlatformInputManager.GetAxis("Right");
			Vector3 euler = new Vector3(0f, axis * maxRotationRight, 0f);
			Vector3 position = new Vector3(defaultPOS.x, defaultPOS.y + axis * maxTopPeak, defaultPOS.z + axis * maxRightPeak);
			base.transform.position = position;
			base.transform.rotation = Quaternion.Euler(euler);
			if (axis <= 0.4f && CrossPlatformInputManager.GetButtonDown("RightClick"))
			{
				switchToRoamController();
			}
		}
	}

	private void switchToRoamController()
	{
		PlayerLeftEvent.Execute();
		LoseControl();
		ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).GlobalTakeOver();
	}

	private void playerIsBracingDoor(float SetValue)
	{
		if (SetValue >= lastBraceValue && SetValue != 0f && !playerRanOutOfEnergy)
		{
			BracingDoor = true;
		}
		else
		{
			BracingDoor = false;
		}
		lastBraceValue = SetValue;
	}

	private void postBaseStage()
	{
		PostStage.Event -= postBaseStage;
	}

	private void postBaseLive()
	{
		PostLive.Event -= postBaseLive;
	}
}
