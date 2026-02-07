using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BotnetBehaviour : MonoBehaviour
{
	public static BotnetBehaviour Ins;

	[SerializeField]
	private CanvasGroup DeviceNotConnected1;

	[SerializeField]
	private CanvasGroup DeviceNotConnected2;

	[SerializeField]
	private CanvasGroup DeviceNotConnected3;

	[SerializeField]
	private CanvasGroup DeviceNotConnected4;

	[SerializeField]
	private CanvasGroup DeviceNotConnected5;

	[SerializeField]
	private CanvasGroup DeviceNotConnected6;

	[SerializeField]
	private CanvasGroup DeviceNotConnected7;

	[SerializeField]
	private CanvasGroup DeviceNotConnected8;

	[SerializeField]
	private CanvasGroup DeviceNotConnected9;

	[SerializeField]
	private CanvasGroup DeviceNotConnected10;

	public TMP_Text backdoorCount;

	public TMP_Text connectedDevices;

	public TMP_Text totalIncomeText;

	public TMP_Text collectedDosText;

	public Image[] connectedDevicesImages;

	[SerializeField]
	private TMP_Text[] names;

	[SerializeField]
	private TMP_Text[] sex;

	[SerializeField]
	private BotnetAppBTN payoutBTN;

	[SerializeField]
	private BotnetAppBTN connectionLostPayoutBTN;

	[SerializeField]
	private CanvasGroup popup;

	public GameObject foundDevicePrefab;

	public GameObject foundDevicesList;

	[HideInInspector]
	public List<Device> AllDevices = new List<Device>();

	[HideInInspector]
	public List<Device> AvailableDevices = new List<Device>();

	[HideInInspector]
	public List<Device> workingDevices = new List<Device>();

	[HideInInspector]
	public List<Device> foundDevices = new List<Device>();

	[HideInInspector]
	public List<Device> deskDevices = new List<Device>();

	[HideInInspector]
	public List<Device> windowDevices = new List<Device>();

	[HideInInspector]
	public List<Device> bedDevices = new List<Device>();

	[HideInInspector]
	public List<string> realNames = new List<string>();

	public KAttack kernelCompiler;

	public int connectedDevicesCount;

	public float generatedDos;

	private DongleLocation.Location LastDongleLocation = DongleLocation.Location.DESK;

	public static void SetHermit()
	{
		if (!(Ins == null))
		{
			Ins.CalculateTotalIncome();
		}
	}

	private void Awake()
	{
		Ins = this;
		BuildNames();
		BuildAllDevices();
		BuildDevicesPerLocation();
		LookForDevices();
		CalculateTotalIncome();
		payoutBTN.setMyAction(Payout);
		GameManager.TimeSlinger.FireTimer(1f, DoJobs);
		connectionLostPayoutBTN.setMyAction(PopupClose);
	}

	private void PopupClose()
	{
		popup.interactable = false;
		popup.blocksRaycasts = false;
		popup.DOFade(0f, 0.5f);
		GameManager.HackerManager.WhiteHatSound();
		DOSCoinsCurrencyManager.AddCurrency(generatedDos);
		generatedDos = 0f;
	}

	public void AddDevDevice(Device device)
	{
		switch (DongleLocation.GetCurrentDongleLocation())
		{
		case DongleLocation.Location.DESK:
			deskDevices.Add(device);
			break;
		case DongleLocation.Location.BED:
			bedDevices.Add(device);
			break;
		case DongleLocation.Location.WINDOW:
			windowDevices.Add(device);
			break;
		}
		LookForDevices();
	}

	public void LookForDevices()
	{
		BotnetFoundDeviceBTN[] componentsInChildren = foundDevicesList.transform.GetComponentsInChildren<BotnetFoundDeviceBTN>();
		foreach (BotnetFoundDeviceBTN botnetFoundDeviceBTN in componentsInChildren)
		{
			UnityEngine.Object.Destroy(botnetFoundDeviceBTN.gameObject);
		}
		switch (DongleLocation.GetCurrentDongleLocation())
		{
		case DongleLocation.Location.DESK:
			foundDevices = deskDevices;
			break;
		case DongleLocation.Location.BED:
			foundDevices = bedDevices;
			break;
		case DongleLocation.Location.WINDOW:
			foundDevices = windowDevices;
			break;
		default:
			foundDevices = new List<Device>();
			break;
		}
		if (DongleLocation.GetCurrentDongleLocation() != LastDongleLocation)
		{
			LoseConnection();
		}
		LastDongleLocation = DongleLocation.GetCurrentDongleLocation();
		foreach (Device foundDevice in foundDevices)
		{
			if (!foundDevice.isOffline)
			{
				BotnetFoundDeviceBTN component = UnityEngine.Object.Instantiate(foundDevicePrefab, foundDevicesList.transform).GetComponent<BotnetFoundDeviceBTN>();
				switch (foundDevice.deviceType)
				{
				case DeviceType.LAPTOP:
					component.deviceIcon.sprite = CustomSpriteLookUp.laptopIcon;
					break;
				case DeviceType.COMPUTER:
					component.deviceIcon.sprite = CustomSpriteLookUp.computerIcon;
					break;
				}
				component.deviceName.text = foundDevice.name;
				component.hackPrice.text = foundDevice.hackPrice.ToString();
				component.device = foundDevice;
			}
		}
	}

	public void CalculateTotalIncome()
	{
		float num = 0f;
		foreach (Device workingDevice in workingDevices)
		{
			num += workingDevice.genDos / workingDevice.genTime;
		}
		float num2 = num * 120f;
		num2 = (float)(Math.Floor(num2 * 100f) / 100.0);
		totalIncomeText.text = num2.ToString("0.###") + (TarotManager.HermitActive ? "/60s" : "/120s");
	}

	public void BuildAllDevices()
	{
		for (int i = 0; i < 100; i++)
		{
			AllDevices.Add(GenerateRandomDevice());
		}
		AllDevices.Add(new Device().BuildMe("CyberFurry Elite", 7f, 39f, DeviceType.LAPTOP, 50, 10));
		AllDevices.Add(new Device().BuildMe("LaptopWithThickAss", 4f, 60f, DeviceType.LAPTOP, 11, 4));
		AllDevices.Add(new Device().BuildMe("HahaMaster 5000", 14.6f, 40.1f, DeviceType.COMPUTER, 95, 10));
		AllDevices.Add(new Device().BuildMe("Guest PC", 3f, 122f, DeviceType.COMPUTER, 8, 2));
		AllDevices.Add(new Device().BuildMe("404 Not Found", 12f, 25f, DeviceType.LAPTOP, 28, 9));
		AllDevices.Add(new Device().BuildMe("myass open 6 AM", 6f, 66f, DeviceType.COMPUTER, 12, 7));
		AllDevices.Add(new Device().BuildMe("Shawn's Microwave", 5.3f, 119f, DeviceType.LAPTOP, 30, 6));
		AllDevices.Add(new Device().BuildMe("Otrex Fridge", 6.9f, 420f, DeviceType.LAPTOP, 16, 2));
		AllDevices.Add(new Device().BuildMe("PleaseNoHack", 8.21f, 168f, DeviceType.COMPUTER, 42, 8));
		AllDevices.Add(new Device().BuildMe("JestDrive", 7.91f, 359f, DeviceType.COMPUTER, 38, 6));
		AllDevices.Add(new Device().BuildMe("LOLoader", 6.14f, 108f, DeviceType.LAPTOP, 26, 5));
		AllDevices.Add(new Device().BuildMe("poggers", 0.1f, 700f, DeviceType.COMPUTER, 99, 3));
	}

	public void DoJobs()
	{
		foreach (Device workingDevice in workingDevices)
		{
			generatedDos += workingDevice.genDos / workingDevice.genTime * (float)((!TarotManager.HermitActive) ? 1 : 2);
		}
		collectedDosText.text = "Collected DOS: " + generatedDos;
		GameManager.TimeSlinger.FireTimer(1f, DoJobs);
	}

	public Device GenerateRandomDevice()
	{
		Device device = new Device();
		bool flag = false;
		int num = -1;
		if (realNames.Count > 0)
		{
			num = UnityEngine.Random.Range(0, realNames.Count - 1);
		}
		else
		{
			bool flag2 = false;
		}
		switch ((num < 0) ? UnityEngine.Random.Range(1, 3) : UnityEngine.Random.Range(0, 4))
		{
		case 0:
			flag = true;
			device.name = realNames[num] + "-PC";
			device.deviceType = DeviceType.COMPUTER;
			break;
		case 1:
			device.name = "DESKTOP-" + GenerateRandomString(7);
			device.deviceType = DeviceType.COMPUTER;
			break;
		case 2:
			device.name = "LAPTOP-" + GenerateRandomString(7);
			device.deviceType = DeviceType.LAPTOP;
			break;
		case 3:
			flag = true;
			device.name = realNames[num] + "'s Laptop";
			device.deviceType = DeviceType.LAPTOP;
			break;
		}
		if (flag)
		{
			realNames.RemoveAt(num);
		}
		switch (UnityEngine.Random.Range(0, 3))
		{
		case 0:
			device.genDos = UnityEngine.Random.Range(0.5f, 3.7f);
			device.genTime = UnityEngine.Random.Range(120f, 150f);
			device.hackPrice = UnityEngine.Random.Range(5, 20);
			device.hackDifficulty = UnityEngine.Random.Range(0, 5);
			break;
		case 1:
			device.genDos = UnityEngine.Random.Range(4f, 7.8f);
			device.genTime = UnityEngine.Random.Range(75f, 123f);
			device.hackPrice = UnityEngine.Random.Range(20, 40);
			device.hackDifficulty = UnityEngine.Random.Range(4, 8);
			break;
		case 2:
			device.genDos = UnityEngine.Random.Range(6.98f, 9.1f);
			device.genTime = UnityEngine.Random.Range(35f, 88f);
			device.hackPrice = UnityEngine.Random.Range(35, 70);
			device.hackDifficulty = UnityEngine.Random.Range(7, 10);
			break;
		}
		device.genDos *= 100f;
		device.genDos = (int)device.genDos;
		device.genDos /= 100f;
		device.genTime *= 100f;
		device.genTime = (int)device.genTime;
		device.genTime /= 100f;
		return device;
	}

	public void BuildNames()
	{
		realNames.Add("James");
		realNames.Add("Jimmy");
		realNames.Add("Emma");
		realNames.Add("Liam");
		realNames.Add("Olivia");
		realNames.Add("Noah");
		realNames.Add("Ava");
		realNames.Add("Isabella");
		realNames.Add("Sophia");
		realNames.Add("Jackson");
		realNames.Add("Benjamin");
		realNames.Add("Mia");
		realNames.Add("Elijah");
		realNames.Add("Amelia");
		realNames.Add("Carter");
		realNames.Add("Evelyn");
		realNames.Add("Alexander");
		realNames.Add("Harper");
		realNames.Add("Aiden");
		realNames.Add("Ella");
		realNames.Add("Michael");
		realNames.Add("Scarlett");
		realNames.Add("Owen");
		realNames.Add("Grace");
		realNames.Add("Mason");
		realNames.Add("Chloe");
		realNames.Add("Logan");
		realNames.Add("Aubrey");
		realNames.Add("Ethan");
		realNames.Add("Zoey");
		realNames.Add("Lucas");
		realNames.Add("Lily");
		realNames.Add("Daniel");
		realNames.Add("Layla");
		realNames.Add("Matthew");
		realNames.Add("Riley");
		realNames.Add("Jackson");
		realNames.Add("Nora");
		realNames.Add("Caleb");
		realNames.Add("Sofia");
		realNames.Add("Sebastian");
		realNames.Add("Camila");
		realNames.Add("Gabriel");
		realNames.Add("Avery");
		realNames.Add("Wyatt");
		realNames.Add("Eleanor");
		realNames.Add("Henry");
		realNames.Add("Madison");
		realNames.Add("David");
		realNames.Add("Penelope");
		realNames.Add("Joseph");
		realNames.Add("Aria");
		realNames.Add("Dylan");
		realNames.Add("Scarlet");
		realNames.Add("Nasko");
		realNames.Add("Landon");
		realNames.Add("Hazel");
		realNames.Add("Cameron");
		realNames.Add("Aiden");
		realNames.Add("Brooklyn");
		realNames.Add("Zachary");
		realNames.Add("Luna");
		realNames.Add("Grayson");
		realNames.Add("Violet");
		realNames.Add("Nathan");
		realNames.Add("Stella");
		realNames.Add("Leo");
		realNames.Add("Peyton");
		realNames.Add("Isaac");
		realNames.Add("Zara");
		realNames.Add("Miles");
		realNames.Add("Bella");
		realNames.Add("Caleb");
		realNames.Add("Lila");
		realNames.Add("Nicholas");
		realNames.Add("Gianna");
		realNames.Add("Oscar");
		realNames.Add("Natalie");
		realNames.Add("Ezra");
		realNames.Add("Leah");
		realNames.Add("Eli");
		realNames.Add("Samantha");
		realNames.Add("Hunter");
		realNames.Add("Audrey");
		realNames.Add("Cooper");
		realNames.Add("Maya");
		realNames.Add("Lincoln");
		realNames.Add("Ashley");
		realNames.Add("Carson");
		realNames.Add("Aria");
		realNames.Add("Sadie");
		realNames.Add("Adam");
		realNames.Add("Victoria");
		realNames.Add("Max");
		realNames.Add("Alexa");
		realNames.Add("Caden");
		realNames.Add("Nina");
		realNames.Add("Julian");
		realNames.Add("Claire");
		realNames.Add("Levi");
		realNames.Add("Skylar");
		realNames.Add("Gustavo");
		realNames.Add("Saul");
		realNames.Add("Hector");
		realNames.Add("Lalo");
		realNames.Add("Walter");
		realNames.Add("Tristan");
		realNames.Add("Jesse");
		realNames.Add("Insym");
	}

	private static string GenerateRandomString(int length)
	{
		char[] array = new char[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"[UnityEngine.Random.Range(0, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".Length)];
		}
		return new string(array);
	}

	public void AddNewDevice()
	{
		if (AvailableDevices.Count <= 0)
		{
			Debug.Log("[BOTNET] No more devices!");
			return;
		}
		int index = UnityEngine.Random.Range(0, AvailableDevices.Count);
		switch (UnityEngine.Random.Range(0, 3))
		{
		case 0:
			deskDevices.Add(AvailableDevices[index]);
			Debug.Log("[BOTNET] (DESK) New device. " + AvailableDevices[index].name);
			AvailableDevices.RemoveAt(index);
			break;
		case 1:
			bedDevices.Add(AvailableDevices[index]);
			Debug.Log("[BOTNET] (BED) New device. " + AvailableDevices[index].name);
			AvailableDevices.RemoveAt(index);
			break;
		case 2:
			windowDevices.Add(AvailableDevices[index]);
			Debug.Log("[BOTNET] (WINDOW) New device. " + AvailableDevices[index].name);
			AvailableDevices.RemoveAt(index);
			break;
		}
		LookForDevices();
		GameManager.TimeSlinger.FireTimer(UnityEngine.Random.Range(800f, 1200f), AddNewDevice);
	}

	public void BuildDevicesPerLocation()
	{
		AvailableDevices = AllDevices;
		for (int i = 0; i < 9; i++)
		{
			int index = UnityEngine.Random.Range(0, AvailableDevices.Count);
			deskDevices.Add(AvailableDevices[index]);
			AvailableDevices.RemoveAt(index);
		}
		for (int j = 0; j < 15; j++)
		{
			int index2 = UnityEngine.Random.Range(0, AvailableDevices.Count);
			windowDevices.Add(AvailableDevices[index2]);
			AvailableDevices.RemoveAt(index2);
		}
		for (int k = 0; k < 11; k++)
		{
			int index3 = UnityEngine.Random.Range(0, AvailableDevices.Count);
			bedDevices.Add(AvailableDevices[index3]);
			AvailableDevices.RemoveAt(index3);
		}
	}

	public void AddDevice(Device device)
	{
		if (connectedDevicesCount != 10)
		{
			device.isOffline = true;
			LookForDevices();
			connectedDevicesCount++;
			connectedDevices.text = connectedDevicesCount.ToString();
			names[connectedDevicesCount - 1].text = device.name;
			sex[connectedDevicesCount - 1].text = device.genDos + "/" + device.genTime + "s";
			switch (device.deviceType)
			{
			case DeviceType.LAPTOP:
				connectedDevicesImages[connectedDevicesCount - 1].sprite = CustomSpriteLookUp.laptopIcon;
				break;
			case DeviceType.COMPUTER:
				connectedDevicesImages[connectedDevicesCount - 1].sprite = CustomSpriteLookUp.computerIcon;
				break;
			}
			workingDevices.Add(device);
			switch (connectedDevicesCount)
			{
			case 1:
				DeviceNotConnected1.alpha = 0f;
				break;
			case 2:
				DeviceNotConnected2.alpha = 0f;
				break;
			case 3:
				DeviceNotConnected3.alpha = 0f;
				break;
			case 4:
				DeviceNotConnected4.alpha = 0f;
				break;
			case 5:
				DeviceNotConnected5.alpha = 0f;
				break;
			case 6:
				DeviceNotConnected6.alpha = 0f;
				break;
			case 7:
				DeviceNotConnected7.alpha = 0f;
				break;
			case 8:
				DeviceNotConnected8.alpha = 0f;
				break;
			case 9:
				DeviceNotConnected9.alpha = 0f;
				break;
			case 10:
				DeviceNotConnected10.alpha = 0f;
				break;
			}
			CalculateTotalIncome();
		}
	}

	public void DismissKernelCompiler()
	{
		CanvasGroup myCG = kernelCompiler.gameObject.GetComponent<CanvasGroup>();
		DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 0f, 0.25f).SetEase(Ease.Linear).OnComplete(delegate
		{
			kernelCompiler.gameObject.SetActive(value: false);
		});
	}

	public void LaunchKernelCompiler(Device device)
	{
		kernelCompiler.nowHacking.text = "Currently hacking: " + device.name;
		kernelCompiler.gameObject.SetActive(value: true);
		CanvasGroup myCG = kernelCompiler.gameObject.GetComponent<CanvasGroup>();
		DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 1f, 0.25f).SetEase(Ease.Linear).OnComplete(delegate
		{
			kernelCompiler.startAttack(device);
		});
	}

	public void LoseConnection()
	{
		if (connectedDevicesCount > 0)
		{
			if (BotnetAppBehaviour.AppIsLaunched && generatedDos > 0f)
			{
				GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.NodeCold);
			}
			DisconnectAllDevices(ConnectionLost: true);
			popup.DOFade(1f, 0.5f).OnComplete(delegate
			{
				popup.interactable = true;
				popup.blocksRaycasts = true;
			});
		}
	}

	public void DisconnectAllDevices(bool ConnectionLost = false)
	{
		connectedDevices.text = "0";
		generatedDos = (ConnectionLost ? (generatedDos / 2f) : 0f);
		connectedDevicesCount = 0;
		DeviceNotConnected1.alpha = 1f;
		DeviceNotConnected2.alpha = 1f;
		DeviceNotConnected3.alpha = 1f;
		DeviceNotConnected4.alpha = 1f;
		DeviceNotConnected5.alpha = 1f;
		DeviceNotConnected6.alpha = 1f;
		DeviceNotConnected7.alpha = 1f;
		DeviceNotConnected8.alpha = 1f;
		DeviceNotConnected9.alpha = 1f;
		DeviceNotConnected10.alpha = 1f;
		workingDevices.Clear();
		CalculateTotalIncome();
	}

	public void Payout()
	{
		if (generatedDos <= 0f)
		{
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.Denied);
			return;
		}
		DOSCoinsCurrencyManager.AddCurrency(generatedDos);
		GameManager.HackerManager.WhiteHatSound();
		DisconnectAllDevices();
	}
}
