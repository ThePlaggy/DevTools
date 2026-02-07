using System;
using System.Collections.Generic;

public class tweenSlinger
{
	public CustomEvent FrozeTick = new CustomEvent(10);

	public CustomEvent Tick = new CustomEvent(10);

	private bool timeFroze;

	private PooledStack<DOSTween> tweenPool;

	private HashSet<DOSTween> tweens = new HashSet<DOSTween>();

	public tweenSlinger()
	{
		GameManager.PauseManager.GamePaused += playerHasPaused;
		GameManager.PauseManager.GameUnPaused += playerHasUnPaused;
		tweenPool = new PooledStack<DOSTween>(() => new DOSTween(this), 5);
	}

	~tweenSlinger()
	{
		GameManager.PauseManager.GamePaused -= playerHasPaused;
		GameManager.PauseManager.GameUnPaused -= playerHasUnPaused;
	}

	public void FireDOSTweenLiner(float setFromValue, float setToValue, float setDuration, Action<float> setCallBackAction)
	{
		DOSTween dOSTween = tweenPool.Pop();
		dOSTween.Build(setFromValue, setToValue, setDuration, setCallBackAction);
		tweens.Add(dOSTween);
	}

	public void FireDOSTweenLiner(float setFromValue, float setToValue, float setDuration, Action<float> setCallBackAction, bool ignoreFreezeTime = false)
	{
		DOSTween dOSTween = tweenPool.Pop();
		dOSTween.Build(setFromValue, setToValue, setDuration, setCallBackAction, ignoreFreezeTime);
		tweens.Add(dOSTween);
	}

	public void FireDOSTweenLiner(float setFromValue, float setToValue, float setDuration, Action<float> setCallBackAction, Action<float> setDoneCallBackAction)
	{
		DOSTween dOSTween = tweenPool.Pop();
		dOSTween.Build(setFromValue, setToValue, setDuration, setCallBackAction, setDoneCallBackAction);
		tweens.Add(dOSTween);
	}

	public void FireDOSTweenLiner(float setFromValue, float setToValue, float setDuration, Action<float> setCallBackAction, Action<float> setDoneCallBackAction, bool ignoreFreezeTime = false)
	{
		DOSTween dOSTween = tweenPool.Pop();
		dOSTween.Build(setFromValue, setToValue, setDuration, setCallBackAction, setDoneCallBackAction, ignoreFreezeTime);
		tweens.Add(dOSTween);
	}

	public DOSTween PlayDOSTweenLiner(float setFromValue, float setToValue, float setDuration, Action<float> setCallBackAction)
	{
		DOSTween dOSTween = new DOSTween(this);
		dOSTween.HardTween = true;
		dOSTween.Build(setFromValue, setToValue, setDuration, setCallBackAction);
		tweens.Add(dOSTween);
		return dOSTween;
	}

	public DOSTween PlayDOSTweenLiner(float setFromValue, float setToValue, float setDuration, Action<float> setCallBackAction, bool ignoreFreezeTime = false)
	{
		DOSTween dOSTween = new DOSTween(this);
		dOSTween.HardTween = true;
		dOSTween.Build(setFromValue, setToValue, setDuration, setCallBackAction, ignoreFreezeTime);
		tweens.Add(dOSTween);
		return dOSTween;
	}

	public DOSTween PlayDOSTweenLiner(float setFromValue, float setToValue, float setDuration, Action<float> setCallBackAction, Action<float> setDoneCallBackAction)
	{
		DOSTween dOSTween = new DOSTween(this);
		dOSTween.HardTween = true;
		dOSTween.Build(setFromValue, setToValue, setDuration, setCallBackAction, setDoneCallBackAction);
		tweens.Add(dOSTween);
		return dOSTween;
	}

	public DOSTween PlayDOSTweenLiner(float setFromValue, float setToValue, float setDuration, Action<float> setCallBackAction, Action<float> setDoneCallBackAction, bool ignoreFreezeTime = false)
	{
		DOSTween dOSTween = new DOSTween(this);
		dOSTween.HardTween = true;
		dOSTween.Build(setFromValue, setToValue, setDuration, setCallBackAction, setDoneCallBackAction, ignoreFreezeTime);
		tweens.Add(dOSTween);
		return dOSTween;
	}

	public void FireDOSTweenStep(float setFromValue, float setToValue, float setDelayPerUnit, float setMaxDuration, Action<float> setCallBackAction)
	{
		DOSTween dOSTween = new DOSTween(this);
		dOSTween.HardTween = true;
		dOSTween.Build(setFromValue, setToValue, setDelayPerUnit, setMaxDuration, setCallBackAction);
		tweens.Add(dOSTween);
	}

	public void FireDOSTweenStep(float setFromValue, float setToValue, float setDelayPerUnit, float setMaxDuration, Action<float> setCallBackAction, bool ignoreFreezeTime = false)
	{
		DOSTween dOSTween = tweenPool.Pop();
		dOSTween.Build(setFromValue, setToValue, setDelayPerUnit, setMaxDuration, setCallBackAction, ignoreFreezeTime);
		tweens.Add(dOSTween);
	}

	public void FireDOSTweenStep(float setFromValue, float setToValue, float setDelayPerUnit, float setMaxDuration, Action<float> setCallBackAction, Action<float> setDoneCallBackAction)
	{
		DOSTween dOSTween = tweenPool.Pop();
		dOSTween.Build(setFromValue, setToValue, setDelayPerUnit, setMaxDuration, setCallBackAction, setDoneCallBackAction);
		tweens.Add(dOSTween);
	}

	public void FireDOSTweenStep(float setFromValue, float setToValue, float setDelayPerUnit, float setMaxDuration, Action<float> setCallBackAction, Action<float> setDoneCallBackAction, bool ignoreFreezeTime = false)
	{
		DOSTween dOSTween = tweenPool.Pop();
		dOSTween.Build(setFromValue, setToValue, setDelayPerUnit, setMaxDuration, setCallBackAction, setDoneCallBackAction, ignoreFreezeTime);
		tweens.Add(dOSTween);
	}

	public DOSTween PlayDOSTweenStep(float setFromValue, float setToValue, float setDelayPerUnit, float setMaxDuration, Action<float> setCallBackAction)
	{
		DOSTween dOSTween = new DOSTween(this);
		dOSTween.HardTween = true;
		dOSTween.Build(setFromValue, setToValue, setDelayPerUnit, setMaxDuration, setCallBackAction);
		tweens.Add(dOSTween);
		return dOSTween;
	}

	public DOSTween PlayDOSTweenStep(float setFromValue, float setToValue, float setDelayPerUnit, float setMaxDuration, Action<float> setCallBackAction, bool ignoreFreezeTime = false)
	{
		DOSTween dOSTween = new DOSTween(this);
		dOSTween.HardTween = true;
		dOSTween.Build(setFromValue, setToValue, setDelayPerUnit, setMaxDuration, setCallBackAction, ignoreFreezeTime);
		tweens.Add(dOSTween);
		return dOSTween;
	}

	public DOSTween PlayDOSTweenStep(float setFromValue, float setToValue, float setDelayPerUnit, float setMaxDuration, Action<float> setCallBackAction, Action<float> setDoneCallBackAction)
	{
		DOSTween dOSTween = new DOSTween(this);
		dOSTween.HardTween = true;
		dOSTween.Build(setFromValue, setToValue, setDelayPerUnit, setMaxDuration, setCallBackAction, setDoneCallBackAction);
		tweens.Add(dOSTween);
		return dOSTween;
	}

	public DOSTween PlayDOSTweenStep(float setFromValue, float setToValue, float setDelayPerUnit, float setMaxDuration, Action<float> setCallBackAction, Action<float> setDoneCallBackAction, bool ignoreFreezeTime = false)
	{
		DOSTween dOSTween = new DOSTween(this);
		dOSTween.HardTween = true;
		dOSTween.Build(setFromValue, setToValue, setDelayPerUnit, setMaxDuration, setCallBackAction, setDoneCallBackAction, ignoreFreezeTime);
		tweens.Add(dOSTween);
		return dOSTween;
	}

	public void KillAllTweens()
	{
		foreach (DOSTween tween in tweens)
		{
			tween.killMe();
			if (!tween.HardTween)
			{
				tweenPool.Push(tween);
			}
		}
		tweens.Clear();
	}

	public void KillTween(DOSTween theTween)
	{
		if (tweens.Remove(theTween))
		{
			theTween.killMe();
			if (!theTween.HardTween)
			{
				tweenPool.Push(theTween);
			}
			theTween = null;
		}
	}

	public void Update()
	{
		if (!timeFroze)
		{
			Tick.Execute();
		}
		else
		{
			FrozeTick.Execute();
		}
	}

	private void playerHasPaused()
	{
		timeFroze = true;
	}

	private void playerHasUnPaused()
	{
		timeFroze = false;
	}
}
