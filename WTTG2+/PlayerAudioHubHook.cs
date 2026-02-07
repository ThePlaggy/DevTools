using UnityEngine;

public class PlayerAudioHubHook : MonoBehaviour
{
	[SerializeField]
	private AudioFileDefinition[] gameLiveSFXS = new AudioFileDefinition[0];

	private void Awake()
	{
		GameManager.StageManager.TheGameIsLive += gameIsLive;
	}

	private void gameIsLive()
	{
		for (int i = 0; i < gameLiveSFXS.Length; i++)
		{
			GameManager.AudioSlinger.PlaySound(gameLiveSFXS[i]);
		}
		GameManager.StageManager.TheGameIsLive -= gameIsLive;
	}
}
