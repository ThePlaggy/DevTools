using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(hideController))]
public class HitmanHideJumper : MonoBehaviour
{
	public static HitmanHideJumper Ins;

	private Camera myCamera;

	private hideController myHideController;

	private void Awake()
	{
		Ins = this;
		myHideController = GetComponent<hideController>();
		GameManager.StageManager.Stage += stageMe;
	}

	public void WardrobeJump()
	{
		myHideController.SetMasterLock(setLock: true);
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
		{
			myCamera.transform.localRotation = x;
		}, Vector3.zero, 0.25f).SetEase(Ease.Linear).SetOptions());
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localPosition, delegate(Vector3 x)
		{
			myCamera.transform.localPosition = x;
		}, new Vector3(0f, 0f, 0.218f), 0.3f).SetEase(Ease.Linear));
		sequence.Play();
	}

	public void ShowerJump()
	{
		myHideController.SetMasterLock(setLock: true);
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
		{
			myCamera.transform.localRotation = x;
		}, Vector3.zero, 0.25f).SetEase(Ease.Linear).SetOptions());
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localPosition, delegate(Vector3 x)
		{
			myCamera.transform.localPosition = x;
		}, new Vector3(0f, 0f, 0.218f), 0.3f).SetEase(Ease.Linear));
		sequence.Play();
	}

	public void ShowerCaughtJump()
	{
		GameManager.TimeSlinger.FireTimer(0.75f, delegate
		{
			DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
			{
				base.transform.rotation = x;
			}, new Vector3(0f, -18.88f, 0f), 0.3f).SetEase(Ease.Linear).SetOptions();
			myHideController.SetMasterLock(setLock: true);
		});
	}

	private void stageMe()
	{
		CameraManager.Get(myHideController.CameraIControl, out myCamera);
		GameManager.StageManager.Stage -= stageMe;
	}
}
