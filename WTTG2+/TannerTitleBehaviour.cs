using UnityEngine;

public class TannerTitleBehaviour : MonoBehaviour
{
	private float AnimMaxRange = 30f;

	private float AnimMinRange = 20f;

	private string[] Anims = new string[4] { "Idle1", "Idle3", "Idle4", "Idle5" };

	private int CurrentAnim;

	private Animator myAC;

	private Animator SyringeAC;

	private void Awake()
	{
		base.gameObject.name = "TitleTanner";
		base.transform.position = new Vector3(0f, 0.12f, 0.8f);
		GameObject.Find("TannerHeadLight").GetComponent<Light>().intensity = 0.6f;
		GameObject gameObject = Object.Instantiate(CustomObjectLookUp.TheSyringe, base.transform, worldPositionStays: true);
		gameObject.transform.position = new Vector3(0.0065f, 0.2f, 0.813f);
		SyringeAC = gameObject.GetComponent<Animator>();
		myAC = GetComponent<Animator>();
		SyringeAC.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		myAC.runtimeAnimatorController = CustomObjectLookUp.TannerTitleAC;
		SyringeAC.runtimeAnimatorController = CustomObjectLookUp.SyringeTitleAC;
	}

	private void Start()
	{
		TimeSlinger.FireTimer(TriggerAnim, 9f);
	}

	public void EnableSyringeMesh()
	{
		GameObject.Find("SK_Syringe_01").GetComponent<SkinnedMeshRenderer>().enabled = true;
	}

	public void DisableSyringeMesh()
	{
		GameObject.Find("SK_Syringe_01").GetComponent<SkinnedMeshRenderer>().enabled = false;
	}

	private void TriggerAnim()
	{
		if (CurrentAnim == 0)
		{
			SyringeAC.SetTrigger("Idle1");
		}
		myAC.SetTrigger(Anims[CurrentAnim]);
		CurrentAnim++;
		if (CurrentAnim == Anims.Length)
		{
			CurrentAnim = 0;
		}
		TimeSlinger.FireTimer(TriggerAnim, Random.Range(AnimMinRange, AnimMaxRange));
	}
}
