using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StackPusherGridPusherObject : MonoBehaviour
{
	public delegate void MovedActions(MatrixStackCord Setcord);

	public delegate void SetStackActions(Dictionary<MatrixStackCord, short> TheCords);

	public GameObject InactiveIMG;

	public GameObject HoverIMG;

	public GameObject ActiveIMG;

	private CanvasGroup activeCG;

	private Vector2 centerPOS = Vector2.zero;

	private Tweener clearTween;

	private RectTransform defaultParent;

	private Dictionary<MatrixStackCord, short> effectedCords = new Dictionary<MatrixStackCord, short>(MatrixStackCordCompare.Ins);

	private Sequence gridScaleDownSeq;

	private Sequence gridScaleUpSeq;

	private CanvasGroup hoverCG;

	private Tweener hoverEnterTween;

	private Tweener hoverExitTween;

	private CanvasGroup inactiveCG;

	private Vector3 maxScale = Vector3.one;

	private Vector3 minScale = new Vector3(0.1f, 0.1f, 1f);

	private CanvasGroup myCG;

	private MatrixStackCord myCurrentCord;

	private RectTransform myRT;

	private Dictionary<MatrixStackCord, short> oldCords = new Dictionary<MatrixStackCord, short>(MatrixStackCordCompare.Ins);

	private Tweener presentTween;

	private Dictionary<MatrixStackCord, short> resetCords = new Dictionary<MatrixStackCord, short>(MatrixStackCordCompare.Ins);

	private Tweener setActiveTween;

	private Tweener setInActiveTween;

	public MatrixStackCord CurrentCord
	{
		get
		{
			return myCurrentCord;
		}
		set
		{
			myCurrentCord = value;
			setNewEffectedCords();
		}
	}

	public event MovedActions ClearOldPushBlock;

	public event SetStackActions ResetStacks;

	public event MovedActions SetNewPushBlock;

	public event SetStackActions SetNewStacks;

	private void Awake()
	{
		myRT = GetComponent<RectTransform>();
		myCG = GetComponent<CanvasGroup>();
		inactiveCG = InactiveIMG.GetComponent<CanvasGroup>();
		hoverCG = HoverIMG.GetComponent<CanvasGroup>();
		activeCG = ActiveIMG.GetComponent<CanvasGroup>();
		defaultParent = (RectTransform)myRT.parent;
		presentTween = DOTween.To(() => inactiveCG.alpha, delegate(float x)
		{
			inactiveCG.alpha = x;
		}, 1f, 0.15f).SetEase(Ease.Linear);
		presentTween.Pause();
		presentTween.SetAutoKill(autoKillOnCompletion: false);
		hoverEnterTween = DOTween.To(() => hoverCG.alpha, delegate(float x)
		{
			hoverCG.alpha = x;
		}, 1f, 0.15f).SetEase(Ease.Linear);
		hoverEnterTween.Pause();
		hoverEnterTween.SetAutoKill(autoKillOnCompletion: false);
		hoverExitTween = DOTween.To(() => hoverCG.alpha, delegate(float x)
		{
			hoverCG.alpha = x;
		}, 0f, 0.15f).SetEase(Ease.Linear);
		hoverExitTween.Pause();
		hoverExitTween.SetAutoKill(autoKillOnCompletion: false);
		setActiveTween = DOTween.To(() => activeCG.alpha, delegate(float x)
		{
			activeCG.alpha = x;
		}, 1f, 0.15f).SetEase(Ease.Linear).OnComplete(delegate
		{
			hoverCG.alpha = 0f;
		});
		setActiveTween.Pause();
		setActiveTween.SetAutoKill(autoKillOnCompletion: false);
		setInActiveTween = DOTween.To(() => activeCG.alpha, delegate(float x)
		{
			activeCG.alpha = x;
		}, 0f, 0.15f).SetEase(Ease.Linear);
		setInActiveTween.Pause();
		setInActiveTween.SetAutoKill(autoKillOnCompletion: false);
		gridScaleDownSeq = DOTween.Sequence();
		gridScaleDownSeq.Insert(0f, DOTween.To(() => myRT.localScale, delegate(Vector3 x)
		{
			myRT.localScale = x;
		}, minScale, 0.15f).SetEase(Ease.Linear));
		gridScaleDownSeq.Insert(0f, DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 0f, 0.15f).SetEase(Ease.Linear));
		gridScaleDownSeq.Pause();
		gridScaleDownSeq.SetAutoKill(autoKillOnCompletion: false);
		gridScaleUpSeq = DOTween.Sequence();
		gridScaleUpSeq.Insert(0.15f, DOTween.To(() => myRT.localScale, delegate(Vector3 x)
		{
			myRT.localScale = x;
		}, maxScale, 0.15f).SetEase(Ease.Linear));
		gridScaleUpSeq.Insert(0.15f, DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 1f, 0.15f).SetEase(Ease.Linear));
		gridScaleUpSeq.Pause();
		gridScaleUpSeq.SetAutoKill(autoKillOnCompletion: false);
		clearTween = DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 0f, 0.15f).SetEase(Ease.Linear).OnComplete(delegate
		{
			activeCG.alpha = 0f;
			SetMyParent(defaultParent);
		});
		clearTween.Pause();
		clearTween.SetAutoKill(autoKillOnCompletion: false);
	}

	public void Clear()
	{
		effectedCords.Clear();
		oldCords.Clear();
		resetCords.Clear();
		clearTween.Restart();
	}

	public void SetMyParent(RectTransform ParentRectTrans)
	{
		myRT.SetParent(ParentRectTrans);
		myRT.anchoredPosition = centerPOS;
	}

	public void PresentMe()
	{
		myCG.alpha = 1f;
		presentTween.Play();
	}

	public void PointerEnter()
	{
		hoverEnterTween.Restart();
	}

	public void PointerExit()
	{
		hoverExitTween.Restart();
	}

	public void SetActive()
	{
		setActiveTween.Restart();
	}

	public void SetInActive()
	{
		setInActiveTween.Restart();
	}

	public bool AmNextTo(MatrixStackCord CheckCord)
	{
		return effectedCords.ContainsKey(CheckCord);
	}

	public void Move(RectTransform NewParent, MatrixStackCord NewCord)
	{
		foreach (KeyValuePair<MatrixStackCord, short> effectedCord in effectedCords)
		{
			oldCords.Add(effectedCord.Key, effectedCord.Value);
		}
		if (this.ClearOldPushBlock != null)
		{
			this.ClearOldPushBlock(myCurrentCord);
		}
		CurrentCord = NewCord;
		foreach (KeyValuePair<MatrixStackCord, short> oldCord in oldCords)
		{
			if (!effectedCords.ContainsKey(oldCord.Key))
			{
				resetCords.Add(oldCord.Key, oldCord.Value);
			}
		}
		if (this.ResetStacks != null)
		{
			this.ResetStacks(resetCords);
		}
		if (this.SetNewPushBlock != null)
		{
			this.SetNewPushBlock(NewCord);
		}
		SetInActive();
		gridScaleDownSeq.Restart();
		gridScaleUpSeq.Restart();
		GameManager.TimeSlinger.FireTimer(0.15f, delegate
		{
			oldCords.Clear();
			resetCords.Clear();
			SetMyParent(NewParent);
			if (this.SetNewStacks != null)
			{
				this.SetNewStacks(effectedCords);
			}
		});
	}

	private void setNewEffectedCords()
	{
		effectedCords.Clear();
		int num = myCurrentCord.X - 1;
		int num2 = myCurrentCord.Y - 1;
		for (int i = 0; i < 9; i++)
		{
			effectedCords.Add(new MatrixStackCord(num, num2), 3);
			bool flag = num > myCurrentCord.X;
			num = ((!flag) ? (num + 1) : (myCurrentCord.X - 1));
			num2 = ((!flag) ? num2 : (num2 + 1));
		}
	}
}
