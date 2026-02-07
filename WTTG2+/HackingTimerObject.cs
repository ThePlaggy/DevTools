using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HackingTimerObject : MonoBehaviour
{
	public static bool IsWTTG1Hack;

	public Image FillIMG;

	public bool Active;

	public AudioFileDefinition PresentSFX;

	public AudioFileDefinition TickSFX;

	private DOSTween hackerTimerTween;

	private Timer hackerTimeTimer;

	private bool isPaused;

	private Tweener killMeTween;

	private Action myCallBack;

	private CanvasGroup myCG;

	private RectTransform myRT;

	private Timer panicTickTimer;

	private bool tickActive;

	private float tickTimeStamp;

	private Action<float> updateFillAction;

	public Timer HackerTimeTimer => hackerTimeTimer;

	private void Awake()
	{
		myCG = GetComponent<CanvasGroup>();
		myRT = GetComponent<RectTransform>();
		killMeTween = DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 0f, 0.25f).OnComplete(delegate
		{
			GameManager.HackerManager.HackingTimer.setTimerOverLayInactive();
		});
		killMeTween.Pause();
		killMeTween.SetAutoKill(autoKillOnCompletion: false);
		updateFillAction = updateFill;
	}

	private void Update()
	{
		if (!IsWTTG1Hack && tickActive && !isPaused && Time.time - tickTimeStamp >= 0.1f)
		{
			tickTimeStamp = Time.time;
			GameManager.AudioSlinger.PlaySound(TickSFX);
		}
	}

	public void FireMe(float setDur, Action SetCallBack)
	{
		myCG.alpha = 1f;
		Active = true;
		myCallBack = SetCallBack;
		if (!IsWTTG1Hack)
		{
			GameManager.AudioSlinger.PlaySound(PresentSFX);
		}
		myRT.anchoredPosition = new Vector2(0f, 0f - (GetComponent<RectTransform>().sizeDelta.y / 2f + 5f));
		FillIMG.fillAmount = 1f;
		hackerTimerTween = GameManager.TweenSlinger.PlayDOSTweenLiner(1f, 0f, setDur, updateFillAction);
		GameManager.TimeSlinger.FireHardTimer(out hackerTimeTimer, setDur, timesUp);
		float duration = Mathf.Max(setDur * 0.65f, setDur - 7.5f);
		GameManager.TimeSlinger.FireHardTimer(out panicTickTimer, duration, ActivateTick);
	}

	public void ResetMe(float SetDur, Action SetCallBack)
	{
		tickActive = false;
		GameManager.TweenSlinger.KillTween(hackerTimerTween);
		GameManager.TimeSlinger.KillTimer(hackerTimeTimer);
		GameManager.TimeSlinger.KillTimer(panicTickTimer);
		FillIMG.fillAmount = 1f;
		hackerTimerTween = GameManager.TweenSlinger.PlayDOSTweenLiner(1f, 0f, SetDur, updateFillAction);
		GameManager.TimeSlinger.FireHardTimer(out hackerTimeTimer, SetDur, timesUp);
		float duration = Mathf.Max(SetDur * 0.65f, SetDur - 7.5f);
		GameManager.TimeSlinger.FireHardTimer(out panicTickTimer, duration, ActivateTick);
	}

	public void ForceKillMe()
	{
		Active = false;
		tickActive = false;
		isPaused = false;
		GameManager.TweenSlinger.KillTween(hackerTimerTween);
		GameManager.TimeSlinger.KillTimer(hackerTimeTimer);
		GameManager.TimeSlinger.KillTimer(panicTickTimer);
		hackerTimerTween = null;
		hackerTimeTimer = null;
		panicTickTimer = null;
		killMeTween.Restart();
	}

	public void Pause()
	{
		if (hackerTimeTimer != null)
		{
			isPaused = true;
			hackerTimerTween.Pause();
			hackerTimeTimer.Pause();
			panicTickTimer.Pause();
		}
	}

	public void UnPause()
	{
		if (hackerTimeTimer != null)
		{
			isPaused = false;
			hackerTimeTimer.UnPause();
			hackerTimerTween.UnPause();
			panicTickTimer.UnPause();
		}
	}

	public float GetTimeLeft()
	{
		return hackerTimeTimer.TimeLeft;
	}

	private void timesUp()
	{
		if (myCallBack != null)
		{
			myCallBack();
		}
		ForceKillMe();
	}

	private void ActivateTick()
	{
		tickTimeStamp = Time.time;
		tickActive = true;
	}

	private void updateFill(float setValue)
	{
		FillIMG.fillAmount = setValue;
	}
}
