using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MinWindowBehaviour : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public Sprite hoverSprite;

	public SOFTWARE_PRODUCTS MyProduct;

	public SoftwareProductDefinition UniProductData;

	private Sprite defaultSprite;

	private void Awake()
	{
		defaultSprite = GetComponent<Image>().sprite;
		if (Themes.selected <= THEME.TWR)
		{
			GetComponent<Image>().sprite = ThemesLookUp.WTTG1TWR.minBTNinactive;
			defaultSprite = ThemesLookUp.WTTG1TWR.minBTNinactive;
			hoverSprite = ThemesLookUp.WTTG1TWR.minBTNactive;
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (MyProduct != SOFTWARE_PRODUCTS.ANN || !TutorialAnnHook.YAAIRunning)
		{
			if (MyProduct == SOFTWARE_PRODUCTS.UNIVERSAL)
			{
				GameManager.ManagerSlinger.AppManager.MinApp(UniProductData);
			}
			else
			{
				GameManager.ManagerSlinger.AppManager.MinApp(MyProduct);
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (hoverSprite != null)
		{
			GetComponent<Image>().sprite = hoverSprite;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		GetComponent<Image>().sprite = defaultSprite;
	}
}
