using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CultFemaleEnding : MonoBehaviour
{
	public static CultFemaleEnding Ins;

	[SerializeField]
	private AudioHubObject footHub;

	[SerializeField]
	private AudioFileDefinition[] footStepSFXs = new AudioFileDefinition[0];

	[SerializeField]
	private SkinnedMeshRenderer[] rends = new SkinnedMeshRenderer[0];

	private float aniTimeStamp;

	private float aniWindow;

	private bool inIdle2;

	private bool lockOut;

	private Animator myAC;

	private void Awake()
	{
		Ins = this;
		myAC = GetComponent<Animator>();
		inIdle2 = false;
		aniTimeStamp = Time.time;
		aniWindow = Random.Range(15f, 45f);
	}

	private void Update()
	{
		if (lockOut || !(Time.time - aniTimeStamp >= aniWindow))
		{
			return;
		}
		aniTimeStamp = Time.time;
		aniWindow = Random.Range(15f, 45f);
		if (!inIdle2)
		{
			int num = Random.Range(0, 10);
			if (num < 2)
			{
				myAC.SetTrigger("fidget1");
				return;
			}
			if (num >= 2 && num < 5)
			{
				myAC.SetTrigger("fidget2");
				return;
			}
			if (num >= 5 && num < 8)
			{
				myAC.SetTrigger("fidget3");
				return;
			}
			myAC.SetTrigger("idle2");
			inIdle2 = true;
		}
		else
		{
			myAC.SetTrigger("exitIdle2");
			inIdle2 = false;
		}
	}

	public void WalkBehindPlayer()
	{
		lockOut = true;
		if (inIdle2)
		{
			myAC.SetTrigger("exitIdle2");
			GameManager.TimeSlinger.FireTimer(1f, delegate
			{
				myAC.SetTrigger("walkBehindPlayer");
			});
		}
		else
		{
			myAC.SetTrigger("walkBehindPlayer");
		}
	}

	public void TriggerFootSound()
	{
		int num = Random.Range(1, footStepSFXs.Length);
		AudioFileDefinition audioFileDefinition = footStepSFXs[num];
		footHub.PlaySound(footStepSFXs[num]);
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
