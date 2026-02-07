using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DLLTitleManager : MonoBehaviour
{
	private GameObject MainCanvas;

	public static int ChosenBoy;

	private void Awake()
	{
		Object.Destroy(GameObject.Find("HelpMeVideoPlayer"));
		MainCanvas = GameObject.Find("MainCanvas");
		if (AssetsManager.AssetsReady)
		{
			InstantiateBoys();
			OnAssetsCached();
		}
		else
		{
			AssetBundleManager.AssetsCached += OnAssetsCached;
			TitleMessageHook.Ins.Init();
		}
	}

	private void OnDestroy()
	{
		AssetBundleManager.AssetsCached -= OnAssetsCached;
	}

	public static void InstantiateBoys()
	{
		if (ChosenBoy != 5)
		{
			GameObject.Find("CultMaleTitle").SetActive(value: false);
			GameObject.Find("CultFemaleTitle").SetActive(value: false);
		}
		switch (ChosenBoy)
		{
		case 0:
			Object.Instantiate(CustomObjectLookUp.TheBombMaker).AddComponent<BombMakerTitleBehaviour>();
			break;
		case 1:
			Object.Instantiate(CustomObjectLookUp.TheTanner).AddComponent<TannerTitleBehaviour>();
			break;
		case 2:
		{
			GameObject gameObject = Object.Instantiate(CustomObjectLookUp.Delfalco);
			Object.Destroy(gameObject.GetComponent<DelfalcoBehaviour>());
			gameObject.AddComponent<TitleDelfalcoBehaviour>();
			break;
		}
		case 3:
			Object.Instantiate(CustomObjectLookUp.ExecutionerCustomRig).AddComponent<EXETitleBehaviour>();
			break;
		case 4:
			Object.Instantiate(CustomObjectLookUp.KidnapperTitle);
			break;
		}
	}

	private void BuildMenu()
	{
		GameObject.Find("PPVol").GetComponent<PostProcessVolume>().sharedProfile.GetSetting<DepthOfField>().focusDistance.value = 0.7f;
	}

	private void TriggerAssetsNotify()
	{
		GameObject notifyCard = Object.Instantiate(CustomObjectLookUp.NotifyCard, MainCanvas.transform);
		notifyCard.transform.DOMoveX(150f, 0.75f).OnComplete(delegate
		{
			notifyCard.transform.DOMoveX(-125f, 0.5f).SetDelay(2f);
		});
		GameObject.Find("NotifyText").GetComponent<TMP_Text>().font = LookUps.TitleFont;
		GameObject.Find("NotifyText").GetComponent<TMP_Text>().text = "Assets loaded successfully!";
	}

	private void OnAssetsCached()
	{
		AssetBundleManager.AssetsCached -= OnAssetsCached;
		TriggerAssetsNotify();
		BuildMenu();
		TwitchManager.Ins.InitTitleSettings();
		Object.Destroy(GameObject.Find("ApplyButton"));
		TimeSlinger.FireTimer(TitleManager.Ins.presentTitle, 0.5f);
		Debug.Log("[DLL Title Manager] Trying to spawn new menu canvas");
		TitleManager.Ins.newCanvas = Object.Instantiate(CustomObjectLookUp.newMenuCanvas);
	}
}
