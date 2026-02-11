using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HackerModeManager : MonoBehaviour
{
	public static HackerModeManager Ins;

	public static HACK_TYPE currenthax;

	private const string _nodeTopScoreID = "[HACKERMODE]Score1";

	private const string _stackTopScoreID = "[HACKERMODE]Score2";

	private const string _vapeTopScoreID = "[HACKERMODE]Score3";

	private const string _dosTopScoreID = "[HACKERMODE]Score4";

	public HMLookUp lookUp;

	public bool sfxMuted;

	public int SkillPoints;

	public int chainCount;

	private int bannedSeconds;

	private float ChainModeYAxis;

	private string[] cloudGridLeaders;

	private string[] dosBlockerLeaders;

	private bool firstTIME;

	private GameObject gameOver;

	private HMGameOverLookUp gameOverLookUp;

	private bool glitchEnabled;

	private GameObject hackerModeMenu;

	private AudioClip hackMusic;

	private AudioClip menuMusic;

	private AudioSource menuMusicSource;

	private string[] nodeHexerLeaders;

	private float QuitHolderYAxis;

	private bool shouldTrySubmitScore;

	private string[] stackPusherLeaders;

	private int timeStarted;

	private bool presetPanelBusy;

	private bool presetPanelOpen;

	public static CUSTOM_HACK CurrentCustomHack;

	private void Awake()
	{
		Ins = this;
		menuMusicSource = base.gameObject.AddComponent<AudioSource>();
	}

	private void OnDisable()
	{
		SkillPoints = 0;
		chainCount = 0;
		SaveScores();
		SaveSettings();
		HMCustomHack.IsCustomHack = false;
	}

	private void SaveSettings()
	{
		PlayerPrefs.SetFloat("HMMusicVol", lookUp.musicSlider.slider.value);
		PlayerPrefs.SetInt("HMSoundOn", (!sfxMuted) ? 1 : 0);
		PlayerPrefs.SetInt("HMGlitchOn", glitchEnabled ? 1 : 0);
	}

	private void LoadSettings()
	{
		if (PlayerPrefs.HasKey("HMMusicVol"))
		{
			lookUp.musicSlider.slider.value = PlayerPrefs.GetFloat("HMMusicVol");
		}
		if (PlayerPrefs.HasKey("HMSoundOn"))
		{
			bool active = PlayerPrefs.GetInt("HMSoundOn") == 0;
			lookUp.sfxButton.SetActive(active);
			sfxMuted = active;
		}
		if (PlayerPrefs.HasKey("HMGlitchOn"))
		{
			bool flag = PlayerPrefs.GetInt("HMGlitchOn") == 0;
			lookUp.glitchButton.SetActive(flag);
			glitchEnabled = !flag;
		}
	}

	public void GameLoaded()
	{
		Debug.Log("Hacker Mode Loaded");
		menuMusic = CustomSoundLookUp.HackerModeMenuMusic;
		hackMusic = CustomSoundLookUp.hackermode;
		glitchEnabled = true;
		SkillPoints = 0;
		chainCount = 0;
		GameObject.Find("computerAudioHub").transform.position = Vector3.zero;
		menuMusicSource.loop = true;
		menuMusicSource.volume = 0.8f;
		hackerModeMenu = UnityEngine.Object.Instantiate(CustomObjectLookUp.HackerModeMenu, WallpaperUtils.desktopWallpaper.transform.parent.gameObject.transform.parent);
		gameOver = UnityEngine.Object.Instantiate(CustomObjectLookUp.hackerModeGameOver, WallpaperUtils.desktopWallpaper.transform.parent.gameObject.transform.parent);
		gameOver.SetActive(value: false);
		gameOverLookUp = gameOver.GetComponent<HMGameOverLookUp>();
		gameOverLookUp.menuButton.setMyAction(GameOverReturn);
		RectTransform component = hackerModeMenu.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 0f);
		component.anchorMax = new Vector2(1f, 1f);
		component.pivot = new Vector2(0.5f, 0.5f);
		component.anchoredPosition = new Vector2(0f, 0f);
		lookUp = hackerModeMenu.GetComponent<HMLookUp>();
		PauseManager.LockPause();
		lookUp.customHack.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Screen.height * -1);
		if (Screen.height <= 800)
		{
			lookUp.bannedHolder.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			lookUp.chainModeHolder.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
			lookUp.quitHolder.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
			GameObject.Find("TOP SCORES").transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
			GameObject.Find("VarHolder").transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
			GameObject.Find("HMMenu(Clone)/Logo").transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
			GameObject.Find("CustomHackMenu").transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
		}
		else if (Screen.height <= 1000)
		{
			lookUp.bannedHolder.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
			lookUp.chainModeHolder.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
			lookUp.quitHolder.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
			GameObject.Find("TOP SCORES").transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
			GameObject.Find("VarHolder").transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
			GameObject.Find("HMMenu(Clone)/Logo").transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
			GameObject.Find("CustomHackMenu").transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
		}
		LoadKeys();
		LoadScores();
		CursorManager.Ins.SwitchToHackerCursor();
		lookUp.nodeHexerStart.setMyAction(nodeHexerButton);
		lookUp.stackPusherStart.setMyAction(stackPusherButton);
		lookUp.cloudGridStart.setMyAction(cloudGridButton);
		lookUp.dosBlockerStart.setMyAction(dosBlockerButton);
		lookUp.quitGame.setMyAction(quitGame);
		lookUp.quitToMenu.setMyAction(exitToMainMenu);
		lookUp.customHack.closeBTN.setMyAction(CloseCustomHack);
		lookUp.nodeHexerCustom.setMyAction(PresentCustomNodeHexer);
		lookUp.stackPusherCustom.setMyAction(PresentCustomStackPusher);
		lookUp.cloudGridCustom.setMyAction(PresentCustomVapeAttack);
		lookUp.dosBlockerCustom.setMyAction(PresentCustomDosBlocker);
		lookUp.glitchButton.setMyAction(GlitchButton);
		lookUp.sfxButton.setMyAction(SFXButton);
		lookUp.presetsBTN.setMyAction(TogglePresets);
		ChainModeYAxis = lookUp.chainModeHolder.transform.position.y;
		QuitHolderYAxis = lookUp.quitHolder.transform.position.y;
		DestroyUnusedObjs();
		ChangeMusic(HackerModeMusicType.menu);
		LoadSettings();
		RandomGlitch();
	}

	private void RandomGlitch()
	{
		if (!StateManager.BeingHacked && glitchEnabled)
		{
			ComputerCameraManager.Ins.TriggerHackingTerminalDumpGlitch();
			GameManager.TimeSlinger.FireTimer(UnityEngine.Random.Range(2, 4), ComputerCameraManager.Ins.ClearPostFXs);
		}
		GameManager.TimeSlinger.FireTimer(UnityEngine.Random.Range(6, 12), RandomGlitch);
	}

	public static void quitGame()
	{
		Application.Quit();
	}

	public static void exitToMainMenu()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(1);
	}

	public void TogglePresets()
	{
		if (!presetPanelBusy)
		{
			presetPanelOpen = !presetPanelOpen;
			if (presetPanelOpen)
			{
				lookUp.presetsHolder.GetComponent<RectTransform>().DOAnchorPosX(0f, 0.5f);
			}
			if (!presetPanelOpen)
			{
				lookUp.presetsHolder.GetComponent<RectTransform>().DOAnchorPosX(-252f, 0.5f);
			}
			presetPanelBusy = true;
			GameManager.TimeSlinger.FireTimer(0.5f, delegate
			{
				presetPanelBusy = false;
			});
		}
	}

	public void nodeHexerButton()
	{
		LaunchHackFromButton(HACK_TYPE.NODEHEXER);
	}

	public void stackPusherButton()
	{
		LaunchHackFromButton(HACK_TYPE.STACKPUSHER);
	}

	public void musicButton()
	{
	}

	private void DestroyUnusedObjs()
	{
		SceneManager.UnloadSceneAsync(4);
		DestroyTheObject("LoadingScreenManager");
		DestroyTheObject("WorldManager");
		DestroyTheObject("WifiDongleHotspot1");
		DestroyTheObject("WifiDongleHotspot2");
		DestroyTheObject("WifiDongleHotspot3");
		DestroyTheObject("WifiDongleHotspot4");
		DestroyTheObject("UBER_PresetReference");
		DestroyTheObject("startController");
		DestroyTheObject("LOLPYDiscParent");
		DestroyTheObject("PackagesParent");
		DestroyTheObject("PoliceScannerParent");
		DestroyTheObject("MainCameraGlobalParent");
		DestroyTheObject("RemoteVPNParent");
		DestroyTheObject("roamController");
		DestroyTheObject("WifiDONGLE");
		DestroyTheObject("WifiDongleHotspot2");
		DestroyTheObject("lobbyComputerCamera");
		DestroyTheObject("UILobbyComputer");
		DestroyTheObject("UICanvas");
		DestroyTheObject("Managers/Managers");
		DestroyTheObject("TimeKeeperManager");
		DestroyTheObject("TutorialManager");
		DestroyTheObject("InteractionManager");
		DestroyTheObject("Dialogs");
		DestroyTheObject("Behaviours");
		DestroyTheObject("SkyBreakBehavior");
		DestroyTheObject("UIInteractionManager");
		DestroyTheObject("Docs");
		DestroyTheObject("SulphurPackageObject");
		DestroyTheObject("Router(Clone)");
		DestroyTheObject("TarotCards(Clone)");
		DestroyTheObject("Hooks");
		DestroyTheObject("Lookups");
		DestroyTheObject("TheCloud");
		DestroyTheObject("PowerOverlay");
		DestroyTheObject("MemDefrag");
		DestroyTheObject("shadowIcon");
		DestroyTheObject("zeroDayIcon");
		DestroyTheObject("ADOS");
		DestroyTheObject("BotBar");
		DestroyTheObject("WifiButton/hoverIMG");
		DestroyTheObject("PowerButton/hoverIMG");
		DestroyTheObject("SoundButton/hoverIMG");
		DestroyTheObject("MotionSensorButton");
		DestroyTheObject("VPNButton");
		DestroyTheObject("DOSCoinBTN");
		DestroyTheObject("BackDoorIMG");
		DestroyTheObject("CurrentBackDoors");
		DestroyTheObject("WallPaper");
		DestroyTheObject("IconsHolder");
		DestroyTheObject("WindowHolder");
		DestroyTheObject("MotionSensorMenu");
		DestroyTheObject("CurrencyMenu");
		DestroyTheObject("WifiMenu");
		DestroyTheObject("VPNMenu");
		DestroyTheObject("TopBar");
		UnityEngine.Object.Destroy(GameObject.Find("MotionSensorFinal(Clone)").transform.parent.gameObject);
	}

	public static void DestroyTheObject(string name)
	{
		UnityEngine.Object.Destroy(GameObject.Find(name));
	}

	public void cloudGridButton()
	{
		LaunchHackFromButton(HACK_TYPE.CLOUDGRID);
	}

	public void dosBlockerButton()
	{
		LaunchHackFromButton(HACK_TYPE.DOSBLOCK);
	}

	public void ChangeMusic(HackerModeMusicType type)
	{
		menuMusicSource.Stop();
		switch (type)
		{
		case HackerModeMusicType.menu:
			menuMusicSource.clip = menuMusic;
			menuMusicSource.Play();
			break;
		case HackerModeMusicType.hack:
			menuMusicSource.clip = hackMusic;
			menuMusicSource.Play();
			break;
		}
	}

	public void GameOverMusic()
	{
		ChangeMusic(HackerModeMusicType.menu);
		menuMusicSource.volume = 0f;
		if (lookUp.musicSlider.volume != 0f)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.Insert(0f, DOTween.To(() => menuMusicSource.volume, delegate(float x)
			{
				menuMusicSource.volume = x;
			}, lookUp.musicSlider.volume, 3f));
			sequence.Play();
		}
	}

	public void PlayGameOverSound(HMGameOverSound type)
	{
		switch (type)
		{
		case HMGameOverSound.totalPoints:
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.totalPointSlide);
			break;
		case HMGameOverSound.finalPoints:
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.finalPointsImpact);
			break;
		case HMGameOverSound.timeShow:
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.timeShow);
			break;
		case HMGameOverSound.newHighScore:
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.newHighScore);
			break;
		case HMGameOverSound.gameOverImpact:
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.gameOverTrigger);
			break;
		case HMGameOverSound.gameOver:
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.gameOver);
			break;
		}
	}

	private void GameOverReturn()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => gameOver.GetComponent<CanvasGroup>().alpha, delegate(float x)
		{
			gameOver.GetComponent<CanvasGroup>().alpha = x;
		}, 0f, 1f));
		sequence.Play();
		GameManager.TimeSlinger.FireTimer(1f, delegate
		{
			gameOver.SetActive(value: false);
			gameOverLookUp.flash.CrossFadeAlpha(1f, 0f, ignoreTimeScale: true);
			gameOverLookUp.levelOBJ.SetActive(value: false);
			gameOverLookUp.countOBJ.SetActive(value: false);
			gameOverLookUp.finalScoreOBJ.SetActive(value: false);
			gameOverLookUp.menuButton.gameObject.SetActive(value: false);
			gameOverLookUp.timeOBJ.GetComponent<CanvasGroup>().alpha = 0f;
			gameOverLookUp.menuButton.GetComponent<CanvasGroup>().alpha = 0f;
			gameOverLookUp.newHighScore.SetActive(value: false);
		});
	}

	public void ShowGameOver()
	{
		int chainLevel = GameManager.HackerManager.GetChainLevel;
		int chainCount = this.chainCount - 1;
		int skillPoints = SkillPoints;
		int totalPoints = chainCount * 50 + chainLevel * 10 + skillPoints;
		int score = GetScore(currenthax);
		Debug.Log("[HackerMode] Game Over! Skill points: " + skillPoints);
		SkillPoints = 0;
		this.chainCount = 0;
		gameOverLookUp.timeOBJ.GetComponent<CanvasGroup>().alpha = 0f;
		gameOver.SetActive(value: true);
		gameOver.GetComponent<CanvasGroup>().alpha = 1f;
		gameOverLookUp.time.text = cockstring();
		GameOverMusic();
		PlayGameOverSound(HMGameOverSound.gameOverImpact);
		bool isNewHighScore = totalPoints > score;
		GameManager.TimeSlinger.FireTimer(1.3f, delegate
		{
			PlayGameOverSound(HMGameOverSound.gameOver);
		});
		gameOverLookUp.flash.gameObject.SetActive(value: true);
		gameOverLookUp.flash.CrossFadeAlpha(0f, 1f, ignoreTimeScale: false);
		GameManager.TimeSlinger.FireTimer(1f, delegate
		{
			gameOverLookUp.flash.gameObject.SetActive(value: false);
		});
		GameManager.TimeSlinger.FireTimer(4f, delegate
		{
			PlayGameOverSound(HMGameOverSound.totalPoints);
			gameOverLookUp.level.text = (chainLevel + 1).ToString();
			gameOverLookUp.levelOBJ.SetActive(value: true);
			gameOverLookUp.levelOBJ.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
			gameOverLookUp.levelOBJ.transform.DOScale(new Vector3(1f, 1f, 1f), 0.3f);
		});
		GameManager.TimeSlinger.FireTimer(4.75f, delegate
		{
			PlayGameOverSound(HMGameOverSound.totalPoints);
			gameOverLookUp.count.text = chainCount.ToString();
			gameOverLookUp.countOBJ.SetActive(value: true);
			gameOverLookUp.countOBJ.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
			gameOverLookUp.countOBJ.transform.DOScale(new Vector3(1f, 1f, 1f), 0.3f);
		});
		GameManager.TimeSlinger.FireTimer(5.5f, delegate
		{
			PlayGameOverSound(HMGameOverSound.finalPoints);
			gameOverLookUp.finalScore.text = totalPoints.ToString();
			gameOverLookUp.finalScoreOBJ.SetActive(value: true);
			gameOverLookUp.finalScoreOBJ.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
			gameOverLookUp.finalScoreOBJ.transform.DOScale(new Vector3(1f, 1f, 1f), 0.3f);
		});
		GameManager.TimeSlinger.FireTimer(8f, delegate
		{
			if (isNewHighScore)
			{
				PlayGameOverSound(HMGameOverSound.newHighScore);
				gameOverLookUp.newHighScore.SetActive(value: true);
				gameOverLookUp.newHighScore.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
				gameOverLookUp.newHighScore.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f);
				UpdateHighScore(totalPoints);
			}
		});
		GameManager.TimeSlinger.FireTimer(10f, delegate
		{
			PlayGameOverSound(HMGameOverSound.timeShow);
			Sequence sequence = DOTween.Sequence();
			sequence.Insert(0f, DOTween.To(() => gameOverLookUp.timeOBJ.GetComponent<CanvasGroup>().alpha, delegate(float x)
			{
				gameOverLookUp.timeOBJ.GetComponent<CanvasGroup>().alpha = x;
			}, 1f, 1f));
			sequence.Play();
		});
		GameManager.TimeSlinger.FireTimer(11f, delegate
		{
			gameOverLookUp.menuButton.gameObject.SetActive(value: true);
			Sequence sequence = DOTween.Sequence();
			sequence.Insert(0f, DOTween.To(() => gameOverLookUp.menuButton.GetComponent<CanvasGroup>().alpha, delegate(float x)
			{
				gameOverLookUp.menuButton.GetComponent<CanvasGroup>().alpha = x;
			}, 1f, 1f));
			sequence.Play();
		});
	}

	private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
	{
		return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp);
	}

	private void PresentBanHammerInfo()
	{
		if (lookUp.bannedHolder.transform.position.x == -302f)
		{
			lookUp.bannedHolder.transform.DOMoveX(0f, 1f);
		}
	}

	private void DeactivateBanHammerInfo()
	{
		if (lookUp.bannedHolder.transform.position.x == 0f)
		{
			lookUp.bannedHolder.transform.DOMoveX(-302f, 1f);
		}
	}

	private string cockstring()
	{
		int num = TimeSlinger.CurrentTimestamp - timeStarted;
		int num2 = num % 86400 / 3600;
		int num3 = num % 3600 / 60;
		int num4 = num % 60;
		return $"{num2:D2}:{num3:D2}:{num4:D2}";
	}

	public void CloseCustomHack()
	{
		lookUp.customHack.Close();
		GameManager.TimeSlinger.FireTimer(1f, delegate
		{
			lookUp.chainModeHolder.transform.DOMoveY(ChainModeYAxis, 1f);
		});
		lookUp.customHack.transform.DOMoveY((float)Screen.height * -1.5f, 1f);
		GameManager.TimeSlinger.FireTimer(2f, delegate
		{
			lookUp.quitHolder.transform.DOMoveY(QuitHolderYAxis, 0.2f);
		});
	}

	public void PresentCustomNodeHexer()
	{
		PresentCustomHackMenu(CUSTOM_HACK.NODE_HEXER);
	}

	public void PresentCustomStackPusher()
	{
		PresentCustomHackMenu(CUSTOM_HACK.STACK_PUSHER);
	}

	public void PresentCustomVapeAttack()
	{
		PresentCustomHackMenu(CUSTOM_HACK.VAPE_ATTACK);
	}

	public void PresentCustomDosBlocker()
	{
		PresentCustomHackMenu(CUSTOM_HACK.DOS_BLOCKER);
	}

	public void PresentCustomHackMenu(CUSTOM_HACK hack)
	{
		CurrentCustomHack = hack;
		lookUp.level11button.SetActive(CurrentCustomHack == CUSTOM_HACK.STACK_PUSHER || CurrentCustomHack == CUSTOM_HACK.VAPE_ATTACK);
		lookUp.level12button.SetActive(CurrentCustomHack == CUSTOM_HACK.STACK_PUSHER || CurrentCustomHack == CUSTOM_HACK.VAPE_ATTACK);
		lookUp.customHack.PresentHackOptions(hack);
		GameManager.TimeSlinger.FireTimer(0.2f, delegate
		{
			lookUp.chainModeHolder.transform.DOMoveY((float)Screen.height * -1.5f, 1f);
		});
		lookUp.quitHolder.transform.DOMoveY((float)Screen.height * -1.3f, 0.2f);
		GameManager.TimeSlinger.FireTimer(1.2f, delegate
		{
			lookUp.customHack.transform.DOMoveY(ChainModeYAxis - 40f, 1f);
		});
	}

	public void GlitchButton()
	{
		glitchEnabled = !glitchEnabled;
		ComputerCameraManager.Ins.ClearPostFXs();
	}

	public void SFXButton()
	{
		sfxMuted = !sfxMuted;
	}

	public void LaunchHackFromButton(HACK_TYPE hACK, bool isCustom = false)
	{
		SkillPoints = 0;
		chainCount = 0;
		HMCustomHack.IsCustomHack = isCustom;
		timeStarted = TimeSlinger.CurrentTimestamp;
		ComputerCameraManager.Ins.ClearPostFXs();
		currenthax = hACK;
		GameManager.HackerManager.LaunchTestHack(currenthax);
		ChangeMusic(HackerModeMusicType.hack);
	}

	private void UpdateHighScore(int newHighScore)
	{
		SetScore(currenthax, newHighScore);
		SaveScores();
	}

	public int GetScore(HACK_TYPE hax)
	{
        switch (hax)
        {
            case HACK_TYPE.STACKPUSHER:
                return int.Parse(lookUp.stackPusherTop.text);
            case HACK_TYPE.NODEHEXER:
                return int.Parse(lookUp.nodeHexerTop.text);
            case HACK_TYPE.DOSBLOCK:
                return int.Parse(lookUp.dosBlockerTop.text);
            case HACK_TYPE.CLOUDGRID:
                return int.Parse(lookUp.cloudGridTop.text);
            default:
                return 0;
        }
    }

    public void SetScore(HACK_TYPE hax, int value)
	{
		switch (hax)
		{
		case HACK_TYPE.STACKPUSHER:
			lookUp.stackPusherTop.text = value.ToString();
			break;
		case HACK_TYPE.NODEHEXER:
			lookUp.nodeHexerTop.text = value.ToString();
			break;
		case HACK_TYPE.DOSBLOCK:
			lookUp.dosBlockerTop.text = value.ToString();
			break;
		case HACK_TYPE.CLOUDGRID:
			lookUp.cloudGridTop.text = value.ToString();
			break;
		}
	}

	public void LoadScores()
	{
		int num = PlayerPrefs.GetInt("[HACKERMODE]Score1");
		int num2 = PlayerPrefs.GetInt("[HACKERMODE]Score2");
		int num3 = PlayerPrefs.GetInt("[HACKERMODE]Score3");
		int num4 = PlayerPrefs.GetInt("[HACKERMODE]Score4");
		num = (((num - 6) % 137 == 0) ? ((num - 6) / 137) : 0);
		num2 = (((num2 - 9) % 137 == 0) ? ((num2 - 9) / 137) : 0);
		num3 = (((num3 - 4) % 137 == 0) ? ((num3 - 4) / 137) : 0);
		num4 = (((num4 - 2) % 137 == 0) ? ((num4 - 2) / 137) : 0);
		lookUp.nodeHexerTop.text = num.ToString();
		lookUp.stackPusherTop.text = num2.ToString();
		lookUp.cloudGridTop.text = num3.ToString();
		lookUp.dosBlockerTop.text = num4.ToString();
	}

	public void SaveScores()
	{
		PlayerPrefs.SetInt("[HACKERMODE]Score1", int.Parse(lookUp.nodeHexerTop.text) * 137 + 6);
		PlayerPrefs.SetInt("[HACKERMODE]Score2", int.Parse(lookUp.stackPusherTop.text) * 137 + 9);
		PlayerPrefs.SetInt("[HACKERMODE]Score3", int.Parse(lookUp.cloudGridTop.text) * 137 + 4);
		PlayerPrefs.SetInt("[HACKERMODE]Score4", int.Parse(lookUp.dosBlockerTop.text) * 137 + 2);
		if (int.Parse(lookUp.nodeHexerTop.text) >= 1000 && int.Parse(lookUp.stackPusherTop.text) >= 1000 && int.Parse(lookUp.cloudGridTop.text) >= 1000 && int.Parse(lookUp.dosBlockerTop.text) >= 1000)
		{
			PlayerPrefs.SetInt("HackermodeTrophy1", 1);
		}
		if (int.Parse(lookUp.nodeHexerTop.text) >= 5000 && int.Parse(lookUp.stackPusherTop.text) >= 5000 && int.Parse(lookUp.cloudGridTop.text) >= 5000 && int.Parse(lookUp.dosBlockerTop.text) >= 5000)
		{
			PlayerPrefs.SetInt("HackermodeTrophy2", 2);
		}
		if (int.Parse(lookUp.nodeHexerTop.text) >= 10000 && int.Parse(lookUp.stackPusherTop.text) >= 10000 && int.Parse(lookUp.cloudGridTop.text) >= 10000 && int.Parse(lookUp.dosBlockerTop.text) >= 10000)
		{
			PlayerPrefs.SetInt("HackermodeTrophy3", 3);
		}
	}

	private void LoadKeys()
	{
		if (!PlayerPrefs.HasKey("[HACKERMODE]Score1"))
		{
			PlayerPrefs.SetInt("[HACKERMODE]Score1", 0);
		}
		if (!PlayerPrefs.HasKey("[HACKERMODE]Score2"))
		{
			PlayerPrefs.SetInt("[HACKERMODE]Score2", 0);
		}
		if (!PlayerPrefs.HasKey("[HACKERMODE]Score3"))
		{
			PlayerPrefs.SetInt("[HACKERMODE]Score3", 0);
		}
		if (!PlayerPrefs.HasKey("[HACKERMODE]Score4"))
		{
			PlayerPrefs.SetInt("[HACKERMODE]Score4", 0);
		}
	}

	public void SetMusicVolume(float vol)
	{
		menuMusicSource.volume = vol;
	}
}
