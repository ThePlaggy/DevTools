using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VWipeButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	[SerializeField]
	private Image bgImage;

	[SerializeField]
	private Sprite hoverSprite;

	[SerializeField]
	private Sprite clickSprite;

	public TMP_Text text;

	public TMP_Text priceText;

	public CanvasGroup CG;

	public CustomEvent ClickAction = new CustomEvent(2);

	private Sprite defaultSprite;

	private bool hardLock;

	private bool locked;

	private void Awake()
	{
		defaultSprite = bgImage.sprite;
		priceText.fontSize = 36f;
		CG = base.gameObject.AddComponent<CanvasGroup>();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!hardLock && !locked)
		{
			locked = true;
			bgImage.sprite = clickSprite;
			GameManager.TimeSlinger.FireTimer(0.25f, (Action)delegate
			{
				locked = false;
			}, 1);
			ClickAction.Execute();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!hardLock)
		{
			GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: true);
			if (!locked)
			{
				bgImage.sprite = hoverSprite;
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!hardLock)
		{
			GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: false);
			bgImage.sprite = defaultSprite;
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (!hardLock)
		{
			bgImage.sprite = hoverSprite;
		}
	}

	public void SetLock(bool value)
	{
		locked = value;
	}

	public void SetHardLock(bool value)
	{
		hardLock = value;
		bgImage.sprite = (value ? clickSprite : defaultSprite);
	}
}
