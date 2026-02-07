using DG.Tweening;
using UnityEngine;

public class CultDeskJumper : MonoBehaviour
{
	public static CultDeskJumper Ins;

	private Camera myCamera;

	private deskController myDeskController;

	private void Awake()
	{
		Ins = this;
		myDeskController = GetComponent<deskController>();
		CameraManager.Get(CAMERA_ID.MAIN, out myCamera);
	}

	public void TriggerLightsOffJump()
	{
		DataManager.LockSave = true;
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		myDeskController.LockRecovery();
		myDeskController.SetMasterLock(setLock: true);
		Sequence sequence = DOTween.Sequence().OnComplete(delegate
		{
			myCamera.transform.SetParent(CultFemaleBehaviour.Ins.CameraBone);
			myCamera.transform.localPosition = Vector3.zero;
			EnemyManager.CultManager.TriggerDeskJump();
		});
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
		{
			myCamera.transform.localRotation = x;
		}, new Vector3(0f, -90f, 0f), 0.25f).SetEase(Ease.Linear).SetOptions());
		sequence.Play();
	}
}
