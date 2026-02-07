using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TerminalLineObject : MonoBehaviour
{
	public delegate void VoidActions(TerminalLineObject TLO);

	protected static Vector2 startPOS = new Vector2(0f, 10f);

	public GameObject textLine;

	public bool SoftLine;

	private string crackLineTitle;

	private float crackTimeStamp;

	private bool delayTypeShowActive;

	private bool isCrackLine;

	private CanvasGroup myCanvasGroup;

	private DOSTween myDOSTween;

	private string myLine;

	private Vector2 myPOS = Vector2.zero;

	private RectTransform myRectTrans;

	private Text myText;

	private Timer myTimer;

	private Tweener myTweener;

	private float typeShowDelay;

	private float typeShowDelayTimeStamp;

	private float typeShowTime;

	private float updateDelay;

	private Action<float> updateStrTextAction;

	public event VoidActions ClearLine;

	public event VoidActions HardClearLine;

	private void Awake()
	{
	}

	private void Update()
	{
		if (isCrackLine && Time.time - crackTimeStamp >= updateDelay)
		{
			crackTimeStamp = Time.time;
			myLine = crackLineTitle + MagicSlinger.GenerateRandomHexCode(2, 16, " ");
			hardShow();
		}
		if (delayTypeShowActive && Time.time - typeShowDelayTimeStamp >= typeShowDelay)
		{
			delayTypeShowActive = false;
			typeShow(typeShowTime);
		}
	}

	public void SoftBuild()
	{
		myCanvasGroup = base.gameObject.AddComponent<CanvasGroup>();
		myCanvasGroup.blocksRaycasts = false;
		myCanvasGroup.interactable = false;
		myRectTrans = base.gameObject.GetComponent<RectTransform>();
		myText = textLine.GetComponent<Text>();
		myRectTrans.anchoredPosition = startPOS;
		updateStrTextAction = updateStrText;
	}

	public void Build(TERMINAL_LINE_TYPE LineType, string SetLine, float LengthAmount = 0f, float SetDelay = 0f)
	{
		myLine = SetLine;
		switch (LineType)
		{
		case TERMINAL_LINE_TYPE.HARD:
			hardShow(SetDelay);
			break;
		case TERMINAL_LINE_TYPE.TYPE:
			typeShow(LengthAmount, SetDelay);
			break;
		case TERMINAL_LINE_TYPE.FADE:
			fadeShow(LengthAmount, SetDelay);
			break;
		case TERMINAL_LINE_TYPE.CRACK:
			buildCrackLine(SetLine);
			break;
		}
	}

	public void Clear()
	{
		myRectTrans.anchoredPosition = startPOS;
		isCrackLine = false;
		crackTimeStamp = 0f;
		updateDelay = 0f;
		myTweener.Kill();
		GameManager.TimeSlinger.KillTimer(myTimer);
		GameManager.TweenSlinger.KillTween(myDOSTween);
		myTweener = null;
		myTimer = null;
		myDOSTween = null;
		if (!SoftLine)
		{
			myText.text = string.Empty;
			myLine = string.Empty;
			crackLineTitle = string.Empty;
			if (this.ClearLine != null)
			{
				this.ClearLine(this);
			}
		}
	}

	public void AniHardClear(TERMINAL_LINE_TYPE LineType, float SetTime)
	{
		switch (LineType)
		{
		case TERMINAL_LINE_TYPE.HARD:
			HardClear();
			break;
		case TERMINAL_LINE_TYPE.FADE:
			fadeHide(SetTime);
			break;
		case TERMINAL_LINE_TYPE.TYPE:
			typeHide(SetTime);
			break;
		}
		if (LineType != TERMINAL_LINE_TYPE.HARD)
		{
			GameManager.TimeSlinger.FireTimer(SetTime, HardClear);
		}
	}

	public void HardClear()
	{
		myRectTrans.anchoredPosition = startPOS;
		myText.text = string.Empty;
		myLine = string.Empty;
		crackLineTitle = string.Empty;
		isCrackLine = false;
		crackTimeStamp = 0f;
		updateDelay = 0f;
		myTweener.Kill();
		GameManager.TimeSlinger.KillTimer(myTimer);
		GameManager.TweenSlinger.KillTween(myDOSTween);
		myTweener = null;
		myTimer = null;
		myDOSTween = null;
		if (this.HardClearLine != null)
		{
			this.HardClearLine(this);
		}
	}

	public void Move(float SetY)
	{
		myPOS.y = SetY;
		myRectTrans.anchoredPosition = myPOS;
	}

	public void UpdateMyText(string setText)
	{
		myLine = setText;
		textLine.GetComponent<Text>().text = setText;
	}

	public void KillCrackLine()
	{
		if (isCrackLine)
		{
			isCrackLine = false;
		}
	}

	public string GetMyLine()
	{
		return myLine;
	}

	public void buildCrackLine(string SetTitle = "")
	{
		if (SetTitle == string.Empty)
		{
			crackLineTitle = MagicSlinger.FluffString(" ", " ", 17);
		}
		else
		{
			crackLineTitle = MagicSlinger.FluffString(SetTitle, " ", 15) + ": ";
		}
		myLine = crackLineTitle + MagicSlinger.GenerateRandomHexCode(2, 16, " ");
		hardShow();
		crackTimeStamp = Time.time;
		updateDelay = 0.15f;
		isCrackLine = true;
	}

	public void BuildBlank()
	{
		myLine = string.Empty;
		hardShow();
	}

	private void hardShow()
	{
		if (myText != null)
		{
			myText.text = myLine;
		}
	}

	private void hardShow(float SetDelay)
	{
		if (SetDelay > 0f)
		{
			GameManager.TimeSlinger.FireHardTimer(out myTimer, SetDelay, hardShow);
		}
		else
		{
			hardShow();
		}
	}

	private void fadeShow(float SetTime)
	{
		myCanvasGroup.alpha = 0f;
		myText.text = myLine;
		myTweener = DOTween.To(() => myCanvasGroup.alpha, delegate(float x)
		{
			myCanvasGroup.alpha = x;
		}, 1f, SetTime).SetEase(Ease.Linear);
	}

	private void fadeShow(float SetTime, float SetDelay)
	{
		if (SetDelay > 0f)
		{
			GameManager.TimeSlinger.FireHardTimer(out myTimer, SetDelay, fadeShow, SetTime);
		}
		else
		{
			fadeShow(SetTime);
		}
	}

	private void fadeHide(float SetTime)
	{
		DOTween.To(() => myCanvasGroup.alpha, delegate(float x)
		{
			myCanvasGroup.alpha = x;
		}, 0f, SetTime).SetEase(Ease.Linear);
	}

	private void typeShow(float SetTime)
	{
		myDOSTween = GameManager.TweenSlinger.PlayDOSTweenLiner(0f, myLine.Length, SetTime, updateStrTextAction);
	}

	private void typeShow(float SetTime, float SetDelay)
	{
		if (SetDelay > 0f)
		{
			typeShowTime = SetTime;
			typeShowDelay = SetDelay;
			typeShowDelayTimeStamp = Time.time;
			delayTypeShowActive = true;
		}
		else
		{
			typeShow(SetTime);
		}
	}

	private void typeHide(float SetTime)
	{
		myDOSTween = GameManager.TweenSlinger.PlayDOSTweenLiner(myLine.Length, 0f, SetTime, updateStrTextAction);
	}

	private void updateStrText(float SetCount)
	{
		if (myLine.Length > 0 && Mathf.RoundToInt(SetCount) <= myLine.Length)
		{
			myText.text = myLine.Substring(0, Mathf.RoundToInt(SetCount));
		}
	}
}
