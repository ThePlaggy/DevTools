using System;

public class skyBreakBehavior : WindowBehaviour
{
	public ZeroDayProductDefinition wpaCracking;

	public ZeroDayProductDefinition wpa2Cracking;

	public SkyBreakTerminalBlockDefinition introBlock;

	public SkyBreakTerminalBlockDefinition modeHelpBlock;

	public SkyBreakTerminalBlockDefinition wepHelpBlock;

	public SkyBreakTerminalBlockDefinition wpaHelpBlock;

	public SkyBreakTerminalBlockDefinition wpa2HelpBlock;

	private SkyBreakActionState currentState;

	private skyBreakModeBehavior mySkyBreakModeBeahvior;

	private skyBreakWEPBehavior mySkyBreakWEPBehavior;

	private skyBreakWPA2Behavior mySkyBreakWPA2Behavior;

	private skyBreakWPABehavior mySkyBreakWPABehavior;

	private TerminalHelperBehavior myTerminalHelper;

	protected new void Awake()
	{
		base.Awake();
		TheSwan.mySkyBreak = this;
		myTerminalHelper = GetComponent<TerminalHelperBehavior>();
		mySkyBreakModeBeahvior = GetComponent<skyBreakModeBehavior>();
		mySkyBreakWEPBehavior = GetComponent<skyBreakWEPBehavior>();
		mySkyBreakWPABehavior = GetComponent<skyBreakWPABehavior>();
		mySkyBreakWPA2Behavior = GetComponent<skyBreakWPA2Behavior>();
		if (Window.GetComponent<BringWindowToFrontBehaviour>() != null)
		{
			Window.GetComponent<BringWindowToFrontBehaviour>().OnTap += SetCMDInputAsFocus;
		}
	}

	public void ProcessCMD(string theCMD)
	{
		string[] array = theCMD.Split(new string[1] { " " }, StringSplitOptions.None);
		array[0] = array[0].ToLower();
		bool flag = true;
		string text = array[0];
		if (text != null)
		{
			switch (text)
			{
			case "help":
				flag = false;
				SpitLastCMD(theCMD);
				processHelp();
				break;
			case "quit":
				flag = false;
				Window.SetActive(value: false);
				OnClose();
				break;
			case "exit":
				if (currentState == SkyBreakActionState.MODE)
				{
					flag = false;
					Window.SetActive(value: false);
					OnClose();
				}
				break;
			case "clear":
				flag = false;
				myTerminalHelper.ClearTerminal();
				myTerminalHelper.PushInputLineToBottom();
				break;
			}
		}
		if (flag)
		{
			switch (currentState)
			{
			case SkyBreakActionState.MODE:
				flag = mySkyBreakModeBeahvior.ProcessCMD(theCMD);
				break;
			case SkyBreakActionState.WEP:
				flag = mySkyBreakWEPBehavior.ProcessCMD(theCMD);
				break;
			case SkyBreakActionState.WPA:
				flag = mySkyBreakWPABehavior.ProcessCMD(theCMD);
				break;
			case SkyBreakActionState.WPA2:
				flag = mySkyBreakWPA2Behavior.ProcessCMD(theCMD);
				break;
			}
		}
		if (flag)
		{
			SpitLastCMD(theCMD);
			SpitInvalidCMD(theCMD);
			myTerminalHelper.PushInputLineToBottom();
		}
		myTerminalHelper.UpdateTerminalContentScrollHeight();
	}

	public void SetCMDInputAsFocus()
	{
		if (myTerminalHelper.TerminalInput != null)
		{
			myTerminalHelper.TerminalInput.inputLine.ActivateInputField();
		}
	}

	public void SpitLastCMD(string theCMD)
	{
		string setLine = string.Empty;
		switch (currentState)
		{
		case SkyBreakActionState.MODE:
			setLine = "skyBREAK > " + theCMD;
			break;
		case SkyBreakActionState.WEP:
			setLine = "skyBREAK > WEP > " + theCMD;
			break;
		case SkyBreakActionState.WPA:
			setLine = "skyBREAK > WPA > " + theCMD;
			break;
		case SkyBreakActionState.WPA2:
			setLine = "skyBREAK > WPA2 > " + theCMD;
			break;
		}
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, setLine);
	}

	public void addCMDInputLine()
	{
		if (Window.activeSelf)
		{
			switch (currentState)
			{
			case SkyBreakActionState.MODE:
				myTerminalHelper.AddInputLine(ProcessCMD, "skyBREAK >");
				break;
			case SkyBreakActionState.WEP:
				myTerminalHelper.AddInputLine(ProcessCMD, "skyBREAK > WEP >");
				break;
			case SkyBreakActionState.WPA:
				myTerminalHelper.AddInputLine(ProcessCMD, "skyBREAK > WPA >");
				break;
			case SkyBreakActionState.WPA2:
				myTerminalHelper.AddInputLine(ProcessCMD, "skyBREAK > WPA2 >");
				break;
			}
		}
	}

	public void UpdateTerminalInputLineMode()
	{
		if (Window.activeSelf && myTerminalHelper.TerminalInput != null)
		{
			switch (currentState)
			{
			case SkyBreakActionState.MODE:
				myTerminalHelper.TerminalInput.UpdateTitle("skyBREAK >");
				break;
			case SkyBreakActionState.WEP:
				myTerminalHelper.TerminalInput.UpdateTitle("skyBREAK > WEP >");
				break;
			case SkyBreakActionState.WPA:
				myTerminalHelper.TerminalInput.UpdateTitle("skyBREAK > WPA >");
				break;
			case SkyBreakActionState.WPA2:
				myTerminalHelper.TerminalInput.UpdateTitle("skyBREAK > WPA2 >");
				break;
			}
		}
	}

	public void SwitchStateMode(SkyBreakActionState setState)
	{
		currentState = setState;
		UpdateTerminalInputLineMode();
		myTerminalHelper.PushInputLineToBottom();
	}

	protected override void OnLaunch()
	{
		if (!Window.activeSelf)
		{
			currentState = SkyBreakActionState.MODE;
			showIntro();
		}
	}

	protected override void OnClose()
	{
		myTerminalHelper.FullClear();
		mySkyBreakWEPBehavior.CloseMeOut();
		mySkyBreakWPABehavior.CloseMeOut();
		mySkyBreakWPA2Behavior.CloseMeOut();
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

	private void showIntro()
	{
		if (introBlock != null)
		{
			for (int i = 0; i < introBlock.terminalLines.Count; i++)
			{
				myTerminalHelper.AddLine(introBlock.terminalLines[i]);
			}
		}
		float num = 2.5f;
		if (wpaCracking.productOwned)
		{
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.FADE, "        > WPA", 0.3f, num);
			num = 2.8f;
		}
		if (wpa2Cracking.productOwned)
		{
			myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.FADE, "        > WPA2", 0.3f, num);
			num = 3.1f;
		}
		GameManager.TimeSlinger.FireTimer(num, addCMDInputLine);
	}

	private void processTerminalBlock(SkyBreakTerminalBlockDefinition termBlock)
	{
		if (termBlock != null)
		{
			for (int i = 0; i < termBlock.terminalLines.Count; i++)
			{
				myTerminalHelper.AddLine(termBlock.terminalLines[i]);
			}
			myTerminalHelper.PushInputLineToBottom();
		}
	}

	private void processHelp()
	{
		switch (currentState)
		{
		case SkyBreakActionState.MODE:
			processTerminalBlock(modeHelpBlock);
			break;
		case SkyBreakActionState.WEP:
			processTerminalBlock(wepHelpBlock);
			break;
		case SkyBreakActionState.WPA:
			processTerminalBlock(wpaHelpBlock);
			break;
		case SkyBreakActionState.WPA2:
			processTerminalBlock(wpa2HelpBlock);
			break;
		}
	}

	private void SpitInvalidCMD(string theCMD)
	{
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, "'" + theCMD + "' is not a recognized valid command. Type HELP for more information.");
	}

	public void CauseSystemFailure()
	{
		myTerminalHelper.ClearInputLine();
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "System FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem Failure", 0.5f);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "System FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem Failure", 0.5f, 0.5f);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "System FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem Failure", 0.5f, 1f);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "System FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem Failure", 0.5f, 1.5f);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "System FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem Failure", 0.5f, 2f);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "System FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem Failure", 0.5f, 2.5f);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "System FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem Failure", 0.5f, 3f);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "System FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem Failure", 0.5f, 3.5f);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "System FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem Failure", 0.5f, 4f);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "System FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem Failure", 0.5f, 4.5f);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "System FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem Failure", 0.5f, 5f);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "System FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem Failure", 0.5f, 5.5f);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "System FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem Failure", 0.5f, 6f);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "System FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem Failure", 0.5f, 6.5f);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "System FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem Failure", 0.5f, 7f);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "System FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem Failure", 0.5f, 7.5f);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "System FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem Failure", 0.5f, 8f);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "System FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem Failure", 0.5f, 8.5f);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "System FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem Failure", 0.5f, 9f);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "System FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem Failure", 0.5f, 9.5f);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "System FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem Failure", 0.5f, 10f);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "System FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem Failure", 0.5f, 10.5f);
		myTerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "System FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem FailureSystem Failure", 0.5f, 11f);
		GameManager.TimeSlinger.FireTimer(11.4f, ResetSwan);
	}

	private void ResetSwan()
	{
		myTerminalHelper.ClearTerminal();
		addCMDInputLine();
	}
}
