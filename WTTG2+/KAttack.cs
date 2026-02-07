using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KAttack : MonoBehaviour
{
	[Range(1f, 10f)]
	public float warmUpTime = 3f;

	public KernelClock kernelClock;

	public GameObject KCLObject;

	public GameObject KGroup;

	public GameObject KTermTitle;

	public GameObject KTermInput;

	public GameObject KCodeLineHolder;

	public Image KBase;

	public Image KCodeBG;

	public Image KCodeNumberLine;

	public InputField KCodeLineInput;

	public float TargetWidth = 1076f;

	public float TargetBorderWidth = 2f;

	public float CodeLineHeight = 44f;

	public Font clockFont;

	public Color clockColor;

	public List<KLevelDefinition> KLevels;

	public List<string> codeLines;

	private float KTime = 30f;

	private float KBoostTime = 5f;

	private short codeLineCount = 3;

	private Sequence KAniSeq;

	private List<string> currentCodeStack;

	private List<GameObject> currentCodeObjects;

	private short currentCodeStackIndex;

	private Text clockText;

	private float clockTimeStamp;

	private float clockMicroTimeStamp;

	private float clockMicroCount;

	private float KGameTimeStamp;

	private bool warmClockActive;

	private bool kISHot;

	private bool finalCountDownFired;

	private bool DidPlayerPass;

	private Sequence warmClockSeq;

	private Sequence KAttackClockSeq;

	private Sequence ByeSeq;

	private int currentKChainIndex;

	private int currentBackSpaceCount;

	private Device currentDevice;

	public TMP_Text nowHacking;

	public static bool IsInAttack;

	public void Awake()
	{
		Debug.Log("Initialized kernel");
		IsInAttack = false;
		kernelClock.myKAttack = this;
		for (int i = 0; i < codeLines.Count; i++)
		{
			codeLines[i] = codeLines[i].Trim();
		}
		codeLines.Add("int main(void){");
		codeLines.Add("int.MaxValue = 2147483647;");
		codeLines.Add("int64_t val = 4815162342;");
		codeLines.Add("botnetActions.exec();");
		codeLines.Add(";");
		KLevelDefinition kLevelDefinition = ScriptableObject.CreateInstance<KLevelDefinition>();
		kLevelDefinition.KNumOfLines = (short)(KLevels[KLevels.Count - 1].KNumOfLines + 1);
		kLevelDefinition.KBoostTime = KLevels[KLevels.Count - 1].KBoostTime - 1f;
		kLevelDefinition.KTime = KLevels[KLevels.Count - 1].KTime - 2f;
		KLevels.Add(kLevelDefinition);
		foreach (KLevelDefinition kLevel in KLevels)
		{
			kLevel.KTime += 8f;
		}
	}

	public void startAttack(Device device)
	{
		IsInAttack = true;
		currentDevice = device;
		prepLevel();
	}

	private void prepLevel()
	{
		int num = currentDevice.hackDifficulty;
		if (num < 0)
		{
			Debug.Log("Kernel attack index id less than 0");
			num = 0;
		}
		if (num >= KLevels.Count)
		{
			Debug.Log("Kernel attack index id greater than param");
			num = KLevels.Count - 1;
		}
		KTime = KLevels[num].KTime;
		KBoostTime = KLevels[num].KBoostTime;
		codeLineCount = KLevels[num].KNumOfLines;
		prepKAttack();
	}

	private void prepKAttack()
	{
		bool flag = false;
		short num = 0;
		currentCodeStack = new List<string>();
		while (!flag)
		{
			int index = UnityEngine.Random.Range(0, codeLines.Count);
			string item = codeLines[index];
			if (!currentCodeStack.Contains(item))
			{
				currentCodeStack.Add(item);
				num++;
				if (num >= codeLineCount)
				{
					flag = true;
				}
			}
		}
		float targetWidth = TargetWidth;
		float num2 = (float)codeLineCount * CodeLineHeight + TargetBorderWidth * 2f;
		float x = TargetWidth - TargetBorderWidth * 2f;
		float y = (float)codeLineCount * CodeLineHeight;
		float num3 = num2 / 2f + KTermTitle.GetComponent<RectTransform>().sizeDelta.y / 2f + 3f;
		float y2 = num3 + 50f;
		float num4 = 0f - (num2 / 2f + KTermInput.GetComponent<RectTransform>().sizeDelta.y / 2f + 3f);
		float y3 = num4 - 50f;
		KGroup.SetActive(value: true);
		KGroup.GetComponent<RectTransform>().sizeDelta = new Vector2(targetWidth, num2);
		InputField.SubmitEvent submitEvent = new InputField.SubmitEvent();
		submitEvent.AddListener(verifyCodeInput);
		KCodeLineInput.onEndEdit = submitEvent;
		KCodeLineInput.enabled = false;
		KTermTitle.GetComponent<RectTransform>().transform.localPosition = new Vector3(0f, y2, 0f);
		KTermInput.GetComponent<RectTransform>().transform.localPosition = new Vector3(0f, y3, 0f);
		KBase.GetComponent<RectTransform>().sizeDelta = new Vector2(targetWidth, num2);
		KCodeBG.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
		KCodeNumberLine.GetComponent<RectTransform>().sizeDelta = new Vector2(KCodeNumberLine.GetComponent<RectTransform>().sizeDelta.x, y);
		KCodeLineHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(KCodeLineHolder.GetComponent<RectTransform>().sizeDelta.x, y);
		KCodeLineHolder.GetComponent<CanvasGroup>().alpha = 1f;
		if (ComputerPowerHook.Ins.FullyPoweredOn)
		{
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.KernelPowerUp);
		}
		KAniSeq = DOTween.Sequence().OnComplete(engageKAttack);
		KAniSeq.Insert(0f, DOTween.To(() => KBase.fillAmount, delegate(float fillAmount)
		{
			KBase.fillAmount = fillAmount;
		}, 1f, 1f).SetEase(Ease.Linear));
		KAniSeq.Insert(1f, DOTween.To(() => KCodeNumberLine.fillAmount, delegate(float fillAmount)
		{
			KCodeNumberLine.fillAmount = fillAmount;
		}, 1f, 0.5f).SetEase(Ease.OutSine));
		KAniSeq.Insert(1f, DOTween.To(() => KTermTitle.GetComponent<RectTransform>().transform.localPosition, delegate(Vector3 localPosition)
		{
			KTermTitle.GetComponent<RectTransform>().transform.localPosition = localPosition;
		}, new Vector3(0f, num3, 0f), 0.5f).SetEase(Ease.OutSine));
		KAniSeq.Insert(1f, DOTween.To(() => KTermTitle.GetComponent<CanvasGroup>().alpha, delegate(float alpha)
		{
			KTermTitle.GetComponent<CanvasGroup>().alpha = alpha;
		}, 1f, 0.5f).SetEase(Ease.OutSine));
		KAniSeq.Insert(1f, DOTween.To(() => KTermInput.GetComponent<RectTransform>().transform.localPosition, delegate(Vector3 localPosition)
		{
			KTermInput.GetComponent<RectTransform>().transform.localPosition = localPosition;
		}, new Vector3(0f, num4, 0f), 0.5f).SetEase(Ease.OutSine));
		KAniSeq.Insert(1f, DOTween.To(() => KTermInput.GetComponent<CanvasGroup>().alpha, delegate(float alpha)
		{
			KTermInput.GetComponent<CanvasGroup>().alpha = alpha;
		}, 1f, 0.5f).SetEase(Ease.OutSine));
		KAniSeq.Play();
	}

	private void engageKAttack()
	{
		float num = 0f;
		currentCodeObjects = new List<GameObject>();
		for (int i = 0; i < currentCodeStack.Count; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(KCLObject);
			gameObject.transform.SetParent(KCodeLineHolder.transform);
			gameObject.GetComponent<KCodeLineObject>().myKAttack = this;
			gameObject.GetComponent<KCodeLineObject>().buildMe(i, (i + 1).ToString(), currentCodeStack[i], 0f, num);
			num -= 44f;
			currentCodeObjects.Add(gameObject);
		}
		currentCodeStackIndex = 0;
		fireWarmClock();
	}

	private void fireKAttack()
	{
		UnityEngine.Object.Destroy(clockText.gameObject);
		if (ComputerPowerHook.Ins.FullyPoweredOn)
		{
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.CountDownTick2);
		}
		kernelClock.StartTicking(KTime);
		KCodeLineInput.enabled = true;
		KCodeLineInput.ActivateInputField();
		currentCodeObjects[currentCodeStackIndex].GetComponent<KCodeLineObject>().IAmActive();
		finalCountDownFired = false;
		KGameTimeStamp = Time.time;
		kISHot = true;
	}

	private void fireWarmClock()
	{
		TextGenerationSettings settings = default(TextGenerationSettings);
		TextGenerator textGenerator = new TextGenerator();
		settings.textAnchor = TextAnchor.UpperCenter;
		settings.generateOutOfBounds = true;
		settings.generationExtents = new Vector2(50f, 20f);
		settings.pivot = Vector2.zero;
		settings.richText = true;
		settings.font = clockFont;
		settings.fontSize = 56;
		settings.fontStyle = FontStyle.Normal;
		settings.lineSpacing = 1f;
		settings.scaleFactor = 1f;
		settings.verticalOverflow = VerticalWrapMode.Overflow;
		settings.horizontalOverflow = HorizontalWrapMode.Wrap;
		clockText = new GameObject("clockText", typeof(Text)).GetComponent<Text>();
		clockText.font = clockFont;
		clockText.text = warmUpTime.ToString();
		clockText.color = clockColor;
		clockText.fontSize = 56;
		clockText.transform.SetParent(KGroup.transform);
		clockText.transform.localPosition = new Vector3(0f, KGroup.GetComponent<RectTransform>().sizeDelta.y / 2f + 75f, 0f);
		clockText.rectTransform.sizeDelta = new Vector2(textGenerator.GetPreferredWidth(clockText.text, settings), textGenerator.GetPreferredHeight(clockText.text, settings));
		clockText.transform.localScale = new Vector3(1f, 1f, 1f);
		clockText.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
		warmClockSeq = DOTween.Sequence();
		warmClockSeq.Insert(0f, DOTween.To(() => clockText.transform.localScale, delegate(Vector3 x)
		{
			clockText.transform.localScale = x;
		}, new Vector3(1f, 1f, 1f), 0f));
		warmClockSeq.Insert(0.1f, DOTween.To(() => clockText.transform.localScale, delegate(Vector3 x)
		{
			clockText.transform.localScale = x;
		}, new Vector3(0.33f, 0.33f, 0.33f), 0.9f).SetEase(Ease.Linear));
		warmClockSeq.SetLoops(Mathf.RoundToInt(warmUpTime));
		warmClockSeq.Play();
		GameManager.TimeSlinger.FireTimer(0.9f, (Action)delegate
		{
			clockMicroCount -= 1f;
			clockMicroTimeStamp = Time.time;
			clockText.text = clockMicroCount.ToString();
			if (clockMicroCount > 0f && ComputerPowerHook.Ins.FullyPoweredOn)
			{
				GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.CountDownTick1);
			}
			if (clockMicroCount <= 0f)
			{
				warmClockActive = false;
				fireKAttack();
				if (ComputerPowerHook.Ins.FullyPoweredOn)
				{
					GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.CountDownTick2);
				}
			}
		}, 3);
		clockTimeStamp = Time.time;
		clockMicroTimeStamp = Time.time;
		clockMicroCount = warmUpTime;
		warmClockActive = true;
		if (ComputerPowerHook.Ins.FullyPoweredOn)
		{
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.CountDownTick1);
		}
	}

	private void verifyCodeInput(string theCode)
	{
		if (theCode != string.Empty)
		{
			KCodeLineInput.text = string.Empty;
			KCodeLineInput.ActivateInputField();
			if (theCode == currentCodeStack[currentCodeStackIndex])
			{
				currentCodeObjects[currentCodeStackIndex].GetComponent<KCodeLineObject>().ValidInput();
				currentCodeStackIndex++;
				if (currentCodeStackIndex >= currentCodeStack.Count)
				{
					KAttackFailed();
					return;
				}
				boostKTime();
				currentCodeObjects[currentCodeStackIndex].GetComponent<KCodeLineObject>().IAmActive();
			}
			else
			{
				currentCodeObjects[currentCodeStackIndex].GetComponent<KCodeLineObject>().InvalidInput();
			}
		}
		else
		{
			KCodeLineInput.ActivateInputField();
		}
	}

	private void boostKTime()
	{
		if (KBoostTime > 0f)
		{
			kernelClock.BoostTime(KBoostTime);
		}
	}

	private void KAttackPassed()
	{
		IsInAttack = false;
		GameManager.AudioSlinger.KillSound(CustomSoundLookUp.ClockAlmostUp);
		if (ComputerPowerHook.Ins.FullyPoweredOn)
		{
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.GameFail);
		}
		BotnetBehaviour.Ins.LoseConnection();
		if (UnityEngine.Random.Range(0, 100) < 20 && !StateManager.BeingHacked && !GameManager.HackerManager.theSwan.SwanFailure && ComputerPowerHook.Ins.FullyPoweredOn && EnvironmentManager.PowerState != POWER_STATE.OFF && !EnemyStateManager.IsInEnemyState() && !IsInAttack)
		{
			GameManager.HackerManager.ForceTwitchHack();
		}
		KCodeLineInput.enabled = false;
		kISHot = false;
		DidPlayerPass = false;
		TriggerByeAni();
	}

	private void KAttackFailed()
	{
		IsInAttack = false;
		GameManager.AudioSlinger.KillSound(CustomSoundLookUp.ClockAlmostUp);
		if (ComputerPowerHook.Ins.FullyPoweredOn)
		{
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.GamePass);
		}
		kernelClock.StopTicking();
		BotnetBehaviour.Ins.AddDevice(currentDevice);
		KCodeLineInput.enabled = false;
		kISHot = false;
		DidPlayerPass = true;
		TriggerByeAni();
	}

	private void TriggerByeAni()
	{
		float num = (float)codeLineCount * CodeLineHeight + TargetBorderWidth * 2f;
		float num2 = num / 2f + KTermTitle.GetComponent<RectTransform>().sizeDelta.y / 2f + 3f;
		float num3 = 0f - (num / 2f + KTermInput.GetComponent<RectTransform>().sizeDelta.y / 2f + 3f);
		float y = num3 - 50f;
		float y2 = num2 + 50f;
		ByeSeq = DOTween.Sequence().OnComplete(ByeAniDone);
		ByeSeq.Insert(0f, DOTween.To(() => KCodeLineHolder.GetComponent<CanvasGroup>().alpha, delegate(float x)
		{
			KCodeLineHolder.GetComponent<CanvasGroup>().alpha = x;
		}, 0f, 0.5f).SetEase(Ease.OutSine));
		ByeSeq.Insert(0f, DOTween.To(() => KBase.fillAmount, delegate(float x)
		{
			KBase.fillAmount = x;
		}, 0f, 0.5f).SetEase(Ease.OutSine));
		ByeSeq.Insert(0f, DOTween.To(() => KCodeNumberLine.fillAmount, delegate(float x)
		{
			KCodeNumberLine.fillAmount = x;
		}, 0f, 0.5f).SetEase(Ease.OutSine));
		ByeSeq.Insert(0f, DOTween.To(() => KTermTitle.GetComponent<RectTransform>().transform.localPosition, delegate(Vector3 x)
		{
			KTermTitle.GetComponent<RectTransform>().transform.localPosition = x;
		}, new Vector3(0f, y2, 0f), 0.5f).SetEase(Ease.OutSine));
		ByeSeq.Insert(0f, DOTween.To(() => KTermTitle.GetComponent<CanvasGroup>().alpha, delegate(float x)
		{
			KTermTitle.GetComponent<CanvasGroup>().alpha = x;
		}, 0f, 0.5f).SetEase(Ease.OutSine));
		ByeSeq.Insert(0f, DOTween.To(() => KTermInput.GetComponent<RectTransform>().transform.localPosition, delegate(Vector3 x)
		{
			KTermInput.GetComponent<RectTransform>().transform.localPosition = x;
		}, new Vector3(0f, y, 0f), 0.5f).SetEase(Ease.OutSine));
		ByeSeq.Insert(0f, DOTween.To(() => KTermInput.GetComponent<CanvasGroup>().alpha, delegate(float x)
		{
			KTermInput.GetComponent<CanvasGroup>().alpha = x;
		}, 0f, 0.5f).SetEase(Ease.OutSine));
	}

	private void ByeAniDone()
	{
		for (int i = 0; i < currentCodeObjects.Count; i++)
		{
			UnityEngine.Object.Destroy(currentCodeObjects[i]);
		}
		currentCodeObjects.Clear();
		currentCodeStack.Clear();
		currentCodeStackIndex = 0;
		KGroup.SetActive(value: false);
		BotnetBehaviour.Ins.DismissKernelCompiler();
	}

	private void Update()
	{
		if (kISHot && Time.time - KGameTimeStamp >= KTime - CustomSoundLookUp.ClockAlmostUp.AudioClip.length && !finalCountDownFired)
		{
			finalCountDownFired = true;
		}
		if (KCodeLineInput.enabled)
		{
			if (!KCodeLineInput.isFocused)
			{
				KCodeLineInput.ActivateInputField();
			}
			else if (kISHot)
			{
				currentCodeObjects[currentCodeStackIndex].GetComponent<KCodeLineObject>().hotCheck(KCodeLineInput.text);
			}
		}
	}

	public void TimesUp()
	{
		kISHot = false;
		KAttackPassed();
	}
}
