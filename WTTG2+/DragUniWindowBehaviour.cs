using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragUniWindowBehaviour : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler
{
	public Image parentWindow;

	public bool FactorInScreenSize;

	private Vector3 bufferPos;

	private RectTransform dragPlane;

	private Vector2 setPOS;

	private Vector3 tempPOS;

	private void Awake()
	{
		dragPlane = LookUp.DesktopUI.DRAG_PLANE;
		base.gameObject.AddComponent<MouseClickSoundScrub>();
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (!GameManager.PauseManager.Paused)
		{
			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(dragPlane, eventData.position, eventData.pressEventCamera, out tempPOS))
			{
				bufferPos = tempPOS - parentWindow.rectTransform.position;
			}
			SetDragPos(eventData);
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (!GameManager.PauseManager.Paused)
		{
			SetDragPos(eventData);
		}
	}

	private void SetDragPos(PointerEventData data)
	{
		bool flag = false;
		if (Input.mousePosition.x < MagicSlinger.GetScreenWidthPXByPerc(0.01f))
		{
			flag = true;
		}
		else if (Input.mousePosition.x > MagicSlinger.GetScreenWidthPXByPerc(0.98f))
		{
			flag = true;
		}
		else if (Input.mousePosition.y < MagicSlinger.GetScreenHeightPXByPerc(0.05f))
		{
			flag = true;
		}
		else if (Input.mousePosition.y > MagicSlinger.GetScreenHeightPXByPerc(0.95f))
		{
			flag = true;
		}
		if (!flag && RectTransformUtility.ScreenPointToWorldPointInRectangle(dragPlane, data.position, data.pressEventCamera, out tempPOS))
		{
			tempPOS -= bufferPos;
			if (FactorInScreenSize)
			{
				setPOS.x = Mathf.Round(tempPOS.x - (float)Screen.width / 2f);
				setPOS.y = Mathf.Round(tempPOS.y + (float)Screen.height / 2f);
			}
			else
			{
				setPOS.x = Mathf.Round(tempPOS.x);
				setPOS.y = Mathf.Round(tempPOS.y);
			}
			parentWindow.rectTransform.anchoredPosition = setPOS;
		}
	}
}
