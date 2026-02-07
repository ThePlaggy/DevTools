using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextRollerObject : MonoBehaviour
{
	public struct textTween
	{
		public bool isStep;

		public float fromValue;

		public float toValue;

		public float duration;

		public float delayPerUnit;

		public float maxDuration;

		public textTween(bool setStep, float setFromValue, float setToValue, float setDuration, float setDelayPerUnit, float setMaxDuration)
		{
			isStep = setStep;
			fromValue = setFromValue;
			toValue = setToValue;
			duration = setDuration;
			delayPerUnit = setDelayPerUnit;
			maxDuration = setMaxDuration;
		}
	}

	private List<textTween> curTweens = new List<textTween>();

	private Text myText;

	private float toValue;

	private void Awake()
	{
		myText = GetComponent<Text>();
	}

	public void ProcessLinerRequest(float FromValue, float ToValue, float SetDuration)
	{
		if (curTweens.Count > 0)
		{
			AddLinerTweenToQue(FromValue, ToValue, SetDuration);
		}
		else
		{
			FireLiner(FromValue, ToValue, SetDuration);
		}
	}

	public void AddLinerTweenToQue(float fromValue, float toValue, float setDuration)
	{
		curTweens.Add(new textTween(setStep: false, fromValue, toValue, setDuration, 0f, 0f));
	}

	public void AddStepTweenToQue(float fromValue, float toValue, float delayPerUnit, float maxDuration)
	{
		curTweens.Add(new textTween(setStep: true, fromValue, toValue, 0f, delayPerUnit, maxDuration));
	}

	public void FireLiner(float fromValue, float toValue, float setDuration)
	{
		this.toValue = toValue;
		GameManager.TweenSlinger.FireDOSTweenLiner(fromValue, toValue, setDuration, UpdateText);
	}

	public void FireLiner(float fromValue, float toValue, float setDuration, string setID)
	{
		this.toValue = toValue;
		GameManager.TweenSlinger.FireDOSTweenLiner(fromValue, toValue, setDuration, UpdateText);
	}

	public void FireStep(float fromValue, float toValue, float delayPerUnit, float maxDuration)
	{
		this.toValue = toValue;
		GameManager.TweenSlinger.FireDOSTweenStep(fromValue, toValue, delayPerUnit, maxDuration, UpdateText);
	}

	public void FireStep(float fromValue, float toValue, float delayPerUnit, float maxDuration, string setID)
	{
		this.toValue = toValue;
		GameManager.TweenSlinger.FireDOSTweenStep(fromValue, toValue, delayPerUnit, maxDuration, UpdateText);
	}

	private void UpdateText(float newValue)
	{
		if (!(myText != null))
		{
			return;
		}
		myText.text = newValue.ToString();
		if (newValue == toValue && curTweens.Count > 0)
		{
			if (curTweens[0].isStep)
			{
				toValue = curTweens[0].toValue;
				GameManager.TweenSlinger.FireDOSTweenStep(curTweens[0].fromValue, curTweens[0].toValue, curTweens[0].delayPerUnit, curTweens[0].maxDuration, UpdateText);
			}
			else
			{
				toValue = curTweens[0].toValue;
				GameManager.TweenSlinger.FireDOSTweenLiner(curTweens[0].fromValue, curTweens[0].toValue, curTweens[0].duration, UpdateText);
			}
			curTweens.RemoveAt(0);
		}
	}
}
