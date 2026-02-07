using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class lobbyComputerController : baseController
{
	[SerializeField]
	private Vector3 cameraPOS;

	[SerializeField]
	private Vector3 cameraROT;

	private DepthOfField tmpDOF;

	private PostProcessVolume tmpPPVol;

	protected new void Awake()
	{
		base.Awake();
		ControllerManager.Add(this);
	}

	protected new void Start()
	{
		base.Start();
		if (Screen.width <= 1300)
		{
			GameObject.Find("UI/UILobbyComputer").GetComponent<CanvasScaler>().scaleFactor = 0.8f;
		}
		else if (Screen.width <= 1400)
		{
			GameObject.Find("UI/UILobbyComputer").GetComponent<CanvasScaler>().scaleFactor = 0.9f;
		}
	}

	protected new void Update()
	{
		base.Update();
		if (!lockControl && CrossPlatformInputManager.GetButtonDown("RightClick"))
		{
			LooseControl();
		}
	}

	protected new void OnDestroy()
	{
		ControllerManager.Remove(Controller);
		if (tmpPPVol != null)
		{
			RuntimeUtilities.DestroyVolume(tmpPPVol, destroyProfile: false);
		}
		else
		{
			Debug.Log("Tried to destroy lobby PP Vol, but it's null");
		}
		base.OnDestroy();
	}

	public void TakeControl()
	{
		DataManager.LockSave = true;
		Active = true;
		SetMasterLock(setLock: false);
		MyState = GAME_CONTROLLER_STATE.IDLE;
		StateManager.PlayerState = PLAYER_STATE.LOBBY_COMPUTER;
		CameraManager.GetCameraHook(CameraIControl).SetMyParent(base.transform);
		tmpDOF = ScriptableObject.CreateInstance<DepthOfField>();
		tmpDOF.enabled.Override(x: true);
		tmpDOF.focusDistance.Override(0.36f);
		tmpDOF.aperture.Override(6.1f);
		tmpDOF.focalLength.Override(36f);
		tmpPPVol = PostProcessManager.instance.QuickVolume(base.gameObject.layer, 100f, tmpDOF);
		tmpPPVol.weight = 0f;
		GameManager.InteractionManager.LockInteraction();
		GameManager.BehaviourManager.CrossHairBehaviour.HideCrossHairGroup();
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => MyCamera.transform.localPosition, delegate(Vector3 x)
		{
			MyCamera.transform.localPosition = x;
		}, cameraPOS, 0.7f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
		{
			MyCamera.transform.localRotation = x;
		}, cameraROT, 0.4f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => tmpPPVol.weight, delegate(float x)
		{
			tmpPPVol.weight = x;
		}, 1f, 0.7f).SetEase(Ease.Linear));
		sequence.Play();
	}

	private void LooseControl()
	{
		Active = false;
		SetMasterLock(setLock: true);
		MyState = GAME_CONTROLLER_STATE.LOCKED;
		StateManager.PlayerState = PLAYER_STATE.BUSY;
		GameManager.InteractionManager.UnLockInteraction();
		GameManager.BehaviourManager.CrossHairBehaviour.ShowCrossHairGroup();
		DOTween.To(() => tmpPPVol.weight, delegate(float x)
		{
			tmpPPVol.weight = x;
		}, 0f, 0.7f).SetEase(Ease.Linear).OnComplete(delegate
		{
			DataManager.LockSave = false;
			RuntimeUtilities.DestroyVolume(tmpPPVol, destroyProfile: false);
		});
		ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).GlobalTakeOver();
	}
}
