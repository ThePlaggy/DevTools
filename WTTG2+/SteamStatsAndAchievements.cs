using Steamworks;
using UnityEngine;

internal class SteamStatsAndAchievements : MonoBehaviour
{
	private enum Achievement
	{
		ACH_WIN_ONE_GAME,
		ACH_WIN_100_GAMES,
		ACH_HEAVY_FIRE,
		ACH_TRAVEL_FAR_ACCUM,
		ACH_TRAVEL_FAR_SINGLE
	}

	private class Achievement_t
	{
		public bool m_bAchieved;

		public Achievement m_eAchievementID;

		public string m_strDescription;

		public string m_strName;

		public Achievement_t(Achievement achievementID, string name, string desc)
		{
			m_eAchievementID = achievementID;
			m_strName = name;
			m_strDescription = desc;
			m_bAchieved = false;
		}
	}

	private Achievement_t[] m_Achievements = new Achievement_t[4]
	{
		new Achievement_t(Achievement.ACH_WIN_ONE_GAME, "Winner", string.Empty),
		new Achievement_t(Achievement.ACH_WIN_100_GAMES, "Champion", string.Empty),
		new Achievement_t(Achievement.ACH_TRAVEL_FAR_ACCUM, "Interstellar", string.Empty),
		new Achievement_t(Achievement.ACH_TRAVEL_FAR_SINGLE, "Orbiter", string.Empty)
	};

	private bool m_bRequestedStats;

	private bool m_bStatsValid;

	private bool m_bStoreStats;

	private float m_flAverageSpeed;

	private double m_flGameDurationSeconds;

	private float m_flGameFeetTraveled;

	private float m_flMaxFeetTraveled;

	private float m_flTotalFeetTraveled;

	private CGameID m_GameID;

	private int m_nTotalGamesPlayed;

	private int m_nTotalNumLosses;

	private int m_nTotalNumWins;

	private float m_ulTickCountGameStart;

	protected Callback<UserAchievementStored_t> m_UserAchievementStored;

	protected Callback<UserStatsReceived_t> m_UserStatsReceived;

	protected Callback<UserStatsStored_t> m_UserStatsStored;

	private void Update()
	{
		if (!SteamManager.Initialized)
		{
			return;
		}
		if (!m_bRequestedStats)
		{
			if (!SteamManager.Initialized)
			{
				m_bRequestedStats = true;
				return;
			}
			bool bRequestedStats = SteamUserStats.RequestCurrentStats();
			m_bRequestedStats = bRequestedStats;
		}
		if (!m_bStatsValid)
		{
			return;
		}
		Achievement_t[] achievements = m_Achievements;
		foreach (Achievement_t achievement_t in achievements)
		{
			if (achievement_t.m_bAchieved)
			{
				continue;
			}
			switch (achievement_t.m_eAchievementID)
			{
			case Achievement.ACH_WIN_ONE_GAME:
				if (m_nTotalNumWins != 0)
				{
					UnlockAchievement(achievement_t);
				}
				break;
			case Achievement.ACH_WIN_100_GAMES:
				if (m_nTotalNumWins >= 100)
				{
					UnlockAchievement(achievement_t);
				}
				break;
			case Achievement.ACH_TRAVEL_FAR_ACCUM:
				if (m_flTotalFeetTraveled >= 5280f)
				{
					UnlockAchievement(achievement_t);
				}
				break;
			case Achievement.ACH_TRAVEL_FAR_SINGLE:
				if (m_flGameFeetTraveled >= 500f)
				{
					UnlockAchievement(achievement_t);
				}
				break;
			}
		}
		if (m_bStoreStats)
		{
			SteamUserStats.SetStat("NumGames", m_nTotalGamesPlayed);
			SteamUserStats.SetStat("NumWins", m_nTotalNumWins);
			SteamUserStats.SetStat("NumLosses", m_nTotalNumLosses);
			SteamUserStats.SetStat("FeetTraveled", m_flTotalFeetTraveled);
			SteamUserStats.SetStat("MaxFeetTraveled", m_flMaxFeetTraveled);
			SteamUserStats.UpdateAvgRateStat("AverageSpeed", m_flGameFeetTraveled, m_flGameDurationSeconds);
			SteamUserStats.GetStat("AverageSpeed", out m_flAverageSpeed);
			bool flag = SteamUserStats.StoreStats();
			m_bStoreStats = !flag;
		}
	}

	private void OnEnable()
	{
		if (SteamManager.Initialized)
		{
			m_GameID = new CGameID(SteamUtils.GetAppID());
			m_UserStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
			m_UserStatsStored = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
			m_UserAchievementStored = Callback<UserAchievementStored_t>.Create(OnAchievementStored);
			m_bRequestedStats = false;
			m_bStatsValid = false;
		}
	}

	public void AddDistanceTraveled(float flDistance)
	{
		m_flGameFeetTraveled += flDistance;
	}

	private void UnlockAchievement(Achievement_t achievement)
	{
		achievement.m_bAchieved = true;
		SteamUserStats.SetAchievement(achievement.m_eAchievementID.ToString());
		m_bStoreStats = true;
	}

	private void OnUserStatsReceived(UserStatsReceived_t pCallback)
	{
		if (!SteamManager.Initialized || (ulong)m_GameID != pCallback.m_nGameID)
		{
			return;
		}
		if (pCallback.m_eResult == EResult.k_EResultOK)
		{
			Debug.Log("Received stats and achievements from Steam\n");
			m_bStatsValid = true;
			Achievement_t[] achievements = m_Achievements;
			foreach (Achievement_t achievement_t in achievements)
			{
				if (SteamUserStats.GetAchievement(achievement_t.m_eAchievementID.ToString(), out achievement_t.m_bAchieved))
				{
					achievement_t.m_strName = SteamUserStats.GetAchievementDisplayAttribute(achievement_t.m_eAchievementID.ToString(), "name");
					achievement_t.m_strDescription = SteamUserStats.GetAchievementDisplayAttribute(achievement_t.m_eAchievementID.ToString(), "desc");
				}
				else
				{
					Debug.LogWarning("SteamUserStats.GetAchievement failed for Achievement " + achievement_t.m_eAchievementID.ToString() + "\nIs it registered in the Steam Partner site?");
				}
			}
			SteamUserStats.GetStat("NumGames", out m_nTotalGamesPlayed);
			SteamUserStats.GetStat("NumWins", out m_nTotalNumWins);
			SteamUserStats.GetStat("NumLosses", out m_nTotalNumLosses);
			SteamUserStats.GetStat("FeetTraveled", out m_flTotalFeetTraveled);
			SteamUserStats.GetStat("MaxFeetTraveled", out m_flMaxFeetTraveled);
			SteamUserStats.GetStat("AverageSpeed", out m_flAverageSpeed);
		}
		else
		{
			Debug.Log("RequestStats - failed, " + pCallback.m_eResult);
		}
	}

	private void OnUserStatsStored(UserStatsStored_t pCallback)
	{
		if ((ulong)m_GameID == pCallback.m_nGameID)
		{
			if (pCallback.m_eResult == EResult.k_EResultOK)
			{
				Debug.Log("StoreStats - success");
			}
			else if (pCallback.m_eResult == EResult.k_EResultInvalidParam)
			{
				Debug.Log("StoreStats - some failed to validate");
				OnUserStatsReceived(new UserStatsReceived_t
				{
					m_eResult = EResult.k_EResultOK,
					m_nGameID = (ulong)m_GameID
				});
			}
			else
			{
				Debug.Log("StoreStats - failed, " + pCallback.m_eResult);
			}
		}
	}

	private void OnAchievementStored(UserAchievementStored_t pCallback)
	{
		if ((ulong)m_GameID == pCallback.m_nGameID)
		{
			if (pCallback.m_nMaxProgress == 0)
			{
				Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' unlocked!");
				return;
			}
			Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' progress callback, (" + pCallback.m_nCurProgress + "," + pCallback.m_nMaxProgress + ")");
		}
	}

	public void Render()
	{
		if (!SteamManager.Initialized)
		{
			GUILayout.Label("Steamworks not Initialized");
			return;
		}
		GUILayout.Label("m_ulTickCountGameStart: " + m_ulTickCountGameStart);
		GUILayout.Label("m_flGameDurationSeconds: " + m_flGameDurationSeconds);
		GUILayout.Label("m_flGameFeetTraveled: " + m_flGameFeetTraveled);
		GUILayout.Space(10f);
		GUILayout.Label("NumGames: " + m_nTotalGamesPlayed);
		GUILayout.Label("NumWins: " + m_nTotalNumWins);
		GUILayout.Label("NumLosses: " + m_nTotalNumLosses);
		GUILayout.Label("FeetTraveled: " + m_flTotalFeetTraveled);
		GUILayout.Label("MaxFeetTraveled: " + m_flMaxFeetTraveled);
		GUILayout.Label("AverageSpeed: " + m_flAverageSpeed);
		GUILayout.BeginArea(new Rect(Screen.width - 300, 0f, 300f, 800f));
		Achievement_t[] achievements = m_Achievements;
		foreach (Achievement_t achievement_t in achievements)
		{
			GUILayout.Label(achievement_t.m_eAchievementID.ToString());
			GUILayout.Label(achievement_t.m_strName + " - " + achievement_t.m_strDescription);
			GUILayout.Label("Achieved: " + achievement_t.m_bAchieved);
			GUILayout.Space(20f);
		}
		if (GUILayout.Button("RESET STATS AND ACHIEVEMENTS"))
		{
			SteamUserStats.ResetAllStats(bAchievementsToo: true);
			SteamUserStats.RequestCurrentStats();
		}
		GUILayout.EndArea();
	}
}
