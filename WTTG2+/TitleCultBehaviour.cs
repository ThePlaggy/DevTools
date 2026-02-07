using UnityEngine;

public class TitleCultBehaviour : MonoBehaviour
{
	public static TitleCultBehaviour Ins;

	[SerializeField]
	private float fireMin = 10f;

	[SerializeField]
	private float fireMax = 20f;

	private bool fireActive;

	private float fireTimeStamp;

	private float fireWindow;

	private Animator myAC;

	private void Awake()
	{
		myAC = GetComponent<Animator>();
		Ins = this;
		TitleManager.Ins.TitlePresented.Event += generateFireTime;
	}

	private void Update()
	{
		if (fireActive && Time.time - fireTimeStamp >= fireWindow)
		{
			fireActive = false;
			doHeadTilt();
		}
	}

	private void OnDestroy()
	{
		TitleManager.Ins.TitlePresented.Event -= generateFireTime;
	}

	public void TriggerLoopIdle()
	{
		myAC.SetTrigger("triggerLoopIdle");
	}

	private void generateFireTime()
	{
		fireWindow = Random.Range(fireMin, fireMin);
		fireTimeStamp = Time.time;
		fireActive = true;
	}

	private void doHeadTilt()
	{
		int num = Random.Range(1, 11);
		if (num <= 5)
		{
			myAC.SetTrigger("headTiltTrigger");
		}
		else
		{
			myAC.SetTrigger("headTiltTrigger2");
		}
		generateFireTime();
	}
}
