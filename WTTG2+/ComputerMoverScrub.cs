using UnityEngine;

public class ComputerMoverScrub
{
	public enum COMPUTER_SIDE
	{
		LEFT,
		RIGHT
	}

	public ComputerMoverScrub(float x, float y, float z)
	{
		MoveMe(x, y, z, COMPUTER_SIDE.LEFT);
	}

	public ComputerMoverScrub()
	{
		RevertMe();
	}

	public ComputerMoverScrub(float x, float y, float z, COMPUTER_SIDE side)
	{
		MoveMe(x, y, z, side);
	}

	private void RevertMe()
	{
		int num = 3;
		int num2 = 40;
		int num3 = -3;
		GameObject gameObject = GameObject.Find("computerAudioHub");
		GameObject gameObject2 = GameObject.Find("computerScreen");
		GameObject gameObject3 = GameObject.Find("deskController");
		GameObject gameObject4 = GameObject.Find("switchToComputerController");
		GameObject gameObject5 = GameObject.Find("switchToDeskController");
		gameObject.transform.position = new Vector3((float)num + 0.951f, (float)num2 + 0.664f, (float)num3 - 0.7195f);
		gameObject2.transform.position = new Vector3((float)num + 0.9468f, (float)num2 + 0.8604f, (float)num3 - 0.7249f);
		gameObject3.transform.position = new Vector3((float)num + 0.175f, (float)num2 + 0.395f, (float)num3 - 0.752f);
		gameObject4.transform.position = new Vector3((float)num + 0.929f, (float)num2 + 0.8604f, (float)num3 - 0.7249f);
		gameObject5.transform.position = new Vector3((float)num + 0.927f, (float)num2 + 0.8604f, (float)num3 - 0.7249f);
		if (StateManager.PlayerState == PLAYER_STATE.COMPUTER)
		{
			ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).transform.position = new Vector3((float)num + 0.093f, (float)num2 + 0.556f, (float)num3 - 0.09f);
		}
		gameObject.GetComponent<VolumeAudioHubObject>().enabled = true;
	}

	private void MoveMe(float x, float y, float z, COMPUTER_SIDE side)
	{
		GameObject gameObject = GameObject.Find("computerAudioHub");
		GameObject gameObject2 = GameObject.Find("computerScreen");
		GameObject gameObject3 = GameObject.Find("deskController");
		GameObject gameObject4 = GameObject.Find("switchToComputerController");
		GameObject gameObject5 = GameObject.Find("switchToDeskController");
		gameObject.transform.position = new Vector3(x + 0.951f, y + 0.664f, z - 0.7195f);
		gameObject2.transform.position = new Vector3(x + 0.9468f, y + 0.8604f, z - 0.7249f);
		gameObject3.transform.position = new Vector3(x + 0.175f, y + 0.395f, z - 0.752f);
		gameObject4.transform.position = new Vector3(x + 0.929f, y + 0.8604f, z - 0.7249f);
		gameObject5.transform.position = new Vector3(x + 0.927f, y + 0.8604f, z - 0.7249f);
		gameObject.GetComponent<VolumeAudioHubObject>().enabled = false;
		if (side == COMPUTER_SIDE.LEFT)
		{
			if (StateManager.PlayerState == PLAYER_STATE.COMPUTER)
			{
				ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).transform.position = new Vector3(x + 0.093f, y + 0.556f, z - 0.09f);
			}
		}
		else if (StateManager.PlayerState == PLAYER_STATE.COMPUTER)
		{
			ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).transform.position = new Vector3(x + 0.093f, y + 0.556f, z - 1.39f);
		}
	}
}
