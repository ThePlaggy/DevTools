using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CultMaleEnding : MonoBehaviour
{
	public static CultMaleEnding Ins;

	[SerializeField]
	private AudioHubObject footHub;

	[SerializeField]
	private AudioFileDefinition[] footStepSFXs = new AudioFileDefinition[0];

	[SerializeField]
	private SkinnedMeshRenderer[] rends = new SkinnedMeshRenderer[0];

	private float aniTimeStamp;

	private bool lockOut;

	private Animator myAC;

	private void Awake()
	{
		Ins = this;
		myAC = GetComponent<Animator>();
	}

	private void Start()
	{
		aniTimeStamp = Time.time;
	}

	private void Update()
	{
		if (lockOut || !(Time.time - aniTimeStamp >= 30f))
		{
			return;
		}
		int num = Random.Range(0, 10);
		aniTimeStamp = Time.time;
		if (num < 3)
		{
			int num2 = Random.Range(0, 10);
			if (num2 < 5)
			{
				myAC.SetTrigger("triggerHeadTilt");
			}
			else
			{
				myAC.SetTrigger("triggerNeckCrack");
			}
		}
	}

	public void WalkBehindPlayer()
	{
		lockOut = true;
		myAC.SetTrigger("triggerWalkBehindPlayer");
	}

	public void TriggerFootSound()
	{
		int num = Random.Range(1, footStepSFXs.Length);
		AudioFileDefinition audioFileDefinition = footStepSFXs[num];
		footHub.PlaySoundWithWildPitch(footStepSFXs[num], 1.1f, 1.2f);
		footStepSFXs[num] = footStepSFXs[0];
		footStepSFXs[0] = audioFileDefinition;
	}

	public void DeSpawn()
	{
		lockOut = true;
		for (int i = 0; i < rends.Length; i++)
		{
			rends[i].enabled = false;
		}
	}
}
