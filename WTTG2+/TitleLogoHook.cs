using DG.Tweening;
using UnityEngine;

public class TitleLogoHook : MonoBehaviour
{
	public static TitleLogoHook Ins;

	private CanvasGroup myCG;

	private void Awake()
	{
		Ins = this;
		myCG = GetComponent<CanvasGroup>();
		TitleManager.Ins.TitlePresent.Event += presentLogo;
		TitleManager.Ins.TitleDismissing.Event += dismissLogo;
		TitleManager.Ins.OptionsDismissed.Event += presentLogoQuick;
		TitleManager.Ins.OptionsPresent.Event += dismissLogoQuick;
	}

	private void OnDestroy()
	{
		TitleManager.Ins.TitlePresent.Event -= presentLogo;
		TitleManager.Ins.TitleDismissing.Event -= dismissLogo;
		TitleManager.Ins.OptionsDismissed.Event -= presentLogoQuick;
		TitleManager.Ins.OptionsPresent.Event -= dismissLogoQuick;
	}

	private void presentLogo()
	{
		DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 1f, 2.5f).SetEase(Ease.Linear).SetDelay(5.5f)
			.OnComplete(delegate
			{
				TitleManager.Ins.LogoWasPresented();
			});
	}

	private void dismissLogo()
	{
		DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 0f, 0.75f).SetEase(Ease.Linear).SetDelay(0.75f);
	}

	private void presentLogoQuick()
	{
		DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 1f, 0.25f).SetEase(Ease.Linear);
	}

	private void dismissLogoQuick()
	{
		DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 0f, 0.25f).SetEase(Ease.Linear);
	}
}
