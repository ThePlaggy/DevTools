using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CloseWindowBehaviour : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public Image parentWindow;

	public Sprite hoverSprite;

	public SOFTWARE_PRODUCTS MyProduct;

	public SoftwareProductDefinition MyProductData;

	public bool IgnoreProduct;

	private Sprite defaultSprite;

	public void Start()
	{
		defaultSprite = GetComponent<Image>().sprite;
		if (Themes.selected <= THEME.TWR)
		{
			GetComponent<Image>().sprite = ThemesLookUp.WTTG1TWR.closeBTNinactive;
			defaultSprite = ThemesLookUp.WTTG1TWR.closeBTNinactive;
			hoverSprite = ThemesLookUp.WTTG1TWR.closeBTNactive;
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (MyProduct == SOFTWARE_PRODUCTS.TEXT_DOC)
		{
			GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.MouseClick);
		}
		if (!IgnoreProduct)
		{
			if (MyProduct == SOFTWARE_PRODUCTS.BOTNET && KAttack.IsInAttack)
			{
				return;
			}
			if (MyProduct == SOFTWARE_PRODUCTS.VWIPE)
			{
				UIDialogManager.VWipeDialog.Close();
			}
			else if (MyProduct == SOFTWARE_PRODUCTS.UNIVERSAL)
			{
				GameManager.ManagerSlinger.AppManager.CloseApp(MyProductData);
			}
			else
			{
				GameManager.ManagerSlinger.AppManager.CloseApp(MyProduct);
			}
		}
		GetComponent<Image>().sprite = defaultSprite;
		if (MyProduct != SOFTWARE_PRODUCTS.VWIPE)
		{
			parentWindow.gameObject.SetActive(value: false);
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
