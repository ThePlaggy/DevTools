using System;

public class CustomTimer
{
	public Action onFinishAction;

	public float timeout;

	public string name;

	public long created;

	public CustomTimer(Action _onFinishAction, float _timeout, string _name)
	{
		onFinishAction = _onFinishAction;
		timeout = _timeout;
		name = _name;
		created = TimeSlinger.CurrentTimestamp;
	}
}
