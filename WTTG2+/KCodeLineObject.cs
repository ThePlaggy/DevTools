using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class KCodeLineObject : MonoBehaviour
{
	public KAttack myKAttack;

	public Image checkMark;

	public Image codeHighlight;

	public Image boxHighlight;

	public Text lineNumber;

	public Text codeLine;

	public Color codeHighlightColor;

	public Color boxHighlightColor;

	public Color validCodeHighlightColor;

	public Color invalidCodeHighlightColor;

	private Color activeColor;

	private Sequence amActiveSeq;

	private string myCodeLine;

	private string myCodeNumber;

	private Sequence showMeSeq;

	private Sequence vSeq;

	public void buildMe(int myIndex, string codeNumber, string codeText, float myX, float myY)
	{
		myCodeNumber = codeNumber;
		myCodeLine = codeText;
		GetComponent<RectTransform>().transform.localPosition = new Vector3(myX, myY, 0f);
		GetComponent<RectTransform>().transform.localScale = new Vector3(1f, 1f, 1f);
		GameManager.TimeSlinger.FireTimer((float)myIndex * 0.2f, showMe);
	}

	public void IAmActive()
	{
		codeHighlight.color = codeHighlightColor;
		activeColor = codeHighlightColor;
		amActiveSeq = DOTween.Sequence();
		amActiveSeq.Insert(0f, DOTween.To(() => codeHighlight.color, delegate(Color x)
		{
			codeHighlight.color = x;
		}, new Color(activeColor.r, activeColor.g, activeColor.b, 0.1f), 1f).SetEase(Ease.Linear));
		amActiveSeq.Insert(1f, DOTween.To(() => codeHighlight.color, delegate(Color x)
		{
			codeHighlight.color = x;
		}, new Color(activeColor.r, activeColor.g, activeColor.b, 1f), 1f).SetEase(Ease.Linear));
		amActiveSeq.SetLoops(-1);
		amActiveSeq.Play();
	}

	public void InvalidInput()
	{
		if (ComputerPowerHook.Ins.FullyPoweredOn && StateManager.PlayerState == PLAYER_STATE.COMPUTER)
		{
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.InvalidInput);
		}
		lineNumber.color = boxHighlightColor;
		codeLine.color = boxHighlightColor;
		boxHighlight.color = new Color(invalidCodeHighlightColor.r, invalidCodeHighlightColor.g, invalidCodeHighlightColor.b, 1f);
		vSeq = DOTween.Sequence();
		vSeq.Insert(0f, DOTween.To(() => boxHighlight.color, delegate(Color x)
		{
			boxHighlight.color = x;
		}, new Color(invalidCodeHighlightColor.r, invalidCodeHighlightColor.g, invalidCodeHighlightColor.b, 0f), 0.25f).SetEase(Ease.Linear));
		vSeq.Play();
	}

	public void ValidInput()
	{
		if (ComputerPowerHook.Ins.FullyPoweredOn)
		{
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.ValidInput);
		}
		boxHighlight.color = validCodeHighlightColor;
		amActiveSeq.Kill();
		codeHighlight.color = new Color(codeHighlightColor.r, codeHighlightColor.g, codeHighlightColor.b, 0f);
		lineNumber.gameObject.SetActive(value: false);
		checkMark.gameObject.SetActive(value: true);
		codeLine.fontStyle = FontStyle.Italic;
		codeLine.color = new Color(codeLine.color.r, codeLine.color.g, codeLine.color.b, 0.3f);
		showMeSeq = DOTween.Sequence();
		showMeSeq.Insert(0f, DOTween.To(() => boxHighlight.color, delegate(Color x)
		{
			boxHighlight.color = x;
		}, new Color(validCodeHighlightColor.r, validCodeHighlightColor.g, validCodeHighlightColor.b, 0f), 0.25f).SetEase(Ease.Linear));
		showMeSeq.Play();
	}

	public void hotCheck(string compareString = "")
	{
		if (compareString.Length <= myCodeLine.Length)
		{
			if (compareString.Equals(myCodeLine.Substring(0, compareString.Length)))
			{
				lineNumber.color = boxHighlightColor;
				codeLine.color = boxHighlightColor;
			}
			else
			{
				lineNumber.color = invalidCodeHighlightColor;
				codeLine.color = invalidCodeHighlightColor;
			}
		}
		else
		{
			lineNumber.color = invalidCodeHighlightColor;
			codeLine.color = invalidCodeHighlightColor;
		}
	}

	private void showMe()
	{
		if (ComputerPowerHook.Ins.FullyPoweredOn)
		{
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.highLightCode);
		}
		boxHighlight.color = boxHighlightColor;
		lineNumber.text = myCodeNumber;
		codeLine.text = myCodeLine;
		showMeSeq = DOTween.Sequence();
		showMeSeq.Insert(0f, DOTween.To(() => boxHighlight.color, delegate(Color x)
		{
			boxHighlight.color = x;
		}, new Color(boxHighlightColor.r, boxHighlightColor.g, boxHighlightColor.b, 0f), 0.3f).SetEase(Ease.Linear));
		showMeSeq.Play();
	}
}
