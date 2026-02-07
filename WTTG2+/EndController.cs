using DG.Tweening;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class EndController : mouseableController
{
	public static EndController Ins;

	[SerializeField]
	private AudioHubObject playerEndingAudioHub;

	[SerializeField]
	private AudioFileDefinition endingBGMusic;

	[SerializeField]
	private Transform chairTrans;

	private bool achAlreadySent;

	private bool allowPlayerResponseSelection;

	public CustomEvent PlayerSelectedOptionOne = new CustomEvent(2);

	public CustomEvent PlayerSelectedOptionTwo = new CustomEvent(2);

	public bool AllowPlayerResponseSelection
	{
		set
		{
			allowPlayerResponseSelection = value;
		}
	}

	protected new void Awake()
	{
		Ins = this;
		base.Awake();
		ControllerManager.Add(this);
	}

	protected new void Start()
	{
		base.Start();
		MyCamera.transform.SetParent(MouseRotatingObject.transform);
		MyCamera.transform.localPosition = Vector3.zero;
		MyCamera.transform.localRotation = Quaternion.Euler(Vector3.zero);
		if (!MouseCaptureInit)
		{
			Init();
		}
		Active = true;
		MyState = GAME_CONTROLLER_STATE.IDLE;
		playerEndingAudioHub.PlaySound(endingBGMusic);
	}

	protected new void Update()
	{
		base.Update();
		getInput();
	}

	protected new void OnDestroy()
	{
		ControllerManager.Remove(Controller);
		base.OnDestroy();
	}

	public void PrepareForDeath()
	{
		SetMasterLock(setLock: true);
		DOTween.To(() => base.transform.localRotation, delegate(Quaternion x)
		{
			base.transform.localRotation = x;
		}, new Vector3(0f, -180f, 0f), 0.5f).SetEase(Ease.Linear).SetOptions()
			.OnComplete(delegate
			{
				Sequence sequence = DOTween.Sequence().OnComplete(delegate
				{
					CultFemaleEndingDeath.Ins.TriggerDeath();
				});
				GameManager.TimeSlinger.FireTimer(3.5f, AdamBehaviour.Ins.LetHerGo);
				sequence.Insert(0f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
				{
					MyCamera.transform.localRotation = x;
				}, Vector3.zero, 0.25f).SetEase(Ease.Linear).SetOptions());
				sequence.Insert(0.25f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
				{
					MyCamera.transform.localRotation = x;
				}, new Vector3(-15f, 0f, 0f), 1f).SetEase(Ease.Linear).SetOptions());
				sequence.Insert(1.25f, DOTween.To(() => base.transform.localRotation, delegate(Quaternion x)
				{
					base.transform.localRotation = x;
				}, Vector3.zero, 4f).SetEase(Ease.Linear).SetOptions());
				sequence.Insert(1.25f, DOTween.To(() => chairTrans.localRotation, delegate(Quaternion x)
				{
					chairTrans.localRotation = x;
				}, new Vector3(0f, -180f, 0f), 4f).SetEase(Ease.Linear).SetOptions());
				sequence.Insert(5.25f, DOTween.To(() => MyCamera.transform.localRotation, delegate(Quaternion x)
				{
					MyCamera.transform.localRotation = x;
				}, Vector3.zero, 1f).SetEase(Ease.Linear).SetOptions());
				sequence.Play();
			});
	}

	public void PrepareForLife()
	{
		SetMasterLock(setLock: true);
		CamWalkOut.Ins.WalkOut();
	}

	private void getInput()
	{
		if (lockControl)
		{
			return;
		}
		float axis = CrossPlatformInputManager.GetAxis("HorizontalDesk");
		if (axis >= 1f && !achAlreadySent)
		{
			achAlreadySent = true;
			SteamSlinger.Ins.UnlockSteamAchievement(STEAM_ACHIEVEMENT.WHOSTHATLADY);
		}
		float y = -180f + axis * 75f;
		Vector3 euler = new Vector3(0f, y, 0f);
		base.transform.rotation = Quaternion.Euler(euler);
		if (allowPlayerResponseSelection)
		{
			if (CrossPlatformInputManager.GetButtonDown("AlphaOne"))
			{
				allowPlayerResponseSelection = false;
				PlayerSelectedOptionOne.Execute();
			}
			else if (CrossPlatformInputManager.GetButtonDown("AlphaTwo"))
			{
				allowPlayerResponseSelection = false;
				PlayerSelectedOptionTwo.Execute();
			}
		}
	}
}
