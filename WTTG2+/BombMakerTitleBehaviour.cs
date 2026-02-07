using UnityEngine;

public class BombMakerTitleBehaviour : MonoBehaviour
{
	private float AnimMaxRange = 30f;

	private float AnimMinRange = 20f;

	private string[] Anims;

	private int CurrentAnim;

	private Animator myAC;

	private MeshRenderer PistolMesh;

	public BombMakerTitleBehaviour()
	{
		Anims = new string[3] { "Idle2", "Idle1", "Idle3" };
	}

	private void Awake()
	{
		base.gameObject.name = "TitleBombMaker";
		base.transform.position = new Vector3(0f, 0.2f, 0.95f);
		base.transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
		GameObject.Find("BombMakerHeadLight").GetComponent<Light>().intensity = 0.5f;
		PistolMesh = GameObject.Find("BM_Pistol").GetComponent<MeshRenderer>();
		myAC = GetComponent<Animator>();
		myAC.runtimeAnimatorController = CustomObjectLookUp.BM_TitleAC;
	}

	private void Start()
	{
		TimeSlinger.FireTimer(TriggerAnim, 15.5f);
	}

	public void EnablePistolMesh()
	{
		PistolMesh.enabled = true;
	}

	public void DisablePistolMesh()
	{
		PistolMesh.enabled = false;
	}

	private void TriggerAnim()
	{
		myAC.SetTrigger(Anims[CurrentAnim]);
		CurrentAnim++;
		if (CurrentAnim == Anims.Length)
		{
			CurrentAnim = 0;
		}
		TimeSlinger.FireTimer(TriggerAnim, Random.Range(AnimMinRange, AnimMaxRange));
	}
}
