using System;
using UnityEngine;

public class Timer
{
	private bool amActive;

	private float duration;

	private Action freezeAction;

	private float freezeAddTime;

	private float freezeTimeStamp;

	public bool HardTimer;

	private ActionSlinger loopCallBack;

	private int loopCount;

	private ActionSlinger myCallBack;

	private float startTimeStamp;

	private Action tickAction;

	private bool wasFrozen;

	public float TimeLeft { get; private set; }

	public Timer()
	{
		tickAction = Tick;
		freezeAction = FreezeTick;
	}

	public Timer(float timerDuration, int setLoopCount = 0)
	{
		duration = timerDuration;
		startTimeStamp = Time.time;
		loopCount = setLoopCount;
		tickAction = Tick;
		freezeAction = FreezeTick;
	}

	public void Build(float timerDuration, int setLoopCount = 0)
	{
		duration = timerDuration;
		startTimeStamp = Time.time;
		loopCount = setLoopCount;
	}

	public void SetAction(Action setCallBack)
	{
		myCallBack = new ActionStorage(setCallBack);
	}

	public void SetAction<A>(Action<A> setCallBack, A ASetValue)
	{
		myCallBack = new ActionStorage<A>(setCallBack, ASetValue);
	}

	public void SetAction<A, B>(Action<A, B> setCallBack, A ASetValue, B BSetValue)
	{
		myCallBack = new ActionStorage<A, B>(setCallBack, ASetValue, BSetValue);
	}

	public void SetAction<A, B, C>(Action<A, B, C> setCallBack, A ASetValue, B BSetValue, C CSetValue)
	{
		myCallBack = new ActionStorage<A, B, C>(setCallBack, ASetValue, BSetValue, CSetValue);
	}

	public void SetAction<A, B, C, D>(Action<A, B, C, D> setCallBack, A ASetValue, B BSetValue, C CSetValue, D DSetValue)
	{
		myCallBack = new ActionStorage<A, B, C, D>(setCallBack, ASetValue, BSetValue, CSetValue, DSetValue);
	}

	public void AddLoopCallBack(Action setCallBack)
	{
		loopCallBack = new ActionStorage(setCallBack);
	}

	public void AddLoopCallBack<A>(Action<A> setCallBack, A ASetValue)
	{
		loopCallBack = new ActionStorage<A>(setCallBack, ASetValue);
	}

	public void AddLoopCallBack<A, B>(Action<A, B> setCallBack, A ASetValue, B BSetValue)
	{
		loopCallBack = new ActionStorage<A, B>(setCallBack, ASetValue, BSetValue);
	}

	public void AddLoopCallBack<A, B, C>(Action<A, B, C> setCallBack, A ASetValue, B BSetValue, C CSetValue)
	{
		loopCallBack = new ActionStorage<A, B, C>(setCallBack, ASetValue, BSetValue, CSetValue);
	}

	public void AddLoopCallBack<A, B, C, D>(Action<A, B, C, D> setCallBack, A ASetValue, B BSetValue, C CSetValue, D DSetValue)
	{
		loopCallBack = new ActionStorage<A, B, C, D>(setCallBack, ASetValue, BSetValue, CSetValue, DSetValue);
	}

	public void Fire()
	{
		loopCount--;
		freezeTimeStamp = 0f;
		wasFrozen = false;
		freezeAddTime = 0f;
		GameManager.TimeSlinger.Tick.Event += tickAction;
		GameManager.TimeSlinger.FreezeTick.Event += freezeAction;
		amActive = true;
	}

	public void KillMe()
	{
		GameManager.TimeSlinger.Tick.Event -= tickAction;
		GameManager.TimeSlinger.FreezeTick.Event -= freezeAction;
		myCallBack = null;
		loopCallBack = null;
		amActive = false;
		wasFrozen = false;
		loopCount = 0;
		freezeTimeStamp = 0f;
		freezeAddTime = 0f;
		duration = 0f;
		startTimeStamp = 0f;
		TimeLeft = 0f;
	}

	public void Pause()
	{
		FreezeTick();
		amActive = false;
	}

	public void UnPause()
	{
		amActive = true;
	}

	private void TriggerCallBack()
	{
		myCallBack.Fire();
		if (loopCount > 0)
		{
			loopCount--;
			freezeTimeStamp = 0f;
			freezeAddTime = 0f;
			startTimeStamp = Time.time;
			amActive = true;
		}
		else
		{
			if (loopCallBack != null)
			{
				loopCallBack.Fire();
			}
			GameManager.TimeSlinger.KillTimer(this);
		}
	}

	private void Tick()
	{
		if (amActive)
		{
			if (wasFrozen)
			{
				wasFrozen = false;
				freezeAddTime += Time.time - freezeTimeStamp;
			}
			TimeLeft = duration - (Time.time - freezeAddTime - startTimeStamp);
			if (Time.time - freezeAddTime - startTimeStamp >= duration)
			{
				amActive = false;
				TriggerCallBack();
			}
		}
	}

	private void FreezeTick()
	{
		if (amActive && !wasFrozen)
		{
			wasFrozen = true;
			freezeTimeStamp = Time.time;
		}
	}

	~Timer()
	{
		KillMe();
	}
}
