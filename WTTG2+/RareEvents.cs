using UnityEngine;

public static class RareEvents
{
	public static void Roll()
	{
		Rickroller.VACation = Random.Range(0, 100) <= 10;
		DLLTitleManager.ChosenBoy = Random.Range(0, 6);
		TitleManager.easter_randchance = Random.Range(0, 100) <= 5;
		Debug.Log($"[RareEvents] Rolling random events, TWR Secret {TitleManager.easter_randchance} , Vacation Lobotomy: {Rickroller.VACation}");
		if (DLLTitleManager.ChosenBoy == 3)
		{
			TitleManager.easter_randchance = true;
		}
	}
}
