using UnityEngine;
using UnityEngine.UI;

public class ZeroDayProductObject : MonoBehaviour
{
	public delegate void VoidActions();

	public static bool isDiscountOn;

	public GameObject ProductTitle;

	public GameObject ProductPrice;

	public GameObject ProductDesc;

	public GameObject ProductIMG;

	public GameObject ProductBTN;

	public Color defaultPriceColor;

	public Color tooMuchPriceColor;

	private bool iAmBusy;

	private ZeroDayProductDefinition myProduct;

	public event VoidActions RefreshProducts;

	private void Awake()
	{
		if (base.transform.parent.parent.parent.parent.gameObject.name == "ZeroDayMarket")
		{
			myProduct.myProductObject = this;
		}
	}

	private void OnDestroy()
	{
		ProductBTN.GetComponent<ZeroDayProductBTNBehaviour>().BuyItem -= BuyItem;
		ProductBTN.GetComponent<ZeroDayProductBTNBehaviour>().InstallItem -= InstallItem;
		ProductBTN.GetComponent<ZeroDayProductBTNBehaviour>().CantBuy -= CantBuyItem;
	}

	public void BuildMe(ZeroDayProductDefinition myProduct)
	{
		this.myProduct = myProduct;
		ProductTitle.GetComponent<Text>().text = myProduct.productName;
		ProductPrice.GetComponent<Text>().text = myProduct.productPrice.ToString();
		ProductDesc.GetComponent<Text>().text = myProduct.productDesc;
		if (myProduct.productSprite != null)
		{
			ProductIMG.GetComponent<Image>().sprite = myProduct.productSprite;
			bool flag = Themes.selected <= THEME.TWR;
			Sprite sprite = (flag ? ThemesLookUp.WTTG1TWR.blueBGF : ThemesLookUp.WTTG2.orangeBGF);
			Object.Instantiate(ProductIMG, ProductIMG.transform.parent, worldPositionStays: true).GetComponent<Image>().sprite = sprite;
			Image component = ProductIMG.transform.parent.Find("BottomBorder").GetComponent<Image>();
			component.sprite = sprite;
			if (flag)
			{
				Color color = (component.color = new Color(0.326f, 0.706f, 1f, 1f));
				Image component2 = ProductIMG.transform.parent.Find("DOSCoinIcon").GetComponent<Image>();
				component2.sprite = SpriteLookUp.DOSCoin;
				component2.color = color;
			}
			ProductIMG.transform.SetAsLastSibling();
		}
		if (myProduct.productOwned)
		{
			ProductBTN.GetComponent<ZeroDayProductBTNBehaviour>().SetToOwned();
		}
		else
		{
			if (myProduct.productRequiresOtherProduct && !myProduct.productToOwn.productOwned)
			{
				ProductBTN.GetComponent<ZeroDayProductBTNBehaviour>().SetToDisabled();
			}
			ProductBTN.GetComponent<ZeroDayProductBTNBehaviour>().BuyItem += BuyItem;
			ProductBTN.GetComponent<ZeroDayProductBTNBehaviour>().InstallItem += InstallItem;
			ProductBTN.GetComponent<ZeroDayProductBTNBehaviour>().CantBuy += CantBuyItem;
		}
		if (myProduct.productName == "VWipe" && DifficultyManager.CasualMode)
		{
			ProductBTN.GetComponent<ZeroDayProductBTNBehaviour>().SetToDisabled();
		}
		if (myProduct.productIsInstalling)
		{
			InstallItem();
		}
	}

	public void RefreshMe(ZeroDayProductDefinition updatedProduct)
	{
		myProduct = updatedProduct;
		if (!iAmBusy)
		{
			ProductBTN.GetComponent<ZeroDayProductBTNBehaviour>().SetToBuy();
			if (myProduct.productOwned)
			{
				ProductBTN.GetComponent<ZeroDayProductBTNBehaviour>().SetToOwned();
			}
			else if (myProduct.productRequiresOtherProduct && !myProduct.productToOwn.productOwned)
			{
				ProductBTN.GetComponent<ZeroDayProductBTNBehaviour>().SetToDisabled();
			}
		}
	}

	private bool BuyItem()
	{
		return DOSCoinsCurrencyManager.PurchaseItem(myProduct);
	}

	private void CantBuyItem()
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.CantBuyItem);
		ProductPrice.GetComponent<Text>().color = tooMuchPriceColor;
		GameManager.TimeSlinger.FireTimer(3f, ResetPriceColor);
	}

	private void ResetPriceColor()
	{
		ProductPrice.GetComponent<Text>().color = defaultPriceColor;
	}

	private void InstallItem()
	{
		iAmBusy = true;
		myProduct.productIsInstalling = true;
		ProductBTN.GetComponent<ZeroDayProductBTNBehaviour>().InstallAni(myProduct.installTime);
		GameManager.ManagerSlinger.ProductsManager.MarkZeroDayProductAsInstalling(myProduct);
		GameManager.TimeSlinger.FireTimer(myProduct.installTime - 0.25f, GameManager.ManagerSlinger.ProductsManager.ActivateZeroDayProduct, myProduct);
		GameManager.TimeSlinger.FireTimer(myProduct.installTime - 0.25f, delegate
		{
			iAmBusy = false;
		});
		GameManager.TimeSlinger.FireTimer(myProduct.installTime, delegate
		{
			if (this.RefreshProducts != null)
			{
				this.RefreshProducts();
			}
		});
	}

	public void DiscountMe()
	{
		myProduct.productPrice *= 0.75f;
		ProductPrice.GetComponent<Text>().text = myProduct.productPrice.ToString();
		ProductPrice.GetComponent<Text>().color = Color.blue;
		defaultPriceColor = Color.blue;
		myProduct.isDiscounted = true;
		isDiscountOn = true;
	}

	public void UnDiscountMe()
	{
		myProduct.productPrice /= 0.75f;
		ProductPrice.GetComponent<Text>().text = myProduct.productPrice.ToString();
		ProductPrice.GetComponent<Text>().color = Color.black;
		defaultPriceColor = Color.black;
		myProduct.isDiscounted = false;
		isDiscountOn = false;
	}
}
