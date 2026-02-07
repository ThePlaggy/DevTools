using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
	public static TitleManager Ins;

	public static bool easter_randchance;

	[SerializeField]
	private float stageTime = 0.5f;

	[SerializeField]
	private CanvasGroup bgScreen;

	[SerializeField]
	private CanvasGroup blackScreen;

	[SerializeField]
	public AudioFileDefinition titleMusic;

	[HideInInspector]
	public GameObject newCanvas;

	private bool loadGameActive;

	private float loadTimeStamp;

	public CustomEvent OptionsDismissed = new CustomEvent(5);

	public CustomEvent OptionsDismissing = new CustomEvent(5);

	public CustomEvent OptionsPresent = new CustomEvent(5);

	public CustomEvent OptionsPresented = new CustomEvent(5);

	private bool stageTimeActive;

	private float startTimeStamp;

	public CustomEvent TitleDismissing = new CustomEvent(5);

	public CustomEvent TitlePresent = new CustomEvent(5);

	public CustomEvent TitlePresented = new CustomEvent(5);

	public CustomEvent TitleStaging = new CustomEvent(5);

	public AudioSource musicAS;

	public TitleMainMenuHook MainMenu { get; set; }

	private void Awake()
	{
		Ins = this;
		GameObject.Find("Ver").GetComponent<RectTransform>().sizeDelta = new Vector2(240f, 50f);
		GameObject.Find("Ver").GetComponent<TextMeshProUGUI>().text = "v1.614";
		LookUps.TitleFont = GameObject.Find("TitleText").GetComponent<TextMeshProUGUI>().font;
		musicAS = new GameObject("MusicSource").AddComponent<AudioSource>();
	}

	private void Start()
	{
		TitleStaging.Execute();
		startTimeStamp = Time.time;
		stageTimeActive = true;
		if (AssetBundleManager.loaded)
		{
			Ins.PlayMusic();
		}
	}

	private void Update()
	{
		if (loadGameActive && Time.time - loadTimeStamp >= 2.4f)
		{
			loadGameActive = false;
			loadGame();
		}
	}

	public void LogoWasPresented()
	{
		CursorManager.Ins.EnableCursor();
		CursorManager.Ins.SwitchToCustomCursor();
		TitlePresented.Execute();
	}

	public void DismissTitle()
	{
		DOTween.To(() => blackScreen.alpha, delegate(float x)
		{
			blackScreen.alpha = x;
		}, 1f, 0.5f).SetEase(Ease.Linear).SetDelay(1.8f);
		GameManager.AudioSlinger.MuffleAudioHub(AUDIO_HUB.TITLE_HUB, 0f, 2.3f);
		musicAS.DOFade(0f, 2.3f);
		TitleDismissing.Execute();
		CursorManager.Ins.DisableCursor();
		CursorManager.Ins.SwitchToDefaultCursor();
		loadTimeStamp = Time.time;
		loadGameActive = true;
	}

	public void PresentOptions()
	{
		OptionsPresent.Execute();
	}

	public void presentTitle()
	{
		TitlePresent.Execute();
	}

	private void loadGame()
	{
		SceneManager.LoadScene(2, LoadSceneMode.Single);
	}

	public void LoadMusic()
	{
		MusicLookUp.WTTG2Music = titleMusic.AudioClip;
		PlayMusic();
	}

	public void PlayMusic()
	{
		if (PlayerPrefs.HasKey("[TITLE]TitleMusicID"))
		{
			switch (PlayerPrefs.GetInt("[TITLE]TitleMusicID"))
			{
			case 0:
				titleMusic.AudioClip = MusicLookUp.WTTG1Music;
				break;
			case 1:
				titleMusic.AudioClip = (easter_randchance ? MusicLookUp.TWRSecretMusic : MusicLookUp.TWRMusic);
				if (easter_randchance)
				{
					Debug.Log("Secret");
				}
				break;
			case 2:
				titleMusic.AudioClip = MusicLookUp.WTTG2Music;
				break;
			case 3:
				titleMusic.AudioClip = MusicLookUp.ScrutinizedMusic;
				break;
			case 4:
				titleMusic.AudioClip = MusicLookUp.DeadSignalMusic;
				break;
			case 5:
				titleMusic.AudioClip = MusicLookUp.TheLastSightMusic;
				break;
			case -2:
				titleMusic.AudioClip = ((PlayerPrefs.HasKey("[MOD]MenuTheme") && PlayerPrefs.GetInt("[MOD]MenuTheme") == 0) ? MusicLookUp.WTTG1Music : MusicLookUp.WTTG2Music);
				break;
			case -3:
				titleMusic.AudioClip = GetMusicWithID(Random.Range(0, 6));
				break;
			}
		}
		else
		{
			PlayerPrefs.SetInt("[TITLE]TitleMusicID", 2);
			titleMusic.AudioClip = MusicLookUp.WTTG2Music;
		}
		if (PlayerPrefs.GetInt("[TITLE]TitleMusicID") != -1)
		{
			PlayMusicThroughSource(titleMusic.AudioClip);
			musicAS.volume = PlayerPrefs.GetFloat("[TITLE]MenuMusicVolume") / 100f;
		}
	}

	private void PlayMusicThroughSource(AudioClip clip)
	{
		musicAS.Stop();
		musicAS.clip = clip;
		musicAS.loop = true;
		musicAS.Play();
	}

	public void UpdateMusic(int id)
	{
		PlayerPrefs.SetInt("[TITLE]TitleMusicID", id);
		musicAS.Stop();
		switch (id)
		{
		case -1:
			break;
		case -2:
			if (!PlayerPrefs.HasKey("[MOD]MenuTheme"))
			{
				titleMusic.AudioClip = MusicLookUp.WTTG2Music;
				PlayMusicThroughSource(titleMusic.AudioClip);
				break;
			}
			if (PlayerPrefs.GetInt("[MOD]MenuTheme") == 0)
			{
				titleMusic.AudioClip = MusicLookUp.WTTG1Music;
			}
			else
			{
				titleMusic.AudioClip = MusicLookUp.WTTG2Music;
			}
			PlayMusicThroughSource(titleMusic.AudioClip);
			break;
		case -3:
		{
			int id2 = Random.Range(0, 6);
			AudioClip musicWithID = GetMusicWithID(id2);
			titleMusic.AudioClip = musicWithID;
			PlayMusicThroughSource(titleMusic.AudioClip);
			break;
		}
		default:
			titleMusic.AudioClip = GetMusicWithID(id);
			PlayMusicThroughSource(titleMusic.AudioClip);
			musicAS.volume = PlayerPrefs.GetFloat("[TITLE]MenuMusicVolume") / 100f;
			break;
		}
	}

	public AudioClip GetMusicWithID(int id)
	{
		switch (id)
		{
		case 0:
			return MusicLookUp.WTTG1Music;
		case 1:
			if (!easter_randchance)
			{
				return MusicLookUp.TWRMusic;
			}
			Debug.Log("Secret");
			return MusicLookUp.TWRSecretMusic;
		case 2:
			return MusicLookUp.WTTG2Music;
		case 3:
			return MusicLookUp.ScrutinizedMusic;
		case 4:
			return MusicLookUp.DeadSignalMusic;
		case 5:
			return MusicLookUp.TheLastSightMusic;
		default:
			return MusicLookUp.WTTG2Music;
		}
	}
}
