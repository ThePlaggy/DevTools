using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ClockHook : MonoBehaviour
{
	private Text myText;

	private void Awake()
	{
		myText = GetComponent<Text>();
		GameManager.TimeKeeper.UpdateClockEvents.Event += updateClock;
	}

	private void updateClock(string ClockValue)
	{
		if (!(base.name == "KeypadPass"))
		{
			myText.text = ClockValue;
		}
	}
}
