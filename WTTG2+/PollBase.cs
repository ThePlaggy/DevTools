public abstract class PollBase
{
	protected Timer pollTimer;

	protected void StartPollTimer()
	{
		GameManager.TimeSlinger.FireHardTimer(out pollTimer, 60f, PollEnd);
	}

	public void RTVKillTimer()
	{
		GameManager.TimeSlinger.KillTimer(pollTimer);
		PollEnd();
	}

	protected abstract void PollEnd();
}
