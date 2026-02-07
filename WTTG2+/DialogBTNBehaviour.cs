using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogBTNBehaviour : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public delegate void MyActions();

	public Sprite hoverSprite;

	private Sprite defaultSprite;

	public event MyActions OnPress;

	public void Start()
	{
		defaultSprite = GetComponent<Image>().sprite;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (this.OnPress != null)
		{
			GetComponent<Image>().sprite = defaultSprite;
			this.OnPress();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		GetComponent<Image>().sprite = hoverSprite;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		GetComponent<Image>().sprite = defaultSprite;
	}
}
