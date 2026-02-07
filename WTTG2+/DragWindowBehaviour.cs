using UnityEngine;
using UnityEngine.EventSystems;

public class DragWindowBehaviour : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IEndDragHandler, IDragHandler
{
	public SOFTWARE_PRODUCTS MyProduct;

	public SoftwareProductDefinition MyProductData;

	private Vector3 bufferPos;

	private RectTransform dragPlane;

	private WindowBehaviour myWindowBeh;

	private Vector2 setPOS;

	private Vector3 tempPOS;

	private RectTransform windowRT;

	private void Awake()
	{
		if (MyProduct == SOFTWARE_PRODUCTS.UNIVERSAL)
		{
			myWindowBeh = WindowManager.Get(MyProductData);
		}
		else
		{
			myWindowBeh = WindowManager.Get(MyProduct);
		}
		windowRT = myWindowBeh.Window.GetComponent<RectTransform>();
		dragPlane = LookUp.DesktopUI.DRAG_PLANE;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(dragPlane, eventData.position, eventData.pressEventCamera, out tempPOS))
		{
			bufferPos = tempPOS - windowRT.position;
		}
		SetDragPos(eventData);
	}

	public void OnDrag(PointerEventData eventData)
	{
		SetDragPos(eventData);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		myWindowBeh.MoveMe(windowRT.anchoredPosition);
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
			setPOS.x = Mathf.Round(tempPOS.x);
			setPOS.y = Mathf.Round(tempPOS.y);
			windowRT.anchoredPosition = setPOS;
		}
	}

	public void ReInstantiate()
	{
		myWindowBeh = WindowManager.Get(MyProduct);
		windowRT = myWindowBeh.Window.GetComponent<RectTransform>();
	}
}
