using System;
using UnityEngine;

[Serializable]
public class AudioFileDefinition : Definition
{
	public AudioClip AudioClip;

	public AUDIO_HUB MyAudioHub;

	public AUDIO_LAYER MyAudioLayer;

	public float Volume;

	public bool Delay;

	public float DelayAmount;

	public bool Loop;

	public int LoopCount;

	public AudioSourceDefinition MyAudioSourceDefinition;

	public AudioFileDefinition()
	{
	}

	public AudioFileDefinition(AudioFileDefinition CopyAFD)
	{
		AudioClip = CopyAFD.AudioClip;
		MyAudioHub = CopyAFD.MyAudioHub;
		MyAudioLayer = CopyAFD.MyAudioLayer;
		Volume = CopyAFD.Volume;
		Delay = CopyAFD.Delay;
		DelayAmount = CopyAFD.DelayAmount;
		Loop = CopyAFD.Loop;
		LoopCount = CopyAFD.LoopCount;
	}
}
