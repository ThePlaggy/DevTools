using UnityEngine;

public class resizeScreenScrub : MonoBehaviour
{
	public bool resizeWidthToScreen;

	public bool resizeHeightToScreen;

	public bool resizeWidthToScreenPer;

	public float screenWidthPer;

	public bool clampWidth;

	public float clampWidthPX;

	public bool resizeHeightToScreenPer;

	public float screenHeightPer;

	public bool clampHeight;

	public float clampHeightPX;

	private RectTransform myRectTR;

	private void Start()
	{
		myRectTR = GetComponent<RectTransform>();
		resizeMe();
	}

	private void resizeMe()
	{
		if (resizeWidthToScreen)
		{
			myRectTR.sizeDelta = new Vector2(Screen.width, myRectTR.sizeDelta.y);
		}
		if (resizeHeightToScreen)
		{
			myRectTR.sizeDelta = new Vector2(myRectTR.sizeDelta.x, Screen.height);
		}
		if (resizeWidthToScreenPer)
		{
			float num = Mathf.Round(MagicSlinger.GetScreenWidthPXByPerc(screenWidthPer));
			if (clampWidth && num >= clampWidthPX)
			{
				num = clampWidthPX;
			}
			myRectTR.sizeDelta = new Vector2(num, myRectTR.sizeDelta.y);
		}
		if (resizeHeightToScreenPer)
		{
			float num2 = Mathf.Round(MagicSlinger.GetScreenHeightPXByPerc(screenHeightPer));
			if (clampHeight && num2 >= clampHeightPX)
			{
				num2 = clampHeightPX;
			}
			myRectTR.sizeDelta = new Vector2(myRectTR.sizeDelta.x, num2);
		}
	}
}
