using DG.Tweening;
using UnityEngine;

public class StackPusherGridPoperObject : MonoBehaviour
{
	public GameObject DefaultBGIMG;

	public GameObject PopArrowIMG;

	public GameObject PopHoverIMG;

	private CanvasGroup arrowCG;

	private Vector2 centerPOS = Vector2.zero;

	private Tweener clearTween;

	private RectTransform defaultParent;

	private CanvasGroup hoverCG;

	private Tweener mouseEnterTween;

	private Tweener mouseExitTween;

	private CanvasGroup myCG;

	private RectTransform myRT;

	private Tweener presentSelf;

	private Tweener setActiveTween;

	private Tweener setInActiveTween;

	private void Awake()
	{
		myRT = GetComponent<RectTransform>();
		myCG = GetComponent<CanvasGroup>();
		arrowCG = PopArrowIMG.GetComponent<CanvasGroup>();
		hoverCG = PopHoverIMG.GetComponent<CanvasGroup>();
		defaultParent = (RectTransform)myRT.parent;
		presentSelf = DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 1f, 0.15f).SetEase(Ease.Linear);
		presentSelf.Pause();
		presentSelf.SetAutoKill(autoKillOnCompletion: false);
		setActiveTween = DOTween.To(() => arrowCG.alpha, delegate(float x)
		{
			arrowCG.alpha = x;
		}, 1f, 0.15f).SetEase(Ease.Linear);
		setActiveTween.Pause();
		setActiveTween.SetAutoKill(autoKillOnCompletion: false);
		setInActiveTween = DOTween.To(() => arrowCG.alpha, delegate(float x)
		{
			arrowCG.alpha = x;
		}, 0.1f, 0.15f).SetEase(Ease.Linear);
		setInActiveTween.Pause();
		setInActiveTween.SetAutoKill(autoKillOnCompletion: false);
		mouseEnterTween = DOTween.To(() => hoverCG.alpha, delegate(float x)
		{
			hoverCG.alpha = x;
		}, 1f, 0.15f).SetEase(Ease.Linear);
		mouseEnterTween.Pause();
		mouseEnterTween.SetAutoKill(autoKillOnCompletion: false);
		mouseExitTween = DOTween.To(() => hoverCG.alpha, delegate(float x)
		{
			hoverCG.alpha = x;
		}, 0f, 0.15f).SetEase(Ease.Linear);
		mouseExitTween.Pause();
		mouseExitTween.SetAutoKill(autoKillOnCompletion: false);
		clearTween = DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 0f, 0.15f).SetEase(Ease.Linear).OnComplete(delegate
		{
			arrowCG.alpha = 0f;
			hoverCG.alpha = 0f;
			SetMyParent(defaultParent);
		});
		clearTween.Pause();
		clearTween.SetAutoKill(autoKillOnCompletion: false);
	}

	public void Clear()
	{
		clearTween.Restart();
	}

	public void SetMyParent(RectTransform ParentRectTrans)
	{
		myRT.SetParent(ParentRectTrans);
		myRT.anchoredPosition = centerPOS;
	}

	public void PresentActive()
	{
		presentSelf.Restart();
		SetActive();
	}

	public void PresentInactive()
	{
		presentSelf.Restart();
		SetInactive();
	}

	public void SetActive()
	{
		if (arrowCG.alpha != 1f)
		{
			setActiveTween.Restart();
		}
	}

	public void SetInactive()
	{
		setInActiveTween.Restart();
	}

	public void PopMouseEnter()
	{
		mouseEnterTween.Restart();
	}

	public void PopMouseExit()
	{
		mouseExitTween.Restart();
	}
}
