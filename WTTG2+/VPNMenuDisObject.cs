using UnityEngine;
using UnityEngine.EventSystems;

public class VPNMenuDisObject : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public delegate void PressedActions();

	[SerializeField]
	private Color hoverColor;

	public event PressedActions IWasPressed;

	public void OnPointerDown(PointerEventData eventData)
	{
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
	}

	public void OnPointerExit(PointerEventData eventData)
	{
	}
}
