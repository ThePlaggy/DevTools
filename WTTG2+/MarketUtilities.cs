using System.Collections.Generic;
using UnityEngine;

public static class MarketUtilities
{
	private static bool vpnFIX;

	public static void ApplyMarketSettings()
	{
		if (!vpnFIX)
		{
			GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[6].isDiscounted = false;
			GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[4].productDesc += " Doesn't work when your computer is muted.";
			vpnFIX = true;
		}
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[0].productPrice = 20f;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[1].productPrice = (DifficultyManager.CasualMode ? 30f : 95f);
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[2].productPrice = (DifficultyManager.CasualMode ? 30f : 45f);
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[3].productPrice = (DifficultyManager.CasualMode ? 10f : 25f);
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[4].productPrice = (DifficultyManager.CasualMode ? 45f : 90f);
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[5].productPrice = (DifficultyManager.CasualMode ? 1f : 35f);
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[6].productPrice = 60f;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[5].productRequiresOtherProduct = true;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[6].productRequiresOtherProduct = true;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[7].productRequiresOtherProduct = true;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[8].productRequiresOtherProduct = true;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[9].productRequiresOtherProduct = true;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[5].productToOwn = GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[3];
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[6].productToOwn = GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[3];
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[7].productToOwn = GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[3];
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[8].productToOwn = GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[3];
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[9].productToOwn = GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[3];
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[2].productToOwn = GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[1];
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[0].deliveryTimeMin = 30f;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[1].deliveryTimeMin = 90f;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[2].deliveryTimeMin = 10f;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[3].deliveryTimeMin = 5f;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[4].deliveryTimeMin = 150f;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[5].deliveryTimeMin = 120f;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[0].deliveryTimeMax = 90f;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[1].deliveryTimeMax = 300f;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[2].deliveryTimeMax = 30f;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[3].deliveryTimeMax = 20f;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[4].deliveryTimeMax = 300f;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[5].deliveryTimeMax = 240f;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[3].productMaxPurchaseAmount = ((!DifficultyManager.LeetMode && !DifficultyManager.Nightmare) ? 1 : 2);
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[0].productPrice = 8f;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[1].productPrice = (DifficultyManager.CasualMode ? 15f : 30f);
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[2].productPrice = (DifficultyManager.CasualMode ? 30f : 45f);
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[3].productPrice = 1f;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[4].productPrice = 10f;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[5].productPrice = 5f;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[6].productPrice = 15f;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[7].productPrice = 30f;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[8].productPrice = 40f;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[9].productPrice = 55f;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[10].productPrice = (DifficultyManager.CasualMode ? 275f : 375f);
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[2].productMaxPurchaseAmount = 5;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[3].installTime = 0.3f;
		bool flag = false;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[3].productDesc = GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[3].productDesc.Replace("scirpts", "scripts");
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[0].productIsRequiredByAnotherProduct = true;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[0].productDepended = GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[1];
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[0].productSprite = SpriteLookUp.wifiDongleLevel2;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[1].productSprite = SpriteLookUp.wifiDongleLevel3;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[2].productSprite = SpriteLookUp.motionSensor;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[3].productSprite = SpriteLookUp.remoteVPN;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[4].productSprite = SpriteLookUp.policeScanner;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[5].productSprite = SpriteLookUp.lolpyDisc;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[0].productSprite = SpriteLookUp.skyBreak;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[1].productSprite = SpriteLookUp.skyBreakWPA;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[2].productSprite = SpriteLookUp.skyBreakWPA2;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[3].productSprite = SpriteLookUp.backDoorHack;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[4].productSprite = SpriteLookUp.motionAlert;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[10].productSprite = SpriteLookUp.keyCue;
		ArrangeStuff();
	}

	public static void ArrangeStuff()
	{
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[0].arrangeSlot = 0;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[1].arrangeSlot = 1;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[2].arrangeSlot = 2;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[3].arrangeSlot = 4;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[4].arrangeSlot = 6;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[5].arrangeSlot = -2;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[6].arrangeSlot = -2;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[7].arrangeSlot = -2;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[8].arrangeSlot = -2;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[9].arrangeSlot = -2;
		GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[10].arrangeSlot = 9;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[0].arrangeSlot = 5;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[1].arrangeSlot = 6;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[2].arrangeSlot = 4;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[3].arrangeSlot = 0;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[4].arrangeSlot = 7;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[5].arrangeSlot = 11;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[6].arrangeSlot = -2;
	}

	public static void addRemoteVPNS(List<ShadowMarketProductDefinition> myProducts)
	{
		ShadowMarketProductDefinition shadowMarketProductDefinition = ScriptableObject.CreateInstance<ShadowMarketProductDefinition>();
		ShadowMarketProductDefinition shadowMarketProductDefinition2 = ScriptableObject.CreateInstance<ShadowMarketProductDefinition>();
		shadowMarketProductDefinition.deliveryTimeMin = 60f;
		shadowMarketProductDefinition.deliveryTimeMax = 120f;
		shadowMarketProductDefinition.id = 6301;
		shadowMarketProductDefinition.isDiscounted = false;
		shadowMarketProductDefinition.productDesc = "Looking to score even more DOS coins? We got you covered! This upgraded device will use an advanced script to accquire 2x DOS coins when placed.";
		shadowMarketProductDefinition.productHasLimitPurchases = true;
		shadowMarketProductDefinition.productID = HARDWARE_PRODUCTS.REMOTE_VPN_LEVEL2;
		shadowMarketProductDefinition.productMaxPurchaseAmount = 2;
		shadowMarketProductDefinition.productName = "Remote VPN Level 2";
		shadowMarketProductDefinition.productRequiresOtherProduct = false;
		shadowMarketProductDefinition.productIsRequiredByAnotherProduct = false;
		shadowMarketProductDefinition.productDepended = shadowMarketProductDefinition2;
		shadowMarketProductDefinition.productSprite = CustomSpriteLookUp.remoteVPNlvl2;
		shadowMarketProductDefinition.productPrice = (DifficultyManager.CasualMode ? 75f : 100f);
		shadowMarketProductDefinition.arrangeSlot = 1;
		myProducts.Add(shadowMarketProductDefinition);
		shadowMarketProductDefinition2.deliveryTimeMin = 90f;
		shadowMarketProductDefinition2.deliveryTimeMax = 180f;
		shadowMarketProductDefinition2.id = 6302;
		shadowMarketProductDefinition2.isDiscounted = false;
		shadowMarketProductDefinition2.productDesc = "Are you even more desperate for that cash? Are you trying to never run out of money in these hard times while wanting to do absolutely nothing? Look no further with this top tier performance!";
		shadowMarketProductDefinition2.productHasLimitPurchases = true;
		shadowMarketProductDefinition2.productID = HARDWARE_PRODUCTS.REMOTE_VPN_LEVEL3;
		shadowMarketProductDefinition2.productMaxPurchaseAmount = 1;
		shadowMarketProductDefinition2.productName = "Remote VPN Level 3";
		shadowMarketProductDefinition2.productRequiresOtherProduct = false;
		shadowMarketProductDefinition2.productIsRequiredByAnotherProduct = false;
		shadowMarketProductDefinition2.productSprite = CustomSpriteLookUp.remoteVPNlvl3;
		shadowMarketProductDefinition2.productPrice = (DifficultyManager.CasualMode ? 120f : 185f);
		shadowMarketProductDefinition2.arrangeSlot = 2;
		myProducts.Add(shadowMarketProductDefinition2);
	}

	public static void addSulphurToMarket(List<ShadowMarketProductDefinition> myProducts)
	{
		ShadowMarketProductDefinition shadowMarketProductDefinition = ScriptableObject.CreateInstance<ShadowMarketProductDefinition>();
		shadowMarketProductDefinition.deliveryTimeMin = 15f;
		shadowMarketProductDefinition.deliveryTimeMax = 25f;
		shadowMarketProductDefinition.id = 6306;
		shadowMarketProductDefinition.isDiscounted = false;
		shadowMarketProductDefinition.productDesc = "Have you ever wanted to make a bomb? Does the thrill of destruction and smoking death of lives unworthy your concern move you? Look no further! With this package of basic sulphur you will get started!";
		shadowMarketProductDefinition.productHasLimitPurchases = true;
		shadowMarketProductDefinition.productID = HARDWARE_PRODUCTS.SULPHUR;
		shadowMarketProductDefinition.productMaxPurchaseAmount = 6;
		shadowMarketProductDefinition.productName = "Sulfur";
		shadowMarketProductDefinition.productRequiresOtherProduct = false;
		shadowMarketProductDefinition.productIsRequiredByAnotherProduct = false;
		shadowMarketProductDefinition.productSprite = CustomSpriteLookUp.sulphur;
		shadowMarketProductDefinition.productPrice = (DifficultyManager.CasualMode ? 5f : 40f);
		shadowMarketProductDefinition.arrangeSlot = 12;
		myProducts.Add(shadowMarketProductDefinition);
	}

	public static void addDWR921Router(List<ShadowMarketProductDefinition> myProducts)
	{
		ShadowMarketProductDefinition shadowMarketProductDefinition = ScriptableObject.CreateInstance<ShadowMarketProductDefinition>();
		shadowMarketProductDefinition.deliveryTimeMin = 120f;
		shadowMarketProductDefinition.deliveryTimeMax = 240f;
		shadowMarketProductDefinition.id = 6307;
		shadowMarketProductDefinition.isDiscounted = false;
		shadowMarketProductDefinition.productDesc = "Are you getting IP tracked by hackers? Do you have issues with your internet speed? This router guarantees secure untraceable connection and fast Automatic VPN. Look no further to get this device!";
		shadowMarketProductDefinition.productHasLimitPurchases = true;
		shadowMarketProductDefinition.productID = HARDWARE_PRODUCTS.ROUTER;
		shadowMarketProductDefinition.productMaxPurchaseAmount = 1;
		shadowMarketProductDefinition.productName = "DWR-921 Router";
		shadowMarketProductDefinition.productRequiresOtherProduct = false;
		shadowMarketProductDefinition.productIsRequiredByAnotherProduct = false;
		shadowMarketProductDefinition.productSprite = CustomSpriteLookUp.router;
		shadowMarketProductDefinition.productPrice = (DifficultyManager.CasualMode ? 40f : 70f);
		shadowMarketProductDefinition.arrangeSlot = 8;
		myProducts.Add(shadowMarketProductDefinition);
	}

	public static void addTarotCards(List<ShadowMarketProductDefinition> myProducts)
	{
		ShadowMarketProductDefinition shadowMarketProductDefinition = ScriptableObject.CreateInstance<ShadowMarketProductDefinition>();
		shadowMarketProductDefinition.deliveryTimeMin = 20f;
		shadowMarketProductDefinition.deliveryTimeMax = 45f;
		shadowMarketProductDefinition.id = 6308;
		shadowMarketProductDefinition.isDiscounted = false;
		shadowMarketProductDefinition.productDesc = "Do you believe in fortune?";
		shadowMarketProductDefinition.productHasLimitPurchases = true;
		shadowMarketProductDefinition.productID = HARDWARE_PRODUCTS.TAROT_CARDS;
		shadowMarketProductDefinition.productMaxPurchaseAmount = 5;
		shadowMarketProductDefinition.productName = "Tarot Cards";
		shadowMarketProductDefinition.productRequiresOtherProduct = false;
		shadowMarketProductDefinition.productIsRequiredByAnotherProduct = false;
		shadowMarketProductDefinition.productSprite = CustomSpriteLookUp.tarotcard;
		shadowMarketProductDefinition.productPrice = 80f;
		shadowMarketProductDefinition.arrangeSlot = 13;
		myProducts.Add(shadowMarketProductDefinition);
	}

	public static void AddSecCams(List<ShadowMarketProductDefinition> myProducts)
	{
		ShadowMarketProductDefinition shadowMarketProductDefinition = ScriptableObject.CreateInstance<ShadowMarketProductDefinition>();
		shadowMarketProductDefinition.deliveryTimeMin = 180f;
		shadowMarketProductDefinition.deliveryTimeMax = 220f;
		shadowMarketProductDefinition.id = 6309;
		shadowMarketProductDefinition.isDiscounted = false;
		shadowMarketProductDefinition.productDesc = "Do you want to add extra security to your house? Are you worried from burglars and other bad guys? Our product promises high resolution security camera!";
		shadowMarketProductDefinition.productHasLimitPurchases = true;
		shadowMarketProductDefinition.productID = HARDWARE_PRODUCTS.CAMERA;
		shadowMarketProductDefinition.productMaxPurchaseAmount = 1;
		shadowMarketProductDefinition.productName = "Surveillance Camera";
		shadowMarketProductDefinition.productRequiresOtherProduct = false;
		shadowMarketProductDefinition.productIsRequiredByAnotherProduct = false;
		shadowMarketProductDefinition.productSprite = CustomSpriteLookUp.seckscamsStore;
		shadowMarketProductDefinition.productPrice = (DifficultyManager.CasualMode ? 60f : 120f);
		shadowMarketProductDefinition.arrangeSlot = 10;
		myProducts.Add(shadowMarketProductDefinition);
	}

	public static void addKeypad(List<ShadowMarketProductDefinition> myProducts)
	{
		ShadowMarketProductDefinition shadowMarketProductDefinition = ScriptableObject.CreateInstance<ShadowMarketProductDefinition>();
		shadowMarketProductDefinition.deliveryTimeMin = 100f;
		shadowMarketProductDefinition.deliveryTimeMax = 150f;
		shadowMarketProductDefinition.id = 6310;
		shadowMarketProductDefinition.isDiscounted = false;
		shadowMarketProductDefinition.productDesc = "Worried about break-ins? Need reliable security? Then this security lock is for you! The code is regenerated every now and then to ensure max security!";
		shadowMarketProductDefinition.productHasLimitPurchases = true;
		shadowMarketProductDefinition.productID = HARDWARE_PRODUCTS.KEYPAD;
		shadowMarketProductDefinition.productMaxPurchaseAmount = 1;
		shadowMarketProductDefinition.productName = "MK-II Security Lock";
		shadowMarketProductDefinition.productRequiresOtherProduct = false;
		shadowMarketProductDefinition.productIsRequiredByAnotherProduct = false;
		shadowMarketProductDefinition.productSprite = CustomSpriteLookUp.KeypadStore;
		shadowMarketProductDefinition.productPrice = (DifficultyManager.CasualMode ? 150f : 235f);
		shadowMarketProductDefinition.arrangeSlot = 9;
		myProducts.Add(shadowMarketProductDefinition);
	}

	public static void addStrongFlashlight(List<ShadowMarketProductDefinition> myProducts)
	{
		ShadowMarketProductDefinition shadowMarketProductDefinition = ScriptableObject.CreateInstance<ShadowMarketProductDefinition>();
		shadowMarketProductDefinition.deliveryTimeMin = 50f;
		shadowMarketProductDefinition.deliveryTimeMax = 100f;
		shadowMarketProductDefinition.id = 6322;
		shadowMarketProductDefinition.isDiscounted = false;
		shadowMarketProductDefinition.productDesc = "Can't see in the dark? Traditional flashlights aren't good enough? Not anymore! With the strong flashlight you'll be able to see everything in the dark.";
		shadowMarketProductDefinition.productHasLimitPurchases = true;
		shadowMarketProductDefinition.productID = HARDWARE_PRODUCTS.STRONG_FLASHLIGHT;
		shadowMarketProductDefinition.productMaxPurchaseAmount = 1;
		shadowMarketProductDefinition.productName = "Strong Flashlight";
		shadowMarketProductDefinition.productRequiresOtherProduct = false;
		shadowMarketProductDefinition.productIsRequiredByAnotherProduct = false;
		shadowMarketProductDefinition.productSprite = CustomSpriteLookUp.strongflashlight;
		shadowMarketProductDefinition.productPrice = 10f;
		shadowMarketProductDefinition.arrangeSlot = 3;
		myProducts.Add(shadowMarketProductDefinition);
	}

	public static void ReplaceVWipeWithSpeed()
	{
		ZeroDayProductDefinition zeroDayProductDefinition = GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[11];
		zeroDayProductDefinition.installTime = 8f;
		zeroDayProductDefinition.isDiscounted = false;
		zeroDayProductDefinition.productDesc = "Do you have really high ping? This script may help you boost up your internet speed by 3 times.";
		zeroDayProductDefinition.productID = SOFTWARE_PRODUCTS.SPEED_POWERUP;
		zeroDayProductDefinition.productName = "P1NG_B005T.EXE";
		zeroDayProductDefinition.productSprite = CustomSpriteLookUp.speeditem;
		zeroDayProductDefinition.productRequiresOtherProduct = false;
		zeroDayProductDefinition.productToOwn = null;
		zeroDayProductDefinition.unlimtedUse = true;
		zeroDayProductDefinition.productToOwn = GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[3];
		zeroDayProductDefinition.productPrice = (DifficultyManager.CasualMode ? 5f : 50f);
		zeroDayProductDefinition.arrangeSlot = 5;
	}

	public static void AddBotnetApp(List<ZeroDayProductDefinition> myProducts)
	{
		ZeroDayProductDefinition zeroDayProductDefinition = ScriptableObject.CreateInstance<ZeroDayProductDefinition>();
		zeroDayProductDefinition.id = 6305;
		zeroDayProductDefinition.installTime = 15f;
		zeroDayProductDefinition.isDiscounted = false;
		zeroDayProductDefinition.productDesc = "Having trouble getting money? With this tool you can hack nearby computers and create a botnet network that will generate DOSCoins for you.";
		zeroDayProductDefinition.productID = SOFTWARE_PRODUCTS.BOTNET;
		zeroDayProductDefinition.productName = "botnetMNGR";
		zeroDayProductDefinition.productSprite = CustomSpriteLookUp.botnetStore;
		zeroDayProductDefinition.productRequiresOtherProduct = false;
		zeroDayProductDefinition.productToOwn = null;
		zeroDayProductDefinition.unlimtedUse = false;
		zeroDayProductDefinition.productPrice = (DifficultyManager.CasualMode ? 25f : 50f);
		zeroDayProductDefinition.arrangeSlot = 3;
		myProducts.Add(zeroDayProductDefinition);
	}

	public static void AddDoorLogApp(List<ZeroDayProductDefinition> myProducts)
	{
		ZeroDayProductDefinition zeroDayProductDefinition = ScriptableObject.CreateInstance<ZeroDayProductDefinition>();
		zeroDayProductDefinition.id = 6315;
		zeroDayProductDefinition.installTime = 20f;
		zeroDayProductDefinition.isDiscounted = false;
		zeroDayProductDefinition.productDesc = "Do you want to spy on your neighbors? Paranoid that someone can break into your apartment while you're gone? Seamlessly track door movements of your entire apartment complex with this tool.";
		zeroDayProductDefinition.productID = SOFTWARE_PRODUCTS.DOORLOG;
		zeroDayProductDefinition.productName = "doorLOG";
		zeroDayProductDefinition.productSprite = CustomSpriteLookUp.doorLogStore;
		zeroDayProductDefinition.productRequiresOtherProduct = false;
		zeroDayProductDefinition.productToOwn = null;
		zeroDayProductDefinition.unlimtedUse = false;
		zeroDayProductDefinition.productPrice = (DifficultyManager.CasualMode ? 20f : 30f);
		zeroDayProductDefinition.arrangeSlot = 8;
		myProducts.Add(zeroDayProductDefinition);
	}

	public static void AddLocationServices(List<ZeroDayProductDefinition> myProducts)
	{
		ZeroDayProductDefinition zeroDayProductDefinition = ScriptableObject.CreateInstance<ZeroDayProductDefinition>();
		zeroDayProductDefinition.id = 6322;
		zeroDayProductDefinition.installTime = 10f;
		zeroDayProductDefinition.isDiscounted = false;
		zeroDayProductDefinition.productDesc = "Get notified with an audio cue every time your A D.O S is broadcasting your current position and the location service icon is displayed. Doesn't work when your computer is muted.";
		zeroDayProductDefinition.productID = SOFTWARE_PRODUCTS.LOCATION_SERVICES;
		zeroDayProductDefinition.productName = "Location Services Alert";
		zeroDayProductDefinition.productSprite = CustomSpriteLookUp.locationServicesBuyIcon;
		zeroDayProductDefinition.productRequiresOtherProduct = false;
		zeroDayProductDefinition.productToOwn = null;
		zeroDayProductDefinition.unlimtedUse = false;
		zeroDayProductDefinition.productPrice = (DifficultyManager.CasualMode ? 20f : 45f);
		zeroDayProductDefinition.arrangeSlot = 7;
		myProducts.Add(zeroDayProductDefinition);
	}
}
