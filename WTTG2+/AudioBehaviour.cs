using System.Collections.Generic;
using UnityEngine;

public class AudioBehaviour : MonoBehaviour
{
	private bool gameIsLive;

	private List<AudioHubObject> preGameLiveHubs = new List<AudioHubObject>();

	private void Awake()
	{
		gameIsLive = false;
		GameManager.StageManager.TheGameIsLive += prepLiveGameAudio;
		GameManager.AudioSlinger.AddedNewHub += processNewHub;
		GameManager.AudioSlinger.AddedNewLayer += processNewLayer;
	}

	private void OnDestroy()
	{
	}

	private void prepLiveGameAudio()
	{
		gameIsLive = true;
		for (int i = 0; i < preGameLiveHubs.Count; i++)
		{
			processNewHub(preGameLiveHubs[i]);
		}
		preGameLiveHubs.Clear();
	}

	private void processNewHub(AudioHubObject theHub)
	{
		if (gameIsLive)
		{
			AUDIO_HUB myAudioHub = theHub.MyAudioHub;
			if (myAudioHub == AUDIO_HUB.COMPUTER_HUB && !ControllerManager.Get<computerController>(GAME_CONTROLLER.COMPUTER).Active)
			{
				GameManager.AudioSlinger.MuffleAudioHub(AUDIO_HUB.COMPUTER_HUB, 0.6f);
			}
		}
		else
		{
			preGameLiveHubs.Add(theHub);
		}
	}

	private void processNewLayer(AudioLayerObject theLayer)
	{
		AUDIO_LAYER myAudioLayer = theLayer.MyAudioLayer;
		if (myAudioLayer == AUDIO_LAYER.OUTSIDE && StateManager.PlayerLocation != PLAYER_LOCATION.OUTSIDE)
		{
			GameManager.AudioSlinger.MuffleAudioLayer(AUDIO_LAYER.OUTSIDE, 0f);
		}
	}
}
