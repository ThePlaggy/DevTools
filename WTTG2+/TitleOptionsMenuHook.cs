using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleOptionsMenuHook : MonoBehaviour
{
	public static TitleOptionsMenuHook Ins;

	public static CustomEvent SettingsApplied = new CustomEvent();

	[SerializeField]
	private Slider qualitySlider;

	[SerializeField]
	private Slider resoultionSlider;

	[SerializeField]
	private OptionsMenuBTN vSyncOnBTN;

	[SerializeField]
	private OptionsMenuBTN vSyncOffBTN;

	[SerializeField]
	private OptionsMenuBTN windowModeOnBTN;

	[SerializeField]
	private OptionsMenuBTN windowModeOffBTN;

	[SerializeField]
	private OptionsMenuBTN micOnBTN;

	[SerializeField]
	private OptionsMenuBTN micOffBTN;

	[SerializeField]
	private OptionsMenuBTN nudityOnBTN;

	[SerializeField]
	private OptionsMenuBTN nudityOffBTN;

	[SerializeField]
	private TextMeshProUGUI qualityValue;

	[SerializeField]
	private TextMeshProUGUI resoultionValue;

	[SerializeField]
	private TitleMenuBTN applyBTN;

	[SerializeField]
	private TitleMenuBTN backBTN;

	private CanvasGroup myCG;

	private Options myOptionData;

	private string[] quailtySettingNames = new string[0];

	private List<int> screenSizeLookUp = new List<int>(10);

	private Dictionary<int, Resolution> screenSizes = new Dictionary<int, Resolution>(10);

	private void Awake()
	{
		Ins = this;
		myOptionData = new Options(12);
		myOptionData.ScreenWidth = Screen.width;
		myOptionData.ScreenHeight = Screen.height;
		myOptionData.QualitySettingIndex = QualitySettings.GetQualityLevel();
		myOptionData.WindowMode = !Screen.fullScreen;
		myOptionData.VSync = QualitySettings.vSyncCount > 0;
		myOptionData.Mic = true;
		myOptionData.Nudity = true;
		myOptionData.MouseSens = 2;
		myCG = GetComponent<CanvasGroup>();
		myCG.interactable = false;
		myCG.blocksRaycasts = false;
		Resolution[] resolutions = Screen.resolutions;
		for (int i = 0; i < resolutions.Length; i++)
		{
			Resolution value = resolutions[i];
			int num = value.width + value.height;
			if (value.height >= 720 && !screenSizes.ContainsKey(num))
			{
				screenSizes.Add(num, value);
				screenSizeLookUp.Add(num);
			}
		}
		int num2 = 0;
		int num3 = 0;
		foreach (KeyValuePair<int, Resolution> screenSize in screenSizes)
		{
			if (Screen.width == screenSize.Value.width && Screen.height == screenSize.Value.height)
			{
				num2 = num3;
			}
			num3++;
		}
		resoultionSlider.wholeNumbers = true;
		resoultionSlider.minValue = 0f;
		resoultionSlider.maxValue = screenSizeLookUp.Count - 1;
		resoultionSlider.onValueChanged.AddListener(resoultionChange);
		resoultionSlider.value = num2;
		quailtySettingNames = QualitySettings.names;
		qualitySlider.wholeNumbers = true;
		qualitySlider.minValue = 0f;
		qualitySlider.maxValue = quailtySettingNames.Length - 1;
		qualitySlider.onValueChanged.AddListener(qualityChange);
		qualitySlider.value = QualitySettings.GetQualityLevel();
		if (Screen.fullScreen)
		{
			windowModeOffBTN.SetActive();
		}
		else
		{
			windowModeOnBTN.SetActive();
		}
		if (QualitySettings.vSyncCount > 0)
		{
			vSyncOnBTN.SetActive();
		}
		else
		{
			vSyncOffBTN.SetActive();
		}
		if (myOptionData.Mic)
		{
			micOnBTN.SetActive();
		}
		else
		{
			micOffBTN.SetActive();
		}
		if (myOptionData.Nudity)
		{
			nudityOnBTN.SetActive();
		}
		else
		{
			nudityOffBTN.SetActive();
		}
		TitleManager.Ins.OptionsPresented.Event += presentMe;
		backBTN.MyAction.Event += applySettings;
		backBTN.MyAction.Event += dismissMe;
	}

	private void OnDestroy()
	{
		TitleManager.Ins.OptionsPresented.Event -= presentMe;
		backBTN.MyAction.Event -= applySettings;
		backBTN.MyAction.Event -= dismissMe;
	}

	private void presentMe()
	{
		DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 1f, 0.5f).SetEase(Ease.Linear).OnComplete(delegate
		{
			myCG.interactable = true;
			myCG.blocksRaycasts = true;
		});
	}

	private void dismissMe()
	{
		myCG.interactable = false;
		myCG.blocksRaycasts = false;
		DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 0f, 0.5f).SetEase(Ease.Linear).OnComplete(delegate
		{
			TitleManager.Ins.OptionsDismissing.Execute();
		});
	}

	private void resoultionChange(float value)
	{
		int index = Mathf.RoundToInt(value);
		int key = screenSizeLookUp[index];
		string text = screenSizes[key].width + "x" + screenSizes[key].height;
		resoultionValue.SetText(text);
	}

	private void qualityChange(float value)
	{
		int num = Mathf.RoundToInt(value);
		qualityValue.SetText(quailtySettingNames[num]);
	}

	private void applySettings()
	{
		int index = Mathf.RoundToInt(resoultionSlider.value);
		int key = screenSizeLookUp[index];
		bool fullscreen = !windowModeOnBTN.Active;
		int vSyncCount = (vSyncOnBTN.Active ? 1 : 0);
		int num = Mathf.RoundToInt(qualitySlider.value);
		myOptionData.ScreenWidth = screenSizes[key].width;
		myOptionData.ScreenHeight = screenSizes[key].height;
		myOptionData.QualitySettingIndex = num;
		myOptionData.WindowMode = windowModeOnBTN.Active;
		myOptionData.VSync = vSyncOnBTN.Active;
		myOptionData.Mic = micOnBTN.Active;
		myOptionData.Nudity = nudityOnBTN.Active;
		QualitySettings.SetQualityLevel(num, applyExpensiveChanges: true);
		if (num <= 1)
		{
			QualitySettings.shadows = ShadowQuality.Disable;
		}
		Screen.SetResolution(screenSizes[key].width, screenSizes[key].height, fullscreen);
		QualitySettings.vSyncCount = vSyncCount;
		SettingsApplied.Execute();
	}
}
