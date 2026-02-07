public static class StateManager
{
	public static CustomEvent PlayerStateChangeEvents = new CustomEvent(5);

	public static CustomEvent PlayerLocationChangeEvents = new CustomEvent(5);

	private static GAME_STATE _gameState;

	private static PLAYER_STATE _playerState;

	private static PLAYER_LOCATION _playerLocation;

	private static bool _beingHacked;

	public static GAME_STATE GameState
	{
		get
		{
			return _gameState;
		}
		set
		{
			_gameState = value;
		}
	}

	public static PLAYER_STATE PlayerState
	{
		get
		{
			return _playerState;
		}
		set
		{
			_playerState = value;
			PlayerStateChangeEvents.Execute();
		}
	}

	public static PLAYER_LOCATION PlayerLocation
	{
		get
		{
			return _playerLocation;
		}
		set
		{
			_playerLocation = value;
			PlayerLocationChangeEvents.Execute();
		}
	}

	public static bool BeingHacked
	{
		get
		{
			return _beingHacked || TarotManager.InDenial;
		}
		set
		{
			_beingHacked = value;
		}
	}

	public static void Clear()
	{
		PlayerStateChangeEvents.Clear();
		PlayerLocationChangeEvents.Clear();
		_gameState = GAME_STATE.LIVE;
		_playerState = PLAYER_STATE.BUSY;
		_playerLocation = PLAYER_LOCATION.UNKNOWN;
		_beingHacked = false;
	}
}
