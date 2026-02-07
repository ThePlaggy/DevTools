using DG.Tweening;
using UnityEngine;

public class BreatherNightNightTrigger : MonoBehaviour
{
	public static BreatherNightNightTrigger me;

	[SerializeField]
	private GameObject BreatherJumpscareObject;

	[SerializeField]
	private AudioHubObject BreatherAHO;

	private void Awake()
	{
		me = this;
	}

	private void OnDestroy()
	{
		me = null;
	}

	public void ActivateJumpscare()
	{
		BreatherJumpscareObject.SetActive(value: true);
	}

	public void PlayBreatherSound(AudioFileDefinition audio)
	{
		BreatherAHO.PlaySound(audio);
	}

	public static void SpawnBreatherNightNight()
	{
		GameObject gameObject = Object.Instantiate(CustomObjectLookUp.NightNightBreather);
		gameObject.transform.position = new Vector3(1.55f, 40.5f, -3.775f);
		gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
	}

	public void MoveBreatherDown()
	{
		BreatherJumpscareObject.transform.parent.DOMoveY(40.19f, 1.5f);
	}
}
