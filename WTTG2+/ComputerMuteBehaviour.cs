using UnityEngine;
using UnityEngine.UI;

public class ComputerMuteBehaviour : MonoBehaviour
{
	public static ComputerMuteBehaviour Ins;

	[SerializeField]
	private float fireMin;

	[SerializeField]
	private float fireMax;

	[SerializeField]
	private Image soundIcon;

	[SerializeField]
	private Sprite mutedSprite;

	[SerializeField]
	private Sprite unmutedSprite;

	private bool fireActive;

	private float fireTimeStamp;

	private float fireTimeWindow;

	public bool Muted { get; private set; }

	private void Awake()
	{
		Ins = this;
		GameManager.StageManager.TheGameIsLive += gameLive;
	}

	private void Update()
	{
		if (fireActive && Time.time - fireTimeStamp >= fireTimeWindow)
		{
			fireActive = false;
			trollUnMute();
		}
	}

	public void ToggleMute()
	{
		if (!TutorialAnnHook.YAAIRunning)
		{
			if (Muted)
			{
				Muted = false;
				GameManager.AudioSlinger.UnMuteAudioLayer(AUDIO_LAYER.WEBSITE);
				soundIcon.sprite = unmutedSprite;
			}
			else
			{
				Muted = true;
				GameManager.AudioSlinger.MuteAudioLayer(AUDIO_LAYER.WEBSITE);
				soundIcon.sprite = mutedSprite;
			}
		}
	}

	public void trollUnMute()
	{
		if (Muted && !StateManager.BeingHacked)
		{
			Muted = false;
			GameManager.AudioSlinger.UnMuteAudioLayer(AUDIO_LAYER.WEBSITE);
			soundIcon.sprite = unmutedSprite;
		}
		generateTrollWindow();
	}

	private void generateTrollWindow()
	{
		fireTimeWindow = Random.Range(fireMin, fireMax);
		fireTimeStamp = Time.time;
		fireActive = true;
	}

	private void gameLive()
	{
		GameManager.StageManager.TheGameIsLive -= gameLive;
		generateTrollWindow();
	}
}
