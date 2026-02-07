using DG.Tweening;
using UnityEngine;

public class trailerController : baseController
{
	[SerializeField]
	private Vector3 startingCameraPOS = Vector3.zero;

	[SerializeField]
	private Vector3 startingCameraROT = Vector3.zero;

	protected new void Awake()
	{
		base.Awake();
		ControllerManager.Add(this);
		PostStage.Event += postBaseStage;
		PostLive.Event += postBaseLive;
	}

	protected new void Start()
	{
		base.Start();
	}

	protected new void Update()
	{
		base.Update();
	}

	protected new void OnDestroy()
	{
		ControllerManager.Remove(Controller);
		base.OnDestroy();
	}

	private void takeInput()
	{
		if (Input.GetKeyDown(KeyCode.L))
		{
			lobbyPan();
		}
		if (Input.GetKeyDown(KeyCode.H))
		{
			hallwayPan();
		}
	}

	private void lobbyPan()
	{
		base.transform.position = new Vector3(-1.5f, 1.386f, -9.27f);
		base.transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(3f, DOTween.To(() => base.transform.position, delegate(Vector3 x)
		{
			base.transform.position = x;
		}, new Vector3(-1.5f, 1.386f, -26.569f), 6f).SetEase(Ease.Linear));
		sequence.Insert(6f, DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, new Vector3(0f, 100f, 0f), 3f).SetEase(Ease.Linear).SetOptions());
		sequence.Play();
	}

	private void hallwayPan()
	{
		base.transform.position = new Vector3(-25.2f, 41.1f, -6.273f);
		base.transform.rotation = Quaternion.Euler(new Vector3(0f, -90f, 0f));
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(3f, DOTween.To(() => base.transform.position, delegate(Vector3 x)
		{
			base.transform.position = x;
		}, new Vector3(-3.9f, 41.1f, -6.273f), 5f).SetEase(Ease.Linear));
		sequence.Insert(3f, DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, new Vector3(0f, -90f, -13f), 5f).SetEase(Ease.Linear).SetOptions());
		sequence.Play();
	}

	private void postBaseStage()
	{
		PostStage.Event -= postBaseStage;
		if (Active)
		{
			CameraManager.GetCameraHook(CameraIControl).SetMyParent(base.transform);
			MyCamera.transform.localPosition = startingCameraPOS;
		}
	}

	private void postBaseLive()
	{
		PostLive.Event -= postBaseLive;
		if (Active)
		{
			MyCamera.transform.localPosition = startingCameraPOS;
			MyCamera.transform.localRotation = Quaternion.Euler(startingCameraROT);
			GameManager.BehaviourManager.CrossHairBehaviour.HideCrossHairGroup();
		}
	}
}
