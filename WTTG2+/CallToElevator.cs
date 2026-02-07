using UnityEngine;

public class CallToElevator : MonoBehaviour
{
	public int MyElevatorFloor;

	private InteractionHook myIH;

	private void Awake()
	{
		myIH = GetComponent<InteractionHook>();
		myIH.LeftClickAction += CallElevator;
	}

	private void OnDestroy()
	{
		myIH.LeftClickAction -= CallElevator;
	}

	private void CallElevator()
	{
		if (CustomElevatorManager.Ins.WhereIAm != MyElevatorFloor)
		{
			CustomElevatorManager.Ins.CallElevator(MyElevatorFloor);
		}
	}
}
