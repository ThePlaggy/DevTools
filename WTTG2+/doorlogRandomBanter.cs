using UnityEngine;

public class doorlogRandomBanter : MonoBehaviour
{
	private bool banterActive;

	private float banterFireWindow;

	private float banterTimeStamp;

	public static doorlogRandomBanter Ins;

	public string BanterDebug => (banterFireWindow - (Time.time - banterTimeStamp) > 0f) ? ((int)(banterFireWindow - (Time.time - banterTimeStamp))).ToString() : 0.ToString();

	private void Awake()
	{
		Ins = this;
	}

	private void OnDestroy()
	{
		Ins = null;
	}

	public void generateFireWindow()
	{
		banterActive = true;
		banterFireWindow = Random.Range(320f, 730f);
		banterTimeStamp = Time.time;
	}

	private void Update()
	{
		if (banterActive && Time.time - banterTimeStamp >= banterFireWindow)
		{
			banterActive = false;
			spawnRandomBanter();
		}
	}

	public void spawnRandomBanter()
	{
		if (Random.Range(0, 100) <= 5)
		{
			doorlogBehaviour.UnknownAdd();
			generateFireWindow();
			return;
		}
		string ap = getRandomApartment();
		if (ap.Length <= 2)
		{
			Debug.Log("doorLOG Banter: Picked Lucas Holmes, Returning Unknown");
			doorlogBehaviour.UnknownAdd();
			generateFireWindow();
			return;
		}
		bool flag = Random.Range(0, 100) >= 90;
		doorlogBehaviour.MayAddDoorlog("Apartment " + ap, mode: true);
		GameManager.TimeSlinger.FireTimer(flag ? Random.Range(1f, 6f) : Random.Range(32f, 51f), delegate
		{
			doorlogBehaviour.MayAddDoorlog("Apartment " + ap, mode: false);
		});
		generateFireWindow();
	}

	private string getRandomApartment()
	{
		int num = Random.Range(0, 6) + 1;
		int[] array = new int[5] { 1, 3, 5, 6, 10 };
		int num2 = array[Random.Range(0, array.Length)];
		if (num2 == 1 && num == 3)
		{
			num = ((Random.Range(0, 2) != 0) ? (num - 1) : (num + 1));
		}
		if (num2 == 5 && num == 4)
		{
			num = ((Random.Range(0, 2) != 0) ? (num - 1) : (num + 1));
		}
		if (num2 == 6 && num == 2)
		{
			num = ((Random.Range(0, 2) != 0) ? (num - 1) : (num + 1));
		}
		if (num2 == HitmanProxyBehaviour.ChosenRoomFloor && num == HitmanProxyBehaviour.ChosenRoomIndex + 1)
		{
			return "";
		}
		return $"{num2}0{num}";
	}
}
