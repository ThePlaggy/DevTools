using UnityEngine;

public class RandomDOSDrainer : MonoBehaviour
{
	private bool windowActive;

	private float fireWindow;

	private float timeStamp;

	public string RandomDOSDrainerDebug
	{
		get
		{
			if (fireWindow - (Time.time - timeStamp) > 0f)
			{
				return ((int)(fireWindow - (Time.time - timeStamp))).ToString();
			}
			return 0.ToString();
		}
	}

	private void Start()
	{
		generateFireWindow();
	}

	private void generateFireWindow()
	{
		windowActive = true;
		fireWindow = (DifficultyManager.Nightmare ? Random.Range(600f, 1800f) : Random.Range(1200f, 2700f));
		timeStamp = Time.time;
	}

	private void Update()
	{
		if (windowActive && Time.time - timeStamp >= fireWindow)
		{
			windowActive = false;
			activateDosDrainer();
		}
	}

	private void activateDosDrainer()
	{
		if (Random.Range(0, 100) <= 20 || DifficultyManager.Nightmare)
		{
			WiFiPoll.lastConnectedWifi.affectedByDosDrainer = true;
		}
		generateFireWindow();
	}
}
