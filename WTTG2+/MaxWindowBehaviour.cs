using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MaxWindowBehaviour : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public SOFTWARE_PRODUCTS MyProduct;

	public SoftwareProductDefinition MyProductData;

	public Sprite defaultSpriteUnMax;

	public Sprite hoverSpriteDefault;

	public Sprite hoverSpriteUnMax;

	private Sprite defaultSprite;

	private bool isMaxed;

	private void Awake()
	{
		defaultSprite = GetComponent<Image>().sprite;
		if (Themes.selected <= THEME.TWR)
		{
			GetComponent<Image>().sprite = ThemesLookUp.WTTG1TWR.maxBTNinactive;
			defaultSprite = ThemesLookUp.WTTG1TWR.maxBTNinactive;
			hoverSpriteDefault = ThemesLookUp.WTTG1TWR.maxBTNactive;
			defaultSpriteUnMax = ThemesLookUp.WTTG1TWR.unmaxBTNinactive;
			hoverSpriteUnMax = ThemesLookUp.WTTG1TWR.unmaxBTNactive;
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (MyProduct == SOFTWARE_PRODUCTS.ANN && TutorialAnnHook.YAAIRunning)
		{
			return;
		}
		if (isMaxed)
		{
			isMaxed = false;
			if (MyProduct == SOFTWARE_PRODUCTS.UNIVERSAL)
			{
				GameManager.ManagerSlinger.AppManager.UnMaxApp(MyProductData);
			}
			else
			{
				GameManager.ManagerSlinger.AppManager.UnMaxApp(MyProduct);
			}
		}
		else
		{
			isMaxed = true;
			if (MyProduct == SOFTWARE_PRODUCTS.UNIVERSAL)
			{
				GameManager.ManagerSlinger.AppManager.MaxApp(MyProductData);
			}
			else
			{
				GameManager.ManagerSlinger.AppManager.MaxApp(MyProduct);
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (isMaxed)
		{
			GetComponent<Image>().sprite = hoverSpriteUnMax;
		}
		else
		{
			GetComponent<Image>().sprite = hoverSpriteDefault;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (isMaxed)
		{
			GetComponent<Image>().sprite = defaultSpriteUnMax;
		}
		else
		{
			GetComponent<Image>().sprite = defaultSprite;
		}
	}

	public void HardMax()
	{
		isMaxed = true;
		GetComponent<Image>().sprite = defaultSpriteUnMax;
	}
}
