using UnityEngine;

public class DeBugger : MonoBehaviour
{
	public GAME_STATE CurrentGameState;

	public PLAYER_STATE CurrentPlayerState;

	public PLAYER_LOCATION CurrentPlayerLocation;

	public ENEMY_STATE CurrentEnemyState;

	public bool BeingHacked;

	public GAME_CONTROLLER_STATE CurrentRoamControllerState;

	public GAME_CONTROLLER_STATE CurrentDeskControllerState;

	public GAME_CONTROLLER_STATE CurrentComputerControllerState;

	public GAME_CONTROLLER_STATE CurrentHideControllerState;

	public bool SaveLocked;

	public Vector3 ReSpawnLocation = Vector3.zero;

	public Vector3 LobbyLocation = Vector3.zero;

	public Vector3 DeadDropLocation = Vector3.zero;

	private bool gameIsLive;

	private static int DebugKeys;

	public static bool _getDebugKeys => DebugKeys > 0;

	private void Awake()
	{
		GameManager.StageManager.TheGameIsLive += gameLiveDebug;
	}

	public void Update()
	{
		CurrentGameState = StateManager.GameState;
		CurrentPlayerState = StateManager.PlayerState;
		CurrentPlayerLocation = StateManager.PlayerLocation;
		BeingHacked = StateManager.BeingHacked;
		SaveLocked = DataManager.LockSave;
		if (gameIsLive)
		{
			CurrentRoamControllerState = ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).MyState;
			CurrentDeskControllerState = ControllerManager.Get<deskController>(GAME_CONTROLLER.DESK).MyState;
			CurrentComputerControllerState = ControllerManager.Get<computerController>(GAME_CONTROLLER.COMPUTER).MyState;
			CurrentHideControllerState = ControllerManager.Get<hideController>(GAME_CONTROLLER.HIDE).MyState;
		}
	}

	private void OnDestroy()
	{
	}

	public void ClearGameData()
	{
	}

	public void ClearOptionData()
	{
	}

	public void SpawnRoom()
	{
		ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).transform.position = ReSpawnLocation;
	}

	public void SpawnLobby()
	{
		ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).transform.position = LobbyLocation;
	}

	public void SpawnDeadDrop()
	{
		ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).transform.position = DeadDropLocation;
	}

	private void gameLiveDebug()
	{
		gameIsLive = true;
	}
}
