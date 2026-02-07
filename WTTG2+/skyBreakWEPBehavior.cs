using System;
using System.Collections.Generic;
using UnityEngine;

public class skyBreakWEPBehavior : MonoBehaviour
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

	private WifiNetworkDefinition currentTargetedWEP;

	private int currentTargetedWEPMaxPort;

	private skyBreakBehavior mySkyBreakBehavior;

	private TerminalHelperBehavior myTerminalHelper;

	private DOSTween skyBREAKWepPortProbe;

	private Timer wepCrackedTimer;

	private TerminalLineObject wepCrackKeys;

	private TerminalLineObject wepProbeLine;

	private DOSTween wepUpdateKeysTested;

	private void Start()
	{
		PrepMe();
	}

	public void CloseMeOut()
	{
		currentTargetedWEP = null;
		GameManager.TweenSlinger.KillTween(skyBREAKWepPortProbe);
		GameManager.TweenSlinger.KillTween(wepUpdateKeysTested);
		GameManager.TimeSlinger.KillTimer(wepCrackedTimer);
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
			case "probe":
				result = false;
				PerformProbe(theCMD);
				break;
			case "crack":
				result = false;
				PerformCrack(theCMD);
				break;
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
			List<WifiNetworkDefinition> secureNetworks = GameManager.ManagerSlinger.WifiManager.GetSecureNetworks(WIFI_SECURITY.WEP);
			float num = 0.3f;
			string setLine = MagicSlinger.FluffString("ESSID", " ", 20) + "    " + MagicSlinger.FluffString("BSSID", " ", 20) + "    " + MagicSlinger.FluffString("CH", " ", 10) + "    " + MagicSlinger.FluffString("PING", " ", 10) + "    SIG";
			myTerminalHelper.ClearTerminal();
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, "Scanned WEP Network Results");
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.FADE, setLine, 0.3f);
			for (int i = 0; i < secureNetworks.Count; i++)
			{
				string setLine2 = MagicSlinger.FluffString(secureNetworks[i].networkName, " ", 20) + "    " + MagicSlinger.FluffString(secureNetworks[i].networkBSSID, " ", 20) + "    " + MagicSlinger.FluffString(secureNetworks[i].networkChannel.ToString(), " ", 10) + "    " + MagicSlinger.FluffString(secureNetworks[i].networkPower.ToString(), " ", 10) + "    " + MagicSlinger.GetWifiSiginalType(secureNetworks[i].networkSignal);
				myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.FADE, setLine2, 0.2f, num);
				num += 0.2f;
			}
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, string.Empty);
			GameManager.TimeSlinger.FireTimer(num, mySkyBreakBehavior.addCMDInputLine);
		}
	}

	private void PerformProbe(string theCMD)
	{
		string setLine = string.Empty;
		bool flag = true;
		int result = -1;
		int result2 = -1;
		string[] array = theCMD.Split(new string[1] { " " }, StringSplitOptions.None);
		if (array.Length < 4)
		{
			flag = false;
			setLine = "Invalid usage of probe. Valid usage: probe <BSSID> <Port Start> <Port End>";
		}
		if (flag)
		{
			if (GameManager.ManagerSlinger.WifiManager.CheckBSSID(array[1], out currentTargetedWEP, WIFI_SECURITY.WEP))
			{
				if (!int.TryParse(array[2], out result))
				{
					flag = false;
					setLine = "Invalid port number. Valid port range is 1 - 1000.";
				}
				if (!int.TryParse(array[3], out result2))
				{
					flag = false;
					setLine = "Invalid port number. Valid port range is 1 - 1000.";
				}
			}
			else
			{
				flag = false;
				setLine = "Could not find a network with the BSSID Of '" + array[1] + "'";
			}
		}
		if (flag && (result > 1000 || result < 1 || result2 > 1000 || result2 < 1))
		{
			flag = false;
			setLine = "Invalid port number. Valid port range is 1 - 1000.";
		}
		if (flag && result2 < result)
		{
			flag = false;
			setLine = "Invalid port entry. Ending port can not be greater then starting port.";
		}
		if (flag)
		{
			mySkyBreakBehavior.SpitLastCMD(theCMD);
			DoProbe(result, result2);
		}
		else
		{
			mySkyBreakBehavior.SpitLastCMD(theCMD);
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, setLine);
			myTerminalHelper.PushInputLineToBottom();
		}
	}

	private void DoProbe(int targetStartingPort, int targetEndingPort)
	{
		myTerminalHelper.ClearInputLine();
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, "Targeting '" + currentTargetedWEP.networkBSSID + "' - '" + currentTargetedWEP.networkName + "'");
		myTerminalHelper.AddLine(out wepProbeLine, TERMINAL_LINE_TYPE.HARD, "Probing port: " + targetStartingPort);
		currentTargetedWEPMaxPort = targetEndingPort;
		skyBREAKWepPortProbe = GameManager.TweenSlinger.PlayDOSTweenLiner(targetStartingPort, targetEndingPort, (float)(targetEndingPort - targetStartingPort) * 0.1f, UpdateProbeLine);
	}

	private void UpdateProbeLine(float setValue)
	{
		int num = Mathf.RoundToInt(setValue);
		if ((short)num == currentTargetedWEP.networkOpenPort)
		{
			GameManager.TweenSlinger.KillTween(skyBREAKWepPortProbe);
			ProbeFoundPort();
			return;
		}
		if (wepProbeLine != null)
		{
			wepProbeLine.UpdateMyText("Probing port: " + num);
		}
		if (num == currentTargetedWEPMaxPort)
		{
			GameManager.TweenSlinger.KillTween(skyBREAKWepPortProbe);
			ProbeNoPortFound();
		}
	}

	private void ProbeFoundPort()
	{
		if (mySkyBreakBehavior.Window.activeSelf)
		{
			wepProbeLine.HardClear();
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, "FOUND OPEN PORT! ON '" + currentTargetedWEP.networkBSSID + "' - '" + currentTargetedWEP.networkName + "'");
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, "Open Port #:" + currentTargetedWEP.networkOpenPort);
			mySkyBreakBehavior.addCMDInputLine();
		}
	}

	private void ProbeNoPortFound()
	{
		if (mySkyBreakBehavior.Window.activeSelf)
		{
			wepProbeLine.HardClear();
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, "Could not find an open port on '" + currentTargetedWEP.networkBSSID + "' - '" + currentTargetedWEP.networkName + "'");
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, "Try a differnt port range.");
			mySkyBreakBehavior.addCMDInputLine();
		}
	}

	private void PerformCrack(string theCMD)
	{
		string setLine = string.Empty;
		bool flag = true;
		int result = -1;
		int result2 = 1;
		string[] array = theCMD.Split(new string[1] { " " }, StringSplitOptions.None);
		if (array.Length < 4)
		{
			flag = false;
			setLine = "Invalid usage of crack. Valid usage: crack <BSSID> <CH> <PORT>";
		}
		if (flag)
		{
			if (GameManager.ManagerSlinger.WifiManager.CheckBSSID(array[1], out currentTargetedWEP, WIFI_SECURITY.WEP))
			{
				if (!int.TryParse(array[2], out result))
				{
					flag = false;
					setLine = "Invalid channel number.";
				}
				if (!int.TryParse(array[3], out result2))
				{
					flag = false;
					setLine = "Invalid port number.";
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
			if (currentTargetedWEP.networkChannel != (short)result)
			{
				flag = false;
				setLine = "Could not connect to network '" + currentTargetedWEP.networkBSSID + "' on channel:" + result;
			}
			if (currentTargetedWEP.networkOpenPort != (short)result2)
			{
				flag = false;
				setLine = "Could not connect to network '" + currentTargetedWEP.networkBSSID + "' with port:" + result2;
			}
			if (result2 < 1 || result2 > 1000)
			{
				flag = false;
				setLine = "Invalid port number. Valid port range is 1 - 1000.";
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
		anticheatWifiTag = currentTargetedWEP;
		myTerminalHelper.ClearInputLine();
		myTerminalHelper.ClearTerminal();
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, MagicSlinger.FluffString(" ", " ", 24) + "    skyBREAK 1.5.89");
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, string.Empty);
		string setLine = MagicSlinger.FluffString(" ", " ", 16) + "    [10 keys tested (500 k/s)]";
		myTerminalHelper.AddLine(out wepCrackKeys, TERMINAL_LINE_TYPE.HARD, setLine);
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
		wepUpdateKeysTested = GameManager.TweenSlinger.PlayDOSTweenLiner(0f, setToValue, num, UpdateKeysTested);
		GameManager.TimeSlinger.FireHardTimer(out wepCrackedTimer, num, Cracked);
	}

	private void UpdateKeysTested(float setValue)
	{
		if (mySkyBreakBehavior.Window.activeSelf)
		{
			wepCrackKeys.UpdateMyText(MagicSlinger.FluffString(" ", " ", 16) + "    [" + Mathf.RoundToInt(setValue) + " keys tested (500 k/s)]");
		}
	}

	private void Cracked()
	{
		if (mySkyBreakBehavior.Window.activeSelf)
		{
			currentTargetedWEP = anticheatWifiTag;
			GameManager.TweenSlinger.KillTween(wepUpdateKeysTested);
			myTerminalHelper.KillCrackLines();
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, string.Empty);
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, MagicSlinger.FluffString(" ", " ", 11) + "'" + currentTargetedWEP.networkBSSID + "' - '" + currentTargetedWEP.networkName + "' HAS BEEN CRACKED!");
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, MagicSlinger.FluffString(" ", " ", 20) + "    PASSWORD FOUND! [" + currentTargetedWEP.networkPassword + "]");
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, string.Empty);
			mySkyBreakBehavior.addCMDInputLine();
			SteamSlinger.Ins.CrackWifiNetwork(currentTargetedWEP.GetHashCode());
		}
	}

	private float GetRandScanTime()
	{
		return UnityEngine.Random.Range(minScanTime, maxScanTime);
	}

	private void PrepMe()
	{
		mySkyBreakBehavior = GetComponent<skyBreakBehavior>();
		myTerminalHelper = GetComponent<TerminalHelperBehavior>();
	}
}
