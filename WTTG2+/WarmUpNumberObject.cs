using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WarmUpNumberObject : MonoBehaviour
{
	private static Vector3 myDownScale = new Vector3(0.1f, 0.1f, 1f);

	public AudioFileDefinition MySFX;

	private Tweener fadeOutTween;

	private CanvasGroup myCG;

	private Vector3 myDefaultScale = Vector3.one;

	private RectTransform myRT;

	private Vector2 myStartPOS = Vector2.zero;

	private Text myText;

	private Tweener scaleDownTween;

	private void Awake()
	{
		myCG = GetComponent<CanvasGroup>();
		myRT = GetComponent<RectTransform>();
		myText = GetComponent<Text>();
		myCG.alpha = 0f;
		myStartPOS.x = 0f;
		myStartPOS.y = 0f - (myRT.sizeDelta.y / 2f + 5f);
		scaleDownTween = DOTween.To(() => myRT.localScale, delegate(Vector3 x)
		{
			myRT.localScale = x;
		}, myDownScale, 0.6f).SetEase(Ease.InQuint);
		scaleDownTween.Pause();
		scaleDownTween.SetAutoKill(autoKillOnCompletion: false);
		fadeOutTween = DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 0f, 0.6f).SetEase(Ease.Linear).OnComplete(delegate
		{
			myCG.alpha = 0f;
			myRT.localScale = myDefaultScale;
		});
		fadeOutTween.Pause();
		fadeOutTween.SetAutoKill(autoKillOnCompletion: false);
	}

	public void FireMe(string setNumber)
	{
		if (!HackingTimerObject.IsWTTG1Hack)
		{
			GameManager.AudioSlinger.PlaySound(MySFX);
		}
		myRT.anchoredPosition = myStartPOS;
		myText.text = setNumber;
		myCG.alpha = 1f;
		scaleDownTween.Restart();
		fadeOutTween.Restart();
	}
}
