using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShadowMarketProductsBTNBehaviour : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public delegate bool BoolActions();

	public delegate void VoidActions();

	[SerializeField]
	private Text btnText;

	[SerializeField]
	private RectTransform shippingIMGRT;

	[SerializeField]
	private Sprite defaultSprite;

	[SerializeField]
	private Sprite hoverSprite;

	[SerializeField]
	private Sprite disabledSprite;

	private bool isDisabled;

	private Image myImage;

	private RectTransform myRT;

	public event BoolActions BuyItem;

	public event VoidActions ShipItem;

	public event VoidActions CantBuy;

	private void Awake()
	{
		myImage = GetComponent<Image>();
		myRT = GetComponent<RectTransform>();
	}

	private void Start()
	{
		if (Themes.selected <= THEME.TWR)
		{
			defaultSprite = ThemesLookUp.WTTG1TWR.blueBG;
			if (!isDisabled)
			{
				GetComponent<Image>().sprite = defaultSprite;
			}
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (GameManager.PauseManager.Paused)
		{
			return;
		}
		if (!isDisabled && this.BuyItem != null)
		{
			if (this.BuyItem())
			{
				if (this.ShipItem != null)
				{
					this.ShipItem();
				}
			}
			else
			{
				if (this.CantBuy != null)
				{
					this.CantBuy();
				}
				SetToDisabled();
				GameManager.TimeSlinger.FireTimer(3f, SetToBuy);
			}
		}
		GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: false);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!isDisabled)
		{
			GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: true);
			myImage.sprite = hoverSprite;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!isDisabled)
		{
			GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: false);
			myImage.sprite = defaultSprite;
		}
	}

	public void SetDefaults()
	{
		myImage = GetComponent<Image>();
		myRT = GetComponent<RectTransform>();
	}

	public void SetToBuy()
	{
		isDisabled = false;
		myImage.sprite = defaultSprite;
		btnText.text = "BUY";
		btnText.fontStyle = FontStyle.Normal;
		btnText.fontSize = 18;
	}

	public void SetToOwned()
	{
		isDisabled = true;
		myImage.sprite = disabledSprite;
		btnText.text = "OWNED";
		btnText.fontStyle = FontStyle.Italic;
		btnText.fontSize = 18;
	}

	public void SetToDisabled()
	{
		isDisabled = true;
		myImage.sprite = disabledSprite;
		btnText.fontStyle = FontStyle.Italic;
		btnText.fontSize = 18;
	}

	public void SetToShipped()
	{
		isDisabled = true;
		myImage.sprite = disabledSprite;
		btnText.text = "SHIPPED!";
		btnText.fontStyle = FontStyle.Bold;
		btnText.fontSize = 14;
	}

	public void ShipAni(float setTime)
	{
		isDisabled = true;
		myImage.sprite = disabledSprite;
		btnText.text = "Shipping...";
		btnText.fontStyle = FontStyle.Italic;
		btnText.fontSize = 14;
		Vector2 shipIMGSize = new Vector2(myRT.sizeDelta.x, 0f);
		Sequence sequence = DOTween.Sequence().OnComplete(delegate
		{
			shipIMGSize.x = 0f;
			shipIMGSize.y = shippingIMGRT.sizeDelta.y;
			shippingIMGRT.sizeDelta = shipIMGSize;
		});
		sequence.Insert(0f, DOTween.To(() => shippingIMGRT.sizeDelta, delegate(Vector2 x)
		{
			shippingIMGRT.sizeDelta = x;
		}, shipIMGSize, setTime).SetEase(Ease.Linear).SetRelative(isRelative: true));
		sequence.Play();
	}
}
