using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class vapeAttack : MonoBehaviour
{
	public struct vapeNodePosition
	{
		public short x;

		public short y;

		public vapeNodePosition(short x, short y)
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
			vapeNodePosition vapeNodePosition2 = (vapeNodePosition)obj;
			return x == vapeNodePosition2.x && y == vapeNodePosition2.y;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public bool Equals(vapeNodePosition o)
		{
			return x == o.x && y == o.y;
		}

		public static bool operator ==(vapeNodePosition lhs, vapeNodePosition rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(vapeNodePosition lhs, vapeNodePosition rhs)
		{
			return !lhs.Equals(rhs);
		}
	}

	public class vapeNode
	{
		public vapeNodePosition myPOS;

		public Sprite mySprite;

		public vapeNodeType myType;

		public vapeNode()
		{
		}

		public vapeNode(vapeNodeType setType, vapeNodePosition setPOS, Sprite setSprite)
		{
			myType = setType;
			myPOS = setPOS;
			mySprite = setSprite;
		}

		public void updateMyInfo(vapeNodeType newType, Sprite newSprite)
		{
			myType = newType;
			mySprite = newSprite;
		}

		public string getMyPOS()
		{
			return myPOS.x + ":" + myPOS.y;
		}

		public void myInfo()
		{
			Debug.Log("My Type: " + myType);
			Debug.Log("My POS: " + myPOS.x + ":" + myPOS.y);
			Debug.Log("My Sprite:" + mySprite.name);
		}
	}

	public static int getPlayerSkillPoints3;

	[Range(1f, 10f)]
	public float warmUpTime = 5f;

	[Range(5f, 120f)]
	public float VapeTime = 60f;

	[Range(2f, 11f)]
	public short MATRIX_SIZE = 2;

	[Range(0f, 1f)]
	public float FREE_COUNT_PER = 0.5f;

	[Range(2f, 6f)]
	public short GROUP_SIZE = 3;

	public bool HAS_DEAD_NODES;

	[Range(1f, 8f)]
	public short DEAD_NODE_SIZE = 1;

	public int rootVapeNodeWidth = 100;

	public int rootVapeNodeHeight = 100;

	public int vapeNodeWidth = 100;

	public int vapeNodeHeight = 100;

	public short SkillPointsA = 5;

	public short SkillPointsB = 2;

	public AudioFileDefinition boxNodeHoverClip;

	public AudioFileDefinition boxNodeActiveClip;

	public AudioFileDefinition blankNodeHoverClip;

	public AudioFileDefinition goodNodeActiveClip;

	public AudioFileDefinition CountDownTick1;

	public AudioFileDefinition CountDownTick2;

	public AudioFileDefinition ClockAlmostUp;

	public AudioFileDefinition HMPuzzlePass;

	public Font clockFont;

	public Color clockColor;

	public GameObject VapeHolder;

	public CanvasGroup vapeNodeHolderCG;

	public GameObject VapeNodeObject;

	public GameObject VapeClockObject;

	public Sprite blankNodeSprite;

	public Sprite boxNodeSprite;

	public Sprite goodNodeSprite;

	public Sprite deadNodeSprite;

	public Sprite boxNodeHoverSprite;

	public Sprite boxNodeActiveSprite;

	public Sprite blankNodeHoverSprite;

	public Sprite goodNodeActiveSprite;

	public Color nodeBorderColor;

	public Sprite pixelSprite;

	public bool curActiveNodeSet;

	public bool vapeAttackFired;

	public List<VLevelDefinition> VapeLevels;

	public List<VapeChainDefinition> VapeChains;

	[HideInInspector]
	public TerminalLineObject termLine1;

	[HideInInspector]
	public TerminalLineObject termLine2;

	[HideInInspector]
	public TerminalLineObject termLine3;

	private bool addSkillPoints;

	private float clockMicroCount;

	private float clockMicroTimeStamp;

	private Text clockText;

	private float clockTimeStamp;

	private string curActiveNode = string.Empty;

	private int currentLevelIndex;

	private int currentMoveCount;

	private int currentVapeChainIndex;

	private GameObject curVapeClockObject;

	private bool finalCountDownFired;

	private List<vapeNode> masterVapeNodes;

	private Image nodeBGIMG;

	private bool timeIsFrozen;

	private Sequence VapeAttackClockSeq;

	private Sequence VapeAttackOverSeq;

	private float VapeGameTimeStamp;

	private bool vapeIsHot;

	private RectTransform vapeNodeHolder;

	private Dictionary<string, GameObject> vapeNodeObjects;

	private Sequence vapePresentSeq;

	private bool warmClockActive;

	private Sequence warmClockSeq;

	private void Start()
	{
		GameManager.HackerManager.myVapeAttack = this;
		for (int i = 0; i < VapeLevels.Count; i++)
		{
			VapeLevels[i].skillPointesRequired = GameManager.HackerManager.StackPusherLevelRef[i].SkillPointsRequired;
			VapeLevels[i].SkillPointsA = GameManager.HackerManager.StackPusherLevelRef[i].PointsRewaredTier1;
			VapeLevels[i].SkillPointsB = GameManager.HackerManager.StackPusherLevelRef[i].PointsRewaredTier2;
		}
		getPlayerSkillPoints3 = 0;
		VapeHolder = Object.Instantiate(CustomObjectLookUp.vapeHolder);
		vapeNodeHolderCG = VapeHolder.GetComponent<CanvasGroup>();
		VapeHolder.transform.SetParent(GameObject.Find("UIComputer").transform);
		boxNodeHoverClip = CustomSoundLookUp.boxNodeHoverClip;
		boxNodeActiveClip = CustomSoundLookUp.boxNodeActiveClip;
		blankNodeHoverClip = CustomSoundLookUp.blankNodeHoverClip;
		goodNodeActiveClip = CustomSoundLookUp.goodNodeActiveClip;
		CountDownTick1 = CustomSoundLookUp.CountDownTick1;
		CountDownTick2 = CustomSoundLookUp.CountDownTick2;
		ClockAlmostUp = CustomSoundLookUp.ClockAlmostUp;
		VapeHolder.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		if (DifficultyManager.LeetMode || DifficultyManager.Nightmare)
		{
			getPlayerSkillPoints3 = VapeLevels[VapeLevels.Count - 1].skillPointesRequired;
		}
	}

	private void Update()
	{
		if (warmClockActive)
		{
			if (Time.time - clockTimeStamp >= warmUpTime)
			{
				warmClockActive = false;
				fireVapeAttack();
			}
			else if (Time.time - clockMicroTimeStamp >= 1f)
			{
				GameManager.AudioSlinger.PlaySound(CountDownTick1);
				clockMicroCount -= 1f;
				clockMicroTimeStamp = Time.time;
				clockText.text = clockMicroCount.ToString();
			}
		}
		else if (vapeIsHot)
		{
			if (Time.time - VapeGameTimeStamp >= VapeTime - ClockAlmostUp.AudioClip.length && !finalCountDownFired)
			{
				finalCountDownFired = true;
				GameManager.AudioSlinger.PlaySound(ClockAlmostUp);
			}
			if (Time.time - VapeGameTimeStamp >= VapeTime)
			{
				vapeIsHot = false;
				VapeAttackSucceeded();
			}
		}
	}

	public void prepVapeAttack()
	{
		int index = currentLevelIndex;
		MATRIX_SIZE = (short)(VapeLevels[index].matrixSize - 1);
		FREE_COUNT_PER = VapeLevels[index].freeCountPer;
		GROUP_SIZE = VapeLevels[index].groupSize;
		HAS_DEAD_NODES = VapeLevels[index].hasDeadNodes;
		DEAD_NODE_SIZE = VapeLevels[index].deadNodeSize;
		int num = Mathf.FloorToInt((float)Mathf.CeilToInt((float)((MATRIX_SIZE + 1) * (MATRIX_SIZE + 1)) / 2f) * FREE_COUNT_PER);
		VapeTime = (float)num * VapeLevels[index].timePerBlock;
		if (Screen.height <= 800)
		{
			vapeNodeWidth = 50;
			vapeNodeHeight = 50;
		}
		else if (Screen.height <= 1300)
		{
			if (MATRIX_SIZE >= 8)
			{
				vapeNodeWidth = 75;
				vapeNodeHeight = 75;
			}
			else
			{
				vapeNodeWidth = 75;
				vapeNodeHeight = 75;
			}
		}
		else
		{
			vapeNodeWidth = 100;
			vapeNodeHeight = 100;
		}
		prepPuzzle();
	}

	public void warmVapeAttack()
	{
		vapePresentSeq = DOTween.Sequence();
		vapePresentSeq.Insert(0f, DOTween.To(() => vapeNodeHolderCG.alpha, delegate(float x)
		{
			vapeNodeHolderCG.alpha = x;
		}, 1f, 0.5f).SetEase(Ease.OutQuad));
		vapePresentSeq.Play();
		int num = 72;
		float num2 = (float)vapeNodeWidth / (float)rootVapeNodeWidth;
		num = Mathf.RoundToInt((float)num * num2);
		TextGenerationSettings settings = default(TextGenerationSettings);
		TextGenerator textGenerator = new TextGenerator();
		settings.textAnchor = TextAnchor.UpperCenter;
		settings.generateOutOfBounds = true;
		settings.generationExtents = new Vector2(50f, 20f);
		settings.pivot = Vector2.zero;
		settings.richText = true;
		settings.font = clockFont;
		settings.fontSize = num;
		settings.fontStyle = FontStyle.Normal;
		settings.lineSpacing = 1f;
		settings.scaleFactor = 1f;
		settings.verticalOverflow = VerticalWrapMode.Overflow;
		settings.horizontalOverflow = HorizontalWrapMode.Wrap;
		GameManager.HackerManager.HackingTimer.FireWarmUpTimer(3);
		clockText = new GameObject("clockText", typeof(Text)).GetComponent<Text>();
		clockText.transform.SetParent(VapeHolder.transform);
		clockText.transform.localPosition = new Vector3(0f, VapeHolder.GetComponent<RectTransform>().sizeDelta.y / 2f + 75f, 0f);
		clockText.font = clockFont;
		clockText.text = warmUpTime.ToString();
		clockText.color = clockColor;
		clockText.fontSize = num;
		clockText.rectTransform.sizeDelta = new Vector2(textGenerator.GetPreferredWidth(clockText.text, settings), textGenerator.GetPreferredHeight(clockText.text, settings));
		clockText.transform.localScale = new Vector3(1f, 1f, 1f);
		clockText.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
		warmClockSeq = DOTween.Sequence();
		warmClockSeq.Insert(0f, DOTween.To(() => clockText.transform.localScale, delegate(Vector3 x)
		{
			clockText.transform.localScale = x;
		}, new Vector3(1f, 1f, 1f), 0f));
		warmClockSeq.Insert(0.1f, DOTween.To(() => clockText.transform.localScale, delegate(Vector3 x)
		{
			clockText.transform.localScale = x;
		}, new Vector3(0.33f, 0.33f, 0.33f), 0.9f).SetEase(Ease.Linear));
		warmClockSeq.SetLoops(5);
		warmClockSeq.Play();
		clockTimeStamp = Time.time;
		clockMicroTimeStamp = Time.time;
		clockMicroCount = warmUpTime;
		warmClockActive = true;
		GameManager.AudioSlinger.PlaySound(CountDownTick1);
	}

	public void fireVapeAttack()
	{
		Object.Destroy(clockText.gameObject);
		GameManager.AudioSlinger.PlaySound(CountDownTick2);
		float num = (float)vapeNodeWidth / (float)rootVapeNodeWidth;
		float num2 = (float)vapeNodeHeight / (float)rootVapeNodeHeight;
		finalCountDownFired = false;
		VapeGameTimeStamp = Time.time;
		vapeIsHot = true;
		vapeAttackFired = true;
		float vapeTime = VapeTime;
		GameManager.HackerManager.HackingTimer.FireHackingTimer(VapeTime, TimerAction);
	}

	public void vapeNodeAction(vapeNode vnData)
	{
		bool flag = false;
		List<int> list = new List<int>();
		if (!vapeAttackFired || vnData.myType == vapeNodeType.DEADNODE || vnData.myType == vapeNodeType.GOODNODE)
		{
			return;
		}
		if (!curActiveNodeSet)
		{
			if (vnData.myType != vapeNodeType.BLANKNODE)
			{
				curActiveNodeSet = true;
				curActiveNode = vnData.getMyPOS();
			}
		}
		else if (vnData.myType == vapeNodeType.BLANKNODE)
		{
			if (vapeNodeObjects.ContainsKey(curActiveNode) && vapeNodeObjects.ContainsKey(vnData.getMyPOS()))
			{
				vapeNodeObjects[curActiveNode].GetComponent<VapeNodeObject>().updateMyType(vapeNodeType.BLANKNODE);
				vapeNodeObjects[curActiveNode].GetComponent<VapeNodeObject>().clearActiveState();
				vapeNodeObjects[vnData.getMyPOS()].GetComponent<VapeNodeObject>().updateMyType(vapeNodeType.BOXNODE);
				vapeNodeObjects[vnData.getMyPOS()].GetComponent<VapeNodeObject>().clearActiveState();
				flag = true;
			}
			curActiveNodeSet = false;
			curActiveNode = string.Empty;
		}
		else
		{
			vapeNodeObjects[curActiveNode].GetComponent<VapeNodeObject>().clearActiveState();
			curActiveNodeSet = false;
			curActiveNode = string.Empty;
		}
		if (!flag)
		{
			return;
		}
		list = getCurActiveBoxNodes();
		if (list.Count <= 0)
		{
			return;
		}
		bool flag2 = true;
		for (int i = 0; i < list.Count; i++)
		{
			if (checkBoxNodeIsGood(masterVapeNodes[list[i]]))
			{
				if (masterVapeNodes[list[i]].myType != vapeNodeType.GOODNODE)
				{
					vapeNodeObjects[masterVapeNodes[list[i]].getMyPOS()].GetComponent<VapeNodeObject>().setAsGoodNode();
				}
			}
			else
			{
				vapeNodeObjects[masterVapeNodes[list[i]].getMyPOS()].GetComponent<VapeNodeObject>().updateMyType(vapeNodeType.BOXNODE);
				flag2 = false;
			}
		}
		if (flag2)
		{
			VapeAttackFailed();
		}
	}

	public void freezeTime()
	{
		GameManager.AudioSlinger.KillSound(ClockAlmostUp);
		finalCountDownFired = true;
		VapeAttackClockSeq.Pause();
		timeIsFrozen = true;
		GameManager.TimeSlinger.FireTimer(30f, unFreezeTime);
	}

	private void prepPuzzle()
	{
		int num = 0;
		int num2 = 0;
		List<int> list = new List<int>();
		masterVapeNodes = new List<vapeNode>();
		int num3 = Mathf.FloorToInt((float)Mathf.CeilToInt((float)((MATRIX_SIZE + 1) * (MATRIX_SIZE + 1)) / 2f) * FREE_COUNT_PER);
		for (int i = 0; i < (MATRIX_SIZE + 1) * (MATRIX_SIZE + 1); i++)
		{
			short x = (short)(i % (MATRIX_SIZE + 1));
			short y = (short)Mathf.FloorToInt(i / (MATRIX_SIZE + 1));
			masterVapeNodes.Add(new vapeNode(vapeNodeType.BLANKNODE, new vapeNodePosition(x, y), blankNodeSprite));
		}
		int max = Mathf.CeilToInt(((float)MATRIX_SIZE + 1f) * ((float)MATRIX_SIZE + 1f) / Mathf.Ceil((float)num3 / (float)GROUP_SIZE));
		int num4 = Random.Range(1, max);
		for (int j = 0; j < num3; j++)
		{
			int num5 = num + j % GROUP_SIZE;
			if (num5 < masterVapeNodes.Count)
			{
				masterVapeNodes[num5].updateMyInfo(vapeNodeType.BOXNODE, boxNodeSprite);
				num2++;
				if (num2 > GROUP_SIZE - 1)
				{
					int num6 = num + GROUP_SIZE + num4;
					num = ((num6 + GROUP_SIZE >= masterVapeNodes.Count) ? (num + GROUP_SIZE) : num6);
					num2 = 0;
					num4 = Random.Range(1, max);
				}
			}
		}
		if (HAS_DEAD_NODES)
		{
			list = getCurBlankBoxNodes();
			for (int k = 0; k < DEAD_NODE_SIZE; k++)
			{
				int index = Random.Range(0, list.Count);
				masterVapeNodes[list[index]].updateMyInfo(vapeNodeType.DEADNODE, deadNodeSprite);
				list.RemoveAt(index);
			}
		}
		drawPuzzle();
	}

	private void drawPuzzle()
	{
		vapeNodeObjects = new Dictionary<string, GameObject>();
		float num = vapeNodeWidth * (MATRIX_SIZE + 1);
		float num2 = vapeNodeHeight * (MATRIX_SIZE + 1);
		float x = 0f - num / 2f;
		float y = num2 / 2f;
		VapeHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(num, num2);
		vapeNodeHolder = new GameObject("VapeNodeHolder", typeof(RectTransform)).GetComponent<RectTransform>();
		vapeNodeHolder.GetComponent<RectTransform>().SetParent(VapeHolder.transform);
		vapeNodeHolderCG.alpha = 0f;
		vapeNodeHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(num, num2);
		vapeNodeHolder.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1f);
		vapeNodeHolder.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 1f);
		vapeNodeHolder.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
		vapeNodeHolder.transform.localScale = new Vector3(1f, 1f, 1f);
		vapeNodeHolder.transform.localPosition = new Vector3(x, y, 0f);
		nodeBGIMG = new GameObject("nodeBGIMG", typeof(Image)).GetComponent<Image>();
		nodeBGIMG.transform.SetParent(vapeNodeHolder.transform);
		nodeBGIMG.sprite = pixelSprite;
		nodeBGIMG.type = Image.Type.Filled;
		nodeBGIMG.fillMethod = Image.FillMethod.Radial360;
		nodeBGIMG.fillAmount = 1f;
		nodeBGIMG.color = nodeBorderColor;
		nodeBGIMG.GetComponent<RectTransform>().sizeDelta = new Vector2(num, num2);
		nodeBGIMG.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1f);
		nodeBGIMG.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 1f);
		nodeBGIMG.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
		nodeBGIMG.transform.localPosition = new Vector3(0f, 0f, 0f);
		nodeBGIMG.transform.localScale = new Vector3(1f, 1f, 1f);
		for (int i = 0; i < masterVapeNodes.Count; i++)
		{
			float setX = i % (MATRIX_SIZE + 1) * vapeNodeWidth;
			float setY = (float)Mathf.FloorToInt(i / (MATRIX_SIZE + 1)) * (0f - (float)vapeNodeHeight);
			GameObject gameObject = Object.Instantiate(VapeNodeObject);
			gameObject.GetComponent<VapeNodeObject>().blankNodeSprite = blankNodeSprite;
			gameObject.GetComponent<VapeNodeObject>().boxNodeSprite = boxNodeSprite;
			gameObject.GetComponent<VapeNodeObject>().goodNodeSprite = goodNodeSprite;
			gameObject.GetComponent<VapeNodeObject>().deadNodeSprite = deadNodeSprite;
			gameObject.GetComponent<VapeNodeObject>().boxNodeHoverSprite = boxNodeHoverSprite;
			gameObject.GetComponent<VapeNodeObject>().boxNodeActiveSprite = boxNodeActiveSprite;
			gameObject.GetComponent<VapeNodeObject>().blankNodeHoverSprite = blankNodeHoverSprite;
			gameObject.GetComponent<VapeNodeObject>().goodNodeActiveSprite = goodNodeActiveSprite;
			gameObject.transform.SetParent(vapeNodeHolder.transform);
			gameObject.GetComponent<VapeNodeObject>().buildMe(this, masterVapeNodes[i], setX, setY, vapeNodeWidth, vapeNodeHeight);
			vapeNodeObjects.Add(gameObject.GetComponent<VapeNodeObject>().myVapeNodeData.myPOS.x + ":" + gameObject.GetComponent<VapeNodeObject>().myVapeNodeData.myPOS.y, gameObject);
		}
		curActiveNodeSet = false;
		curActiveNode = string.Empty;
	}

	private List<int> getCurBlankBoxNodes()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < masterVapeNodes.Count; i++)
		{
			if (masterVapeNodes[i].myType == vapeNodeType.BLANKNODE)
			{
				list.Add(i);
			}
		}
		return list;
	}

	private List<int> getCurActiveBoxNodes()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < masterVapeNodes.Count; i++)
		{
			if (masterVapeNodes[i].myType == vapeNodeType.BOXNODE || masterVapeNodes[i].myType == vapeNodeType.GOODNODE)
			{
				list.Add(i);
			}
		}
		return list;
	}

	private bool checkBoxNodeIsGood(vapeNode theVapeNode)
	{
		bool result = true;
		if (theVapeNode.myPOS.x - 1 >= 0 && vapeNodeObjects.ContainsKey(theVapeNode.myPOS.x - 1 + ":" + theVapeNode.myPOS.y) && vapeNodeObjects[theVapeNode.myPOS.x - 1 + ":" + theVapeNode.myPOS.y].GetComponent<VapeNodeObject>().myVapeNodeData.myType != vapeNodeType.BLANKNODE && vapeNodeObjects[theVapeNode.myPOS.x - 1 + ":" + theVapeNode.myPOS.y].GetComponent<VapeNodeObject>().myVapeNodeData.myType != vapeNodeType.DEADNODE)
		{
			result = false;
		}
		if (theVapeNode.myPOS.x + 1 <= MATRIX_SIZE && vapeNodeObjects.ContainsKey(theVapeNode.myPOS.x + 1 + ":" + theVapeNode.myPOS.y) && vapeNodeObjects[theVapeNode.myPOS.x + 1 + ":" + theVapeNode.myPOS.y].GetComponent<VapeNodeObject>().myVapeNodeData.myType != vapeNodeType.BLANKNODE && vapeNodeObjects[theVapeNode.myPOS.x + 1 + ":" + theVapeNode.myPOS.y].GetComponent<VapeNodeObject>().myVapeNodeData.myType != vapeNodeType.DEADNODE)
		{
			result = false;
		}
		if (theVapeNode.myPOS.y - 1 >= 0 && vapeNodeObjects.ContainsKey(theVapeNode.myPOS.x + ":" + (theVapeNode.myPOS.y - 1)) && vapeNodeObjects[theVapeNode.myPOS.x + ":" + (theVapeNode.myPOS.y - 1)].GetComponent<VapeNodeObject>().myVapeNodeData.myType != vapeNodeType.BLANKNODE && vapeNodeObjects[theVapeNode.myPOS.x + ":" + (theVapeNode.myPOS.y - 1)].GetComponent<VapeNodeObject>().myVapeNodeData.myType != vapeNodeType.DEADNODE)
		{
			result = false;
		}
		if (theVapeNode.myPOS.y + 1 <= MATRIX_SIZE && vapeNodeObjects.ContainsKey(theVapeNode.myPOS.x + ":" + (theVapeNode.myPOS.y + 1)) && vapeNodeObjects[theVapeNode.myPOS.x + ":" + (theVapeNode.myPOS.y + 1)].GetComponent<VapeNodeObject>().myVapeNodeData.myType != vapeNodeType.BLANKNODE && vapeNodeObjects[theVapeNode.myPOS.x + ":" + (theVapeNode.myPOS.y + 1)].GetComponent<VapeNodeObject>().myVapeNodeData.myType != vapeNodeType.DEADNODE)
		{
			result = false;
		}
		return result;
	}

	private void VapeAttackFailed()
	{
		GameManager.AudioSlinger.KillSound(ClockAlmostUp);
		termLine1.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
		termLine2.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
		termLine3.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
		GameManager.TimeSlinger.FireTimer(0.3f, GameManager.HackerManager.HackingTerminal.TerminalHelper.ClearTerminal);
		if ((double)(Time.time - VapeGameTimeStamp) <= (double)VapeTime * 0.45)
		{
			if (DifficultyManager.HackerMode)
			{
				HackerModeManager.Ins.SkillPoints += VapeLevels[currentLevelIndex].SkillPointsA;
			}
			else
			{
				getPlayerSkillPoints3 += VapeLevels[currentLevelIndex].SkillPointsA;
			}
		}
		else if (DifficultyManager.HackerMode)
		{
			HackerModeManager.Ins.SkillPoints += VapeLevels[currentLevelIndex].SkillPointsB;
		}
		else
		{
			getPlayerSkillPoints3 += VapeLevels[currentLevelIndex].SkillPointsB;
		}
		masterVapeNodes.Clear();
		vapeNodeObjects.Clear();
		curActiveNodeSet = false;
		curActiveNode = string.Empty;
		vapeAttackFired = false;
		vapeIsHot = false;
		VapeAttackClockSeq.Kill();
		Object.Destroy(curVapeClockObject);
		VapeAttackOverSeq = DOTween.Sequence().OnComplete(VapeAttackFailOver);
		VapeAttackOverSeq.Insert(0f, DOTween.To(() => vapeNodeHolderCG.alpha, delegate(float x)
		{
			vapeNodeHolderCG.alpha = x;
		}, 0f, 0.4f).SetEase(Ease.OutSine));
		VapeAttackOverSeq.Play();
		GameManager.HackerManager.HackingTimer.KillHackerTimer();
	}

	private void VapeAttackSucceeded()
	{
		GameManager.AudioSlinger.KillSound(ClockAlmostUp);
		termLine1.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
		termLine2.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
		termLine3.AniHardClear(TERMINAL_LINE_TYPE.TYPE, 0.25f);
		GameManager.TimeSlinger.FireTimer(0.3f, GameManager.HackerManager.HackingTerminal.TerminalHelper.ClearTerminal);
		masterVapeNodes.Clear();
		vapeNodeObjects.Clear();
		curActiveNodeSet = false;
		curActiveNode = string.Empty;
		vapeAttackFired = false;
		vapeIsHot = false;
		VapeAttackClockSeq.Kill();
		GameManager.HackerManager.HackingTimer.KillHackerTimer();
		Object.Destroy(curVapeClockObject);
		VapeAttackOverSeq = DOTween.Sequence().OnComplete(VapeAttackSucOver);
		VapeAttackOverSeq.Insert(0f, DOTween.To(() => vapeNodeHolderCG.alpha, delegate(float x)
		{
			vapeNodeHolderCG.alpha = x;
		}, 0f, 0.4f).SetEase(Ease.OutSine));
		VapeAttackOverSeq.Play();
	}

	private void VapeAttackFailOver()
	{
		Object.Destroy(vapeNodeHolder.gameObject);
		VapeHolder.GetComponent<CanvasGroup>().alpha = 0f;
		GameManager.HackerManager.PlayerWon(currentLevelIndex);
		GameManager.HackerManager.HackingTimer.KillHackerTimer();
	}

	private void VapeAttackSucOver()
	{
		Object.Destroy(vapeNodeHolder.gameObject);
		VapeHolder.GetComponent<CanvasGroup>().alpha = 0f;
		GameManager.HackerManager.PlayerLost();
		GameManager.HackerManager.HackingTimer.KillHackerTimer();
	}

	private void unFreezeTime()
	{
		VapeAttackClockSeq.Play();
		timeIsFrozen = false;
	}

	public void CreateNewVapeAttack(HACK_SWEEPER_SKILL_TIER SetTier)
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
			currentLevelIndex = VapeLevels.Count - 1;
			break;
		case HACK_SWEEPER_SKILL_TIER.HACKER_MODE:
			currentLevelIndex = HackerModeSetSkill();
			break;
		}
		if (currentLevelIndex > VapeLevels.Count - 1)
		{
			currentLevelIndex = VapeLevels.Count - 1;
		}
		Debug.Log("[Vape Attack] Current level:" + (currentLevelIndex + 1));
		prepVapeAttack();
	}

	private void setCurrentLevelIndex()
	{
		for (int i = 0; i < VapeLevels.Count; i++)
		{
			if (getPlayerSkillPoints3 >= VapeLevels[i].skillPointesRequired)
			{
				currentLevelIndex = i;
			}
			else
			{
				i = VapeLevels.Count;
			}
		}
	}

	private void TimerAction()
	{
	}

	public void CreateNewVapeAttackTarot(int level)
	{
		GameManager.AudioSlinger.PlaySound(GameManager.HackerManager.HackingTypeSFX);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine1, TERMINAL_LINE_TYPE.TYPE, "> ./CLOUDGRID", 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine2, TERMINAL_LINE_TYPE.TYPE, "  Loading CLOUDGRID v.AP3nA710N...", 0.2f, 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine3, TERMINAL_LINE_TYPE.TYPE, "  Initialzing...", 0.2f, 0.4f);
		setCurrentLevelIndex();
		currentLevelIndex = level;
		if (currentLevelIndex > VapeLevels.Count - 1)
		{
			currentLevelIndex = VapeLevels.Count - 1;
		}
		Debug.Log("[Vape Attack] Current level:" + (currentLevelIndex + 1));
		prepVapeAttack();
		GameManager.TimeSlinger.FireTimer(1f, warmVapeAttack);
	}

	public void prepCustomVapeAttack(short matrixSize, float freeCountPer, short groupSize, short deadNodeSize, float timePerBlock)
	{
		GameManager.AudioSlinger.PlaySound(GameManager.HackerManager.HackingTypeSFX);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine1, TERMINAL_LINE_TYPE.TYPE, "> ./CLOUDGRID", 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine2, TERMINAL_LINE_TYPE.TYPE, "  Loading CLOUDGRID v.AP3nA710N...", 0.2f, 0.2f);
		GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine3, TERMINAL_LINE_TYPE.TYPE, "  Initialzing...", 0.2f, 0.4f);
		bool hAS_DEAD_NODES = deadNodeSize > 0;
		MATRIX_SIZE = (short)(matrixSize - 1);
		FREE_COUNT_PER = freeCountPer;
		GROUP_SIZE = groupSize;
		HAS_DEAD_NODES = hAS_DEAD_NODES;
		DEAD_NODE_SIZE = deadNodeSize;
		int num = Mathf.FloorToInt((float)Mathf.CeilToInt((float)((MATRIX_SIZE + 1) * (MATRIX_SIZE + 1)) / 2f) * FREE_COUNT_PER);
		VapeTime = (float)num * timePerBlock;
		int num2 = (MATRIX_SIZE + 1) * (MATRIX_SIZE + 1) - num;
		if (DEAD_NODE_SIZE > num2)
		{
			DEAD_NODE_SIZE = (short)num2;
		}
		if (Screen.height <= 800)
		{
			vapeNodeWidth = 50;
			vapeNodeHeight = 50;
		}
		else if (Screen.height <= 1300)
		{
			if (MATRIX_SIZE >= 8)
			{
				vapeNodeWidth = 75;
				vapeNodeHeight = 75;
			}
			else
			{
				vapeNodeWidth = 75;
				vapeNodeHeight = 75;
			}
		}
		else
		{
			vapeNodeWidth = 100;
			vapeNodeHeight = 100;
		}
		prepPuzzle();
		GameManager.TimeSlinger.FireTimer(0.8f, warmVapeAttack);
	}

	public int HackerModeSetSkill()
	{
		int result = 0;
		for (int i = 0; i < VapeLevels.Count; i++)
		{
			if (HackerModeManager.Ins.SkillPoints >= VapeLevels[i].skillPointesRequired)
			{
				result = i;
			}
			else
			{
				i = VapeLevels.Count;
			}
		}
		return result;
	}
}
