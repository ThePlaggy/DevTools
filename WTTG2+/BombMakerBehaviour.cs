using UnityEngine;

public class BombMakerBehaviour : MonoBehaviour
{
	public static BombMakerBehaviour Ins;

	private BombMakerMainDoorJump BMAj;

	private BombMakerFloor10Jump BMF10j;

	private BombMakerFloor1Jump BMF1j;

	private BombMakerFloor3Jump BMF3j;

	private BombMakerFloor5Jump BMF5j;

	private BombMakerFloor6Jump BMF6j;

	private BombMakerFloor8Jump BMF8j;

	private void Awake()
	{
		Ins = this;
		BMF10j = new BombMakerFloor10Jump();
		BMF8j = new BombMakerFloor8Jump();
		BMF6j = new BombMakerFloor6Jump();
		BMF5j = new BombMakerFloor5Jump();
		BMF3j = new BombMakerFloor3Jump();
		BMF1j = new BombMakerFloor1Jump();
		BMAj = new BombMakerMainDoorJump();
	}

	private void stageFloor10Jump()
	{
		LookUp.Doors.Door10.DoorOpenEvent.AddListener(BMF10j.Stage);
		LookUp.Doors.Door10.DoorWasOpenedEvent.AddListener(BMF10j.Execute);
	}

	public void stageFloor8Jump()
	{
		LookUp.Doors.Door8.DoorOpenEvent.AddListener(BMF8j.Stage);
		LookUp.Doors.Door8.DoorWasOpenedEvent.AddListener(BMF8j.Execute);
	}

	private void stageFloor6Jump()
	{
		LookUp.Doors.Door6.DoorOpenEvent.AddListener(BMF6j.Stage);
		LookUp.Doors.Door6.DoorWasOpenedEvent.AddListener(BMF6j.Execute);
	}

	private void stageFloor5Jump()
	{
		LookUp.Doors.Door5.DoorOpenEvent.AddListener(BMF5j.Stage);
		LookUp.Doors.Door5.DoorWasOpenedEvent.AddListener(BMF5j.Execute);
	}

	private void stageFloor3Jump()
	{
		LookUp.Doors.Door3.DoorOpenEvent.AddListener(BMF3j.Stage);
		LookUp.Doors.Door3.DoorWasOpenedEvent.AddListener(BMF3j.Execute);
	}

	public void stageFloor1Jump()
	{
		LookUp.Doors.Door1.DoorOpenEvent.AddListener(BMF1j.Stage);
		LookUp.Doors.Door1.DoorWasOpenedEvent.AddListener(BMF1j.Execute);
	}

	public void StageBombMakerOutsideKill()
	{
		if (StateManager.PlayerLocation == PLAYER_LOCATION.DEAD_DROP || StateManager.PlayerLocation == PLAYER_LOCATION.DEAD_DROP_ROOM || StateManager.PlayerLocation == PLAYER_LOCATION.LOBBY || StateManager.PlayerLocation == PLAYER_LOCATION.LOBBY_COMPUTER || StateManager.PlayerLocation == PLAYER_LOCATION.HALL_WAY1)
		{
			stageFloor1Jump();
			stageApartmentJump();
			return;
		}
		if (StateManager.PlayerLocation == PLAYER_LOCATION.HALL_WAY3)
		{
			stageFloor3Jump();
			stageApartmentJump();
			return;
		}
		if (StateManager.PlayerLocation == PLAYER_LOCATION.HALL_WAY5)
		{
			stageFloor5Jump();
			stageApartmentJump();
			return;
		}
		if (StateManager.PlayerLocation == PLAYER_LOCATION.HALL_WAY6)
		{
			stageFloor6Jump();
			stageApartmentJump();
			return;
		}
		if (StateManager.PlayerLocation == PLAYER_LOCATION.HALL_WAY8)
		{
			stageFloor8Jump();
			stageApartmentJump();
			return;
		}
		if (StateManager.PlayerLocation == PLAYER_LOCATION.HALL_WAY10)
		{
			stageFloor10Jump();
			stageApartmentJump();
			return;
		}
		if (StateManager.PlayerLocation == PLAYER_LOCATION.MAINTENANCE_ROOM || StateManager.PlayerLocation == PLAYER_LOCATION.STAIR_WAY)
		{
			stageApartmentJump();
		}
		if (StateManager.PlayerLocation == PLAYER_LOCATION.MAIN_ROON || StateManager.PlayerLocation == PLAYER_LOCATION.BATH_ROOM || StateManager.PlayerLocation == PLAYER_LOCATION.OUTSIDE)
		{
			stageFloor8Jump();
		}
	}

	public void stageApartmentJump()
	{
		LookUp.Doors.MainDoor.DoorOpenEvent.AddListener(BMAj.Stage);
		LookUp.Doors.MainDoor.DoorWasOpenedEvent.AddListener(BMAj.Execute);
	}
}
