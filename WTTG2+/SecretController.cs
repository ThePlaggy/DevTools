using UnityEngine;

public class SecretController : moveableController
{
	public static SecretController Ins;

	protected new void Awake()
	{
		Ins = this;
		base.Awake();
		ControllerManager.Add(this);
	}

	protected new void Start()
	{
		base.Start();
	}

	protected new void Update()
	{
		if (StateManager.GameState == GAME_STATE.PAUSED)
		{
			if (!lockControl)
			{
				SetMasterLock(setLock: true);
			}
			if (Input.GetKeyDown(KeyCode.Q))
			{
				Application.Quit();
			}
		}
		else if (lockControl)
		{
			SetMasterLock(setLock: false);
		}
		base.Update();
	}

	protected new void OnDestroy()
	{
		ControllerManager.Remove(Controller);
		base.OnDestroy();
	}

	public void Release()
	{
		MyCamera.transform.SetParent(MouseRotatingObject.transform);
		MyCamera.transform.localPosition = DefaultCameraPOS;
		MyCamera.transform.localRotation = Quaternion.Euler(Vector3.zero);
		if (!MouseCaptureInit)
		{
			Init();
		}
		Active = true;
		MyState = GAME_CONTROLLER_STATE.IDLE;
		SetMasterLock(setLock: false);
	}
}
