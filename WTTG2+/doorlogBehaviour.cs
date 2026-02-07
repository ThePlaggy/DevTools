using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class doorlogBehaviour : WindowBehaviour
{
	public static doorlogBehaviour Ins;

	public static bool Launched;

	[HideInInspector]
	public GameObject theLine;

	[HideInInspector]
	public GameObject lineHolder;

	private List<GameObject> lines = new List<GameObject>();

	private const int MAX_LOGS = 37;

	public static readonly string bleepingDigit = "▐";

	private static Dictionary<string, string> interactableDoors = new Dictionary<string, string>();

	private new void Start()
	{
		interactableDoors.Clear();
		interactableDoors.Add("doorApartment", "Apartment 803");
		interactableDoors.Add("MaintenanceRoomDoor", "Floor 8 Maintenance");
		interactableDoors.Add("Floor10Door", "Floor 10 Hallway");
		interactableDoors.Add("Floor8Door", "Floor 8 Hallway");
		interactableDoors.Add("Floor6Door", "Floor 6 Hallway");
		interactableDoors.Add("Floor5Door", "Floor 5 Hallway");
		interactableDoors.Add("Floor3Door", "Floor 3 Hallway");
		interactableDoors.Add("Floor1Door", "Floor 1 Hallway");
	}

	private new void OnDestroy()
	{
		Ins = null;
	}

	public static void AddDoorlog(DoorTrigger door, bool mode)
	{
		if (Launched && interactableDoors.ContainsKey(door.transform.parent.gameObject.name))
		{
			string text = interactableDoors[door.transform.parent.gameObject.name];
			if (!(text == "") && Ins != null)
			{
				Ins.writeLine(string.Format("#DoorLog Report - ! {0} Door Event @--> {1} <--@ {2}", mode ? "Open" : "Close", text, TimeKeeper.Time), mode ? Color.yellow : Color.red);
			}
		}
	}

	public static void MayAddDoorlog(string doorName, bool mode)
	{
		if (Launched && !(doorName == "") && Ins != null)
		{
			Ins.writeLine(string.Format("#DoorLog Report - ! {0} Door Event @--> {1} <--@ {2}", mode ? "Open" : "Close", doorName, TimeKeeper.Time), mode ? Color.yellow : Color.red);
		}
	}

	public static void UnknownAdd()
	{
		if (Launched && Ins != null)
		{
			Ins.writeLine("(NOTICE) #DoorLog Unknown Door Event - ! @--> ??? <--@ " + TimeKeeper.Time, new Color(1f, 0f, 1f));
		}
	}

	public static void DoLucas()
	{
		if (HitmanProxyBehaviour.ChosenRoom <= 0)
		{
			Debug.Log("[DoorLog] Error spawning lucas");
			return;
		}
		MayAddDoorlog($"Apartment {HitmanProxyBehaviour.ChosenRoom}", mode: true);
		GameManager.TimeSlinger.FireTimer(Random.Range(2f, 7.5f), delegate
		{
			MayAddDoorlog($"Apartment {HitmanProxyBehaviour.ChosenRoom}", mode: false);
		});
	}

	public void writeLine(string text, Color color)
	{
		GameObject gameObject = Object.Instantiate(theLine);
		gameObject.transform.SetParent(lineHolder.transform);
		Text component = gameObject.GetComponent<Text>();
		component.color = color;
		component.text = text;
		lines.Add(gameObject);
		if (lines.Count > 37)
		{
			Object.Destroy(lines[2]);
			lines.RemoveAt(2);
		}
	}

	protected override void OnLaunch()
	{
		if (!Launched)
		{
			Launched = true;
			writeLine("Welcome to doorLOG, Stand by...", Color.green);
			writeLine("", Color.green);
		}
	}

	protected override void OnClose()
	{
		Launched = false;
		for (int i = 0; i < lines.Count; i++)
		{
			Object.Destroy(lines[i]);
		}
		lines.Clear();
	}

	protected override void OnMin()
	{
	}

	protected override void OnUnMin()
	{
	}

	protected override void OnMax()
	{
	}

	protected override void OnUnMax()
	{
	}

	protected override void OnResized()
	{
	}
}
