using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(deskController))]
public class BombMakerDeskPresence : MonoBehaviour
{
	public static BombMakerDeskPresence Ins;

	private deskController myDeskController;

	private void Awake()
	{
		Ins = this;
		myDeskController = GetComponent<deskController>();
	}

	private void OnDestroy()
	{
		HitmanComputerJumper.Ins.myComputerController.LeaveEvents.Event -= TriggerPresence;
		Ins = null;
	}

	public void AddComputerPresence()
	{
		CamHookBehaviour.Interruptions = true;
		CamHookBehaviour.SwitchCameraStatus(enabled: false);
		doorlogBehaviour.MayAddDoorlog("Apartment 803", mode: true);
		HitmanComputerJumper.Ins.myComputerController.LeaveEvents.Event += TriggerPresence;
	}

	public void TriggerPresence()
	{
		HitmanComputerJumper.Ins.myComputerController.LeaveEvents.Event -= TriggerPresence;
		BombMakerManager.scheduledAutoLeave = false;
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		myDeskController.LockRecovery();
		myDeskController.SetMasterLock(setLock: true);
		GameObject gameObject = Object.Instantiate(CustomObjectLookUp.BombMakerPresence);
		GameObject.Find("SuitGEO01").GetComponent<SkinnedMeshRenderer>().material = BombMakerRecolorer.chosenMat;
		gameObject.transform.position = new Vector3(2.95f, 39.545f, -4.264f);
		gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
		BombMakerPresenceGunJump component = gameObject.GetComponent<BombMakerPresenceGunJump>();
		component.hub.PlaySoundCustomDelay(CustomSoundLookUp.bombmakertalk, 2f);
		DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, new Vector3(0f, 60f, 0f), 0.25f).SetEase(Ease.Linear).SetOptions()
			.OnComplete(delegate
			{
				component.ArmAppear();
				GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.JumpHit1);
			});
		GameManager.TimeSlinger.FireTimer(9f, component.RandGunShake, 5);
		GameManager.TimeSlinger.FireTimer(51f, delegate
		{
			Object.Destroy(gameObject);
		});
		GameManager.TimeSlinger.FireTimer(51f, UnlockController);
		GameManager.TimeSlinger.FireTimer(52f, LookUp.Doors.MainDoor.ForceOpenDoor);
		GameManager.TimeSlinger.FireTimer(54f, EnemyManager.BombMakerManager.ClearPresenceState);
	}

	private void UnlockController()
	{
		PauseManager.UnLockPause();
		GameManager.BehaviourManager.CrossHairBehaviour.ShowCrossHairGroup();
		GameManager.InteractionManager.UnLockInteraction();
		myDeskController.SetMasterLock(setLock: false);
		myDeskController.UnLockRecovery();
		StateManager.PlayerState = PLAYER_STATE.DESK;
		DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, new Vector3(0f, 0f, 0f), 0.25f).SetEase(Ease.Linear).SetOptions();
		GameManager.TimeSlinger.FireTimer(3f, delegate
		{
			CamHookBehaviour.Interruptions = false;
			CamHookBehaviour.SwitchCameraStatus(enabled: true);
		});
	}
}
