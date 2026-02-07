using UnityEngine;
using UnityEngine.EventSystems;

public class ResizeWindowBehaviour : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
	public SOFTWARE_PRODUCTS MyProduct;

	public SoftwareProductDefinition MyProductData;

	private Vector3 bufferPOS;

	private RectTransform dragPlane;

	private GameObject myWindow;

	private Vector2 startingSize;

	public void Start()
	{
		if (MyProduct == SOFTWARE_PRODUCTS.UNIVERSAL)
		{
			myWindow = WindowManager.Get(MyProductData).Window;
		}
		else
		{
			myWindow = WindowManager.Get(MyProduct).Window;
		}
		dragPlane = LookUp.DesktopUI.DRAG_PLANE;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		myWindow.gameObject.GetComponent<BringWindowToFrontBehaviour>().forceTap();
		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(dragPlane, eventData.position, eventData.pressEventCamera, out var worldPoint))
		{
			startingSize = myWindow.GetComponent<RectTransform>().sizeDelta;
			bufferPOS.x = worldPoint.x;
			bufferPOS.y = worldPoint.y;
		}
		SetDragPos(eventData);
	}

	public void OnDrag(PointerEventData eventData)
	{
		SetDragPos(eventData);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		GameManager.ManagerSlinger.AppManager.ResizedApp(MyProduct);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		GameManager.ManagerSlinger.CursorManager.ResizeCursorState(active: true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		GameManager.ManagerSlinger.CursorManager.ResizeCursorState(active: false);
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
		if (!flag && RectTransformUtility.ScreenPointToWorldPointInRectangle(dragPlane, data.position, data.pressEventCamera, out var worldPoint))
		{
			worldPoint.x = startingSize.x + (worldPoint.x - bufferPOS.x);
			worldPoint.y = startingSize.y - (worldPoint.y - bufferPOS.y);
			worldPoint.x = Mathf.Max(worldPoint.x, 335f);
			worldPoint.y = Mathf.Max(worldPoint.y, 300f);
			worldPoint.x = Mathf.Round(worldPoint.x);
			worldPoint.y = Mathf.Round(worldPoint.y);
			myWindow.GetComponent<RectTransform>().sizeDelta = new Vector2(worldPoint.x, worldPoint.y);
		}
	}
}
