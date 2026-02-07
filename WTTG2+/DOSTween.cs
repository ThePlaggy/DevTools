using System;
using UnityEngine;

public class DOSTween
{
	private bool active;

	private Action<float> callBackAction;

	private float delayPerUnit;

	private Action<float> doneCallBackAction;

	private float freeAddTime;

	private float freezeTimeStamp;

	private float fromValue;

	public bool HardTween;

	private bool ignoreFreezeTime;

	private bool linerTweenActive;

	private float maxTweenDuration;

	private tweenSlinger myTweenSlinger;

	private float startTimeStamp;

	private float stepTimeStamp;

	private bool stepTweenActive;

	private float tempValue;

	private float toValue;

	private float tweenDuration;

	private bool wasFrozen;

	public DOSTween(tweenSlinger curTweenSlinger)
	{
		myTweenSlinger = curTweenSlinger;
		curTweenSlinger.Tick.Event += Tick;
		curTweenSlinger.FrozeTick.Event += FreezeTick;
	}

	~DOSTween()
	{
		if (!HardTween)
		{
			myTweenSlinger.Tick.Event -= Tick;
			myTweenSlinger.FrozeTick.Event -= FreezeTick;
		}
	}

	public void Build(float setFromValue, float setToValue, float setDuration, Action<float> setCallBackAction, bool setIgnoreFreezeTime = false)
	{
		fromValue = setFromValue;
		toValue = setToValue;
		tweenDuration = setDuration;
		ignoreFreezeTime = setIgnoreFreezeTime;
		active = true;
		callBackAction = setCallBackAction;
		startTimeStamp = Time.time;
		linerTweenActive = true;
	}

	public void Build(float setFromValue, float setToValue, float setDuration, Action<float> setCallBackAction, Action<float> setDoneCallBackAction, bool setIgnoreFreezeTime = false)
	{
		Build(setFromValue, setToValue, setDuration, setCallBackAction, setIgnoreFreezeTime);
		doneCallBackAction = setDoneCallBackAction;
	}

	public void Build(float setFromValue, float setToValue, float setDelayPerUnit, float setMaxDuration, Action<float> setCallBackAction, bool setIgnoreFreezeTime = false)
	{
		fromValue = setFromValue;
		toValue = setToValue;
		delayPerUnit = setDelayPerUnit;
		maxTweenDuration = setMaxDuration;
		ignoreFreezeTime = setIgnoreFreezeTime;
		tweenDuration = Mathf.Min(Mathf.Abs(toValue - fromValue) * delayPerUnit, maxTweenDuration);
		startTimeStamp = (stepTimeStamp = Time.time);
		if (tempValue == toValue)
		{
			tempValue = fromValue;
		}
		active = true;
		callBackAction = setCallBackAction;
		stepTweenActive = true;
	}

	public void Build(float setFromValue, float setToValue, float setDelayPerUnit, float setMaxDuration, Action<float> setCallBackAction, Action<float> setDoneCallBackAction, bool setIgnoreFreezeTime = false)
	{
		Build(setFromValue, setToValue, setDelayPerUnit, setMaxDuration, setCallBackAction, setIgnoreFreezeTime);
		doneCallBackAction = setDoneCallBackAction;
	}

	public void Pause()
	{
		FreezeTick();
		active = false;
	}

	public void UnPause()
	{
		active = true;
	}

	public void killMe()
	{
		startTimeStamp = 0f;
		stepTimeStamp = 0f;
		toValue = 0f;
		fromValue = 0f;
		tempValue = 0f;
		delayPerUnit = 0f;
		tweenDuration = 0f;
		maxTweenDuration = 0f;
		freezeTimeStamp = 0f;
		freeAddTime = 0f;
		callBackAction = null;
		doneCallBackAction = null;
		linerTweenActive = false;
		stepTweenActive = false;
		ignoreFreezeTime = false;
		wasFrozen = false;
		active = false;
		if (HardTween)
		{
			myTweenSlinger.Tick.Event -= Tick;
			myTweenSlinger.FrozeTick.Event -= FreezeTick;
		}
	}

	public void Tick()
	{
		if (!active)
		{
			return;
		}
		if (wasFrozen)
		{
			wasFrozen = false;
			freeAddTime += Time.time - freezeTimeStamp;
		}
		if (linerTweenActive)
		{
			if (Time.time - freeAddTime - startTimeStamp < tweenDuration)
			{
				tempValue = Mathf.Lerp(fromValue, toValue, (Time.time - freeAddTime - startTimeStamp) / tweenDuration);
				callBackAction(tempValue);
				return;
			}
			tempValue = toValue;
			callBackAction(tempValue);
			if (doneCallBackAction != null)
			{
				doneCallBackAction(tempValue);
			}
			else
			{
				myTweenSlinger.KillTween(this);
			}
		}
		else
		{
			if (!stepTweenActive)
			{
				return;
			}
			if (Time.time - freeAddTime - stepTimeStamp >= delayPerUnit && tempValue != toValue)
			{
				stepTimeStamp = Time.time;
				tempValue = fromValue + Mathf.Lerp(0f, toValue - fromValue, (Time.time - freeAddTime - startTimeStamp) / tweenDuration);
				callBackAction(tempValue);
			}
			else if (tempValue == toValue)
			{
				if (doneCallBackAction != null)
				{
					doneCallBackAction(tempValue);
				}
				else
				{
					myTweenSlinger.KillTween(this);
				}
			}
		}
	}

	public void FreezeTick()
	{
		if (!active)
		{
			return;
		}
		if (!ignoreFreezeTime)
		{
			if (!wasFrozen)
			{
				wasFrozen = true;
				freezeTimeStamp = Time.time;
			}
		}
		else
		{
			Tick();
		}
	}
}
