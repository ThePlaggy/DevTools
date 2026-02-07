using UnityEngine;

public class OptionDataHook : MonoBehaviour
{
	public static OptionDataHook Ins;

	public Options Options { get; private set; }

	private void Awake()
	{
		Ins = this;
		Options = new Options(12);
		Options.ScreenWidth = Screen.width;
		Options.ScreenHeight = Screen.height;
		Options.QualitySettingIndex = QualitySettings.GetQualityLevel();
		Options.WindowMode = !Screen.fullScreen;
		Options.VSync = QualitySettings.vSyncCount > 0;
		Options.Mic = true;
		Options.Nudity = true;
		Options.MouseSens = 2;
	}

	private void OnDestroy()
	{
		Ins = null;
	}

	public void SaveOptionData()
	{
	}
}
