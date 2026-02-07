using System;
using UnityEngine;

public class HackingTimerBehaviour : MonoBehaviour
{
	public GameObject WarmUpClockOverlay;

	public GameObject WarmUpNumberObject;

	public GameObject TimerOverLay;

	public GameObject HackerTimerObject;

	private bool hideWarmUpClockActive;

	private float hideWarmUpDelay;

	private float hideWarmUpTimeStamp;

	private CanvasGroup timerOverlayCG;

	private CanvasGroup warmUpClockOverlayCG;

	private WarmUpNumberObject warmUpNumber;

	private string[] warmUpNumberToStringLookUp = new string[6] { "0", "1", "2", "3", "4", "5" };

	public HackingTimerObject CurrentHackerTimerObject { get; private set; }

	private void Awake()
	{
		warmUpClockOverlayCG = WarmUpClockOverlay.GetComponent<CanvasGroup>();
		timerOverlayCG = TimerOverLay.GetComponent<CanvasGroup>();
		warmUpNumber = UnityEngine.Object.Instantiate(WarmUpNumberObject, WarmUpClockOverlay.GetComponent<RectTransform>()).GetComponent<WarmUpNumberObject>();
		CurrentHackerTimerObject = UnityEngine.Object.Instantiate(HackerTimerObject, TimerOverLay.GetComponent<RectTransform>()).GetComponent<HackingTimerObject>();
	}

	private void Update()
	{
		if (hideWarmUpClockActive && Time.time - hideWarmUpTimeStamp >= hideWarmUpDelay)
		{
			hideWarmUpClockActive = false;
			warmUpClockOverlayCG.alpha = 0f;
		}
	}

	public void FireWarmUpTimer(int setWarmUpTime)
	{
		warmUpClockOverlayCG.alpha = 1f;
		warmUpNumber.FireMe(warmUpNumberToStringLookUp[setWarmUpTime]);
		for (int i = 1; i < setWarmUpTime; i++)
		{
			GameManager.TimeSlinger.FireTimer(i, warmUpNumber.FireMe, warmUpNumberToStringLookUp[setWarmUpTime - i]);
		}
		hideWarmUpDelay = setWarmUpTime;
		hideWarmUpTimeStamp = Time.time;
		hideWarmUpClockActive = true;
	}

	public void FireHackingTimer(float setDur, Action SetCallBack)
	{
		timerOverlayCG.alpha = 1f;
		if (CurrentHackerTimerObject.Active)
		{
			CurrentHackerTimerObject.ResetMe(setDur, SetCallBack);
		}
		else
		{
			CurrentHackerTimerObject.FireMe(setDur, SetCallBack);
		}
	}

	public void KillHackerTimer()
	{
		timerOverlayCG.alpha = 0f;
		if (CurrentHackerTimerObject != null)
		{
			CurrentHackerTimerObject.ForceKillMe();
		}
	}

	public void setTimerOverLayInactive()
	{
		timerOverlayCG.alpha = 0f;
	}
}
