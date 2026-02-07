using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(GraphicRaycaster))]
public class OptionsMenuBTN : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public TextMeshProUGUI titleText;

	[SerializeField]
	private Color activeDefaultColor;

	[SerializeField]
	private Color activeHoverColor;

	[SerializeField]
	private Color inActiveDefaultColor;

	[SerializeField]
	private Color inActiveHoverColor;

	[SerializeField]
	public UnityEvent ClickAction;

	[SerializeField]
	private UnityEvent HoverAction;

	[SerializeField]
	private UnityEvent ExitAction;

	public bool Active { get; private set; }

	private void Awake()
	{
		Clear();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		GameManager.AudioSlinger.PlaySound(TitleLookUp.Ins.TitleMenuClickSFX);
		SetActive();
		ClickAction.Invoke();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		GameManager.AudioSlinger.PlaySound(TitleLookUp.Ins.TitleMenuHoverSFX);
		CursorManager.Ins.PointerCursorState(active: true);
		if (Active)
		{
			titleText.color = activeHoverColor;
		}
		else
		{
			titleText.color = inActiveHoverColor;
		}
		if (base.gameObject.name != "MicOffButton")
		{
			HoverAction.Invoke();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		CursorManager.Ins.PointerCursorState(active: false);
		if (Active)
		{
			titleText.color = activeDefaultColor;
		}
		else
		{
			titleText.color = inActiveDefaultColor;
		}
		if (base.gameObject.name != "MicOffButton")
		{
			ExitAction.Invoke();
		}
	}

	public void Clear()
	{
		titleText.color = inActiveDefaultColor;
		Active = false;
	}

	public void SetActive()
	{
		Active = true;
		titleText.color = activeDefaultColor;
	}
}
