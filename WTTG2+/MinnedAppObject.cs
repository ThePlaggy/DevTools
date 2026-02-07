using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MinnedAppObject : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public static float MIN_APP_SPACING = 5f;

	public static float MIN_APP_WIDTH = 137f;

	public Image appIconIMG;

	public Image tabHoverIMG;

	public Text title1Text;

	public Text title2Text;

	private Sequence hoverSEQ;

	private string myKey;

	private SOFTWARE_PRODUCTS MyProduct;

	private SoftwareProductDefinition MyProductData;

	private Vector2 MyShowPOS = new Vector2(0f, -4f);

	private Vector2 MyStartPOS = new Vector2(0f, -50f);

	public void Start()
	{
		hoverSEQ = DOTween.Sequence();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		dismissMe();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		hoverSEQ.Kill();
		hoverSEQ.Insert(0f, DOTween.To(() => tabHoverIMG.GetComponent<CanvasGroup>().alpha, delegate(float x)
		{
			tabHoverIMG.GetComponent<CanvasGroup>().alpha = x;
		}, 1f, 0.2f).SetEase(Ease.Linear));
		hoverSEQ.Play();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		hoverSEQ.Kill();
		hoverSEQ.Insert(0f, DOTween.To(() => tabHoverIMG.GetComponent<CanvasGroup>().alpha, delegate(float x)
		{
			tabHoverIMG.GetComponent<CanvasGroup>().alpha = x;
		}, 0f, 0.2f).SetEase(Ease.Linear));
		hoverSEQ.Play();
	}

	public void SoftBuild()
	{
		GetComponent<RectTransform>().anchoredPosition = MyStartPOS;
	}

	public void BuildMe(SOFTWARE_PRODUCTS SetMyProduct, int MyCount)
	{
		MyProduct = SetMyProduct;
		title1Text.text = LookUp.SoftwareProducts.Get(SetMyProduct).MinProductTitle;
		title2Text.text = LookUp.SoftwareProducts.Get(SetMyProduct).MinProductTitle;
		appIconIMG.sprite = LookUp.SoftwareProducts.Get(SetMyProduct).MinProductSprite;
		applytheme();
		float x = ((MyCount == 0) ? MIN_APP_SPACING : ((float)MyCount * MIN_APP_WIDTH + (float)MyCount * MIN_APP_SPACING + MIN_APP_SPACING));
		MyStartPOS.x = x;
		MyShowPOS.x = x;
		GetComponent<RectTransform>().anchoredPosition = MyStartPOS;
		DOTween.To(() => GetComponent<RectTransform>().anchoredPosition, delegate(Vector2 anchoredPosition)
		{
			GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
		}, MyShowPOS, 0.2f).SetEase(Ease.Linear);
	}

	public void BuildMe(SoftwareProductDefinition SetMyProduct, int MyCount)
	{
		MyProductData = SetMyProduct;
		title1Text.text = MyProductData.MinProductTitle;
		title2Text.text = MyProductData.MinProductTitle;
		appIconIMG.sprite = MyProductData.MinProductSprite;
		applytheme();
		float x = ((MyCount == 0) ? MIN_APP_SPACING : ((float)MyCount * MIN_APP_WIDTH + (float)MyCount * MIN_APP_SPACING + MIN_APP_SPACING));
		MyStartPOS.x = x;
		MyShowPOS.x = x;
		GetComponent<RectTransform>().anchoredPosition = MyStartPOS;
		DOTween.To(() => GetComponent<RectTransform>().anchoredPosition, delegate(Vector2 anchoredPosition)
		{
			GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
		}, MyShowPOS, 0.2f).SetEase(Ease.Linear);
	}

	public void RePOSMe(int setIndex)
	{
		float x = ((setIndex == 0) ? MIN_APP_SPACING : ((float)setIndex * MIN_APP_WIDTH + (float)setIndex * MIN_APP_SPACING + MIN_APP_SPACING));
		MyShowPOS.x = x;
		MyStartPOS.x = x;
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => GetComponent<RectTransform>().anchoredPosition, delegate(Vector2 anchoredPosition)
		{
			GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
		}, MyShowPOS, 0.1f).SetEase(Ease.Linear));
		sequence.Play();
	}

	public void ForceDismissMe()
	{
		dismissMe();
	}

	private void dismissMe()
	{
		hoverSEQ.Kill();
		Sequence sequence = DOTween.Sequence().OnComplete(unMinApp);
		sequence.Insert(0f, DOTween.To(() => tabHoverIMG.GetComponent<CanvasGroup>().alpha, delegate(float x)
		{
			tabHoverIMG.GetComponent<CanvasGroup>().alpha = x;
		}, 0f, 0.2f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => GetComponent<RectTransform>().anchoredPosition, delegate(Vector2 x)
		{
			GetComponent<RectTransform>().anchoredPosition = x;
		}, MyStartPOS, 0.2f).SetEase(Ease.Linear));
		sequence.Play();
	}

	private void unMinApp()
	{
		if (MyProductData != null)
		{
			GameManager.ManagerSlinger.AppManager.UnMinApp(MyProductData);
		}
		else
		{
			GameManager.ManagerSlinger.AppManager.UnMinApp(MyProduct);
		}
	}

	private int applytheme()
	{
		if (Themes.selected <= THEME.TWR)
		{
			SOFTWARE_PRODUCTS myProduct = MyProduct;
			SOFTWARE_PRODUCTS sOFTWARE_PRODUCTS = myProduct;
			if (sOFTWARE_PRODUCTS == SOFTWARE_PRODUCTS.SKYBREAK)
			{
				appIconIMG.sprite = ThemesLookUp.WTTG1TWR.terminal;
			}
			if (!DifficultyManager.Nightmare)
			{
				tabHoverIMG.sprite = ThemesLookUp.WTTG1TWR.MinAppTabHover;
			}
			return 1;
		}
		if (Themes.selected == THEME.WTTG2BETA)
		{
			switch (MyProduct)
			{
			case SOFTWARE_PRODUCTS.SKYBREAK:
				appIconIMG.sprite = ThemesLookUp.WTTG2Beta.skybreak;
				break;
			case SOFTWARE_PRODUCTS.ZERODAY:
				appIconIMG.sprite = ThemesLookUp.WTTG2Beta.zeroDay;
				break;
			case SOFTWARE_PRODUCTS.SHADOW_MARKET:
				appIconIMG.sprite = ThemesLookUp.WTTG2Beta.shadowMarket;
				break;
			}
			return 1337;
		}
		return -69420;
	}
}
