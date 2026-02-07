using System.Collections.Generic;
using UnityEngine;

public class ProductsManager : MonoBehaviour
{
	public static bool ownsWhitehatScanner;

	public static bool ownsWhitehatDongle2;

	public static bool ownsWhitehatDongle3;

	public static bool ownsWhitehatRemoteVPN2;

	public static bool ownsWhitehatRemoteVPN3;

	public static bool ownsWhitehatRouter;

	public static bool ownsSecCams;

	[SerializeField]
	private int SHIPPED_PRODUCT_POOL_COUNT = 5;

	[SerializeField]
	private Transform shippedProductsParent;

	[SerializeField]
	private GameObject shippedProductObject;

	[SerializeField]
	private Vector3 shippedProductPOS;

	[SerializeField]
	private Vector3 shippedProductROT;

	[SerializeField]
	private List<ZeroDayProductDefinition> zeroDayProducts;

	[SerializeField]
	private List<ShadowMarketProductDefinition> shadowMarketProducts;

	private List<ShippedProductObject> currentShippedProducts = new List<ShippedProductObject>(10);

	public CustomEvent ProductWasPickedUp = new CustomEvent(2);

	public CustomEvent<ShadowMarketProductDefinition> ShadowMarketProductWasActivated = new CustomEvent<ShadowMarketProductDefinition>(10);

	private PooledStack<ShippedProductObject> shippedProductPool;

	public List<ZeroDayProductDefinition> ZeroDayProducts => zeroDayProducts;

	public List<ShadowMarketProductDefinition> ShadowMarketProducts => shadowMarketProducts;

	public Transform ShippedProductParent => shippedProductsParent;

	private void Awake()
	{
		GameManager.ManagerSlinger.ProductsManager = this;
		shippedProductPool = new PooledStack<ShippedProductObject>(delegate
		{
			ShippedProductObject component = Object.Instantiate(shippedProductObject, shippedProductsParent).GetComponent<ShippedProductObject>();
			component.SoftBuild();
			return component;
		}, SHIPPED_PRODUCT_POOL_COUNT);
		for (int num = 0; num < zeroDayProducts.Count; num++)
		{
			SteamSlinger.Ins.AddZeroDayProduct(zeroDayProducts[num].GetHashCode());
		}
		for (int num2 = 0; num2 < shadowMarketProducts.Count; num2++)
		{
			SteamSlinger.Ins.AddShadowMarketProduct(shadowMarketProducts[num2].GetHashCode());
		}
		GameManager.StageManager.Stage += stageMe;
	}

	public void ActivateZeroDayProduct(ZeroDayProductDefinition TheProduct)
	{
		if (SteamSlinger.Ins != null)
		{
			SteamSlinger.Ins.AddPurchasedProduct();
		}
		ZeroDayProductData zeroDayProductData = DataManager.Load<ZeroDayProductData>((int)TheProduct.productID);
		TheProduct.productIsInstalling = false;
		if (!TheProduct.unlimtedUse)
		{
			TheProduct.productOwned = true;
			if (zeroDayProductData != null)
			{
				zeroDayProductData.Owned = true;
			}
		}
		else
		{
			TheProduct.productInventory++;
			if (zeroDayProductData != null)
			{
				zeroDayProductData.InventoryCount++;
			}
		}
		if (zeroDayProductData != null)
		{
			zeroDayProductData.Installing = false;
			DataManager.Save(zeroDayProductData);
		}
		SteamSlinger.Ins.ActivateZeroDayProduct(TheProduct.GetHashCode());
		GameManager.ManagerSlinger.AppManager.ActivateApp(TheProduct);
	}

	public void ActivateShadowMarketProduct(ShadowMarketProductDefinition ProductToActivate)
	{
		if (SteamSlinger.Ins != null)
		{
			SteamSlinger.Ins.AddPurchasedProduct();
		}
		ShadowMarketProductData shadowMarketProductData = DataManager.Load<ShadowMarketProductData>((int)ProductToActivate.productID);
		ProductToActivate.productCurrentInventoryCount++;
		ProductToActivate.productIsShipped = false;
		ProductToActivate.productIsPending = false;
		if (ProductToActivate.productHasLimitPurchases && ProductToActivate.myProductObject.boughtCount >= ProductToActivate.productMaxPurchaseAmount)
		{
			ProductToActivate.productOwned = true;
		}
		if (shadowMarketProductData != null)
		{
			shadowMarketProductData.Owned = ProductToActivate.productOwned;
			shadowMarketProductData.InventoryCount = ProductToActivate.productCurrentInventoryCount;
			shadowMarketProductData.Pending = ProductToActivate.productIsPending;
			shadowMarketProductData.Shipped = ProductToActivate.productIsShipped;
			DataManager.Save(shadowMarketProductData);
		}
		SteamSlinger.Ins.ActivateShadowMarketProduct(ProductToActivate.GetHashCode());
		ShadowMarketProductWasActivated.Execute(ProductToActivate);
	}

	public void ShipProduct(ShadowMarketProductDefinition TheProduct)
	{
		ShippedProductObject shippedProductObject = shippedProductPool.Pop();
		shippedProductObject.ProductPickUp.Event += productWasPickedUp;
		shippedProductObject.DroneDeliver(TheProduct);
		currentShippedProducts.Add(shippedProductObject);
		ShadowMarketProductData shadowMarketProductData = DataManager.Load<ShadowMarketProductData>((int)TheProduct.productID);
		if (shadowMarketProductData != null)
		{
			shadowMarketProductData.Pending = TheProduct.productIsPending;
			shadowMarketProductData.Shipped = TheProduct.productIsShipped;
			DataManager.Save(shadowMarketProductData);
		}
	}

	public void MarkShadowProductAsPending(ShadowMarketProductDefinition TheProduct)
	{
		ShadowMarketProductData shadowMarketProductData = DataManager.Load<ShadowMarketProductData>((int)TheProduct.productID);
		if (shadowMarketProductData != null)
		{
			shadowMarketProductData.Pending = true;
			DataManager.Save(shadowMarketProductData);
		}
	}

	public void MarkZeroDayProductAsInstalling(ZeroDayProductDefinition TheProduct)
	{
		ZeroDayProductData zeroDayProductData = DataManager.Load<ZeroDayProductData>((int)TheProduct.productID);
		if (zeroDayProductData != null)
		{
			zeroDayProductData.Installing = true;
			DataManager.Save(zeroDayProductData);
		}
	}

	private void directShipProduct(ShadowMarketProductDefinition TheProduct)
	{
		ShippedProductObject shippedProductObject = shippedProductPool.Pop();
		shippedProductObject.ProductPickUp.Event += productWasPickedUp;
		shippedProductObject.ShipMe(TheProduct, shippedProductPOS, shippedProductROT);
		currentShippedProducts.Add(shippedProductObject);
	}

	private void productWasPickedUp(ShippedProductObject TheProduct)
	{
		TheProduct.ProductPickUp.Event -= productWasPickedUp;
		if (currentShippedProducts.Count >= 2)
		{
			SteamSlinger.Ins.UnlockSteamAchievement(STEAM_ACHIEVEMENT.DOUBLEUP);
		}
		ProductWasPickedUp.Execute();
		currentShippedProducts.Remove(TheProduct);
		shippedProductPool.Push(TheProduct);
	}

	private void stageMe()
	{
		for (int i = 0; i < shadowMarketProducts.Count; i++)
		{
			ShadowMarketProductData shadowMarketProductData = DataManager.Load<ShadowMarketProductData>((int)shadowMarketProducts[i].productID);
			if (shadowMarketProductData == null)
			{
				shadowMarketProductData = new ShadowMarketProductData((int)shadowMarketProducts[i].productID);
				shadowMarketProductData.Owned = false;
				shadowMarketProductData.InventoryCount = 0;
				shadowMarketProductData.Pending = false;
				shadowMarketProductData.Shipped = false;
			}
			shadowMarketProducts[i].productOwned = shadowMarketProductData.Owned;
			shadowMarketProducts[i].productCurrentInventoryCount = shadowMarketProductData.InventoryCount;
			shadowMarketProducts[i].productIsPending = shadowMarketProductData.Pending;
			shadowMarketProducts[i].productIsShipped = shadowMarketProductData.Shipped;
			if (shadowMarketProductData.InventoryCount > 0)
			{
				for (int j = 0; j < shadowMarketProductData.InventoryCount; j++)
				{
					ShadowMarketProductWasActivated.Execute(shadowMarketProducts[i]);
				}
			}
			if (shadowMarketProductData.Shipped)
			{
				directShipProduct(shadowMarketProducts[i]);
			}
			DataManager.Save(shadowMarketProductData);
		}
		for (int k = 0; k < zeroDayProducts.Count; k++)
		{
			ZeroDayProductData zeroDayProductData = DataManager.Load<ZeroDayProductData>((int)zeroDayProducts[k].productID);
			if (zeroDayProductData == null)
			{
				zeroDayProductData = new ZeroDayProductData((int)zeroDayProducts[k].productID);
				zeroDayProductData.InventoryCount = 0;
				zeroDayProductData.Owned = false;
				zeroDayProductData.Installing = false;
			}
			zeroDayProducts[k].productOwned = zeroDayProductData.Owned;
			zeroDayProducts[k].productIsInstalling = zeroDayProductData.Installing;
			zeroDayProducts[k].productInventory = zeroDayProductData.InventoryCount;
			if (zeroDayProducts[k].unlimtedUse)
			{
				if (zeroDayProductData.InventoryCount > 0)
				{
					InventoryManager.AddProduct(zeroDayProducts[k]);
				}
			}
			else if (zeroDayProducts[k].productOwned)
			{
				GameManager.ManagerSlinger.AppManager.ActivateApp(zeroDayProducts[k]);
			}
			DataManager.Save(zeroDayProductData);
		}
		if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			ActivateZeroDayProduct(zeroDayProducts[3]);
		}
		GameManager.StageManager.Stage -= stageMe;
	}
}
