using UnityEngine;
using UnityEngine.EventSystems;

public class MouseHoverPointerScrub : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public void OnPointerEnter(PointerEventData eventData)
	{
		GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: false);
	}
}
