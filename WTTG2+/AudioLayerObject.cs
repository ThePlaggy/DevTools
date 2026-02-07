public class AudioLayerObject
{
	public bool IAmMuffled;

	public bool IAmMuted;

	public bool IAmPaused;

	public CustomEvent IWasDestroyed = new CustomEvent(10);

	public float MuffledAMT;

	public CustomEvent<float> MuffleSound = new CustomEvent<float>(10);

	public AUDIO_LAYER MyAudioLayer;

	public CustomEvent TriggerMute = new CustomEvent(10);

	public CustomEvent TriggerPause = new CustomEvent(10);

	public CustomEvent<float> UnMuffleSound = new CustomEvent<float>(10);

	public AudioLayerObject(AUDIO_LAYER setAudioLayer)
	{
		MyAudioLayer = setAudioLayer;
		IAmMuffled = false;
		MuffledAMT = 0f;
	}

	public AudioLayerObject(AUDIO_LAYER setAudioLayer, bool setAmMuffled, float setMuffleAMT)
	{
		MyAudioLayer = setAudioLayer;
		IAmMuffled = setAmMuffled;
		MuffledAMT = setMuffleAMT;
	}

	public void MuffleMe(float setAMT, float fadeTime = 0f)
	{
		IAmMuffled = true;
		MuffledAMT = setAMT;
		MuffleSound.Execute(fadeTime);
	}

	public void UnMuffleMe(float fadeTime = 0f)
	{
		IAmMuffled = false;
		MuffledAMT = 0f;
		MuffleSound.Execute(fadeTime);
	}

	public void PauseMe()
	{
		IAmPaused = true;
		TriggerPause.Execute();
	}

	public void UnPauseMe()
	{
		IAmPaused = false;
		TriggerPause.Execute();
	}

	public void MuteMe()
	{
		IAmMuted = true;
		TriggerMute.Execute();
	}

	public void UnMuteMe()
	{
		IAmMuted = false;
		TriggerMute.Execute();
	}

	public void KillMe()
	{
		IWasDestroyed.Execute();
	}
}
