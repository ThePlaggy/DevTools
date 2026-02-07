using UnityEngine;

public class CustomElevatorCanvas : MonoBehaviour
{
	public void ButtonAction(int id)
	{
		CustomElevatorManager.Ins.ButtonPressed(id);
	}
}
