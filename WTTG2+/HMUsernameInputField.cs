using System;
using TMPro;
using UnityEngine;

public class HMUsernameInputField : MonoBehaviour
{
	public TMP_InputField inputField;

	public string username;

	private void Start()
	{
		TMP_InputField tMP_InputField = inputField;
		tMP_InputField.onValidateInput = (TMP_InputField.OnValidateInput)Delegate.Combine(tMP_InputField.onValidateInput, new TMP_InputField.OnValidateInput(OnValidateInput));
	}

	public void ValueChanged(string naskopipe)
	{
		username = naskopipe;
	}

	private char OnValidateInput(string text, int charIndex, char addedChar)
	{
		if (!IsValidCharacter(addedChar))
		{
			return '\0';
		}
		return addedChar;
	}

	private bool IsValidCharacter(char character)
	{
		return (character >= 'A' && character <= 'Z') || (character >= 'a' && character <= 'z') || (character >= '0' && character <= '9') || character == '_';
	}
}
