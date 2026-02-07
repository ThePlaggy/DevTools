public static class VoIPManager
{
	public static bool VoIPShown;

	public static void ShowVoIP()
	{
		if (!VoIPShown)
		{
			VoIPShown = true;
			if (!TarotVengeance.Killed(ENEMY_STATE.EXECUTIONER) && !DifficultyManager.LeetMode && !DifficultyManager.Nightmare && !DifficultyManager.CasualMode)
			{
				AppCreator.VoIPGameObject.SetActive(value: true);
				EXEManager.Ins.ReleaseExecutor();
				TarotVengeance.ActivateEnemy(ENEMY_STATE.EXECUTIONER);
			}
		}
	}
}
