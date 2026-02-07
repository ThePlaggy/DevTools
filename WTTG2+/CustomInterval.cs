using System;

public class CustomInterval
{
	public Action onExecuteAction;

	public float timeout;

	public string name;

	public long created;

	public CustomInterval(Action _onExecuteAction, float _timeout, string _name)
	{
		onExecuteAction = _onExecuteAction;
		timeout = _timeout;
		name = _name;
		created = TimeSlinger.CurrentTimestamp;
	}
}
