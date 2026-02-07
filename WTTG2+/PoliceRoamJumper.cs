using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PoliceRoamJumper : MonoBehaviour
{
	public static PoliceRoamJumper Ins;

	[SerializeField]
	private GameObject PPLayerObject;

	private Camera myCamera;

	private roamController myRoamController;

	[HideInInspector]
	private PostProcessVolume thedof;

	private void Awake()
	{
		Ins = this;
		myRoamController = GetComponent<roamController>();
		CameraManager.Get(CAMERA_ID.MAIN, out myCamera);
	}

	public void TriggerFloor8DoorJump()
	{
		myRoamController.SetMasterLock(setLock: true);
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localPosition, delegate(Vector3 x)
		{
			myCamera.transform.localPosition = x;
		}, Vector3.zero, 0.45f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
		{
			myCamera.transform.localRotation = x;
		}, new Vector3(0f, -135f, 9.9f), 0.45f).SetEase(Ease.Linear));
		sequence.Play();
	}

	public void TriggerRoomRaid(Vector3 LookAtObject)
	{
		myRoamController.SetMasterLock(setLock: true);
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		Vector3 eulerAngles = Quaternion.LookRotation(LookAtObject - base.transform.position, Vector3.up).eulerAngles;
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localPosition, delegate(Vector3 x)
		{
			myCamera.transform.localPosition = x;
		}, myRoamController.DefaultCameraPOS, 0.5f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
		{
			myCamera.transform.localRotation = x;
		}, myRoamController.DefaultCameraROT, 0.5f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, eulerAngles, 0.5f).SetEase(Ease.Linear));
		sequence.Play();
	}

	public void TriggerConstantLookAt(Vector3 LookAtObject)
	{
		Vector3 eulerAngles = Quaternion.LookRotation(LookAtObject - myCamera.transform.position).eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		base.transform.rotation = Quaternion.Euler(eulerAngles);
	}

	public void TriggerCameraConstantLookAt(Vector3 LookAtObject)
	{
		myCamera.transform.rotation = Quaternion.LookRotation(LookAtObject - myCamera.transform.position);
	}

	public void TriggerStairWayJump()
	{
		PauseManager.LockPause();
		myRoamController.SetMasterLock(setLock: true);
		GameManager.InteractionManager.LockInteraction();
		DepthOfField depthOfField = ScriptableObject.CreateInstance<DepthOfField>();
		depthOfField.enabled.Override(x: true);
		depthOfField.focusDistance.Override(0.1f);
		depthOfField.aperture.Override(5.5f);
		depthOfField.focalLength.Override(9f);
		thedof = PostProcessManager.instance.QuickVolume(PPLayerObject.layer, 100f, depthOfField);
		thedof.weight = 0f;
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => base.transform.position, delegate(Vector3 x)
		{
			base.transform.position = x;
		}, new Vector3(24.51742f, 40.51829f, -6.294f), 0.6f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, new Vector3(0f, 90f, 0f), 0.6f).SetEase(Ease.Linear).SetOptions());
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
		{
			myCamera.transform.localRotation = x;
		}, new Vector3(0.6f, 0f, 0f), 0.6f).SetEase(Ease.Linear).SetOptions());
		sequence.Insert(0.68f, DOTween.To(() => myCamera.transform.localPosition, delegate(Vector3 x)
		{
			myCamera.transform.localPosition = x;
		}, new Vector3(0.023f, 0.246f, -0.15f), 0.2f).SetEase(Ease.Linear));
		sequence.Insert(0.68f, DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
		{
			myCamera.transform.localRotation = x;
		}, new Vector3(-56.68f, 0f, 0f), 0.2f).SetEase(Ease.Linear).SetOptions());
		sequence.Insert(0.88f, DOTween.To(() => myCamera.transform.localPosition, delegate(Vector3 x)
		{
			myCamera.transform.localPosition = x;
		}, new Vector3(0.023f, -0.228f, -1.056f), 0.5f).SetEase(Ease.Linear));
		sequence.Insert(0.88f, DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
		{
			myCamera.transform.localRotation = x;
		}, new Vector3(-57.83f, 0f, 0f), 0.5f).SetEase(Ease.Linear).SetOptions());
		sequence.Insert(1.38f, DOTween.To(() => myCamera.transform.localPosition, delegate(Vector3 x)
		{
			myCamera.transform.localPosition = x;
		}, new Vector3(0.023f, -1f, -1.354f), 0.3f).SetEase(Ease.Linear));
		sequence.Insert(1.38f, DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
		{
			myCamera.transform.localRotation = x;
		}, new Vector3(-71.237f, 0f, 0f), 0.3f).SetEase(Ease.Linear).SetOptions());
		sequence.Insert(2.73f, DOTween.To(() => thedof.weight, delegate(float x)
		{
			thedof.weight = x;
		}, 1f, 0.75f).SetEase(Ease.Linear));
		sequence.Play();
	}

	private void OnDestroy()
	{
		if (thedof != null)
		{
			Debug.Log("Destroyin police PPVOL");
			RuntimeUtilities.DestroyVolume(thedof, destroyProfile: false);
			Debug.Log("police PPVOL gone");
		}
	}
}
