using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(braceController))]
public class BreatherBraceJumper : MonoBehaviour
{
	public static BreatherBraceJumper Ins;

	[SerializeField]
	private GameObject PPLayerObject;

	private braceController myBraceController;

	private Camera myCamera;

	private void Awake()
	{
		Ins = this;
		myBraceController = GetComponent<braceController>();
		CameraManager.Get(CAMERA_ID.MAIN, out myCamera);
	}

	public void TriggerDoorJump()
	{
		myBraceController.SetMasterLock(setLock: true);
		MainCameraHook.Ins.AddHeadHit(0f);
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
}
