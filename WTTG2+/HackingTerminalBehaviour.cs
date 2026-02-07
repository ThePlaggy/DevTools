using System;
using System.Collections.Generic;
using UnityEngine;

public class HackingTerminalBehaviour : MonoBehaviour
{
	public delegate void VoidActions();

	public PasswordListDefinition TerminalDumpText;

	private bool doDumpActive;

	private float dumpTimeStamp;

	private int lineIndex;

	private List<TerminalLineObject> terminalDumpLineObjects = new List<TerminalLineObject>();

	private string[] terminalLines;

	public TerminalHelperBehavior TerminalHelper { get; private set; }

	public event VoidActions DumpDone;

	private void Awake()
	{
		terminalLines = TerminalDumpText.PasswordList.Split(new string[1] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
		TerminalHelper = GetComponent<TerminalHelperBehavior>();
	}

	private void Start()
	{
		buildDumpLines();
	}

	private void Update()
	{
		if (!doDumpActive || !(Time.time - dumpTimeStamp >= 0.0165f))
		{
			return;
		}
		dumpTimeStamp = Time.time;
		TerminalHelper.AddSoftLine(terminalDumpLineObjects[lineIndex]);
		TerminalHelper.UpdateTerminalContentScrollHeightHackingDump();
		lineIndex++;
		if (lineIndex >= terminalLines.Length - 1)
		{
			doDumpActive = false;
			if (this.DumpDone != null)
			{
				this.DumpDone();
			}
		}
	}

	public void DoDump()
	{
		dumpTimeStamp = Time.time;
		lineIndex = 0;
		doDumpActive = true;
	}

	private void buildDumpLines()
	{
		for (int i = 0; i < terminalLines.Length; i++)
		{
			terminalDumpLineObjects.Add(TerminalHelper.BuildSoftLine(TERMINAL_LINE_TYPE.HARD, terminalLines[i]));
		}
	}
}
