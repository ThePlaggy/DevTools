using System.Collections.Generic;
using SWS;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(splineMove))]
public class HitmanProxyBehaviour : MonoBehaviour
{
	public UnityEvent PathCompletedEvents = new UnityEvent();

	[SerializeField]
	private PathManagerDefinition[] paths = new PathManagerDefinition[0];

	private splineMove mySplineMove;

	private static readonly int[] RoomNumbers = new int[15]
	{
		1001, 1002, 1003, 1004, 603, 604, 605, 606, 501, 502,
		301, 302, 305, 306, 104
	};

	private Dictionary<string, float> PathManagers = new Dictionary<string, float>();

	private List<PathManagerDefinition> PathManagerPaths = new List<PathManagerDefinition>();

	public static HitmanProxyBehaviour Ins;

	public static int ChosenRoom;

	public static int ChosenRoomIndex;

	public static int ChosenRoomFloor;

	public static bool FromElevator;

	public static void SetLucasHolmes()
	{
		ChosenRoom = RoomNumbers[Random.Range(0, RoomNumbers.Length)];
		if (ChosenRoom >= 100)
		{
			ChosenRoomFloor = 1;
		}
		if (ChosenRoom >= 300)
		{
			ChosenRoomFloor = 3;
		}
		if (ChosenRoom >= 500)
		{
			ChosenRoomFloor = 5;
		}
		if (ChosenRoom >= 600)
		{
			ChosenRoomFloor = 6;
		}
		if (ChosenRoom >= 1000)
		{
			ChosenRoomFloor = 10;
		}
		ChosenRoomIndex = ChosenRoom % 100 - 1;
		Debug.Log("[Lucas+MEGA] Chosen Room: " + ChosenRoom);
	}

	private void Awake()
	{
		Ins = this;
		mySplineMove = GetComponent<splineMove>();
		PathManagers.Clear();
		PathManagers.Add("HitManPath1", 30f);
		PathManagers.Add("HitmanPath2", 45f);
		PathManagers.Add("HitmanPath3", 45f);
		PathManagers.Add("HitmanPath4", 30f);
		PathManagers.Add("HitmanPath5", 60f);
		PathManagers.Add("HitmanPath6", 45f);
		PathManagers.Add("HitmanPath7", 90f);
		PathManagers.Add("HitmanPath8", 45f);
		PathManagers.Add("HitmanPath9", 60f);
		if (ChosenRoom != 1003 && ChosenRoom != 1004)
		{
			PathManagers.Remove("HitManPath1");
		}
		if (ChosenRoom != 1001 && ChosenRoom != 1002)
		{
			PathManagers.Remove("HitmanPath2");
		}
		if (ChosenRoom != 605 && ChosenRoom != 606)
		{
			PathManagers.Remove("HitmanPath4");
		}
		if (ChosenRoom != 603 && ChosenRoom != 604)
		{
			PathManagers.Remove("HitmanPath5");
		}
		if (ChosenRoom != 501 && ChosenRoom != 502)
		{
			PathManagers.Remove("HitmanPath6");
		}
		if (ChosenRoom != 305 && ChosenRoom != 306)
		{
			PathManagers.Remove("HitmanPath7");
		}
		if (ChosenRoom != 301 && ChosenRoom != 302)
		{
			PathManagers.Remove("HitmanPath8");
		}
		PathManagerPaths.Clear();
		foreach (KeyValuePair<string, float> pathManager in PathManagers)
		{
			PathManagerDefinition pathManagerDefinition = ScriptableObject.CreateInstance<PathManagerDefinition>();
			pathManagerDefinition.PathTime = pathManager.Value;
			pathManagerDefinition.ThePath = GameObject.Find(pathManager.Key).GetComponent<PathManager>();
			PathManagerPaths.Add(pathManagerDefinition);
		}
		Debug.Log("[Lucas+MEGA] New Hitman Proxy");
		foreach (PathManagerDefinition pathManagerPath in PathManagerPaths)
		{
			Debug.Log("[Lucas+MEGA] Path added: " + pathManagerPath.ThePath.gameObject.name);
		}
	}

	private void OnDestroy()
	{
		Ins = null;
		PathManagers.Clear();
		PathManagerPaths.Clear();
		Debug.Log("[Lucas+MEGA] Disabling Proxy");
	}

	public void TriggerPath()
	{
		int index = Random.Range(0, PathManagerPaths.Count);
		Debug.Log("[HitmanProxyBehaviour] Started proxy on : " + PathManagerPaths[index].ThePath.gameObject.name);
		if (PathManagerPaths[index].ThePath.gameObject.name != "HitmanPath3" && PathManagerPaths[index].ThePath.gameObject.name != "HitmanPath9")
		{
			doorlogBehaviour.DoLucas();
		}
		if (PathManagerPaths[index].ThePath.gameObject.name == "HitmanPath3")
		{
			doorlogBehaviour.UnknownAdd();
		}
		if (PathManagerPaths[index].ThePath.gameObject.name == "HitmanPath9" && ChosenRoom == 104 && Random.Range(0, 2) == 0)
		{
			doorlogBehaviour.DoLucas();
		}
		if (PathManagerPaths[index].ThePath.gameObject.name == "HitManPath1" || PathManagerPaths[index].ThePath.gameObject.name == "HitmanPath4" || PathManagerPaths[index].ThePath.gameObject.name == "HitmanPath6" || PathManagerPaths[index].ThePath.gameObject.name == "HitmanPath8" || PathManagerPaths[index].ThePath.gameObject.name == "HitmanPath9")
		{
			FromElevator = true;
		}
		else
		{
			FromElevator = false;
		}
		mySplineMove.PathIsCompleted += triggerPathCompleteEvents;
		mySplineMove.pathContainer = PathManagerPaths[index].ThePath;
		mySplineMove.SetPath(PathManagerPaths[index].ThePath);
		mySplineMove.speed = PathManagerPaths[index].PathTime;
		mySplineMove.StartMove();
	}

	private void triggerPathCompleteEvents()
	{
		mySplineMove.PathIsCompleted -= triggerPathCompleteEvents;
		EnemyManager.HitManManager.HitmanEventDone();
		GameManager.TimeSlinger.FireTimer(1f, delegate
		{
			base.transform.position = Vector3.zero;
		});
	}
}
