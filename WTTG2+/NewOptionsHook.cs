using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewOptionsHook : MonoBehaviour
{
	public TMP_Dropdown resDropdown;

	public TMP_Dropdown qualityDropdown;

	public TMP_Dropdown vsyncDropdown;

	public TMP_Dropdown windowedDropdown;

	public TMP_Dropdown nudityDropdown;

	public TMP_Dropdown devToolsDropdown;

	public TMP_Dropdown dmcaDropdown;

	public TMP_Dropdown menuThemeDropdown;

	public TMP_Dropdown menuMusicDropdown;

	private bool fullscreen;

	private bool properCodeMusicUpdate;

	private bool themeWarmedUp;

	private List<TMP_Dropdown.OptionData> resolutions = new List<TMP_Dropdown.OptionData>();

	public TMP_Dropdown gameThemeDropdown;

	public TMP_Text volumeText;

	public Slider volumeSlider;

	private void Start()
	{
		RefreshResolutions();
		SetSettings();
		if (!PlayerPrefs.HasKey("[GAME]MouseSens"))
		{
			PlayerPrefs.SetInt("[GAME]MouseSens", 2);
		}
	}

	public void RefreshResolutions()
	{
		resolutions.Clear();
		resDropdown.ClearOptions();
		Resolution[] array = Screen.resolutions;
		for (int i = 0; i < array.Length; i++)
		{
			Resolution resolution = array[i];
			if (Mathf.Approximately((float)resolution.width / (float)resolution.height, 1.7777778f))
			{
				resolutions.Add(new TMP_Dropdown.OptionData($"{resolution.width}x{resolution.height} {resolution.refreshRate}hz"));
			}
		}
		resDropdown.AddOptions(resolutions);
		if (!PlayerPrefs.HasKey("[GAME]ResolutionIndex"))
		{
			int num = resolutions.Count - 1;
			resDropdown.value = num;
			string[] array2 = resolutions[num].text.Split('x');
			int value = int.Parse(array2[0]);
			int value2 = int.Parse(array2[1].Split(' ')[0]);
			PlayerPrefs.SetInt("[GAME]ResolutionX", value);
			PlayerPrefs.SetInt("[GAME]ResolutionY", value2);
			PlayerPrefs.SetInt("[GAME]ResolutionIndex", num);
		}
	}

	public void UpdateQuality(int num)
	{
		QualitySettings.SetQualityLevel(num, applyExpensiveChanges: true);
		PlayerPrefs.SetInt("[GAME]Quality", num);
	}

	public void UpdateResolution(int num)
	{
		string[] array = resolutions[num].text.Split('x');
		int num2 = int.Parse(array[0]);
		int num3 = int.Parse(array[1].Split(' ')[0]);
		Screen.SetResolution(num2, num3, fullscreen);
		PlayerPrefs.SetInt("[GAME]ResolutionX", num2);
		PlayerPrefs.SetInt("[GAME]ResolutionY", num3);
		PlayerPrefs.SetInt("[GAME]ResolutionIndex", num);
	}

	public void UpdateWindowed(int num)
	{
		fullscreen = num == 1;
		PlayerPrefs.SetInt("[GAME]Windowed", num);
		Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, fullscreen);
	}

	public void UpdateVsync(int num)
	{
		QualitySettings.vSyncCount = ((num != 1) ? 1 : 0);
		PlayerPrefs.SetInt("[GAME]VSync", num);
	}

	public void UpdateNudity(int num)
	{
		PlayerPrefs.SetInt("[GAME]Nudity", num);
	}

	public void UpdateDevtools(int num)
	{
	}

	public void UpdateDMCA(int num)
	{
		PlayerPrefs.SetInt("[MOD]TrolloPollo", (num != 1) ? 1 : 0);
	}

	public void UpdateMenuTheme(int num)
	{
		PlayerPrefs.SetInt("[MOD]MenuTheme", num);
		Debug.Log("[Options] Changing Menu Theme...");
		if (NewMenuManager.Ins.menuScenery != null)
		{
			Object.Destroy(NewMenuManager.Ins.menuScenery);
		}
		NewMenuManager.Ins.SpawnMenuScenery(num);
		if (PlayerPrefs.GetInt("[TITLE]TitleMusicID") == -2)
		{
			TitleManager.Ins.UpdateMusic(-2);
		}
	}

	public void UpdateMenuMusic(int num)
	{
		if (properCodeMusicUpdate)
		{
			properCodeMusicUpdate = false;
			return;
		}
		TitleManager.Ins.UpdateMusic(num switch
		{
			0 => -2, 
			7 => -1, 
			6 => -3, 
			_ => num - 1, 
		});
	}

	private void SetSettings()
	{
		if (!PlayerPrefs.HasKey("[GAME]Quality"))
		{
			PlayerPrefs.SetInt("[GAME]Quality", 5);
		}
		if (!PlayerPrefs.HasKey("[GAME]Nudity"))
		{
			PlayerPrefs.SetInt("[GAME]Nudity", 0);
		}
		if (!PlayerPrefs.HasKey("[GAME]Windowed"))
		{
			PlayerPrefs.SetInt("[GAME]Windowed", 1);
		}
		if (!PlayerPrefs.HasKey("[GAME]VSync"))
		{
			PlayerPrefs.SetInt("[GAME]VSync", 0);
		}
		if (!PlayerPrefs.HasKey("[MOD]DevTools"))
		{
			PlayerPrefs.SetInt("[MOD]DevTools", 1);
		}
		if (!PlayerPrefs.HasKey("[MOD]TrolloPollo"))
		{
			PlayerPrefs.SetInt("[MOD]TrolloPollo", 1);
		}
		if (!PlayerPrefs.HasKey("[MOD]MenuTheme"))
		{
			PlayerPrefs.SetInt("[MOD]MenuTheme", 1);
		}
		if (!PlayerPrefs.HasKey("[Game]GameTheme"))
		{
			PlayerPrefs.SetInt("[Game]GameTheme", 3);
		}
		if (!PlayerPrefs.HasKey("[TITLE]MenuMusicVolume"))
		{
			PlayerPrefs.SetFloat("[TITLE]MenuMusicVolume", 100f);
		}
		qualityDropdown.value = PlayerPrefs.GetInt("[GAME]Quality");
		nudityDropdown.value = PlayerPrefs.GetInt("[GAME]Nudity");
		windowedDropdown.value = PlayerPrefs.GetInt("[GAME]Windowed");
		vsyncDropdown.value = PlayerPrefs.GetInt("[GAME]VSync");
		dmcaDropdown.value = ((PlayerPrefs.GetInt("[MOD]TrolloPollo") != 1) ? 1 : 0);
		fullscreen = PlayerPrefs.GetInt("[GAME]Windowed") == 1;
		resDropdown.value = PlayerPrefs.GetInt("[GAME]ResolutionIndex");
		menuThemeDropdown.value = PlayerPrefs.GetInt("[MOD]MenuTheme");
		gameThemeDropdown.value = PlayerPrefs.GetInt("[Game]GameTheme");
		Themes.selected = (THEME)gameThemeDropdown.value;
		volumeText.text = gameThemeDropdown.value.ToString();
		volumeSlider.value = PlayerPrefs.GetFloat("[TITLE]MenuMusicVolume");
		volumeText.text = PlayerPrefs.GetFloat("[TITLE]MenuMusicVolume").ToString();
		TitleManager.Ins.musicAS.volume = PlayerPrefs.GetFloat("[TITLE]MenuMusicVolume") / 100f;
		properCodeMusicUpdate = true;
		int num = PlayerPrefs.GetInt("[TITLE]TitleMusicID");
		switch (num)
		{
		case -2:
			num = 0;
			properCodeMusicUpdate = false;
			break;
		case -1:
			num = 8;
			break;
		case -3:
			num = 7;
			break;
		default:
			num++;
			break;
		}
		menuMusicDropdown.value = num;
	}

	public void UpdateGameTheme(int num)
	{
		Themes.selected = (THEME)num;
		PlayerPrefs.SetInt("[Game]GameTheme", num);
	}

	public void ChangeVolume(float value)
	{
		Debug.Log("[Options] Setting menu music volume to: " + value);
		PlayerPrefs.SetFloat("[TITLE]MenuMusicVolume", value);
		volumeText.text = value.ToString();
		TitleManager.Ins.musicAS.volume = PlayerPrefs.GetFloat("[TITLE]MenuMusicVolume") / 100f;
	}
}
