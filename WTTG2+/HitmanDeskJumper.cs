using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(deskController))]
public class HitmanDeskJumper : MonoBehaviour
{
	public static HitmanDeskJumper Ins;

	private deskController myDeskController;

	private void Awake()
	{
		Ins = this;
		myDeskController = GetComponent<deskController>();
	}

	public void TriggerJump()
	{
		DataManager.LockSave = true;
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		myDeskController.LockRecovery();
		myDeskController.SetMasterLock(setLock: true);
		HitmanBehaviour.Ins.Spawn(new Vector3(3.227f, 39.565f, -2.661f), new Vector3(0f, -180f, 0f));
		HitmanBehaviour.Ins.ActivateGunMesh();
		HitmanBehaviour.Ins.TriggerAnim("deskJumpIdle");
		HitmanBehaviour.Ins.GunFlashDoneEvents.Event += EnemyManager.HitManManager.GunFlashGameOver;
		DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, new Vector3(0f, -90f, 0f), 0.25f).SetEase(Ease.Linear).SetOptions()
			.OnComplete(delegate
			{
				HitmanBehaviour.Ins.TriggerAnim("deskJump");
				MainCameraHook.Ins.TriggerHitManJump();
				GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.JumpHit3);
			});
	}
}
