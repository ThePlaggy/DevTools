using UnityEngine;
using UnityEngine.UI;

public class MotionSensorMenuOptionObject : MonoBehaviour
{
	private const float OPT_SPACING = 4f;

	private const float MENU_OPT_X = 4f;

	[SerializeField]
	private Text locationText;

	[SerializeField]
	private Image stateImage;

	[SerializeField]
	private CanvasGroup activeCG;

	[SerializeField]
	private float triggerWarningDisplayTime = 0.2f;

	private MotionSensorObject myMotionSensorObject;

	private RectTransform myRT;

	private Vector2 spawnPOS = new Vector2(0f, 24f);

	private bool triggerWarningActive;

	private float triggerWarningTimeStamp;

	private void Update()
	{
		if (triggerWarningActive)
		{
			activeCG.alpha = Mathf.Lerp(1f, 0f, (Time.time - triggerWarningTimeStamp) / triggerWarningDisplayTime);
			if (Time.time - triggerWarningTimeStamp >= triggerWarningDisplayTime)
			{
				triggerWarningActive = false;
			}
		}
	}

	public void ClearMe()
	{
		myRT.anchoredPosition = spawnPOS;
		locationText.text = string.Empty;
		myMotionSensorObject.IWasTripped -= motionTriggerWasTripped;
		myMotionSensorObject = null;
	}

	public void BuildMe(MotionSensorObject TheMotionSensor)
	{
		locationText.text = MagicSlinger.GetFriendlyLocationName(TheMotionSensor.Location);
		myMotionSensorObject = TheMotionSensor;
		myMotionSensorObject.IWasTripped += motionTriggerWasTripped;
	}

	public void SoftBuild()
	{
		myRT = GetComponent<RectTransform>();
		myRT.anchoredPosition = spawnPOS;
	}

	public void PutMe(int SetIndex)
	{
		float y = 0f - (4f + ((float)SetIndex * 4f + (float)SetIndex * 24f));
		myRT.anchoredPosition = new Vector2(4f, y);
	}

	private void motionTriggerWasTripped(MotionSensorObject TheMotionSensor)
	{
		if (!triggerWarningActive)
		{
			triggerWarningTimeStamp = Time.time;
			triggerWarningActive = true;
		}
	}
}
