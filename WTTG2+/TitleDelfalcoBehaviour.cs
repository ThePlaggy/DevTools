using UnityEngine;

public class TitleDelfalcoBehaviour : MonoBehaviour
{
	private float AnimMaxRange = 30f;

	private float AnimMinRange = 20f;

	private Animator myAC;

	private void Awake()
	{
		base.gameObject.name = "TitleDelfalco";
		base.transform.position = new Vector3(0f, 0.12f, 0.8f);
		base.transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
		myAC = GetComponentInChildren<Animator>();
	}

	private void Start()
	{
		TimeSlinger.FireTimer(TriggerAnim, 9f);
	}

	private void TriggerAnim()
	{
		int num = Random.Range(6, 12);
		myAC.SetBool("LookAround", value: true);
		TimeSlinger.FireTimer(delegate
		{
			myAC.SetBool("LookAround", value: false);
		}, num, "naskoSucks");
		TimeSlinger.FireTimer(TriggerAnim, Random.Range(AnimMinRange, AnimMaxRange));
	}
}
