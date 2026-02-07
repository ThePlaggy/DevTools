using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CamWalkOut : MonoBehaviour
{
	public static CamWalkOut Ins;

	[SerializeField]
	private Transform cameraBone;

	[SerializeField]
	private AudioHubObject footHub;

	[SerializeField]
	private AudioFileDefinition[] footStepSFXs = new AudioFileDefinition[0];

	private Animator myAC;

	private void Awake()
	{
		Ins = this;
		myAC = GetComponent<Animator>();
	}

	public void WalkOut()
	{
		EndingCameraHook.Ins.transform.SetParent(cameraBone);
		myAC.SetTrigger("walkOut");
	}

	public void TriggerFootStep()
	{
		int num = Random.Range(1, footStepSFXs.Length);
		AudioFileDefinition audioFileDefinition = footStepSFXs[num];
		footHub.PlaySound(footStepSFXs[num]);
		footStepSFXs[num] = footStepSFXs[0];
		footStepSFXs[0] = audioFileDefinition;
	}
}
