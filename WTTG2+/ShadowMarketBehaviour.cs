using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShadowMarketBehaviour : WindowBehaviour
{
	private List<ShadowProductObject> currentProducts = new List<ShadowProductObject>();

	private List<ShadowMarketProductDefinition> myProducts = new List<ShadowMarketProductDefinition>();

	private GameObject offLineHolder;

	private bool productsAreBuilt;

	private RectTransform productsContentHolder;

	private GameObject productsHolder;

	private GameObject shadowMarketProductObject;

	protected new void Awake()
	{
		if (!DifficultyManager.HackerMode)
		{
			base.Awake();
			productsHolder = LookUp.DesktopUI.SHADOW_MARKET_PRODUCTS_HOLDER;
			productsContentHolder = LookUp.DesktopUI.SHADOW_MARKET_PRODUCTS_CONTENT_HOLDER;
			shadowMarketProductObject = LookUp.DesktopUI.SHADOW_MARKET_PRODUCT_OBJECT;
			offLineHolder = LookUp.DesktopUI.SHADOW_MARKET_OFF_LINE_HOLDER;
			myProducts = GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts;
			GameManager.ManagerSlinger.WifiManager.WentOffline += setMeOffline;
			GameManager.ManagerSlinger.WifiManager.WentOnline += setMeOnline;
			GameManager.ManagerSlinger.ProductsManager.ShadowMarketProductWasActivated.Event += productWasPickedUp;
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
			MarketUtilities.addRemoteVPNS(myProducts);
			MarketUtilities.addSulphurToMarket(myProducts);
			MarketUtilities.addDWR921Router(myProducts);
			MarketUtilities.addTarotCards(myProducts);
			MarketUtilities.AddSecCams(myProducts);
			MarketUtilities.addKeypad(myProducts);
			MarketUtilities.addStrongFlashlight(myProducts);
		}
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
	}

	protected override void OnLaunch()
	{
		if (!productsAreBuilt)
		{
			buildMyProducts();
		}
	}

	protected override void OnClose()
	{
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

	private void productWasPickedUp(ShadowMarketProductDefinition TheProduct)
	{
		for (int i = 0; i < currentProducts.Count; i++)
		{
			currentProducts[i].RefreshMe();
		}
	}

	private void buildMyProducts()
	{
		Vector2 sizeDelta = new Vector2(productsContentHolder.sizeDelta.x, (float)(myProducts.Count - 1) * 126f);
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < myProducts.Count; i++)
		{
			ShadowProductObject component = Object.Instantiate(shadowMarketProductObject, productsContentHolder).GetComponent<ShadowProductObject>();
			component.BuildMe(myProducts[i]);
			zero.y = 0f - (float)myProducts[i].arrangeSlot * 126f;
			component.gameObject.GetComponent<RectTransform>().anchoredPosition = zero;
			currentProducts.Add(component);
		}
		productsContentHolder.sizeDelta = sizeDelta;
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
