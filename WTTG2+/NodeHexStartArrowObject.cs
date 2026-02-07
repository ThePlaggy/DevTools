using DG.Tweening;
using UnityEngine;

public class NodeHexStartArrowObject : MonoBehaviour
{
	public RectTransform DefaultParent;

	private Tweener hideMeTween;

	private CanvasGroup myCG;

	private RectTransform myRT;

	private Tweener presentMeTween1;

	private Tweener presentMeTween2;

	private void Awake()
	{
		myCG = GetComponent<CanvasGroup>();
		myRT = GetComponent<RectTransform>();
		presentMeTween1 = DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 1f, 0.45f).SetEase(Ease.Linear);
		presentMeTween1.Pause();
		presentMeTween1.SetAutoKill(autoKillOnCompletion: false);
		presentMeTween2 = DOTween.To(() => myRT.anchoredPosition, delegate(Vector2 x)
		{
			myRT.anchoredPosition = x;
		}, new Vector2(-35f, 0f), 0.15f).SetEase(Ease.OutSine);
		presentMeTween2.Pause();
		presentMeTween2.SetAutoKill(autoKillOnCompletion: false);
		hideMeTween = DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 0f, 0.25f).SetEase(Ease.Linear).OnComplete(delegate
		{
			myRT.SetParent(DefaultParent);
			myRT.anchoredPosition = Vector2.zero;
		});
		hideMeTween.Pause();
		hideMeTween.SetAutoKill(autoKillOnCompletion: false);
	}

	public void Clear()
	{
		hideMeTween.Restart();
	}

	public void Present(RectTransform NewParent)
	{
		myRT.SetParent(NewParent);
		myRT.anchoredPosition = Vector2.zero;
		presentMeTween1.Restart();
		presentMeTween2.Restart();
	}
}
