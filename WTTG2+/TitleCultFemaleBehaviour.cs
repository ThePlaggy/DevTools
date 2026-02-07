using UnityEngine;

public class TitleCultFemaleBehaviour : MonoBehaviour
{
	[SerializeField]
	private SkinnedMeshRenderer mySkinMesh;

	[SerializeField]
	private float fireMin = 10f;

	[SerializeField]
	private float fireMax = 30f;

	private bool aniTriggerActive;

	private float aniTriggerTimeStamp;

	private bool headTiltActive;

	private float headTiltTimeStamp;

	private float headTiltWindow;

	private Animator myAC;

	private bool walkUpActive;

	private float walkUpTimeStamp;

	private void Awake()
	{
		myAC = GetComponent<Animator>();
		mySkinMesh.enabled = false;
		TitleManager.Ins.TitlePresent.Event += triggerWalkUp;
	}

	private void Update()
	{
		if (aniTriggerActive && Time.time - aniTriggerTimeStamp >= 3f)
		{
			aniTriggerActive = false;
			myAC.SetTrigger("triggerWalkUp");
			walkUpTimeStamp = Time.time;
			walkUpActive = true;
		}
		if (walkUpActive && Time.time - walkUpTimeStamp >= 1f)
		{
			walkUpActive = false;
			mySkinMesh.enabled = true;
			generateHeadTiltFire();
		}
		if (headTiltActive && Time.time - headTiltTimeStamp >= headTiltWindow)
		{
			headTiltActive = false;
			myAC.SetTrigger("triggerHeadTilt");
			generateHeadTiltFire();
		}
	}

	private void OnDestroy()
	{
		TitleManager.Ins.TitlePresent.Event -= triggerWalkUp;
	}

	private void triggerWalkUp()
	{
		aniTriggerTimeStamp = Time.time;
		aniTriggerActive = true;
	}

	private void generateHeadTiltFire()
	{
		headTiltWindow = Random.Range(fireMin, fireMax);
		headTiltTimeStamp = Time.time;
		headTiltActive = true;
	}
}
