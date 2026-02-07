using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CultMaleBehaviour : MonoBehaviour
{
	[SerializeField]
	private float headActionWindowMin = 6f;

	[SerializeField]
	private float headActionWindowMax = 60f;

	private bool headActionActive;

	private float headActionTimeStamp;

	private float headActionTimeWindow;

	private Animator myAC;

	private CultSpawner mySpawner;

	private void Awake()
	{
		myAC = GetComponent<Animator>();
		mySpawner = GetComponent<CultSpawner>();
	}

	private void Update()
	{
		if (headActionActive && Time.time - headActionTimeStamp >= headActionTimeWindow)
		{
			headActionActive = false;
			performHeadAction();
		}
	}

	public void TriggerAnim(string SetTrigger)
	{
		myAC.SetTrigger(SetTrigger);
	}

	public void WasSpawned()
	{
		pickIdleAnim();
		generateHeadAction();
	}

	public void WasDeSpawned()
	{
		TriggerAnim("exitIdle");
		headActionActive = false;
	}

	public void StageDeskJump()
	{
		TriggerAnim("deskJumpIdle");
		mySpawner.Place(new Vector3(3.103f, 39.585f, -2.092f), new Vector3(0f, 180f, 0f));
	}

	public void StageEndJump()
	{
		headActionActive = false;
		TriggerAnim("stageEndJump");
		mySpawner.Place(new Vector3(24.448f, 0f, -6.319f), new Vector3(0f, 90f, 0f));
	}

	public void TriggerDeskJump()
	{
		TriggerAnim("deskJump");
	}

	public void BodyHit()
	{
		MainCameraHook.Ins.AddBodyHit();
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.BodyHit);
	}

	public void HeadHit()
	{
		MainCameraHook.Ins.AddHeadHit(2f);
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.HeadHit);
	}

	public void FloorHit()
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.FloorHit);
	}

	public void TriggerLoadEnding()
	{
		UIManager.TriggerLoadEnding();
	}

	private void generateHeadAction()
	{
		headActionTimeWindow = Random.Range(headActionWindowMin, headActionWindowMax);
		headActionTimeStamp = Time.time;
		headActionActive = true;
	}

	private void performHeadAction()
	{
		switch (Random.Range(0, 2))
		{
		case 1:
			TriggerAnim("headTilt");
			break;
		case 0:
			TriggerAnim("neckCrack");
			break;
		}
		generateHeadAction();
	}

	private void pickIdleAnim()
	{
		switch (Random.Range(0, 4))
		{
		case 0:
			TriggerAnim("idle1");
			break;
		case 1:
			TriggerAnim("idle2");
			break;
		case 2:
			TriggerAnim("idle3");
			break;
		case 3:
			TriggerAnim("idle4");
			break;
		}
	}
}
