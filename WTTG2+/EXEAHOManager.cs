using DG.Tweening;
using UnityEngine;

public class EXEAHOManager : MonoBehaviour
{
	public static EXEAHOManager Ins;

	private static readonly Vector3 stairwayVector = new Vector3(24.368f, 40.8183f, -6.309f);

	private static readonly Vector3 elevatorVector = new Vector3(-26.411f, 40.8183f, -6.309f);

	public AudioHubObject myAho;

	private GameObject theAho;

	private void Awake()
	{
		Ins = this;
		theAho = new GameObject();
		theAho.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		myAho = theAho.AddComponent<AudioHubObject>();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		Debug.Log("[EXEAHO] Killed");
		Destroy();
	}

	private void Destroy()
	{
		Ins = null;
		Object.Destroy(theAho);
	}

	public void EXEPlaySound(AudioFileDefinition AFD)
	{
		myAho.PlaySound(AFD);
	}

	public void MoveMeToStairwayDoor(float duration = 0.5f)
	{
		theAho.transform.DOMove(stairwayVector, duration);
	}

	public void MoveMeToElevator(float duration = 0.5f)
	{
		theAho.transform.DOMove(elevatorVector, duration);
	}

	private void DePopEXESound()
	{
	}
}
