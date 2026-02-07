using System.Collections.Generic;
using UnityEngine;

public class AudioHubObject : MonoBehaviour
{
	public delegate void AudioHubMuffleActions(float fadeTime);

	public delegate void AudioHubVoidActions();

	public delegate void PlayingActions(AudioFileDefinition TheAFD);

	public int START_SO_POOL_COUNT = 2;

	public AUDIO_HUB MyAudioHub;

	public bool HubPaused;

	public bool HubMuted;

	public bool HubMuffled;

	public float MuffledPerc;

	private Dictionary<AudioClip, List<SoundObject>> mySoundObjects = new Dictionary<AudioClip, List<SoundObject>>(10, AudioClipComparer.Ins);

	private SoundObject.SoundObjectDone soundObjectDoneAction;

	private PooledStack<SoundObject> soundObjectsPool;

	public event AudioHubVoidActions IAmAlive;

	public event AudioHubMuffleActions MuffleSound;

	public event PlayingActions DonePlaying;

	public event AudioHubVoidActions IAmDead;

	public event AudioHubVoidActions TriggerPause;

	public event AudioHubVoidActions TriggerMute;

	public event AudioHubMuffleActions UnMuffleSound;

	private void Awake()
	{
		soundObjectDoneAction = RemoveSoundObject;
		soundObjectsPool = new PooledStack<SoundObject>(delegate
		{
			SoundObject soundObject = base.gameObject.AddComponent<SoundObject>();
			soundObject.AttachAudioHubObject(this);
			soundObject.Build();
			soundObject.enabled = false;
			return soundObject;
		}, START_SO_POOL_COUNT);
		if (MyAudioHub != AUDIO_HUB.UNIVERSAL)
		{
			GameManager.AudioSlinger.AddAudioHub(this);
		}
		if (this.IAmAlive != null)
		{
			this.IAmAlive();
		}
	}

	public void PlaySoundCustomDelay(AudioFileDefinition AudioFile, float SetDelay)
	{
		AudioFileDefinition audioFileDefinition = Object.Instantiate(AudioFile);
		audioFileDefinition.Delay = true;
		audioFileDefinition.DelayAmount = SetDelay;
		PlaySound(audioFileDefinition);
	}

	public void PlaySound(AudioFileDefinition AudioFile)
	{
		if (!TarotManager.DeafActive && (!DifficultyManager.HackerMode || !(HackerModeManager.Ins != null) || !HackerModeManager.Ins.sfxMuted))
		{
			if (EnemyStateManager.HasEnemyState(ENEMY_STATE.EXECUTIONER))
			{
				ExeSoundPopCheck(AudioFile);
			}
			SoundObject soundObject = soundObjectsPool.Pop();
			if (!mySoundObjects.TryGetValue(AudioFile.AudioClip, out var value))
			{
				value = new List<SoundObject>();
				mySoundObjects[AudioFile.AudioClip] = value;
			}
			value.Add(soundObject);
			soundObject.enabled = true;
			soundObject.DonePlaying += soundObjectDoneAction;
			if (DeepWebRadioManager.DWRDizzy)
			{
				soundObject.SetCustomPitch(Random.Range(0.55f, 1.45f));
			}
			soundObject.Fire(AudioFile);
		}
	}

	public void PlaySoundWithWildPitch(AudioFileDefinition AudioFile, float MinLevel, float MaxLevel)
	{
		if (!TarotManager.DeafActive)
		{
			SoundObject soundObject = soundObjectsPool.Pop();
			if (!mySoundObjects.TryGetValue(AudioFile.AudioClip, out var value))
			{
				value = new List<SoundObject>();
				mySoundObjects[AudioFile.AudioClip] = value;
			}
			value.Add(soundObject);
			soundObject.enabled = true;
			soundObject.DonePlaying += soundObjectDoneAction;
			soundObject.SetCustomPitch(Random.Range(MinLevel, MaxLevel));
			soundObject.Fire(AudioFile);
		}
	}

	public void KillSound(AudioClip setAudioClip)
	{
		if (mySoundObjects.TryGetValue(setAudioClip, out var value))
		{
			for (int i = 0; i < value.Count; i++)
			{
				value[i].KillMe();
			}
		}
	}

	public void KillMe()
	{
		if (this.IAmDead != null)
		{
			this.IAmDead();
		}
		mySoundObjects.Clear();
		soundObjectsPool.Clear();
		Object.Destroy(this);
	}

	public void MuffleHub(float setMuffleAmount, float fadeTime = 0f)
	{
		HubMuffled = true;
		MuffledPerc = setMuffleAmount;
		if (this.MuffleSound != null)
		{
			this.MuffleSound(fadeTime);
		}
	}

	public void UnMuffleHub(float fadeTime = 0f)
	{
		HubMuffled = false;
		if (this.UnMuffleSound != null)
		{
			this.UnMuffleSound(fadeTime);
		}
	}

	public void PauseHub()
	{
		HubPaused = true;
		if (this.TriggerPause != null)
		{
			this.TriggerPause();
		}
	}

	public void UnPauseHub()
	{
		HubPaused = false;
		if (this.TriggerPause != null)
		{
			this.TriggerPause();
		}
	}

	public void MuteHub()
	{
		HubMuted = true;
		if (this.TriggerMute != null)
		{
			this.TriggerMute();
		}
	}

	public void UnMuteHub()
	{
		HubMuted = false;
		if (this.TriggerMute != null)
		{
			this.TriggerMute();
		}
	}

	private void RemoveSoundObject(SoundObject soundObjectToRemove)
	{
		soundObjectToRemove.DonePlaying -= soundObjectDoneAction;
		if (this.DonePlaying != null)
		{
			this.DonePlaying(soundObjectToRemove.MyAudioFile);
		}
		if (mySoundObjects.TryGetValue(soundObjectToRemove.MyAudioFile.AudioClip, out var value))
		{
			value.Remove(soundObjectToRemove);
			soundObjectsPool.Push(soundObjectToRemove);
		}
	}

	private void ExeSoundPopCheck(AudioFileDefinition afd)
	{
		for (int i = 0; i < LookUp.SoundLookUp.KeyboardSounds.Length; i++)
		{
			if (afd == LookUp.SoundLookUp.KeyboardSounds[i])
			{
				EXESoundPopper.PopSound(1);
			}
		}
		if (afd == LookUp.SoundLookUp.MouseClick)
		{
			EXESoundPopper.PopSound(2);
		}
		if (afd == LookUp.SoundLookUp.KeyboardSpaceReturnSound)
		{
			EXESoundPopper.PopSound(2);
		}
		if (afd == LookUp.SoundLookUp.PuzzleFailClick)
		{
			EXESoundPopper.PopSound(2);
		}
		if (afd == LookUp.SoundLookUp.PuzzleGoodClick)
		{
			EXESoundPopper.PopSound(2);
		}
		if (afd == LookUp.SoundLookUp.CantBuyItem)
		{
			EXESoundPopper.PopSound(3);
		}
		if (afd == LookUp.SoundLookUp.KeyFound)
		{
			EXESoundPopper.PopSound(3);
		}
		if (afd == LookUp.SoundLookUp.PuzzleSolved)
		{
			EXESoundPopper.PopSound(99);
		}
		if (afd == LookUp.SoundLookUp.HolyClick)
		{
			EXESoundPopper.PopSound(99);
		}
	}
}
