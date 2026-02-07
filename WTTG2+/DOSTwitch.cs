using System;
using System.Collections.Generic;
using UnityEngine;

public class DOSTwitch : MonoBehaviour
{
	public static bool dosTwitchEnabled;

	public static DOSTwitch instance;

	public static bool Initialized;

	public static bool InitializedIRC;

	public float hackerPollMinWindow;

	public float hackerPollMaxWindow;

	private Action<string, string> currentPollAction;

	[NonSerialized]
	public PollBase currentPollRef;

	private float discountPollMaxWindow;

	private float discountPollMinWindow;

	private float discountPollTimeStamp;

	private float discountPollTimeWindow;

	private bool discountPollWindowActive;

	private float DOSCoinPollMaxWindow;

	private float DOSCoinPollMinWindow;

	private float DOSCoinPollTimeStamp;

	private float DOSCoinPollTimeWindow;

	private bool DOSCoinPollWindowActive;

	private float EarlyGamePollMaxWindow;

	private float EarlyGamePollMinWindow;

	private float EarlyGamePollTimeStamp;

	private float EarlyGamePollTimeWindow;

	private bool EarlyGamePollWindowActive;

	private float hackerPollTimeStamp;

	private float hackerPollTimeWindow;

	private bool hackerPollWindowActive;

	private float keyPollMaxWindow;

	private float keyPollMinWindow;

	private float keyPollTimeStamp;

	private float keyPollTimeWindow;

	private bool keyPollWindowActive;

	private DiscountPoll myDiscountPoll;

	public DOSCoinPoll myDOSCoinPoll;

	private EarlyGamePoll myEarlyGamePoll;

	private HackerPoll myHackerPoll;

	private KeyPoll myKeyPoll;

	private SpeedPoll mySpeedPoll;

	private TarotPoll myTarotPoll;

	private VirusPoll myVirusPoll;

	private WiFiPoll myWiFiPoll;

	[NonSerialized]
	public bool pollActive;

	private float speedPollMaxWindow;

	private float speedPollMinWindow;

	private float speedPollTimeStamp;

	private float speedPollTimeWindow;

	private bool speedPollWindowActive;

	private float tarotPollMaxWindow;

	private float tarotPollMinWindow;

	private float tarotPollTimeStamp;

	private float tarotPollTimeWindow;

	private bool tarotPollWindowActive;

	private float VirusPollMaxWindow;

	private float VirusPollMinWindow;

	private float VirusPollTimeStamp;

	private float VirusPollTimeWindow;

	private bool VirusPollWindowActive;

	private float wifiPollMaxWindow;

	private float wifiPollMinWindow;

	private float wifiPollTimeStamp;

	private float wifiPollTimeWindow;

	private bool wifiPollWindowActive;

	public static int rockTheVote = -1;

	private List<string> UniqueUsernames;

	private bool BadgeGrant;

	public string EarlyGameDebug
	{
		get
		{
			if (EarlyGamePollTimeWindow - (Time.time - EarlyGamePollTimeStamp) > 0f)
			{
				return ((int)(EarlyGamePollTimeWindow - (Time.time - EarlyGamePollTimeStamp))).ToString();
			}
			return 0.ToString();
		}
	}

	public string DiscountDebug
	{
		get
		{
			if (discountPollTimeWindow - (Time.time - discountPollTimeStamp) > 0f)
			{
				return ((int)(discountPollTimeWindow - (Time.time - discountPollTimeStamp))).ToString();
			}
			return 0.ToString();
		}
	}

	public string VirusDebug
	{
		get
		{
			if (VirusPollTimeWindow - (Time.time - VirusPollTimeStamp) > 0f)
			{
				return ((int)(VirusPollTimeWindow - (Time.time - VirusPollTimeStamp))).ToString();
			}
			return 0.ToString();
		}
	}

	public string DOSCoinDebug
	{
		get
		{
			if (DOSCoinPollTimeWindow - (Time.time - DOSCoinPollTimeStamp) > 0f)
			{
				return ((int)(DOSCoinPollTimeWindow - (Time.time - DOSCoinPollTimeStamp))).ToString();
			}
			return 0.ToString();
		}
	}

	public string HACKERMANSDebug
	{
		get
		{
			if (hackerPollTimeWindow - (Time.time - hackerPollTimeStamp) > 0f)
			{
				return ((int)(hackerPollTimeWindow - (Time.time - hackerPollTimeStamp))).ToString();
			}
			return 0.ToString();
		}
	}

	public string WiFiDebug
	{
		get
		{
			if (wifiPollTimeWindow - (Time.time - wifiPollTimeStamp) > 0f)
			{
				return ((int)(wifiPollTimeWindow - (Time.time - wifiPollTimeStamp))).ToString();
			}
			return 0.ToString();
		}
	}

	public string SpeedDebug
	{
		get
		{
			if (speedPollTimeWindow - (Time.time - speedPollTimeStamp) > 0f)
			{
				return ((int)(speedPollTimeWindow - (Time.time - speedPollTimeStamp))).ToString();
			}
			return 0.ToString();
		}
	}

	public string KeyDebug
	{
		get
		{
			if (keyPollTimeWindow - (Time.time - keyPollTimeStamp) > 0f)
			{
				return ((int)(keyPollTimeWindow - (Time.time - keyPollTimeStamp))).ToString();
			}
			return 0.ToString();
		}
	}

	public string TarotDebug
	{
		get
		{
			if (tarotPollTimeWindow - (Time.time - tarotPollTimeStamp) > 0f)
			{
				return ((int)(tarotPollTimeWindow - (Time.time - tarotPollTimeStamp))).ToString();
			}
			return 0.ToString();
		}
	}

	private void Awake()
	{
		instance = this;
		UniqueUsernames = new List<string>();
	}

	private void Start()
	{
		Debug.Log("[DOSTwitch] Initiating new DOSTwitch with serial: " + GetHashCode());
		bool flag = false;
		DOSCoinPollMinWindow = 120f;
		DOSCoinPollMaxWindow = 300f;
		EarlyGamePollMinWindow = 120f;
		EarlyGamePollMaxWindow = 240f;
		VirusPollMinWindow = 120f;
		VirusPollMaxWindow = 300f;
		hackerPollMinWindow = 120f;
		hackerPollMaxWindow = 360f;
		discountPollMinWindow = 10f;
		discountPollMaxWindow = 60f;
		wifiPollMinWindow = 120f;
		wifiPollMaxWindow = 360f;
		speedPollMinWindow = 180f;
		speedPollMaxWindow = 240f;
		keyPollMinWindow = 120f;
		keyPollMaxWindow = 240f;
		tarotPollMinWindow = 180f;
		tarotPollMaxWindow = 300f;
	}

	private void Update()
	{
		if (!dosTwitchEnabled)
		{
			return;
		}
		if (EarlyGamePollWindowActive && Time.time - EarlyGamePollTimeStamp >= EarlyGamePollTimeWindow)
		{
			EarlyGamePollWindowActive = false;
			if (DifficultyManager.Nightmare)
			{
				triggerDiscountPoll();
			}
			else
			{
				triggerEarlyGamePoll();
			}
			generateVirusPollWindow();
		}
		if (discountPollWindowActive && Time.time - discountPollTimeStamp >= discountPollTimeWindow)
		{
			discountPollWindowActive = false;
			triggerDiscountPoll();
			generateVirusPollWindow();
		}
		if (wifiPollWindowActive && Time.time - wifiPollTimeStamp >= wifiPollTimeWindow)
		{
			wifiPollWindowActive = false;
			triggerWiFiPoll();
			generateSpeedPollWindow();
		}
		if (speedPollWindowActive && Time.time - speedPollTimeStamp >= speedPollTimeWindow)
		{
			speedPollWindowActive = false;
			triggerSpeedPoll();
			generateKeyPollWindow();
		}
		if (keyPollWindowActive && Time.time - keyPollTimeStamp >= keyPollTimeWindow)
		{
			keyPollWindowActive = false;
			triggerKeyPoll();
			generateTarotPollWindow();
		}
		if (tarotPollWindowActive && Time.time - tarotPollTimeStamp >= tarotPollTimeWindow)
		{
			tarotPollWindowActive = false;
			triggerTarotPoll();
			generateVirusPollWindow();
		}
		if (VirusPollWindowActive && Time.time - VirusPollTimeStamp >= VirusPollTimeWindow)
		{
			VirusPollWindowActive = false;
			triggerVirusPoll();
			generateDOSCoinPollWindow();
		}
		if (DOSCoinPollWindowActive && Time.time - DOSCoinPollTimeStamp >= DOSCoinPollTimeWindow)
		{
			DOSCoinPollWindowActive = false;
			triggerDOSCoinPoll();
			generateHackerPollWindow();
		}
		if (hackerPollWindowActive && Time.time - hackerPollTimeStamp >= hackerPollTimeWindow)
		{
			hackerPollWindowActive = false;
			triggerHackerPoll();
			generateWiFiPollWindow();
		}
	}

	private void OnDisable()
	{
		if (dosTwitchEnabled)
		{
			Debug.Log("[DOSTwitch] Twitch integration unloaded successfully.");
			if (TwitchManager.Ins == null)
			{
				Debug.Log("[DOSTwitch] Twitch integration unloaded successfully. [1]");
				return;
			}
			if (TwitchManager.Ins.Hook == null)
			{
				Debug.Log("[DOSTwitch] Twitch integration unloaded successfully. [2]");
				return;
			}
			TwitchManager.Ins.Hook.SendMessage("Twitch integration unloaded successfully. Have a nice day! :)");
			TwitchManager.Ins.Hook.ClientConnected.Event -= warmDOSTwitch;
			TwitchManager.Ins.Hook.MessageReceived.Event -= ChatMessageReceived;
		}
	}

	private void OnDestroy()
	{
		instance = null;
	}

	public void setPollInactive()
	{
		rockTheVote = -1;
		pollActive = false;
	}

	private void warmDOSTwitch()
	{
		rockTheVote = -1;
		mySpeedPoll = new SpeedPoll(this);
		myWiFiPoll = new WiFiPoll(this);
		myDOSCoinPoll = new DOSCoinPoll(this);
		myEarlyGamePoll = new EarlyGamePoll(this);
		myVirusPoll = new VirusPoll(this);
		myHackerPoll = new HackerPoll(this);
		myDiscountPoll = new DiscountPoll(this);
		myKeyPoll = new KeyPoll(this);
		myTarotPoll = new TarotPoll(this);
		displayTwitchConnected();
		if (DifficultyManager.LeetMode)
		{
			generateDiscountPollWindow();
		}
		else
		{
			generateEarlyGamePollWindow();
		}
		GiveTarotCardsDelayed((DifficultyManager.LeetMode || DifficultyManager.Nightmare) ? 100f : 200f);
	}

	private void displayTwitchConnected()
	{
		Debug.Log("Twitch Integration Now Live! FeelsGoodMan");
		TwitchManager.Ins.Hook.SendMessage("Welcome to the Game 2 - WTTG2+ Mod [v1.614] - Twitch Integration Live - FeelsGoodMan Clap");
		dosTwitchEnabled = true;
		Debug.Log("DOSTwitch was enabled and put in an instance (Serial: " + GetHashCode() + ")");
	}

	private void generateDOSCoinPollWindow()
	{
		if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			DOSCoinPollTimeWindow = UnityEngine.Random.RandomRange(90f, 120f);
		}
		else
		{
			DOSCoinPollTimeWindow = UnityEngine.Random.Range(DOSCoinPollMinWindow, DOSCoinPollMaxWindow);
		}
		DOSCoinPollTimeStamp = Time.time;
		DOSCoinPollWindowActive = true;
	}

	private void generateEarlyGamePollWindow()
	{
		EarlyGamePollTimeWindow = UnityEngine.Random.Range(EarlyGamePollMinWindow, EarlyGamePollMaxWindow);
		EarlyGamePollTimeStamp = Time.time;
		EarlyGamePollWindowActive = true;
	}

	private void triggerDOSCoinPoll()
	{
		if (!pollActive)
		{
			currentPollAction = myDOSCoinPoll.CastVote;
			currentPollRef = myDOSCoinPoll;
			pollActive = true;
			myDOSCoinPoll.BeginVote();
			TwitchManager.Ins.TriggerPopUp("DOS Coins Poll in progress...");
		}
		else if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			GameManager.TimeSlinger.FireTimer(UnityEngine.Random.Range(28f, 69f), triggerDOSCoinPoll);
		}
		else
		{
			GameManager.TimeSlinger.FireTimer(UnityEngine.Random.Range(42f, 128f), triggerDOSCoinPoll);
		}
	}

	private void triggerEarlyGamePoll()
	{
		if (!pollActive)
		{
			currentPollAction = myEarlyGamePoll.CastVote;
			currentPollRef = myEarlyGamePoll;
			pollActive = true;
			myEarlyGamePoll.BeginVote();
			TwitchManager.Ins.TriggerPopUp("Escalation Poll in progress...");
		}
		else
		{
			GameManager.TimeSlinger.FireTimer(UnityEngine.Random.Range(42f, 128f), triggerEarlyGamePoll);
		}
	}

	private void generateVirusPollWindow()
	{
		if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			VirusPollTimeWindow = UnityEngine.Random.RandomRange(90f, 120f);
		}
		else
		{
			VirusPollTimeWindow = UnityEngine.Random.Range(VirusPollMinWindow, VirusPollMaxWindow);
		}
		VirusPollTimeStamp = Time.time;
		VirusPollWindowActive = true;
	}

	private void triggerVirusPoll()
	{
		if (!pollActive)
		{
			currentPollAction = myVirusPoll.CastVote;
			currentPollRef = myVirusPoll;
			pollActive = true;
			myVirusPoll.BeginVote();
			TwitchManager.Ins.TriggerPopUp("Virus Poll in progress...");
		}
		else if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			GameManager.TimeSlinger.FireTimer(UnityEngine.Random.Range(25f, 69f), triggerVirusPoll);
		}
		else
		{
			GameManager.TimeSlinger.FireTimer(UnityEngine.Random.Range(42f, 128f), triggerVirusPoll);
		}
	}

	private void generateHackerPollWindow()
	{
		if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			hackerPollTimeWindow = UnityEngine.Random.RandomRange(90f, 120f);
		}
		else
		{
			hackerPollTimeWindow = UnityEngine.Random.Range(hackerPollMinWindow, hackerPollMaxWindow);
		}
		hackerPollTimeStamp = Time.time;
		hackerPollWindowActive = true;
	}

	private void triggerHackerPoll()
	{
		if (!pollActive)
		{
			currentPollAction = myHackerPoll.CastVote;
			currentPollRef = myHackerPoll;
			pollActive = true;
			myHackerPoll.BeginVote();
			TwitchManager.Ins.TriggerPopUp("HACKERMANS Poll in progress...");
		}
		else if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			GameManager.TimeSlinger.FireTimer(UnityEngine.Random.Range(25f, 69f), triggerHackerPoll);
		}
		else
		{
			GameManager.TimeSlinger.FireTimer(UnityEngine.Random.Range(45f, 128f), triggerHackerPoll);
		}
	}

	private void triggerDiscountPoll()
	{
		if (pollActive)
		{
			GameManager.TimeSlinger.FireTimer(UnityEngine.Random.Range(32f, 81f), triggerDiscountPoll);
			return;
		}
		if (DifficultyManager.Nightmare)
		{
			currentPollAction = myDiscountPoll.CastVoteNightmare;
		}
		else
		{
			currentPollAction = myDiscountPoll.CastVote;
		}
		currentPollRef = myDiscountPoll;
		pollActive = true;
		if (DifficultyManager.Nightmare)
		{
			myDiscountPoll.BeginVoteNightmare();
		}
		else
		{
			myDiscountPoll.BeginVote();
		}
		TwitchManager.Ins.TriggerPopUp("Discount Poll in progress...");
	}

	private void generateDiscountPollWindow()
	{
		if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			discountPollTimeWindow = UnityEngine.Random.RandomRange(90f, 120f);
		}
		else
		{
			discountPollTimeWindow = UnityEngine.Random.Range(discountPollMinWindow, discountPollMaxWindow);
		}
		discountPollTimeStamp = Time.time;
		discountPollWindowActive = true;
	}

	private void generateWiFiPollWindow()
	{
		if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			wifiPollTimeWindow = UnityEngine.Random.RandomRange(90f, 120f);
		}
		else
		{
			wifiPollTimeWindow = UnityEngine.Random.Range(wifiPollMinWindow, wifiPollMaxWindow);
		}
		wifiPollTimeStamp = Time.time;
		wifiPollWindowActive = true;
	}

	private void triggerWiFiPoll()
	{
		if (!pollActive)
		{
			currentPollAction = myWiFiPoll.CastVote;
			currentPollRef = myWiFiPoll;
			pollActive = true;
			myWiFiPoll.BeginVote();
			TwitchManager.Ins.TriggerPopUp("WiFi Poll in progress...");
		}
		else if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			GameManager.TimeSlinger.FireTimer(UnityEngine.Random.Range(25f, 69f), triggerWiFiPoll);
		}
		else
		{
			GameManager.TimeSlinger.FireTimer(UnityEngine.Random.Range(42f, 128f), triggerWiFiPoll);
		}
	}

	private void generateSpeedPollWindow()
	{
		if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			speedPollTimeWindow = UnityEngine.Random.RandomRange(90f, 120f);
		}
		else
		{
			speedPollTimeWindow = UnityEngine.Random.Range(speedPollMinWindow, speedPollMaxWindow);
		}
		speedPollTimeStamp = Time.time;
		speedPollWindowActive = true;
	}

	private void triggerSpeedPoll()
	{
		if (!pollActive)
		{
			currentPollAction = mySpeedPoll.CastVote;
			currentPollRef = mySpeedPoll;
			pollActive = true;
			mySpeedPoll.BeginVote();
			TwitchManager.Ins.TriggerPopUp("Loading Speed Poll in progress...");
		}
		else if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			GameManager.TimeSlinger.FireTimer(UnityEngine.Random.Range(25f, 69f), triggerSpeedPoll);
		}
		else
		{
			GameManager.TimeSlinger.FireTimer(UnityEngine.Random.Range(49f, 101f), triggerSpeedPoll);
		}
	}

	private void generateKeyPollWindow()
	{
		if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			keyPollTimeWindow = UnityEngine.Random.RandomRange(90f, 120f);
		}
		else
		{
			keyPollTimeWindow = UnityEngine.Random.Range(keyPollMinWindow, keyPollMaxWindow);
		}
		keyPollTimeStamp = Time.time;
		keyPollWindowActive = true;
	}

	private void triggerKeyPoll()
	{
		if (!pollActive)
		{
			currentPollAction = myKeyPoll.CastVote;
			currentPollRef = myKeyPoll;
			pollActive = true;
			myKeyPoll.BeginVote();
			TwitchManager.Ins.TriggerPopUp("Key Cue Poll in progress...");
		}
		else if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			GameManager.TimeSlinger.FireTimer(UnityEngine.Random.Range(25f, 69f), triggerKeyPoll);
		}
		else
		{
			GameManager.TimeSlinger.FireTimer(UnityEngine.Random.Range(49f, 101f), triggerKeyPoll);
		}
	}

	private void GiveTarotCardsDelayed(float seconds)
	{
		ItemSlinger.TarotCards.productMaxPurchaseAmount = 0;
		ItemSlinger.TarotCards.myProductObject.RefreshMe();
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[11].productRequiresOtherProduct = true;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[11].myProductObject.RefreshMe(GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[11]);
		GameManager.TimeSlinger.FireTimer(seconds, delegate
		{
			if (!TarotCardsBehaviour.Owned)
			{
				TarotCardsBehaviour.Ins.MoveMe(new Vector3(1.393f, 40.68f, 2.489f), new Vector3(0f, -20f, 180f), new Vector3(0.3f, 0.3f, 0.3f));
			}
		});
	}

	private void generateTarotPollWindow()
	{
		if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			tarotPollTimeWindow = UnityEngine.Random.RandomRange(90f, 120f);
		}
		else
		{
			tarotPollTimeWindow = UnityEngine.Random.Range(tarotPollMinWindow, tarotPollMaxWindow);
		}
		tarotPollTimeStamp = Time.time;
		tarotPollWindowActive = true;
	}

	private void triggerTarotPoll()
	{
		if (!pollActive)
		{
			currentPollAction = myTarotPoll.CastVote;
			currentPollRef = myTarotPoll;
			pollActive = true;
			myTarotPoll.BeginVote();
			TwitchManager.Ins.TriggerPopUp("Tarot Poll in progress...");
		}
		else if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			GameManager.TimeSlinger.FireTimer(UnityEngine.Random.Range(25f, 69f), triggerTarotPoll);
		}
		else
		{
			GameManager.TimeSlinger.FireTimer(UnityEngine.Random.Range(49f, 101f), triggerTarotPoll);
		}
	}

	private void ChatMessageReceived(ChatMessage Message)
	{
		if (pollActive)
		{
			currentPollAction(Message.Username, Message.Content.ToUpper());
		}
		if (!BadgeGrant)
		{
			if (!UniqueUsernames.Contains(Message.Username) && Message.Username != TwitchManager.Ins.Account.Username)
			{
				UniqueUsernames.Add(Message.Username);
			}
			if (UniqueUsernames.Count >= 5)
			{
				BadgeGrant = true;
				PlayerPrefs.SetInt("iAmFamous", 1);
			}
		}
	}

	public void Init()
	{
		if (Initialized)
		{
			Debug.Log("[WARNING] Tried to initialize DOSTwitch again, Memory overflow or bad TwitchIRC connection?");
			Debug.Log("Original DOSTwitch instance serial corresponds to " + GetHashCode());
			return;
		}
		Initialized = true;
		TwitchManager.Ins.Hook.ClientConnected.Event += warmDOSTwitch;
		TwitchManager.Ins.Hook.MessageReceived.Event += ChatMessageReceived;
		Debug.Log("DOSTwitch Init Executed Successfully!");
	}
}
