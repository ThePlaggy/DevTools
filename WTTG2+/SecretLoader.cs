using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SecretLoader : MonoBehaviour
{
	[SerializeField]
	private CanvasGroup loadingScreenCG;

	[SerializeField]
	private CanvasGroup skullCanvasGroup;

	[SerializeField]
	private int secretWorldID;

	[SerializeField]
	private bool debugMode;

	private Tweener skullTween;

	private void Awake()
	{
		skullTween = DOTween.To(() => skullCanvasGroup.alpha, delegate(float x)
		{
			skullCanvasGroup.alpha = x;
		}, 1f, 0.75f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
		skullTween.SetAutoKill(autoKillOnCompletion: false);
		skullTween.Pause();
	}

	private void Start()
	{
		if (!debugMode)
		{
			skullTween.Restart();
			GameManager.TimeSlinger.FireTimer(4f, delegate
			{
				StartCoroutine(loadWorld(secretWorldID));
			});
		}
		else
		{
			loadingScreenCG.alpha = 0f;
		}
	}

	private IEnumerator loadWorld(int worldID)
	{
		AsyncOperation result = SceneManager.LoadSceneAsync(worldID, LoadSceneMode.Additive);
		while (!result.isDone)
		{
			yield return new WaitForEndOfFrame();
		}
		ClearLoading();
	}

	private void ClearLoading()
	{
		skullTween.Kill();
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => skullCanvasGroup.alpha, delegate(float x)
		{
			skullCanvasGroup.alpha = x;
		}, 0f, 0.5f).SetEase(Ease.Linear));
		sequence.Insert(0.5f, DOTween.To(() => loadingScreenCG.alpha, delegate(float x)
		{
			loadingScreenCG.alpha = x;
		}, 0f, 1f).SetEase(Ease.Linear));
		sequence.Play();
	}
}
