using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenuHook : MonoBehaviour
{
	public static PauseMenuHook Ins;

	[SerializeField]
	private Slider mouseSensSlider;

	[SerializeField]
	private TextMeshProUGUI mouseSensValue;

	[SerializeField]
	private TitleMenuBTN quitGameBTN;

	[SerializeField]
	private TitleMenuBTN exitToMainMenuBTN;

	private CanvasGroup myCG;

	private Tweener pauseTween;

	private Tweener unPauseTween;

	public CustomEvent<int> UpdatedMouseSens = new CustomEvent<int>(5);

	private void Awake()
	{
		Ins = this;
		myCG = GetComponent<CanvasGroup>();
		pauseTween = DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 1f, 0.2f).SetEase(Ease.Linear).SetUpdate(isIndependentUpdate: true);
		pauseTween.Pause();
		pauseTween.SetAutoKill(autoKillOnCompletion: false);
		unPauseTween = DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 0f, 0.2f).SetEase(Ease.Linear).SetUpdate(isIndependentUpdate: true);
		unPauseTween.Pause();
		unPauseTween.SetAutoKill(autoKillOnCompletion: false);
		mouseSensSlider.onValueChanged.AddListener(playerChangedMouseSens);
		int num = PlayerPrefs.GetInt("[GAME]MouseSens");
		mouseSensSlider.value = num;
		mouseSensValue.SetText(num.ToString());
		GameManager.PauseManager.GamePaused += playerHitPause;
		GameManager.PauseManager.GameUnPaused += playerHitUnPause;
		quitGameBTN.MyAction.Event += quitGame;
		exitToMainMenuBTN.MyAction.Event += exitToMainMenu;
	}

	private void OnDestroy()
	{
		Ins = null;
		mouseSensSlider.onValueChanged.RemoveListener(playerChangedMouseSens);
		GameManager.PauseManager.GamePaused -= playerHitPause;
		GameManager.PauseManager.GameUnPaused -= playerHitUnPause;
		quitGameBTN.MyAction.Event -= quitGame;
		exitToMainMenuBTN.MyAction.Event -= exitToMainMenu;
	}

	private void playerHitPause()
	{
		pauseTween.Restart();
		myCG.blocksRaycasts = true;
		myCG.interactable = true;
	}

	private void playerHitUnPause()
	{
		unPauseTween.Restart();
		myCG.blocksRaycasts = false;
		myCG.interactable = false;
	}

	private void playerChangedMouseSens(float SetValue)
	{
		int num = Mathf.RoundToInt(SetValue);
		mouseSensSlider.value = num;
		mouseSensValue.SetText(num.ToString());
		UpdatedMouseSens.Execute(num);
		PlayerPrefs.SetInt("[GAME]MouseSens", num);
	}

	private void quitGame()
	{
		if (TimeKeeper.NightmareEndingTriggered)
		{
			PostCreditSceneDONOTWATCH();
		}
		else
		{
			Application.Quit();
		}
	}

	private void exitToMainMenu()
	{
		if (TimeKeeper.NightmareEndingTriggered)
		{
			PostCreditSceneDONOTWATCH();
			return;
		}
		Time.timeScale = 1f;
		SceneManager.LoadScene(1);
	}

	public static void PostCreditSceneDONOTWATCH()
	{
		Time.timeScale = 1f;
		PostCreditsManager.megasex = true;
		SceneManager.LoadScene(7);
		SceneManager.LoadSceneAsync("postcredits", LoadSceneMode.Additive);
	}
}
