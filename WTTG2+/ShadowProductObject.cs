using UnityEngine;
using UnityEngine.UI;

public class ShadowProductObject : MonoBehaviour
{
	public static bool isDiscountOn;

	[SerializeField]
	private Text productTitle;

	[SerializeField]
	private Text productPrice;

	[SerializeField]
	private Text productDesc;

	[SerializeField]
	private Text stockValue;

	[SerializeField]
	private Image productIMG;

	[SerializeField]
	private ShadowMarketProductsBTNBehaviour productsBTN;

	[SerializeField]
	private Color defaultPriceColor;

	[SerializeField]
	private Color tooMuchPriceColor;

	public ShadowMarketProductDefinition myProduct;

	public bool iAmBusy;

	public int boughtCount { get; private set; }

	private void Awake()
	{
		myProduct.myProductObject = this;
	}

	private void Start()
	{
		boughtCount = 0;
	}

	private void OnDestroy()
	{
		productsBTN.BuyItem -= buyItem;
		productsBTN.CantBuy -= cantBuyItem;
		productsBTN.ShipItem -= shipItem;
	}

	public void RefreshMe()
	{
		if (iAmBusy)
		{
			return;
		}
		stockValue.text = "Stock: " + (myProduct.productMaxPurchaseAmount - boughtCount);
		if (myProduct.productOwned)
		{
			productsBTN.SetToOwned();
			return;
		}
		productsBTN.SetToBuy();
		if (myProduct.productIsShipped)
		{
			productsBTN.SetToShipped();
		}
		if (myProduct.productRequiresOtherProduct && myProduct.productToOwn.myProductObject.boughtCount < myProduct.productToOwn.productMaxPurchaseAmount)
		{
			productsBTN.SetToDisabled();
		}
		if (myProduct.productName == "Tarot Cards" && myProduct.productMaxPurchaseAmount == 0)
		{
			productsBTN.SetToDisabled();
		}
	}

	public void BuildMe(ShadowMarketProductDefinition SetProduct)
	{
		myProduct = SetProduct;
		productTitle.text = myProduct.productName;
		productPrice.text = myProduct.productPrice.ToString();
		productDesc.text = myProduct.productDesc;
		stockValue.text = "Stock: " + (myProduct.productMaxPurchaseAmount - boughtCount);
		productsBTN.SetDefaults();
		if (myProduct.productSprite != null)
		{
			productIMG.sprite = myProduct.productSprite;
			bool flag = Themes.selected <= THEME.TWR;
			Sprite sprite = (flag ? ThemesLookUp.WTTG1TWR.blueBGF : ThemesLookUp.WTTG2.orangeBGF);
			Object.Instantiate(productIMG, productIMG.transform.parent, worldPositionStays: true).sprite = sprite;
			Image component = productIMG.transform.parent.Find("BottomBorder").GetComponent<Image>();
			component.sprite = sprite;
			if (flag)
			{
				Color color = (component.color = new Color(0.326f, 0.706f, 1f, 1f));
				Image component2 = productIMG.transform.parent.Find("DOSCoinIcon").GetComponent<Image>();
				component2.sprite = SpriteLookUp.DOSCoin;
				component2.color = color;
			}
			productIMG.transform.SetAsLastSibling();
		}
		if (myProduct.productOwned)
		{
			productsBTN.SetToOwned();
			return;
		}
		if (myProduct.productIsPending)
		{
			shipItem();
		}
		else if (myProduct.productIsShipped)
		{
			productsBTN.SetToShipped();
		}
		if (myProduct.productRequiresOtherProduct && !myProduct.productToOwn.productOwned)
		{
			productsBTN.SetToDisabled();
		}
		if (myProduct.productName == "Tarot Cards" && myProduct.productMaxPurchaseAmount == 0)
		{
			productsBTN.SetToDisabled();
		}
		productsBTN.BuyItem += buyItem;
		productsBTN.CantBuy += cantBuyItem;
		productsBTN.ShipItem += shipItem;
	}

	private bool buyItem()
	{
		return DOSCoinsCurrencyManager.PurchaseItem(myProduct);
	}

	private void cantBuyItem()
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.CantBuyItem);
		productPrice.color = tooMuchPriceColor;
		GameManager.TimeSlinger.FireTimer(3f, delegate
		{
			productPrice.color = defaultPriceColor;
		});
	}

	public void shipItem()
	{
		float num = Random.Range(myProduct.deliveryTimeMin, myProduct.deliveryTimeMax);
		iAmBusy = true;
		myProduct.productIsPending = true;
		productsBTN.ShipAni(num);
		GameManager.ManagerSlinger.ProductsManager.MarkShadowProductAsPending(myProduct);
		GameManager.TimeSlinger.FireTimer(num, delegate
		{
			iAmBusy = false;
			myProduct.productIsPending = false;
			myProduct.productIsShipped = true;
			boughtCount++;
			if (myProduct.productHasLimitPurchases && boughtCount >= myProduct.productMaxPurchaseAmount)
			{
				productsBTN.SetToShipped();
				stockValue.text = "Stock: 0";
				if (myProduct.productIsRequiredByAnotherProduct)
				{
					myProduct.productDepended.myProductObject.productsBTN.SetToBuy();
				}
			}
			else
			{
				productsBTN.SetToBuy();
				stockValue.text = "Stock: " + (myProduct.productMaxPurchaseAmount - boughtCount);
			}
			GameManager.ManagerSlinger.ProductsManager.ShipProduct(myProduct);
		});
	}

	public void DiscountMe()
	{
		myProduct.productPrice *= 0.75f;
		productPrice.GetComponent<Text>().text = myProduct.productPrice.ToString();
		productPrice.GetComponent<Text>().color = Color.blue;
		defaultPriceColor = Color.blue;
		myProduct.isDiscounted = true;
		isDiscountOn = true;
	}

	public void UnDiscountMe()
	{
		myProduct.productPrice /= 0.75f;
		productPrice.GetComponent<Text>().text = myProduct.productPrice.ToString();
		productPrice.GetComponent<Text>().color = Color.black;
		defaultPriceColor = Color.black;
		myProduct.isDiscounted = false;
		isDiscountOn = false;
	}
}
