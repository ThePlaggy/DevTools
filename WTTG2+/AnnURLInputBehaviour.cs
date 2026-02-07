using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class AnnURLInputBehaviour : MonoBehaviour
{
	private InputField inputLine;

	private void Start()
	{
		inputLine = GetComponent<InputField>();
	}

	private void Update()
	{
		if (!GameManager.StageManager.GameIsLive)
		{
			return;
		}
		if (ControllerManager.Get<computerController>(GAME_CONTROLLER.COMPUTER).Active)
		{
			if (CrossPlatformInputManager.GetButtonDown("Return"))
			{
				InputURL(inputLine.text);
			}
		}
		else
		{
			inputLine.DeactivateInputField();
		}
	}

	public void InputURL(string setURL)
	{
		setURL = setURL.Replace("\n", string.Empty);
		if (setURL != string.Empty)
		{
			GameManager.BehaviourManager.AnnBehaviour.GotoURL(setURL);
		}
	}
}
