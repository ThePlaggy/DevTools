using TMPro;
using UnityEngine;

public class StatisticsTabLookup : MonoBehaviour
{
	private static readonly string _nodeTopScoreID = "[HACKERMODE]Score1";

	private static readonly string _stackTopScoreID = "[HACKERMODE]Score2";

	private static readonly string _vapeTopScoreID = "[HACKERMODE]Score3";

	private static readonly string _dosTopScoreID = "[HACKERMODE]Score4";

	public TMP_Text totalTimePlayed;

	public TMP_Text totalHackBeaten;

	public TMP_Text generalHackSuccessRate;

	public TMP_Text totalRunsAttempted;

	public TMP_Text totalRunsBeaten;

	public TMP_Text generalRunSuccessRate;

	public TMP_Text normalRunsAttempted;

	public TMP_Text normalRunsBeaten;

	public TMP_Text normalRunSuccessRate;

	public TMP_Text leetRunsAttempted;

	public TMP_Text leetRunsBeaten;

	public TMP_Text leetRunSuccessRate;

	public TMP_Text nightmareRunsAttempted;

	public TMP_Text hasBeatenNightmare;

	public TMP_Text nightmareRunsBeaten;

	public TMP_Text nightmareRunSuccessRate;

	public TMP_Text nodeHexerHighScore;

	public TMP_Text stackPusherHighScore;

	public TMP_Text cloudGridHighScore;

	public TMP_Text dosBlockerHighScore;

	private void OnEnable()
	{
		int num = PlayerPrefs.GetInt(_nodeTopScoreID);
		int num2 = PlayerPrefs.GetInt(_stackTopScoreID);
		int num3 = PlayerPrefs.GetInt(_vapeTopScoreID);
		int num4 = PlayerPrefs.GetInt(_dosTopScoreID);
		num = (((num - 6) % 137 == 0) ? ((num - 6) / 137) : 0);
		num2 = (((num2 - 9) % 137 == 0) ? ((num2 - 9) / 137) : 0);
		num3 = (((num3 - 4) % 137 == 0) ? ((num3 - 4) / 137) : 0);
		num4 = (((num4 - 2) % 137 == 0) ? ((num4 - 2) / 137) : 0);
		nodeHexerHighScore.text = num.ToString();
		stackPusherHighScore.text = num2.ToString();
		cloudGridHighScore.text = num3.ToString();
		dosBlockerHighScore.text = num4.ToString();
		totalTimePlayed.text = StatisticsManager.Ins.GetPlaytime();
		generalHackSuccessRate.text = GetTotalHackPercentage();
		UpdateTotalRunInfo();
		if (PlayerPrefs.HasKey("[Stats]HacksBeaten"))
		{
			totalHackBeaten.text = PlayerPrefs.GetInt("[Stats]HacksBeaten").ToString();
		}
		else
		{
			totalHackBeaten.text = "0";
		}
	}

	private string GetTotalHackPercentage()
	{
		if (!PlayerPrefs.HasKey("[Stats]HacksLaunched"))
		{
			return "Unknown";
		}
		int num = PlayerPrefs.GetInt("[Stats]HacksLaunched");
		int num2 = (PlayerPrefs.HasKey("[Stats]HacksBeaten") ? PlayerPrefs.GetInt("[Stats]HacksBeaten") : 0);
		return (float)(int)((float)num2 / (float)num * 100f * 10f) / 10f + "%";
	}

	public void UpdateTotalRunInfo()
	{
		Debug.Log("Updating Run Info");
		int num = (PlayerPrefs.HasKey("[Stats]NormalAttempted") ? PlayerPrefs.GetInt("[Stats]NormalAttempted") : 0);
		int num2 = (PlayerPrefs.HasKey("[Stats]LeetAttempted") ? PlayerPrefs.GetInt("[Stats]LeetAttempted") : 0);
		int num3 = (PlayerPrefs.HasKey("[Stats]NightmareAttempted") ? PlayerPrefs.GetInt("[Stats]NightmareAttempted") : 0);
		int num4 = (PlayerPrefs.HasKey("[Stats]NormalBeaten") ? PlayerPrefs.GetInt("[Stats]NormalBeaten") : 0);
		int num5 = (PlayerPrefs.HasKey("[Stats]LeetBeaten") ? PlayerPrefs.GetInt("[Stats]LeetBeaten") : 0);
		int num6 = (PlayerPrefs.HasKey("[Stats]NightmareBeaten") ? PlayerPrefs.GetInt("[Stats]NightmareBeaten") : 0);
		normalRunsAttempted.text = num.ToString();
		normalRunsBeaten.text = num4.ToString();
		leetRunsAttempted.text = num2.ToString();
		leetRunsBeaten.text = num5.ToString();
		nightmareRunsAttempted.text = num3.ToString();
		nightmareRunsBeaten.text = num6.ToString();
		totalRunsAttempted.text = (num + num2 + num3).ToString();
		totalRunsBeaten.text = (num4 + num5 + num6).ToString();
		normalRunSuccessRate.text = (float)(int)((float)num4 / (float)num * 100f * 10f) / 10f + "%";
		leetRunSuccessRate.text = (float)(int)((float)num5 / (float)num2 * 100f * 10f) / 10f + "%";
		nightmareRunSuccessRate.text = (float)(int)((float)num6 / (float)num3 * 100f * 10f) / 10f + "%";
		generalRunSuccessRate.text = (float)(int)((float)(num4 + num5 + num6) / (float)(num + num2 + num3) * 100f * 10f) / 10f + "%";
		if (num <= 0)
		{
			normalRunSuccessRate.text = "Unknown";
		}
		if (num2 <= 0)
		{
			leetRunSuccessRate.text = "Unknown";
		}
		if (num3 <= 0)
		{
			nightmareRunSuccessRate.text = "Unknown";
		}
		if (num6 <= 0)
		{
			hasBeatenNightmare.text = "No";
		}
		if (num + num2 + num3 <= 0)
		{
			generalRunSuccessRate.text = "Unknown";
		}
	}
}
