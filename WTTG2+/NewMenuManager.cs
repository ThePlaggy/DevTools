using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewMenuManager : MonoBehaviour
{
	public static NewMenuManager Ins;

	public GameObject ChallengesBTN;

	public GameObject ChallengesTab;

	public GameObject TwitchBTN;

	public GameObject TwitchTab;

	public GameObject StatisticsBTN;

	public GameObject StatisticsTab;

	public GameObject Sidebar;

	public StatisticsTabLookup statisticsTabLookup;

	public TwitchTabLookup twitchTabLookup;

	public bool sidebarOpen;

	public SidebarTabType currentTab;

	public GameObject normalButtons;

	public GameObject diffSelectButtons;

	public TitleMenuBTN newGameBTN;

	public TitleMenuBTN continueBTN;

	public TitleMenuBTN hackerModeBTN;

	public TitleMenuBTN optionsBTN;

	public TitleMenuBTN quitBTN;

	public TitleMenuBTN casualBTN;

	public TitleMenuBTN normalBTN;

	public TitleMenuBTN leetBTN;

	public TitleMenuBTN nightmareBTN;

	public TitleMenuBTN entropyBTN;

	public TitleMenuBTN diffGoBackBTN;

	public GameObject logo;

	public CanvasGroup versionText;

	public TitleMenuBTN optionsReturnBTN;

	public GameObject options;

	public GameObject saveDeleteBTN;

	public GameObject CreditsBTN;

	public GameObject CreditsTab;

	public CustomEvent DismissActions;

	private bool openIsBusy;

	private float sidebar_xHidden;

	private float sidebar_xOpened;

	private float sidebar_xShown;

	public GameObject menuScenery;

	public GameObject christmasBadge;

	public GameObject easterBadge;

	public GameObject halloweenBadge;

	public GameObject twitchBadge;

	public GameObject vipBadge;

	public CanvasGroup badgeHolder;

	private GameObject defaultMenuBG;

	public TMP_Text volumeText;

	public Slider volumeSlider;

	public GameObject imGonnaShitMySelf;

	public const string ModVersion = "v1.614";

	private void Awake()
	{
		Ins = this;
		Debug.Log("New Canvas Created Successfully");
		newGameBTN.MyAction.Event += NewGameAction;
		quitBTN.MyAction.Event += QuitAction;
		diffGoBackBTN.MyAction.Event += diffReturn;
		normalBTN.MyAction.Event += NormalMode;
		leetBTN.MyAction.Event += leetAction;
		nightmareBTN.MyAction.Event += nightmareAction;
		casualBTN.MyAction.Event += casualAction;
		hackerModeBTN.MyAction.Event += hackerModeAction;
		continueBTN.MyAction.Event += continueAction;
		optionsBTN.MyAction.Event += Options;
		optionsReturnBTN.MyAction.Event += ReturnFromOptions;
		defaultMenuBG = GameObject.Find("Props");
		defaultMenuBG.SetActive(value: false);
		GameObject.Find("MainCanvas/BG").SetActive(value: false);
		GameObject.Find("MainCanvas/Logo").SetActive(value: false);
		GameObject.Find("MainCanvas/MainMenuHolder").SetActive(value: false);
		GameObject.Find("MainCanvas/AreYouSureHolder").SetActive(value: false);
		GameObject.Find("MainCanvas/DifficultLevelSelect").SetActive(value: false);
		newGameBTN.gameObject.SetActive(!DataManager.ContinuedGame);
		continueBTN.gameObject.SetActive(DataManager.ContinuedGame);
		saveDeleteBTN.SetActive(DataManager.ContinuedGame);
		versionText.gameObject.GetComponent<TMP_Text>().text = "v1.614";
		if (PlayerPrefs.HasKey("[MOD]MenuTheme"))
		{
			SpawnMenuScenery(PlayerPrefs.GetInt("[MOD]MenuTheme"));
		}
		else
		{
			SpawnMenuScenery(1);
			PlayerPrefs.SetInt("[MOD]MenuTheme", 1);
		}
		GameObject.Find("SettingsHolder/List/GameObject (3)/Setting (1)").SetActive(value: false);
		GameObject.Find("Sidebar UI/Sidebar/Image (1)").SetActive(value: false);
		GameObject.Find("Sidebar UI/Sidebar/Image (2)").GetComponent<RectTransform>().anchoredPosition += new Vector2(0f, 25f);
		GameObject.Find("Sidebar UI/Sidebar/Image (3)").GetComponent<RectTransform>().anchoredPosition += new Vector2(0f, 25f);
		GameObject.Find("Sidebar UI/Sidebar/Image (4)").GetComponent<RectTransform>().anchoredPosition += new Vector2(0f, 25f);
	}

	private void Start()
	{
		PresentMenuStart();
		vipBadge.SetActive(PlayerPrefs.HasKey("iAmVIP"));
		twitchBadge.SetActive(PlayerPrefs.HasKey("iAmFamous"));
		christmasBadge.SetActive(PlayerPrefs.HasKey("EventXMASTrophy"));
		easterBadge.SetActive(PlayerPrefs.HasKey("EventEasterTrophy"));
		halloweenBadge.SetActive(PlayerPrefs.HasKey("EventHalloTrophy"));
		GameObject.Find("Farewell").SetActive(value: false);
	}

	private void OnDestroy()
	{
		Ins = null;
	}

	public void OpenTabButton(int id)
	{
		switch (id)
		{
		case 1:
			OpenSideBarTab(SidebarTabType.CHALLENGES);
			break;
		case 2:
			OpenSideBarTab(SidebarTabType.TWITCH);
			break;
		case 3:
			OpenSideBarTab(SidebarTabType.STATISTICS);
			break;
		case 4:
			OpenSideBarTab(SidebarTabType.CREDITS);
			break;
		}
	}

	public void OpenSideBarTab(SidebarTabType tabType)
	{
		if (openIsBusy)
		{
			return;
		}
		if (tabType == currentTab && sidebarOpen)
		{
			Debug.Log("Closing Sidebar");
			sidebarOpen = false;
			openIsBusy = true;
			GetTabBTN(currentTab).SetInactive();
			Sidebar.transform.DOLocalMoveX(sidebar_xShown, 0.75f).OnComplete(delegate
			{
				openIsBusy = false;
			});
			return;
		}
		if (!sidebarOpen)
		{
			Debug.Log("Opening Sidebar");
			sidebarOpen = true;
			openIsBusy = true;
			Sidebar.transform.DOLocalMoveX(sidebar_xOpened, 0.75f).OnComplete(delegate
			{
				openIsBusy = false;
			});
		}
		GetTabGameObject(currentTab).SetActive(value: false);
		GetTabBTN(currentTab).SetInactive();
		currentTab = tabType;
		GetTabGameObject(currentTab).SetActive(value: true);
		GetTabBTN(currentTab).SetActive();
	}

	public GameObject GetTabGameObject(SidebarTabType type)
	{
		return type switch
		{
			SidebarTabType.CHALLENGES => ChallengesTab, 
			SidebarTabType.TWITCH => TwitchTab, 
			SidebarTabType.STATISTICS => StatisticsTab, 
			SidebarTabType.CREDITS => CreditsTab, 
			_ => ChallengesTab, 
		};
	}

	public void NewGameAction()
	{
		CanvasGroup myCG = normalButtons.GetComponent<CanvasGroup>();
		CanvasGroup myCG2 = diffSelectButtons.GetComponent<CanvasGroup>();
		myCG.interactable = false;
		myCG.blocksRaycasts = false;
		DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 0f, 0.25f).SetEase(Ease.Linear).OnComplete(delegate
		{
			myCG2.interactable = true;
			myCG2.blocksRaycasts = true;
			DOTween.To(() => myCG2.alpha, delegate(float x)
			{
				myCG2.alpha = x;
			}, 1f, 0.25f).SetEase(Ease.Linear);
		});
	}

	public void diffReturn()
	{
		CanvasGroup myCG = normalButtons.GetComponent<CanvasGroup>();
		CanvasGroup myCG2 = diffSelectButtons.GetComponent<CanvasGroup>();
		myCG2.interactable = false;
		myCG2.blocksRaycasts = false;
		DOTween.To(() => myCG2.alpha, delegate(float x)
		{
			myCG2.alpha = x;
		}, 0f, 0.25f).SetEase(Ease.Linear).OnComplete(delegate
		{
			myCG.interactable = true;
			myCG.blocksRaycasts = true;
			DOTween.To(() => myCG.alpha, delegate(float x)
			{
				myCG.alpha = x;
			}, 1f, 0.25f).SetEase(Ease.Linear);
		});
	}

	public void QuitAction()
	{
		Application.Quit();
	}

	public void NormalMode()
	{
		DifficultyManager.LeetMode = false;
		DifficultyManager.Nightmare = false;
		DifficultyManager.CasualMode = false;
		DifficultyManager.HackerMode = false;
		StatisticsManager.Ins.StartRun(Difficulty.NORMAL);
		DismissDiff();
	}

	public void DismissDiff()
	{
		normalButtons.GetComponent<CanvasGroup>();
		CanvasGroup myCG2 = diffSelectButtons.GetComponent<CanvasGroup>();
		myCG2.interactable = false;
		myCG2.blocksRaycasts = false;
		DOTween.To(() => myCG2.alpha, delegate(float x)
		{
			myCG2.alpha = x;
		}, 0f, 0.25f).SetEase(Ease.Linear).OnComplete(delegate
		{
			TitleManager.Ins.DismissTitle();
			DismissMenu();
			logo.GetComponent<NewLogoHook>().DismissMe();
			badgeHolder.DOFade(0f, 0.25f).SetEase(Ease.Linear);
		});
	}

	private void leetAction()
	{
		DifficultyManager.LeetMode = true;
		DifficultyManager.Nightmare = false;
		DifficultyManager.CasualMode = false;
		DifficultyManager.HackerMode = false;
		StatisticsManager.Ins.StartRun(Difficulty.LEET);
		DismissDiff();
	}

	private void nightmareAction()
	{
		DifficultyManager.LeetMode = false;
		DifficultyManager.Nightmare = true;
		DifficultyManager.CasualMode = false;
		DifficultyManager.HackerMode = false;
		StatisticsManager.Ins.StartRun(Difficulty.NIGHTMARE);
		DismissDiff();
	}

	private void casualAction()
	{
		DifficultyManager.LeetMode = false;
		DifficultyManager.Nightmare = false;
		DifficultyManager.CasualMode = true;
		DifficultyManager.HackerMode = false;
		DismissDiff();
	}

	private void hackerModeAction()
	{
		DifficultyManager.LeetMode = false;
		DifficultyManager.Nightmare = false;
		DifficultyManager.CasualMode = false;
		DifficultyManager.HackerMode = true;
		TitleManager.Ins.DismissTitle();
		DismissMenu();
		logo.GetComponent<NewLogoHook>().DismissMe();
		badgeHolder.DOFade(0f, 0.25f).SetEase(Ease.Linear);
	}

	private void continueAction()
	{
		DifficultyManager.LeetMode = false;
		DifficultyManager.Nightmare = false;
		DifficultyManager.CasualMode = false;
		DifficultyManager.HackerMode = false;
		TitleManager.Ins.DismissTitle();
		DismissMenu();
		logo.GetComponent<NewLogoHook>().DismissMe();
		badgeHolder.DOFade(0f, 0.25f).SetEase(Ease.Linear);
	}

	public void DismissMenu()
	{
		CanvasGroup myCG = normalButtons.GetComponent<CanvasGroup>();
		CanvasGroup myCG2 = versionText.GetComponent<CanvasGroup>();
		CanvasGroup component = Sidebar.GetComponent<CanvasGroup>();
		component.interactable = false;
		component.blocksRaycasts = false;
		myCG.interactable = false;
		myCG.blocksRaycasts = false;
		myCG2.interactable = false;
		myCG2.blocksRaycasts = false;
		DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 0f, 0.25f).SetEase(Ease.Linear).OnComplete(delegate
		{
		});
		DOTween.To(() => myCG2.alpha, delegate(float x)
		{
			myCG2.alpha = x;
		}, 0f, 0.25f).SetEase(Ease.Linear).OnComplete(delegate
		{
		});
		openIsBusy = true;
		Sidebar.transform.DOLocalMoveX(sidebar_xHidden, 1f).OnComplete(delegate
		{
			if (sidebarOpen)
			{
				sidebarOpen = false;
				GetTabBTN(currentTab).SetInactive();
			}
			openIsBusy = false;
		});
	}

	private void presentMe()
	{
		CanvasGroup myCG = normalButtons.GetComponent<CanvasGroup>();
		CanvasGroup myCG2 = versionText.GetComponent<CanvasGroup>();
		CanvasGroup myCG3 = Sidebar.GetComponent<CanvasGroup>();
		DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 1f, 2f).SetEase(Ease.Linear).OnComplete(delegate
		{
			myCG.interactable = true;
			myCG.blocksRaycasts = true;
		});
		DOTween.To(() => myCG2.alpha, delegate(float x)
		{
			myCG2.alpha = x;
		}, 1f, 2f).SetEase(Ease.Linear).OnComplete(delegate
		{
			myCG2.interactable = true;
			myCG2.blocksRaycasts = true;
		});
		Sidebar.transform.DOLocalMoveX(Sidebar.transform.localPosition.x + 40f, 0.01f).OnComplete(delegate
		{
			sidebar_xHidden = Sidebar.transform.localPosition.x;
			myCG3.alpha = 1f;
			Sidebar.transform.DOLocalMoveX(Sidebar.transform.localPosition.x - 40f, 1f).OnComplete(delegate
			{
				myCG3.interactable = true;
				myCG3.blocksRaycasts = true;
				sidebar_xShown = Sidebar.transform.localPosition.x;
				sidebar_xOpened = Sidebar.transform.localPosition.x - 505f;
			});
		});
	}

	private void presentMeQuick()
	{
		CanvasGroup myCG = normalButtons.GetComponent<CanvasGroup>();
		CanvasGroup myCG2 = versionText.GetComponent<CanvasGroup>();
		CanvasGroup myCG3 = Sidebar.GetComponent<CanvasGroup>();
		DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 1f, 0.25f).SetEase(Ease.Linear).OnComplete(delegate
		{
			myCG.interactable = true;
			myCG.blocksRaycasts = true;
		});
		DOTween.To(() => myCG2.alpha, delegate(float x)
		{
			myCG2.alpha = x;
		}, 1f, 0.25f).SetEase(Ease.Linear).OnComplete(delegate
		{
			myCG2.interactable = true;
			myCG2.blocksRaycasts = true;
		});
		openIsBusy = true;
		Sidebar.transform.DOLocalMoveX(sidebar_xShown, 1f).OnComplete(delegate
		{
			openIsBusy = false;
			myCG3.interactable = true;
			myCG3.blocksRaycasts = true;
		});
	}

	public void SpawnMenuScenery(int id)
	{
		Debug.Log($"Current Menu Scenery: {id}");
		defaultMenuBG.SetActive(value: false);
		switch (id)
		{
		case 0:
			menuScenery = Object.Instantiate(CustomObjectLookUp.wttg1TitleScenery);
			menuScenery.transform.position = new Vector3(0.46f, 0.31f, 2.37f);
			break;
		case 1:
			menuScenery = Object.Instantiate(CustomObjectLookUp.wttg2TitleScenery);
			menuScenery.transform.position = new Vector3(-11f, 6.5f, 12f);
			break;
		case 2:
			defaultMenuBG.SetActive(value: true);
			break;
		}
	}

	private void PresentMenuStart()
	{
		TimeSlinger.FireTimer(delegate
		{
			logo.GetComponent<NewLogoHook>().PresentMe();
			badgeHolder.DOFade(1f, 0.25f).SetEase(Ease.Linear);
			TimeSlinger.FireTimer(presentMe, 2f);
		}, 4f);
	}

	public void Options()
	{
		DismissMenu();
		TimeSlinger.FireTimer(PresentOptions, 0.25f);
	}

	public void ReturnFromOptions()
	{
		DismissOptions();
		TimeSlinger.FireTimer(presentMeQuick, 0.25f);
	}

	public void DismissOptions()
	{
		options.GetComponent<CanvasGroup>();
		CanvasGroup component = options.GetComponent<CanvasGroup>();
		component.interactable = false;
		component.blocksRaycasts = false;
		component.DOFade(0f, 0.25f).SetEase(Ease.Linear);
	}

	public void PresentOptions()
	{
		options.GetComponent<CanvasGroup>();
		CanvasGroup myCG2 = options.GetComponent<CanvasGroup>();
		myCG2.DOFade(1f, 0.25f).SetEase(Ease.Linear).OnComplete(delegate
		{
			myCG2.interactable = true;
			myCG2.blocksRaycasts = true;
		});
	}

	public SidebarBTN GetTabBTN(SidebarTabType type)
	{
		GameObject gameObject = ChallengesBTN;
		switch (type)
		{
		case SidebarTabType.CHALLENGES:
			gameObject = ChallengesBTN;
			break;
		case SidebarTabType.TWITCH:
			gameObject = TwitchBTN;
			break;
		case SidebarTabType.STATISTICS:
			gameObject = StatisticsBTN;
			break;
		case SidebarTabType.CREDITS:
			gameObject = CreditsBTN;
			break;
		}
		return gameObject.GetComponentInChildren<SidebarBTN>();
	}

	public void DeleteSave()
	{
		DataManager.ClearGameData();
		newGameBTN.gameObject.SetActive(!DataManager.ContinuedGame);
		continueBTN.gameObject.SetActive(DataManager.ContinuedGame);
		saveDeleteBTN.SetActive(DataManager.ContinuedGame);
	}

	public void ChangeVolume(float value)
	{
	}
}
