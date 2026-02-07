using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BookmarkTABObject : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	protected static Vector2 StartPOS = new Vector2(0f, 10f);

	protected static Vector2 StartSize = new Vector2(237f, 1f);

	protected static Vector2 EndSize = new Vector2(237f, 28f);

	public Text pageTitleText;

	public Color hoverColor;

	public Color defaultColor;

	private Vector2 myCurrentPOS = new Vector2(0f, 0f);

	private string myPageTitle;

	private string myPageURL;

	public void OnPointerDown(PointerEventData eventData)
	{
		GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: true);
		GameManager.BehaviourManager.AnnBehaviour.ForceLoadURL(myPageURL);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: true);
		GetComponent<Image>().color = hoverColor;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: false);
		GetComponent<Image>().color = defaultColor;
	}

	public void SoftBuild()
	{
		GetComponent<RectTransform>().anchoredPosition = StartPOS;
	}

	public void Build(BookmarkData SetBookMarkData, float SetY)
	{
		pageTitleText.text = SetBookMarkData.MyTitle;
		myPageTitle = SetBookMarkData.MyTitle;
		myPageURL = SetBookMarkData.MyURL;
		myCurrentPOS.y = SetY;
		GetComponent<RectTransform>().anchoredPosition = myCurrentPOS;
		DOTween.To(() => GetComponent<RectTransform>().sizeDelta, delegate(Vector2 x)
		{
			GetComponent<RectTransform>().sizeDelta = x;
		}, EndSize, 0.2f).SetEase(Ease.OutQuad);
	}

	public void KillMe()
	{
		DOTween.To(() => GetComponent<RectTransform>().sizeDelta, delegate(Vector2 x)
		{
			GetComponent<RectTransform>().sizeDelta = x;
		}, StartSize, 0.2f).SetEase(Ease.OutQuad).OnComplete(delegate
		{
			GetComponent<RectTransform>().anchoredPosition = StartPOS;
		});
	}

	public void RePOSMe(float setY)
	{
		myCurrentPOS.y = setY;
		DOTween.To(() => GetComponent<RectTransform>().anchoredPosition, delegate(Vector2 x)
		{
			GetComponent<RectTransform>().anchoredPosition = x;
		}, myCurrentPOS, 0.2f).SetEase(Ease.Linear);
	}
}
