using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class MotionSensorPlacementBehaviour : MonoBehaviour
{
	[SerializeField]
	private GameObject motionSensorObject;

	private bool canPlaceSensor;

	private MotionSensorObject currentMotionSensorBeingPlaced;

	private bool gameIsLive;

	private bool gameIsPaused;

	private MeshRenderer motionSensorObjectMeshRenderer;

	private void Awake()
	{
		motionSensorObjectMeshRenderer = motionSensorObject.GetComponent<MeshRenderer>();
		motionSensorObjectMeshRenderer.enabled = false;
		GameManager.StageManager.TheGameIsLive += gameIsNowLive;
	}

	private void Start()
	{
		GameManager.PauseManager.GamePaused += playerPausedGame;
		GameManager.PauseManager.GameUnPaused += playerUnPausedGame;
	}

	private void Update()
	{
		takeInput();
	}

	private void FixedUpdate()
	{
		if (!gameIsLive)
		{
			return;
		}
		if (StateManager.PlayerState == PLAYER_STATE.MOTION_SENSOR_PLACEMENT)
		{
			if (Physics.Raycast(base.transform.position, base.transform.forward, out var hitInfo, 1f))
			{
				bool flag = false;
				if (hitInfo.collider != null)
				{
					StickySurface component = hitInfo.collider.GetComponent<StickySurface>();
					if (component != null)
					{
						flag = true;
					}
				}
				if (flag)
				{
					switch (StateManager.PlayerLocation)
					{
					case PLAYER_LOCATION.HALL_WAY8:
						ChangeToGreen();
						break;
					case PLAYER_LOCATION.STAIR_WAY:
						ChangeToYellow();
						break;
					default:
						ChangeToRed();
						break;
					}
					motionSensorObject.transform.position = hitInfo.point;
					motionSensorObject.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
					canPlaceSensor = true;
				}
				else
				{
					motionSensorObject.transform.position = Vector3.zero;
					canPlaceSensor = false;
				}
			}
			else
			{
				motionSensorObject.transform.position = Vector3.zero;
				canPlaceSensor = false;
			}
		}
		else
		{
			canPlaceSensor = false;
		}
	}

	private void OnDestroy()
	{
		GameManager.PauseManager.GamePaused -= playerPausedGame;
		GameManager.PauseManager.GameUnPaused -= playerUnPausedGame;
	}

	private void takeInput()
	{
		if (!gameIsPaused && canPlaceSensor && CrossPlatformInputManager.GetButtonDown("LeftClick"))
		{
			placeMotionSensor();
		}
	}

	private void playerPausedGame()
	{
		gameIsPaused = true;
	}

	private void playerUnPausedGame()
	{
		gameIsPaused = false;
	}

	private void triggerPlacementMode(MotionSensorObject TheMotionSensor)
	{
		currentMotionSensorBeingPlaced = TheMotionSensor;
		motionSensorObjectMeshRenderer.enabled = true;
	}

	private void placeMotionSensor()
	{
		if (currentMotionSensorBeingPlaced != null)
		{
			currentMotionSensorBeingPlaced.PlaceMe(motionSensorObject.transform.position, motionSensorObject.transform.rotation.eulerAngles);
			motionSensorObjectMeshRenderer.enabled = false;
		}
	}

	private void gameIsNowLive()
	{
		gameIsLive = true;
		GameManager.StageManager.TheGameIsLive -= gameIsNowLive;
		GameManager.ManagerSlinger.MotionSensorManager.EnteredPlacementMode += triggerPlacementMode;
	}

	private void ChangeToRed()
	{
		motionSensorObjectMeshRenderer.material.SetColor("_EmissionColor", Color.red);
	}

	private void ChangeToYellow()
	{
		motionSensorObjectMeshRenderer.material.SetColor("_EmissionColor", Color.yellow);
	}

	private void ChangeToGreen()
	{
		motionSensorObjectMeshRenderer.material.SetColor("_EmissionColor", Color.green);
	}
}
