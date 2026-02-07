using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PauseManager
{
	public delegate void PauseEvents();

	private static bool _lockPause;

	public bool Paused { get; private set; }

	public event PauseEvents GamePaused;

	public event PauseEvents GameUnPaused;

	public static void LockPause()
	{
		_lockPause = true;
	}

	public static void UnLockPause()
	{
		_lockPause = DifficultyManager.HackerMode;
	}

	public void Update()
	{
		if (_lockPause || !CrossPlatformInputManager.GetButtonDown("Cancel"))
		{
			return;
		}
		if (Paused)
		{
			Paused = false;
			StateManager.GameState = GAME_STATE.LIVE;
			if (this.GameUnPaused != null)
			{
				this.GameUnPaused();
			}
			Time.timeScale = 1f;
		}
		else
		{
			Paused = true;
			StateManager.GameState = GAME_STATE.PAUSED;
			if (this.GamePaused != null)
			{
				this.GamePaused();
			}
			Time.timeScale = 0f;
		}
	}
}
