using System;
using UnityEngine;

public class skyBreakModeBehavior : MonoBehaviour
{
	private skyBreakBehavior mySkyBreakBehavior;

	private TerminalHelperBehavior myTerminalHelper;

	private void Awake()
	{
		mySkyBreakBehavior = GetComponent<skyBreakBehavior>();
		myTerminalHelper = GetComponent<TerminalHelperBehavior>();
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
			case "wep":
				result = false;
				mySkyBreakBehavior.SpitLastCMD(theCMD);
				ModeSwitchToWEP();
				break;
			case "wpa":
				result = false;
				mySkyBreakBehavior.SpitLastCMD(theCMD);
				ModeSwitchToWPA();
				break;
			case "wpa2":
				result = false;
				mySkyBreakBehavior.SpitLastCMD(theCMD);
				ModeSwitchToWPA2();
				break;
			case "list":
				result = false;
				mySkyBreakBehavior.SpitLastCMD(theCMD);
				ModeListCrackers();
				break;
			}
		}
		return result;
	}

	private void ModeListCrackers()
	{
		myTerminalHelper.ClearInputLine();
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "You have the following crackers installed:", 0.4f);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.FADE, "    > WEP", 0.25f, 0.5f);
		float num = 0.75f;
		if (mySkyBreakBehavior.wpaCracking.productOwned)
		{
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.FADE, "    > WPA", 0.25f, num);
			num = 1f;
		}
		if (mySkyBreakBehavior.wpa2Cracking.productOwned)
		{
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.FADE, "    > WPA2", 0.25f, num);
			num = 1.25f;
		}
		GameManager.TimeSlinger.FireTimer(num, mySkyBreakBehavior.addCMDInputLine);
	}

	private void ModeSwitchToWEP()
	{
		mySkyBreakBehavior.SwitchStateMode(SkyBreakActionState.WEP);
	}

	private void ModeSwitchToWPA()
	{
		if (mySkyBreakBehavior.wpaCracking.productOwned)
		{
			mySkyBreakBehavior.SwitchStateMode(SkyBreakActionState.WPA);
			return;
		}
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, "You do not have the WPA cracker installed!");
		myTerminalHelper.PushInputLineToBottom();
	}

	private void ModeSwitchToWPA2()
	{
		if (mySkyBreakBehavior.wpa2Cracking.productOwned)
		{
			mySkyBreakBehavior.SwitchStateMode(SkyBreakActionState.WPA2);
			return;
		}
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, "You do not have the WPA2 cracker installed!");
		myTerminalHelper.PushInputLineToBottom();
	}
}
