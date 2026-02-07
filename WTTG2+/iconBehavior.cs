using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(GraphicRaycaster))]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class iconBehavior : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler
{
	public Image DefaultIMG;

	public Image HoverIMG;

	public RectTransform DragPlane;

	public SOFTWARE_PRODUCTS MyProduct;

	public SoftwareProductDefinition UniProductData;

	private Vector3 bufferPos;

	private int clickCount;

	private CanvasGroup myCG;

	private IconData myData;

	private int myID;

	private GraphicRaycaster myRayCaster;

	private RectTransform myRT;

	private float myTimeStamp;

	private void Awake()
	{
		if (MyProduct == SOFTWARE_PRODUCTS.UNIVERSAL)
		{
			myID = UniProductData.GetHashCode();
		}
		else
		{
			myID = (int)MyProduct;
		}
		myRayCaster = GetComponent<GraphicRaycaster>();
		myCG = GetComponent<CanvasGroup>();
		myRT = GetComponent<RectTransform>();
		GameManager.StageManager.Stage += stageMe;
	}

	private void Start()
	{
		clickCount = 0;
		if (Themes.selected == THEME.TWR)
		{
			SOFTWARE_PRODUCTS myProduct = MyProduct;
			SOFTWARE_PRODUCTS sOFTWARE_PRODUCTS = myProduct;
			if (sOFTWARE_PRODUCTS == SOFTWARE_PRODUCTS.SKYBREAK)
			{
				DefaultIMG.sprite = ThemesLookUp.WTTG1TWR.terminal;
				HoverIMG.sprite = ThemesLookUp.WTTG1TWR.terminalActive;
			}
		}
		if (Themes.selected == THEME.WTTG2BETA)
		{
			switch (MyProduct)
			{
			case SOFTWARE_PRODUCTS.SKYBREAK:
				DefaultIMG.sprite = ThemesLookUp.WTTG2Beta.skybreak;
				HoverIMG.sprite = ThemesLookUp.WTTG2Beta.skybreak;
				DefaultIMG.rectTransform.sizeDelta = new Vector2(80f, 80f);
				HoverIMG.rectTransform.sizeDelta = new Vector2(80f, 80f);
				break;
			case SOFTWARE_PRODUCTS.ZERODAY:
				DefaultIMG.sprite = ThemesLookUp.WTTG2Beta.zeroDay;
				HoverIMG.sprite = ThemesLookUp.WTTG2Beta.zeroDay;
				DefaultIMG.rectTransform.sizeDelta = new Vector2(80f, 80f);
				HoverIMG.rectTransform.sizeDelta = new Vector2(80f, 80f);
				break;
			case SOFTWARE_PRODUCTS.SHADOW_MARKET:
				DefaultIMG.sprite = ThemesLookUp.WTTG2Beta.shadowMarket;
				HoverIMG.sprite = ThemesLookUp.WTTG2Beta.shadowMarket;
				DefaultIMG.rectTransform.sizeDelta = new Vector2(80f, 80f);
				HoverIMG.rectTransform.sizeDelta = new Vector2(80f, 80f);
				break;
			case SOFTWARE_PRODUCTS.VWIPE:
			case SOFTWARE_PRODUCTS.CAMHOOK:
			case SOFTWARE_PRODUCTS.BOTNET:
			case SOFTWARE_PRODUCTS.DOORLOG:
				DefaultIMG.rectTransform.sizeDelta = new Vector2(80f, 80f);
				HoverIMG.rectTransform.sizeDelta = new Vector2(80f, 80f);
				break;
			}
		}
	}

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
		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(DragPlane, eventData.position, eventData.pressEventCamera, out var worldPoint))
		{
			bufferPos = worldPoint - myRT.position;
		}
		SetDragPos(eventData);
	}

	public void OnDrag(PointerEventData eventData)
	{
		SetDragPos(eventData);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (BinBehaviour.BinMode && (MyProduct == SOFTWARE_PRODUCTS.TEXT_DOC || MyProduct == SOFTWARE_PRODUCTS.UNIVERSAL || MyProduct == SOFTWARE_PRODUCTS.ROUTERDOC))
		{
			myCG.gameObject.SetActive(value: false);
			myCG.alpha = 0f;
			return;
		}
		clickCount++;
		if (clickCount >= 2)
		{
			switch (MyProduct)
			{
			case SOFTWARE_PRODUCTS.VWIPE:
				UIDialogManager.VWipeDialog.Present();
				break;
			case SOFTWARE_PRODUCTS.UNIVERSAL:
				GameManager.ManagerSlinger.AppManager.LaunchApp(UniProductData);
				break;
			default:
				GameManager.ManagerSlinger.AppManager.LaunchApp(MyProduct);
				break;
			}
			clickCount = 0;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		HoverIMG.enabled = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		HoverIMG.enabled = false;
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
		if (!flag && RectTransformUtility.ScreenPointToWorldPointInRectangle(DragPlane, data.position, data.pressEventCamera, out var worldPoint))
		{
			worldPoint -= bufferPos;
			myRT.position = worldPoint;
			if (myData != null)
			{
				myData.MyPOS = Vect2.Convert(myRT.anchoredPosition);
				DataManager.Save(myData);
			}
		}
	}

	public void ActivateMe()
	{
		myCG.alpha = 1f;
		myRayCaster.enabled = true;
	}

	private void stageMe()
	{
		myData = DataManager.Load<IconData>(myID);
		if (myData == null)
		{
			myData = new IconData(myID);
			myData.MyPOS = Vect2.Convert(myRT.anchoredPosition);
		}
		myRT.anchoredPosition = myData.MyPOS.ToVector2;
		GameManager.StageManager.Stage -= stageMe;
	}
}
