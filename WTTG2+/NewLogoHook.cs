using DG.Tweening;
using UnityEngine;

public class NewLogoHook : MonoBehaviour
{
	public static NewLogoHook Ins;

	private CanvasGroup myCG;

	private void Awake()
	{
		Ins = this;
		myCG = GetComponent<CanvasGroup>();
		myCG.alpha = 0f;
		myCG.interactable = false;
		myCG.blocksRaycasts = false;
	}

	public void DismissMe()
	{
		myCG.DOFade(0f, 0.25f).SetEase(Ease.Linear);
	}

	public void PresentMe()
	{
		myCG.DOFade(1f, 0.25f).SetEase(Ease.Linear);
	}
}
