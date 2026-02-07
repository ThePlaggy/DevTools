using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeHexerHack : MonoBehaviour
{
	public int START_NODE_HEX_OBJECT_POOL_COUNT = 36;

	public int TotalMoves;

	public GraphicRaycaster MyRayCaster;

	public GameObject NodeHexObject;

	public NodeHexStartArrowObject StartArrow;

	public CanvasGroup NodeHexerContentCG;

	public RectTransform NodeHexerContentRT;

	public RectTransform NodeHolderRT;

	public List<NodeHexerLevelDefinition> NodeHexerLevels;

	public AudioFileDefinition ShowRowSFX;

	public AudioFileDefinition TimeBoostSFX;

	public AudioFileDefinition ForceGameOverSFX;

	public AudioFileDefinition GameWonSFX;

	private int currentLevelIndex;

	private int currentTagPieces;

	private bool gameWon;

	private MatrixStackCord hexTypePicker = new MatrixStackCord(0, 0);

	private NodeHexerHackData myData;

	private int myID;

	private PooledStack<NodeHexObject> nodeHexObjectPool;

	private Vector2 nodeHolderSize = Vector2.zero;

	private Dictionary<MatrixStackCord, bool> nodesThatNeedToBeTagged = new Dictionary<MatrixStackCord, bool>(MatrixStackCordCompare.Ins);

	private List<MatrixStackCord> stackCords = new List<MatrixStackCord>();

	private NodeHexObject startNode;

	private float StartTimeFix;

	private TerminalLineObject termLine1;

	private TerminalLineObject termLine2;

	private TerminalLineObject termLine3;

	private float timeBoost;

	private bool waitingForCustomHack;

	public MatrixStack<NodeHexObject> CurrentMatrix { get; } = new MatrixStack<NodeHexObject>();

	private void Awake()
	{
		nodeHexObjectPool = new PooledStack<NodeHexObject>(delegate
		{
			NodeHexObject component = Object.Instantiate(NodeHexObject, NodeHolderRT).GetComponent<NodeHexObject>();
			component.PoolBuild(this);
			return component;
		}, START_NODE_HEX_OBJECT_POOL_COUNT);
		GameManager.StageManager.Stage += stageMe;
	}

	private void Start()
	{
		waitingForCustomHack = false;
		StartTimeFix = 0f;
	}

	public bool IsGameWon()
	{
		currentTagPieces--;
		if (currentTagPieces <= 0)
		{
			GameManager.AudioSlinger.PlaySound(GameWonSFX);
			gameWon = true;
			clearGame();
			return true;
		}
		return false;
	}

	public void ForceGameOver()
	{
		GameManager.AudioSlinger.PlaySound(ForceGameOverSFX);
		gameWon = false;
		clearGame();
	}

	public void PrepNodeHexAttack(HACK_SWEEPER_SKILL_TIER SetTier)
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
			currentLevelIndex = NodeHexerLevels.Count - 1;
			break;
		case HACK_SWEEPER_SKILL_TIER.HACKER_MODE:
			currentLevelIndex = HackerModeSetSkill();
			break;
		}
		if (currentLevelIndex > NodeHexerLevels.Count - 1)
		{
			currentLevelIndex = NodeHexerLevels.Count - 1;
		}
		MyRayCaster.enabled = true;
		NodeHexerContentCG.interactable = true;
		NodeHexerContentCG.blocksRaycasts = true;
		NodeHexerContentCG.ignoreParentGroups = true;
		GameManager.AudioSlinger.PlaySound(GameManager.HackerManager.HackingTypeSFX);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine1, TERMINAL_LINE_TYPE.TYPE, "> ./N0D3H3X3R", 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine2, TERMINAL_LINE_TYPE.TYPE, "  Loading N0D3H3X3R v6.13.70", 0.2f, 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine3, TERMINAL_LINE_TYPE.TYPE, "  Initialzing...", 0.2f, 0.4f);
		GameManager.TimeSlinger.FireTimer(0.6f, buildNodeHexAttack);
		Debug.Log("[Node Hexer Attack] Current level:" + (currentLevelIndex + 1));
	}

	public void AddTimeBoost(MatrixStackCord TagCord)
	{
		if (!nodesThatNeedToBeTagged.ContainsKey(TagCord))
		{
			return;
		}
		nodesThatNeedToBeTagged.Remove(TagCord);
		if (GameManager.HackerManager.HackingTimer.CurrentHackerTimerObject.HackerTimeTimer != null)
		{
			GameManager.AudioSlinger.PlaySound(TimeBoostSFX);
			float num = GameManager.HackerManager.HackingTimer.CurrentHackerTimerObject.GetTimeLeft() + timeBoost;
			if (HMCustomHack.IsCustomHack && num > 15f)
			{
				num = 15f;
			}
			GameManager.HackerManager.HackingTimer.CurrentHackerTimerObject.ResetMe(num, firePath);
		}
	}

	private void buildNodeHexAttack()
	{
		int matrixSize = NodeHexerLevels[currentLevelIndex].MatrixSize;
		int tagPieces = NodeHexerLevels[currentLevelIndex].TagPieces;
		timeBoost = NodeHexerLevels[currentLevelIndex].TimeBoost;
		TotalMoves = 0;
		nodeHolderSize.x = (float)matrixSize * 50f + (float)(matrixSize - 1) * 10f;
		nodeHolderSize.y = (float)matrixSize * 50f + (float)(matrixSize - 1) * 10f;
		NodeHolderRT.sizeDelta = nodeHolderSize;
		NodeHexerContentRT.sizeDelta = nodeHolderSize;
		if (NodeHexerContentRT.sizeDelta.y >= 650f && Screen.height <= 800)
		{
			NodeHexerContentRT.localScale = new Vector3(0.72f, 0.72f, 1f);
		}
		NodeHexerContentCG.alpha = 1f;
		CurrentMatrix.SetMatrixSize(matrixSize);
		int num = matrixSize * matrixSize / 2;
		int num2 = matrixSize * matrixSize / 2;
		int num3 = 0;
		int num4 = 0;
		stackCords.Clear();
		nodesThatNeedToBeTagged.Clear();
		if (matrixSize * matrixSize - (num + num2) != 0)
		{
			if (Random.Range(0, 2) == 0)
			{
				num++;
			}
			else
			{
				num2++;
			}
		}
		for (int i = 0; i < matrixSize * matrixSize; i++)
		{
			NodeHexObject nodeHexObject = nodeHexObjectPool.Pop();
			CurrentMatrix.Push(nodeHexObject);
			nodeHexObject.SoftBuild(CurrentMatrix.Pointer, HACK_NODE_HEXER_NODE_TYPE.BETA);
			stackCords.Add(CurrentMatrix.Pointer);
		}
		bool flag = true;
		int num5 = 0;
		int num6 = Random.Range(0, stackCords.Count);
		int num7 = Random.Range(2, matrixSize - 1);
		while (flag)
		{
			while (num3 < num)
			{
				if (CurrentMatrix.TryAndGetValue(out var ReturnValue, stackCords[num6]))
				{
					if (ReturnValue.Type == HACK_NODE_HEXER_NODE_TYPE.ALPHA)
					{
						num7 = ((num7 != 2) ? (num7 - 1) : (num7 + 1));
					}
					else
					{
						ReturnValue.Type = HACK_NODE_HEXER_NODE_TYPE.ALPHA;
						if (CurrentMatrix.Set(ReturnValue, stackCords[num6]))
						{
							num3++;
						}
					}
				}
				num6 += num7;
				if (num6 >= stackCords.Count)
				{
					num6 -= stackCords.Count;
				}
			}
			foreach (NodeHexObject item in CurrentMatrix.GetAll())
			{
				if (noWayOutCheck(item))
				{
					num5++;
				}
			}
			if (num5 > 0)
			{
				num6 = Random.Range(0, stackCords.Count);
				num7 = Random.Range(2, matrixSize - 1);
				num3 = 0;
				num5 = 0;
				foreach (NodeHexObject item2 in CurrentMatrix.GetAll())
				{
					item2.Type = HACK_NODE_HEXER_NODE_TYPE.BETA;
				}
			}
			else
			{
				num5 = 0;
				flag = false;
			}
		}
		hexTypePicker.X = 0;
		hexTypePicker.Y = Random.Range(1, matrixSize);
		startNode = CurrentMatrix.Get(hexTypePicker);
		currentTagPieces = tagPieces;
		while (num4 < tagPieces)
		{
			hexTypePicker.X = ((num4 != tagPieces - 1) ? Random.Range(1, matrixSize - 1) : (matrixSize - 1));
			hexTypePicker.Y = Random.Range(0, matrixSize);
			if (!nodesThatNeedToBeTagged.ContainsKey(hexTypePicker))
			{
				nodesThatNeedToBeTagged.Add(hexTypePicker, value: true);
				num4++;
			}
		}
		int colIndex = 0;
		int num8 = 0;
		GameManager.AudioSlinger.PlaySound(ShowRowSFX);
		foreach (NodeHexObject item3 in CurrentMatrix.GetAll())
		{
			bool needsToBeTagged = nodesThatNeedToBeTagged.ContainsKey(item3.MyCord);
			item3.Build(matrixSize, colIndex, needsToBeTagged);
			colIndex++;
			if (colIndex >= matrixSize)
			{
				num8++;
				colIndex = 0;
				if (num8 < matrixSize)
				{
					GameManager.TimeSlinger.FireTimer((float)num8 * 0.2f, GameManager.AudioSlinger.PlaySound, ShowRowSFX);
				}
			}
		}
		colIndex = 0;
		GameManager.TimeSlinger.FireTimer((float)matrixSize * 0.2f, delegate
		{
			StartArrow.Present(startNode.GetComponent<RectTransform>());
			foreach (KeyValuePair<MatrixStackCord, bool> item4 in nodesThatNeedToBeTagged)
			{
				if (CurrentMatrix.TryAndGetValue(out var ReturnValue2, item4.Key))
				{
					ReturnValue2.ActivateNeedsToBeTagged(colIndex);
					colIndex++;
				}
			}
			GameManager.HackerManager.HackingTimer.FireWarmUpTimer(3);
			GameManager.TimeSlinger.FireTimer(3f, beginGame);
		});
	}

	private void beginGame()
	{
		gameWon = false;
		startNode.SetPlayable();
		GameManager.HackerManager.HackingTimer.FireHackingTimer(waitingForCustomHack ? StartTimeFix : NodeHexerLevels[currentLevelIndex].StartTime, firePath);
	}

	private void clearGame()
	{
		foreach (NodeHexObject item in CurrentMatrix.GetAll())
		{
			item.Clear();
			nodeHexObjectPool.Push(item);
		}
		StartArrow.Clear();
		CurrentMatrix.Clear();
		termLine1.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
		termLine2.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
		termLine3.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
		GameManager.TimeSlinger.FireTimer(0.4f, delegate
		{
			MyRayCaster.enabled = false;
			NodeHexerContentCG.alpha = 0f;
			NodeHexerContentCG.interactable = false;
			NodeHexerContentCG.blocksRaycasts = false;
			NodeHexerContentCG.ignoreParentGroups = false;
			if (NodeHexerContentRT.localScale != Vector3.one)
			{
				NodeHexerContentRT.localScale = Vector3.one;
			}
			GameManager.HackerManager.HackingTerminal.TerminalHelper.ClearTerminal();
			if (gameWon)
			{
				if (!DifficultyManager.HackerMode)
				{
					if (!DifficultyManager.LeetMode)
					{
						if (TotalMoves <= NodeHexerLevels[currentLevelIndex].MatrixSize * NodeHexerLevels[currentLevelIndex].MatrixSize / 2)
						{
							myData.SkillPoints += NodeHexerLevels[currentLevelIndex].PointsRewaredTier1;
						}
						else
						{
							myData.SkillPoints += NodeHexerLevels[currentLevelIndex].PointsRewaredTier2;
						}
						DataManager.Save(myData);
					}
					if (currentLevelIndex == NodeHexerLevels.Count - 1)
					{
						SteamSlinger.Ins.UnlockSteamAchievement(STEAM_ACHIEVEMENT.HOOKUPMASTER);
					}
				}
				else if (TotalMoves <= NodeHexerLevels[currentLevelIndex].MatrixSize * NodeHexerLevels[currentLevelIndex].MatrixSize / 2)
				{
					HackerModeManager.Ins.SkillPoints += NodeHexerLevels[currentLevelIndex].PointsRewaredTier1;
				}
				else
				{
					HackerModeManager.Ins.SkillPoints += NodeHexerLevels[currentLevelIndex].PointsRewaredTier2;
				}
				GameManager.HackerManager.PlayerWon(currentLevelIndex);
			}
			else
			{
				if (!DifficultyManager.HackerMode)
				{
					if (!DifficultyManager.LeetMode)
					{
						myData.SkillPoints -= NodeHexerLevels[currentLevelIndex].PointsDeducted;
						DataManager.Save(myData);
					}
					if (currentLevelIndex == 0)
					{
						SteamSlinger.Ins.UnlockSteamAchievement(STEAM_ACHIEVEMENT.IDONTNODE);
					}
				}
				GameManager.HackerManager.PlayerLost();
			}
		});
	}

	private void firePath()
	{
		waitingForCustomHack = false;
		StartTimeFix = 0f;
		startNode.Activate(HACK_NODE_HEXER_NODE_TYPE.DEAD);
	}

	public bool noWayOutCheck(NodeHexObject SourceNode)
	{
		int num = 0;
		if (CurrentMatrix.TryAndGetValueByClock(out var ReturnValue, SourceNode.MyCord, MATRIX_STACK_CLOCK_POSITION.NOON))
		{
			if (ReturnValue.Type == SourceNode.Type)
			{
				num++;
			}
		}
		else
		{
			num++;
		}
		if (CurrentMatrix.TryAndGetValueByClock(out ReturnValue, SourceNode.MyCord, MATRIX_STACK_CLOCK_POSITION.ONE))
		{
			if (ReturnValue.Type == SourceNode.Type)
			{
				num++;
			}
		}
		else
		{
			num++;
		}
		if (CurrentMatrix.TryAndGetValueByClock(out ReturnValue, SourceNode.MyCord, MATRIX_STACK_CLOCK_POSITION.THREE))
		{
			if (ReturnValue.Type == SourceNode.Type)
			{
				num++;
			}
		}
		else
		{
			num++;
		}
		if (CurrentMatrix.TryAndGetValueByClock(out ReturnValue, SourceNode.MyCord, MATRIX_STACK_CLOCK_POSITION.FOUR))
		{
			if (ReturnValue.Type == SourceNode.Type)
			{
				num++;
			}
		}
		else
		{
			num++;
		}
		if (CurrentMatrix.TryAndGetValueByClock(out ReturnValue, SourceNode.MyCord, MATRIX_STACK_CLOCK_POSITION.SIX))
		{
			if (ReturnValue.Type == SourceNode.Type)
			{
				num++;
			}
		}
		else
		{
			num++;
		}
		if (CurrentMatrix.TryAndGetValueByClock(out ReturnValue, SourceNode.MyCord, MATRIX_STACK_CLOCK_POSITION.SEVEN))
		{
			if (ReturnValue.Type == SourceNode.Type)
			{
				num++;
			}
		}
		else
		{
			num++;
		}
		if (CurrentMatrix.TryAndGetValueByClock(out ReturnValue, SourceNode.MyCord, MATRIX_STACK_CLOCK_POSITION.NINE))
		{
			if (ReturnValue.Type == SourceNode.Type)
			{
				num++;
			}
		}
		else
		{
			num++;
		}
		if (CurrentMatrix.TryAndGetValueByClock(out ReturnValue, SourceNode.MyCord, MATRIX_STACK_CLOCK_POSITION.TEN))
		{
			if (ReturnValue.Type == SourceNode.Type)
			{
				num++;
			}
		}
		else
		{
			num++;
		}
		return num >= 7;
	}

	private void setCurrentLevelIndex()
	{
		waitingForCustomHack = false;
		StartTimeFix = 0f;
		for (int i = 0; i < NodeHexerLevels.Count; i++)
		{
			if (myData.SkillPoints >= NodeHexerLevels[i].SkillPointsRequired)
			{
				currentLevelIndex = i;
			}
			else
			{
				i = NodeHexerLevels.Count;
			}
		}
	}

	private void stageMe()
	{
		myData = DataManager.Load<NodeHexerHackData>(myID);
		if (myData == null)
		{
			myData = new NodeHexerHackData(myID);
			myData.SkillPoints = 0;
		}
		if (DifficultyManager.LeetMode)
		{
			myData.SkillPoints = NodeHexerLevels[NodeHexerLevels.Count - 1].SkillPointsRequired;
		}
		GameManager.StageManager.Stage -= stageMe;
	}

	public void PrepNodeHexAttackTarot(int level)
	{
		setCurrentLevelIndex();
		currentLevelIndex = level;
		if (currentLevelIndex > NodeHexerLevels.Count - 1)
		{
			currentLevelIndex = NodeHexerLevels.Count - 1;
		}
		MyRayCaster.enabled = true;
		NodeHexerContentCG.interactable = true;
		NodeHexerContentCG.blocksRaycasts = true;
		NodeHexerContentCG.ignoreParentGroups = true;
		GameManager.AudioSlinger.PlaySound(GameManager.HackerManager.HackingTypeSFX);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine1, TERMINAL_LINE_TYPE.TYPE, "> ./N0D3H3X3R", 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine2, TERMINAL_LINE_TYPE.TYPE, "  Loading N0D3H3X3R v6.13.70", 0.2f, 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine3, TERMINAL_LINE_TYPE.TYPE, "  Initialzing...", 0.2f, 0.4f);
		GameManager.TimeSlinger.FireTimer(0.6f, buildNodeHexAttack);
		Debug.Log("[Node Hexer Attack] Current level:" + (currentLevelIndex + 1));
	}

	public void buildCustomNodeHexAttack(int MatrixSize, int TagPieces, float TimeBoost, float StartTime)
	{
		MyRayCaster.enabled = true;
		NodeHexerContentCG.interactable = true;
		NodeHexerContentCG.blocksRaycasts = true;
		NodeHexerContentCG.ignoreParentGroups = true;
		GameManager.AudioSlinger.PlaySound(GameManager.HackerManager.HackingTypeSFX);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine1, TERMINAL_LINE_TYPE.TYPE, "> ./N0D3H3X3R", 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine2, TERMINAL_LINE_TYPE.TYPE, "  Loading N0D3H3X3R v6.13.70", 0.2f, 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine3, TERMINAL_LINE_TYPE.TYPE, "  Initialzing...", 0.2f, 0.4f);
		GameManager.TimeSlinger.FireTimer(0.6f, delegate
		{
			waitingForCustomHack = true;
			StartTimeFix = StartTime;
			int num = MatrixSize;
			int num2 = TagPieces;
			timeBoost = TimeBoost;
			TotalMoves = 0;
			nodeHolderSize.x = (float)num * 50f + (float)(num - 1) * 10f;
			nodeHolderSize.y = (float)num * 50f + (float)(num - 1) * 10f;
			NodeHolderRT.sizeDelta = nodeHolderSize;
			NodeHexerContentRT.sizeDelta = nodeHolderSize;
			if (NodeHexerContentRT.sizeDelta.y >= 650f && Screen.height <= 800)
			{
				NodeHexerContentRT.localScale = new Vector3(0.72f, 0.72f, 1f);
			}
			NodeHexerContentCG.alpha = 1f;
			CurrentMatrix.SetMatrixSize(num);
			int num3 = num * num / 2;
			int num4 = num * num / 2;
			int num5 = 0;
			int num6 = 0;
			stackCords.Clear();
			nodesThatNeedToBeTagged.Clear();
			if (num * num - (num3 + num4) != 0)
			{
				if (Random.Range(0, 2) == 0)
				{
					num3++;
				}
				else
				{
					num4++;
				}
			}
			for (int i = 0; i < num * num; i++)
			{
				NodeHexObject nodeHexObject = nodeHexObjectPool.Pop();
				CurrentMatrix.Push(nodeHexObject);
				nodeHexObject.SoftBuild(CurrentMatrix.Pointer, HACK_NODE_HEXER_NODE_TYPE.BETA);
				stackCords.Add(CurrentMatrix.Pointer);
			}
			bool flag = true;
			int num7 = 0;
			int num8 = Random.Range(0, stackCords.Count);
			int num9 = Random.Range(2, num - 1);
			while (flag)
			{
				while (num5 < num3)
				{
					if (CurrentMatrix.TryAndGetValue(out var ReturnValue, stackCords[num8]))
					{
						if (ReturnValue.Type == HACK_NODE_HEXER_NODE_TYPE.ALPHA)
						{
							num9 = ((num9 != 2) ? (num9 - 1) : (num9 + 1));
						}
						else
						{
							ReturnValue.Type = HACK_NODE_HEXER_NODE_TYPE.ALPHA;
							if (CurrentMatrix.Set(ReturnValue, stackCords[num8]))
							{
								num5++;
							}
						}
					}
					num8 += num9;
					if (num8 >= stackCords.Count)
					{
						num8 -= stackCords.Count;
					}
				}
				foreach (NodeHexObject item in CurrentMatrix.GetAll())
				{
					if (noWayOutCheck(item))
					{
						num7++;
					}
				}
				if (num7 > 0)
				{
					num8 = Random.Range(0, stackCords.Count);
					num9 = Random.Range(2, num - 1);
					num5 = 0;
					num7 = 0;
					foreach (NodeHexObject item2 in CurrentMatrix.GetAll())
					{
						item2.Type = HACK_NODE_HEXER_NODE_TYPE.BETA;
					}
				}
				else
				{
					num7 = 0;
					flag = false;
				}
			}
			hexTypePicker.X = 0;
			hexTypePicker.Y = Random.Range(1, num);
			startNode = CurrentMatrix.Get(hexTypePicker);
			currentTagPieces = num2;
			while (num6 < num2)
			{
				hexTypePicker.X = ((num6 != num2 - 1) ? Random.Range(1, num - 1) : (num - 1));
				hexTypePicker.Y = Random.Range(0, num);
				if (!nodesThatNeedToBeTagged.ContainsKey(hexTypePicker))
				{
					nodesThatNeedToBeTagged.Add(hexTypePicker, value: true);
					num6++;
				}
			}
			int colIndex = 0;
			int num10 = 0;
			GameManager.AudioSlinger.PlaySound(ShowRowSFX);
			foreach (NodeHexObject item3 in CurrentMatrix.GetAll())
			{
				bool needsToBeTagged = nodesThatNeedToBeTagged.ContainsKey(item3.MyCord);
				item3.Build(num, colIndex, needsToBeTagged);
				colIndex++;
				if (colIndex >= num)
				{
					num10++;
					colIndex = 0;
					if (num10 < num)
					{
						GameManager.TimeSlinger.FireTimer((float)num10 * 0.2f, GameManager.AudioSlinger.PlaySound, ShowRowSFX);
						timeSlinger timeSlinger2 = GameManager.TimeSlinger;
					}
				}
			}
			colIndex = 0;
			GameManager.TimeSlinger.FireTimer((float)num * 0.2f, delegate
			{
				StartArrow.Present(startNode.GetComponent<RectTransform>());
				foreach (KeyValuePair<MatrixStackCord, bool> item4 in nodesThatNeedToBeTagged)
				{
					if (CurrentMatrix.TryAndGetValue(out var ReturnValue2, item4.Key))
					{
						ReturnValue2.ActivateNeedsToBeTagged(colIndex);
						colIndex++;
					}
				}
				GameManager.HackerManager.HackingTimer.FireWarmUpTimer(3);
				GameManager.TimeSlinger.FireTimer(3f, beginGame);
			});
		});
	}

	public int HackerModeSetSkill()
	{
		int result = 0;
		for (int i = 0; i < NodeHexerLevels.Count; i++)
		{
			if (HackerModeManager.Ins.SkillPoints >= NodeHexerLevels[i].SkillPointsRequired)
			{
				result = i;
			}
			else
			{
				i = NodeHexerLevels.Count;
			}
		}
		return result;
	}
}
