using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnnBTNBehaviour : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public bool activeAtStart;

	public Sprite hoverSprite;

	public ANN_BTN_ACTIONS MyAction;

	private Sprite defaultSprite;

	private bool isActive;

	private bool isLocked;

	private CanvasGroup myCG;

	private void Start()
	{
		prepBTN();
		if (Themes.selected <= THEME.TWR)
		{
			switch (MyAction)
			{
			case ANN_BTN_ACTIONS.BACK:
				hoverSprite = ThemesLookUp.WTTG1TWR.AnnBackBTNActive;
				break;
			case ANN_BTN_ACTIONS.FORWARD:
				hoverSprite = ThemesLookUp.WTTG1TWR.AnnForwardBTNActive;
				break;
			case ANN_BTN_ACTIONS.REFRESH:
				hoverSprite = ThemesLookUp.WTTG1TWR.AnnRefreshBTNActive;
				break;
			case ANN_BTN_ACTIONS.BOOKMARKS:
				hoverSprite = ThemesLookUp.WTTG1TWR.AnnBookmarksBTNActive;
				break;
			case ANN_BTN_ACTIONS.HOME:
				hoverSprite = ThemesLookUp.WTTG1TWR.AnnHomeBTNActive;
				break;
			case ANN_BTN_ACTIONS.CODE:
				hoverSprite = ThemesLookUp.WTTG1TWR.AnnSourceBTNActive;
				break;
			}
		}
	}

	private void Update()
	{
		if (isActive && !isLocked)
		{
			myCG.alpha = 1f;
			return;
		}
		GetComponent<Image>().sprite = defaultSprite;
		myCG.alpha = 0.25f;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!isLocked && isActive)
		{
			GameManager.BehaviourManager.AnnBehaviour.AnnBTNAction(MyAction);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!isLocked && isActive && hoverSprite != null)
		{
			GetComponent<Image>().sprite = hoverSprite;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!isLocked && isActive)
		{
			GetComponent<Image>().sprite = defaultSprite;
		}
	}

	public void setActive(bool setValue)
	{
		isActive = setValue;
	}

	public void setLock(bool setValue)
	{
		isLocked = setValue;
	}

	public bool GetActiveState()
	{
		return isActive;
	}

	private void prepBTN()
	{
		isActive = activeAtStart;
		defaultSprite = GetComponent<Image>().sprite;
		myCG = base.gameObject.AddComponent<CanvasGroup>();
		myCG.interactable = true;
		myCG.blocksRaycasts = true;
	}
}
