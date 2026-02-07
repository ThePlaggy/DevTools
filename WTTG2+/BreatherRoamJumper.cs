using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class BreatherRoamJumper : MonoBehaviour
{
	public static BreatherRoamJumper Ins;

	[SerializeField]
	private GameObject PPLayerObject;

	private Camera myCamera;

	private roamController myRoamController;

	private PostProcessVolume ppVol;

	private void Awake()
	{
		Ins = this;
		myRoamController = GetComponent<roamController>();
		CameraManager.Get(CAMERA_ID.MAIN, out myCamera);
	}

	public void StagePickUpJump()
	{
		GameManager.TimeSlinger.FireTimer(0.35f, delegate
		{
			myRoamController.SetMasterLock(setLock: true);
			DOTween.To(() => myRoamController.transform.rotation, delegate(Quaternion x)
			{
				myRoamController.transform.rotation = x;
			}, new Vector3(0f, 270f, 0f), 0.35f).SetEase(Ease.Linear).SetOptions()
				.OnComplete(delegate
				{
					roamController.Ins.MyMouseCapture.setRotatingObjectTargetRot(new Vector3(0f, 270f, 0f));
					myRoamController.SetMasterLock(setLock: false);
				});
		});
	}

	public void TriggerExitRushJump()
	{
		myRoamController.SetMasterLock(setLock: true);
		myCamera.transform.SetParent(BreatherBehaviour.Ins.HelperBone);
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localPosition, delegate(Vector3 x)
		{
			myCamera.transform.localPosition = x;
		}, Vector3.zero, 0.5f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
		{
			myCamera.transform.localRotation = x;
		}, new Vector3(0f, 90f, 0f), 0.5f).SetOptions().SetEase(Ease.Linear));
		sequence.Play();
	}

	public void TriggerDumpsterJump()
	{
		myRoamController.SetMasterLock(setLock: true);
		myCamera.transform.SetParent(BreatherBehaviour.Ins.HelperBone);
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localPosition, delegate(Vector3 x)
		{
			myCamera.transform.localPosition = x;
		}, Vector3.zero, 0.5f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
		{
			myCamera.transform.localRotation = x;
		}, new Vector3(0f, 90f, 0f), 0.1f).SetOptions().SetEase(Ease.Linear));
		sequence.Play();
	}

	public void TriggerPeekABooJump()
	{
		myRoamController.SetMasterLock(setLock: true);
		myCamera.transform.SetParent(BreatherBehaviour.Ins.HelperBone);
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localPosition, delegate(Vector3 x)
		{
			myCamera.transform.localPosition = x;
		}, Vector3.zero, 0.4f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
		{
			myCamera.transform.localRotation = x;
		}, new Vector3(0f, 90f, 0f), 0.25f).SetOptions().SetEase(Ease.Linear));
		sequence.Play();
	}
}
