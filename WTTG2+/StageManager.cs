using UnityEngine;

public class StageManager : MonoBehaviour
{
	public delegate void LiveActions();

	public delegate void StageManagerActions();

	public bool DebugMode;

	public bool LeetMode;

	public bool GameIsLive;

	[SerializeField]
	private GAME_CONTROLLER defaultController;

	[SerializeField]
	private float stageTime = 0.5f;

	private StageManagerData myData;

	private int myID;

	private bool threatsActivated;

	private bool threatTimerActive;

	private float threatTimeStamp;

	private float threatWindow;

	public GAME_CONTROLLER DefaultController => defaultController;

	public event StageManagerActions Stage;

	public event LiveActions TheGameIsLive;

	public event LiveActions TheGameIsStageing;

	public event LiveActions ThreatsNowActivated;

	private void Awake()
	{
		myID = base.transform.position.GetHashCode();
		GameManager.StageManager = this;
		PauseManager.LockPause();
		StateManager.PlayerState = PLAYER_STATE.STAGING;
		if (DebugMode && LeetMode)
		{
			DifficultyManager.LeetMode = true;
		}
	}

	private void Start()
	{
		prepStageManager();
	}

	private void Update()
	{
		if (threatTimerActive && Time.time - threatTimeStamp >= threatWindow)
		{
			CancelInvoke("updateTimeLeft");
			threatTimerActive = false;
			threatsActivated = true;
			myData.ThreatsActivated = true;
			myData.TimeLeft = 0f;
			DataManager.Save(myData);
			if (this.ThreatsNowActivated != null)
			{
				this.ThreatsNowActivated();
			}
		}
	}

	public void ManuallyActivateThreats()
	{
		if (!threatsActivated)
		{
			CancelInvoke("updateTimeLeft");
			threatTimerActive = false;
			threatsActivated = true;
			myData.ThreatsActivated = true;
			myData.TimeLeft = 0f;
			DataManager.Save(myData);
			if (this.ThreatsNowActivated != null)
			{
				this.ThreatsNowActivated();
			}
		}
	}

	private void prepStageManager()
	{
		if (this.TheGameIsStageing != null)
		{
			this.TheGameIsStageing();
		}
		if (!DebugMode)
		{
			GameManager.WorldManager.WorldLoaded += stageLevel;
		}
		else
		{
			stageLevel();
		}
	}

	private void stageLevel()
	{
		if (!DebugMode)
		{
			GameManager.WorldManager.WorldLoaded -= stageLevel;
		}
		myData = DataManager.Load<StageManagerData>(myID);
		if (myData == null)
		{
			myData = new StageManagerData(myID);
			myData.ThreatsActivated = false;
			myData.TimeLeft = 450f;
		}
		if (this.Stage != null)
		{
			this.Stage();
		}
		if (!DebugMode)
		{
			GameManager.TimeSlinger.FireTimer(stageTime, setGameLive);
		}
		else
		{
			GameManager.TimeSlinger.FireTimer(0.5f, setGameLive);
		}
	}

	private void setGameLive()
	{
		GameIsLive = true;
		if (this.TheGameIsLive != null)
		{
			this.TheGameIsLive();
		}
		PauseManager.UnLockPause();
		if (DifficultyManager.LeetMode)
		{
			if (this.ThreatsNowActivated != null)
			{
				this.ThreatsNowActivated();
			}
			for (int i = 0; i < 8; i++)
			{
				GameManager.TheCloud.ForceKeyDiscover();
			}
		}
		else if (myData.ThreatsActivated)
		{
			if (this.ThreatsNowActivated != null)
			{
				this.ThreatsNowActivated();
			}
		}
		else
		{
			threatWindow = myData.TimeLeft;
			threatTimeStamp = Time.time;
			threatTimerActive = true;
			InvokeRepeating("updateTimeLeft", 0f, 10f);
		}
		GameManager.TheCloud.KeyDiscoveredEvent.Event += keyWasFound;
	}

	private void updateTimeLeft()
	{
		myData.TimeLeft = threatWindow - (Time.time - threatTimeStamp);
		DataManager.Save(myData);
	}

	private void keyWasFound()
	{
		GameManager.TheCloud.KeyDiscoveredEvent.Event -= keyWasFound;
		if (!threatsActivated)
		{
			CancelInvoke("updateTimeLeft");
			threatTimerActive = false;
			threatsActivated = true;
			myData.ThreatsActivated = true;
			myData.TimeLeft = 0f;
			DataManager.Save(myData);
			if (this.ThreatsNowActivated != null)
			{
				this.ThreatsNowActivated();
			}
		}
	}
}
