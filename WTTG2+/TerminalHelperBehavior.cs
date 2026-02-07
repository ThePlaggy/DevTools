using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerminalHelperBehavior : MonoBehaviour
{
	private const float TERMINAL_LINE_STARTY = 18f;

	public int STARTING_TERMINAL_LINE_POOL = 10;

	public GameObject terminalContentHolder;

	public GameObject terminalContentBox;

	public GameObject terminalLineObject;

	public GameObject terminalInputLineObject;

	private TerminalLineObject.VoidActions clearLineAction;

	private List<TerminalLineObject> currentTerminalLines = new List<TerminalLineObject>(10);

	private TerminalLineObject.VoidActions hardClearLineAction;

	private int lastCommandIndex;

	private List<string> lastCommands = new List<string>();

	private Action<string> myCallBackAction;

	private Vector2 terminalContentSize = Vector2.zero;

	private PooledStack<TerminalLineObject> terminalLineObjectPool;

	public TerminalInputLineObject TerminalInput { get; private set; }

	private void Awake()
	{
		terminalLineObjectPool = new PooledStack<TerminalLineObject>(delegate
		{
			TerminalLineObject component = UnityEngine.Object.Instantiate(terminalLineObject, terminalContentBox.GetComponent<RectTransform>()).GetComponent<TerminalLineObject>();
			component.SoftBuild();
			return component;
		}, STARTING_TERMINAL_LINE_POOL);
		clearLineAction = clearLine;
		hardClearLineAction = hardClearLine;
		GameManager.StageManager.Stage += stageMe;
	}

	private void Update()
	{
		if (TerminalInput != null && TerminalInput.GetComponent<TerminalInputLineObject>().inputLine.isFocused)
		{
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				getLastCommand();
			}
			else if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				getNextCommand();
			}
		}
	}

	public void FullClear()
	{
		TerminalInput.Clear();
		ClearTerminal();
	}

	public TerminalLineObject BuildSoftLine(TERMINAL_LINE_TYPE LineType, string SetLine, float SetLength = 0f, float SetDelay = 0f)
	{
		TerminalLineObject terminalLineObject = terminalLineObjectPool.Pop();
		terminalLineObject.SoftLine = true;
		terminalLineObject.Build(LineType, SetLine, SetLength, SetDelay);
		return terminalLineObject;
	}

	public void AddSoftLine(TerminalLineObject SoftLine)
	{
		SoftLine.Move(getLatestY());
		currentTerminalLines.Add(SoftLine);
	}

	public void AddLine(TerminalLineDefinition TheLine)
	{
		AddLine(TheLine.terminalLineType, TheLine.terminalText, TheLine.terminalAniLength, TheLine.terminalDelayAmount);
	}

	public void AddLine(TERMINAL_LINE_TYPE LineType, string SetLine, float SetLength = 0f, float SetDelay = 0f)
	{
		TerminalLineObject terminalLineObject = terminalLineObjectPool.Pop();
		terminalLineObject.ClearLine += clearLine;
		terminalLineObject.Build(LineType, SetLine, SetLength, SetDelay);
		terminalLineObject.Move(getLatestY());
		currentTerminalLines.Add(terminalLineObject);
	}

	public void AddLine(out TerminalLineObject ReturnTerminalLineObject, TERMINAL_LINE_TYPE LineType, string SetLine, float SetLength = 0f, float SetDelay = 0f)
	{
		ReturnTerminalLineObject = terminalLineObjectPool.Pop();
		ReturnTerminalLineObject.ClearLine += clearLine;
		ReturnTerminalLineObject.HardClearLine += hardClearLine;
		ReturnTerminalLineObject.Build(LineType, SetLine, SetLength, SetDelay);
		ReturnTerminalLineObject.Move(getLatestY());
		currentTerminalLines.Add(ReturnTerminalLineObject);
	}

	public void AddInputLine(Action<string> SetCallBack, string SetTitle = "")
	{
		myCallBackAction = SetCallBack;
		if (SetTitle != string.Empty)
		{
			TerminalInput.UpdateTitle(SetTitle);
		}
		TerminalInput.inputLine.ActivateInputField();
		TerminalInput.Move(getLatestY());
		TerminalInput.Active = true;
	}

	public void ClearTerminal()
	{
		for (int i = 0; i < currentTerminalLines.Count; i++)
		{
			currentTerminalLines[i].Clear();
		}
		currentTerminalLines.Clear();
		terminalContentSize.x = terminalContentBox.GetComponent<RectTransform>().sizeDelta.x;
		terminalContentSize.y = 300f;
		terminalContentBox.GetComponent<RectTransform>().sizeDelta = terminalContentSize;
		terminalContentHolder.GetComponent<ScrollRect>().normalizedPosition = Vector2.zero;
	}

	public void ClearInputLine()
	{
		TerminalInput.Clear();
	}

	public void PushInputLineToBottom()
	{
		TerminalInput.Move(getLatestY());
	}

	public void UpdateTerminalContentScrollHeight()
	{
		terminalContentSize.x = terminalContentBox.GetComponent<RectTransform>().sizeDelta.x;
		terminalContentSize.y = Mathf.Abs(getLatestY());
		terminalContentSize.y = terminalContentSize.y + 20f + 20f;
		terminalContentBox.GetComponent<RectTransform>().sizeDelta = terminalContentSize;
		terminalContentHolder.GetComponent<ScrollRect>().normalizedPosition = Vector2.zero;
	}

	public void UpdateTerminalContentScrollHeightHackingDump()
	{
		terminalContentSize.x = terminalContentBox.GetComponent<RectTransform>().sizeDelta.x;
		terminalContentSize.y = Mathf.Abs(getLatestY());
		terminalContentSize.y = terminalContentSize.y + 20f - 25f;
		terminalContentBox.GetComponent<RectTransform>().sizeDelta = terminalContentSize;
		terminalContentHolder.GetComponent<ScrollRect>().normalizedPosition = Vector2.zero;
	}

	public void KillCrackLines()
	{
		for (int i = 0; i < currentTerminalLines.Count; i++)
		{
			currentTerminalLines[i].KillCrackLine();
		}
	}

	private void clearLine(TerminalLineObject TheLine)
	{
		TheLine.ClearLine -= clearLine;
		TheLine.HardClearLine -= hardClearLine;
		terminalLineObjectPool.Push(TheLine);
	}

	private void hardClearLine(TerminalLineObject TheLine)
	{
		TheLine.ClearLine -= clearLine;
		TheLine.HardClearLine -= hardClearLine;
		terminalLineObjectPool.Push(TheLine);
		currentTerminalLines.Remove(TheLine);
	}

	private float getLatestY()
	{
		return 0f - (18f + (float)currentTerminalLines.Count * 20f);
	}

	private void processCMD(string theCMD)
	{
		if (lastCommands.Count >= 1)
		{
			if (lastCommands[lastCommands.Count - 1] != theCMD)
			{
				lastCommands.Add(theCMD);
			}
			lastCommandIndex = lastCommands.Count - 1;
		}
		else
		{
			lastCommands.Add(theCMD);
			lastCommandIndex = 0;
		}
		myCallBackAction(theCMD);
	}

	private void getLastCommand()
	{
		if (lastCommandIndex >= 0 && lastCommands.Count != 0 && lastCommands[lastCommandIndex] != null)
		{
			TerminalInput.inputLine.text = lastCommands[lastCommandIndex];
			TerminalInput.inputLine.MoveTextEnd(shift: false);
			lastCommandIndex--;
			if (lastCommandIndex < 0)
			{
				lastCommandIndex = 0;
			}
		}
	}

	private void getNextCommand()
	{
		int num = lastCommandIndex + 1;
		if (num == lastCommands.Count)
		{
			TerminalInput.inputLine.text = string.Empty;
		}
		else if (num < lastCommands.Count && lastCommands[num] != null)
		{
			lastCommandIndex = num;
			TerminalInput.inputLine.text = lastCommands[lastCommandIndex];
			TerminalInput.inputLine.MoveTextEnd(shift: false);
		}
	}

	private void stageMe()
	{
		GameManager.StageManager.Stage -= stageMe;
		TerminalInput = UnityEngine.Object.Instantiate(terminalInputLineObject, terminalContentBox.GetComponent<RectTransform>()).GetComponent<TerminalInputLineObject>();
		TerminalInput.SoftBuild(processCMD);
	}
}
