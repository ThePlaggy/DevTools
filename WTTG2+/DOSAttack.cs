using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DOSAttack : MonoBehaviour
{
	public class node
	{
		public List<node> drillUpNodes;

		public bool iWasChecked;

		public List<string> myBranches;

		public List<node> myBranchNodes;

		public DOSAttack myDAP;

		public string myParent;

		public node myParentNode;

		public nodePosition myPos;

		public List<node> mySibNodes;

		public Sprite mySprite;

		public DOS_NODE_TYPE mySubType;

		public DOS_NODE_TYPE myType;

		public node()
		{
		}

		public node(DOS_NODE_TYPE setType, nodePosition setPos, Sprite setSprite, DOSAttack setDAP)
		{
			myType = setType;
			myPos = setPos;
			mySprite = setSprite;
			myParent = "none";
			myParentNode = new node();
			myBranches = new List<string>();
			myBranchNodes = new List<node>();
			mySibNodes = new List<node>();
			drillUpNodes = new List<node>();
			iWasChecked = false;
			myDAP = setDAP;
		}

		public string getNodeKey(short x = 0, short y = 0)
		{
			return myPos.x + x + ":" + (myPos.y + y);
		}

		public bool getNextNodeToCheck(out node nodeToCheck)
		{
			nodeToCheck = new node();
			bool flag = false;
			myDAP.reCount++;
			if (myDAP.reCount >= myDAP.getMatrixCount())
			{
				return false;
			}
			if (!iWasChecked)
			{
				nodeToCheck = this;
				return true;
			}
			if (mySibNodes.Count > 0)
			{
				for (int i = 0; i < mySibNodes.Count; i++)
				{
					if (!mySibNodes[i].iWasChecked)
					{
						flag = true;
						nodeToCheck = mySibNodes[i];
						i = mySibNodes.Count + 1;
					}
				}
				if (flag)
				{
					return true;
				}
				if (myParent != "none")
				{
					for (int j = 0; j < myParentNode.myBranchNodes.Count; j++)
					{
						if (myParentNode.myBranchNodes[j].myBranchNodes.Count > 0 && myParentNode.myBranchNodes[j].myBranchNodes[0].getNextNodeToCheck(out nodeToCheck))
						{
							flag = true;
							j = mySibNodes.Count + 1;
						}
					}
				}
				else
				{
					for (int k = 0; k < mySibNodes.Count; k++)
					{
						if (mySibNodes[k].myBranchNodes.Count > 0 && mySibNodes[k].myBranchNodes[0].getNextNodeToCheck(out nodeToCheck))
						{
							flag = true;
							k = mySibNodes.Count + 1;
						}
					}
				}
				if (flag)
				{
					return true;
				}
				bool flag2 = false;
				node node2 = this;
				node node3 = new node();
				while (!flag2)
				{
					drillUpNodes.Add(node2);
					for (int l = 0; l < node2.mySibNodes.Count; l++)
					{
						if (flag)
						{
							continue;
						}
						if (!node2.mySibNodes[l].iWasChecked)
						{
							node3 = node2.mySibNodes[l];
							flag = true;
							flag2 = true;
							l = node2.mySibNodes.Count + 1;
						}
						for (int m = 0; m < node2.mySibNodes[l].myBranchNodes.Count; m++)
						{
							if (!node2.mySibNodes[l].myBranchNodes[m].iWasChecked)
							{
								node3 = node2.mySibNodes[l].myBranchNodes[m];
								flag = true;
								flag2 = true;
							}
						}
					}
					if (node2.myParent == "none")
					{
						flag2 = true;
					}
					else
					{
						node2 = node2.myParentNode;
					}
				}
				if (flag)
				{
					nodeToCheck = node3;
					return true;
				}
				return false;
			}
			if (myBranchNodes.Count > 0)
			{
				return myBranchNodes[0].getNextNodeToCheck(out nodeToCheck);
			}
			bool flag3 = false;
			node node4 = this;
			node node5 = new node();
			while (!flag3)
			{
				drillUpNodes.Add(node4);
				for (int n = 0; n < node4.mySibNodes.Count; n++)
				{
					if (flag)
					{
						continue;
					}
					if (!node4.mySibNodes[n].iWasChecked)
					{
						node5 = node4.mySibNodes[n];
						flag = true;
						flag3 = true;
						n = node4.mySibNodes.Count + 1;
					}
					for (int num = 0; num < node4.mySibNodes[n].myBranchNodes.Count; num++)
					{
						if (!node4.mySibNodes[n].myBranchNodes[num].iWasChecked)
						{
							node5 = node4.mySibNodes[n].myBranchNodes[num];
							flag = true;
							flag3 = true;
						}
					}
				}
				if (node4.myParent == "none")
				{
					flag3 = true;
				}
				else
				{
					node4 = node4.myParentNode;
				}
			}
			if (flag)
			{
				nodeToCheck = node5;
				return true;
			}
			return false;
		}

		public void generateSibs()
		{
			if (myBranchNodes.Count <= 0)
			{
				return;
			}
			List<node> list = new List<node>();
			for (int i = 0; i < myBranchNodes.Count; i++)
			{
				list.Add(myBranchNodes[i]);
			}
			for (int j = 0; j < myBranchNodes.Count; j++)
			{
				for (int k = 0; k < list.Count; k++)
				{
					if (myBranchNodes[j] != list[k])
					{
						myBranchNodes[j].mySibNodes.Add(list[k]);
					}
				}
			}
		}
	}

	public struct nodePosition
	{
		public short x;

		public short y;

		public nodePosition(short x, short y)
		{
			this.x = x;
			this.y = y;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			nodePosition nodePosition2 = (nodePosition)obj;
			return x == nodePosition2.x && y == nodePosition2.y;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public bool Equals(nodePosition o)
		{
			return x == o.x && y == o.y;
		}

		public static bool operator ==(nodePosition lhs, nodePosition rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(nodePosition lhs, nodePosition rhs)
		{
			return !lhs.Equals(rhs);
		}
	}

	public static int getStaticPointes1;

	[Range(2f, 11f)]
	public short MATRIX_SIZE = 2;

	[Range(3f, 8f)]
	public short ACTION_BLOCK_SIZE = 3;

	[Range(1f, 10f)]
	public float warmUpTime = 5f;

	[Range(0.1f, 2.5f)]
	public float hotTime = 1f;

	[Range(0.1f, 0.5f)]
	public float boostTime = 0.1f;

	[Range(5f, 120f)]
	public float DOSTime = 60f;

	public bool trollNodesActive;

	public AudioFileDefinition CountDownTick1;

	public AudioFileDefinition CountDownTick2;

	public AudioFileDefinition NodeHot;

	public AudioFileDefinition NodeCold;

	public AudioFileDefinition NodeActive;

	public AudioFileDefinition ExitNodeActive;

	public AudioFileDefinition ActionNodeClick;

	public AudioFileDefinition ClockAlmostUp;

	public AudioFileDefinition HMPuzzlePass;

	public Font clockFont;

	public Color clockColor;

	public GameObject DOSHolder;

	public GameObject DOSClockObject;

	public Color nodeBorderColor;

	public Sprite pixelSprite;

	public Sprite whiteNodeSprite;

	public Sprite startNodeSprite;

	public Sprite startNodeDownArrowSprite;

	public Sprite endNodeSprite;

	public Sprite actionNodeSprite;

	public Sprite leftNodeSprite;

	public Sprite rightNodeSprite;

	public Sprite upNodeSrpite;

	public Sprite downNodeSprite;

	public Sprite trollLeftNodeSprite;

	public Sprite trollRightNodeSprite;

	public Sprite trollUpNodeSprite;

	public Sprite trollDownNodeSprite;

	public GameObject NodeObject;

	public int rootNodeWidth = 100;

	public int rootNodeHeight = 100;

	public int nodeWidth = 100;

	public int nodeHeight = 100;

	public int reCount;

	public Sprite actionNodeHoverSprite;

	public Sprite actionNodeUpA;

	public Sprite actionNodeRightA;

	public Sprite actionNodeDownA;

	public Sprite actionNodeLeftA;

	public Sprite actionNodeActivatedSprite;

	public Sprite actionNodeActivatedHoverSprite;

	public Sprite endNodeActivatedSprite;

	public short SkillPointsA = 5;

	public short SkillPointsB = 2;

	public List<DOSLevelDefinition> DOSLevels;

	public List<DOSChainDefinition> DOSChains;

	[HideInInspector]
	public TerminalLineObject termLine1;

	[HideInInspector]
	public TerminalLineObject termLine2;

	[HideInInspector]
	public TerminalLineObject termLine3;

	private List<nodePosition> actionBlockNodes;

	private short actionNodeActivatedCount;

	private bool addSkillPoints = true;

	private bool allActionNodesActivated;

	private Dictionary<string, int> arrowNodes;

	private float clockMicroCount;

	private float clockMicroTimeStamp;

	private Text clockText;

	private RectTransform clockTextHolder;

	private float clockTimeStamp;

	private bool clockWise;

	private short currentActionFilledNodeCount;

	private int currentDOSChainIndex;

	private int currentLevelIndex;

	private Dictionary<string, int> currentTreeNodes;

	private Sequence DOSAttackClockSeq;

	private Sequence DOSAttackOverSeq;

	private float DOSGameTimeStamp;

	private float DOSHOTTimeStamp;

	private bool DOSiSHot;

	private NodeObject endNodeObject;

	private bool finalCountDownFired;

	private NodeObject hotNodeObject;

	private bool isAStaleMate = true;

	public List<node> masterNodes;

	private int maxNodeClickCount;

	private Image nodeBGIMG;

	private int nodeClickCount;

	private RectTransform nodeHolder;

	private CanvasGroup nodeHolderCG;

	private Dictionary<string, int> nodeLookUp;

	private Dictionary<string, GameObject> nodeObjects;

	private bool searchingForActionNodes = true;

	private bool searchingForEndNode;

	private GameObject setDOSClockObject;

	private List<nodePosition> startEndNodes;

	private NodeObject startNodeObject;

	private bool warmClockActive;

	private Sequence warmClockSeq;

	private void Start()
	{
		startEndNodes = new List<nodePosition>();
		actionBlockNodes = new List<nodePosition>();
		masterNodes = new List<node>();
		nodeLookUp = new Dictionary<string, int>();
		arrowNodes = new Dictionary<string, int>();
		currentTreeNodes = new Dictionary<string, int>();
		StageMe();
	}

	private void Update()
	{
		if (warmClockActive)
		{
			if (Time.time - clockTimeStamp >= warmUpTime)
			{
				warmClockActive = false;
				fireDOSAttack();
			}
			else if (Time.time - clockMicroTimeStamp >= 1f)
			{
				GameManager.AudioSlinger.PlaySound(CountDownTick1);
				clockMicroCount -= 1f;
				clockMicroTimeStamp = Time.time;
			}
		}
		else
		{
			if (!DOSiSHot)
			{
				return;
			}
			float num = hotTime;
			if (Input.GetMouseButton(2) || Input.GetKey(KeyCode.LeftControl))
			{
				num = boostTime;
			}
			if (Time.time - DOSGameTimeStamp >= DOSTime - ClockAlmostUp.AudioClip.length && !finalCountDownFired)
			{
				finalCountDownFired = true;
				GameManager.AudioSlinger.PlaySound(ClockAlmostUp);
			}
			if (Time.time - DOSGameTimeStamp >= DOSTime)
			{
				DOSiSHot = false;
				DOSAttackSucceeded();
			}
			else if (Time.time - DOSHOTTimeStamp >= num)
			{
				hotNodeObject.untap();
				setNextHotNode();
				if (DOSiSHot)
				{
					hotNodeObject.tap();
					DOSHOTTimeStamp = Time.time;
				}
			}
		}
	}

	public bool getNodeWithKey(string theKey, out node returnNode)
	{
		returnNode = new node();
		if (nodeLookUp.ContainsKey(theKey))
		{
			returnNode = masterNodes[nodeLookUp[theKey]];
			return true;
		}
		return false;
	}

	public short getMatrixCount()
	{
		return (short)((MATRIX_SIZE + 1) * (MATRIX_SIZE + 1));
	}

	public int getNextChainPoints()
	{
		return 0;
	}

	public void skipCurrentLevel()
	{
	}

	private void setCurrentLevelIndex()
	{
		for (int i = 0; i < DOSLevels.Count; i++)
		{
			if (getStaticPointes1 >= DOSLevels[i].skillPointesRequired)
			{
				currentLevelIndex = i;
			}
			else
			{
				i = DOSLevels.Count;
			}
		}
	}

	public void CreateNewDOSAttack(HACK_SWEEPER_SKILL_TIER SetTier)
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
			currentLevelIndex = DOSLevels.Count - 1;
			break;
		case HACK_SWEEPER_SKILL_TIER.HACKER_MODE:
			currentLevelIndex = HackerModeSetSkill();
			break;
		}
		if (currentLevelIndex > DOSLevels.Count - 1)
		{
			currentLevelIndex = DOSLevels.Count - 1;
		}
		Debug.Log("[DOS Attack] Current level:" + (currentLevelIndex + 1));
		prepDOSAttack();
	}

	public void prepDOSAttack()
	{
		MATRIX_SIZE = (short)(DOSLevels[currentLevelIndex].matrixSize - 1);
		ACTION_BLOCK_SIZE = DOSLevels[currentLevelIndex].actionBlockSize;
		hotTime = DOSLevels[currentLevelIndex].hotTime;
		DOSTime = (float)(DOSLevels[currentLevelIndex].matrixSize * DOSLevels[currentLevelIndex].matrixSize) * hotTime * DOSLevels[currentLevelIndex].gameTimeModifier;
		trollNodesActive = DOSLevels[currentLevelIndex].trollNodesActive;
		if (Screen.height <= 800)
		{
			nodeWidth = 50;
			nodeHeight = 50;
		}
		else if (Screen.height <= 1300)
		{
			nodeWidth = 75;
			nodeHeight = 75;
		}
		else
		{
			nodeWidth = 100;
			nodeHeight = 100;
		}
		prepPuzzle();
	}

	public void warmDOSAttack()
	{
		int num = 72;
		float num2 = (float)nodeWidth / (float)rootNodeWidth;
		num = Mathf.RoundToInt((float)num * num2);
		TextGenerationSettings textGenerationSettings = default(TextGenerationSettings);
		new TextGenerator();
		textGenerationSettings.textAnchor = TextAnchor.UpperCenter;
		textGenerationSettings.generateOutOfBounds = true;
		textGenerationSettings.generationExtents = new Vector2(50f, 20f);
		textGenerationSettings.pivot = Vector2.zero;
		textGenerationSettings.richText = true;
		textGenerationSettings.font = clockFont;
		textGenerationSettings.fontSize = num;
		textGenerationSettings.fontStyle = FontStyle.Normal;
		textGenerationSettings.lineSpacing = 1f;
		textGenerationSettings.scaleFactor = 1f;
		textGenerationSettings.verticalOverflow = VerticalWrapMode.Overflow;
		textGenerationSettings.horizontalOverflow = HorizontalWrapMode.Wrap;
		GameManager.HackerManager.HackingTimer.FireWarmUpTimer(3);
		clockTimeStamp = Time.time;
		clockMicroTimeStamp = Time.time;
		clockMicroCount = warmUpTime;
		warmClockActive = true;
		GameManager.AudioSlinger.PlaySound(CountDownTick1);
	}

	public void fireDOSAttack()
	{
		GameManager.AudioSlinger.PlaySound(CountDownTick2);
		float num = (float)nodeWidth / (float)rootNodeWidth;
		float num2 = (float)nodeHeight / (float)rootNodeHeight;
		finalCountDownFired = false;
		GameManager.HackerManager.HackingTimer.FireHackingTimer(DOSTime, TimerAction);
		startNodeObject.stopSubAction();
		hotNodeObject = startNodeObject;
		hotNodeObject.tap();
		DOSHOTTimeStamp = Time.time;
		DOSGameTimeStamp = Time.time;
		DOSiSHot = true;
	}

	public void actionNodeActivated()
	{
		actionNodeActivatedCount++;
		if (actionNodeActivatedCount >= ACTION_BLOCK_SIZE)
		{
			GameManager.AudioSlinger.PlaySound(ExitNodeActive);
			endNodeObject.endNodeNowActive();
			allActionNodesActivated = true;
		}
	}

	public void addNodeClickCount()
	{
	}

	private void prepPuzzle()
	{
		searchingForActionNodes = true;
		searchingForEndNode = false;
		startEndNodes.Clear();
		actionBlockNodes.Clear();
		masterNodes.Clear();
		nodeLookUp.Clear();
		arrowNodes.Clear();
		currentTreeNodes.Clear();
		currentActionFilledNodeCount = 0;
		reCount = 0;
		if (Random.Range(1, 3) == 1)
		{
			clockWise = false;
		}
		else
		{
			clockWise = true;
		}
		for (int i = 0; i < (MATRIX_SIZE + 1) * (MATRIX_SIZE + 1); i++)
		{
			short x = (short)(i % (MATRIX_SIZE + 1));
			short y = (short)Mathf.FloorToInt(i / (MATRIX_SIZE + 1));
			masterNodes.Add(new node(DOS_NODE_TYPE.WHITENODE, new nodePosition(x, y), whiteNodeSprite, this));
			nodeLookUp.Add(x + ":" + y, i);
		}
		startEndNodes.Add(new nodePosition((short)Random.Range(0, MATRIX_SIZE), 0));
		startEndNodes.Add(new nodePosition((short)Random.Range(0, MATRIX_SIZE), MATRIX_SIZE));
		node node2 = masterNodes[nodeLookUp[startEndNodes[0].x + ":" + startEndNodes[0].y]];
		node2.myType = DOS_NODE_TYPE.STARTNODE;
		node2.mySprite = startNodeSprite;
		masterNodes[nodeLookUp[startEndNodes[0].x + ":" + startEndNodes[0].y]] = node2;
		node2 = masterNodes[nodeLookUp[startEndNodes[1].x + ":" + startEndNodes[1].y]];
		node2.myType = DOS_NODE_TYPE.ENDNODE;
		node2.mySprite = endNodeSprite;
		masterNodes[nodeLookUp[startEndNodes[1].x + ":" + startEndNodes[1].y]] = node2;
		for (int j = 0; j < ACTION_BLOCK_SIZE; j++)
		{
			actionBlockNodes.Add(generateValidActionBlock());
			node2 = masterNodes[nodeLookUp[actionBlockNodes[j].x + ":" + actionBlockNodes[j].y]];
			node2.myType = DOS_NODE_TYPE.ACTIONNODE;
			node2.mySprite = actionNodeSprite;
			masterNodes[nodeLookUp[actionBlockNodes[j].x + ":" + actionBlockNodes[j].y]] = node2;
		}
		generateValidArrowBlock(masterNodes[nodeLookUp[startEndNodes[0].x + ":" + startEndNodes[0].y]]);
		if (!isAStaleMate)
		{
			drawPuzzle();
		}
		else
		{
			prepPuzzle();
		}
	}

	public void CreateNewDOSAttackTarot(int level)
	{
		GameManager.AudioSlinger.PlaySound(GameManager.HackerManager.HackingTypeSFX);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine1, TERMINAL_LINE_TYPE.TYPE, "> ./DOS_Blocker", 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine2, TERMINAL_LINE_TYPE.TYPE, "  Loading DOS Blocker v0.6b...", 0.2f, 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine3, TERMINAL_LINE_TYPE.TYPE, "  Initialzing...", 0.2f, 0.4f);
		setCurrentLevelIndex();
		currentLevelIndex = level;
		if (currentLevelIndex > DOSLevels.Count - 1)
		{
			currentLevelIndex = DOSLevels.Count - 1;
		}
		Debug.Log("[DOS Attack] Current level:" + (currentLevelIndex + 1));
		GameManager.TimeSlinger.FireTimer(0.8f, prepDOSAttack);
		GameManager.TimeSlinger.FireTimer(1f, warmDOSAttack);
	}

	private nodePosition generateValidActionBlock()
	{
		bool flag = true;
		nodePosition nodePosition2 = new nodePosition(0, 0);
		while (flag)
		{
			nodePosition2 = new nodePosition((short)Random.Range(0, MATRIX_SIZE + 1), (short)Random.Range(0, MATRIX_SIZE + 1));
			if (!(nodePosition2 != startEndNodes[0]) || !(nodePosition2 != startEndNodes[1]))
			{
				continue;
			}
			if (actionBlockNodes.Count > 0)
			{
				bool flag2 = true;
				for (int i = 0; i < actionBlockNodes.Count; i++)
				{
					if (nodePosition2.y == actionBlockNodes[i].y)
					{
						flag2 = false;
						i = actionBlockNodes.Count;
					}
				}
				if (flag2)
				{
					flag = false;
				}
			}
			else
			{
				flag = false;
			}
		}
		return nodePosition2;
	}

	private void generateValidArrowBlock(node checkNode)
	{
		List<node> list = new List<node>();
		List<node> list2 = new List<node>();
		node node2 = new node();
		node nodeToCheck = new node();
		node node3 = new node();
		bool flag = false;
		bool flag2 = false;
		checkNode.iWasChecked = true;
		if (clockWise)
		{
			if (nodeLookUp.ContainsKey(checkNode.getNodeKey(0, -1)) && isNodeValid(masterNodes[nodeLookUp[checkNode.getNodeKey(0, -1)]]))
			{
				list.Add(masterNodes[nodeLookUp[checkNode.getNodeKey(0, -1)]]);
			}
			if (nodeLookUp.ContainsKey(checkNode.getNodeKey(1, 0)) && isNodeValid(masterNodes[nodeLookUp[checkNode.getNodeKey(1, 0)]]))
			{
				list.Add(masterNodes[nodeLookUp[checkNode.getNodeKey(1, 0)]]);
			}
			if (nodeLookUp.ContainsKey(checkNode.getNodeKey(0, 1)) && isNodeValid(masterNodes[nodeLookUp[checkNode.getNodeKey(0, 1)]]))
			{
				list.Add(masterNodes[nodeLookUp[checkNode.getNodeKey(0, 1)]]);
			}
			if (nodeLookUp.ContainsKey(checkNode.getNodeKey(-1, 0)) && isNodeValid(masterNodes[nodeLookUp[checkNode.getNodeKey(-1, 0)]]))
			{
				list.Add(masterNodes[nodeLookUp[checkNode.getNodeKey(-1, 0)]]);
			}
		}
		else
		{
			if (nodeLookUp.ContainsKey(checkNode.getNodeKey(0, -1)) && isNodeValid(masterNodes[nodeLookUp[checkNode.getNodeKey(0, -1)]]))
			{
				list.Add(masterNodes[nodeLookUp[checkNode.getNodeKey(0, -1)]]);
			}
			if (nodeLookUp.ContainsKey(checkNode.getNodeKey(-1, 0)) && isNodeValid(masterNodes[nodeLookUp[checkNode.getNodeKey(-1, 0)]]))
			{
				list.Add(masterNodes[nodeLookUp[checkNode.getNodeKey(-1, 0)]]);
			}
			if (nodeLookUp.ContainsKey(checkNode.getNodeKey(0, 1)) && isNodeValid(masterNodes[nodeLookUp[checkNode.getNodeKey(0, 1)]]))
			{
				list.Add(masterNodes[nodeLookUp[checkNode.getNodeKey(0, 1)]]);
			}
			if (nodeLookUp.ContainsKey(checkNode.getNodeKey(1, 0)) && isNodeValid(masterNodes[nodeLookUp[checkNode.getNodeKey(1, 0)]]))
			{
				list.Add(masterNodes[nodeLookUp[checkNode.getNodeKey(1, 0)]]);
			}
		}
		if (list.Count > 0)
		{
			if (searchingForActionNodes)
			{
				if (checkNode.myType == DOS_NODE_TYPE.ACTIONNODE)
				{
					node2 = checkNode;
					node2.myType = DOS_NODE_TYPE.ACTIONFILLEDNODE;
					masterNodes[nodeLookUp[checkNode.myPos.x + ":" + checkNode.myPos.y]] = node2;
					flag = true;
				}
				else
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i].myType == DOS_NODE_TYPE.ACTIONNODE)
						{
							node2 = list[i];
							node2.myType = DOS_NODE_TYPE.ACTIONFILLEDNODE;
							node2.myParent = checkNode.myPos.x + ":" + checkNode.myPos.y;
							list[i] = node2;
							masterNodes[nodeLookUp[node2.myPos.x + ":" + node2.myPos.y]] = node2;
							flag = true;
							i = list.Count + 1;
						}
						else
						{
							list2.Add(list[i]);
						}
					}
				}
				if (flag)
				{
					currentActionFilledNodeCount++;
					List<string> list3 = new List<string>();
					list3.Add(node2.myPos.x + ":" + node2.myPos.y);
					list3.Add(node2.myParent);
					if (masterNodes[nodeLookUp[node2.myParent]].myParent != "none")
					{
						bool flag3 = true;
						node node4 = masterNodes[nodeLookUp[node2.myParent]];
						while (flag3)
						{
							if (node4.myParent != "none")
							{
								list3.Add(node4.myParent);
								node4 = masterNodes[nodeLookUp[node4.myParent]];
							}
							else
							{
								flag3 = false;
							}
						}
					}
					for (int num = list3.Count - 1; num >= 0; num--)
					{
						if (!arrowNodes.ContainsKey(list3[num]))
						{
							arrowNodes.Add(list3[num], nodeLookUp[list3[num]]);
						}
						if (masterNodes[nodeLookUp[list3[num]]].myType == DOS_NODE_TYPE.WHITENODE)
						{
							if (num - 1 >= 0)
							{
								node node5 = masterNodes[nodeLookUp[list3[num]]];
								node node6 = masterNodes[nodeLookUp[list3[num - 1]]];
								if (node5.myPos.x == node6.myPos.x)
								{
									if (node5.myPos.y > node6.myPos.y)
									{
										node5.myType = DOS_NODE_TYPE.UPNODE;
										node5.mySprite = upNodeSrpite;
									}
									else
									{
										node5.myType = DOS_NODE_TYPE.DOWNNODE;
										node5.mySprite = downNodeSprite;
									}
								}
								else if (node5.myPos.x > node6.myPos.x)
								{
									node5.myType = DOS_NODE_TYPE.LEFTNODE;
									node5.mySprite = leftNodeSprite;
								}
								else
								{
									node5.myType = DOS_NODE_TYPE.RIGHTNODE;
									node5.mySprite = rightNodeSprite;
								}
								masterNodes[nodeLookUp[list3[num]]] = node5;
							}
							else
							{
								searchingForActionNodes = false;
								searchingForEndNode = false;
							}
						}
					}
					currentTreeNodes.Clear();
					reCount = 0;
					list3.Clear();
					for (int j = 0; j < masterNodes.Count; j++)
					{
						node node7 = masterNodes[j];
						node7.myParent = "none";
						node7.myParentNode = new node();
						node7.myBranches.Clear();
						node7.myBranchNodes.Clear();
						node7.mySibNodes.Clear();
						node7.drillUpNodes.Clear();
						node7.iWasChecked = false;
						masterNodes[j] = node7;
					}
					if (currentActionFilledNodeCount >= ACTION_BLOCK_SIZE)
					{
						searchingForActionNodes = false;
						searchingForEndNode = true;
						string key = string.Empty;
						foreach (KeyValuePair<string, int> arrowNode in arrowNodes)
						{
							key = arrowNode.Key;
						}
						nodeToCheck = masterNodes[nodeLookUp[key]];
					}
				}
				else
				{
					if (!currentTreeNodes.ContainsKey(checkNode.myPos.x + ":" + checkNode.myPos.y))
					{
						currentTreeNodes.Add(checkNode.myPos.x + ":" + checkNode.myPos.y, 0);
					}
					for (int k = 0; k < list2.Count; k++)
					{
						if (!currentTreeNodes.ContainsKey(list2[k].myPos.x + ":" + list2[k].myPos.y))
						{
							node node8 = masterNodes[nodeLookUp[list2[k].myPos.x + ":" + list2[k].myPos.y]];
							node8.myParent = checkNode.myPos.x + ":" + checkNode.myPos.y;
							node8.myParentNode = checkNode;
							masterNodes[nodeLookUp[list2[k].myPos.x + ":" + list2[k].myPos.y]] = node8;
							checkNode.myBranches.Add(list2[k].myPos.x + ":" + list2[k].myPos.y);
							checkNode.myBranchNodes.Add(node8);
							currentTreeNodes.Add(list2[k].myPos.x + ":" + list2[k].myPos.y, 0);
						}
					}
					checkNode.generateSibs();
					searchingForActionNodes = checkNode.getNextNodeToCheck(out nodeToCheck);
				}
			}
			else if (searchingForEndNode)
			{
				if (checkNode.myType == DOS_NODE_TYPE.ENDNODE)
				{
					node3 = checkNode;
					flag2 = true;
				}
				else
				{
					for (int l = 0; l < list.Count; l++)
					{
						if (list[l].myType == DOS_NODE_TYPE.ENDNODE)
						{
							node3 = list[l];
							node3.myParent = checkNode.myPos.x + ":" + checkNode.myPos.y;
							flag2 = true;
							l = list.Count + 1;
						}
						else
						{
							list2.Add(list[l]);
						}
					}
				}
				if (flag2)
				{
					List<string> list4 = new List<string>();
					list4.Add(node3.myPos.x + ":" + node3.myPos.y);
					list4.Add(node3.myParent);
					if (masterNodes[nodeLookUp[node3.myParent]].myParent != "none")
					{
						bool flag4 = true;
						node node9 = masterNodes[nodeLookUp[node3.myParent]];
						while (flag4)
						{
							if (node9.myParent != "none")
							{
								list4.Add(node9.myParent);
								node9 = masterNodes[nodeLookUp[node9.myParent]];
							}
							else
							{
								flag4 = false;
							}
						}
					}
					for (int num2 = list4.Count - 1; num2 >= 0; num2--)
					{
						if (!arrowNodes.ContainsKey(list4[num2]))
						{
							arrowNodes.Add(list4[num2], nodeLookUp[list4[num2]]);
						}
						if (masterNodes[nodeLookUp[list4[num2]]].myType == DOS_NODE_TYPE.WHITENODE)
						{
							if (num2 - 1 >= 0)
							{
								node node10 = masterNodes[nodeLookUp[list4[num2]]];
								node node11 = masterNodes[nodeLookUp[list4[num2 - 1]]];
								if (node10.myPos.x == node11.myPos.x)
								{
									if (node10.myPos.y > node11.myPos.y)
									{
										node10.myType = DOS_NODE_TYPE.UPNODE;
										node10.mySprite = upNodeSrpite;
									}
									else
									{
										node10.myType = DOS_NODE_TYPE.DOWNNODE;
										node10.mySprite = downNodeSprite;
									}
								}
								else if (node10.myPos.x > node11.myPos.x)
								{
									node10.myType = DOS_NODE_TYPE.LEFTNODE;
									node10.mySprite = leftNodeSprite;
								}
								else
								{
									node10.myType = DOS_NODE_TYPE.RIGHTNODE;
									node10.mySprite = rightNodeSprite;
								}
								masterNodes[nodeLookUp[list4[num2]]] = node10;
							}
							else
							{
								searchingForActionNodes = false;
								searchingForEndNode = false;
							}
						}
					}
					currentTreeNodes.Clear();
					reCount = 0;
					list4.Clear();
					for (int m = 0; m < masterNodes.Count; m++)
					{
						node node12 = masterNodes[m];
						node12.myParent = "none";
						node12.myParentNode = new node();
						node12.myBranches.Clear();
						node12.myBranchNodes.Clear();
						node12.mySibNodes.Clear();
						node12.drillUpNodes.Clear();
						node12.iWasChecked = false;
						masterNodes[m] = node12;
					}
				}
				else
				{
					if (!currentTreeNodes.ContainsKey(checkNode.myPos.x + ":" + checkNode.myPos.y))
					{
						currentTreeNodes.Add(checkNode.myPos.x + ":" + checkNode.myPos.y, 0);
					}
					for (int n = 0; n < list2.Count; n++)
					{
						if (!currentTreeNodes.ContainsKey(list2[n].myPos.x + ":" + list2[n].myPos.y))
						{
							node node13 = masterNodes[nodeLookUp[list2[n].myPos.x + ":" + list2[n].myPos.y]];
							node13.myParent = checkNode.myPos.x + ":" + checkNode.myPos.y;
							node13.myParentNode = checkNode;
							masterNodes[nodeLookUp[list2[n].myPos.x + ":" + list2[n].myPos.y]] = node13;
							checkNode.myBranches.Add(list2[n].myPos.x + ":" + list2[n].myPos.y);
							checkNode.myBranchNodes.Add(node13);
							currentTreeNodes.Add(list2[n].myPos.x + ":" + list2[n].myPos.y, 0);
						}
					}
					checkNode.generateSibs();
					searchingForEndNode = checkNode.getNextNodeToCheck(out nodeToCheck);
				}
			}
		}
		else if (!checkNode.getNextNodeToCheck(out nodeToCheck))
		{
			searchingForActionNodes = false;
			searchingForEndNode = false;
		}
		if (searchingForActionNodes)
		{
			if (!flag)
			{
				generateValidArrowBlock(nodeToCheck);
				return;
			}
			string key2 = string.Empty;
			foreach (KeyValuePair<string, int> arrowNode2 in arrowNodes)
			{
				key2 = arrowNode2.Key;
			}
			generateValidArrowBlock(masterNodes[nodeLookUp[key2]]);
		}
		else if (!searchingForEndNode)
		{
			isAStaleMate = true;
		}
		else if (!flag2)
		{
			generateValidArrowBlock(nodeToCheck);
		}
		else
		{
			isAStaleMate = false;
		}
	}

	private bool isNodeValid(node checkNode)
	{
		return checkNode.myType != DOS_NODE_TYPE.STARTNODE && (searchingForEndNode || checkNode.myType != DOS_NODE_TYPE.ENDNODE) && (searchingForEndNode || checkNode.myType != DOS_NODE_TYPE.ACTIONFILLEDNODE) && checkNode.myType != DOS_NODE_TYPE.LEFTNODE && checkNode.myType != DOS_NODE_TYPE.RIGHTNODE && checkNode.myType != DOS_NODE_TYPE.UPNODE && checkNode.myType != DOS_NODE_TYPE.DOWNNODE && !currentTreeNodes.ContainsKey(checkNode.myPos.x + ":" + checkNode.myPos.y) && (searchingForEndNode || !arrowNodes.ContainsKey(checkNode.myPos.x + ":" + checkNode.myPos.y));
	}

	private void drawPuzzle()
	{
		List<int> list = new List<int>(arrowNodes.Values);
		nodeObjects = new Dictionary<string, GameObject>();
		if (masterNodes[list[0]].myPos.x != masterNodes[list[1]].myPos.x)
		{
			if (masterNodes[list[0]].myPos.x > masterNodes[list[1]].myPos.x)
			{
				masterNodes[list[0]].mySubType = DOS_NODE_TYPE.LEFTNODE;
			}
			else
			{
				masterNodes[list[0]].mySubType = DOS_NODE_TYPE.RIGHTNODE;
			}
		}
		else if (masterNodes[list[0]].myPos.y > masterNodes[list[1]].myPos.y)
		{
			masterNodes[list[0]].mySubType = DOS_NODE_TYPE.UPNODE;
		}
		else
		{
			masterNodes[list[0]].mySubType = DOS_NODE_TYPE.DOWNNODE;
		}
		float num = nodeWidth * (MATRIX_SIZE + 1);
		float num2 = nodeHeight * (MATRIX_SIZE + 1);
		nodeHolder = new GameObject("NodeHolder", typeof(RectTransform)).GetComponent<RectTransform>();
		nodeHolder.SetParent(DOSHolder.transform);
		nodeHolderCG = nodeHolder.gameObject.AddComponent<CanvasGroup>();
		nodeHolder.sizeDelta = new Vector2(num, num2);
		nodeHolder.anchorMin = new Vector2(0f, 1f);
		nodeHolder.anchorMax = new Vector2(0f, 1f);
		nodeHolder.pivot = new Vector2(0f, 1f);
		nodeHolder.localScale = new Vector3(1f, 1f, 1f);
		nodeHolder.localPosition = new Vector3(0f - num / 2f, num2 / 2f, 0f);
		nodeBGIMG = new GameObject("nodeBGIMG", typeof(Image)).GetComponent<Image>();
		nodeBGIMG.transform.SetParent(nodeHolder.transform);
		nodeBGIMG.sprite = pixelSprite;
		nodeBGIMG.type = Image.Type.Filled;
		nodeBGIMG.type = Image.Type.Filled;
		nodeBGIMG.fillMethod = Image.FillMethod.Radial360;
		nodeBGIMG.fillAmount = 0f;
		nodeBGIMG.color = nodeBorderColor;
		nodeBGIMG.GetComponent<RectTransform>().sizeDelta = new Vector2(num, num2);
		nodeBGIMG.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1f);
		nodeBGIMG.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 1f);
		nodeBGIMG.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
		nodeBGIMG.transform.localPosition = new Vector3(0f, 0f, 0f);
		nodeBGIMG.transform.localScale = new Vector3(1f, 1f, 1f);
		for (int i = 0; i < masterNodes.Count; i++)
		{
			float setX = i % (MATRIX_SIZE + 1) * nodeWidth;
			float setY = (float)Mathf.FloorToInt(i / (MATRIX_SIZE + 1)) * (0f - (float)nodeHeight);
			GameObject gameObject = Object.Instantiate(NodeObject);
			gameObject.transform.SetParent(nodeHolder.transform);
			gameObject.GetComponent<NodeObject>().myDoSAttack = this;
			gameObject.GetComponent<NodeObject>().trollNode = trollNodesActive;
			gameObject.GetComponent<NodeObject>().buildMe(masterNodes[i], setX, setY, nodeWidth, nodeHeight);
			gameObject.GetComponent<NodeObject>().doSubAction();
			if (masterNodes[i].myType == DOS_NODE_TYPE.STARTNODE)
			{
				startNodeObject = gameObject.GetComponent<NodeObject>();
			}
			else if (masterNodes[i].myType == DOS_NODE_TYPE.ENDNODE)
			{
				endNodeObject = gameObject.GetComponent<NodeObject>();
			}
			nodeObjects.Add(gameObject.GetComponent<NodeObject>().myNodeData.myPos.x + ":" + gameObject.GetComponent<NodeObject>().myNodeData.myPos.y, gameObject);
		}
		DOTween.To(() => nodeBGIMG.fillAmount, delegate(float x)
		{
			nodeBGIMG.fillAmount = x;
		}, 1f, 0.4f).SetEase(Ease.Linear);
		masterNodes.Clear();
		nodeLookUp.Clear();
		startEndNodes.Clear();
		actionBlockNodes.Clear();
	}

	private void setNextHotNode()
	{
		string empty = string.Empty;
		switch (hotNodeObject.myNodeData.myType)
		{
		case DOS_NODE_TYPE.STARTNODE:
			switch (hotNodeObject.myNodeData.mySubType)
			{
			case DOS_NODE_TYPE.LEFTNODE:
				empty = (float)hotNodeObject.myNodeData.myPos.x - 1f + ":" + hotNodeObject.myNodeData.myPos.y;
				if (nodeObjects.ContainsKey(empty))
				{
					hotNodeObject = nodeObjects[empty].GetComponent<NodeObject>();
				}
				else
				{
					hotNodeObject = startNodeObject;
				}
				break;
			case DOS_NODE_TYPE.RIGHTNODE:
				empty = (float)hotNodeObject.myNodeData.myPos.x + 1f + ":" + hotNodeObject.myNodeData.myPos.y;
				if (nodeObjects.ContainsKey(empty))
				{
					hotNodeObject = nodeObjects[empty].GetComponent<NodeObject>();
				}
				else
				{
					hotNodeObject = startNodeObject;
				}
				break;
			case DOS_NODE_TYPE.UPNODE:
			{
				float num3 = (float)hotNodeObject.myNodeData.myPos.y - 1f;
				empty = hotNodeObject.myNodeData.myPos.x + ":" + num3;
				if (nodeObjects.ContainsKey(empty))
				{
					hotNodeObject = nodeObjects[empty].GetComponent<NodeObject>();
				}
				else
				{
					hotNodeObject = startNodeObject;
				}
				break;
			}
			case DOS_NODE_TYPE.DOWNNODE:
			{
				float num2 = (float)hotNodeObject.myNodeData.myPos.y + 1f;
				empty = hotNodeObject.myNodeData.myPos.x + ":" + num2;
				if (nodeObjects.ContainsKey(empty))
				{
					hotNodeObject = nodeObjects[empty].GetComponent<NodeObject>();
				}
				else
				{
					hotNodeObject = startNodeObject;
				}
				break;
			}
			}
			break;
		case DOS_NODE_TYPE.ENDNODE:
			DOSiSHot = false;
			if (allActionNodesActivated)
			{
				DOSAttackFailed();
			}
			else
			{
				DOSAttackSucceeded();
			}
			break;
		case DOS_NODE_TYPE.ACTIONNODE:
			break;
		case DOS_NODE_TYPE.ACTIONFILLEDNODE:
			switch (hotNodeObject.actionNodeDirection)
			{
			case 0:
				hotNodeObject = startNodeObject;
				break;
			case 1:
			{
				float num6 = (float)hotNodeObject.myNodeData.myPos.y - 1f;
				empty = hotNodeObject.myNodeData.myPos.x + ":" + num6;
				if (nodeObjects.ContainsKey(empty))
				{
					hotNodeObject = nodeObjects[empty].GetComponent<NodeObject>();
				}
				else
				{
					hotNodeObject = startNodeObject;
				}
				break;
			}
			case 2:
				empty = (float)hotNodeObject.myNodeData.myPos.x + 1f + ":" + hotNodeObject.myNodeData.myPos.y;
				if (nodeObjects.ContainsKey(empty))
				{
					hotNodeObject = nodeObjects[empty].GetComponent<NodeObject>();
				}
				else
				{
					hotNodeObject = startNodeObject;
				}
				break;
			case 3:
			{
				float num5 = (float)hotNodeObject.myNodeData.myPos.y + 1f;
				empty = hotNodeObject.myNodeData.myPos.x + ":" + num5;
				if (nodeObjects.ContainsKey(empty))
				{
					hotNodeObject = nodeObjects[empty].GetComponent<NodeObject>();
				}
				else
				{
					hotNodeObject = startNodeObject;
				}
				break;
			}
			case 4:
				empty = (float)hotNodeObject.myNodeData.myPos.x - 1f + ":" + hotNodeObject.myNodeData.myPos.y;
				if (nodeObjects.ContainsKey(empty))
				{
					hotNodeObject = nodeObjects[empty].GetComponent<NodeObject>();
				}
				else
				{
					hotNodeObject = startNodeObject;
				}
				break;
			default:
				hotNodeObject = startNodeObject;
				break;
			}
			break;
		case DOS_NODE_TYPE.WHITENODE:
			if (hotNodeObject.trollNode)
			{
				hotNodeObject.nodeImage.sprite = hotNodeObject.myDoSAttack.whiteNodeSprite;
			}
			hotNodeObject = startNodeObject;
			break;
		case DOS_NODE_TYPE.LEFTNODE:
			empty = (float)hotNodeObject.myNodeData.myPos.x - 1f + ":" + hotNodeObject.myNodeData.myPos.y;
			if (nodeObjects.ContainsKey(empty))
			{
				hotNodeObject = nodeObjects[empty].GetComponent<NodeObject>();
			}
			else
			{
				hotNodeObject = startNodeObject;
			}
			break;
		case DOS_NODE_TYPE.RIGHTNODE:
			empty = (float)hotNodeObject.myNodeData.myPos.x + 1f + ":" + hotNodeObject.myNodeData.myPos.y;
			if (nodeObjects.ContainsKey(empty))
			{
				hotNodeObject = nodeObjects[empty].GetComponent<NodeObject>();
			}
			else
			{
				hotNodeObject = startNodeObject;
			}
			break;
		case DOS_NODE_TYPE.UPNODE:
		{
			float num4 = (float)hotNodeObject.myNodeData.myPos.y - 1f;
			empty = hotNodeObject.myNodeData.myPos.x + ":" + num4;
			if (nodeObjects.ContainsKey(empty))
			{
				hotNodeObject = nodeObjects[empty].GetComponent<NodeObject>();
			}
			else
			{
				hotNodeObject = startNodeObject;
			}
			break;
		}
		case DOS_NODE_TYPE.DOWNNODE:
		{
			float num = (float)hotNodeObject.myNodeData.myPos.y + 1f;
			empty = hotNodeObject.myNodeData.myPos.x + ":" + num;
			if (nodeObjects.ContainsKey(empty))
			{
				hotNodeObject = nodeObjects[empty].GetComponent<NodeObject>();
			}
			else
			{
				hotNodeObject = startNodeObject;
			}
			break;
		}
		}
	}

	private void ReloadDOSAttack()
	{
		Object.Destroy(nodeHolder.gameObject);
		prepDOSAttack();
		warmDOSAttack();
	}

	private void DOSAttackSucceeded()
	{
		bool flag = true;
		GameManager.AudioSlinger.KillSound(ClockAlmostUp);
		termLine1.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
		termLine2.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
		termLine3.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
		GameManager.TimeSlinger.FireTimer(0.4f, GameManager.HackerManager.HackingTerminal.TerminalHelper.ClearTerminal);
		nodeObjects.Clear();
		hotNodeObject = null;
		actionNodeActivatedCount = 0;
		allActionNodesActivated = false;
		DOSAttackClockSeq.Kill();
		Object.Destroy(setDOSClockObject);
		if (flag)
		{
			DOSAttackOverSeq = DOTween.Sequence().OnComplete(DOSAttackSucOver);
		}
		else
		{
			DOSAttackOverSeq = DOTween.Sequence().OnComplete(ReloadDOSAttack);
		}
		DOSAttackOverSeq.Insert(0f, DOTween.To(() => nodeBGIMG.fillAmount, delegate(float x)
		{
			nodeBGIMG.fillAmount = x;
		}, 0f, 0.4f).SetEase(Ease.Linear));
		DOSAttackOverSeq.Insert(0.4f, DOTween.To(() => nodeHolderCG.alpha, delegate(float x)
		{
			nodeHolderCG.alpha = x;
		}, 0f, 0.3f).SetEase(Ease.OutSine));
		DOSAttackOverSeq.Play();
		GameManager.HackerManager.HackingTimer.KillHackerTimer();
	}

	private void DOSAttackSucOver()
	{
		Object.Destroy(nodeHolder.gameObject);
		GameManager.HackerManager.PlayerLost();
		GameManager.HackerManager.HackingTimer.KillHackerTimer();
	}

	public void DOSAttackFailed()
	{
		GameManager.AudioSlinger.KillSound(ClockAlmostUp);
		termLine1.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
		termLine2.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
		termLine3.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
		GameManager.TimeSlinger.FireTimer(0.4f, GameManager.HackerManager.HackingTerminal.TerminalHelper.ClearTerminal);
		if ((double)(Time.time - DOSGameTimeStamp) <= (double)DOSTime * 0.4)
		{
			if (DifficultyManager.HackerMode)
			{
				HackerModeManager.Ins.SkillPoints += DOSLevels[currentLevelIndex].SkillPointsA;
			}
			else
			{
				getStaticPointes1 += DOSLevels[currentLevelIndex].SkillPointsA;
			}
		}
		else if (DifficultyManager.HackerMode)
		{
			HackerModeManager.Ins.SkillPoints += DOSLevels[currentLevelIndex].SkillPointsB;
		}
		else
		{
			getStaticPointes1 += DOSLevels[currentLevelIndex].SkillPointsB;
		}
		nodeObjects.Clear();
		hotNodeObject = null;
		actionNodeActivatedCount = 0;
		allActionNodesActivated = false;
		DOSAttackClockSeq.Kill();
		Object.Destroy(setDOSClockObject);
		DOSAttackOverSeq = DOTween.Sequence().OnComplete(DOSAttackFailOver);
		DOSAttackOverSeq.Insert(0f, DOTween.To(() => nodeBGIMG.fillAmount, delegate(float x)
		{
			nodeBGIMG.fillAmount = x;
		}, 0f, 0.4f).SetEase(Ease.Linear));
		DOSAttackOverSeq.Insert(0.4f, DOTween.To(() => nodeHolderCG.alpha, delegate(float x)
		{
			nodeHolderCG.alpha = x;
		}, 0f, 0.3f).SetEase(Ease.OutSine));
		DOSAttackOverSeq.Play();
		GameManager.HackerManager.HackingTimer.KillHackerTimer();
	}

	private void DOSAttackFailOver()
	{
		Object.Destroy(nodeHolder.gameObject);
		GameManager.HackerManager.PlayerWon(currentLevelIndex);
		GameManager.HackerManager.HackingTimer.KillHackerTimer();
	}

	private void tallyBonusPoints()
	{
	}

	private void StageMe()
	{
		GameManager.HackerManager.myDosAttack = this;
		for (int i = 0; i < DOSLevels.Count; i++)
		{
			DOSLevels[i].skillPointesRequired = GameManager.HackerManager.NodeHexerLevelRef[i].SkillPointsRequired;
			DOSLevels[i].SkillPointsA = GameManager.HackerManager.NodeHexerLevelRef[i].PointsRewaredTier1;
			DOSLevels[i].SkillPointsB = GameManager.HackerManager.NodeHexerLevelRef[i].PointsRewaredTier2;
		}
		getStaticPointes1 = 0;
		CountDownTick1 = CustomSoundLookUp.CountDownTick1;
		CountDownTick2 = CustomSoundLookUp.CountDownTick2;
		NodeHot = CustomSoundLookUp.NodeHot;
		NodeCold = CustomSoundLookUp.NodeCold;
		NodeActive = CustomSoundLookUp.NodeActive;
		ExitNodeActive = CustomSoundLookUp.ExitNodeActive;
		ActionNodeClick = CustomSoundLookUp.ActionNodeClick;
		ClockAlmostUp = CustomSoundLookUp.ClockAlmostUp;
		ActionNodeClick.Volume = 0.2f;
		DOSHolder = Object.Instantiate(CustomObjectLookUp.DOSHolder);
		nodeHolderCG = DOSHolder.GetComponent<CanvasGroup>();
		DOSHolder.transform.SetParent(GameObject.Find("UIComputer").transform);
		DOSHolder.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			getStaticPointes1 = DOSLevels[DOSLevels.Count - 1].skillPointesRequired;
		}
	}

	private void TimerAction()
	{
	}

	public void prepCustomDOSAttack(short matrixSize, short actionBlockSize, float hotTime, float gameTimeModifier, bool trollNodesActive)
	{
		GameManager.AudioSlinger.PlaySound(GameManager.HackerManager.HackingTypeSFX);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine1, TERMINAL_LINE_TYPE.TYPE, "> ./DOS_Blocker", 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine2, TERMINAL_LINE_TYPE.TYPE, "  Loading DOS Blocker v0.6b...", 0.2f, 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine3, TERMINAL_LINE_TYPE.TYPE, "  Initialzing...", 0.2f, 0.4f);
		GameManager.TimeSlinger.FireTimer(0.8f, delegate
		{
			MATRIX_SIZE = (short)(matrixSize - 1);
			ACTION_BLOCK_SIZE = actionBlockSize;
			this.hotTime = hotTime;
			DOSTime = (float)(matrixSize * matrixSize) * this.hotTime * gameTimeModifier;
			this.trollNodesActive = trollNodesActive;
			if (Screen.height <= 800)
			{
				nodeWidth = 50;
				nodeHeight = 50;
			}
			else if (Screen.height <= 1300)
			{
				nodeWidth = 75;
				nodeHeight = 75;
			}
			else
			{
				nodeWidth = 100;
				nodeHeight = 100;
			}
			prepPuzzle();
		});
		GameManager.TimeSlinger.FireTimer(1f, warmDOSAttack);
	}

	public int HackerModeSetSkill()
	{
		int result = 0;
		for (int i = 0; i < DOSLevels.Count; i++)
		{
			if (HackerModeManager.Ins.SkillPoints >= DOSLevels[i].skillPointesRequired)
			{
				result = i;
			}
			else
			{
				i = DOSLevels.Count;
			}
		}
		return result;
	}
}
