using DG.Tweening;
using UnityEngine;

public class DollMakerDeskJumper : MonoBehaviour
{
	public static DollMakerDeskJumper Ins;

	private Camera myCamera;

	private deskController myDeskController;

	private void Awake()
	{
		Ins = this;
		myDeskController = GetComponent<deskController>();
		CameraManager.Get(CAMERA_ID.MAIN, out myCamera);
	}

	public void TriggerDeskJump()
	{
		myDeskController.LockRecovery();
		myDeskController.SetMasterLock(setLock: true);
		GameManager.TimeSlinger.FireTimer(0.1f, delegate
		{
			myCamera.transform.SetParent(DollMakerBehaviour.Ins.HelperBone);
			Sequence sequence = DOTween.Sequence();
			sequence.Insert(0.1f, DOTween.To(() => myCamera.transform.localPosition, delegate(Vector3 x)
			{
				myCamera.transform.localPosition = x;
			}, Vector3.zero, 0.5f).SetEase(Ease.Linear));
			sequence.Insert(0.1f, DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
			{
				myCamera.transform.localRotation = x;
			}, Vector3.zero, 0.5f).SetEase(Ease.Linear).SetOptions());
			sequence.Play();
		});
	}
}
