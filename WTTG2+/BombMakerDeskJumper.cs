using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(deskController))]
public class BombMakerDeskJumper : MonoBehaviour
{
	public static BombMakerDeskJumper Ins;

	public deskController myDeskController;

	private void Awake()
	{
		Ins = this;
		myDeskController = GetComponent<deskController>();
	}

	private void OnDestroy()
	{
		HitmanComputerJumper.Ins.myComputerController.LeaveEvents.Event -= TriggerJump;
		HitmanComputerJumper.Ins.myComputerController.LeaveEvents.Event -= TriggerLobbyLucas;
		Ins = null;
	}

	public void AddComputerJump()
	{
		HitmanComputerJumper.Ins.myComputerController.LeaveEvents.Event += TriggerJump;
	}

	public void TriggerJump()
	{
		HitmanComputerJumper.Ins.myComputerController.LeaveEvents.Event -= TriggerJump;
		DataManager.LockSave = true;
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		myDeskController.LockRecovery();
		myDeskController.SetMasterLock(setLock: true);
		GameObject gameObject = Object.Instantiate(CustomObjectLookUp.BombMakerPCKill);
		GameObject.Find("SuitGEO01").GetComponent<SkinnedMeshRenderer>().material = BombMakerRecolorer.chosenMat;
		gameObject.transform.position = new Vector3(3.3f, 39.675f, -2.794f);
		gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, -180f, 0f));
		gameObject.GetComponent<BombMakerYoureUseless>().StagePCKill();
		DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, new Vector3(0f, -90f, 0f), 0.25f).SetEase(Ease.Linear).SetOptions()
			.OnComplete(delegate
			{
				MainCameraHook.Ins.TriggerHitManJump();
				GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.JumpHit1);
				GameManager.TimeSlinger.FireTimer(3.05f, delegate
				{
					DataManager.ClearGameData();
					MainCameraHook.Ins.ClearARF();
					UIManager.TriggerHardGameOver("YOU WERE USELESS TO THE BOMB MAKER");
				});
			});
	}

	public void StageLobbyLucas()
	{
		HitmanComputerJumper.Ins.myComputerController.LeaveEvents.Event += TriggerLobbyLucas;
	}

	public void TriggerLobbyLucas()
	{
		HitmanComputerJumper.Ins.myComputerController.LeaveEvents.Event -= TriggerLobbyLucas;
		DataManager.LockSave = true;
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		myDeskController.LockRecovery();
		myDeskController.SetMasterLock(setLock: true);
		EnemyManager.HitManManager.ExecuteLobbyComputerJump();
		DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, new Vector3(0f, -180f, 0f), 0.25f).SetEase(Ease.Linear).SetOptions();
	}

	public void KidnapperRotator()
	{
		DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, new Vector3(0f, -105f, 0f), 0.25f).SetEase(Ease.Linear).SetOptions();
	}

	public void KidnapperDeRotator()
	{
		float duration = 0.7f;
		base.transform.DORotate(new Vector3(3.6f, -40f, -25f), duration).SetEase(Ease.OutExpo);
	}

	public void EXERotator()
	{
		DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, new Vector3(0f, -170f, 10f), 0.25f).SetEase(Ease.Linear).SetOptions();
	}

	public void BreatherRotator()
	{
		DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, new Vector3(0f, -180f, 0f), 0.25f).SetEase(Ease.Linear).SetOptions();
	}
}
