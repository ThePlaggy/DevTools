using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class RemoteVPNPlacementBehaviour : MonoBehaviour
{
	[SerializeField]
	private GameObject remoteVPNObject;

	private bool canPlaceRemote;

	private RemoteVPNObject currentRemoteVPNBeingPlaced;

	private bool gameIsLive;

	private bool gameIsPaused;

	private MeshRenderer remoteVPNObjectMeshRenderer;

	private RemoteVPNPlacementPreview remoteVPNPlacementBeh;

	[NonSerialized]
	public readonly Color orangeColor = new Color(1f, 0.64f, 0f);

	[NonSerialized]
	public readonly Color limeColor = new Color(0.75f, 1f, 0f);

	private void Awake()
	{
		remoteVPNObjectMeshRenderer = remoteVPNObject.GetComponent<MeshRenderer>();
		remoteVPNObjectMeshRenderer.enabled = false;
		remoteVPNPlacementBeh = remoteVPNObject.GetComponent<RemoteVPNPlacementPreview>();
		GameManager.StageManager.Stage += stageMe;
		GameManager.StageManager.TheGameIsLive += theGameIsLive;
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
		if (StateManager.PlayerState == PLAYER_STATE.REMOTE_VPN_PLACEMENT)
		{
			if (!Physics.Raycast(base.transform.position, base.transform.forward, out var hitInfo, 1f))
			{
				remoteVPNObject.transform.position = Vector3.zero;
				canPlaceRemote = false;
			}
			else if (hitInfo.collider != null && hitInfo.collider.GetComponent<StickySurface>() != null)
			{
				float vPNValues = GameManager.WorldManager.GetVPNValues(remoteVPNObject.transform);
				if (vPNValues.Equals(2500f))
				{
					remoteVPNPlacementBeh.ChangePreview(new Color(0f, 0f, 0f, 0f), LookUp.PlayerUI.RemoteVPNZeroBar);
				}
				else if (vPNValues > 2700f)
				{
					if (RemoteVPNManager.GodSpot)
					{
						remoteVPNPlacementBeh.ChangePreview(Color.blue, LookUp.PlayerUI.RemoteVPNTwoBar);
					}
					else
					{
						remoteVPNPlacementBeh.ChangePreview(Color.red, LookUp.PlayerUI.RemoteVPNZeroBar);
					}
				}
				else if (vPNValues >= 1000f && vPNValues < 2700f)
				{
					remoteVPNPlacementBeh.ChangePreview(Color.red, LookUp.PlayerUI.RemoteVPNZeroBar);
				}
				else if (vPNValues < 1000f && vPNValues > 600f)
				{
					remoteVPNPlacementBeh.ChangePreview(orangeColor, LookUp.PlayerUI.RemoteVPNZeroBar);
				}
				else if (vPNValues < 600f && vPNValues > 300f)
				{
					remoteVPNPlacementBeh.ChangePreview(Color.yellow, LookUp.PlayerUI.RemoteVPNOneBar);
				}
				else if (vPNValues < 300f && vPNValues > 150f)
				{
					remoteVPNPlacementBeh.ChangePreview(limeColor, LookUp.PlayerUI.RemoteVPNOneBar);
				}
				else
				{
					remoteVPNPlacementBeh.ChangePreview(Color.green, LookUp.PlayerUI.RemoteVPNTwoBar);
				}
				remoteVPNObject.transform.position = hitInfo.point;
				remoteVPNObject.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
				canPlaceRemote = true;
			}
			else
			{
				remoteVPNObject.transform.position = Vector3.zero;
				canPlaceRemote = false;
			}
		}
		else
		{
			canPlaceRemote = false;
		}
	}

	private void OnDestroy()
	{
		GameManager.PauseManager.GamePaused -= playerPausedGame;
		GameManager.PauseManager.GameUnPaused -= playerUnPausedGame;
	}

	private void takeInput()
	{
		if (!gameIsPaused && canPlaceRemote && CrossPlatformInputManager.GetButtonDown("LeftClick") && !GameManager.WorldManager.GetVPNValues(remoteVPNObject.transform).Equals(2500f))
		{
			placeRemoteVPN();
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

	private void triggerPlacementMode(RemoteVPNObject TheRemoteVPN)
	{
		currentRemoteVPNBeingPlaced = TheRemoteVPN;
		remoteVPNObjectMeshRenderer.enabled = true;
	}

	private void placeRemoteVPN()
	{
		if (currentRemoteVPNBeingPlaced != null)
		{
			currentRemoteVPNBeingPlaced.PlaceMe(remoteVPNObject.transform.position, remoteVPNObject.transform.rotation.eulerAngles);
			remoteVPNObjectMeshRenderer.enabled = false;
		}
	}

	private void stageMe()
	{
		GameManager.StageManager.Stage -= stageMe;
		GameManager.ManagerSlinger.RemoteVPNManager.EnteredPlacementMode += triggerPlacementMode;
	}

	private void theGameIsLive()
	{
		gameIsLive = true;
		GameManager.StageManager.TheGameIsLive -= theGameIsLive;
	}
}
