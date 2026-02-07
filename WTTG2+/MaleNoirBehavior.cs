using DG.Tweening;
using UnityEngine;

public class MaleNoirBehavior : MonoBehaviour
{
	public static MaleNoirBehavior Ins;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private Transform cameraBone;

	private bool doorJumpStaged;

	private bool behindJumpStaged;

	private void Awake()
	{
		Ins = this;
	}

	private void OnDestroy()
	{
		LookUp.Doors.Door8.DoorOpenEvent.RemoveListener(Trigger8thFloorHallwayJump);
	}

	public void StageBehindJump()
	{
		if (!behindJumpStaged)
		{
			Debug.Log("[Male Noir] Staged Behind Jump");
			behindJumpStaged = true;
			animator.SetBool("StageBehindJump", value: true);
			base.transform.position = roamController.Ins.transform.position + roamController.Ins.transform.forward * -1f - new Vector3(0f, 0.95f, 0f);
			Vector3 eulerAngles = Quaternion.LookRotation(roamController.Ins.transform.position - base.transform.position).eulerAngles;
			eulerAngles.x = 0f;
			eulerAngles.z = 0f;
			base.transform.rotation = Quaternion.Euler(eulerAngles);
			GameManager.TimeSlinger.FireTimer(0.1f, TriggerBehindJump);
		}
	}

	public void TriggerBehindJump()
	{
		GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.MaleNoirJump);
		GameObject.Find("SM_NoirMale.mo").GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen = true;
		animator.SetBool("BehindJump", value: true);
		roamController.Ins.LoseControl();
		PauseManager.LockPause();
		FlashLightBehaviour.Ins.LockOut();
		GameManager.BehaviourManager.CrossHairBehaviour.HideCrossHairGroup();
		CameraManager.Get(roamController.Ins.CameraIControl, out var ReturnCamera);
		Transform transform = ReturnCamera.transform;
		transform.SetParent(cameraBone);
		transform.DOLocalMove(new Vector3(0f, 0f, 0f), 0.2f).SetEase(Ease.Linear);
		transform.DOLocalRotate(new Vector3(0f, 180f, 0f), 0.2f).SetEase(Ease.Linear);
		GameManager.TimeSlinger.FireTimer(5.75f, delegate
		{
			MainCameraHook.Ins.ClearARF(0.45f);
		});
		GameManager.TimeSlinger.FireTimer(6f, delegate
		{
			UIManager.TriggerGameOver("KILLED");
		});
		GameManager.TimeSlinger.FireTimer(0.43f, TriggerPunch);
		GameManager.TimeSlinger.FireTimer(1.1f, TriggerPunch);
		GameManager.TimeSlinger.FireTimer(2.866f, TriggerKick);
	}

	private void TriggerPunch()
	{
		GameManager.TimeSlinger.FireTimer(0.2f, delegate
		{
			MainCameraHook.Ins.AddHeadHit();
		});
		GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.punch);
	}

	private void TriggerKick()
	{
		GameManager.TimeSlinger.FireTimer(0.2f, delegate
		{
			MainCameraHook.Ins.AddHeadHit(2f);
		});
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.BodyHit);
	}

	public void Stage8thFloorHallwayJump()
	{
		if (!doorJumpStaged)
		{
			doorJumpStaged = true;
			base.transform.position = new Vector3(24.3516f, 39.6f, -6.2854f);
			base.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
			animator.SetBool("StageBehindDoorJump", value: true);
			LookUp.Doors.Door8.DoorOpenEvent.AddListener(Trigger8thFloorHallwayJump);
		}
	}

	private void Trigger8thFloorHallwayJump()
	{
		roamController.Ins.LockFromDoorRecovry();
		GameManager.TimeSlinger.FireTimer(1f, delegate
		{
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.MaleNoirJump);
			GameObject.Find("SM_NoirMale.mo").GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen = true;
			animator.SetBool("BehindDoorJump", value: true);
			PauseManager.LockPause();
			FlashLightBehaviour.Ins.LockOut();
			GameManager.BehaviourManager.CrossHairBehaviour.HideCrossHairGroup();
			roamController.Ins.SetMasterLock(setLock: true);
			CameraManager.Get(roamController.Ins.CameraIControl, out var ReturnCamera);
			Transform transform = ReturnCamera.transform;
			transform.SetParent(cameraBone);
			transform.DOLocalMove(new Vector3(0f, 0f, 0f), 0.2f).SetEase(Ease.Linear);
			transform.DOLocalRotate(new Vector3(0f, 180f, 0f), 0.2f).SetEase(Ease.Linear);
			GameManager.TimeSlinger.FireTimer(5.75f, delegate
			{
				MainCameraHook.Ins.ClearARF(0.45f);
			});
			GameManager.TimeSlinger.FireTimer(6f, delegate
			{
				UIManager.TriggerGameOver("KILLED");
			});
			GameManager.TimeSlinger.FireTimer(0.266f, TriggerPunch);
			GameManager.TimeSlinger.FireTimer(1.76f, TriggerKick);
			GameManager.TimeSlinger.FireTimer(2.94f, TriggerPunch);
			GameManager.TimeSlinger.FireTimer(4f, TriggerKick);
		});
	}
}
