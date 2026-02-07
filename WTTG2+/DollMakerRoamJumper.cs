using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DollMakerRoamJumper : MonoBehaviour
{
	public static DollMakerRoamJumper Ins;

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

	public void TriggerSpeechJump()
	{
		myRoamController.SetMasterLock(setLock: true);
		GameManager.TimeSlinger.FireTimer(0.1f, delegate
		{
			myCamera.transform.SetParent(DollMakerBehaviour.Ins.HelperBone);
			DollMakerBehaviour.Ins.TriggerSpeech();
			DepthOfField depthOfField = ScriptableObject.CreateInstance<DepthOfField>();
			depthOfField.enabled.Override(x: true);
			depthOfField.focusDistance.Override(0.13f);
			depthOfField.aperture.Override(8.9f);
			depthOfField.focalLength.Override(13f);
			ppVol = PostProcessManager.instance.QuickVolume(PPLayerObject.layer, 100f, depthOfField);
			ppVol.weight = 0f;
			Sequence sequence = DOTween.Sequence();
			sequence.Insert(0f, DOTween.To(() => myCamera.transform.localPosition, delegate(Vector3 x)
			{
				myCamera.transform.localPosition = x;
			}, Vector3.zero, 0.5f).SetEase(Ease.OutCirc));
			sequence.Insert(0f, DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
			{
				myCamera.transform.localRotation = x;
			}, Vector3.zero, 0.5f).SetEase(Ease.Linear).SetOptions());
			sequence.Insert(0.75f, DOTween.To(() => ppVol.weight, delegate(float x)
			{
				ppVol.weight = x;
			}, 1f, 0.5f).SetEase(Ease.Linear));
			sequence.Play();
		});
	}

	public void ClearSpeechJump()
	{
		MainCameraHook.Ins.BlackOut(1f, 3f);
		MainCameraHook.Ins.AddHeadHit(3f);
		RuntimeUtilities.DestroyVolume(ppVol, destroyProfile: false);
		GameManager.TimeSlinger.FireTimer(1.5f, delegate
		{
			MainCameraHook.Ins.ClearARF(2.75f);
			MainCameraHook.Ins.ClearDoubleVis(2.5f);
		});
		myRoamController.GlobalTakeOver(2f, 1f, 1f);
	}

	public void TriggerUniJump()
	{
		myRoamController.SetMasterLock(setLock: true);
		myCamera.transform.SetParent(DollMakerBehaviour.Ins.HelperBone);
		DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
		{
			myCamera.transform.localRotation = x;
		}, Vector3.zero, 0.5f).SetEase(Ease.Linear).SetOptions();
	}
}
