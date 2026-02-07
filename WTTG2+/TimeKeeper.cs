using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeKeeper : MonoBehaviour
{
	public static string Time;

	public static bool NightmareEndingTriggered;

	[SerializeField]
	[Range(1f, 60f)]
	private float secondsToMin = 30f;

	private Dictionary<string, Action> currentClocks = new Dictionary<string, Action>();

	private float curTimeStamp;

	private bool freezeTime;

	private int gameHour;

	private int gameMin;

	private List<string> hourArray = new List<string>();

	private TimeData myTimeData;

	private bool updateAllClocks;

	public CustomEvent<string> UpdateClockEvents = new CustomEvent<string>(3);

	public int zonewallLevel
	{
		get
		{
			if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
			{
				return 11;
			}
			int num = (gameHour - (DifficultyManager.CasualMode ? 8 : 10)) * 2;
			int num2 = ((gameMin >= 30) ? 1 : 0);
			int num3 = num + num2;
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num3 > 11)
			{
				num3 = 11;
			}
			return num3;
		}
	}

	private void Awake()
	{
		GameManager.TimeKeeper = this;
		updateAllClocks = false;
		prepHourArray();
		curTimeStamp = UnityEngine.Time.time;
		freezeTime = false;
		GameManager.StageManager.Stage += stageMe;
		GameManager.PauseManager.GamePaused += playerHitPause;
		GameManager.PauseManager.GameUnPaused += playerHitUnPause;
		freezeTime = true;
	}

	private void Start()
	{
		if (!DifficultyManager.Nightmare)
		{
			float num = UnityEngine.Random.Range(3500f, 9000f);
			GameManager.TimeSlinger.FireTimer(num, GameManager.HackerManager.theSwan.ActivateTheSwan);
			Debug.Log($"[TimeKeeper] The Swan will spawn after {num} seconds");
		}
	}

	private void Update()
	{
		if (!freezeTime)
		{
			updateClock();
		}
		if (Input.GetKeyDown(KeyCode.Keypad7) && false)
		{
			DataManager.WriteData();
		}
		secondsToMin = TarotManager.TimeController;
	}

	private void OnDestroy()
	{
		GameManager.PauseManager.GamePaused -= playerHitPause;
		GameManager.PauseManager.GameUnPaused -= playerHitUnPause;
	}

	public string GetCurrentTimeString()
	{
		string text = hourArray[gameHour] + ":" + gameMin.ToString("D2");
		if (gameHour > 11)
		{
			text += " AM";
			if (gameHour >= 12 && (gameMin == 0 || gameMin == 30) && !DifficultyManager.CasualMode && !DifficultyManager.LeetMode && !DifficultyManager.Nightmare)
			{
				GameManager.TheCloud.ForceKeyDiscover();
			}
			if (gameHour == 15 && gameMin == 33 && DeepWebRadioManager.radio == RADIO_TYPE.ANONYJAZZ)
			{
				GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.welcometothegametwoplus);
			}
		}
		else
		{
			if (gameHour == 11 && gameMin == 30 && !DifficultyManager.CasualMode && !DifficultyManager.LeetMode && !DifficultyManager.Nightmare)
			{
				GameManager.TheCloud.ForceKeyDiscover();
			}
			text += " PM";
		}
		if (PhoneBehaviour.Ins != null)
		{
			PhoneBehaviour.Ins.UpdateClock(text);
		}
		Time = text;
		return text;
	}

	public int GetCurrentGameHour()
	{
		return gameHour;
	}

	public int GetCurrentGameMin()
	{
		return gameMin;
	}

	private void prepHourArray()
	{
		for (int i = 0; i < 2; i++)
		{
			hourArray.Add(12.ToString());
			for (int j = 1; j <= 11; j++)
			{
				hourArray.Add(j.ToString());
			}
		}
	}

	private void updateClock()
	{
		if (!DifficultyManager.HackerMode && !NightmareEndingTriggered)
		{
			if (secondsToMin <= UnityEngine.Time.time - curTimeStamp)
			{
				gameMin++;
				curTimeStamp = UnityEngine.Time.time;
				updateAllClocks = true;
			}
			if (gameMin == 20 || gameMin == 40 || gameMin == 60)
			{
				DataManager.WriteData();
			}
			if (gameMin >= 60)
			{
				gameMin = 0;
				gameHour++;
			}
			if (gameHour == 13 && gameMin == 0 && DifficultyManager.Nightmare)
			{
				DelfalcoBehaviour.Ins.SetKnowsApartment();
			}
			if (gameHour >= 16)
			{
				DataManager.ClearGameData();
				UIManager.TriggerGameOver("TIMES UP!");
			}
			if (updateAllClocks)
			{
				updateAllClocks = false;
				myTimeData.GameHour = gameHour;
				myTimeData.GameMin = gameMin;
				DataManager.Save(myTimeData);
				UpdateClockEvents.Execute(GetCurrentTimeString());
			}
		}
	}

	private void playerHitPause()
	{
		freezeTime = true;
	}

	private void playerHitUnPause()
	{
		freezeTime = false;
	}

	private void stageMe()
	{
		GameManager.StageManager.Stage -= stageMe;
		myTimeData = DataManager.Load<TimeData>(52085);
		if (myTimeData == null)
		{
			myTimeData = new TimeData(52085);
			myTimeData.GameHour = (DifficultyManager.CasualMode ? 8 : 10);
			myTimeData.GameMin = 0;
		}
		gameHour = myTimeData.GameHour;
		gameMin = myTimeData.GameMin;
		UpdateClockEvents.Execute(GetCurrentTimeString());
		freezeTime = false;
	}

	public void DevSetTime(int hour, int min)
	{
		if (hour < 8)
		{
			if (hour > 5)
			{
				return;
			}
			hour += 12;
		}
		gameHour = hour;
		gameMin = min;
		UpdateClockEvents.Execute(GetCurrentTimeString());
	}
}
