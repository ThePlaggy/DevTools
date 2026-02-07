using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(InteractionHook))]
public class GoHideTrigger : MonoBehaviour
{
	[SerializeField]
	private HotZoneTrigger activeHotZone;

	[SerializeField]
	private Vector3 hideControllerPOS;

	[SerializeField]
	private Vector3 hideControllerROT;

	[SerializeField]
	private Vector3 returnRoamROT;

	[SerializeField]
	private Vector3 peakLocationPOS;

	[SerializeField]
	private UnityEvent preHideEvents;

	[SerializeField]
	private UnityEvent hideEvents;

	[SerializeField]
	private UnityEvent preLeaveEvents;

	[SerializeField]
	private UnityEvent leaveEvents;

	[SerializeField]
	private FloatUnityEvent peakingEvents;

	private bool attemptedLeaveFired;

	private float currentPeakAmount;

	private bool isHiding;

	public CustomEvent LeaveDoomActions = new CustomEvent(2);

	private bool lockInteraction;

	private hideController myHideController;

	private InteractionHook myInteractionHook;

	private roamController myRoamController;

	public CustomEvent StageLeaveDoomActions = new CustomEvent(2);

	public CustomEvent PlayerLeftHidingActions = new CustomEvent(2);

	public bool LockedOut { get; set; }

	public bool LeaveDoom { get; set; }

	private void Awake()
	{
		myInteractionHook = GetComponent<InteractionHook>();
		myInteractionHook.LeftClickAction += hidePlayer;
		GameManager.StageManager.Stage += stageMe;
	}

	private void Update()
	{
		if (!lockInteraction)
		{
			myInteractionHook.ForceLock = !activeHotZone.IsHot;
		}
		if (!LockedOut && isHiding && !attemptedLeaveFired && CrossPlatformInputManager.GetButtonDown("RightClick") && currentPeakAmount == 0f)
		{
			attemptToLeaveHideMode();
		}
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= hidePlayer;
	}

	public void TriggerAction()
	{
		if (isHiding)
		{
			triggerLeave();
		}
		else
		{
			triggerHide();
		}
	}

	public void TriggerHidden()
	{
		isHiding = !isHiding;
		if (isHiding)
		{
			myHideController.FullyHidden = true;
		}
	}

	public void ForceLeave()
	{
		myHideController.FullyHidden = false;
		attemptedLeaveFired = true;
		preLeaveEvents.Invoke();
	}

	public void attemptToLeaveHideMode()
	{
		if (preLeaveEvents != null)
		{
			myHideController.FullyHidden = false;
			attemptedLeaveFired = true;
			if (LeaveDoom)
			{
				StageLeaveDoomActions.Execute();
				GameManager.TimeSlinger.FireTimer(0.2f, preLeaveEvents.Invoke);
			}
			else
			{
				PlayerLeftHidingActions.Execute();
				preLeaveEvents.Invoke();
			}
		}
	}

	public void triggerLeave()
	{
		if (attemptedLeaveFired)
		{
			if (LeaveDoom)
			{
				LeaveDoomActions.Execute();
				return;
			}
			myHideController.LoseControlToRoam(returnRoamROT);
			myHideController.PlayerPeakingEvent.Event -= playerIsPeaking;
			roamController.Ins.TookControlActions.Event += triggerLeaveEvents;
			lockInteraction = false;
			myInteractionHook.ForceLock = false;
		}
	}

	private void hidePlayer()
	{
		if (!LockedOut)
		{
			roamController.Ins.SetMasterLock(setLock: true);
			myHideController.SetMasterLock(setLock: true);
			DataManager.LockSave = true;
			lockInteraction = true;
			myInteractionHook.ForceLock = true;
			if (preHideEvents != null)
			{
				preHideEvents.Invoke();
			}
		}
	}

	private void triggerHide()
	{
		LockedOut = true;
		myHideController.SetMasterLock(setLock: true);
		myHideController.PutMe(hideControllerPOS, hideControllerROT);
		myHideController.PeakPOS = peakLocationPOS;
		myHideController.PlayerPeakingEvent.Event += playerIsPeaking;
		myHideController.TakeOverFromRoam(delegate
		{
			hideEvents.Invoke();
			myRoamController.PutMe(myRoamController.transform.position, myRoamController.transform.rotation.eulerAngles, RememberLastLocation: true);
			attemptedLeaveFired = false;
			GameManager.TimeSlinger.FireTimer(1f, delegate
			{
				LockedOut = false;
			});
		}, this);
	}

	private void playerIsPeaking(float PeakAmount)
	{
		currentPeakAmount = PeakAmount;
		if (peakingEvents != null)
		{
			peakingEvents.Invoke(PeakAmount);
		}
	}

	private void triggerLeaveEvents()
	{
		DataManager.LockSave = false;
		leaveEvents.Invoke();
		roamController.Ins.TookControlActions.Event -= triggerLeaveEvents;
	}

	private void stageMe()
	{
		myHideController = ControllerManager.Get<hideController>(GAME_CONTROLLER.HIDE);
		myRoamController = ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM);
		GameManager.StageManager.Stage -= stageMe;
	}
}
