using UnityEngine;

public class CameraHook : MonoBehaviour
{
	public CAMERA_ID MyCameraID;

	public Transform GlobalParent;

	private Camera MyCamera;

	private CameraHookData myData;

	private int myID;

	public Transform CurrentParent { get; private set; }

	private void Awake()
	{
		MyCamera = GetComponent<Camera>();
		myID = (int)MyCameraID;
		CameraManager.Add(MyCameraID, GetComponent<Camera>());
		if (GameManager.StageManager != null)
		{
			GameManager.StageManager.Stage += StageMe;
			GameManager.StageManager.TheGameIsLive += gameLive;
		}
	}

	private void OnDestroy()
	{
		CancelInvoke("updateMyData");
		CameraManager.Remove(MyCameraID);
	}

	public void SetMyParent(Transform SetParent)
	{
		CurrentParent = SetParent;
		base.transform.SetParent(SetParent);
	}

	public void SwitchToGlobalParent()
	{
		base.transform.SetParent(GlobalParent);
	}

	public void ManualPushDataUpdate()
	{
		myData.POSX = base.transform.localPosition.x;
		myData.POSY = base.transform.localPosition.y;
		myData.POSZ = base.transform.localPosition.z;
		myData.ROTX = base.transform.localRotation.eulerAngles.x;
		myData.ROTY = base.transform.localRotation.eulerAngles.y;
		myData.ROTZ = base.transform.localRotation.eulerAngles.z;
		myData.FOV = MyCamera.fieldOfView;
	}

	private void updateMyData()
	{
		if (StateManager.PlayerState != PLAYER_STATE.BUSY)
		{
			myData.POSX = base.transform.localPosition.x;
			myData.POSY = base.transform.localPosition.y;
			myData.POSZ = base.transform.localPosition.z;
			myData.ROTX = base.transform.localRotation.eulerAngles.x;
			myData.ROTY = base.transform.localRotation.eulerAngles.y;
			myData.ROTZ = base.transform.localRotation.eulerAngles.z;
			myData.FOV = MyCamera.fieldOfView;
			DataManager.Save(myData);
		}
	}

	private void StageMe()
	{
		myData = DataManager.Load<CameraHookData>(myID);
		if (myData == null)
		{
			myData = new CameraHookData(myID);
			myData.POSX = base.transform.localPosition.x;
			myData.POSY = base.transform.localPosition.y;
			myData.POSZ = base.transform.localPosition.z;
			myData.ROTX = base.transform.localRotation.eulerAngles.x;
			myData.ROTY = base.transform.localRotation.eulerAngles.y;
			myData.ROTZ = base.transform.localRotation.eulerAngles.z;
			myData.FOV = MyCamera.fieldOfView;
		}
		GameManager.StageManager.Stage -= StageMe;
	}

	private void gameLive()
	{
		base.transform.localPosition = new Vector3(myData.POSX, myData.POSY, myData.POSZ);
		base.transform.localRotation = Quaternion.Euler(new Vector3(myData.ROTX, myData.ROTY, myData.ROTZ));
		MyCamera.fieldOfView = myData.FOV;
		InvokeRepeating("updateMyData", 0f, 5f);
		GameManager.StageManager.TheGameIsLive -= gameLive;
	}
}
