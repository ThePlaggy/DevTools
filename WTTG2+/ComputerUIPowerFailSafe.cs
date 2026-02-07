using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class ComputerUIPowerFailSafe : MonoBehaviour
{
	private bool envCheck;

	private Canvas myCanvas;

	private bool previousValue;

	private float timeStamp;

	private void Awake()
	{
		myCanvas = GetComponent<Canvas>();
	}

	private void Start()
	{
		if (EnvironmentManager.PowerBehaviour != null)
		{
			EnvironmentManager.PowerBehaviour.PowerOffEvent.Event += powerWentOff;
			EnvironmentManager.PowerBehaviour.PowerOnEvent.Event += powerWentOn;
		}
		else
		{
			timeStamp = Time.time;
			envCheck = true;
		}
	}

	private void Update()
	{
		if (envCheck && Time.time - timeStamp >= 10f)
		{
			if (EnvironmentManager.PowerBehaviour != null)
			{
				EnvironmentManager.PowerBehaviour.PowerOffEvent.Event += powerWentOff;
				EnvironmentManager.PowerBehaviour.PowerOnEvent.Event += powerWentOn;
				envCheck = false;
			}
			else
			{
				timeStamp = Time.time;
			}
		}
	}

	private void OnDestroy()
	{
	}

	private void powerWentOff()
	{
		previousValue = myCanvas.enabled;
		myCanvas.enabled = false;
	}

	private void powerWentOn()
	{
		myCanvas.enabled = previousValue;
	}
}
