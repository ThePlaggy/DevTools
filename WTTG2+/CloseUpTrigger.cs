using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(InteractionHook))]
public class CloseUpTrigger : MonoBehaviour
{
	[SerializeField]
	private CAMERA_ID cameraIControl;

	[SerializeField]
	private Vector3 CameraTargetPOS;

	[SerializeField]
	private Vector3 CameraTargetROT;

	[SerializeField]
	private bool disableDOF;

	[SerializeField]
	private GameObject PPObject;

	[HideInInspector]
	public bool amIKeypad;

	[NonSerialized]
	public bool nvmNotKeypad;

	private bool interactLock;

	private bool isCloseUp;

	private Transform lastCameraParent;

	private Vector3 lastCameraPOS = Vector3.zero;

	private Vector3 lastCameraROT = Vector3.zero;

	private Camera myCamera;

	private CameraHook myCameraHook;

	private InteractionHook myInteractionHook;

	private DepthOfField tmpDOF;

	private PostProcessVolume tmpPPVol;

	public InteractionHook getInteractionHook => myInteractionHook;

	public void setNewCameraPos(Vector3 pos)
	{
		CameraTargetPOS = pos;
	}

	private void Awake()
	{
		myInteractionHook = GetComponent<InteractionHook>();
		myInteractionHook.LeftClickAction += goClose;
		GameManager.StageManager.Stage += stageMe;
	}

	private void Update()
	{
		if (isCloseUp && CrossPlatformInputManager.GetButtonDown("RightClick"))
		{
			leaveClose();
		}
	}

	private void OnDestroy()
	{
		if (amIKeypad)
		{
			if (GameObject.Find("NoDOF") != null)
			{
				GameObject.Find("NoDOF").GetComponent<PostProcessVolume>().isGlobal = false;
			}
			else
			{
				Debug.Log("Tried to destroy keypad PP Vol, but it's null");
			}
		}
		myInteractionHook.LeftClickAction -= goClose;
	}

	private void goClose()
	{
		if (interactLock)
		{
			return;
		}
		interactLock = true;
		lastCameraPOS = myCamera.transform.localPosition;
		lastCameraROT = myCamera.transform.localRotation.eulerAngles;
		lastCameraParent = myCameraHook.CurrentParent;
		if (disableDOF)
		{
			tmpDOF = ScriptableObject.CreateInstance<DepthOfField>();
			tmpDOF.enabled.Override(x: false);
			tmpPPVol = PostProcessManager.instance.QuickVolume(PPObject.layer, 100f, tmpDOF);
			tmpPPVol.weight = 1f;
		}
		if (amIKeypad)
		{
			GameObject.Find("NoDOF").GetComponent<PostProcessVolume>().isGlobal = true;
			CursorManager.Ins.SetOverwrite(setValue: true);
			if (!nvmNotKeypad)
			{
				KeypadManager.ChangekeypadRCState(state: true);
			}
			else
			{
				GameManager.TimeSlinger.FireTimer(0.5f, delegate
				{
					if (CustomElevatorManager.Ins != null)
					{
						CustomElevatorManager.Ins.canvasRef.SetActive(value: true);
					}
				});
			}
		}
		ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).SetMasterLock(setLock: true);
		GameManager.InteractionManager.LockInteraction();
		GameManager.BehaviourManager.CrossHairBehaviour.HideCrossHairGroup();
		myCamera.transform.SetParent(base.transform);
		DOTween.To(() => myCamera.transform.rotation, delegate(Quaternion x)
		{
			myCamera.transform.rotation = x;
		}, CameraTargetROT, 0.5f).SetEase(Ease.Linear).SetOptions();
		DOTween.To(() => myCamera.transform.position, delegate(Vector3 x)
		{
			myCamera.transform.position = x;
		}, CameraTargetPOS, 0.75f).SetEase(Ease.Linear).OnComplete(delegate
		{
			interactLock = false;
			isCloseUp = true;
		});
	}

	public void MeLeaveClose()
	{
		leaveClose();
	}

	private void leaveClose()
	{
		if (interactLock)
		{
			return;
		}
		isCloseUp = false;
		interactLock = true;
		myCamera.transform.SetParent(lastCameraParent);
		if (amIKeypad)
		{
			GameObject.Find("NoDOF").GetComponent<PostProcessVolume>().isGlobal = false;
			CursorManager.Ins.SetOverwrite(setValue: false);
			if (!nvmNotKeypad)
			{
				KeypadManager.ChangekeypadRCState(state: false);
			}
			else
			{
				GameManager.TimeSlinger.FireTimer(0.5f, delegate
				{
					if (CustomElevatorManager.Ins != null)
					{
						CustomElevatorManager.Ins.canvasRef.SetActive(value: false);
					}
				});
			}
		}
		DOTween.To(() => myCamera.transform.localPosition, delegate(Vector3 x)
		{
			myCamera.transform.localPosition = x;
		}, roamController.Ins.DefaultCameraPOS, 0.75f).SetEase(Ease.Linear).OnComplete(delegate
		{
			if (disableDOF)
			{
				RuntimeUtilities.DestroyVolume(tmpPPVol, destroyProfile: false);
			}
			GameManager.InteractionManager.UnLockInteraction();
			GameManager.BehaviourManager.CrossHairBehaviour.ShowCrossHairGroup();
			ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).SetMasterLock(setLock: false);
			GameManager.TimeSlinger.FireTimer(amIKeypad ? 0.2f : 1f, delegate
			{
				interactLock = false;
			});
		});
		DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
		{
			myCamera.transform.localRotation = x;
		}, lastCameraROT, 0.5f).SetEase(Ease.Linear).SetOptions();
	}

	private void stageMe()
	{
		CameraManager.Get(cameraIControl, out myCamera);
		myCameraHook = CameraManager.GetCameraHook(cameraIControl);
		GameManager.StageManager.Stage -= stageMe;
	}

	public void SetKeypad(Vector3 Pos, bool unknown = false)
	{
		CameraManager.Get(cameraIControl, out myCamera);
		myCameraHook = CameraManager.GetCameraHook(cameraIControl);
		amIKeypad = true;
		CameraTargetPOS = Pos;
		if (unknown)
		{
			myInteractionHook.AllowMultiStates = true;
			myInteractionHook.MultiStatesActive = new PLAYER_STATE[3]
			{
				PLAYER_STATE.ROAMING,
				PLAYER_STATE.REMOTE_VPN_PLACEMENT,
				PLAYER_STATE.MOTION_SENSOR_PLACEMENT
			};
			myInteractionHook.LocationToCheck = PLAYER_LOCATION.UNKNOWN;
			myInteractionHook.RequireLocationCheck = false;
			GetComponent<InteractionDisplayHook>().leftClickLocation = PLAYER_LOCATION.UNKNOWN;
			GetComponent<InteractionDisplayHook>().requireLocationCheckForLeftClick = false;
			nvmNotKeypad = true;
		}
	}
}
