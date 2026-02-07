using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TheSwanBehaviour : MonoBehaviour
{
	public static TheSwanBehaviour Ins;

	public static readonly string computerScreenInput = ">:";

	public static readonly string bleepingDigit = "▐";

	public static bool SwanActivatedBefore;

	public bool systemFailure;

	public bool Staged;

	public Text digit1text;

	public Text digit2text;

	public Text digit3text;

	public Text digit4text;

	public Sprite[] glyphimgs;

	public Image[] glyphobjs;

	public Text[] buttonstext;

	public GameObject glyphs;

	public Text computerScreen;

	[HideInInspector]
	public bool shouldTick;

	[HideInInspector]
	public float doomsdayClock = 108f;

	public bool loading;

	private bool bleepOn = true;

	private bool ShouldCountDown;

	public static bool SwanInControlOfDisco;

	private List<string> inputFieldNew = new List<string>();

	private string systemFailureField;

	public static Color MySwanDefaultColor;

	private void Awake()
	{
		Ins = this;
		ToggleGlyphs(active: true);
		for (int i = 0; i < buttonstext.Length; i++)
		{
			buttonstext[i].text = Random.Range(0, 100).ToString();
		}
		Debug.Log("[TH3SW4N] TheSwanBehaviour.Awake()");
	}

	private void FixedUpdate()
	{
		if (bleepOn)
		{
			Text text = computerScreen;
			string text2 = (computerScreen.text = computerScreenInput + GetInputField() + bleepingDigit);
			text.text = text2;
		}
		else
		{
			Text text4 = computerScreen;
			string text2 = (computerScreen.text = computerScreenInput + GetInputField());
			text4.text = text2;
		}
	}

	private void OnDestroy()
	{
		Debug.Log("[TH3SW4N] Unstaged");
		Ins = null;
	}

	private string GetInputField()
	{
		return systemFailure ? systemFailureField : inputFieldNew.Aggregate(string.Empty, (string current, string t) => current + " " + t);
	}

	public void SubmitNumber(string number)
	{
		if (inputFieldNew.Count < 6 && !(doomsdayClock > 60f))
		{
			inputFieldNew.Add(number);
		}
	}

	public void EnterCode()
	{
		if (inputFieldNew.Count > 0)
		{
			if (inputFieldNew.Count == 6 && inputFieldNew[0] == "4" && inputFieldNew[1] == "8" && inputFieldNew[2] == "15" && inputFieldNew[3] == "16" && inputFieldNew[4] == "23" && inputFieldNew[5] == "42")
			{
				resetSwan();
				DOSCoinsCurrencyManager.AddCurrency(Random.Range(0.5f, 3.5f));
			}
			else
			{
				DOSCoinsCurrencyManager.RemoveCurrency(Random.Range(0.5f, 3.5f));
				GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.strike);
			}
			inputFieldNew.Clear();
		}
	}

	public void DeleteChar()
	{
		if (inputFieldNew.Count > 0 && !(doomsdayClock > 60f))
		{
			inputFieldNew.RemoveAt(inputFieldNew.Count - 1);
		}
	}

	private void resetSwan()
	{
		GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.reset);
		shouldTick = false;
		ShouldCountDown = false;
		doomsdayClock = 108f;
		bleepOn = false;
		ResetAnimation();
	}

	private void InitReset()
	{
		shouldTick = true;
		if (DifficultyManager.Nightmare)
		{
			if (RouterBehaviour.Ins.Owned && RouterBehaviour.Ins.RouterIsActive && !RouterBehaviour.Ins.IsJammed)
			{
				GameManager.TimeSlinger.FireTimer(Random.Range(30f, 90f), InitDoReset);
			}
			else
			{
				ShouldCountDown = true;
			}
		}
		else if (RouterBehaviour.Ins.Owned && RouterBehaviour.Ins.RouterIsActive && !RouterBehaviour.Ins.IsJammed)
		{
			GameManager.TimeSlinger.FireTimer(Random.Range(130f, 180f), InitDoReset);
		}
		else
		{
			GameManager.TimeSlinger.FireTimer(Random.Range(20f, 80f), InitDoReset);
		}
	}

	public void ActivateTheSwan()
	{
		loading = true;
		ScramblingNumbers();
	}

	private void ScramblingNumbers()
	{
		if (!Staged)
		{
			bleepOn = !bleepOn;
			for (float num = 0.1f; num < 18f; num += 0.1f)
			{
				GameManager.TimeSlinger.FireTimer(num, ClockRandomDigits);
			}
			GameManager.TimeSlinger.FireTimer(18.5f, StageSwan);
		}
	}

	private void CauseSystemFailure()
	{
		if (systemFailure)
		{
			return;
		}
		systemFailure = true;
		TheSwan.mySkyBreak.CauseSystemFailure();
		GameManager.HackerManager.theSwan.TakeSwanDOSB4();
		for (float num = 0f; num < 10f; num += 1.9f)
		{
			GameManager.TimeSlinger.FireTimer(0.1f + num, delegate
			{
				systemFailureField = " S";
			});
			GameManager.TimeSlinger.FireTimer(0.2f + num, delegate
			{
				systemFailureField = " Sy";
			});
			GameManager.TimeSlinger.FireTimer(0.3f + num, delegate
			{
				systemFailureField = " Sys";
			});
			GameManager.TimeSlinger.FireTimer(0.4f + num, delegate
			{
				systemFailureField = " Syst";
			});
			GameManager.TimeSlinger.FireTimer(0.5f + num, delegate
			{
				systemFailureField = " Syste";
			});
			GameManager.TimeSlinger.FireTimer(0.6f + num, delegate
			{
				systemFailureField = " System";
			});
			GameManager.TimeSlinger.FireTimer(0.7f + num, delegate
			{
				systemFailureField = " System ";
			});
			GameManager.TimeSlinger.FireTimer(0.8f + num, delegate
			{
				systemFailureField = " System F";
			});
			GameManager.TimeSlinger.FireTimer(0.9f + num, delegate
			{
				systemFailureField = " System Fa";
			});
			GameManager.TimeSlinger.FireTimer(1f + num, delegate
			{
				systemFailureField = " System Fai";
			});
			GameManager.TimeSlinger.FireTimer(1.1f + num, delegate
			{
				systemFailureField = " System Fail";
			});
			GameManager.TimeSlinger.FireTimer(1.2f + num, delegate
			{
				systemFailureField = " System Failu";
			});
			GameManager.TimeSlinger.FireTimer(1.3f + num, delegate
			{
				systemFailureField = " System Failur";
			});
			GameManager.TimeSlinger.FireTimer(1.4f + num, delegate
			{
				systemFailureField = " System Failure";
			});
			GameManager.TimeSlinger.FireTimer(1.9f + num, delegate
			{
				systemFailureField = string.Empty;
			});
		}
		SwanInControlOfDisco = true;
		EnvironmentManager.PowerBehaviour.SwanDisco(Color.red);
		for (int num2 = 1; num2 <= 11; num2++)
		{
			if (num2 % 2 == 0)
			{
				GameManager.TimeSlinger.FireTimer(num2, EnvironmentManager.PowerBehaviour.SwanDisco, Color.red);
			}
			else
			{
				GameManager.TimeSlinger.FireTimer(num2, EnvironmentManager.PowerBehaviour.ResetDefaultLights);
			}
		}
		for (float num3 = 0.1f; num3 < 11f; num3 += 0.1f)
		{
			GameManager.TimeSlinger.FireTimer(num3, ClockRandomDigits);
		}
		for (float num4 = 0.1f; num4 < 10f; num4 += 1.6f)
		{
			GameManager.TimeSlinger.FireTimer(num4, BleepingWallpaper);
		}
		for (float num5 = 0.9f; num5 < 12f; num5 += 1.6f)
		{
			GameManager.TimeSlinger.FireTimer(num5, BleepingWallpaperBack);
		}
		GameManager.TimeSlinger.FireTimer(5f, EnemyManager.CultManager.BREAKLIGHT, -1);
		GameManager.TimeSlinger.FireTimer(11.1f, ReStageSwan);
	}

	private void ResetAnimation()
	{
		GameManager.TimeSlinger.FireTimer(0.1f, ClockRandomDigits);
		GameManager.TimeSlinger.FireTimer(0.2f, ClockRandomDigits);
		GameManager.TimeSlinger.FireTimer(0.3f, ClockRandomDigits);
		GameManager.TimeSlinger.FireTimer(0.4f, ClockRandomDigits);
		GameManager.TimeSlinger.FireTimer(0.5f, ClockRandomDigits);
		GameManager.TimeSlinger.FireTimer(0.6f, ClockRandomDigits);
		GameManager.TimeSlinger.FireTimer(0.7f, ClockRandomDigits);
		GameManager.TimeSlinger.FireTimer(0.8f, ClockRandomDigits);
		GameManager.TimeSlinger.FireTimer(0.9f, ClockRandomDigits);
		GameManager.TimeSlinger.FireTimer(1f, SetCorrectClock);
		GameManager.TimeSlinger.FireTimer(1f, randomizeNumbers);
		GameManager.TimeSlinger.FireTimer(1.5f, InitReset);
	}

	private void StageSwan()
	{
		Staged = true;
		loading = false;
		ToggleGlyphs(active: false);
		resetSwan();
		GameManager.TimeSlinger.FireTimer(1.35f, UpdateClock);
		GameManager.TimeSlinger.FireTimer(0.4f, BleepingComputer);
	}

	private void ReStageSwan()
	{
		inputFieldNew.Clear();
		doomsdayClock = 108f;
		systemFailure = false;
		ToggleGlyphs(active: false);
		resetSwan();
		SwanInControlOfDisco = false;
	}

	private void BleepingComputer()
	{
		if (!CultManager.StagedEnd)
		{
			GameManager.TimeSlinger.FireTimer(0.4f, BleepingComputer);
			if (shouldTick)
			{
				bleepOn = !bleepOn;
			}
		}
	}

	private void UpdateClock()
	{
		if (CultManager.StagedEnd)
		{
			return;
		}
		GameManager.TimeSlinger.FireTimer(1.35f, UpdateClock);
		if (!shouldTick || systemFailure || !ComputerPowerHook.Ins.FullyPoweredOn || EnvironmentManager.PowerState == POWER_STATE.OFF || StateManager.BeingHacked)
		{
			return;
		}
		if (doomsdayClock <= 0f)
		{
			ToggleGlyphs(active: true);
			CauseSystemFailure();
			return;
		}
		if (ShouldCountDown)
		{
			doomsdayClock -= 1f;
			PlaySwanAudio();
		}
		SetCorrectClock();
	}

	private void ToggleGlyphs(bool active)
	{
		glyphs.SetActive(active);
	}

	private void ClockRandomDigits()
	{
		int num = Random.Range(0, 10);
		int num2 = Random.Range(0, 10);
		int num3 = Random.Range(0, 10);
		int num4 = Random.Range(0, 10);
		for (int i = 0; i < buttonstext.Length; i++)
		{
			buttonstext[i].text = Random.Range(0, 100).ToString();
		}
		digit1text.text = num.ToString();
		digit2text.text = num2.ToString();
		digit3text.text = num3.ToString();
		digit4text.text = num4.ToString();
		if (glyphs.activeSelf)
		{
			int[] uniqueRandomArray = getUniqueRandomArray(0, 4, 4);
			glyphobjs[0].sprite = glyphimgs[uniqueRandomArray[0]];
			glyphobjs[1].sprite = glyphimgs[uniqueRandomArray[1]];
			glyphobjs[2].sprite = glyphimgs[uniqueRandomArray[2]];
			glyphobjs[3].sprite = glyphimgs[uniqueRandomArray[3]];
			bleepOn = !bleepOn;
		}
	}

	private void SetCorrectClock()
	{
		int num = (int)doomsdayClock / 100;
		int num2 = (int)doomsdayClock / 10 % 10;
		int num3 = (int)doomsdayClock % 10;
		digit1text.text = "0";
		digit2text.text = num.ToString();
		digit3text.text = num2.ToString();
		digit4text.text = num3.ToString();
	}

	public static int[] getUniqueRandomArray(int min, int max, int count)
	{
		int[] array = new int[count];
		List<int> list = new List<int>();
		for (int i = min; i < max; i++)
		{
			list.Add(i);
		}
		for (int j = 0; j < count; j++)
		{
			int index = Random.Range(0, list.Count);
			array[j] = list[index];
			list.RemoveAt(index);
		}
		return array;
	}

	public static int[] getUniqueRandomArrayNoDoom(int min, int max, int count)
	{
		int[] array = new int[count];
		List<int> list = new List<int>();
		for (int i = min; i < max; i++)
		{
			if (i != 4 && i != 8 && i != 15 && i != 16 && i != 23 && i != 42)
			{
				list.Add(i);
			}
		}
		for (int j = 0; j < count; j++)
		{
			int index = Random.Range(0, list.Count);
			array[j] = list[index];
			list.RemoveAt(index);
		}
		return array;
	}

	private void randomizeNumbers()
	{
		int[] uniqueRandomArrayNoDoom = getUniqueRandomArrayNoDoom(1, 100, 12);
		for (int i = 0; i < buttonstext.Length; i++)
		{
			buttonstext[i].text = uniqueRandomArrayNoDoom[i].ToString();
		}
		putSwanNumbers();
	}

	private void putSwanNumbers()
	{
		int[] uniqueRandomArray = getUniqueRandomArray(0, 12, 6);
		buttonstext[uniqueRandomArray[0]].text = "4";
		buttonstext[uniqueRandomArray[1]].text = "8";
		buttonstext[uniqueRandomArray[2]].text = "15";
		buttonstext[uniqueRandomArray[3]].text = "16";
		buttonstext[uniqueRandomArray[4]].text = "23";
		buttonstext[uniqueRandomArray[5]].text = "42";
	}

	public static void FailSafeActivate()
	{
		if (!SwanActivatedBefore)
		{
			SwanActivatedBefore = true;
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.failsafe);
			AppCreator.TheSwanAppObject.SetActive(value: true);
			GameObject gameObject = Object.Instantiate(CustomObjectLookUp.TH3SW4N, AppCreator.TheSwanAppObject.transform, worldPositionStays: true);
			gameObject.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
			gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(Vector3.zero);
			gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
			gameObject.AddComponent<GraphicRaycaster>();
			gameObject.AddComponent<GraphicsRayCasterCatcher>();
			AppCreator.TheSwanAppObject.transform.GetChild(0).SetAsLastSibling();
			GameManager.TimeSlinger.FireTimer(0.05f, Ins.ActivateTheSwan);
		}
	}

	private void PlaySwanAudio()
	{
		if (doomsdayClock < 10f && doomsdayClock > 0f)
		{
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.alarm);
			GameManager.AudioSlinger.PlaySoundWithCustomDelay(CustomSoundLookUp.alarm, 0.5f);
		}
		else if (doomsdayClock >= 10f && doomsdayClock <= 20f)
		{
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.alarm);
		}
		else if (doomsdayClock > 20f && doomsdayClock <= 60f)
		{
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.beep);
		}
		else
		{
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.glyphClick);
		}
	}

	private void InitDoReset()
	{
		ShouldCountDown = true;
	}

	private void BleepingWallpaper()
	{
		WallpaperUtils.desktopWallpaper.GetComponent<Image>().DOColor(DifficultyManager.Nightmare ? new Color(0.752f, 0.75f, 0.241f, 1f) : new Color(0.552f, 0.05f, 0.111f, 1f), 0.8f);
	}

	private void BleepingWallpaperBack()
	{
		WallpaperUtils.desktopWallpaper.GetComponent<Image>().DOColor(MySwanDefaultColor, 0.8f);
	}
}
