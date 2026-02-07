using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(roamController))]
public class HitmanRoamJumper : MonoBehaviour
{
	public static HitmanRoamJumper Ins;

	[SerializeField]
	private GameObject PPLayerObject;

	public roamController myRoamController;

	private Camera myCamera;

	private PostProcessVolume ppVol;

	private void Awake()
	{
		Ins = this;
		myRoamController = GetComponent<roamController>();
		CameraManager.Get(CAMERA_ID.MAIN, out myCamera);
	}

	public void AddLobbyComputerJump()
	{
		Debug.Log("[Lucas+] Stagging New Lobby Computer Jump....");
		myRoamController.TookControlActions.Event += triggerLobbyComputerJump;
	}

	public void ClearPPVol()
	{
		if (ppVol != null)
		{
			RuntimeUtilities.DestroyVolume(ppVol, destroyProfile: false);
		}
	}

	public void TriggerBathroomJump()
	{
		LookUp.Doors.BathroomDoor.CancelAutoClose();
		DataManager.LockSave = true;
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		myRoamController.SetMasterLock(setLock: true);
		DepthOfField depthOfField = ScriptableObject.CreateInstance<DepthOfField>();
		depthOfField.enabled.Override(x: true);
		depthOfField.focusDistance.Override(0.23f);
		depthOfField.aperture.Override(25.4f);
		depthOfField.focalLength.Override(28f);
		ppVol = PostProcessManager.instance.QuickVolume(PPLayerObject.layer, 100f, depthOfField);
		ppVol.weight = 0f;
		DOTween.To(() => ppVol.weight, delegate(float x)
		{
			ppVol.weight = x;
		}, 1f, 2f).SetEase(Ease.Linear);
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => base.transform.position, delegate(Vector3 x)
		{
			base.transform.position = x;
		}, new Vector3(5.244362f, 40.52543f, 1.948186f), 0.75f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, new Vector3(0f, -90f, 0f), 0.5f).SetEase(Ease.Linear).SetOptions());
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
		{
			myCamera.transform.localRotation = x;
		}, Vector3.zero, 0.5f).SetEase(Ease.Linear).SetOptions());
		sequence.Play();
	}

	public void TriggerMainDoorOpenJump()
	{
		DataManager.LockSave = true;
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		myRoamController.SetMasterLock(setLock: true);
		GameManager.TimeSlinger.FireTimer(0.5f, delegate
		{
			myRoamController.KillOutOfWay();
			Sequence sequence = DOTween.Sequence();
			sequence.Insert(0f, DOTween.To(() => base.transform.position, delegate(Vector3 x)
			{
				base.transform.position = x;
			}, new Vector3(-2.185477f, 40.52925f, -3.811371f), 0.3f).SetEase(Ease.Linear));
			sequence.Insert(0f, DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
			{
				myCamera.transform.localRotation = x;
			}, Vector3.zero, 0.3f).SetEase(Ease.Linear).SetOptions());
			sequence.Play();
		});
	}

	public void TriggerMainDoorOpendJump()
	{
		myRoamController.LockFromDoorRecovry();
		DepthOfField depthOfField = ScriptableObject.CreateInstance<DepthOfField>();
		depthOfField.enabled.Override(x: true);
		depthOfField.focusDistance.Override(0.23f);
		depthOfField.aperture.Override(25.4f);
		depthOfField.focalLength.Override(28f);
		ppVol = PostProcessManager.instance.QuickVolume(PPLayerObject.layer, 100f, depthOfField);
		ppVol.weight = 0f;
		DOTween.To(() => ppVol.weight, delegate(float x)
		{
			ppVol.weight = x;
		}, 1f, 2f).SetEase(Ease.Linear);
	}

	public void TriggerMainDoorOutSideJump()
	{
		DataManager.LockSave = true;
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		myRoamController.SetMasterLock(setLock: true);
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => base.transform.position, delegate(Vector3 x)
		{
			base.transform.position = x;
		}, new Vector3(-2.253f, base.transform.position.y, -5.411f), 0.5f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, Vector3.zero, 0.4f).SetEase(Ease.Linear).SetOptions());
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
		{
			myCamera.transform.localRotation = x;
		}, Vector3.zero, 0.5f).SetEase(Ease.Linear).SetOptions());
		sequence.Play();
	}

	public void TriggerHallWayDoorJump()
	{
		DataManager.LockSave = true;
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		myRoamController.SetMasterLock(setLock: true);
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => base.transform.position, delegate(Vector3 x)
		{
			base.transform.position = x;
		}, new Vector3(24.727f, base.transform.position.y, -6.284f), 0.5f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, new Vector3(0f, 90f, 0f), 0.4f).SetEase(Ease.Linear).SetOptions());
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
		{
			myCamera.transform.localRotation = x;
		}, Vector3.zero, 0.5f).SetEase(Ease.Linear).SetOptions());
		sequence.Play();
	}

	public void TriggerStairWayDoorJump()
	{
		DataManager.LockSave = true;
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		myRoamController.SetMasterLock(setLock: true);
		myRoamController.LockFromDoorRecovry();
	}

	private void triggerLobbyComputerJump()
	{
		myRoamController.TookControlActions.Event -= triggerLobbyComputerJump;
		DataManager.LockSave = true;
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		myRoamController.SetMasterLock(setLock: true);
		Sequence sequence = DOTween.Sequence().OnComplete(delegate
		{
			EnemyManager.HitManManager.ExecuteLobbyComputerJump();
		});
		sequence.Insert(0f, DOTween.To(() => base.transform.position, delegate(Vector3 x)
		{
			base.transform.position = x;
		}, new Vector3(3.587919f, base.transform.position.y, -22.74409f), 0.3f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, new Vector3(0f, 0f, 0f), 0.25f).SetEase(Ease.Linear).SetOptions());
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
		{
			myCamera.transform.localRotation = x;
		}, Vector3.zero, 0.3f).SetEase(Ease.Linear).SetOptions());
		sequence.Play();
	}
}
