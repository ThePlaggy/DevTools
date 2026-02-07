using DG.Tweening;
using UnityEngine;

public class CurrencyMenuTriggerBehaviour : MonoBehaviour
{
	private bool currencyMenuActive;

	private bool currencyMenuAniActive;

	public void TriggerCurrencyMenu()
	{
		if (currencyMenuAniActive)
		{
			return;
		}
		currencyMenuAniActive = true;
		if (currencyMenuActive)
		{
			currencyMenuActive = false;
			DOTween.To(endValue: new Vector2(LookUp.DesktopUI.CURRENCY_MENU.anchoredPosition.x, LookUp.DesktopUI.CURRENCY_MENU.sizeDelta.y), getter: () => LookUp.DesktopUI.CURRENCY_MENU.anchoredPosition, setter: delegate(Vector2 x)
			{
				LookUp.DesktopUI.CURRENCY_MENU.anchoredPosition = x;
			}, duration: 0.25f).SetEase(Ease.InQuad).OnComplete(delegate
			{
				currencyMenuAniActive = false;
			});
		}
		else
		{
			currencyMenuActive = true;
			DOTween.To(endValue: new Vector2(LookUp.DesktopUI.CURRENCY_MENU.anchoredPosition.x, -41f), getter: () => LookUp.DesktopUI.CURRENCY_MENU.anchoredPosition, setter: delegate(Vector2 x)
			{
				LookUp.DesktopUI.CURRENCY_MENU.anchoredPosition = x;
			}, duration: 0.25f).SetEase(Ease.OutQuad).OnComplete(delegate
			{
				currencyMenuAniActive = false;
			});
		}
	}
}
