using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour
{
	public int START_MIN_APP_POOL_COUNT = 4;

	[SerializeField]
	private iconBehavior skyBreakIcon;

	private Dictionary<int, MinnedAppObject> currentMinApps = new Dictionary<int, MinnedAppObject>();

	private PooledStack<MinnedAppObject> minTabObjectPool;

	private void Awake()
	{
		GameManager.ManagerSlinger.AppManager = this;
		minTabObjectPool = new PooledStack<MinnedAppObject>(delegate
		{
			MinnedAppObject component = Object.Instantiate(LookUp.DesktopUI.MIN_WINDOW_TAB_OBJECT, LookUp.DesktopUI.MIN_WINDOW_TAB_HOLDER.GetComponent<RectTransform>()).GetComponent<MinnedAppObject>();
			component.SoftBuild();
			return component;
		}, START_MIN_APP_POOL_COUNT);
	}

	public void LaunchApp(SOFTWARE_PRODUCTS AppToLaunch)
	{
		WindowManager.Get(AppToLaunch).Launch();
	}

	public void LaunchApp(SoftwareProductDefinition UniAppToLaunch)
	{
		WindowManager.Get(UniAppToLaunch).Launch();
	}

	public void CloseApp(SOFTWARE_PRODUCTS AppToClose)
	{
		WindowManager.Get(AppToClose).Close();
	}

	public void CloseApp(SoftwareProductDefinition UniAppToClose)
	{
		WindowManager.Get(UniAppToClose).Close();
	}

	public void MaxApp(SOFTWARE_PRODUCTS AppToMax)
	{
		WindowManager.Get(AppToMax).Max();
	}

	public void MaxApp(SoftwareProductDefinition AppToMax)
	{
		WindowManager.Get(AppToMax).Max();
	}

	public void UnMaxApp(SOFTWARE_PRODUCTS AppToUnMax)
	{
		WindowManager.Get(AppToUnMax).UnMax();
	}

	public void UnMaxApp(SoftwareProductDefinition AppToUnMax)
	{
		WindowManager.Get(AppToUnMax).UnMax();
	}

	public void MinApp(SOFTWARE_PRODUCTS AppToMin)
	{
		WindowManager.Get(AppToMin).Min();
	}

	public void MinApp(SoftwareProductDefinition AppToMin)
	{
		WindowManager.Get(AppToMin).Min();
	}

	public void ResizedApp(SOFTWARE_PRODUCTS AppToResized)
	{
		WindowManager.Get(AppToResized).Resized();
	}

	public void ResizedApp(SoftwareProductDefinition AppToResized)
	{
		WindowManager.Get(AppToResized).Resized();
	}

	public void UnMinApp(SOFTWARE_PRODUCTS AppToUnMin)
	{
		if (currentMinApps.TryGetValue((int)AppToUnMin, out var value))
		{
			currentMinApps.Remove((int)AppToUnMin);
			minTabObjectPool.Push(value);
			WindowManager.Get(AppToUnMin).UnMin();
			minTabsCleanUp();
		}
	}

	public void UnMinApp(SoftwareProductDefinition AppToUnMin)
	{
		if (currentMinApps.TryGetValue(AppToUnMin.GetHashCode(), out var value))
		{
			currentMinApps.Remove(AppToUnMin.GetHashCode());
			minTabObjectPool.Push(value);
			WindowManager.Get(AppToUnMin).UnMin();
			minTabsCleanUp();
		}
	}

	public void DoMinApp(SOFTWARE_PRODUCTS TheAppToMin)
	{
		if (!currentMinApps.ContainsKey((int)TheAppToMin))
		{
			MinnedAppObject minnedAppObject = minTabObjectPool.Pop();
			minnedAppObject.BuildMe(TheAppToMin, currentMinApps.Count);
			currentMinApps.Add((int)TheAppToMin, minnedAppObject);
		}
	}

	public void DoMinApp(SoftwareProductDefinition TheAppToMin)
	{
		if (!currentMinApps.ContainsKey(TheAppToMin.GetHashCode()))
		{
			MinnedAppObject minnedAppObject = minTabObjectPool.Pop();
			minnedAppObject.BuildMe(TheAppToMin, currentMinApps.Count);
			currentMinApps.Add(TheAppToMin.GetHashCode(), minnedAppObject);
		}
	}

	public void ForceUnMinApp(SOFTWARE_PRODUCTS AppToForceUnMin)
	{
		if (currentMinApps.TryGetValue((int)AppToForceUnMin, out var value))
		{
			value.ForceDismissMe();
		}
	}

	public void ForceUnMinApp(SoftwareProductDefinition AppToForceUnMin)
	{
		if (currentMinApps.TryGetValue(AppToForceUnMin.GetHashCode(), out var value))
		{
			value.ForceDismissMe();
		}
	}

	public void ActivateApp(ZeroDayProductDefinition appToActivate)
	{
		switch (appToActivate.productID)
		{
		case SOFTWARE_PRODUCTS.MOTION_SENSOR_AUDIO_QUE:
			InventoryManager.OwnsMotionSensorAudioCue = true;
			break;
		case SOFTWARE_PRODUCTS.LOCATION_SERVICES:
			TrackerManager.LocationServicesBought = true;
			break;
		case SOFTWARE_PRODUCTS.BOTNET:
			BotnetAppBehaviour.Ins.InstalledApp();
			break;
		case SOFTWARE_PRODUCTS.DOORLOG:
			AppCreator.DoorlogAppIcon.SetActive(value: true);
			doorlogRandomBanter.Ins.generateFireWindow();
			break;
		case SOFTWARE_PRODUCTS.SKYBREAK:
			skyBreakIcon.ActivateMe();
			break;
		case SOFTWARE_PRODUCTS.VWIPE:
			VWipeApp.Ins.AddVWipeIcon();
			break;
		case SOFTWARE_PRODUCTS.KEY_CUE:
			InventoryManager.OwnsKeyCue = true;
			break;
		}
		InventoryManager.AddProduct(appToActivate);
	}

	public void minTabsCleanUp()
	{
		int num = 0;
		foreach (KeyValuePair<int, MinnedAppObject> currentMinApp in currentMinApps)
		{
			currentMinApp.Value.RePOSMe(num);
			num++;
		}
	}
}
