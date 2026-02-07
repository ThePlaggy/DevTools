using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleMessageHook : MonoBehaviour
{
	public TMP_Text loadingText;

	public CanvasGroup loadingTextCG;

	private Tweener loadingAni;

	private CanvasGroup loadingCG;

	private GameObject loadingBG;

	private static TitleMessageHook s_instance;

	public static TitleMessageHook Ins
	{
		get
		{
			if (s_instance == null)
			{
				s_instance = new GameObject("LoadingAssetsHook").AddComponent<TitleMessageHook>();
			}
			return s_instance;
		}
	}

	public void BuildMessage()
	{
		LookUps.TitleFont = GameObject.Find("TitleText").GetComponent<TextMeshProUGUI>().font;
		loadingBG = new GameObject("LoadingAssetsBG");
		loadingBG.transform.SetParent(GameObject.Find("MainCanvas").transform);
		loadingBG.AddComponent<Image>().color = new Color(0f, 0f, 0f, 255f);
		loadingCG = loadingBG.AddComponent<CanvasGroup>();
		RectTransform component = loadingBG.GetComponent<RectTransform>();
		component.transform.localPosition = Vector2.zero;
		component.anchorMin = new Vector2(0f, 0f);
		component.anchorMax = new Vector2(1f, 1f);
		component.pivot = new Vector2(0.5f, 0.5f);
		GameObject gameObject = new GameObject("LoadingAssetsText");
		gameObject.transform.SetParent(GameObject.Find("MainCanvas").transform);
		loadingTextCG = gameObject.AddComponent<CanvasGroup>();
		loadingTextCG.alpha = 0f;
		loadingTextCG.interactable = false;
		loadingTextCG.blocksRaycasts = false;
		loadingTextCG.ignoreParentGroups = false;
		loadingText = gameObject.AddComponent<TextMeshProUGUI>();
		loadingText.text = "";
		loadingText.fontSize = 30f;
		loadingText.font = LookUps.TitleFont;
		loadingText.characterSpacing = 30f;
		loadingText.alignment = TextAlignmentOptions.Center;
		RectTransform component2 = gameObject.GetComponent<RectTransform>();
		component2.transform.localPosition = Vector2.zero;
		component2.anchorMin = new Vector2(0f, 0.5f);
		component2.anchorMax = new Vector2(1f, 0.5f);
		component2.pivot = new Vector2(0.5f, 0.5f);
	}

	private void Handle_AssetsCached()
	{
		AssetBundleManager.AssetsCached -= Handle_AssetsCached;
		TimeSlinger.FireTimer(delegate
		{
			loadingAni.Kill();
			loadingTextCG.alpha = 1f;
			DOTween.To(() => loadingText.alpha, delegate(float x)
			{
				loadingText.alpha = x;
			}, 0f, 2f).SetEase(Ease.Linear);
			DOTween.To(() => loadingCG.alpha, delegate(float x)
			{
				loadingCG.alpha = x;
			}, 0f, 2f).SetEase(Ease.Linear).OnComplete(delegate
			{
				loadingBG.SetActive(value: false);
			});
			DLLTitleManager.InstantiateBoys();
		}, 0.5f);
	}

	private void Handle_AssetsFailed(string reason)
	{
		AssetsManager.AssetsFailed = (Action<string>)Delegate.Remove(AssetsManager.AssetsFailed, new Action<string>(Handle_AssetsFailed));
		loadingAni.Kill();
		loadingTextCG.alpha = 1f;
		loadingText.text = reason + "\nPlease Restart Your Game";
		DOTween.To(() => loadingText.characterSpacing, delegate(float x)
		{
			loadingText.characterSpacing = x;
		}, 5f, 1f).SetEase(Ease.Linear);
	}

	public void Init()
	{
		BuildMessage();
		AssetsManager.AssetsFailed = (Action<string>)Delegate.Combine(AssetsManager.AssetsFailed, new Action<string>(Handle_AssetsFailed));
		AssetBundleManager.AssetsCached += Handle_AssetsCached;
		AssetsManager.Ins.LoadAssets();
		if (!Bootstrapper.VersionIsExpired)
		{
			loadingText.text = "Loading WTTG2+ Assets";
			loadingAni = DOTween.To(() => loadingTextCG.alpha, delegate(float x)
			{
				loadingTextCG.alpha = x;
			}, 1f, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
			DOTween.To(() => loadingText.characterSpacing, delegate(float x)
			{
				loadingText.characterSpacing = x;
			}, 15f, 1f).SetEase(Ease.Linear);
		}
	}

	public void Kill()
	{
		s_instance = null;
		AssetsManager.AssetsFailed = (Action<string>)Delegate.Remove(AssetsManager.AssetsFailed, new Action<string>(Handle_AssetsFailed));
		AssetBundleManager.AssetsCached -= Handle_AssetsCached;
	}
}
