using DG.Tweening;
using UnityEngine;

public class BreatherFloor8Trigger : MonoBehaviour
{
	[SerializeField]
	private GameObject BreatherJumpscareObject;

	[SerializeField]
	private AudioHubObject BreatherAHO;

	private const float offsetfix = 5f;

	public static BreatherFloor8Trigger me;

	public void ActivateJumpscare()
	{
		BreatherJumpscareObject.SetActive(value: true);
		BreatherJumpscareObject.transform.parent.DOMoveX(BreatherJumpscareObject.transform.parent.position.x + 5f, 3f);
	}

	private void Awake()
	{
		me = this;
	}

	private void OnDestroy()
	{
		me = null;
	}

	public static void SpawnBreatherFloor8()
	{
		GameObject gameObject = Object.Instantiate(CustomObjectLookUp.Floor8Breather);
		gameObject.transform.position = new Vector3(24.8076f, 40.7143f, -6.1981f);
		gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 270f, 0f));
		gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		EnemyManager.PoliceManager.StageBreatherFloor8();
	}
}
