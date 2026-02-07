using DG.Tweening;
using UnityEngine;

public class crossHairBehaviour : MonoBehaviour
{
	public Canvas CrossHairCanvas;

	public Transform CrossHairTrans;

	public CanvasGroup CrossHairCG;

	private Tweener activateCrossHairCG;

	private Tweener activateCrossHairTrans;

	private Tweener deActivateCrossHairCG;

	private Tweener deActivateCrossHairTrans;

	private bool hideCrossHairGroup;

	private void Awake()
	{
		GameManager.BehaviourManager.CrossHairBehaviour = this;
		GameManager.PauseManager.GamePaused += PlayerHitPause;
		GameManager.PauseManager.GameUnPaused += PlayerHitUnPause;
		activateCrossHairTrans = DOTween.To(() => new Vector3(0.5f, 0.5f, 1f), delegate(Vector3 x)
		{
			CrossHairTrans.localScale = x;
		}, Vector3.one, 0.15f).SetEase(Ease.Linear);
		activateCrossHairTrans.SetAutoKill(autoKillOnCompletion: false);
		activateCrossHairTrans.Pause();
		deActivateCrossHairTrans = DOTween.To(() => Vector3.one, delegate(Vector3 x)
		{
			CrossHairTrans.localScale = x;
		}, new Vector3(0.5f, 0.5f, 1f), 0.15f).SetEase(Ease.Linear);
		deActivateCrossHairTrans.SetAutoKill(autoKillOnCompletion: false);
		deActivateCrossHairTrans.Pause();
		activateCrossHairCG = DOTween.To(() => 0.25f, delegate(float x)
		{
			CrossHairCG.alpha = x;
		}, 0.9f, 0.15f).SetEase(Ease.Linear);
		activateCrossHairCG.SetAutoKill(autoKillOnCompletion: false);
		activateCrossHairCG.Pause();
		deActivateCrossHairCG = DOTween.To(() => 0.9f, delegate(float x)
		{
			CrossHairCG.alpha = x;
		}, 0.25f, 0.15f).SetEase(Ease.Linear);
		deActivateCrossHairCG.SetAutoKill(autoKillOnCompletion: false);
		deActivateCrossHairCG.Pause();
	}

	private void OnDestroy()
	{
		GameManager.PauseManager.GamePaused -= PlayerHitPause;
		GameManager.PauseManager.GameUnPaused -= PlayerHitUnPause;
	}

	public void ShowCrossHairGroup()
	{
		CrossHairCanvas.enabled = true;
		hideCrossHairGroup = false;
	}

	public void HideCrossHairGroup()
	{
		CrossHairCanvas.enabled = false;
		hideCrossHairGroup = true;
	}

	public void ShowActiveCrossHair()
	{
		deActivateCrossHairTrans.Pause();
		deActivateCrossHairCG.Pause();
		activateCrossHairTrans.Restart();
		activateCrossHairCG.Restart();
	}

	public void HideActiveCrossHair()
	{
		activateCrossHairTrans.Pause();
		activateCrossHairCG.Pause();
		deActivateCrossHairTrans.Restart();
		deActivateCrossHairCG.Restart();
	}

	private void PlayerHitPause()
	{
		if (!hideCrossHairGroup)
		{
			CrossHairCanvas.enabled = false;
		}
	}

	private void PlayerHitUnPause()
	{
		if (!hideCrossHairGroup)
		{
			CrossHairCanvas.enabled = true;
		}
	}
}
