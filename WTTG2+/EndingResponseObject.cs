using DG.Tweening;
using TMPro;
using UnityEngine;

public class EndingResponseObject : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI responseText;

	private CanvasGroup myCG;

	private RectTransform myRT;

	public void SoftBuild()
	{
		myCG = GetComponent<CanvasGroup>();
		myRT = GetComponent<RectTransform>();
		myCG.alpha = 0f;
		myRT.anchoredPosition = new Vector2(0f, -50f);
	}

	public void Build(EndingResponseDefinition TheResponse, int SetIndex, float SetY, float SetDisplayDelay)
	{
		myRT.anchoredPosition = new Vector2(0f, SetY);
		responseText.SetText(SetIndex + 1 + ". " + TheResponse.ResponseOption);
		DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 1f, 1f).SetEase(Ease.Linear).SetDelay(SetDisplayDelay);
	}

	public void Dismiss(float setDelay)
	{
		DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 0f, 0.5f).SetDelay(setDelay).OnComplete(delegate
		{
			myCG.alpha = 0f;
			myRT.anchoredPosition = new Vector2(0f, -50f);
		});
	}
}
