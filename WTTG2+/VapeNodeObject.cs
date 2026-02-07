using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VapeNodeObject : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public Image nodeImage;

	public CanvasGroup nodeHoverCanvas;

	public Image nodeHoverImage;

	public CanvasGroup nodeActiveCanvas;

	public Image nodeActiveImage;

	public Sprite blankNodeSprite;

	public Sprite boxNodeSprite;

	public Sprite goodNodeSprite;

	public Sprite deadNodeSprite;

	public Sprite boxNodeHoverSprite;

	public Sprite boxNodeActiveSprite;

	public Sprite blankNodeHoverSprite;

	public Sprite goodNodeActiveSprite;

	private Sequence actionSeq;

	private Sequence goodSeq;

	private vapeAttack myVapeAttack;

	public vapeAttack.vapeNode myVapeNodeData;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!myVapeAttack.vapeAttackFired)
		{
			return;
		}
		actionSeq.Kill();
		actionSeq = DOTween.Sequence();
		if (myVapeNodeData.myType == vapeNodeType.BOXNODE)
		{
			if (!myVapeAttack.curActiveNodeSet)
			{
				GameManager.AudioSlinger.PlaySound(myVapeAttack.boxNodeActiveClip);
				actionSeq.Insert(0f, DOTween.To(() => nodeHoverCanvas.alpha, delegate(float x)
				{
					nodeHoverCanvas.alpha = x;
				}, 0f, 0.25f).SetEase(Ease.Linear));
				actionSeq.Insert(0f, DOTween.To(() => nodeActiveCanvas.alpha, delegate(float x)
				{
					nodeActiveCanvas.alpha = x;
				}, 1f, 0.25f).SetEase(Ease.Linear));
			}
		}
		else if (myVapeNodeData.myType == vapeNodeType.BLANKNODE && myVapeAttack.curActiveNodeSet)
		{
			actionSeq.Insert(0f, DOTween.To(() => nodeHoverCanvas.alpha, delegate(float x)
			{
				nodeHoverCanvas.alpha = x;
			}, 0f, 0.15f).SetEase(Ease.Linear));
		}
		actionSeq.Play();
		myVapeAttack.vapeNodeAction(myVapeNodeData);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!myVapeAttack.vapeAttackFired)
		{
			return;
		}
		actionSeq = DOTween.Sequence();
		if (myVapeNodeData.myType == vapeNodeType.BOXNODE)
		{
			if (!myVapeAttack.curActiveNodeSet)
			{
				GameManager.AudioSlinger.PlaySound(myVapeAttack.boxNodeHoverClip);
			}
			actionSeq.Insert(0f, DOTween.To(() => nodeHoverCanvas.alpha, delegate(float x)
			{
				nodeHoverCanvas.alpha = x;
			}, 1f, 0.25f).SetEase(Ease.Linear));
		}
		else if (myVapeNodeData.myType == vapeNodeType.BLANKNODE && myVapeAttack.curActiveNodeSet)
		{
			GameManager.AudioSlinger.PlaySound(myVapeAttack.blankNodeHoverClip);
			actionSeq.Insert(0f, DOTween.To(() => nodeHoverCanvas.alpha, delegate(float x)
			{
				nodeHoverCanvas.alpha = x;
			}, 1f, 0.15f).SetEase(Ease.Linear));
		}
		actionSeq.Play();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!myVapeAttack.vapeAttackFired)
		{
			return;
		}
		actionSeq.Kill();
		actionSeq = DOTween.Sequence();
		if (myVapeNodeData.myType == vapeNodeType.BOXNODE)
		{
			actionSeq.Insert(0f, DOTween.To(() => nodeHoverCanvas.alpha, delegate(float x)
			{
				nodeHoverCanvas.alpha = x;
			}, 0f, 0.25f).SetEase(Ease.Linear));
		}
		else if (myVapeNodeData.myType == vapeNodeType.BLANKNODE)
		{
			actionSeq.Insert(0f, DOTween.To(() => nodeHoverCanvas.alpha, delegate(float x)
			{
				nodeHoverCanvas.alpha = x;
			}, 0f, 0.15f).SetEase(Ease.Linear));
		}
		actionSeq.Play();
	}

	public void buildMe(vapeAttack setVapeAttack, vapeAttack.vapeNode setNodeData = null, float setX = 0f, float setY = 0f, float setWidth = 100f, float setHeight = 100f)
	{
		myVapeAttack = setVapeAttack;
		myVapeNodeData = setNodeData;
		nodeImage.sprite = myVapeNodeData.mySprite;
		base.gameObject.AddComponent<GraphicsRayCasterCatcher>();
		GetComponent<RectTransform>().sizeDelta = new Vector2(setWidth, setHeight);
		GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1f);
		GetComponent<RectTransform>().anchorMax = new Vector2(0f, 1f);
		GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		base.transform.localPosition = new Vector3(setX, setY, 0f);
		nodeImage.GetComponent<RectTransform>().sizeDelta = new Vector2(setWidth, setHeight);
		nodeHoverImage.GetComponent<RectTransform>().sizeDelta = new Vector2(setWidth, setHeight);
		nodeHoverCanvas.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(setWidth, setHeight);
		nodeActiveImage.GetComponent<RectTransform>().sizeDelta = new Vector2(setWidth, setHeight);
		nodeActiveCanvas.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(setWidth, setHeight);
		if (setNodeData.myType == vapeNodeType.BLANKNODE)
		{
			nodeHoverImage.sprite = blankNodeHoverSprite;
		}
		else if (setNodeData.myType == vapeNodeType.BOXNODE)
		{
			nodeHoverImage.sprite = boxNodeHoverSprite;
			nodeActiveImage.sprite = boxNodeActiveSprite;
		}
		else if (setNodeData.myType != vapeNodeType.GOODNODE)
		{
			vapeNodeType myType = setNodeData.myType;
		}
	}

	public void updateMyType(vapeNodeType setType)
	{
		switch (setType)
		{
		case vapeNodeType.BLANKNODE:
			myVapeNodeData.updateMyInfo(setType, blankNodeSprite);
			nodeImage.sprite = blankNodeSprite;
			nodeHoverImage.sprite = blankNodeHoverSprite;
			break;
		case vapeNodeType.BOXNODE:
			myVapeNodeData.updateMyInfo(setType, boxNodeSprite);
			nodeImage.sprite = boxNodeSprite;
			nodeHoverImage.sprite = boxNodeHoverSprite;
			nodeActiveImage.sprite = boxNodeActiveSprite;
			break;
		case vapeNodeType.GOODNODE:
			myVapeNodeData.updateMyInfo(setType, goodNodeSprite);
			nodeImage.sprite = goodNodeSprite;
			break;
		case vapeNodeType.DEADNODE:
			myVapeNodeData.updateMyInfo(setType, deadNodeSprite);
			nodeImage.sprite = deadNodeSprite;
			break;
		}
	}

	public void clearActiveState()
	{
		actionSeq.Kill();
		nodeHoverCanvas.alpha = 0f;
		nodeActiveCanvas.alpha = 0f;
	}

	public void setAsGoodNode()
	{
		GameManager.AudioSlinger.PlaySound(myVapeAttack.goodNodeActiveClip);
		nodeActiveImage.sprite = goodNodeActiveSprite;
		nodeActiveCanvas.alpha = 1f;
		goodSeq = DOTween.Sequence();
		goodSeq.Insert(0f, DOTween.To(() => nodeActiveCanvas.alpha, delegate(float x)
		{
			nodeActiveCanvas.alpha = x;
		}, 0f, 0.25f).SetEase(Ease.Linear));
		goodSeq.Play();
		updateMyType(vapeNodeType.GOODNODE);
	}
}
