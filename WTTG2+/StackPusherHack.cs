using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StackPusherHack : MonoBehaviour
{
	public int GRID_BLOCK_POOL_START_COUNT = 169;

	public HACK_STACK_PUSHER_STATE State;

	public StackPusherGridBlockObject CurrentPushingStackBlock;

	public GraphicRaycaster MyRayCaster;

	public GameObject StackPusherContent;

	public GameObject GridHolder;

	public GameObject GridBlockObject;

	public StackPusherGridPoperObject GridPoper;

	public StackPusherGridPusherObject GridPusher;

	public AudioFileDefinition PresentStackSFX;

	public AudioFileDefinition DismissStackSFX;

	public List<StackPusherLevelDefinition> StackPusherLevels;

	private StackPusherGridBlockObject.SelfPassActions clearGridBlockAction;

	private Sequence clearStackPusherSeq;

	private MatrixStackCord currentCenterCord = new MatrixStackCord(0, 0);

	private int currentLevelIndex;

	private MatrixStack<StackPusherGridBlockObject> currentMatrix = new MatrixStack<StackPusherGridBlockObject>();

	private Tweener fadeinGridBlockHolderTween;

	private float gameTime;

	private bool gameWon;

	private CanvasGroup gridBlockHolderCG;

	private PooledStack<StackPusherGridBlockObject> gridBlockPool;

	private Vector2 gridContentSize = Vector2.zero;

	private int matrixSizeFixNumber;

	private StackPusherHackData myData;

	private int myID;

	private Sequence presentStackPusherSeq;

	private Sequence presentStackPusherSeqSmall;

	private Dictionary<MatrixStackCord, HACK_STACK_PUSHER_GRID_BLOCK_STATE> specialBlockTypes = new Dictionary<MatrixStackCord, HACK_STACK_PUSHER_GRID_BLOCK_STATE>(MatrixStackCordCompare.Ins);

	private MatrixStackCord specialPeicePicker = new MatrixStackCord(0, 0);

	private int stackPopIndex;

	private CanvasGroup stackPusherContentCG;

	private Vector3 stackPusherContentMaxScale = Vector3.one;

	private Vector3 stackPusherContentMinScale = new Vector3(0.1f, 0.1f, 1f);

	private RectTransform stackPusherContentRT;

	private Vector2 stackPusherContentSize = Vector2.zero;

	private TerminalLineObject termLine1;

	private TerminalLineObject termLine2;

	private TerminalLineObject termLine3;

	private bool waitingForCustomHack;

	private void Awake()
	{
		gridBlockPool = new PooledStack<StackPusherGridBlockObject>(delegate
		{
			StackPusherGridBlockObject component = UnityEngine.Object.Instantiate(GridBlockObject, GridHolder.GetComponent<RectTransform>()).GetComponent<StackPusherGridBlockObject>();
			component.SoftBuild(this);
			return component;
		}, GRID_BLOCK_POOL_START_COUNT);
		stackPusherContentCG = StackPusherContent.GetComponent<CanvasGroup>();
		stackPusherContentRT = StackPusherContent.GetComponent<RectTransform>();
		gridBlockHolderCG = GridHolder.GetComponent<CanvasGroup>();
		clearGridBlockAction = clearGridBlock;
		GridPusher.ClearOldPushBlock += clearOldPushBlock;
		GridPusher.SetNewPushBlock += setNewPushBlock;
		GridPusher.ResetStacks += resetOldStacks;
		GridPusher.SetNewStacks += setNewStacks;
		fadeinGridBlockHolderTween = DOTween.To(() => gridBlockHolderCG.alpha, delegate(float x)
		{
			gridBlockHolderCG.alpha = x;
		}, 1f, 0.25f).SetEase(Ease.Linear);
		fadeinGridBlockHolderTween.Pause();
		fadeinGridBlockHolderTween.SetAutoKill(autoKillOnCompletion: false);
		presentStackPusherSeq = DOTween.Sequence().OnComplete(buildBlocks);
		presentStackPusherSeq.Insert(0f, DOTween.To(() => stackPusherContentCG.alpha, delegate(float x)
		{
			stackPusherContentCG.alpha = x;
		}, 1f, 0.25f).SetEase(Ease.Linear));
		presentStackPusherSeq.Insert(0f, DOTween.To(() => stackPusherContentRT.localScale, delegate(Vector3 x)
		{
			stackPusherContentRT.localScale = x;
		}, stackPusherContentMaxScale, 0.25f).SetEase(Ease.InCirc));
		presentStackPusherSeq.Pause();
		presentStackPusherSeq.SetAutoKill(autoKillOnCompletion: false);
		presentStackPusherSeqSmall = DOTween.Sequence().OnComplete(buildBlocks);
		presentStackPusherSeqSmall.Insert(0f, DOTween.To(() => stackPusherContentCG.alpha, delegate(float x)
		{
			stackPusherContentCG.alpha = x;
		}, 1f, 0.25f).SetEase(Ease.Linear));
		presentStackPusherSeqSmall.Insert(0f, DOTween.To(() => stackPusherContentRT.localScale, delegate(Vector3 x)
		{
			stackPusherContentRT.localScale = x;
		}, new Vector3(0.9f, 0.9f, 1f), 0.25f).SetEase(Ease.InCirc));
		presentStackPusherSeqSmall.Pause();
		presentStackPusherSeqSmall.SetAutoKill(autoKillOnCompletion: false);
		clearStackPusherSeq = DOTween.Sequence();
		clearStackPusherSeq.Insert(0f, DOTween.To(() => stackPusherContentCG.alpha, delegate(float x)
		{
			stackPusherContentCG.alpha = x;
		}, 0f, 0.25f).SetEase(Ease.Linear));
		clearStackPusherSeq.Insert(0f, DOTween.To(() => stackPusherContentRT.localScale, delegate(Vector3 x)
		{
			stackPusherContentRT.GetComponent<RectTransform>().localScale = x;
		}, stackPusherContentMinScale, 0.25f).SetEase(Ease.InCirc));
		clearStackPusherSeq.Pause();
		clearStackPusherSeq.SetAutoKill(autoKillOnCompletion: false);
		GameManager.StageManager.Stage += stageMe;
	}

	private void Start()
	{
		waitingForCustomHack = false;
		matrixSizeFixNumber = 0;
	}

	private void OnDestroy()
	{
		GridPusher.ClearOldPushBlock -= clearOldPushBlock;
		GridPusher.SetNewPushBlock -= setNewPushBlock;
		GridPusher.ResetStacks -= resetOldStacks;
		GridPusher.SetNewStacks -= setNewStacks;
	}

	public void PopStack()
	{
		stackPopIndex--;
		if (CurrentPushingStackBlock != null)
		{
			CurrentPushingStackBlock.Pop();
			CurrentPushingStackBlock = null;
		}
		if (stackPopIndex <= 0)
		{
			gameWon = true;
			clearGame();
		}
	}

	public void Boom()
	{
		gameWon = false;
		clearGame();
	}

	public void PrepStackPusherAttack(HACK_SWEEPER_SKILL_TIER SetTier)
	{
		setCurrentLevelIndex();
		switch (SetTier)
		{
		case HACK_SWEEPER_SKILL_TIER.TIER1:
			currentLevelIndex = ((currentLevelIndex != 0) ? (currentLevelIndex - 1) : 0);
			break;
		case HACK_SWEEPER_SKILL_TIER.TIER3:
			currentLevelIndex++;
			break;
		case HACK_SWEEPER_SKILL_TIER.TIER4:
			currentLevelIndex += 2;
			break;
		case HACK_SWEEPER_SKILL_TIER.TIER5:
			currentLevelIndex += 3;
			break;
		case HACK_SWEEPER_SKILL_TIER.GOD_TIER:
			currentLevelIndex = StackPusherLevels.Count - 1;
			break;
		case HACK_SWEEPER_SKILL_TIER.HACKER_MODE:
			currentLevelIndex = HackerModeSetSkill();
			break;
		}
		if (currentLevelIndex > StackPusherLevels.Count - 1)
		{
			currentLevelIndex = StackPusherLevels.Count - 1;
		}
		MyRayCaster.enabled = true;
		stackPusherContentCG.interactable = true;
		stackPusherContentCG.blocksRaycasts = true;
		stackPusherContentCG.ignoreParentGroups = true;
		GameManager.AudioSlinger.PlaySound(GameManager.HackerManager.HackingTypeSFX);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine1, TERMINAL_LINE_TYPE.TYPE, "> ./stackPUSHER", 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine2, TERMINAL_LINE_TYPE.TYPE, "  Loading stackPUSHER v1.5.20", 0.2f, 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine3, TERMINAL_LINE_TYPE.TYPE, "  Initialzing...", 0.2f, 0.4f);
		GameManager.TimeSlinger.FireTimer(0.6f, buildStackPusherAttack);
		Debug.Log("[Stack Pusher Attack] Current level:" + (currentLevelIndex + 1));
	}

	private void buildStackPusherAttack()
	{
		buildStackPusherAttackNew();
	}

	private void buildStackPusherAttackNew(bool IsCustom = false, CustomStackPusher csp = default(CustomStackPusher))
	{
		int matrixSize = StackPusherLevels[currentLevelIndex].MatrixSize;
		int stackPeices = StackPusherLevels[currentLevelIndex].StackPeices;
		int deadPeices = StackPusherLevels[currentLevelIndex].DeadPeices;
		if (IsCustom)
		{
			waitingForCustomHack = true;
			matrixSizeFixNumber = csp.MatrixSize;
			matrixSize = csp.MatrixSize;
			stackPeices = csp.StackPeices;
			deadPeices = csp.DeadPeices;
		}
		float num = (float)matrixSize * 40f;
		State = HACK_STACK_PUSHER_STATE.IDLE;
		gridContentSize.x = num;
		gridContentSize.y = num;
		stackPusherContentSize.x = num + 2f;
		stackPusherContentSize.y = num + 2f;
		StackPusherContent.GetComponent<RectTransform>().sizeDelta = stackPusherContentSize;
		GridHolder.GetComponent<RectTransform>().sizeDelta = gridContentSize;
		currentMatrix.SetMatrixSize(matrixSize);
		specialBlockTypes.Clear();
		List<int> grid = new List<int>(new int[matrixSize * matrixSize]);
		grid[(IsCustom && csp.RandomExit) ? UnityEngine.Random.Range(0, grid.Count) : ((matrixSize / 2 - UnityEngine.Random.Range(0, ((matrixSize % 2) ^ 1) + 1)) * matrixSize + (matrixSize / 2 - UnityEngine.Random.Range(0, ((matrixSize % 2) ^ 1) + 1)))] = 3;
		List<int> list = (from e in Enumerable.Range(0, grid.Count)
			where grid[e] == 0
			select e).ToList();
		ShuffleList(list);
		list.Take(stackPeices).ToList().ForEach(delegate(int e)
		{
			grid[e] = 1;
		});
		list = list.Skip(stackPeices).ToList();
		list.Take(deadPeices).ToList().ForEach(delegate(int e)
		{
			grid[e] = 2;
		});
		if (grid.All((int e) => e != 3))
		{
			int num2 = ((IsCustom && csp.RandomExit) ? UnityEngine.Random.Range(0, grid.Count) : ((matrixSize / 2 - UnityEngine.Random.Range(0, ((matrixSize % 2) ^ 1) + 1)) * matrixSize + (matrixSize / 2 - UnityEngine.Random.Range(0, ((matrixSize % 2) ^ 1) + 1))));
			grid[num2] = 3;
			Debug.Log($"[FiercePusher] [#1] (No popper nodes) Set node {num2} to popper");
		}
		List<int> list2 = (from i in Enumerable.Range(0, grid.Count)
			where grid[i] == 3
			select i).ToList();
		List<int> list3 = list2.SelectMany((int e) => from v in FindNeighbors(e, matrixSize)
			where grid[v] != 3
			select v).Distinct().ToList();
		List<int> list4 = list3.Where((int e) => grid[e] == 2).ToList();
		ShuffleList(list4);
		list4.Take(Math.Max(0, (int)Math.Ceiling(((double)list4.Count / (double)list3.Count - 0.7) * (double)list4.Count))).ToList().ForEach(delegate(int e)
		{
			grid[e] = 0;
			Debug.Log($"[FiercePusher] [#1] (Over skull limit) Set node {e} to empty");
		});
		if (list2.Count != 0 && !list3.Any((int e) => grid[e] < 2 && FindNeighbors(e, matrixSize).Any((int v) => grid[v] < 2)))
		{
			List<int> list5 = list3.Where((int e) => grid[e] == 0).ToList();
			ShuffleList(list5);
			List<int> list6 = (from e in FindNeighbors(list5[0], matrixSize)
				where grid[e] == 2
				select e).ToList();
			ShuffleList(list6);
			Debug.Log($"[FiercePusher] [#1] (No reachable nodes) Set node {list6[0]} to empty");
			grid[list6[0]] = 0;
		}
		if (grid.All((int e) => e != 1))
		{
			List<int> list7 = (from i in Enumerable.Range(0, grid.Count)
				where grid[i] == 0
				select i).ToList();
			ShuffleList(list7);
			grid[list7[0]] = 1;
			Debug.Log($"[FiercePusher] [#1] (No packet nodes) Set node {list7[0]} to packet");
		}
		var anon = new
		{
			pool = new List<int>(),
			seen = new List<int>()
		};
		var data1 = new
		{
			pool = new List<int>(),
			seen = new List<int>()
		};
		int num3 = 0;
		anon.pool.AddRange(list2);
		data1.seen.AddRange(list2);
		while (anon.pool.Count != 0 || num3 != 0)
		{
			for (int num4 = 0; num4 < anon.pool.Count; num4++)
			{
				int index = anon.pool[num4];
				data1.pool.AddRange(from v in FindNeighbors(index, matrixSize)
					where grid[v] != 2 && !data1.pool.Contains(v) && !data1.seen.Contains(v)
					select v);
			}
			anon.seen.AddRange(anon.pool);
			anon.pool.Clear();
			var anon2 = data1;
			anon = data1;
			data1 = anon2;
			num3 ^= 1;
		}
		List<int> reachable = anon.seen;
		List<int> list8 = (from i in Enumerable.Range(0, grid.Count)
			where grid[i] == 1 && !reachable.Contains(i)
			select i).ToList();
		if (list8.Count != 0)
		{
			List<int> list9 = reachable.Where((int e) => grid[e] == 0).ToList();
			ShuffleList(list9);
			foreach (int item in list8)
			{
				grid[item] = 0;
				if (list9.Count > 0)
				{
					grid[list9[0]] = 1;
					Debug.Log($"[FiercePusher] [#2] (Unreachable packet) Moved node {item} to node {list9[0]}");
					list9.RemoveAt(0);
				}
				else
				{
					Debug.Log($"[FiercePusher] [#2] (Unreachable packet) Set node {item} to empty");
				}
			}
		}
		List<int> list10 = reachable.Where((int e) => grid[e] == 1 && FindNeighbors(e, matrixSize).Any((int v) => reachable.Contains(v))).ToList();
		while (list10.Count != 0)
		{
			var jam = new
			{
				pool = new List<int>(),
				seen = new List<int>()
			};
			do
			{
				if (jam.pool.Count == 0)
				{
					jam.pool.Add(list10[0]);
					list10.RemoveAt(0);
				}
				List<int> collection = (from v in jam.pool.SelectMany((int e) => FindNeighbors(e, matrixSize)).Distinct()
					where grid[v] <= 1 && !jam.pool.Contains(v) && !jam.seen.Contains(v)
					select v).ToList();
				jam.seen.AddRange(jam.pool);
				jam.pool.Clear();
				jam.pool.AddRange(collection);
			}
			while (jam.pool.Count != 0);
			List<int> traversed = jam.seen;
			list10 = list10.Where((int e) => !traversed.Contains(e)).ToList();
			if (traversed.Where((int e) => grid[e] == 0).Any((int e) => FindNeighbors(e, matrixSize).Any((int v) => grid[v] == 0 || grid[v] == 3)))
			{
				continue;
			}
			List<int> list11 = traversed.Where((int e) => FindNeighbors(e, matrixSize).Any((int v) => grid[v] == 3)).ToList();
			ShuffleList(list11);
			Debug.Log($"[FiercePusher] [#3] (No moveable packet) Set node {list11[0]} to empty");
			grid[list11[0]] = 0;
		}
		List<int> list12 = (from e in Enumerable.Range(0, grid.Count)
			where grid[e] == 0
			select e).ToList();
		ShuffleList(list12);
		grid[list12[0]] = 4;
		stackPopIndex = 0;
		for (int num5 = 0; num5 < grid.Count; num5++)
		{
			specialPeicePicker.X = num5 % matrixSize;
			specialPeicePicker.Y = Mathf.FloorToInt(num5 / matrixSize);
			HACK_STACK_PUSHER_GRID_BLOCK_STATE value;
			switch (grid[num5])
			{
			case 1:
				value = (FindNeighbors(num5, matrixSize).Any((int v) => grid[v] == 4) ? HACK_STACK_PUSHER_GRID_BLOCK_STATE.STACK_READY : HACK_STACK_PUSHER_GRID_BLOCK_STATE.STACK_LOCKED);
				stackPopIndex++;
				break;
			case 2:
				value = HACK_STACK_PUSHER_GRID_BLOCK_STATE.DEAD;
				break;
			case 3:
				value = (FindNeighbors(num5, matrixSize).Any((int v) => grid[v] == 4) ? HACK_STACK_PUSHER_GRID_BLOCK_STATE.POPER_AND_ACTIVE : HACK_STACK_PUSHER_GRID_BLOCK_STATE.POPER);
				break;
			case 4:
				value = HACK_STACK_PUSHER_GRID_BLOCK_STATE.PUSHER;
				GridPusher.CurrentCord = specialPeicePicker;
				break;
			default:
				continue;
			}
			specialBlockTypes.Add(specialPeicePicker, value);
		}
		Debug.Log("[FiercePusher] Paper nodes: " + stackPopIndex);
		GameManager.AudioSlinger.PlaySound(PresentStackSFX);
		if (StackPusherContent.GetComponent<RectTransform>().sizeDelta.y >= 575f && Screen.height <= 800)
		{
			presentStackPusherSeqSmall.Restart();
		}
		else
		{
			presentStackPusherSeq.Restart();
		}
		if (IsCustom)
		{
			GameManager.HackerManager.HackingTimer.FireWarmUpTimer(csp.WarmUpTime);
			GameManager.TimeSlinger.FireTimer(csp.WarmUpTime, delegate
			{
				gameTime = (float)(csp.MatrixSize * csp.MatrixSize) * csp.TimePerPeice;
				GameManager.HackerManager.HackingTimer.FireHackingTimer(gameTime, timesUp);
				fadeinGridBlockHolderTween.Restart();
				gridBlockHolderCG.interactable = true;
				gridBlockHolderCG.ignoreParentGroups = true;
				gridBlockHolderCG.blocksRaycasts = true;
				if (stackPopIndex <= 0)
				{
					gameWon = true;
					clearGame();
				}
			});
		}
		else
		{
			GameManager.HackerManager.HackingTimer.FireWarmUpTimer(StackPusherLevels[currentLevelIndex].WarmUpTime);
			GameManager.TimeSlinger.FireTimer(StackPusherLevels[currentLevelIndex].WarmUpTime, beginGame);
		}
	}

	private void clearGridBlock(StackPusherGridBlockObject TheBlock)
	{
		TheBlock.Kill -= clearGridBlockAction;
		gridBlockPool.Push(TheBlock);
	}

	private void clearOldPushBlock(MatrixStackCord OldCord)
	{
		currentMatrix.Get(OldCord).State = HACK_STACK_PUSHER_GRID_BLOCK_STATE.ACTIVE;
	}

	private void setNewPushBlock(MatrixStackCord NewCord)
	{
		currentMatrix.Get(NewCord).State = HACK_STACK_PUSHER_GRID_BLOCK_STATE.PUSHER;
	}

	private void resetOldStacks(Dictionary<MatrixStackCord, short> OldCords)
	{
		foreach (KeyValuePair<MatrixStackCord, short> OldCord in OldCords)
		{
			if (currentMatrix.TryAndGetValue(out var ReturnValue, OldCord.Key))
			{
				ReturnValue.ClearStackState();
			}
		}
	}

	private void setNewStacks(Dictionary<MatrixStackCord, short> NewCords)
	{
		foreach (KeyValuePair<MatrixStackCord, short> NewCord in NewCords)
		{
			if (currentMatrix.TryAndGetValue(out var ReturnValue, NewCord.Key))
			{
				ReturnValue.SetStackStateReady();
			}
		}
	}

	private void buildBlocks()
	{
		int num = (waitingForCustomHack ? matrixSizeFixNumber : StackPusherLevels[currentLevelIndex].MatrixSize);
		for (int i = 0; i < num * num; i++)
		{
			StackPusherGridBlockObject stackPusherGridBlockObject = gridBlockPool.Pop();
			currentMatrix.Push(stackPusherGridBlockObject);
			if (specialBlockTypes.ContainsKey(currentMatrix.Pointer))
			{
				stackPusherGridBlockObject.Build(currentMatrix.Pointer, specialBlockTypes[currentMatrix.Pointer]);
			}
			else if (GridPusher.AmNextTo(currentMatrix.Pointer))
			{
				stackPusherGridBlockObject.Build(currentMatrix.Pointer, HACK_STACK_PUSHER_GRID_BLOCK_STATE.ACTIVE_AND_PUSHABLE);
			}
			else
			{
				stackPusherGridBlockObject.Build(currentMatrix.Pointer, HACK_STACK_PUSHER_GRID_BLOCK_STATE.ACTIVE);
			}
			stackPusherGridBlockObject.Kill += clearGridBlockAction;
		}
		waitingForCustomHack = false;
		matrixSizeFixNumber = 0;
	}

	private void beginGame()
	{
		gameTime = (float)(StackPusherLevels[currentLevelIndex].MatrixSize * StackPusherLevels[currentLevelIndex].MatrixSize) * StackPusherLevels[currentLevelIndex].TimePerPeice;
		GameManager.HackerManager.HackingTimer.FireHackingTimer(gameTime, timesUp);
		fadeinGridBlockHolderTween.Restart();
		gridBlockHolderCG.interactable = true;
		gridBlockHolderCG.ignoreParentGroups = true;
		gridBlockHolderCG.blocksRaycasts = true;
		if (stackPopIndex <= 0)
		{
			gameWon = true;
			clearGame();
		}
	}

	private void timesUp()
	{
		gameWon = false;
		clearGame();
	}

	private void clearGame()
	{
		GameManager.HackerManager.HackingTimer.CurrentHackerTimerObject.Pause();
		float timeLeft = GameManager.HackerManager.HackingTimer.CurrentHackerTimerObject.GetTimeLeft();
		MyRayCaster.enabled = false;
		stackPusherContentCG.interactable = false;
		stackPusherContentCG.blocksRaycasts = false;
		stackPusherContentCG.ignoreParentGroups = false;
		gridBlockHolderCG.interactable = false;
		gridBlockHolderCG.ignoreParentGroups = false;
		gridBlockHolderCG.blocksRaycasts = false;
		GridPoper.Clear();
		GridPusher.Clear();
		foreach (StackPusherGridBlockObject item in currentMatrix.GetAll())
		{
			if (item != null)
			{
				item.Destroy();
			}
		}
		currentMatrix.Clear();
		GameManager.TimeSlinger.FireTimer(0.15f, delegate
		{
			GameManager.AudioSlinger.PlaySound(DismissStackSFX);
			clearStackPusherSeq.Restart();
			GameManager.HackerManager.HackingTimer.KillHackerTimer();
			termLine1.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
			termLine2.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
			termLine3.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
			GameManager.TimeSlinger.FireTimer(0.4f, delegate
			{
				stackPusherContentCG.alpha = 0f;
				gridBlockHolderCG.alpha = 0.95f;
				GameManager.HackerManager.HackingTerminal.TerminalHelper.ClearTerminal();
				if (gameWon)
				{
					if (!DifficultyManager.HackerMode)
					{
						if (!DifficultyManager.LeetMode)
						{
							float num = timeLeft / gameTime;
							if (num >= 0.65f)
							{
								myData.SkillPoints += StackPusherLevels[currentLevelIndex].PointsRewaredTier1;
							}
							else if (num >= 0.45f && num <= 0.65f)
							{
								myData.SkillPoints += StackPusherLevels[currentLevelIndex].PointsRewaredTier1;
							}
							else if (num >= 0.25f && num <= 0.45f)
							{
								myData.SkillPoints += StackPusherLevels[currentLevelIndex].PointsRewaredTier2;
							}
							else
							{
								myData.SkillPoints += StackPusherLevels[currentLevelIndex].PointsRewaredTier2;
							}
							DataManager.Save(myData);
						}
						if (currentLevelIndex == StackPusherLevels.Count - 1)
						{
							SteamSlinger.Ins.UnlockSteamAchievement(STEAM_ACHIEVEMENT.POPPERPRO);
						}
					}
					else
					{
						float num2 = timeLeft / gameTime;
						if (num2 >= 0.65f)
						{
							HackerModeManager.Ins.SkillPoints += StackPusherLevels[currentLevelIndex].PointsRewaredTier1;
						}
						else if (num2 >= 0.45f && num2 <= 0.65f)
						{
							HackerModeManager.Ins.SkillPoints += StackPusherLevels[currentLevelIndex].PointsRewaredTier1;
						}
						else if (num2 >= 0.25f && num2 <= 0.45f)
						{
							HackerModeManager.Ins.SkillPoints += StackPusherLevels[currentLevelIndex].PointsRewaredTier2;
						}
						else
						{
							HackerModeManager.Ins.SkillPoints += StackPusherLevels[currentLevelIndex].PointsRewaredTier2;
						}
					}
					GameManager.HackerManager.PlayerWon(currentLevelIndex);
				}
				else
				{
					if (!DifficultyManager.HackerMode)
					{
						if (!DifficultyManager.LeetMode)
						{
							myData.SkillPoints -= StackPusherLevels[currentLevelIndex].PointsDeducted;
							DataManager.Save(myData);
						}
						if (currentLevelIndex == 0)
						{
							SteamSlinger.Ins.UnlockSteamAchievement(STEAM_ACHIEVEMENT.STACKOVERLOAD);
						}
					}
					GameManager.HackerManager.PlayerLost();
				}
			});
		});
	}

	private void setCurrentLevelIndex()
	{
		waitingForCustomHack = false;
		matrixSizeFixNumber = 0;
		for (int i = 0; i < StackPusherLevels.Count; i++)
		{
			if (myData.SkillPoints >= StackPusherLevels[i].SkillPointsRequired)
			{
				currentLevelIndex = i;
			}
			else
			{
				i = StackPusherLevels.Count;
			}
		}
	}

	private void stageMe()
	{
		myData = DataManager.Load<StackPusherHackData>(myID);
		if (myData == null)
		{
			myData = new StackPusherHackData(myID);
			myData.SkillPoints = 0;
		}
		if (DifficultyManager.LeetMode)
		{
			myData.SkillPoints = StackPusherLevels[StackPusherLevels.Count - 1].SkillPointsRequired;
		}
		GameManager.StageManager.Stage -= stageMe;
	}

	public void PrepStackPusherAttackTarot(int level)
	{
		setCurrentLevelIndex();
		currentLevelIndex = level;
		if (currentLevelIndex > StackPusherLevels.Count - 1)
		{
			currentLevelIndex = StackPusherLevels.Count - 1;
		}
		MyRayCaster.enabled = true;
		stackPusherContentCG.interactable = true;
		stackPusherContentCG.blocksRaycasts = true;
		stackPusherContentCG.ignoreParentGroups = true;
		GameManager.AudioSlinger.PlaySound(GameManager.HackerManager.HackingTypeSFX);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine1, TERMINAL_LINE_TYPE.TYPE, "> ./stackPUSHER", 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine2, TERMINAL_LINE_TYPE.TYPE, "  Loading stackPUSHER v1.5.20", 0.2f, 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine3, TERMINAL_LINE_TYPE.TYPE, "  Initialzing...", 0.2f, 0.4f);
		GameManager.TimeSlinger.FireTimer(0.6f, buildStackPusherAttack);
		Debug.Log("[Stack Pusher Attack] Current level:" + (currentLevelIndex + 1));
	}

	public void buildCustomStackPusherAttack(int MatrixSize, int StackPeices, int DeadPeices, float TimePerPeice, int WarmUpTime, bool RandomExit)
	{
		MyRayCaster.enabled = true;
		stackPusherContentCG.interactable = true;
		stackPusherContentCG.blocksRaycasts = true;
		stackPusherContentCG.ignoreParentGroups = true;
		GameManager.AudioSlinger.PlaySound(GameManager.HackerManager.HackingTypeSFX);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine1, TERMINAL_LINE_TYPE.TYPE, "> ./stackPUSHER", 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine2, TERMINAL_LINE_TYPE.TYPE, "  Loading stackPUSHER v1.5.20", 0.2f, 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine3, TERMINAL_LINE_TYPE.TYPE, "  Initialzing...", 0.2f, 0.4f);
		GameManager.TimeSlinger.FireTimer(0.6f, delegate
		{
			buildStackPusherAttackNew(IsCustom: true, new CustomStackPusher(MatrixSize, StackPeices, DeadPeices, TimePerPeice, WarmUpTime, RandomExit));
		});
	}

	public int HackerModeSetSkill()
	{
		int result = 0;
		for (int i = 0; i < StackPusherLevels.Count; i++)
		{
			if (HackerModeManager.Ins.SkillPoints >= StackPusherLevels[i].SkillPointsRequired)
			{
				result = i;
			}
			else
			{
				i = StackPusherLevels.Count;
			}
		}
		return result;
	}

	private void ShuffleList<T>(IList<T> list)
	{
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = UnityEngine.Random.Range(0, num + 1);
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}

	public static IEnumerable<int> FindNeighbors(int index, int length)
	{
		int[] array = new int[2]
		{
			index % length,
			(int)Math.Floor((decimal)index / (decimal)length)
		};
		int[,] array2 = new int[8, 2]
		{
			{ 1, 1 },
			{ 1, 0 },
			{ 1, -1 },
			{ 0, 1 },
			{ 0, -1 },
			{ -1, 1 },
			{ -1, 0 },
			{ -1, -1 }
		};
		List<int> list = new List<int>();
		for (int i = 0; i < 8; i++)
		{
			List<int> list2 = new List<int>
			{
				array2[i, 0] + array[0],
				array2[i, 1] + array[1]
			};
			if (list2.All((int e) => e >= 0 && e < length))
			{
				list.Add(list2[1] * length + list2[0]);
			}
		}
		return list;
	}
}
