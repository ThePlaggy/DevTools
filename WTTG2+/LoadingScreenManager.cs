using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
	public static LoadingScreenManager Ins;

	[SerializeField]
	private bool debugMode;

	[SerializeField]
	private GameObject loadingScreenGameObject;

	[SerializeField]
	private CanvasGroup loadingScreenCanvasGroup;

	[SerializeField]
	private CanvasGroup contentCanvasGroup;

	[SerializeField]
	private CanvasGroup skullCanvasGroup;

	[SerializeField]
	private CanvasGroup proTipCG;

	[SerializeField]
	private TextMeshProUGUI proTipText;

	[SerializeField]
	private AudioFileDefinition loadingMusic;

	[SerializeField]
	private string[] tips;

	private AudioHubObject myAudioHub;

	private Tweener skullTween;

	private List<string> newTips = new List<string>();

	private void Awake()
	{
		Ins = this;
		myAudioHub = GetComponent<AudioHubObject>();
		skullTween = DOTween.To(() => skullCanvasGroup.alpha, delegate(float x)
		{
			skullCanvasGroup.alpha = x;
		}, 1f, 0.75f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
		skullTween.SetAutoKill(autoKillOnCompletion: false);
		skullTween.Pause();
		GameManager.StageManager.TheGameIsLive += gameIsLive;
		createTips();
	}

	private void createTips()
	{
		newTips.Add("Don't forget the flashlight.");
		newTips.Add("Remember to switch WiFi networks.");
		newTips.Add("You can bookmark important pages in A.N.N");
		newTips.Add("Place your motion sensors where it makes the most sense.");
		newTips.Add("Don’t peek too much.");
		newTips.Add("Some WiFi networks are better than the others.");
		newTips.Add("Pay attention to the time when web pages are down.");
		newTips.Add("There's a chance for a Blue God VPN spot to spawn in the apartment complex.");
		newTips.Add("DOSDrainer virus can only attack you if you're connected to WiFi");
		newTips.Add("Unlike other enemies, Tanner activates when there's more user activity.");
		newTips.Add("You don't wanna stay in the dark for too long.");
		newTips.Add("Don't forget to claim payout from your hacked devices.");
		newTips.Add("He doesn't like when you hang up.");
		newTips.Add("ZoneWall becomes harder as the night progresses.");
		newTips.Add("Turn off your PC when not using it.");
		newTips.Add("Don't look at them.");
		newTips.Add("Turn off your computer when you're not using it.");
		newTips.Add("Be less paranoid, not everything you hear is going to get you.");
		newTips.Add("Remember to check in the source code.");
		newTips.Add("Listen carefully when going to the dead drop.");
		newTips.Add("You don't want to see The Dead tarot card.");
		newTips.Add("Enemies cannot pick-lock your door if you have better security lock.");
		newTips.Add("Don't stroll in the hallways at the late hours.");
		newTips.Add("Turning off lights before hiding might convince intruding enemies to leave earlier.");
		newTips.Add("While being extremely rare, The Artist tarot card can give you all of the key’s locations.");
		newTips.Add("If you are chased it’s best to not reveal your apartment’s location to your pursuer.");
		newTips.Add("Don’t be fooled by the powerful Security Lock as some enemies might try to find a way around to kill you.");
		newTips.Add("When buying a Remote VPN upgrade, all your existing VPNs are upgraded automatically.");
		newTips.Add("A tarot card deck can hold insanely good cards but also equally bad ones. Use with caution.");
		newTips.Add("Security Cameras are wonderful to anticipate enemy activity but suffer a limited battery life.");
		newTips.Add("Don’t be an idiot.");
		newTips.Add("WPA2 networks are the fastest and most protected networks. WPA networks are slightly worse but way easier to crack.");
		newTips.Add("The Police Scanner is a very handy tool to track the Police as long as you can understand its messages.");
		newTips.Add("The Caller tarot card can send an inconvenient enemy after you within a short time frame.");
		newTips.Add("Task enemies will pay you extra DOS coins for progressing their mission.");
		newTips.Add("The rare Vengeance tarot card can remove a problem of yours.");
		newTips.Add("Don’t check seized websites too much.");
		newTips.Add("Think twice before connecting to random suspicious WiFi networks.");
		newTips.Add("Buying a router device adds an extra layer of protection and makes your internet better.");
		newTips.Add("Be dead silent.");
		newTips.Add("4 8 15 16 23 42");
	}

	private void Start()
	{
		if (!debugMode)
		{
			stageLoading();
		}
	}

	private void stageLoading()
	{
		int index = Random.Range(0, newTips.Count);
		proTipText.SetText(DifficultyManager.HackerMode ? "" : ("Pro Tip: " + newTips[index]));
		myAudioHub.PlaySound(loadingMusic);
		DOTween.To(() => contentCanvasGroup.alpha, delegate(float x)
		{
			contentCanvasGroup.alpha = x;
		}, 1f, 2f).SetDelay(3f).SetEase(Ease.Linear)
			.OnComplete(delegate
			{
				DOTween.To(() => proTipCG.alpha, delegate(float x)
				{
					proTipCG.alpha = x;
				}, 1f, 1f).SetEase(Ease.Linear);
				skullTween.Restart();
				GameManager.WorldManager.StageGame();
			});
	}

	private void gameIsLive()
	{
		GameManager.StageManager.TheGameIsLive -= gameIsLive;
		if (!debugMode)
		{
			myAudioHub.MuffleHub(0f, 1.75f);
			DOTween.To(() => contentCanvasGroup.alpha, delegate(float x)
			{
				contentCanvasGroup.alpha = x;
			}, 0f, 1f).SetDelay(1f).SetEase(Ease.Linear)
				.OnComplete(delegate
				{
					DOTween.To(() => loadingScreenCanvasGroup.alpha, delegate(float x)
					{
						loadingScreenCanvasGroup.alpha = x;
					}, 0f, 0.75f).SetEase(Ease.Linear).OnComplete(delegate
					{
						skullTween.Kill();
						loadingScreenGameObject.SetActive(value: false);
					});
				});
			if (DifficultyManager.HackerMode)
			{
				computerController.Ins.TakeControl();
				HackerModeManager.Ins.GameLoaded();
			}
		}
		else
		{
			loadingScreenGameObject.SetActive(value: false);
		}
	}
}
