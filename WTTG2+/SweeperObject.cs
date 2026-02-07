using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SweeperObject : MonoBehaviour
{
	public delegate void VoidActions();

	private const float SLIDER_SPACING = 10f;

	private const float FADE_DELAY_TIME = 0.2f;

	private const float DOT_WIDTH_SIZE = 11f;

	private const float DOT_SPACING_BUFFER = 4f;

	private const float DISMISS_TIME = 0.15f;

	public GameObject DotObject;

	public GameObject DotHolder;

	public GameObject HotBar;

	public int MyScore;

	public AudioFileDefinition Ping;

	public AudioFileDefinition Pong;

	public AudioFileDefinition Aced;

	public AudioFileDefinition Scored;

	public AudioFileDefinition Failed;

	private bool activateBuildMe;

	private SweeperDotObject.UpdateHotSpotActions activateTheHotSpotsAction;

	private int activeIndex;

	private float buildMeDelay;

	private float buildMeTimeStamp;

	private List<SweeperDotObject> curDotObjects = new List<SweeperDotObject>(30);

	private Vector2 curHotBarPOS = Vector2.zero;

	private RectTransform dotHolderRT;

	private Tweener fadeMeOutTween;

	private Tweener fullShowMeTween;

	private CanvasGroup hotBarCG;

	private float hotBarMaxX;

	private RectTransform hotBarRT;

	private float hotBarWidth;

	private int hotZoneEndIndex;

	private int hotZoneStartIndex;

	private Tweener moveMeUpTween;

	private CanvasGroup myCG;

	private SweeperObjectDefinition myDeff;

	private RectTransform myRT;

	private Tweener scaleMeDownTween;

	private Tweener scaleMeUpTween;

	private bool scrollIsActive;

	private float scrollSpeed;

	private PooledStack<SweeperDotObject> sweeperDotObjectPool;

	private DOSTween sweeperHotBarScroll;

	private Action<float> updateHotBarDirAction;

	private Action<float> updateHotBarPOSAction;

	private Tweener warmTween;

	public event VoidActions IWasDismissed;

	private void Awake()
	{
		myCG = GetComponent<CanvasGroup>();
		hotBarCG = HotBar.GetComponent<CanvasGroup>();
		myRT = GetComponent<RectTransform>();
		dotHolderRT = DotHolder.GetComponent<RectTransform>();
		hotBarRT = HotBar.GetComponent<RectTransform>();
		warmTween = DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 0.25f, 0.2f).SetEase(Ease.Linear);
		warmTween.Pause();
		warmTween.SetAutoKill(autoKillOnCompletion: false);
		fullShowMeTween = DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 1f, 0.2f).SetEase(Ease.InSine);
		fullShowMeTween.Pause();
		fullShowMeTween.SetAutoKill(autoKillOnCompletion: false);
		fadeMeOutTween = DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 0f, 0.15f).SetEase(Ease.Linear);
		fadeMeOutTween.Pause();
		fadeMeOutTween.SetAutoKill(autoKillOnCompletion: false);
		scaleMeDownTween = DOTween.To(() => myRT.localScale, delegate(Vector3 x)
		{
			myRT.localScale = x;
		}, Vector3.zero, 0.15f).SetEase(Ease.OutCirc).OnComplete(delegate
		{
			myRT.localScale = Vector3.one;
		});
		scaleMeDownTween.Pause();
		scaleMeDownTween.SetAutoKill(autoKillOnCompletion: false);
		scaleMeUpTween = DOTween.To(() => myRT.localScale, delegate(Vector3 x)
		{
			myRT.localScale = x;
		}, new Vector3(1.1f, 1.1f, 1f), 0.15f).SetEase(Ease.OutCirc);
		scaleMeUpTween.Pause();
		scaleMeUpTween.SetAutoKill(autoKillOnCompletion: false);
		activateTheHotSpotsAction = ActivateTheHotSpots;
		updateHotBarPOSAction = updateHotBarPOS;
		updateHotBarDirAction = updateHotBarDir;
		sweeperDotObjectPool = new PooledStack<SweeperDotObject>(() => UnityEngine.Object.Instantiate(DotObject, dotHolderRT).GetComponent<SweeperDotObject>(), 30);
	}

	private void Update()
	{
		if (activateBuildMe && Time.time - buildMeTimeStamp >= buildMeDelay)
		{
			activateBuildMe = false;
			BuildMe();
		}
	}

	public void WarmMe(int myIndex, SweeperObjectDefinition SetDef)
	{
		myDeff = SetDef;
		if (myIndex == 0)
		{
			warmTween.Restart(includeDelay: false);
			BuildMe();
			return;
		}
		warmTween.Restart(includeDelay: true, (float)myIndex * 0.2f);
		buildMeDelay = (float)myIndex * 0.2f;
		buildMeTimeStamp = Time.time;
		activateBuildMe = true;
	}

	public void BuildMe()
	{
		Vector2 sizeDelta = new Vector2((float)myDeff.NumOfDotsPerSweeper * 11f - 4f, GetComponent<RectTransform>().sizeDelta.y);
		Vector2 zero = Vector2.zero;
		int num = UnityEngine.Random.Range(myDeff.HotZoneDotSizeMin, myDeff.HotZoneDotSizeMax);
		int num2 = UnityEngine.Random.Range(0, myDeff.NumOfDotsPerSweeper - num);
		myRT.sizeDelta = sizeDelta;
		dotHolderRT.sizeDelta = sizeDelta;
		hotZoneStartIndex = num2;
		hotZoneEndIndex = num2 + (num - 1);
		for (int i = 0; i < myDeff.NumOfDotsPerSweeper; i++)
		{
			SweeperDotObject sweeperDotObject = sweeperDotObjectPool.Pop();
			sweeperDotObject.ActivateHotSpot += activateTheHotSpotsAction;
			sweeperDotObject.BuildMe(i, myDeff.NumOfDotsPerSweeper - 1);
			if (i >= num2 && i <= num2 + (num - 1))
			{
				sweeperDotObject.MakeMeHot();
			}
			sweeperDotObject.GetComponent<RectTransform>().anchoredPosition = zero;
			zero.x += 11f;
			curDotObjects.Add(sweeperDotObject);
		}
		hotBarWidth = UnityEngine.Random.Range(myDeff.HotBarWidthMin, myDeff.HotBarWidthMax);
		hotBarRT.sizeDelta = new Vector2(hotBarWidth, hotBarRT.sizeDelta.y);
		hotBarRT.anchoredPosition = Vector2.zero;
		curHotBarPOS = Vector2.zero;
		hotBarMaxX = myRT.sizeDelta.x - hotBarWidth;
		scrollSpeed = UnityEngine.Random.Range(myDeff.ScrollSpeedMin, myDeff.ScrollSpeedMax);
	}

	public void FireMe()
	{
		hotBarCG.alpha = 1f;
		fullShowMeTween.Restart();
		scrollIsActive = true;
		sweeperHotBarScroll = GameManager.TweenSlinger.PlayDOSTweenLiner(0f, hotBarMaxX, scrollSpeed, updateHotBarPOSAction, updateHotBarDirAction);
	}

	public void PlayerHasDecided()
	{
		fullShowMeTween.Pause();
		scrollIsActive = false;
		GameManager.TweenSlinger.KillTween(sweeperHotBarScroll);
		sweeperHotBarScroll = null;
		float num = (float)hotZoneStartIndex * 11f;
		float num2 = (float)hotZoneEndIndex * 11f;
		if (hotBarRT.anchoredPosition.x >= num && hotBarRT.anchoredPosition.x + hotBarRT.sizeDelta.x <= num2)
		{
			MyScore = 5;
		}
		else
		{
			int num3 = ((!(hotBarRT.anchoredPosition.x + hotBarRT.sizeDelta.x < num)) ? Mathf.FloorToInt((hotBarRT.anchoredPosition.x + hotBarRT.sizeDelta.x - num2) / 11f) : Mathf.FloorToInt((num - (hotBarRT.anchoredPosition.x + hotBarRT.sizeDelta.x)) / 11f));
			num3 = ((num3 >= 0) ? num3 : 0);
			if (num3 <= 4)
			{
				MyScore = 5 - num3;
			}
			else
			{
				MyScore = 0;
			}
		}
		if (MyScore == 5)
		{
			GameManager.AudioSlinger.PlaySound(Aced);
		}
		else if (MyScore < 5 && MyScore > 0)
		{
			GameManager.AudioSlinger.PlaySound(Scored);
		}
		else if (MyScore == 0)
		{
			GameManager.AudioSlinger.PlaySound(Failed);
		}
		for (int i = 0; i < curDotObjects.Count; i++)
		{
			curDotObjects[i].ActivateHotSpot -= activateTheHotSpotsAction;
		}
		DismissMe();
	}

	public void MoveMeUp()
	{
		DOTween.To(() => myRT.anchoredPosition, delegate(Vector2 x)
		{
			myRT.anchoredPosition = x;
		}, new Vector2(0f, myRT.sizeDelta.y + 10f), 0.15f).SetRelative(isRelative: true).SetDelay(0.15f);
	}

	public void ForceEnd()
	{
		scrollIsActive = false;
		GameManager.TweenSlinger.KillTween(sweeperHotBarScroll);
		sweeperHotBarScroll = null;
		for (int i = 0; i < curDotObjects.Count; i++)
		{
			curDotObjects[i].ActivateHotSpot -= activateTheHotSpotsAction;
			curDotObjects[i].Destroy();
			sweeperDotObjectPool.Push(curDotObjects[i]);
		}
		fadeMeOutTween.Restart();
		scaleMeDownTween.Restart();
	}

	public void DestroyMe()
	{
		for (int i = 0; i < curDotObjects.Count; i++)
		{
			curDotObjects[i].ActivateHotSpot -= activateTheHotSpotsAction;
			curDotObjects[i].Destroy();
			sweeperDotObjectPool.Push(curDotObjects[i]);
		}
		scrollIsActive = false;
		GameManager.TweenSlinger.KillTween(sweeperHotBarScroll);
		sweeperHotBarScroll = null;
		curDotObjects.Clear();
		myRT.anchoredPosition = Vector2.zero;
		myRT.localScale = Vector3.one;
		myCG.alpha = 0f;
		hotBarCG.alpha = 0f;
	}

	private void updateHotBarPOS(float setX)
	{
		if (scrollIsActive)
		{
			curHotBarPOS.x = setX;
			hotBarRT.anchoredPosition = curHotBarPOS;
		}
	}

	private void updateHotBarDir(float setX)
	{
		GameManager.TweenSlinger.KillTween(sweeperHotBarScroll);
		if (scrollIsActive)
		{
			updateHotBarPOS(setX);
			if (setX >= hotBarMaxX)
			{
				GameManager.AudioSlinger.PlaySound(Ping);
				sweeperHotBarScroll = GameManager.TweenSlinger.PlayDOSTweenLiner(hotBarMaxX, 0f, scrollSpeed, updateHotBarPOSAction, updateHotBarDirAction);
			}
			else if (setX == 0f)
			{
				GameManager.AudioSlinger.PlaySound(Pong);
				sweeperHotBarScroll = GameManager.TweenSlinger.PlayDOSTweenLiner(0f, hotBarMaxX, scrollSpeed, updateHotBarPOSAction, updateHotBarDirAction);
			}
		}
	}

	private void UpdateActiveIndex(int setIndex)
	{
		activeIndex = setIndex;
	}

	private void ActivateTheHotSpots()
	{
		int num = 0;
		for (int i = 0; i < curDotObjects.Count; i++)
		{
			if (curDotObjects[i].GetAmHot())
			{
				curDotObjects[i].ActivateMyHotSpot(num);
				num++;
			}
		}
	}

	private void DismissMe()
	{
		fadeMeOutTween.Restart();
		if (MyScore == 5)
		{
			scaleMeUpTween.Restart();
		}
		else
		{
			scaleMeDownTween.Restart();
		}
		if (this.IWasDismissed != null)
		{
			this.IWasDismissed();
		}
	}
}
