using System;
using DG.Tweening;
using UnityEngine;

public class SoundObject : MonoBehaviour
{
	public delegate void SoundObjectDone(SoundObject soundObject);

	private delegate void pauseActions();

	public AudioFileDefinition MyAudioFile;

	private bool audioFileWasPlayed;

	private AudioHubObject.AudioHubVoidActions audioHubDestroyMeAction;

	private AudioHubObject.AudioHubMuffleActions audioHubMuffleAction;

	private AudioHubObject.AudioHubVoidActions audioHubMuteAction;

	private AudioHubObject.AudioHubVoidActions audioHubPauseAction;

	private Action audioLayerDestroyedAction;

	private Action<float> audioLayerMuffleAction;

	private Action audioLayerMuteAction;

	private Action audioLayerPauseAction;

	private int curLoopCount;

	private bool delayIsActive;

	private float delayTimeStamp;

	private float freezeAddTime;

	private float freezeTimeStamp;

	private bool iAmPaused;

	private AudioSource myAS;

	private AudioHubObject myAudioHubObject;

	private AudioLayerObject myAudioLayerObject;

	private bool timeIsFrozen;

	private bool wasFrozen;

	public event SoundObjectDone DonePlaying;

	private event pauseActions soundUnPaused;

	private void Awake()
	{
		myAudioHubObject = null;
		audioHubMuffleAction = mufflePass;
		audioHubPauseAction = triggerPause;
		audioHubDestroyMeAction = destroyMe;
		audioHubMuteAction = triggerMute;
		audioLayerMuffleAction = mufflePass;
		audioLayerPauseAction = triggerPause;
		audioLayerMuteAction = triggerMute;
		audioLayerDestroyedAction = destroyMe;
		if (GameManager.PauseManager != null)
		{
			GameManager.PauseManager.GamePaused += PlayerHitPause;
			GameManager.PauseManager.GameUnPaused += PlayerHitUnPause;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (timeIsFrozen)
		{
			if (!wasFrozen)
			{
				wasFrozen = true;
				freezeTimeStamp = Time.time;
			}
			return;
		}
		if (wasFrozen)
		{
			wasFrozen = false;
			freezeAddTime += Time.time - freezeTimeStamp;
		}
		if (delayIsActive && Time.time - freezeAddTime - delayTimeStamp >= MyAudioFile.DelayAmount)
		{
			delayIsActive = false;
			audioFileWasPlayed = true;
		}
		if (audioFileWasPlayed && !myAS.isPlaying && !iAmPaused)
		{
			audioFileWasPlayed = false;
			if (MyAudioFile.Loop)
			{
				reLoop();
			}
			else
			{
				destroyMe();
			}
		}
	}

	private void OnDestroy()
	{
		if (myAudioHubObject != null)
		{
			myAudioHubObject.MuffleSound -= audioHubMuffleAction;
			myAudioHubObject.UnMuffleSound -= audioHubMuffleAction;
			myAudioHubObject.TriggerPause -= audioHubPauseAction;
			myAudioHubObject.IAmDead -= audioHubDestroyMeAction;
			myAudioHubObject.TriggerMute -= audioHubMuteAction;
		}
		destroyMe();
		if (GameManager.PauseManager != null)
		{
			GameManager.PauseManager.GamePaused -= PlayerHitPause;
			GameManager.PauseManager.GameUnPaused -= PlayerHitUnPause;
		}
	}

	public void AttachAudioHubObject(AudioHubObject attachedAudioHub)
	{
		myAudioHubObject = attachedAudioHub;
	}

	public void Build()
	{
		myAS = base.gameObject.AddComponent<AudioSource>();
		myAS.playOnAwake = false;
		if (myAudioHubObject != null)
		{
			myAudioHubObject.MuffleSound += audioHubMuffleAction;
			myAudioHubObject.UnMuffleSound += audioHubMuffleAction;
			myAudioHubObject.TriggerPause += audioHubPauseAction;
			myAudioHubObject.IAmDead += audioHubDestroyMeAction;
			myAudioHubObject.TriggerMute += audioHubMuteAction;
		}
	}

	public void Fire(AudioFileDefinition AudioFile)
	{
		MyAudioFile = AudioFile;
		if (GameManager.AudioSlinger != null)
		{
			GameManager.AudioSlinger.AddAudioLayer(MyAudioFile.MyAudioLayer, out myAudioLayerObject);
		}
		else
		{
			AudioSlingerHook.Ins.AddAudioLayer(MyAudioFile.MyAudioLayer, out myAudioLayerObject);
		}
		if (myAudioLayerObject != null)
		{
			myAudioLayerObject.MuffleSound.Event += audioLayerMuffleAction;
			myAudioLayerObject.UnMuffleSound.Event += audioLayerMuffleAction;
			myAudioLayerObject.TriggerPause.Event += audioLayerPauseAction;
			myAudioLayerObject.TriggerMute.Event += audioLayerMuteAction;
			myAudioLayerObject.IWasDestroyed.Event += audioLayerDestroyedAction;
		}
		stageAudioSource();
		triggerPause();
		triggerMute();
		stageAudioFile();
		if (iAmPaused)
		{
			soundUnPaused += playAudioFile;
		}
		else
		{
			playAudioFile();
		}
	}

	public void KillMe()
	{
		destroyMe();
	}

	public void SetCustomPitch(float PitchLevel)
	{
		myAS.pitch = PitchLevel;
	}

	private void stageAudioSource()
	{
		if (!(MyAudioFile != null))
		{
			return;
		}
		if (MyAudioFile.MyAudioSourceDefinition.GoCustom)
		{
			myAS.panStereo = MyAudioFile.MyAudioSourceDefinition.CustomAudioSource.panStereo;
			myAS.dopplerLevel = MyAudioFile.MyAudioSourceDefinition.CustomAudioSource.dopplerLevel;
			myAS.spatialBlend = MyAudioFile.MyAudioSourceDefinition.CustomAudioSource.spatialBlend;
			myAS.reverbZoneMix = MyAudioFile.MyAudioSourceDefinition.CustomAudioSource.reverbZoneMix;
			myAS.spread = MyAudioFile.MyAudioSourceDefinition.CustomAudioSource.spread;
			myAS.rolloffMode = AudioRolloffMode.Custom;
			myAS.SetCustomCurve(AudioSourceCurveType.CustomRolloff, MyAudioFile.MyAudioSourceDefinition.CustomAudioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff));
			myAS.minDistance = MyAudioFile.MyAudioSourceDefinition.CustomAudioSource.minDistance;
			myAS.maxDistance = MyAudioFile.MyAudioSourceDefinition.CustomAudioSource.maxDistance;
			return;
		}
		myAS.panStereo = MyAudioFile.MyAudioSourceDefinition.PanStero;
		myAS.spatialBlend = MyAudioFile.MyAudioSourceDefinition.SpatialBlend;
		myAS.reverbZoneMix = MyAudioFile.MyAudioSourceDefinition.ReverbZoneMix;
		myAS.dopplerLevel = MyAudioFile.MyAudioSourceDefinition.DopplerLevel;
		if (MyAudioFile.MyAudioSourceDefinition.IsLiner)
		{
			myAS.rolloffMode = AudioRolloffMode.Linear;
		}
		else
		{
			myAS.rolloffMode = AudioRolloffMode.Logarithmic;
			myAS.spread = MyAudioFile.MyAudioSourceDefinition.Spread;
		}
		myAS.minDistance = MyAudioFile.MyAudioSourceDefinition.MinDistance;
		myAS.maxDistance = MyAudioFile.MyAudioSourceDefinition.MaxDistance;
	}

	private void stageAudioFile()
	{
		if (MyAudioFile != null)
		{
			float num = MyAudioFile.Volume;
			myAS.clip = MyAudioFile.AudioClip;
			if (myAudioHubObject != null && myAudioHubObject.HubMuffled)
			{
				num *= myAudioHubObject.MuffledPerc;
			}
			if (myAudioLayerObject != null && myAudioLayerObject.IAmMuffled)
			{
				num *= myAudioLayerObject.MuffledAMT;
			}
			myAS.volume = num;
		}
	}

	private void playAudioFile()
	{
		if (!(MyAudioFile != null))
		{
			return;
		}
		if (MyAudioFile.Loop)
		{
			if (MyAudioFile.LoopCount == -1)
			{
				myAS.loop = true;
			}
			else
			{
				curLoopCount = MyAudioFile.LoopCount;
			}
		}
		else
		{
			myAS.loop = false;
		}
		if (MyAudioFile.Delay)
		{
			delayTimeStamp = Time.time;
			delayIsActive = true;
			myAS.PlayDelayed(MyAudioFile.DelayAmount);
		}
		else
		{
			myAS.Play();
			audioFileWasPlayed = true;
		}
	}

	private void mufflePass(float FadeTime)
	{
		if (!(MyAudioFile != null))
		{
			return;
		}
		float num = MyAudioFile.Volume;
		if (myAudioHubObject != null && myAudioHubObject.HubMuffled)
		{
			num *= myAudioHubObject.MuffledPerc;
		}
		if (myAudioLayerObject != null && myAudioLayerObject.IAmMuffled)
		{
			num *= myAudioLayerObject.MuffledAMT;
		}
		if (FadeTime == 0f)
		{
			myAS.volume = num;
			return;
		}
		DOTween.To(() => myAS.volume, delegate(float x)
		{
			myAS.volume = x;
		}, num, FadeTime).SetEase(Ease.Linear);
	}

	private void triggerPause()
	{
		bool flag = false;
		if (myAudioHubObject != null)
		{
			flag = myAudioHubObject.HubPaused;
		}
		if (myAudioLayerObject != null && myAudioLayerObject.IAmPaused)
		{
			flag = true;
		}
		iAmPaused = flag;
		if (flag)
		{
			myAS.Pause();
			return;
		}
		myAS.UnPause();
		if (this.soundUnPaused != null)
		{
			this.soundUnPaused();
			soundUnPaused -= playAudioFile;
		}
	}

	private void triggerMute()
	{
		bool mute = false;
		if (myAudioHubObject != null)
		{
			mute = myAudioHubObject.HubMuted;
		}
		if (myAudioLayerObject != null && myAudioLayerObject.IAmMuted)
		{
			mute = true;
		}
		myAS.mute = mute;
	}

	private void reLoop()
	{
		curLoopCount--;
		if (curLoopCount > 0)
		{
			myAS.Play();
			audioFileWasPlayed = true;
		}
		else
		{
			destroyMe();
		}
	}

	private void destroyMe()
	{
		base.enabled = false;
		audioFileWasPlayed = false;
		myAS.Stop();
		myAS.pitch = 1f;
		wasFrozen = false;
		freezeAddTime = 0f;
		freezeTimeStamp = 0f;
		delayIsActive = false;
		delayTimeStamp = 0f;
		curLoopCount = 0;
		if (myAudioHubObject != null)
		{
		}
		if (myAudioLayerObject != null)
		{
			myAudioLayerObject.MuffleSound.Event -= audioLayerMuffleAction;
			myAudioLayerObject.UnMuffleSound.Event -= audioLayerMuffleAction;
			myAudioLayerObject.TriggerPause.Event -= audioLayerPauseAction;
			myAudioLayerObject.TriggerMute.Event -= audioLayerMuteAction;
			myAudioLayerObject.IWasDestroyed.Event -= audioLayerDestroyedAction;
			myAudioLayerObject = null;
		}
		if (this.DonePlaying != null)
		{
			this.DonePlaying(this);
		}
	}

	private void PlayerHitPause()
	{
		if (base.enabled)
		{
			timeIsFrozen = true;
			myAS.Pause();
		}
	}

	private void PlayerHitUnPause()
	{
		if (base.enabled)
		{
			timeIsFrozen = false;
			triggerPause();
		}
	}
}
