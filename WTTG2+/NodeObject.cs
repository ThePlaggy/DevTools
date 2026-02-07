using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class NodeObject : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public Image nodeBGImage;

	public Image nodeImage;

	public Image nodeHotImage;

	public DOSAttack myDoSAttack;

	public short actionNodeDirection;

	public bool actionNodeActive;

	public bool trollNode;

	private Image actionArrowIMG;

	private Sequence myAniSeq;

	public DOSAttack.node myNodeData;

	private float mySetH = 100f;

	private float mySetW = 100f;

	private Image startNodeArr;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (myNodeData.myType == DOS_NODE_TYPE.ACTIONFILLEDNODE && CrossPlatformInputManager.GetButtonDown("LeftClick"))
		{
			actionNodeDirection++;
			GameManager.AudioSlinger.PlaySound(myDoSAttack.ActionNodeClick);
			if (actionNodeDirection > 4)
			{
				myDoSAttack.addNodeClickCount();
				actionNodeDirection = 1;
			}
			float num = mySetW / (float)myDoSAttack.rootNodeWidth;
			float num2 = mySetH / (float)myDoSAttack.rootNodeHeight;
			switch (actionNodeDirection)
			{
			case 1:
				actionArrowIMG.sprite = myDoSAttack.actionNodeUpA;
				actionArrowIMG.color = new Color(1f, 1f, 1f, 1f);
				actionArrowIMG.GetComponent<RectTransform>().sizeDelta = new Vector2(myDoSAttack.actionNodeUpA.rect.width * num, myDoSAttack.actionNodeUpA.rect.height * num2);
				actionArrowIMG.GetComponent<RectTransform>().localPosition = new Vector3(mySetW / 2f - myDoSAttack.actionNodeUpA.rect.width * num / 2f, myDoSAttack.actionNodeUpA.rect.height * num / 2f - mySetH / 2f, 0f);
				break;
			case 2:
				actionArrowIMG.sprite = myDoSAttack.actionNodeRightA;
				actionArrowIMG.color = new Color(1f, 1f, 1f, 1f);
				actionArrowIMG.GetComponent<RectTransform>().sizeDelta = new Vector2(myDoSAttack.actionNodeRightA.rect.width * num, myDoSAttack.actionNodeRightA.rect.height * num2);
				actionArrowIMG.GetComponent<RectTransform>().localPosition = new Vector3(mySetW / 2f - myDoSAttack.actionNodeRightA.rect.width * num / 2f, myDoSAttack.actionNodeRightA.rect.height * num / 2f - mySetH / 2f, 0f);
				break;
			case 3:
				actionArrowIMG.sprite = myDoSAttack.actionNodeDownA;
				actionArrowIMG.color = new Color(1f, 1f, 1f, 1f);
				actionArrowIMG.GetComponent<RectTransform>().sizeDelta = new Vector2(myDoSAttack.actionNodeDownA.rect.width * num, myDoSAttack.actionNodeDownA.rect.height * num2);
				actionArrowIMG.GetComponent<RectTransform>().localPosition = new Vector3(mySetW / 2f - myDoSAttack.actionNodeDownA.rect.width * num / 2f, myDoSAttack.actionNodeDownA.rect.height * num / 2f - mySetH / 2f, 0f);
				break;
			case 4:
				actionArrowIMG.sprite = myDoSAttack.actionNodeLeftA;
				actionArrowIMG.color = new Color(1f, 1f, 1f, 1f);
				actionArrowIMG.GetComponent<RectTransform>().sizeDelta = new Vector2(myDoSAttack.actionNodeLeftA.rect.width * num, myDoSAttack.actionNodeLeftA.rect.height * num2);
				actionArrowIMG.GetComponent<RectTransform>().localPosition = new Vector3(mySetW / 2f - myDoSAttack.actionNodeLeftA.rect.width * num / 2f, myDoSAttack.actionNodeLeftA.rect.height * num / 2f - mySetH / 2f, 0f);
				break;
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (myNodeData.myType == DOS_NODE_TYPE.ACTIONFILLEDNODE)
		{
			if (actionNodeActive)
			{
				nodeImage.sprite = myDoSAttack.actionNodeActivatedHoverSprite;
			}
			else
			{
				nodeImage.sprite = myDoSAttack.actionNodeHoverSprite;
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (myNodeData.myType == DOS_NODE_TYPE.ACTIONFILLEDNODE)
		{
			if (actionNodeActive)
			{
				nodeImage.sprite = myDoSAttack.actionNodeActivatedSprite;
			}
			else
			{
				nodeImage.sprite = myDoSAttack.actionNodeSprite;
			}
		}
	}

	public void buildMe(DOSAttack.node setNodeData = null, float setX = 0f, float setY = 0f, float setWidth = 100f, float setHeight = 100f)
	{
		myNodeData = setNodeData;
		nodeImage.sprite = myNodeData.mySprite;
		mySetW = setWidth;
		mySetH = setHeight;
		base.gameObject.AddComponent<GraphicsRayCasterCatcher>();
		nodeBGImage.GetComponent<RectTransform>().sizeDelta = new Vector2(setWidth, setHeight);
		nodeImage.GetComponent<RectTransform>().sizeDelta = new Vector2(setWidth - 4f, setHeight - 4f);
		nodeHotImage.GetComponent<RectTransform>().sizeDelta = new Vector2(setWidth - 4f, setHeight - 4f);
		GetComponent<RectTransform>().sizeDelta = new Vector2(setWidth, setHeight);
		GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1f);
		GetComponent<RectTransform>().anchorMax = new Vector2(0f, 1f);
		GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		base.transform.localPosition = new Vector3(setX, setY, 0f);
	}

	public void doSubAction()
	{
		if (myNodeData.myType == DOS_NODE_TYPE.STARTNODE)
		{
			switch (myNodeData.mySubType)
			{
			case DOS_NODE_TYPE.LEFTNODE:
				nodeImage.sprite = myDoSAttack.leftNodeSprite;
				break;
			case DOS_NODE_TYPE.RIGHTNODE:
				nodeImage.sprite = myDoSAttack.rightNodeSprite;
				break;
			case DOS_NODE_TYPE.UPNODE:
				nodeImage.sprite = myDoSAttack.upNodeSrpite;
				break;
			case DOS_NODE_TYPE.DOWNNODE:
				nodeImage.sprite = myDoSAttack.downNodeSprite;
				break;
			}
			float num = mySetW / (float)myDoSAttack.rootNodeWidth;
			float num2 = mySetH / (float)myDoSAttack.rootNodeHeight;
			startNodeArr = new GameObject("sn1337", typeof(Image)).GetComponent<Image>();
			startNodeArr.sprite = myDoSAttack.startNodeDownArrowSprite;
			startNodeArr.GetComponent<RectTransform>().SetParent(base.transform);
			startNodeArr.GetComponent<RectTransform>().sizeDelta = new Vector2(myDoSAttack.startNodeDownArrowSprite.rect.width * num, myDoSAttack.startNodeDownArrowSprite.rect.height * num2);
			startNodeArr.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1f);
			startNodeArr.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 1f);
			startNodeArr.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
			startNodeArr.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
			startNodeArr.GetComponent<RectTransform>().localPosition = new Vector3(mySetW / 2f - myDoSAttack.startNodeDownArrowSprite.rect.width * num / 2f, myDoSAttack.nodeHeight, 0f);
			aniStartNodeArrow();
		}
		else if (myNodeData.myType == DOS_NODE_TYPE.ACTIONFILLEDNODE)
		{
			actionArrowIMG = new GameObject("actionARRIMG", typeof(Image)).GetComponent<Image>();
			actionArrowIMG.GetComponent<RectTransform>().SetParent(base.transform);
			actionArrowIMG.color = new Color(1f, 1f, 1f, 0f);
			actionArrowIMG.GetComponent<RectTransform>().sizeDelta = new Vector2(mySetW, mySetH);
			actionArrowIMG.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1f);
			actionArrowIMG.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 1f);
			actionArrowIMG.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
			actionArrowIMG.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
			actionArrowIMG.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
		}
		else if (myNodeData.myType == DOS_NODE_TYPE.WHITENODE && trollNode && Random.Range(0, 100) >= 50)
		{
			switch (Random.Range(1, 5))
			{
			case 1:
				nodeImage.sprite = myDoSAttack.trollLeftNodeSprite;
				break;
			case 2:
				nodeImage.sprite = myDoSAttack.trollDownNodeSprite;
				break;
			case 3:
				nodeImage.sprite = myDoSAttack.trollRightNodeSprite;
				break;
			case 4:
				nodeImage.sprite = myDoSAttack.trollUpNodeSprite;
				break;
			}
		}
	}

	public void tap()
	{
		nodeHotImage.color = new Color(1f, 1f, 1f, 1f);
		if (myNodeData.myType == DOS_NODE_TYPE.ACTIONFILLEDNODE && !actionNodeActive)
		{
			GameManager.AudioSlinger.PlaySound(myDoSAttack.NodeActive);
			nodeImage.sprite = myDoSAttack.actionNodeActivatedSprite;
			actionNodeActive = true;
			myDoSAttack.actionNodeActivated();
		}
		if (myNodeData.myType == DOS_NODE_TYPE.WHITENODE)
		{
			GameManager.AudioSlinger.PlaySound(myDoSAttack.NodeCold);
		}
		else if (myNodeData.myType == DOS_NODE_TYPE.ACTIONFILLEDNODE)
		{
			if (actionNodeDirection == 0)
			{
				GameManager.AudioSlinger.PlaySound(myDoSAttack.NodeCold);
			}
			GameManager.AudioSlinger.PlaySound(myDoSAttack.NodeHot);
		}
		GameManager.AudioSlinger.PlaySound(myDoSAttack.NodeHot);
	}

	public void untap()
	{
		nodeHotImage.color = new Color(1f, 1f, 1f, 0f);
	}

	public void stopSubAction()
	{
		if (myNodeData.myType == DOS_NODE_TYPE.STARTNODE)
		{
			aniStopNodeArrow();
		}
	}

	public void endNodeNowActive()
	{
		if (myNodeData.myType == DOS_NODE_TYPE.ENDNODE)
		{
			nodeImage.sprite = myDoSAttack.endNodeActivatedSprite;
		}
	}

	private void aniStartNodeArrow()
	{
		myAniSeq = DOTween.Sequence();
		myAniSeq.Insert(0f, DOTween.To(() => startNodeArr.transform.localPosition, delegate(Vector3 x)
		{
			startNodeArr.transform.localPosition = x;
		}, new Vector3(startNodeArr.transform.localPosition.x, (float)myDoSAttack.nodeHeight / 2f + startNodeArr.rectTransform.sizeDelta.y / 2f, 0f), 0.6f).SetEase(Ease.OutSine));
		myAniSeq.Insert(0.6f, DOTween.To(() => startNodeArr.transform.localPosition, delegate(Vector3 x)
		{
			startNodeArr.transform.localPosition = x;
		}, new Vector3(startNodeArr.transform.localPosition.x, myDoSAttack.nodeHeight, 0f), 0.6f).SetEase(Ease.OutSine));
		myAniSeq.SetLoops(-1);
		myAniSeq.Play();
	}

	private void aniStopNodeArrow()
	{
		myAniSeq.Kill();
		Object.Destroy(startNodeArr);
	}
}
