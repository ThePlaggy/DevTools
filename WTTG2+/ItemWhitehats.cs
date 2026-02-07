using System;
using System.Collections.Generic;
using UnityEngine;

public static class ItemWhitehats
{
	public static void GiveItemWhitehat(bool FromTwitch)
	{
		int seed = UnityEngine.Random.Range(1, 100);
		bool[] array = new bool[5]
		{
			!ProductsManager.ownsWhitehatScanner && !PoliceScannerBehaviour.Ins.ownPoliceScanner && !ItemSlinger.PoliceScanner.myProductObject.myProduct.productIsPending && !ItemSlinger.PoliceScanner.myProductObject.myProduct.productIsShipped,
			!ProductsManager.ownsWhitehatRemoteVPN3 && RemoteVPNManager.AmountOfRemoteVPNsLVL3 == 0 && !ItemSlinger.RemoteVPNLevel3.myProductObject.myProduct.productIsPending && !ItemSlinger.RemoteVPNLevel3.myProductObject.myProduct.productIsShipped,
			!ProductsManager.ownsWhitehatRouter && !RouterBehaviour.Ins.Owned && !ItemSlinger.Router.myProductObject.myProduct.productIsPending && !ItemSlinger.Router.myProductObject.myProduct.productIsShipped,
			!ProductsManager.ownsWhitehatDongle2 && InventoryManager.WifiDongleLevel == 0 && !ItemSlinger.WiFiDongleLevel2.myProductObject.myProduct.productIsPending && !ItemSlinger.WiFiDongleLevel2.myProductObject.myProduct.productIsShipped,
			!ProductsManager.ownsSecCams && !CamHookBehaviour.BoughtCameras && !ItemSlinger.SecCams.myProductObject.myProduct.productIsPending && !ItemSlinger.SecCams.myProductObject.myProduct.productIsShipped
		};
		List<Action<bool>> list = new List<Action<bool>>();
		list.Add(WhitehatScanner);
		list.Add(WhitehatVPN);
		list.Add(WhitehatRouter);
		list.Add(WhitehatDongle);
		list.Add(WhitehatCams);
		List<Action<bool>> list2 = list;
		Scramble(array, seed);
		Scramble(list2, seed);
		if (array[0])
		{
			list2[0](FromTwitch);
			return;
		}
		if (array[1])
		{
			list2[1](FromTwitch);
			return;
		}
		if (array[2])
		{
			list2[2](FromTwitch);
			return;
		}
		if (array[3])
		{
			list2[3](FromTwitch);
			return;
		}
		if (array[4])
		{
			list2[4](FromTwitch);
			return;
		}
		GameManager.HackerManager.WhiteHatSound();
		DOSCoinsCurrencyManager.AddCurrency(2.5f);
	}

	private static void WhitehatScanner(bool FromTwitch)
	{
		WindowManager.Get(SOFTWARE_PRODUCTS.SHADOW_MARKET).Launch();
		ProductsManager.ownsWhitehatScanner = true;
		ItemSlinger.PoliceScanner.myProductObject.shipItem();
	}

	private static void WhitehatVPN(bool FromTwitch)
	{
		WindowManager.Get(SOFTWARE_PRODUCTS.SHADOW_MARKET).Launch();
		ProductsManager.ownsWhitehatRemoteVPN3 = true;
		ItemSlinger.RemoteVPNLevel3.myProductObject.shipItem();
	}

	private static void WhitehatRouter(bool FromTwitch)
	{
		WindowManager.Get(SOFTWARE_PRODUCTS.SHADOW_MARKET).Launch();
		ProductsManager.ownsWhitehatRouter = true;
		ItemSlinger.Router.myProductObject.shipItem();
	}

	private static void WhitehatDongle(bool FromTwitch)
	{
		WindowManager.Get(SOFTWARE_PRODUCTS.SHADOW_MARKET).Launch();
		ProductsManager.ownsWhitehatDongle2 = true;
		ItemSlinger.WiFiDongleLevel2.myProductObject.shipItem();
	}

	private static void WhitehatCams(bool FromTwitch)
	{
		WindowManager.Get(SOFTWARE_PRODUCTS.SHADOW_MARKET).Launch();
		ProductsManager.ownsSecCams = true;
		ItemSlinger.SecCams.myProductObject.shipItem();
	}

	private static void Scramble<T>(List<T> list, int seed)
	{
		System.Random random = new System.Random(seed);
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = random.Next(num + 1);
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}

	private static void Scramble(bool[] array, int seed)
	{
		System.Random random = new System.Random(seed);
		int num = array.Length;
		while (num > 1)
		{
			num--;
			int num2 = random.Next(num + 1);
			bool flag = array[num2];
			array[num2] = array[num];
			array[num] = flag;
		}
	}
}
