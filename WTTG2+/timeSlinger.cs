using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

public class timeSlinger
{
	public CustomEvent FreezeTick = new CustomEvent(10);

	private bool freezeTime;

	private bool isFromGameManager;

	public CustomEvent Tick = new CustomEvent(10);

	private PooledStack<Timer> timerPool;

	private HashSet<Timer> timers = new HashSet<Timer>();

	public timeSlinger(bool FromGameManager = true)
	{
		if (FromGameManager)
		{
			GameManager.PauseManager.GamePaused += playerHasPaused;
			GameManager.PauseManager.GameUnPaused += playerHasUnPaused;
			isFromGameManager = false;
		}
		timerPool = new PooledStack<Timer>(() => new Timer(), 10);
	}

	~timeSlinger()
	{
		if (isFromGameManager)
		{
			GameManager.PauseManager.GamePaused -= playerHasPaused;
			GameManager.PauseManager.GameUnPaused -= playerHasUnPaused;
		}
	}

	public void FireTimer(float duration, Action callBack, int loopCount = 0)
	{
		Timer timer = timerPool.Pop();
		timer.Build(duration, loopCount);
		timer.SetAction(callBack);
		timer.Fire();
		timers.Add(timer);
	}

	public void FireHardTimer(out Timer returnTimer, float duration, Action callBack, int loopCount = 0)
	{
		returnTimer = new Timer();
		returnTimer.HardTimer = true;
		returnTimer.Build(duration, loopCount);
		returnTimer.SetAction(callBack);
		returnTimer.Fire();
		timers.Add(returnTimer);
	}

	public void FireTimer<A>(float duration, Action<A> callBack, A callBackValue, int loopCount = 0)
	{
		Timer timer = timerPool.Pop();
		timer.Build(duration, loopCount);
		timer.SetAction(callBack, callBackValue);
		timer.Fire();
		timers.Add(timer);
	}

	public void FireHardTimer<A>(out Timer returnTimer, float duration, Action<A> callBack, A callBackValue, int loopCount = 0)
	{
		returnTimer = new Timer();
		returnTimer.HardTimer = true;
		returnTimer.Build(duration, loopCount);
		returnTimer.SetAction(callBack, callBackValue);
		returnTimer.Fire();
		timers.Add(returnTimer);
	}

	public void FireTimer<A, B>(float duration, Action<A, B> callBack, A aCallBackValue, B bCallBackValue, int loopCount = 0)
	{
		Timer timer = timerPool.Pop();
		timer.Build(duration, loopCount);
		timer.SetAction(callBack, aCallBackValue, bCallBackValue);
		timer.Fire();
		timers.Add(timer);
	}

	public void FireHardTimer<A, B>(out Timer returnTimer, float duration, Action<A, B> callBack, A aCallBackValue, B bCallBackValue, int loopCount = 0)
	{
		returnTimer = new Timer();
		returnTimer.HardTimer = true;
		returnTimer.Build(duration, loopCount);
		returnTimer.SetAction(callBack, aCallBackValue, bCallBackValue);
		returnTimer.Fire();
		timers.Add(returnTimer);
	}

	public void FireTimer<A, B, C>(float duration, Action<A, B, C> callBack, A aCallBackValue, B bCallBackValue, C cCallBackValue, int loopCount = 0)
	{
		Timer timer = timerPool.Pop();
		timer.Build(duration, loopCount);
		timer.SetAction(callBack, aCallBackValue, bCallBackValue, cCallBackValue);
		timer.Fire();
		timers.Add(timer);
	}

	public void FireHardTimer<A, B, C>(out Timer returnTimer, float duration, Action<A, B, C> callBack, A aCallBackValue, B bCallBackValue, C cCallBackValue, int loopCount = 0)
	{
		returnTimer = new Timer();
		returnTimer.HardTimer = true;
		returnTimer.Build(duration, loopCount);
		returnTimer.SetAction(callBack, aCallBackValue, bCallBackValue, cCallBackValue);
		returnTimer.Fire();
		timers.Add(returnTimer);
	}

	public void FireTimer<A, B, C, D>(float duration, Action<A, B, C, D> callBack, A aCallBackValue, B bCallBackValue, C cCallBackValue, D dCallBackValue, int loopCount = 0)
	{
		Timer timer = timerPool.Pop();
		timer.Build(duration, loopCount);
		timer.SetAction(callBack, aCallBackValue, bCallBackValue, cCallBackValue, dCallBackValue);
		timer.Fire();
		timers.Add(timer);
	}

	public void FireHardTimer<A, B, C, D>(out Timer returnTimer, float duration, Action<A, B, C, D> callBack, A aCallBackValue, B bCallBackValue, C cCallBackValue, D dCallBackValue, int loopCount = 0)
	{
		returnTimer = new Timer();
		returnTimer.HardTimer = true;
		returnTimer.Build(duration, loopCount);
		returnTimer.SetAction(callBack, aCallBackValue, bCallBackValue, cCallBackValue, dCallBackValue);
		returnTimer.Fire();
		timers.Add(returnTimer);
	}

	public void KillTimer(Timer timerToKill)
	{
		if (timerToKill != null)
		{
			timerToKill.KillMe();
			timers.Remove(timerToKill);
			if (!timerToKill.HardTimer)
			{
				timerPool.Push(timerToKill);
			}
			timerToKill = null;
		}
	}

	public void Update()
	{
		if (!freezeTime)
		{
			Tick.Execute();
		}
		else
		{
			FreezeTick.Execute();
		}
	}

	private void playerHasPaused()
	{
		freezeTime = true;
	}

	private void playerHasUnPaused()
	{
		freezeTime = false;
	}
}
public static class TimeSlinger
{
	public static List<CustomInterval> ActiveIntervals = new List<CustomInterval>();

	public static List<CustomTimer> ActiveTimers = new List<CustomTimer>();

	private static System.Timers.Timer updateTimer = new System.Timers.Timer();

	private static Random random = new Random();

	public static int CurrentTimestamp => (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

	public static long CurrentTimestampMs => (long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

	private static void TimeSlingerTick(object sender, ElapsedEventArgs e)
	{
		foreach (CustomTimer item in ActiveTimers.Where((CustomTimer timer) => timer.timeout - (float)(CurrentTimestamp - timer.created) < 0f).ToList())
		{
			try
			{
				item.onFinishAction?.Invoke();
			}
			catch
			{
				ActiveTimers.Remove(item);
				throw;
			}
			finally
			{
				ActiveTimers.Remove(item);
			}
		}
		foreach (CustomInterval item2 in ActiveIntervals.Where((CustomInterval interval) => interval.timeout - (float)(CurrentTimestamp - interval.created) < 0f).ToList())
		{
			try
			{
				item2.onExecuteAction?.Invoke();
			}
			catch
			{
				ActiveIntervals.Remove(item2);
				throw;
			}
			finally
			{
				item2.created = CurrentTimestamp;
			}
		}
	}

	public static void FireTimer(Action onFinishAction, float timeout, string timerName = "")
	{
		ActiveTimers.Add(new CustomTimer(onFinishAction, timeout, timerName));
	}

	public static void KillTimer(string timerName)
	{
		ActiveTimers.RemoveAll((CustomTimer timer) => timer.name == timerName);
	}

	public static bool TimerExists(string timerName)
	{
		return ActiveTimers.Find((CustomTimer timer) => timer.name == timerName) != null;
	}

	public static void ForceExecuteTimer(string timerName)
	{
		CustomTimer customTimer = ActiveTimers.Find((CustomTimer _timer) => _timer.name == timerName);
		try
		{
			customTimer.onFinishAction?.Invoke();
		}
		catch
		{
			ActiveTimers.Remove(customTimer);
			throw;
		}
		finally
		{
			ActiveTimers.Remove(customTimer);
		}
	}

	public static void FireInterval(Action onExecuteAction, float timeout, string intervalName = "")
	{
		ActiveIntervals.Add(new CustomInterval(onExecuteAction, timeout, intervalName));
	}

	public static void FireIntervalUntil(Func<bool> condition, Action onExecuteAction, Action onConditionAction, float timeout, string untilIntervalName = "")
	{
		if (string.IsNullOrEmpty(untilIntervalName))
		{
			untilIntervalName = randomID();
		}
		FireInterval(delegate
		{
			onExecuteAction?.Invoke();
			if (condition())
			{
				onConditionAction?.Invoke();
				KillInterval(untilIntervalName);
			}
		}, timeout, untilIntervalName);
	}

	public static void KillInterval(string intervalName)
	{
		ActiveIntervals.RemoveAll((CustomInterval interval) => interval.name == intervalName);
	}

	public static bool IntervalExists(string intervalName)
	{
		return ActiveIntervals.Find((CustomInterval interval) => interval.name == intervalName) != null;
	}

	public static void ForceExecuteInterval(string intervalName)
	{
		CustomInterval customInterval = ActiveIntervals.Find((CustomInterval _interval) => _interval.name == intervalName);
		try
		{
			customInterval.onExecuteAction?.Invoke();
		}
		catch
		{
			ActiveIntervals.Remove(customInterval);
			throw;
		}
		finally
		{
			customInterval.created = CurrentTimestamp;
		}
	}

	public static void Init()
	{
		updateTimer.Elapsed += TimeSlingerTick;
		updateTimer.Interval = 100.0;
		updateTimer.Start();
	}

	private static string randomID(int length = 16)
	{
		return new string((from s in Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
			select s[random.Next(s.Length)]).ToArray());
	}
}
