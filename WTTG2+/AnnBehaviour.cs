using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using ZenFulcrum.EmbeddedBrowser;

public class AnnBehaviour : WindowBehaviour
{
	private const string NOT_FOUND_URL = "localGame://NotFound/index.html";

	public static bool InstalledGamingVPN;

	public static bool Claffis;

	public static bool ChosenAwake;

	public static bool DoneMurderPlayer;

	public int BOOKMARK_TAB_START_POOL_COUNT = 10;

	public bool isPlayingAudio;

	private Sequence aniLoadingBarSeq;

	private Sequence aniLoadingGlobeSeq;

	private Timer annLoadPageTimer;

	private InputField annURLBox;

	private Dictionary<int, BookmarkTABObject> currentBookmarkTabs = new Dictionary<int, BookmarkTABObject>();

	private PooledStack<BookmarkTABObject> bookmarkTabObjectPool;

	private List<string> backHistory = new List<string>();

	private Vector2 bookmarksTabHolderSize = Vector2.zero;

	private bool bookmarkMenuActive;

	private bool bookmarkMenuAniActive;

	private GameObject bookmarksBTN;

	private GameObject browserObject;

	private GameObject BookmarksMenu;

	private GameObject BookmarkTabObject;

	private GameObject BookmarksMenuScrollBar;

	private GameObject BookmarksMenuTabHolder;

	public CustomEvent FakePageLoaded = new CustomEvent(2);

	private List<string> forwardHistory = new List<string>();

	private bool isMin;

	private bool isOpened;

	private string lastURL = string.Empty;

	private Browser myBrowser;

	private bool userIsOffline;

	public bool isThePageLoading { get; private set; }

	protected new void Awake()
	{
		base.Awake();
		GameManager.BehaviourManager.AnnBehaviour = this;
		browserObject = LookUp.DesktopUI.ANN_WINDOW_BROWSER_OBJECT;
		annURLBox = LookUp.DesktopUI.ANN_WINDOW_URL_BOX;
		bookmarksBTN = LookUp.DesktopUI.ANN_WINDOW_BOOKMARKS_BTN;
		BookmarksMenu = LookUp.DesktopUI.ANN_WINDOW_BOOKMARKS_MENU;
		BookmarksMenuTabHolder = LookUp.DesktopUI.ANN_WINDOW_BOOKMARKS_MENU_TAB_HOLDER;
		BookmarkTabObject = LookUp.DesktopUI.ANN_WINDOW_BOOKMARKS_TAB_OBJECT;
		bookmarkTabObjectPool = new PooledStack<BookmarkTABObject>(delegate
		{
			BookmarkTABObject component = UnityEngine.Object.Instantiate(BookmarkTabObject, BookmarksMenuTabHolder.GetComponent<RectTransform>()).GetComponent<BookmarkTABObject>();
			component.SoftBuild();
			return component;
		}, BOOKMARK_TAB_START_POOL_COUNT);
		GameManager.StageManager.Stage += stageMe;
	}

	protected new void Update()
	{
		base.Update();
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
	}

	public void ForceLoadURL(string setURL)
	{
		annURLBox.text = setURL;
		GotoURL(setURL);
	}

	public void GotoURL(string setURL, bool AddHistory = true)
	{
		annURLBox.text = setURL;
		if (bookmarkMenuActive)
		{
			triggerBookmarksMenu();
		}
		if (!(setURL != lastURL))
		{
			return;
		}
		clearCurrentMusic();
		if (AddHistory && lastURL != string.Empty)
		{
			backHistory.Add(lastURL);
			forwardHistory.Clear();
		}
		lastURL = setURL;
		float num = 0.75f;
		string returnURL;
		if (userIsOffline)
		{
			GameManager.TheCloud.InvalidURL(out returnURL);
		}
		else
		{
			GameManager.TheCloud.ValidateURL(out returnURL, setURL);
			num = GameManager.ManagerSlinger.WifiManager.GenereatePageLoadingTime();
		}
		if (RouterBehaviour.Ins.Owned && RouterBehaviour.Ins.RouterIsActive)
		{
            switch (RouterBehaviour.Ins.routerHubSwitch)
            {
                case 1:
                    num = num / 0.5f;
                    break;
                case 2:
                    num = num / 1f;
                    break;
                case 3:
                    num = num / 1.5f;
                    break;
                case 4:
                    num = num / 2f;
                    break;
                default:
                    num = num / 0.1f;
                    break;
            }
            if (RouterBehaviour.Ins.IsJammed)
			{
				num = 45f;
			}
		}
		LookUp.DesktopUI.ANN_WINDOW_HOME_BTN.setLock(setValue: true);
		LookUp.DesktopUI.ANN_WINDOW_BACK_BTN.setLock(setValue: true);
		LookUp.DesktopUI.ANN_WINDOW_FORWARD_BTN.setLock(setValue: true);
		LookUp.DesktopUI.ANN_WINDOW_REFRESH_BTN.setLock(setValue: true);
		LookUp.DesktopUI.ANN_WINDOW_CODE_BTN.setLock(setValue: true);
		annURLBox.enabled = false;
		isThePageLoading = true;
		aniLoadingPage(num);
		GameManager.TimeSlinger.FireHardTimer(out annLoadPageTimer, num, loadBrowserURL, returnURL);
	}

	public void GotoFakeURL(string FakeURL)
	{
		float num = 1.25f;
		annURLBox.text = FakeURL;
		LookUp.DesktopUI.ANN_WINDOW_HOME_BTN.setLock(setValue: true);
		LookUp.DesktopUI.ANN_WINDOW_BACK_BTN.setLock(setValue: true);
		LookUp.DesktopUI.ANN_WINDOW_FORWARD_BTN.setLock(setValue: true);
		LookUp.DesktopUI.ANN_WINDOW_REFRESH_BTN.setLock(setValue: true);
		LookUp.DesktopUI.ANN_WINDOW_CODE_BTN.setLock(setValue: true);
		isThePageLoading = true;
		aniLoadingPage(num);
		GameManager.TimeSlinger.FireTimer(num, delegate
		{
			LookUp.DesktopUI.ANN_WINDOW_HOME_BTN.setLock(setValue: false);
			LookUp.DesktopUI.ANN_WINDOW_BACK_BTN.setLock(setValue: false);
			LookUp.DesktopUI.ANN_WINDOW_FORWARD_BTN.setLock(setValue: false);
			LookUp.DesktopUI.ANN_WINDOW_REFRESH_BTN.setLock(setValue: false);
			LookUp.DesktopUI.ANN_WINDOW_CODE_BTN.setLock(setValue: false);
			isThePageLoading = false;
			aniLoadingBarSeq.Kill();
			aniLoadingGlobeSeq.Kill();
			LookUp.DesktopUI.ANN_WINDOW_LOADING_BAR.fillAmount = 0f;
			LookUp.DesktopUI.ANN_WINDOW_GLOBE.alpha = 1f;
			FakePageLoaded.Execute();
		});
	}

	public void ClearFakeURL()
	{
		annURLBox.text = string.Empty;
	}

	public void AnnBTNAction(ANN_BTN_ACTIONS Action)
	{
		switch (Action)
		{
		case ANN_BTN_ACTIONS.BACK:
			goBack();
			break;
		case ANN_BTN_ACTIONS.FORWARD:
			goForward();
			break;
		case ANN_BTN_ACTIONS.REFRESH:
			refreshPage();
			break;
		case ANN_BTN_ACTIONS.BOOKMARKS:
			triggerBookmarksMenu();
			break;
		case ANN_BTN_ACTIONS.HOME:
			if (lastURL == GameManager.TheCloud.GetWikiURL(0))
			{
				refreshPage();
			}
			else
			{
				GotoURL(GameManager.TheCloud.GetWikiURL(0));
			}
			break;
		case ANN_BTN_ACTIONS.CODE:
			GameManager.TheCloud.GetCurrentPageSourceCode();
			break;
		}
	}

	public void AddBookmarkTab(int SetHashCode, BookmarkData SetBookmarkData)
	{
		float setY = 0f - (28f * (float)currentBookmarkTabs.Count + 2f * (float)currentBookmarkTabs.Count);
		BookmarkTABObject bookmarkTABObject = bookmarkTabObjectPool.Pop();
		bookmarkTABObject.Build(SetBookmarkData, setY);
		currentBookmarkTabs.Add(SetHashCode, bookmarkTABObject);
		bookmarksTabHolderSize.y = 30f * (float)currentBookmarkTabs.Count;
		BookmarksMenuTabHolder.GetComponent<RectTransform>().sizeDelta = bookmarksTabHolderSize;
	}

	public void RemoveBookmarkTab(int SetHashCode)
	{
		if (currentBookmarkTabs.TryGetValue(SetHashCode, out var value))
		{
			value.KillMe();
			currentBookmarkTabs.Remove(SetHashCode);
			bookmarkTabObjectPool.Push(value);
			rePOSBookmarkTabs();
		}
	}

	protected override void OnLaunch()
	{
		if (!Window.activeSelf && myBrowser == null)
		{
			myBrowser = browserObject.GetComponent<Browser>();
			myBrowser.onLoad += pageLoaded;
		}
	}

	protected override void OnClose()
	{
		clearCurrentMusic();
		if (isThePageLoading)
		{
			GameManager.TimeSlinger.KillTimer(annLoadPageTimer);
			aniLoadingPageStop();
		}
		if (myBrowser != null)
		{
			myBrowser.CookieManager.ClearAll();
			loadBrowserURL("localGame://blank.html");
		}
		lastURL = string.Empty;
		annURLBox.text = string.Empty;
		backHistory.Clear();
		forwardHistory.Clear();
		if (bookmarkMenuActive)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.Insert(0f, DOTween.To(() => BookmarksMenu.GetComponent<RectTransform>().localPosition, delegate(Vector3 x)
			{
				BookmarksMenu.GetComponent<RectTransform>().localPosition = x;
			}, new Vector3(237f, 0f, 0f), 0.01f).SetRelative(isRelative: true).SetEase(Ease.OutSine));
			sequence.Play();
			bookmarkMenuActive = false;
		}
	}

	protected override void OnMin()
	{
	}

	protected override void OnUnMin()
	{
	}

	protected override void OnMax()
	{
	}

	protected override void OnUnMax()
	{
	}

	protected override void OnResized()
	{
	}

	public void goOffLine()
	{
		if (isThePageLoading)
		{
			GameManager.TimeSlinger.KillTimer(annLoadPageTimer);
			aniLoadingPageStop();
		}
		if (myBrowser != null)
		{
			myBrowser.LoadURL("localGame://NotFound/index.html", force: false);
		}
		clearCurrentMusic();
	}

	private void loadBrowserURL(string setURL)
	{
		if (setURL != string.Empty)
		{
			myBrowser.LoadURL(setURL, force: false);
		}
	}

	private void pageLoaded(JSONNode obj)
	{
		if (GameManager.TheCloud.SoftValidateURL(out var returnURL, myBrowser.Url))
		{
			registerPageJS();
			checkToSeeIfPageHasMusic();
			checkToSeeIfPageIsTapped();
			thePrey();
			aniLoadingPageStop();
			checkToSeeIfPageIsBookmakred();
			annBTNCheck();
		}
		else
		{
			myBrowser.LoadURL(returnURL, force: true);
		}
	}

	public void aniLoadingPageStop()
	{
		aniLoadingBarSeq.Kill();
		aniLoadingGlobeSeq.Kill();
		LookUp.DesktopUI.ANN_WINDOW_LOADING_BAR.fillAmount = 0f;
		LookUp.DesktopUI.ANN_WINDOW_GLOBE.alpha = 1f;
		LookUp.DesktopUI.ANN_WINDOW_HOME_BTN.setLock(setValue: false);
		LookUp.DesktopUI.ANN_WINDOW_BACK_BTN.setLock(setValue: false);
		LookUp.DesktopUI.ANN_WINDOW_FORWARD_BTN.setLock(setValue: false);
		LookUp.DesktopUI.ANN_WINDOW_REFRESH_BTN.setLock(setValue: false);
		LookUp.DesktopUI.ANN_WINDOW_CODE_BTN.setLock(setValue: false);
		annURLBox.enabled = true;
		isThePageLoading = false;
	}

	private void checkToSeeIfPageHasMusic()
	{
		if (GameManager.TheCloud.GetCurrentWebPageDef() != null && GameManager.TheCloud.GetCurrentWebPageDef().HasMusic)
		{
			GameManager.AudioSlinger.PlaySound(GameManager.TheCloud.GetCurrentWebPageDef().AudioFile);
			isPlayingAudio = true;
		}
	}

	private void checkToSeeIfPageIsTapped()
	{
		if (!GameManager.TheCloud.CheckIfSiteWasTapped())
		{
			return;
		}
		WebPageDefinition pageDef = GameManager.TheCloud.GetCurrentWebPageDef();
		if (KeyPoll.keyManipulatorData == KEY_CUE_MODE.DEFAULT)
		{
			if (InventoryManager.OwnsKeyCue)
			{
				LookUp.DesktopUI.ANN_KEY_CUE.enabled = true;
			}
		}
		else if (KeyPoll.keyManipulatorData == KEY_CUE_MODE.ENABLED)
		{
			LookUp.DesktopUI.ANN_KEY_CUE.enabled = true;
		}
		KEY_DISCOVERY_MODES keyDiscoverMode = pageDef.KeyDiscoverMode;
		if (pageDef.isWTTG1Website)
		{
			switch (keyDiscoverMode)
			{
			case KEY_DISCOVERY_MODES.PLAIN_SIGHT:
			{
				int num = pageDef.HashIndex + 1;
				myBrowser.CallFunction("PlaceKey", num + " - " + pageDef.HashValue);
				break;
			}
			case KEY_DISCOVERY_MODES.CLICK_TO_PLAIN_SIGHT:
				myBrowser.CallFunction("PickHotSpot", string.Empty);
				myBrowser.RegisterFunction("HotSpotHit", delegate
				{
					GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.KeyFound);
					int num2 = pageDef.HashIndex + 1;
					myBrowser.CallFunction("PlaceKey", num2 + " - " + pageDef.HashValue);
				});
				break;
			case KEY_DISCOVERY_MODES.CLICK_TO_FILE:
				myBrowser.CallFunction("PickHotSpot", string.Empty);
				myBrowser.RegisterFunction("HotSpotHit", delegate
				{
					GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.KeyFound);
					GameManager.ManagerSlinger.TextDocManager.CreateTextDoc("Key" + (pageDef.HashIndex + 1) + ".txt", pageDef.HashIndex + 1 + " - " + pageDef.HashValue);
				});
				break;
			case KEY_DISCOVERY_MODES.SOURCE_CODE:
				break;
			}
		}
		else if (keyDiscoverMode != KEY_DISCOVERY_MODES.PLAIN_SIGHT)
		{
			switch (keyDiscoverMode)
			{
			case KEY_DISCOVERY_MODES.CLICK_TO_PLAIN_SIGHT:
				myBrowser.CallFunction("PickCPTag", string.Empty);
				myBrowser.RegisterFunction("PlainKeyShow", delegate
				{
					GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.KeyFound);
					myBrowser.CallFunction("PickPTag", new JSONNode(pageDef.HashIndex + 1), new JSONNode(pageDef.HashValue));
				});
				break;
			case KEY_DISCOVERY_MODES.CLICK_TO_FILE:
				myBrowser.CallFunction("PickCFTag", string.Empty);
				myBrowser.RegisterFunction("FileKeyShow", delegate
				{
					GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.KeyFound);
					GameManager.ManagerSlinger.TextDocManager.CreateTextDoc("Key" + (pageDef.HashIndex + 1) + ".txt", pageDef.HashIndex + 1 + " - " + pageDef.HashValue);
				});
				break;
			}
		}
		else
		{
			myBrowser.CallFunction("PickPTag", new JSONNode(pageDef.HashIndex + 1), new JSONNode(pageDef.HashValue));
		}
	}

	private void checkToSeeIfPageIsBookmakred()
	{
		if (GameManager.TheCloud.CheckToSeeIfPageIsBookMarked())
		{
			LookUp.DesktopUI.ANN_WINDOW_BOOKMARK_BTN.setBookmarked(nowBookmarked: true);
		}
		else
		{
			LookUp.DesktopUI.ANN_WINDOW_BOOKMARK_BTN.setBookmarked(nowBookmarked: false);
		}
	}

	private void clearCurrentMusic()
	{
		LookUp.DesktopUI.ANN_KEY_CUE.enabled = false;
		if (GameManager.TheCloud.GetCurrentWebPageDef() != null && GameManager.TheCloud.GetCurrentWebPageDef().HasMusic)
		{
			GameManager.AudioSlinger.KillSound(GameManager.TheCloud.GetCurrentWebPageDef().AudioFile);
			isPlayingAudio = false;
		}
		GameManager.TheCloud.ClearCurrentWebDeff();
		DeepWebRadioManager.radio = RADIO_TYPE.NONE;
		TutorialAnnHook.Ins.KillIdiotANNInit();
	}

	private void aniLoadingPage(float setLoadingTime)
	{
		aniLoadingBarSeq.Kill();
		aniLoadingGlobeSeq.Kill();
		aniLoadingGlobeSeq = DOTween.Sequence();
		aniLoadingGlobeSeq.Insert(0f, DOTween.To(() => LookUp.DesktopUI.ANN_WINDOW_GLOBE.alpha, delegate(float x)
		{
			LookUp.DesktopUI.ANN_WINDOW_GLOBE.alpha = x;
		}, 0.3f, 0.5f)).SetEase(Ease.Linear);
		aniLoadingGlobeSeq.Insert(0.5f, DOTween.To(() => LookUp.DesktopUI.ANN_WINDOW_GLOBE.alpha, delegate(float x)
		{
			LookUp.DesktopUI.ANN_WINDOW_GLOBE.alpha = x;
		}, 1f, 0.5f)).SetEase(Ease.Linear);
		aniLoadingGlobeSeq.SetLoops(-1);
		aniLoadingGlobeSeq.Play();
		LookUp.DesktopUI.ANN_WINDOW_LOADING_BAR.fillAmount = 0f;
		aniLoadingBarSeq = DOTween.Sequence();
		aniLoadingBarSeq.Insert(0f, DOTween.To(() => LookUp.DesktopUI.ANN_WINDOW_LOADING_BAR.fillAmount, delegate(float x)
		{
			LookUp.DesktopUI.ANN_WINDOW_LOADING_BAR.fillAmount = x;
		}, 1f, setLoadingTime));
		aniLoadingBarSeq.Play();
	}

	private void registerPageJS()
	{
		bool flag = false;
		myBrowser.RegisterFunction("LinkHover", delegate
		{
			if (!isThePageLoading)
			{
				GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: true);
			}
		});
		myBrowser.RegisterFunction("LinkOut", delegate
		{
			if (!isThePageLoading)
			{
				GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: false);
			}
		});
		myBrowser.RegisterFunction("LinkClick", delegate(JSONNode args)
		{
			if (!isThePageLoading)
			{
				GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.MouseClick);
				string pageFile = args[0];
				GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: false);
				pageLinkClick(pageFile);
			}
		});
		myBrowser.RegisterFunction("EmptyClick", delegate
		{
			if (!isThePageLoading)
			{
				GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.MouseClick);
			}
		});
		myBrowser.RegisterFunction("SiteClick", delegate(JSONNode args)
		{
			if (!isThePageLoading)
			{
				GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.MouseClick);
				string setURL = args[0];
				GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: false);
				GotoURL(setURL);
			}
		});
		myBrowser.RegisterFunction("KeyPress", delegate
		{
			if (!isThePageLoading)
			{
				int num = UnityEngine.Random.Range(1, LookUp.SoundLookUp.KeyboardSounds.Length);
				AudioFileDefinition audioFileDefinition = LookUp.SoundLookUp.KeyboardSounds[num];
				GameManager.AudioSlinger.PlaySound(audioFileDefinition);
				LookUp.SoundLookUp.KeyboardSounds[num] = LookUp.SoundLookUp.KeyboardSounds[num];
				LookUp.SoundLookUp.KeyboardSounds[0] = audioFileDefinition;
			}
		});
		myBrowser.RegisterFunction("HolyClick", delegate
		{
			if (!isThePageLoading)
			{
				GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: false);
				GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.HolyClick);
			}
		});
		myBrowser.RegisterFunction("puzzleClickedGood", delegate
		{
			if (!isThePageLoading)
			{
				GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: false);
				GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.PuzzleGoodClick);
			}
		});
		myBrowser.RegisterFunction("puzzleClickedWrong", delegate
		{
			if (!isThePageLoading)
			{
				GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: false);
				GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.PuzzleFailClick);
			}
		});
		myBrowser.RegisterFunction("puzzleSolved", delegate
		{
			if (!isThePageLoading)
			{
				GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: false);
				if (!Claffis)
				{
					GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.PuzzleSolved);
				}
				myBrowser.CallFunction(ChosenAwake ? "presentWiki2" : "presentWiki", new JSONNode(GameManager.TheCloud.GetWikiURL(2)));
			}
		});
		myBrowser.RegisterFunction("murderplayer", delegate
		{
			if (!DoneMurderPlayer)
			{
				DoneMurderPlayer = true;
				if (!isThePageLoading)
				{
					EnemyStateManager.AddEnemyState(ENEMY_STATE.CULT);
					CultComputerJumper.Ins.AddLightsOffJumpAndGo();
				}
			}
		});
		myBrowser.RegisterFunction("murderplayer2", delegate
		{
			if (!DoneMurderPlayer)
			{
				DoneMurderPlayer = true;
				if (!isThePageLoading)
				{
					EnemyStateManager.AddEnemyState(ENEMY_STATE.EXECUTIONER);
					EXEManager.Ins.TriggerJump();
					CultComputerJumper.Ins.JustGo();
				}
			}
		});
		myBrowser.RegisterFunction("sound_note", delegate(JSONNode args)
		{
			if (!isThePageLoading)
			{
				int piano = args[0];
				PlayClaffisPiano(piano);
			}
		});
		myBrowser.RegisterFunction("sound_success", delegate
		{
			if (!isThePageLoading)
			{
				GameManager.AudioSlinger.PlaySound(ClaffisLookUp.MysteryPiano);
				if (EnemyStateManager.HasEnemyState(ENEMY_STATE.EXECUTIONER))
				{
					EXESoundPopper.PopSound(199);
				}
			}
		});
		myBrowser.RegisterFunction("sound_failure", delegate
		{
			if (!isThePageLoading)
			{
				GameManager.AudioSlinger.PlaySound(ClaffisLookUp.pianoFail);
				if (EnemyStateManager.HasEnemyState(ENEMY_STATE.EXECUTIONER))
				{
					EXESoundPopper.PopSound(199);
				}
			}
		});
		myBrowser.RegisterFunction("TurnOffAllLights", delegate
		{
			if (!isThePageLoading)
			{
				EnemyManager.CultManager.TurnOffAllLightsDWA();
			}
		});
		myBrowser.RegisterFunction("installvpn", delegate
		{
			if (!isThePageLoading)
			{
				InstallGamingVPN();
			}
		});
		if (!PlayerPrefs.HasKey("[GAME]Nudity"))
		{
			myBrowser.CallFunction("ApplyNudityFilter", string.Empty);
		}
		if (PlayerPrefs.HasKey("[GAME]Nudity") && PlayerPrefs.GetInt("[GAME]Nudity") == 1)
		{
			myBrowser.CallFunction("ApplyNudityFilter", string.Empty);
		}
		if (GameManager.TheCloud.CheckIfWiki())
		{
			myBrowser.CallFunction("BuildWiki", GameManager.TheCloud.BuildCurrentWiki());
		}
		bool flag2 = false;
	}

	private void pageLinkClick(string pageFile)
	{
		string text = lastURL;
		text = text.Replace("http://", string.Empty);
		text = text.Replace(".ann", string.Empty);
		string[] array = text.Split(new string[1] { "/" }, StringSplitOptions.None);
		if (array.Length != 0)
		{
			text = "http://" + array[0] + ".ann/" + pageFile;
			ForceLoadURL(text);
		}
	}

	private void triggerBookmarksMenu()
	{
		if (bookmarkMenuAniActive)
		{
			return;
		}
		bookmarkMenuAniActive = true;
		GameManager.TimeSlinger.FireTimer(0.3f, delegate
		{
			bookmarkMenuAniActive = false;
		});
		if (bookmarkMenuActive)
		{
			bookmarkMenuActive = false;
			Sequence sequence = DOTween.Sequence();
			sequence.Insert(0f, DOTween.To(() => BookmarksMenu.GetComponent<RectTransform>().localPosition, delegate(Vector3 x)
			{
				BookmarksMenu.GetComponent<RectTransform>().localPosition = x;
			}, new Vector3(237f, 0f, 0f), 0.25f).SetRelative(isRelative: true).SetEase(Ease.OutSine));
			sequence.Play();
		}
		else
		{
			bookmarkMenuActive = true;
			Sequence sequence2 = DOTween.Sequence();
			sequence2.Insert(0f, DOTween.To(() => BookmarksMenu.GetComponent<RectTransform>().localPosition, delegate(Vector3 x)
			{
				BookmarksMenu.GetComponent<RectTransform>().localPosition = x;
			}, new Vector3(-237f, 0f, 0f), 0.25f).SetRelative(isRelative: true).SetEase(Ease.InSine));
			sequence2.Play();
		}
	}

	private void rePOSBookmarkTabs()
	{
		int num = 0;
		foreach (KeyValuePair<int, BookmarkTABObject> currentBookmarkTab in currentBookmarkTabs)
		{
			float setY = 0f - (28f * (float)num + 2f * (float)num);
			currentBookmarkTab.Value.RePOSMe(setY);
			num++;
		}
		bookmarksTabHolderSize.y = 30f * (float)currentBookmarkTabs.Count;
		BookmarksMenuTabHolder.GetComponent<RectTransform>().sizeDelta = bookmarksTabHolderSize;
	}

	private void refreshPage()
	{
		string text = lastURL;
		lastURL = string.Empty;
		annURLBox.text = text;
		GotoURL(text, AddHistory: false);
	}

	private void goBack()
	{
		if (backHistory.Count != 0)
		{
			forwardHistory.Add(lastURL);
			string text = backHistory[backHistory.Count - 1];
			backHistory.RemoveAt(backHistory.Count - 1);
			annURLBox.text = text;
			GotoURL(text, AddHistory: false);
		}
	}

	private void goForward()
	{
		if (forwardHistory.Count != 0)
		{
			backHistory.Add(lastURL);
			string text = forwardHistory[forwardHistory.Count - 1];
			forwardHistory.RemoveAt(forwardHistory.Count - 1);
			annURLBox.text = text;
			GotoURL(text, AddHistory: false);
		}
	}

	private void annBTNCheck()
	{
		LookUp.DesktopUI.ANN_WINDOW_BACK_BTN.setActive(backHistory.Count > 0);
		LookUp.DesktopUI.ANN_WINDOW_FORWARD_BTN.setActive(forwardHistory.Count > 0);
		if (lastURL != string.Empty)
		{
			LookUp.DesktopUI.ANN_WINDOW_REFRESH_BTN.setActive(setValue: true);
			LookUp.DesktopUI.ANN_WINDOW_CODE_BTN.setActive(setValue: true);
		}
		else
		{
			LookUp.DesktopUI.ANN_WINDOW_REFRESH_BTN.setActive(setValue: false);
			LookUp.DesktopUI.ANN_WINDOW_CODE_BTN.setActive(setValue: false);
		}
	}

	private void stageMe()
	{
		bool flag = false;
		AudioFileDefinition[] piano = ClaffisLookUp.piano;
		foreach (AudioFileDefinition audioFileDefinition in piano)
		{
			audioFileDefinition.MyAudioLayer = AUDIO_LAYER.WEBSITE;
		}
		ClaffisLookUp.pianoFail.MyAudioLayer = AUDIO_LAYER.WEBSITE;
		ClaffisLookUp.MysteryPiano.MyAudioLayer = AUDIO_LAYER.WEBSITE;
		bool flag2 = false;
		GameManager.StageManager.TheGameIsLive -= stageMe;
		GameManager.ManagerSlinger.WifiManager.WentOnline += userWentOnline;
		GameManager.ManagerSlinger.WifiManager.WentOffline += userWentOffLine;
	}

	private void userWentOnline()
	{
		userIsOffline = false;
	}

	private void userWentOffLine()
	{
		userIsOffline = true;
		goOffLine();
	}

	public void TwitchTrollURL(string SetURL)
	{
		goOffLine();
		myBrowser.LoadURL(SetURL, force: true);
	}

	private void thePrey()
	{
		switch (GameManager.TheCloud.GetCurrentWebPageDef()?.PageName.ToLower().Replace(" ", ""))
		{
		case "theprey":
			EnemyManager.CultManager.attemptSpawn();
			break;
		case "deepwebradiojazz":
			DeepWebRadioManager.radio = RADIO_TYPE.ANONYJAZZ;
			break;
		case "deepwebradiodsbm":
			DeepWebRadioManager.radio = RADIO_TYPE.DSBM;
			break;
		case "youareanidiot":
			TutorialAnnHook.Ins.StartIdiotANNInit();
			break;
		case "iamthegreatest":
			GameManager.HackerManager.virusManager.ForceVirus();
			break;
		case "vengeanceangel":
			if (GameManager.TheCloud.GetCurrentWebPageDef()?.FileName == "freedom.html" && !DifficultyManager.CasualMode)
			{
				GameManager.TimeSlinger.FireTimer(UnityEngine.Random.Range(300f, 600f), doHarbinger);
			}
			break;
		case "shadowfaces":
			if (GameManager.TheCloud.GetCurrentWebPageDef()?.FileName == "index.html")
			{
				EnemyManager.CultManager.turnOffAllLights();
			}
			break;
		}
	}

	private void doHarbinger()
	{
		if (!PhoneManager.HarbingerHappened)
		{
			if (EnemyStateManager.IsInEnemyStateOrLocked())
			{
				GameManager.TimeSlinger.FireTimer(10f, doHarbinger);
				return;
			}
			if (StateManager.PlayerLocation != PLAYER_LOCATION.MAIN_ROON)
			{
				GameManager.TimeSlinger.FireTimer(25f, doHarbinger);
				return;
			}
			if (StateManager.PlayerState != PLAYER_STATE.COMPUTER)
			{
				GameManager.TimeSlinger.FireTimer(1f, doHarbinger);
				return;
			}
			PhoneManager.HarbingerHappened = true;
			GameManager.ManagerSlinger.TextDocManager.CreateTextDoc("Harbinger.txt", "I know who you are.");
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.keyFound2);
			PhoneManager.Ins.StageHarbinger();
		}
	}

	private void InstallGamingVPN()
	{
		if (!InstalledGamingVPN)
		{
			InstalledGamingVPN = true;
			GameManager.ManagerSlinger.TextDocManager.CreateGamingVPN();
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.keyFound2);
		}
	}

	private void PlayClaffisPiano(int piano)
	{
		bool flag = false;
		if (EnemyStateManager.HasEnemyState(ENEMY_STATE.EXECUTIONER))
		{
			EXESoundPopper.PopSound(199);
		}
		GameManager.AudioSlinger.PlaySound(ClaffisLookUp.piano[piano - 1]);
	}
}
