using UnityEngine;
using UnityEngine.UI;

public class KernelClock : MonoBehaviour
{
	public KAttack myKAttack;

	public Image left;

	public Image right;

	public float currentValue;

	private float currentTime;

	private float MaxTime;

	private bool ticking;

	private bool finalCountdownTriggered;

	public void FixedUpdate()
	{
		if (currentTime > 0f && ticking)
		{
			currentTime -= 0.02f;
			UpdateValue(currentTime / MaxTime);
		}
		if (ticking && currentTime <= 0f)
		{
			StopTicking();
			myKAttack.TimesUp();
		}
		if (!finalCountdownTriggered && (double)currentTime <= 4.334 && ticking)
		{
			finalCountdownTriggered = true;
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.ClockAlmostUp);
		}
	}

	public void StartTicking(float time)
	{
		UpdateValue(1f);
		MaxTime = time;
		currentTime = time;
		ticking = true;
		finalCountdownTriggered = false;
		GameManager.AudioSlinger.KillSound(CustomSoundLookUp.ClockAlmostUp);
	}

	public void UpdateValue(float value)
	{
		if (value > 1f)
		{
			value = 1f;
		}
		left.fillAmount = value;
		right.fillAmount = value;
		currentValue = value;
	}

	public void BoostTime(float boost)
	{
		currentTime += boost;
		if (currentTime > MaxTime)
		{
			currentTime = MaxTime;
		}
		UpdateValue(currentTime / MaxTime);
		finalCountdownTriggered = false;
		GameManager.AudioSlinger.KillSound(CustomSoundLookUp.ClockAlmostUp);
	}

	public void StopTicking()
	{
		UpdateValue(1f);
		ticking = false;
		finalCountdownTriggered = false;
		GameManager.AudioSlinger.KillSound(CustomSoundLookUp.ClockAlmostUp);
	}
}
