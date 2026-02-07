using UnityEngine;

public class mouseableController : baseController
{
	public GameObject MouseRotatingObject;

	public mouseCapture MyMouseCapture;

	[SerializeField]
	private bool useOptDataForMouseSens;

	protected bool MouseCaptureInit;

	private Options myOptData;

	protected new void Awake()
	{
		base.Awake();
		if (useOptDataForMouseSens)
		{
			int num = PlayerPrefs.GetInt("[GAME]MouseSens");
			MyMouseCapture.XSensitivity = num;
			MyMouseCapture.YSensitivity = num;
		}
		if (GameManager.StageManager != null)
		{
			GameManager.StageManager.Stage += stageMe;
		}
	}

	protected new void Start()
	{
		base.Start();
	}

	protected new void Update()
	{
		base.Update();
		RotateView();
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
	}

	protected void Init()
	{
		if (cameraIsSet)
		{
			MyMouseCapture.Init(MouseRotatingObject, MyCamera.gameObject);
			MouseCaptureInit = true;
		}
	}

	private void RotateView()
	{
		if (MouseCaptureInit && !lockMouse)
		{
			MyMouseCapture.LookRotation();
		}
	}

	private void playerUpdateMouseSens(int SetValue)
	{
		MyMouseCapture.XSensitivity = SetValue;
		MyMouseCapture.YSensitivity = SetValue;
	}

	private void stageMe()
	{
		GameManager.StageManager.Stage -= stageMe;
		if (useOptDataForMouseSens)
		{
			PauseMenuHook.Ins.UpdatedMouseSens.Event += playerUpdateMouseSens;
		}
	}
}
