using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(InteractionHook))]
[RequireComponent(typeof(AudioHubObject))]
[RequireComponent(typeof(BoxCollider))]
public class DoorTrigger : MonoBehaviour
{
	public bool MakeManual;

	public bool NoClip;

	public MeshCollider NoClipMesh;

	public DistanceLOD[] SetDistanceLODs;

	[SerializeField]
	private float updateDelay = 0.1f;

	[SerializeField]
	private float closeDistance = 1f;

	[SerializeField]
	private Transform moveMeToTrans;

	public bool DoorLockable;

	public bool CheckPlayerIsBlockingDoorPath;

	public LayerMask PlayerMask;

	public Vector3 CheckForPlayerFirePOS = Vector3.zero;

	public float SphereCastRadius = 0.2f;

	public float SphereCastDistance = 0.2f;

	public float PlayerLookAtDirection;

	public Transform DoorTransform;

	public Transform DoorMeshTransform;

	public AudioFileDefinition OpenSFX;

	public AudioFileDefinition CloseSFX;

	public AudioFileDefinition LockDoorSFX;

	public AudioFileDefinition UnLockDoorSFX;

	public AudioFileDefinition DoorIsLockedSFX;

	public UnityEvent DoneWithAnimationEvents;

	public UnityEvent DoorWasOpenedEvent;

	public UnityEvent DoorWasClosedEvent;

	public UnityEvent DoorOpenEvent;

	public UnityEvent DoorCloseEvent;

	[NonSerialized]
	public bool dollmakerHeadOn;

	private bool amBusy;

	private Timer autoCloseTimer;

	private bool checkColliderRange;

	private bool checkDoorCloseRange;

	private List<DOTweenAnimation> doorCloseAnimations = new List<DOTweenAnimation>(5);

	private DOTweenAnimation doorLockAnimation;

	private Bounds doorMeshBounds;

	private List<DOTweenAnimation> doorOpenAnimations = new List<DOTweenAnimation>(5);

	private DOTweenAnimation doorUnLockAnimation;

	private bool fireCheckForPlayer;

	private bool lucasDoor;

	private Camera mainCamera;

	private InteractionHook myInteractionHook;

	private float playersCurrentDistance;

	public bool Locked { get; private set; }

	public AudioHubObject AudioHub { get; private set; }

	public bool DoingSomething => amBusy || IsOpen;

	public bool IsBusy => amBusy;

	public bool LockOutAutoClose { get; set; }

	public bool IsOpen { get; private set; }

	private void Awake()
	{
		myInteractionHook = GetComponent<InteractionHook>();
		AudioHub = GetComponent<AudioHubObject>();
		if (!MakeManual)
		{
			myInteractionHook.LeftClickAction += leftClickAction;
			myInteractionHook.RightClickAction += rightClickAction;
		}
		doorMeshBounds = NoClipMesh.gameObject.GetComponent<Renderer>().bounds;
		doorMeshBounds.size *= 1.5f;
		lucasDoor = false;
		if (base.transform.parent.gameObject.name == "doorApartment" || base.transform.parent.gameObject.name == "Floor8Door" || base.transform.parent.gameObject.name == "Floor1Door")
		{
			myInteractionHook.MultiStatesActive = new PLAYER_STATE[4]
			{
				PLAYER_STATE.ROAMING,
				PLAYER_STATE.MOTION_SENSOR_PLACEMENT,
				PLAYER_STATE.REMOTE_VPN_PLACEMENT,
				PLAYER_STATE.WIFI_DONGLE_PLACEMENT
			};
		}
		if (base.transform.parent.gameObject.name == "BalcondyDoor")
		{
			myInteractionHook.MultiStatesActive = new PLAYER_STATE[3]
			{
				PLAYER_STATE.ROAMING,
				PLAYER_STATE.MOTION_SENSOR_PLACEMENT,
				PLAYER_STATE.REMOTE_VPN_PLACEMENT
			};
		}
	}

	private void Start()
	{
		mainCamera = Camera.main;
		if (MakeManual)
		{
			myInteractionHook.ForceLock = true;
		}
	}

	private void Update()
	{
		if (!MakeManual)
		{
			myInteractionHook.ForceLock = amBusy;
		}
		if (checkDoorCloseRange && playersCurrentDistance > closeDistance)
		{
			checkDoorCloseRange = false;
			if (!lucasDoor)
			{
				closeTheDoor();
			}
		}
		if (checkColliderRange)
		{
			if (!doorMeshBounds.Contains(mainCamera.transform.position))
			{
				NoClipMesh.enabled = true;
				checkColliderRange = false;
			}
			else
			{
				ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).HardMove(moveMeToTrans.position);
			}
		}
	}

	private void FixedUpdate()
	{
		if (fireCheckForPlayer)
		{
			RaycastHit hitInfo;
			if (Physics.CheckSphere(CheckForPlayerFirePOS + base.transform.forward * SphereCastRadius, SphereCastRadius, PlayerMask.value))
			{
				Vector3 destination = CheckForPlayerFirePOS + base.transform.forward * (SphereCastDistance + SphereCastRadius * 2f);
				ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).MoveOutOfTheWay(PlayerLookAtDirection, destination, CARDINAL_DIR.BACK);
			}
			else if (Physics.SphereCast(CheckForPlayerFirePOS + base.transform.forward * SphereCastRadius, SphereCastRadius, Vector3.forward, out hitInfo, SphereCastDistance, PlayerMask.value))
			{
				Vector3 destination2 = CheckForPlayerFirePOS + base.transform.forward * (SphereCastDistance + SphereCastRadius * 2f);
				ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).MoveOutOfTheWay(PlayerLookAtDirection, destination2, CARDINAL_DIR.BACK);
			}
			fireCheckForPlayer = false;
			openTheDoor();
		}
	}

	private void OnEnable()
	{
		if (!DifficultyManager.HackerMode)
		{
			InvokeRepeating("updatePlayerDistance", 0f, updateDelay);
		}
	}

	private void OnDisable()
	{
		if (!DifficultyManager.HackerMode)
		{
			CancelInvoke("updatePlayerDistance");
		}
	}

	private void OnDestroy()
	{
		if (!MakeManual)
		{
			myInteractionHook.LeftClickAction -= leftClickAction;
			myInteractionHook.RightClickAction -= rightClickAction;
		}
		DoneWithAnimationEvents.RemoveAllListeners();
		DoorWasOpenedEvent.RemoveAllListeners();
		DoorWasClosedEvent.RemoveAllListeners();
		DoorWasOpenedEvent.RemoveAllListeners();
		DoorCloseEvent.RemoveAllListeners();
	}

	private void OnDrawGizmos()
	{
		if (CheckPlayerIsBlockingDoorPath)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(CheckForPlayerFirePOS + base.transform.forward * SphereCastRadius, SphereCastRadius);
			Gizmos.DrawLine(CheckForPlayerFirePOS + base.transform.forward * SphereCastRadius, CheckForPlayerFirePOS + base.transform.forward * (SphereCastDistance + SphereCastRadius));
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(CheckForPlayerFirePOS + base.transform.forward * (SphereCastDistance + SphereCastRadius), SphereCastRadius);
		}
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(doorMeshBounds.center, doorMeshBounds.size);
	}

	public void SetOpenDoorAnimation(DOTweenAnimation SetAnimation)
	{
		if (!doorOpenAnimations.Contains(SetAnimation))
		{
			doorOpenAnimations.Add(SetAnimation);
		}
	}

	public void SetCloseDoorAnimation(DOTweenAnimation SetAnimation)
	{
		if (!doorCloseAnimations.Contains(SetAnimation))
		{
			doorCloseAnimations.Add(SetAnimation);
		}
	}

	public void SetLockDoorAnimation(DOTweenAnimation SetAnimation)
	{
		doorLockAnimation = SetAnimation;
	}

	public void SetUnLockDoorAnimation(DOTweenAnimation SetAnimation)
	{
		doorUnLockAnimation = SetAnimation;
	}

	public void DoneWithAnimation()
	{
		if (IsOpen)
		{
			if (!LockOutAutoClose)
			{
				checkDoorCloseRange = true;
			}
			DoorWasOpenedEvent.Invoke();
		}
		else
		{
			amBusy = false;
			DoorWasClosedEvent.Invoke();
			if (NoClip)
			{
				checkColliderRange = true;
			}
			if (SetDistanceLODs != null)
			{
				for (int i = 0; i < SetDistanceLODs.Length; i++)
				{
					SetDistanceLODs[i].OverwriteCulling = false;
				}
			}
		}
		DoneWithAnimationEvents?.Invoke();
	}

	public void DoneWithLockAnimations()
	{
		if (DoorLockable)
		{
			Locked = !Locked;
			amBusy = false;
			myInteractionHook.MyBoxCollider.enabled = true;
		}
	}

	public void ShowLockState()
	{
		if (Locked)
		{
			UIInteractionManager.Ins.ShowUnLock();
		}
		else
		{
			UIInteractionManager.Ins.ShowLock();
		}
	}

	public void HideLockState()
	{
		if (Locked)
		{
			UIInteractionManager.Ins.HideUnLock();
		}
		else
		{
			UIInteractionManager.Ins.HideLock();
		}
	}

	public void ForceOpenDoor()
	{
		if (CheckPlayerIsBlockingDoorPath)
		{
			fireCheckForPlayer = true;
		}
		else
		{
			openTheDoor();
		}
	}

	public void ForceOpenDoorDisableAutoClose()
	{
		LockOutAutoClose = true;
		if (CheckPlayerIsBlockingDoorPath)
		{
			fireCheckForPlayer = true;
		}
		else
		{
			openTheDoor();
		}
		GameManager.TimeSlinger.FireTimer(120f, delegate
		{
			if (LockOutAutoClose)
			{
				LockOutAutoClose = false;
			}
		});
	}

	public void NPCOpenDoor()
	{
		LockOutAutoClose = true;
		openTheDoor();
		if (dollmakerHeadOn)
		{
			EnemyManager.DollMakerManager.TheMaker.HideMesh();
		}
		GameManager.TimeSlinger.FireTimer(120f, delegate
		{
			if (LockOutAutoClose)
			{
				LockOutAutoClose = false;
			}
		});
	}

	public void ForceDoorClose()
	{
		closeTheDoor();
		LockOutAutoClose = false;
	}

	public void CancelAutoClose()
	{
		lucasDoor = true;
		checkDoorCloseRange = false;
		GameManager.TimeSlinger.FireTimer(120f, delegate
		{
			if (!checkDoorCloseRange)
			{
				checkDoorCloseRange = true;
			}
		});
	}

	public void SetCustomOpenDoorTime(float SetValue)
	{
		for (int i = 0; i < doorOpenAnimations.Count; i++)
		{
			doorOpenAnimations[i].duration = SetValue;
		}
	}

	public void KickDoorOpen()
	{
		if (DoorTransform != null)
		{
			DOTween.To(() => DoorTransform.localRotation, delegate(Quaternion x)
			{
				DoorTransform.localRotation = x;
			}, new Vector3(0f, 90f, 0f), 0.25f).SetEase(Ease.Linear);
		}
	}

	public void DisableDoor()
	{
		amBusy = true;
		myInteractionHook.MyBoxCollider.enabled = false;
		dollmakerHeadOn = false;
	}

	public void EnableDoor()
	{
		amBusy = false;
		myInteractionHook.MyBoxCollider.enabled = true;
	}

	private void leftClickAction()
	{
		if (amBusy)
		{
			return;
		}
		if (DoorLockable && TarotManager.HermitActive)
		{
			AudioHub.PlaySound(Locked ? DoorIsLockedSFX : CustomSoundLookUp.Denied);
			amBusy = true;
			GameManager.TimeSlinger.FireTimer(1f, delegate
			{
				amBusy = false;
			});
		}
		else if (IsOpen)
		{
			closeTheDoor();
		}
		else if (!Locked)
		{
			if (CheckPlayerIsBlockingDoorPath)
			{
				fireCheckForPlayer = true;
			}
			else
			{
				openTheDoor();
			}
		}
		else
		{
			AudioHub.PlaySound(DoorIsLockedSFX);
			amBusy = true;
			GameManager.TimeSlinger.FireTimer(1f, delegate
			{
				amBusy = false;
			});
		}
	}

	private void rightClickAction()
	{
		if (!amBusy && DoorLockable)
		{
			amBusy = true;
			myInteractionHook.MyBoxCollider.enabled = false;
			if (Locked)
			{
				AudioHub.PlaySound(UnLockDoorSFX);
				doorLockAnimation.DORestartById("unlock");
			}
			else
			{
				AudioHub.PlaySound(LockDoorSFX);
				doorLockAnimation.DORestartById("lock");
			}
		}
	}

	private void openTheDoor()
	{
		if (doorOpenAnimations == null)
		{
			return;
		}
		DoorOpenEvent.Invoke();
		AudioHub.PlaySound(OpenSFX);
		doorlogBehaviour.AddDoorlog(this, mode: true);
		for (int i = 0; i < doorOpenAnimations.Count; i++)
		{
			doorOpenAnimations[i].DORestartById("open");
		}
		amBusy = true;
		IsOpen = true;
		if (SetDistanceLODs != null)
		{
			for (int j = 0; j < SetDistanceLODs.Length; j++)
			{
				SetDistanceLODs[j].OverwriteCulling = true;
			}
		}
	}

	private void closeTheDoor()
	{
		if (doorCloseAnimations != null)
		{
			if (NoClip)
			{
				NoClipMesh.enabled = false;
			}
			DoorCloseEvent.Invoke();
			AudioHub.PlaySound(CloseSFX);
			doorlogBehaviour.AddDoorlog(this, mode: false);
			amBusy = true;
			IsOpen = false;
			if (dollmakerHeadOn)
			{
				EnemyManager.DollMakerManager.TheMaker.ShowMesh();
			}
			for (int i = 0; i < doorCloseAnimations.Count; i++)
			{
				doorCloseAnimations[i].DORestartById("close");
			}
		}
	}

	private void updatePlayerDistance()
	{
		playersCurrentDistance = (base.transform.position - mainCamera.transform.position).magnitude;
	}

	[ContextMenu("Reset Sphere")]
	private void resetSphere()
	{
		CheckForPlayerFirePOS = base.transform.position;
	}

	[ContextMenu("Recalculate Door Bounds")]
	private void calcDoorBounds()
	{
		doorMeshBounds = NoClipMesh.gameObject.GetComponent<Renderer>().bounds;
		doorMeshBounds.size *= 1.5f;
	}

	public void ToggleLock()
	{
		rightClickAction();
	}

	public void DisableDoorDollmaker()
	{
		amBusy = true;
		myInteractionHook.MyBoxCollider.enabled = false;
		dollmakerHeadOn = true;
	}

	public void NightmareLock()
	{
		myInteractionHook.MyBoxCollider.enabled = false;
		myInteractionHook.AllowMultiStates = false;
		myInteractionHook.StateActive = PLAYER_STATE.BUSY;
	}
}
