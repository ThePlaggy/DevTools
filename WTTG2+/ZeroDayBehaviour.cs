using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZeroDayBehaviour : WindowBehaviour
{
	private List<GameObject> currentProducts = new List<GameObject>();

	private bool isMin;

	private List<ZeroDayProductDefinition> myProducts;

	private GameObject offLineHolder;

	private bool productIsInstalling;

	private bool productsAreBuilt;

	private RectTransform productsContentHolder;

	private Vector2 productsContentHolderSize = Vector2.zero;

	private GameObject productsHolder;

	private GameObject zeroDayProductObject;

	protected new void Awake()
	{
		if (!DifficultyManager.HackerMode)
		{
			base.Awake();
			productsHolder = LookUp.DesktopUI.ZERO_DAY_PRODUCTS_HOLDER;
			productsContentHolder = LookUp.DesktopUI.ZERO_DAY_PRODUCTS_CONTENT_HOLDER;
			zeroDayProductObject = LookUp.DesktopUI.ZERO_DAY_PRODUCT_OBJECT;
			offLineHolder = LookUp.DesktopUI.ZERO_DAY_OFF_LINE_HOLDER;
			myProducts = GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts;
			GameManager.ManagerSlinger.WifiManager.WentOffline += setMeOffline;
			GameManager.ManagerSlinger.WifiManager.WentOnline += setMeOnline;
			if (Themes.selected <= THEME.TWR)
			{
				productsHolder.GetComponent<Image>().color = new Color(0.326f, 0.706f, 1f, 1f);
				Scrollbar component = productsHolder.transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>();
				ColorBlock colors = component.colors;
				colors.normalColor = new Color(0.326f, 0.706f, 1f);
				colors.highlightedColor = new Color(0.306f, 0.581f, 0.9f);
				colors.pressedColor = new Color(0.126f, 0.411f, 0.7f);
				colors.disabledColor = new Color(0f, 0.3f, 0.5f);
				component.colors = colors;
			}
		}
	}

	protected new void Start()
	{
		if (!DifficultyManager.HackerMode)
		{
			base.Start();
			MarketUtilities.ReplaceVWipeWithSpeed();
			MarketUtilities.AddBotnetApp(myProducts);
			MarketUtilities.AddDoorLogApp(myProducts);
			MarketUtilities.AddLocationServices(myProducts);
		}
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
	}

	public void RefreshProducts()
	{
		for (int i = 0; i < currentProducts.Count; i++)
		{
			currentProducts[i].GetComponent<ZeroDayProductObject>().RefreshMe(myProducts[i]);
		}
	}

	protected override void OnLaunch()
	{
		if (productIsInstalling)
		{
			productIsInstalling = false;
		}
		else if (!productsAreBuilt)
		{
			buildMyProducts();
		}
	}

	protected override void OnClose()
	{
		bool flag = false;
		for (int i = 0; i < myProducts.Count; i++)
		{
			if (myProducts[i].productIsInstalling)
			{
				flag = true;
				i = myProducts.Count;
			}
		}
		if (flag)
		{
			productIsInstalling = true;
		}
	}

	protected override void OnMin()
	{
	}

	protected override void OnUnMin()
	{
	}

	protected override void OnMax()
	{
	}

	protected override void OnUnMax()
	{
	}

	protected override void OnResized()
	{
	}

	private void buildMyProducts()
	{
		productsContentHolderSize.x = productsContentHolder.sizeDelta.x;
		productsContentHolderSize.y = (float)(myProducts.Count - 5) * 126f;
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < myProducts.Count; i++)
		{
			GameObject gameObject = Object.Instantiate(zeroDayProductObject, productsContentHolder.GetComponent<RectTransform>());
			gameObject.GetComponent<ZeroDayProductObject>().BuildMe(myProducts[i]);
			gameObject.GetComponent<ZeroDayProductObject>().RefreshProducts += RefreshProducts;
			zero.y = 0f - (float)myProducts[i].arrangeSlot * 126f;
			gameObject.GetComponent<RectTransform>().anchoredPosition = zero;
			currentProducts.Add(gameObject);
		}
		productsContentHolder.sizeDelta = productsContentHolderSize;
		productsAreBuilt = true;
	}

	private void setMeOffline()
	{
		offLineHolder.SetActive(value: true);
		productsHolder.SetActive(value: false);
	}

	private void setMeOnline()
	{
		offLineHolder.SetActive(value: false);
		productsHolder.SetActive(value: true);
	}
}
