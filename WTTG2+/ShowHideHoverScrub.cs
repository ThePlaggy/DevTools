using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ShowHideHoverScrub : MonoBehaviour
{
	private CanvasGroup myCG;

	private void Awake()
	{
		myCG = GetComponent<CanvasGroup>();
		myCG.alpha = 0f;
	}

	public void ShowMe()
	{
		myCG.alpha = 1f;
	}

	public void HideMe()
	{
		myCG.alpha = 0f;
	}
}
