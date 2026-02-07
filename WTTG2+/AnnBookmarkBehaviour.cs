using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnnBookmarkBehaviour : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public Sprite bookmarkedSprite;

	private Sprite defaultSprite;

	private void Awake()
	{
		defaultSprite = GetComponent<Image>().sprite;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		setBookmarked(GameManager.TheCloud.TriggerBookMark());
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: false);
	}

	public void setBookmarked(bool nowBookmarked)
	{
		if (nowBookmarked)
		{
			GetComponent<Image>().sprite = bookmarkedSprite;
		}
		else
		{
			GetComponent<Image>().sprite = defaultSprite;
		}
	}
}
