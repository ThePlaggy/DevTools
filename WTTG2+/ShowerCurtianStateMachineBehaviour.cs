using UnityEngine;

public class ShowerCurtianStateMachineBehaviour : StateMachineBehaviour
{
	[SerializeField]
	private SHOWER_CURTIAN_STATES myState;

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		ShowerCurtianTrigger.Ins.AnimationCompleted(myState);
	}
}
