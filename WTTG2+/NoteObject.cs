using UnityEngine;
using UnityEngine.UI;

public class NoteObject : MonoBehaviour
{
	public InputField noteField;

	public Text noteText;

	private Vector2 extends = Vector2.zero;

	private Vector2 mySize = Vector2.zero;

	public void BuildMe(string setText, SOFTWARE_PRODUCTS SetType)
	{
		extends.x = WindowManager.Get(SetType).Window.GetComponent<RectTransform>().sizeDelta.x - 12f;
		extends.y = WindowManager.Get(SetType).Window.GetComponent<RectTransform>().sizeDelta.y - 89f;
		noteField.text = setText;
		mySize.x = GetComponent<RectTransform>().sizeDelta.x;
		mySize.y = MagicSlinger.GetStringHeight(setText, noteText.font, noteText.fontSize, extends) + 10f;
		GetComponent<RectTransform>().sizeDelta = mySize;
	}
}
