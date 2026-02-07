using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
	public static EndingManager Ins;

	[SerializeField]
	private CanvasGroup blackScreenCG;

	[SerializeField]
	private CanvasGroup logoCG;

	[SerializeField]
	private GameObject tripodCamera;

	[SerializeField]
	private AudioHubObject doorHub;

	[SerializeField]
	private AudioFileDefinition wakeHimUp;

	[SerializeField]
	private AudioFileDefinition faceSlap;

	[SerializeField]
	private AudioFileDefinition logoJump;

	[SerializeField]
	private AudioFileDefinition endingToture;

	[SerializeField]
	private AudioFileDefinition doorOpenSFX;

	[SerializeField]
	private AdamBehaviour myAdamBehaviour;

	[SerializeField]
	private EndingStepDefinition[] endingSteps = new EndingStepDefinition[0];

	private int currentStepIndex;

	private EndingResponseManager playerResponseManager;

	private void Awake()
	{
		Ins = this;
		playerResponseManager = GetComponent<EndingResponseManager>();
	}

	private void Start()
	{
		PauseManager.LockPause();
		SteamSlinger.Ins.CheckForPro();
		ayanaRemover();
		GameManager.TimeSlinger.FireTimer(5f, wakeClintUp);
	}

	public void ManualProcessEndingStep(EndingStepDefinition TheStep)
	{
		if (TheStep.HasAnimationTrigger)
		{
			myAdamBehaviour.CallAniTrigger(TheStep.AnimationTriggerName);
		}
		if (TheStep.LookingForPlayerResponse)
		{
			playerResponseManager.ProcessPlayerReponse(TheStep);
		}
	}

	public void PlayerChoiceDeath()
	{
		tripodCamera.SetActive(value: true);
		CultFemaleEndingDeath.Ins.StageTriggerDeath();
		CultFemaleEnding.Ins.WalkBehindPlayer();
		GameManager.TimeSlinger.FireTimer(6f, CultMaleEnding.Ins.WalkBehindPlayer);
		GameManager.TimeSlinger.FireTimer(12.5f, EndController.Ins.PrepareForDeath);
	}

	public void PlayerChoiceLife()
	{
		CultMaleEnding.Ins.WalkBehindPlayer();
		GameManager.TimeSlinger.FireTimer(5f, EndController.Ins.PrepareForLife);
		GameManager.AudioSlinger.PlaySoundWithCustomDelay(endingToture, 12f);
		GameManager.TimeSlinger.FireTimer(25f, ShowLifeFadeOut);
	}

	public void ShowDeathFadeOut()
	{
		DOTween.To(() => blackScreenCG.alpha, delegate(float x)
		{
			blackScreenCG.alpha = x;
		}, 1f, 1.6f).SetEase(Ease.Linear).SetDelay(0.5f);
		GameManager.TimeSlinger.FireTimer(4f, delegate
		{
			GameManager.AudioSlinger.MuffleAudioLayer(AUDIO_LAYER.MUSIC, 0f, 3f);
			GameManager.AudioSlinger.MuffleAudioLayer(AUDIO_LAYER.ENVIRONMENT, 0f, 3f);
		});
		GameManager.TimeSlinger.FireTimer(9f, delegate
		{
			EndingCameraHook.Ins.ClearAFR();
			logoCG.alpha = 1f;
			GameManager.AudioSlinger.PlaySound(logoJump);
		});
		GameManager.TimeSlinger.FireTimer(22f, delegate
		{
			logoCG.alpha = 0f;
			SceneManager.LoadScene(0);
		});
	}

	public void ShowLifeFadeOut()
	{
		DOTween.To(() => blackScreenCG.alpha, delegate(float x)
		{
			blackScreenCG.alpha = x;
		}, 1f, 1.6f).SetEase(Ease.Linear);
		doorHub.PlaySoundCustomDelay(doorOpenSFX, 2f);
		GameManager.TimeSlinger.FireTimer(15f, delegate
		{
			EndingCameraHook.Ins.EnableAFR();
		});
		GameManager.TimeSlinger.FireTimer(20f, delegate
		{
			GameManager.AudioSlinger.MuffleAudioLayer(AUDIO_LAYER.MUSIC, 0f, 3f);
			GameManager.AudioSlinger.MuffleAudioLayer(AUDIO_LAYER.ENVIRONMENT, 0f, 3f);
		});
		GameManager.TimeSlinger.FireTimer(26f, delegate
		{
			EndingCameraHook.Ins.ClearAFR();
			logoCG.alpha = 1f;
			GameManager.AudioSlinger.PlaySound(logoJump);
		});
		GameManager.TimeSlinger.FireTimer(37f, delegate
		{
			logoCG.alpha = 0f;
			SceneManager.LoadScene(0);
		});
	}

	private void startEndingTalk()
	{
		currentStepIndex = 0;
		processEndingStep();
	}

	private void wakeClintUp()
	{
		DataManager.ClearGameData();
		SteamSlinger.Ins.UnlockSteamAchievement(STEAM_ACHIEVEMENT.THEONEPERCENT);
		StatisticsManager.Ins.BeatRun(Difficulty.NORMAL);
		GameManager.AudioSlinger.PlaySound(wakeHimUp);
		GameManager.AudioSlinger.PlaySoundWithCustomDelay(faceSlap, 1.75f);
		GameManager.TimeSlinger.FireTimer(2.5f, delegate
		{
			DOTween.To(() => blackScreenCG.alpha, delegate(float x)
			{
				blackScreenCG.alpha = x;
			}, 0f, 1f).SetEase(Ease.Linear);
			EndingCameraHook.Ins.WakeUp();
		});
		GameManager.TimeSlinger.FireTimer(5.5f, delegate
		{
			startEndingTalk();
			EndController.Ins.SetMasterLock(setLock: false);
		});
	}

	private void processEndingStep()
	{
		if (currentStepIndex < endingSteps.Length && endingSteps[currentStepIndex].HasAnimationTrigger)
		{
			myAdamBehaviour.CallAniTrigger(endingSteps[currentStepIndex].AnimationTriggerName);
		}
	}

	private void ayanaRemover()
	{
		if (TenantTrackManager.DidAyana)
		{
			Object.Destroy(GameObject.Find("NymphoEnding"));
		}
		GameObject.Find("EndingCanvas/Logo").GetComponent<Image>().sprite = CustomSpriteLookUp.wttg2PlusLogo;
		GameObject gameObject = Object.Instantiate(CustomObjectLookUp.TheTanner);
		gameObject.transform.position = new Vector3(3.8537f, 0f, -2.217f);
		gameObject.transform.rotation = Quaternion.Euler(0f, 313.6545f, 0f);
		GameObject.Find("TannerHeadLight").SetActive(value: false);
		GameObject gameObject2 = Object.Instantiate(CustomObjectLookUp.ExecutionerCustomRig);
		gameObject2.transform.position = new Vector3(-4.2118f, 0f, -1.2752f);
		gameObject2.transform.rotation = Quaternion.Euler(0f, 51.7272f, 0f);
		Object.Instantiate(CustomObjectLookUp.KidnapperTitle).GetComponent<KidnapperTitleManager>().isEnding = true;
	}
}
