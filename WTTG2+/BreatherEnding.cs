using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BreatherEnding : MonoBehaviour
{
	private float aniTimeStamp;

	private Animator myAC;

	private void Awake()
	{
		myAC = GetComponent<Animator>();
	}

	private void Start()
	{
		aniTimeStamp = Time.time;
	}

	private void Update()
	{
		if (Time.time - aniTimeStamp >= 20f)
		{
			int num = Random.Range(0, 10);
			aniTimeStamp = Time.time;
			if (num < 4)
			{
				myAC.SetTrigger("triggerFidget");
			}
		}
	}
}
