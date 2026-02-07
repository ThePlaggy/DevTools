using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CultMaleCamHelper : MonoBehaviour
{
	public static CultMaleCamHelper Ins;

	[SerializeField]
	private Transform camHelper;

	private Animator myAC;

	private Camera myCamera;

	private void Awake()
	{
		Ins = this;
		myAC = GetComponent<Animator>();
		CameraManager.Get(CAMERA_ID.MAIN, out myCamera);
	}

	public void StageEndJump()
	{
		base.transform.position = new Vector3(24.448f, 0f, -6.319f);
		base.transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
	}

	public void TriggerEndJump()
	{
		myCamera.transform.SetParent(camHelper);
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localPosition, delegate(Vector3 x)
		{
			myCamera.transform.localPosition = x;
		}, Vector3.zero, 0.25f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => myCamera.transform.localRotation, delegate(Quaternion x)
		{
			myCamera.transform.localRotation = x;
		}, new Vector3(0f, 180f, 0f), 0.25f).SetEase(Ease.Linear).SetOptions());
		sequence.Play();
		myAC.SetTrigger("triggerJump");
	}
}
