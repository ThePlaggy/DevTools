using UnityEngine.Events;

public class UniWindowBehaviour : WindowBehaviour
{
	public UnityEvent CloseEvents;

	public UnityEvent LaunchEvents;

	public UnityEvent MaxEvents;

	public UnityEvent UnMaxEvents;

	public UnityEvent MinEvents;

	public UnityEvent UnMinEvents;

	public UnityEvent ResizedEvents;

	protected override void OnClose()
	{
		CloseEvents?.Invoke();
	}

	protected override void OnLaunch()
	{
		LaunchEvents?.Invoke();
	}

	protected override void OnMax()
	{
		MaxEvents?.Invoke();
	}

	protected override void OnMin()
	{
		MinEvents?.Invoke();
	}

	protected override void OnResized()
	{
		ResizedEvents?.Invoke();
	}

	protected override void OnUnMax()
	{
		UnMaxEvents?.Invoke();
	}

	protected override void OnUnMin()
	{
		UnMinEvents?.Invoke();
	}
}
