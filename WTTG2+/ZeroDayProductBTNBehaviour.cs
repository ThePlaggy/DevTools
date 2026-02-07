using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ZeroDayProductBTNBehaviour : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public delegate bool BoolActions();

	public delegate void VoidActions();

	public Text btnText;

	public Image InstallingIMG;

	public Sprite DefaultSprite;

	public Sprite HoverSprite;

	public Sprite DisabledSprite;

	private Vector2 installIMGSize = Vector2.zero;

	private bool isDisabled;

	public event BoolActions BuyItem;

	public event VoidActions InstallItem;

	public event VoidActions CantBuy;

	private void Start()
	{
		if (Themes.selected <= THEME.TWR)
		{
			DefaultSprite = ThemesLookUp.WTTG1TWR.blueBG;
			if (!isDisabled)
			{
				GetComponent<Image>().sprite = DefaultSprite;
			}
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (GameManager.PauseManager.Paused || isDisabled)
		{
			return;
		}
		if (this.BuyItem != null)
		{
			if (this.BuyItem())
			{
				if (this.InstallItem != null)
				{
					this.InstallItem();
				}
			}
			else
			{
				if (this.CantBuy != null)
				{
					this.CantBuy();
				}
				SetToDisabled();
				GameManager.TimeSlinger.FireTimer(3f, SetToBuy);
			}
		}
		GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: false);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!isDisabled)
		{
			GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: true);
			GetComponent<Image>().sprite = HoverSprite;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!isDisabled)
		{
			GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: false);
			GetComponent<Image>().sprite = DefaultSprite;
		}
	}

	public void SetToBuy()
	{
		isDisabled = false;
		GetComponent<Image>().sprite = DefaultSprite;
		btnText.text = "BUY";
		btnText.fontStyle = FontStyle.Normal;
		btnText.fontSize = 18;
	}

	public void SetToOwned()
	{
		isDisabled = true;
		GetComponent<Image>().sprite = DisabledSprite;
		btnText.text = "OWNED";
		btnText.fontStyle = FontStyle.Italic;
		btnText.fontSize = 18;
	}

	public void SetToDisabled()
	{
		isDisabled = true;
		GetComponent<Image>().sprite = DisabledSprite;
		btnText.fontStyle = FontStyle.Italic;
		btnText.fontSize = 18;
	}

	public void InstallAni(float setTime)
	{
		isDisabled = true;
		GetComponent<Image>().sprite = DisabledSprite;
		btnText.text = "Installing...";
		btnText.fontStyle = FontStyle.Italic;
		btnText.fontSize = 14;
		installIMGSize.x = GetComponent<RectTransform>().sizeDelta.x;
		installIMGSize.y = 0f;
		Sequence sequence = DOTween.Sequence().OnComplete(delegate
		{
			installIMGSize.x = 0f;
			installIMGSize.y = InstallingIMG.GetComponent<RectTransform>().sizeDelta.y;
			InstallingIMG.GetComponent<RectTransform>().sizeDelta = installIMGSize;
		});
		sequence.Insert(0f, DOTween.To(() => InstallingIMG.GetComponent<RectTransform>().sizeDelta, delegate(Vector2 x)
		{
			InstallingIMG.GetComponent<RectTransform>().sizeDelta = x;
		}, installIMGSize, setTime).SetEase(Ease.Linear).SetRelative(isRelative: true));
		sequence.Play();
	}
}
