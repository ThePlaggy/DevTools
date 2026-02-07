using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextDocObject : MonoBehaviour
{
	private const float HEIGHT_BUFFER = 60f;

	public static float ImageX;

	public static float ImageY;

	public static Sprite unused;

	public static List<Sprite> DevImageCollection;

	public static int CurrentPNG;

	public static List<int> DevImageX;

	public static List<int> DevImageY;

	[SerializeField]
	private TextMeshProUGUI titleText;

	[SerializeField]
	private TMP_InputField docText;

	[SerializeField]
	private Font docFont;

	[SerializeField]
	private int fontSize;

	private BringWindowToFrontBehaviour myBringWindowToFront;

	private RectTransform myRT;

	static TextDocObject()
	{
		DevImageCollection = new List<Sprite>();
		DevImageX = new List<int>();
		DevImageY = new List<int>();
		CurrentPNG = -1;
	}

	public void SoftBuild()
	{
		myRT = GetComponent<RectTransform>();
		myBringWindowToFront = GetComponent<BringWindowToFrontBehaviour>();
		myBringWindowToFront.parentTrans = LookUp.DesktopUI.WINDOW_HOLDER;
		myRT.anchoredPosition = new Vector2(0f - myRT.sizeDelta.x, 0f);
	}

	public void Build(string SetTitle, string SetText)
	{
		float y = MagicSlinger.GetStringHeight(SetText, docFont, fontSize, myRT.sizeDelta) + 60f;
		myRT.sizeDelta = new Vector2(myRT.sizeDelta.x, y);
		float x = Mathf.Round(Random.Range(15f, (float)Screen.width - myRT.sizeDelta.x - 15f));
		float y2 = 0f - Mathf.Round(Random.Range(56f, (float)Screen.height - 40f - myRT.sizeDelta.y - 15f));
		titleText.SetText(SetTitle);
		docText.text = SetText;
		myRT.anchoredPosition = new Vector2(x, y2);
		if (Themes.selected <= THEME.TWR)
		{
			myRT.gameObject.GetComponent<Image>().sprite = ThemesLookUp.WTTG1TWR.baseapp;
		}
		if (TextDocIconObject.OpeningPNG)
		{
			TransformIntoImage(DevImageX[CurrentPNG], DevImageY[CurrentPNG]);
		}
	}

	public void BumpMe()
	{
		myBringWindowToFront.forceTap();
	}

	public void TransformIntoImage(float x = 500f, float y = 450f)
	{
		if (x > 1280f)
		{
			x = 1280f;
		}
		if (y > 720f)
		{
			y = 720f;
		}
		if (x < 100f)
		{
			x = 100f;
		}
		if (y < 150f)
		{
			y = 150f;
		}
		myRT.sizeDelta = new Vector2(x, y);
		base.transform.Find("TextHolder").gameObject.SetActive(value: false);
		GameObject gameObject = new GameObject("image");
		gameObject.transform.SetParent(myRT.transform);
		gameObject.AddComponent<RectTransform>();
		gameObject.AddComponent<Image>().sprite = DevImageCollection[CurrentPNG];
		gameObject.GetComponent<RectTransform>().localPosition = new Vector2(1f, -40f);
		gameObject.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
		gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(x - 2f, y - 40f - 2f);
		base.gameObject.AddComponent<GraphicsRayCasterCatcher>();
	}

	public static void ResetImageMargin()
	{
		TextDocIconObject.OpeningPNG = false;
		TextDocManager.imageCorresponding = 0;
		CurrentPNG = -1;
		DevImageCollection.Clear();
		DevImageX.Clear();
		DevImageY.Clear();
	}
}
