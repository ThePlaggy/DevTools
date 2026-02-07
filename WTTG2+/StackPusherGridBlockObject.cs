using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class StackPusherGridBlockObject : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public delegate void SelfPassActions(StackPusherGridBlockObject SPGO);

	public HACK_STACK_PUSHER_GRID_BLOCK_STATE State;

	public GameObject SkullBlock;

	public GameObject SkullBot;

	public GameObject StackLocked;

	public GameObject StackReady;

	public GameObject StackActive;

	public GameObject StackActivated;

	public GameObject PushCross;

	public GameObject PushStack;

	public AudioFileDefinition PusherClickSFX;

	public AudioFileDefinition PusherPlaceSFX;

	public AudioFileDefinition StackClick;

	public AudioFileDefinition StackPlace;

	public AudioFileDefinition StackPop;

	private bool canBePushed;

	private Sequence clearMyStackSeq;

	private Tweener hideBlockTween;

	private Tweener hidePushStackTween;

	private Tweener hideStackActivated;

	private Tweener hideStackActiveTween;

	private Tweener hideStackLockedTween;

	private Tweener hideStackReadyTween;

	private CanvasGroup myCG;

	private MatrixStackCord myCord;

	private Vector3 myMaxScale = Vector3.one;

	private Vector3 myMidScale = new Vector3(0.67f, 0.68f, 1f);

	private Vector3 myMinScale = new Vector3(0.1f, 0.1f, 1f);

	private Vector2 myPOS = Vector2.zero;

	private RectTransform myRT;

	private StackPusherHack myStackPusherHack;

	private Vector2 myStartPOS = new Vector2(-40f, 40f);

	private Sequence popSeq;

	private Sequence presentStackReadySeq;

	private CanvasGroup pushCrossCG;

	private CanvasGroup pushStackCG;

	private RectTransform pushStackRT;

	private Tweener showBlockTween;

	private Tweener showPushStackTween;

	private Tweener showSkullTween;

	private Tweener showStackActivated;

	private Tweener showStackActiveTween;

	private Tweener showStackLockedTween;

	private Tweener showStackReadyTween;

	private CanvasGroup skullBlockCG;

	private RectTransform skullBotRT;

	private Vector2 skullLaughClose = new Vector2(0f, 3f);

	private Vector2 skullLaughOpen = new Vector2(0f, -3f);

	private Sequence skullLaughSeq;

	private CanvasGroup stackActivatedCG;

	private RectTransform stackActivatedRT;

	private CanvasGroup stackActiveCG;

	private RectTransform stackActiveRT;

	private CanvasGroup stackLockedCG;

	private CanvasGroup stackReadyCG;

	private RectTransform stackReadyRT;

	public event SelfPassActions Kill;

	private void Awake()
	{
		myRT = GetComponent<RectTransform>();
		stackActiveRT = StackActive.GetComponent<RectTransform>();
		stackReadyRT = StackReady.GetComponent<RectTransform>();
		pushStackRT = PushStack.GetComponent<RectTransform>();
		stackActivatedRT = StackActivated.GetComponent<RectTransform>();
		skullBotRT = SkullBot.GetComponent<RectTransform>();
		myCG = GetComponent<CanvasGroup>();
		stackLockedCG = StackLocked.GetComponent<CanvasGroup>();
		stackReadyCG = StackReady.GetComponent<CanvasGroup>();
		stackActiveCG = StackActive.GetComponent<CanvasGroup>();
		stackActivatedCG = StackActivated.GetComponent<CanvasGroup>();
		pushStackCG = PushStack.GetComponent<CanvasGroup>();
		pushCrossCG = PushCross.GetComponent<CanvasGroup>();
		skullBlockCG = SkullBlock.GetComponent<CanvasGroup>();
		showBlockTween = DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 1f, 0.25f).SetEase(Ease.Linear).OnComplete(subBuild);
		showBlockTween.Pause();
		showBlockTween.SetAutoKill(autoKillOnCompletion: false);
		showSkullTween = DOTween.To(() => skullBlockCG.alpha, delegate(float x)
		{
			skullBlockCG.alpha = x;
		}, 1f, 0.25f).SetEase(Ease.Linear);
		showSkullTween.Pause();
		showSkullTween.SetAutoKill(autoKillOnCompletion: false);
		hideBlockTween = DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 0f, 0.15f).SetEase(Ease.Linear).OnComplete(kill);
		hideBlockTween.Pause();
		hideBlockTween.SetAutoKill(autoKillOnCompletion: false);
		showStackLockedTween = DOTween.To(() => stackLockedCG.alpha, delegate(float x)
		{
			stackLockedCG.alpha = x;
		}, 1f, 0.15f).SetEase(Ease.Linear);
		showStackLockedTween.Pause();
		showStackLockedTween.SetAutoKill(autoKillOnCompletion: false);
		hideStackLockedTween = DOTween.To(() => stackLockedCG.alpha, delegate(float x)
		{
			stackLockedCG.alpha = x;
		}, 0f, 0.15f).SetEase(Ease.Linear);
		hideStackLockedTween.Pause();
		hideStackLockedTween.SetAutoKill(autoKillOnCompletion: false);
		showStackReadyTween = DOTween.To(() => stackReadyCG.alpha, delegate(float x)
		{
			stackReadyCG.alpha = x;
		}, 1f, 0.15f).SetEase(Ease.Linear);
		showStackReadyTween.Pause();
		showStackReadyTween.SetAutoKill(autoKillOnCompletion: false);
		hideStackReadyTween = DOTween.To(() => stackReadyCG.alpha, delegate(float x)
		{
			stackReadyCG.alpha = x;
		}, 0f, 0.15f).SetEase(Ease.Linear);
		hideStackReadyTween.Pause();
		hideStackReadyTween.SetAutoKill(autoKillOnCompletion: false);
		showStackActiveTween = DOTween.To(() => stackActiveCG.alpha, delegate(float x)
		{
			stackActiveCG.alpha = x;
		}, 1f, 0.15f).SetEase(Ease.Linear);
		showStackActiveTween.Pause();
		showStackActiveTween.SetAutoKill(autoKillOnCompletion: false);
		hideStackActiveTween = DOTween.To(() => stackActiveCG.alpha, delegate(float x)
		{
			stackActiveCG.alpha = x;
		}, 0f, 0.15f).SetEase(Ease.Linear);
		hideStackActiveTween.Pause();
		hideStackActiveTween.SetAutoKill(autoKillOnCompletion: false);
		showStackActivated = DOTween.To(() => stackActivatedCG.alpha, delegate(float x)
		{
			stackActivatedCG.alpha = x;
		}, 1f, 0.15f).SetEase(Ease.Linear);
		showStackActivated.Pause();
		showStackActivated.SetAutoKill(autoKillOnCompletion: false);
		hideStackActivated = DOTween.To(() => stackActivatedCG.alpha, delegate(float x)
		{
			stackActivatedCG.alpha = x;
		}, 0f, 0.15f).SetEase(Ease.Linear);
		hideStackActivated.Pause();
		hideStackActivated.SetAutoKill(autoKillOnCompletion: false);
		showPushStackTween = DOTween.To(() => pushStackCG.alpha, delegate(float x)
		{
			pushStackCG.alpha = x;
		}, 1f, 0.15f).SetEase(Ease.Linear);
		showPushStackTween.Pause();
		showPushStackTween.SetAutoKill(autoKillOnCompletion: false);
		hidePushStackTween = DOTween.To(() => pushStackCG.alpha, delegate(float x)
		{
			pushStackCG.alpha = x;
		}, 0f, 0.15f).SetEase(Ease.Linear);
		hidePushStackTween.Pause();
		hidePushStackTween.SetAutoKill(autoKillOnCompletion: false);
		clearMyStackSeq = DOTween.Sequence().OnComplete(delegate
		{
			stackActivatedRT.localScale = myMaxScale;
		});
		clearMyStackSeq.Insert(0f, DOTween.To(() => stackActivatedRT.localScale, delegate(Vector3 x)
		{
			stackActivatedRT.localScale = x;
		}, myMinScale, 0.15f).SetEase(Ease.Linear));
		clearMyStackSeq.Insert(0f, DOTween.To(() => stackActivatedCG.alpha, delegate(float x)
		{
			stackActivatedCG.alpha = x;
		}, 0f, 0.15f).SetEase(Ease.Linear));
		clearMyStackSeq.Pause();
		clearMyStackSeq.SetAutoKill(autoKillOnCompletion: false);
		presentStackReadySeq = DOTween.Sequence();
		presentStackReadySeq.Insert(0.15f, DOTween.To(() => stackReadyRT.localScale, delegate(Vector3 x)
		{
			stackReadyRT.localScale = x;
		}, myMaxScale, 0.15f).SetEase(Ease.Linear));
		presentStackReadySeq.Insert(0.15f, DOTween.To(() => stackReadyCG.alpha, delegate(float x)
		{
			stackReadyCG.alpha = x;
		}, 1f, 0.15f).SetEase(Ease.Linear));
		presentStackReadySeq.Pause();
		presentStackReadySeq.SetAutoKill(autoKillOnCompletion: false);
		popSeq = DOTween.Sequence().OnComplete(delegate
		{
			pushStackRT.localScale = myMidScale;
		});
		popSeq.Insert(0f, DOTween.To(() => stackActiveCG.alpha, delegate(float x)
		{
			stackActiveCG.alpha = x;
		}, 0f, 0.15f).SetEase(Ease.Linear));
		popSeq.Insert(0f, DOTween.To(() => pushStackRT.localScale, delegate(Vector3 x)
		{
			pushStackRT.localScale = x;
		}, myMaxScale, 0.25f).SetEase(Ease.Linear));
		popSeq.Insert(0f, DOTween.To(() => pushStackCG.alpha, delegate(float x)
		{
			pushStackCG.alpha = x;
		}, 0f, 0.3f).SetEase(Ease.Linear));
		popSeq.Pause();
		popSeq.SetAutoKill(autoKillOnCompletion: false);
		skullLaughSeq = DOTween.Sequence();
		skullLaughSeq.Insert(0f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, skullLaughOpen, 0.2f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		skullLaughSeq.Insert(0.2f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, skullLaughClose, 0.2f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		skullLaughSeq.SetLoops(-1);
		skullLaughSeq.Pause();
		skullLaughSeq.SetAutoKill(autoKillOnCompletion: false);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		switch (myStackPusherHack.State)
		{
		case HACK_STACK_PUSHER_STATE.IDLE:
			switch (State)
			{
			case HACK_STACK_PUSHER_GRID_BLOCK_STATE.DEAD:
				myStackPusherHack.Boom();
				break;
			case HACK_STACK_PUSHER_GRID_BLOCK_STATE.STACK_READY:
				GameManager.AudioSlinger.PlaySound(StackClick);
				myStackPusherHack.CurrentPushingStackBlock = this;
				myStackPusherHack.State = HACK_STACK_PUSHER_STATE.PUSHING;
				hideStackActiveTween.Restart();
				showStackActivated.Restart();
				break;
			case HACK_STACK_PUSHER_GRID_BLOCK_STATE.PUSHER:
				GameManager.AudioSlinger.PlaySound(PusherClickSFX);
				myStackPusherHack.State = HACK_STACK_PUSHER_STATE.PUSHER_PLACEMENT;
				myStackPusherHack.GridPusher.SetActive();
				break;
			}
			break;
		case HACK_STACK_PUSHER_STATE.PUSHER_PLACEMENT:
			switch (State)
			{
			default:
				myStackPusherHack.State = HACK_STACK_PUSHER_STATE.IDLE;
				myStackPusherHack.GridPusher.SetInActive();
				break;
			case HACK_STACK_PUSHER_GRID_BLOCK_STATE.DEAD:
				myStackPusherHack.Boom();
				break;
			case HACK_STACK_PUSHER_GRID_BLOCK_STATE.ACTIVE:
				GameManager.AudioSlinger.PlaySound(PusherPlaceSFX);
				pushCrossCG.alpha = 0f;
				myStackPusherHack.State = HACK_STACK_PUSHER_STATE.IDLE;
				myStackPusherHack.GridPusher.Move(myRT, myCord);
				break;
			}
			break;
		case HACK_STACK_PUSHER_STATE.PUSHING:
		{
			HACK_STACK_PUSHER_GRID_BLOCK_STATE state2 = State;
			switch (state2)
			{
			case HACK_STACK_PUSHER_GRID_BLOCK_STATE.POPER:
				if (canBePushed)
				{
					GameManager.AudioSlinger.PlaySound(StackPop);
					myStackPusherHack.State = HACK_STACK_PUSHER_STATE.IDLE;
					myStackPusherHack.GridPoper.PopMouseExit();
					myStackPusherHack.PopStack();
				}
				break;
			default:
				if (state2 != HACK_STACK_PUSHER_GRID_BLOCK_STATE.ACTIVE)
				{
					myStackPusherHack.State = HACK_STACK_PUSHER_STATE.IDLE;
					myStackPusherHack.CurrentPushingStackBlock.CancelPushingStack();
					myStackPusherHack.CurrentPushingStackBlock = null;
				}
				else if (canBePushed)
				{
					GameManager.AudioSlinger.PlaySound(StackPlace);
					myStackPusherHack.State = HACK_STACK_PUSHER_STATE.IDLE;
					myStackPusherHack.CurrentPushingStackBlock.ClearMyStack();
					myStackPusherHack.CurrentPushingStackBlock = null;
					nowHasStack();
				}
				else
				{
					myStackPusherHack.State = HACK_STACK_PUSHER_STATE.IDLE;
					myStackPusherHack.CurrentPushingStackBlock.CancelPushingStack();
					myStackPusherHack.CurrentPushingStackBlock = null;
				}
				break;
			case HACK_STACK_PUSHER_GRID_BLOCK_STATE.DEAD:
				myStackPusherHack.Boom();
				break;
			}
			break;
		}
		default:
		{
			HACK_STACK_PUSHER_GRID_BLOCK_STATE state = State;
			if (state == HACK_STACK_PUSHER_GRID_BLOCK_STATE.DEAD)
			{
				myStackPusherHack.Boom();
			}
			break;
		}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		switch (myStackPusherHack.State)
		{
		case HACK_STACK_PUSHER_STATE.PUSHING:
			switch (State)
			{
			case HACK_STACK_PUSHER_GRID_BLOCK_STATE.POPER:
				if (canBePushed)
				{
					myStackPusherHack.GridPoper.PopMouseEnter();
				}
				break;
			case HACK_STACK_PUSHER_GRID_BLOCK_STATE.ACTIVE:
				if (canBePushed)
				{
					showPushStackTween.Restart();
				}
				break;
			}
			break;
		case HACK_STACK_PUSHER_STATE.PUSHER_PLACEMENT:
		{
			HACK_STACK_PUSHER_GRID_BLOCK_STATE state = State;
			if (state == HACK_STACK_PUSHER_GRID_BLOCK_STATE.ACTIVE)
			{
				pushCrossCG.alpha = 1f;
			}
			break;
		}
		case HACK_STACK_PUSHER_STATE.IDLE:
			switch (State)
			{
			case HACK_STACK_PUSHER_GRID_BLOCK_STATE.STACK_READY:
				showStackActiveTween.Restart();
				break;
			case HACK_STACK_PUSHER_GRID_BLOCK_STATE.PUSHER:
				myStackPusherHack.GridPusher.PointerEnter();
				break;
			}
			break;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		switch (myStackPusherHack.State)
		{
		case HACK_STACK_PUSHER_STATE.PUSHING:
			switch (State)
			{
			case HACK_STACK_PUSHER_GRID_BLOCK_STATE.POPER:
				if (canBePushed)
				{
					myStackPusherHack.GridPoper.PopMouseExit();
				}
				break;
			case HACK_STACK_PUSHER_GRID_BLOCK_STATE.ACTIVE:
				if (canBePushed)
				{
					hidePushStackTween.Restart();
				}
				break;
			}
			break;
		case HACK_STACK_PUSHER_STATE.PUSHER_PLACEMENT:
		{
			HACK_STACK_PUSHER_GRID_BLOCK_STATE state = State;
			if (state == HACK_STACK_PUSHER_GRID_BLOCK_STATE.ACTIVE)
			{
				pushCrossCG.alpha = 0f;
			}
			break;
		}
		case HACK_STACK_PUSHER_STATE.IDLE:
			switch (State)
			{
			case HACK_STACK_PUSHER_GRID_BLOCK_STATE.STACK_READY:
				hideStackActiveTween.Restart();
				break;
			case HACK_STACK_PUSHER_GRID_BLOCK_STATE.PUSHER:
				myStackPusherHack.GridPusher.PointerExit();
				break;
			}
			break;
		}
	}

	public void SoftBuild(StackPusherHack SetStackPusher)
	{
		myStackPusherHack = SetStackPusher;
		myCG.alpha = 0f;
		myRT.anchoredPosition = myStartPOS;
		State = HACK_STACK_PUSHER_GRID_BLOCK_STATE.INACTIVE;
	}

	public void Build(MatrixStackCord SetCord, HACK_STACK_PUSHER_GRID_BLOCK_STATE SetState)
	{
		State = SetState;
		myCord = SetCord;
		myPOS.x = (float)myCord.X * 40f;
		myPOS.y = 0f - (float)myCord.Y * 40f;
		myRT.anchoredPosition = myPOS;
		showBlockTween.Rewind();
		showBlockTween.Play();
	}

	public void ClearStackState()
	{
		canBePushed = false;
		if (State == HACK_STACK_PUSHER_GRID_BLOCK_STATE.STACK_LOCKED || State == HACK_STACK_PUSHER_GRID_BLOCK_STATE.STACK_READY)
		{
			State = HACK_STACK_PUSHER_GRID_BLOCK_STATE.STACK_LOCKED;
			showStackLockedTween.Restart();
			hideStackReadyTween.Restart();
		}
		if (State == HACK_STACK_PUSHER_GRID_BLOCK_STATE.POPER)
		{
			myStackPusherHack.GridPoper.SetInactive();
		}
	}

	public void SetStackStateReady()
	{
		if (State == HACK_STACK_PUSHER_GRID_BLOCK_STATE.STACK_LOCKED)
		{
			State = HACK_STACK_PUSHER_GRID_BLOCK_STATE.STACK_READY;
			showStackReadyTween.Restart();
			hideStackLockedTween.Restart();
		}
		if (State == HACK_STACK_PUSHER_GRID_BLOCK_STATE.ACTIVE)
		{
			canBePushed = true;
		}
		if (State == HACK_STACK_PUSHER_GRID_BLOCK_STATE.POPER)
		{
			canBePushed = true;
			myStackPusherHack.GridPoper.SetActive();
		}
	}

	public void ClearMyStack()
	{
		State = HACK_STACK_PUSHER_GRID_BLOCK_STATE.ACTIVE;
		canBePushed = true;
		stackReadyCG.alpha = 0f;
		stackLockedCG.alpha = 0f;
		clearMyStackSeq.Restart();
	}

	public void CancelPushingStack()
	{
		hideStackActivated.Restart();
	}

	public void Pop()
	{
		State = HACK_STACK_PUSHER_GRID_BLOCK_STATE.ACTIVE;
		pushStackCG.alpha = 1f;
		stackReadyCG.alpha = 0f;
		stackLockedCG.alpha = 0f;
		stackActivatedCG.alpha = 0f;
		popSeq.Restart();
	}

	public void Destroy()
	{
		State = HACK_STACK_PUSHER_GRID_BLOCK_STATE.INACTIVE;
		canBePushed = false;
		stackLockedCG.alpha = 0f;
		stackReadyCG.alpha = 0f;
		stackActiveCG.alpha = 0f;
		stackActivatedCG.alpha = 0f;
		pushCrossCG.alpha = 0f;
		pushStackCG.alpha = 0f;
		skullBlockCG.alpha = 0f;
		hideBlockTween.Rewind();
		hideBlockTween.Play();
	}

	private void nowHasStack()
	{
		State = HACK_STACK_PUSHER_GRID_BLOCK_STATE.STACK_READY;
		stackReadyRT.localScale = myMinScale;
		presentStackReadySeq.Rewind();
		hidePushStackTween.Rewind();
		presentStackReadySeq.Play();
		hidePushStackTween.Play();
	}

	private void kill()
	{
		myRT.anchoredPosition = myStartPOS;
		stackLockedCG.alpha = 0f;
		stackReadyCG.alpha = 0f;
		stackActiveCG.alpha = 0f;
		stackActivatedCG.alpha = 0f;
		pushCrossCG.alpha = 0f;
		pushStackCG.alpha = 0f;
		skullBlockCG.alpha = 0f;
		skullLaughSeq.Pause();
		if (this.Kill != null)
		{
			this.Kill(this);
		}
	}

	private void subBuild()
	{
		switch (State)
		{
		case HACK_STACK_PUSHER_GRID_BLOCK_STATE.ACTIVE_AND_PUSHABLE:
			State = HACK_STACK_PUSHER_GRID_BLOCK_STATE.ACTIVE;
			canBePushed = true;
			break;
		case HACK_STACK_PUSHER_GRID_BLOCK_STATE.STACK_LOCKED:
			showStackLockedTween.Restart();
			break;
		case HACK_STACK_PUSHER_GRID_BLOCK_STATE.STACK_READY:
			showStackReadyTween.Restart();
			break;
		case HACK_STACK_PUSHER_GRID_BLOCK_STATE.PUSHER:
			myStackPusherHack.GridPusher.SetMyParent(myRT);
			myStackPusherHack.GridPusher.PresentMe();
			break;
		case HACK_STACK_PUSHER_GRID_BLOCK_STATE.POPER:
			myStackPusherHack.GridPoper.SetMyParent(myRT);
			myStackPusherHack.GridPoper.PresentInactive();
			break;
		case HACK_STACK_PUSHER_GRID_BLOCK_STATE.POPER_AND_ACTIVE:
			State = HACK_STACK_PUSHER_GRID_BLOCK_STATE.POPER;
			myStackPusherHack.GridPoper.SetMyParent(myRT);
			myStackPusherHack.GridPoper.PresentActive();
			canBePushed = true;
			break;
		case HACK_STACK_PUSHER_GRID_BLOCK_STATE.DEAD:
			showSkullTween.Restart();
			skullLaughSeq.Restart();
			break;
		default:
			stackLockedCG.alpha = 0f;
			stackReadyCG.alpha = 0f;
			stackActiveCG.alpha = 0f;
			stackActivatedCG.alpha = 0f;
			pushCrossCG.alpha = 0f;
			pushStackCG.alpha = 0f;
			skullBlockCG.alpha = 0f;
			break;
		}
	}
}
