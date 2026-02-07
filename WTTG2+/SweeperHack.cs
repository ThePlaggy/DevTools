using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class SweeperHack : MonoBehaviour
{
	private const float SLIDER_SPACING = 10f;

	private const float DOT_WIDTH = 11f;

	private const float TERM_BLOCK_BUFFER_WIDTH = 50f;

	private const float TERM_BLOCK_BUFFER_HEIGHT = 20f;

	private const float TERM_BLOCK_SLIDE_UP_TIME = 0.15f;

	public int SWEEPER_OBJ_POOL_COUNT = 10;

	public GameObject SweeperHackOverlay;

	public GameObject SweeperObject;

	public GameObject SweeperObjectsHolder;

	public GameObject TermBlock;

	public AudioFileDefinition TermBlockShowSFX;

	public AudioFileDefinition SweeperEnd;

	public List<SweeperObjectDefinition> SweeperLevels;

	private int curLevelIndex;

	private int curSweeperIndex;

	private List<SweeperObject> curSweeperObjects = new List<SweeperObject>(10);

	private Vector2 DefaultTermBlockSize = Vector2.zero;

	private SweeperObject.VoidActions fireNextSweeperAction;

	private bool gameIsActive;

	private bool isTestMode;

	private SweeperHackData myData;

	private int myID;

	private CanvasGroup sweeperHackOverlayCG;

	private RectTransform sweeperObjectHolderRT;

	private PooledStack<SweeperObject> sweeperObjectPool;

	private Vector2 sweeperObjectPOS = Vector2.zero;

	private RectTransform sweeperObjectRT;

	private CanvasGroup termBlockCG;

	private Tweener termBlockDismiss1;

	private Tweener termBlockDismiss2;

	private Tweener termBlockPresent1;

	private RectTransform termBlockRT;

	private Vector2 termBlockTargetHeight = Vector2.zero;

	private Vector2 termBlockTargetWidth = Vector2.zero;

	private TerminalLineObject termLine1;

	private TerminalLineObject termLine2;

	private TerminalLineObject termLine3;

	private Timer testTimer;

	private void Awake()
	{
		myID = base.transform.position.GetHashCode();
		termBlockCG = TermBlock.GetComponent<CanvasGroup>();
		sweeperHackOverlayCG = SweeperHackOverlay.GetComponent<CanvasGroup>();
		termBlockRT = TermBlock.GetComponent<RectTransform>();
		sweeperObjectRT = SweeperObject.GetComponent<RectTransform>();
		sweeperObjectHolderRT = SweeperObjectsHolder.GetComponent<RectTransform>();
		DefaultTermBlockSize = termBlockRT.sizeDelta;
		termBlockPresent1 = DOTween.To(() => termBlockCG.alpha, delegate(float x)
		{
			termBlockCG.alpha = x;
		}, 1f, 0.25f).SetEase(Ease.Linear);
		termBlockPresent1.Pause();
		termBlockPresent1.SetAutoKill(autoKillOnCompletion: false);
		termBlockDismiss1 = DOTween.To(() => termBlockRT.sizeDelta, delegate(Vector2 x)
		{
			termBlockRT.sizeDelta = x;
		}, DefaultTermBlockSize, 0.25f).SetEase(Ease.Linear);
		termBlockDismiss1.Pause();
		termBlockDismiss1.SetAutoKill(autoKillOnCompletion: false);
		termBlockDismiss2 = DOTween.To(() => termBlockCG.alpha, delegate(float x)
		{
			termBlockCG.alpha = x;
		}, 0f, 0.3f).SetEase(Ease.Linear).OnComplete(delegate
		{
			termBlockRT.pivot = new Vector2(0.5f, 0.5f);
			termBlockRT.anchoredPosition = Vector2.zero;
		});
		termBlockDismiss2.Pause();
		termBlockDismiss2.SetAutoKill(autoKillOnCompletion: false);
		SweeperHackOverlay.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
		SweeperHackOverlay.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		fireNextSweeperAction = FireNextSweeper;
		sweeperObjectPool = new PooledStack<SweeperObject>(() => Object.Instantiate(SweeperObject, sweeperObjectHolderRT).GetComponent<SweeperObject>(), SWEEPER_OBJ_POOL_COUNT);
		GameManager.StageManager.Stage += stageMe;
	}

	private void Update()
	{
		if (gameIsActive && StateManager.GameState == GAME_STATE.LIVE && computerController.Ins.Active && CrossPlatformInputManager.GetButtonDown("LeftClick") && curSweeperObjects.Count > curSweeperIndex && curSweeperObjects[curSweeperIndex] != null)
		{
			gameIsActive = false;
			curSweeperObjects[curSweeperIndex].PlayerHasDecided();
		}
	}

	public void ActivateMe()
	{
		sweeperHackOverlayCG.alpha = 1f;
		sweeperHackOverlayCG.interactable = true;
		sweeperHackOverlayCG.blocksRaycasts = true;
		sweeperHackOverlayCG.ignoreParentGroups = true;
	}

	public void DeActivateMe()
	{
		sweeperHackOverlayCG.alpha = 0f;
		sweeperHackOverlayCG.interactable = false;
		sweeperHackOverlayCG.blocksRaycasts = false;
		sweeperHackOverlayCG.ignoreParentGroups = false;
	}

	public void PrepSweepAttack(bool LeetMode)
	{
		setCurrentLevelIndex();
		if (LeetMode)
		{
			curLevelIndex = 11;
		}
		GameManager.AudioSlinger.PlaySound(GameManager.HackerManager.HackingTypeSFX);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine1, TERMINAL_LINE_TYPE.TYPE, "> ./ZONEWALL", 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine2, TERMINAL_LINE_TYPE.TYPE, "  Loading ZONEWALL v1.88.520", 0.2f, 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine3, TERMINAL_LINE_TYPE.TYPE, "  Initialzing...", 0.2f, 0.4f);
		termBlockTargetWidth.x = (float)SweeperLevels[curLevelIndex].NumOfDotsPerSweeper * 11f + 50f;
		termBlockTargetWidth.y = termBlockRT.sizeDelta.y;
		termBlockTargetHeight.x = (float)SweeperLevels[curLevelIndex].NumOfDotsPerSweeper * 11f + 50f;
		termBlockTargetHeight.y = (float)SweeperLevels[curLevelIndex].NumOfSweepers * sweeperObjectRT.sizeDelta.y + (float)SweeperLevels[curLevelIndex].NumOfSweepers * 10f + 20f;
		GameManager.TimeSlinger.FireTimer(0.6f, delegate
		{
			GameManager.AudioSlinger.PlaySound(TermBlockShowSFX);
			termBlockPresent1.Restart();
			DOTween.To(() => termBlockRT.sizeDelta, delegate(Vector2 x)
			{
				termBlockRT.sizeDelta = x;
			}, termBlockTargetWidth, 0.35f).SetEase(Ease.Linear).SetDelay(0.25f);
			DOTween.To(() => termBlockRT.sizeDelta, delegate(Vector2 x)
			{
				termBlockRT.sizeDelta = x;
			}, termBlockTargetHeight, 0.35f).SetEase(Ease.Linear).SetDelay(0.6f)
				.OnComplete(BuildSweepAttack);
		});
	}

	public void BuildSweepAttack()
	{
		Vector2 zero = Vector2.zero;
		termBlockRT.pivot = new Vector2(0.5f, 1f);
		termBlockRT.anchoredPosition = new Vector2(0f, termBlockRT.sizeDelta.y / 2f);
		sweeperObjectHolderRT.sizeDelta = new Vector2(sweeperObjectRT.sizeDelta.x, (float)SweeperLevels[curLevelIndex].NumOfSweepers * sweeperObjectRT.sizeDelta.y + (float)SweeperLevels[curLevelIndex].NumOfSweepers * 10f);
		for (int i = 0; i < SweeperLevels[curLevelIndex].NumOfSweepers; i++)
		{
			SweeperObject sweeperObject = sweeperObjectPool.Pop();
			sweeperObject.IWasDismissed += fireNextSweeperAction;
			sweeperObject.WarmMe(i, SweeperLevels[curLevelIndex]);
			sweeperObject.gameObject.GetComponent<RectTransform>().anchoredPosition = zero;
			curSweeperObjects.Add(sweeperObject);
			zero.y = zero.y - sweeperObjectRT.sizeDelta.y - 10f;
		}
		curSweeperIndex = 0;
		GameManager.HackerManager.HackingTimer.FireWarmUpTimer(SweeperLevels[curLevelIndex].WarmUpTime);
		GameManager.TimeSlinger.FireTimer(SweeperLevels[curLevelIndex].WarmUpTime, BeginGame);
	}

	private void BeginGame()
	{
		float setDur = (float)SweeperLevels[curLevelIndex].NumOfSweepsPerSweeper * SweeperLevels[curLevelIndex].ScrollSpeedMax + (float)SweeperLevels[curLevelIndex].NumOfSweepers + SweeperLevels[curLevelIndex].BufferTime;
		GameManager.HackerManager.HackingTimer.FireHackingTimer(setDur, TimesUp);
		curSweeperObjects[curSweeperIndex].FireMe();
		gameIsActive = true;
	}

	private void TimesUp()
	{
		gameIsActive = false;
		for (int i = 0; i < curSweeperObjects.Count; i++)
		{
			curSweeperObjects[i].ForceEnd();
		}
		GameManager.TimeSlinger.FireTimer(0.15f, ForceFinishGame);
	}

	private void FireNextSweeper()
	{
		curSweeperIndex++;
		if (curSweeperIndex < curSweeperObjects.Count)
		{
			for (int i = curSweeperIndex; i < curSweeperObjects.Count; i++)
			{
				curSweeperObjects[i].MoveMeUp();
			}
			DOTween.To(() => termBlockRT.sizeDelta, delegate(Vector2 x)
			{
				termBlockRT.sizeDelta = x;
			}, new Vector2(0f, 0f - (sweeperObjectRT.sizeDelta.y + 10f)), 0.15f).SetEase(Ease.Linear).SetRelative(isRelative: true)
				.SetDelay(0.15f)
				.OnComplete(delegate
				{
					curSweeperObjects[curSweeperIndex].FireMe();
					gameIsActive = true;
				});
		}
		else
		{
			GameManager.HackerManager.HackingTimer.KillHackerTimer();
			DOTween.To(() => termBlockRT.sizeDelta, delegate(Vector2 x)
			{
				termBlockRT.sizeDelta = x;
			}, new Vector2(0f, 0f - (sweeperObjectRT.sizeDelta.y + 10f)), 0.15f).SetEase(Ease.Linear).SetRelative(isRelative: true)
				.SetDelay(0.15f)
				.OnComplete(delegate
				{
					FinishGame();
				});
		}
	}

	private void ForceFinishGame()
	{
		for (int i = 0; i < curSweeperObjects.Count; i++)
		{
			curSweeperObjects[i].IWasDismissed -= fireNextSweeperAction;
			curSweeperObjects[i].DestroyMe();
			sweeperObjectPool.Push(curSweeperObjects[i]);
		}
		curSweeperObjects.Clear();
		ClearTerm();
		GameManager.HackerManager.HackingTimer.KillHackerTimer();
		termLine1.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
		termLine2.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
		termLine3.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
		GameManager.TimeSlinger.FireTimer(0.4f, tallyPlayerPoints, 0);
	}

	private void FinishGame()
	{
		int num = 0;
		for (int i = 0; i < curSweeperObjects.Count; i++)
		{
			num += curSweeperObjects[i].MyScore;
			curSweeperObjects[i].IWasDismissed -= fireNextSweeperAction;
			curSweeperObjects[i].DestroyMe();
			sweeperObjectPool.Push(curSweeperObjects[i]);
		}
		curSweeperObjects.Clear();
		ClearTerm();
		GameManager.HackerManager.HackingTimer.KillHackerTimer();
		termLine1.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
		termLine2.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
		termLine3.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
		GameManager.TimeSlinger.FireTimer(0.4f, tallyPlayerPoints, num);
	}

	private void ClearTerm()
	{
		GameManager.AudioSlinger.PlaySound(SweeperEnd);
		termBlockDismiss1.Restart();
		termBlockDismiss2.Restart();
	}

	private void tallyPlayerPoints(int scoreCount)
	{
		GameManager.HackerManager.HackingTerminal.TerminalHelper.ClearTerminal();
		if (!isTestMode)
		{
			int num = 5 * SweeperLevels[curLevelIndex].NumOfSweepers;
			HACK_SWEEPER_SKILL_TIER setTier;
			if (scoreCount == 0)
			{
				myData.SkillPoints -= SweeperLevels[curLevelIndex].PointsDeducted;
				setTier = HACK_SWEEPER_SKILL_TIER.TIER5;
			}
			else if ((float)scoreCount >= (float)num * 0.9f)
			{
				myData.SkillPoints += SweeperLevels[curLevelIndex].PointsRewaredTier1;
				setTier = ((!((float)scoreCount >= (float)num * 0.95f)) ? HACK_SWEEPER_SKILL_TIER.TIER1 : HACK_SWEEPER_SKILL_TIER.INSTABLOCK);
			}
			else if ((float)scoreCount >= (float)num * 0.75f && (float)scoreCount < (float)num * 0.9f)
			{
				myData.SkillPoints += SweeperLevels[curLevelIndex].PointsRewaredTier1;
				setTier = HACK_SWEEPER_SKILL_TIER.TIER2;
			}
			else if ((float)scoreCount >= (float)num * 0.5f && (float)scoreCount < (float)num * 0.75f)
			{
				myData.SkillPoints += SweeperLevels[curLevelIndex].PointsRewaredTier2;
				setTier = HACK_SWEEPER_SKILL_TIER.TIER3;
			}
			else
			{
				myData.SkillPoints += SweeperLevels[curLevelIndex].PointsRewaredTier2;
				setTier = HACK_SWEEPER_SKILL_TIER.TIER4;
			}
			DataManager.Save(myData);
			GameManager.HackerManager.ProcessSweepAttack(setTier);
		}
		else
		{
			GameManager.HackerManager.ProcessSweepAttack(HACK_SWEEPER_SKILL_TIER.TIER4);
		}
		isTestMode = false;
	}

	private void setCurrentLevelIndex()
	{
		curLevelIndex = GameManager.TimeKeeper.zonewallLevel;
	}

	private void stageMe()
	{
		myData = DataManager.Load<SweeperHackData>(myID);
		if (myData == null)
		{
			myData = new SweeperHackData(myID);
			myData.SkillPoints = 0;
		}
		GameManager.StageManager.Stage -= stageMe;
	}
}
