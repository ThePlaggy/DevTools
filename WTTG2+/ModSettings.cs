using UnityEngine;

public abstract class ModSettings
{
	public static bool DMCA;

	public static bool DOSTwitchActive;

	public static void ApplyMods()
	{
		ApplyDOSTwitch();
		DMCA = PlayerPrefs.GetInt("[MOD]TrolloPollo", 1) == 1;
		Debug.Log("[ModsManager] Applies mod settings");
	}

	private static void ApplyDOSTwitch()
	{
		DOSTwitchActive = PlayerPrefs.GetInt("[MOD]TTVInt", 1) == 1;
		if (!TwitchManager.Ins.LoggedIn)
		{
			DOSTwitchActive = false;
			PlayerPrefs.SetInt("[MOD]TTVInt", 0);
		}
		if (DifficultyManager.CasualMode || DifficultyManager.HackerMode)
		{
			DOSTwitchActive = false;
		}
	}
}
