using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class NotesInputBehaviour : MonoBehaviour
{
	private InputField inputLine;

	private void Start()
	{
		inputLine = GetComponent<InputField>();
	}

	private void Update()
	{
		if (ControllerManager.Get<computerController>(GAME_CONTROLLER.COMPUTER).Active)
		{
			if (CrossPlatformInputManager.GetButtonDown("Return"))
			{
				InputNote(inputLine.text);
			}
		}
		else
		{
			inputLine.DeactivateInputField();
		}
	}

	public void InputNote(string setNote)
	{
		setNote = setNote.Replace("\n", string.Empty);
		if (setNote != string.Empty)
		{
			GameManager.BehaviourManager.NotesBehaviour.AddNote(setNote);
			inputLine.text = string.Empty;
		}
	}
}
