using UnityEngine;

public class StatisticsManager : MonoBehaviour
{
	public static StatisticsManager Ins;

	public float startTime;

	private float playtimeInSeconds;

	private float playtimeSinceLaunch;

	private void Awake()
	{
		if (Ins != null)
		{
			Debug.Log("More than one statistics manager is active! DESTROYING!!");
			Object.Destroy(base.gameObject);
		}
		else
		{
			Ins = this;
		}
	}

	private void Start()
	{
		startTime = Time.time;
		if (PlayerPrefs.HasKey("Playtime"))
		{
			playtimeInSeconds = PlayerPrefs.GetFloat("Playtime");
		}
	}

	private void Update()
	{
		playtimeSinceLaunch = Time.time - startTime;
	}

	private void OnApplicationQuit()
	{
		PlayerPrefs.SetFloat("Playtime", playtimeSinceLaunch + playtimeInSeconds);
	}

	public string GetPlaytime()
	{
		return (float)(int)((playtimeInSeconds + playtimeSinceLaunch) / 60f / 60f * 10f) / 10f + " hours";
	}

	public void HackLaunched()
	{
		if (!PlayerPrefs.HasKey("[Stats]HacksLaunched"))
		{
			PlayerPrefs.SetInt("[Stats]HacksLaunched", 1);
		}
		else
		{
			PlayerPrefs.SetInt("[Stats]HacksLaunched", 1 + PlayerPrefs.GetInt("[Stats]HacksLaunched"));
		}
	}

	public void HackBeaten()
	{
		if (!PlayerPrefs.HasKey("[Stats]HacksBeaten"))
		{
			PlayerPrefs.SetInt("[Stats]HacksBeaten", 1);
		}
		else
		{
			PlayerPrefs.SetInt("[Stats]HacksBeaten", 1 + PlayerPrefs.GetInt("[Stats]HacksBeaten"));
		}
	}

	public void StartRun(Difficulty difficulty)
	{
		string key = "[Stats]NormalAttempted";
		switch (difficulty)
		{
		case Difficulty.NORMAL:
			key = "[Stats]NormalAttempted";
			break;
		case Difficulty.LEET:
			key = "[Stats]LeetAttempted";
			break;
		case Difficulty.NIGHTMARE:
			key = "[Stats]NightmareAttempted";
			break;
		}
		if (!PlayerPrefs.HasKey(key))
		{
			PlayerPrefs.SetInt(key, 1);
		}
		else
		{
			PlayerPrefs.SetInt(key, 1 + PlayerPrefs.GetInt(key));
		}
	}

	public void BeatRun(Difficulty difficulty)
	{
		string key = "[Stats]NormalBeaten";
		switch (difficulty)
		{
		case Difficulty.NORMAL:
			key = "[Stats]NormalBeaten";
			break;
		case Difficulty.LEET:
			key = "[Stats]LeetBeaten";
			break;
		case Difficulty.NIGHTMARE:
			key = "[Stats]NightmareBeaten";
			break;
		}
		if (!PlayerPrefs.HasKey(key))
		{
			PlayerPrefs.SetInt(key, 1);
		}
		else
		{
			PlayerPrefs.SetInt(key, 1 + PlayerPrefs.GetInt(key));
		}
	}
}
