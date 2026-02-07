using System.Collections.Generic;
using UnityEngine;

public class MotionSensorMenuBehaviour : MonoBehaviour
{
	private const float OPT_SPACING = 4f;

	private const float BOT_SPACING = 4f;

	[SerializeField]
	private int MOTION_SENSOR_MENU_POOL_COUNT = 6;

	[SerializeField]
	private CanvasGroup nonActiveCG;

	[SerializeField]
	private GameObject motionSensorMenuObject;

	private Dictionary<MotionSensorObject, MotionSensorMenuOptionObject> currentActiveMotionSensors = new Dictionary<MotionSensorObject, MotionSensorMenuOptionObject>(6);

	private PooledStack<MotionSensorMenuOptionObject> motionSensorMenuOptionsPool;

	private RectTransform myRT;

	private void Awake()
	{
		myRT = GetComponent<RectTransform>();
		motionSensorMenuOptionsPool = new PooledStack<MotionSensorMenuOptionObject>(delegate
		{
			MotionSensorMenuOptionObject component = Object.Instantiate(motionSensorMenuObject, myRT).GetComponent<MotionSensorMenuOptionObject>();
			component.SoftBuild();
			return component;
		}, MOTION_SENSOR_MENU_POOL_COUNT);
		GameManager.StageManager.Stage += stageMe;
	}

	private void OnDestroy()
	{
	}

	private void rebuildMenu()
	{
		float num = 0f;
		if (currentActiveMotionSensors.Count > 0)
		{
			num = 4f + ((float)currentActiveMotionSensors.Count * 24f + (float)currentActiveMotionSensors.Count * 4f) + 4f;
			nonActiveCG.alpha = 0f;
		}
		else
		{
			num = 32f;
			nonActiveCG.alpha = 1f;
		}
		int num2 = 0;
		foreach (KeyValuePair<MotionSensorObject, MotionSensorMenuOptionObject> currentActiveMotionSensor in currentActiveMotionSensors)
		{
			currentActiveMotionSensor.Value.PutMe(num2);
			num2++;
		}
		Vector2 sizeDelta = new Vector2(myRT.sizeDelta.x, num);
		Vector2 anchoredPosition = new Vector2(myRT.anchoredPosition.x, num);
		if (myRT.anchoredPosition.y == -41f)
		{
			anchoredPosition.y = -41f;
		}
		myRT.sizeDelta = sizeDelta;
		myRT.anchoredPosition = anchoredPosition;
	}

	private void clearMotionSensorFromMenu(MotionSensorObject TheMotionSensor)
	{
		if (currentActiveMotionSensors.TryGetValue(TheMotionSensor, out var value))
		{
			value.ClearMe();
			currentActiveMotionSensors.Remove(TheMotionSensor);
			motionSensorMenuOptionsPool.Push(value);
			rebuildMenu();
		}
	}

	private void addMotionSensorToMenu(MotionSensorObject TheMotionSensor)
	{
		if (TheMotionSensor.Placed)
		{
			if (!currentActiveMotionSensors.ContainsKey(TheMotionSensor))
			{
				MotionSensorMenuOptionObject motionSensorMenuOptionObject = motionSensorMenuOptionsPool.Pop();
				motionSensorMenuOptionObject.BuildMe(TheMotionSensor);
				currentActiveMotionSensors.Add(TheMotionSensor, motionSensorMenuOptionObject);
			}
			rebuildMenu();
		}
	}

	private void stageMe()
	{
		GameManager.StageManager.Stage -= stageMe;
		rebuildMenu();
		GameManager.ManagerSlinger.MotionSensorManager.EnteredPlacementMode += clearMotionSensorFromMenu;
		GameManager.ManagerSlinger.MotionSensorManager.MotionSensorPlaced += addMotionSensorToMenu;
		GameManager.ManagerSlinger.MotionSensorManager.MotionSensorWasReturned += clearMotionSensorFromMenu;
	}
}
