using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(GraphicRaycaster))]
public class TitleMenuBTN : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public TextMeshProUGUI titleText;

	[SerializeField]
	private Color hoverColor;

	[SerializeField]
	private bool isPauseMenu;

	private Color defaultColor;

	public CustomEvent MyAction = new CustomEvent();

	private CanvasGroup myCG;

	private void Awake()
	{
		myCG = GetComponent<CanvasGroup>();
		defaultColor = titleText.color;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (isPauseMenu)
		{
			GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.MenuClick);
		}
		else
		{
			GameManager.AudioSlinger.PlaySound(TitleLookUp.Ins.TitleMenuClickSFX);
		}
		MyAction.Execute();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (isPauseMenu)
		{
			GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.MenuHover);
		}
		else
		{
			GameManager.AudioSlinger.PlaySound(TitleLookUp.Ins.TitleMenuHoverSFX);
		}
		titleText.color = hoverColor;
		if (CursorManager.Ins != null)
		{
			CursorManager.Ins.PointerCursorState(active: true);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		titleText.color = defaultColor;
		if (CursorManager.Ins != null)
		{
			CursorManager.Ins.PointerCursorState(active: false);
		}
	}

	public void ActiveState(bool Active)
	{
		myCG.alpha = ((!Active) ? 0.15f : 1f);
		myCG.interactable = Active;
		myCG.blocksRaycasts = Active;
	}
}
