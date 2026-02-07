using UnityEngine;
using UnityEngine.EventSystems;

public class NodeHexInteractObject : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public delegate void SetData(MATRIX_STACK_CLOCK_POSITION SetPOS);

	public delegate bool SetDirection(MATRIX_STACK_CLOCK_POSITION SetPOS);

	public bool Playable;

	public MATRIX_STACK_CLOCK_POSITION Position;

	public CanvasGroup HoverCG;

	public CanvasGroup ActiveCG;

	private bool amLocked;

	private bool amSet;

	public event SetData CounterDirectionMouseEnter;

	public event SetDirection SetNodeHexDirection;

	public event SetData CounterDirectionMouseExit;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!Playable || amLocked)
		{
			return;
		}
		if (amSet)
		{
			if (this.SetNodeHexDirection != null && this.SetNodeHexDirection(MATRIX_STACK_CLOCK_POSITION.NEUTRAL))
			{
				HoverCG.alpha = 0f;
				ActiveCG.alpha = 0f;
				amSet = false;
			}
		}
		else if (this.SetNodeHexDirection != null && this.SetNodeHexDirection(Position))
		{
			HoverCG.alpha = 0f;
			ActiveCG.alpha = 1f;
			amSet = true;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (Playable && !amLocked)
		{
			HoverCG.alpha = 1f;
			if (this.CounterDirectionMouseEnter != null)
			{
				this.CounterDirectionMouseEnter(Position);
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (Playable && !amLocked)
		{
			HoverCG.alpha = 0f;
			if (this.CounterDirectionMouseExit != null)
			{
				this.CounterDirectionMouseExit(Position);
			}
		}
	}

	public void ClearState()
	{
		if (!amLocked)
		{
			HoverCG.alpha = 0f;
			ActiveCG.alpha = 0f;
			amSet = false;
		}
	}

	public void CounterDirectionMouseOver()
	{
		HoverCG.alpha = 1f;
	}

	public void CounterDirectionMouseOut()
	{
		HoverCG.alpha = 0f;
	}

	public void ActivateCounterDirection()
	{
		amLocked = true;
		ActiveCG.alpha = 1f;
	}

	public void DeActivateCounterDirection()
	{
		amLocked = false;
		ActiveCG.alpha = 0f;
	}
}
