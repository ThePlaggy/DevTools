using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CultRoamJumper : MonoBehaviour
{
	public static CultRoamJumper Ins;

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

	public void TriggerHammerJump()
	{
		DataManager.LockSave = true;
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		myRoamController.SetMasterLock(setLock: true);
		myCamera.transform.SetParent(CultFemaleBehaviour.Ins.CameraBone);
		DepthOfField depthOfField = ScriptableObject.CreateInstance<DepthOfField>();
		depthOfField.enabled.Override(x: true);
		depthOfField.focusDistance.Override(0.7f);
		depthOfField.aperture.Override(25.6f);
		depthOfField.focalLength.Override(56f);
		ppVol = PostProcessManager.instance.QuickVolume(PPLayerObject.layer, 100f, depthOfField);
		ppVol.weight = 0f;
		Vector3 zero = Vector3.zero;
		zero.x = 0f;
		zero.y = -180f;
		zero.z = 0f;
		Sequence sequence = DOTween.Sequence().OnComplete(delegate
		{
			CultFemaleBehaviour.Ins.HammerJump();
		});
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
		{
			myCamera.transform.localRotation = x;
		}, zero, 0.25f).SetEase(Ease.Linear).SetOptions());
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localPosition, delegate(Vector3 x)
		{
			myCamera.transform.localPosition = x;
		}, Vector3.zero, 0.3f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => ppVol.weight, delegate(float x)
		{
			ppVol.weight = x;
		}, 1f, 0.3f).SetEase(Ease.Linear));
		sequence.Play();
	}

	public void ClearDOF()
	{
		RuntimeUtilities.DestroyVolume(ppVol, destroyProfile: false);
	}

	public void TriggerEndJump()
	{
		myRoamController.KillOutOfWay();
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		myRoamController.SetMasterLock(setLock: true);
	}
}
