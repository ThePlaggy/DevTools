using TMPro;
using UnityEngine;

public class OverrideInformationWindow : MonoBehaviour
{
	public TextMeshProUGUI MyText;

	public CanvasGroup MyTextCG;

	public CanvasGroup MyBGCG;

	private string DefaultText;

	private void Awake()
	{
		DefaultText = MyText.text;
	}

	public void MeReset()
	{
		MyText.text = DefaultText;
	}

	public void UpdateMe(string a0, string a1, string a2, string a3, bool b1, bool b2)
	{
		MeReset();
		string newValue = ((a0 == "fast") ? "green" : "red");
		string newValue2 = ((a2 == "enabled") ? "green" : "red");
		MyText.text = MyText.text.Replace("{4}", newValue);
		MyText.text = MyText.text.Replace("{5}", newValue2);
		string[] array = MyText.text.Split('\n');
		if (!b1)
		{
			array[1] = "* INACTIVE";
		}
		if (!b2)
		{
			array[4] = "* INACTIVE";
		}
		MyText.text = string.Join("\n", array);
		MyText.text = MyText.text.Replace("{0}", a0);
		MyText.text = MyText.text.Replace("{1}", a1);
		MyText.text = MyText.text.Replace("{2}", a2);
		MyText.text = MyText.text.Replace("{3}", a3);
	}
}
