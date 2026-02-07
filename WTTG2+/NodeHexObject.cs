using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NodeHexObject : MonoBehaviour
{
	private const float FIRE_NEXT_NODE_DELAY_TIME = 0.15f;

	private const float PRESENT_ME_DELAY_TIME = 0.2f;

	public HACK_NODE_HEXER_NODE_TYPE Type;

	public MatrixStackCord MyCord;

	public bool WasConnected;

	public bool IsReadyToConnect;

	public CanvasGroup MyCG;

	public RectTransform MyRT;

	public CanvasGroup BaseCG;

	public CanvasGroup ActiveBaseCG;

	public CanvasGroup TypeAlphaCG;

	public CanvasGroup TypeBetaCG;

	public CanvasGroup NotPlayableCG;

	public CanvasGroup CardinalConnectorsCG;

	public CanvasGroup NonCardinalConnectorsCG;

	public Image Hand12On;

	public Image Hand3On;

	public Image Hand6On;

	public Image Hand9On;

	public RectTransform Hand1On;

	public RectTransform Hand4On;

	public RectTransform Hand7On;

	public RectTransform Hand10On;

	public NodeHexInteractObject[] InteractObjects;

	public AudioFileDefinition ShowNeedsToBeTaggedSFX;

	public AudioFileDefinition HoverDirectionSFX;

	public AudioFileDefinition ClickDirectionSFX;

	public AudioFileDefinition ConnectSFX;

	private NodeHexObject connectingNode;

	private bool fireNextNodeActive;

	private float fireNextNodeTimeStamp;

	private Tweener hand10OnTween;

	private Tweener hand12OnTween;

	private Tweener hand1OnTween;

	private Tweener hand3OnTween;

	private Tweener hand4OnTween;

	private Tweener hand6OnTween;

	private Tweener hand7OnTween;

	private Tweener hand9OnTween;

	private Tweener hideActiveBase;

	private Tweener hideActiveBaseTween;

	private Tweener hideBaseCG;

	private Sequence hideMeSeq;

	private Tweener hideMeTween;

	private Tweener hideNonPlayableCG;

	private MATRIX_STACK_CLOCK_POSITION myDirection;

	private NodeHexerHack myNodeHexerHack;

	private Vector2 myPOS = Vector2.zero;

	private bool needsToBeTagged;

	private bool presentMeActivated;

	private float presentMeDelay;

	private float presentMeTimeStamp;

	private Tweener presentMeTween;

	private Tweener showActiveBase;

	private Tweener showBaseCG;

	private Tweener showNonPlayableCG;

	private void Awake()
	{
		hand12OnTween = DOTween.To(() => Hand12On.fillAmount, delegate(float x)
		{
			Hand12On.fillAmount = x;
		}, 1f, 0.15f).SetEase(Ease.Linear);
		hand12OnTween.Pause();
		hand12OnTween.SetAutoKill(autoKillOnCompletion: false);
		hand3OnTween = DOTween.To(() => Hand3On.fillAmount, delegate(float x)
		{
			Hand3On.fillAmount = x;
		}, 1f, 0.15f).SetEase(Ease.Linear);
		hand3OnTween.Pause();
		hand3OnTween.SetAutoKill(autoKillOnCompletion: false);
		hand6OnTween = DOTween.To(() => Hand6On.fillAmount, delegate(float x)
		{
			Hand6On.fillAmount = x;
		}, 1f, 0.15f).SetEase(Ease.Linear);
		hand6OnTween.Pause();
		hand6OnTween.SetAutoKill(autoKillOnCompletion: false);
		hand9OnTween = DOTween.To(() => Hand9On.fillAmount, delegate(float x)
		{
			Hand9On.fillAmount = x;
		}, 1f, 0.15f).SetEase(Ease.Linear);
		hand9OnTween.Pause();
		hand9OnTween.SetAutoKill(autoKillOnCompletion: false);
		hand1OnTween = DOTween.To(() => Hand1On.anchoredPosition, delegate(Vector2 x)
		{
			Hand1On.anchoredPosition = x;
		}, new Vector2(29f, 29f), 0.15f).SetEase(Ease.Linear);
		hand1OnTween.Pause();
		hand1OnTween.SetAutoKill(autoKillOnCompletion: false);
		hand4OnTween = DOTween.To(() => Hand4On.anchoredPosition, delegate(Vector2 x)
		{
			Hand4On.anchoredPosition = x;
		}, new Vector2(29f, -29f), 0.15f).SetEase(Ease.Linear);
		hand4OnTween.Pause();
		hand4OnTween.SetAutoKill(autoKillOnCompletion: false);
		hand7OnTween = DOTween.To(() => Hand7On.anchoredPosition, delegate(Vector2 x)
		{
			Hand7On.anchoredPosition = x;
		}, new Vector2(-29f, -29f), 0.15f).SetEase(Ease.Linear);
		hand7OnTween.Pause();
		hand7OnTween.SetAutoKill(autoKillOnCompletion: false);
		hand10OnTween = DOTween.To(() => Hand10On.anchoredPosition, delegate(Vector2 x)
		{
			Hand10On.anchoredPosition = x;
		}, new Vector2(-29f, 29f), 0.15f).SetEase(Ease.Linear);
		hand10OnTween.Pause();
		hand10OnTween.SetAutoKill(autoKillOnCompletion: false);
		hideActiveBaseTween = DOTween.To(() => ActiveBaseCG.alpha, delegate(float x)
		{
			ActiveBaseCG.alpha = x;
		}, 0f, 0.15f).SetEase(Ease.Linear);
		hideActiveBaseTween.Pause();
		hideActiveBaseTween.SetAutoKill(autoKillOnCompletion: false);
		presentMeTween = DOTween.To(() => MyCG.alpha, delegate(float x)
		{
			MyCG.alpha = x;
		}, 1f, 0.25f).SetEase(Ease.Linear);
		presentMeTween.Pause();
		presentMeTween.SetAutoKill(autoKillOnCompletion: false);
		hideMeTween = DOTween.To(() => MyCG.alpha, delegate(float x)
		{
			MyCG.alpha = x;
		}, 0f, 0.25f).SetEase(Ease.Linear).OnComplete(finalClear);
		hideMeTween.Pause();
		hideMeTween.SetAutoKill(autoKillOnCompletion: false);
		showBaseCG = DOTween.To(() => BaseCG.alpha, delegate(float x)
		{
			BaseCG.alpha = x;
		}, 1f, 0.15f).SetEase(Ease.Linear);
		showBaseCG.Pause();
		showBaseCG.SetAutoKill(autoKillOnCompletion: false);
		hideBaseCG = DOTween.To(() => BaseCG.alpha, delegate(float x)
		{
			BaseCG.alpha = x;
		}, 0f, 0.15f).SetEase(Ease.Linear);
		hideBaseCG.Pause();
		hideBaseCG.SetAutoKill(autoKillOnCompletion: false);
		showNonPlayableCG = DOTween.To(() => NotPlayableCG.alpha, delegate(float x)
		{
			NotPlayableCG.alpha = x;
		}, 1f, 0.15f).SetEase(Ease.Linear);
		showNonPlayableCG.Pause();
		showNonPlayableCG.SetAutoKill(autoKillOnCompletion: false);
		hideNonPlayableCG = DOTween.To(() => NotPlayableCG.alpha, delegate(float x)
		{
			NotPlayableCG.alpha = x;
		}, 0f, 0.15f).SetEase(Ease.Linear);
		hideNonPlayableCG.Pause();
		hideNonPlayableCG.SetAutoKill(autoKillOnCompletion: false);
		showActiveBase = DOTween.To(() => ActiveBaseCG.alpha, delegate(float x)
		{
			ActiveBaseCG.alpha = x;
		}, 1f, 0.5f).SetEase(Ease.Linear);
		showActiveBase.Pause();
		showActiveBase.SetAutoKill(autoKillOnCompletion: false);
		hideActiveBase = DOTween.To(() => ActiveBaseCG.alpha, delegate(float x)
		{
			ActiveBaseCG.alpha = x;
		}, 0f, 0.35f).SetEase(Ease.Linear);
		hideActiveBase.Pause();
		hideActiveBase.SetAutoKill(autoKillOnCompletion: false);
		hideMeSeq = DOTween.Sequence().OnComplete(finalClear);
		hideMeSeq.Insert(0f, DOTween.To(() => CardinalConnectorsCG.alpha, delegate(float x)
		{
			CardinalConnectorsCG.alpha = x;
		}, 0f, 0.15f).SetEase(Ease.Linear));
		hideMeSeq.Insert(0f, DOTween.To(() => NonCardinalConnectorsCG.alpha, delegate(float x)
		{
			NonCardinalConnectorsCG.alpha = x;
		}, 0f, 0.15f).SetEase(Ease.Linear));
		hideMeSeq.Insert(0f, DOTween.To(() => MyCG.alpha, delegate(float x)
		{
			MyCG.alpha = x;
		}, 0f, 0.25f).SetEase(Ease.Linear));
		hideMeSeq.Pause();
		hideMeSeq.SetAutoKill(autoKillOnCompletion: false);
		for (int num = 0; num < InteractObjects.Length; num++)
		{
			InteractObjects[num].SetNodeHexDirection += setMyDirection;
			InteractObjects[num].CounterDirectionMouseEnter += counterNodeDirectionMouseOver;
			InteractObjects[num].CounterDirectionMouseExit += counterNodeDirectionMouseExit;
		}
	}

	private void Update()
	{
		if (fireNextNodeActive && Time.time - fireNextNodeTimeStamp >= 0.15f)
		{
			fireNextNodeActive = false;
			if (connectingNode != null)
			{
				connectingNode.Activate(Type);
			}
		}
		if (presentMeActivated && Time.time - presentMeTimeStamp >= presentMeDelay)
		{
			presentMeActivated = false;
			presentMeTween.Restart();
		}
	}

	private void OnDestroy()
	{
		for (int i = 0; i < InteractObjects.Length; i++)
		{
			InteractObjects[i].SetNodeHexDirection -= setMyDirection;
			InteractObjects[i].CounterDirectionMouseEnter -= counterNodeDirectionMouseOver;
			InteractObjects[i].CounterDirectionMouseExit -= counterNodeDirectionMouseExit;
		}
	}

	public void Clear()
	{
		SetNonPlayable();
		if (CardinalConnectorsCG.alpha == 1f || NonCardinalConnectorsCG.alpha == 1f)
		{
			hideMeSeq.Restart();
		}
		else
		{
			hideMeTween.Restart();
		}
		Type = HACK_NODE_HEXER_NODE_TYPE.DEAD;
		myDirection = MATRIX_STACK_CLOCK_POSITION.NEUTRAL;
		WasConnected = false;
		IsReadyToConnect = false;
	}

	public void PoolBuild(NodeHexerHack SetNodeHexerHack)
	{
		myNodeHexerHack = SetNodeHexerHack;
	}

	public void SoftBuild(MatrixStackCord SetCord, HACK_NODE_HEXER_NODE_TYPE SetType)
	{
		MyCord = SetCord;
		Type = SetType;
		myDirection = MATRIX_STACK_CLOCK_POSITION.NEUTRAL;
	}

	public void ChangeType(HACK_NODE_HEXER_NODE_TYPE NewType)
	{
		Type = NewType;
	}

	public void Build(int MatrixSize, int MyIndex, bool NeedsToBeTagged = false)
	{
		myDirection = MATRIX_STACK_CLOCK_POSITION.NEUTRAL;
		myPOS.x = (float)MyCord.X * 50f;
		myPOS.y = 0f - (float)MyCord.Y * 50f;
		if (MyCord.X != 0)
		{
			myPOS.x += 10f * (float)MyCord.X;
		}
		if (MyCord.Y != 0)
		{
			myPOS.y -= 10f * (float)MyCord.Y;
		}
		MyRT.anchoredPosition = myPOS;
		switch (Type)
		{
		case HACK_NODE_HEXER_NODE_TYPE.BETA:
			TypeBetaCG.alpha = 0.35f;
			break;
		case HACK_NODE_HEXER_NODE_TYPE.ALPHA:
			TypeAlphaCG.alpha = 0.35f;
			break;
		}
		if (NeedsToBeTagged)
		{
			needsToBeTagged = true;
		}
		presentMeDelay = (float)MyIndex * 0.2f;
		presentMeTimeStamp = Time.time;
		presentMeActivated = true;
	}

	public void ActivateNeedsToBeTagged(int index)
	{
		showActiveBase.Restart(includeDelay: true, (float)index * 0.3f);
		GameManager.TimeSlinger.FireTimer((float)index * 0.3f, delegate
		{
			GameManager.AudioSlinger.PlaySound(ShowNeedsToBeTaggedSFX);
		});
	}

	public void Activate(HACK_NODE_HEXER_NODE_TYPE LastType)
	{
		if (LastType == Type)
		{
			myNodeHexerHack.ForceGameOver();
			return;
		}
		bool flag = true;
		if (needsToBeTagged)
		{
			needsToBeTagged = false;
			hideActiveBase.Restart();
			if (myNodeHexerHack.IsGameWon())
			{
				flag = false;
			}
		}
		if (!flag)
		{
			return;
		}
		if (myDirection == MATRIX_STACK_CLOCK_POSITION.NEUTRAL)
		{
			myNodeHexerHack.ForceGameOver();
		}
		else if (WasConnected)
		{
			myNodeHexerHack.ForceGameOver();
		}
		else if (myNodeHexerHack.CurrentMatrix.TryAndGetValueByClock(out connectingNode, MyCord, myDirection))
		{
			WasConnected = true;
			CardinalConnectorsCG.alpha = 1f;
			NonCardinalConnectorsCG.alpha = 1f;
			switch (Type)
			{
			case HACK_NODE_HEXER_NODE_TYPE.BETA:
				TypeBetaCG.alpha = 1f;
				break;
			case HACK_NODE_HEXER_NODE_TYPE.ALPHA:
				TypeAlphaCG.alpha = 1f;
				break;
			}
			switch (myDirection)
			{
			case MATRIX_STACK_CLOCK_POSITION.NOON:
				hand12OnTween.Restart();
				break;
			case MATRIX_STACK_CLOCK_POSITION.ONE:
				hand1OnTween.Restart();
				break;
			case MATRIX_STACK_CLOCK_POSITION.THREE:
				hand3OnTween.Restart();
				break;
			case MATRIX_STACK_CLOCK_POSITION.FOUR:
				hand4OnTween.Restart();
				break;
			case MATRIX_STACK_CLOCK_POSITION.SIX:
				hand6OnTween.Restart();
				break;
			case MATRIX_STACK_CLOCK_POSITION.SEVEN:
				hand7OnTween.Restart();
				break;
			case MATRIX_STACK_CLOCK_POSITION.NINE:
				hand9OnTween.Restart();
				break;
			case MATRIX_STACK_CLOCK_POSITION.TEN:
				hand10OnTween.Restart();
				break;
			}
			GameManager.AudioSlinger.PlaySound(ConnectSFX);
			fireNextNodeTimeStamp = Time.time;
			fireNextNodeActive = true;
		}
		else
		{
			myNodeHexerHack.ForceGameOver();
		}
	}

	public void HoverCounterDirection(MATRIX_STACK_CLOCK_POSITION CounterPOS)
	{
		switch (CounterPOS)
		{
		case MATRIX_STACK_CLOCK_POSITION.NOON:
			InteractObjects[4].CounterDirectionMouseOver();
			break;
		case MATRIX_STACK_CLOCK_POSITION.ONE:
			InteractObjects[5].CounterDirectionMouseOver();
			break;
		case MATRIX_STACK_CLOCK_POSITION.THREE:
			InteractObjects[6].CounterDirectionMouseOver();
			break;
		case MATRIX_STACK_CLOCK_POSITION.FOUR:
			InteractObjects[7].CounterDirectionMouseOver();
			break;
		case MATRIX_STACK_CLOCK_POSITION.SIX:
			InteractObjects[0].CounterDirectionMouseOver();
			break;
		case MATRIX_STACK_CLOCK_POSITION.SEVEN:
			InteractObjects[1].CounterDirectionMouseOver();
			break;
		case MATRIX_STACK_CLOCK_POSITION.NINE:
			InteractObjects[2].CounterDirectionMouseOver();
			break;
		case MATRIX_STACK_CLOCK_POSITION.TEN:
			InteractObjects[3].CounterDirectionMouseOver();
			break;
		}
	}

	public void ExitCounterDirection(MATRIX_STACK_CLOCK_POSITION CounterPOS)
	{
		switch (CounterPOS)
		{
		case MATRIX_STACK_CLOCK_POSITION.NOON:
			InteractObjects[4].CounterDirectionMouseOut();
			break;
		case MATRIX_STACK_CLOCK_POSITION.ONE:
			InteractObjects[5].CounterDirectionMouseOut();
			break;
		case MATRIX_STACK_CLOCK_POSITION.THREE:
			InteractObjects[6].CounterDirectionMouseOut();
			break;
		case MATRIX_STACK_CLOCK_POSITION.FOUR:
			InteractObjects[7].CounterDirectionMouseOut();
			break;
		case MATRIX_STACK_CLOCK_POSITION.SIX:
			InteractObjects[0].CounterDirectionMouseOut();
			break;
		case MATRIX_STACK_CLOCK_POSITION.SEVEN:
			InteractObjects[1].CounterDirectionMouseOut();
			break;
		case MATRIX_STACK_CLOCK_POSITION.NINE:
			InteractObjects[2].CounterDirectionMouseOut();
			break;
		case MATRIX_STACK_CLOCK_POSITION.TEN:
			InteractObjects[3].CounterDirectionMouseOut();
			break;
		}
	}

	public void ActivateCounterDirection(MATRIX_STACK_CLOCK_POSITION CounterPOS)
	{
		SetPlayable();
		switch (CounterPOS)
		{
		case MATRIX_STACK_CLOCK_POSITION.NOON:
			InteractObjects[4].ActivateCounterDirection();
			break;
		case MATRIX_STACK_CLOCK_POSITION.ONE:
			InteractObjects[5].ActivateCounterDirection();
			break;
		case MATRIX_STACK_CLOCK_POSITION.THREE:
			InteractObjects[6].ActivateCounterDirection();
			break;
		case MATRIX_STACK_CLOCK_POSITION.FOUR:
			InteractObjects[7].ActivateCounterDirection();
			break;
		case MATRIX_STACK_CLOCK_POSITION.SIX:
			InteractObjects[0].ActivateCounterDirection();
			break;
		case MATRIX_STACK_CLOCK_POSITION.SEVEN:
			InteractObjects[1].ActivateCounterDirection();
			break;
		case MATRIX_STACK_CLOCK_POSITION.NINE:
			InteractObjects[2].ActivateCounterDirection();
			break;
		case MATRIX_STACK_CLOCK_POSITION.TEN:
			InteractObjects[3].ActivateCounterDirection();
			break;
		}
	}

	public void DeActivateCounterDirection()
	{
		IsReadyToConnect = false;
		SetNonPlayable();
		for (int i = 0; i < InteractObjects.Length; i++)
		{
			InteractObjects[i].DeActivateCounterDirection();
			InteractObjects[i].ClearState();
		}
		if (connectingNode != null)
		{
			connectingNode.DeActivateCounterDirection();
			connectingNode = null;
		}
	}

	public void SetPlayable()
	{
		hideNonPlayableCG.Restart();
		showBaseCG.Restart();
		for (int i = 0; i < InteractObjects.Length; i++)
		{
			InteractObjects[i].Playable = true;
		}
	}

	public void SetNonPlayable()
	{
		hideBaseCG.Restart();
		showNonPlayableCG.Restart();
		for (int i = 0; i < InteractObjects.Length; i++)
		{
			InteractObjects[i].Playable = false;
		}
	}

	private bool setMyDirection(MATRIX_STACK_CLOCK_POSITION SetPOS)
	{
		if (!myNodeHexerHack.CurrentMatrix.TryAndGetValueByClock(out var ReturnValue, MyCord, SetPOS))
		{
			return false;
		}
		if (!ReturnValue.IsReadyToConnect)
		{
			GameManager.AudioSlinger.PlaySound(ClickDirectionSFX);
			myDirection = SetPOS;
			myNodeHexerHack.TotalMoves++;
			for (int i = 0; i < InteractObjects.Length; i++)
			{
				InteractObjects[i].ClearState();
			}
			if (SetPOS == MATRIX_STACK_CLOCK_POSITION.NEUTRAL)
			{
				connectingNode.DeActivateCounterDirection();
				connectingNode = null;
				IsReadyToConnect = false;
			}
			else
			{
				if (connectingNode != null)
				{
					connectingNode.DeActivateCounterDirection();
					connectingNode = null;
				}
				if (myNodeHexerHack.CurrentMatrix.TryAndGetValueByClock(out connectingNode, MyCord, SetPOS))
				{
					connectingNode.ActivateCounterDirection(SetPOS);
				}
				myNodeHexerHack.AddTimeBoost(MyCord);
				IsReadyToConnect = true;
			}
			return true;
		}
		return false;
	}

	private void counterNodeDirectionMouseOver(MATRIX_STACK_CLOCK_POSITION NodeDirection)
	{
		GameManager.AudioSlinger.PlaySound(HoverDirectionSFX);
		if (myNodeHexerHack.CurrentMatrix.TryAndGetValueByClock(out var ReturnValue, MyCord, NodeDirection) && !ReturnValue.IsReadyToConnect)
		{
			ReturnValue.HoverCounterDirection(NodeDirection);
		}
	}

	private void counterNodeDirectionMouseExit(MATRIX_STACK_CLOCK_POSITION NodeDirection)
	{
		if (myNodeHexerHack.CurrentMatrix.TryAndGetValueByClock(out var ReturnValue, MyCord, NodeDirection) && !ReturnValue.IsReadyToConnect)
		{
			ReturnValue.ExitCounterDirection(NodeDirection);
		}
	}

	private void finalClear()
	{
		TypeAlphaCG.alpha = 0f;
		TypeBetaCG.alpha = 0f;
		Hand12On.fillAmount = 0f;
		Hand3On.fillAmount = 0f;
		Hand6On.fillAmount = 0f;
		Hand9On.fillAmount = 0f;
		Hand1On.anchoredPosition = Vector2.zero;
		Hand4On.anchoredPosition = Vector2.zero;
		Hand7On.anchoredPosition = Vector2.zero;
		Hand10On.anchoredPosition = Vector2.zero;
		myPOS = Vector2.zero;
		MyRT.anchoredPosition = myPOS;
		for (int i = 0; i < InteractObjects.Length; i++)
		{
			InteractObjects[i].DeActivateCounterDirection();
			InteractObjects[i].ClearState();
		}
		needsToBeTagged = false;
		ActiveBaseCG.alpha = 0f;
		connectingNode = null;
		fireNextNodeActive = false;
		fireNextNodeTimeStamp = 0f;
		CardinalConnectorsCG.alpha = 0f;
		NonCardinalConnectorsCG.alpha = 0f;
	}
}
