using UnityEngine;

public abstract class baseController : MonoBehaviour
{
	public GAME_CONTROLLER Controller;

	public CAMERA_ID CameraIControl;

	public GAME_CONTROLLER_STATE MyState;

	public bool Active;

	protected bool cameraIsSet;

	protected bool lockControl;

	protected bool lockMouse;

	protected bool masterLock;

	protected Camera MyCamera;

	private BaseControllerData myData;

	private int myID;

	protected bool overWriteLocks;

	protected CustomEvent PostLive = new CustomEvent(2);

	protected CustomEvent PostStage = new CustomEvent(2);

	protected virtual void Awake()
	{
		myID = (int)Controller;
		if (GameManager.StageManager != null)
		{
			GameManager.StageManager.Stage += stageMe;
			GameManager.StageManager.TheGameIsLive += gameLive;
		}
		if (CameraManager.Get(CameraIControl, out MyCamera))
		{
			cameraIsSet = true;
		}
		MyState = GAME_CONTROLLER_STATE.LOCKED;
		Active = false;
		SetMasterLock(setLock: true);
	}

	protected virtual void Start()
	{
	}

	protected virtual void Update()
	{
	}

	protected virtual void OnDestroy()
	{
		CancelInvoke("updateMyData");
		GameManager.PauseManager.GamePaused -= PlayerHitPause;
		GameManager.PauseManager.GameUnPaused -= PlayerHitUnPause;
	}

	public void SetMasterLock(bool setLock)
	{
		masterLock = setLock;
		lockControl = setLock;
		lockMouse = setLock;
		overWriteLocks = setLock;
		if (setLock)
		{
			MyState = GAME_CONTROLLER_STATE.LOCKED;
		}
	}

	public void SetLock(bool setLock)
	{
		lockControl = setLock;
	}

	private void PlayerHitPause()
	{
		if (!overWriteLocks)
		{
			lockControl = true;
			lockMouse = true;
			MyState = GAME_CONTROLLER_STATE.LOCKED;
		}
	}

	private void PlayerHitUnPause()
	{
		if (!overWriteLocks)
		{
			lockControl = false;
			lockMouse = false;
			MyState = GAME_CONTROLLER_STATE.IDLE;
		}
	}

	private void updateMyData()
	{
		if (myData != null && StateManager.PlayerState != PLAYER_STATE.BUSY)
		{
			myData.MyState = (int)MyState;
			myData.Active = Active;
			myData.POSX = base.transform.position.x;
			myData.POSY = base.transform.position.y;
			myData.POSZ = base.transform.position.z;
			myData.ROTX = base.transform.rotation.eulerAngles.x;
			myData.ROTY = base.transform.rotation.eulerAngles.y;
			myData.ROTZ = base.transform.rotation.eulerAngles.z;
			DataManager.Save(myData);
		}
	}

	private void gameStaging()
	{
	}

	private void stageMe()
	{
		myData = DataManager.Load<BaseControllerData>(myID);
		if (myData == null)
		{
			myData = new BaseControllerData(myID);
			myData.MyState = (int)MyState;
			if (Controller == GameManager.StageManager.DefaultController)
			{
				myData.Active = true;
			}
			else
			{
				myData.Active = false;
			}
			myData.POSX = base.transform.position.x;
			myData.POSY = base.transform.position.y;
			myData.POSZ = base.transform.position.z;
			myData.ROTX = base.transform.rotation.eulerAngles.x;
			myData.ROTY = base.transform.rotation.eulerAngles.y;
			myData.ROTZ = base.transform.rotation.eulerAngles.z;
		}
		if (!DataManager.ContinuedGame)
		{
			MyState = (GAME_CONTROLLER_STATE)myData.MyState;
			Active = myData.Active;
		}
		GameManager.StageManager.Stage -= stageMe;
		GameManager.PauseManager.GamePaused += PlayerHitPause;
		GameManager.PauseManager.GameUnPaused += PlayerHitUnPause;
		PostStage.Execute();
	}

	private void gameLive()
	{
		base.transform.position = new Vector3(myData.POSX, myData.POSY, myData.POSZ);
		base.transform.rotation = Quaternion.Euler(new Vector3(myData.ROTX, myData.ROTY, myData.ROTZ));
		GameManager.StageManager.TheGameIsLive -= gameLive;
		PostLive.Execute();
	}
}
