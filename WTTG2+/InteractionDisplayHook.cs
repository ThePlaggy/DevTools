using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(InteractionHook))]
public class InteractionDisplayHook : MonoBehaviour
{
	public bool hasLeftClickDisplay;

	public bool hasRightClickDisplay;

	public bool requireLocationCheckForLeftClick;

	public bool requireLocationCheckForRightClick;

	public PLAYER_LOCATION leftClickLocation;

	public PLAYER_LOCATION rightClickLocation;

	public UnityEvent leftClickHoverEvents;

	public UnityEvent leftClickExitHoverEvents;

	public UnityEvent rightClickHoverEvents;

	public UnityEvent rightClickExitHoverEvents;

	private InteractionHook myInteractionHook;

	private void Awake()
	{
		myInteractionHook = GetComponent<InteractionHook>();
		myInteractionHook.RecvAction += hoverState;
		myInteractionHook.RecindAction += exitHoverState;
	}

	private void OnDestroy()
	{
		myInteractionHook.RecvAction -= hoverState;
		myInteractionHook.RecindAction -= exitHoverState;
	}

	private void hoverState()
	{
		if (hasLeftClickDisplay)
		{
			bool flag = true;
			if (requireLocationCheckForLeftClick && StateManager.PlayerLocation != leftClickLocation)
			{
				flag = false;
			}
			if (flag)
			{
				leftClickHoverEvents.Invoke();
			}
		}
		if (hasRightClickDisplay)
		{
			bool flag2 = true;
			if (requireLocationCheckForRightClick && StateManager.PlayerLocation != rightClickLocation)
			{
				flag2 = false;
			}
			if (flag2)
			{
				rightClickHoverEvents.Invoke();
			}
		}
	}

	private void exitHoverState()
	{
		if (hasLeftClickDisplay)
		{
			bool flag = true;
			if (requireLocationCheckForLeftClick && StateManager.PlayerLocation != leftClickLocation)
			{
				flag = false;
			}
			if (flag)
			{
				leftClickExitHoverEvents.Invoke();
			}
		}
		if (hasRightClickDisplay)
		{
			bool flag2 = true;
			if (requireLocationCheckForRightClick && StateManager.PlayerLocation != rightClickLocation)
			{
				flag2 = false;
			}
			if (flag2)
			{
				rightClickExitHoverEvents.Invoke();
			}
		}
	}
}
