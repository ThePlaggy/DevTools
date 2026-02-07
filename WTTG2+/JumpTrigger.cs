using UnityEngine;

public class JumpTrigger : MonoBehaviour
{
	public delegate void JumpActions();

	private bool activated;

	public event JumpActions JumpAction;

	public void Activate()
	{
		activated = true;
	}

	public void Trigger()
	{
		if (activated && this.JumpAction != null)
		{
			this.JumpAction();
		}
	}
}
