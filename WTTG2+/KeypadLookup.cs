using TMPro;
using UnityEngine;

public class KeypadLookup : MonoBehaviour
{
	public static KeypadLookup Ins;

	public GameObject button0;

	public GameObject button1;

	public GameObject button2;

	public GameObject button3;

	public GameObject button4;

	public GameObject button5;

	public GameObject button6;

	public GameObject button7;

	public GameObject button8;

	public GameObject button9;

	public GameObject enterButton;

	public GameObject lockedIcon;

	public GameObject unlockedIcon;

	public GameObject lightBar;

	public TMP_Text displayText;

	public Material displayRed;

	public Material displayGreen;

	public Material lightBarRed;

	public Material lightBarGreen;

	private void Awake()
	{
		Ins = this;
	}

	private void OnDestroy()
	{
		Ins = null;
	}

	public void Lock()
	{
		displayText.text = "LOCKED";
		displayText.GetComponent<MeshRenderer>().material = displayGreen;
		lockedIcon.SetActive(value: true);
		unlockedIcon.SetActive(value: false);
		lightBar.GetComponent<MeshRenderer>().material = lightBarGreen;
	}

	public void Unlock()
	{
		displayText.text = "UNLOCKED";
		displayText.GetComponent<MeshRenderer>().material = displayRed;
		lockedIcon.SetActive(value: false);
		unlockedIcon.SetActive(value: true);
		lightBar.GetComponent<MeshRenderer>().material = lightBarRed;
	}
}
