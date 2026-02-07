using UnityEngine;

public class EXETitleBehaviour : MonoBehaviour
{
	private float AnimMaxRange = 25f;

	private float AnimMinRange = 15f;

	private string[] Anims = new string[3] { "nervous", "scare", "laughing" };

	private int CurrentAnim;

	private Animator myAC;

	private Animator SyringeAC;

	private void Awake()
	{
		base.gameObject.name = "TitleEXE";
		base.transform.position = new Vector3(0.01f, 0.21f, 1.2f);
		base.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
		myAC = GetComponent<Animator>();
	}

	private void Start()
	{
		TimeSlinger.FireTimer(TriggerAnim, 9f);
	}

	private void TriggerAnim()
	{
		int currentAnim = CurrentAnim;
		myAC.SetBool(Anims[currentAnim], value: true);
		TimeSlinger.FireTimer(delegate
		{
			myAC.SetBool(Anims[currentAnim], value: false);
		}, 1f);
		CurrentAnim++;
		if (CurrentAnim == Anims.Length)
		{
			CurrentAnim = 0;
		}
		TimeSlinger.FireTimer(TriggerAnim, Random.Range(AnimMinRange, AnimMaxRange));
	}
}
