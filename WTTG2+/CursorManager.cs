using UnityEngine;

public class CursorManager : MonoBehaviour
{
	public static CursorManager Ins;

	[SerializeField]
	private Texture2D defaultCursor;

	[SerializeField]
	private Texture2D resizeCursor;

	[SerializeField]
	private Texture2D pointerCursor;

	[SerializeField]
	private Texture2D hackerCursor;

	[SerializeField]
	private bool skipGameManager;

	public bool hideCursorOnStart;

	public bool cursorIsDisabled;

	public bool overwrite;

	public bool hackerCursorActive;

	private void Awake()
	{
		Ins = this;
		if (!skipGameManager)
		{
			GameManager.ManagerSlinger.CursorManager = this;
		}
		if (hideCursorOnStart)
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			cursorIsDisabled = true;
			overwrite = false;
		}
		else
		{
			SetOverwrite(setValue: true);
			EnableCursor();
		}
	}

	private void Update()
	{
		if (!overwrite && !skipGameManager)
		{
			if (StateManager.GameState == GAME_STATE.PAUSED)
			{
				EnableCursor();
			}
			else
			{
				DisableCursor();
			}
		}
	}

	private void OnDestroy()
	{
		Ins = null;
	}

	public void SetOverwrite(bool setValue)
	{
		overwrite = setValue;
		if (overwrite)
		{
			EnableCursor();
		}
		else
		{
			DisableCursor();
		}
	}

	public void EnableCursor()
	{
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		cursorIsDisabled = false;
	}

	public void DisableCursor()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		cursorIsDisabled = true;
	}

	public void SwitchToDefaultCursor()
	{
		if (!DifficultyManager.HackerMode)
		{
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
	}

	public void SwitchToCustomCursor()
	{
		if (!hackerCursorActive)
		{
			Cursor.SetCursor(defaultCursor, new Vector2(12f, 12f), CursorMode.Auto);
		}
		else
		{
			SwitchToHackerCursor();
		}
	}

	public void SwitchToHackerCursor()
	{
		hackerCursorActive = true;
		Cursor.SetCursor(hackerCursor, new Vector2(16f, 16f), CursorMode.Auto);
	}

	public void ClearHackerCursor()
	{
		hackerCursorActive = false;
		SwitchToCustomCursor();
	}

	public void ResizeCursorState(bool active)
	{
		if (active)
		{
			Cursor.SetCursor(resizeCursor, new Vector2(12f, 12f), CursorMode.Auto);
		}
		else
		{
			SwitchToCustomCursor();
		}
	}

	public void PointerCursorState(bool active)
	{
		if (active)
		{
			Cursor.SetCursor(pointerCursor, new Vector2(12f, 12f), CursorMode.Auto);
		}
		else
		{
			SwitchToCustomCursor();
		}
	}
}
