using System;
using System.Collections.Generic;
using UnityEngine;

public class skyBreakWPABehavior : MonoBehaviour
{
	[Range(3.5f, 8f)]
	public float minScanTime = 4f;

	[Range(5.5f, 15f)]
	public float maxScanTime = 10f;

	[Range(5f, 30f)]
	public float minCrackTime = 8f;

	[Range(15f, 120f)]
	public float maxCrackTime = 45f;

	private WifiNetworkDefinition anticheatWifiTag;

	private WifiNetworkDefinition currentTargetedWPA;

	private Timer injectionLoopTimer;

	private TerminalLineObject injectLine;

	private skyBreakBehavior mySkyBreakBehavior;

	private TerminalHelperBehavior myTerminalHelper;

	private Dictionary<string, WifiWPAObject> targetedWPANetworks;

	private Timer wpaCrackedTimer;

	private TerminalLineObject wpaCrackKeys;

	private DOSTween wpaUpdateKeysTested;

	protected WIFI_SECURITY myCracker;

	protected string crackerName;

	private void Awake()
	{
		myCracker = WIFI_SECURITY.WPA;
		crackerName = "WPA";
	}

	private void Start()
	{
		targetedWPANetworks = new Dictionary<string, WifiWPAObject>();
		mySkyBreakBehavior = GetComponent<skyBreakBehavior>();
		myTerminalHelper = GetComponent<TerminalHelperBehavior>();
	}

	public void CloseMeOut()
	{
		GameManager.TweenSlinger.KillTween(wpaUpdateKeysTested);
		GameManager.TimeSlinger.KillTimer(injectionLoopTimer);
		GameManager.TimeSlinger.KillTimer(wpaCrackedTimer);
		targetedWPANetworks.Clear();
	}

	public bool ProcessCMD(string theCMD)
	{
		bool result = true;
		string[] array = theCMD.Split(new string[1] { " " }, StringSplitOptions.None);
		array[0] = array[0].ToLower();
		string text = array[0];
		if (text != null)
		{
			switch (text)
			{
			case "exit":
				result = false;
				mySkyBreakBehavior.SpitLastCMD(theCMD);
				SwitchToMode();
				break;
			case "scan":
				result = false;
				PerformScan();
				break;
			case "inject":
				result = false;
				PerformInject(theCMD);
				break;
			case "crack":
				result = false;
				PerformCrack(theCMD);
				break;
			default:
				return true;
			}
		}
		return result;
	}

	private void SwitchToMode()
	{
		mySkyBreakBehavior.SwitchStateMode(SkyBreakActionState.MODE);
	}

	private void PerformScan()
	{
		myTerminalHelper.ClearInputLine();
		myTerminalHelper.ClearTerminal();
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "Scanning...", 0.5f);
		GameManager.TimeSlinger.FireTimer(GetRandScanTime(), ScanResults);
	}

	private void ScanResults()
	{
		if (mySkyBreakBehavior.Window.activeSelf)
		{
			List<WifiNetworkDefinition> secureNetworks = GameManager.ManagerSlinger.WifiManager.GetSecureNetworks(myCracker);
			float num = 0.3f;
			myTerminalHelper.ClearTerminal();
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, "Scanned " + crackerName + " Network Results");
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.FADE, MagicSlinger.FluffString("ESSID", " ", 20) + "    " + MagicSlinger.FluffString("BSSID", " ", 20) + "    " + MagicSlinger.FluffString("CH", " ", 10) + "    " + MagicSlinger.FluffString("PING", " ", 10) + "    SIG", 0.3f);
			for (int i = 0; i < secureNetworks.Count; i++)
			{
				myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.FADE, MagicSlinger.FluffString(secureNetworks[i].networkName, " ", 20) + "    " + MagicSlinger.FluffString(secureNetworks[i].networkBSSID, " ", 20) + "    " + MagicSlinger.FluffString(secureNetworks[i].networkChannel.ToString(), " ", 10) + "    " + MagicSlinger.FluffString(secureNetworks[i].networkPower.ToString(), " ", 10) + "    " + MagicSlinger.GetWifiSiginalType(secureNetworks[i].networkSignal), 0.2f, num);
				num += 0.2f;
			}
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, string.Empty);
			GameManager.TimeSlinger.FireTimer(num, mySkyBreakBehavior.addCMDInputLine);
		}
	}

	private void PerformInject(string theCMD)
	{
		string setLine = string.Empty;
		bool flag = true;
		int result = -1;
		int result2 = -1;
		string[] array = theCMD.Split(new string[1] { " " }, StringSplitOptions.None);
		if (array.Length < 4)
		{
			flag = false;
			setLine = "Invalid usage of inject. Valid usage: inject <BSSID> <CH> <Injections>";
		}
		if (flag)
		{
			if (GameManager.ManagerSlinger.WifiManager.CheckBSSID(array[1], out currentTargetedWPA, myCracker))
			{
				if (!int.TryParse(array[2], out result))
				{
					flag = false;
					setLine = "Invalid channel number.";
				}
				if (!int.TryParse(array[3], out result2))
				{
					flag = false;
					setLine = "Invalid injection amount.";
				}
			}
			else
			{
				flag = false;
				setLine = "Could not find a network with the BSSID Of '" + array[1] + "'";
			}
		}
		if (flag)
		{
			if (currentTargetedWPA.networkChannel != (short)result)
			{
				flag = false;
				setLine = "Invalid channel number.";
			}
			if (result2 < 1 || result2 > 1000)
			{
				flag = false;
				setLine = "Invalid injection amount. Please select 1 - 1000";
			}
		}
		if (flag)
		{
			mySkyBreakBehavior.SpitLastCMD(theCMD);
			myTerminalHelper.ClearInputLine();
			GameManager.TimeSlinger.FireHardTimer(out injectionLoopTimer, (myCracker == WIFI_SECURITY.WPA2) ? 0.1f : 0.03f, AddInjection, result2);
			if (injectionLoopTimer != null)
			{
				injectionLoopTimer.AddLoopCallBack(AddInjectionLoopOver);
			}
		}
		else
		{
			mySkyBreakBehavior.SpitLastCMD(theCMD);
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, setLine);
			myTerminalHelper.PushInputLineToBottom();
		}
	}

	private void AddInjection()
	{
		string key = MagicSlinger.MD5It(currentTargetedWPA.networkName);
		if (!targetedWPANetworks.ContainsKey(key))
		{
			targetedWPANetworks.Add(key, new WifiWPAObject(currentTargetedWPA, 0, 0));
		}
		else if (currentTargetedWPA.injectWindowActive)
		{
			if (Time.time - currentTargetedWPA.injectTimeStamp >= currentTargetedWPA.injectFireWindow)
			{
				Debug.Log("[WPAObject] " + targetedWPANetworks[key].myWifiNetwork.networkName + " injection cooloff reset");
				targetedWPANetworks[key].CurrentInjectionAmount = 0;
			}
			else if (targetedWPANetworks[key].myWifiNetwork.networkInjectionCoolOffTime * ((myCracker == WIFI_SECURITY.WPA) ? 0.8f : 1f) - (targetedWPANetworks[key].myWifiNetwork.networkInjectionCoolOffTime * ((myCracker == WIFI_SECURITY.WPA) ? 0.8f : 1f) - (Time.time - currentTargetedWPA.injectTimeStamp)) > 0.15f)
			{
				Debug.Log("[WPAObject] " + targetedWPANetworks[key].myWifiNetwork.networkName + " wasnt reset yet, " + (currentTargetedWPA.injectFireWindow - (Time.time - currentTargetedWPA.injectTimeStamp)) + " seconds left");
			}
		}
		if (injectLine == null)
		{
			myTerminalHelper.AddLine(out injectLine, TERMINAL_LINE_TYPE.HARD, " ");
		}
		targetedWPANetworks[key].TotalInjectionAmountAdded++;
		targetedWPANetworks[key].CurrentInjectionAmount++;
		string setText = "Injecting De-Auth request into '" + currentTargetedWPA.networkBSSID + "' - '" + currentTargetedWPA.networkName + "' #:" + targetedWPANetworks[key].TotalInjectionAmountAdded;
		injectLine.UpdateMyText(setText);
		if (targetedWPANetworks[key].CurrentInjectionAmount >= targetedWPANetworks[key].myWifiNetwork.networkMaxInjectionAmount)
		{
			GameManager.TimeSlinger.KillTimer(injectionLoopTimer);
			GameManager.ManagerSlinger.WifiManager.TakeNetworkOffLine(currentTargetedWPA);
			string myLine = injectLine.GetMyLine();
			injectLine.HardClear();
			injectLine = null;
			targetedWPANetworks[key].TotalInjectionAmountAdded = 0;
			targetedWPANetworks[key].CurrentInjectionAmount = 0;
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, myLine);
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, "Too many injection requests were sent at once. Network is now offline.");
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, "Remember to only send a little at a time to prevent the system from going offline.");
			mySkyBreakBehavior.addCMDInputLine();
		}
		else if (targetedWPANetworks[key].TotalInjectionAmountAdded >= targetedWPANetworks[key].myWifiNetwork.networkInjectionAmount)
		{
			targetedWPANetworks[key].SetInjectionReady(setValue: true);
			GameManager.TimeSlinger.KillTimer(injectionLoopTimer);
			string myLine2 = injectLine.GetMyLine();
			injectLine.HardClear();
			injectLine = null;
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, myLine2);
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, "Injection De-Auth injection was successful! Network is now ready to crack.");
			mySkyBreakBehavior.addCMDInputLine();
		}
		else
		{
			currentTargetedWPA.injectFireWindow = targetedWPANetworks[key].myWifiNetwork.networkInjectionCoolOffTime * ((myCracker == WIFI_SECURITY.WPA) ? 0.8f : 1f);
			currentTargetedWPA.injectTimeStamp = Time.time;
			currentTargetedWPA.injectWindowActive = true;
		}
	}

	private void AddInjectionLoopOver()
	{
		if (!myTerminalHelper.TerminalInput.Active && injectLine != null)
		{
			string myLine = injectLine.GetMyLine();
			injectLine.HardClear();
			injectLine = null;
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, myLine);
			mySkyBreakBehavior.addCMDInputLine();
		}
	}

	private void PerformCrack(string theCMD)
	{
		string setLine = string.Empty;
		bool flag = true;
		int result = -1;
		string[] array = theCMD.Split(new string[1] { " " }, StringSplitOptions.None);
		if (array.Length < 3)
		{
			flag = false;
			setLine = "Invalid usage of crack. Valid usage: crack <BSSID> <CH>";
		}
		if (flag)
		{
			if (GameManager.ManagerSlinger.WifiManager.CheckBSSID(array[1], out currentTargetedWPA, myCracker))
			{
				if (!int.TryParse(array[2], out result))
				{
					flag = false;
					setLine = "Invalid channel number.";
				}
			}
			else
			{
				flag = false;
				setLine = "Could not find a network with the BSSID Of '" + array[1] + "'";
			}
		}
		if (flag && currentTargetedWPA.networkChannel != (short)result)
		{
			flag = false;
			setLine = "Could not connect to network '" + currentTargetedWPA.networkBSSID + "' on channel:" + result;
		}
		if (flag)
		{
			string key = MagicSlinger.MD5It(currentTargetedWPA.networkName);
			if (targetedWPANetworks.ContainsKey(key))
			{
				if (!targetedWPANetworks[key].GetInjectionReady())
				{
					flag = false;
					setLine = "Can not crack network '" + currentTargetedWPA.networkBSSID + "'. No De-Auth request has been set!";
				}
			}
			else
			{
				flag = false;
				setLine = "Can not crack network '" + currentTargetedWPA.networkBSSID + "'. No De-Auth request has been set!";
			}
		}
		if (flag)
		{
			DoCrack();
			return;
		}
		mySkyBreakBehavior.SpitLastCMD(theCMD);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, setLine);
		myTerminalHelper.PushInputLineToBottom();
	}

	private void DoCrack()
	{
		anticheatWifiTag = currentTargetedWPA;
		myTerminalHelper.ClearInputLine();
		myTerminalHelper.ClearTerminal();
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, MagicSlinger.FluffString(" ", " ", 24) + "    skyBREAK 1.5.89");
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, string.Empty);
		string setLine = MagicSlinger.FluffString(" ", " ", 16) + "    [10 keys tested (500 k/s)]";
		myTerminalHelper.AddLine(out wpaCrackKeys, TERMINAL_LINE_TYPE.HARD, setLine);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, string.Empty);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.CRACK, "Master Key");
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.CRACK, string.Empty);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, string.Empty);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.CRACK, "Transient Key");
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.CRACK, string.Empty);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.CRACK, string.Empty);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.CRACK, string.Empty);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, string.Empty);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.CRACK, "EAPOL HMAC");
		float num = UnityEngine.Random.Range(minCrackTime, maxCrackTime);
		float setToValue = Mathf.Round(num * 500f);
		wpaUpdateKeysTested = GameManager.TweenSlinger.PlayDOSTweenLiner(0f, setToValue, num, UpdateKeysTested);
		GameManager.TimeSlinger.FireHardTimer(out wpaCrackedTimer, num, Cracked);
	}

	private void UpdateKeysTested(float setValue)
	{
		if (mySkyBreakBehavior.Window.activeSelf)
		{
			wpaCrackKeys.UpdateMyText(MagicSlinger.FluffString(" ", " ", 16) + "    [" + Mathf.RoundToInt(setValue) + " keys tested (500 k/s)]");
		}
	}

	private void Cracked()
	{
		if (mySkyBreakBehavior.Window.activeSelf)
		{
			currentTargetedWPA = anticheatWifiTag;
			GameManager.TweenSlinger.KillTween(wpaUpdateKeysTested);
			myTerminalHelper.KillCrackLines();
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, string.Empty);
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, MagicSlinger.FluffString(" ", " ", 11) + "'" + currentTargetedWPA.networkBSSID + "' - '" + currentTargetedWPA.networkName + "' HAS BEEN CRACKED!");
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, MagicSlinger.FluffString(" ", " ", 20) + "    PASSWORD FOUND! [" + currentTargetedWPA.networkPassword + "]");
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, string.Empty);
			mySkyBreakBehavior.addCMDInputLine();
			SteamSlinger.Ins.CrackWifiNetwork(currentTargetedWPA.GetHashCode());
		}
	}

	private float GetRandScanTime()
	{
		return UnityEngine.Random.Range(minScanTime, maxScanTime);
	}
}
