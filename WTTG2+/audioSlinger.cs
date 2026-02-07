using System.Collections.Generic;

public class audioSlinger
{
	public delegate void AudioHubActions(AudioHubObject theHub);

	public delegate void AudioLayerActions(AudioLayerObject theLayer);

	private Dictionary<AUDIO_HUB, AudioHubObject> currentAudioHubs = new Dictionary<AUDIO_HUB, AudioHubObject>(EnumComparer.AudioHubCompare);

	private Dictionary<AUDIO_LAYER, AudioLayerObject> currentAudioLayers = new Dictionary<AUDIO_LAYER, AudioLayerObject>(EnumComparer.AudioLayerCompare);

	public event AudioHubActions AddedNewHub;

	public event AudioLayerActions AddedNewLayer;

	public void AddAudioHub(AudioHubObject theHub)
	{
		if (currentAudioHubs.TryGetValue(theHub.MyAudioHub, out var value))
		{
			value.KillMe();
			currentAudioHubs.Remove(theHub.MyAudioHub);
		}
		currentAudioHubs.Add(theHub.MyAudioHub, theHub);
		if (this.AddedNewHub != null)
		{
			this.AddedNewHub(theHub);
		}
	}

	public void RemoveAudioHub(AUDIO_HUB hubToRemove)
	{
		if (currentAudioHubs.TryGetValue(hubToRemove, out var value))
		{
			value.KillMe();
			currentAudioHubs.Remove(hubToRemove);
		}
	}

	public void MuffleAudioHub(AUDIO_HUB hubToMuffle, float setPer, float fadeTime = 0f)
	{
		if (currentAudioHubs.ContainsKey(hubToMuffle))
		{
			currentAudioHubs[hubToMuffle].MuffleHub(setPer, fadeTime);
		}
	}

	public void UnMuffleAudioHub(AUDIO_HUB hubToUnMuffle, float fadeTime = 0f)
	{
		if (currentAudioHubs.ContainsKey(hubToUnMuffle))
		{
			currentAudioHubs[hubToUnMuffle].UnMuffleHub(fadeTime);
		}
	}

	public void MuteAudioHub(AUDIO_HUB hubToMute)
	{
		if (currentAudioHubs.ContainsKey(hubToMute))
		{
			currentAudioHubs[hubToMute].MuteHub();
		}
	}

	public void UnMuteAudioHub(AUDIO_HUB hubToUnMute)
	{
		if (currentAudioHubs.ContainsKey(hubToUnMute))
		{
			currentAudioHubs[hubToUnMute].UnMuteHub();
		}
	}

	public void PauseAudioHub(AUDIO_HUB hubToPause)
	{
		if (currentAudioHubs.ContainsKey(hubToPause))
		{
			currentAudioHubs[hubToPause].PauseHub();
		}
	}

	public void UnPauseAudioHub(AUDIO_HUB hubToUnPause)
	{
		if (currentAudioHubs.ContainsKey(hubToUnPause))
		{
			currentAudioHubs[hubToUnPause].UnPauseHub();
		}
	}

	public void AddAudioLayer(AUDIO_LAYER AudioLayer)
	{
		if (!currentAudioLayers.ContainsKey(AudioLayer))
		{
			currentAudioLayers.Add(AudioLayer, new AudioLayerObject(AudioLayer));
			if (this.AddedNewLayer != null)
			{
				this.AddedNewLayer(currentAudioLayers[AudioLayer]);
			}
		}
	}

	public void AddAudioLayer(AUDIO_LAYER AudioLayer, out AudioLayerObject returnAudioLayer)
	{
		if (!currentAudioLayers.ContainsKey(AudioLayer))
		{
			currentAudioLayers.Add(AudioLayer, new AudioLayerObject(AudioLayer));
			if (this.AddedNewLayer != null)
			{
				this.AddedNewLayer(currentAudioLayers[AudioLayer]);
			}
		}
		returnAudioLayer = currentAudioLayers[AudioLayer];
	}

	public void RemoveAudioLayer(AUDIO_LAYER AudioLayerToRemove)
	{
		if (currentAudioLayers.TryGetValue(AudioLayerToRemove, out var value))
		{
			value.KillMe();
			currentAudioLayers.Remove(AudioLayerToRemove);
		}
	}

	public void MuffleAudioLayer(AUDIO_LAYER AudioLayerToMuffle, float setPer, float fadeTime = 0f)
	{
		if (currentAudioLayers.ContainsKey(AudioLayerToMuffle))
		{
			currentAudioLayers[AudioLayerToMuffle].MuffleMe(setPer, fadeTime);
		}
	}

	public void UnMuffleAudioLayer(AUDIO_LAYER AudioLayerToUnMuffle, float fadeTime = 0f)
	{
		if (currentAudioLayers.ContainsKey(AudioLayerToUnMuffle))
		{
			currentAudioLayers[AudioLayerToUnMuffle].UnMuffleMe(fadeTime);
		}
	}

	public void MuteAudioLayer(AUDIO_LAYER AudioLayerToMute)
	{
		if (currentAudioLayers.ContainsKey(AudioLayerToMute))
		{
			currentAudioLayers[AudioLayerToMute].MuteMe();
		}
	}

	public void UnMuteAudioLayer(AUDIO_LAYER AudioLayerToUnMute)
	{
		if (currentAudioLayers.ContainsKey(AudioLayerToUnMute))
		{
			currentAudioLayers[AudioLayerToUnMute].UnMuteMe();
		}
	}

	public void PauseAudioLayer(AUDIO_LAYER AudioLayerToPause)
	{
		if (currentAudioLayers.ContainsKey(AudioLayerToPause))
		{
			currentAudioLayers[AudioLayerToPause].PauseMe();
		}
	}

	public void UnPauseAudioLayer(AUDIO_LAYER AudioLayerToUnPause)
	{
		if (currentAudioLayers.ContainsKey(AudioLayerToUnPause))
		{
			currentAudioLayers[AudioLayerToUnPause].UnPauseMe();
		}
	}

	public void PlaySound(AudioFileDefinition AudioFile)
	{
		if (currentAudioHubs.ContainsKey(AudioFile.MyAudioHub))
		{
			currentAudioHubs[AudioFile.MyAudioHub].PlaySound(AudioFile);
		}
	}

	public void PlaySoundWithWildPitch(AudioFileDefinition AudioFile, float Min, float Max)
	{
		if (currentAudioHubs.ContainsKey(AudioFile.MyAudioHub))
		{
			currentAudioHubs[AudioFile.MyAudioHub].PlaySoundWithWildPitch(AudioFile, Min, Max);
		}
	}

	public void PlaySoundWithCustomDelay(AudioFileDefinition AudioFile, float SetDelay)
	{
		if (currentAudioHubs.ContainsKey(AudioFile.MyAudioHub))
		{
			currentAudioHubs[AudioFile.MyAudioHub].PlaySoundCustomDelay(AudioFile, SetDelay);
		}
	}

	public void KillSound(AudioFileDefinition AudioFile)
	{
		if (currentAudioHubs.ContainsKey(AudioFile.MyAudioHub))
		{
			currentAudioHubs[AudioFile.MyAudioHub].KillSound(AudioFile.AudioClip);
		}
	}

	public void UniKillSound(AudioFileDefinition AudioFile)
	{
		foreach (KeyValuePair<AUDIO_HUB, AudioHubObject> currentAudioHub in currentAudioHubs)
		{
			currentAudioHub.Value.KillSound(AudioFile.AudioClip);
		}
	}
}
