using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextDocIconObject : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public static bool OpeningPNG;

	[SerializeField]
	private Image defaultIMG;

	[SerializeField]
	private Image hoverIMG;

	[SerializeField]
	private Text title1;

	[SerializeField]
	private Text title2;

	private Vector3 bufferPos;

	private int clickCount;

	private RectTransform dragPlane;

	private CanvasGroup myCG;

	private TextDocIconData myData;

	private GraphicRaycaster myRayCaster;

	private RectTransform myRT;

	private float myTimeStamp;

	public CustomEvent<TextDocIconData> OpenEvents = new CustomEvent<TextDocIconData>();

	public CustomEvent UpdateMyData = new CustomEvent();

	private void Update()
	{
		if (Time.time - myTimeStamp >= 1f)
		{
			clickCount = 0;
			myTimeStamp = Time.time;
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(dragPlane, eventData.position, eventData.pressEventCamera, out var worldPoint))
		{
			bufferPos = worldPoint - myRT.position;
		}
		setDragPOS(eventData);
	}

	public void OnDrag(PointerEventData eventData)
	{
		setDragPOS(eventData);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		UpdateMyData.Execute();
	}

	public void DeleteMe()
	{
		myCG.gameObject.SetActive(value: false);
		myCG.alpha = 0f;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (BinBehaviour.BinMode)
		{
			myCG.gameObject.SetActive(value: false);
			myCG.alpha = 0f;
			return;
		}
		clickCount++;
		if (clickCount < 2)
		{
			return;
		}
		if (myData.TextDocText.StartsWith("|[.vpn]|"))
		{
			GameManager.HackerManager.ForceTwitchHack();
			Object.Destroy(base.gameObject);
			return;
		}
		if (myData.TextDocText.StartsWith("|[.png]|"))
		{
			OpeningPNG = true;
			TextDocObject.CurrentPNG = int.Parse(myData.TextDocText.Substring(8));
		}
		else
		{
			OpeningPNG = false;
		}
		OpenEvents.Execute(myData);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		hoverIMG.enabled = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		hoverIMG.enabled = false;
	}

	public void SoftBuild()
	{
		myRayCaster = GetComponent<GraphicRaycaster>();
		myCG = GetComponent<CanvasGroup>();
		myRT = GetComponent<RectTransform>();
		dragPlane = LookUp.DesktopUI.DRAG_PLANE;
		myRT.anchoredPosition = new Vector2(-100f, 0f);
		myCG.alpha = 0f;
	}

	public void Build(TextDocIconData SetData)
	{
		myData = SetData;
		title1.text = myData.TextDocName;
		title2.text = myData.TextDocName;
		myRT.anchoredPosition = myData.TextDocPOS.ToVector2;
		myCG.alpha = 1f;
	}

	private void setDragPOS(PointerEventData data)
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
			worldPoint -= bufferPos;
			myRT.position = worldPoint;
			myData.TextDocPOS = new Vect2(worldPoint.x, worldPoint.y);
		}
	}

	public void SetDevToolsIcon(Sprite icon)
	{
		defaultIMG.sprite = icon;
		hoverIMG.sprite = icon;
	}

	public void SetPNGIcon()
	{
		defaultIMG.sprite = CustomSpriteLookUp.dicidle;
		hoverIMG.sprite = CustomSpriteLookUp.dicoutline;
	}
}
