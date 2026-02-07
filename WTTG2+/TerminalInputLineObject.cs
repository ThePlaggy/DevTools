using System;
using UnityEngine;
using UnityEngine.UI;

public class TerminalInputLineObject : MonoBehaviour
{
	public GameObject titleText;

	public InputField inputLine;

	public bool Active;

	private Action<string> myCallBackAction;

	private Vector2 myPOS = Vector2.zero;

	private RectTransform myRT;

	private Vector2 mySize = new Vector2(0f, 20f);

	private Vector2 startPOS = new Vector2(0f, 10f);

	private void OnDestroy()
	{
		myCallBackAction = null;
		computerController.Ins.LeaveEvents.Event -= looseFocus;
	}

	public void SoftBuild(Action<string> SetAction)
	{
		myRT = GetComponent<RectTransform>();
		InputField.OnChangeEvent onChangeEvent = new InputField.OnChangeEvent();
		onChangeEvent.AddListener(veryifyInput);
		inputLine.onValueChanged = onChangeEvent;
		if (computerController.Ins != null)
		{
			computerController.Ins.LeaveEvents.Event += looseFocus;
		}
		myCallBackAction = SetAction;
		myRT.anchoredPosition = startPOS;
	}

	public void Move(float SetY)
	{
		myPOS.y = SetY;
		myRT.anchoredPosition = myPOS;
	}

	public void Clear()
	{
		Active = false;
		inputLine.text = string.Empty;
		titleText.GetComponent<Text>().text = string.Empty;
		myRT.anchoredPosition = startPOS;
	}

	public void InputCMD(string setCMD)
	{
		setCMD = setCMD.Replace("\n", string.Empty);
		if (setCMD != string.Empty && myCallBackAction != null)
		{
			myCallBackAction(setCMD);
		}
	}

	public void UpdateTitle(string titleString)
	{
		float stringWidth = MagicSlinger.GetStringWidth(titleString, titleText.GetComponent<Text>().font, titleText.GetComponent<Text>().fontSize, titleText.GetComponent<RectTransform>().sizeDelta);
		titleText.GetComponent<Text>().text = titleString;
		titleText.GetComponent<RectTransform>().sizeDelta = new Vector2(stringWidth, titleText.GetComponent<RectTransform>().sizeDelta.y);
		inputLine.GetComponent<RectTransform>().anchoredPosition = new Vector2(titleText.GetComponent<RectTransform>().anchoredPosition.x + stringWidth + 8f, inputLine.GetComponent<RectTransform>().anchoredPosition.y);
		inputLine.GetComponent<RectTransform>().sizeDelta = mySize;
	}

	private void veryifyInput(string setInput)
	{
		if (setInput.Contains("\n"))
		{
			InputCMD(setInput);
			inputLine.text = string.Empty;
		}
	}

	private void looseFocus()
	{
		if (inputLine != null)
		{
			inputLine.DeactivateInputField();
		}
	}
}
