using UnityEngine;

[RequireComponent(typeof(peepHoleController))]
public class HitmanPeepJumper : MonoBehaviour
{
	public static HitmanPeepJumper Ins;

	private HitmanPeepJump hitmanPeepJump = new HitmanPeepJump();

	private peepHoleController myPeepHoleController;

	private void Awake()
	{
		Ins = this;
		myPeepHoleController = GetComponent<peepHoleController>();
	}

	public void AddPeepJump()
	{
		myPeepHoleController.TakingOverEvents.Event += hitmanPeepJump.Stage;
		myPeepHoleController.TookOverEvents.Event += hitmanPeepJump.Execute;
		myPeepHoleController.TookOverEvents.Event += peepJump;
	}

	private void peepJump()
	{
		myPeepHoleController.TakingOverEvents.Event -= hitmanPeepJump.Stage;
		myPeepHoleController.TookOverEvents.Event -= hitmanPeepJump.Execute;
		myPeepHoleController.TookOverEvents.Event -= peepJump;
		myPeepHoleController.SetMasterLock(setLock: true);
	}
}
