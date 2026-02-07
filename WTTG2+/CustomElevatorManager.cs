using System;
using DG.Tweening;
using UnityEngine;

public class CustomElevatorManager : MonoBehaviour
{
	public static CustomElevatorManager Ins;

	public GameObject myElevator;

	private GameObject myDoor;

	private GameObject floor8impostorDoor;

	private GameObject floor3impostorDoor;

	private GameObject floor5impostorDoor;

	private GameObject floor6impostorDoor;

	private GameObject floor10impostorDoor;

	private GameObject floor1impostorDoor;

	private readonly Vector3 elevatorRoamVector = new Vector3(0f, -0.38f, 0f);

	[NonSerialized]
	public int WhereIAm = 8;

	public static bool Elevating;

	private CloseUpTrigger myClose;

	[NonSerialized]
	public GameObject canvasRef;

	private const float ELEVATOR_SPEED = 1.7f;

	private const float FLOOR10_LOC = 52.2248f;

	private const float FLOOR8_LOC = 40.9348f;

	private const float FLOOR6_LOC = 29.6348f;

	private const float FLOOR5_LOC = 23.9648f;

	private const float FLOOR3_LOC = 12.6548f;

	private const float FLOOR1_LOC = 1.3548f;

	private bool glassDarkened;

	private const float LEAVE_FLOOR_10_DOWN = 50.2781f;

	private const float LEAVE_FLOOR_8_UP = 41.8156f;

	private const float LEAVE_FLOOR_8_DOWN = 38.9881f;

	private const float LEAVE_FLOOR_6_UP = 30.515602f;

	private const float LEAVE_FLOOR_6_DOWN = 27.791f;

	private const float LEAVE_FLOOR_5_UP = 24.8455f;

	private const float LEAVE_FLOOR_5_DOWN = 22.0181f;

	private const float LEAVE_FLOOR_3_UP = 13.535603f;

	private const float LEAVE_FLOOR_3_DOWN = 11.774f;

	private const float LEAVE_FLOOR_1_UP = 2.2356f;

	private GameObject Floor10Hallway;

	private GameObject Floor8Hallway;

	private GameObject Floor6Hallway;

	private GameObject Floor5Hallway;

	private GameObject Floor3Hallway;

	private GameObject Floor1Hallway;

	public static bool DOOM3D;

	public static bool KillDSAS;

	private Vector3 originalPosition;

	private bool iAmRiding;

	private void Awake()
	{
		Ins = this;
		myElevator = UnityEngine.Object.Instantiate(CustomObjectLookUp.CustomElevator, new Vector3(-27.8527f, 40.9348f, -6.2923f), Quaternion.identity);
		myDoor = myElevator.transform.GetChild(0).gameObject;
		GameObject.Find("WTTG2_AB_Elevator/ExteriorParts").transform.SetParent(GameObject.Find("ApartmentStructure/Static/Misc").transform);
		GameObject.Find("ElevatorLight").transform.SetParent(myElevator.transform);
		floor8impostorDoor = GameObject.Find("WTTG2_AB_Elevator/Door");
		floor8impostorDoor.transform.SetParent(GameObject.Find("ApartmentStructure/Static/Misc").transform);
		floor8impostorDoor.SetActive(value: false);
		floor3impostorDoor = GameObject.Find("WTTG2_AB_EmptyFloor4/Door");
		floor5impostorDoor = GameObject.Find("WTTG2_AB_EmptyFloor3/Door");
		floor6impostorDoor = GameObject.Find("WTTG2_AB_EmptyFloor2/Door");
		floor10impostorDoor = GameObject.Find("WTTG2_AB_EmptyFloor1/Door");
		floor1impostorDoor = GameObject.Find("WTTG2_AB_lobbyHallWay/ElevatorDoor");
		GameObject.Find("WTTG2_AB_Elevator").SetActive(value: false);
		if (DifficultyManager.CasualMode)
		{
			myDoor.transform.localPosition = new Vector3(0.982f, -1.281f, 0.702f);
			Floor8Hallway = GameObject.Find("WTTG2_AB_mainHall");
			Floor10Hallway = GameObject.Find("WTTG2_AB_EmptyFloor1");
			Floor6Hallway = GameObject.Find("WTTG2_AB_EmptyFloor2");
			Floor5Hallway = GameObject.Find("WTTG2_AB_EmptyFloor3");
			Floor3Hallway = GameObject.Find("WTTG2_AB_EmptyFloor4");
			Floor1Hallway = GameObject.Find("WTTG2_AB_lobbyHallWay");
		}
		Debug.Log("[Custom Elevator] Initiated");
	}

	private void Start()
	{
		if (DifficultyManager.CasualMode)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(GameObject.Find("BillBoard4"));
			gameObject.transform.position = new Vector3(-27.351f, 40.9568f, -7f);
			gameObject.transform.localScale = new Vector3(2f, 2f, 2f);
			gameObject.GetComponent<CloseUpTrigger>().SetKeypad(new Vector3(-27.26f, 40.9568f, -6.5f), unknown: true);
			myClose = gameObject.GetComponent<CloseUpTrigger>();
			canvasRef = UnityEngine.Object.Instantiate(CustomObjectLookUp.ElevatorCanvas);
			canvasRef.SetActive(value: false);
			UnityEngine.Object.Destroy(myElevator.GetComponentInChildren<InteractiveLight>());
			myElevator.GetComponentInChildren<Light>().intensity = 1.5f;
			myElevator.GetComponentInChildren<Light>().enabled = true;
			originalPosition = myElevator.transform.position;
		}
	}

	public void StartElevatorSequence(int from, int to)
	{
		Elevating = true;
		GroundMe();
		GameManager.TimeSlinger.FireTimer(0.5f, GoFromTo, from, to);
	}

	private void GroundMe()
	{
		iAmRiding = true;
		ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).transform.SetParent(myElevator.transform);
		ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).transform.DOLocalMove(elevatorRoamVector, 0.25f).OnComplete(delegate
		{
			roamController.Ins.SetLock(setLock: true);
			PauseManager.LockPause();
		});
	}

	private void FixedUpdate()
	{
		if (DOOM3D)
		{
			float x = (Mathf.PerlinNoise(Time.time * 20f, 0f) - 0.5f) * 0.35f;
			float z = (Mathf.PerlinNoise(0f, Time.time * 20f) - 0.5f) * 0.35f;
			myElevator.transform.position = originalPosition + new Vector3(x, 0f, z);
		}
	}

	private void Update()
	{
		if (!iAmRiding)
		{
			return;
		}
		if (!glassDarkened)
		{
			if ((myElevator.transform.position.y <= 50.2781f && myElevator.transform.position.y >= 41.8156f) || (myElevator.transform.position.y <= 38.9881f && myElevator.transform.position.y >= 30.515602f) || (myElevator.transform.position.y <= 27.791f && myElevator.transform.position.y >= 24.8455f) || (myElevator.transform.position.y <= 22.0181f && myElevator.transform.position.y >= 13.535603f) || (myElevator.transform.position.y <= 11.774f && myElevator.transform.position.y >= 2.2356f))
			{
				glassDarkened = true;
				myDoor.transform.Find("Glass/Glass_0").GetComponent<MeshRenderer>().material.color = Color.black;
				Floor10Hallway.SetActive(value: false);
				Floor8Hallway.SetActive(value: false);
				Floor6Hallway.SetActive(value: false);
				Floor5Hallway.SetActive(value: false);
				Floor3Hallway.SetActive(value: false);
				Floor1Hallway.SetActive(value: false);
			}
		}
		else if ((!(myElevator.transform.position.y <= 50.2781f) || !(myElevator.transform.position.y >= 41.8156f)) && (!(myElevator.transform.position.y <= 38.9881f) || !(myElevator.transform.position.y >= 30.515602f)) && (!(myElevator.transform.position.y <= 27.791f) || !(myElevator.transform.position.y >= 24.8455f)) && (!(myElevator.transform.position.y <= 22.0181f) || !(myElevator.transform.position.y >= 13.535603f)) && (!(myElevator.transform.position.y <= 11.774f) || !(myElevator.transform.position.y >= 2.2356f)))
		{
			glassDarkened = false;
			myDoor.transform.Find("Glass/Glass_0").GetComponent<MeshRenderer>().material.DOColor(new Color(1f, 1f, 1f, 0.2f), 0.3f);
			Floor10Hallway.SetActive(value: true);
			Floor8Hallway.SetActive(value: true);
			Floor6Hallway.SetActive(value: true);
			Floor5Hallway.SetActive(value: true);
			Floor3Hallway.SetActive(value: true);
			Floor1Hallway.SetActive(value: true);
		}
	}

	private void UngroundMe()
	{
		ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).transform.SetParent(null);
		roamController.Ins.SetLock(setLock: false);
		PauseManager.UnLockPause();
		originalPosition = myElevator.transform.position;
		iAmRiding = false;
	}

	private void GoFromTo(int from, int to)
	{
		GameObject oldImpostorDoor = null;
		switch (from)
		{
		case 10:
			oldImpostorDoor = floor10impostorDoor;
			break;
		case 8:
			oldImpostorDoor = floor8impostorDoor;
			break;
		case 6:
			oldImpostorDoor = floor6impostorDoor;
			break;
		case 5:
			oldImpostorDoor = floor5impostorDoor;
			break;
		case 3:
			oldImpostorDoor = floor3impostorDoor;
			break;
		case 1:
			oldImpostorDoor = floor1impostorDoor;
			break;
		}
		CloseMyDoor();
		switch (to)
		{
		case 10:
			floor10impostorDoor.SetActive(value: false);
			myClose.getInteractionHook.transform.position = new Vector3(-27.351f, 52.2468f, -7f);
			myClose.setNewCameraPos(new Vector3(-27.26f, 52.2468f, -6.5f));
			MoveMeSlowly(52.2248f, oldImpostorDoor);
			break;
		case 8:
			floor8impostorDoor.SetActive(value: false);
			myClose.getInteractionHook.transform.position = new Vector3(-27.351f, 40.9568f, -7f);
			myClose.setNewCameraPos(new Vector3(-27.26f, 40.9568f, -6.5f));
			MoveMeSlowly(40.9348f, oldImpostorDoor);
			break;
		case 6:
			floor6impostorDoor.SetActive(value: false);
			myClose.getInteractionHook.transform.position = new Vector3(-27.351f, 29.6568f, -7f);
			myClose.setNewCameraPos(new Vector3(-27.26f, 29.6568f, -6.5f));
			MoveMeSlowly(29.6348f, oldImpostorDoor);
			break;
		case 5:
			floor5impostorDoor.SetActive(value: false);
			myClose.getInteractionHook.transform.position = new Vector3(-27.351f, 23.9868f, -7f);
			myClose.setNewCameraPos(new Vector3(-27.26f, 23.9868f, -6.5f));
			MoveMeSlowly(23.9648f, oldImpostorDoor);
			break;
		case 3:
			floor3impostorDoor.SetActive(value: false);
			myClose.getInteractionHook.transform.position = new Vector3(-27.351f, 12.676801f, -7f);
			myClose.setNewCameraPos(new Vector3(-27.26f, 12.676801f, -6.5f));
			MoveMeSlowly(12.6548f, oldImpostorDoor);
			break;
		case 1:
			floor1impostorDoor.SetActive(value: false);
			myClose.getInteractionHook.transform.position = new Vector3(-27.351f, 1.3768f, -7f);
			myClose.setNewCameraPos(new Vector3(-27.26f, 1.3768f, -6.5f));
			MoveMeSlowly(1.3548f, oldImpostorDoor);
			break;
		}
		WhereIAm = to;
	}

	private void GoFromToNotRiding(int from, int to)
	{
		GameObject oldImpostorDoor = null;
		switch (from)
		{
		case 10:
			oldImpostorDoor = floor10impostorDoor;
			break;
		case 8:
			oldImpostorDoor = floor8impostorDoor;
			break;
		case 6:
			oldImpostorDoor = floor6impostorDoor;
			break;
		case 5:
			oldImpostorDoor = floor5impostorDoor;
			break;
		case 3:
			oldImpostorDoor = floor3impostorDoor;
			break;
		case 1:
			oldImpostorDoor = floor1impostorDoor;
			break;
		}
		CloseMyDoor2();
		switch (to)
		{
		case 10:
			myClose.getInteractionHook.transform.position = new Vector3(-27.351f, 52.2468f, -7f);
			myClose.setNewCameraPos(new Vector3(-27.26f, 52.2468f, -6.5f));
			MoveMeSlowly(52.2248f, oldImpostorDoor, floor10impostorDoor);
			break;
		case 8:
			myClose.getInteractionHook.transform.position = new Vector3(-27.351f, 40.9568f, -7f);
			myClose.setNewCameraPos(new Vector3(-27.26f, 40.9568f, -6.5f));
			MoveMeSlowly(40.9348f, oldImpostorDoor, floor8impostorDoor);
			break;
		case 6:
			myClose.getInteractionHook.transform.position = new Vector3(-27.351f, 29.6568f, -7f);
			myClose.setNewCameraPos(new Vector3(-27.26f, 29.6568f, -6.5f));
			MoveMeSlowly(29.6348f, oldImpostorDoor, floor6impostorDoor);
			break;
		case 5:
			myClose.getInteractionHook.transform.position = new Vector3(-27.351f, 23.9868f, -7f);
			myClose.setNewCameraPos(new Vector3(-27.26f, 23.9868f, -6.5f));
			MoveMeSlowly(23.9648f, oldImpostorDoor, floor5impostorDoor);
			break;
		case 3:
			myClose.getInteractionHook.transform.position = new Vector3(-27.351f, 12.676801f, -7f);
			myClose.setNewCameraPos(new Vector3(-27.26f, 12.676801f, -6.5f));
			MoveMeSlowly(12.6548f, oldImpostorDoor, floor3impostorDoor);
			break;
		case 1:
			myClose.getInteractionHook.transform.position = new Vector3(-27.351f, 1.3768f, -7f);
			myClose.setNewCameraPos(new Vector3(-27.26f, 1.3768f, -6.5f));
			MoveMeSlowly(1.3548f, oldImpostorDoor, floor1impostorDoor);
			break;
		}
		WhereIAm = to;
	}

	public void MoveMe(float newHeight)
	{
		Vector3 position = myElevator.transform.position;
		myElevator.transform.position = new Vector3(position.x, newHeight, position.z);
	}

	private void MoveMeSlowly(float newHeight, GameObject oldImpostorDoor)
	{
		float num = Mathf.Abs(newHeight - myElevator.transform.position.y);
		float duration = num / 1.7f;
		myElevator.transform.DOMoveY(newHeight, duration).SetEase(Ease.InOutQuad).OnComplete(delegate
		{
			oldImpostorDoor.SetActive(value: true);
			GameManager.TimeSlinger.FireTimer(0.8f, delegate
			{
				UngroundMe();
				OpenMyDoor();
				GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.elevatorDing);
				Elevating = false;
				myClose.getInteractionHook.ForceLock = false;
			});
		});
	}

	private void MoveMeSlowly(float newHeight, GameObject oldImpostorDoor, GameObject newImpostorDoor)
	{
		float num = Mathf.Abs(newHeight - myElevator.transform.position.y);
		float duration = num / 1.7f;
		myElevator.transform.DOMoveY(newHeight, duration).SetEase(Ease.InOutQuad).OnComplete(delegate
		{
			oldImpostorDoor.SetActive(value: true);
			newImpostorDoor.SetActive(value: false);
			GameManager.TimeSlinger.FireTimer(0.8f, delegate
			{
				myDoor.SetActive(value: true);
				OpenMyDoor();
				GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.elevatorDing);
				myClose.getInteractionHook.ForceLock = false;
			});
		});
	}

	private void OnDestroy()
	{
		Ins = null;
	}

	public void OpenMyDoor()
	{
		myDoor.transform.DOLocalRotate(new Vector3(0f, -90f, 0f), 0.5f);
	}

	public void CloseMyDoor()
	{
		myDoor.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.2f);
	}

	public void CloseMyDoor2()
	{
		myDoor.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.2f).OnComplete(delegate
		{
			myDoor.SetActive(value: false);
		});
	}

	public void MoveMeToFloor(int floor)
	{
		floor8impostorDoor.SetActive(value: true);
		floor3impostorDoor.SetActive(value: true);
		floor5impostorDoor.SetActive(value: true);
		floor6impostorDoor.SetActive(value: true);
		floor10impostorDoor.SetActive(value: true);
		floor1impostorDoor.SetActive(value: true);
		switch (floor)
		{
		case 8:
			MoveMe(40.9348f);
			floor8impostorDoor.SetActive(value: false);
			break;
		case 10:
			MoveMe(52.2248f);
			floor10impostorDoor.SetActive(value: false);
			break;
		case 6:
			MoveMe(29.6348f);
			floor6impostorDoor.SetActive(value: false);
			break;
		case 5:
			MoveMe(23.9648f);
			floor5impostorDoor.SetActive(value: false);
			break;
		case 3:
			MoveMe(12.6548f);
			floor3impostorDoor.SetActive(value: false);
			break;
		case 1:
			MoveMe(1.3548f);
			floor1impostorDoor.SetActive(value: false);
			break;
		case 2:
		case 4:
		case 7:
		case 9:
			break;
		}
	}

	public void ButtonPressed(int id)
	{
		if (UnityEngine.Random.Range(0, 100) == 0)
		{
			id = 1337;
		}
		myClose.getInteractionHook.ForceLock = true;
		int num = id;
		int num2 = num;
		if (num2 == 1337)
		{
			KillDSAS = true;
			UnityEngine.Object.Destroy(GameObject.Find("Dead Signal Audio Source"));
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.elevatorBreak);
			myClose.MeLeaveClose();
			CloseMyDoor();
			GameManager.TimeSlinger.FireTimer(0.3f, delegate
			{
				DOOM3D = true;
			});
			myElevator.GetComponentInChildren<Light>().DOColor(new Color(1f, 0.2f, 0f, 0.6f), 0.5f);
			GameManager.TimeSlinger.FireTimer(1.75f, delegate
			{
				myElevator.GetComponentInChildren<Light>().enabled = false;
			});
			GameManager.TimeSlinger.FireTimer(2f, UIManager.TriggerHardGameOver, "DEAD");
		}
		else if (WhereIAm != id)
		{
			myClose.MeLeaveClose();
			GameManager.AudioSlinger.PlaySoundWithCustomDelay(CustomSoundLookUp.elevatorStart, 1f);
			GameManager.TimeSlinger.FireTimer(1.25f, StartElevatorSequence, WhereIAm, id);
		}
	}

	public void CallElevator(int flor)
	{
		GoFromToNotRiding(WhereIAm, flor);
	}
}
